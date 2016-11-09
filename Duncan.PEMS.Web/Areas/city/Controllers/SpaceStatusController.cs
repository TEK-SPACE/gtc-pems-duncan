using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using System.Web.UI;
using Duncan.PEMS.SpaceStatus.UtilityClasses;
using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;

using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Business.FileUpload;
using Duncan.PEMS.Entities.Enumerations;
using WebMatrix.WebData;
using Duncan.PEMS.Entities.Reports;
using Duncan.PEMS.Business.Reports;
using System.Configuration;
using System.Web.Configuration;




namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    // Attribute indicates the controller should use the "CompressFilter" action filter so we can conserve bandwidth when supported
    [CompressFilter]
    public class SpaceStatusController : PemsController
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            // Some of these ideas for better logging and error messages came from: http://stackoverflow.com/questions/1464531/how-do-i-use-application-error-in-asp-net-mvc
            // One problem with the Application_Error modthod in global.asax.cs is that it is oblivious to our ViewData, which has some elements that are used on the site.master header, etc.

            if (filterContext.ExceptionHandled == false)
            {
                // Try to get the class and method that raised the error.  If not available, then we will just get this current method instead
                string errSourceClass = string.Empty;
                string errSourceMethod = string.Empty;
                if (filterContext.Exception.TargetSite != null)
                {
                    errSourceClass = filterContext.Exception.TargetSite.DeclaringType.Name;
                    errSourceMethod = filterContext.Exception.TargetSite.Name;
                }

                if (string.IsNullOrEmpty(errSourceClass))
                {
                    errSourceClass = MethodBase.GetCurrentMethod().DeclaringType.Name;
                    errSourceMethod = MethodBase.GetCurrentMethod().Name;
                }

                // Get the controller and action that was current when the error was raised
                var httpContext = filterContext.HttpContext;
                var currentRouteData = filterContext.RouteData;
                var currentController = " ";
                var currentAction = " ";
                if (currentRouteData != null)
                {
                    if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                        currentController = currentRouteData.Values["controller"].ToString();

                    if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                        currentAction = currentRouteData.Values["action"].ToString();
                }

                // Log the error
                Logging.AddTextToGenericLog(Logging.LogLevel.Error, filterContext.Exception.ToString(),
                    errSourceClass + "." + errSourceMethod, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // Output to diagnostic listener in debug mode
                var err = string.Concat("Error in: ", Request.Url, "\nMessage:", filterContext.Exception.ToString(), "\nStack Trace:", filterContext.Exception.StackTrace);
                System.Diagnostics.Debug.WriteLine(err);

                // Prepare the error controller to display our error information
                var errController = new Duncan.PEMS.Web.Areas.city.Controllers.ErrorController();

                // Try to determine if the target is Windows Mobile
                bool isWinCE = IsRequestFromWindowsMobileDevice();
                errController.ViewData["IsWinCE"] = isWinCE;

                // Build error information from current context
                // Show full error information when using LocalHost, otherwise only show the "Message" portion (without inner exceptions, etc)
                string errMsgToRender = string.Empty;
                if (Request.Url.Host.ToUpper() == "LOCALHOST")
                {
                    errMsgToRender = filterContext.Exception.ToString();
                }
                else
                {
                    if (string.Compare(filterContext.Exception.GetType().Name, "Exception", true) != 0)
                        errMsgToRender = filterContext.Exception.GetType().Name + ": " + filterContext.Exception.Message;
                    else
                        errMsgToRender = filterContext.Exception.Message;
                }
                ExtendedHandleErrorInfo errorModel = new ExtendedHandleErrorInfo(filterContext.Exception,
                    currentController, currentAction,
                    errMsgToRender);

                // Add the error model to the error controller's view data model
                errorModel.NeverShowStackTrace = false;
                errController.ViewData.Model = errorModel;

                // Copy view data elements from this controller to the error controller
                try
                {
                    foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in this.ViewData)
                    {
                        errController.ViewData[kvp.Key] = kvp.Value;
                    }
                }
                catch { }

                // TrySkipIisCustomErrors must be true to prevent IIS 7 from hijacking our custom error screens
                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = filterContext.Exception is HttpException ? ((HttpException)filterContext.Exception).GetHttpCode() : 500;
                httpContext.Response.TrySkipIisCustomErrors = true;

                // Set the result as the view from the error controller
                filterContext.Result = errController.Index();
                filterContext.ExceptionHandled = true;
            }
        }

        private bool IsRequestFromWindowsMobileDevice()
        {
            bool result = false;
            try
            {
                // Look for certain user agents or browser names
                if ((this.Request.UserAgent.IndexOf("windows ce", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                    (this.Request.UserAgent.IndexOf("msiemobile", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                    (this.Request.Browser.Browser.IndexOf("iemobile", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                    (this.Request.Browser.Platform.IndexOf("wince", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                    (
                    // IE on Windows Mobile has a "View | Desktop" option that will spoof a regular IE and make it hard to know 
                    // its really a mobile device.  In this case, we will look for "IE" browser with ScreenPixelWidth <= 640
                    // and an older version of IE (Side-effect is some desktop users with older IE will be treated as mobile device...)
                      (this.Request.Browser.Browser.IndexOf("ie", StringComparison.InvariantCultureIgnoreCase) == 0) &&
                      (this.Request.Browser.ScreenPixelsWidth <= 640) &&
                      (this.Request.Browser.MajorVersion <= 7)
                    )
                   )
                {
                    result = true;
                }
            }
            catch { }

            this.ViewData["IsWinCE"] = result;

            this.ViewData["IsWinMobile6_5"] = false;
            if (result == true)
            {
                if ((this.Request.UserAgent.IndexOf("IEMobile 8.12", StringComparison.InvariantCultureIgnoreCase) != -1) ||
                    (this.Request.UserAgent.IndexOf("Windows Phone 6.5", StringComparison.InvariantCultureIgnoreCase) != -1))
                {
                    this.ViewData["IsWinMobile6_5"] = true;
                }
            }

            return result;
        }

        private ActionResult ShowSimpleError(string errorMessage, string fromAction)
        {
            // Prepare the error controller to display our error information
            var controller = new Duncan.PEMS.Web.Areas.city.Controllers.ErrorController();

            // Try to determine if the target is Windows Mobile
            bool isWinCE = IsRequestFromWindowsMobileDevice();
            controller.ViewData["IsWinCE"] = isWinCE;

            // Create an extended error info object and invoke the controller method
            ExtendedHandleErrorInfo errorModel = new ExtendedHandleErrorInfo(new Exception(), MethodBase.GetCurrentMethod().DeclaringType.Name,
                fromAction, errorMessage);
            errorModel.NeverShowStackTrace = true;
            controller.ViewData.Model = errorModel;

            try
            {
                foreach (System.Collections.Generic.KeyValuePair<string, object> kvp in this.ViewData)
                {
                    controller.ViewData[kvp.Key] = kvp.Value;
                }
            }
            catch { }

            return controller.Index();
        }




        public ActionResult ReportSpaceStatus()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.SpaceStatus);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        //
        // GET: /SpaceStatus/
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("GrpSummary");
        }

        #region Select Client Views

        // GET: /SpaceStatus/SelectClient
        [Authorize]
        public ActionResult SelectClient()
        {
            Logging.AddTextToGenericLog(Logging.LogLevel.DebugTraceOutput,
                "Entered SelectClient action for SpaceStatus Controller",
                MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // Build list of customers
            List<string> availCustomers = new List<string>();

            List<CustomerConfig> allCustomerDTOs = CustomerLogic.CustomerManager.GetAllCustomerDTOs();
            foreach (CustomerConfig nextCustCfg in allCustomerDTOs)
            {
                // We need to verify that the user has sufficient RBAC access to view this customer!
                bool userHasAccessToCustomer = MvcApplication.RBAC.RoleProvider.IsUserInRole(User.Identity.Name, "Customer:" + nextCustCfg.CustomerId.ToString());
                if (userHasAccessToCustomer == false)
                {
                    // If no access, see if user is an Admin -- that will override all
                    userHasAccessToCustomer = MvcApplication.RBAC.RoleProvider.IsUserInRole(User.Identity.Name, "Admin");
                }

                // Only add the customer to the list if user has sufficient access rights
                if (userHasAccessToCustomer == true)
                    availCustomers.Add(nextCustCfg.CustomerName);
            }

            // Now sort the list of customers
            availCustomers.Sort();

            Logging.AddTextToGenericLog(Logging.LogLevel.DebugTraceOutput,
                "Got list of accessible customers for user: " + User.Identity.Name,
                MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

            // If there is only 1 customer to choose from, we don't want to display this page to the user.
            // Instead, we will redirect back to calling action, but include the CID as a query param
            if (availCustomers.Count == 1)
            {
                // Build a RouteValueDictionary containing the values derived from the passed collection 
                RouteValueDictionary routeValues = new RouteValueDictionary();
                foreach (string key in Request.QueryString.Keys)
                {
                    // We want to retain passed query params except for the ReturnAction and CID (if present)
                    if ((string.Compare(key, "ReturnAction") != 0) && (string.Compare(key, "CID") != 0) && (string.Compare(key, "ReturnController") != 0))
                        routeValues[key] = Request.QueryString[key].ToString();
                }

                // See if there is a ReturnAction specified. If none, set a default appropriate for this controller
                string ReturnAction = Request.QueryString["ReturnAction"];
                if (string.IsNullOrEmpty(ReturnAction))
                    ReturnAction = Url.Content("Index");

                // See if there is a ReturnController specified. If none, assume we are in the current controller (i.e. SpaceStatus)
                string ReturnController = Request.QueryString["ReturnController"];
                if (!string.IsNullOrEmpty(ReturnController))
                    routeValues["controller"] = ReturnController;

                // Cross-ref the customer name to the CustomerID
                string CIDValue = "";
                foreach (CustomerConfig nextCustCfg in allCustomerDTOs)
                {
                    if (string.Compare(nextCustCfg.CustomerName.ToString(), availCustomers[0], true) == 0)
                    {
                        CIDValue = nextCustCfg.CustomerId.ToString();
                        routeValues.Add("CID", CIDValue);

                        // This is the customer config we want, so retain it and put it in ViewData so the View has access to it
                        this.ViewData["CustomerCfg"] = nextCustCfg;
                        this.ViewData["CID"] = nextCustCfg.CustomerId;
                        this.ViewData["CustomerName"] = nextCustCfg.CustomerName;

                        break;
                    }
                }

                // Redirect back to the calling action (if known), otherwise go to our default action. We will send the CID to the return action
                if ((ViewData["CustomerCfg"] as CustomerConfig) != null)
                    return RedirectToAction(ReturnAction, routeValues);
                /*return RedirectToAction(ReturnAction, new { CID = customerDto.CustomerId.ToString() });*/
            }

            // Put the available customer list in the ViewData so the View can access them
            if (availCustomers.Count > 0)
            {
                ViewData["AvailCustomers"] = new SelectList(availCustomers, availCustomers[0]);
            }
            else
            {
                /*throw new Exception("User '" + User.Identity.Name + "' hasn't been assigned access rights to any customers yet.");*/
                ActionResult errorResult = ShowSimpleError("User '" + User.Identity.Name + "' hasn't been assigned access rights to any customers yet.", MethodBase.GetCurrentMethod().Name);
                return errorResult;
            }

            return View("SelectClient");
        }

        // POST: /SpaceStatus/SelectClient
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelectClient(FormCollection formValues)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // See if there a ReturnAction specified. If none, set a default appropriate for this controller
            string ReturnAction = Request.QueryString["ReturnAction"];
            if (string.IsNullOrEmpty(ReturnAction))
                ReturnAction = Url.Content("Index");

            // Build a RouteValueDictionary containing the values derived from the passed collection 
            RouteValueDictionary routeValues = new RouteValueDictionary();
            foreach (string key in Request.QueryString.Keys)
            {
                // We want to retain passed query params except for the ReturnAction and CID (if present)
                if ((string.Compare(key, "ReturnAction") != 0) && (string.Compare(key, "CID") != 0) && (string.Compare(key, "ReturnController") != 0))
                    routeValues[key] = Request.QueryString[key].ToString();
            }

            // See if there is a ReturnController specified. If none, assume we are in the current controller (i.e. SpaceStatus)
            string ReturnController = Request.QueryString["ReturnController"];
            if (!string.IsNullOrEmpty(ReturnController))
                routeValues["controller"] = ReturnController;

            // Get the name of the customer that was selected
            string selectedCustomerName = formValues["Customers"];

            // Cross-ref the customer name to the CustomerID
            string CIDValue = "";
            List<CustomerConfig> allCustomerDTOs = CustomerLogic.CustomerManager.GetAllCustomerDTOs();
            foreach (CustomerConfig nextCustCfg in allCustomerDTOs)
            {
                if (string.Compare(nextCustCfg.CustomerName.ToString().Trim(), selectedCustomerName.Trim(), true) == 0)
                {
                    CIDValue = nextCustCfg.CustomerId.ToString();
                    break;
                }
            }
            routeValues.Add("CID", CIDValue);

            // Redirect back to the calling action (if known), otherwise go to our default action. We will send the CID to the return action
            return RedirectToAction(ReturnAction, routeValues);
            /*return RedirectToAction(ReturnAction, new { CID = CIDValue });*/
        }

        #endregion // Select Client Views


        #region Mobile Vehicle Sensing Views (Deprecated/Not Used, but maybe useful for future)

        // Mobile VehSensing Group Summary
        // GET: /SpaceStatus/mVSGrpSummary
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>

        public ActionResult mVSGrpSummary(string G, string V)
        {
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area:";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter:";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterName;
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(mVSGrpSummary) Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);

            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            return View("mVSGrpSummary", modelForView);
        }

        // Mobile VehSensing Group Summary (Partial View)
        // GET: /SpaceStatus/mVSGrpSummaryPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses
        [ValidateSelectedCustomer(Order = 2)]
        public ActionResult mVSGrpSummaryPartial(string G, string V, List<SpaceStatusModel> modelForView)
        {
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Build list of choices based on the group type
            if (groupType == "A")
            {
            }
            else if (groupType == "M")
            {
            }
            else
            {
                throw new Exception("(mVSGrpSummaryPartial) Support for group type '" + groupType + "' not implemented.");
            }



            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);

            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            if (modelForView == null)
                modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            return View("pv_mVSGrpSummaryContent", modelForView);
        }



        // Mobile VehSensing Space Summary
        // GET: /SpaceStatus/mVSSpcSummary
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>

        public ActionResult mVSSpcSummary(string T, string G, string V)
        {
            // Retain original target
            string originalTarget = T;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;


            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);

                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (spcAsset.MeterID == mtrAsset.MeterID)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));

                // Find all SpaceIDs that are associated with the current meter
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.MeterID.ToString(), T) == 0)
                    {
                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (string.Compare(spcAsset.MeterID.ToString(), T) == 0)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // If we ended up with only 1 choice, we might as well skip this summary level and go directly to the space detail screen instead
            if (groupChoices.Count == 1)
            {
                GenericObjForHTMLSelectList onlyChoice = groupChoices[0];

                // Build a RouteValueDictionary for redirecting to detail page
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", onlyChoice.DataValue);
                routeValues.Add("PT", originalTarget);
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());

                // Now redirect to space details view
                return RedirectToAction("mVSSpcDetail", routeValues);
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces associated with our grouping
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            return View("mVSSpcSummary", modelForView);
        }

        // Mobile VehSensing Space Summary (Partial View)
        // GET: /SpaceStatus/mVSSpcSummaryPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult mVSSpcSummaryPartial(string T, string G, string V)
        {
            // Retain original target
            string originalTarget = T;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // We will need to gather current info for all meters and spaces associated with our grouping
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            return View("pv_mVSSpcSummaryContent", modelForView);
        }



        // Mobile VehSensing Space Detail
        // GET: /SpaceStatus/mVSSpcDetail
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>

        public ActionResult mVSSpcDetail(string T, string PT, string G, string V)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            string groupChoiceDefaultSelection = null;
            int SelectedIndex = 0;
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current area
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (spcAsset.MeterID == mtrAsset.MeterID)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if ((string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0) &&
                                        (string.Compare(MID, spcAsset.MeterID.ToString()) == 0))
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current meter
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.MeterID.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (string.Compare(spcAsset.MeterID.ToString(), PT) == 0)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if (string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0)
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText", groupChoiceDefaultSelection);
            ViewData["SelectedIndex"] = SelectedIndex;

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));

            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            return View("mVSSpcDetail", modelForView);
        }

        // Mobile VehSensing Space Detail (Partial View)
        // GET: /SpaceStatus/mVSSpcDetailPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult mVSSpcDetailPartial(string T, string PT, string G, string V)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            return View("pv_mVSSpcDetailContent", modelForView);
        }

        #endregion // Mobile Vehicle Sensing Views


        #region Mobile Enforcement Views (Deprecated/Not Used, but maybe useful for future)

        // Mobile EnfSensing Group Summary
        // GET: /SpaceStatus/mEnfGrpSummary
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>

        public ActionResult mEnfGrpSummary(string G, string V)
        {
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area:";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter:";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterName;
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(mEnfGrpSummary) Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("mEnfGrpSummary", modelForView);
        }

        // Mobile EnfSensing Group Summary (Partial View)
        // GET: /SpaceStatus/mEnfGrpSummaryPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult mEnfGrpSummaryPartial(string G, string V, List<SpaceStatusModel> modelForView)
        {
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Build list of choices based on the group type
            if (groupType == "A")
            {
            }
            else if (groupType == "M")
            {
            }
            else
            {
                throw new Exception("(mEnfGrpSummaryPartial) Support for group type '" + groupType + "' not implemented.");
            }

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            if (modelForView == null)
                modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("pv_mEnfGrpSummaryContent", modelForView);
        }


        // Mobile EnfSensing Space Summary
        // GET: /SpaceStatus/mEnfSpcSummary
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>

        public ActionResult mEnfSpcSummary(string T, string G, string V)
        {
            // Retain original target
            string originalTarget = T;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);

                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (spcAsset.MeterID == mtrAsset.MeterID)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));

                // Find all SpaceIDs that are associated with the current meter
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.MeterID.ToString(), T) == 0)
                    {
                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (string.Compare(spcAsset.MeterID.ToString(), T) == 0)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // If we ended up with only 1 choice, we might as well skip this summary level and go directly to the space detail screen instead
            if (groupChoices.Count == 1)
            {
                GenericObjForHTMLSelectList onlyChoice = groupChoices[0];

                // Build a RouteValueDictionary for redirecting to detail page
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", onlyChoice.DataValue);
                routeValues.Add("PT", originalTarget);
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());

                // Now redirect to space details view
                return RedirectToAction("mEnfSpcDetail", routeValues);
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces associated with our grouping
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("mEnfSpcSummary", modelForView);
        }

        // Mobile Enforcement Space Summary (Partial View)
        // GET: /SpaceStatus/mEnfSpcSummaryPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult mEnfSpcSummaryPartial(string T, string G, string V)
        {
            // Retain original target
            string originalTarget = T;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // We will need to gather current info for all meters and spaces associated with our grouping
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("pv_mEnfSpcSummaryContent", modelForView);
        }


        // Mobile Enforcement Space Detail
        // GET: /SpaceStatus/mEnfSpcDetail
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>

        public ActionResult mEnfSpcDetail(string T, string PT, string G, string V)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            string groupChoiceDefaultSelection = null;
            int SelectedIndex = 0;
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current area
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (spcAsset.MeterID == mtrAsset.MeterID)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if ((string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0) &&
                                        (string.Compare(MID, spcAsset.MeterID.ToString()) == 0))
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current meter
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.MeterID.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (string.Compare(spcAsset.MeterID.ToString(), PT) == 0)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if (string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0)
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText", groupChoiceDefaultSelection);
            ViewData["SelectedIndex"] = SelectedIndex;

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            return View("mEnfSpcDetail", modelForView);
        }

        // Mobile Enforcement Space Detail (Partial View)
        // GET: /SpaceStatus/mEnfSpcDetailPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult mEnfSpcDetailPartial(string T, string PT, string G, string V)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }



            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            return View("pv_mEnfSpcDetailContent", modelForView);
        }

        #endregion // Mobile Enforcement Views


        #region Mobile Drill-Down Views (Deprecated/Not Used, but maybe useful for future)

        // GET: /SpaceStatus/MobileDrillDown

        public ActionResult MobileDrillDown()
        {
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // DEBUG: Just testing...
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            /*
            int loClusterID = 101;
            listOfMeterIDs = GetMeterIDsInCluster(loClusterID, customerDto);
            */
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }

            ViewData["MeterList"] = new SelectList(listOfMeterIDs);


            List<string> listOfAreaNames = new List<string>();
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);

            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);
            //GetCurrentSpaceStatusForView

            return View("MobileDrillDown", modelForView);
        }

        // GET: /SpaceStatus/SpaceDrillDown

        public ActionResult SpaceDrillDown(string CID, string targetID)
        {
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // DEBUG: Just testing...
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(targetID));

            ViewData["MeterList"] = new SelectList(listOfMeterIDs);

            ViewData["CurrentMeter"] = targetID;

            /*
            List<string> listOfAreaNames = new List<string>();
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetCustomerAreaAssets(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);
            */

            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            return View("SpaceDrillDown", modelForView);
        }

        // GET: /SpaceStatus/MobileSpaceDetails

        public ActionResult MobileSpaceDetails(string CID, string targetID, string MID)
        {

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // DEBUG: Just testing...
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfSpaceNumbers = new List<int>();
            listOfSpaceNumbers.Add(Convert.ToInt32(MID));

            ViewData["SpaceList"] = new SelectList(listOfSpaceNumbers);

            ViewData["CurrentSpace"] = targetID;

            /*
            List<string> listOfAreaNames = new List<string>();
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetCustomerAreaAssets(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);
            */

            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info -- but since this is for mobile occupancy view, we don't need to waste time gathering PAM data...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfSpaceNumbers, false, false, result);

            return View("MobileSpaceDetails", modelForView);
        }

        #endregion // Mobile Drill-Down Views


        #region Desktop/Mobile Summary Views

        // Group Summary (Desktop view)
        // GET: /SpaceStatus/GrpSummary
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>
        /// 

        public ActionResult SpaceStatusbyArea()
        {
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            result.GatherAllInventory();

            if (result != null)
            {
                if (Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.ContainsKey(result.CustomerId) == false)
                    Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.Add(result.CustomerId, result);
            }
            string G = "A";
            string V = "L";
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterName;
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }

            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("GrpSummary_WinCE", modelForView);
            else
                return View("GrpSummary", modelForView);
            /*
            if (this.Request.UserAgent.IndexOf("windows ce", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                ViewData["IsWinCE"] = true;
                return View("GrpSummary_WinCE", modelForView);
            }
            else
            {
                ViewData["IsWinCE"] = false;
                return View("GrpSummary", modelForView);
            }
            */
        }
        public ActionResult SpaceStatusbyMeter()
        {
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            result.GatherAllInventory();

            if (result != null)
            {
                if (Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.ContainsKey(result.CustomerId) == false)
                    Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.Add(result.CustomerId, result);
            }
            string G = "M";
            string V = "L";
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterName;
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }

            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("GrpSummary_WinCE", modelForView);
            else
                return View("GrpSummary", modelForView);
            /*
            if (this.Request.UserAgent.IndexOf("windows ce", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                ViewData["IsWinCE"] = true;
                return View("GrpSummary_WinCE", modelForView);
            }
            else
            {
                ViewData["IsWinCE"] = false;
                return View("GrpSummary", modelForView);
            }
            */
        }
        public ActionResult GrpSummary(string G, string V)
        {
            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area:";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter:";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("GrpSummary_WinCE", modelForView);
            else
                return View("GrpSummary", modelForView);
            /*
            if (this.Request.UserAgent.IndexOf("windows ce", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                ViewData["IsWinCE"] = true;
                return View("GrpSummary_WinCE", modelForView);
            }
            else
            {
                ViewData["IsWinCE"] = false;
                return View("GrpSummary", modelForView);
            }
            */
        }

        // Group Summary (Partial View for desktop view contents)
        // GET: /SpaceStatus/GrpSummaryPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>


        public ActionResult GrpSummaryPartial(string G, string V, List<SpaceStatusModel> modelForView)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;
            /*
            if (this.Request.UserAgent.IndexOf("windows ce", StringComparison.InvariantCultureIgnoreCase) != -1)
                ViewData["IsWinCE"] = true;
            else
                ViewData["IsWinCE"] = false;
            */

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Build list of choices based on the group type
            if (groupType == "A")
            {
            }
            else if (groupType == "M")
            {
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;


            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider((ViewData["CustomerCfg"] as CustomerConfig));
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer((ViewData["CustomerCfg"] as CustomerConfig)))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sensing and PAM data)
            if (modelForView == null)
                modelForView = space.GetCurrentSpaceStatusForView((ViewData["CustomerCfg"] as CustomerConfig).CustomerId, listOfMeterIDs, true, false, result);

            return View("pv_GrpSummaryContent", modelForView);
        }


        // Space Summary (Desktop view)
        // GET: /SpaceStatus/SpcSummary
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="DS">DisplayStyle. Values are D = Dynamic, 2 = "2-row" (Graphic + text), 1 = "1-row" (Text-only)</param>
        /// <param name="F">Filter.</param>
        /// <returns></returns>

        public ActionResult SpcSummary(string T, string G, string V, string DS, string F)
        {
            // Retain original target
            string originalTarget = T;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "A"; // Default to "A" (All, or no filtering)
            ViewData["filter"] = filter;

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FilterDisplay"] = "(Showing all spaces)";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied spaces)";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FilterDisplay"] = "(Showing violations)";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied or violations)";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FilterDisplay"] = "(Showing vacant spaces)";

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);

                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (spcAsset.MeterID == mtrAsset.MeterID)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                   // choice.DataText = spcAsset.MeterID.ToString();
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                    //choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));

                // Find all SpaceIDs that are associated with the current meter
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.MeterID.ToString(), T) == 0)
                    {
                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (string.Compare(spcAsset.MeterID.ToString(), T) == 0)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                   // choice.DataText = spcAsset.MeterID.ToString();
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                  //  choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // If we ended up with only 1 choice, we might as well skip this summary level and go directly to the space detail screen instead
            if (groupChoices.Count == 1)
            {
                GenericObjForHTMLSelectList onlyChoice = groupChoices[0];

                // Build a RouteValueDictionary for redirecting to detail page
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", onlyChoice.DataValue);
                routeValues.Add("PT", originalTarget);
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());

                // Now redirect to space details view
                return RedirectToAction("SpcDetail", routeValues);
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces associated with our grouping
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("SpcSummary_WinCE", modelForView);
            else
                return View("SpcSummary", modelForView);
        }

        // Space Summary (Partial View for desktop view contents)
        // GET: /SpaceStatus/SpcSummaryPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="DS"></param>DisplayStyle. Values are D = Dynamic, 2 = "2-row" (Graphic + text), 1 = "1-row" (Text-only)</param>
        /// <param name="F">Filter.</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult SpcSummaryPartial(string T, string G, string V, string DS, string F, List<SpaceStatusModel> modelForView)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // Retain original target
            string originalTarget = T;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "A"; // Default to "A" (All, or no filtering)
            ViewData["filter"] = filter;

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FilterDisplay"] = "(Showing all spaces)";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied spaces)";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FilterDisplay"] = "(Showing violations)";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied or violations)";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FilterDisplay"] = "(Showing vacant spaces)";

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // We will need to gather current info for all meters and spaces associated with our grouping
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            // Get current space info for enforcement views (include vehicle sensing and PAM data)
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            if (modelForView == null)
                modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("pv_SpcSummaryContent", modelForView);
        }


        // Space Detail (Desktop view)
        // GET: /SpaceStatus/SpcDetail
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>

        public ActionResult SpcDetail(string T, string PT, string G, string V)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            string groupChoiceDefaultSelection = null;
            int SelectedIndex = 0;
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current area
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (spcAsset.MeterID == mtrAsset.MeterID)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                        //choice.DataText = spcAsset.MeterID.ToString();
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        //choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if ((string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0) &&
                                        (string.Compare(MID, spcAsset.MeterID.ToString()) == 0))
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current meter
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.MeterID.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (string.Compare(spcAsset.MeterID.ToString(), PT) == 0)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                       // choice.DataText = spcAsset.MeterID.ToString();
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        //choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if (string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0)
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText", groupChoiceDefaultSelection);
            ViewData["SelectedIndex"] = SelectedIndex;

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("SpcDetail_WinCE", modelForView);
            else
                return View("SpcDetail", modelForView);
        }

        // Space Detail (Partial View content for desktop view)
        // GET: /SpaceStatus/SpcDetailPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses


        public ActionResult SpcDetailPartial(string T, string PT, string G, string V)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;

            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            return View("pv_SpcDetailContent", modelForView);
        }


        // Space Summary Options (Desktop view)
        // GET: /SpaceStatus/SpcSummaryOptions
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="DS">DisplayStyle. Values are D = Dynamic, 2 = "2-row" (Graphic + text), 1 = "1-row" (Text-only)</param>
        /// <param name="F">Filter. </param>
        /// <returns></returns>

        public ActionResult SpcSummaryOptions(string T, string G, string V, string DS, string F)
        {
            // Retain original target
            string originalTarget = T;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "D" (Dynamic, with normally is graphic + text, but becomes text-only on small devices)
            ViewData["displayStyle"] = displayStyle;

            if (string.Compare(displayStyle, "1", true) == 0)
            {
                ViewData["DSBTN1ID"] = "";
                ViewData["DSBTN2ID"] = "";
                ViewData["DSBTN3ID"] = " id=\"DSRBtnChoiceDefault\""; // 1-Row option will be default
            }
            else if (string.Compare(displayStyle, "2", true) == 0)
            {
                ViewData["DSBTN1ID"] = "";
                ViewData["DSBTN2ID"] = " id=\"DSRBtnChoiceDefault\""; // 2-Row option will be default
                ViewData["DSBTN3ID"] = "";
            }
            else
            {
                ViewData["DSBTN1ID"] = " id=\"DSRBtnChoiceDefault\""; // Dynamic option will be default
                ViewData["DSBTN2ID"] = "";
                ViewData["DSBTN3ID"] = "";
            }

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "A"; // Default to "A", for All (no filtering)
            ViewData["filter"] = filter;
            ViewData["FBTN1ID"] = "";
            ViewData["FBTN2ID"] = "";
            ViewData["FBTN3ID"] = "";
            ViewData["FBTN4ID"] = "";
            ViewData["FBTN5ID"] = "";

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FBTN1ID"] = " id=\"FRBtnChoiceDefault\"";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FBTN2ID"] = " id=\"FRBtnChoiceDefault\"";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FBTN3ID"] = " id=\"FRBtnChoiceDefault\"";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FBTN4ID"] = " id=\"FRBtnChoiceDefault\"";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FBTN5ID"] = " id=\"FRBtnChoiceDefault\"";

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FilterDisplay"] = "(Showing all spaces)";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied spaces)";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FilterDisplay"] = "(Showing violations)";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied or violations)";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FilterDisplay"] = "(Showing vacant spaces)";

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("SpcSummaryOptions_WinCE");
            else
                return View("SpcSummaryOptions");
        }

        // Space Summary Options (Desktop view)
        // POST: /SpaceStatus/SpcSummaryOptions
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="DS">DisplayStyle. Values are D = Dynamic, 2 = "2-row" (Graphic + text), 1 = "1-row" (Text-only)</param>
        /// <param name="F">Filter. </param>
        /// <returns></returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SpcSummaryOptions(FormCollection formValues, string T, string G, string V)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // Retain original target
            string originalTarget = T;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            // Determine which display style the user chose
            string displayStyle = "D"; //D=Dynamcic, 2=2-row, 1=1-row
            try
            {
                if (!string.IsNullOrEmpty(formValues["DSRBtn"]))
                {
                    string selectedActivityRestriction = formValues["DSRBtn"];
                    if (selectedActivityRestriction == "Dynamic")
                        displayStyle = "D";
                    else if (selectedActivityRestriction == "2Row")
                        displayStyle = "2";
                    else if (selectedActivityRestriction == "1Row")
                        displayStyle = "1";
                    else
                        displayStyle = "D";
                }
            }
            catch { }
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style the user chose
            string filter = "A"; // Default to "A", for "All (no filtering)
            try
            {
                if (!string.IsNullOrEmpty(formValues["FRBtn"]))
                {
                    string selectedFilter = formValues["FRBtn"];
                    if (selectedFilter == "All")
                        filter = "A";
                    else if (selectedFilter == "Occupied")
                        filter = "O";
                    else if (selectedFilter == "Violations")
                        filter = "E";
                    else if (selectedFilter == "OccupiedOrViolations")
                        filter = "OE";
                    else if (selectedFilter == "Vacant")
                        filter = "V";
                    else
                        filter = "A";
                }
            }
            catch { }
            ViewData["filter"] = filter;

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FilterDisplay"] = "(Showing all spaces)";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied spaces)";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FilterDisplay"] = "(Showing violations)";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied or violations)";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FilterDisplay"] = "(Showing vacant spaces)";

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            /*return View("SpcSummaryOptions");*/

            RouteValueDictionary routeValues = new RouteValueDictionary();
            routeValues.Add("T", originalTarget);
            routeValues.Add("G", G);
            routeValues.Add("V", V);
            routeValues.Add("CID", customerDto.CustomerId.ToString());
            routeValues.Add("DS", displayStyle);
            routeValues.Add("F", filter);

            // Now redirect to space summary view, with the options that were chosen
            return RedirectToAction("SpcSummary", routeValues);
        }

        #endregion // Desktop/Mobile Summary Views


        #region Mobile Overstay Violation Enforcement Views

        // Overstay Violation Group Summary
        // GET: /SpaceStatus/StayVioGrpSum
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>


        public ActionResult OverstayEnforcement()
        {
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            result.GatherAllInventory();

            if (result != null)
            {
                if (Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.ContainsKey(result.CustomerId) == false)
                    Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.Add(result.CustomerId, result);
            }

            string G = "A";
            string V = "L";
            string DS = "";
            string F = "E";

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");

            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area:";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter:";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }

            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("StayVioGrpSum", modelForView);
            else
                return View("StayVioGrpSum", modelForView);
        }

        public ActionResult StayVioGrpSum(string G, string V, string DS, string F)
        {
            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            // Build list of choices based on the group type
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Area:";
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "a" + asset.AreaID.ToString();
                    if (!string.IsNullOrEmpty(asset.AreaName))
                        choice.DataText = asset.AreaName;
                    else
                        choice.DataText = asset.AreaID.ToString();
                    groupChoices.Add(choice);
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Meter:";
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                    choice.DataValue = "m" + asset.MeterID.ToString();
                    choice.DataText = asset.MeterName;
                    groupChoices.Add(choice);
                }
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("StayVioGrpSum", modelForView);
            else
                return View("StayVioGrpSum", modelForView);
        }

        // Overstay Violation Group Summary (Partial View for ajax contents)
        // GET: /SpaceStatus/StayVioGrpSumPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult StayVioGrpSumPartial(string G, string V, string DS, string F, List<SpaceStatusModel> modelForView)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;
            /*
            if (this.Request.UserAgent.IndexOf("windows ce", StringComparison.InvariantCultureIgnoreCase) != -1)
                ViewData["IsWinCE"] = true;
            else
                ViewData["IsWinCE"] = false;
            */

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            // Build list of choices based on the group type
            if (groupType == "A")
            {
            }
            else if (groupType == "M")
            {
            }
            else
            {
                throw new Exception("(" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + ") " +
                    "Support for group type '" + groupType + "' not implemented.");
            }

            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sensing and PAM data)
            if (modelForView == null)
                modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("pv_StayVioGrpSumContent", modelForView);
        }


        // GET: /SpaceStatus/StayVioSpcSum
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="DS">DisplayStyle. Values are D = Dynamic, 2 = "2-row" (Graphic + text), 1 = "1-row" (Text-only)</param>
        /// <param name="F">Filter.</param>
        /// <returns></returns>

        public ActionResult StayVioSpcSum(string T, string G, string V, string DS, string F)
        {
            // Retain original target
            string originalTarget = T;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FilterDisplay"] = "(Showing all spaces)";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied spaces)";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FilterDisplay"] = "(Showing violations)";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied or violations)";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FilterDisplay"] = "(Showing vacant spaces)";

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);

                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (spcAsset.MeterID == mtrAsset.MeterID)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                    // choice.DataText = spcAsset.MeterID.ToString();
                                    choice.DataText = mtrAsset.MeterName;
                                else
                                    //choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));

                // Find all SpaceIDs that are associated with the current meter
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.MeterID.ToString(), T) == 0)
                    {
                        foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                        {
                            if (string.Compare(spcAsset.MeterID.ToString(), T) == 0)
                            {
                                GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                if (mtrAsset.MeterGroupID <= 0) // SSM
                                   //choice.DataText = spcAsset.MeterID.ToString();
                                    choice.DataText = spcAsset.MeterName;
                                else
                                   // choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                    choice.DataText = spcAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                groupChoices.Add(choice);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // If we ended up with only 1 choice, we might as well skip this summary level and go directly to the space detail screen instead
            if (groupChoices.Count == 1)
            {
                GenericObjForHTMLSelectList onlyChoice = groupChoices[0];

                // Build a RouteValueDictionary for redirecting to detail page
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", onlyChoice.DataValue);
                routeValues.Add("PT", originalTarget);
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());

                // Now redirect to space details view
                return RedirectToAction("SpcDetail", routeValues);
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText");

            // We will need to gather current info for all meters and spaces associated with our grouping
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("StayVioSpcSum", modelForView);
            else
                return View("StayVioSpcSum", modelForView);
        }

        // GET: /SpaceStatus/StayVioSpcSumPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="DS"></param>DisplayStyle. Values are D = Dynamic, 2 = "2-row" (Graphic + text), 1 = "1-row" (Text-only)</param>
        /// <param name="F">Filter.</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult StayVioSpcSumPartial(string T, string G, string V, string DS, string F, List<SpaceStatusModel> modelForView)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // Retain original target
            string originalTarget = T;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            if (string.Compare(filter, "A", true) == 0)
                ViewData["FilterDisplay"] = "(Showing all spaces)";
            else if (string.Compare(filter, "O", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied spaces)";
            else if (string.Compare(filter, "E", true) == 0)
                ViewData["FilterDisplay"] = "(Showing violations)";
            else if (string.Compare(filter, "OE", true) == 0)
                ViewData["FilterDisplay"] = "(Showing occupied or violations)";
            else if (string.Compare(filter, "V", true) == 0)
                ViewData["FilterDisplay"] = "(Showing vacant spaces)";

            // Determine which type of grouping to use, and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            // Retain current target as a "parentTarget" in ViewData, because it might get passed to the next drill-down level
            ViewData["parentTarget"] = T;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            if (groupType == "A")
            {
                if (T.StartsWith("a"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Area:";

                // Resolve the area asset so we can display Name instead of ID
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (string.Compare(asset.AreaID.ToString(), T) == 0)
                    {
                        if (!string.IsNullOrEmpty(asset.AreaName))
                            ViewData["groupID"] = asset.AreaName;
                        else
                            ViewData["groupID"] = T;
                    }
                }
            }
            else if (groupType == "M")
            {
                if (T.StartsWith("m"))
                    T = T.Remove(0, 1);

                ViewData["groupTypeName"] = "Meter:";
                ViewData["groupID"] = T;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;


            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by T (target) and G (group type) parameters.
            // DEBUG: For SSM or non-metered spaces, maybe we should be using the MeterID instead of Bay Number?
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            List<int> listOfMeterIDs = new List<int>();

            if (groupType == "A")
            {
                ViewData["groupPrompt"] = "Space:";

                // Find all SpaceIDs that are associated with the current area
                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), T) == 0)
                    {
                        listOfMeterIDs.Add(mtrAsset.MeterID);
                    }
                }
            }
            else if (groupType == "M")
            {
                ViewData["groupPrompt"] = "Space:";
                listOfMeterIDs.Add(Convert.ToInt32(T));
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // We will need to gather current info for all meters and spaces associated with our grouping
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            // Get current space info for enforcement views (include vehicle sensing and PAM data)
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            if (modelForView == null)
                modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, false, result);

            return View("pv_StayVioSpcSumContent", modelForView);
        }

        // GET: /SpaceStatus/StayVioSpcAction
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>

        public ActionResult StayVioSpcAction(string T, string PT, string G, string V, string DS, string F, string ReturnAction)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            if (string.IsNullOrEmpty(ReturnAction))
                ReturnAction = "StayVioSpcSum";

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of action taken choices
            List<GenericObjForHTMLSelectList> actionChoices = new List<GenericObjForHTMLSelectList>();

            GenericObjForHTMLSelectList actionChoice = new GenericObjForHTMLSelectList();
            actionChoice.DataValue = "Enforced";
            actionChoice.DataText = "Enforced";
            actionChoices.Add(actionChoice);

            actionChoice = new GenericObjForHTMLSelectList();
            actionChoice.DataValue = "Cautioned";
            actionChoice.DataText = "Cautioned Driver";
            actionChoices.Add(actionChoice);

            actionChoice = new GenericObjForHTMLSelectList();
            actionChoice.DataValue = "NotEnforced";
            actionChoice.DataText = "Not Enforced";
            actionChoices.Add(actionChoice);

            actionChoice = new GenericObjForHTMLSelectList();
            actionChoice.DataValue = "Fault";
            actionChoice.DataText = "Fault";
            actionChoices.Add(actionChoice);

            // Add the group choices to the view data
            ViewData["actionChoices"] = new SelectList(actionChoices, "DataValue", "DataText", "Enforced");

            string returnURL = "";
            if (string.Compare(ReturnAction, "StayVioSpcSum", true) == 0)
            {
                //returnURL = Url.Content("~/SpaceStatus/StayVioSpcSum") +
                returnURL = Url.Action("StayVioSpcSum", "SpaceStatus") +
                     "?T=" + Url.Encode(Convert.ToString(this.ViewData["parentTarget"])) +
                     "&G=" + Url.Encode(Convert.ToString(this.ViewData["groupType"])) +
                     "&V=" + Url.Encode(Convert.ToString(this.ViewData["viewType"])) +
                     "&CID=" + Url.Encode((this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString()) +
                     "&DS=" + Url.Encode(Convert.ToString(this.ViewData["displayStyle"])) +
                     "&F=" + Url.Encode(Convert.ToString(this.ViewData["filter"]));
            }
            else if (string.Compare(ReturnAction, "StayVioSpcDetail", true) == 0)
            {
                //returnURL = Url.Content("~/SpaceStatus/StayVioSpcDetail") +
                returnURL = Url.Action("StayVioSpcDetail", "SpaceStatus") +
                     "?T=" + Url.Encode(Convert.ToString(this.ViewData["originalTarget"])) +
                     "&PT=" + Url.Encode(Convert.ToString(this.ViewData["parentTarget"])) +
                     "&G=" + Url.Encode(Convert.ToString(this.ViewData["groupType"])) +
                     "&V=" + Url.Encode(Convert.ToString(this.ViewData["viewType"])) +
                     "&CID=" + Url.Encode((this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString()) +
                     "&DS=" + Url.Encode(Convert.ToString(this.ViewData["displayStyle"])) +
                     "&F=" + Url.Encode(Convert.ToString(this.ViewData["filter"]));
            }

            ViewData["returnURL"] = returnURL;

            // Now that we are integrated with the issuance application on PDA, we do need to gather vehicle sensor data now
            /*
            // This method is only used for assigning an "Action Taken" enforcement status to a space, so we don't actually
            // need to gather current PAM or Vehicle sensor data. 
            List<SpaceStatusModel> modelForView = new List<SpaceStatusModel>();
            SpaceStatusModel spaceModel = new SpaceStatusModel();
            modelForView.Add(spaceModel);
            spaceModel.BayID = Convert.ToInt32(SNUM);
            spaceModel.MeterID = Convert.ToInt32(MID);
            */

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            if (modelForView.Count > 0)
            {
                ViewData["ARRIVAL"] = modelForView[0].BayVehicleSensingTimestamp.ToString("yyyyMMddHHmmss");

                OverstayViolationInfo overstayInfo = modelForView[0].CurrentOverstayOrLatestDiscretionaryVio;
                if (overstayInfo != null)
                {
                    string regulationTxt = string.Empty;
                    if (string.Compare(overstayInfo.Regulation_Type, "No Parking", true) == 0)
                        regulationTxt = "NO PARKING";
                    else
                        regulationTxt = overstayInfo.Regulation_MaxStayMinutes.ToString() + " MINS";

                    regulationTxt = regulationTxt + " " + ((DayOfWeek)(overstayInfo.Regulation_DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                         overstayInfo.Regulation_StartTime.ToString("hh:mm:ss tt") + " - " +
                         overstayInfo.Regulation_EndTime.ToString("hh:mm:ss tt");

                    ViewData["REGULATIONS"] = regulationTxt;
                }
            }


            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("StayVioSpcAction", modelForView);
            else
                return View("StayVioSpcAction", modelForView);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StayVioSpcAction(FormCollection formValues, string T, string PT, string G, string V, string DS, string F, string ReturnAction)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            if (string.IsNullOrEmpty(ReturnAction))
                ReturnAction = "StayVioSpcSum";

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;

            // Need to update Enforcement Action in SQLite database
            string selectedAction = formValues["ActionSelect"];
            if (!string.IsNullOrEmpty(selectedAction))
            {
                // Gather info we need for inserting info in the database
                int bayNumber = Convert.ToInt32(SNUM);
                int meterID = Convert.ToInt32(MID);
                DateTime NowAtDestination = Convert.ToDateTime(customerDto.DestinationTimeZoneDisplayName);// UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, customerDto.ServerTimeZone, customerDto.CustomerTimeZone);

                SpaceAsset asset = CustomerLogic.CustomerManager.GetSpaceAsset(customerDto, meterID, bayNumber);
                int areaID = 0;
                if (asset != null)
                    areaID = asset.AreaID_Internal;
                //MvcApplication.RBAC.RoleProvider = new RBACProvider.RBACRoleProvider();
                //  RBACProvider.RBACUserInfo rbacUser = MvcApplication.RBAC.RoleProvider.GetRBACUserFromCacheOrDB(this.User.Identity.Name);
                int RBACUserID = 0;
                //  if (rbacUser != null)
                RBACUserID = WebSecurity.CurrentUserId;//rbacUser.DBUserCustomSID_AsInt;

                CustomerLogic result = new CustomerLogic();
                result.CustomerId = CurrentCity.Id;
                string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
                result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);



                // Insert the action taken into the SqLite database
                result.InsertOverstayVioAction(customerDto.CustomerId, meterID, areaID, bayNumber, RBACUserID, NowAtDestination, selectedAction);
            }

            if (string.Compare(ReturnAction, "StayVioSpcSum", true) == 0)
            {
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", Convert.ToString(ViewData["parentTarget"]));
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());
                routeValues.Add("DS", displayStyle);
                routeValues.Add("F", filter);

                // Now redirect to space summary view, with the options that were chosen
                return RedirectToAction("StayVioSpcSum", routeValues);
            }
            else if (string.Compare(ReturnAction, "StayVioSpcDetail", true) == 0)
            {
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", Convert.ToString(ViewData["originalTarget"]));
                routeValues.Add("PT", Convert.ToString(ViewData["parentTarget"]));
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());
                routeValues.Add("DS", displayStyle);
                routeValues.Add("F", filter);

                // Now redirect to space summary view, with the options that were chosen
                return RedirectToAction("StayVioSpcDetail", routeValues);
            }
            else
            {
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add("T", Convert.ToString(ViewData["parentTarget"]));
                routeValues.Add("G", G);
                routeValues.Add("V", V);
                routeValues.Add("CID", customerDto.CustomerId.ToString());
                routeValues.Add("DS", displayStyle);
                routeValues.Add("F", filter);

                // Now redirect to space summary view, with the options that were chosen
                return RedirectToAction("StayVioSpcSum", routeValues);
            }
        }


        // GET: /SpaceStatus/StayVioSpcDetail
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>

        public ActionResult StayVioSpcDetail(string T, string PT, string G, string V, string DS, string F)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            string groupChoiceDefaultSelection = null;
            int SelectedIndex = 0;
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current area
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.AreaID_PreferLibertyBeforeInternal.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (spcAsset.MeterID == mtrAsset.MeterID)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                        //choice.DataText = spcAsset.MeterID.ToString();
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                        //choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                        choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if ((string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0) &&
                                        (string.Compare(MID, spcAsset.MeterID.ToString()) == 0))
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";

                    // Find all SpaceIDs that are associated with the current meter
                    foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(mtrAsset.MeterID.ToString(), PT) == 0)
                        {
                            foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                            {
                                if (string.Compare(spcAsset.MeterID.ToString(), PT) == 0)
                                {
                                    GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                                    choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                                    if (mtrAsset.MeterGroupID <= 0) // SSM
                                       // choice.DataText = spcAsset.MeterID.ToString();
                                        choice.DataText = mtrAsset.MeterName;
                                    else
                                      //  choice.DataText = spcAsset.MeterID.ToString() + "-" + spcAsset.SpaceID.ToString();
                                    choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                                    groupChoices.Add(choice);

                                    // Should this be our default selection for the combobox?
                                    if (string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0)
                                    {
                                        groupChoiceDefaultSelection = choice.DataValue;
                                        SelectedIndex = groupChoices.Count - 1;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText", groupChoiceDefaultSelection);
            ViewData["SelectedIndex"] = SelectedIndex;

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sensing and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, true, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            if (IsWinCE)
                return View("StayVioSpcDetail", modelForView);
            else
                return View("StayVioSpcDetail", modelForView);
        }

        // GET: /SpaceStatus/StayVioSpcDetailPartial
        /// <summary>
        /// Input parameters are cryptic to keep URLs and URL query parameters smaller, for bandwidth reasons
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="PT">Parent Target. Value is either an AreaID or MeterID, prefixed with letter 'a' or 'm', respectively</param>
        /// <param name="G">GroupType. Recognized values are A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute </param>
        /// <param name="V">ViewType. Recognized values are L=List, T=Tile</param>
        /// <param name="MID">MeterID. MeterID associated with the Target space number (for disambiguation)</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult StayVioSpcDetailPartial(string T, string PT, string G, string V, string DS, string F)
        {
            bool IsWinCE = IsRequestFromWindowsMobileDevice();
            ViewData["IsWinCE"] = IsWinCE;

            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            int loIdxStart = T.IndexOf('m') + 1;
            int loIdxEnd = T.IndexOf('s');
            MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
            loIdxStart = loIdxEnd + 1;
            SNUM = T.Substring(loIdxStart);

            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;

            // Determine the display style, and retain in view data
            var displayStyle = DS;
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (graphic + text)
            ViewData["displayStyle"] = displayStyle;

            // Determine the filter style, and retain in view data
            var filter = F;
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable)
            ViewData["filter"] = filter;

            // Retain the verbatim passed Parent Target (PT) in the ViewData
            ViewData["parentTarget"] = PT;
            ViewData["originalTarget"] = T;

            // Determine which type of view mode to use, and retain in view data
            var viewType = V;
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared
            ViewData["viewType"] = viewType;

            // Init group info from Parent target
            ViewData["groupTypeName"] = string.Empty;
            ViewData["groupID"] = string.Empty;

            // Determine which type of grouping to use, based on Parent Target (PT), and retain in view data
            var groupType = G;
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared
            ViewData["groupType"] = groupType;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            if (!string.IsNullOrEmpty(PT))
            {
                if (groupType == "A")
                {
                    if (PT.StartsWith("a"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Area:";

                    // Resolve the area asset so we can display Name instead of ID
                    foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                    {
                        if (string.Compare(asset.AreaID.ToString(), PT) == 0)
                        {
                            if (!string.IsNullOrEmpty(asset.AreaName))
                                ViewData["groupID"] = asset.AreaName;
                            else
                                ViewData["groupID"] = PT;
                        }
                    }
                }
                else if (groupType == "M")
                {
                    if (PT.StartsWith("m"))
                        PT = PT.Remove(0, 1);

                    ViewData["groupTypeName"] = "Meter:";
                    ViewData["groupID"] = PT;
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // Build list of choices, which are the spaces associated with the passed AreaID or MeterID, as determined by PT (parent target) and G (group type) parameters.
            if (!string.IsNullOrEmpty(ViewData["groupID"].ToString()))
            {
                if (groupType == "A")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else if (groupType == "M")
                {
                    ViewData["groupPrompt"] = "Space:";
                }
                else
                {
                    // DEBUG: We don't want to throw exception when parent group type isn't available -- we just won't include the related portions in the rendered HTML
                    /*
                    throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    */
                }
            }

            // We will need to gather current info for the given meter and space number
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            listOfMeterIDs.Add(Convert.ToInt32(MID));
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info for enforcement views (include vehicle sening and PAM data)
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, true, true, result);

            // Weed out any results that aren't for the requested space (When dealing with Multi-space meters, we should expect to have related
            // space information that needs to be weeded out because its not for the exact space number we are reporting details on)
            int BayID = Convert.ToInt32(SNUM);
            for (int loIdx = modelForView.Count - 1; loIdx >= 0; loIdx--)
            {
                if (modelForView[loIdx].BayID != BayID)
                    modelForView.RemoveAt(loIdx);
            }

            return View("pv_StayVioSpcDetailContent", modelForView);
        }

        #endregion //Mobile Overstay Violation Enforcement Views



        #region Reports

        private List<int> PopulateCommonReportParametersFromFormAndGetListOfMeterIds(FormCollection formValues, SensorAndPaymentReportEngine.CommonReportParameters repParams)
        {
            List<int> listOfMeterIDs = new List<int>();

            // Get the user's selected Start and End date parameters from the formValues
            DateTime reportStartDate = DateTime.MinValue;
            reportStartDate = Convert.ToDateTime(formValues["startDate"]);

            DateTime reportEndDate = DateTime.MinValue;
            reportEndDate = Convert.ToDateTime(formValues["endDate"]);

            string scopedAreaName = string.Empty;
            string scopedMeter = string.Empty;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");

            // We need to generate a list of all MeterIDs that will be included in the report.
            // The "Scope" parameter might limit the reportable meters to an area or specific MeterID, etc.
            // First, we need to see which "scope" the user selected
            string selectedScope = formValues["ScopeRBtn"];
            if (selectedScope == "pnlArea")
            {
                // User chose to scope the report to a specific Area, we need to resolve the AreaID for the
                // selected area name, and then find all MeterIDs that are associated with the area

                // Get the selected Area Name, then resolve to an Area ID
                string selectedAreaName = formValues["AreaSelect"];
                scopedAreaName = selectedAreaName;
                int selectedAreaID = -1;
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (selectedAreaName == asset.AreaName)
                    {
                        selectedAreaID = asset.AreaID;
                        break;
                    }
                }

                // Get the Meter IDs that are applicable to the chosen area
                if (selectedAreaID != -1)
                {
                    foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (asset.AreaID_PreferLibertyBeforeInternal == selectedAreaID)
                            listOfMeterIDs.Add(asset.MeterID);
                    }
                }
            }
            else if (selectedScope == "pnlMeter")
            {
                // User chose to scope the report to a specific MeterID
                if (!string.IsNullOrEmpty(formValues["MeterSelect"]))
                {
                    scopedMeter = formValues["MeterSelect"];
                    listOfMeterIDs.Add(Convert.ToInt32(formValues["MeterSelect"]));
                }
            }
            else if (selectedScope == "pnlSiteWide")
            {
                // User chose to scope the report to entire site, so get a list of all Meter IDs for this customer
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    listOfMeterIDs.Add(asset.MeterID);
                }
            }

            // Determine which sections of the report the user want to include (from checkbox options)
            bool includeHourlyStatistics = false;
            bool includeMeterSummary = false;
            bool includeAreaSummary = false;
            bool includeSpaceSummary = false;
            bool includeDailySummary = false;
            bool includeMonthlySummary = false;
            bool includeDetailRecords = false;
            string selectedContentToIncludeCSV = formValues["chkRptContent"];

            if (!string.IsNullOrEmpty(selectedContentToIncludeCSV))
            {
                string[] arrayOfContentToInclude = selectedContentToIncludeCSV.Split(new char[] { ',' });
                foreach (string nextContentToInclude in arrayOfContentToInclude)
                {
                    if (string.Compare("inclHourlyStats", nextContentToInclude.Trim(), true) == 0)
                        includeHourlyStatistics = true;
                    if (string.Compare("inclAreaSum", nextContentToInclude.Trim(), true) == 0)
                        includeAreaSummary = true;
                    if (string.Compare("inclMeterSum", nextContentToInclude.Trim(), true) == 0)
                        includeMeterSummary = true;
                    if (string.Compare("inclSpaceSum", nextContentToInclude.Trim(), true) == 0)
                        includeSpaceSummary = true;
                    if (string.Compare("inclDailySum", nextContentToInclude.Trim(), true) == 0)
                        includeDailySummary = true;
                    if (string.Compare("inclMonthlySum", nextContentToInclude.Trim(), true) == 0)
                        includeMonthlySummary = true;
                    if (string.Compare("inclDetailRecords", nextContentToInclude.Trim(), true) == 0)
                        includeDetailRecords = true;
                    if (string.Compare("All", nextContentToInclude.Trim(), true) == 0)
                    {
                        includeHourlyStatistics = true;
                        includeAreaSummary = true;
                        includeMeterSummary = true;
                        includeSpaceSummary = true;
                        includeDailySummary = true;
                        includeMonthlySummary = true;
                        includeDetailRecords = true;
                    }
                }
            }
            else
            {
                includeHourlyStatistics = true;
                includeAreaSummary = true;
                includeMeterSummary = true;
                includeSpaceSummary = true;
                includeDailySummary = true;
                includeMonthlySummary = true;
                includeDetailRecords = true;

            }



            // Populate the report parameters
            string selectedActivityRestriction = formValues["ActRestrictRBtn"];
            if (selectedActivityRestriction == "All")
                repParams.RegulatedHoursRestrictionFilter = SensorAndPaymentReportEngine.RegulatedHoursRestrictions.AllActivity;
            else if (selectedActivityRestriction == "Regulated")
                repParams.RegulatedHoursRestrictionFilter = SensorAndPaymentReportEngine.RegulatedHoursRestrictions.OnlyDuringRegulatedHours;
            else if (selectedActivityRestriction == "Unregulated")
                repParams.RegulatedHoursRestrictionFilter = SensorAndPaymentReportEngine.RegulatedHoursRestrictions.OnlyDuringUnregulatedHours;

            // DEBUG: We are explicitly setting this for all activity -- someday we might want to add this param to the GUI, but not yet
            repParams.ActionTakenRestrictionFilter = SensorAndPaymentReportEngine.ReportableEnforcementActivity.AllActivity;

            repParams.StartTime = reportStartDate;
            repParams.EndTime = reportEndDate;

            repParams.IncludeAreaSummary = includeAreaSummary;
            repParams.IncludeMeterSummary = includeMeterSummary;
            repParams.IncludeSpaceSummary = includeSpaceSummary;
            repParams.IncludeHourlyStatistics = includeHourlyStatistics;
            repParams.IncludeDailySummary = includeDailySummary;
            repParams.IncludeMonthlySummary = includeMonthlySummary;
            repParams.IncludeDetailRecords = includeDetailRecords;

            repParams.ScopedAreaName = scopedAreaName;
            repParams.ScopedMeter = scopedMeter;

            return listOfMeterIDs;
        }


        private List<int> PopulateCommonReportParametersFromFormAndGetListOfMeterIds1(FormCollection formValues, SensorAndPaymentReportEngine.CommonReportParameters repParams)
        {
            List<int> listOfMeterIDs = new List<int>();

            // Get the user's selected Start and End date parameters from the formValues
            DateTime reportStartDate = DateTime.MinValue;
            reportStartDate = Convert.ToDateTime(formValues["startDate"]);

            DateTime reportEndDate = DateTime.MinValue;
            reportEndDate = Convert.ToDateTime(formValues["endDate"]);

            string scopedAreaName = string.Empty;
            string scopedMeter = string.Empty;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");

            // We need to generate a list of all MeterIDs that will be included in the report.
            // The "Scope" parameter might limit the reportable meters to an area or specific MeterID, etc.
            // First, we need to see which "scope" the user selected
            string selectedScope = formValues["ScopeRBtn"];
            if (selectedScope == "pnlArea")
            {
                // User chose to scope the report to a specific Area, we need to resolve the AreaID for the
                // selected area name, and then find all MeterIDs that are associated with the area

                // Get the selected Area Name, then resolve to an Area ID
                string selectedAreaName = formValues["AreaSelect"];
                scopedAreaName = selectedAreaName;
                int selectedAreaID = -1;
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (selectedAreaName == asset.AreaName)
                    {
                        selectedAreaID = asset.AreaID;
                        break;
                    }
                }

                // Get the Meter IDs that are applicable to the chosen area
                if (selectedAreaID != -1)
                {
                    foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (asset.AreaID_PreferLibertyBeforeInternal == selectedAreaID)
                            listOfMeterIDs.Add(asset.MeterID);
                    }
                }
            }
            else if (selectedScope == "pnlMeter")
            {
                // User chose to scope the report to a specific MeterID
                if (!string.IsNullOrEmpty(formValues["MeterSelect"]))
                {
                    scopedMeter = formValues["MeterSelect"];
                    listOfMeterIDs.Add(Convert.ToInt32(formValues["MeterSelect"]));
                }
            }
            else if (selectedScope == "pnlSiteWide")
            {
                // User chose to scope the report to entire site, so get a list of all Meter IDs for this customer
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    listOfMeterIDs.Add(asset.MeterID);
                }
            }

            // Determine which sections of the report the user want to include (from checkbox options)
            bool includeHourlyStatistics = false;
            bool includeMeterSummary = false;
            bool includeAreaSummary = false;
            bool includeSpaceSummary = false;
            bool includeDailySummary = false;
            bool includeMonthlySummary = false;
            bool includeDetailRecords = false;
            string selectedContentToIncludeCSV = formValues["chkRptContent"];

            // Populate the report parameters
            string selectedActivityRestriction = formValues["ActRestrictRBtn"];
            if (selectedActivityRestriction == "All")
                repParams.RegulatedHoursRestrictionFilter = SensorAndPaymentReportEngine.RegulatedHoursRestrictions.AllActivity;
            else if (selectedActivityRestriction == "Regulated")
                repParams.RegulatedHoursRestrictionFilter = SensorAndPaymentReportEngine.RegulatedHoursRestrictions.OnlyDuringRegulatedHours;
            else if (selectedActivityRestriction == "Unregulated")
                repParams.RegulatedHoursRestrictionFilter = SensorAndPaymentReportEngine.RegulatedHoursRestrictions.OnlyDuringUnregulatedHours;

            // DEBUG: We are explicitly setting this for all activity -- someday we might want to add this param to the GUI, but not yet
            repParams.ActionTakenRestrictionFilter = SensorAndPaymentReportEngine.ReportableEnforcementActivity.AllActivity;

            repParams.StartTime = reportStartDate;
            repParams.EndTime = reportEndDate;

            repParams.IncludeAreaSummary = includeAreaSummary;
            repParams.IncludeMeterSummary = includeMeterSummary;
            repParams.IncludeSpaceSummary = includeSpaceSummary;
            repParams.IncludeHourlyStatistics = includeHourlyStatistics;
            repParams.IncludeDailySummary = includeDailySummary;
            repParams.IncludeMonthlySummary = includeMonthlySummary;
            repParams.IncludeDetailRecords = includeDetailRecords;

            repParams.ScopedAreaName = scopedAreaName;
            repParams.ScopedMeter = scopedMeter;

            return listOfMeterIDs;
        }


        private void SetViewDataForCommonReportChoices(string commonTitle, string commonHeader, bool supportsContentToIncludeOptions)
        {
            ViewData["commonTitle"] = commonTitle;
            ViewData["commonHeader"] = commonHeader;
            ViewData["supportsContentToIncludeOptions"] = supportsContentToIncludeOptions;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            ViewData["CustomerCfg"] = customerDto;
            // Get list of all Area Names (not ID) for the customer, then add to the ViewData
            List<string> listOfAreaNames = new List<string>();
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);

            // Get list of all MeterIDs for the customer, then add to the ViewData
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            ViewData["MeterList"] = new SelectList(listOfMeterIDs);

            // We need to build a Javascript statement that will be used to set the default value for the startDate parameter.
            // We will default the startDate to the beginning of today
            // Note however that in Javascript, the Date() class uses months 0 - 11, so we must subtract one from our desired .NET month!
            DateTime defaultReportStartDate = DateTime.Today;
            defaultReportStartDate = new DateTime(defaultReportStartDate.Year, defaultReportStartDate.Month, defaultReportStartDate.Day, 0, 0, 0);
            ViewData["ReportStartDate_JavascriptDefaulter"] = string.Format("$(\"#startDate\").datetimepicker('setDate', (new Date({0}, {1}, {2}, {3}, {4})));",
                defaultReportStartDate.Year, defaultReportStartDate.Month - 1, defaultReportStartDate.Day, defaultReportStartDate.Hour, defaultReportStartDate.Minute);

            // We need to build a Javascript statement that will be used to set the default value for the endDate parameter.
            // We will default the endDate to the end of today (only to the minutes resolution though)
            // Note however that in Javascript, the Date() class uses months 0 - 11, so we must subtract one from our desired .NET month!
            DateTime defaultReportEndDate = DateTime.Today;
            defaultReportEndDate = new DateTime(defaultReportEndDate.Year, defaultReportEndDate.Month, defaultReportEndDate.Day, 23, 59, 0);
            ViewData["ReportEndDate_JavascriptDefaulter"] = string.Format("$(\"#endDate\").datetimepicker('setDate', (new Date({0}, {1}, {2}, {3}, {4})));",
                defaultReportEndDate.Year, defaultReportEndDate.Month - 1, defaultReportEndDate.Day, defaultReportEndDate.Hour, defaultReportEndDate.Minute);
        }


        // POST: /SpaceStatus/OccupancyReport
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OccupancyReport(FormCollection formValues)
        {


            CustomerLogic result = new CustomerLogic();


            // Create the report parameter object, then populate it from form values and get the list of applicable meters 
            SensorAndPaymentReportEngine.CommonReportParameters repParams = new SensorAndPaymentReportEngine.CommonReportParameters();
            List<int> listOfMeterIDs = PopulateCommonReportParametersFromFormAndGetListOfMeterIds(formValues, repParams);


            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            // Generate the report and return it to the client as a file to download
            OccupancyReport report = new OccupancyReport(customerDto, repParams);
            MemoryStream m = new MemoryStream();
            report.GetReportAsExcelSpreadsheet(listOfMeterIDs, m, result);

            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("Occupancy_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/OccupancyReport

        public ActionResult OccupancyReport()
        {
            //  CustomerLogic result = new CustomerLogic();
            //   result.CustomerId = CurrentCity.Id;
            // string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            // result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            //  result.GatherAllInventory();


            //if (result != null)
            //{
            //    if (Duncan.PEMS.Web.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.ContainsKey(result.CustomerId) == false)
            //        Duncan.PEMS.Web.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.Add(result.CustomerId, result);
            //}
            // Set view data with info used by common Sensor and Payment reports
            SetViewDataForCommonReportChoices("Occupancy Report", "Space Occupancy Reporting", true);

            // Render the view
            return View("CommonReportParamScreen", null);
        }



        // POST: /SpaceStatus/ComplianceOverstayReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplianceOverstayReport(FormCollection formValues)
        {
            // Create the report parameter object, then populate it from form values and get the list of applicable meters 
            SensorAndPaymentReportEngine.CommonReportParameters repParams = new SensorAndPaymentReportEngine.CommonReportParameters();
            List<int> listOfMeterIDs = PopulateCommonReportParametersFromFormAndGetListOfMeterIds(formValues, repParams);



            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            // Generate the report and return it to the client as a file to download
            ComplianceOverstayReport report = new ComplianceOverstayReport(customerDto, repParams);
            MemoryStream m = new MemoryStream();

            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            report.GetReportAsExcelSpreadsheet(listOfMeterIDs, m, result);

            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("ComplianceOverstay_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/ComplianceOverstayReport

        public ActionResult ComplianceOverstayReport()
        {
            // Set view data with info used by common Sensor and Payment reports
            SetViewDataForCommonReportChoices("Overstay Compliance", "Compliance -- Overstay Reporting", true);

            // Render the view
            return View("CommonReportParamScreen", null);
        }



        // POST: /SpaceStatus/CompliancePaymentReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompliancePaymentReport(FormCollection formValues)
        {
            // Create the report parameter object, then populate it from form values and get the list of applicable meters 
            SensorAndPaymentReportEngine.CommonReportParameters repParams = new SensorAndPaymentReportEngine.CommonReportParameters();
            List<int> listOfMeterIDs = PopulateCommonReportParametersFromFormAndGetListOfMeterIds(formValues, repParams);


            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            // Generate the report and return it to the client as a file to download
            CompliancePaymentReport report = new CompliancePaymentReport(customerDto, repParams);
            MemoryStream m = new MemoryStream();
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            report.GetReportAsExcelSpreadsheet(listOfMeterIDs, m, result);

            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("CompliancePayment_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/CompliancePaymentReport

        public ActionResult CompliancePaymentReport()
        {
            // Set view data with info used by common Sensor and Payment reports
            SetViewDataForCommonReportChoices("Payment Compliance", "Compliance -- Payment Reporting", true);

            // Render the view
            return View("CommonReportParamScreen", null);
        }



        // POST: /SpaceStatus/EnforcementReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EnforcementReport(FormCollection formValues)
        {
            // Create the report parameter object, then populate it from form values and get the list of applicable meters 
            SensorAndPaymentReportEngine.CommonReportParameters repParams = new SensorAndPaymentReportEngine.CommonReportParameters();
            List<int> listOfMeterIDs = PopulateCommonReportParametersFromFormAndGetListOfMeterIds(formValues, repParams);


            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // Generate the report and return it to the client as a file to download
            EnforcementReport report = new EnforcementReport(customerDto, repParams);
            MemoryStream m = new MemoryStream();
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            report.GetReportAsExcelSpreadsheet(listOfMeterIDs, m, result);

            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("Enforcement_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/EnforcementReport

        public ActionResult EnforcementReport()
        {
            // Set view data with info used by common Sensor and Payment reports
            SetViewDataForCommonReportChoices("Enforcement Report", "Enforcement Reporting", true);

            // Render the view
            return View("CommonReportParamScreen", null);
        }



        // POST: /SpaceStatus/ParkingAndOverstayEnforcementDetailsReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ParkingAndOverstayEnforcement(FormCollection formValues)
        {
            // Create the report parameter object, then populate it from form values and get the list of applicable meters 
            SensorAndPaymentReportEngine.CommonReportParameters repParams = new SensorAndPaymentReportEngine.CommonReportParameters();
            List<int> listOfMeterIDs = PopulateCommonReportParametersFromFormAndGetListOfMeterIds1(formValues, repParams);
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            // Generate the report and return it to the client as a file to download
            ParkingAndOverstayEnforcementDetailsReport report = new ParkingAndOverstayEnforcementDetailsReport(customerDto, repParams);
            MemoryStream m = new MemoryStream();
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            report.GetReportAsExcelSpreadsheet(listOfMeterIDs, m, result);

            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("ParkingAndOverstayEnforcement_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/ParkingAndOverstayEnforcementDetailsReport

        public ActionResult ParkingAndOverstayEnforcement()
        {
            // Set view data with info used by common Sensor and Payment reports
            SetViewDataForCommonReportChoices("Parking Enforcement", "Parking and Overstay Enforcement Reporting", false);

            // Render the view
            return View("CommonReportParamScreen", null);
        }



        // GET: /SpaceStatus/OverstayReport (Deprecated -- replace by ComplianceOverstayReport)

        public ActionResult OverstayReport()
        {
            return RedirectToAction("ComplianceOverstayReport");
        }


        // GET: /SpaceStatus/ViolationReport (Deprecated -- replaced by EnforcementReport)

        public ActionResult ViolationReport()
        {
            return RedirectToAction("EnforcementReport");
        }


        // GET: /SpaceStatus/ComplianceReport (Deprecated -- replaced by ComplianceOverstayReport)

        public ActionResult ComplianceReport()
        {
            return RedirectToAction("ComplianceOverstayReport");
        }



        // POST: /SpaceStatus/OpsAdminMaintExReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OpsAdminMaintExReport(FormCollection formValues)
        {
            // Get the user's selected Start and End date parameters from the formValues
            DateTime reportStartDate = DateTime.MinValue;
            reportStartDate = Convert.ToDateTime(formValues["startDate"]);

            DateTime reportEndDate = DateTime.MinValue;
            reportEndDate = Convert.ToDateTime(formValues["endDate"]);

            string scopedAreaName = string.Empty;
            string scopedMeter = string.Empty;

            // Need to see if user chose "Current Status" or "Historical" reporting style
            string selectedRepType = formValues["RepTypeRBtn"];

            // We need to generate a list of all MeterIDs that will be included in the report.
            // The "Scope" parameter might limit the reportable meters to an area or specific MeterID, etc.
            // First, we need to see which "scope" the user selected
            List<int> listOfMeterIDs = new List<int>();
            string selectedScope = formValues["ScopeRBtn"];

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            if (selectedScope == "pnlArea")
            {
                // User chose to scope the report to a specific Area, we need to resolve the AreaID for the
                // selected area name, and then find all MeterIDs that are associated with the area

                // Get the selected Area Name, then resolve to an Area ID
                string selectedAreaName = formValues["AreaSelect"];
                scopedAreaName = selectedAreaName;
                int selectedAreaID = -1;
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (selectedAreaName == asset.AreaName)
                    {
                        selectedAreaID = asset.AreaID;
                        break;
                    }
                }

                // Get the Meter IDs that are applicable to the chosen area
                if (selectedAreaID != -1)
                {
                    foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (asset.AreaID_PreferLibertyBeforeInternal == selectedAreaID)
                            listOfMeterIDs.Add(asset.MeterID);
                    }
                }
            }
            else if (selectedScope == "pnlMeter")
            {
                // User chose to scope the report to a specifice MeterID
                if (!string.IsNullOrEmpty(formValues["MeterSelect"]))
                {
                    scopedMeter = formValues["MeterSelect"];
                    listOfMeterIDs.Add(Convert.ToInt32(formValues["MeterSelect"]));
                }
            }
            else if (selectedScope == "pnlSiteWide")
            {
                // User chose to scope the report to entire site, so get a list of all Meter IDs for this customer
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    listOfMeterIDs.Add(asset.MeterID);
                }
            }

            // Determine which sections of the report the user want to include (from checkbox options)
            string selectedContentToIncludeCSV = formValues["chkRptContent"];
            string[] arrayOfContentToInclude = selectedContentToIncludeCSV.Split(new char[] { ',' });
            bool includeMeterSummary = false;
            bool includeAreaSummary = false;
            bool includeSpaceSummary = false;
            if (!string.IsNullOrEmpty(selectedContentToIncludeCSV))
            {
                foreach (string nextContentToInclude in arrayOfContentToInclude)
                {
                    if (string.Compare("inclAreaSum", nextContentToInclude.Trim(), true) == 0)
                        includeAreaSummary = true;
                    if (string.Compare("inclMeterSum", nextContentToInclude.Trim(), true) == 0)
                        includeMeterSummary = true;
                    if (string.Compare("inclSpaceSum", nextContentToInclude.Trim(), true) == 0)
                        includeSpaceSummary = true;
                    if (string.Compare("All", nextContentToInclude.Trim(), true) == 0)
                    {
                        includeAreaSummary = true;
                        includeMeterSummary = true;
                        includeSpaceSummary = true;
                    }
                }
            }
            else
            {
                includeAreaSummary = true;
                includeMeterSummary = true;
                includeSpaceSummary = true;

            }

            // Build a report parameter object container based on the user's selections
            OpsAdminMaintExceptionsReportParameters reportParams = new OpsAdminMaintExceptionsReportParameters();
            if (string.Compare(selectedRepType, "pnlCurrentStatus", true) == 0)
                reportParams.ReportOnHistoricalData = false;
            else
                reportParams.ReportOnHistoricalData = true;
            reportParams.IncludeDetails = true;
            reportParams.IncludeAreaSummary = includeAreaSummary;
            reportParams.IncludeMeterSummary = includeMeterSummary;
            reportParams.IncludeSpaceSummary = includeSpaceSummary;
            reportParams.StartTime = reportStartDate;
            reportParams.EndTime = reportEndDate;
            reportParams.ScopedAreaName = scopedAreaName;
            reportParams.ScopedMeter = scopedMeter;

            // Generate the report and return it to the client as a file to download
            OpsAdminMaintExceptionsReportEngine OpsAdminMaintExReport = new OpsAdminMaintExceptionsReportEngine(customerDto);
            MemoryStream repMS = new MemoryStream();
            OpsAdminMaintExReport.GetReportAsExcelSpreadsheet(listOfMeterIDs, repMS, reportParams);
            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            repMS.Flush();
            repMS.Position = 0;
            repMS.Capacity = Convert.ToInt32(repMS.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(repMS, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("OpsAdminMaintEx_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/OpsAdminMaintExReport

        public ActionResult OpsAdminMaintExReport()
        {
            // Get list of all Area Names (not ID) for the customer, then add to the ViewData
            List<string> listOfAreaNames = new List<string>();

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);

            // Get list of all MeterIDs for the customer, then add to the ViewData
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            ViewData["MeterList"] = new SelectList(listOfMeterIDs);

            // We need to build a Javascript statement that will be used to set the default value for the startDate parameter.
            // We will default the startDate to the beginning of today
            // Note however that in Javascript, the Date() class uses months 0 - 11, so we must subtract one from our desired .NET month!
            DateTime defaultReportStartDate = DateTime.Today;
            defaultReportStartDate = new DateTime(defaultReportStartDate.Year, defaultReportStartDate.Month, defaultReportStartDate.Day, 0, 0, 0);
            ViewData["ReportStartDate_JavascriptDefaulter"] = string.Format("$(\"#startDate\").datetimepicker('setDate', (new Date({0}, {1}, {2}, {3}, {4})));",
                defaultReportStartDate.Year, defaultReportStartDate.Month - 1, defaultReportStartDate.Day, defaultReportStartDate.Hour, defaultReportStartDate.Minute);

            // We need to build a Javascript statement that will be used to set the default value for the endDate parameter.
            // We will default the endDate to the end of today (only to the minutes resolution though)
            // Note however that in Javascript, the Date() class uses months 0 - 11, so we must subtract one from our desired .NET month!
            DateTime defaultReportEndDate = DateTime.Today;
            defaultReportEndDate = new DateTime(defaultReportEndDate.Year, defaultReportEndDate.Month, defaultReportEndDate.Day, 23, 59, 0);
            ViewData["ReportEndDate_JavascriptDefaulter"] = string.Format("$(\"#endDate\").datetimepicker('setDate', (new Date({0}, {1}, {2}, {3}, {4})));",
                defaultReportEndDate.Year, defaultReportEndDate.Month - 1, defaultReportEndDate.Day, defaultReportEndDate.Hour, defaultReportEndDate.Minute);

            // Render the view
            return View("OpsAdminMaintExReport", null);
        }



        // POST: /SpaceStatus/VSLatencyReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VSLatencyReport(FormCollection formValues)
        {
            // Get the user's selected Start and End date parameters from the formValues
            DateTime reportStartDate = DateTime.MinValue;
            reportStartDate = Convert.ToDateTime(formValues["startDate"]);

            DateTime reportEndDate = DateTime.MinValue;
            reportEndDate = Convert.ToDateTime(formValues["endDate"]);

            string scopedAreaName = string.Empty;
            string scopedMeter = string.Empty;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            // We need to generate a list of all MeterIDs that will be included in the report.
            // The "Scope" parameter might limit the reportable meters to an area or specific MeterID, etc.
            // First, we need to see which "scope" the user selected
            List<int> listOfMeterIDs = new List<int>();
            string selectedScope = formValues["ScopeRBtn"];
            if (selectedScope == "pnlArea")
            {
                // User chose to scope the report to a specific Area, we need to resolve the AreaID for the
                // selected area name, and then find all MeterIDs that are associated with the area

                // Get the selected Area Name, then resolve to an Area ID
                string selectedAreaName = formValues["AreaSelect"];
                scopedAreaName = selectedAreaName;
                int selectedAreaID = -1;
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (selectedAreaName == asset.AreaName)
                    {
                        selectedAreaID = asset.AreaID;
                        break;
                    }
                }

                // Get the Meter IDs that are applicable to the chosen area
                if (selectedAreaID != -1)
                {
                    foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (asset.AreaID_PreferLibertyBeforeInternal == selectedAreaID)
                            listOfMeterIDs.Add(asset.MeterID);
                    }
                }
            }
            else if (selectedScope == "pnlMeter")
            {
                // User chose to scope the report to a specifice MeterID
                if (!string.IsNullOrEmpty(formValues["MeterSelect"]))
                {
                    scopedMeter = formValues["MeterSelect"];
                    listOfMeterIDs.Add(Convert.ToInt32(formValues["MeterSelect"]));
                }
            }
            else if (selectedScope == "pnlSiteWide")
            {
                // User chose to scope the report to entire site, so get a list of all Meter IDs for this customer
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    listOfMeterIDs.Add(asset.MeterID);
                }
            }

            /*
            // Determine which sections of the report the user want to include (from checkbox options)
            string selectedContentToIncludeCSV = formValues["chkRptContent"];
            string[] arrayOfContentToInclude = selectedContentToIncludeCSV.Split(new char[] { ',' });
            bool includeMeterSummary = false;
            bool includeAreaSummary = false;
            bool includeSpaceSummary = false;
            foreach (string nextContentToInclude in arrayOfContentToInclude)
            {
                if (string.Compare("inclAreaSum", nextContentToInclude.Trim(), true) == 0)
                    includeAreaSummary = true;
                if (string.Compare("inclMeterSum", nextContentToInclude.Trim(), true) == 0)
                    includeMeterSummary = true;
                if (string.Compare("inclSpaceSum", nextContentToInclude.Trim(), true) == 0)
                    includeSpaceSummary = true;
            }
            */

            /*
            // Determine which activity restriction style the user chose
            OccupancyReportEngine.ActivityRestrictions activityRestrict = OccupancyReportEngine.ActivityRestrictions.AllActivity;
            string selectedActivityRestriction = formValues["ActRestrictRBtn"];
            if (selectedActivityRestriction == "All")
                activityRestrict = OccupancyReportEngine.ActivityRestrictions.AllActivity;
            else if (selectedActivityRestriction == "Regulated")
                activityRestrict = OccupancyReportEngine.ActivityRestrictions.OnlyDuringRegulatedHours;
            else if (selectedActivityRestriction == "Unregulated")
                activityRestrict = OccupancyReportEngine.ActivityRestrictions.OnlyDuringUnregulatedHours;
            */

            // Generate the report and return it to the client as a file to download
            VehSensingDataExport report = new VehSensingDataExport(customerDto);
            MemoryStream m = new MemoryStream();
            report.GetLatencyDataXLS(customerDto.CustomerId, listOfMeterIDs, reportStartDate, reportEndDate, m);
            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("VSLatency_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/VSLatencyReport

        public ActionResult VSLatencyReport()
        {
            // Get list of all Area Names (not ID) for the customer, then add to the ViewData
            List<string> listOfAreaNames = new List<string>();

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;

            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);

            // Get list of all MeterIDs for the customer, then add to the ViewData
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            ViewData["MeterList"] = new SelectList(listOfMeterIDs);

            // We need to build a Javascript statement that will be used to set the default value for the startDate parameter.
            // We will default the startDate to the beginning of today
            // Note however that in Javascript, the Date() class uses months 0 - 11, so we must subtract one from our desired .NET month!
            DateTime defaultReportStartDate = DateTime.Today;
            defaultReportStartDate = new DateTime(defaultReportStartDate.Year, defaultReportStartDate.Month, defaultReportStartDate.Day, 0, 0, 0);
            ViewData["ReportStartDate_JavascriptDefaulter"] = string.Format("$(\"#startDate\").datetimepicker('setDate', (new Date({0}, {1}, {2}, {3}, {4})));",
                defaultReportStartDate.Year, defaultReportStartDate.Month - 1, defaultReportStartDate.Day, defaultReportStartDate.Hour, defaultReportStartDate.Minute);

            // We need to build a Javascript statement that will be used to set the default value for the endDate parameter.
            // We will default the endDate to the end of today (only to the minutes resolution though)
            // Note however that in Javascript, the Date() class uses months 0 - 11, so we must subtract one from our desired .NET month!
            DateTime defaultReportEndDate = DateTime.Today;
            defaultReportEndDate = new DateTime(defaultReportEndDate.Year, defaultReportEndDate.Month, defaultReportEndDate.Day, 23, 59, 0);
            ViewData["ReportEndDate_JavascriptDefaulter"] = string.Format("$(\"#endDate\").datetimepicker('setDate', (new Date({0}, {1}, {2}, {3}, {4})));",
                defaultReportEndDate.Year, defaultReportEndDate.Month - 1, defaultReportEndDate.Day, defaultReportEndDate.Hour, defaultReportEndDate.Minute);

            // Render the view
            return View("VSLatencyReport", null);
        }


        // POST: /SpaceStatus/AssetReport

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetReport(FormCollection formValues)
        {
            string scopedAreaName = string.Empty;
            string scopedMeter = string.Empty;

            // We need to generate a list of all MeterIDs that will be included in the report.
            // The "Scope" parameter might limit the reportable meters to an area or specific MeterID, etc.
            // First, we need to see which "scope" the user selected
            List<int> listOfMeterIDs = new List<int>();
            string selectedScope = formValues["ScopeRBtn"];


            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }


            if (selectedScope == "pnlArea")
            {
                // User chose to scope the report to a specific Area, we need to resolve the AreaID for the
                // selected area name, and then find all MeterIDs that are associated with the area

                // Get the selected Area Name, then resolve to an Area ID
                string selectedAreaName = formValues["AreaSelect"];
                scopedAreaName = selectedAreaName;
                int selectedAreaID = -1;
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
                {
                    if (selectedAreaName == asset.AreaName)
                    {
                        selectedAreaID = asset.AreaID;
                        break;
                    }
                }

                // Get the Meter IDs that are applicable to the chosen area
                if (selectedAreaID != -1)
                {
                    foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                    {
                        if (asset.AreaID_PreferLibertyBeforeInternal == selectedAreaID)
                            listOfMeterIDs.Add(asset.MeterID);
                    }
                }
            }
            else if (selectedScope == "pnlMeter")
            {
                // User chose to scope the report to a specifice MeterID
                if (!string.IsNullOrEmpty(formValues["MeterSelect"]))
                {
                    scopedMeter = formValues["MeterSelect"];
                    listOfMeterIDs.Add(Convert.ToInt32(formValues["MeterSelect"]));
                }
            }
            else if (selectedScope == "pnlSiteWide")
            {
                // User chose to scope the report to entire site, so get a list of all Meter IDs for this customer
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    listOfMeterIDs.Add(asset.MeterID);
                }
            }

            // Determine which activity restriction style the user chose
            AssetListingReportEngine.ActivityRestrictions activityRestrict = AssetListingReportEngine.ActivityRestrictions.AllActivity;

            // Generate the report and return it to the client as a file to download
            AssetListingReportEngine report = new AssetListingReportEngine(customerDto);
            MemoryStream m = new MemoryStream();
            report.GetReportAsExcelSpreadsheet(listOfMeterIDs, m, activityRestrict, scopedAreaName, scopedMeter);
            // To ensure best results, we must make sure the stream has been fully written, its capacity matches its length, and the stream position is reset to the beginning!
            m.Flush();
            m.Position = 0;
            m.Capacity = Convert.ToInt32(m.Length);
            // NOTE: Although it looks like an oversight, we can't Dispose of the stream, or wrap it in a "using" block here.  As it turns out,
            // the stream will automatically be disposed by the FileStreamResult object when it is finished with it.  For more info, refer to
            // this link:  http://stackoverflow.com/questions/3084366/how-do-i-dispose-my-filestream-when-implementing-a-file-download-in-asp-net

            // Return the Excel file that was generated
            return File(m, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("AssetListing_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx"));
        }

        // GET: /SpaceStatus/AssetReport

        public ActionResult AssetReport()
        {
            // Get list of all Area Names (not ID) for the customer, then add to the ViewData
            List<string> listOfAreaNames = new List<string>();
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerDto))
            {
                listOfAreaNames.Add(asset.AreaName);
            }
            ViewData["AreaNameList"] = new SelectList(listOfAreaNames);

            // Get list of all MeterIDs for the customer, then add to the ViewData
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            ViewData["MeterList"] = new SelectList(listOfMeterIDs);

            // Render the view
            return View("AssetDetail", null);
        }

        #endregion // Reports



        #region Fake Sensor Event (Internal tool for simulating vehicle sensor events)

        // FakeSensorEvent
        // GET: /SpaceStatus/FakeSensorEvent
        /// <summary>
        /// Simulates a vehcile sensor event.  This is intended only for testing/demo purposes!
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="S">Status. 0=Vacant, 1=Occupied</param>
        /// <returns></returns>

        public ActionResult FakeSensorEvent(string T, string S, string ResponseText)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            if (!string.IsNullOrEmpty(T))
            {
                int loIdxStart = T.IndexOf('m') + 1;
                int loIdxEnd = T.IndexOf('s');
                MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
                loIdxStart = loIdxEnd + 1;
                SNUM = T.Substring(loIdxStart);
            }

            // Default the occupancy status if not already set
            if (string.IsNullOrEmpty(S))
                S = "0";
            if (string.IsNullOrEmpty(ResponseText))
                ResponseText = "";

            // Retain usefule values in the view data
            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;
            ViewData["originalTarget"] = T;
            ViewData["ResponseText"] = ResponseText;
            ViewData["StatusCode"] = S;

            // Build list of choices, which are the spaces associated with the customer
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            string groupChoiceDefaultSelection = null;
            int SelectedIndex = 0;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                {
                    if (spcAsset.MeterID == mtrAsset.MeterID)
                    {
                        GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                        choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                        if (mtrAsset.MeterGroupID <= 0) // SSM
                            choice.DataText = mtrAsset.MeterName;
                        else
                            choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                        groupChoices.Add(choice);

                        // Should this be our default selection for the combobox?
                        if ((!string.IsNullOrEmpty(SNUM)) && (!string.IsNullOrEmpty(MID)))
                        {
                            if ((string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0) &&
                                (string.Compare(MID, spcAsset.MeterID.ToString()) == 0))
                            {
                                groupChoiceDefaultSelection = choice.DataValue;
                                SelectedIndex = groupChoices.Count - 1;
                            }
                        }
                    }
                }
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText", groupChoiceDefaultSelection);
            ViewData["SelectedIndex"] = SelectedIndex;

            return View("FakeSensorEvent");
        }

        // FakeSensorEvent
        // POST: /SpaceStatus/FakeSensorEvent
        /// <summary>
        /// Simulates a vehcile sensor event.  This is intended only for testing/demo purposes!
        /// </summary>
        /// <param name="T">Target. Value is a MeterID and Space number in format of m#s#. It is prefixed with MeterID for disambiguation</param>
        /// <param name="S">Status. 0=Vacant, 1=Occupied</param>
        /// <returns></returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FakeSensorEvent(FormCollection formValues, string T, string S, string ResponseText)
        {
            // Extract the MeterID and Space Number from the target (T) parameter
            string MID = string.Empty;
            string SNUM = string.Empty;
            if (!string.IsNullOrEmpty(T))
            {
                int loIdxStart = T.IndexOf('m') + 1;
                int loIdxEnd = T.IndexOf('s');
                MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
                loIdxStart = loIdxEnd + 1;
                SNUM = T.Substring(loIdxStart);
            }

            // Extract the target from the submitted form values
            T = formValues["GroupSelect"];
            if (!string.IsNullOrEmpty(T))
            {
                int loIdxStart = T.IndexOf('m') + 1;
                int loIdxEnd = T.IndexOf('s');
                MID = T.Substring(loIdxStart, (loIdxEnd - loIdxStart));
                loIdxStart = loIdxEnd + 1;
                SNUM = T.Substring(loIdxStart);
            }

            // Default the occupancy status if not already set
            if (string.IsNullOrEmpty(S))
                S = "0";
            if (string.IsNullOrEmpty(ResponseText))
                ResponseText = "";

            // Retain usefule values in the view data
            ViewData["MID"] = MID;
            ViewData["SNUM"] = SNUM;
            ViewData["originalTarget"] = T;
            ViewData["ResponseText"] = ResponseText;
            ViewData["StatusCode"] = S;

            // Build list of choices, which are the spaces associated with the customer
            List<GenericObjForHTMLSelectList> groupChoices = new List<GenericObjForHTMLSelectList>();
            string groupChoiceDefaultSelection = null;
            int SelectedIndex = 0;

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                foreach (SpaceAsset spcAsset in CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(customerDto))
                {
                    if (spcAsset.MeterID == mtrAsset.MeterID)
                    {
                        GenericObjForHTMLSelectList choice = new GenericObjForHTMLSelectList();
                        choice.DataValue = "m" + spcAsset.MeterID.ToString() + "s" + spcAsset.SpaceID.ToString();
                        if (mtrAsset.MeterGroupID <= 0) // SSM
                            choice.DataText = mtrAsset.MeterName;
                        else
                            choice.DataText = mtrAsset.MeterName + "-" + spcAsset.SpaceID.ToString();
                        groupChoices.Add(choice);

                        // Should this be our default selection for the combobox?
                        if ((!string.IsNullOrEmpty(SNUM)) && (!string.IsNullOrEmpty(MID)))
                        {
                            if ((string.Compare(SNUM, spcAsset.SpaceID.ToString()) == 0) &&
                                (string.Compare(MID, spcAsset.MeterID.ToString()) == 0))
                            {
                                groupChoiceDefaultSelection = choice.DataValue;
                                SelectedIndex = groupChoices.Count - 1;
                            }
                        }
                    }
                }
            }

            // Add the group choices to the view data
            ViewData["groupChoices"] = new SelectList(groupChoices, "DataValue", "DataText", groupChoiceDefaultSelection);
            ViewData["SelectedIndex"] = SelectedIndex;

            // Determine which sections of the report the user want to include (from checkbox options)
            string selectedContentToIncludeCSV = formValues["chkIsOccupied"];
            bool isOccupied = false;
            if (!string.IsNullOrEmpty(selectedContentToIncludeCSV))
            {
                string[] arrayOfContentToInclude = selectedContentToIncludeCSV.Split(new char[] { ',' });
                foreach (string nextContentToInclude in arrayOfContentToInclude)
                {
                    if (string.Compare("IsOccupied", nextContentToInclude.Trim(), true) == 0)
                        isOccupied = true;
                }
            }

            // Update view data with status code derived from the form values
            if (isOccupied)
                ViewData["StatusCode"] = "1";
            else
                ViewData["StatusCode"] = "0";

            try
            {
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerDto, Convert.ToInt32(MID));
                SpaceAsset spcAsset = CustomerLogic.CustomerManager.GetSpaceAsset(customerDto, Convert.ToInt32(MID), Convert.ToInt32(SNUM));

                SensingDatabaseSource _VSProvider = new SensingDatabaseSource(customerDto);
                string UDPServerResponse = _VSProvider.SimulateVehicleSensingEvent(customerDto.CustomerId, Convert.ToInt32(MID), Convert.ToInt32(SNUM), asset.AreaID_Internal, spcAsset.ParkingSpaceId_Internal, isOccupied);
                ViewData["ResponseText"] = UDPServerResponse;
                _VSProvider = null;
            }
            catch (Exception ex)
            {
                ViewData["ResponseText"] = "Failed to simulate event due to error: " + ex.Message;
            }

            return View("FakeSensorEvent");
        }

        #endregion // Fake Sensor Event


        #region Sensor Status Demo

        // Sensor Status Demo 
        // GET: /SpaceStatus/SensorStatusDemo

        public ActionResult SensorStatusDemo()
        {
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            List<int> listOfMeterIDs = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                listOfMeterIDs.Add(asset.MeterID);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            // Get current space info -- but since this is just for sensor info, we don't need to waste time gathering PAM data or Action Taken info...
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);

            ViewData["groupPrompt"] = "Sensor Status Demo";
            ViewData["displayStyle"] = "D";
            return View("SensorStatusDemo", modelForView);
        }

        // Sensor Status Demo (Partial View for desktop view contents)
        // GET: /SpaceStatus/SensorStatusDemoPartial
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult SensorStatusDemoPartial(List<SpaceStatusModel> modelForView)
        {

            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            ViewData["CustomerCfg"] = customerDto;
            // We will need to gather current info for all meters and spaces
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);

            // Get current space info -- but since this is just for sensor info, we don't need to waste time gathering PAM data or Action Taken info...
            if (modelForView == null)
            {
                List<int> listOfMeterIDs = new List<int>();
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
                {
                    listOfMeterIDs.Add(asset.MeterID);
                }
                CustomerLogic result = new CustomerLogic();
                result.CustomerId = CurrentCity.Id;
                string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
                result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
                modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, false, false, result);
            }

            ViewData["groupPrompt"] = "Sensor Status Demo";
            ViewData["displayStyle"] = "D";
            return View("pv_SensorStatusDemoContent", modelForView);
        }

        #endregion // Sensor Status Demo


        #region Utility Methods

        private int GetClusterPrimaryKeyForMeterID(int meterID, CustomerConfig customerCfg)
        {
            int clusterPK = -1;
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                if (asset.MeterID == meterID)
                {
                    clusterPK = asset.PAMClusterID;
                    break;
                }
            }
            return clusterPK;
        }

        private List<int> GetMeterIDsInSameClusterAsMeter(int meterID, CustomerConfig customerCfg)
        {
            // Find the PAM Cluster Id associated with the meter
            int clusterPK = -1;
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                if (asset.MeterID == meterID)
                {
                    clusterPK = asset.PAMClusterID;
                    break;
                }
            }

            // Now find all meters that have the same PAM Cluster Id
            List<int> result = new List<int>();
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                if (asset.PAMClusterID == clusterPK)
                {
                    result.Add(asset.MeterID);
                }
            }

            return result;
        }

        private int GetClusterPrimaryKeyForClusterID(int clusterID, CustomerConfig customerCfg)
        {
            return clusterID;
        }

        private List<int> GetMeterIDsInCluster(int clusterID, CustomerConfig customerCfg)
        {

            // Find all meters that have the same PAM Cluster Id
            List<int> result = new List<int>();
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }

            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerDto))
            {
                if (asset.PAMClusterID == clusterID)
                {
                    result.Add(asset.MeterID);
                }
            }

            return result;
        }

        #endregion // Utility methods



        // DEBUG: "AllOfThem" is largely deprecated and replaced with views in the MeterStatus controller (But there is still potentially useful stuff in this view, such as the TreeView)

        // GET: /SpaceStatus/AllOfThem

        public ActionResult AllOfThem()
        {
            List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider> spaces = new List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider>();

            // DEBUG: Our view model needs to be updated -- the view won't use the array of SpaceStatus
            /*
            Duncan.PEMS.SpaceStatus.Models.SpaceStatus space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(_CustomerCfg);
            spaces.Add(space);
            space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(_CustomerCfg);
            space.MeterID = "7456";
            space.SpaceID = "145";
            space.IsOccupied = false;
            spaces.Add(space);
            */

            return View("ViewPage3", spaces);
        }

        // GET: /SpaceStatus/AllOfThem_Partial
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] // Need this so Internet Explorer doesn't cache AJAX responses

        public ActionResult AllOfThem_PartialView(string valueThatAintGonnaBeThere, string CID, string targetID)
        {
            // DEV Note:  We can have parameters for this function, and they will automatically get filled
            // in if they exist as URL query parameters in the request (i.e. this.Request.QueryString).
            // Also note that the parameters are completely optional and don't need to be passed.
            // It is also possible to configure routes in Global.asax.cs so RESTful style URLs are supported...


            // Start diagnostics timer
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            CustomerConfig customerDto = new CustomerConfig();
            customerDto.CustomerId = CurrentCity.Id;
            customerDto.CustomerName = CurrentCity.DisplayName;
            customerDto.CustomerNameForFiles = Regex.Replace(CurrentCity.DisplayName, @"[?:\/*""<>|]", "_");
            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);
            if (TimeZoneDateTime.Count() > 0)
            {
                customerDto.DestinationTimeZoneDisplayName = TimeZoneDateTime[0].ToString();

            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                customerDto.DestinationTimeZoneDisplayName = DateTime.Now.ToString();

            }
            List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider> spaces = new List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider>();
            Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);

            // DEBUG: We are not using a view Model of enumerable SpaceStatus objects.  Instead, we will use a generic DataSet as the Model
            /*
            spaces.Add(space);

            space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            space.MeterID = "7456";
            space.SpaceID = "145";
            space.IsOccupied = false;
            spaces.Add(space);

            space = new Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider(customerDto);
            space.MeterID = "Another one";
            space.SpaceID = "667";
            space.IsOccupied = true;
            spaces.Add(space);
            */

            // DEBUG: just a test of getting PAM + VS data...
            //DataSet dsPAMInfo = space.GetReport(/*9991*//*217*/3105, /*19572*//*7001*//*9301*/1001);
            //ds1 = space.GetReport(/*9991*//*217*/3105, /*19572*//*7001*//*9301*/1001); // Cale

            // We are using ID attributes for the anchors in the HTML.  Clusters will be prefixed with "tCL_" (tree cluster) or "tME_" (tree meter), and then followed by the ClusterID or MeterID.
            // So, we will look at "targetID" parameter, which comes from an anchor with the above mentioned format.
            // If the parameter is prefixed with "tCL_", then we know it contains a ClusterID, and if it is prefixed with "tME_", then it contains a MeterID
            List<int> listOfMeterIDs = new List<int>();
            if (targetID.StartsWith("tME_"))
            {
                // Extract single MeterID from the parameter
                listOfMeterIDs.Add(Convert.ToInt32(targetID.Remove(0, "tME_".Length)));
            }
            else if (targetID.StartsWith("tCL_"))
            {
                // Extract the ClusterID from the parameters by chopping off the "tCL_" prefix (for cluster)
                int loClusterID = Convert.ToInt32(targetID.Remove(0, "tCL_".Length));
                listOfMeterIDs = GetMeterIDsInCluster(loClusterID, customerDto);
            }
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            List<SpaceStatusModel> modelForView = space.GetCurrentSpaceStatusForView(customerDto.CustomerId, listOfMeterIDs, result);

            // DEBUG: This next line is if our view is based on the "System.Data.DataSet" Model
            // Get dataset representing current state of each desired meter
            //DataSet ds1 = space.GetReport(customerDto.CustomerId, listOfMeterIDs);

            // DEBUG: Since we are using the dataset as our model, we don't have to add it to the ViewData in order to access it
            ////this.ViewData["MeterStatusDataset"] = ds1;

            // Stop diagnostics timer
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Partial View - Elapsed: " + sw.Elapsed.ToString());

            // DEBUG: This next line is if our view was based on the "IEnumerable<Duncan.PEMS.SpaceStatus.Models.SpaceStatus>>" Model
            ////return View("PartialVS", spaces);

            // DEBUG: This next line is if our view is based on the "System.Data.DataSet" Model
            ////return View("PartialVS", ds1);

            // This next line is if our view is based on the "List<SpaceStatusModel>" Model
            return View("PartialVS", modelForView);
        }


        // DEBUG: MobileIETest can safely be removed -- it was introduced for testing purposes during development
        public ActionResult MobileIETest()
        {
            return View("MobileIETest");
        }


        public ActionResult TestLink()
        {
            return View();
        }

        public ActionResult SpaceOccupanyStatus()
        {
            int customerId = CurrentCity.Id;

            string destinationURL = Request.Url.Scheme + "://" + Request.Url.Authority +
    Request.ApplicationPath.TrimEnd('/') + "/" + "SpaceStatus/SpaceStatus/SpcSummary/"+ customerId.ToString();

            ViewBag.Path = destinationURL;// @"http://www.civicsmart.com/";

            return View("PEMSSpaceStatus");
        }
    }
}
