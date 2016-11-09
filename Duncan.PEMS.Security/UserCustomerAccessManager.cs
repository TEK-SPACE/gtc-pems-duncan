using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using NLog;


namespace Duncan.PEMS.Security
{
    public class UserCustomerAccessManager 
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        readonly PEMRBACEntities _rbacEntities = new PEMRBACEntities();


        /// <summary>
        /// Gets a distinct list of customer IDs that the user has access to.
        /// </summary>
        /// <param name="userId">ID of the user to get access IDs for.</param>
        /// <returns></returns>
        public List<int> GetCustomersIds(int userId)
        {
            var customerIds = _rbacEntities.UserCustomerAccesses.Where(x => x.UserId == userId).Select(x => x.CustomerId).Distinct().ToList();
            return customerIds;
        }

        /// <summary>
        /// Sets the customer access for a user based on the customer IDs passed in
        /// </summary>
        /// <param name="userId">id of the user</param>
        /// <param name="customerIds">List of customerIDs to add tot he users access</param>
        public void SetCustomersIds(int userId, List<int> customerIds)
        {
            foreach (var customerId in customerIds)
            {
                var uca = new UserCustomerAccess { CustomerId = customerId, UserId = userId };
                _rbacEntities.UserCustomerAccesses.Add(uca);
            }
            _rbacEntities.SaveChanges();

        }

        /// <summary>
        ///Removes all customer access for a specific user. 
        /// </summary>
        /// <param name="userId">ID of the user to remove customer access from.</param>
        public void ClearCustomerAccess(int userId)
        {
            var ucas = _rbacEntities.UserCustomerAccesses.Where(x => x.UserId == userId);
            if (ucas.Any())
                foreach (var userCustomerAccess in ucas)
                    _rbacEntities.UserCustomerAccesses.Remove(userCustomerAccess);
            _rbacEntities.SaveChanges();
        }

        /// <summary>
        /// Adds the customer to the access table for a user
        /// </summary>
        /// <param name="userId">ID of hte user to add access for</param>
        /// <param name="customerId">ID of the customer to add to the user</param>
        public void AddCustomerAccess(int userId, int customerId)
        {
            var existingUca = _rbacEntities.UserCustomerAccesses.FirstOrDefault(x => x.UserId == userId && x.CustomerId == customerId);
            if (existingUca == null)
            {
                var uca = new UserCustomerAccess { CustomerId = customerId, UserId = userId };
                _rbacEntities.UserCustomerAccesses.Add(uca);
                _rbacEntities.SaveChanges();
            }
        }

    }
}
