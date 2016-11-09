using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Utilities;
using CustomerSettingsFactory = Duncan.PEMS.Business.Customers.SettingsFactory;
using UserSettingsFactory = Duncan.PEMS.Business.Users.SettingsFactory;

namespace Duncan.PEMS.Web.Areas.maint.Controllers
{
    public class UsersController : shared.Controllers.UsersController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Edit(ProfileModel model, string submitButton, string password, string status, FormCollection formColl)
        {
            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            //we dont care about the validation for questions and answers, so remove them from the model state
            ModelState.Remove("Question1.Question");
            ModelState.Remove("Question1.Answer");
            ModelState.Remove("Question2.Question");
            ModelState.Remove("Question2.Answer");
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Change Password":
                    return (ResetPassword(model));
                case "Clear Password History":
                    return (ClearPasswordHistory(model));
                case "Unlock User":
                    return (UnlockUser(model));
                //check the other submit buttons and act on them, or continue
                case "Cancel":
                    return RedirectToAction("Index", "Users");
                case "Customer Roles":
                    return RedirectToAction("CustomerRoles", "Users", new { username = model.Username });
            }

            //case "Save":
            ModelState.Remove("Password.NewPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var usrMgr = new UserFactory();
                    // update the users profile

                    bool active = status == "Active";
                    usrMgr.UpdateUserProfile(model, active);
                    //now we have to check to see if they are a technician. if they are, create a technician in the pems db
                    var settingsFactory = new SettingsFactory();
                    int userId = usrMgr.GetUserId(model.Username);
                    settingsFactory.Set(userId, Constants.User.IsTechnician, model.IsTechnician.ToString());
                   
                    (new TechnicianFactory()).SetTechnician(model, CurrentCity);
                    //clear all their group permissions
                    CurrentAuthorizationManager.RemoveMemberGroups(model.Username);

                    // now add them to the specific group for this store they will have access to
                    CurrentAuthorizationManager.AddGroupMember(model.Role, model.Username);

                    // send them back to the user listing page.
                    return RedirectToAction("Index", "Users");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not update users profile.");
                }
            }
            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups(model.Username);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Create(UserModel model, string status, string submitButton)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Cancel":
                    return RedirectToAction("Index", "Users");
            }

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    var usrMgr = new UserFactory();

                    //go get the default password for the current city.
                    var settings = (new CustomerSettingsFactory()).Get("DefaultPassword", CurrentCity.Id);
                    string defaultPassword;
                    if (settings.Any())
                        defaultPassword = settings[0].Value;
                    else
                    {
                        ModelState.AddModelError("DefaultPassword", "No default password defined for customer.");
                        model.Groups = GetGroups();
                        return View(model);
                    }

                    //if the middle name is empty, set it to a default
                    if (string.IsNullOrEmpty(model.MiddleInitial))
                        model.MiddleInitial = Constants.User.DefaultMiddleName;

                    //get the phone to empty string, since int he DB it shouldnt be null
                    if (string.IsNullOrEmpty(model.PhoneNumber))
                        model.PhoneNumber = "";

                    bool active = status == "Active";
                    //create the user, set their profile information, and add them to the default group for this store
                    string username = usrMgr.CreateUser(model, CurrentCity.InternalName, defaultPassword, active);
                    var settingsFactory = new SettingsFactory();
                    int userId = usrMgr.GetUserId(model.Username);
                    settingsFactory.Set(userId, Constants.User.IsTechnician, model.IsTechnician.ToString());
                    (new TechnicianFactory()).SetTechnician(model, CurrentCity);
                    return RedirectToAction("Index", "Users");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", HelperMethods.ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups();
            return View(model);
        }
    }
}
