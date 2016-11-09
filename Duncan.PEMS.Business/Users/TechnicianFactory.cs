using System.Linq;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;

namespace Duncan.PEMS.Business.Users
{
    public class TechnicianFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// This constructor is used when you do not want to be specific to a customer (for maintenance groups)
        /// </summary>
        public TechnicianFactory()
        {
        }

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public TechnicianFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Sets the user to a technican if they havent done so already. This is only a set, never a remove, since this will break existing data
        /// This forces a user to use  a specific connection string, so when we are looping through customers and adding the tech to all pems DBs, they will be persisted to all pems dbs. 
        /// //individual customers will just use the otheer non connection string specific method (so it used the pemsentities from teh constructor) so their db context stays up to date (when adding users, etc)
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="currentCity">pems city that holds all of the maintenance customers. used mostly by passing in the currentcity set on the framework.</param>
        public void SetTechnician(UserModel userModel, PemsCity currentCity = null)
        {
            //if they are
            if (userModel.IsTechnician)
            {
                int userId = (new UserFactory()).GetUserId(userModel.Username);
                //loop through each mg customer and add the technian
                if (currentCity != null && currentCity.CustomerType == CustomerProfileType.MaintenanceGroup)
                {
                    AddTechnicianToMaintenanceGroup(userModel, currentCity, userId);
                }
                //if they are a customer and being set as a technician, this means that the customer is part of a maintenance group (otherwise the check box wont show up)
                   //find the maint group and add the tech to that maint gorup (and all of its customers)
                else
                {
                    //go find the maint group
                    var existingMGCustomer = RbacEntities.MaintenanceGroupCustomers.FirstOrDefault(x => x.CustomerId == currentCity.Id);
                   if (existingMGCustomer != null)
                   {
                       var maintGroup = new PemsCity(existingMGCustomer.MaintenanceGroupId.ToString());
                       AddTechnicianToMaintenanceGroup(userModel, maintGroup, userId);
                   }
                }
            }
        }

        private void AddTechnicianToMaintenanceGroup(UserModel userModel, PemsCity mainGroup, int userId)
        {
            //make sure the user has access to the maintenance group. Add them as a _maint mmeber and to the caching table
            (new UserCustomerAccessManager()).AddCustomerAccess(userId, mainGroup.Id);
            var mgAuthMgr = new AuthorizationManager(mainGroup.InternalName);
            mgAuthMgr.AddGroupMember(Constants.Security.DefaultMaintenanceGroupName, userModel.Username);

            //add them to all discinct pems conn strings
            foreach (
                var maintCustomer in
                    mainGroup.MaintenanceCustomers)
            {
                var maintCustomerPemsConnString = maintCustomer.PemsConnectionStringName;

                //create the technician. only do this if they dont exist already, otherwise update
                using (var pemsEntities = new PEMEntities(maintCustomerPemsConnString))
                {
                    SetMaintenanceGroupTechnician(userModel, pemsEntities, userId);
                    //add user to _maintenance group here for the specific customer
                    var authorizationManager = new AuthorizationManager(maintCustomer.InternalName);
                    authorizationManager.AddGroupMember(Constants.Security.DefaultMaintenanceGroupName, userModel.Username);

                    //now add them to caching table for this customer
                    //not sure if we need this or not

                    (new UserCustomerAccessManager()).AddCustomerAccess(userId, maintCustomer.Id);
                }
            }
        }

        #region Helper Methods

        private void SetMaintenanceGroupTechnician(UserModel userModel, PEMEntities pemsEntities, int userId)
        {
            //either get or create a new one
            var existingTech =
                pemsEntities.TechnicianDetails.FirstOrDefault(x => x.TechnicianId == userId);
            if (existingTech != null)
            {
                existingTech.Contact = userModel.PhoneNumber;
                existingTech.Name = userModel.FirstName + " " + userModel.LastName;
                pemsEntities.SaveChanges();
            }
            else
            {
                //set the tech ID to the user iD
                var technician = new TechnicianDetail
                {
                    Contact = userModel.PhoneNumber,
                    Name = userModel.FirstName + " " + userModel.LastName,
                    TechnicianId = userId
                };

                pemsEntities.TechnicianDetails.Add(technician);
                pemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Determines if the user is a technician
        /// </summary>
        public static bool IsUserTechnician(string username)
        {
            //set if they are a tech right off the bat
            var settingsFactory = new SettingsFactory();
            int userId = (new UserFactory()).GetUserId(username);
            var isTech = settingsFactory.Get(userId, Constants.User.IsTechnician);
            bool isTECH = false;

            if (isTech != null)
            {
                bool.TryParse(isTech.Value, out isTECH);
            }
            return isTECH;
        }

        /// <summary>
        /// Adds an admin to the maintenance group. This DOES NOT create a technician out of the administrator
        /// this just gives the admin user access to the MG and its customers.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="mainGroup"></param>
        /// <param name="userId"></param>
        public void AddAdminTechnicianToMaintenanceGroup(string username, PemsCity mainGroup, int userId)
        {
            //make sure the user has access to the maintenance group. Add them as a _maint mmeber and to the caching table
            (new UserCustomerAccessManager()).AddCustomerAccess(userId, mainGroup.Id);
           
            var mgAuthMgr = new AuthorizationManager(mainGroup.InternalName);
            mgAuthMgr.AddGroupMember(Constants.Security.DefaultMaintenanceGroupName, username);
            //add them to all discinct pems conn strings
            foreach (var maintCustomer in mainGroup.MaintenanceCustomers)
            {
                //create the technician. only do this if they dont exist already, otherwise update
                    //add user to _maintenance group here for the specific customer
                    var authorizationManager = new AuthorizationManager(maintCustomer.InternalName);
                    authorizationManager.AddGroupMember(Constants.Security.DefaultMaintenanceGroupName, username);
                    //now add them to caching table for this customer
                    (new UserCustomerAccessManager()).AddCustomerAccess(userId, maintCustomer.Id);
            }
        }
        #endregion
    }
}
