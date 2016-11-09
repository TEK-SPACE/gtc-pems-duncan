using System;
using System.IO;
using System.Reflection;

namespace Duncan.PEMS.Utilities
{
    public class ReflectionUtilities
    {
        /// <summary>
        ///   Get assembly version via reflection
        /// </summary>
        /// <returns>Assembly version</returns>
        protected static string GetApplicationVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        ///   Get assembly build date via reflection
        /// </summary>
        /// <returns>Assembly build date</returns>
        public static string GetBuildDate()
        {
            DateTime buildDate = new FileInfo( Assembly.GetExecutingAssembly().Location ).LastWriteTime;
            return buildDate.ToString();
        }
    }
}