using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;


/*
// References to 3rd party NetSqlAzMan.dll assembly (No longer needed because we gather our info directly from DB, which is faster than the 3rd party library!)
using NetSqlAzMan;
using NetSqlAzMan.Interfaces;
using NetSqlAzMan.Cache;
*/

using RBACToolbox;

namespace RBACProvider
{

    public class RBACRoleProvider : RoleProvider
    {
        public AppRBACInterface _RBACInterface = null;
        
        // For thread-safety, use _CacheLocker when accessing the cache collections
        private ReaderWriterLockSlim _CacheLocker = new ReaderWriterLockSlim();
        private Dictionary<string, List<string>> _cachedRolesForUsernames = new Dictionary<string, List<string>>();
        private List<RBACUserInfo> _cachedRBACUsers = new List<RBACUserInfo>();
        
        public RBACRoleProvider()
        {
        }
        
        public RBACRoleProvider(AppRBACInterface rbacInterface)
        {
            _RBACInterface = rbacInterface;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> results = new List<string>();

            RBACUserInfo rbacUser = GetRBACUserFromCacheOrDB(username);
            if (rbacUser == null)
                return results.ToArray();

            List<RBACItemInfo> grantedItems = _RBACInterface.GetGrantedItemsForUser(rbacUser, false);
            foreach (RBACItemInfo nextItem in grantedItems)
            {
                results.Add(nextItem.ItemName);
            }

            return results.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public RBACUserInfo GetRBACUserFromCacheOrDB(string username)
        {
            RBACUserInfo result = null;

            try
            {
                // Use a read lock for this operation
                _CacheLocker.EnterReadLock();

                // First we will look for a match in our cached users
                foreach (RBACUserInfo cachedUser in _cachedRBACUsers)
                {
                    if (string.Compare(username, cachedUser.UserName, true) == 0)
                    {
                        result = cachedUser;
                        break;
                    }
                }
            }
            finally
            {
                // Release the read lock           
                if (_CacheLocker.IsReadLockHeld)
                    _CacheLocker.ExitReadLock();
            }

            // If we already found a match, return it
            if (result != null)
                return result;

            // Ok, this user wasn't cached, so lets see if we can obtain it from the actual RBAC database,
            // then add it to our cache
            result = _RBACInterface.GetUser(username);
            if (result != null)
            {
                try
                {
                    // Use an exclusive lock when modifying the collection
                    _CacheLocker.EnterWriteLock();

                    _cachedRBACUsers.Add(result);
                }
                finally
                {
                    // Release the exclusive write lock           
                    if (_CacheLocker.IsWriteLockHeld)
                        _CacheLocker.ExitWriteLock();
                }
            }
            return result;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            // First we will look at cached stuff.  If not found there, then we will search the DB (and update the cache)
            bool matchedFromCache = false;

            try
            {
                // Use a read lock for this operation
                _CacheLocker.EnterReadLock();

                if (_cachedRolesForUsernames.ContainsKey(username))
                {
                    List<string> cachedRolesForUsername = _cachedRolesForUsernames[username];
                    if ((cachedRolesForUsername != null) && (cachedRolesForUsername.Count > 0))
                    {
                        if (cachedRolesForUsername.IndexOf(roleName) != -1)
                            matchedFromCache = true;
                    }
                }
            }
            finally
            {
                // Release the read lock           
                if (_CacheLocker.IsReadLockHeld)
                    _CacheLocker.ExitReadLock();
            }

            if (matchedFromCache == true)
            {
                return true;
            }
            else
            {
                // Get the user
                RBACUserInfo rbacUser = GetRBACUserFromCacheOrDB(username);
                if (rbacUser == null)
                    return false;

                // Not found in the cache yet, but let's see if access can be assumed from the "PEM" default CustomerID assigned to the user
                string defaultCustomerAccess = "Customer:" + rbacUser.PEMDefaultCustomerID.ToString();
                if (string.Compare(defaultCustomerAccess, roleName, true) == 0)
                {
                    try
                    {
                        // Use an exclusive lock when modifying the collection
                        _CacheLocker.EnterWriteLock();

                        // Is a match, so lets update the local cache
                        if (_cachedRolesForUsernames.ContainsKey(username) == false)
                            _cachedRolesForUsernames.Add(username, new List<string>());

                        if (_cachedRolesForUsernames[username].IndexOf(defaultCustomerAccess) != -1)
                            _cachedRolesForUsernames[username].Add(defaultCustomerAccess);

                        return true;
                    }
                    finally
                    {
                        // Release the exclusive write lock           
                        if (_CacheLocker.IsWriteLockHeld)
                            _CacheLocker.ExitWriteLock();
                    }
                }

                // We will get all granted items for the user, add them to the cache as needed, and also
                // check to see if any of them are the ones we are explicitly looking for
                bool grantedItemFound = false;
                List<RBACItemInfo> grantedItems = _RBACInterface.GetGrantedItemsForUser(rbacUser, false);
                try
                {
                    // Use an exclusive lock when modifying the collection
                    _CacheLocker.EnterWriteLock();

                    foreach (RBACItemInfo nextItem in grantedItems)
                    {
                        // Update the local cache with this item
                        if (_cachedRolesForUsernames.ContainsKey(username) == false)
                            _cachedRolesForUsernames.Add(username, new List<string>());

                        // If the granted item isn't in the user's cache yet, add it now
                        if (_cachedRolesForUsernames[username].IndexOf(nextItem.ItemName) != -1)
                            _cachedRolesForUsernames[username].Add(nextItem.ItemName);

                        // If it matches the item we are looking for, then set a flag (but don't break out of loop)
                        if (string.Compare(nextItem.ItemName, roleName, true) == 0)
                            grantedItemFound = true;
                    }
                }
                finally
                {
                    // Release the exclusive write lock           
                    if (_CacheLocker.IsWriteLockHeld)
                        _CacheLocker.ExitWriteLock();
                }

                // If granted item was found (and cache was updated), then return true
                if (grantedItemFound == true)
                    return true;
            }

            // If we get this far, then user doesn't have access to requested role
            return false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }

    public class AppRBACInterface
    {
        public RBACRoleProvider RoleProvider = null;

        internal DBAccess _DAL_RBAC = null;

        private string _ApplicationName = string.Empty;
        private string _ApplicationDescription = string.Empty;
        private int _ApplicationID = -1;

        public bool ApplicationExists
        {
            get { return _ApplicationID >= 0; }
        }
        public int ApplicationID
        {
            get { return _ApplicationID; }
        }

        private string _StoreName = string.Empty;
        private string _StoreDescription = string.Empty;
        private int _StoreID = -1;

        public bool StoreExists
        {
            get { return _StoreID >= 0; }
        }
        public int StoreID
        {
            get { return _StoreID; }
        }

        public AppRBACInterface(DBAccess dbAccessLayer, string targetApplicationName, string targetApplicationDescription, string targetStoreName, string targetStoreDescription)
        {
            _DAL_RBAC = dbAccessLayer;

            // Init Application variables
            _ApplicationName = targetApplicationName;
            _ApplicationDescription = targetApplicationDescription;
            _ApplicationID = -1;

            // Init Store variables
            _StoreName = targetStoreName;
            _StoreDescription = targetStoreDescription;
            _StoreID = -1;

            // Create our role provider, with reference to ourself
            RoleProvider = new RBACRoleProvider(this);
        }

        public void DiscoverRBACStoreAndAppIDs()
        {
            // Start by querying for our desired Store name
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@StoreName", this._StoreName);
            DataSet dsRBACStores = _DAL_RBAC.GetDatasetFromSQL(
                "select * from netsqlazman_storestable where Name = @StoreName", sqlParams, true);
            dsRBACStores.Tables[0].TableName = "Stores";
            // Extract the StoreId if we found it
            if (dsRBACStores.Tables["Stores"].Rows.Count > 0)
                _StoreID = Convert.ToInt32(dsRBACStores.Tables["Stores"].Rows[0]["StoreId"]);
            else
                _StoreID = -1;
            dsRBACStores.Dispose();

            // Next query for desired Application name inside the Store
            sqlParams.Clear();
            sqlParams.Add("@StoreId", _StoreID);
            sqlParams.Add("@ApplicationName", this._ApplicationName);
            DataSet dsApplications = _DAL_RBAC.GetDatasetFromSQL(
                "select * from netsqlazman_applicationstable where StoreId = @StoreId and Name = @ApplicationName", sqlParams, true);
            dsApplications.Tables[0].TableName = "Applications";
            // Extract the ApplicationId if we found it
            if (dsApplications.Tables["Applications"].Rows.Count > 0)
                _ApplicationID = Convert.ToInt32(dsApplications.Tables["Applications"].Rows[0]["ApplicationId"]);
            else
                _ApplicationID = -1;
            dsApplications.Dispose();
        }

        public List<RBACCustomerInfo> GetCustomers()
        {
            List<RBACCustomerInfo> result = new List<RBACCustomerInfo>();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@ItemType", Convert.ToInt16(/*ItemType.Task*/1));
            sqlParams.Add("@ApplicationId", this.ApplicationID);
            DataSet dsCustomerList = _DAL_RBAC.GetDatasetFromSQL(
                "select t1.ItemId, t1.Name, t1.ItemType," +
                " convert(int, t2.AttributeValue) as CustomerID," +
                " t3.AttributeValue as CustomerName," +
                " t4.AttributeValue as SFParkFunctionality" +
                " from netsqlazman_ItemsTable as t1" +
                " left outer join netsqlazman_ItemAttributesTable as t2 on t1.itemid = t2.itemid and t2.attributekey = 'CustomerID'" +
                " left outer join netsqlazman_ItemAttributesTable as t3 on t1.itemid = t3.itemid and t3.attributekey = 'CustomerName'" +
                " left join netsqlazman_ItemAttributesTable as t4 on t1.itemid = t4.itemid and t4.attributekey = 'SFParkFunctionality'" +
                " where t1.ItemType = @ItemType and t1.Name like 'Customer:%' " +
                " and t1.ApplicationId = @ApplicationId " +
                " order by t3.AttributeValue", sqlParams, false);

            if ((dsCustomerList != null) && (dsCustomerList.Tables.Count > 0) && (dsCustomerList.Tables[0].Rows.Count > 0))
            {
                DataTable resultTable = dsCustomerList.Tables[0];
                foreach (DataRow nextRow in resultTable.Rows)
                {
                    // If the customer id isn't declared, this is a bad entry we need to skip!
                    if (nextRow["CustomerId"] == DBNull.Value)
                        continue;

                    try
                    {
                        RBACCustomerInfo customerObj = new RBACCustomerInfo();
                        customerObj.RBACItemId = Convert.ToInt32(nextRow["ItemId"]);
                        customerObj.RBACItemName = nextRow["Name"].ToString();
                        customerObj.CustomerId = Convert.ToInt32(nextRow["CustomerId"]);
                        customerObj.CustomerName = Convert.ToString(nextRow["CustomerName"]);

                        if (nextRow["SFParkFunctionality"] != DBNull.Value)
                        {
                            customerObj.SFParkFunctionality = Convert.ToBoolean(nextRow["SFParkFunctionality"].ToString());
                        }

                        result.Add(customerObj);
                    }
                    catch (Exception ex)
                    {
                        // DEBUG: Need to log this?
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
            }
            dsCustomerList.Dispose();
            return result;
        }

        public RBACUserInfo GetUser(string username)
        {
            try
            {
                // Get dataset of filtered users from DB (Note: this will contain more info than from the RBAC method)
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                sqlParams.Add("@UserName", username);
                DataSet _dsUsers = _DAL_RBAC.GetDatasetFromSQL(
                    "select CONVERT(VARBINARY(85), UserID) AS DBUserSid, UserID, UserName, DomainUserName, Password, FullName, CustomerID from Users where UserName = @UserName order by UserName", sqlParams, true);
                _dsUsers.Tables[0].TableName = "Users";

                if (_dsUsers.Tables[0].Rows.Count == 0)
                {
                    _dsUsers.Dispose();
                    return null;
                }

                // Get specific user from RBAC
                RBACUserInfo rbacUser = null;
                rbacUser = new RBACUserInfo();
                DataRow nextRow = _dsUsers.Tables["Users"].Rows[0];

                rbacUser.UserName = Convert.ToString(nextRow["UserName"]);
                rbacUser.FullName = Convert.ToString(nextRow["FullName"]);
                rbacUser.DomainUserName = Convert.ToString(nextRow["DomainUserName"]);

                if (nextRow["DBUserSid"] != DBNull.Value)
                    rbacUser.DBUserCustomSID = (nextRow["DBUserSid"] as byte[]);

                if (nextRow["CustomerID"] != DBNull.Value)
                    rbacUser.PEMDefaultCustomerID = Convert.ToInt32(nextRow["CustomerID"]);

                // Retrieve persisted password bytes, then convert to a string, and then decode from Base64 encoding
                rbacUser.Password_StoredBytes = nextRow["Password"] as byte[];
                byte[] pwdBytes = nextRow["Password"] as byte[];
                string userPwd = ASCIIEncoding.ASCII.GetString(pwdBytes);
                pwdBytes = Convert.FromBase64String(userPwd);
                rbacUser.Password_Encrytped = userPwd;
                // Create DES decryption key that is based on the username (reversed string)
                string Key = MD5Class.Reverse(rbacUser.UserName /*rbacUser.DBUser.UserName*/);
                string SecretKey = MD5Class.ConvertToMD5(Key);
                // Decrypt stored password using DES
                SymmetricEncryption SE = new SymmetricEncryption(EncryptionType.DES);
                string MDecrypt = SE.Decrypt(userPwd, SecretKey);
                rbacUser.Password_PlainText = MDecrypt; // Decrypted password (Plain-text)
                _dsUsers.Dispose();
                return rbacUser;
            }
            catch
            {
                return null;
            }
        }

        public List<RBACUserInfo> GetAllUserNamesAndIDs()
        {
            List<RBACUserInfo> result = new List<RBACUserInfo>();
            try
            {
                // Get dataset of filtered users from DB (Note: this will contain more info than from the RBAC method)
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                DataSet _dsUsers = _DAL_RBAC.GetDatasetFromSQL(
                    "select CONVERT(VARBINARY(85), UserID) AS DBUserSid, UserID, UserName, DomainUserName, FullName, CustomerID from Users order by UserName", sqlParams, true);
                _dsUsers.Tables[0].TableName = "Users";

                if (_dsUsers.Tables[0].Rows.Count == 0)
                {
                    _dsUsers.Dispose();
                    return null;
                }

                RBACUserInfo rbacUser = null;
                foreach (DataRow nextRow in _dsUsers.Tables["Users"].Rows)
                {
                    // Create user from info in data row
                    rbacUser = rbacUser = new RBACUserInfo();
                    rbacUser.UserName = Convert.ToString(nextRow["UserName"]);
                    rbacUser.FullName = Convert.ToString(nextRow["FullName"]);
                    rbacUser.DomainUserName = Convert.ToString(nextRow["DomainUserName"]);

                    if (nextRow["DBUserSid"] != DBNull.Value)
                        rbacUser.DBUserCustomSID = (nextRow["DBUserSid"] as byte[]);

                    if (nextRow["CustomerID"] != DBNull.Value)
                        rbacUser.PEMDefaultCustomerID = Convert.ToInt32(nextRow["CustomerID"]);

                    // Add user to result list
                    result.Add(rbacUser);
                }

                // Free DB resources
                _dsUsers.Dispose();
                return result;
            }
            catch
            {
                return null;
            }
        }

        public List<RBACItemInfo> GetAllItems()
        {
            List<RBACItemInfo> result = new List<RBACItemInfo>();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@ApplicationId", this.ApplicationID);
            DataSet dsResults = _DAL_RBAC.GetDatasetFromSQL(
                "select ItemId, Name, Description, ItemType" +
                " from  netsqlazman_ItemsTable" +
                " where ApplicationId = @ApplicationId" +
                " order by ItemType asc, Description asc, Name asc", sqlParams, false);
            if ((dsResults != null) && (dsResults.Tables.Count > 0) && (dsResults.Tables[0].Rows.Count > 0))
            {
                DataTable resultTable = dsResults.Tables[0];
                foreach (DataRow nextRow in resultTable.Rows)
                {
                    RBACItemInfo itemInfo = new RBACItemInfo();
                    itemInfo.ItemID = Convert.ToInt32(nextRow["ItemId"]);
                    itemInfo.ItemName = Convert.ToString(nextRow["Name"]);
                    itemInfo.Description = Convert.ToString(nextRow["Description"]);
                    itemInfo.ItemType = (RBACItemType)Enum.Parse(typeof(RBACItemType), nextRow["ItemType"].ToString());
                    result.Add(itemInfo);
                }
            }
            dsResults.Dispose();
            return result;
        }

        public List<RBACItemInfo> GetGrantedItemsForUser(RBACUserInfo user, bool onlyExplicitGrants)
        {
            List<RBACItemInfo> result = new List<RBACItemInfo>();

            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@DBUserSid", user.DBUserCustomSID);
            sqlParams.Add("@ApplicationID", _ApplicationID);

            DataSet dsUserRights = _DAL_RBAC.GetDatasetFromSQL(
                    "select t1.ItemId, t1.Name, t1.ItemType, t1.Description, t2.AuthorizationType, 'Direct' as Grantor " +
                     "from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
                     "where t1.ApplicationID = @ApplicationID " +
                     " and t2.objectSid =  @DBUserSid " +
                     " and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
                     "union all  " +
                     "select t4.ItemId, t4.Name, t4.ItemType, t4.Description, t5.AuthorizationType, 'Role' as Grantor " +
                     "from  netsqlazman_ItemsHierarchyTable  t3, netsqlazman_ItemsTable t4, netsqlazman_AuthorizationsTable t5 " +
                     "where t3.ItemId = t4.ItemId " +
                     "and t5.AuthorizationType in (1, 3) " +
                     "and t5.ItemId = t3.MemberOfItemId " +
                     "and t3.MemberOfItemId in " +
                     "( " +
                     "select t1.ItemId " +
                     "from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
                     "where t1.ApplicationID = @ApplicationID and t2.objectSid = @DBUserSid " +
                     "and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
                     ") " +
                     "order by 3 asc, 4 asc, 2 asc", sqlParams, true);



                     //"select t1.ItemId, t1.Name, t1.ItemType, t1.Description, t2.AuthorizationType, 'Direct' as Grantor " +
                     //"from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
                     //"where t1.ApplicationID = @ApplicationID " +
                     //" and t2.OwnerSid =  CONVERT(varchar(max),ownerSid,2) " + /*" and t2.objectSid = @DBUserSid " +*/
                     //" and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
                     //"union all  " +
                     //"select t4.ItemId, t4.Name, t4.ItemType, t4.Description, t5.AuthorizationType, 'Role' as Grantor " +
                     //"from  netsqlazman_ItemsHierarchyTable  t3, netsqlazman_ItemsTable t4, netsqlazman_AuthorizationsTable t5 " +
                     //"where t3.ItemId = t4.ItemId " +
                     //"and t5.AuthorizationType in (1, 3) " +
                     //"and t5.ItemId = t3.MemberOfItemId " +
                     //"and t3.MemberOfItemId in " +
                     //"( " +
                     //"select t1.ItemId " +
                     //"from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
                     //"where t1.ApplicationID = 73 and t2.OwnerSid = CONVERT(varchar(max),ownerSid,2) " + /*"where t1.ApplicationID = @ApplicationID and t2.objectSid = @DBUserSid " +*/
                     //"and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
                     //") " +
                     //"order by 3 asc, 4 asc, 2 asc", sqlParams, true);

            //DataSet dsUserRights = _DAL_RBAC.GetDatasetFromSQL(
            //         "select t1.ItemId, t1.Name, t1.ItemType, t1.Description, t2.AuthorizationType, 'Direct' as Grantor " +
            //         "from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
            //         "where t1.ApplicationID = @ApplicationID " +
            //         " and t2.OwnerSid = @DBUserSid " + /*" and t2.objectSid = @DBUserSid " +*/
            //         " and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
            //         "union all  " +
            //         "select t4.ItemId, t4.Name, t4.ItemType, t4.Description, t5.AuthorizationType, 'Role' as Grantor " +
            //         "from  netsqlazman_ItemsHierarchyTable  t3, netsqlazman_ItemsTable t4, netsqlazman_AuthorizationsTable t5 " +
            //         "where t3.ItemId = t4.ItemId " +
            //         "and t5.AuthorizationType in (1, 3) " +
            //         "and t5.ItemId = t3.MemberOfItemId " +
            //         "and t3.MemberOfItemId in " +
            //         "( " +
            //         "select t1.ItemId " +
            //         "from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
            //         "where t1.ApplicationID = @ApplicationID and t2.OwnerSid = @DBUserSid " + /*"where t1.ApplicationID = @ApplicationID and t2.objectSid = @DBUserSid " +*/
            //         "and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
            //         ") " +
            //         "order by 3 asc, 4 asc, 2 asc", sqlParams, true);

            foreach (DataRow dr in dsUserRights.Tables[0].Rows)
            {
                if ((onlyExplicitGrants == false) || (dr["Grantor"].ToString() == "Direct"))
                {
                    RBACItemInfo item = new RBACItemInfo();
                    item.ItemID = Convert.ToInt32(dr["ItemId"]);
                    item.ItemName = dr["Name"].ToString();
                    item.Description = Convert.ToString(dr["Description"]);
                    item.ItemType = (RBACItemType)Enum.Parse(typeof(RBACItemType), dr["ItemType"].ToString());

                    bool alreadyExists = false;
                    foreach (RBACItemInfo nextItem in result)
                    {
                        if (nextItem.ItemID == item.ItemID)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }
                    if (alreadyExists == false)
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public List<RBACItemInfo> GetAllRoles()
        {
            List<RBACItemInfo> result = new List<RBACItemInfo>();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@ApplicationId", this.ApplicationID);
            sqlParams.Add("@ItemType", Convert.ToInt32(0/*ItemType.Role*/));
            DataSet dsResults = _DAL_RBAC.GetDatasetFromSQL(
                "select ItemId, Name, Description, ItemType" +
                " from  netsqlazman_ItemsTable" +
                " where ApplicationId = @ApplicationId" +
                " and ItemType = @ItemType" +
                " order by Name", sqlParams, false);
            if ((dsResults != null) && (dsResults.Tables.Count > 0) && (dsResults.Tables[0].Rows.Count > 0))
            {
                DataTable resultTable = dsResults.Tables[0];
                foreach (DataRow nextRow in resultTable.Rows)
                {
                    RBACItemInfo itemInfo = new RBACItemInfo();
                    itemInfo.ItemID = Convert.ToInt32(nextRow["ItemId"]);
                    itemInfo.ItemName = Convert.ToString(nextRow["Name"]);
                    itemInfo.Description = Convert.ToString(nextRow["Description"]);
                    itemInfo.ItemType = RBACItemType.Role;
                    result.Add(itemInfo);
                }
            }
            dsResults.Dispose();
            return result;
        }

        public ApplicationLogonResponse LogonAsRBACUser(string username, string password)
        {
            // Create response object
            ApplicationLogonResponse responseObj = new ApplicationLogonResponse();

            try
            {
                // Try to get RBAC user
                RBACUserInfo rbacUserInfo = GetUser(username);
                if (rbacUserInfo == null)
                {
                    responseObj.ErrorMsg = "Username not found in system";
                    return responseObj;
                }

                // Check to make sure passwords match
                if (string.Compare(rbacUserInfo.Password_PlainText, password) != 0)
                {
                    responseObj.ErrorMsg = "Incorrect username or password";
                    return responseObj;
                }

                // Create a new RBAC Session ID
                string SessionId = System.Guid.NewGuid().ToString();

                // Create a new record in SessionDetails table
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                sqlParams.Clear();
                sqlParams.Add("@UserId", rbacUserInfo.DBUserCustomSID_AsInt);
                sqlParams.Add("@SessionID", SessionId);
                int result = _DAL_RBAC.ExecuteNonQuery(
                    "insert into SessionDetails (UserId, SessionID, SessionExpTime, Createdby, CreateDateTime, Updatedby, UpdateDateTime)" +
                    " values (@UserId, @SessionID, DATEADD(hour,12,getdate()), @UserId, getdate(), @UserId, getdate())", sqlParams, true);

                // Create a new record in LoginDetails table
                sqlParams.Clear();
                sqlParams.Add("@UserId", rbacUserInfo.DBUserCustomSID_AsInt); 
                sqlParams.Add("@SessionID", SessionId);
                result = _DAL_RBAC.ExecuteNonQuery(
                    "insert into LoginDetails (Userid, SessionID, LoginTime, Createdby, CreateDateTime, Updatedby, UpdateDateTime)" +
                    " values (@UserId, @SessionID, getdate(), @UserId, getdate(), @UserId, getdate())", sqlParams, true);

                // Get all granted items of the user
                List<RBACItemInfo> grantedItemsForUser = GetGrantedItemsForUser(rbacUserInfo, false);
                List<RBACItemInfo> itemsToRemove = new List<RBACItemInfo>();
                List<RBACCustomerInfo> allCustomers = GetCustomers();
                List<RBACCustomerInfo> grantedCustomers = new List<RBACCustomerInfo>();

                // Look through each item. If its actually a customer, we will use a customized object instead
                foreach (RBACItemInfo nextItem in grantedItemsForUser)
                {
                    if (nextItem.ItemName.StartsWith("Customer:"))
                    {
                        itemsToRemove.Add(nextItem);
                        RBACCustomerInfoPredicate customerPredicate = new RBACCustomerInfoPredicate(nextItem.ItemID);
                        RBACCustomerInfo customerObj = allCustomers.Find(customerPredicate.CompareByRbacID);
                        if (customerObj != null)
                            grantedCustomers.Add(customerObj);
                    }
                }
                foreach (RBACItemInfo nextItem in itemsToRemove)
                {
                    grantedItemsForUser.Remove(nextItem);
                }

                // Update the response object
                responseObj.SessionId = SessionId;
                responseObj.Username = rbacUserInfo.UserName;
                responseObj.DomainUsername = rbacUserInfo.DomainUserName;
                responseObj.FullName = rbacUserInfo.FullName;
                responseObj.RbacUserId = rbacUserInfo.DBUserCustomSID_AsInt;
                responseObj.GrantedItems.AddRange(grantedItemsForUser);
                responseObj.GrantedCustomers.AddRange(grantedCustomers);
            }
            catch (Exception ex)
            {
                responseObj.ErrorMsg = ex.Message;

                // Debug: Need to log this?
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            // Return the result object
            return responseObj;
        }

        public void LogoutRBACUser(int RbacUserID, string RbacSessionID)
        {
            try
            {
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                sqlParams.Clear();
                sqlParams.Add("@UserId", RbacUserID);
                sqlParams.Add("@SessionID", RbacSessionID);
                int result = _DAL_RBAC.ExecuteNonQuery(
                    "Delete from SessionDetails where UserId = @UserId and SessionID = @SessionID", sqlParams, false);
                result = _DAL_RBAC.ExecuteNonQuery(
                    "Update LoginDetails set LogoutTime = getdate(), UpdatedBy = @UserId, UpdateDateTime = getdate() where UserId=@UserId and SessionID=@SessionID",
                    sqlParams, false);
            }
            catch
            {
            }
        }

        public string GetUserNameFromRBACSessionID(string RBACSession)
        {
            try
            {
                // Query the RBAC database to find user associated with SessionID (and make sure the session is still valid)
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                sqlParams.Add("@SessionID", RBACSession);
                DataSet _dsUsers = _DAL_RBAC.GetDatasetFromSQL(
                    "select top 1 u.UserName, u.UserID, ld.SessionID, ld.LoginTime, ld.LogoutTime, ld.UpdateDateTime from" +
                    " Users u, LoginDetails ld where ld.UserID = u.UserID and ld.LogoutTime is null" +
                    " and ld.SessionID = @SessionID" +
                    " order by ld.UpdateDateTime desc, ld.LoginTime desc",
                    sqlParams, true);
                _dsUsers.Tables[0].TableName = "Users";

                // If query came back empty, then no user was found that is associated with the RBAC session
                if (_dsUsers == null)
                    return string.Empty;
                if (_dsUsers.Tables[0].Rows.Count == 0)
                {
                    _dsUsers.Dispose();
                    return null;
                }

                string result = Convert.ToString(_dsUsers.Tables[0].Rows[0]["UserName"]);
                _dsUsers.Dispose();
                return result;
            }
            catch
            {
                // Failed, so we'll just return an empty string indicating we were unable to automatically logon
                return string.Empty;
            }
        }

        #region Deprecated -- not needed for this application
        /*
        public RBACCustomerInfo GetCustomerInfo(string customerName)
        {
            RBACCustomerInfo result = new RBACCustomerInfo();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@ItemCustomerName", "Customer:" + customerName);
            sqlParams.Add("@ApplicationId", this.ApplicationID);
            DataSet dsCustomerList = _DAL_RBAC.GetDatasetFromSQL(
                "select t1.ItemId, t1.Name, t1.ItemType," +
                " convert(int, t2.AttributeValue) as CustomerID," +
                " t3.AttributeValue as CustomerName," +
                " t4.AttributeValue as SFParkFunctionality" +
                " from netsqlazman_ItemsTable as t1" +
                " left outer join netsqlazman_ItemAttributesTable as t2 on t1.itemid = t2.itemid and t2.attributekey = 'CustomerID'" +
                " left outer join netsqlazman_ItemAttributesTable as t3 on t1.itemid = t3.itemid and t3.attributekey = 'CustomerName'" +
                " left join netsqlazman_ItemAttributesTable as t4 on t1.itemid = t4.itemid and t4.attributekey = 'SFParkFunctionality'" +
                " where t1.Name = @ItemCustomerName" +
                " and t1.ApplicationId = @ApplicationId", sqlParams, false);
            if ((dsCustomerList != null) && (dsCustomerList.Tables.Count > 0) && (dsCustomerList.Tables[0].Rows.Count > 0))
            {
                DataTable resultTable = dsCustomerList.Tables[0];
                foreach (DataRow nextRow in resultTable.Rows)
                {
                    result.RBACItemId = Convert.ToInt32(nextRow["ItemId"]);
                    result.RBACItemName = nextRow["Name"].ToString();
                    result.CustomerId = Convert.ToInt32(nextRow["CustomerId"]);
                    result.CustomerName = Convert.ToString(nextRow["CustomerName"]);

                    if (nextRow["SFParkFunctionality"] != DBNull.Value)
                    {
                        result.SFParkFunctionality = Convert.ToBoolean(nextRow["SFParkFunctionality"].ToString());
                    }
                }
            }
            dsCustomerList.Dispose();
            return result;
        }
        */

        /*
        public List<RBACItemInfo> GetRolesWithAccessToItem(int itemId)
        {
            List<RBACItemInfo> result = new List<RBACItemInfo>();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@ItemId", itemId);
            DataSet dsResults = _DAL_RBAC.GetDatasetFromSQL(
                "select roles.ItemId as RoleId, roles.Name as RoleName, roles.Description" +
                " from  netsqlazman_ItemsHierarchyTable as relate," +
                " netsqlazman_ItemsTable as roles" +
                " where relate.ItemId = @ItemId" +
                " and roles.ItemId = relate.MemberOfItemId" +
                " and roles.ItemType = 0", sqlParams, false);
            if ((dsResults != null) && (dsResults.Tables.Count > 0) && (dsResults.Tables[0].Rows.Count > 0))
            {
                DataTable resultTable = dsResults.Tables[0];
                foreach (DataRow nextRow in resultTable.Rows)
                {
                    RBACItemInfo itemInfo = new RBACItemInfo();
                    itemInfo.ItemID = Convert.ToInt32(nextRow["RoleId"]);
                    itemInfo.ItemName = Convert.ToString(nextRow["RoleName"]);
                    itemInfo.Description = Convert.ToString(nextRow["Description"]);
                    itemInfo.ItemType = RBACItemType.Role;
                    result.Add(itemInfo);
                }
            }
            dsResults.Dispose();
            return result;
        }
        */

        /*
        public List<RBACItemInfo> GetChildRoles(int itemId)
        {
            List<RBACItemInfo> result = new List<RBACItemInfo>();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@ItemId", itemId);
            DataSet dsResults = _DAL_RBAC.GetDatasetFromSQL(
                "select roles.ItemId as RoleId, roles.Name as RoleName, roles.Description" +
                " from  netsqlazman_ItemsHierarchyTable as relate," +
                " netsqlazman_ItemsTable as roles" +
                " where relate.MemberOfItemId = @ItemId" +
                " and roles.ItemId = relate.ItemId" // and roles.ItemType = 0"
                , sqlParams, false);
            if ((dsResults != null) && (dsResults.Tables.Count > 0) && (dsResults.Tables[0].Rows.Count > 0))
            {
                DataTable resultTable = dsResults.Tables[0];
                foreach (DataRow nextRow in resultTable.Rows)
                {
                    RBACItemInfo itemInfo = new RBACItemInfo();
                    itemInfo.ItemID = Convert.ToInt32(nextRow["RoleId"]);
                    itemInfo.ItemName = Convert.ToString(nextRow["RoleName"]);
                    itemInfo.Description = Convert.ToString(nextRow["Description"]);
                    itemInfo.ItemType = RBACItemType.Role;
                    result.Add(itemInfo);
                }
            }
            dsResults.Dispose();
            return result;
        }
        */

        /*
        // Returns null if unable to automatically logon based on domain username.
        // When successful, ApplicationLogonResponse object is returned, which contains the RBAC Session ID.
        public ApplicationLogonResponse LogonAsKnownDomainUser(string domainUsername)
        {
            try
            {
                // Query the RBAC database to find user by DomainUserName
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                sqlParams.Add("@DomainUserName", domainUsername);
                DataSet _dsUsers = _DAL_RBAC.GetDatasetFromSQL(
                    "select UserID, UserName, DomainUserName, Password, FullName from Users where DomainUserName = @DomainUserName order by UserName",
                    sqlParams, true);
                _dsUsers.Tables[0].TableName = "Users";

                // If query came back empty, then no user was found that is associated with the DomainUserName
                if (_dsUsers.Tables[0].Rows.Count == 0)
                {
                    return null;
                }

                // DEBUG: Possbile idea below, but probably won't implement:
                // Its possible the user has an active RBAC session from using a different application or website. 
                // If so, we could just reuse the existing SessionId instead of creating a new one.  Problem is, 
                // if user logs out of another application or website, our Session ID would become invalid, which
                // isn't nice for the user.

                // Create a new RBAC Session ID
                string SessionId = System.Guid.NewGuid().ToString();

                // Create a new record in SessionDetails table
                sqlParams.Clear();
                sqlParams.Add("@UserId", Convert.ToInt32(_dsUsers.Tables[0].Rows[0]["UserID"]));
                sqlParams.Add("@SessionID", SessionId);
                int result = _DAL_RBAC.ExecuteNonQuery(
                    "insert into SessionDetails (UserId, SessionID, SessionExpTime, Createdby, CreateDateTime, Updatedby, UpdateDateTime)" +
                    " values (@UserId, @SessionID, DATEADD(hour,12,getdate()), @UserId, getdate(), @UserId, getdate())", sqlParams, true);

                // Create a new record in LoginDetails table
                sqlParams.Clear();
                sqlParams.Add("@UserId", Convert.ToInt32(_dsUsers.Tables[0].Rows[0]["UserID"]));
                sqlParams.Add("@SessionID", SessionId);
                result = _DAL_RBAC.ExecuteNonQuery(
                    "insert into LoginDetails (Userid, SessionID, LoginTime, Createdby, CreateDateTime, Updatedby, UpdateDateTime)" +
                    " values (@UserId, @SessionID, getdate(), @UserId, getdate(), @UserId, getdate())", sqlParams, true);

                // Get RBAC user info
                RBACUserInfo rbacUserInfo = GetUser(_dsUsers.Tables[0].Rows[0]["UserName"].ToString());
                // Get all granted items of the user
                List<RBACItemInfo> grantedItemsForUser = GetGrantedItemsForUser(rbacUserInfo, false);
                List<RBACItemInfo> itemsToRemove = new List<RBACItemInfo>();
                List<RBACCustomerInfo> allCustomers = GetCustomers();
                List<RBACCustomerInfo> grantedCustomers = new List<RBACCustomerInfo>();

                // Look through each item. If its actually a customer, we will use a customized object instead
                foreach (RBACItemInfo nextItem in grantedItemsForUser)
                {
                    if (nextItem.ItemName.StartsWith("Customer:"))
                    {
                        itemsToRemove.Add(nextItem);
                        RBACCustomerInfoPredicate customerPredicate = new RBACCustomerInfoPredicate(nextItem.ItemID);
                        RBACCustomerInfo customerObj = allCustomers.Find(customerPredicate.CompareByRbacID);
                        if (customerObj != null)
                            grantedCustomers.Add(customerObj);
                    }
                }
                foreach (RBACItemInfo nextItem in itemsToRemove)
                {
                    grantedItemsForUser.Remove(nextItem);
                }

                // Create response object
                ApplicationLogonResponse responseObj = new ApplicationLogonResponse();
                responseObj.SessionId = SessionId;
                responseObj.Username = rbacUserInfo.UserName;
                responseObj.DomainUsername = rbacUserInfo.DomainUserName;
                responseObj.FullName = rbacUserInfo.FullName;
                responseObj.RbacUserId = Convert.ToInt32(_dsUsers.Tables[0].Rows[0]["UserID"]);
                responseObj.GrantedItems.AddRange(grantedItemsForUser);
                responseObj.GrantedCustomers.AddRange(grantedCustomers);

                // Cleanup DB resource and return the SessionID
                _dsUsers.Dispose();

                // Return the result object
                return responseObj;
            }
            catch (Exception ex)
            {
                // Failed, so we'll just return a null object indicating we were unable to automatically logon
                return null;
            }
        }
        */
        #endregion
    }

    public class RBACItemInfoComparer : IComparer<RBACItemInfo>
    {
        public RBACItemInfoComparer()
        {
        }

        public int Compare(RBACItemInfo elementX, RBACItemInfo elementY)
        {
            return string.Compare(elementX.ToString(), elementY.ToString());
        }
    }

    public class RBACUserInfo
    {
        public byte[] DBUserCustomSID = new byte[]{};

        public int DBUserCustomSID_AsInt
        {
            get
            {
                string str = "";
                if (this.DBUserCustomSID != null)
                {
                    for (int i = 0; i < this.DBUserCustomSID.Length; i++)
                    {
                        str = str + this.DBUserCustomSID[i].ToString("X2");
                    }
                }

                return int.Parse(str, System.Globalization.NumberStyles.HexNumber);
            }
        }

        public string DomainUserName = string.Empty;

        public string Password_PlainText = string.Empty;
        public string Password_Encrytped = string.Empty;
        public byte[] Password_StoredBytes = null;

        public string UserName = string.Empty;
        public string FullName = string.Empty;

        public int PEMDefaultCustomerID = 0;

        public RBACUserInfo()
        {
        }

        public override string ToString()
        {
            return this.UserName.ToString();
        }

        #region Deprecated
        /*
        public List<string> AssociatedDuncanRoles(AppRBACInterface rbac, DBAccess rbacDBAccess)
        {
            List<string> result = new List<string>();
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            sqlParams.Add("@DBUserSid", this.DBUserCustomSID);
            sqlParams.Add("@ApplicationID", rbac.ApplicationID);
            DataSet dsUserRights = rbacDBAccess.GetDatasetFromSQL(
                "select t1.ItemId, t1.Name, t1.ItemType, t1.Description, t2.AuthorizationType, 'Direct' as Grantor " +
                "from netsqlazman_ItemsTable as t1, netsqlazman_Authorizationstable t2 " +
                "where t1.ApplicationID = @ApplicationID " +
                " and t2.OwnerSid = @DBUserSid " + // " and t2.objectSid = @DBUserSid "
                " and t1.ItemId = t2.ItemId and t2.AuthorizationType in (1, 3) " +
                " and t1.Name in ('Admin', 'Operations', 'Engineering') " +
                " and t1.ItemType = " + Convert.ToInt32(0).ToString() + " " + // Convert.ToInt32(ItemType.Role)
                "order by 3 asc, 4 asc, 2 asc", sqlParams, true);
            foreach (DataRow nextRow in dsUserRights.Tables[0].Rows)
            {
                result.Add(nextRow["Name"].ToString());
            }
            dsUserRights.Dispose();
            return result;
        }
        */
        #endregion
    }
}
