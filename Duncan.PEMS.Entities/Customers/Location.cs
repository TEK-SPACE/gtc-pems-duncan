using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Duncan.PEMS.Entities.Customers
{
    public class Location : IValidatableObject
    {
        [Required]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Required]
        public string City { get; set; }

        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var list = new List<ValidationResult>();

            if ( Address1 != null && Address2 != null )
            {
                if ( Address1.Equals( Address2 ) )
                    list.Add( new ValidationResult( "Address 1 and Address 2 are the same", new[] {"Address2"} ) );
            }

            return list;
        }
    }
}