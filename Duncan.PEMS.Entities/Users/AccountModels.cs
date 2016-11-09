using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Users
{
    public class SecurityQuestionsModel
    {
        public string UserName { get; set; }
        public string QuestionID { get; set; }
        public string QuestionValue { get; set; }
        public string QuestionText { get; set; }
        public string FailureCount { get; set; }
    }

    public class CustomerSupportModel
    {
        private string _supportNumber;

        public string SupportNumber
        {
            get
            {
                if ( _supportNumber == null )
                {
                    _supportNumber = ConfigurationManager.AppSettings["pems.default.support_number"];
                }
                return _supportNumber;
            }
            set
            {
                _supportNumber = value;
            } 
        }
        public string Message { get; set; }
    }

    public class LoginRedirectModel
    {
        public string SubTitle { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    ///     Base class for models that need 'NewPassword' and 'ConfirmPassword' properties
    /// </summary>
    public class NewPasswordModel
    {
        [Required]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "Must be between 7 and 8 characters long.")]
        [RegularExpression(@"^(?=.{7,8}$)(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!_@#$%^&*()-+=]).*$", ErrorMessage = "Must contain 1: uppercase letter, number, special character")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public virtual string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new and confirmation password do not match.")]
        public virtual string ConfirmPassword { get; set; }
    }

    public class ChangePasswordModel : NewPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        // New password and confirm password properties inherited
    }

    public class ResetPasswordModel : NewPasswordModel
    {
        // New password and confirm password properties inherited

        [HiddenInput(DisplayValue = false)]
        public string Token { get; set; }

        public bool Expired { get; set; }
    }

    public enum ChangeType
    {
        Status,
        Role
    }

    public class BulkUpdateModel
    {
        public ChangeType ChangeType { get; set; }
        public List<CheckBoxItem> Groups { get; set; }
        public string Action { get; set; }
        public string[] UserNames { get; set; }

        public string ChangeDisplay
        {
            get
            {
                //set the defaults here
                string roleChange = "Change Role To";
                string statusChange = "Change Status To";
                var roleChangeObj = HttpContext.GetGlobalResourceObject( "Glossary", "Change Role To" );
                var statusChangeObj = HttpContext.GetGlobalResourceObject( "Glossary", "Change Status To" );
                //if they are changing statys, then provide thsoe options
                if ( roleChangeObj != null && statusChangeObj != null )
                {
                    roleChange = roleChangeObj.ToString();
                    statusChange = statusChangeObj.ToString();
                }

                if ( ChangeType == ChangeType.Status )
                    return statusChange;
                else
                    return roleChange;
            }
        }

        public IEnumerable<SelectListItem> ChangeOptions
        {
            get
            {
                //if they are changing statys, then provide thsoe options
                if ( ChangeType == ChangeType.Status )
                {
                    var activeObject = HttpContext.GetGlobalResourceObject( "Glossary", "Active" );
                    var termedObject = HttpContext.GetGlobalResourceObject( "Glossary", "Terminated" );
                    if ( activeObject != null && termedObject != null )
                        return new[]
                                   {
                                       new SelectListItem {Value = "Active", Text = activeObject.ToString()},
                                       new SelectListItem {Value = "Terminated", Text = termedObject.ToString()},
                                   };
                    return new[]
                               {
                                   new SelectListItem {Value = "Active", Text = "Active"},
                                   new SelectListItem {Value = "Terminated", Text = "Terminated"},
                               };
                }
                    //otherwise, the options are a list of roles
                else
                {
                    //convert the array of groups to an array of selectedListItems
                    return Array.ConvertAll( Groups.ToArray(), @group => new SelectListItem {Value = @group.Value, Text = @group.Text} );
                }
            }
        }
    }

    public class UserModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Initial")]
        public string MiddleInitial { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public List<CheckBoxItem> Groups { get; set; }

        [Display(Name = "Organization Name")]
        public string OrganizationName { get; set; }

        [Display(Name = "Secondary ID Type")]
        public string SecondaryIDType { get; set; }

        [Display(Name = "Secondary ID Value")]
        public string SecondaryIDValue { get; set; }

        [Display(Name = "Role Name")]
        public string Role { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        public bool IsTechnician { get; set; }

        public IEnumerable<SelectListItem> StatusOptions
        {
            get
            {
                var activeObject = HttpContext.GetGlobalResourceObject( "Glossary", "Active" );
                var termedObject = HttpContext.GetGlobalResourceObject( "Glossary", "Terminated" );
                if ( activeObject != null && termedObject != null )
                    return new[]
                               {
                                   new SelectListItem {Value = "Active", Text = activeObject.ToString()},
                                   new SelectListItem {Value = "Terminated", Text = termedObject.ToString()},
                               };
                return new[]
                           {
                               new SelectListItem {Value = "Active", Text = "Active"},
                               new SelectListItem {Value = "Terminated", Text = "Terminated"},
                           };
            }
        }
    }

    public class ProfileModel : UserStatsModel
    {
        public int UserId { get; set; }
        public NewPasswordModel Password { get; set; }
        public bool PasswordResetRequired { get; set; }
    }

    public class UserStatsModel : UserModel
    {
        [Display(Name = "Question 1")]
        public PasswordQuestion Question1 { get; set; }

        [Display(Name = "Question 2")]
        public PasswordQuestion Question2 { get; set; }

        [Display(Name = "Bad Login Count")]
        public int BadLoginCount { get; set; }

        [Display(Name = "Password Exipration")]
        public DateTime PasswordExipration { get; set; }

        [Display(Name = "Last Password Change Date")]
        public DateTime LastPasswordChangeDate { get; set; }

        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate { get; set; }
        public string LastLoginDateDisplay { get { return LastLoginDate == DateTime.MinValue ? string.Empty : LastLoginDate.ToString("d"); } }

        [Display(Name = "Last Login Failure")]
        public DateTime LastLoginFailure { get; set; }

        [Display(Name = "User Creation Date")]
        public DateTime CreationDate { get; set; }
        public string CreationDateDisplay { get { return CreationDate == DateTime.MinValue ? string.Empty : CreationDate.ToString("d"); } }


        [Display(Name = "Active")]
        public bool Active { get; set; }
    }

    public class ListUserModel : UserStatsModel
    {
        [Display(Name = "User Id")]
        public int UserId { get; set; }

        [Display(Name = "Min Date")]
        public DateTime MinDate { get; set; }
    }

    public class CheckBoxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }

    public class DropDownModel
    {
        public string SelectedItem { get; set; }
        public SelectList Items { get; set; }
    }

    public class LandingDropDownModel
    {
        public string SelectedItem { get; set; }
        public List<LandingDropDownItem> Items { get; set; }
    }
    public class LandingDropDownItem
    {
        public CustomerLoginType LoginType { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
    public enum CustomerLoginType
    {
        Unknown = 0,
        Customer = 1,
        MaintenanceGroupCustomer = 2
    }
}