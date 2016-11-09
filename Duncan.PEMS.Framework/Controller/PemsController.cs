/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 01/24/2014       Sergey Ostrerov            Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 *                                             DPTXPEMS-8, 45 - Can't create new customer; Replace text box to Drop Down Box for Area editing.
 * 
 * *****************************************************************************************************************************************************/

using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using Duncan.PEMS.Business.Utility;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities;
using Duncan.PEMS.Framework;
using Duncan.PEMS.DataAccess;
using System.Linq;
using System.Collections.Generic;

namespace Duncan.PEMS.Framework.Controller
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Framework.Controller"/> namespace for the base controller classes.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }



    /// <summary>
    /// This is the base controller for all Controller classes for the PEMS system.  All controllers
    /// that support the PEMS page access model should inherit from this controller.
    /// 
    /// This base controller enforces authorization rights, populates Session and ViewData common properties and 
    /// sets the appropriate <see cref="CultureInfo"/> for the currently executing thread.
    /// </summary>
    public class PemsController : BaseController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Logging Properties

        /// <summary>
        /// <see cref="AuthorizationManager.AccessRights"/> variable indicating present rights of the user to 
        /// the requested page.  This is used per page access.
        /// </summary>
        private AuthorizationManager.AccessRights _accessRights = AuthorizationManager.AccessRights.UndefinedAction;

        /// <summary>
        /// DateTime variables used to determine the overhead of the rights authorization and access process.
        /// </summary>
        private DateTime? _beginAccessExecutionOverhead = DateTime.Now;
        private DateTime? _endAccessExecutionOverhead;

        /// <summary>
        /// DateTime variables used to determine the time to process and return a page.
        /// </summary>
        private DateTime? _beginAccessExecution;
        private DateTime? _endAccessExecution;

        #endregion

        #region Common Controller Instance Properties

        /// <summary>
        /// The current city (customer) for this instance of a controller.
        /// </summary>
        protected PemsCity CurrentCity { get; private set; }

        /// <summary>
        /// String representing currently executing controller name
        /// </summary>
        protected string CurrentController { get; private set; }

        /// <summary>
        /// String representing current controller action name
        /// </summary>
        protected string CurrentAction { get; private set; }

        /// <summary>
        /// Strings representing Area of currently executing controller and action.
        /// </summary>
        protected string CurrentArea { get; private set; }

        /// <summary>
        /// The current <see cref="AuthorizationManager"/> for this executing controller.
        /// </summary>
        protected AuthorizationManager CurrentAuthorizationManager { get; private set; }

        #endregion

        /// <summary>
        /// Pre-event before a controller action is called.  This is where system authorization is checked and
        /// controller instance-specific properties and session variables are initialized.
        /// </summary>
        /// <param name="filterContext">Active context</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _logger.Trace("Enter");

            // Check to make sure the user is logged in, if they are not, throw an unauthorized
            if (User == null || (User != null && !User.Identity.IsAuthenticated))
            {
                filterContext.Result = new HttpUnauthorizedResult();
                // now check to see if the current city is the city they have logged into
            }
            else
            {
                SetProperties(filterContext);
                SetCurrentCulture();
                _accessRights = CheckUserAccess(filterContext);
                SetViewData(filterContext);
                _logger.Trace("{0}, C: {1}, A:{2}",
                              _accessRights,
                              filterContext == null ? "?" : ( filterContext.ActionDescriptor == null ? "?" : filterContext.ActionDescriptor.ActionName ?? "??" ),
                              filterContext == null ? "?" : ( filterContext.ActionDescriptor == null ? "?" : filterContext.ActionDescriptor.ControllerDescriptor == null ? "?" : 
                                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName ?? "?"));

                // Act on user rights results.
                switch (_accessRights)
                {
                    case AuthorizationManager.AccessRights.Allowed:
                    case AuthorizationManager.AccessRights.UndefinedAction:
                    case AuthorizationManager.AccessRights.UndefinedAjax:
                        _logger.Debug("{0} Pass execution to action", _accessRights);
                        base.OnActionExecuting(filterContext);
                        break;
                    case AuthorizationManager.AccessRights.DeniedWrongCity:
                        _logger.Debug("{0} Send to city home page", _accessRights);
                        filterContext.Result = SendToCityHomePage(CurrentCity.InternalName);
                        break;
                    case AuthorizationManager.AccessRights.DeniedNoCity:
                        _logger.Debug("{0} Send to landing page", _accessRights);
                        filterContext.Result = SendToLandingPage();
                        break;
                    case AuthorizationManager.AccessRights.DeniedBadUserName:
                        _logger.Debug("{0} Log user out, send to login page", _accessRights);
                        Logout();
                        filterContext.Result = SendToLoginPage();
                        break;
                    case AuthorizationManager.AccessRights.DeniedNoCookie:
                        _logger.Debug("{0} Send to route {1}", _accessRights, Constants.Routing.LandingRouteName);
                        filterContext.Result = RedirectToRoute(Constants.Routing.LandingRouteName);
                        break;
                    case AuthorizationManager.AccessRights.DeniedRBAC:
                        _logger.Debug("{0} Send to city home page", _accessRights);
                        filterContext.Result = SendToCityHomePage(CurrentCity.InternalName);
                        break;
                }

                // Log the end of overhead execution time.
                // Log the beginning of access execution time.
                _beginAccessExecution = _endAccessExecutionOverhead = DateTime.Now;
            }
            _logger.Trace("Exit");
        }

        /// <summary>
        /// Called after the controller action has completed.  The main function of this override
        /// is to enable logging of the controller action time duration.
        /// </summary>
        /// <param name="filterContext">Active context</param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var accessLoggingManager = new AccessLoggingManager();

            // If logging is not enabled, why bother with gathering durations, etc.
            if (!accessLoggingManager.Enabled) return;


            // Log the end of access execution time.
            _endAccessExecution = DateTime.Now;

            // Determine durations.
            double overheadMilliseconds = -1.0;
            if (_beginAccessExecutionOverhead.HasValue && _endAccessExecutionOverhead.HasValue)
            {
                TimeSpan? overheadTime = _endAccessExecutionOverhead - _beginAccessExecutionOverhead;
                overheadMilliseconds = overheadTime.Value.TotalMilliseconds;
            }

            double executionMilliseconds = -1.0;
            if (_beginAccessExecution.HasValue)
            {
                TimeSpan? executionTime = _endAccessExecution - _beginAccessExecution;
                executionMilliseconds = executionTime.Value.TotalMilliseconds;
            }



            // Log the access
            accessLoggingManager.Log(CurrentArea,
                CurrentCity == null ? null : CurrentCity.InternalName,
                CurrentController,
                CurrentAction,
                System.Web.HttpContext.Current.Session.SessionID,
                WebSecurity.CurrentUserId,
                _accessRights,
                executionMilliseconds,
                overheadMilliseconds);
        }

        /// <summary>
        /// Check user access to this controller/action.  This also checks that the user is not trying to spoof a different city as
        /// would be seen in a cross-site attack.
        /// </summary>
        /// <param name="filterContext">Active context</param>
        /// <returns><see cref="AuthorizationManager.AccessRights"/> result</returns>
        private AuthorizationManager.AccessRights CheckUserAccess(ActionExecutingContext filterContext)
        {
            var userCityCookie = GetCityCookie();
            //cant find the cookie, re-send them to the landing page 
            if (userCityCookie == null)
            {
                return AuthorizationManager.AccessRights.DeniedNoCookie;
            }

            //if they do have a cookie, check to see if it matches what they have currently set
            string username = userCityCookie.Value.Split('|')[0];
            string userCity = userCityCookie.Value.Split('|')[1];
            string loginType = userCityCookie.Value.Split('|')[2];

            //if the name doesnt match, delete the cookie, log the user out, and force them to log in again
            if (User.Identity.Name.Trim().ToLower() != username.Trim().ToLower())
            {
                return AuthorizationManager.AccessRights.DeniedBadUserName;
            }

            //now check to make sure the city matches their "logged In" city, otherwise send them to their citys homepage
            if (CurrentCity != null)
            {
                if (CurrentCity.InternalName == null || CurrentCity.InternalName.ToLower() != userCity.ToLower())
                {
                    //only do this if the city is not "None". if it is, then they were jsut logged in and hit the Change Client button. send them back to the select client page.
                    if (userCity == "None")
                    {
                        return AuthorizationManager.AccessRights.DeniedNoCity;
                    }
                    //cities dont match, so change the route datas City to use thiers, then resend
                    return AuthorizationManager.AccessRights.DeniedWrongCity;
                }
            }

            // Now that we have all that done, we need to make sure the user has access in AzMan
            // This check can return one of the following:
            //  AccessRights.UndefinedAjax
            //  AccessRights.DeniedRBAC
            //  AccessRights.Allowed
            //  AccessRights.UndefinedAction

            return (new SecurityManager()).CheckAccess(CurrentCity, CurrentController, CurrentAction, WebSecurity.CurrentUserName);
        }

        /// <summary>
        /// Sets the current properties for city, controller and action based on the URL data coming in.
        /// </summary>
        private void SetProperties(ActionExecutingContext filterContext)
        {
            if (filterContext.RouteData.Values.ContainsKey("city"))
            {
                CurrentCity = new PemsCity(filterContext.RouteData.Values["city"].ToString());

                // Make sure CurrentCity points to a valid city/customer.
                if ( CurrentCity.IsValid )
                {
                    // Populate our static CustomerId
                  //  SqlResourceHelper.CustomerId = CurrentCity.Id;
                }
                else
                {
                    CurrentCity = null;
                }
            }
            if (filterContext.RouteData.Values.ContainsKey("controller"))
                CurrentController = filterContext.RouteData.Values["controller"].ToString();

            if (filterContext.RouteData.Values.ContainsKey("action"))
                CurrentAction = filterContext.RouteData.Values["action"].ToString();

            if (filterContext.Controller.ControllerContext.RequestContext.RouteData.DataTokens.ContainsKey("area"))
                CurrentArea = filterContext.Controller.ControllerContext.RequestContext.RouteData.DataTokens["area"].ToString();

            CurrentAuthorizationManager = CurrentCity != null ? new AuthorizationManager(CurrentCity) : null;

            //
            filterContext.Controller.ViewData[Constants.ViewData.CurrentCity] = CurrentCity == null ? "-" : CurrentCity.DisplayName;
            filterContext.Controller.ViewData[Constants.ViewData.CurrentUser] = User.Identity.Name;

        }

        /// <summary>
        /// Sets useful data in controller ViewData container. Session and ViewData are initialized here for each controller action.
        /// </summary>
        private void SetViewData(ActionExecutingContext filterContext)
        {
            if (CurrentCity == null)
            {
               //do not set session information, just viewdata.

                filterContext.Controller.ViewData[Constants.ViewData.CurrentCity] = "-";
                filterContext.Controller.ViewData[Constants.ViewData.CurrentCityId] = "0";
                filterContext.Controller.ViewData[Constants.ViewData.CurrentCityType] = CustomerProfileType.Customer.ToString();
                filterContext.Controller.ViewData[Constants.ViewData.CurrentLocale] = "en-us";
                filterContext.Controller.ViewData[Constants.ViewData.CurrentTimeFormatIs24] = false;
                filterContext.Controller.ViewData[Constants.ViewData.CurrentTimeZoneOffset] = (int)System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours;
                filterContext.Controller.ViewData[Constants.ViewData.CurrentLocalTimeDisplay] = DateTime.Now.ToString("F");
                filterContext.Controller.ViewData[Constants.ViewData.CurrentLocalTime] = DateTime.Now;
            }
            else
            {
                Session[Constants.ViewData.CurrentCity] = filterContext.Controller.ViewData[Constants.ViewData.CurrentCity] = CurrentCity.DisplayName;
                Session[Constants.ViewData.CurrentCityId] = filterContext.Controller.ViewData[Constants.ViewData.CurrentCityId] = CurrentCity.Id.ToString();
                Session[Constants.ViewData.CurrentCityType] = filterContext.Controller.ViewData[Constants.ViewData.CurrentCityType] = CurrentCity.CustomerType.ToString();
                Session[Constants.ViewData.CurrentLocale] = filterContext.Controller.ViewData[Constants.ViewData.CurrentLocale] = CurrentCity.Locale.Name;
                Session[Constants.ViewData.CurrentTimeFormatIs24] = filterContext.Controller.ViewData[Constants.ViewData.CurrentTimeFormatIs24] = CurrentCity.Is24HourFormat;
                Session[Constants.ViewData.CurrentTimeZoneOffset] = filterContext.Controller.ViewData[Constants.ViewData.CurrentTimeZoneOffset] = CurrentCity.UTCOffset;
                Session[Constants.ViewData.CurrentLocalTimeDisplay] = filterContext.Controller.ViewData[Constants.ViewData.CurrentLocalTimeDisplay] = CurrentCity.LocalTime.ToString("F");
                Session[Constants.ViewData.CurrentLocalTime] = filterContext.Controller.ViewData[Constants.ViewData.CurrentLocalTime] = CurrentCity.LocalTime;
                Session[Constants.Security.ConnectionStringSessionVariableName] = CurrentCity.PemsConnectionStringName ?? ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName];
                Session[Constants.Security.MaintenanceConnectionStringSessionVariableName] = CurrentCity.MaintenanceConnectionStringName ??  ConfigurationManager.AppSettings[Constants.Security.DefaultMaintConnectionStringName];
                Session[Constants.Security.ReportingConnectionStringSessionVariableName] = CurrentCity.ReportingConnectionStringName ?? ConfigurationManager.AppSettings[ Constants.Security.DefaultReportingConnectionStringName];
            }

            Session[Constants.ViewData.CurrentUser] = filterContext.Controller.ViewData[Constants.ViewData.CurrentUser] = User.Identity.Name;
            Session[Constants.ViewData.CurrentAction] = filterContext.Controller.ViewData[Constants.ViewData.CurrentAction] = CurrentAction;
            Session[Constants.ViewData.CurrentController] = filterContext.Controller.ViewData[Constants.ViewData.CurrentController] = CurrentController;
            Session[Constants.ViewData.CurrentArea] = filterContext.Controller.ViewData[Constants.ViewData.CurrentArea] = CurrentArea;

            // Get Idle Timeout settings
            filterContext.Controller.ViewData[Constants.ViewData.IdleTimeout] = ConfigurationManager.AppSettings[Constants.Security.IdleTimeout] ?? Constants.IdleTimeoutDefaults.IdleTimeout;
            filterContext.Controller.ViewData[Constants.ViewData.IdleTimeoutWarning] = ConfigurationManager.AppSettings[Constants.Security.IdleTimeoutWarning] ?? Constants.IdleTimeoutDefaults.IdleTimeoutWarning;
            filterContext.Controller.ViewData[Constants.ViewData.IdleTimeoutPolling] = ConfigurationManager.AppSettings[Constants.Security.IdleTimeoutPolling] ?? Constants.IdleTimeoutDefaults.IdleTimeoutPolling;

            // Get database versions
            var envFactory = new EnvironmentSettingsFactory(ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName]);
            string dbRevision = Session["__DbRevision"] as string;
            if (dbRevision == null)
            {
                dbRevision = envFactory.GetPemsDbRevision();
                Session["__DbRevision"] = dbRevision;
            }
            Session[Constants.ViewData.CurrentRevision] = filterContext.Controller.ViewData[Constants.ViewData.CurrentRevision] = envFactory.GetPemsRevision() + "/" + dbRevision ?? "?";
        }

        /// <summary>
        /// Set the thread's current culture based on <see cref="CurrentCity"/> locale
        /// or if <see cref="CurrentCity"/> is null, default to CultureInfo.CurrentCulture
        /// </summary>
        private void SetCurrentCulture()
        {
            CultureInfo locale = CurrentCity == null
                                     ? CultureInfo.CurrentCulture
                                     : CurrentCity.Locale ?? CultureInfo.CurrentCulture;
            // Is 24-hour clock selected and is culture only 12?
            if ((CurrentCity == null ? false : CurrentCity.Is24HourFormat) && locale.DateTimeFormat.ShortTimePattern.Contains("h:"))
            {
                // Set 24-hour time format at the thread level.
                locale = new CultureInfo(locale.Name, false);
                locale.DateTimeFormat.ShortTimePattern = "HH:mm";
                locale.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            }

            Thread.CurrentThread.CurrentCulture = locale;
            Thread.CurrentThread.CurrentUICulture = locale;
        }

        // Check AssetID entered by User
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="customerID"></param>
        /// <param name="areaID"></param>
        /// <param name="assetTpeId"></param>
        /// <returns></returns>
        protected ActionResult AssetIDExistence(string assetID, string customerID, string areaID, string assetTpeId)
        {
            int customerId = Int32.Parse(customerID);
            int areaId = Int32.Parse(areaID);
            string assetIDNew = "0";
            int asID = 0;
            int astTpeId = 0;
            var device = new object();

            PEMEntities pe = new PEMEntities();
            Meter meter = null;
            CashBox cashbox = null;
            Gateway gteway = null;
            MechMaster mechanism = null;
            DataKey dataKey = null;
            device = null;

            if (!String.IsNullOrEmpty(assetID))
            {
                asID = Int32.Parse(assetID);
            }

            if (!String.IsNullOrEmpty(assetTpeId))
            {
                astTpeId = Int32.Parse(assetTpeId);
            }


            switch (Int32.Parse(assetTpeId))
            {
                case (int)MeterGroups.MultiSpaceMeter:
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.SingleSpaceMeter:
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.Cashbox:
                    cashbox = pe.CashBoxes.FirstOrDefault(c => c.CustomerID == customerId && c.CashBoxSeq == asID);
                    device = cashbox;
                    break;
                case (int)MeterGroups.Gateway:
                    gteway = pe.Gateways.FirstOrDefault(g => g.CustomerID == customerId && g.GateWayID == asID);
                    device = gteway;
                    break;
                case (int)MeterGroups.Sensor:
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.Mechanism:
                   // mechanism = pe.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechIdNumber == asID);
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.Datakey:
                    dataKey = pe.DataKeys.FirstOrDefault(m => m.CustomerID == customerId && m.DataKeyIdNumber == asID);
                    device = dataKey;
                    break;
            }

            if (device != null)
            {
                assetIDNew = "Invalid";
            }
            else if (device == null)
            {
                assetIDNew = "Valid";
            }

            if (device != null)
            {
                assetIDNew = "Meter " + assetID + " already in use";
            }
            else
            {
                assetIDNew = "Meter " + assetID + " Valid";
            }
            return Json(assetIDNew, JsonRequestBehavior.AllowGet);
        }

    }
}