using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{
    /// <summary>
    /// 
    /// </summary>
    public class DataKeyViewModel : AssetBaseModel
    {
        public const string DefaultType = "DataKey";
        public string DataKeyDesc { get; set; }
        public int DataKeyTypeId { get; set; }
        public int? StorageMBit { get; set; }
        public string CityCode { get; set; }
        public int? ColorCode { get; set; }
        public int? SLAMinutes { get; set; }
        public int? DataKeyIdNumber { get; set; } //since DataKeyId has auto increment, this is for user defined ids

        public int? PreventativeMaintenanceScheduleDays { get; set; }
        public string Location { get; set; }
        public DateTime? WarrantyExpiration { get; set; }
        public string WarrantyExpirationDisplay { get { return WarrantyExpiration.HasValue ? WarrantyExpiration.Value.ToShortDateString() : string.Empty; } }
        public DateTime? InstallationDate { get; set; }
        public string InstallationDateDisplay { get { return InstallationDate.HasValue ? InstallationDate.Value.ToShortDateString() : string.Empty; } }
        public AssetConfigurationModel Configuration { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataKeyHistoryViewModel : DataKeyViewModel
    {
        //todo - GTC: DataKey work - add history model properties here
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataKeyEditModel : AssetEditBaseModel, IValidatableObject
    {
        public AssetConfigurationModel Configuration { get; set; }
        public string Location { get; set; }

        public int? DataKeyIdNumber { get; set; } //since DataKeyId has auto increment, this is for user defined ids

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
            if (!ActivationDate.HasValue)
            {
                errors.Add(new ValidationResult("Activation date required.", new[] { "ActivationDate" }));
            }

            return errors;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataKeyMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {
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
