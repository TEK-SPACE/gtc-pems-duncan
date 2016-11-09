using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerIdentificationModel : CustomerBaseModel, IValidatableObject
    {
        public CustomerIdentificationModel()
        {
            Status = new CustomerStatusModel();

            AddressCountry = new List<SelectListItem>();

            Contact = new CustomerContactModel();

            Localization = new CustomerLocalizationModel();
        }

        [Required]
        public string InternalName { get; set; }

        [Required]
        public new string DisplayName { get; set; }

        public string CityCode { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressState { get; set; }
        public string AddressCountryDisplay { get; set; }
        public List<SelectListItem> AddressCountry { get; set; }
        public int AddressCountryId { get; set; }
        public string AddressPostalCode { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string FromEmailAddress { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Must be between 6 and 8 characters long.")]
        [RegularExpression(@"(?=.*\d)(?=.*\W+)(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Must contain 1: uppercase letter, number, special character")]
        public string DefaultPassword { get; set; }


        public CustomerContactModel Contact { get; set; }

        public CustomerLocalizationModel Localization { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if ( Longitude.Equals( 0.0 ) )
            {
                errors.Add(new ValidationResult("Enter Non-Zero Longitude.", new string[] { "Longitude" }));
            }
            if (Latitude.Equals(0.0))
            {
                errors.Add(new ValidationResult("Enter Non-Zero Latitude.", new string[] { "Latitude" }));
            }

            return errors;
        }
    }

    public class CustomerContactModel : IValidatableObject
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string FromEmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            //            if ("" == null)
            //                yield return new ValidationResult("EMail is required.", new string[] { "PrimaryEMail" });

            return errors;
        }
    }

    public class CustomerLocalizationModel : IValidatableObject
    {
        public CustomerLocalizationModel()
        {
            TimeZone = new List<SelectListItem>();
            TimeZoneId = -1;
            Language = new List<SelectListItem>();
            LanguageId = 0;
        }

        public string TimeZoneDisplay { get; set; }
        public List<SelectListItem> TimeZone { get; set; }
        public int TimeZoneId { get; set; }
        public string LanguageDisplay { get; set; }
        public List<SelectListItem> Language { get; set; }
        public int LanguageId { get; set; }

        public bool Is24Hr { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if ( TimeZoneId == -1 )
                errors.Add( new ValidationResult( "Time Zone is required.", new[] {"TimeZone"} ) );

            if ( LanguageId == 0 )
                errors.Add( new ValidationResult( "Language is required.", new[] {"Language"} ) );

            return errors;
        }
    }


    public class AdminCustomerIdentificationModel : CustomerBaseModel
    {
        [Required]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Must be between 6 and 8 characters long.")]
        [RegularExpression(@"(?=.*\d)(?=.*\W+)(?![.\n])(?=.*[A-Z])(?=.*[a-z]).*$", ErrorMessage = "Must contain 1: uppercase letter, number, special character")]
        public string DefaultPassword { get; set; }
    }


}