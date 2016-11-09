using System.Collections.Generic;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Entities.MaintenanceGroups
{
    public class MaintenanceGroupActivateIssue
    {
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
    public class MaintenanceGroupActivateModel : CustomerBaseModel
    {
        public List<MaintenanceGroupActivateIssue> Issues { get; set; }
        public bool CanActivate { get; set; }
        public string ActivateMessage { get; set; }

        public MaintenanceGroupActivateModel()
        {
            Issues = new List<MaintenanceGroupActivateIssue>();
        }
    }
}
