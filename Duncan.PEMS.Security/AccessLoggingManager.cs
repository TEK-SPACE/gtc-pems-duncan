using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;

namespace Duncan.PEMS.Security
{
    public class AccessLoggingManager
    {
        public bool Enabled { get { return _logPages || _logAjax; } }

        private bool _logPages = false;
        private bool _logAjax = false;

        private bool _logAccessDenied = false;
        private bool _logAccessAllowed = false;
        private bool _logAccessUndefined = false;


        public AccessLoggingManager()
        {
            // Check if logging is enabled.
            var key = System.Configuration.ConfigurationManager.AppSettings["pems.logging.access.pages"];
            if ( key != null && key.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
            {
                _logPages = true;
            }

            key = System.Configuration.ConfigurationManager.AppSettings["pems.logging.access.ajax"];
            if ( key != null && key.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
            {
                _logAjax = true;
            }

            if ( Enabled )
            {
                key = System.Configuration.ConfigurationManager.AppSettings["pems.logging.access.rights.allowed"];
                if ( key != null && key.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
                {
                    _logAccessAllowed = true;
                }

                key = System.Configuration.ConfigurationManager.AppSettings["pems.logging.access.rights.denied"];
                if ( key != null && key.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
                {
                    _logAccessDenied = true;
                }

                key = System.Configuration.ConfigurationManager.AppSettings["pems.logging.access.rights.undefined"];
                if ( key != null && key.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
                {
                    _logAccessUndefined = true;
                }
            }
        }


        public void Log(string area, string city, string controller, string action,
            string sessionID, int userId, AuthorizationManager.AccessRights accessRights,
            double accessDuration, double accessOverhead)
        {
            if (!Enabled) return;

            var rbacEntities = new PEMRBACEntities();


            if ( !_logAjax )
            {
                if ( string.IsNullOrEmpty( area ) && string.IsNullOrEmpty( city ) )
                    return;
            }

            if ( !_logPages )
            {
                if ( !string.IsNullOrEmpty( area ) && !string.IsNullOrEmpty( city ) )
                    return;
            }

            if ( !_logAccessAllowed )
            {
                if ( accessRights == AuthorizationManager.AccessRights.Allowed )
                    return;
            }

            if ( !_logAccessUndefined )
            {
                if ( ((int)accessRights) > 0 )
                    return;
            }

            if ( !_logAccessDenied )
            {
                if ( ((int)accessRights) < 0 )
                    return;
            }


            rbacEntities.AccessLogs.Add( new AccessLog()
                {
                    Area = area,
                    City = city,
                    Controller = controller,
                    Action = action,
                    SessionID = sessionID,
                    UserId = userId,
                    AccessRights = (int)accessRights,
                    AccessDuration = accessDuration,
                    AccessOverhead = accessOverhead,
                    AccessDate = DateTime.Now
                } );

            rbacEntities.SaveChanges();
        }
    }
}
