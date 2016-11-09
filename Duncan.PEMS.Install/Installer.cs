using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Text;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using WebMatrix.WebData;
using SecurityManager = Duncan.PEMS.Security.SecurityManager;
using SettingsFactory = Duncan.PEMS.Business.Customers.SettingsFactory;

namespace Duncan.PEMS.Install
{
    class Installer
    {
        private Options _options = new Options();

        private bool _parseSuccess = false;

        public List<string> Logs { get; private set; } 

        private string Log
        {
            set {Logs.Add(value);}
        }

        private string LogError
        {
            set { Logs.Add("Error - " + value); }
        }



        public Installer(string[] args)
        {
            Logs = new List<string>();

            _parseSuccess = CommandLine.Parser.Default.ParseArguments(args, _options);
            if (!_parseSuccess)
            {
                // Log the parsing errors.
                LogError = "Failure parsing command line data.";
                var helpText = new HelpText();
                if ( _options.LastParserState.Errors.Count > 0 )
                {
                    LogError = helpText.RenderParsingErrorsText( _options, 2 );
                }

                helpText.AddOptions( _options );

                Log = helpText;
                Log = "";
                StringBuilder sb = new StringBuilder();
                foreach (var s in args)
                {
                    sb.Append( s ).Append( " " );
                }
                Log = sb.ToString();
            }
        }

        public void Run()
        {
            if ( _parseSuccess )
            {
                if (_options.HasAdminOptions)
                    AddAdminUser();

                if ( _options.HasAdminTemplate )
                    AddAdminSite();
            }

            // Log results.
            if ( _options.Verbose )
            {
                foreach (var log in Logs)
                {
                    Console.WriteLine(log);
                }
            }

            // Write log file if requested.
            if ( _options.LogFile != null )
            {
                try
                {
                    // Open the new file
                    using (StreamWriter sw = System.IO.File.AppendText( _options.LogFile ))
                    {
                        // Append the date to it.
                        sw.WriteLine();
                        sw.WriteLine("PEMS RBAC Initialization");
                        sw.WriteLine(DateTime.Now.ToShortDateString());
                        sw.WriteLine();

                        foreach (var log in Logs)
                        {
                            sw.WriteLine( log );
                        }

                        // Save file.
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failure writing log file.");
                    Console.WriteLine(ex.Message);
                }

            }

            // If verbose, wait for response from user.
            if ( _options.Verbose )
            {
                Console.WriteLine();
                Console.WriteLine("Hit any key to close console");
                Console.ReadKey();
            }
        }

        private void AddAdminUser()
        {
            UserFactory userFactory = new UserFactory();
            UserModel userModel = new UserModel()
                {
                    Username = _options.AdminUserName,
                    EmailAddress = _options.AdminEmail,
                    FirstName = _options.AdminFname ?? "System",
                    MiddleInitial = _options.AdminMname != null ? _options.AdminMname.Substring(0, 1) : null,
                    LastName = _options.AdminLname ?? "Administrator",
                    PhoneNumber = _options.AdminPhone,
                    IsTechnician = false,
                    OrganizationName = "Duncan Technologies",
                    Role = "Administrators"
                };

            SecurityManager.InitializeMembershipConnection();
            Log = "Initialized security manager.";
            userFactory.CreateUser( userModel, null, _options.AdminPassword, true, true );
            Log = "Created admin user " + _options.AdminUserName;
        }
    
        /// <summary>
        /// Add the RBAC entry for the admin site.
        /// </summary>
        private void AddAdminSite()
        {
            var authorizationManager = new AuthorizationManager();
            Log = "Creating RBAC store for the Admin site.";

            if (authorizationManager.CreateCity( _options.AdminSiteId, "Admin", "PEMS Administration" ) )
            {
                Log = "Created RBAC Admin store";
            }
            else
            {
                LogError = "Failed to create RBAC Admin store";
            }


            Log = "Creating RBAC entries for the administration site...";
            bool success = authorizationManager.SetConfiguration( _options.AdminSiteTemplate );

            // Now write out the process log.
            foreach (var xmlProcessLog in authorizationManager.XmlProcessLogs)
            {
                Log = xmlProcessLog;
            }

            if ( success )
            {
                Log = "***** RBAC entries successfully processed. *****";
            }
            else
            {
                LogError = "***** Errors were encountered creating the RBAC entries.  See below. *****";

                // Now write out errors.
                foreach (var xmlProcessError in authorizationManager.XmlProcessErrors)
                {
                    LogError = xmlProcessError;
                }
            }


            // Create an entry in [CustomerProfiles] if required.
            var RbacEntities = new PEMRBACEntities();

            // Get the user id that is adding this Admin site.
            UserFactory userFactory = new UserFactory();
            int userId = userFactory.GetUserId( _options.AdminUserName );

            if ( userId != (int) Constants.User.InvalidUserId )
            {
                CustomerProfile customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault( m => m.DisplayName.Equals( "Admin" ) );
                if ( customerProfile == null )
                {
                    customerProfile = new CustomerProfile()
                        {
                            CustomerId = _options.AdminSiteId,
                            DisplayName = "Admin",
                            CreatedOn = DateTime.Now,
                            CreatedBy = userId,
                            StatusChangeDate = DateTime.Now,
                            PEMSConnectionStringName = null,
                            ReportingConnectionStringName = null,
                            CustomerTypeId = (int)CustomerProfileType.Admin,
                            Status = (int) CustomerStatus.Active
                        };
                    RbacEntities.CustomerProfiles.Add( customerProfile );
                    RbacEntities.SaveChanges();
                    Log = "Created entry in CustomerProfiles for Admin.";
                }

            }
            else
            {
                LogError = "Unable to create an entry in CustomerProfiles for Admin - Invalid admin user name.";
            }


        }
    }
}
