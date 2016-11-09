using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Web.Areas.maint.Controllers
{
    public class ProfileController : shared.Controllers.ProfileController
    {
        //
        // GET: /maint/Profile/

        #region Mobile Password Reset
        public ActionResult ChangePassword()
        {
            var model = (new UserFactory()).GetProfileModel(User.Identity.Name, CurrentCity);
            //if they are required to change their password, tell them
            if (model.PasswordResetRequired)
            {
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                         (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,
                                                                           "Password Change Required"));
            }
            return View("ChangePassword", model);
        }

        //
        // POST: /admin/Profile/Edit/5

        [ChildActionOnly]
        private ActionResult MobileResetPassword(ProfileModel model)
        {
            TryValidateModel(model.Password);
            if (ModelState.IsValid)
            {
                // check to make sure the passwords have values
                if (string.IsNullOrEmpty(model.Password.NewPassword) ||
                    string.IsNullOrEmpty(model.Password.ConfirmPassword))
                {
                    ModelState.AddModelError("Password",
                                             (new ResourceFactory()).GetLocalizedTitle(
                                                 ResourceTypes.ErrorMessage,
                                                 "Password must be set"));
                    return View(model);
                }
                // Did password change?
                var passwordManager = new PasswordManager(User.Identity.Name);
                //if we are forcing a password change, we need to force the change, update their status to say they dont have it required anymore, then log them out and force another login
                if (model.PasswordResetRequired)
                {
                    var returnValue = passwordManager.ChangePassword(model.Password.NewPassword, false);
                    if (returnValue != PasswordManager.ChangeResult.Ok)
                    {
                        ModelState.AddModelError("Password", passwordManager.LastError);
                        return View(model);
                    }
                    // If here and password reset was required then clear the flag for the user.
                    var uMgr = new UserFactory();
                    uMgr.UpdateUserPasswordReset(User.Identity.Name, false);

                    // Log out.
                    Logout();
                    // Send user to login page
                    return SendToLoginPage();
                }
                else
                {
                    //otherwise, follow the regular PW reset rules
                    PasswordManager.ChangeResult returnValue = passwordManager.ChangePassword(model.Password.NewPassword);
                    if (returnValue != PasswordManager.ChangeResult.Ok)
                    {
                        ModelState.AddModelError("Password", passwordManager.LastError);
                        return View(model);
                    }
                }


                //if we made it here, then the password was reset successfully. clear all the other model errors and add the status message
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                         (new ResourceFactory()).GetLocalizedTitle(
                                             ResourceTypes.StatusMessage,
                                             "Password Change Successful"));

                return View(model);
            }

            //if we got here, something is wrong
            //if they are required to change their password, tell them
            if (model.PasswordResetRequired)
            {
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                         (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,
                                                                           "Password Change Required"));
            }

            //otherwise, rol lthoreugh and add the errors
            List<string> errorsToAdd = ModelState.Values.SelectMany(modelValue => modelValue.Errors).Select(modelError => modelError.ErrorMessage).ToList();

            foreach (var error in errorsToAdd)
                ModelState.AddModelError("Password", (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage, error));

            return View("ChangePassword", model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ProfileModel model, string submitButton, FormCollection formColl)
        {
            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            return (MobileResetPassword(model));
        }
        #endregion

    }
}
