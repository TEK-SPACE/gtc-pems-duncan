using System.Collections.Generic;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Entities.Users
{
    public class CustomerRoleModel
    {
        public string Username { get; set; }
        public List<CustomerRole> CustomerRoles { get; set; }
    }

    public class CustomerRole
    {
        public PemsCity Customer { get; set; }
        public string CustomerInternalName { get; set; }
        public Dictionary<string, string> Roles { get; set; }
        public string CurrentRole { get; set; }
    }
}