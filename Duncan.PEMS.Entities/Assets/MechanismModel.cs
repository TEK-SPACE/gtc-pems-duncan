using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{
    public class MechanismViewModel : AssetBaseModel
    {
        public const string DefaultType = "Mechanism";
        public string MechanismDesc { get; set; }
        public int? SLAMinutes { get; set; }
        public int? PreventativeMaintenanceScheduleDays { get; set; }
        public string SerialNumber { get; set; }
        public int? MechIdNumber { get; set; } //since mechid has auto increment, this is for user defined ids

        public string Location { get; set; }
        public DateTime? WarrantyExpiration { get; set; }
        public string WarrantyExpirationDisplay { get { return WarrantyExpiration.HasValue ? WarrantyExpiration.Value.ToShortDateString() : string.Empty; } }
        public DateTime? InstallationDate { get; set; }
        public string InstallationDateDisplay { get { return InstallationDate.HasValue ? InstallationDate.Value.ToShortDateString() : string.Empty; } }
        public AssetConfigurationModel Configuration { get; set; }
    }

    public class MechanismHistoryViewModel : MechanismViewModel
    {
        //todo - GTC: Mechanism work - add history model properties here
    }

  

    public class MechanismEditModel : AssetEditBaseModel, IValidatableObject
    {

        public string AssetModelName { get; set; } //** Sairam added on 21st sep 2014;
        public string AssetNameIs { get; set; } //** Sairam added on 21st sep 2014;

        public AssetConfigurationModel Configuration { get; set; }
        public string SerialNumber { get; set; }
        public string Location { get; set; }
        public int? MechIdNumber { get; set; } //since mechid has auto increment, this is for user defined ids

        public DateTime? InstallationDate { get; set; }
        public string InstallationDateDisplay { get { return InstallationDate.HasValue ? InstallationDate.Value.ToShortDateString() : string.Empty; } }
        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (AssetModelId < 0)
            {
                errors.Add(new ValidationResult("Model must be defined.", new[] { "AssetModelId" }));
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add(new ValidationResult("Name must be defined.", new[] { "Name" }));
            }
            if (string.IsNullOrWhiteSpace(SerialNumber))
            {
                errors.Add(new ValidationResult("SerialNumber must be defined.", new[] { "SerialNumber" }));
            }
            if (!ActivationDate.HasValue)
            {
                errors.Add(new ValidationResult("Activation date required.", new[] { "ActivationDate" }));
            }

            return errors;
        }
    }

    public class MechanismMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {
        public string SerialNumber { get; set; }
        public AssetConfigurationModel Configuration { get; set; }
        public string Location { get; set; }
        public DateTime? LastPrevMaint { get; set; }
        public DateTime? NextPrevMaint { get; set; }
        public DateTime? WarrantyExpiration { get; set; }
        public DateTime? InstallationDate { get; set; }
        public string InstallationDateDisplay { get { return InstallationDate.HasValue ? InstallationDate.Value.ToShortDateString() : string.Empty; } }
        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            errors.AddRange(base.Validate(validationContext));

            return errors;
        }
    }
}
