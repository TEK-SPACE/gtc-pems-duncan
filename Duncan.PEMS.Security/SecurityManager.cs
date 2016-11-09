using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using NLog;
using WebMatrix.WebData;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Security
{
    public class SecurityManager
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        private readonly AuthorizationManager _azManager = new AuthorizationManager();

        #region "Access"

        public List<PemsCity> GetCitiesForUser(string username)
        {
            //we have to get the id of the user for our caching table
            var rbacEntities = new PEMRBACEntities();
            username = username.Trim();
            var user = rbacEntities.UserProfiles.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                int userId = user.UserId;
                //added caching to curcumvent azman performance issues
                //first, try to get a list of cities in the caching table
                var ucaManager = new UserCustomerAccessManager();
                var existingCustomerIds = ucaManager.GetCustomersIds(userId);

                //if we found any, build a lsit of pems cities and return
                if (existingCustomerIds.Any())
                    return existingCustomerIds.Select(customerId => new PemsCity(customerId.ToString())).ToList();

                //if we havent found any, we have to get them,
                var cities = _azManager.GetAuthorizedCities(username);
                var customerIds = cities.Select(x => x.Id).Distinct().ToList();
                // save them to the caching table,
                ucaManager.SetCustomersIds(userId, customerIds);
                // then return them.
                return cities;
            }
            return new List<PemsCity>();
        }
        public List<string> GetUsersForCity(string cityName)
        {
            return _azManager.GetUsersForStore(cityName);
        }

        public bool CheckUserAccessForCity(string username, PemsCity city)
        {
            var cities = GetCitiesForUser( username );
            return cities.Any(x => x.InternalName == city.InternalName);
        }

        public Dictionary<string, int> GetGroups(PemsCity city, bool includeDefaultGroups = false)
        {
            return GetGroups(city.InternalName, includeDefaultGroups);
        }

        public Dictionary<string, int> GetGroups(string  internalName, bool includeDefaultGroups = false)
        {
            var groups = _azManager.GetStoreGroups(internalName); ;
            if (includeDefaultGroups)
                return groups;
            return groups.Where(x => !x.Key.StartsWith(Constants.Security.DefaultGroupNamePrefix))
                         .ToDictionary(x => x.Key, x => x.Value);
        }

        public Dictionary<string, bool> GetGroupsForUser(PemsCity city, string username, bool includeDefaultGroups = false)
        {
            //get all the groups
            var groups = _azManager.GetStoreGroupsForUser(city.InternalName, username);
            //include the default group
            if (includeDefaultGroups)
                return groups;

            //remove the default group
            return groups.Where(x => !x.Key.StartsWith(Constants.Security.DefaultGroupNamePrefix)).ToDictionary(x => x.Key, x => x.Value);

        }


        public  string GetGroupForUser(PemsCity city, string username)
        {
            //get all the groups
           return _azManager.GetStoreGroupForUser(city.InternalName, username);
        }

        public List<string> GetCities()
        {
            return _azManager.GetStores();
        }

        public AuthorizationManager.AccessRights CheckAccess(PemsCity city, string controller, string action, string userName)
        {
            return _azManager.CheckAccess( city, controller, action, userName );
        }

        #endregion

        #region "Membership"

        /// Log login attempts.  This can be enabled/disabled in the web.config.
        ///  <appSettings>
        ///    <add key="pems.logging.log_attempts" value="true" />
        ///  </appSettings>
        public void LogLogin(string username, string password, string url)
        {
            var key = System.Configuration.ConfigurationManager.AppSettings["pems.logging.log_attempts"];
            if ( key != null && key.Equals( "true", StringComparison.InvariantCultureIgnoreCase ) )
            {

                var ipAddress = url==null? "-" :GetDomainName(url);
                PEMRBACEntities context = new PEMRBACEntities();
                context.LoginAttemptHistories.Add( new LoginAttemptHistory()
                    {
                        UserName = username ?? "-",
                        Password = password ?? "-",
                        IpAddress = url == null ? "-" : url.Substring(0, Math.Min(url.Length, 128)),
                        AccessDate = DateTime.Now
                    } );
                context.SaveChanges();
            }
        }

        private static string GetDomainName(string url)
        {
            string domain = new Uri(url).DnsSafeHost.ToLower();
            var tokens = domain.Split('.');
            if (tokens.Length > 2)
            {
                //Add only second level exceptions to the < 3 rule here
                string[] exceptions = { "info", "firm", "name", "com", "biz", "gen", "ltd", "web", "net", "pro", "org" };
                var validTokens = 2 + ((tokens[tokens.Length - 2].Length < 3 || exceptions.Contains(tokens[tokens.Length - 2])) ? 1 : 0);
                domain = string.Join(".", tokens, tokens.Length - validTokens, validTokens);
            }
            return domain;
        }



        public bool Login(string username, string password)
        {
            // Validate credentials
            if ( string.IsNullOrEmpty( username ) || string.IsNullOrEmpty( password ) )
                return false;

            bool credentialsAreValid = WebSecurity.Login( username, password );
            if ( credentialsAreValid )
            {
                // Must add object to session in order to create a _persistent_ session id
                HttpContext.Current.Session.Add( "sessionPlaceholder", "" );

                // Log event
                PEMRBACEntities context = new PEMRBACEntities();
                context.AccessLogMembershipEvents.Add( new AccessLogMembershipEvent()
                    {
                        EventTypeId = (int) MembershipEvent.Login,
                        UserId = WebSecurity.GetUserId( username ),
                        IPAddress = HttpContext.Current.Request.GetIpAddress().ToString(),
                        SessionID = HttpContext.Current.Session.SessionID,
                        TimeStamp = DateTime.Now
                    } );
                context.SaveChanges();
            }
            else
            {
                if ( WebSecurity.UserExists( username ) )
                {
                    // username is valid, so password must have been invalid
                    PEMRBACEntities context = new PEMRBACEntities();
                    context.AccessLogMembershipEvents.Add(new AccessLogMembershipEvent()
                    {
                        EventTypeId = (int)MembershipEvent.LoginFailure,
                        UserId = WebSecurity.GetUserId(username),
                        IPAddress = HttpContext.Current.Request.GetIpAddress().ToString(),
                        SessionID = null,
                        TimeStamp = DateTime.Now
                    });
                    context.SaveChanges();
                }
            }

            return credentialsAreValid;
        }

        public static void Logout()
        {
            PEMRBACEntities context = new PEMRBACEntities();
            context.AccessLogMembershipEvents.Add(new AccessLogMembershipEvent()
            {
                EventTypeId = (int)MembershipEvent.Logout,
                UserId = WebSecurity.CurrentUserId,
                IPAddress = HttpContext.Current.Request.GetIpAddress().ToString(),
                SessionID = HttpContext.Current.Session.SessionID,
                TimeStamp = DateTime.Now
            });
            context.SaveChanges();

            DeleteSessionCookie();
            WebSecurity.Logout();
        }

        /// <summary>
        /// Deletes the session cookie and clears the server session space
        /// </summary>
        public static void DeleteSessionCookie()
        {
            HttpContext.Current.Session.Abandon(); // trash server side session objects
            HttpCookie cookie = HttpContext.Current.Response.Cookies["ASP.NET_SessionId"];
            if ( cookie != null )
                cookie.Expires = DateTime.Now.AddYears( -1 ); // expire browser session cookie
        }

        

        /// <summary>
        /// Initializes connection to the PEMRBAC database for SimpleMembership
        /// </summary>
        public static void InitializeMembershipConnection()
        {
            try
            {
                if ( !WebSecurity.Initialized )
                    WebSecurity.InitializeDatabaseConnection( Constants.Security.RbacConnectionStringName, "UserProfile", "UserId", "UserName", true );
            }
            catch (Exception ex)
            {
                const string errorDetail = "Error initializing database connection '" + Constants.Security.RbacConnectionStringName + "'";
                _logger.FatalException(errorDetail, ex);
                throw(new Exception(errorDetail, ex));
            }
        }

        #endregion
    }
}