using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using Duncan.PEMS.Web.Mailers;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Web.Controllers
{
    /// <summary>
    /// The <see cref="AccountController"/> class 
    /// </summary>
    [Authorize]
    public class AccountController : BaseController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Login / Logout

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login()
        {
            //throw new System.InvalidOperationException("This is a test error to see if the pem controller will work for error trapping purposes.", new Exception("THIS IS AN INNER EXCEPTION"));
            // If already logged in, redirect to landing page
            if ( WebSecurity.IsAuthenticated )
                return SendToLandingPage();
            return View();
        }

        [AllowAnonymous]
        public ActionResult Terms()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Login(string userName, string password)
        {
            var secMgr = new SecurityManager();

            // Log login attempts.  This can be enabled/disabled in the web.config.
            //  <appSettings>
            //    <add key="pems.logging.log_attempts" value="true" />
            //  </appSettings>
            secMgr.LogLogin( userName, password, Request.Url.ToString());


            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                // First check if account is locked and if so display message to user
                if (WebSecurity.IsAccountLockedOut(userName, Constants.Security.NumPasswordFailuresBeforeLockout,
                                                   Constants.Security.NumSecondsToLockout))
                {
                    ModelState.AddModelError("", "This account is locked due to too many failed login attempts. Please contact your system administrator to unlock.");
                    return View();
                }

                var userFactory = new UserFactory();
                // Check if account is disabled
                if (!userFactory.DoesUserExist(userName))
                {
                    ModelState.AddModelError("", "An account with that username does not exist.");
                    return View();
                }


                if (!userFactory.IsUserActive(userName))
                {
                    ModelState.AddModelError("", "This account has been disabled. Please contact your system administrator to enable this account.");
                    return View();
                }

                // Finally, account is not locked and is not disabled so attempt to login
                if (secMgr.Login(userName, password))
                {
                    SetCityCookie(userName + "|None|" + CustomerLoginType.Unknown);
                    //update the last login time setting for this user
                    //we have to convert the datetime to sortable pattern
                    var invC = new CultureInfo(""); // Creates a CultureInfo set to InvariantCulture.
                    (new SettingsFactory()).Set(userFactory.GetUserId(userName), Constants.User.LastLoginTime, DateTime.Now.ToString("s", invC));
                    return SendToLandingPage();
                }
                    //login failed
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
                //model is invalid, perform the appropriate checks
            else
            {
                if (string.IsNullOrEmpty(userName))
                    ModelState.AddModelError("", "The User name field is required.");
                if (string.IsNullOrEmpty(password))
                    ModelState.AddModelError("", "The Password field is required.");
            }
            // If we got this far, something failed, redisplay form 

            return View();
        }

        /// <summary>
        /// Logs the currently logged in user off and sends them to the default home page
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            Logout();
            return SendToLoginPage();
        }

        #endregion

        #region Change / Reset Password

        [AllowAnonymous]
        public ActionResult ForgotPassword(ManageMessageId? message)
        {
            ViewBag.StatusMessage = message == ManageMessageId.PasswordEmailed
                                        ? "A link to reset your password has been sent to the email address we have on record for you"
                                        : "";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string userName)
        {
            if ( ModelState.IsValid )
            {
                string username = userName;
                //make sure the user exists
                if (WebSecurity.UserExists(username))
                {
                    //check if they have an email associated with their account.
                    if ((new UserFactory()).UserHasEmail(username))
                    {
                        //display the form that allows them to answer the seciryt questions
                        var pwMgr = new PasswordManager(username);

                        //get the questions for this user
                        var questions = pwMgr.GetQuestions();
                        //if they are both empty, send the user to the customer service page
                        bool validQs = false;
                        foreach (var passwordQuestion in questions)
                            if (!string.IsNullOrEmpty(passwordQuestion.Question))
                            {
                                validQs = true;
                                break;
                            }


                        if (!validQs)
                            //they didnt answer their quesitons correctly, display the system admin contact view.
                            return View("CustomerService", new CustomerSupportModel { Message = "Your security questions have not been defined." });

                        //get a random question
                        var rand = new Random();
                        var randomQuestionIndex = rand.Next(0, 2);
                        var question = questions[randomQuestionIndex];
                        
                        //setting the failure to 0 on the first go around
                        var secModel = new SecurityQuestionsModel
                            {
                                UserName = username,
                                FailureCount = "0",
                                QuestionID = question.QuestionNumber.ToString(),
                                QuestionText = question.Question
                            };

                        return View("SecurityQuestions", secModel);
                    }
                    else
                    {
                        //otherwise, show the customer support page
                        return View("CustomerService", new CustomerSupportModel());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No account with that username found. Please enter a valid username");
                }
            }

            // If we got this far, something failed. redisplay form
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult SecurityQuestions(SecurityQuestionsModel model, string username, string failureCount, string questionID)
        {
            if (ModelState.IsValid)
            {
               // string username = model.UserName;
                if (WebSecurity.UserExists(username))
                {
                    //get the question being tested
                    var pwMgr = new PasswordManager(username);
                    //get the questions for this user
                    var questions = pwMgr.GetQuestions();
                   
                 


                    var question = questions.FirstOrDefault(x => x.QuestionNumber.ToString() == model.QuestionID);

                     //check to see if the answer is valid
                    bool questionMatch = false;
                    if (question != null)
                    {
                        question.Answer = model.QuestionValue;
                        questionMatch = pwMgr.CheckAnswer(question);
                    }

                    //if it is, email the user the link and display the redirect to login view
                    if (questionMatch)
                    {
                        string token = WebSecurity.GeneratePasswordResetToken(username, 10);
                        string email = "";

                        using (var userContext = new PEMRBACEntities())
                        {
                            var profile = userContext.UserProfiles.SingleOrDefault(u => u.UserName == username);
                            if (profile != null)
                            {
                                email = profile.Email;
                            }
                        }

                        if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(token))
                        {
                            // Send password reset email
                            var mailer = new UserMailer();
                            mailer.PasswordReset(token, email).Send();
                        }
                        else
                        {
                            ModelState.AddModelError("",
                                                     "Could not send email at this time. If the problem perists please contact your system administrator");
                        }

                        //if everythign was successful, then we need to return the login redirect view
                        return ReturnLoginRedirectView("You have been emailed a link to reset your password.",
                                                       "Password Reset - Emailed");
                    }
                    
                    //if the question didnt match, and this is the first failure (0), then retry with the other question
                    //also, lets make sure we are telling hte user why they have to answer again
                    if (model.FailureCount == "0")
                    {
                        ModelState.AddModelError("","Incorrect Answer. Please Try Again.");
                        //get the question that we did NOT just ask
                        var unansweredQuestion = questions.FirstOrDefault(x => x.QuestionNumber.ToString() != model.QuestionID);
                        //re-ask them
                     
                        var secModel = new SecurityQuestionsModel
                        {
                            UserName = username,
                            FailureCount = "1",
                            QuestionID = unansweredQuestion.QuestionNumber.ToString(),
                            QuestionText = unansweredQuestion.Question,
                            QuestionValue = string.Empty
                        };

                        return  View("SecurityQuestions", secModel);
                    }
                       
                    //they didnt answer their quesitons correctly, display the system admin contact view.
                    return View("CustomerService", new CustomerSupportModel());
                 
                }
                else
                {
                    ModelState.AddModelError("", "No account with that username found. Please enter a valid username");
                }
            }

            // If we got this far, something failed. redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {
            string token = id;

            using (var userContext = new PEMRBACEntities())
            {
                var user = userContext.Memberships.SingleOrDefault( u => u.PasswordVerificationToken == token );
                if ( user != null && user.PasswordVerificationTokenExpirationDate != null )
                {
                    DateTime expiration = ( (DateTime) user.PasswordVerificationTokenExpirationDate ).ToLocalTime();
                    if ( DateTime.Now > expiration )
                    {
                        // token expired
                        ModelState.AddModelError( "", "This token has expired. You must complete the reset password process within 10 minutes." );
                        var model = new ResetPasswordModel { Expired = true};
                        return View(model);
                    }
                    else
                    {
                        // token is valid, show password reset form
                        var model = new ResetPasswordModel {Token = token, Expired = false};
                        return View( model );
                    }
                }
                else
                {
                    // no user exists with this token
                    return SendToLoginPage();
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if ( ModelState.IsValid )
            {
                //get the user to reset the password for
                using (var userContext = new PEMRBACEntities())
                {
                    var user = userContext.Memberships.SingleOrDefault(u => u.PasswordVerificationToken == model.Token);
                    if (user != null && user.PasswordVerificationTokenExpirationDate != null)
                    {
                        DateTime expiration = ((DateTime)user.PasswordVerificationTokenExpirationDate).ToLocalTime();
                        if (DateTime.Now > expiration)
                        {
                            // token expired
                            ModelState.AddModelError("", "This token has expired. You must complete the reset password process within 10 minutes.");
                            var newModel = new ResetPasswordModel {  Expired = false };
                            return View(newModel);
                        }
                        else
                        {
                            // token is valid, reset their password
                            var passwordManager = new PasswordManager(user.UserProfile.UserName);
                            var returnValue = passwordManager.ChangePassword(model.NewPassword, model.Token);

                            if (returnValue != PasswordManager.ChangeResult.Ok)
                            {
                                ModelState.AddModelError("", passwordManager.LastError);
                                return View(model);
                            }
                                //return ContactSupport(passwordManager.LastError);

                            
                         return ReturnLoginRedirectView("Your password has been reset.", "Password Reset - Success");
                
                        }
                    }
                    else
                    {
                        // no user exists with this token
                        return SendToLoginPage();
                    }
                }
            }

            // If we got this far, something failed. redisplay form
            return View( model );
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            PasswordEmailed
        }

        #endregion

        #region "Support"
        [AllowAnonymous]
        public ActionResult ContactSupport(string message = null)
        {
            return View("CustomerService", new CustomerSupportModel { Message = message});
        }
        #endregion
        
        #region "Forgot Username"


        [AllowAnonymous]
        public ActionResult ForgotUsername()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotUsername(string emailAddress)
        {
            if (ModelState.IsValid)
            {
                //try to find a user with the email address.
                var username = (new UserFactory()).GetUsernameFromEmail(emailAddress);
                //if found, email them their username and show the return to login page
                if (!string.IsNullOrEmpty(username))
                {
                    //email and show the return
                    var mailer = new UserMailer();
                    mailer.ForgotUsername(username, emailAddress).Send();



                    //if everythign was successful, then we need to return the login redirect view
                    return ReturnLoginRedirectView("The username for that account has been emailed.",
                                                   "Forgot Username - Emailed");
                }
                //Otherwise, show the message that email doesnt exist in system

                ModelState.AddModelError("", "We could not find that email address on file.");
            }

            // If we got this far, something failed. redisplay form
            return View();
        }

        #endregion

        #region "Header Links"
        [ChildActionOnly]
        [AllowAnonymous]
        public ActionResult HeaderLinks()
        {
            return PartialView();
        }
        #endregion
    }
}