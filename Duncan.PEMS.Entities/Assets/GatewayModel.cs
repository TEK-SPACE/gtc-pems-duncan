using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{

  


    public class GatewayViewModel : AssetBaseModel
    {
        public const string DefaultType = "GATEWAY";

        public string WarrantyExpiration { get; set; }

        public GatewayConfigurationModel Configuration { get; set; }

        public AssetAlarmConfigModel AlarmConfiguration { get; set; }
    }

    public class GatewayConfigurationModel : AssetConfigurationModel
    {
        public string PowerSource { get; set; }
        public string GatewayModel { get; set; }
        public string Manufacturer { get; set; }
    }

    public class GatewayEditModel : AssetEditBaseModel, IValidatableObject
    {
        public AssetConfigurationModel Configuration { get; set; }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            errors.AddRange(base.Validate(validationContext));

            // NextPrevMaint
            if (NextPrevMaint != null && NextPrevMaint > DateTime.MinValue && NextPrevMaint < DateTime.Now)
            {
                errors.Add(new ValidationResult("Date must be in the future.", new[] { "NextPrevMaint" }));
            }

            // Activation Date
            if (!ActivationDate.HasValue)
            {
                errors.Add(new ValidationResult("Activation date required.", new[] { "ActivationDate" }));
            }

            return errors;
        }
    }

    public class GatewayMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {

        public GatewayMassEditModel()
        {
            Configuration = new AssetConfigurationModel();
        }

        public AssetConfigurationModel Configuration { get; set; }

        public List<SelectListItemWrapper> Area { get; set; }
        public int AreaListId { get; set; }

        public string Street { get; set; }
        public List<SelectListItemWrapper> Zone { get; set; }
        public int ZoneId { get; set; }
        public List<SelectListItemWrapper> Suburb { get; set; }
        public int SuburbId { get; set; }

        public DateTime? LastPrevMaint { get; set; }
        public DateTime? NextPrevMaint { get; set; }

        public int MaintenanceRouteId { get; set; }
        public List<SelectListItemWrapper> MaintenanceRoute { get; set; }

        public DateTime? WarrantyExpiration { get; set; }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            errors.AddRange(base.Validate(validationContext));

            // NextPrevMaint
            if (NextPrevMaint > DateTime.MinValue && NextPrevMaint < DateTime.Now)
            {
                errors.Add(new ValidationResult("Date must be in the future.", new[] { "NextPrevMaint" }));
            }

            // WarrantyExpiration
            if (WarrantyExpiration > DateTime.MinValue && WarrantyExpiration < DateTime.Now)
            {
                errors.Add(new ValidationResult("Date must be in the future.", new[] { "WarrantyExpiration" }));
            }

            return errors;
        }
    }


    public class GatewayHistoryModel
    {
        public List<GatewayViewModel> History { get; set; }
    }
}