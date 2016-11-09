using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Business.Users
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

        /// <summary>
        /// Gets a list of the present setting for a city based on setting name.
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <returns>List of <see cref="Setting"/> representing value or empty list.</returns>
        public  List<Setting> GetList(int userId)
        {
            var settingsList = new List<Setting>();

            var settingType = RbacEntities.UserSettings.FirstOrDefault(x => x.UserId == userId);
            if (settingType != null)
            {
                settingsList.AddRange(RbacEntities.UserSettings.Select(userSetting => new Setting()
                    {
                        Id = userSetting.UserSettingsId,
                        Name = userSetting.SettingName,
                        Value = userSetting.SettingValue,
                    }));
            }
            return settingsList;
        }

        /// <summary>
        /// Get the setting value of a setting name for a user.  This returns only the value.
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="settingName">String name of setting</param>
        /// <returns>The setting value or null if value does not exist.</returns>
        public  Setting Get(int userId, string settingName)
        {
            var settingsList =
                RbacEntities.UserSettings.FirstOrDefault(x => x.UserId == userId && x.SettingName == settingName);

            return settingsList == null
                       ? null
                       : new Setting()
                           {
                               Id = settingsList.UserSettingsId,
                               Name = settingsList.SettingName,
                               Value = settingsList.SettingValue,
                           };
        }


        /// <summary>
        /// Get the setting value of a setting name for a user.  This returns only the value.
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="settingName">String name of setting</param>
        /// <returns>The setting value or null if value does not exist.</returns>
        public  string GetValue(int userId, string settingName)
        {
            var settingsList =RbacEntities.UserSettings.FirstOrDefault(x => x.UserId == userId && x.SettingName == settingName);
            return settingsList == null? null: settingsList.SettingValue;
        }


        /// <summary>
        /// Update a single user setting
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="settingName">Name of setting</param>
        /// <param name="settingValue">Value of setting</param>
        public  void Set(int userId, string settingName, string settingValue)
        {
            //we may want to set it to empty string, since we might want to set it to empty sometimes
            if (settingValue == null)
                settingValue = string.Empty;
                // Does this customer already have one of these settings?
                var setting = RbacEntities.UserSettings.FirstOrDefault(userSetting => userSetting.UserId == userId
                                                                                      &&
                                                                                      userSetting.SettingName ==
                                                                                      settingName);

                if (setting == null)
                {
                    // Create new customer setting entry
                    setting = new UserSetting()
                        {
                            UserId = userId,
                            SettingName = settingName,
                            SettingValue = settingValue
                        };
                    RbacEntities.UserSettings.Add(setting);
                }
                else
                {
                    setting.SettingValue = settingValue;
                }
                RbacEntities.SaveChanges();
            }


        /// <summary>
        /// Update user settings via a list of <see cref="Setting"/>
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="settings">List of <see cref="Setting"/></param>
        public  void Set(int userId, List<Setting> settings)
        {
            foreach (var newSetting in settings)
            {
                if (!string.IsNullOrEmpty(newSetting.Value))
                {
                    // Does this customer already have one of these settings?
                    var setting = RbacEntities.UserSettings.FirstOrDefault(userSetting => userSetting.UserId == userId
                                                                                          &&
                                                                                          userSetting.SettingName ==
                                                                                          newSetting.Name);

                    if (setting == null)
                    {
                        // Create new customer setting entry
                        setting = new UserSetting()
                            {
                                UserId = userId,
                                SettingName = newSetting.Name,
                                SettingValue = newSetting.Value
                            };
                        RbacEntities.UserSettings.Add(setting);
                    }
                    else
                    {
                        setting.SettingValue = newSetting.Value;
                    }
                }
            }
            RbacEntities.SaveChanges(); 
        }

        /// <summary>
        /// Gets a list of user ids that are marked as technicians int he system.
        /// </summary>
        /// <returns></returns>
        public List<UserProfile> GetAllTechnicianProfiles()
        {
            var items = RbacEntities.UserSettings.Where(x => x.SettingName == Constants.User.IsTechnician && x.SettingValue == "True").Select(y=>y.UserProfile).ToList();
            return items;
        }
    }
}
