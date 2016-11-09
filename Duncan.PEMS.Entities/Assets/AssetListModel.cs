/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 01/08/2014       Sergey Ostrerov                 Issue: DPTXPEMS-137 - Exported files (pdf/csv/xl) match the grid but they all show up to decimal 5 digits only.     
 *                                                                        Changed StringLongitude/StringLatitude format from 5 to 6 digits. 
 * 
 * *******************************************************************************************************************************************************************/

using System;
using System.Reflection;
using Duncan.PEMS.Entities.General;
using System.Collections.Generic;

namespace Duncan.PEMS.Entities.Assets
{
    //base model, holds all the files for all the filters and grid rows

    //the only reason we need these models is so we can respect the custom columns grid orders / hidden columns in the export funcitons. diff positions for the same data, so have to do this
    public class ConfigurationAssetListModel:AssetListModel
    {
         [OriginalGridPosition(Position = 1)]
        public new long AssetId { get; set; }
         [OriginalGridPosition(Position = 2)]
         public new string AssetName { get; set; }
         [OriginalGridPosition(Position = 3)]
         public new string AssetModel { get; set; } // Note:  Don't change this to "Model".  Causes issues with MVC
         [OriginalGridPosition(Position = 4)]
         public new string DateInstalledDisplay { get { return DateInstalled.HasValue ? DateInstalled.Value.ToString("g") : string.Empty; } }
         [OriginalGridPosition(Position = 5)]
         public new long? ConfigurationId { get; set; }
         [OriginalGridPosition(Position = 6)]
         public new string ConfigCreationDateDisplay { get { return ConfigCreationDate.HasValue ? ConfigCreationDate.Value.ToString("g") : string.Empty; } }
         [OriginalGridPosition(Position = 7)]
         public new string ConfigScheduleDateDisplay { get { return ConfigScheduleDate.HasValue ? ConfigScheduleDate.Value.ToString("g") : string.Empty; } }
         [OriginalGridPosition(Position = 8)]
         public new string ConfigActivationDateDisplay { get { return ConfigActivationDate.HasValue ? ConfigActivationDate.Value.ToString("g") : string.Empty; } }
         [OriginalGridPosition(Position = 9)]
         public new string FirmwareVersion { get; set; }
         [OriginalGridPosition(Position = 10)]
         public new string SoftwareVersion { get; set; }
         [OriginalGridPosition(Position = 11)]
         public new string MpvVersion { get; set; }
    }

    public class FunctionalStatusAssetListModel : AssetListModel
    {
        [OriginalGridPosition(Position = 1)]
        public new long AssetId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public new string AssetName { get; set; }
        [OriginalGridPosition(Position = 3)]
        public new string AssetModel { get; set; } // Note:  Don't change this to "Model".  Causes issues with MVC
        [OriginalGridPosition(Position = 4)]
        public new string OperationalStatus { get; set; }
        [OriginalGridPosition(Position = 5)]
        public new string OperationalStatusDateDisplay { get { return OperationalStatusDate.HasValue ? OperationalStatusDate.Value.ToString("g") : string.Empty; } }
        [OriginalGridPosition(Position = 6)]
        public new string AlarmClass { get; set; }
        [OriginalGridPosition(Position = 7)]
        public new string AlarmCode { get; set; }
        [OriginalGridPosition(Position = 8)]
         public new string AlarmDuration { get
        {
            //TimeOfOccurrence- CurrentTIme  in minutes
            if (AlarmTimeOfOccurance.HasValue)
            {
                var timeSpan = (LocalTime - AlarmTimeOfOccurance.Value );
                    return String.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
            }
            return string.Empty;
        }}
        [OriginalGridPosition(Position = 9)]
        public new string AlarmRepairTargetTimeDisplay { get { return AlarmRepairTargetTime.HasValue ? AlarmRepairTargetTime.Value.ToString("g") : string.Empty; } }
    }
    
    public class OccupancyAssetListModel : AssetListModel
    {
        [OriginalGridPosition(Position = 1)]
        public new long AssetId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public new string AssetName { get; set; }
        [OriginalGridPosition(Position = 3)]
        public new string MeterName { get; set; }
        [OriginalGridPosition(Position = 4)]
        public new string SensorName { get; set; }
        [OriginalGridPosition(Position = 5)]
        public new string OperationalStatus { get; set; }
        [OriginalGridPosition(Position = 6)]
        public new  string OperationalStatusDateDisplay { get { return OperationalStatusDate.HasValue ? OperationalStatusDate.Value.ToString("g") : string.Empty; } }
        [OriginalGridPosition(Position = 7)]
        public new string OccupancyStatus { get; set; }
        [OriginalGridPosition(Position = 8)]
        public new string OccupancyStatusDateDisplay { get { return OccupancyStatusDate.HasValue ? OccupancyStatusDate.Value.ToString("g") : string.Empty; } }
        [OriginalGridPosition(Position = 9)]
        public new string NonComplianceStatus { get; set; }
        [OriginalGridPosition(Position = 10)]
        public new string NonComplianceStatusDateDisplay { get { return NonComplianceStatusDate.HasValue ? NonComplianceStatusDate.Value.ToString("g") : string.Empty; } }
    }

    public class SummaryAssetListModel : AssetListModel
    {
        [OriginalGridPosition(Position = 1)]
        public new long AssetId { get; set; }
        [OriginalGridPosition(Position = 2)]
        public new string AssetName { get; set; }
        [OriginalGridPosition(Position = 3)]
        public new string AssetModel { get; set; } // Note:  Don't change this to "Model".  Causes issues with MVC
        [OriginalGridPosition(Position = 4)]
        public new string Street { get; set; }

        

        [OriginalGridPosition(Position = 5)]
        public new string OperationalStatus { get; set; }
        [OriginalGridPosition(Position = 6)]
        public new string Area { get; set; }
        [OriginalGridPosition(Position = 7)]
        public new string Zone { get; set; }
        [OriginalGridPosition(Position = 8)]
        public new string Suburb { get; set; }
        [OriginalGridPosition(Position = 9)]
        public new string StringLatitude
        {
            get
        {
            if (Latitude.HasValue)
                return Latitude.Value.ToString("F6");
            return string.Empty;
        } }
        [OriginalGridPosition(Position = 10)]
        public new string StringLongitude
        {
            get
            {
                if (Longitude.HasValue)
                    return Longitude.Value.ToString("F6");
                return string.Empty;
            }
        }
        [OriginalGridPosition(Position = 11)]
        public new long? SpacesCount { get; set; }
        [OriginalGridPosition(Position = 12)]
        public new string DemandStatus { get; set; }

        [OriginalGridPosition(Position = 13)]
        public new string InventoryStatus { get; set; }

        [OriginalGridPosition(Position = 14)]
        public new string SpecialActionText { get; set; }

        [OriginalGridPosition(Position = 15)]
        public new string HasSensorText { get; set; }
    }

    public class AssetListModel
    {
        public long AssetId { get; set; }
        //Detail links
        public string Type { get; set; }
        public int AreaId { get; set; }
        public int CustomerId { get; set; }

        //Filterable items
        public string AssetType { get; set; }
     
        public string AssetName { get; set; }

        public string InventoryStatus { get; set; }
        public string OperationalStatus { get; set; }
        public double? Latitude { get; set; }
        public string StringLatitude { get
        {
            if (Latitude.HasValue)
                return Latitude.Value.ToString("F6");
            return string.Empty;
        } }
        public double? Longitude { get; set; }
        public string StringLongitude
        {
            get
            {
                if (Longitude.HasValue)
                    return Longitude.Value.ToString("F6");
                return string.Empty;
            }
        }
        public int? AreaId2 { get; set; }
        public int? ZoneId { get; set; }
        public string Suburb { get; set; }
        public string Street { get; set; }
        public string DemandStatus { get; set; }

        //common columns
        public string AssetModel { get; set; } // Note:  Don't change this to "Model".  Causes issues with MVC

        //summary
        public long? SpacesCount { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }

        //Configuration
        public DateTime? DateInstalled { get; set; }
        public string DateInstalledDisplay { get { return DateInstalled.HasValue ? DateInstalled.Value.ToString("g") : string.Empty; } }
        public long? ConfigurationId { get; set; }
        public DateTime? ConfigCreationDate { get; set; }
        public string ConfigCreationDateDisplay { get { return ConfigCreationDate.HasValue ? ConfigCreationDate.Value.ToString("g") : string.Empty; } }
        public DateTime? ConfigScheduleDate { get; set; }
        public string ConfigScheduleDateDisplay { get { return ConfigScheduleDate.HasValue ? ConfigScheduleDate.Value.ToString("g") : string.Empty; } }
        public DateTime? ConfigActivationDate { get; set; }
        public string ConfigActivationDateDisplay { get { return ConfigActivationDate.HasValue ? ConfigActivationDate.Value.ToString("g") : string.Empty; } }

        public string MpvVersion { get; set; }
        public string SoftwareVersion { get; set; }
        public string FirmwareVersion { get; set; }

        //Occupancy - only valid for spaces
        public string MeterName { get; set; }
        public string SensorName { get; set; }
        public DateTime? OperationalStatusDate { get; set; }
        public string OperationalStatusDateDisplay { get { return OperationalStatusDate.HasValue ? OperationalStatusDate.Value.ToString("g") : string.Empty; } }

        public string OccupancyStatus { get; set; }
        public DateTime? OccupancyStatusDate { get; set; }
        public string OccupancyStatusDateDisplay { get { return OccupancyStatusDate.HasValue ? OccupancyStatusDate.Value.ToString("g") : string.Empty; } }

        public string NonComplianceStatus { get; set; }
        public DateTime? NonComplianceStatusDate { get; set; }
        public string NonComplianceStatusDateDisplay { get { return NonComplianceStatusDate.HasValue ? NonComplianceStatusDate.Value.ToString("g") : string.Empty; } }

        //Functional Status
        public string AlarmClass { get; set; }
        public string AlarmCode { get; set; }
        public DateTime? AlarmTimeOfOccurance { get; set; }
        public string AlarmTimeOfOccuranceDisplay { get { return AlarmTimeOfOccurance.HasValue ? AlarmTimeOfOccurance.Value.ToString("g") : string.Empty; } }
        public DateTime? AlarmRepairTargetTime { get; set; }
        public string AlarmRepairTargetTimeDisplay { get { return AlarmRepairTargetTime.HasValue ? AlarmRepairTargetTime.Value.ToString("g") : string.Empty; } }
        public DateTime LocalTime { get; set; }
        public string AlarmDuration { get
        {
            //TimeOfOccurrence- CurrentTIme  in minutes
            if (AlarmTimeOfOccurance.HasValue)
            {
                var timeSpan = (LocalTime - AlarmTimeOfOccurance.Value );
                    return String.Format("{0:00}:{1:00}", timeSpan.Hours, timeSpan.Minutes);
            }
            return string.Empty;
        }}

        //display for kendo bigint isue
        public string DisplayAssetId { get { return AssetId.ToString(); } }

        //Specail Action
        public int? SpecialActionId { get; set; }
        public string SpecialActionText
        {
            get
            {
                string value = string.Empty;
                if( this.SpecialActionId.HasValue == false)
                    return value;
               
                switch (this.SpecialActionId.Value)
                {
                    case 1:
                        value = "Quick File Sync";
                        break;
                    case 241:
                        value = "Full Meter Reset(Adds Max Time)";
                        break;
                    case 242:
                        value = "Reset Peripheral Board";
                        break;
                    case 243:
                        value = "Reset Sensor";
                        break;
                    default:
                        break;
                }
                return value;
            }
        }

        public bool HasSensorId { get; set; }

        public string HasSensorText
        {
            get
            {
                string value = string.Empty;
                switch (this.AreaId)
                {
                    case 99:
                        value = "True";
                        break;
                    case 1:
                        if (this.HasSensorId == true)
                            value = "True";
                        else
                            value = "False";
                        break;
                    default:
                        break;
                }
               return value;
            }
        }

    }

    public class SpecialActions
    {
        public List<AssetTypeDDLModel> GetResetTypes()
        {
            var resetTypes = new List<AssetTypeDDLModel>();
            resetTypes.Add(new AssetTypeDDLModel() { Value = 1, Name =  "Quick File Sync"});
            resetTypes.Add(new AssetTypeDDLModel() { Value = 241, Name = "Full Meter Reset(Adds Max Time)" });
            resetTypes.Add(new AssetTypeDDLModel() { Value = 242, Name = "Reset Peripheral Board" });
            resetTypes.Add(new AssetTypeDDLModel() { Value = 243, Name = "Reset Sensor" });

            return resetTypes;
        }
    }


    public class HasSensor
    {
        public List<AssetTypeDDLModel> GetHasSensorTypes()
        {
            var hasSensorTypes = new List<AssetTypeDDLModel>();
            hasSensorTypes.Add(new AssetTypeDDLModel() { Value = 1, Name = "Has Sensor" });
            hasSensorTypes.Add(new AssetTypeDDLModel() { Value = 0, Name = "No Sensor" });
            return hasSensorTypes;
        }
    }

}
