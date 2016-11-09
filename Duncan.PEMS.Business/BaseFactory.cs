using System.Linq;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Utilities;
using System.Configuration;
using NLog;

namespace Duncan.PEMS.Business
{
    /// <summary>
    /// This is the base business factory class for PEMS factories.  This class encapsulates the Entity Framework context 
    /// that is used to access the PEMS database.  This class would be inherited from for factories
    /// that accessed the PEMS database.  This class inherits from <see cref="RbacBaseFactory"/> to allow the factories
    /// to access PEMS RBAC database also.
    /// </summary>
    public abstract class BaseFactory : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private string _connectionStringName;
        /// <summary>
        /// This represents the name of the connection string for accessing the PEMS database instance.  This 
        /// name is used to look up the connection string in the web.config or connectionStrings.config.
        /// </summary>
        protected string ConnectionStringName
        { 
            get
            {
                // Default it to the default connection string if it hasnt been set
                if ( string.IsNullOrEmpty( _connectionStringName ) )
                {
                    _connectionStringName = ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName];
                }
                return _connectionStringName;
            }
            set
            {
                _connectionStringName = value;
            } 
        }

       
            
        /// <summary>
        /// Instance of <see cref="PEMEntities"/> for factory use.
        /// </summary>
        private PEMEntities _pemsEntities;
        /// <summary>
        /// Protected property to get/create instance of <see cref="PEMEntities"/>
        /// </summary> 
        protected PEMEntities PemsEntities
        {
            get
            {
                // Create or, if exists, return an instance of a PEMEntities
                return _pemsEntities ?? (_pemsEntities = new PEMEntities(ConnectionStringName));
            }
        }

        /// <summary>
        /// Checks to see if all the values in the list are equal
        /// </summary>
        public bool AllEqual<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
                return true;
            return values.All(v => v.Equals(values[0]));
        }

    }
}
