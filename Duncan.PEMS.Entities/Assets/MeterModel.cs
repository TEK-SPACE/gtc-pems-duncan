using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{


    public class MeterViewModel : AssetBaseModel
    {
        public const string DefaultType = "METER";
        public string CollectionRoute { get; set; }
        public string WarrantyExpiration { get; set; }
        public int Spaces { get; set; }
        public string DemandStatus { get; set; }
        public AssetConfigurationModel Configuration { get; set; }
        public AssetAlarmConfigModel AlarmConfiguration { get; set; }
        public bool ViewOnly { get; set; }
        public long ActiveSpace { get; set; }
        public int ActiveSensor { get; set; }

        public string PhoneNumber { get; set; }
        public int? MechanismId { get; set; }
        public string MechanismName { get; set; }

        public string LocationName { get; set; }
        public string MechSerialNo { get; set; }
        public MeterViewModel()
        {
            ViewOnly = false;
        }
    }

    public class MeterEditModel : AssetEditBaseModel, IValidatableObject
    {
        public string CollectionRoute { get; set; }

        public int Spaces { get; set; }

        public string PhoneNumber { get; set; }

        public AssetConfigurationModel Configuration { get; set; }
        public int? MechanismId { get; set; }
        public string MechanismName { get; set; }




        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            errors.AddRange(base.Validate(validationContext));

            // NextPrevMaint
            if (NextPrevMaint != null && NextPrevMaint > DateTime.MinValue && NextPrevMaint < DateTime.Now)
            {
                errors.Add(new ValidationResult("Date must be in the future.", new[] { "NextPrevMaint" }));
            }

            // PhoneNumber
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                errors.Add(new ValidationResult("Phone Number is required.", new[] { "PhoneNumber" }));
            }

            return errors;
        }
    }

    public class MeterSpacesModel : AssetBaseModel
    {
        public MeterSpacesModel()
        {
            Spaces = new List<SpaceListModel>();
            ViewOnly = false;
        }

        public bool ViewOnly { get; set; }

        public List<SpaceListModel> Spaces { get; set; }
    }

    public class MeterMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {

        public MeterMassEditModel()
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

    public class MeterHistoryModel
    {
        public List<MeterViewModel> History { get; set; }
    }
}