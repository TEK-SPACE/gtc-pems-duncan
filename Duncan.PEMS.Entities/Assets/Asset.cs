using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;


namespace Duncan.PEMS.Entities.Assets
{
    public class AssetTypeDDLModel
    {
        public string Name { get; set; }
        public int Value { get; set; }
        
    }

    public class AssetStatusEditModel
    {
        public List<SelectListItem> Status { get; set; }
        public int StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string StatusDateDisplay { get { return StatusDate.HasValue ? StatusDate.Value.ToString("g") : string.Empty; } }
    }

    public class AssetIdentifier
    {
        public int CustomerId { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public long AssetId { get; set; }
        public string AssetIdDisplay
        {
            get { return AssetId.ToString(); }
        }

        public string AssetIdFullDisplay
        {
            get
            {
                //try id (areaname) first
                if (!string.IsNullOrEmpty(AreaName))
                    return AssetId + " (" + AreaName + ")";
                
                else if ( AreaId > -1)
                    return AssetId + " (" + AreaId + ")";
                else
                    return AssetId.ToString();
            }
        }

        public string UniqueKey
        {
            get { return CustomerId + "|" + AreaId + "|" + AssetId; }
        }

        public string CustomerName { get; set; }

        public AssetIdentifier()
        {
            CustomerId = 0;
            AreaId = 0;
            AssetId = 0;
            AreaName = string.Empty;
        }

        public AssetIdentifier(AssetIdentifier assetIdentifier)
        {
            CustomerId = assetIdentifier.CustomerId;
            AreaId = assetIdentifier.AreaId;
            AssetId = assetIdentifier.AssetId;
            AreaName = assetIdentifier.AreaName;
        }
    }

    public class AssetBaseModel : AssetIdentifier
    {
        public AssetBaseModel()
        {
            State = AssetStateType.Pending;
        }

        public AssetBaseModel(AssetBaseModel model)
            : base(model)
        {
            Status = model.Status;
            StatusDate = model.StatusDate;
            LastUpdatedBy = model.LastUpdatedBy;
            LastUpdatedOn = model.LastUpdatedOn;
            LastUpdatedReason = model.LastUpdatedReason;

            State = model.State;

            GlobalId = model.GlobalId;
            Type = model.Type;
            AssetModel = model.AssetModel;
            Name = model.Name;

            Area = model.Area;
            AreaId2 = model.AreaId2;
            Street = model.Street;
            Zone = model.Zone;
            ZoneId = model.ZoneId;
            Suburb = model.Suburb;

            Latitude = model.Latitude;
            Longitude = model.Longitude;

            LastPrevMaint = model.LastPrevMaint;
            NextPrevMaint = model.NextPrevMaint;

            MaintenanceRoute = model.MaintenanceRoute;
        }

        public string ActivationDate { get; set; }

        // Status related fields
        public string Status { get; set; }
        public int StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string StatusDateDisplay { get { return StatusDate.HasValue ? StatusDate.Value.ToString("g") : string.Empty; } }
        public bool HasPendingChanges { get; set; }

        // Created By/On/Reason
        public int CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("g") : string.Empty; } }

        // LastUpdated By/On/Reason
        public int LastUpdatedById { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedOnDisplay { get { return LastUpdatedOn.HasValue ? LastUpdatedOn.Value.ToString("g") : string.Empty; } }
        public AssetPendingReasonType LastUpdatedReason { get; set; }
        public string LastUpdatedReasonDisplay { get; set; }

        // The pertinent date that this record was created.
        public DateTime? RecordDate { get; set; }
        public string RecordDateDisplay { get { return RecordDate.HasValue ? RecordDate.Value.ToString("g") : string.Empty; } }

        // The pertinent date that this record was superceeded by next update.  May be null.
        public DateTime? RecordSuperceededDate { get; set; }
        public string RecordSuperceededDateDisplay { get { return RecordSuperceededDate.HasValue ? RecordSuperceededDate.Value.ToString("g") : string.Empty; } }

        // if this model was created from an audit record, this will point to the 
        // associated audit table record.
        public Int64 AuditId { get; set; }
        public string AuditIdDisplay { get { return AuditId.ToString(); } }

        // Configuration name - This may represent data associated with a particular type of asset - i.e. Space -> TariffConfiguration name
        // Or in the case where this model reflects an audit record then the VersionProfileMeter data may be reflected here.
        // Generally this field will be blank.
        public string ConfigurationName { get; set; }

        /// <summary>
        /// This is the overall state of the asset in the system.  This
        /// is generally use to activate/inactive the asset.  This is not the same as the 
        /// operational status.
        /// </summary>
        public AssetStateType State { get; set; }


        public string GlobalId { get; set; }    //Kendo 
        public string Type { get; set; }        // MeterGroup.Description
        public int TypeId { get; set; }         // MeterGroup.MeterGroupId
      
        public string AssetModel { get; set; }  // Note:  Don't change this to "Model".  Causes issues with MVC
        public int AssetModelId { get; set; }
        public string Name { get; set; }

        public string Area { get; set; }
        public int? AreaId2 { get; set; }
        public string Street { get; set; }
        public string Zone { get; set; }
        public int? ZoneId { get; set; }
        public string Suburb { get; set; }
        public int? SuburbId { get; set; }
        public string Mechanism { get; set; }
        public int? MechanismId { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string LastPrevMaint { get; set; }
        public string NextPrevMaint { get; set; }

        public string MaintenanceRoute { get; set; }

    }

    public class AssetEditBaseModel : AssetIdentifier
    {
//        public AssetEditBaseModel()
//        {
////            Status = new AssetStatusEditModel();
//        }

        public DateTime? ActivationDate { get; set; }
        public int isNewAsset { get; set; }

        // Status related fields
        public string Status { get; set; }
        public int StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string StatusDateDisplay { get { return StatusDate.HasValue ? StatusDate.Value.ToString("g") : string.Empty; } }
        // These two values are only used when setting an asset from Inactive to Operational
        public string StatusOperational { get; set; }

        public string StatusInactive { get; set; }
        public int StatusOperationalId { get; set; }
        public int StatusInactiveId { get; set; }
        public string LocationName { get; set; }
        public string MechSerialNo { get; set; }
//        public AssetStatusEditModel Status { get; set; }

        /// <summary>
        /// This is the overall state of the asset in the system.  This
        /// is generally use to activate/inactive the asset.  This is not the same as the 
        /// operational status.
        /// </summary>
        public List<SelectListItemWrapper> State { get; set; }
        public int StateId { get; set; }

        public string GlobalId { get; set; }
        public string Type { get; set; }
        public MeterGroups TypeId { get; set; }

        public List<SelectListItemWrapper> AssetModel { get; set; } // Note:  Don't change this to "Model".  Causes issues with MVC
        public int AssetModelId { get; set; }

        public AssetIdentifier ClonedFromAsset { get; set; }

        public bool Saved { get; set; }

        public string Name { get; set; }

        public List<SelectListItemWrapper> Area { get; set; }
        public int AreaListId { get; set; }

        public string Street { get; set; }
        public List<SelectListItemWrapper> Zone { get; set; }
        public int ZoneId { get; set; }
        public List<SelectListItemWrapper> Suburb { get; set; }
        public int SuburbId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string LastPrevMaint { get; set; }
        public DateTime? NextPrevMaint { get; set; }

        public int MaintenanceRouteId { get; set; }
        public List<SelectListItemWrapper> MaintenanceRoute { get; set; }

        public DateTime? WarrantyExpiration { get; set; }

       

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // Latitude
            if ( Latitude < -90.0 || Latitude > 90.0 || Latitude == 0.0)
            {
                errors.Add( new ValidationResult( "Must be between -90.0 and 90.0 and non-zero", new[] {"Latitude"} ) );
            }

            // Longitude
            if ( Longitude < -180.0 || Longitude > 180.0 || Longitude == 0.0)
            {
                errors.Add(new ValidationResult("Must be between -180.0 and 180.0 and non-zero", new[] { "Longitude" }));
            }

            // Street
            if ( string.IsNullOrWhiteSpace( Street ) )
            {
                errors.Add(new ValidationResult("Street may not be blank", new[] { "Street" }));
            }

            // Area 
            if (AreaListId < 0)
            {
                errors.Add(new ValidationResult("Area required", new[] { "AreaListId" }));
            }

            // Zone
            if (ZoneId < 0)
            {
                errors.Add(new ValidationResult("Zone required", new[] { "ZoneId" }));
            }

            // Suburb
            if (SuburbId < 0)
            {
                errors.Add(new ValidationResult("Suburb required", new[] { "SuburbId" }));
            }

            // Asset Model
            if (AssetModelId < 0)
            {
                errors.Add(new ValidationResult("Asset Model required", new[] { "AssetModelId" }));
            }

            // Name
            if (string.IsNullOrWhiteSpace( Name ))
            {
                errors.Add(new ValidationResult("Name required", new[] { "Name" }));
            }

            // Activation Date
            if (!ActivationDate.HasValue)
            {
                errors.Add(new ValidationResult("Activation date required.", new[] { "ActivationDate" }));
            }
            // mech serial
            //if (string.IsNullOrWhiteSpace(MechSerialNo) && Type == "Single Space Meter")
            //{
            //    errors.Add(new ValidationResult("MechSerialNo may not be blank", new[] { "MechSerialNo" }));
            //}

            // mech serial
            if (string.IsNullOrWhiteSpace(LocationName) && Type == "Single Space Meter")
            {
                errors.Add(new ValidationResult("MechSerialNo may not be blank", new[] { "LocationName" }));
            }

            return errors;
        }


        public bool PropertyChanged(object otherObject, string propertyName)
        {
            bool propertyChanged = false;

            // Get out if one of the parameters is meaningless
            if ( otherObject == null || string.IsNullOrWhiteSpace( propertyName ) )
            {
                return false;
            }

            Type thisType = this.GetType();
            Type otherType = otherObject.GetType();

            if (thisType == otherType)
            {
                var propertyInfo = thisType.GetProperties().FirstOrDefault(p => p.Name == propertyName);

                if ( propertyInfo != null )
                {
                    var valueThis = propertyInfo.GetValue(this, null);
                    var valueOther = propertyInfo.GetValue(otherObject, null);

                    if ( valueThis != null && valueOther != null )
                    {

                        if ( !valueThis.Equals( valueOther ) )
                        {
                            propertyChanged = true;
                        }
                    }
                    else if ( valueThis == null && valueOther != null )
                    {
                        propertyChanged = true;
                    }
                    else if (valueThis != null && valueOther == null)
                    {
                        propertyChanged = true;
                    }
                }
            }

            return propertyChanged;
        }


    }

    public class AssetListItemModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class AssetMassUpdateBaseModel
    {
        private const char AssetListDivider = ':';
        private const char AssetDivider = '*';
        private const string AssetDisplayDivider = ", ";

        public int CustomerId { get; set; }

        public DateTime? ActivationDate { get; set; }

        public string AssetsDisplay { get; set; }


        /// <summary>
        /// This is the overall state of the asset in the system.  This
        /// is generally use to activate/inactive the asset.  This is not the same as the 
        /// operational status.
        /// </summary>
        public List<SelectListItemWrapper> State { get; set; }
        public int StateId { get; set; }

        /// <summary>
        /// This is a string that contains an encoded list of <see cref="AssetIdentifier"/> of the form
        ///     AI:AI:AI
        /// where 
        ///     AI = CustomerId*AreaId*AssetId
        /// </summary>
        public string TokenizableAssetsList { get; set; }

        public void SetList(List<AssetIdentifier> assets)
        {
            if (assets == null)
            {
                AssetsDisplay = "";
                TokenizableAssetsList = "";
            }
            else
            {
                StringBuilder sbAssetsDisplay = new StringBuilder();
                StringBuilder sbAssetsList = new StringBuilder();

                foreach (var assetIdentifier in assets)
                {
                    sbAssetsDisplay.Append(assetIdentifier.AssetId).Append(AssetDisplayDivider);
                    sbAssetsList
                        .Append(assetIdentifier.CustomerId).Append(AssetDivider)
                        .Append(assetIdentifier.AreaId).Append(AssetDivider)
                        .Append(assetIdentifier.AssetId).Append(AssetListDivider);
                }
                if (sbAssetsDisplay.Length >= AssetDisplayDivider.Length)
                    sbAssetsDisplay.Remove(sbAssetsDisplay.Length - AssetDisplayDivider.Length, AssetDisplayDivider.Length);
                AssetsDisplay = sbAssetsDisplay.ToString();

                if (sbAssetsList.Length >= AssetDisplayDivider.Length)
                    sbAssetsList.Remove(sbAssetsList.Length - 1, 1);
                TokenizableAssetsList = sbAssetsList.ToString();
            }
        }

        public List<AssetIdentifier> AssetIds()
        {
            List<AssetIdentifier> list = new List<AssetIdentifier>();
            if ( TokenizableAssetsList != null )
            {
                string[] tokens = TokenizableAssetsList.Split( AssetListDivider );
                foreach (var token in tokens)
                {
                    string[] assetIdentifierTokens = token.Split( AssetDivider );
                    if ( assetIdentifierTokens.Length == 3 )
                    {
                        AssetIdentifier assetId = new AssetIdentifier()
                            {
                                CustomerId = int.Parse(assetIdentifierTokens[0]),
                                AreaId = int.Parse(assetIdentifierTokens[1]),
                                AssetId = Int64.Parse(assetIdentifierTokens[2]),
                            };

                        list.Add(assetId);
                    }
                }
            }
            return list;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // Activation Date
            if (!ActivationDate.HasValue)
            {
                errors.Add(new ValidationResult("Activation date required.", new[] { "ActivationDate" }));
            }

            return errors;
        }
    
    }

    public class AssetConfigurationModel
    {
        public DateTime? DateInstalled { get; set; }
        public string DateInstalledDisplay { get { return DateInstalled.HasValue ? DateInstalled.Value.ToString("d") : string.Empty; } }

        public string MPVVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public string HardwareVersion { get; set; }
        public string FirmwareVersion { get; set; }

        public Int64 ConfigurationId { get; set; }
        public string ConfigurationName { get; set; }

        public AssetConfigurationModel()
        {
            
        }

        public AssetConfigurationModel(AssetConfigurationModel model)
        {
            DateInstalled = model.DateInstalled;
            MPVVersion = model.MPVVersion;
            SoftwareVersion = model.SoftwareVersion;
            HardwareVersion = model.HardwareVersion;
            FirmwareVersion = model.FirmwareVersion;
        }
    }

    public class AssetConfigurationEditModel
    {
        public List<SelectListItem> MPVVersion { get; set; }
        public int MPVVersionId { get; set; }
        public List<SelectListItem> SoftwareVersion { get; set; }
        public int SoftwareVersionId { get; set; }
        public List<SelectListItem> HardwareVersion { get; set; }
        public int HardwareVersionId { get; set; }
        public List<SelectListItem> FirmwareVersion { get; set; }
        public int FirmwareVersionId { get; set; }

        public void SetSelected(string text, VersionGroupType versionGroup)
        {
            switch (versionGroup)
            {
                case VersionGroupType.MPV:
                    MPVVersionId = SetSelectedItem( text, MPVVersion );
                    break;
                case VersionGroupType.Software:
                    SoftwareVersionId = SetSelectedItem( text, SoftwareVersion );
                    break;
                case VersionGroupType.Hardware:
                    HardwareVersionId = SetSelectedItem( text, HardwareVersion );
                    break;
                case VersionGroupType.Firmware:
                    FirmwareVersionId = SetSelectedItem( text, FirmwareVersion );
                    break;
            }
        }

        private int SetSelectedItem(string text, List<SelectListItem> list)
        {
            int selectedListItemId = 0;

            // Unselect all items.
            foreach (var selectListItem in list)
            {
                selectListItem.Selected = false;
            }

            var listItem = list.FirstOrDefault( m => m.Text.Equals( text, StringComparison.CurrentCultureIgnoreCase ) );
            if ( listItem == null )
            {
                list.Insert( list.Count > 0 ? 1 : 0, new SelectListItem {Selected = true, Text = text, Value = "0"} );
            }
            else
            {
                selectedListItemId = int.Parse( listItem.Value );
                listItem.Selected = true;
            }

            return selectedListItemId;
        }
    }

    public class AssetAlarmConfigModel
    {
        public string Class { get; set; }
        public string Code { get; set; }
        public string Duration { get; set; }
        public string RepairTargetTime { get; set; }

        public AssetAlarmConfigModel()
        {
            Class = " ";
            Code = " ";
            Duration = " ";
            RepairTargetTime = " ";
        }

        public AssetAlarmConfigModel(AssetAlarmConfigModel model)
        {
            Class = model.Class;
            Code = model.Code;
            Duration = model.Duration;
            RepairTargetTime = model.RepairTargetTime;
        }
    }

    public class AssetHistoryModel : AssetIdentifier
    {
        public string Type { get; set; }        // MeterGroup.Description
        public int TypeId { get; set; }         // MeterGroup.MeterGroupId
        public string Name { get; set; }
        public string Street { get; set; }
    }

    public class MeterCount
    {
        public int ActMeterCount { get; set; }
    }
}
