using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        public ActionResult Landing()
        {
            //check to see if they have a city. if they do, then send them to the correct city homepage.
            var cityCookie = GetCityCookie();
            var userFactory = new UserFactory();
            //if they have a cookie (which they should at this point
            if (cityCookie != null)
            {
                //check to see if a city is set, if it is, then send them to that city
                string username = WebSecurity.CurrentUserName;
                string emptyCookieValue = username + "|None|" + CustomerLoginType.Unknown;
                if (cityCookie.Value != emptyCookieValue)
                    return SendToCityHomePage(cityCookie.Value.Split('|')[1]);
            }
                //if the cookie is null, send them to the login page
            else
                return SendToLoginPage();

            ViewBag.PWExpiration = userFactory.GetPasswordExpirationInDays();
            var model = new LandingDropDownModel();
            var secMgr = new SecurityManager();
            var userCities = secMgr.GetCitiesForUser(WebSecurity.CurrentUserName);

            // Need to check if CustomerProfile.Status == CustomerStatus.Active
            var rbacEntities = new PEMRBACEntities();
            var landingItems = new List<LandingDropDownItem>();

            foreach (var userCity in userCities)
            {
                var customerProfile = rbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == userCity.Id);
                if (customerProfile != null && customerProfile.Status == (int) CustomerStatus.Active)
                {
                    landingItems.Add(new LandingDropDownItem
                    {
                        Text = userCity.DisplayName,
                        Value = userCity.InternalName,
                        LoginType = CustomerLoginType.Customer
                    });

                    var _secMgr = new SecurityManager();
                    //if this is a amaintenance group, then you need to add all of the active 
                    if (userCity.CustomerType == CustomerProfileType.MaintenanceGroup)
                    {
                        //t add the maint group customers login optoipns
                        foreach (var maintCustomer in userCity.MaintenanceCustomers.Where(x=>x.IsActive))
                        {
                            //only add this option if they are a technician.
                            //first, go check to see if they have access ot this customer - 
                            var cityGroups = _secMgr.GetGroupsForUser(maintCustomer, WebSecurity.CurrentUserName, true);
                            var authMgr = new AuthorizationManager(maintCustomer);
                            var storeRole = authMgr.GetMaintenanceUsersForStore(maintCustomer.InternalName);

                            var isMaintGroup = storeRole.Any(x => x == Constants.Security.DefaultMaintenanceGroupName);

                            //then test for the _maintenance group
                            var isPartOfMainGroup = cityGroups.Any(x => x.Key == Constants.Security.DefaultMaintenanceGroupName && x.Value );
                            if (isPartOfMainGroup || isMaintGroup)
                            {
                                landingItems.Add(new LandingDropDownItem
                                    {
                                        Text = maintCustomer.DisplayName,
                                        Value = maintCustomer.InternalName,
                                        LoginType = CustomerLoginType.MaintenanceGroupCustomer
                                    });
                            }
                        }
                    }
                }
            }

            //if the user only has one city, set their cookie and send them to the city homepage
            if (landingItems.Count == 1)
            {
                //set the cookie and send them to ttheir new homepage.
                var city = landingItems.FirstOrDefault();
                SetCityCookie(WebSecurity.CurrentUserName + "|" + city.Value + "|" + city.LoginType);
                return SendToCityHomePage(city.Value);
            }

            model.Items = landingItems;
            return View(model);
        }

        [HttpPost]
        public ActionResult Landing(LandingDropDownModel model, string hiddenSelectedClient)
        {
            //set the cookie and send them to ttheir new homepage.
            //the hiddenselectedclient is pipe seperated, with the value first, then the login type. need to check the login type and send them either to the maintenance version or the admin version.
            //first one is internal name of city, second is the type (customer, maintenance group) 
            SetCityCookie(WebSecurity.CurrentUserName + "|" + hiddenSelectedClient);
            return SendToLandingPage();
        }

        public ActionResult Error()
        {
            return View();
        }

    }

}