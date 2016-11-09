using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Utilities;
using NLog;


namespace Duncan.PEMS.Business.Customers
{
    /// <summary>
    /// This class is used for operational settings of a city.  These settings are applied to a city and are not
    /// ultimately displayed to a city user.  The effects of these settings may impact what a particular 
    /// city user sees but the user is not expected to select these.  These settings may also affect how a city operates for 
    /// the users of the individual city.
    /// 
    /// These settings are used only at the creation of a city and any subsequent administrative updates.
    /// </summary>
    public class SettingsFactory : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        public string GetCustomerConnectionStringName(int customerId)
        {
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            return customerProfile == null ||  string.IsNullOrWhiteSpace( customerProfile.PEMSConnectionStringName ) ?
                ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName] : customerProfile.PEMSConnectionStringName;
        }

        public string GetMaintenanceGroupConnectionStringName(int customerId)
        {
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            return customerProfile == null || string.IsNullOrWhiteSpace(customerProfile.MaintenanceConnectionStringName) ?
                ConfigurationManager.AppSettings[Constants.Security.DefaultMaintConnectionStringName] : customerProfile.MaintenanceConnectionStringName;
        }

        public string GetReportingConnectionStringName(int customerId)
        {
            var customerProfile = RbacEntities.CustomerProfiles.SingleOrDefault(m => m.CustomerId == customerId);
            return customerProfile == null || string.IsNullOrWhiteSpace(customerProfile.ReportingConnectionStringName) ?
                ConfigurationManager.AppSettings[Constants.Security.DefaultReportingConnectionStringName] : customerProfile.ReportingConnectionStringName;
        }

        #region Localization


        /// <summary>
        /// Gets the local time for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public DateTime GetCustomerLocalTime(int customerId)
        {
            // Get the basic UTC offset of server.
            var utcOffset = (int)System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours;
            var rbacEntities = new PEMRBACEntities();
            var customerProfile = rbacEntities.CustomerProfiles.SingleOrDefault(cp => cp.CustomerId == customerId);
            if (customerProfile != null)
            {
                var timeZone = rbacEntities.CustomerTimeZones.FirstOrDefault(m => m.TimeZoneID == customerProfile.TimeZoneID);
                // LocalTimeUTCDifference is in minutes in the [TimeZones] table.  Convert it to hours.
                if (timeZone != null)
                    // If timeZone.DaylightSavingAdjustment != 0 add 1 hour to UTCOffset to handle Daylight Saving Time
                    utcOffset = timeZone.LocalTimeUTCDifference / 60 + (timeZone.DaylightSavingAdjustment != 0 ? 1 : 0);
            }
            var localTime = DateTime.UtcNow + new TimeSpan(0, utcOffset, 0, 0);
            return localTime;
        }

        #endregion



        /// <summary>
        /// Gets a list of items that represent valid PEMS connection string names in the connection.config file
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetCustomerConnectionStringNames()
        {
            var items = new List<SelectListItem>();
            string cnxNameTemplate = ConfigurationManager.AppSettings[Constants.Security.PemsConnectionStringNameTemplate];
            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
            {
                // Only add items that start with the naming convention
                if (connString.Name.StartsWith(cnxNameTemplate))
                {
                    items.Add(new SelectListItem
                    {
                            Text   = connString.Name,
                            Value = connString.Name,
                        });
                }
            }
            //gets a list of conneciton string names from the connectionstrings.config file.
            return items;
        }

        /// <summary>
        /// Gets a lsit of items that represent valid PEMS connection string names in the connection.config file
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetReportingConnectionStringNames()
        {
            var items = new List<SelectListItem>();
            string cnxNameTemplate = ConfigurationManager.AppSettings[Constants.Security.ReportingConnectionStringNameTemplate];
            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
            {
                // Only add items that start with the naming convention
                if (connString.Name.StartsWith(cnxNameTemplate))
                {
                    items.Add(new SelectListItem
                    {
                        Text = connString.Name,
                        Value = connString.Name,
                    });
                }
            }
            //gets a list of conneciton string names from the connectionstrings.config file.
            return items;
        }

        /// <summary>
        /// Gets a list of items that represent valid connection string names int eh connection.config file
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetMaintenanceGroupConnectionStringNames()
        {
            var items = new List<SelectListItem>();
            string cnxNameTemplate = ConfigurationManager.AppSettings[Constants.Security.MaintConnectionStringNameTemplate];
            foreach (ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
            {
                // Only add items that start with the naming convention
                if (connString.Name.StartsWith(cnxNameTemplate))
                {
                    items.Add(new SelectListItem
                    {
                        Text = connString.Name,
                        Value = connString.Name,
                    });
                }
            }
            //gets a list of conneciton string names from the connectionstrings.config file.
            return items;
        }

        public enum SettingType
        {
            Single = 0,
            OneOfList = 1,
            InternalLabelSelectList = 11,
            FieldLabelSelectList = 10,
            GridLabelSelectList = 12
        }


        /// <summary>
        /// Gets a list of the present setting for a customer based on setting name.
        /// </summary>
        /// <param name="settingName">String name of setting</param>
        /// <param name="customerId">Id of customer</param>
        /// <returns>List of <see cref="Setting"/> representing value or empty list.</returns>
        public  List<Setting> Get(string settingName, int customerId)
        {
            var settingsList = new List<Setting>();

            var settingType = RbacEntities.CustomerSettingTypes.FirstOrDefault(x => x.CustomerSettingTypeName == settingName);
            if ( settingType != null )
            {
                int sortOrder = 0;
                foreach (var customerSetting in RbacEntities.CustomerSettings.Where( customerSetting => customerSetting.CustomerId == customerId 
                    && customerSetting.CustomerSettingTypeId == settingType.CustomerSettingTypeId ))
                {
                    // Add to list
                    settingsList.Add(new Setting()
                        {
                            Id = customerSetting.CustomerSettingsId,
                            Value = customerSetting.SettingValue,
                            Default = false,
                            SortOrder = sortOrder++
                        });
                }
            }
            return settingsList;
        }

        /// <summary>
        /// Get the setting value of a setting name for a customer.  This returns only the value.
        /// </summary>
        /// <param name="settingName">String name of setting</param>
        /// <param name="customerId">Id of customer</param>
        /// <returns>The setting value or null string if value does not exist.</returns>
        public  string GetValue(string settingName, int customerId)
        {
            List<Setting> settingsList = Get( settingName, customerId );
            return settingsList.Any() ? settingsList[0].Value : null;
        }


        /// <summary>
        /// Gets a list of allowable values for a setting.
        /// </summary>
        /// <param name="settingName">String name of setting</param>
        /// <param name="settingType">Type of setting.  <see cref="SettingsFactory.SettingType"/></param>
        /// <returns>Ordered list of allowable settings.  Empty list indicates no known settings.</returns>
        public  List<Setting> GetList(string settingName, SettingType settingType = SettingType.OneOfList)
        {
            var settingsList = new List<Setting>();

            var settingTypeVal = RbacEntities.CustomerSettingTypes.FirstOrDefault(x => x.CustomerSettingTypeName == settingName && x.SettingTypeId == (int)settingType);
            if (settingTypeVal != null)
            {

                foreach (var listSetting in RbacEntities.CustomerSettingTypeLists.Where(setting => setting.CustomerSettingTypeId == settingTypeVal.CustomerSettingTypeId).OrderBy(setting => setting.SortOrder))
                {
                    // Add to list
                    settingsList.Add(new Setting()
                    {
                        Id = listSetting.CustomerSettingTypeListId,
                        Value = listSetting.SettingValue,
                        Default = listSetting.Default,
                        SortOrder = listSetting.SortOrder
                    });
                }
            }

            settingsList.Sort();

            return settingsList;
        }


        /// <summary>
        /// Gets value of a list item.
        /// </summary>
        /// <param name="settingName">String name of setting</param>
        /// <param name="settingId">Integer id of the list item.</param>
        /// <returns>Returns a <see cref="Setting"/> instance if found.  Otherwise returns null.</returns>
        public  Setting GetListValue(string settingName, int settingId)
        {
            Setting setting = null;

            var settingType = RbacEntities.CustomerSettingTypes.FirstOrDefault(x => x.CustomerSettingTypeName == settingName);
            if (settingType != null)
            {
                var settingValue = RbacEntities.CustomerSettingTypeLists.FirstOrDefault(x => x.CustomerSettingTypeId == settingType.CustomerSettingTypeId
                    && x.CustomerSettingTypeListId == settingId);

                if(settingValue != null)
                {
                    setting = new Setting()
                    {
                        Id = settingValue.CustomerSettingTypeListId,
                        Value = settingValue.SettingValue,
                        Default = settingValue.Default,
                        SortOrder = settingValue.SortOrder
                    };
                }
            }


            return setting;
        }


        /// <summary>
        /// Gets value of a list item.
        /// </summary>
        /// <param name="settingId">Integer id of the list item.</param>
        /// <returns>Returns a <see cref="Setting"/> instance if found.  Otherwise returns null.</returns>
        public  Setting GetListValue(int settingId)
        {
            Setting setting = null;

                var settingValue = RbacEntities.CustomerSettingTypeLists.FirstOrDefault(x => x.CustomerSettingTypeListId == settingId);

                if (settingValue != null)
                {
                    setting = new Setting()
                    {
                        Id = settingValue.CustomerSettingTypeListId,
                        Value = settingValue.SettingValue,
                        Default = settingValue.Default,
                        SortOrder = settingValue.SortOrder
                    };
                }


            return setting;
        }

        /// <summary>
        /// Sets a customer setting
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="settingName"></param>
        /// <param name="settingValue"></param>
        public  void Set(int customerId, string settingName, string settingValue)
        {
            if (settingValue == null)
                settingValue = string.Empty;
            var settingType = RbacEntities.CustomerSettingTypes.FirstOrDefault(x => x.CustomerSettingTypeName == settingName);
            if ( settingType == null )
            {
                settingType = new CustomerSettingType()
                    {
                        CustomerSettingTypeName = settingName,
                        CustomerSettingTypeId = 0,
                        IsRequired = false
                    };
                RbacEntities.CustomerSettingTypes.Add( settingType );
                RbacEntities.SaveChanges();
            }

            // Does this customer already have one of these settings?
            var setting = RbacEntities.CustomerSettings.FirstOrDefault( customerSetting => customerSetting.CustomerId == customerId
                    && customerSetting.CustomerSettingTypeId == settingType.CustomerSettingTypeId );

            if ( setting == null )
            {
                // Create new customer setting entry
                setting = new CustomerSetting()
                    {
                        CustomerId = customerId,
                        CustomerSettingTypeId = settingType.CustomerSettingTypeId,
                        SettingValue = settingValue
                    };
                RbacEntities.CustomerSettings.Add( setting );
            }
            else
            {
                setting.SettingValue = settingValue;
            }
            RbacEntities.SaveChanges();
            

        }
    }
}
