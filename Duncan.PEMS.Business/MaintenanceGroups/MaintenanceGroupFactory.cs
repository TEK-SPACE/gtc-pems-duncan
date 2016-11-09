using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.MaintenanceGroups;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;
using SettingsFactory = Duncan.PEMS.Business.Customers.SettingsFactory;

namespace Duncan.PEMS.Business.MaintenanceGroups
{
    public class MaintenanceGroupFactory : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const string MaintenanceGroupTemplateName = "rbac.menu.template.maint";
       /// <summary>
       /// Gets a list of customers that have the appropiate type associated with them (Customer)
       /// </summary>
       /// <returns></returns>
       public List<ListMaintenanceGroupModel> GetMaintenanceGroupsList()
       {
           var list = new List<ListMaintenanceGroupModel>();

           // Walk a list of customer profiles and return a list
           var query = from customers in RbacEntities.CustomerProfiles
                       select customers;

           foreach (var customer in query)
           {
               if (customer.CustomerTypeId == (int)CustomerProfileType.MaintenanceGroup)
               {
                   var customerModel = new ListMaintenanceGroupModel()
                   {
                       DisplayName = customer.DisplayName,
                       Id = customer.CustomerId,
                       Status = ((CustomerStatus)customer.Status).ToString(),
                       //PemsConnectionStringName = customer.PEMSConnectionStringName,
                       MaintenanceConnectionStringName = customer.MaintenanceConnectionStringName,
                      // ReportingConnectionStringName = customer.ReportingConnectionStringName,
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
                   customerModel.Name = customer.DisplayName;
                   list.Add(customerModel);
               }
           }
           return list;
       }

       public int CreateNewMaintananceGroup(MaintenangeGroupCreateModel model, string templateFolder, string workingFolder)
       {
           // In order to create a new maintenance group
           // 1.  Create customer in AuthorizationManager
           // 3.  Create the customer (customer profile) in the RBAC database.
           // 6.  Create menu and authorization entries in NetSqlAzMan
           // 7.  Add the current user to the list in the cache table

           int customerId = 0;

           var authorizationManager = new AuthorizationManager();
           if (authorizationManager.CreateCity(model.Id, model.DisplayName, "Internal Name: " + model.InternalName))
           {
               var customerProfile = new CustomerProfile()
               {
                   CustomerId = model.Id,
                   DisplayName = model.DisplayName,
                   CreatedOn = DateTime.Now,
                   CreatedBy = WebSecurity.CurrentUserId,
                   StatusChangeDate = DateTime.Now,
                   Is24HrFormat = false,
                   MaintenanceConnectionStringName = model.ConnectionStringName,
                   CustomerTypeId = (int)CustomerProfileType.MaintenanceGroup,
                   Status = (int)CustomerStatus.New
               };
               RbacEntities.CustomerProfiles.Add(customerProfile);
               RbacEntities.SaveChanges();
               customerId = model.Id;
           }


           if (customerId != 0)
           {
               // Set the menus into RBAC
               SetMenus(model.DisplayName, templateFolder, workingFolder, MaintenanceGroupTemplateName);

               //NOTE: We dont need to do this, no need to custom hidden columns for maintenanceGroups
              // RbacEntities.InitializeCustomerGrids(customerId);

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

       #region Maintenance Group Identification

       public MaintenanceGroupIdentificationModel GetIdentificationModel(int customerId)
       {
           var identificationModel = new MaintenanceGroupIdentificationModel();

           identificationModel.CustomerId = customerId;
           var settingFactory = new SettingsFactory();
           var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
           if (customerProfile != null)
           {

               GetCustomerBaseModel(customerId, identificationModel);
               identificationModel.DisplayName = customerProfile.DisplayName;
               identificationModel.DefaultPassword = settingFactory.GetValue("DefaultPassword", customerId);
           }

           // Get customer Address
           identificationModel.Contact =  GetCustomerContactModel(customerId);

           // Get customer localization data.
           identificationModel.Localization =  GetCustomerLocalizationModel(customerId);
           return identificationModel;
       }

       public MaintenanceGroupIdentificationModel GetIdentificationModel(int customerId, MaintenanceGroupIdentificationModel identificationModel)
       {
           // Save selected localization data indicies.
           int timeZoneId = identificationModel.Localization.TimeZoneId;
           int languageId = identificationModel.Localization.LanguageId;
           // Refresh customer localization data.
           identificationModel.Localization =  GetCustomerLocalizationModel(customerId);

           // Restore selected localization data indicies.
           identificationModel.Localization.TimeZoneId = timeZoneId;
           identificationModel.Localization.LanguageId = languageId;

           // Get customer Status
           identificationModel.Status =  GetCustomerStatusModel(customerId);
           return identificationModel;
       }

       public void SetIdentificationModel(int customerId, MaintenanceGroupIdentificationModel identificationModel)
       {

           var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);

           if (customerProfile == null )
               return;

           customerProfile.DisplayName = identificationModel.DisplayName;
        
           var settingFactory = new SettingsFactory();
           if (identificationModel.DefaultPassword != null)
               settingFactory.Set(customerId, "DefaultPassword", identificationModel.DefaultPassword);

           // Save customer contact
            SetCustomerContactModel(customerId, identificationModel.Contact);

           // Save Customer Localization data.
           SetCustomerLocalizationModel(customerId, identificationModel.Localization);

           RbacEntities.SaveChanges();
       }

       #endregion

       #region Customer Localization

       public CustomerLocalizationModel GetCustomerLocalizationModel(int customerId)
       {
           var customerLocalizationModel = new CustomerLocalizationModel();

           // TimeZone
           // Get presently assign zone id.
           var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
           customerLocalizationModel.TimeZoneId = customerProfile == null ? -1 : customerProfile.TimeZoneID ?? -1;

           // Get list of possible zones.

           customerLocalizationModel.TimeZone.Add(new SelectListItem()
           {
               Text = "[Select Time Zone]",
               Value = "-1",
               Selected = customerLocalizationModel.TimeZoneId == -1
           });

           if (customerLocalizationModel.TimeZoneId == -1)
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
                   if (customerLocalizationModel.TimeZoneId == timeZone.TimeZoneID)
                   {
                       customerLocalizationModel.TimeZoneDisplay = timeZone.TimeZoneName;
                   }
               }
           // Language
           customerLocalizationModel.LanguageDisplay = "";
           var settingFactory = new SettingsFactory();
           string locale = settingFactory.GetValue("CustomerLocale", customerId);

           customerLocalizationModel.Language = new List<SelectListItem>();
           List<Setting> settingList = settingFactory.GetList("Locale");
           foreach (Setting setting in settingList)
           {
               SelectListItem selectListItem = new SelectListItem()
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
           var customer = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
           if (customer != null)
               customerLocalizationModel.Is24Hr = customer.Is24HrFormat ?? false;


           return customerLocalizationModel;
       }


       public void SetCustomerLocalizationModel(int customerId, CustomerLocalizationModel customerLocalizationModel)
       {
           var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);

           // Language
           var settingFactory = new SettingsFactory();
           Setting setting = settingFactory.GetListValue("Locale", customerLocalizationModel.LanguageId);
           if (setting != null)
           {
               settingFactory.Set(customerId, "CustomerLocale", setting.Value);

               // Write the local to customer.DefaultLocale also
               customerProfile.DefaultLocale = setting.Value;
           }


           // 12/ 24 hour format
           customerProfile.Is24HrFormat = customerLocalizationModel.Is24Hr;

           // TimeZone
           customerProfile.TimeZoneID = customerLocalizationModel.TimeZoneId;
           // Save changes.
           RbacEntities.SaveChanges();
       }
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
           if (customerContactModel.FirstName != null)
               settingFactory.Set(customerId, "CustomerContactFirstName", customerContactModel.FirstName);

           if (customerContactModel.LastName != null)
               settingFactory.Set(customerId, "CustomerContactLastName", customerContactModel.LastName);

           if (customerContactModel.FromEmailAddress != null)
               settingFactory.Set(customerId, "CustomerContactEMail", customerContactModel.FromEmailAddress);

           if (customerContactModel.PhoneNumber != null)
               settingFactory.Set(customerId, "CustomerContactPhoneNumber", customerContactModel.PhoneNumber);
       }

       #endregion
       #endregion

       public void Activate(int customerId)
       {
           var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
           if (customerProfile != null)
           {
               customerProfile.Status = (int)CustomerStatus.Active;
               customerProfile.StatusChangeDate = DateTime.Now;
               RbacEntities.SaveChanges();
           }
       }

       public void Inactivate(int customerId)
       {
           var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
           if (customerProfile != null)
           {
               customerProfile.Status = (int)CustomerStatus.Inactive;
               customerProfile.StatusChangeDate = DateTime.Now;
               RbacEntities.SaveChanges();
           }
       }


       #region Customer Base

       public void GetCustomerBaseModel(int customerId, CustomerBaseModel customerBaseModel)
       {
           var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
           if (customerProfile != null)
           {
               customerBaseModel.CustomerId = customerId;
               customerBaseModel.DisplayName = customerProfile.DisplayName;
               customerBaseModel.Is24HrFormat = customerProfile.Is24HrFormat ?? false;

               // Get customer Status
               customerBaseModel.Status = GetCustomerStatusModel(customerId);
           }
       }
       public CustomerStatusModel GetCustomerStatusModel(int customerId)
       {
           var customerStatusModel = new CustomerStatusModel();
           var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
           if (customerProfile != null)
           {
               var userFactory = new UserFactory();
               customerStatusModel.CreatedBy = userFactory.GetUserById(customerProfile.CreatedBy ?? -1).FullName();
               customerStatusModel.CreatedOn = customerProfile.CreatedOn != null ? customerProfile.CreatedOn.Value.ToShortDateString() : "-";

               customerStatusModel.ModifiedBy = userFactory.GetUserById(customerProfile.ModifiedBy ?? -1).FullName();
               customerStatusModel.ModifiedOn = customerProfile.ModifiedOn != null ? customerProfile.ModifiedOn.Value.ToShortDateString() : "-";

               customerStatusModel.StatusChangeDate = customerProfile.StatusChangeDate.ToShortDateString();
               customerStatusModel.StatusDate = DateTime.Now.ToShortDateString();

               customerStatusModel.Status = ((CustomerStatus)customerProfile.Status).ToString();
               customerStatusModel.StatusId = (CustomerStatus)customerProfile.Status;


               customerStatusModel.StatusList = new List<SelectListItem>();
               foreach (CustomerStatus cs in Enum.GetValues(typeof(CustomerStatus)))
               {
                   var selectListItem = new SelectListItem()
                   {
                       Text = cs.ToString(),
                       Value = ((int)cs).ToString(),
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

       public MaintenanceGroupActivateModel CanActivate(int customerId)
       {
           var model = new MaintenanceGroupActivateModel();
           GetCustomerBaseModel(customerId, model);

           var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
           var settingFactory = new SettingsFactory();

           // Has a default password?
           string defaultPassword = settingFactory.GetValue("DefaultPassword", customerId);
           if (string.IsNullOrWhiteSpace(defaultPassword))
           {
               model.Issues.Add(new MaintenanceGroupActivateIssue()
               {
                   Description = "Default password is missing.",
                   Controller = "MaintenanceGroups",
                   Action = "EditMaintenanceGroup"
               });
           }

           // Has a time zone
           if (customerProfile.TimeZoneID == null || customerProfile.TimeZoneID == -1)
           {
               model.Issues.Add(new MaintenanceGroupActivateIssue()
               {
                   Description = "Select customer time zone.",
                   Controller = "MaintenanceGroups",
                   Action = "EditMaintenanceGroup"
               });
           }

           // Can activate?
           model.CanActivate = !model.Issues.Any();

           // Post a message that customer can be activated.
           if (model.CanActivate)
               model.ActivateMessage = model.DisplayName + " may be activated.";

           return model;
       }


       public MaintenanceGroupCustomersModel GetMaintenanceGroupCustomersModel(int maintGroupId)
       {
           var model = new MaintenanceGroupCustomersModel {CustomerId = maintGroupId};
           var mainGroup = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == maintGroupId);
           if (mainGroup != null)
           {
               //get assigned customers: the pems city based on the maintGroupId passed it, it will build all the customers

               var mainGroupCity = new PemsCity(maintGroupId.ToString());
               model.Customers = mainGroupCity.MaintenanceCustomers.Select(x => new MaintenanceGroupCustomerModel { Id = x.Id, DisplayName = x.DisplayName, NewCustomer = false }).ToList();

               //get possible customers: add the default item as well
               model.AvailableCustomers = new List<SelectListItem>
                   {
                       new SelectListItem {Text = ResourceTypes.DropDownDefault, Value = "-1"}
                   };

               //go grab all the customers
               model.AvailableCustomers.AddRange(GetMaintenanceCustomerListItems(maintGroupId));

               GetCustomerBaseModel(maintGroupId, model);
               model.Status = GetCustomerStatusModel(maintGroupId);
               model.DisplayName = mainGroup.DisplayName;
           }

           return model;
       }

        public List<SelectListItem> GetMaintenanceCustomerListItems(int maintGroupId)
        {
            //get all the customers in the system that are NOT in the maint groups table and are the correct customer type
            var selectList = new List<SelectListItem>();

            // Walk a list of customer profiles and return a list
            var query = from customers in RbacEntities.CustomerProfiles where customers.CustomerId > 0 orderby customers.DisplayName select customers;

            //for each one
            foreach (var customer in query)
            {
                //check to make sure they are the correct type
                if (customer.CustomerTypeId == (int)CustomerProfileType.Customer)
                {
                    //last thing we have to do is check to make sure that the customer exists in azman. if it doesnt, we have problems.
                    var azManStore = RbacEntities.netsqlazman_StoresTable.FirstOrDefault(store => store.StoreId == customer.CustomerId);
                    if (azManStore == null)
                        continue;
                    //now check to see if they are NOT in the maingroup table. if they are, they cant be assigned to another maint group
                    var existing = RbacEntities.MaintenanceGroupCustomers.FirstOrDefault(x => x.CustomerId == customer.CustomerId);
                    if (existing == null)
                        selectList.Add(new SelectListItem { Text = customer.DisplayName, Value = customer.CustomerId.ToString() });
                }
            }
            return selectList;
        }
        

       public void SetMaintenanceGroupCustomersModel(int maintGroupId, MaintenanceGroupCustomersModel model)
       {
           // Save any new areas.
           if (model.NewCustomers != null)
           {
               var maintGroup = new PemsCity(maintGroupId.ToString());

               foreach (var newCustomerId in model.NewCustomers)
               {

                   //try to parse the id to make sure it came though correctly
                   int newCustID;
                   var parsed = int.TryParse(newCustomerId, out newCustID);

                   if (parsed)
                   {
                       //now lets check to see if this customer is in the system
                       var existingCustomer = RbacEntities.CustomerProfiles.FirstOrDefault(x => x.CustomerId == newCustID);
                       if (existingCustomer != null)
                       {
                           //only do it if it doesnt exist there already
                           var existing =RbacEntities.MaintenanceGroupCustomers.FirstOrDefault(x => x.MaintenanceGroupId == maintGroupId && x.CustomerId == newCustID);
                           if (existing != null) continue;
                           RbacEntities.MaintenanceGroupCustomers.Add(new MaintenanceGroupCustomer
                               {
                                   MaintenanceGroupId = maintGroupId,
                                   CustomerId = newCustID
                               });
                           RbacEntities.SaveChanges();

                           //roll thorugh all the ussers for the maint group and if htey are a technician, add them to the _main group for the customer. 
                           //This way they will not be member, but will be part of a role so they have access log in to the maint for that cuity

                           //we do not need to roll thorugh the users for the customers, since they are not allowed to assign technicians until they are part of the maintenance group
                           //this means that the customer will not have any technicians, so we do not need to worry about adding those users as techs for the maint group, 
                           //that will be done when the user is checked as a tech after the customer is assigned to the maint group.
                           //now add all of the users for this maintenance group that are technicians to the _maintenance role for the customer.
                        
                           //we are also only doing this for new customers. 

                           //get all the users for the maintenance group
                            var mainGroupMembersUsernames = (new SecurityManager()).GetUsersForCity(maintGroup.InternalName);
                         
                           //get the customer and an auth manager for that customer
                           var customer = new PemsCity(existingCustomer.CustomerId.ToString());
                             var authorizationManager = new AuthorizationManager(customer);
                          
                           //check to see if they are a technician
                           foreach (var mainGroupMembersUsername in mainGroupMembersUsernames)
                               if (TechnicianFactory.IsUserTechnician(mainGroupMembersUsername))
                               {
                                   //if they are, add them to the _maint role for the customer
                                   authorizationManager.AddGroupMember(Constants.Security.DefaultMaintenanceGroupName,
                                                                       mainGroupMembersUsername);
                                   
                                   //go get the userid from the name
                                   var userId = (new UserFactory()).GetUserId(mainGroupMembersUsername);

                                   //we also need to update the caching table to give the technician access to see the site
                                   (new UserCustomerAccessManager()).AddCustomerAccess(userId, maintGroupId);
                               }
                       }
                   }
               }
           }
       }

        public static bool IsCustomerMaintenanceGroup(string cityname)
        {
            //we are using a new rbac entities here since this has to be a static method
            using (var rbacEntities = new PEMRBACEntities())
            {
                var existingCustomer = rbacEntities.CustomerProfiles.FirstOrDefault(x => x.DisplayName == cityname);
                if (existingCustomer != null)
                    return existingCustomer.CustomerTypeId == (int) CustomerProfileType.MaintenanceGroup;
            }
            return false;
        }
    }
}
