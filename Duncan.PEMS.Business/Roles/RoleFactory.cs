using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Business.Utility.Audit;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Roles;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Roles
{
    public class RoleFactory : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Gets a list of roles for a specific customer. filters by role if neccessary
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public  List<ListRoleModel> GetRoleListModels(string roleId, string customerId)
        {
            //do not include the default role groups (in constants class)

            IQueryable<ListRoleModel> items = RbacEntities.netsqlazman_StoreGroupsTable.Select(
               x =>
               new ListRoleModel
               {
                   CustomerId = x.StoreId,
                   CustomerName = x.netsqlazman_StoresTable.Name,
                   CustomerInternalName = x.netsqlazman_StoresTable.Name,
                   RoleName = x.Name,
                   RoleId = x.StoreGroupId
               }).Where(x => !x.RoleName.StartsWith(Constants.Security.DefaultGroupNamePrefix));
            
            //get a list of clients for htis user
            var secMgr = new SecurityManager();
            var clients = secMgr.GetCitiesForUser(WebSecurity.CurrentUserName);

            //filter clients down if needed
            if (customerId != "All")
            {
                int custID;
                int.TryParse(customerId, out custID);
                if (custID > 0)
                    clients = clients.Where(x => x.Id == custID).ToList();
            }
            //then filter the list of items by access for this user
            var clientIDs = clients.Select(x => x.Id);
            items = items.Where(item => clientIDs.Contains(item.CustomerId));

            //finally, filter by rolename
            if (roleId != "All")
            {
                int roleIDInt;
                int.TryParse(roleId, out roleIDInt);
                if (roleIDInt > 0)
                    items = items.Where(x => x.RoleId == roleIDInt);
            }

            //update display names
            var listItems = items.ToList();
            listItems.ForEach(r =>
                {
                    var firstOrDefault = clients.FirstOrDefault(x => x.Id == r.CustomerId);
                    if (firstOrDefault != null)
                                           r.CustomerName = firstOrDefault.DisplayName;
                });

            //also update last modified and mod by
            listItems.ForEach(r => UpdateLastModified(r));

            return listItems;
        }

        /// <summary>
        /// Updates the last modified information for a role
        /// </summary>
        private  ListRoleModel UpdateLastModified(ListRoleModel roleModel)
        {
            var auditRecord = (new AuditFactory()).Get(Constants.Audits.RoleTableName, roleModel.RoleId);
            roleModel.LastModifiedOn = auditRecord.ModifiedOn;
            roleModel.LastModifiedBy = (new UserFactory()).GetUserById(auditRecord.ModifiedBy).FullName();
            return roleModel;
        }

        /// <summary>
        /// Gets a list of SelectListItems of roles for the customer ID passed in.
        /// </summary>
        public List<SelectListItem> GetRoleListItems(string customerId)
        {
            var items = new List<SelectListItem>();
            var secMgr = new SecurityManager();
            var city = new PemsCity(customerId);
            var clientRoles = secMgr.GetGroups(city, false);
            items.AddRange(clientRoles.Select(clientRole => new SelectListItem
                {
                    Text = clientRole.Key,
                    Value = clientRole.Value.ToString()
                }));
            return items;
        }

        /// <summary>
        /// Returns a bool indicating whether a role exists or not
        /// </summary>
        public  bool DoesRoleExist(string roleName, string customerInternalName)
        {
            try
            {
                var authMan = new AuthorizationManager(customerInternalName);
               return  authMan.DoesGroupExist(roleName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a check box list of groups for a user and city. Used for the user models
        /// </summary>
        public  List<CheckBoxItem> GetGroups(string username, PemsCity currentCity)
        {
            var _secMgr = new SecurityManager();
            var cityGroups = _secMgr.GetGroupsForUser(currentCity, username);
            //Add data in SelectList List
            return cityGroups.Select(item => new CheckBoxItem {Text = item.Key, Selected = item.Value, Value = item.Key}).ToList();
        }

        /// <summary>
        /// Gets a check box list of groups for a city. Used for the user models
        /// </summary>
        public  List<CheckBoxItem> GetGroups(PemsCity currentCity)
        {
            var secMgr = new SecurityManager();
            var cityGroups = secMgr.GetGroups(currentCity);
            var checkBoxList = new List<CheckBoxItem>();
            //Add data in SelectList List
            foreach (var item in cityGroups)
            {
                var chk = new CheckBoxItem { Text = item.Key, Selected = false, Value = item.Key };
                checkBoxList.Add(chk);
            }
            return checkBoxList;
        }
    }
}
