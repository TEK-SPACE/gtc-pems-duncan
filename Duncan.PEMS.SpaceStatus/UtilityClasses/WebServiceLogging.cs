using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;

namespace Duncan.PEMS.SpaceStatus.UtilityClasses
{
    /// <summary>
    /// <para>The Logging class implements a thread-safe singleton instance which controlls a Producer/Consumer queue</para>
    /// <para>for handling logging of information in background threads, with concurrency and performance in mind.</para>
    /// <para>The Producer/Consumer queue is based on info at: http://www.albahari.com/threading/part4.aspx#_Wait_Pulse_Producer_Consumer_Queue </para>
    /// <para> </para>
    /// <para>Some helpful hints about logging information:</para>
    /// <para> </para>
    /// <para>When logging an entry, here is a handy way of obtaining the currently executing method name:</para>
    /// <para>    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name</para>
    /// <para>The method above is more efficient than this alternative:</para>
    /// <para>    System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1, true);</para>
    /// <para>    return this.GetType().Name + "." + sf.GetMethod().Name + "()";</para>
    /// <para> </para>   
    /// <para>Also note that you can get a thread id via: System.Threading.Thread.CurrentThread.ManagedThreadId</para>
    /// </summary>
    public sealed class Logging : IDisposable
    {
        public enum LogLevel { 
            /// <summary>
            /// DebugTraceOutput only writes to the debugger trace listener -- not to log file
            /// </summary>
            DebugTraceOutput, 
            Debug, 
            Info, 
            Message, 
            Warning, 
            Error 
        };

        #region Static members
        static volatile private string _DefaultLogName = "Duncan.PEMS.SpaceStatus.log";
        static volatile private LogLevel _MinimumLogLevelToWrite = LogLevel.DebugTraceOutput;
        static volatile private int _LoggedItemCountSinceCleanup = 0;
        static volatile private int _LogFileRetentionDays = 90; // 90 days is default value we will use for number of days to retain log files
        static volatile private int _LogFileMaxSizeInMB = 20; // 20MB is default value we will used for approximate max size of log files

        // Thread-sync object used when controlling access to underlying log file on disk
        static private object _LogFileLock = new object();
        #endregion

        #region Private members for Producer/Consumer queue
        // Thread-sync object used when controlling access to Producer/Consumer item queue
        private readonly object _PCQueueLocker = new object();
        private Thread[] _PCQueueWorkers;
        private Queue<Action> _itemQ = new Queue<Action>();
        #endregion

        #region Singleton instance construction and destruction
        private static readonly Logging _SingletonInstance = new Logging();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static Logging()
        {
        }

        private Logging()
        {
            // We will only have one consumer thread since we typically we be working with the same underlying file
            _PCQueueWorkers = new Thread[1];

            // Create and start a separate thread for each worker
            (_PCQueueWorkers[0] = new Thread(Consume)).Start();
        }

        public static Logging Instance
        {
            get
            {
                return _SingletonInstance;
            }
        }

        public void Dispose()
        {
            // Signal the consumer thread(s) to shutdown
            Shutdown(false);
        }
        #endregion

        #region Producer/Consumer queue methods
        private void Shutdown(bool waitForWorkers)
        {
            // Enqueue one null item per worker to make each exit.
            foreach (Thread worker in _PCQueueWorkers)
                EnqueueItem(null);

            // Wait for workers to finish
            if (waitForWorkers)
            {
                foreach (Thread worker in _PCQueueWorkers)
                    worker.Join();
            }
        }

        /// <summary>
        /// Adds an item to the queue. Although item is for a parameterless method, we can still represent tasks that call method with parameters
        /// by wrapping the call in an anonymous delegate or lambda expression, such as these examples:
        /// <para> </para>
        /// <para>    Action myFirstTask = delegate</para>
        /// <para>   {</para>
        /// <para>       Console.WriteLine("foo");</para>
        /// <para>   };</para>
        /// <para> </para>
        /// <para>   Action mySecondTask = () => Console.WriteLine("foo");</para>
        /// <para> </para>
        /// </summary>
        /// <param name="item"></param>
        private void EnqueueItem(Action item)
        {
            lock (_PCQueueLocker)
            {
                // Add item to queue, then pulse because we're changing a blocking condition
                _itemQ.Enqueue(item);
                Monitor.Pulse(_PCQueueLocker);
            }
        }

        private void Consume()
        {
            // Keep consuming until told otherwise
            while (true)
            {
                Action item;
                lock (_PCQueueLocker)
                {
                    // Yes, this looks funky. However you must realize that Monitor.Wait actually releases the critical section, and then reacquires it 
                    // when the blocking condition changes! This means the time spent blocking the critical section is minimized.
                    while (_itemQ.Count == 0) Monitor.Wait(_PCQueueLocker);
                    item = _itemQ.Dequeue();
                }

                // A null item is our signal to exit
                if (item == null)
                    return;

                // Execute queued item
                item();
            }
        }
        #endregion

        #region Private method called from consumer thread to write to underlying log file
        private void PersistQueuedItem(Logging.LogLevel logLevel, string logNameWithoutPathOrTimestampPrefix, string textToLog, string method, int threadID = -1)
        {
            /*Debug.WriteLine("Executing PersistQueuedItem via thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());*/

            // Just exit if this is at an undesired logging level
            if (Convert.ToInt32(logLevel) < Convert.ToInt32(Logging._MinimumLogLevelToWrite))
                return;

            // Create a string builder that represents all info we will log
            StringBuilder finalTextToLog = new StringBuilder();
            finalTextToLog.Append(DateTime.UtcNow.ToString("yyyyMMdd_THHmmssZ") + " | " + logLevel.ToString() + " | " + textToLog);
            if (!string.IsNullOrEmpty(method))
                finalTextToLog.Append(" | " + "Method = " + method);
            if (threadID != -1)
                finalTextToLog.Append(" | " + "Thread = " + threadID.ToString());

            // Build the full base filename, including path. The final filename will be altered to include a timestamp prefix in the name
            // Note: Just for info, an alternative way to build the full path is: System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/Logs/" + logNameWithoutPathOrTimestampPrefix);
            string logBaseFileName = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\Logs\" + logNameWithoutPathOrTimestampPrefix);

            // Extract the folder from the complete file path
            string logFileFolder = Path.GetDirectoryName(logBaseFileName);

            // Create folder if it doesn't exist already
            try
            {
                if (!Directory.Exists(logFileFolder))
                    Directory.CreateDirectory(logFileFolder);
            }
            catch { }

            // Check to see if we should cleanup any old log files
            // To reduce strain, let's only do log file maintenance after every 25 activities
            if ((Logging._LoggedItemCountSinceCleanup == 0) || ((Logging._LoggedItemCountSinceCleanup % 25) == 0))
            {
                // Do log file cleanup in a separate thread
                CleanupOldLogFiles(logBaseFileName, Logging._LogFileRetentionDays);
            }

            // Increment the logged item count, but avoid maintaining a large number
            Logging._LoggedItemCountSinceCleanup++;
            if (Logging._LoggedItemCountSinceCleanup > 1024)
                Logging._LoggedItemCountSinceCleanup = 0;

            try
            {
                // Determine the filename we will use for logging (We will roll-over to new filename when date changes, or when current log file reaches max size threshold)
                string fullLogFileName = "";
                DateTime existingLogTimeStamp = DateTime.MinValue;
                try
                {
                    // Find list of today's files for current desired log filename suffix.
                    // Loop through them to find the latest
                    string[] logFiles = Directory.GetFiles(logFileFolder, DateTime.UtcNow.ToString("yyyyMMdd") + "*" + logNameWithoutPathOrTimestampPrefix, SearchOption.TopDirectoryOnly);
                    foreach (string nextLogFile in logFiles)
                    {
                        try
                        {
                            string justFilename = Path.GetFileName(nextLogFile);
                            DateTime logFileTimeStamp = DateTime.SpecifyKind(System.Xml.XmlConvert.ToDateTime(justFilename.Substring(0, 17), "yyyyMMdd_THHmmssZ"), DateTimeKind.Utc);
                            if (logFileTimeStamp > existingLogTimeStamp)
                            {
                                fullLogFileName = nextLogFile;
                                existingLogTimeStamp = logFileTimeStamp;
                            }
                        }
                        catch
                        {
                        }
                    }

                    // If we found the latest existing file, we will use it unless it has already reached maximum size allowed
                    if (fullLogFileName != "")
                    {
                        FileInfo LastLogFileInfo = new FileInfo(fullLogFileName);
                        if (LastLogFileInfo.Length >= (Logging._LogFileMaxSizeInMB * 1024 * 1024))
                            fullLogFileName = "";
                    }
                }
                catch
                {
                }

                // If we don't have a useable name yet, start a new one (and keep track of it)
                if (fullLogFileName == "")
                {
                    fullLogFileName = Path.Combine(logFileFolder, DateTime.UtcNow.ToString("yyyyMMdd_THHmmssZ") + " " + logNameWithoutPathOrTimestampPrefix);
                }

                lock (Logging._LogFileLock)  // wait your turn - avoid concurrent access errors
                {
                    using (FileStream logFileStream = new FileStream(fullLogFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (StreamWriter logWriter = new StreamWriter(logFileStream))
                        {
                            // We need to make sure we are writing at the end of the file (to append text)
                            logFileStream.Seek(0, SeekOrigin.End);

                            // Append the final text to end of file
                            logWriter.WriteLine(finalTextToLog.ToString());

                            // Close file and cleanup
                            if (logWriter.BaseStream.CanWrite == true)
                                logWriter.Flush();
                        }
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Public methods for logging information
        /// <summary>
        /// Records text to default log file. The logged item will be categorized as "Info".
        /// </summary>
        /// <param name="textToLog"></param>
        static public void AddTextToGenericLog(string textToLog)
        {
            Logging.AddTextToGenericLog(LogLevel.Info, textToLog, string.Empty, -1);
        }

        /// <summary>
        /// Records text to default log file, categorized at the passed logLevel.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="textToLog"></param>
        static public void AddTextToGenericLog(Logging.LogLevel logLevel, string textToLog)
        {
            Logging.AddTextToGenericLog(logLevel, textToLog, string.Empty, -1);
        }

        /// <summary>
        /// <para>Records text to default log file, categorized at the passed logLevel.</para>
        /// <para>Method will also be recorded if its not null or empty.</para>
        /// <para>Thread ID will also be recorded if its not -1.</para>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="textToLog"></param>
        /// <param name="method">
        /// <para>Optional information about where in code the logged event occurred.</para>
        /// <para>A common parameter value to pass is:</para>
        /// <para>  System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name</para>
        /// </param>
        /// <param name="threadID">
        /// <para>Optional information to indicate which thread triggered the logging event. Thread ID is not logged if passed value is -1.</para>
        /// </param>
        static public void AddTextToGenericLog(Logging.LogLevel logLevel, string textToLog, string method, int threadID = -1)
        {
            Logging.AddTextToSpecificLog(logLevel, Logging._DefaultLogName, textToLog, method, threadID);
        }

        /// <summary>
        /// <para>Records text to specific log file, categorized at the passed logLevel.</para>
        /// <para>Method will also be recorded if its not null or empty.</para>
        /// <para>Thread ID will also be recorded if its not -1.</para>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="textToLog"></param>
        /// <param name="method">
        /// <para>Optional information about where in code the logged event occurred.</para>
        /// <para>A common parameter value to pass is:</para>
        /// <para>  System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name</para>
        /// </param>
        /// <param name="threadID">
        /// <para>Optional information to indicate which thread triggered the logging event. Thread ID is not logged if passed value is -1.</para>
        /// </param>
        static public void AddTextToSpecificLog(Logging.LogLevel logLevel, string logNameWithoutPathOrTimestampPrefix, string textToLog, string method, int threadID = -1)
        {
            /*Debug.WriteLine("Adding text to log queue via thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());*/

            // If the log level is the special DebugTraceOutput option, then simply send to Debug Diagnostics trace listner,
            // otherwise we will add to a queue to be written to log file
            if (logLevel == LogLevel.DebugTraceOutput)
            {
                System.Diagnostics.Debug.WriteLine(textToLog);
                return;
            }

            // Add item to the Producer/Consumer queue and immediately return back to caller.  
            // The Consumer thread will write to the underlying log file in the background after 
            //it receives its signal and acquires a concurrency lock
            Logging.Instance.EnqueueItem(() =>
            {
                Logging.Instance.PersistQueuedItem(logLevel, logNameWithoutPathOrTimestampPrefix, textToLog, method, threadID);
            });
        }
        #endregion

        #region Purge older log files
        static public void CleanupOldLogFiles(string logBaseFilename, int maxAgeOfLogsInDays)
        {
            // Launch a worker thread that will delete old log files in the background
            LogCleanupParams logCleanupParams = new LogCleanupParams(logBaseFilename, maxAgeOfLogsInDays);
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(CleanupOldLogFiles_DoWork);
            backgroundWorker.RunWorkerAsync(logCleanupParams);
        }

        static private void CleanupOldLogFiles_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Get the background worker that called this event.
                BackgroundWorker backgroundWorker = (sender as BackgroundWorker);

                // Get the parameters object from the argument
                LogCleanupParams logCleanupParams = (e.Argument as LogCleanupParams);

                // Extract folder and filename from the complete file path
                string logFileFolder = Path.GetDirectoryName(logCleanupParams.LogFilename);
                string justLogFilename = Path.GetFileName(logCleanupParams.LogFilename);

                // Create folder if it doesn't exist already
                try
                {
                    if (!Directory.Exists(logFileFolder))
                        Directory.CreateDirectory(logFileFolder);
                }
                catch { }

                // We expect the log files to start with the naming convention of "yyyyMMdd_THHmmssZ"
                // If we are able to decode this prefix to a valid date, then delete the log file if
                // the date signifies that it is older than "maxAgeOfLogsInDays" days
                try
                {
                    string[] logFiles = Directory.GetFiles(logFileFolder, "*" + justLogFilename, SearchOption.TopDirectoryOnly);
                    foreach (string nextLogFile in logFiles)
                    {
                        try
                        {
                            string justFilename = Path.GetFileName(nextLogFile);
                            DateTime logFileTimeStamp = DateTime.SpecifyKind(System.Xml.XmlConvert.ToDateTime(justFilename.Substring(0, 17), "yyyyMMdd_THHmmssZ"), DateTimeKind.Utc);
                            if (logFileTimeStamp < DateTime.Today.AddDays(logCleanupParams.MaxAgeOfLogsInDays * (-1)))
                            {
                                File.Delete(nextLogFile);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
            catch { }
        }
        #endregion
    }

    #region LogCleanupParams
    public class LogCleanupParams
    {
        public string LogFilename;
        public int MaxAgeOfLogsInDays;

        public LogCleanupParams(string logFilename, int maxAgeOfLogsInDays)
        {
            LogFilename = logFilename;
            MaxAgeOfLogsInDays = maxAgeOfLogsInDays;
        }
    }
    #endregion

    public class StopWatchProfiler : System.Diagnostics.Stopwatch, IDisposable
    {
        private string _activity = string.Empty;
        private Logging.LogLevel _logLevel = Logging.LogLevel.Debug;
        private string _logMethod = string.Empty;
        private int _threadID = -1;

        public StopWatchProfiler(string activity)
        {
            _activity = activity;

            Start();
        }

        public StopWatchProfiler(string activity, Logging.LogLevel logLevel)
        {
            _activity = activity;
            _logLevel = logLevel;

            Start();
        }

        public StopWatchProfiler(string activity, Logging.LogLevel logLevel, string logMethod, int threadID = -1)
        {
            _activity = activity;
            _logLevel = logLevel;
            _logMethod = logMethod;
            _threadID = threadID;

            Start();
        }

        public void Restart(string newActivity)
        {
            Stop();
            
            // Only log to file if previous activity string is populated
            if (!string.IsNullOrEmpty(_activity))
                Logging.AddTextToGenericLog(_logLevel, string.Format("Profiler: {0}; Elapsed: {1}", _activity, this.Elapsed), _logMethod, _threadID);

            _activity = newActivity;
            Restart();
        }

        public void Dispose()
        {
            Stop();

            // Only log to file if activity string is populated
            if (!string.IsNullOrEmpty(_activity))
                Logging.AddTextToGenericLog(_logLevel, string.Format("Profiler: {0}; Elapsed: {1}", _activity, this.Elapsed), _logMethod, _threadID);
        }
    }
}
