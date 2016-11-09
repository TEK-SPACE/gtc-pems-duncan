using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{
    public class CashboxHistoryViewModel : CashboxViewModel
    {
        
    }


    public class CashboxViewModel : AssetBaseModel
    {
        public const string DefaultType = "CASHBOX";
        public string CashBoxName { get; set; }
        public string Location { get; set; }
        public string CurrentMeterName { get; set; }

        public string WarrantyExpiration { get; set; }

        public string InstallDate { get; set; }

        public string LastMeter
        {
            get
            {
                return LastMeterId > 0 ? LastMeterId.ToString() : "";
            }
        }

        public int? LastMeterId { get ; set; }
        public string LastCollected { get; set; }
        public double? LastCollectedValue { get; set; }
    }

    public class CashboxEditModel : AssetEditBaseModel, IValidatableObject
    {
        public List<SelectListItemWrapper> Location { get; set; }
        public int LocationId { get; set; }


        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // LocationId
            if (LocationId < 0)
            {
                errors.Add(new ValidationResult("Location must be defined.", new[] { "LocationId" }));
            }

            // Model
            if (AssetModelId < 0)
            {
                errors.Add(new ValidationResult("Model must be defined.", new[] { "AssetModelId" }));
            }

            // Name
            if ( string.IsNullOrWhiteSpace( Name ) )
            {
                errors.Add(new ValidationResult("Name must be defined.", new[] { "Name" }));
            }

            // Activation Date
            if (!ActivationDate.HasValue)
            {
                errors.Add(new ValidationResult("Activation date required.", new[] { "ActivationDate" }));
            }

            return errors;
        }


    }

    public class CashboxMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {
        public List<SelectListItemWrapper> Location { get; set; }
        public int LocationId { get; set; }

        public DateTime? LastPrevMaint { get; set; }
        public DateTime? NextPrevMaint { get; set; }

        public DateTime? WarrantyExpiration { get; set; }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            errors.AddRange(base.Validate(validationContext));

            return errors;
        }
    }
}