using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Entities.MaintenanceGroups
{
    public class MaintenanceGroupCustomersModel : CustomerBaseModel, IValidatableObject
    {
        public List<SelectListItem> AvailableCustomers { get; set; }
        public List<MaintenanceGroupCustomerModel> Customers { get; set; }
        public int CustomerIdToAdd { get; set; }
        public char Separator = '|';
        public List<string> NewCustomers { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
