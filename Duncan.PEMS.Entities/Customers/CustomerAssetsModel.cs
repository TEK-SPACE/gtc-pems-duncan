using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerAssetsModel : CustomerBaseModel, IValidatableObject
    {
        public const string NameChkBoxPrefix = "CHK";
        public const string NameSlaHoursPrefix = "MSLAH";
        public const string NameSlaDaysPrefix = "MSLAD";
        public const string NamePmsPrefix = "PMS";
        public char Separator = '_';

        public CustomerAssetsModel()
        {
            AssetGroups = new List<CustomerAssetGroupModel>();
        }

        public List<CustomerAssetGroupModel> AssetGroups { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            foreach (var customerAssetGroupModel in AssetGroups)
            {
                foreach (var customerAssetTypeModel in customerAssetGroupModel.AssetTypes)
                {
                    errors.AddRange( customerAssetTypeModel.Validate( validationContext ) );
                }
            }
            return errors;
        }

        public CustomerAssetGroupModel GetGroup(int id)
        {
            foreach (var customerAssetGroupModel in AssetGroups)
            {
                if ( customerAssetGroupModel.GroupId == id )
                    return customerAssetGroupModel;
            }
            return null;
        }

        public void DisableAssets()
        {
            foreach (var customerAssetGroupModel in AssetGroups)
            {
                customerAssetGroupModel.DisableAssets();
            }
        }
    }

    /// <summary>
    ///     This class has no analog.  It is used to group asset types under displayable headings.
    ///     For instance, multi-space and single-space meters would go under a heading of meters.
    /// </summary>
    public class CustomerAssetGroupModel
    {
        public CustomerAssetGroupModel()
        {
            AssetTypes = new List<CustomerAssetTypeModel>();
        }

        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int GroupId { get; set; }

        public List<CustomerAssetTypeModel> AssetTypes { get; set; }

        public CustomerAssetTypeModel GetAssetType(int assetTypeId)
        {
            foreach (var customerAssetTypeModel in AssetTypes)
            {
                if ( customerAssetTypeModel.Id == assetTypeId )
                    return customerAssetTypeModel;
            }
            return null;
        }

        public void DisableAssets()
        {
            foreach (var customerAssetTypeModel in AssetTypes)
            {
                customerAssetTypeModel.DisableAssets();
            }
        }
    }

    /// <summary>
    ///     This class is an analog to MeterGroup/AssetType
    /// </summary>
    public class CustomerAssetTypeModel : IValidatableObject
    {
        public const string UiCode = "AT";

        public CustomerAssetTypeModel()
        {
            Active = false;
            MaintenanceSlaHours = -1;
            MaintenanceSlaDays = -1;
            PreventativeMaintenanceSlaDays = -1;
            Assets = new List<CustomerAssetModel>();
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public bool Active { get; set; }
        public int MaintenanceSlaHours { get; set; }
        public int MaintenanceSlaDays { get; set; }
        public int PreventativeMaintenanceSlaDays { get; set; }

        public string SlaErrorField
        {
            get { return "SLA_" + Id; }
        }

        public List<CustomerAssetModel> Assets { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            //if (Active && ((MaintenanceSlaDays <= 0 && MaintenanceSlaHours <= 0) || (PreventativeMaintenanceSlaDays <= 0)))
            //{
            //    errors.Add(new ValidationResult("May not contain zero!", new string[] { SlaErrorField }));
            //}
            foreach (var customerAssetModel in Assets)
            {
                errors.AddRange( customerAssetModel.Validate( validationContext ) );
            }
            return errors;
        }

        public CustomerAssetModel GetAsset(int assetId)
        {
            foreach (var customerAssetModel in Assets)
            {
                if ( customerAssetModel.Id == assetId )
                    return customerAssetModel;
            }
            return null;
        }

        public void DisableAssets()
        {
            Active = false;
            foreach (var customerAssetModel in Assets)
            {
                customerAssetModel.Active = false;
            }
        }
    }

    /// <summary>
    ///     This class is analogous to the tables MechanismMaster/MechanismMasterCustomer
    /// </summary>
    public class CustomerAssetModel : IValidatableObject
    {
        public const string UiCode = "A";

        public CustomerAssetModel()
        {
            Active = false;
            MaintenanceSlaHours = -1;
            MaintenanceSlaDays = -1;
            PreventativeMaintenanceSlaDays = -1;
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int MechanismMasterId { get; set; }
        public string MechanismMasterName { get; set; }
        public bool Active { get; set; }
        public int MaintenanceSlaHours { get; set; }
        public int MaintenanceSlaDays { get; set; }
        public int PreventativeMaintenanceSlaDays { get; set; }

        public string SlaErrorField
        {
            get { return "ASLA_" + Id; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            //if (Active && ((MaintenanceSlaDays <= 0 && MaintenanceSlaHours <= 0)) || (PreventativeMaintenanceSlaDays <= 0))
            //{
            //    errors.Add(new ValidationResult("May not be zero!", new string[] { SlaErrorField }));
            //}
            return errors;
        }
    }
}