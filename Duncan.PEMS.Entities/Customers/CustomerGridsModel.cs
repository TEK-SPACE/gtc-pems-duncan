using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.Grids;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerGridsModel : CustomerBaseModel, IValidatableObject
    {
        public char Separator = '_';
        public List<GridTemplateSet> TemplateSets { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            //            if ("" == null)
            //                yield return new ValidationResult("EMail is required.", new string[] { "PrimaryEMail" });
            return errors;
        }
    }
}