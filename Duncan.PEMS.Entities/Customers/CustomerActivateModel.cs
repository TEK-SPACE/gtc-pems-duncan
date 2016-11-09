using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerActivateIssue
    {
        public string Description { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }


    public class CustomerActivateModel : CustomerBaseModel
    {
        public List<CustomerActivateIssue> Issues { get; set; }

        public bool CanActivate { get; set; }

        public string ActivateMessage { get; set; }


        public CustomerActivateModel()
        {
            Issues = new List<CustomerActivateIssue>();
        }
    }
}
