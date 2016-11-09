using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Duncan.PEMS.Business.Roles;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;
using User = Duncan.PEMS.Entities.Users.User;

namespace Duncan.PEMS.Business.Users
{
    public class UserFactory : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        private  SecurityManager _secMgr;
        private  SecurityManager SecurityMgr
        {
            get { return _secMgr ?? (_secMgr = new SecurityManager()); }
        }

        #region "List User Models"

        /// <summary>
        /// Gets a list of all user models in the system
        /// </summary>
        /// <returns></returns>
        public  List<ListUserModel> GetUsersListModels(PemsCity currentCity)
        {
            //create a list of user models for the city passed in
            var userNamesForThisCity = SecurityMgr.GetUsersForCity(currentCity.InternalName);
            var userProfiles = RbacEntities.UserProfiles.Where(x => userNamesForThisCity.Contains(x.UserName)).ToList();
            return userProfiles.Select(x => MakeListUserModel(x, currentCity)).ToList();
        }

        /// <summary>
        /// Gets a string representation of the users status (Active, Suspended, Terminated)
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        private  string GetStatus(bool active, string username)
        {
            if (!active)
                return "Terminated";

            if (WebSecurity.IsAccountLockedOut(username, Constants.Security.NumPasswordFailuresBeforeLockout,
                                               Constants.Security.NumSecondsToLockout))
                return "Suspended";
            return "Active";
        }

        /// <summary>
        /// Maks a listusermodel object from a user profile object
        /// </summary>
        /// <param name="userProfile"></param>
        /// <param name="currentCity"></param>
        /// <returns></returns>
        private  ListUserModel MakeListUserModel(UserProfile userProfile, PemsCity currentCity)
        {
            var userModel = new ListUserModel
                {
                    CreationDate = userProfile.CreatedDate,
                    FirstName = userProfile.FirstName,
                    LastLoginDate = GetLastLoginTime(userProfile.UserId),
                    LastName = userProfile.LastName,
                    MiddleInitial = userProfile.MiddleName,
                    Username = userProfile.UserName,
                    UserId = userProfile.UserId,
                    Active = false,
                    BadLoginCount = 0
                };

            //update memebership information
            if (userProfile.Membership != null)
            {
                userModel.Active = userProfile.Membership.IsActive.HasValue && userProfile.Membership.IsActive.Value;
                userModel.BadLoginCount = userProfile.Membership.PasswordFailuresSinceLastSuccess;
            }
            //role
            userModel.Role = SecurityMgr.GetGroupForUser(currentCity, userModel.Username);

            //status
            userModel.Status = GetStatus(userModel.Active, userModel.Username);



            return userModel;
        }

        #endregion

        #region "Get User Details"
        public  int GetUserId(string userName)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == userName);
            return userProfile != null ? (userProfile.UserId) : Constants.User.InvalidUserId;
        }

        public string GetUsername(int userId)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserId == userId);
            return userProfile != null ? (userProfile.UserName) : "";
        }

        public  int GetPasswordExpirationInDays()
        {
            var username = WebSecurity.CurrentUserName;
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            if (userProfile == null)
                return -1;

            //get the last password change date
            var pwChangeDate = userProfile.Membership.PasswordChangedDate;

            //get when the password expires
            int daysPwValidFor;
            var validForConfig = ConfigurationManager.AppSettings["DaysPWValidFor"];
            int.TryParse(validForConfig, out daysPwValidFor);
            if (daysPwValidFor == 0)
                daysPwValidFor = 90;
            var dayPwExpires = pwChangeDate.Value.AddDays(daysPwValidFor);

            //if it has expires, force them to change it on next login
            if (dayPwExpires.Date <= DateTime.Now.Date)
            {
                //flip the RequirePasswordREseet flag on the user profile

                userProfile.RequirePasswordReset = true;
                RbacEntities.SaveChanges();
            }

            //return the number of days before it expires - the number of days btw now and the expiration date
            TimeSpan expirationTime = dayPwExpires - DateTime.Now;
            return expirationTime.Days < 0 ? 0 : expirationTime.Days;
        }

        /// <summary>
        /// Checks to see if the user has an email, and returns active or not
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public  bool UserHasEmail(string username)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            bool isActive = userProfile != null && (!String.IsNullOrEmpty(userProfile.Email));
            return isActive;
        }

        /// <summary>
        /// gets a username from a users email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public  string GetUsernameFromEmail(string email)
        {
            var userProfile =
                RbacEntities.UserProfiles.FirstOrDefault(u => u.Email.Trim().ToLower() == email.Trim().ToLower());
            if (userProfile != null)
                return userProfile.UserName;
            return String.Empty;
        }

        /// <summary>
        /// Checks to see if the user needs to reset their password
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public  bool RequiresPasswordReset(string username)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            return userProfile != null && userProfile.RequirePasswordReset.GetValueOrDefault(true);
        }

        /// <summary>
        /// Checks to see if the user is active.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public  bool IsUserActive(string username)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            bool isActive = userProfile != null &&
                            (userProfile.Membership != null && ((userProfile.Membership.IsActive ?? false)));
            return isActive;
        }



        /// <summary>
        /// Checks to see if the user is active.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserActive(int userId)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserId == userId);
            bool isActive = userProfile != null &&
                            (userProfile.Membership != null && ((userProfile.Membership.IsActive ?? false)));
            return isActive;
        }

        /// <summary>
        /// Checks to see if htey user exist int he system
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public  bool DoesUserExist(string username)
        {
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            if (userProfile != null)
                return true;
            return false;
        }

        /// <summary>
        /// Gets the last login time for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public  DateTime GetLastLoginTime(int userId)
        {
            var setting = (new SettingsFactory()).Get(userId, Constants.User.LastLoginTime);
            if (setting != null)
                return Convert.ToDateTime(setting.Value);
         return DateTime.MinValue;
        }

        /// <summary>
        /// GEts a custom user object
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public  User GetUserById(int? userId)
        {
            User user;
            var userProfile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserId == userId);
            if (userProfile != null)
            {
                user = new User()
                    {
                        Id = userProfile.UserId,
                        Username = userProfile.UserName,
                        FirstName = userProfile.FirstName,
                        MiddleName = userProfile.MiddleName,
                        LastName = userProfile.LastName,
                        Phone = userProfile.Phone,
                        Email = userProfile.Email
                    };
            }
            else
            {
                user = new User();
            }

            return user;
        }




        #endregion

        #region Create / Update / Delete

        /// <summary>
        /// Creates the user in the system and returns the system generated username
        /// </summary>
        public string CreateUser(UserModel userModel, string storeName, string password, bool active)
        {
            return CreateUser( userModel, storeName, password, active, false );
        }

        /// <summary>
        /// Creates the user in the system and returns the system generated username
        /// </summary>
        /// <param name="useModelUserName">If true then try to use <see cref="UserModel"/> instance Username if it exists.  Otherwise fall back to building a user name. </param>
        public string CreateUser(UserModel userModel, string storeName, string password, bool active, bool useModelUserName)
        {
            var authorizationManager = new AuthorizationManager(storeName);

            string username = CreateUniqueUserName(userModel.FirstName, userModel.MiddleInitial, userModel.LastName);
            username = ( useModelUserName && !string.IsNullOrWhiteSpace( userModel.Username )) ? userModel.Username : username;

            //register the user
            //try to get the dbuser, and if they arlready exists, just create the account, otherwise create the user and account.
            if (!authorizationManager.InAuthorizationSystem(username))
            {
                WebSecurity.CreateUserAndAccount(username,
                                                 password,
                                                  
                                                 new
                                                 {
                                                     FirstName = userModel.FirstName,
                                                     MiddleName = userModel.MiddleInitial,
                                                     LastName = userModel.LastName,
                                                     Phone = userModel.PhoneNumber,
                                                     Email = userModel.EmailAddress,
                                                     CreatedDate = DateTime.Now,
                                                     RequirePasswordReset = true
                                                 });
            }
            else
            {
                //only create them if they dont already exist
                if (!WebSecurity.UserExists(username))
                    //create them
                    WebSecurity.CreateAccount(username, password);
            }

            //update their profile 
            //set the models username now that we have it
            userModel.Username = username;

            UpdateUserProfile(userModel, active);
            //now force them to change their pw on next login
            UpdateUserPasswordReset(username, true);

            //add the password to the password history so they cant set it back ot the default
            var pwMGr = new PasswordManager(username);
            pwMGr.AddPasswordToHistory(password);

            //now add them to the default group for this store they will have access to
            authorizationManager.AddGroupMember(Constants.Security.DefaultStoreGroupName, username);

            //update the caching table to include this person
            int userID = GetUserId(userModel.Username);
            authorizationManager.AddCustomerAccess(userID);

            //have to check to see if groupname is null - otherwise we throw an error
            if (!string.IsNullOrEmpty(userModel.Role))
                // now add them to the specific group for this store they will have access to
                authorizationManager.AddGroupMember(userModel.Role, username);

            return username;
        }


        public void DeleteUser(string username)
        {
            var authorizationManager = new AuthorizationManager();
            //we are not deleting the user, just setting as inactive
            //AZMan
            authorizationManager.DeleteUser(username);

            //now in our membership
        }

        /// <summary>
        /// Updates a user's profile
        /// </summary>                                                                                                 
        public void UpdateUserProfile(UserModel model, bool isActive)
        {
            var profile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == model.Username);
            if (profile == null) return;
            profile.FirstName = model.FirstName;
            profile.LastName = model.LastName;
            profile.MiddleName = model.MiddleInitial;
            profile.Email = model.EmailAddress;
            profile.Phone = model.PhoneNumber;
            profile.Membership.IsActive = isActive;
            RbacEntities.SaveChanges();

            //now set the settings that arent a part of the profile(company name, secondary type id and value, etc
            int userID = GetUserId(model.Username);
            var settingsFactory = new SettingsFactory();
            settingsFactory.Set(userID, Constants.User.OrganizaitonNameField, model.OrganizationName);
            settingsFactory.Set(userID, Constants.User.SecondaryIDType, model.SecondaryIDType);
            settingsFactory.Set(userID, Constants.User.SecondaryIDValue, model.SecondaryIDValue);

        

        }
        /// <summary>
        /// Updates a user's profile
        /// </summary>                                                                                                 
        public void UpdateUserStatus(string userName, bool active)
        {
            var profile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == userName);
            if (profile == null) return;
            profile.Membership.IsActive = active;
            RbacEntities.SaveChanges();
        }

        /// <summary>
        /// Updates a user to update their PW reset value
        /// </summary>                                                                                                 
        public void UpdateUserPasswordReset(string username, bool requirePasswordReset)
        {
            var profile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            if (profile == null) return;
            profile.RequirePasswordReset = requirePasswordReset;
            RbacEntities.SaveChanges();
        }

        /// <summary>
        /// Updates a user's profile
        /// </summary>                                                                                                 
        public void UnLockUser(string username)
        {
            var profile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            if (profile != null)
            {
                profile.Membership.PasswordFailuresSinceLastSuccess = 0;
                RbacEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Generate a user name ffrom first, middle inital and last name.
        /// Add a number to make it unique if required.
        /// </summary>
        /// <param name="firstName">User first name</param>
        /// <param name="middleName">User middle initial or name</param>
        /// <param name="lastName">User last name</param>
        /// <returns></returns>
        private string CreateUniqueUserName(string firstName, string middleName, string lastName)
        {
            //we are using First Intial, Middle Initial, Last name
            int indexer = 1;
            var baseName = new StringBuilder();
            baseName.Append(firstName[0]);
            if (middleName != null)
                baseName.Append(middleName[0]);
            baseName.Append(lastName);

            // Does this user name already exist?
            string userName = baseName.ToString();
            while (true)
            {
                UserProfile up = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == userName);
                if (up == null || up.LastName == null)
                    break;
                userName = baseName.ToString() + indexer++;
            }

            return userName;
        }

        /// <summary>
        /// Gets the users in the system. if the city is passed in, then get that cities users city
        /// </summary>
        /// <param name="city"></param>
        public List<UserModel> GetUsers(string city = "")
        {
            var userProfiles = RbacEntities.UserProfiles.ToList();
            if (userProfiles.Any())
                return
                    userProfiles.Select(
                        x =>
                        new UserModel
                            {
                                EmailAddress = x.Email,
                                FirstName = x.FirstName,
                                LastName = x.LastName,
                                MiddleInitial = x.MiddleName,
                                PhoneNumber = x.Phone,
                                Username = x.UserName
                            }).ToList();
            return null;

        }

        #endregion


        #region "Profile"
        /// <summary>
        /// Gets a users profile
        /// </summary>
        public  ProfileModel GetProfileModel(string username, PemsCity CurrentCity)
        {
            //populate the fields from the database
            var profile = RbacEntities.UserProfiles.FirstOrDefault(u => u.UserName == username);
            if (profile != null)
            {
                var model = new ProfileModel
                {
                    Groups = (new RoleFactory()).GetGroups(username, CurrentCity),
                    Username = username,
                    EmailAddress = profile.Email,
                    MiddleInitial = profile.MiddleName,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    PhoneNumber = profile.Phone,
                    Active = false
                };

                //update memebership information
                if (profile.Membership != null)
                {
                    model.Active = profile.Membership.IsActive.HasValue && profile.Membership.IsActive.Value;
                    model.BadLoginCount = profile.Membership.PasswordFailuresSinceLastSuccess;
                    model.CreationDate = profile.Membership.CreateDate.HasValue ? profile.Membership.CreateDate.Value : DateTime.MinValue;
                    model.LastLoginFailure = profile.Membership.LastPasswordFailureDate.HasValue ? profile.Membership.LastPasswordFailureDate.Value : DateTime.MinValue;
                    model.LastPasswordChangeDate = profile.Membership.PasswordChangedDate.HasValue ? profile.Membership.PasswordChangedDate.Value : DateTime.MinValue;
                    int daysPwValidFor;
                    var validForConfig = ConfigurationManager.AppSettings["DaysPWValidFor"];
                    int.TryParse(validForConfig, out daysPwValidFor);
                    if (daysPwValidFor == 0)
                        daysPwValidFor = 90;
                    model.PasswordExipration = profile.Membership.PasswordChangedDate != null ? profile.Membership.PasswordChangedDate.Value.AddDays(daysPwValidFor) : DateTime.Now;
                }
                //role
                model.Role = SecurityMgr.GetGroupForUser(CurrentCity, model.Username);

                //status
                model.Status = GetStatus(model.Active, model.Username);

                //user settings
                var settingsFactory = new SettingsFactory();

                var lastLogin = settingsFactory.GetValue(profile.UserId, Constants.User.LastLoginTime);
                model.LastLoginDate = lastLogin != null ? Convert.ToDateTime(lastLogin) : DateTime.MinValue;

                var orgName = settingsFactory.GetValue(profile.UserId, Constants.User.OrganizaitonNameField);
                model.OrganizationName = orgName ?? string.Empty;

                var secType = settingsFactory.GetValue(profile.UserId, Constants.User.SecondaryIDType);
                model.SecondaryIDType = secType ?? string.Empty;

                var secValue = settingsFactory.GetValue(profile.UserId, Constants.User.SecondaryIDValue);
                model.SecondaryIDValue = secValue ?? string.Empty;

                //is technician
                var isTech = settingsFactory.GetValue(profile.UserId, Constants.User.IsTechnician);
                model.IsTechnician = isTech != null && Convert.ToBoolean(isTech);

                //var techID = settingsFactory.GetValue(profile.UserId, Constants.User.TechnicianId);
                //model.TechnicianId = techID != null ? Convert.ToInt32(techID) : -1;

                //then fill in all the profile specific stuff
                var passwordManager = new PasswordManager(username);
                model.PasswordResetRequired = RequiresPasswordReset(username);

                List<PasswordQuestion> questions = passwordManager.GetQuestions();
                model.Question1 = questions.Count > 0 ? questions[0] : new PasswordQuestion(1);
                model.Question2 = questions.Count > 1 ? questions[1] : new PasswordQuestion(2);

                // Set in a dummy password
                model.Password = new ChangePasswordModel();

                return model;
            }

            return null;

        }

        #endregion
    }
}
