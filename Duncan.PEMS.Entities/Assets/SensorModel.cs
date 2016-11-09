using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{
    public class SensorViewModel : AssetBaseModel
    {
        public const string DefaultType = "SENSOR";

        public AssetIdentifier MeterId { get; set; }
        public AssetIdentifier SpaceId { get; set; }
        public long ParkingSpaces { get; set; }
        public DateTime? WarrantyExpiration { get; set; }

        public string SpaceType { get; set; }

        public SensorConfigurationModel Configuration { get; set; }

        public AssetAlarmConfigModel AlarmConfiguration { get; set; }
    }

    public class SensorConfigurationModel : AssetConfigurationModel
    {
        public AssetIdentifier PrimaryGateway { get; set; }
        public AssetIdentifier SecondaryGateway { get; set; }
    }

    public class SensorConfigurationEditModel : AssetConfigurationModel
    {
        public List<SelectListItemWrapper> PrimaryGateway { get; set; }
        public int PrimaryGatewayId { get; set; }

        public List<SelectListItemWrapper> SecondaryGateway { get; set; }
        public int SecondaryGatewayId { get; set; }
    }



    public class SensorEditModel : AssetEditBaseModel, IValidatableObject
    {
        public List<SelectListItemWrapper> AssociatedMeter { get; set; }
        public int AssociatedMeterId { get; set; }

        public List<SelectListItemWrapper> AssociatedSpace { get; set; }
        public Int64 AssociatedSpaceId { get; set; }

        public SensorConfigurationEditModel Configuration { get; set; }

        public string SpaceType { get; set; }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            errors.AddRange( base.Validate( validationContext ) );

            //// Latitude
            //if (Latitude < -90.0 || Latitude > 90.0)
            //{
            //    errors.Add(new ValidationResult("Must be between -90.0 and 90.0", new string[] { "Latitude" }));
            //}

            return errors;
        }
    }

    public class SensorMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {

        public SensorMassEditModel()
        {
            Configuration = new SensorConfigurationEditModel();
        }

        public SensorConfigurationEditModel Configuration { get; set; }

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

    public class SensorHistoryModel
    {
        public List<SensorViewModel> History { get; set; } 
    }
}