using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
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

using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using RBACToolbox;
using RBACProvider;

//using Duncan.PEMS.Web;



namespace Duncan.PEMS.SpaceStatus.UtilityClasses
{
    #region CompressFilter
    /// <summary>
    /// Compression filter based on info at: http://www.singular.co.nz/blog/archive/2008/07/06/finding-preferred-accept-encoding-header-in-csharp.aspx
    /// and http://stackoverflow.com/questions/866489/can-i-use-compressfilter-in-asp-net-mvc-without-breaking-donut-caching
    /// This compression filter respects the preferences and supported capabilities of the client
    /// </summary>
    public class CompressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            string[] supported = new string[] { "gzip", "deflate" };
            IEnumerable<string> preferredOrder = new AcceptList(request.Headers["Accept-Encoding"], supported);
            string preferred = preferredOrder.FirstOrDefault();
            HttpResponseBase response = filterContext.HttpContext.Response;

            switch (preferred)
            {
                case "gzip":
                    response.AppendHeader("Content-Encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    break;

                case "deflate":
                    response.AppendHeader("Content-Encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                    break;

                case "identity":
                default:
                    break;
            }
        }

        private class AcceptList : IEnumerable<string>
        {
            Regex parser = new Regex(@"(?<name>[^;,\r\n]+)(?:;q=(?<value>[\d.]+))?", RegexOptions.Compiled);
            IEnumerable<string> encodings;

            public AcceptList(string acceptHeaderValue, IEnumerable<string> supportedEncodings)
            {
                List<KeyValuePair<string, float>> accepts = new List<KeyValuePair<string, float>>();

                if (!string.IsNullOrEmpty(acceptHeaderValue))
                {
                    MatchCollection matches = parser.Matches(acceptHeaderValue);

                    var values = from Match v in matches
                                 where v.Success
                                 select new
                                 {
                                     Name = v.Groups["name"].Value,
                                     Value = v.Groups["value"].Value
                                 };

                    foreach (var value in values)
                    {
                        if (value.Name == "*")
                        {
                            foreach (string encoding in supportedEncodings)
                            {
                                if (!accepts.Where(a => a.Key.ToUpperInvariant() == encoding.ToUpperInvariant()).Any())
                                {
                                    accepts.Add(new KeyValuePair<string, float>(encoding, 1.0f));
                                }
                            }
                            continue;
                        }

                        float desired = 1.0f;
                        if (!string.IsNullOrEmpty(value.Value))
                        {
                            float.TryParse(value.Value, out desired);
                        }

                        if (desired == 0.0f)
                        {
                            continue;
                        }

                        accepts.Add(new KeyValuePair<string, float>(value.Name, desired));
                    }
                }

                this.encodings = from a in accepts
                                 where supportedEncodings.Where(se => se.ToUpperInvariant() == a.Key.ToUpperInvariant()).Any() || a.Key.ToUpperInvariant() == "IDENTITY"
                                 orderby a.Value descending
                                 select a.Key;
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                return this.encodings.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)this.encodings).GetEnumerator();
            }
        }
    }
    #endregion

    #region ValidateSelectedCustomer
    /// <summary>
    /// Authorization attribute that checks to make sure a valid customer is supplied to the controller via a "CID" query parameter.
    /// Also checks to make sure the user has sufficient access rights to view the customer.
    /// </summary>
    public class ValidateSelectedCustomer : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            // Call routine to check for selected customer. If no valid customer is selected, a "prerequisite"
            // ActionResult will be returned which will execute instead of the ActionResult which uses this filter attribute.
            // The prerequisite action will reroute the user's browser to a page asking 
            // them to choose a customer, and then will redirect them back to the original ActionResult afterward.

            // Before checking for a customer, lets first see if a specific RBAC Session ID is supplied, which
            // means we need to try an auto-login as the user associated with the RBAC session
            ActionResult Prerequisite = CheckForSpecifiedRBACSession(filterContext);
            if (Prerequisite != null)
            {
                filterContext.Result = Prerequisite;
            }
            else
            {
                // First prerequisite is satisfied, so now lets see about the one to check for a customer being selected
                Prerequisite = EnsureCustomerIsSelected(filterContext);
                if (Prerequisite != null)
                    filterContext.Result = Prerequisite;
            }
        }

        private ActionResult EnsureCustomerIsSelected(AuthorizationContext filterContext)
        {
            // Try to get the current CustomerID from URL params, and also verify that user has access to the customer
            CustomerConfig customerDTO = ExtractValidatedCustomerFromQueryParams(filterContext);

            // If no customer is chosen, we will need to redirect to allow user to choose a customer
            if (customerDTO == null)
            {
                // Build a RouteValueDictionary containing the values derived from the passed collection 
                RouteValueDictionary routeValues = new RouteValueDictionary();
                foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                {
                    // We want to retain passed query params except for special purpose ones we will be using such ass ReturnAction, ReturnController, and CID
                    if ((string.Compare(key, "ReturnAction") != 0) && (string.Compare(key, "CID") != 0) && (string.Compare(key, "ReturnController") != 0))
                        routeValues[key] = filterContext.HttpContext.Request.QueryString[key].ToString();
                }

                // Add the return action and controller to the route values
                routeValues.Add("ReturnAction", filterContext.RouteData.Values["action"].ToString());
                routeValues.Add("ReturnController", filterContext.RouteData.Values["controller"].ToString());

                // Build a new URL which we will be redirecting to
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                string url = helper.Action("SelectClient", "SpaceStatus", routeValues);

                // Return a RedirectResult based on the URL we constructed
                ActionResult result = new RedirectResult(url);  //new RedirectToRouteResult( new { controller = "MyController" , action = "MyAction" }  )
                return result;
            }
            else
            {
                // Selected customer and sufficient access are validated, so we will return a null action so calling action knows normal flow can proceed
                return null;
            }
        }

        private CustomerConfig ExtractValidatedCustomerFromQueryParams(AuthorizationContext filterContext)
        {
            CustomerConfig result = null;
            string CIDParamValue = filterContext.HttpContext.Request.QueryString["CID"];
            if (string.IsNullOrEmpty(CIDParamValue) == false)
            {
                result = CustomerLogic.CustomerManager.GetDTOForCID(CIDParamValue);
                if (result != null)
                {
                    // We resolved the customer, but now we need to see if user is allowed to access the customer
                    // We need to verify that the user has sufficient RBAC access to view this customer!
                    bool userHasAccessToCustomer = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Customer:" + result.CustomerId.ToString());
                    // If no normal access, see if user is an Admin -- that will override all
                    if (userHasAccessToCustomer == false)
                    {
                        userHasAccessToCustomer = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Admin");                        
                    }

                    if (userHasAccessToCustomer == false)
                    {
                        // Clear the result and viewdata if user doesn't have access rights for the customer
                        result = null;
                        filterContext.Controller.ViewData["CustomerCfg"] = null;
                        filterContext.Controller.ViewData["CID"] = -1;
                        filterContext.Controller.ViewData["CustomerName"] = "";
                    }
                    else
                    {
                        // Set common items in the View data
                        filterContext.Controller.ViewData["CustomerCfg"] = result;
                        filterContext.Controller.ViewData["CID"] = result.CustomerId;
                        filterContext.Controller.ViewData["CustomerName"] = result.CustomerName;
                    }
                }
            }
            return result;
        }

        private ActionResult CheckForSpecifiedRBACSession(AuthorizationContext filterContext)
        {
            // If an RBAC Session ID is passed in the query parameters, that will be our queue to attempt to auto-login
            // (or switch users) as the user associated with the RBAC session

            // See if there is an RBACSession specified in the query paramters
            string rbacSessionID = filterContext.HttpContext.Request.QueryString["RBACSession"];
            if (!string.IsNullOrEmpty(rbacSessionID))
            {
                // Build a RouteValueDictionary containing the values derived from the passed collection 
                RouteValueDictionary routeValues = new RouteValueDictionary();
                foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                {
                    routeValues[key] = filterContext.HttpContext.Request.QueryString[key].ToString();
                }

                // Add the return action to the route values
                routeValues.Add("ReturnAction", filterContext.RouteData.Values["action"].ToString());
                routeValues.Add("ReturnController", filterContext.RouteData.Values["controller"].ToString());
                routeValues.Add("returnUrl", "/" + filterContext.RouteData.Values["controller"].ToString() + "/" + filterContext.RouteData.Values["action"].ToString());

                // Build a new URL which we will be redirecting to
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                string url = helper.Action("Login", "Account", routeValues);

                // Return a RedirectResult based on the URL we constructed
                ActionResult result = new RedirectResult(url);
                return result;
            }
            else
            {
                // No RBAC Session ID was included in the query parameters, so no further action is required, and no ActionResult needs to be returned
                return null;
            }
        }
    }
    #endregion

    #region AdminRightsForSelectedCustomer
    /// <summary>
    /// Authorization attribute that checks to make sure a valid customer is supplied to the controller via a "CID" query parameter.
    /// Also checks to make sure the user has sufficient access rights to modify the customer.
    /// </summary>
    public class AdminRightsForSelectedCustomer : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            // Call routine to check for selected customer. If no valid customer is selected, a "prerequisite"
            // ActionResult will be returned which will execute instead of the ActionResult which uses this filter attribute.
            // The prerequisite action will reroute the user's browser to a page asking 
            // them to choose a customer, and then will redirect them back to the original ActionResult afterward.

            // Before checking for a customer, lets first see if a specific RBAC Session ID is supplied, which
            // means we need to try an auto-login as the user associated with the RBAC session
            ActionResult Prerequisite = CheckForSpecifiedRBACSession(filterContext);
            if (Prerequisite != null)
            {
                filterContext.Result = Prerequisite;
            }
            else
            {
                // First prerequisite is satisfied, so now lets see about the one to check for a customer being selected
                Prerequisite = EnsureUserHasSufficientRightsToModifyCustomer(filterContext);
                if (Prerequisite != null)
                    filterContext.Result = Prerequisite;
            }
        }

        private ActionResult EnsureUserHasSufficientRightsToModifyCustomer(AuthorizationContext filterContext)
        {
            // First we will try to get the current CustomerID from URL params, and also verify that user has READ access to the customer
            CustomerConfig customerDTO = GetCustomerFromRequest_CheckForReadAccess(filterContext);

            // If no customer is chosen (or user doesn't even have read rights), we will need to redirect to allow user to choose a customer
            if (customerDTO == null)
            {
                // Build a RouteValueDictionary containing the values derived from the passed collection 
                RouteValueDictionary routeValues = new RouteValueDictionary();
                foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                {
                    // We want to retain passed query params except for special purpose ones we will be using such ass ReturnAction, ReturnController, and CID
                    if ((string.Compare(key, "ReturnAction") != 0) && (string.Compare(key, "CID") != 0) && (string.Compare(key, "ReturnController") != 0))
                        routeValues[key] = filterContext.HttpContext.Request.QueryString[key].ToString();
                }

                // Add the return action and controller to the route values
                routeValues.Add("ReturnAction", filterContext.RouteData.Values["action"].ToString());
                routeValues.Add("ReturnController", filterContext.RouteData.Values["controller"].ToString());

                // Build a new URL which we will be redirecting to
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                string url = helper.Action("SelectClient", "SpaceStatus", routeValues);

                // Return a RedirectResult based on the URL we constructed
                ActionResult result = new RedirectResult(url);  //new RedirectToRouteResult( new { controller = "MyController" , action = "MyAction" }  )
                return result;
            }
            else
            {
                // At this point, we have only validated that a valid customer is selected, and user has READ rights.
                // Now we need to go a step further and ensure the the user has MODIFY rights (admin) for the customer.

                bool sufficientRights = UserHasModifyWritesForCustomer(filterContext, customerDTO);
                if (sufficientRights == false)
                {
                    // Insufficient rights, so we want a result that redirects to main screen
                    // DEBUG: For now, we will just redirect to Home. Maybe later we will introduce a "Access Forbidden" page

                    // Build a RouteValueDictionary containing the values derived from the passed collection 
                    RouteValueDictionary routeValues = new RouteValueDictionary();
                    foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                    {
                        routeValues[key] = filterContext.HttpContext.Request.QueryString[key].ToString();
                    }

                    // Build a new URL which we will be redirecting to 
                    UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                    string url = helper.Action("Index", "Home", routeValues);

                    // Return a RedirectResult based on the URL we constructed
                    ActionResult result = new RedirectResult(url);
                    return result;
                }
                else
                {
                    // Selected customer and sufficient access are validated, so we will return a null action so calling action knows normal flow can proceed
                    return null;
                }
            }
        }

        private bool UserHasModifyWritesForCustomer(AuthorizationContext filterContext, CustomerConfig customerDTO)
        {
            bool result = false;
            bool userHasAccessToCustomer = false;
            bool userHasModifyAccess = false;

            // We need to verify that the user has sufficient RBAC access to view AND MODIFY this customer!
            userHasAccessToCustomer = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Customer:" + customerDTO.CustomerId.ToString());
            // If no access, see if user is an Admin -- that will override all
            if (userHasAccessToCustomer == false)
                userHasAccessToCustomer = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Admin");

            userHasModifyAccess = false;
            if (userHasAccessToCustomer == true)
            {
                userHasModifyAccess = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Admin");
                if (userHasModifyAccess == false)
                    userHasModifyAccess = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Engineering");
                if (userHasModifyAccess == false)
                    userHasModifyAccess = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Operations");
                if (userHasModifyAccess == false)
                    userHasModifyAccess = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "RepositoryManagement");
            }

            if ((userHasAccessToCustomer == false) || (userHasModifyAccess == false))
            {
                // Not enough rights, so set negative response
                result = false;
            }
            else
            {
                // Prerequisite is satisfied, so set positive response
                result = true;
            }

            return result;
        }

        private CustomerConfig GetCustomerFromRequest_CheckForReadAccess(AuthorizationContext filterContext)
        {
            CustomerConfig result = null;
            string CIDParamValue = filterContext.HttpContext.Request.QueryString["CID"];
            if (string.IsNullOrEmpty(CIDParamValue) == false)
            {
                result = CustomerLogic.CustomerManager.GetDTOForCID(CIDParamValue);
                if (result != null)
                {
                    // We resolved the customer, but now we need to see if user is allowed to access the customer
                    // We need to verify that the user has sufficient RBAC access to view this customer!
                    bool userHasAccessToCustomer = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Customer:" + result.CustomerId.ToString());
                    // If no normal access, see if user is an Admin -- that will override all
                    if (userHasAccessToCustomer == false)
                    {
                        userHasAccessToCustomer = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Admin");
                    }

                    if (userHasAccessToCustomer == false)
                    {
                        // Clear the result and viewdata if user doesn't have access rights for the customer
                        result = null;
                        filterContext.Controller.ViewData["CustomerCfg"] = null;
                        filterContext.Controller.ViewData["CID"] = -1;
                        filterContext.Controller.ViewData["CustomerName"] = "";
                    }
                    else
                    {
                        // Set common items in the View data
                        filterContext.Controller.ViewData["CustomerCfg"] = result;
                        filterContext.Controller.ViewData["CID"] = result.CustomerId;
                        filterContext.Controller.ViewData["CustomerName"] = result.CustomerName;
                    }
                }
            }
            return result;
        }

        private ActionResult CheckForSpecifiedRBACSession(AuthorizationContext filterContext)
        {
            // If an RBAC Session ID is passed in the query parameters, that will be our queue to attempt to auto-login
            // (or switch users) as the user associated with the RBAC session

            // See if there is an RBACSession specified in the query paramters
            string rbacSessionID = filterContext.HttpContext.Request.QueryString["RBACSession"];
            if (!string.IsNullOrEmpty(rbacSessionID))
            {
                // Build a RouteValueDictionary containing the values derived from the passed collection 
                RouteValueDictionary routeValues = new RouteValueDictionary();
                foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                {
                    routeValues[key] = filterContext.HttpContext.Request.QueryString[key].ToString();
                }

                // Add the return action to the route values
                routeValues.Add("ReturnAction", filterContext.RouteData.Values["action"].ToString());
                routeValues.Add("ReturnController", filterContext.RouteData.Values["controller"].ToString());
                routeValues.Add("returnUrl", "/" + filterContext.RouteData.Values["controller"].ToString() + "/" + filterContext.RouteData.Values["action"].ToString());

                // Build a new URL which we will be redirecting to
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                string url = helper.Action("Login", "Account", routeValues);

                // Return a RedirectResult based on the URL we constructed
                ActionResult result = new RedirectResult(url);
                return result;
            }
            else
            {
                // No RBAC Session ID was included in the query parameters, so no further action is required, and no ActionResult needs to be returned
                return null;
            }
        }
    }
    #endregion


    public class RBACAdminRightsOrLocalHost : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // If the request is being made on local host, we will allow it (assumed admin access)
            if (filterContext.RequestContext.HttpContext.Request.IsLocal == true)
            {
                return;
            }

            // Do base method that will check to see if a user is even logged on
            base.OnAuthorization(filterContext);

            // If we are authenticated, we will check for sufficient admin rights in RBAC
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = EnsureUserHasSufficientAdminRights(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }


        private ActionResult EnsureUserHasSufficientAdminRights(AuthorizationContext filterContext)
        {
            bool sufficientRights = UserHasRBACAdminRights(filterContext);

            if (sufficientRights == false)
            {
                // Insufficient rights, so we want a result that redirects to error screen

                // Build a RouteValueDictionary containing the values derived from the passed collection 
                RouteValueDictionary routeValues = new RouteValueDictionary();
                foreach (string key in filterContext.HttpContext.Request.QueryString.Keys)
                {
                    routeValues[key] = filterContext.HttpContext.Request.QueryString[key].ToString();
                }

                // Build a new URL which we will be redirecting to 
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);
                string url = helper.Action("AccessDenied", "Error", routeValues);

                // Return a RedirectResult based on the URL we constructed
                ActionResult result = new RedirectResult(url);
                return result;

                // DEBUG: Another possibility?
                /*
                result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                */
            }
            else
            {
                // Selected customer and sufficient access are validated, so we will return a null action so calling action knows normal flow can proceed
                return null;
            }
        }

        private bool UserHasRBACAdminRights(AuthorizationContext filterContext)
        {
            bool userHasAdminAccess = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Admin");

            if (userHasAdminAccess == false)
                userHasAdminAccess = MvcApplication1.RBAC.RoleProvider.IsUserInRole(filterContext.HttpContext.User.Identity.Name, "Engineering");

            return userHasAdminAccess;
        }
    }

}