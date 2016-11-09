using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Duncan.PEMS.Install
{
    class Options
    {
        #region Logging

        [Option("logfile", Required = false, HelpText = "Log file where results can be written.")]
        public string LogFile { get; set; }

        [Option("verbose", Required = false, HelpText = "Set if want logging written to console.")]
        public bool Verbose { get; set; }
        
        #endregion


        #region Admin User

        [Option("admin-user", Required = false, HelpText = "Administrator user name")]
        public string AdminUserName { get; set; }

        [Option("admin-pwd", Required = false, HelpText = "Administrator password.  This is a temporary password.")]
        public string AdminPassword { get; set; }

        [Option("admin-email", Required = false, HelpText = "Administrator e-mail address.")]
        public string AdminEmail { get; set; }

        [Option("admin-fname", Required = false, HelpText = "Administrator first name.")]
        public string AdminFname { get; set; }

        [Option("admin-lname", Required = false, HelpText = "Administrator last name.")]
        public string AdminLname { get; set; }

        [Option("admin-mname", Required = false, HelpText = "Administrator middle name.")]
        public string AdminMname { get; set; }

        [Option("admin-phone", Required = false, HelpText = "Administrator phone number.")]
        public string AdminPhone { get; set; }

        public bool HasAdminOptions 
        { 
            get { return
                !string.IsNullOrWhiteSpace(AdminUserName) && 
                !string.IsNullOrWhiteSpace(AdminPassword) && 
                !string.IsNullOrWhiteSpace(AdminEmail); 
            } 
        }

        #endregion

        #region RBAC Menu and Access Rights

        [Option("admin-site-template", Required = false, HelpText = "Template file to build the admin site RBAC entries")]
        public string AdminSiteTemplate { get; set; }

        [Option("admin-site-id", Required = false, HelpText = "Integer id of the Administration site. Recommend value of 1")]
        public int AdminSiteId { get; set; }

        public bool HasAdminTemplate
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AdminSiteTemplate) && 
                    AdminSiteId > 0
                    && !string.IsNullOrWhiteSpace(AdminUserName);
            }
        }

        #endregion

        [ParserState]
        public IParserState LastParserState { get; set; }
    }
}
