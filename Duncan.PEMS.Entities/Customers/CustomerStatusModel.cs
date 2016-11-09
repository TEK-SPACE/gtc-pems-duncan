using System.Collections.Generic;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerStatusModel
    {
        public string Status { get; set; }
        public CustomerStatus StatusId { get; set; }

        public List<SelectListItem> StatusList { get; set; }

        public string StatusChangeDate { get; set; }
        public string StatusDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
    }
}