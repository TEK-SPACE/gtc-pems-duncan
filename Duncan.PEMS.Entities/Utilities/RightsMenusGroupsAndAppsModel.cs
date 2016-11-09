using System.Collections.Generic;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Entities.Utilities
{
    public class RightsMenusGroupsAndAppsModel
    {
        public RightsMenusGroupsAndAppsModel()
        {
            Logs = new List<string>();
            Errors = new List<string>();
        }

        public bool ModelIsResults { get; set; }
        public List<string> Logs { get; set; }
        public List<string> Errors { get; set; }
    }

    public class RbacCustomerListModel
    {
        public List<PemsCity> Customers { get; set; }
    }
}