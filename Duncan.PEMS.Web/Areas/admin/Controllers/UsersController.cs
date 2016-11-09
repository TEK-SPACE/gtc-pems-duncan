using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{

    /// <summary>
    /// Controller to allow control of users for the administrative function.  This
    /// inherits from the shared <see cref="shared.Controllers.UsersController"/>.  Do
    /// not modify this controller.
    /// </summary>
    public class UsersController : shared.Controllers.UsersController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        public ActionResult CustomerRoles(string username)
        {
            //the first thing we have to do is check to see if this user is part of the current city. if it is not, send them back to the index page.
            var userFactory = new UserFactory();
            //now check to se if the requested user exist int he system
            if (!userFactory.DoesUserExist(username))
                return RedirectToAction("Index", "Users");

            //get the model
            var model = userFactory.GetProfileModel(username, CurrentCity);
            if (model == null)
                return RedirectToAction("Index", "Users");

            //now that we have the user prfile, lets build a list of customer role models that contain a list of customers that each have a list of roles
            var crm = new CustomerRoleModel {Username = model.Username, CustomerRoles = new List<CustomerRole>()};

            var secMgr = new SecurityManager();
            var customerNames = secMgr.GetCities();
            foreach (var customerName in customerNames)
            {
                var pemCity = new PemsCity(customerName);
                if (pemCity.IsActive)
                {
                    //need to get the current role the user is assigned to for this city
                  var selectedGroupForCity =  secMgr.GetGroupForUser(pemCity, model.Username);
                    if (selectedGroupForCity == "N/A")
                        selectedGroupForCity = "";
                   // var roles = secMgr.GetGroups(customerName);
                    var roles = new Dictionary<string, string> {{ (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault,
                                                                           "Select One"), ""}};
                    //add the none option
                    var groups = secMgr.GetGroups(customerName);
                    foreach (var group in groups)
                    {
                        if (!roles.ContainsKey(group.Key))
                            roles.Add(group.Key, group.Key);
                    }
                    var custModel = new CustomerRole { Customer = pemCity, Roles = roles, CurrentRole = selectedGroupForCity, CustomerInternalName = pemCity.InternalName };
                    crm.CustomerRoles.Add(custModel);
                }
            }

          
            return View(crm);
        }


        [HttpPost]
        public ActionResult CustomerRoles(CustomerRoleModel model, string submitButton)
        {

            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Cancel":
                    return RedirectToAction("Index", "Users");
            }
            int userID = (new UserFactory()).GetUserId(model.Username);
             //remove all caching table data for this user
            (new UserCustomerAccessManager()).ClearCustomerAccess(userID);


            //first, we HAVE to loop horugh and remove all the groups before we add. This way when adding maintneance groups, the _maint group gtes added correctly.
            foreach (var customerRole in model.CustomerRoles)
            {
                //get a auth manager for this customer
                var authMan = new AuthorizationManager(customerRole.CustomerInternalName);

                //remove the user from all the groups in this customer
                authMan.RemoveMemberGroups(model.Username);


                //remove them from maintenance as well. this is the only place we do this since this is the only place they can be added to a maint gorup without being set as a technician. 
                //IE - only applies to administrative users.
                authMan.RemoveMemberFromMaintenanceGroup(model.Username);
                //now we have to clear the menu cache
                HttpContext.Cache.Remove("__PemsMenu" + customerRole.CustomerInternalName.Trim() + model.Username.Trim());
            }

            //now add the correct roles
            foreach (var customerRole in model.CustomerRoles)
            {

                // now add them to the specific group for this store they will have access to
                //only need to do this when there is a valid group name (not empty)
                if (customerRole.CurrentRole != (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Select One"))
                {
                    var authMan = new AuthorizationManager(customerRole.CustomerInternalName);
                    authMan.AddGroupMember(customerRole.CurrentRole, model.Username, false);
                    //also update the caching table - they have access to one role, so they have access to the site.
                    authMan.AddCustomerAccess(userID);

                    //we also have to add them to the _maintenance Role if this customer is a maintenance group 
                    //IE - we are giving any duncan admin users (which are the only ones using this page) 
                    //that have any access to this maintenance group a  _maintenance role as well, so they can log into the handheld.

                    //have to setup the city first
                    customerRole.Customer = new PemsCity(customerRole.CustomerInternalName);
                    if (customerRole.Customer.CustomerType == CustomerProfileType.MaintenanceGroup)
                        (new TechnicianFactory()).AddAdminTechnicianToMaintenanceGroup(model.Username, customerRole.Customer, userID);
                }
            }

            return RedirectToAction("Index", "Users");
        }


    }
}
