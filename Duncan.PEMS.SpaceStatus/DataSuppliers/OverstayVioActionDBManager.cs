using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System.Web;

using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.DataMappers;
using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{

    public class OverstayVioActionDBManager : IDisposable
    {
        #region Private members
        private string _FolderForDatabase = string.Empty;
        private const string _DefaultDbFilenameWithoutPath = "OverstayVioActions.s3db";

        private object _SQLiteFilesMgr_Lock = new object();
        private List<SqLiteDAO> _SQLiteFiles = new List<SqLiteDAO>();
        private volatile bool _IsInitialized = false;
        private SqLiteDAO _DAO = null;

        private DatabaseMetadata_OverstayVioActions _Metadata_OverstayVios = new DatabaseMetadata_OverstayVioActions();
        #endregion

        #region Public members

        #endregion

        private static readonly OverstayVioActionDBManager _SingletonInstance = new OverstayVioActionDBManager();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static OverstayVioActionDBManager()
        {
        }

        private OverstayVioActionDBManager()
        {
        }

        public static OverstayVioActionDBManager Instance
        {
            get { return _SingletonInstance; }
        }

        public void Dispose()
        {
            // We need to end any pending transactions in the SqLite data access objects
            lock (_SQLiteFilesMgr_Lock)
            {
                if (_SQLiteFiles != null)
                {
                    for (int idx = _SQLiteFiles.Count - 1; idx >= 0; idx--)
                    {
                        SqLiteDAO nextDBLayer = _SQLiteFiles[idx];
                        lock (nextDBLayer.ClientCountWantingTransactionToStayOpen_Lock)
                        {
                            nextDBLayer.ClientCountWantingTransactionToStayOpen = 0;
                            nextDBLayer.CommitTransactionSession_ForcedCommit();
                            try
                            {
                                LogMessage(System.Diagnostics.TraceLevel.Verbose, "SqLite transactions commited and connection closed for: " + nextDBLayer.SqLiteDbPathAndFilename, 
                                    MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, Thread.CurrentThread.ManagedThreadId);
                            }
                            catch{}
                        }
                    }
                    _SQLiteFiles.Clear();
                }

                _DAO = null;
            }
        }

        public void Initialize(string dbFolder)
        {
            Initialize(dbFolder, _DefaultDbFilenameWithoutPath);
        }

        public void Initialize(string dbFolder, string dbFilenameWithoutPath)
        {
            string methodNameForLogging = MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name;
            using (StopWatchProfiler profiler = new StopWatchProfiler("Init Enforcement Actions DB Access (Thread Task)", Logging.LogLevel.Debug, methodNameForLogging, System.Threading.Thread.CurrentThread.ManagedThreadId))
            {

                if (_IsInitialized == true)
                {
                    throw new Exception("OverstayVioActionDBManager singleton can only be initialized once");
                }

                _IsInitialized = true;
                _FolderForDatabase = dbFolder;

                // We're supposed to recieve a filename without path information. But we'll go ahead and fixup if necessary
                if ((dbFilenameWithoutPath.Contains("\\")) || (dbFilenameWithoutPath.Contains("/")))
                {
                    dbFilenameWithoutPath = Path.GetFileName(dbFilenameWithoutPath);
                }
                string fullDBLogPathAndFilename = Path.Combine(_FolderForDatabase, dbFilenameWithoutPath);

                // We need to be thread-safe in this operation
                lock (_SQLiteFilesMgr_Lock)
                {
                    foreach (SqLiteDAO nextDBLayer in _SQLiteFiles)
                    {
                        // If we found an item with the same underlying filename, return it
                        if (string.Compare(nextDBLayer.SqLiteDbPathAndFilename, fullDBLogPathAndFilename, true) == 0)
                        {
                            _DAO = nextDBLayer;
                            break;
                        }
                    }

                    // If the desired layer doesn't exist in our list yet, create a new one and add it to the list
                    if (_DAO == null)
                    {
                        string dbConnectStr =  string.Format("data source={0}", fullDBLogPathAndFilename);
                        _DAO = new SqLiteDAO(dbConnectStr, fullDBLogPathAndFilename, this.LogMessage);
                        _SQLiteFiles.Add(_DAO);
                    }
                }

                // Regardless of if we found an existing DB layer or created a new one, we will perform the DB Metadata check.
                // (This check will be very quick if it has already been validated, and will block until schema validation is finalized by any thread)
                // Note that this should be done outside of the "Lock" becasue we only want to lock out other threads when finding/modifying the 
                // list of DB layers -- not when using them.
                // Before using the database, we need to be sure it has the correct Metadata/Schema information.
                // We will try to get exclusive access to a variable to see if whether or not the Metadata has been checked. 
                // If we get exclusing access and DB Metadata hasn't been validated yet, we will do so now.
                if (!Monitor.TryEnter(_DAO.MetadataHasBeenChecked_Lock))
                {
                    // Somebody else already has this object locked. Let's immediately record this fact
                    Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "SqLite Metadata check is already locked by another thread. (" + _DefaultDbFilenameWithoutPath + ")",
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                    // Now we will do a normal Lock so we are blocked until the Metadata check is finished
                    lock (_DAO.MetadataHasBeenChecked_Lock)
                    {
                        Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "Finished waiting for SqLite Metadata check to complete on different thread. (" + _DefaultDbFilenameWithoutPath + ")",
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }
                }
                else
                {
                    // The Monitor.TryEnter was successful, so we have a lock on the object. We will need to do Monitor.Exit when done
                    try
                    {
                        if (_DAO.MetadataHasBeenChecked == false)
                        {
                            Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "Acquired exclusive access to verify SqLite Metadata. (" + _DefaultDbFilenameWithoutPath + ")",
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                            SqLiteMetadataManager _DBMetadata = new SqLiteMetadataManager(false, LogMessage, _DAO, _Metadata_OverstayVios.Tables);
                            _DBMetadata.CheckAllTables(true);
                            // Set flag so other threads know the Metadata check has already been performed
                            _DAO.MetadataHasBeenChecked = true;

                            // Update Metadata progress log in a thread-safe manner
                            Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "Finished SqLite Metadata verification. (" + _DefaultDbFilenameWithoutPath + ")",
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                        }
                        else
                        {
                            Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "No SqLite Metadata check needed -- it's already been performed. (" + _DefaultDbFilenameWithoutPath + ")",
                                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    finally
                    {
                        // Set flag so other threads know the Metadata check has already been performed
                        _DAO.MetadataHasBeenChecked = true;

                        // Finally we will do the Monitor.Exit which does the unlocking
                        Monitor.Exit(_DAO.MetadataHasBeenChecked_Lock);
                    }
                }

                // Now we need to request DB transaction in the current DB access layer (This should never be done prior to the DB metadata being checked!)
                // Do a thread-safe increment of the counter to indicate transactions should stay active
                if (_DAO != null)
                {
                    lock (_DAO.ClientCountWantingTransactionToStayOpen_Lock)
                    {
                        if (_DAO.ClientCountWantingTransactionToStayOpen == 0)
                        {
                            _DAO.ClientCountWantingTransactionToStayOpen++;

                            // Start a DB session so things are a bit faster in this bulk operation...
                            // (The actual StartTransaction process will depend on how many "listeners" are wanting the transaction to stay active, and current state, etc.)
                            _DAO.StartTransactionSession();
                        }
                    }
                }
            }
        }

        #region Deprecated -- we will just have a single instance of the SqLiteDAO for Overstays
        /*
        public SqLiteDAO GetOrCreateSqLiteDAO()
        {
            return GetOrCreateSqLiteDAO(_DefaultDbFilenameWithoutPath);
        }

        public SqLiteDAO GetOrCreateSqLiteDAO(string dbLogFilenameWithoutPath)
        {
            SqLiteDAO result = null;
            // We're supposed to recieve a filename without path information. But we'll go ahead and fixup if necessary
            if ((dbLogFilenameWithoutPath.Contains("\\")) || (dbLogFilenameWithoutPath.Contains("/")))
            {
                dbLogFilenameWithoutPath = Path.GetFileName(dbLogFilenameWithoutPath);
            }
            string fullDBLogPathAndFilename = Path.Combine(FolderForDatabase, dbLogFilenameWithoutPath);

            // We need to be thread-safe in this operation
            lock (_SQLiteFilesMgr_Lock)
            {
                foreach (SqLiteDAO nextDBLayer in _SQLiteFiles)
                {
                    // If we found an item with the same underlying filename, return it
                    if (string.Compare(nextDBLayer.SqLiteDbPathAndFilename, fullDBLogPathAndFilename, true) == 0)
                    {
                        result = nextDBLayer;
                        break;
                    }
                }

                // If the desired layer doesn't exist in our list yet, create a new one and add it to the list
                if (result == null)
                {
                    string dbConnectStr = string.Format("data source={0}", fullDBLogPathAndFilename);
                    result = new SqLiteDAO(dbConnectStr, fullDBLogPathAndFilename);
                    _SQLiteFiles.Add(result);
                }
            }

            // Regardless of if we found an existing DB layer or created a new one, we will perform the DB Metadata check.
            // (This check will be very quick if it has already been validated, and will block until schema validation is finalized by any thread)
            // Note that this should be done outside of the "Lock" becasue we only want to lock out other threads when finding/modifying the 
            // list of DB layers -- not when using them.
            // Before using the database, we need to be sure it has the correct Metadata/Schema information.
            // We will try to get exclusive access to a variable to see if whether or not the Metadata has been checked. 
            // If we get exclusing access and DB Metadata hasn't been validated yet, we will do so now.
            if (!Monitor.TryEnter(result.MetadataHasBeenChecked_Lock))
            {
                // Somebody else already has this object locked. Let's immediately record this fact
                Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "SqLite Metadata check is already locked by another thread. (" + _DefaultDbFilenameWithoutPath + ")",
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // Now we will do a normal Lock so we are blocked until the Metadata check is finished
                lock (result.MetadataHasBeenChecked_Lock)
                {
                    Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "Finished waiting for SqLite Metadata check to complete on different thread. (" + _DefaultDbFilenameWithoutPath + ")",
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
            }
            else
            {
                // The Monitor.TryEnter was successful, so we have a lock on the object. We will need to do Monitor.Exit when done
                try
                {
                    if (result.MetadataHasBeenChecked == false)
                    {
                        Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "Acquired exclusive access to verify SqLite Metadata. (" + _DefaultDbFilenameWithoutPath + ")",
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                        SqLiteMetadataManager _DBMetadata = new SqLiteMetadataManager(false, LogMessage, result, _Metadata_OverstayVios.Tables);
                        _DBMetadata.CheckAllTables(true);
                        // Set flag so other threads know the Metadata check has already been performed
                        result.MetadataHasBeenChecked = true;

                        // Update Metadata progress log in a thread-safe manner
                        Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "Finished SqLite Metadata verification. (" + _DefaultDbFilenameWithoutPath + ")",
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }
                    else
                    {
                        Logging.AddTextToGenericLog(Logging.LogLevel.Debug, "No SqLite Metadata check needed -- it's already been performed. (" + _DefaultDbFilenameWithoutPath + ")",
                            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }
                }
                finally
                {
                    // Set flag so other threads know the Metadata check has already been performed
                    result.MetadataHasBeenChecked = true;

                    // Finally we will do the Monitor.Exit which does the unlocking
                    Monitor.Exit(result.MetadataHasBeenChecked_Lock);
                }
            }

            // Now we need to request DB transaction in the current DB access layer (This should never be done prior to the DB metadata being checked!)
            // Do a thread-safe increment of the counter to indicate transactions should stay active
            if (result != null)
            {
                lock (result.ClientCountWantingTransactionToStayOpen_Lock)
                {
                    if (result.ClientCountWantingTransactionToStayOpen == 0)
                    {
                        result.ClientCountWantingTransactionToStayOpen++;

                        // Start a DB session so things are a bit faster in this bulk operation...
                        // (The actual StartTransaction process will depend on how many "listeners" are wanting the transaction to stay active, and current state, etc.)
                        result.StartTransactionSession();
                    }
                }
            }

            // Return our final result
            return result;
        }
        */
        #endregion

        public void LogMessage(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID)
        {
            Logging.LogLevel convertedLogLevel = Logging.LogLevel.Debug;
            switch (logLevel)
            {
                case System.Diagnostics.TraceLevel.Error:
                    convertedLogLevel = Logging.LogLevel.Error;
                    break;
                case System.Diagnostics.TraceLevel.Info:
                    convertedLogLevel = Logging.LogLevel.Info;
                    break;
                case System.Diagnostics.TraceLevel.Verbose:
                    convertedLogLevel = Logging.LogLevel.Debug;
                    break;
                case System.Diagnostics.TraceLevel.Warning:
                    convertedLogLevel = Logging.LogLevel.Warning;
                    break;
                case System.Diagnostics.TraceLevel.Off:
                    convertedLogLevel = Logging.LogLevel.DebugTraceOutput;
                    break;
                default:
                    convertedLogLevel = Logging.LogLevel.Debug;
                    break;
            }

            Logging.AddTextToGenericLog(convertedLogLevel, textToLog, classAndMethod, threadID);
        }

        #region Static Public Methods -- DB Inserts and Queries

        static public void InsertOverstayVioAction(int CustomerID, int MeterID, int AreaID, int BayNumber, int RBACUserID,
            DateTime EventTimestamp, string ActionTaken)
        {
            if (OverstayVioActionDBManager.Instance._IsInitialized == false)
            {
                throw new Exception("OverstayVioActionDBManager singleton instance hasn't been initialized for database access");
            }

            // Set the SQL query parameters
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("CustomerID", CustomerID);
            sqlParams.Add("MeterID", MeterID);
            sqlParams.Add("AreaID", AreaID);
            sqlParams.Add("BayNumber", BayNumber);
            sqlParams.Add("RBACUserID", RBACUserID);
            sqlParams.Add("EventTimestamp", EventTimestamp);
            sqlParams.Add("ActionTaken", ActionTaken);

            // Perform DB Insert
            DataSuppliers.SqLiteDAO sqLiteDataLayer = OverstayVioActionDBManager.Instance._DAO;
            Int64 autoIncValue = sqLiteDataLayer.ExecuteInsertAndReturnAutoInc(
                "INSERT INTO [OverstayVioActions]" +
                " (CustomerID, MeterID, AreaID, BayNumber, RBACUserID, EventTimestamp, ActionTaken)" +
                " VALUES (" +
                " ?, ?, ?," +
                " ?, ?, ?," +
                " ?" +
                " )", sqlParams, true);
        }

        static public OverstayVioActionsDTO GetLatestVioActionForSpace(int customerID, int meterID, int areaID, int bayNumber)
        {
            if (OverstayVioActionDBManager.Instance._IsInitialized == false)
            {
                throw new Exception("OverstayVioActionDBManager singleton instance hasn't been initialized for database access");
            }

            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@CustomerID", customerID);
            sqlParams.Add("@MeterID", meterID);
            //sqlParams.Add("AreaID", areaID);
            sqlParams.Add("@BayNumber", bayNumber);

            string query =
                "SELECT UniqueKey, CustomerID, AreaID, MeterID, BayNumber, RBACUserID, EventTimestamp, ActionTaken" +
                " FROM OverstayVioActions" +
                " where CustomerID = @CustomerID" +
                " and MeterID = @MeterID" +
                " and BayNumber = @BayNumber" +
                " order by EventTimestamp desc" +
                " LIMIT 1";

            DataSuppliers.SqLiteDAO sqLiteDataLayer = OverstayVioActionDBManager.Instance._DAO;
            return sqLiteDataLayer.GetSingle<OverstayVioActionsDTO>(query, sqlParams, true);
        }

        static public OverstayVioActionsDTO GetVioActionForSpaceDuringTimeRange(int customerID, int meterID, int areaID, int bayNumber, DateTime startTime, DateTime endTime)
        {
            if (OverstayVioActionDBManager.Instance._IsInitialized == false)
            {
                throw new Exception("OverstayVioActionDBManager singleton instance hasn't been initialized for database access");
            }

            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@CustomerID", customerID);
            sqlParams.Add("@MeterID", meterID);
            //sqlParams.Add("AreaID", areaID);
            sqlParams.Add("@BayNumber", bayNumber);
            sqlParams.Add("@StartTime", startTime);
            sqlParams.Add("@EndTime", endTime);

            string query =
                "SELECT UniqueKey, CustomerID, AreaID, MeterID, BayNumber, RBACUserID, EventTimestamp, ActionTaken" +
                " FROM OverstayVioActions" +
                " where CustomerID = @CustomerID" +
                " and MeterID = @MeterID" +
                " and BayNumber = @BayNumber" +
                " and EventTimestamp >= @StartTime" +
                " and EventTimestamp <= @EndTime" +
                " order by EventTimestamp desc" +
                " LIMIT 1";

            DataSuppliers.SqLiteDAO sqLiteDataLayer = OverstayVioActionDBManager.Instance._DAO;
            return sqLiteDataLayer.GetSingle<OverstayVioActionsDTO>(query, sqlParams, true);
        }

        #endregion

        #region Static Public Methods -- XML Serialization
        static public string SerializeObjectAsXML(object objectToSerialize, string forcedNameSpace)
        {
            XmlSerializer serializer = null;
            if (!string.IsNullOrEmpty(forcedNameSpace))
                serializer = new XmlSerializer(objectToSerialize.GetType(), forcedNameSpace);
            else
                serializer = new XmlSerializer(objectToSerialize.GetType());

            XmlWriterSettings loXMLSettings = new XmlWriterSettings();
            loXMLSettings.Indent = true;
            loXMLSettings.Encoding = UTF8Encoding.UTF8;
            loXMLSettings.OmitXmlDeclaration = true;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlWriter xtWriter = XmlWriter.Create(stream, loXMLSettings);

            if (string.IsNullOrEmpty(forcedNameSpace))
            {
                // Create our own namespaces for the output, which effectively omits all namespaces
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(xtWriter, objectToSerialize, ns);
            }
            else
            {
                serializer.Serialize(xtWriter, objectToSerialize);
            }
            xtWriter.Flush();
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            // Now load the serialized XML into an XMLDocument for some post-processing
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(stream);

            // Let's remove the unwanted "xsd" and "xsi" namespaces from attributes of first child node
            if (xmlDoc.FirstChild.Attributes != null)
            {
                for (int loIdx = xmlDoc.FirstChild.Attributes.Count - 1; loIdx >= 0; loIdx--)
                {
                    XmlAttribute nextAttr = xmlDoc.FirstChild.Attributes[loIdx];
                    if ((nextAttr.Name == "xmlns:xsd") || (nextAttr.Name == "xmlns:xsi"))
                    {
                        xmlDoc.FirstChild.Attributes.Remove(nextAttr);
                    }
                }
            }

            // Capture the outer XML of document as the result 
            string ObjAsString = xmlDoc.OuterXml;

            // Cleanup resources
            xtWriter.Close();
            stream.Close();
            stream.Dispose();

            // Return final result, which is a string representing the serialized XML of object
            return ObjAsString;
        }

        static public object DeSerializeObject(string objectAsString, Type objectType)
        {
            // This is the object we will return (null if fail).
            object objectToReturn = null;
            try
            {
                // Deserialize the object.
                StringReader reader = new StringReader(objectAsString);
                XmlSerializer serializer = new XmlSerializer(objectType);
                objectToReturn = serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception ex)
            {
                // Re-Throw the error
                throw (ex);
            }

            // Return object we created.
            return objectToReturn;
        }

        static public string ConvertDataSetToXML(DataSet dsDataset, bool iIndentXML, bool iIncludeSchema)
        {
            StringBuilder loXMLAsStringBuilder = new StringBuilder();
            XmlWriterSettings loXMLSettings = new XmlWriterSettings();
            loXMLSettings.Indent = iIndentXML;
            if (iIndentXML)
            {
                loXMLSettings.NewLineHandling = NewLineHandling.Entitize;
            }

            loXMLSettings.Encoding = Encoding.UTF8;
            loXMLSettings.OmitXmlDeclaration = true;

            XmlWriter loXMLWriter = XmlWriter.Create(loXMLAsStringBuilder, loXMLSettings);
            if (iIncludeSchema)
                dsDataset.WriteXml(loXMLWriter, XmlWriteMode.WriteSchema);
            else
                dsDataset.WriteXml(loXMLWriter, XmlWriteMode.IgnoreSchema);
            loXMLWriter.Flush();
            loXMLWriter.Close();

            string result = loXMLAsStringBuilder.ToString();
            return result;
        }

        #endregion

        #region Static Public Utility Functions
        static public string GetIP4Address(HttpRequest iHttpRequestContext)
        {
            try
            {
                string IP4Address = String.Empty;

                foreach (IPAddress IPA in Dns.GetHostAddresses(iHttpRequestContext.UserHostAddress))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }

                if (IP4Address != String.Empty)
                {
                    return IP4Address;
                }

                foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (IPA.AddressFamily.ToString() == "InterNetwork")
                    {
                        IP4Address = IPA.ToString();
                        break;
                    }
                }

                return IP4Address;
            }
            catch
            {
                return iHttpRequestContext.UserHostAddress;
            }
        }
        #endregion
    }

    #region DatabaseMetadata_OverstayVioActions
    public class DatabaseMetadata_OverstayVioActions
    {
        protected List<TableMetadata> _Tables = new List<TableMetadata>();
        public List<TableMetadata> Tables
        {
            get { return _Tables; }
            set { _Tables = value; }
        }

        /// <summary>
        /// SystemTableMgr constructor.
        /// 
        /// Constructs list of a system tables;
        /// </summary>
        public DatabaseMetadata_OverstayVioActions()
        {
            BuildMetadataObjects();
        }

        /// <summary>
        /// Creates a TableMetadata
        /// </summary>
        /// <param name="iName"></param>
        /// <returns></returns>
        private TableMetadata CreateTableMetadata(String iName)
        {
            TableMetadata table = new TableMetadata();
            table.Name = iName;
            Tables.Add(table);
            return table;
        }

        /// <summary>
        /// Adds definition of tables and indexes we want in our database
        /// </summary>
        private void BuildMetadataObjects()
        {
            TableMetadata table;
            List<String> indexColumnNames = new List<string>();

            // Create a UniqueKey column definition which will be used by all tables as an "AutoIncrement" primary key.
            // Note that we don't flag it as a NOT NULL attribute because the AutoInc column creation for SQLite will do it automatically.
            IntColumnMetadata UniqueKeyColumnMetadata = new IntColumnMetadata("UniqueKey", 19, false);
            UniqueKeyColumnMetadata.IsIdentityColumn = true;

            // ------------------------------------//
            // OverstayVioActions table definition //
            // ------------------------------------//
            table = CreateTableMetadata("OverstayVioActions");
            // Add UniqueKey which will be the "AutoIncrement" field and primary key
            table.AddField(UniqueKeyColumnMetadata);
            // Now add normal columns
            table.AddField(new IntColumnMetadata("CustomerID", 19, false));
            table.AddField(new IntColumnMetadata("MeterID", 19, false));
            table.AddField(new IntColumnMetadata("AreaID", 19, false));
            table.AddField(new IntColumnMetadata("BayNumber", 19, false));
            table.AddField(new IntColumnMetadata("RBACUserID", 19, false));
            table.AddField(new DateTimeColumnMetadata("EventTimestamp", 0, false, false));
            table.AddField(new StringColumnMetadata("ActionTaken", 30, false));

            // Don't need to add defintion for PrimaryKey Index of the AutoInc column because SQLite takes care of it on its own.

            // Add Regular Indexes
            indexColumnNames.Clear();
            indexColumnNames.Add("CustomerID");
            indexColumnNames.Add("MeterID");
            indexColumnNames.Add("BayNumber");
            indexColumnNames.Add("EventTimestamp");
            new IndexMetadata(table, "IDX_" + table.Name + "_1", indexColumnNames.ToArray(), false, false);

            indexColumnNames.Clear();
            indexColumnNames.Add("CustomerID");
            indexColumnNames.Add("MeterID");
            indexColumnNames.Add("BayNumber");
            new IndexMetadata(table, "IDX_" + table.Name + "_2", indexColumnNames.ToArray(), false, false);

            // -------------------------------------------------------------------------
        }
    }
    #endregion

}
