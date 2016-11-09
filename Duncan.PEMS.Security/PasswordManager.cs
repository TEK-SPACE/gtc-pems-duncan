using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Users;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Security
{
    public class PasswordManager
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private byte[] _salt;
        private int _saltLength;
        private string _userName;
        private int _userId;

        /// <summary>
        /// Description of last error.
        /// </summary>
        public string LastError { get; private set; }

        public enum ChangeResult
        {
            Ok = 0,
            GeneralError = 1,
            MatchesUserName = 2,
            AlreadyUsed = 3,
            OnlyOneChangePerDay = 4, 
            TokenResetFailed = 5
        }

        public PasswordManager(string userName)
        {
            _userName = userName;

            // Get user id.
            var rbacEntities = new PEMRBACEntities();
            var user = (from userProfiles in rbacEntities.UserProfiles
                        where userProfiles.UserName == _userName
                        select userProfiles).SingleOrDefault();

            _userId = user != null ? user.UserId : -1;


            _saltLength = 32;
            _salt = new byte[_saltLength];
        }

        public PasswordManager(int userId)
        {
            _userId = userId;

            // Get user id.
            var rbacEntities = new PEMRBACEntities();
            var user = (from userProfiles in rbacEntities.UserProfiles
                        where userProfiles.UserId == _userId
                        select userProfiles).SingleOrDefault();

            _userName = user != null ? user.UserName : string.Empty;

            
            
            _saltLength = 32;
            _salt = new byte[_saltLength];

        }


        /// <summary>
        /// Attempts to change the user password.  Adds the password to user password history.  
        /// Hashes password appropriately.
        /// </summary>
        /// <param name="password">New password.</param>
        public ChangeResult ChangePassword(string password, bool adminOverride = false)
        {
            // Can this password be used?
            ChangeResult changeResult = CheckPasswordHistory( password );
            
            if ( changeResult == ChangeResult.Ok || adminOverride )
            {
                // Change password in WebSecurity.  Do this via the token system.
                // Get a "forgot password" token.
                string token = WebSecurity.GeneratePasswordResetToken(_userName, 10);
                changeResult = ChangePassword( password, token, adminOverride );
            }

            SetLastError(changeResult);
            return changeResult;
        }

        /// <summary>
        /// Attempts to change the user password.  Adds the password to user password history.  
        /// Hashes password appropriately.
        /// </summary>
        /// <param name="password">New password.</param>
        /// <param name="token">Token from WebSecurity.GeneratePasswordResetToken</param>
        public ChangeResult ChangePassword(string password, string token, bool adminOverride = false)
        {
            // Can this password be used?
            ChangeResult changeResult = CheckPasswordHistory( password );

            if (changeResult == ChangeResult.Ok || adminOverride)
            {
                changeResult = WebSecurity.ResetPassword( token, password ) ? ChangeResult.Ok : ChangeResult.TokenResetFailed;

                if ( changeResult == ChangeResult.Ok )
                {
                    //dont update the pw history if the admin is doing it
                    if (!adminOverride)
                        AddPasswordToHistory(password);

                    //we also need to reset their password attempts here
                    var rbacEntities = new PEMRBACEntities();
                    var membership = rbacEntities.Memberships.FirstOrDefault(x => x.UserId == _userId);
                    if (membership != null)
                    {
                        membership.PasswordFailuresSinceLastSuccess = 0;
                        rbacEntities.SaveChanges();
                    }
                }
            }

            SetLastError(changeResult);
            return changeResult;
        }

        private void SetLastError(ChangeResult changeResult)
        {
            switch (changeResult)
            {
                case ChangeResult.AlreadyUsed:
                    LastError = "Password has already been used.";
                    break;
                case ChangeResult.GeneralError:
                    LastError = "General error checking password.";
                    break;
                case ChangeResult.MatchesUserName:
                    LastError = "Password same as user name.";
                    break;
                case ChangeResult.OnlyOneChangePerDay:
                    LastError = "Password may only be changed once in a 24-hour period.";
                    break;
                case ChangeResult.TokenResetFailed:
                    LastError = "Password could not be changed. If the problem persists please contact your system administrator";
                    break;
                default:
                    LastError = "";
                    break;
            }
        }


        /// <summary>
        /// Adds the password to user password history.  Hashes password appropriately.
        /// </summary>
        /// <param name="password">Password to add to history.</param>
        public void AddPasswordToHistory(string password)
        {
            var rbacEntities = new PEMRBACEntities();

            rbacEntities.UserPasswordHistories.Add(
                new UserPasswordHistory {Password = EncryptionManager.Hash( password, _salt ), UserId = _userId, ChangeDate = DateTime.Now}
                );
            rbacEntities.SaveChanges();
        }

        /// <summary>
        /// Adds the password to user password history.  Hashes password appropriately.
        /// </summary>
        public void ClearPasswordHistory()
        {
            var rbacEntities = new PEMRBACEntities();
            var oldPasswords = (from pwdHistory in rbacEntities.UserPasswordHistories
                                orderby pwdHistory.ChangeDate descending
                                where pwdHistory.UserId == _userId
                                select pwdHistory).ToList();

            foreach (var oldPassword in oldPasswords)
                rbacEntities.UserPasswordHistories.Remove(oldPassword);
            rbacEntities.SaveChanges();
        }


        private ChangeResult CheckPasswordHistory(string password)
        {
            // Does password match user name?
            if (password.Trim().Equals(_userName, StringComparison.CurrentCultureIgnoreCase))
                return ChangeResult.MatchesUserName;

            // Has pasword already been used?
            var rbacEntities = new PEMRBACEntities();

            var oldPasswords = (from pwdHistory in rbacEntities.UserPasswordHistories
                        orderby pwdHistory.ChangeDate descending
                        where pwdHistory.UserId == _userId
                        select pwdHistory).Take(5);

            string passwordHash = EncryptionManager.Hash( password, _salt );

            foreach (var oldPassword in oldPasswords)
            {
                if(oldPassword.Password.Equals(passwordHash))
                    return ChangeResult.AlreadyUsed;

                // If user has just been created then allow password change even though 
                // before the 24 hour mark.
                var userProfile = rbacEntities.UserProfiles.FirstOrDefault( m => m.UserName.Equals( _userName ) );
                if ( userProfile == null )
                {
                    // This should never happen but...
                    return ChangeResult.GeneralError;
                }

                // Is user more than 24 hours old then enforce the 24-hour rule.
                if ( ( DateTime.Now - userProfile.CreatedDate ).TotalHours > 24.0 )
                {
                    if ( ( DateTime.Now - oldPassword.ChangeDate ).TotalHours < 24.0 )
                        return ChangeResult.OnlyOneChangePerDay;
                }
            }
            return ChangeResult.Ok;
        }


        public List<PasswordQuestion> GetQuestions()
        {
            // Get a list of security questions.  
            // If they do not yet exist for this user then return empty ones.
            var rbacEntities = new PEMRBACEntities();
            var questionList = new List<PasswordQuestion>();

            var questions = from securityQuestions in rbacEntities.UserPasswordQuestions
                             orderby securityQuestions.QuestionNumber ascending
                             where securityQuestions.UserId == _userId select securityQuestions;

            foreach (var userPasswordQuestion in questions)
            {
                var question = new PasswordQuestion( userPasswordQuestion.QuestionNumber, userPasswordQuestion.Question, userPasswordQuestion.Answer );
                questionList.Add(question);
            }

            // Are there enough questions? (For now, assume 0 or 2.)
            if ( questionList.Count == 0 )
            {
                questionList.Add(new PasswordQuestion(1, "", ""));
                questionList.Add(new PasswordQuestion(2, "", ""));
            }

            return questionList;
        }

        public void SaveQuestion(PasswordQuestion question)
        {
            // Get a list of security questions.  
            // If they do not yet exist for this user then return empty ones.
            var rbacEntities = new PEMRBACEntities();

            var securityQuestion = (from securityQuestions in rbacEntities.UserPasswordQuestions
                              where securityQuestions.UserId == _userId && securityQuestions.QuestionNumber == question.QuestionNumber
                                    select securityQuestions).FirstOrDefault();

            if ( securityQuestion == null )
            {
                // Add new record
                securityQuestion = new UserPasswordQuestion
                    {
                        UserId = _userId,
                        QuestionNumber = question.QuestionNumber,
                        Question = question.Question,
                        Answer = question.Answer // EncryptionManager.Hash(question.Answer.ToLower(), _salt)
                    };
                rbacEntities.UserPasswordQuestions.Add( securityQuestion );
            }
            else
            {
                // Update existing record
                securityQuestion.Question = question.Question;
                securityQuestion.Answer = question.Answer; // EncryptionManager.Hash(question.Answer.ToLower(), _salt);
            }

            rbacEntities.SaveChanges();
        }

        public PasswordQuestion.QuestionState QuestionState(PasswordQuestion question)
        {
            // Is question number valid?
            if ( question.QuestionNumber == 0 )
            {
                if (!string.IsNullOrEmpty(question.Question) && !string.IsNullOrEmpty(question.Answer))
                {
                    return PasswordQuestion.QuestionState.New;
                    
                }
                return PasswordQuestion.QuestionState.Empty;
            }


            var rbacEntities = new PEMRBACEntities();

            var securityQuestion = (from securityQuestions in rbacEntities.UserPasswordQuestions
                                    where securityQuestions.UserId == _userId && securityQuestions.QuestionNumber == question.QuestionNumber
                                    select securityQuestions).FirstOrDefault();

            if (securityQuestion == null)
            {
                if (!string.IsNullOrEmpty(question.Question) && !string.IsNullOrEmpty(question.Answer))
                {
                    return PasswordQuestion.QuestionState.New;

                }
                return PasswordQuestion.QuestionState.Invalid;
            }

            // Has the question changed?
            if (!securityQuestion.Question.Equals(question.Question, StringComparison.CurrentCultureIgnoreCase))
            {
                //if ( securityQuestion.Answer.Equals( Utilities.Constants.Security.DummyAnswer ) )
                //{
                //    return PasswordQuestion.QuestionState.QuestionChangedNeedAnswer;
                //}
                return PasswordQuestion.QuestionState.Changed;
            }

            // At this point only thing left to check is whether the Answer is still the same as the original stored answer.
            //return securityQuestion.Answer.Equals( EncryptionManager.Hash(question.Answer.ToLower(), _salt))
            //    ? PasswordQuestion.QuestionState.NoChange : PasswordQuestion.QuestionState.Changed;
            return securityQuestion.Answer.Equals(question.Answer, StringComparison.CurrentCultureIgnoreCase)
                ? PasswordQuestion.QuestionState.NoChange : PasswordQuestion.QuestionState.Changed;
        }


        public bool CheckAnswer(PasswordQuestion question)
        {
            bool answerMatches = false;
            var rbacEntities = new PEMRBACEntities();

            var securityQuestion = (from securityQuestions in rbacEntities.UserPasswordQuestions
                                    where securityQuestions.UserId == _userId && securityQuestions.QuestionNumber == question.QuestionNumber
                                    select securityQuestions).First();

            if (securityQuestion != null)
            {
                answerMatches =
                    securityQuestion.Answer.Equals(question.Answer, StringComparison.CurrentCultureIgnoreCase);
//                securityQuestion.Answer.Equals(EncryptionManager.Hash(question.Answer.ToLower(), _salt));
            }

            return answerMatches;

        }
    }
}
