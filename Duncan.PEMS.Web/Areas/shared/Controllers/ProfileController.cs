using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
     [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class ProfileController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        //
        // GET: /admin/Profile/Edit/5

        public ActionResult Edit()
        {
            var model = (new UserFactory()).GetProfileModel(User.Identity.Name, CurrentCity);
            //if they are required to change their password, tell them
            if (model.PasswordResetRequired)
            {
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                         (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,
                                                                           "Password Change Required"));
            }
            return View("Edit", model);
        }

        //
        // POST: /admin/Profile/Edit/5

        [ChildActionOnly]
        private ActionResult ResetPassword(ProfileModel model)
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

            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit(ProfileModel model, string submitButton, FormCollection formColl)
        {
            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Change Password":
                    return (ResetPassword(model));
                    //check the other submit buttons and act on them, or continue
                case "Cancel":
                    return RedirectToAction("Index", "Home");
            }
           
            //also double check to make sure we are not forcing a PW change here
            // Is the password required?
            if (model.PasswordResetRequired && string.IsNullOrEmpty(model.Password.NewPassword))
            {
                ModelState.AddModelError("Password",
                                          (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage,
                                                                           "Password must be set"));
                return View(model);
            }
            //pw is taken care of somewhere else, remove it from the validation
            ModelState.Remove("Password.NewPassword");

            //Update
            if (ModelState.IsValid)
            {
                // Determine what changed on page.
                // Check that the passwords match and that they have not been used yet.
                var passwordManager = new PasswordManager(User.Identity.Name);

                // Did either question/answer change?
                // What is the state of Question 1?
                PasswordQuestion.QuestionState questionState = passwordManager.QuestionState(model.Question1);

                // Switch on state of question.
                switch (questionState)
                {
                    case PasswordQuestion.QuestionState.New:
                        // Set the question number.
                        model.Question1.QuestionNumber = 1;
                        // Store the question
                        passwordManager.SaveQuestion(model.Question1);
                        break;
                    case PasswordQuestion.QuestionState.Changed:
                        // Store the question
                        passwordManager.SaveQuestion(model.Question1);
                        break;
                    case PasswordQuestion.QuestionState.NoChange:
                        // Question/answer was not modified.
                        break;
                    default:
                        // Question and/or answer invalid or missing.
                        ModelState.AddModelError("", "Question 1 must have both a question and an answer.");
                        return View(model);
                }

                // What is the state of Question 2?
                questionState = passwordManager.QuestionState(model.Question2);

                // Switch on state of question.
                switch (questionState)
                {
                    case PasswordQuestion.QuestionState.New:
                        // Set the question number.
                        model.Question2.QuestionNumber = 2;
                        // Store the question
                        passwordManager.SaveQuestion(model.Question2);
                        break;
                    case PasswordQuestion.QuestionState.Changed:
                        // Store the question
                        passwordManager.SaveQuestion(model.Question2);
                        break;
                    case PasswordQuestion.QuestionState.NoChange:
                        // Question/answer was not modified.
                        break;
                    default:
                        // Question and/or answer invalid or missing.
                        ModelState.AddModelError("", "Question 2 must have both a question and an answer.");
                        return View(model);
                }

                //now that we have verified valid questions and answers, we need to save the rest of the users info
                var usrMgr = new UserFactory();
                // update the users profile

                usrMgr.UpdateUserProfile(model, model.Active);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }


   


    }
}
