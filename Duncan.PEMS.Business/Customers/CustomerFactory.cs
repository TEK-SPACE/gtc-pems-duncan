/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/22/2014       Sergey Ostrerov                 DPTXPEMS-8, 14, 45 - Can't create new customer; Replace text box to Drop Down Box for Area editing.
 * 01/29/2014       Sergey Ostrerov                 DPTXPEMS-8, 14, 45 Reopened - Can't create new customer; Replace text box to Drop Down Box for Area editing.
 * 01/30/2014       Sergey Ostrerov                 DPTXPEMS- 45 Reopened - Can't create new customer
 * 01/31/2014       Sergey Ostrerov                 DPTXPEMS - 45 Reopened - Can't create new customer; Replace Drop Down Box to text box for Area editing.
 * 02/17/2014       Sergey Ostrerov                 DPTXPEMS - 240 Reopened - Maintenance Schedules for a given customer is created without schedule 
 *                                                                            start date and end date.
 * 02/20/2014       Sergey Ostrerov                 DPTXPEMS - 251 Payment Gateway Configuration page is missing 'Access Code' field
 * 04/10/2014       Sergey Ostrerov                 Customer Management DamandZone TAB: retreives all available Demand Zones per Customer.
 * *******************************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Grids;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;
using DayOfWeek = Duncan.PEMS.Entities.Customers.DayOfWeek;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.FileUpload;


namespace Duncan.PEMS.Business.Customers
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Customers"/> namespace contains classes for managing customers (clients).
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }


    //This class doesnt follow the same rules as far as detemining the PemsEntities with a connection string.
    //Sometimes (when looping through and getting a list of clients from rbac and generating a list of items for example
    //we need to new a PemsEntities with a connectionstring of the current iteration of the customer instead of what is passed into the factory. Same goes for the custome controller.


    public class CustomerFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public CustomerFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        private const string OperationsTimeStart = "OperationsTimeStart";
        private const string OperationsTimeEnd = "OperationsTimeEnd";
        private const string OperationsPeakStart = "OperationsTimePeakStart";
        private const string OperationsPeakEnd = "OperationsTimePeakEnd";
        private const string OperationsEveningStart = "OperationsTimeEveningStart";
        private const string OperationsEveningEnd = "OperationsTimeEveningEnd";

        private const string RulesZeroOutMeter = "RulesZeroOutMeter";
        private const string RulesStreetline = "RulesStreetline";
        private const string RulesGracePeriod = "RulesGracePeriod";
        private const string RulesFreeParkingLimit = "RulesFreeParkingLimit";

        private const string CustomerTemplateName = "rbac.menu.template.auth";

        public bool CustomerIdExists(int customerId)
        {
            return (from customer in RbacEntities.netsqlazman_StoresTable
                     where customer.StoreId == customerId
                     select customer ).Any();
        }

        /// <summary>
        /// Check if customer name value represents an existing customer.  Case insensitive.  This name
        /// represents a name of a customer in the RBAC system.
        /// </summary>
        /// <param name="customerName">String name to check</param>
        /// <returns>True if customer name already exists in system</returns>
        public bool CustomerNameExists(string customerName)
        {
            return (from customer in RbacEntities.netsqlazman_StoresTable
                    where customer.Name.Equals(customerName, StringComparison.CurrentCultureIgnoreCase)
                    select customer).Any();
        }

        /// <summary>
        /// Check if customer name value represents an internal name of an existing customer.  Case insensitive.  This name
        /// represents a name of a customer in the PEMS Customer table.
        /// </summary>
        /// <param name="customerInternalName">String name to check</param>
        /// <returns>True if customer name already exists in system</returns>
        public bool CustomerInternalNameExists(string customerInternalName)
        {
            return (from customer in PemsEntities.Customers
                    where customer.Name.Equals(customerInternalName, StringComparison.CurrentCultureIgnoreCase)
                    select customer).Any();
        }

        /// <summary>
        /// Get a list of all customers in the PEMS System. Retrieves a list of <see cref="ListCustomerModel"/>
        /// </summary>
        /// <returns>List of <see cref="ListCustomerModel"/></returns>
        public List<ListCustomerModel> GetCustomersList()
        {
            var list = new List<ListCustomerModel>();

            // Walk a list of customer profiles and return a list
            var query = from customers in RbacEntities.CustomerProfiles
                        where customers.CustomerId > 0
                        select customers;

            foreach (var customer in query)
            {
                if (customer.CustomerTypeId == (int) CustomerProfileType.Customer)
                {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customer.CustomerId);
                var customerModel = new ListCustomerModel()
                {
                    DisplayName = customer.DisplayName,
                    Id = customer.CustomerId,
                            Status = ((CustomerStatus) customer.Status).ToString(),
                            PemsConnectionStringName = customer.PEMSConnectionStringName,
                            MaintenanceConnectionStringName = customer.MaintenanceConnectionStringName,
                            ReportingConnectionStringName = customer.ReportingConnectionStringName,
                };

                var userFactory = new UserFactory();
                Entities.Users.User user = userFactory.GetUserById(customer.CreatedBy ?? -1);

                customerModel.CreatedBy = user.FullName();
                customerModel.CreatedOn = customer.CreatedOn ?? DateTime.MinValue;

                    if (customer.ModifiedBy == null)
                {
                    customerModel.UpdatedBy = "-";
                    customerModel.UpdatedOn = DateTime.MinValue;
                }
                else
                {
                    user = userFactory.GetUserById(customer.ModifiedBy ?? -1);
                    customerModel.UpdatedBy = user.FullName(); 
                    customerModel.UpdatedOn = customer.ModifiedOn ?? (customer.ModifiedOn ?? DateTime.MinValue);
                }


                // Get data elements from PEMS
                    var pemsCustomer =
                        (new PEMEntities(connStringName)).Customers.SingleOrDefault(
                            c => c.CustomerID == customerModel.Id);
                    customerModel.Name = pemsCustomer != null ? pemsCustomer.Name : "";

                // Get State and Country
                var settingFactory = new SettingsFactory();
                customerModel.State = settingFactory.GetValue(CustomerSettingsConstants.State, customerModel.Id);
                customerModel.Country = settingFactory.GetValue(CustomerSettingsConstants.Country, customerModel.Id);

                list.Add(customerModel);
            }
            }


            return list;
        }

        /// <summary>
        /// Creates a new customer in the PEMS System.  Creates the necessary enties in the PEMS RBAC database and the appropriate
        /// PEMS database.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="templateFolder"></param>
        /// <param name="workingFolder"></param>
        /// <returns></returns>
        public int CreateNewCustomer(CustomerModel model, string templateFolder, string workingFolder)
        {
            // In order to create a new customer, the following transactions need to take place.
            // 1.  Create customer in AuthorizationManager
            // 2.  Create customer in the PEMS database
            // 3.  Create the customer (customer profile) in the RBAC database.
            // 4.  Create CustomerPropertyGroup/CustomerProperty entries. (spInitializeCustomerProperties)
            // 5.  Call spInitializeCustomer procedure to create reference table
            //       entries in the PEMS database.
            // 6.  Create menu and authorization entries in NetSqlAzMan
            // 7.  Add the current user to the list in the cache table
            // 8.  Add entries for [CustomerReports] for customer in the RBAC database.

            int customerId = 0;

            var authorizationManager = new AuthorizationManager();
            if ( authorizationManager.CreateCity( model.Id, model.DisplayName, "Internal Name: " + model.InternalName ) )
            {
                PemsEntities.Customers.Add(new Customer()
                    {
                        Name = model.InternalName, 
                        CustomerID = model.Id, 
                        FromEmailAddress = "",
                        BlackListCC = false,
                        GracePeriodMinute = 15,
                        FreeParkingMinute = 15,
                        ZeroOutMeter = false
                    });
                PemsEntities.SaveChanges();

                var customerProfile = new CustomerProfile()
                    {
                        CustomerId = model.Id,
                        DisplayName = model.DisplayName,
                        CreatedOn = DateTime.Now,
                        CreatedBy = WebSecurity.CurrentUserId,
                        StatusChangeDate = DateTime.Now,
                        PEMSConnectionStringName = model.ConnectionStringName,
                        ReportingConnectionStringName = model.ReportingConnectionStringName,
                        CustomerTypeId = (int)CustomerProfileType.Customer,
                        Status = (int)CustomerStatus.New
                    };
                RbacEntities.CustomerProfiles.Add( customerProfile );
                RbacEntities.SaveChanges();


                customerId = model.Id;
            }


            if ( customerId != 0 )
            {
                // Insert CutomerProperties entries.
                PemsEntities.spInitializeCustomerProperties( customerId );

                // Call spInitializeCustomer
                PemsEntities.spInitializeCustomer(customerId);

                // Set the menus into RBAC
                SetMenus(model.DisplayName, templateFolder, workingFolder, CustomerTemplateName);

                var authMan = new AuthorizationManager(model.DisplayName);
                authMan.AddGroupMember("Administrators", WebSecurity.CurrentUserName, false);

                // Add customer Grid settings
                //refresh all of the customer grids. Since this customer id exist, then the defaults will be set, as well as fixing up any other grids for customers that might be missing some defaults.
                RbacEntities.sp_RefreshCustomerGrids();

                //add the current customer to the access caching table
                (new UserCustomerAccessManager()).AddCustomerAccess(WebSecurity.CurrentUserId, customerId);

                // Add reports settings (IZenda)
                foreach (var report in RbacEntities.CustomerReports.Where(m => m.CustomerId == 0))
                {
                    RbacEntities.CustomerReports.Add(
                        new CustomerReport()
                            {
                                CustomerId = customerId,
                                CustomerReportsCategoryId = report.CustomerReportsCategoryId,
                                ReportAction = report.ReportAction,
                                Host = report.Host,
                                Parameters = report.Parameters,
                                Path = report.Path
                            } );
                }
                RbacEntities.SaveChanges();
            }

            return customerId;
        }

        /// <summary>
        /// Creates a new customer in the PEMS System.  Creates the necessary enties in the PEMS RBAC database and the appropriate
        /// PEMS database.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="templateFolder"></param>
        /// <param name="workingFolder"></param>
        /// <returns></returns>
        public int CreateNewCustomerOptimzed(CustomerModel model, string templateFolder, string workingFolder)
        {
            // In order to create a new customer, the following transactions need to take place.
            // 1.  Create customer in AuthorizationManager
            // 2.  Create customer in the PEMS database
            // 3.  Create the customer (customer profile) in the RBAC database.
            //
            // 4.  Create menu and authorization entries in NetSqlAzMan
            // 5.  Add the current user to the list in the cache table
         

            int customerId = 0;

            var authorizationManager = new AuthorizationManager();
            if (authorizationManager.CreateCity(model.Id, model.DisplayName, "Internal Name: " + model.InternalName))
            {
                PemsEntities.Customers.Add(new Customer()
                {
                    Name = model.InternalName,
                    CustomerID = model.Id,
                    FromEmailAddress = "",
                    BlackListCC = false,
                    GracePeriodMinute = 15,
                    FreeParkingMinute = 15,
                    ZeroOutMeter = false
                });
                PemsEntities.SaveChanges();

                var customerProfile = new CustomerProfile()
                {
                    CustomerId = model.Id,
                    DisplayName = model.DisplayName,
                    CreatedOn = DateTime.Now,
                    CreatedBy = WebSecurity.CurrentUserId,
                    StatusChangeDate = DateTime.Now,
                    PEMSConnectionStringName = model.ConnectionStringName,
                    ReportingConnectionStringName = model.ReportingConnectionStringName,
                    CustomerTypeId = (int)CustomerProfileType.Customer,
                    Status = (int)CustomerStatus.New
                };
                RbacEntities.CustomerProfiles.Add(customerProfile);
                RbacEntities.SaveChanges();

                customerId = model.Id;
            }


            if (customerId != 0)
            {
                
                // Set the menus into RBAC
                SetMenus(model.DisplayName, templateFolder, workingFolder, CustomerTemplateName);

                var authMan = new AuthorizationManager(model.DisplayName);
                authMan.AddGroupMember("Administrators", WebSecurity.CurrentUserName, false);

                //add the current customer to the access caching table
                (new UserCustomerAccessManager()).AddCustomerAccess(WebSecurity.CurrentUserId, customerId);

                
            }

            return customerId;
        }

        private void SetMenus(string customerName, string templateFolder, string workingFolder, string templateName)
        {
            // Create the menu file for the customer.

            var configTemplate = Path.Combine(templateFolder, System.Configuration.ConfigurationManager.AppSettings[templateName]);
            var destinationFile = Path.Combine(workingFolder, Guid.NewGuid().ToString() + ".xml");

            const string customerField = "[CUSTOMER]";

            StreamWriter writer = null;
            using (writer = File.CreateText(destinationFile))
            {
                foreach (string line in File.ReadLines(configTemplate))
                    writer.WriteLine(line.Replace(customerField, customerName));
            }

            // Now apply the newly created menu file.
            var authorizationManager = new AuthorizationManager();
            authorizationManager.SetConfiguration(destinationFile);
        }

        private string GetCustomerSettingValueOrCreate(string settingName, int customerId, string defaultValue)
        {
            var settingFactory = new SettingsFactory();
            var value = settingFactory.GetValue(settingName, customerId);

            if (value != null) return value;

            // create if it does not exist
            settingFactory.Set(customerId, settingName, defaultValue);
            return defaultValue;
        }

        private void RemoveCachedPemsMenuForCustomer(string cityName)
        {
            var keyPrefix = "__PemsMenu" + cityName;

            var keysToClear = (from System.Collections.DictionaryEntry dict in HttpContext.Current.Cache
                               let key = dict.Key.ToString()
                               where (!string.IsNullOrWhiteSpace(key) && key.StartsWith(keyPrefix))
                               select key).ToList();
            keysToClear.ForEach(key => HttpContext.Current.Cache.Remove(key));
        }

        #region Customer Base

        public void GetCustomerBaseModel(CustomerBaseModel customerBaseModel)
        {
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerBaseModel.CustomerId);
            if ( customerProfile != null )
            {
                customerBaseModel.CustomerId = customerBaseModel.CustomerId;
                customerBaseModel.DisplayName = customerProfile.DisplayName;
                customerBaseModel.Is24HrFormat = customerProfile.Is24HrFormat ?? false;

                // Get customer Status
                customerBaseModel.Status = GetCustomerStatusModel(customerBaseModel.CustomerId);
            }
        }

        #endregion

        #region Customer Status

        public CustomerActivateModel CanActivate(int customerId)
        {
            CustomerActivateModel model = new CustomerActivateModel()
                {
                    CustomerId = customerId
                };
            GetCustomerBaseModel( model );

            var customer = PemsEntities.Customers.FirstOrDefault( m => m.CustomerID == customerId );
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault( m => m.CustomerId == customerId );
            var settingFactory = new SettingsFactory();


            // Has e-mail address?
            if ( string.IsNullOrWhiteSpace( customer.FromEmailAddress ) )
            {
                model.Issues.Add( new CustomerActivateIssue()
                    {
                        Description = "No e-mail address.",
                        Controller = "Customers",
                        Action = "EditCustomer"
                    } );
            }

            // Has phone number?
            string phoneNumber = settingFactory.GetValue("CustomerPhoneNumber", customerId);
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "Required phone number is missing.",
                    Controller = "Customers",
                    Action = "EditCustomer"
                });
            }

            // Has non-zero Lat/Long
            if(customer.Latitude == null || customer.Latitude.Equals(0.0))
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "Latitude should be set.",
                    Controller = "Customers",
                    Action = "EditCustomer"
                });
            }
            if (customer.Longitude == null || customer.Longitude.Equals(0.0))
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "Longitude should be set.",
                    Controller = "Customers",
                    Action = "EditCustomer"
                });
            }

            // Has a default password?
            string defaultPassword = settingFactory.GetValue("DefaultPassword", customerId);
            if (string.IsNullOrWhiteSpace(defaultPassword))
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "Default password is missing.",
                    Controller = "Customers",
                    Action = "EditCustomer"
                });
            }

            // Has a time zone
            if (customerProfile.TimeZoneID == null)
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "Select customer time zone.",
                    Controller = "Customers",
                    Action = "EditCustomer"
                });
            }

            // Has at least 1 coin selected.
            var coinDenomination = PemsEntities.CoinDenominationCustomers.FirstOrDefault( m => m.CustomerId == customerId && m.IsDisplay );
            if (coinDenomination == null)
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "At least one coin needs to be selected.",
                    Controller = "Customers",
                    Action = "EditPayments"
                });
            }

            // Has at least one active asset?
            var assetType = PemsEntities.AssetTypes.FirstOrDefault( m => m.CustomerId == customerId && m.IsDisplay == true );
            if (assetType == null)
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "At least one asset needs to be selected.",
                    Controller = "Customers",
                    Action = "EditAssets"
                });
            }

            // Has at least one Area
            var area = PemsEntities.Areas.FirstOrDefault( m => m.CustomerID == customerId );
            if (area == null)
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "At least one Area needs to be created.",
                    Controller = "Customers",
                    Action = "EditAreas"
                });
            }

            // Has at leat one Zone
            var zone = PemsEntities.Zones.FirstOrDefault(m => m.customerID == customerId);
            if (zone == null)
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "At least one Zone needs to be created.",
                    Controller = "Customers",
                    Action = "EditAreas"
                });
            }

            // Has at least one Suburb (CustomGroup1)
            var customGroup1 = PemsEntities.CustomGroup1.FirstOrDefault(m => m.CustomerId == customerId);
            if (customGroup1 == null)
            {
                model.Issues.Add(new CustomerActivateIssue()
                {
                    Description = "At least one Suburb (Custom Group 1) needs to be created.",
                    Controller = "Customers",
                    Action = "EditAreas"
                });
            }




            // Can activate?
            model.CanActivate = !model.Issues.Any();

            // Post a message that customer can be activated.
            if ( model.CanActivate )
            {
                model.ActivateMessage = model.DisplayName + " may be activated.";
            }

            return model;
        }

        private void ActivateInActivateOLT(int customerId, int cardType,int isActivate)
        {
            var oltAcquirers = PemsEntities.OLTAcquirers.FirstOrDefault(m => m.CardTypeCode == cardType && m.CustomerID == customerId);
            if (oltAcquirers != null)
            {
                oltAcquirers.OLTPActive = isActivate;
                PemsEntities.SaveChanges();
            }
        }

        public void Activate(int customerId)
        {

            // Activate OLT for CREDITCARD payment
            ActivateInActivateOLT(customerId, CustomerFactory.SmartCardTypeId,1);
            // Activate OLT for SMARTCARD-DATAPARK payment
            ActivateInActivateOLT(customerId, CustomerFactory.PaymentTypeId,1);

            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault( m => m.CustomerId == customerId );
            if ( customerProfile != null )
            {
                customerProfile.Status = (int) CustomerStatus.Active;
                customerProfile.StatusChangeDate = DateTime.Now;
                RbacEntities.SaveChanges();
            }
        }

        public void Inactivate(int customerId)
        {

            // Inactivate OLT for CREDITCARD payment
            ActivateInActivateOLT(customerId, CustomerFactory.SmartCardTypeId, 0);
            // Inactivate OLT for SMARTCARD-DATAPARK payment
            ActivateInActivateOLT(customerId, CustomerFactory.PaymentTypeId, 0);

            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            if (customerProfile != null)
            {
                customerProfile.Status = (int)CustomerStatus.Inactive;
                customerProfile.StatusChangeDate = DateTime.Now;
                RbacEntities.SaveChanges();
            }
        }

        public CustomerStatusModel GetCustomerStatusModel(int customerId)
        {
            var customerStatusModel = new CustomerStatusModel();
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault( m => m.CustomerId == customerId );
            if ( customerProfile != null )
            {
                var userFactory = new UserFactory();
                customerStatusModel.CreatedBy = userFactory.GetUserById(customerProfile.CreatedBy ?? -1).FullName();
                customerStatusModel.CreatedOn = customerProfile.CreatedOn != null ? customerProfile.CreatedOn.Value.ToShortDateString() : "-";

                customerStatusModel.ModifiedBy = userFactory.GetUserById(customerProfile.ModifiedBy ?? -1).FullName();
                customerStatusModel.ModifiedOn = customerProfile.ModifiedOn != null ? customerProfile.ModifiedOn.Value.ToShortDateString() : "-";

                customerStatusModel.StatusChangeDate = customerProfile.StatusChangeDate.ToShortDateString();
                customerStatusModel.StatusDate = DateTime.Now.ToShortDateString();

                customerStatusModel.Status = ( (CustomerStatus) customerProfile.Status ).ToString();
                customerStatusModel.StatusId = (CustomerStatus)customerProfile.Status;


                customerStatusModel.StatusList = new List<SelectListItem>();
                foreach (CustomerStatus cs in  Enum.GetValues( typeof (CustomerStatus) ))
                {
                    var selectListItem = new SelectListItem()
                        {
                            Text    = cs.ToString(),
                            Value =  ((int)cs).ToString(),
                            Selected = cs.ToString().Equals(customerStatusModel.Status)
                        };

                    customerStatusModel.StatusList.Add(selectListItem);
                }
            }
            else
            {
                customerStatusModel.Status = "Invalid";
                customerStatusModel.StatusList = new List<SelectListItem>();
                customerStatusModel.StatusList.Add(new SelectListItem()
                {
                    Text = customerStatusModel.Status,
                    Value = "-2",
                    Selected = true
                });
            }

            return customerStatusModel;
        }

        #endregion

        #region Customer Contact

        public CustomerContactModel GetCustomerContactModel(int customerId)
        {
            var customerContactModel = new CustomerContactModel();
            var settingFactory = new SettingsFactory();
            customerContactModel.FirstName = settingFactory.GetValue("CustomerContactFirstName", customerId);
            customerContactModel.LastName = settingFactory.GetValue("CustomerContactLastName", customerId);
            customerContactModel.FromEmailAddress = settingFactory.GetValue("CustomerContactEMail", customerId);
            customerContactModel.PhoneNumber = settingFactory.GetValue("CustomerContactPhoneNumber", customerId);
            return customerContactModel;
        }

        public void SetCustomerContactModel(int customerId, CustomerContactModel customerContactModel)
        {
            var settingFactory = new SettingsFactory();
            if(customerContactModel.FirstName != null)
                settingFactory.Set(customerId, "CustomerContactFirstName", customerContactModel.FirstName);

            if (customerContactModel.LastName != null)
                settingFactory.Set(customerId, "CustomerContactLastName", customerContactModel.LastName);

            if (customerContactModel.FromEmailAddress != null)
                settingFactory.Set(customerId, "CustomerContactEMail", customerContactModel.FromEmailAddress);

            if (customerContactModel.PhoneNumber != null)
                settingFactory.Set(customerId, "CustomerContactPhoneNumber", customerContactModel.PhoneNumber);
        }

        #endregion

        #region Customer Localization

        public CustomerLocalizationModel GetCustomerLocalizationModel(int customerId)
        {
            CustomerLocalizationModel customerLocalizationModel = new CustomerLocalizationModel();
            List<Setting> settingList;
            SelectListItem selectListItem;

            // TimeZone
            // Get presently assign zone id.
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault( m => m.CustomerId == customerId );
            customerLocalizationModel.TimeZoneId = customerProfile == null ? -1 : customerProfile.TimeZoneID ?? -1;

            // Get list of possible zones.
            customerLocalizationModel.TimeZone.Add( new SelectListItem()
                   {
                       Text = "[Select Time Zone]",
                       Value = "-1",
                       Selected = customerLocalizationModel.TimeZoneId == -1
                   });

            if ( customerLocalizationModel.TimeZoneId == -1 )
            {
                customerLocalizationModel.TimeZoneDisplay = "";
            }

            foreach (var timeZone in RbacEntities.CustomerTimeZones)
            {
                customerLocalizationModel.TimeZone.Add(new SelectListItem()
                {
                    Text = timeZone.TimeZoneName,
                    Value = timeZone.TimeZoneID.ToString(),
                    Selected = customerLocalizationModel.TimeZoneId == timeZone.TimeZoneID
                });
                if(customerLocalizationModel.TimeZoneId == timeZone.TimeZoneID)
                {
                    customerLocalizationModel.TimeZoneDisplay = timeZone.TimeZoneName;
                }
            }

            // Language
            customerLocalizationModel.LanguageDisplay = "";

            var settingFactory = new SettingsFactory();

            string locale = settingFactory.GetValue("CustomerLocale", customerId);

            customerLocalizationModel.Language = new List<SelectListItem>();
            settingList = settingFactory.GetList("Locale");
            foreach (Setting setting in settingList)
            {
                selectListItem = new SelectListItem()
                {
                    Text = setting.Value,
                    Value = setting.Id.ToString()
                };
                if (locale != null && locale.Equals(setting.Value))
                {
                    selectListItem.Selected = true;
                    customerLocalizationModel.LanguageId = setting.Id;
                    customerLocalizationModel.LanguageDisplay = locale;
                }
                else
                {
                    selectListItem.Selected = setting.Default;
                    if (selectListItem.Selected)
                        customerLocalizationModel.LanguageId = setting.Id;
                }

                customerLocalizationModel.Language.Add(selectListItem);
            }
            // Is there a selected or default item?  If not add a "select X" item.
            if (customerLocalizationModel.LanguageId == 0)
            {
                customerLocalizationModel.Language.Insert(0,
                   new SelectListItem()
                   {
                       Text = "[Select Language]",
                       Value = "0",
                       Selected = true
                   });
            }

            // 12/24 Hour Clock
            customerLocalizationModel.Is24Hr = false;
            var customer = RbacEntities.CustomerProfiles.FirstOrDefault( m => m.CustomerId == customerId );
            if(customer != null)
                customerLocalizationModel.Is24Hr = customer.Is24HrFormat ?? false;


            return customerLocalizationModel;
        }


        public void SetCustomerLocalizationModel(int customerId, CustomerLocalizationModel customerLocalizationModel)
        {
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);

            // Language
            var settingFactory = new SettingsFactory();
            Setting setting = settingFactory.GetListValue("Locale", customerLocalizationModel.LanguageId);
            if ( setting != null )
            {
                settingFactory.Set( customerId, "CustomerLocale", setting.Value );

                // Write the local to customer.DefaultLocale also
                customerProfile.DefaultLocale = setting.Value;
            }


            // 12/ 24 hour format
            customerProfile.Is24HrFormat = customerLocalizationModel.Is24Hr;

            // TimeZone
            customerProfile.TimeZoneID = customerLocalizationModel.TimeZoneId;
            // Need to also save this data in PEMS Customers table.
            var customer = PemsEntities.Customers.FirstOrDefault( m => m.CustomerID == customerId );
            if ( customer != null )
            {
                customer.TimeZoneID = customerLocalizationModel.TimeZoneId;
                PemsEntities.SaveChanges();
            }



            // Save changes.
            RbacEntities.SaveChanges();
        }

        #endregion
        
        #region Customer Identification

        public AdminCustomerIdentificationModel GetAdminCustomerIdentificationModel(int customerId)
        {
            var model = new AdminCustomerIdentificationModel()
                {
                    CustomerId = customerId
                };

            var settingFactory = new SettingsFactory();
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);

            GetCustomerBaseModel( model);

            model.DisplayName = customerProfile.DisplayName;
            model.DefaultPassword = settingFactory.GetValue("DefaultPassword", customerId);

            return model;
        }
        
        public CustomerIdentificationModel GetCustomerIdentificationModel(int customerId)
        {
            var customerIdentificationModel = new CustomerIdentificationModel()
                {
                    CustomerId = customerId
                };

            var settingFactory = new SettingsFactory();
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            var customer = PemsEntities.Customers.SingleOrDefault(m => m.CustomerID == customerId);
            if (customerProfile != null && customer != null)
            {
                GetCustomerBaseModel(customerIdentificationModel);

                customerIdentificationModel.DisplayName = customerProfile.DisplayName;
                customerIdentificationModel.InternalName = customer.Name;

                customerIdentificationModel.AddressLine1 = settingFactory.GetValue("AddressLine1", customerId);
                customerIdentificationModel.AddressLine2 = settingFactory.GetValue("AddressLine2", customerId);

                customerIdentificationModel.AddressCity = settingFactory.GetValue("AddressCity", customerId);
                customerIdentificationModel.CityCode = customer.CityCode;

                customerIdentificationModel.AddressState = settingFactory.GetValue("AddressState", customerId);

                string country = settingFactory.GetValue("AddressCountry", customerId);

                // Get allowable Countries
                customerIdentificationModel.AddressCountryDisplay = "";
                customerIdentificationModel.AddressCountry = new List<SelectListItem>();
                List<Setting> settingList = settingFactory.GetList("AddressCountry");
                foreach (Setting setting in settingList)
                {
                    var selectListItem = new SelectListItem()
                        {
                            Text = setting.Value,
                            Value = setting.Id.ToString()
                        };
                    if ( country != null && country.Equals(setting.Value))
                    {
                        selectListItem.Selected = true;
                        customerIdentificationModel.AddressCountryId = setting.Id;
                        customerIdentificationModel.AddressCountryDisplay = country;
                    }
                    else
                    {
                        selectListItem.Selected = setting.Default;
                        if (selectListItem.Selected)
                            customerIdentificationModel.AddressCountryId = setting.Id;
                    }

                    customerIdentificationModel.AddressCountry.Add(selectListItem);
                }

                customerIdentificationModel.AddressPostalCode = settingFactory.GetValue("AddressZipCode", customerId);

                customerIdentificationModel.Longitude = customer.Longitude ?? 0.0;
                customerIdentificationModel.Latitude = customer.Latitude ?? 0.0;

                customerIdentificationModel.PhoneNumber = settingFactory.GetValue("CustomerPhoneNumber", customerId);
                customerIdentificationModel.FromEmailAddress = customer.FromEmailAddress;

                customerIdentificationModel.DefaultPassword = settingFactory.GetValue("DefaultPassword", customerId);
            }

            // Get customer Address
            customerIdentificationModel.Contact = GetCustomerContactModel(customerId);

            // Get customer localization data.
            customerIdentificationModel.Localization = GetCustomerLocalizationModel( customerId);
            

            return customerIdentificationModel;
        }

        public CustomerIdentificationModel GetCustomerIdentificationModel(int customerId, CustomerIdentificationModel customerIdentificationModel)
        {
            // Save country data index
            int addressCountryId = customerIdentificationModel.AddressCountryId;

            // Refresh allowable Countries
            customerIdentificationModel.AddressCountry = new List<SelectListItem>();
            List<Setting> settingList = (new SettingsFactory()).GetList("AddressCountry");
            foreach (Setting setting in settingList)
            {
                var selectListItem = new SelectListItem()
                {
                    Text = setting.Value,
                    Value = setting.Id.ToString()
                };
                customerIdentificationModel.AddressCountry.Add(selectListItem);
            }

            // Restore country data index
            customerIdentificationModel.AddressCountryId = addressCountryId;

            // Save selected localization data indicies.
            int timeZoneId = customerIdentificationModel.Localization.TimeZoneId;
            int languageId = customerIdentificationModel.Localization.LanguageId;
            //int currencyFormatId = customerIdentificationModel.Localization.CurrencyFormatId;
            //int dateTimeFormatId = customerIdentificationModel.Localization.DateTimeFormatId;
            
            // Refresh customer localization data.
            customerIdentificationModel.Localization = GetCustomerLocalizationModel(customerId);

            // Restore selected localization data indicies.
            customerIdentificationModel.Localization.TimeZoneId = timeZoneId;
            customerIdentificationModel.Localization.LanguageId = languageId;
            //customerIdentificationModel.Localization.CurrencyFormatId = currencyFormatId;
            //customerIdentificationModel.Localization.DateTimeFormatId = dateTimeFormatId;


            // Get customer Status
            customerIdentificationModel.Status = GetCustomerStatusModel(customerId);

            return customerIdentificationModel;
        }
        
        public void SetAdminCustomerIdentificationModel(int customerId, AdminCustomerIdentificationModel customerIdentificationModel)
        {

            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);

            if (customerProfile == null)
                return;

            customerProfile.DisplayName = customerIdentificationModel.DisplayName;

            var settingFactory = new SettingsFactory();

            if (customerIdentificationModel.DefaultPassword != null)
                settingFactory.Set(customerId, "DefaultPassword", customerIdentificationModel.DefaultPassword);

            RbacEntities.SaveChanges();
        }
        
        public void SetCustomerIdentificationModel(int customerId, CustomerIdentificationModel customerIdentificationModel)
        {

            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            var customer = PemsEntities.Customers.SingleOrDefault(m => m.CustomerID == customerId);

            if ( customerProfile == null || customer == null )
                return;

            customerProfile.DisplayName = customerIdentificationModel.DisplayName;

            customer.Name = customerIdentificationModel.InternalName;
            var settingFactory = new SettingsFactory();
            if(customerIdentificationModel.AddressLine1 != null)
                settingFactory.Set(customerId, "AddressLine1", customerIdentificationModel.AddressLine1);

            if(customerIdentificationModel.AddressLine2 != null)
                settingFactory.Set(customerId, "AddressLine2", customerIdentificationModel.AddressLine2);

            if ( customerIdentificationModel.AddressCity != null )
                settingFactory.Set(customerId, "AddressCity", customerIdentificationModel.AddressCity);

            if (customerIdentificationModel.AddressState != null)
                settingFactory.Set(customerId, "AddressState", customerIdentificationModel.AddressState);

            if ( customerIdentificationModel.AddressCountryId > 0 )
            {
                Setting setting = settingFactory.GetListValue("AddressCountry", customerIdentificationModel.AddressCountryId);
                if ( setting != null )
                {
                    settingFactory.Set(customerId, "AddressCountry", setting.Value);
                }
            }

            if (customerIdentificationModel.AddressPostalCode != null)
                settingFactory.Set(customerId, "AddressZipCode", customerIdentificationModel.AddressPostalCode);

            customer.Latitude = customerIdentificationModel.Latitude;
            customer.Longitude = customerIdentificationModel.Longitude;

            if (customerIdentificationModel.PhoneNumber != null)
                settingFactory.Set(customerId, "CustomerPhoneNumber", customerIdentificationModel.PhoneNumber);

            if (customerIdentificationModel.FromEmailAddress != null)
                customer.FromEmailAddress = customerIdentificationModel.FromEmailAddress;

            if (customerIdentificationModel.DefaultPassword != null)
                settingFactory.Set(customerId, "DefaultPassword", customerIdentificationModel.DefaultPassword);

            // Save customer contact
            SetCustomerContactModel( customerId, customerIdentificationModel.Contact );

            // Save Customer Localization data.
            SetCustomerLocalizationModel( customerId, customerIdentificationModel.Localization );

            RbacEntities.SaveChanges();
            PemsEntities.SaveChanges();
        }

        #endregion

        #region Customer Maintenance Schedule
        public CustomerMaintenanceScheduleModel GetCustomerMaintenanceScheduleModel(int customerId)
        {
            var model = new CustomerMaintenanceScheduleModel
                {
                    CustomerId = customerId,
                    DaysOfWeek = new List<DayOfWeek>()
                };
            GetCustomerBaseModel(model);
            var days = PemsEntities.DayOfWeeks.ToList();
            if (days.Any())
            {
                foreach (var day in days)
                {
                    var dayOfWeek = new DayOfWeek {DayOfWeekId = day.DayOfWeekId, Name = day.DayOfWeekDesc};
                    //go get them from sla_maintenanceschedule for this customer. 
                    var maintSchedule =PemsEntities.SLA_MaintenanceSchedule.FirstOrDefault(x => x.CustomerId == customerId && x.DayOfWeek == day.DayOfWeekId);
                    if (maintSchedule != null)
                    {
                        dayOfWeek.StartMinute = maintSchedule.MaintenanceStartMinuteOfDay;
                        dayOfWeek.EndMinute = maintSchedule.MaintenanceEndMinuteOfDay;
                        if (dayOfWeek.StartMinute == 0 && dayOfWeek.EndMinute == 0)
                            dayOfWeek.NoHours = true;
                    }
                    //if they dont exist, create them
                    //get from sla_maintencanceschedule for this customerid and dayofweekid. if it doesnt exist - create it with 0 / 0 start and end times
                    else
                    {
                        var mainSched = new SLA_MaintenanceSchedule();
                        mainSched.CustomerId = customerId;
                        mainSched.DayOfWeek = day.DayOfWeekId;
                        mainSched.MaintenanceEndMinuteOfDay = 0;
                        mainSched.MaintenanceStartMinuteOfDay = 0;
                        PemsEntities.SLA_MaintenanceSchedule.Add(mainSched);
                        PemsEntities.SaveChanges();
                        dayOfWeek.NoHours = true;
                    }
                  
                    //set day of week properties based on the schedule item
                    model.DaysOfWeek.Add(dayOfWeek);
                }
            }

            return model;
        }

        public void SetCustomerMaintenanceScheduleModel(int customerId, CustomerMaintenanceScheduleModel model)
        {
            //foreach model, update the customer sla maintenance
            foreach (var day in model.DaysOfWeek)
            {
                //find it based on customerid and dayofweekID from the SLA_MaintenanceSchedule Table
                var maintSchedule = PemsEntities.SLA_MaintenanceSchedule.FirstOrDefault(x => x.CustomerId == customerId && x.DayOfWeek == day.DayOfWeekId);
                if (maintSchedule != null)
                {
                    if (day.NoHours)
                    {
                        //updat the start and end minutes
                        maintSchedule.MaintenanceStartMinuteOfDay = 0;
                        maintSchedule.MaintenanceEndMinuteOfDay = 0;
                    }
                    else
                    {
                        //updat the start and end minutes
                        maintSchedule.MaintenanceStartMinuteOfDay = day.StartMinute;
                        maintSchedule.MaintenanceEndMinuteOfDay = day.EndMinute;
                    }
                    maintSchedule.ScheduleStartDate = day.ScheduleStartDate;
                    maintSchedule.ScheduleEndDate = day.ScheduleEndDate;

                }
            }
            //save changes
            PemsEntities.SaveChanges();
        }

        #endregion

        #region Customer Assets

        private CustomerAssetTypeModel GetCustomerAssetTypeModel(int customerId, int meterGroupId)
        {
            CustomerAssetTypeModel customerAssetTypeModel = null;


            MeterGroup meterGroup = PemsEntities.MeterGroups.FirstOrDefault(m => m.MeterGroupId == meterGroupId);

            if ( meterGroup != null )
            {
                customerAssetTypeModel = new CustomerAssetTypeModel()
                {
                    GroupId = meterGroup.MeterGroupId,
                    GroupName = meterGroup.MeterGroupDesc
                };

                // Does this customer already have associated AssetType?
                AssetType assetType = PemsEntities.AssetTypes.FirstOrDefault(m => m.CustomerId == customerId && m.MeterGroupId == meterGroup.MeterGroupId);
                if (assetType == null)
                {
                    // If not, then create one.
                    assetType = new AssetType()
                    {
                        CustomerId = customerId,
                        IsDisplay = false,
                        MeterGroupId = meterGroup.MeterGroupId,
                        MeterGroupDesc = meterGroup.MeterGroupDesc,
                        PreventativeMaintenanceScheduleDays = 180,
                        SLAMinutes = 300
                    };
                    PemsEntities.AssetTypes.Add( assetType );
                }

                PemsEntities.SaveChanges();

                customerAssetTypeModel.Active = assetType.IsDisplay ?? false;
                customerAssetTypeModel.Id = assetType.AssetTypeId;
                customerAssetTypeModel.MaintenanceSlaDays = assetType.SLAMinutes == null ? 0 : (int)assetType.SLAMinutes / 1440;
                customerAssetTypeModel.MaintenanceSlaHours = assetType.SLAMinutes == null ? 0 : ((int)assetType.SLAMinutes % 1440) / 60;
                customerAssetTypeModel.PreventativeMaintenanceSlaDays = assetType.PreventativeMaintenanceScheduleDays ?? 0;
                customerAssetTypeModel.DisplayName = assetType.MeterGroupDesc;
            }

            return customerAssetTypeModel;
        }


        private void PopulateCustomerAssetModels(int customerId, CustomerAssetTypeModel customerAssetTypeModel)
        {
            var mechanisms = PemsEntities.MechanismMasters.Where(m => m.MeterGroupId == customerAssetTypeModel.GroupId);

            // Make sure customer has an entry for each mechanism in MechanismMasterCustomers
            foreach (var mechanism in mechanisms)
            {
                // Does this customer already have associated MechanismMasterCustomer?
                var mechanismMasterCustomer = PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == mechanism.MechanismId );
                if (mechanismMasterCustomer == null)
                {
                    mechanismMasterCustomer = new MechanismMasterCustomer()
                    {
                        CustomerId = customerId,
                        IsDisplay = true,
                        MechanismId = mechanism.MechanismId,
                        MechanismDesc = mechanism.MechanismDesc,
                        PreventativeMaintenanceScheduleDays = 180,
                        SLAMinutes = 300
                    };
                    PemsEntities.MechanismMasterCustomers.Add(mechanismMasterCustomer);
                }
            }
            PemsEntities.SaveChanges();


            // Now get the values
            mechanisms = PemsEntities.MechanismMasters.Where(m => m.MeterGroupId == customerAssetTypeModel.GroupId);
            foreach (var mechanism in mechanisms)
            {
                CustomerAssetModel customerAssetModel = new CustomerAssetModel()
                    {
                        DisplayName = mechanism.MechanismDesc,
                        MechanismMasterId = mechanism.MechanismId,
                        MechanismMasterName = mechanism.MechanismDesc
                    };

                // Does this customer already have associated MechanismMasterCustomer?
                var mechanismMasterCustomer = PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == mechanism.MechanismId);
                if (mechanismMasterCustomer == null)
                {
                    // Something bad happened.
                    continue;
                }

                customerAssetModel.Active = mechanismMasterCustomer.IsDisplay;
                customerAssetModel.MechanismMasterId = mechanismMasterCustomer.MechanismId;
                //check for empty string here 
                if (!string.IsNullOrEmpty(mechanismMasterCustomer.MechanismDesc))
                 customerAssetModel.DisplayName = mechanismMasterCustomer.MechanismDesc;
                customerAssetModel.Id = mechanismMasterCustomer.MechanismMasterCustomerId;
                customerAssetModel.MaintenanceSlaDays = mechanismMasterCustomer.SLAMinutes == null ? 0 : (int)mechanismMasterCustomer.SLAMinutes / 1440;
                customerAssetModel.MaintenanceSlaHours = mechanismMasterCustomer.SLAMinutes == null ? 0 : ((int)mechanismMasterCustomer.SLAMinutes % 1440) / 60;
                customerAssetModel.PreventativeMaintenanceSlaDays = mechanismMasterCustomer.PreventativeMaintenanceScheduleDays ?? 0;

                customerAssetTypeModel.Assets.Add(customerAssetModel);
            }
        }


        public CustomerAssetsModel GetCustomerAssetsModel(int customerId)
        {
            CustomerAssetsModel customerAssetsModel = new CustomerAssetsModel()
                {
                    CustomerId = customerId
                };
            CustomerAssetTypeModel customerAssetTypeModel;
            CustomerAssetGroupModel customerAssetGroupModel;


            GetCustomerBaseModel(customerAssetsModel);


            // Get other data
            // There are no table supporting a data-driven approach to generating these
            // entries.  For now, this is hard-coded.

            // Asset Type: Meters
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "METERS", GroupDescription = "Single-Space and Multi-Space Meters", GroupId = 0};
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 0);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }
            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 1);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }

            // Asset Type: Sensors
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "SENSORS", GroupDescription = "Sensors", GroupId = 1 };
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 10);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }


            // Asset Type: Gateways
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "GATEWAYS", GroupDescription = "Communications Gateways", GroupId = 2 };
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 13);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }


            // Asset Type: Spaces
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "SPACES", GroupDescription = "Parking Space", GroupId = 3 };
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 20);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }


            // Asset Type: Datakeys
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "DATAKEYS", GroupDescription = "Datakeys", GroupId = 4 };
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 32);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }


            // Asset Type: Cashboxes
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "CASHBOXES", GroupDescription = "Cash Boxes", GroupId = 5 };
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 11);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }

            // Asset Type: Mechanisms
            customerAssetGroupModel = new CustomerAssetGroupModel() { GroupName = "MECHANISMS", GroupDescription = "Mechanisms", GroupId = 6 };
            customerAssetsModel.AssetGroups.Add(customerAssetGroupModel);

            customerAssetTypeModel = GetCustomerAssetTypeModel(customerId, 31);
            if (customerAssetTypeModel != null)
            {
                PopulateCustomerAssetModels(customerId, customerAssetTypeModel);
                customerAssetGroupModel.AssetTypes.Add(customerAssetTypeModel);
            }

            return customerAssetsModel;
        }

        public void SetCustomerAssetsModel(int customerId, CustomerAssetsModel customerAssetsModel)
        {
            // Walk the Asset Groups
            foreach (var assetGroup in customerAssetsModel.AssetGroups)
            {
                foreach (var assetType in assetGroup.AssetTypes)
                {
                    // Store AssetType results.
                    var customerAssetType = PemsEntities.AssetTypes.FirstOrDefault(m => m.AssetTypeId == assetType.Id);
                    if (customerAssetType != null)
                    {
                        customerAssetType.IsDisplay = assetType.Active;

                        customerAssetType.SLAMinutes = (assetType.MaintenanceSlaHours < 0 ? 0 : assetType.MaintenanceSlaHours * 60)
                                     + ( assetType.MaintenanceSlaDays < 0 ? 0 : assetType.MaintenanceSlaDays*1440 );

                        if (assetType.PreventativeMaintenanceSlaDays < 0)
                            customerAssetType.PreventativeMaintenanceScheduleDays = null;
                        else
                            customerAssetType.PreventativeMaintenanceScheduleDays = assetType.PreventativeMaintenanceSlaDays;
                    }

                    foreach (var asset in assetType.Assets)
                    {
                        var mechanismMasterCustomer = PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.MechanismMasterCustomerId == asset.Id);
                        if (mechanismMasterCustomer != null)
                        {
                            mechanismMasterCustomer.IsDisplay = asset.Active;

                            mechanismMasterCustomer.SLAMinutes = (asset.MaintenanceSlaHours < 0 ? 0 : asset.MaintenanceSlaHours * 60)
                                         + (asset.MaintenanceSlaDays < 0 ? 0 : asset.MaintenanceSlaDays * 1440);

                            if (asset.PreventativeMaintenanceSlaDays < 0)
                                mechanismMasterCustomer.PreventativeMaintenanceScheduleDays = null;
                            else
                                mechanismMasterCustomer.PreventativeMaintenanceScheduleDays = asset.PreventativeMaintenanceSlaDays;

                        }
                    }


                }
            }

            PemsEntities.SaveChanges();
        }

        #endregion

        #region Customer Custom Grids

        /// <summary>
        /// Description: This Method will reorder postion , title and should be hidden or not
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014)  
        /// </summary>
        /// <param name="GridRowdata"></param>
        public void UpdateCustomGridDetails(GridController GridRowdata)
        {
            (new GridFactory()).UpdateCustomGridDetails(GridRowdata);
            return;
        }

        /// <summary>
        /// Description: This Method will retrive controller list given to customer
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public IQueryable<GridController> GetController(int CustomerID)
        {

            var controller = (new GridFactory()).GetController(CustomerID);
            return controller;
        }
        /// <summary>
        /// Description: This Method will retrive action list given to customer and Controllername
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="Controllername"></param>
        /// <returns></returns>
        public IQueryable<GridController> GetActionnames(int CustomerID, string Controllername)
        {
            var actionname = (new GridFactory()).GetActionnames(CustomerID, Controllername);
            return actionname;

        }

        /// <summary>
        /// Description: This Method will retrive grid postion, title, Ishidden
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="Controllername"></param>
        /// <param name="Actionname"></param>
        /// <returns></returns>
        public List<GridController> GetCustmerGrid(int CustomerID, string Controllername, string Actionname)
        {
            var actionname = (new GridFactory()).GetCustmerGrid(CustomerID, Controllername, Actionname);
            return actionname;

        }      

        public CustomerGridsModel GetCustomerGridsModel(int customerId)
        {
            var customerGridsModel = new CustomerGridsModel()
            {
                CustomerId = customerId
            };

            GetCustomerBaseModel(customerGridsModel);

            // Get other data
            customerGridsModel.TemplateSets = (new GridFactory()).GetGridTemplateSets(customerId);

            return customerGridsModel;
        }
               
        //update this method to take user input and save it to the DB
        public void SetCustomerGridsModel(int customerId, CustomerGridsModel customerGridsModel)
        {
            if (customerGridsModel.TemplateSets == null)
                return;

            // For each template set in customerGridsModel.TemplateSets, update CustomerGrid table.
            foreach (var templateSelected in customerGridsModel.TemplateSets)
            {
                // Double check that there is a selected grid template in this selected template.
                if (!templateSelected.Any())
                    continue;

                // Get the GridTemplate associated with this id.
                GridTemplate gridTemplate = (new GridFactory()).GetGridTemplate(templateSelected[0].GridId);

                if (gridTemplate == null)
                    continue;
               
            }
            
            RbacEntities.SaveChanges();

        }

        #endregion

        #region Customer Rules

        public CustomerRulesOperationalTimeModel GetCustomerOperationalTimingModel(int customerId)
        {
            var propFactory = new PropertiesFactory((new SettingsFactory()).GetCustomerConnectionStringName(customerId));
            CustomerRulesOperationalTimeModel model = new CustomerRulesOperationalTimeModel();
            DateTime dt;

            model.HasOperationTime = true;
            model.HasPeakTime = true;
            model.HasEveningTime = true;

            // Operational Times
            if (propFactory.ToDateTime(propFactory.GetOne(customerId, OperationsTimeStart), out dt))
                model.OperationStart = dt;
            else
                model.HasOperationTime = false;

            if (propFactory.ToDateTime(propFactory.GetOne(customerId, OperationsTimeEnd), out dt))
                model.OperationEnd = dt;
            else
                model.HasOperationTime = false;

            model.OperationStartDisplay =
                model.HasOperationTime ? model.OperationStart.ToShortTimeString() : "";
            model.OperationEndDisplay = 
                model.HasOperationTime ? model.OperationEnd.ToShortTimeString() : "";


            // Peak times
            if (propFactory.ToDateTime(propFactory.GetOne(customerId, OperationsPeakStart), out dt))
                model.PeakStart = dt;
            else
                model.HasPeakTime = false;

            if (propFactory.ToDateTime(propFactory.GetOne(customerId, OperationsPeakEnd), out dt))
                model.PeakEnd = dt;
            else
                model.HasPeakTime = false;

            model.PeakStartDisplay =
                model.HasPeakTime ? model.PeakStart.ToShortTimeString() : "";
            model.PeakEndDisplay =
                model.HasPeakTime ? model.PeakEnd.ToShortTimeString() : "";


            // Evening Times
            if (propFactory.ToDateTime(propFactory.GetOne(customerId, OperationsEveningStart), out dt))
                model.EveningStart = dt;
            else
                model.HasEveningTime = false;

            if (propFactory.ToDateTime(propFactory.GetOne(customerId, OperationsEveningEnd), out dt))
                model.EveningEnd = dt;
            else
                model.HasEveningTime = false;

            model.EveningStartDisplay =
                model.HasEveningTime ? model.EveningStart.ToShortTimeString() : "";
            model.EveningEndDisplay =
                model.HasEveningTime ? model.EveningEnd.ToShortTimeString() : "";



            return model;
        }

        public void GetCustomerDiscountSchemas(int customerId, CustomerRulesModel customerRulesModel, DateTime customerNow)
        {
            customerRulesModel.DiscountSchemas = ( from ds in PemsEntities.DiscountSchemes
                                                   where ds.CustomerId == customerId
                                                   where ds.ExpirationDate > customerNow
                                                   select new CustomerRulesDiscountSchemaModel()
                                                       {
                                                           Id = ds.DiscountSchemeID,
                                                           Name = ds.SchemeName,
                                                           Enabled = ds.IsDisplay,
                                                           Type = ds.SchemeType.HasValue ? ds.DiscountSchemeType.DiscountSchemeTypeDesc ?? "" : "",
                                                           ExpirationType = ds.DiscountSchemeExpirationTypeId.HasValue ? ds.DiscountSchemeExpirationType.DiscountSchemeExpirationTypeDesc ?? "" : "",
                                                           DiscountPercent = ds.DiscountPercentage,
                                                           DiscountMinutes = ds.DiscountMinute,
                                                           MaxAmountCents = ds.MaxAmountInCent,
                                                           ActivationDate = ds.ActivationDate,
                                                           ExpirationDate = ds.ExpirationDate
                                                       }).ToList();

            customerRulesModel.DiscountSchema = customerRulesModel.DiscountSchemas.FirstOrDefault(m => m.Enabled) != null;
        }


        public void SetCustomerDiscountSchemas(int customerId, CustomerRulesModel customerRulesModel, DateTime customerNow)
        {
            // Disable all [DiscountSchema] for this customer.  Then enable only the ones they have selected.
            var discountSchema = from ds in PemsEntities.DiscountSchemes
                                 where ds.CustomerId == customerId
                                 where ds.ExpirationDate > customerNow
                                 select ds;
            foreach (var discountScheme in discountSchema)
            {
                discountScheme.IsDisplay = false;

                // If there are relationships to [DiscountSchemeCustomer], clear IsDisplay there too.
                if (discountScheme.DiscountSchemeCustomers.Any())
                {
                    foreach (var dsc in discountScheme.DiscountSchemeCustomers.Where(m => m.CustomerId == customerId))
                    {
                        dsc.IsDisplay = false;
                    }
                }
            }

            // Now if DiscountScheme was selectd then roll thru all schemes and enable where appropriate
            if ( customerRulesModel.DiscountSchema )
            {
                foreach (var schema in customerRulesModel.DiscountSchemas)
                {
                    var discountScheme = PemsEntities.DiscountSchemes.SingleOrDefault( m => m.CustomerId == customerId && m.DiscountSchemeID == schema.Id );
                    if (discountScheme != null)
                    {
                        discountScheme.IsDisplay = schema.Enabled;

                        // If there are relationships to [DiscountSchemeCustomer], set IsDisplay there too.
                        if ( discountScheme.DiscountSchemeCustomers.Any() )
                        {
                            foreach (var dsc in discountScheme.DiscountSchemeCustomers.Where( m => m.CustomerId == customerId ))
                            {
                                dsc.IsDisplay = discountScheme.IsDisplay;
                            }
                        }
                    }
                }

            }
            PemsEntities.SaveChanges();
        }

        public CustomerRulesModel GetCustomerRulesModel(int customerId, DateTime customerNow)
        {
            var propFactory = new PropertiesFactory((new SettingsFactory()).GetCustomerConnectionStringName(customerId));
            var customerRulesModel = new CustomerRulesModel()
            {
                CustomerId = customerId
            };

            Property property;
            bool trueFalse;

            var customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);
            if (customer != null)
            {
                GetCustomerBaseModel( customerRulesModel );

                // Get other data
                customerRulesModel.BlacklistCC = customer.BlackListCC ?? false;

                // Zero Out Meter - Now from PEMS Customers - 07/08/2013
                customerRulesModel.ZeroOutMeter = customer.ZeroOutMeter ?? false;

                // Street Line - Now from PEMS Customers - 07/08/2013
                customerRulesModel.Streetline = customer.Streetline ?? false;

                // Grace Period - Now from PEMS Customers - 07/08/2013
                customerRulesModel.GracePeriod = customer.GracePeriodMinute;

                // Free Parking Limit - Now from PEMS Customers - 07/08/2013
                customerRulesModel.FreeParkingLimit = customer.FreeParkingMinute;

                // Get operational timing
                customerRulesModel.OperationalTimes = GetCustomerOperationalTimingModel( customerId );

                // Get customer discount schemas.
                GetCustomerDiscountSchemas( customerId, customerRulesModel, customerNow );

                //get if this customer resolves individual events when closing a work order or resolving the entire work order.
                customerRulesModel.CloseWorkOrderEvents = bool.Parse(GetCustomerSettingValueOrCreate("CloseWorkOrderEvents", customerId, false.ToString()));

                customerRulesModel.DisplayFullMenu = bool.Parse(GetCustomerSettingValueOrCreate("DisplayFullMenu", customerId, false.ToString()));

                //get the hidden filters for this customer
                customerRulesModel.FieldDemandArea = bool.Parse(GetCustomerSettingValueOrCreate(Constants.HiddenFields.FieldDemandArea, customerId, false.ToString()));
                customerRulesModel.FieldZone = bool.Parse(GetCustomerSettingValueOrCreate(Constants.HiddenFields.FieldZone, customerId, false.ToString()));
                customerRulesModel.FieldDiscountScheme = bool.Parse(GetCustomerSettingValueOrCreate(Constants.HiddenFields.FieldDiscountScheme, customerId, false.ToString()));
                customerRulesModel.FieldCG1 =  bool.Parse(GetCustomerSettingValueOrCreate(Constants.HiddenFields.FieldCG1, customerId, false.ToString()));
                customerRulesModel.FieldCG2 =  bool.Parse(GetCustomerSettingValueOrCreate(Constants.HiddenFields.FieldCG2, customerId, false.ToString()));
                customerRulesModel.FieldCG3 =  bool.Parse(GetCustomerSettingValueOrCreate(Constants.HiddenFields.FieldCG3, customerId, false.ToString()));

                customerRulesModel.PMEventCode = GetCustomerSettingValueOrCreate("PMEventCode", customerId, string.Empty);
            }

            return customerRulesModel;
        }

        public CustomerRulesModel GetCustomerRulesModel(int customerId, CustomerRulesModel customerRulesModel, DateTime customerNow)
        {
            // Get customer Status
            customerRulesModel.Status = GetCustomerStatusModel(customerId);

            return customerRulesModel;
        }

        public void SetCustomerRulesModel(int customerId, CustomerRulesModel customerRulesModel, DateTime customerNow)
        {
            // Save the basic rules.
            var propFactory = new PropertiesFactory((new SettingsFactory()).GetCustomerConnectionStringName(customerId));
            var customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);
            if ( customer != null )
            {
                customer.BlackListCC = customerRulesModel.BlacklistCC;

                // Now to PEMS Customers - 07/08/2013
                customer.ZeroOutMeter = customerRulesModel.ZeroOutMeter;

                // Now to PEMS Customers - 07/08/2013
                customer.Streetline = customerRulesModel.Streetline;

                // Now to PEMS Customers - 07/08/2013
                customer.GracePeriodMinute = customerRulesModel.GracePeriod;

                // Now to PEMS Customers - 07/08/2013
                customer.FreeParkingMinute = customerRulesModel.FreeParkingLimit;

                var settingFactory = new SettingsFactory();
                settingFactory.Set(customerId, "CloseWorkOrderEvents", customerRulesModel.CloseWorkOrderEvents.ToString());

                //preventative maintenance event code
                settingFactory.Set(customerId, "PMEventCode", customerRulesModel.PMEventCode);

                // if displayFullMenu is changed, clear cache
                var oldDisplayFullMenuSetting = settingFactory.GetValue("DisplayFullMenu", customerId);
                if (string.IsNullOrWhiteSpace(oldDisplayFullMenuSetting) ||
                    !oldDisplayFullMenuSetting.Equals(customerRulesModel.DisplayFullMenu.ToString()))
                {
                    settingFactory.Set(customerId, "DisplayFullMenu", customerRulesModel.DisplayFullMenu.ToString());
                    RemoveCachedPemsMenuForCustomer(customer.Name);
                }

                //hidden filters
                settingFactory.Set(customerId, Constants.HiddenFields.FieldDemandArea, customerRulesModel.FieldDemandArea.ToString());
                settingFactory.Set(customerId, Constants.HiddenFields.FieldZone, customerRulesModel.FieldZone.ToString());
                settingFactory.Set(customerId, Constants.HiddenFields.FieldDiscountScheme, customerRulesModel.FieldDiscountScheme.ToString());
                settingFactory.Set(customerId, Constants.HiddenFields.FieldCG1, customerRulesModel.FieldCG1.ToString());
                settingFactory.Set(customerId, Constants.HiddenFields.FieldCG2, customerRulesModel.FieldCG2.ToString());
                settingFactory.Set(customerId, Constants.HiddenFields.FieldCG3, customerRulesModel.FieldCG3.ToString());
            }

            // Save the time spans.
            // Operations times
            if ( customerRulesModel.OperationalTimes.HasOperationTime )
            {
                propFactory.Set(customerId, OperationsTimeStart, customerRulesModel.OperationalTimes.OperationStart.ToShortTimeString());
                propFactory.Set(customerId, OperationsTimeEnd, customerRulesModel.OperationalTimes.OperationEnd.ToShortTimeString());
            }
            else
            {
                propFactory.Set(customerId, OperationsTimeStart, "");
                propFactory.Set(customerId, OperationsTimeEnd, "");
            }

            // Peak times
            if (customerRulesModel.OperationalTimes.HasPeakTime)
            {
                propFactory.Set(customerId, OperationsPeakStart, customerRulesModel.OperationalTimes.PeakStart.ToShortTimeString());
                propFactory.Set(customerId, OperationsPeakEnd, customerRulesModel.OperationalTimes.PeakEnd.ToShortTimeString());
            }
            else
            {
                propFactory.Set(customerId, OperationsPeakStart, "");
                propFactory.Set(customerId, OperationsPeakEnd, "");
            }

            // Evening times
            if (customerRulesModel.OperationalTimes.HasEveningTime)
            {
                propFactory.Set(customerId, OperationsEveningStart, customerRulesModel.OperationalTimes.EveningStart.ToShortTimeString());
                propFactory.Set(customerId, OperationsEveningEnd, customerRulesModel.OperationalTimes.EveningEnd.ToShortTimeString());
            }
            else
            {
                propFactory.Set(customerId, OperationsEveningStart, "");
                propFactory.Set(customerId, OperationsEveningEnd, "");
            }


            SetCustomerDiscountSchemas(customerId, customerRulesModel, customerNow);

            PemsEntities.SaveChanges();
        }

        #endregion

        #region Customer Payments

        private const int SmartCardTypeId = 20;
        private const int PaymentTypeId = 10;


        private void GetCustomerPaymentsCardVsignPartners(int customerId, CustomerPaymentsCardModel customerPaymentsCardModel, string selectedPartner)
        {
            List<PropertyGroupItem> list = (new PropertiesFactory((new SettingsFactory()).GetCustomerConnectionStringName(customerId))).GetList(customerId, "AcquiresVSignPartner");

            customerPaymentsCardModel.VsignPartnerId = 0;

            foreach (var propertyGroupItem in list)
            {
                // Set fallback default selected item.
                customerPaymentsCardModel.VsignPartnerId = customerPaymentsCardModel.VsignPartnerId == 0 ? 
                    propertyGroupItem.Id : customerPaymentsCardModel.VsignPartnerId;
                customerPaymentsCardModel.VsignPartner.Add(new SelectListItem()
                    {
                        Selected = false,
                        Text = propertyGroupItem.Value.Trim(),
                        Value = propertyGroupItem.Id.ToString()
                    });

                if (selectedPartner != null && selectedPartner.Trim().Equals(propertyGroupItem.Value.Trim()))
                    customerPaymentsCardModel.VsignPartnerId = propertyGroupItem.Id;
            }

            // If no item selected then choose default item.
            if ( customerPaymentsCardModel.VsignPartnerId == 0 )
            {
                customerPaymentsCardModel.VsignPartner[0].Selected = true;
                customerPaymentsCardModel.VsignPartnerId = int.Parse( customerPaymentsCardModel.VsignPartner[0].Value );
            }
        }

        private void GetCustomerPaymentsCardGateways(int customerId, CustomerPaymentsCardModel customerPaymentsCardModel, int gatewayId)
        {
            customerPaymentsCardModel.GatewayId = gatewayId;

            foreach (var acquirer in PemsEntities.AcquirerIFs)
            {
                // Set default selected item.
                customerPaymentsCardModel.Gateway.Add(new SelectListItem()
                {
                    Selected = acquirer.AcquirerIFId == customerPaymentsCardModel.GatewayId,
                    Text = acquirer.AcquirerIFDesc,
                    Value = acquirer.AcquirerIFId.ToString()
                });
            }
        }


        private CustomerPaymentsCardModel GetCustomerPaymentsCardModel(int customerId, int cardType)
        {
            CustomerPaymentsCardModel customerPaymentsCardModel = new CustomerPaymentsCardModel();

            string vsignPartner = null;
            int gatewayId = 0;

            var oltAcquirers = PemsEntities.OLTAcquirers.FirstOrDefault( m => m.CardTypeCode == cardType && m.CustomerID == customerId );
            if ( oltAcquirers != null )
            {
                vsignPartner = oltAcquirers.VSignPartner;
                gatewayId = oltAcquirers.AcquirerIF;
                customerPaymentsCardModel.Description = oltAcquirers.Description;

                customerPaymentsCardModel.ReAuthorize = oltAcquirers.ReAuthorise != 0;
                customerPaymentsCardModel.DelayedProcessing = ( oltAcquirers.DelayedProcessing ?? 0 ) != 0;
                customerPaymentsCardModel.CardPresent = ( oltAcquirers.CardPresent ?? 0 ) != 0;

                customerPaymentsCardModel.UserName = oltAcquirers.UserName;
                customerPaymentsCardModel.Password = oltAcquirers.Password;

                customerPaymentsCardModel.MerchantName = oltAcquirers.VendorMerchant;
                customerPaymentsCardModel.AccessCode = oltAcquirers.MigsAC;
            }

            GetCustomerPaymentsCardVsignPartners(customerId, customerPaymentsCardModel, vsignPartner);
            GetCustomerPaymentsCardGateways(customerId, customerPaymentsCardModel, gatewayId);

            return customerPaymentsCardModel;
        }

        private void SetCustomerPaymentsCardModel(int customerId, int cardType, CustomerPaymentsCardModel customerPaymentsCardModel)
        {
            var oltAcquirers = PemsEntities.OLTAcquirers.FirstOrDefault(m => m.CardTypeCode == cardType && m.CustomerID == customerId);
            if ( oltAcquirers == null )
            {
                // Create new OLTAcquirers entry
                oltAcquirers = new OLTAcquirer()
                    {
                        CustomerID =  customerId,
                        CardTypeCode =  cardType
                    };
                PemsEntities.OLTAcquirers.Add( oltAcquirers );
            }


            // Save Acquirer
            oltAcquirers.AcquirerIF = customerPaymentsCardModel.GatewayId;

            // If Acquirer is PayPal then save 
            //  oltAcquirers.ReAuthorise
            //  oltAcquirers.DelayedProcessing
            //  oltAcquirers.VSignPartner
            // otherwise null them out.
            if ( customerPaymentsCardModel.GatewayId == 0 )
            {
                // Save VSignPartner
                oltAcquirers.ReAuthorise = (customerPaymentsCardModel.ReAuthorize ? 1 : 0);
                oltAcquirers.DelayedProcessing = (customerPaymentsCardModel.DelayedProcessing ? 1 : 0);
                var customerProperty = PemsEntities.CustomerProperties.FirstOrDefault(m => m.CustomerPropertyId == customerPaymentsCardModel.VsignPartnerId);
                oltAcquirers.VSignPartner = customerProperty == null ? null : customerProperty.PropertyDesc;
            }
            else
            {
                oltAcquirers.ReAuthorise = 0;
                oltAcquirers.DelayedProcessing = null;
                oltAcquirers.VSignPartner = null;
            }

            oltAcquirers.Description = customerPaymentsCardModel.Description;
            oltAcquirers.CardPresent = (customerPaymentsCardModel.CardPresent ? 1 : 0);
            oltAcquirers.UserName = customerPaymentsCardModel.UserName ?? "";
            oltAcquirers.Password = customerPaymentsCardModel.Password ?? "";

            oltAcquirers.VendorMerchant = customerPaymentsCardModel.MerchantName ?? "";
            oltAcquirers.MigsAC = customerPaymentsCardModel.AccessCode ?? null;

            PemsEntities.SaveChanges();
        }


        private CustomerPaymentsCreditDebitModel GetCustomerPaymentsCreditDebitModel(int customerId)
        {
            CustomerPaymentsCreditDebitModel customerPaymentsCreditDebitModel = new CustomerPaymentsCreditDebitModel();

            // Get list of allowable credit cards.
            foreach (var cct in PemsEntities.CreditCardTypes)
            {
                customerPaymentsCreditDebitModel.Cards.Add(new SelectListItem()
                    {
                        Text = cct.Name,
                        Value = cct.CreditCardType1.ToString(),
                        Selected = PemsEntities.CreditCardTypesCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.CreditCardType == cct.CreditCardType1) != null
                    } );
            }

            // Get hash digits.
            var hashDigits = PemsEntities.OLTCardHashes.FirstOrDefault( m => m.CustomerId == customerId );
            customerPaymentsCreditDebitModel.HashFirst = hashDigits == null ? 6 : hashDigits.Fdigit;
            customerPaymentsCreditDebitModel.HashLast = hashDigits == null ? 4 : hashDigits.Ldigit;

            // Get rollup and tx timings
            var customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);
            customerPaymentsCreditDebitModel.DaysToWaitToReconcile = customer == null ? 0 : customer.UnReconcileCleanupLag ?? 0;
            customerPaymentsCreditDebitModel.SecondsGap = customer == null ? 0 : customer.TxTiming ?? 0;



            return customerPaymentsCreditDebitModel;
        }

        private void SetCustomerPaymentsCreditDebitModel(int customerId, CustomerPaymentsCreditDebitModel customerPaymentsCreditDebitModel)
        {
            // Save selected credit cards to SupportedCreditCards table.

            if (customerPaymentsCreditDebitModel.Cards.Any())
            {
                foreach (var card in customerPaymentsCreditDebitModel.Cards)
                {
                    int creditCardType = int.Parse( card.Value );
                    var scc = PemsEntities.CreditCardTypesCustomers.FirstOrDefault(m => m.CreditCardType == creditCardType && m.CustomerId == customerId);
                    // Did this card exist for this customer?  Should it?
                    if ( card.Selected && scc == null )
                    {
                        // If card was  selected and it is not in SupportedCreditCards, add it.
                        PemsEntities.CreditCardTypesCustomers.Add(new CreditCardTypesCustomer()
                            {
                                CustomerId =  customerId,
                                CreditCardType =  creditCardType
                            } );
                    }
                    else if ( !card.Selected && scc != null )
                    {
                        // If card was not selected and it is in SupportedCreditCards, remove it.
                        PemsEntities.CreditCardTypesCustomers.Remove(scc);
                    }
                }
            }
            else
            {
                // Neither Credit or Debit selected.  Clear out any entries in SupportedCreditCards
                var ccds = from sccs in PemsEntities.SupportedCreditCards where sccs.CustomerID == customerId select sccs;
                foreach (var card in ccds)
                {
                    PemsEntities.SupportedCreditCards.Remove(card);
                }
            }



            // Save the rollup and tx timings.
            var customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);
            if ( customer != null )
            {
                customer.UnReconcileCleanupLag = customerPaymentsCreditDebitModel.DaysToWaitToReconcile;
                customer.TxTiming = customerPaymentsCreditDebitModel.SecondsGap;
            }


            // Save hash digits.
            var hashDigits = PemsEntities.OLTCardHashes.FirstOrDefault(m => m.CustomerId == customerId);
            if ( hashDigits == null )
            {
                hashDigits = new OLTCardHash()
                    {
                        CustomerId = customerId,
                        InsDate = DateTime.Now
                    };
                PemsEntities.OLTCardHashes.Add( hashDigits );
            }
            hashDigits.Fdigit = customerPaymentsCreditDebitModel.HashFirst;
            hashDigits.Ldigit = customerPaymentsCreditDebitModel.HashLast;


            PemsEntities.SaveChanges();
        }


        private CustomerPaymentsEmvModel GetCustomerPaymentsEmvModel(int customerId)
        {
            CustomerPaymentsEmvModel customerPaymentsEmvModel = new CustomerPaymentsEmvModel();

            var customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);

            customerPaymentsEmvModel.PayByPhone = customer != null ? customer.IsPayByPhone : false;
            customerPaymentsEmvModel.Emv = customer != null ? customer.IsEMV : false;
            
            return customerPaymentsEmvModel;
        }

        private void SetCustomerPaymentsEmvModel(int customerId, CustomerPaymentsEmvModel customerPaymentsEmvModel)
        {
            var customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);
            if (customer != null)
            {
                customer.IsPayByPhone = customerPaymentsEmvModel.PayByPhone;
                customer.IsEMV = customerPaymentsEmvModel.Emv;
                PemsEntities.SaveChanges();
            }
        }


        public void GetCustomerPaymentsCoinsModel(int customerId, CustomerPaymentsModel customerPaymentsModel)
        {
            customerPaymentsModel.Coins = new CustomerPaymentsCoinsModel();
            customerPaymentsModel.AllCoins = new CustomerPaymentsCoinsModel();

            // Get any coin denominations assigned to this customer.
            customerPaymentsModel.Coins.Coins = PemsEntities.CoinDenominationCustomers
                           .Where(m => m.CustomerId == customerId)
                           .Join(PemsEntities.CoinDenominations, cdc => cdc.CoinDenominationId, cd => cd.CoinDenominationId, (cdc, cd) => new { cdc, cd })
                           .OrderBy(x=>x.cdc.CoinTypeOrdinal)
                           .Select(x => new CustomerPaymentsCoinModel()
                           {
                               Id = x.cdc.CoinDenominationCustomerId,
                               CountryCode = x.cd.CountryCode,
                               DenominationId = x.cdc.CoinDenominationId,
                               DenominationValue = x.cd.CoinValue,
                               DenominationName = x.cd.CoinName,
                               Name = x.cdc.CoinName,
                               Enabled = x.cdc.IsDisplay
                           }).ToList();

            // Get coin country, if one is selected.
            string selectedCountry = "NONE";
            if ( customerPaymentsModel.Coins.HasCoinsSelected )
            {
                int denominationId = customerPaymentsModel.Coins.Coins[0].DenominationId;
                var coinDenomination = PemsEntities.CoinDenominations.FirstOrDefault( m => m.CoinDenominationId == denominationId );

                selectedCountry = coinDenomination.CountryCode;
            }

            // Get list of countries that have coin types.
            customerPaymentsModel.Coins.CoinCountry.Add(new SelectListItem()
                {
                    Selected = selectedCountry.Equals("NONE"),
                    Text = "Select Country",
                    Value = "NONE"
                });
            var coinCountries = PemsEntities.CoinDenominations.Select( m => m.CountryCode ).Distinct();
            foreach (var coinCountry in coinCountries)
            {
                customerPaymentsModel.Coins.CoinCountry.Add(new SelectListItem()
                {
                    Selected = selectedCountry.Equals(coinCountry),
                    Text = coinCountry,
                    Value = coinCountry
                });
            }
        }

        /// <summary>
        /// Sets the cusomer data in table [CoinDenominationCustomer]
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="coinCountry"></param>
        public void SetCustomerCoinCountry(int customerId, string coinCountry)
        {
            bool saveChanges = false;
            var coinDenominations = PemsEntities.CoinDenominations.Where( m => m.CountryCode.Equals( coinCountry ) );
            foreach (var coinDenomination in coinDenominations)
            {
                // Only add if does not already exist [CoinDenominationCustomer]
                CoinDenominationCustomer cdc = PemsEntities.CoinDenominationCustomers.FirstOrDefault( m => m.CoinDenominationId == coinDenomination.CoinDenominationId
                    && m.CustomerId == customerId);
                if ( cdc == null )
                {

                    cdc = new CoinDenominationCustomer()
                        {
                            CoinDenominationId = coinDenomination.CoinDenominationId,
                            CustomerId = customerId,
                            CoinName = coinDenomination.CoinName,
                            IsDisplay = false
                        };

                    PemsEntities.CoinDenominationCustomers.Add( cdc );
                    saveChanges = true;
                }
            }

            if ( saveChanges )
            {
                PemsEntities.SaveChanges();
            }
        }


        public bool ValidateCoinCountry(string coinCountry)
        {
            return PemsEntities.CoinDenominations.FirstOrDefault( m => m.CountryCode.Equals( coinCountry ) ) != null;
        }

        /// <summary>
        /// removes all meters from the collection run (colleciton run meters table) based ont he colleciton run meter ID
        /// </summary>
        public void RemoveCoinsForCustomer(int customerId)
        {
            var items = PemsEntities.CoinDenominationCustomers.Where(x => x.CustomerId == customerId).ToList();
            if (items.Any())
            {
                foreach (var item in items)
                    PemsEntities.CoinDenominationCustomers.Remove(item);
                PemsEntities.SaveChanges();
            }
        }


        private void SetCustomerPaymentsCoinsModel(int customerId, CustomerPaymentsModel customerPaymentsModel)
        {
            // Walk all of the coins and update.
            foreach (var coin in customerPaymentsModel.Coins.Coins)
            {
                var customerCoin = PemsEntities.CoinDenominationCustomers.FirstOrDefault( m => m.CustomerId == customerId && m.CoinDenominationCustomerId == coin.Id );
                if ( customerCoin != null )
                {
                    customerCoin.IsDisplay = coin.Enabled;
                    customerCoin.CoinName = coin.Name;
                }
            }

            PemsEntities.SaveChanges();
        }

        public CustomerPaymentsModel GetCustomerPaymentsModel(int customerId)
        {
            CustomerPaymentsModel customerPaymentsModel = new CustomerPaymentsModel()
            {
                CustomerId = customerId
            };


            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            if (customerProfile != null)
            {
                GetCustomerBaseModel( customerPaymentsModel);

                // Get other data
                customerPaymentsModel.SmartCardGateway = GetCustomerPaymentsCardModel(customerId, CustomerFactory.SmartCardTypeId);
                customerPaymentsModel.PaymentGateway = GetCustomerPaymentsCardModel(customerId, CustomerFactory.PaymentTypeId);
                customerPaymentsModel.CreditDebit = GetCustomerPaymentsCreditDebitModel(customerId);
                customerPaymentsModel.EmvPhone = GetCustomerPaymentsEmvModel( customerId );
                GetCustomerPaymentsCoinsModel(customerId, customerPaymentsModel);
            }

            return customerPaymentsModel;
        }

        public void SetCustomerPaymentsModel(int customerId, CustomerPaymentsModel customerPaymentsModel)
        {
            SetCustomerPaymentsCardModel(customerId, CustomerFactory.SmartCardTypeId, customerPaymentsModel.SmartCardGateway);
            SetCustomerPaymentsCardModel(customerId, CustomerFactory.PaymentTypeId, customerPaymentsModel.PaymentGateway);
            SetCustomerPaymentsCreditDebitModel(customerId, customerPaymentsModel.CreditDebit);
            SetCustomerPaymentsEmvModel(customerId, customerPaymentsModel.EmvPhone);
            SetCustomerPaymentsCoinsModel(customerId, customerPaymentsModel);
        }

        #endregion

        #region Customer Custom Labels

        private List<SelectListItem> GetLabelOptions(string label, SettingsFactory.SettingType type, string selectedLabel)
        {
            var selectList = new List<SelectListItem>();
            List<Setting> settings = (new SettingsFactory()).GetList(label, type);

            // Create default (not selected) item.  Set it as first in list.
            bool hasSelectedItem = false;
            var defaultSelectListItem = new SelectListItem()
                {
                    Text = "",
                    Value = "0"
                };
            selectList.Add( defaultSelectListItem );

            foreach (Setting setting in settings)
            {
                var selectListItem = new SelectListItem()
                {
                    Text = setting.Value,
                    Value = setting.Id.ToString(),
                    Selected = (selectedLabel != null && selectedLabel.Equals( setting.Value ))
                };
                // Is this the selected item?
                hasSelectedItem |= selectListItem.Selected;
                selectList.Add(selectListItem);
            }

            // If there is no selected item, choose the default item.
            if ( !hasSelectedItem )
                defaultSelectListItem.Selected = true;

            return selectList;
        }

// 07/08/2013        private string[] InternalLabels = { "CustomLocationGroup1", "CustomLocationGroup2", "CustomLocationGroup3" };

        //private List<CustomerLabel> GetCustomerLabelsInternal(int customerId)
        //{
        //    List<CustomerLabel> list = new List<CustomerLabel>();

        //    // CustomGroup1
        //    var customGroup1 = PemsEntities.CustomGroup1.SingleOrDefault(m => m.CustomerId == customerId);
        //    CustomerLabel customerLabel = new CustomerLabel()
        //    {
        //        LabelName = InternalLabels[0],
        //        LabelId = 0,
        //        CustomLabelList = GetLabelOptions(InternalLabels[0], SettingsFactory.SettingType.InternalLabelSelectList, customGroup1 != null ? customGroup1.DisplayName : null)
        //    };
        //    list.Add(customerLabel);

        //    // CustomGroup2
        //    var customGroup2 = PemsEntities.CustomGroup2.SingleOrDefault(m => m.CustomerId == customerId);
        //    customerLabel = new CustomerLabel()
        //    {
        //        LabelName = InternalLabels[1],
        //        LabelId = 1,
        //        CustomLabelList = GetLabelOptions(InternalLabels[1], SettingsFactory.SettingType.InternalLabelSelectList, customGroup2 != null ? customGroup2.DisplayName : null)
        //    };
        //    list.Add(customerLabel);

        //    // CustomGroup3
        //    var customGroup3 = PemsEntities.CustomGroup3.SingleOrDefault(m => m.CustomerId == customerId);
        //    customerLabel = new CustomerLabel()
        //    {
        //        LabelName = InternalLabels[2],
        //        LabelId = 2,
        //        CustomLabelList = GetLabelOptions(InternalLabels[2], SettingsFactory.SettingType.InternalLabelSelectList, customGroup3 != null ? customGroup3.DisplayName : null)
        //    };
        //    list.Add(customerLabel);

        //    return list;
        //}

        //private void SetCustomerLabelsInternal(int customerId, List<CustomerLabel> list)
        //{
        //    var settingFactory = new SettingsFactory();
        //    int maxId = 1;
        //    foreach (var customerLabel in list)
        //    {
        //        if ( customerLabel.LabelId < InternalLabels.GetLowerBound( 0 ) ||
        //             customerLabel.LabelId > InternalLabels.GetUpperBound( 0 ) )
        //            continue;

        //        // There should be one and only one item in the customerLabel.CustomLabelList
        //        // The Id of the selected value can be found in this item in its Value attribute.
        //        int selectedId = int.Parse(customerLabel.CustomLabelList[0].Value);

        //        switch (customerLabel.LabelId)
        //        {
        //            case 0:
        //                var customGroup1 = PemsEntities.CustomGroup1.SingleOrDefault(m => m.CustomerId == customerId );
        //                if ( selectedId == 0 )
        //                {
        //                    if(customGroup1 != null )
        //                    {
        //                        // Remove this record from PemsEntities.CustomGroup1
        //                        PemsEntities.CustomGroup1.Remove( customGroup1 );
        //                    }
        //                    // If customGroup1 == null, do nothing.
        //                    continue;
        //                }
        //                else
        //                {
        //                    Setting selectedValue = settingFactory.GetListValue(InternalLabels[customerLabel.LabelId], selectedId);
        //                    if (customGroup1 != null)
        //                    {
        //                        // Update this record
        //                        customGroup1.DisplayName = selectedValue.Value;
        //                    }
        //                    else
        //                    {
        //                        maxId = PemsEntities.CustomGroup1.Any() ? PemsEntities.CustomGroup1.Max(m => m.CustomGroupId) : 0;
        //                        customGroup1 = new CustomGroup1()
        //                            {
        //                                CustomerId = customerId,
        //                                DisplayName = selectedValue.Value,
        //                                Comment = "Via PEMS App",
        //                                CustomGroupId = (maxId + 1)
        //                            };
        //                        PemsEntities.CustomGroup1.Add( customGroup1 );
        //                    }
        //                }
        //                break;
        //            case 1:
        //                var customGroup2 = PemsEntities.CustomGroup2.SingleOrDefault(m => m.CustomerId == customerId);
        //                if (selectedId == 0)
        //                {
        //                    if (customGroup2 != null)
        //                    {
        //                        // Remove this record from PemsEntities.CustomGroup2
        //                        PemsEntities.CustomGroup2.Remove(customGroup2);
        //                    }
        //                    // If customGroup2 == null, do nothing.
        //                    continue;
        //                }
        //                else
        //                {
        //                    Setting selectedValue = settingFactory.GetListValue(InternalLabels[customerLabel.LabelId], selectedId);
        //                    if (customGroup2 != null)
        //                    {
        //                        // Update this record
        //                        customGroup2.DisplayName = selectedValue.Value;
        //                    }
        //                    else
        //                    {
        //                        // Get max id
        //                        maxId = PemsEntities.CustomGroup2.Any() ? PemsEntities.CustomGroup2.Max(m => m.CustomGroupId) : 0;
        //                        customGroup2 = new CustomGroup2()
        //                        {
        //                            CustomerId = customerId,
        //                            DisplayName = selectedValue.Value,
        //                            Comment = "Via PEMS App",
        //                            CustomGroupId = (maxId + 1)
        //                        };
        //                        PemsEntities.CustomGroup2.Add(customGroup2);
        //                    }
        //                }
        //                break;
        //            case 2:
        //                var customGroup3 = PemsEntities.CustomGroup3.SingleOrDefault(m => m.CustomerId == customerId );
        //                if ( selectedId == 0 )
        //                {
        //                    if(customGroup3 != null )
        //                    {
        //                        // Remove this record from PemsEntities.CustomGroup3
        //                        PemsEntities.CustomGroup3.Remove( customGroup3 );
        //                    }
        //                    // If customGroup3 == null, do nothing.
        //                    continue;
        //                }
        //                else
        //                {
        //                    Setting selectedValue = settingFactory.GetListValue(InternalLabels[customerLabel.LabelId], selectedId);
        //                    if (customGroup3 != null)
        //                    {
        //                        // Update this record
        //                        customGroup3.DisplayName = selectedValue.Value;
        //                    }
        //                    else
        //                    {
        //                        maxId = PemsEntities.CustomGroup3.Any() ? PemsEntities.CustomGroup3.Max(m => m.CustomGroupId) : 0;
        //                        customGroup3 = new CustomGroup3()
        //                            {
        //                                CustomerId = customerId,
        //                                DisplayName = selectedValue.Value,
        //                                Comment = "Via PEMS App",
        //                                CustomGroupId = (maxId + 1)
        //                            };
        //                        PemsEntities.CustomGroup3.Add( customGroup3 );
        //                    }
        //                }
        //                break;

        //        }
        //    }

        //    PemsEntities.SaveChanges();
        //}

        private List<CustomerLabel> GetCustomerLabels(int customerId, string type)
        {
            List<CustomerLabel> list = new List<CustomerLabel>();
            List<SelectListItem> selectList;
            CustomerLabel customerLabel;

            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault( m => m.CustomerId == customerId );

            // Get all records from LocaleResourceTypes of type Constants.CustomLocaleResources.Label
            var labelCandidates = RbacEntities.LocaleResources.Where(m => m.Type == type 
                && m.CultureCode.Equals(customerProfile.DefaultLocale, StringComparison.InvariantCultureIgnoreCase));
            foreach (var labelCandidate in labelCandidates)
            {
                customerLabel = new CustomerLabel()
                {
                    StockLabel = labelCandidate.Value,
                    LabelName = labelCandidate.Name,
                    LabelId = labelCandidate.Id
                };

                // Has a customized version of this label been selected?
                var replacementLabel = RbacEntities.LocaleResourcesCustoms.SingleOrDefault( m => m.CustomerId == customerId
                    && m.Type == labelCandidate.Type && m.Name == labelCandidate.Name);
                selectList = GetLabelOptions(labelCandidate.Name, SettingsFactory.SettingType.FieldLabelSelectList, replacementLabel != null ? replacementLabel.Value : null);

                // Is there an associated list of values to select from?
                if ( selectList.Count > 1 )
                {
                    customerLabel.CustomLabelList = selectList;
                }
                else
                {
                    customerLabel.CustomLabel = replacementLabel != null ? replacementLabel.Value : "";
                }


                if ( !string.IsNullOrWhiteSpace( customerLabel.LabelName ) )
                {
                    list.Add(customerLabel);
                }
            }

            list.Sort();

            return list;
        }

        private void SetCustomerLabels(int customerId, List<CustomerLabel> list)
        {
            var settingFactory = new SettingsFactory();
            foreach (var customerLabel in list)
            {
                string newLabel = "";

                // Is this a single value or a selected value from a dropdown?
                if ( customerLabel.CustomLabel == null )
                {
                    // Dropdown
                    // There should be one and only one item in the customerLabel.CustomLabelList
                    // The Id of the selected value can be found in this item in its Value attribute.
                    int selectedId;
                    if (int.TryParse(customerLabel.CustomLabelList[0].Value, out selectedId))
                    {
                        Setting selectedValue = settingFactory.GetListValue(selectedId);
                        if (selectedValue != null)
                            newLabel = selectedValue.Value;
                    }
                }
                else
                {
                    if ( customerLabel.CustomLabel != null )
                        newLabel = customerLabel.CustomLabel;
                }


                // Get the associated RbacEntities.LocaleResource element.
                var localeResource = RbacEntities.LocaleResources.FirstOrDefault( m => m.Id == customerLabel.LabelId );

                if ( localeResource == null )
                    continue;

                // See if there is already a custom label
                var localeResourceCustom = RbacEntities.LocaleResourcesCustoms.FirstOrDefault(m => m.Type == localeResource.Type 
                    && m.Name == localeResource.Name && m.CustomerId == customerId);

                if ( string.IsNullOrWhiteSpace( newLabel ) )
                {
                    if ( localeResourceCustom != null )
                    {
                        // If there was a custom label then remove it.
                        RbacEntities.LocaleResourcesCustoms.Remove( localeResourceCustom );
                    }
                    // If localeResourceCustom == null, do nothing.
                    continue;
                }
                else
                {
                    if (localeResourceCustom != null)
                    {
                        // Update this record
                        localeResourceCustom.Value = newLabel;
                    }
                    else
                    {
                        localeResourceCustom = new LocaleResourcesCustom()
                            {
                                CustomerId = customerId,
                                Name = localeResource.Name,
                                Type = localeResource.Type,
                                Value = newLabel
                            };
                        RbacEntities.LocaleResourcesCustoms.Add( localeResourceCustom );
                    }
                }
            }

            RbacEntities.SaveChanges();
        }

        public CustomerLabelsModel GetCustomerLabelsModel(int customerId)
        {
            var customerLabelsModel = new CustomerLabelsModel()
            {
                CustomerId = customerId
            };


            GetCustomerBaseModel(customerLabelsModel);

            //// Internal labels
            //customerLabelsModel.InternalLabels = GetCustomerLabelsInternal(customerId);

            // Field Labels
            customerLabelsModel.LabelGroups[ResourceTypes.Label] = GetCustomerLabels(customerId, ResourceTypes.Label);

            // Grid Labels
            customerLabelsModel.LabelGroups[ResourceTypes.GridColumn] = GetCustomerLabels(customerId, ResourceTypes.GridColumn);

            return customerLabelsModel;
        }

        public CustomerLabelsModel GetCustomerLabelsModel(int customerId, CustomerLabelsModel customerLabelsModel)
        {
            // Get customer Status
            customerLabelsModel.Status = GetCustomerStatusModel(customerId);

            return customerLabelsModel;
        }

        public void SetCustomerLabelsModel(int customerId, CustomerLabelsModel customerLabelsModel)
        {
            SetCustomerLabels(customerId, customerLabelsModel.LabelGroups[ResourceTypes.Label]);
            SetCustomerLabels(customerId, customerLabelsModel.LabelGroups[ResourceTypes.GridColumn]);
            //we have to make sure we are clearing the custom label cache for this customer
            SqlResourceHelper.ResetCustomerResourceCache(customerId);
        }

        #endregion

        #region Customer Event Codes


        public CustomerEventCodesModel GetCustomerEventCodesModel(int customerId)
        {
            var customerEventCodesModel = new CustomerEventCodesModel()
            {
                CustomerId = customerId
            };

            GetCustomerBaseModel(customerEventCodesModel);
            return customerEventCodesModel;
        }

        public CustomerEventCodesModel GetCustomerEventCodesModel(int customerId, CustomerEventCodesModel customerEventCodesModel)
        {
            // Get customer Status
            customerEventCodesModel.Status = GetCustomerStatusModel(customerId);

            return customerEventCodesModel;
        }

        public void SetCustomerEventCodesModel(int customerId, CustomerEventCodesModel customerEventCodesModel)
        {
        }

        #endregion

        #region Customer Demand Zones

        /// <summary>
        /// Description:GetCustomerDemandZonesModel will in turn call 'GetDemandZones' method ro fill the model with demand zone details.
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public CustomerDemandZonesModel GetCustomerDemandZonesModel(int customerId)
        {
            CustomerDemandZonesModel model = new CustomerDemandZonesModel()
            {
                CustomerId = customerId
            };

            GetCustomerBaseModel(model);

            model.DemandZones = GetDemandZones(customerId);
            return model;
        }

        /// <summary>
        /// Description:GetDemandZones will query the table 'DemandZoneCustomers' in database for demand zone details for the perticular customer and return a list of the same.
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>

        private List<CustomerDemandZone> GetDemandZones(int customerId)
        {
            var list = new List<CustomerDemandZone>();
            list = (from dzc in PemsEntities.DemandZoneCustomers
                    join dz in PemsEntities.DemandZones on dzc.DemandZoneId equals dz.DemandZoneId
                    where dzc.CustomerId == customerId && dz.DemandZoneDesc != ""  //dzc.IsDisplay == true && removed by prita
                    orderby dz.DemandZoneId
                    select new CustomerDemandZone()
                    {
                        CustomerId = dzc.CustomerId,
                        DemandZoneId = dz.DemandZoneId,
                        DemandZoneCustomerId = dzc.DemandZoneCustomerId,
                        DemandZoneDesc = dz.DemandZoneDesc,
                        IsDisplay = (bool)dzc.IsDisplay,
                    }).ToList();
            return list;
        }

        /// <summary>
        /// Description:CheckIfExist will query the table 'Meters' in database to check if the demand zone is getting used by any meter record.
        /// This is to see if the demand zone can be altered or not
        /// Modified:Prita()
        /// </summary>
        /// <param name="CustID"></param>
        /// <param name="DmndID"></param>
        /// <returns></returns>
        public bool CheckIfExist(int CustID, int DmndID)
        {
            var DemandZoneExists = (from s in PemsEntities.Meters
                                    where s.CustomerID == CustID && s.DemandZone == DmndID
                                    select s).FirstOrDefault();
            if (DemandZoneExists != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Description:save the selected customer demand zones
        /// Modified:Prita()
        /// </summary>
        /// <param name="model"></param>
        public void SetCustomerDemandZonesModel(CustomerDemandZone model)
        {
            if (model != null)
            {

                var stud = (from s in PemsEntities.DemandZoneCustomers
                            where s.CustomerId == model.CustomerId && s.DemandZoneId == model.DemandZoneId
                            select s).FirstOrDefault();

                stud.IsDisplay = ((bool)model.IsDisplay);
                PemsEntities.SaveChanges();

            }
        }

        /// <summary>
        /// Description:Delete the selected demandzone from 'DemandZoneCustomers' table
        /// Modified:Prita() 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="DemandZoneID"></param>
        public void DeleteDemandZoneCustomers(int customerId, int DemandZoneID)
        {

            var stud = (from s in PemsEntities.DemandZoneCustomers
                        where s.CustomerId == customerId && s.DemandZoneId == DemandZoneID
                        select s).FirstOrDefault();

            PemsEntities.DemandZoneCustomers.Remove(stud);
            PemsEntities.SaveChanges();

        }


        /// <summary>
        /// Description:This method will fetch the demand zone details for the perticular customer from DemandZoneCustomers table to be displayed in EditDemandZones grid
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="DemandZoneID"></param>
        /// <returns></returns>
        public CustomerDemandZone GetDemandZoneEditModel(int customerId, int DemandZoneID)
        {
            CustomerDemandZone model = null;

            model = (from dzc in PemsEntities.DemandZoneCustomers
                     join dz in PemsEntities.DemandZones on dzc.DemandZoneId equals dz.DemandZoneId
                     join cust in PemsEntities.Customers on dzc.CustomerId equals cust.CustomerID
                     where dzc.CustomerId == customerId && dzc.DemandZoneId == DemandZoneID && dz.DemandZoneDesc != ""  //dzc.IsDisplay == true && removed by prita
                     orderby dz.DemandZoneId
                     select new CustomerDemandZone()
                     {
                         CustomerId = dzc.CustomerId,
                         DemandZoneId = dz.DemandZoneId,
                         DemandZoneCustomerName = cust.Name,
                         DemandZoneCustomerId = dzc.DemandZoneCustomerId,
                         DemandZoneDesc = dz.DemandZoneDesc,
                         IsDisplay = (bool)dzc.IsDisplay
                     }).FirstOrDefault();

            //To This is to maintain an alert message if the demand zone es=xist in meter.This alert will show if user try to modify the data.
            var DemandZoneExists = (from s in PemsEntities.Meters
                                    where s.CustomerID == customerId && s.DemandZone == DemandZoneID
                                    select s).FirstOrDefault();
            if (DemandZoneExists != null)
            {
                model.getResponse = "Assets exist for currents customer and selected demand zone,Hence changes cannot be applied";
            }
            else
            {
                model.getResponse = "";
            }
            return model;

        }

        public IQueryable<FDFilesModel> DemandZonesList()
        {
            var DemandZoneslist = (from pam in PemsEntities.DemandZones
                                   select new FDFilesModel
                                   {                                       
                                       FileName = pam.DemandZoneDesc,
                                       FileType = pam.DemandZoneId,
                                   });

            return DemandZoneslist;
        }

        public bool CheckIfDemandZoneExist(int CustID, int DzID)
        {
            var DemandZoneExists = (from s in PemsEntities.DemandZoneCustomers
                                    where s.CustomerId == CustID && s.DemandZoneId == DzID
                                    select s).FirstOrDefault();
            if (DemandZoneExists != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SaveDemandZone(int CustID, int DzID)
        {
            using (PEMEntities context = new PEMEntities())
            {

                DemandZoneCustomer DemandZoneCust = new DemandZoneCustomer();
                DemandZoneCust.CustomerId = CustID;
                DemandZoneCust.DemandZoneId = DzID;
                DemandZoneCust.IsDisplay = true;
                context.DemandZoneCustomers.Add(DemandZoneCust);
                context.SaveChanges();

            }
        }

        #endregion 
        
        #region Customer Areas


        public CustomerAreasModel GetCustomerAreasModel(int customerId)
        {
            CustomerAreasModel model = new CustomerAreasModel()
            {
                CustomerId = customerId
            };


            GetCustomerBaseModel( model);

            // Area - Customer Areas
            model.Areas = GetAreas(customerId);
            //model.AreasList = GetAreasFullList(customerId);


            // Area - Customer Zones
            model.Zones = GetZones(customerId);
            //model.ZoneList = GetZoneList(customerId);

            // Custom Group 1
            model.CustomGroup1 = (from cg in PemsEntities.CustomGroup1
                    where cg.CustomerId == customerId
                    select new CustomerCustomGroup()
                    {
                        CustomerId = customerId,
                        Id = cg.CustomGroupId,
                        Name = cg.DisplayName,
                        Description = cg.Comment
                    }).ToList();

            model.CustomGroup1.Sort();

            // Custom Group 2
            model.CustomGroup2 = (from cg in PemsEntities.CustomGroup2
                                  where cg.CustomerId == customerId
                                  select new CustomerCustomGroup()
                                  {
                                      CustomerId = customerId,
                                      Id = cg.CustomGroupId,
                                      Name = cg.DisplayName,
                                      Description = cg.Comment
                                  }).ToList();

            model.CustomGroup2.Sort();

            // Custom Group 3
            model.CustomGroup3 = (from cg in PemsEntities.CustomGroup3
                                  where cg.CustomerId == customerId
                                  select new CustomerCustomGroup()
                                  {
                                      CustomerId = customerId,
                                      Id = cg.CustomGroupId,
                                      Name = cg.DisplayName,
                                      Description = cg.Comment
                                  }).ToList();

            model.CustomGroup3.Sort();

            return model;
        }


        private List<CustomerArea> GetAreas(int customerId)
        {
            List<CustomerArea> list = new List<CustomerArea>();

            list = (from area in PemsEntities.Areas
                    where area.CustomerID == customerId
                    select new CustomerArea()
                    {
                        CustomerId = customerId,
                        GlobalId = area.GlobalAreaID ?? 0,
                        Id = area.AreaID,
                        Name = area.AreaName,
                        Description = area.Description ?? "",
                    }).ToList();

            list.Sort();
            return list;
        }

        private List<CustomerZone> GetZones(int customerId)
        {
            List<CustomerZone> list = new List<CustomerZone>();

            list = (from zone in PemsEntities.Zones
                    where zone.customerID == customerId
                    select new CustomerZone()
                    {
                        CustomerId = customerId,
                        Id = zone.ZoneId,
                        Name = zone.ZoneName,
                    }).ToList();

            list.Sort();
            return list;
        }


        private int NextAreaId(int customerId)
        {
            int nextId = 1;

            Area area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == customerId);
            if (area != null)
            {
                nextId = PemsEntities.Areas.Where(m => m.CustomerID == customerId).Max(m => m.AreaID) + 1;
            }
            return nextId;
        }

        private int NextZoneId(int customerId)
        {
            int nextId = 1;

            Zone zone = PemsEntities.Zones.FirstOrDefault();
            if (zone != null)
            {
                nextId = PemsEntities.Zones.Max(m => m.ZoneId) + 1;
            }
            return nextId;
        }

        private int NextCustomGroup1Id(int customerId)
        {
            int nextId = 1;

            CustomGroup1 cg = PemsEntities.CustomGroup1.FirstOrDefault();
            if (cg != null)
            {
                nextId = PemsEntities.CustomGroup1.Max(m => m.CustomGroupId) + 1;
            }
            return nextId;
        }

        private int NextCustomGroup2Id(int customerId)
        {
            int nextId = 1;

            CustomGroup2 cg = PemsEntities.CustomGroup2.FirstOrDefault();
            if (cg != null)
            {
                nextId = PemsEntities.CustomGroup2.Max(m => m.CustomGroupId) + 1;
            }
            return nextId;
        }

        private int NextCustomGroup3Id(int customerId)
        {
            int nextId = 1;

            CustomGroup3 cg = PemsEntities.CustomGroup3.FirstOrDefault();
            if (cg != null)
            {
                nextId = PemsEntities.CustomGroup3.Max(m => m.CustomGroupId) + 1;
            }
            return nextId;
        }

        public void SetCustomerAreasModel(int customerId, CustomerAreasModel model)
        {
            // Save any new areas.
            if ( model.NewAreas != null )
            {
                foreach (var name in model.NewAreas)
                {
                    PemsEntities.Areas.Add( new Area()
                        {
                            CustomerID = customerId,
                            AreaID = NextAreaId( customerId ),
                            AreaName = name
                        } );
                    PemsEntities.SaveChanges();
                }
            }

            // Save any new zones.
            if ( model.NewZones != null )
            {
            foreach (var name in model.NewZones)
            {
                PemsEntities.Zones.Add(new Zone()
                {
                    customerID = customerId,
                    ZoneId = NextZoneId(customerId),
                    ZoneName = name
                });
                PemsEntities.SaveChanges();
            }
            }

            // Save any CustomGroup1s
            if (model.NewCustomGroup1s != null)
            {
            foreach (var name in model.NewCustomGroup1s)
            {
                PemsEntities.CustomGroup1.Add(new CustomGroup1()
                {
                    CustomerId = customerId,
                    CustomGroupId = NextCustomGroup1Id(customerId),
                    DisplayName = name,
                    Comment = "Added by PEMS"
                });
                PemsEntities.SaveChanges();
            }
            }

            // Save any CustomGroup2s
            if (model.NewCustomGroup2s != null)
            {
            foreach (var name in model.NewCustomGroup2s)
            {
                PemsEntities.CustomGroup2.Add(new CustomGroup2()
                {
                    CustomerId = customerId,
                    CustomGroupId = NextCustomGroup2Id(customerId),
                    DisplayName = name,
                    Comment = "Added by PEMS"
                });
                PemsEntities.SaveChanges();
            }
            }

            // Save any CustomGroup3s
            if (model.NewCustomGroup3s != null)
            {
            foreach (var name in model.NewCustomGroup3s)
            {
                PemsEntities.CustomGroup3.Add(new CustomGroup3()
                {
                    CustomerId = customerId,
                    CustomGroupId = NextCustomGroup3Id(customerId),
                    DisplayName = name,
                    Comment = "Added by PEMS"
                });
                PemsEntities.SaveChanges();
            }
            }

        }


        #endregion

        #region Pay By Cell

        /// <summary>
        /// Description:GetCustomerPayByCellModel will call GetVendors by passing the customerID to fill model with vendor details for the perticular customer
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>


        public CustomerPayByCellModel GetCustomerPayByCellModel(int customerId)
        {
            CustomerPayByCellModel model = new CustomerPayByCellModel()
            {
                CustomerId = customerId
            };

            GetCustomerBaseModel(model);

            model.CustPayByCell = GetVendors(customerId);

            //if (model.CustPayByCell.Count() > 0)
            //{

            //** Invoke another action method to enable checkboxes for the updated vendors

            //** Step 1 - Fetch the updated vendor IDs 
            model.selectedIds = GetselectedVendorsForCustomers(customerId);
            //List<SelectedIds> inst

            //}

            return model;
        }

        /// <summary>
        /// Description:GetVendors will fetch vendorID and VendorName from 'PayByCellVendors' table for the perticular customer passed to it as parameter and will return a list of the same.
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private List<CustPayByCell> GetVendors(int customerId)
        {
            var list = new List<CustPayByCell>();
            list = (from pbcv in PemsEntities.PayByCellVendors
                    where pbcv.DEPRECATE != false //pbcv.CustomerID == customerId &&
                    orderby pbcv.CustomerID
                    select new CustPayByCell()
                    {
                        VendorID = pbcv.VendorID,
                        VendorName = pbcv.VendorName,

                    }).ToList();
            return list;
        }

        /// <summary>
        /// Description:GetRipnetProperties will first convert the vendorID passed to as comma separated string into an array.
        /// Then for each vendor ID the query will search for relevant customer property for perticular vendor and customer return a list of it.
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>

        public List<RipnetProp> GetRipnetProperties(int customerId, string IDs)
        {
            List<string> VendorIDs = new List<string>();
            List<RipnetProp> Result = new List<RipnetProp>();
            IQueryable<RipnetProp> list = Enumerable.Empty<RipnetProp>().AsQueryable();

            //** First getting the Vendor IDs into an string array
            string[] VendorArr = IDs.Split(',');
            string tobesearched = "";
            int handle;

            for (var i = 0; i < VendorArr.Length; i++)
            {
                VendorIDs.Add("paybycell.cp." + customerId + "." + VendorArr[i] + ".");
                tobesearched = "paybycell.cp." + customerId + "." + VendorArr[i] + ".";



                list = (from RipProp in PemsEntities.RipnetProperties
                        where VendorIDs.Any(sl => RipProp.KeyText.StartsWith(tobesearched))
                        select new RipnetProp()
                        {

                            KeyText = RipProp.KeyText.Substring(RipProp.KeyText.IndexOf(tobesearched) + tobesearched.Length),
                            //KeyText = pbcv.KeyText.LastIndexOf(".",0),
                            ValueText = RipProp.ValueText,

                        });
                List<RipnetProp> dummy = list.ToList();

                for (var j = 0; j < dummy.Count(); j++)
                {

                    string keyTxt = dummy[j].KeyText;
                    string valTxt = dummy[j].ValueText;

                    RipnetProp inst = new RipnetProp();
                    inst.KeyText = keyTxt;
                    inst.ValueText = valTxt;

                    Result.Add(inst);

                }
            }

            return Result;
        }

        public bool SaveVendorCustomerMapping(int CustID, string IDs)
        {
            try
            {

                var pamclusters = (from sub in PemsEntities.vendorcustomerpaybycellmaps
                                   where sub.customerid == CustID
                                   select sub);

                foreach (vendorcustomerpaybycellmap pamcluster in pamclusters)
                {
                    PemsEntities.vendorcustomerpaybycellmaps.Remove(pamcluster);
                }
                PemsEntities.SaveChanges();

                string[] VendorArr = IDs.Split(',');
                for (var i = 0; i < VendorArr.Length; i++)
                {
                    if (VendorArr[i] != "")
                    {
                        vendorcustomerpaybycellmap vendorCustMap = new vendorcustomerpaybycellmap();
                        vendorCustMap.customerid = CustID;
                        vendorCustMap.vendorid = Convert.ToInt32(VendorArr[i]);
                        PemsEntities.vendorcustomerpaybycellmaps.Add(vendorCustMap);
                        PemsEntities.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<SelectedIds> GetselectedVendorsForCustomers(int customerId)
        {
            var list = new List<SelectedIds>();

            list = (from pbcv in PemsEntities.PayByCellVendors
                    join VCMapping in PemsEntities.vendorcustomerpaybycellmaps on pbcv.VendorID equals VCMapping.vendorid
                    where VCMapping.customerid == customerId && pbcv.DEPRECATE != false
                    orderby pbcv.CustomerID
                    select new SelectedIds()
                    {
                        VendorID = pbcv.VendorID,

                    }).ToList();
            return list;
        }


        #endregion

        #region PAM Configuration
        /// <summary>
        /// Description: This Method will returns the Customer details with PAM configuration details
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)     
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public CustomerPAMConfigurationModel GetPAMConfigurationModel(int customerId)
        {
            CustomerPAMConfigurationModel model = new CustomerPAMConfigurationModel()
            {
                CustomerId = customerId
            };

            // Get Custmer Details
            GetCustomerBaseModel(model);

            // Get details PAM Active
            model.PAMActiveCust = (from pamc in PemsEntities.PAMActiveCustomers
                                   where pamc.CustomerID == customerId
                                   select new PAMConfigurationModel()
                                   {
                                       CustomerID = pamc.CustomerID,
                                       CustomerIDPAM = pamc.CustomerID > 0 ? true : false,
                                       ResetImin = pamc.ResetImin,
                                       ExpTimeByPAM = pamc.ExpTimeByPAM

                                   }).FirstOrDefault();

            // If PAM INactive
            if (model.PAMActiveCust == null)
            {
                PAMConfigurationModel obj = new PAMConfigurationModel();
                obj.CustomerIDPAM = false;
                obj.ExpTimeByPAM = false;
                obj.ResetImin = false;
                model.PAMActiveCust = obj;
            }
            return model;
        }



        /// <summary>
        /// Description: This Method will Retrive PAM Clusters with details
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)        
        /// </summary>
        /// <param name="CustmerId"></param>
        /// <returns></returns>
        public List<PAMGracePeriodModel> GetPAMGrace(int CustmerId)
        {
            List<PAMGracePeriodModel> PAMGraceList = new List<Entities.Customers.PAMGracePeriodModel>();
            try
            {
                var PamGraceResult_raw = (from pama in PemsEntities.PAMGracePeriods
                                          where (pama.CustomerId == CustmerId)
                                          select new PAMGracePeriodModel
                                          {
                                              Clusterid = pama.ClusterId,
                                              GracePeriod = pama.GracePeriod
                                          });

                List<PAMGracePeriodModel> PAMGrace = PamGraceResult_raw.ToList();

                if (PAMGrace.Any())
                    PAMGraceList = PAMGrace;
            }
            catch (Exception ex)
            {

            }
            return PAMGraceList;
        }

        /// <summary>
        /// Description:This Method will SET PAM active / InActive
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)        
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="pamCustactive"></param>
        /// <param name="ChkExpTimeByPAM"></param>
        /// <param name="ChkResetImin"></param>
        public void SetPAMActive(int customerId, bool pamCustactive, bool ChkExpTimeByPAM, bool ChkResetImin)
        {
            // PAM Active
            if (pamCustactive == true)
            {
                var pam = (from s in PemsEntities.PAMActiveCustomers
                           where s.CustomerID == customerId
                           select s).FirstOrDefault();
                if (pam != null)
                {
                    // Active and Update other parameters
                    pam.ExpTimeByPAM = ChkExpTimeByPAM;
                    pam.ResetImin = ChkResetImin;
                    PemsEntities.SaveChanges();
                }
                else
                {
                    // Set Active with details
                    PAMActiveCustomer obj = new PAMActiveCustomer();
                    obj.CustomerID = customerId;
                    obj.ExpTimeByPAM = ChkExpTimeByPAM;
                    obj.ResetImin = ChkResetImin;
                    PemsEntities.PAMActiveCustomers.Add(obj);
                    PemsEntities.SaveChanges();
                }

            }
            else // PAM InActive
            {
                var pam = (from s in PemsEntities.PAMActiveCustomers
                           where s.CustomerID == customerId
                           select s).FirstOrDefault();
                if (pam != null)
                {
                    var pamclusters = (from sub in PemsEntities.PAMClusters
                                       where sub.Customerid == customerId
                                       select sub);

                    // Unassign all Meters assign to customer
                    foreach (PAMCluster pamcluster in pamclusters)
                    {
                        PemsEntities.PAMClusters.Remove(pamcluster);
                    }
                    PemsEntities.SaveChanges();

                    var PAMGracePeriods = (from sub in PemsEntities.PAMGracePeriods
                                           where sub.CustomerId == customerId
                                           select sub);

                    // Deleter all cluster created for customer
                    foreach (PAMGracePeriod PAMGracePeriod in PAMGracePeriods)
                    {
                        PemsEntities.PAMGracePeriods.Remove(PAMGracePeriod);
                    }
                    PemsEntities.SaveChanges();

                    PemsEntities.PAMActiveCustomers.Remove(pam);
                    PemsEntities.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Description: This Method will ADD /Update  ClusterID with GracePeriod
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        public void AddGracePeriod(PAMGracePeriodModel GridRowdata, int CustomerId)
        {
            var gp = (from s in PemsEntities.PAMGracePeriods
                      where s.CustomerId == GridRowdata.CustomerID && s.ClusterId == GridRowdata.Clusterid
                      select s).FirstOrDefault();
            if (gp != null)
            {
                //Update GracePeriod for ClusterId
                gp.GracePeriod = GridRowdata.GracePeriod;
                PemsEntities.SaveChanges();
            }
            else
            {
                //Add new ClusterID with Graceperiod
                PAMGracePeriod obj = new PAMGracePeriod();
                obj.CustomerId = CustomerId;
                obj.ClusterId = GridRowdata.Clusterid;
                obj.GracePeriod = GridRowdata.GracePeriod;
                PemsEntities.PAMGracePeriods.Add(obj);
                PemsEntities.SaveChanges();
            }
        }


        /// <summary>
        /// Description: This Method will delete existing Cluster
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// </summary>
        /// <param name="GridRowdata"></param>       
        public void DeleteGracePeriod(PAMGracePeriodModel GridRowdata)
        {
            var gp = (from s in PemsEntities.PAMGracePeriods
                      where s.CustomerId == GridRowdata.CustomerID && s.ClusterId == GridRowdata.Clusterid
                      select s).FirstOrDefault();
            if (gp != null)
            {
                var pamclusters = (from sub in PemsEntities.PAMClusters
                                   where sub.Customerid == GridRowdata.CustomerID && sub.Clusterid == GridRowdata.Clusterid
                                   select sub);

                // Unassign Meters from Cluster
                foreach (PAMCluster pamcluster in pamclusters)
                {
                    PemsEntities.PAMClusters.Remove(pamcluster);
                }
                PemsEntities.SaveChanges();

                PemsEntities.PAMGracePeriods.Remove(gp);
                PemsEntities.SaveChanges();
            }

        }

        /// <summary>
        /// Description: This Method will retrive all clusterID for particular Custermer
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public IQueryable<PAMGracePeriodModel> ClusterIDList(int CustomerId)
        {
            var PamCluster = (from pam in PemsEntities.PAMGracePeriods
                              where pam.CustomerId == CustomerId
                              select new PAMGracePeriodModel
                              {
                                  Clusterid = pam.ClusterId

                              });
            return PamCluster;
        }

        /// <summary>
        /// Description: This Method will retrive all MeterId List given to customer
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public IQueryable<PAMClusters> MeterIDList(int CustomerId)
        {
            var Meters = (from meter in PemsEntities.Meters
                          where meter.CustomerID == CustomerId && meter.AreaID == (int)AssetAreaId.Meter
                          select new PAMClusters
                          {
                              MeterId = meter.MeterId

                          });
            return Meters;
        }
        public IQueryable<PAMClusters> MeterIDListforSSM(int CustomerId)
        {
            var Meters = (from meter in PemsEntities.Meters
                          where meter.CustomerID == CustomerId && meter.AreaID == (int)AssetAreaId.Meter && meter.MeterGroup == (int)MeterGroups.SingleSpaceMeter
                          select new PAMClusters
                          {
                              MeterId = meter.MeterId

                          });
            return Meters;
        }

        /// <summary>
        /// Description: This Method will get all Meters are assigned to a cluster with a bay range
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// </summary>
        /// <param name="CustmerId"></param>
        /// <returns></returns>
        public List<PAMClusters> GetPAMClusterMeter(int CustomerId)
        {
            List<PAMClusters> PAMClusterList = new List<Entities.Customers.PAMClusters>();
            try
            {
                var PamResult_raw = (from pama in PemsEntities.PAMClusters

                                     where (pama.Customerid == CustomerId)

                                     select new PAMClusters
                                     {
                                         Clusterid = pama.Clusterid,
                                         Description = pama.Description,
                                         Hostedbayend = pama.Hostedbayend,
                                         Hostedbaystart = pama.Hostedbaystart,
                                         MeterId = pama.MeterId

                                     });

                List<PAMClusters> PAMCluster = PamResult_raw.ToList();

                if (PAMCluster.Any())
                    PAMClusterList = PAMCluster;
            }
            catch (Exception ex)
            {

            }
            return PAMClusterList;
        }

        /// <summary>
        /// Description:  This Method will Add / Update Meter assigned to a cluster
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014, Prita(16/Dec/2014))        
        /// </summary>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        public void AddPAMClusterMeter(PAMClusters GridRowdata, int CustomerId)
        {
            var gp = (from s in PemsEntities.PAMClusters
                      where s.Customerid == GridRowdata.Customerid && s.Clusterid == GridRowdata.MeterId && s.MeterId == GridRowdata.MeterId
                      //where s.Customerid == GridRowdata.Customerid && s.Clusterid == GridRowdata.Clusterid && s.MeterId == GridRowdata.MeterId(changed By Prita)
                      select s).FirstOrDefault();
            if (gp != null)
            {
                //  Update Meter details
                //gp.Hostedbaystart = GridRowdata.Hostedbaystart;
                //gp.Hostedbayend = GridRowdata.Hostedbayend;

                gp.Hostedbaystart = 1;
                gp.Hostedbayend = 1;
                gp.Description = GridRowdata.Description;
                PemsEntities.SaveChanges();
            }
            else
            {
                // Assign New Meter to cluster
                PAMCluster obj = new PAMCluster();
                obj.Customerid = CustomerId;
                //obj.Clusterid = GridRowdata.Clusterid;
                obj.Clusterid = GridRowdata.MeterId;
                obj.MeterId = GridRowdata.MeterId;
                //obj.Hostedbaystart = GridRowdata.Hostedbaystart;
                //obj.Hostedbayend = GridRowdata.Hostedbayend;
                obj.Hostedbaystart = 1;
                obj.Hostedbayend = 1;
                obj.Description = GridRowdata.Description;
                PemsEntities.PAMClusters.Add(obj);
                PemsEntities.SaveChanges();

            }
        }


        /// <summary>
        /// Description: This Method will Unassign Meter from cluster
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014,Prita(16/Dec/2014))         
        /// </summary>
        /// <param name="GridRowdata"></param>       
        public void DeletePAMClusterMeter(PAMClusters GridRowdata)
        {
            var gp = (from s in PemsEntities.PAMClusters
                      where s.Customerid == GridRowdata.Customerid && s.Clusterid == GridRowdata.MeterId && s.MeterId == GridRowdata.MeterId
                      //where s.Customerid == GridRowdata.Customerid && s.Clusterid == GridRowdata.Clusterid && s.MeterId == GridRowdata.MeterId(Changed by Prita)
                      select s).FirstOrDefault();
            if (gp != null)
            {
                PemsEntities.PAMClusters.Remove(gp);
                PemsEntities.SaveChanges();
            }
        }


        public int GetGroupID(int meterid, int CustID)
        {
            int GroupID = (from Mtrs in PemsEntities.Meters
                           join dz in PemsEntities.MeterGroups on Mtrs.MeterGroup equals dz.MeterGroupId
                           where (Mtrs.MeterId == meterid && Mtrs.CustomerID == CustID)
                           select dz.MeterGroupId).FirstOrDefault();
            return GroupID;
        }

        #endregion       
        
    }
}
