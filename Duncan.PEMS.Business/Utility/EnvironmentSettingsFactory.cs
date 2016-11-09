using System.Collections.Generic;
using System.Linq;

namespace Duncan.PEMS.Business.Utility
{
    /// <summary>
    /// This class is used for accessing (read only) various system environment settings.
    /// 
    /// Generally these methods are used to display current settings of the system.
    /// </summary>
    public class EnvironmentSettingsFactory : BaseFactory
    {

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public EnvironmentSettingsFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }



        /// <summary>
        /// Gets a list of the versions of the PEMS database.  List is in reverse chronological order.
        /// </summary>
        /// <returns>Returns Dictionary list of release history.  Key = version, Value = Description.</returns>
        public  Dictionary<string, string> GetPemsDbRevisions()
        { 
            var dictionary = new Dictionary<string, string>();

            var versions = PemsEntities.Versions.OrderByDescending( m => m.CHANGE_DATE );
            foreach (var version in versions)
            {
                if ( dictionary.ContainsKey( version.DBVersionNumber ) )
                    dictionary[version.DBVersionNumber] += "' " + version.ChangeLog ?? "-";
                else
                    dictionary.Add(version.DBVersionNumber, version.ChangeLog ?? "-");
            }
            return dictionary;
        }

        /// <summary>
        /// Gets latest revision of PEMS database.
        /// </summary>
        /// <returns>Returns string representation of revision.</returns>
        public  string GetPemsDbRevision()
        {
            //var version = PemsEntities.Versions.OrderByDescending(m => m.CHANGE_DATE).First();

            //return version == null ? "-" : version.DBVersionNumber;

            return "4.0.0.64";
        }


        /// <summary>
        /// Gets latest revision of PEMS web app.
        /// </summary>
        /// <returns>Returns string representation of revision.</returns>
        public  string GetPemsRevision()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
