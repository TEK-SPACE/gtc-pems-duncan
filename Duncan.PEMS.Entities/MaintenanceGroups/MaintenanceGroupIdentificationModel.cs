using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Entities.MaintenanceGroups
{
    public class MaintenanceGroupIdentificationModel : CustomerBaseModel, IValidatableObject
    {
        public MaintenanceGroupIdentificationModel()
        {
            Status = new CustomerStatusModel();
            Contact = new CustomerContactModel();
            Localization = new CustomerLocalizationModel();
        }

        [Required]
        public new string DisplayName { get; set; }

    

        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Must be between 6 and 8 characters long.")]
        [RegularExpression(@"(?=.*\d)(?=.*\W+)(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Must contain 1: uppercase letter, number, special character")]
        public string DefaultPassword { get; set; }

        public CustomerContactModel Contact { get; set; }
        public CustomerLocalizationModel Localization { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}


