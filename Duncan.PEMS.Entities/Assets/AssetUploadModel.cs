/******************* CHANGE LOG ***********************************************************************************************************************
* DATE                 NAME                        DESCRIPTION
* ___________      ___________________        __________________________________________________________________________________________________
* 12/20/2013       Sergey Ostrerov                Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
* 
* *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace Duncan.PEMS.Entities.Assets
{
    public class UploadGatewayModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "Name", CanBeNull = true)]
        public string Name { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }

        [CsvColumn(Name = "Model", CanBeNull = true)]
        public string Model { get; set; }                   // [MechanismMaster]

        [CsvColumn(Name = "Street", CanBeNull = true)]
        public string Street { get; set; }

        [CsvColumn(Name = "Area", CanBeNull = true)]
        public string Area { get; set; }                    // [Areas]

        [CsvColumn(Name = "Zone", CanBeNull = true)]
        public string Zone { get; set; }                    // [Zones]

        [CsvColumn(Name = "Suburb", CanBeNull = true)]
        public string Suburb { get; set; }                  // [CustomGroup1]

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string State { get; set; }                   // [AssetState]

        [CsvColumn(Name = "DemandZone", CanBeNull = true)]
        public string DemandZone { get; set; }              // [DemandZone]

        [CsvColumn(Name = "Latitude", CanBeNull = true)]
        public float? Latitude { get; set; }

        [CsvColumn(Name = "Longitude", CanBeNull = true)]
        public float? Longitude { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? InstallDate { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatus { get; set; }       // [OperationalStatus]

        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusTime { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? LastPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? NextPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? WarrantyExpiration { get; set; }

        [CsvColumn(Name = "HardwareVersion", CanBeNull = true)]
        public string HardwareVersion { get; set; }

        [CsvColumn(Name = "SoftwareVersion", CanBeNull = true)]
        public string SoftwareVersion { get; set; }

        [CsvColumn(Name = "FirmwareVersion", CanBeNull = true)]
        public string FirmwareVersion { get; set; }

    }

    public class UploadSensorModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "Name", CanBeNull = true)]
        public string Name { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }

        [CsvColumn(Name = "Model", CanBeNull = true)]
        public string Model { get; set; }                   // [MechanismMaster]

        [CsvColumn(Name = "Street", CanBeNull = true)]
        public string Street { get; set; }

        [CsvColumn(Name = "Area", CanBeNull = true)]
        public string Area { get; set; }                    // [Areas]

        [CsvColumn(Name = "Zone", CanBeNull = true)]
        public string Zone { get; set; }                    // [Zones]

        [CsvColumn(Name = "Suburb", CanBeNull = true)]
        public string Suburb { get; set; }                  // [CustomGroup1]

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string State { get; set; }                   // [AssetState]

        [CsvColumn(Name = "DemandZone", CanBeNull = true)]
        public string DemandZone { get; set; }              // [DemandZone]

        [CsvColumn(Name = "Latitude", CanBeNull = true)]
        public float? Latitude { get; set; }

        [CsvColumn(Name = "Longitude", CanBeNull = true)]
        public float? Longitude { get; set; }

        [CsvColumn(CanBeNull = true)]
        public int? PrimaryGateway { get; set; }            // [Gateways]

        [CsvColumn(Name = "PrimaryGatewayState", CanBeNull = true)]
        public string PrimaryGatewayState { get; set; }                   // [AssetState]

        [CsvColumn(CanBeNull = true)]
        public int? SecondaryGateway { get; set; }          // [Gateways]

        [CsvColumn(Name = "SecondaryGatewayState", CanBeNull = true)]
        public string SecondaryGatewayState { get; set; }                   // [AssetState]

        [CsvColumn(CanBeNull = true)]
        public int? AssociatedMeter { get; set; }           // [Meters]

        [CsvColumn(CanBeNull = true)]
        public int? AssociatedMeterAreaId { get; set; }     // [Meters]

        [CsvColumn(CanBeNull = true)]
        public long? AssociatedSpace { get; set; }          // [ParkingSpaces]

        [CsvColumn(CanBeNull = true)]
        public DateTime? InstallDate { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatus { get; set; }       // [OperationalStatus]

        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusTime { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? LastPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? NextPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? WarrantyExpiration { get; set; }

        [CsvColumn(Name = "CommunicationsVersion", CanBeNull = true)]
        public string CommunicationsVersion { get; set; }

        [CsvColumn(Name = "SoftwareVersion", CanBeNull = true)]
        public string SoftwareVersion { get; set; }

        [CsvColumn(Name = "FirmwareVersion", CanBeNull = true)]
        public string FirmwareVersion { get; set; }
    }

    public class UploadSingleSpaceMeterModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }

        [CsvColumn(Name = "Name", CanBeNull = true)]
        public string Name { get; set; }

        [CsvColumn(Name = "Model", CanBeNull = true)]
        public string Model { get; set; }                   // [MechanismMaster]

        [CsvColumn(Name = "Street", CanBeNull = true)]
        public string Street { get; set; }

        [CsvColumn(Name = "Area", CanBeNull = true)]
        public string Area { get; set; }                    // [Areas]

        [CsvColumn(Name = "Zone", CanBeNull = true)]
        public string Zone { get; set; }                    // [Zones]

        [CsvColumn(Name = "Suburb", CanBeNull = true)]
        public string Suburb { get; set; }                  // [CustomGroup1]

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string State { get; set; }                   // [AssetState]

        [CsvColumn(Name = "DemandZone", CanBeNull = true)]
        public string DemandZone { get; set; }              // [DemandZone]

        [CsvColumn(Name = "Latitude", CanBeNull = true)]
        public float? Latitude { get; set; }

        [CsvColumn(Name = "Longitude", CanBeNull = true)]
        public float? Longitude { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string PhoneNumber { get; set; }

        [CsvColumn(CanBeNull = true)]
        public int? BayNumber { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? InstallDate { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatus { get; set; }       // [OperationalStatus]

        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusTime { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? LastPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? NextPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? WarrantyExpiration { get; set; }

        [CsvColumn(Name = "CommunicationsVersion", CanBeNull = true)]
        public string CommunicationsVersion { get; set; }

        [CsvColumn(Name = "SoftwareVersion", CanBeNull = true)]
        public string SoftwareVersion { get; set; }

        [CsvColumn(Name = "FirmwareVersion", CanBeNull = true)]
        public string FirmwareVersion { get; set; }


        [CsvColumn(Name = "LocationID", CanBeNull = true)]
        public string LocationID { get; set; }

        [CsvColumn(Name = "MechSerialNumber", CanBeNull = true)]
        public string MechSerialNumber { get; set; } 
    }

    public class UploadMultiSpaceMeterModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }

        [CsvColumn(Name = "Name", CanBeNull = true)]
        public string Name { get; set; }

        [CsvColumn(Name = "Model", CanBeNull = true)]
        public string Model { get; set; }                   // [MechanismMaster]

        [CsvColumn(Name = "Street", CanBeNull = true)]
        public string Street { get; set; }

        [CsvColumn(Name = "Area", CanBeNull = true)]
        public string Area { get; set; }                    // [Areas]

        [CsvColumn(Name = "Zone", CanBeNull = true)]
        public string Zone { get; set; }                    // [Zones]

        [CsvColumn(Name = "Suburb", CanBeNull = true)]
        public string Suburb { get; set; }                  // [CustomGroup1]

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string State { get; set; }                   // [AssetState]

        [CsvColumn(Name = "DemandZone", CanBeNull = true)]
        public string DemandZone { get; set; }              // [DemandZone]

        [CsvColumn(Name = "Latitude", CanBeNull = true)]
        public float? Latitude { get; set; }

        [CsvColumn(Name = "Longitude", CanBeNull = true)]
        public float? Longitude { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string PhoneNumber { get; set; }

        [CsvColumn(CanBeNull = true)]
        public int? NumberOfBays { get; set; }

        [CsvColumn(CanBeNull = true)]
        public int? BayStart { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? InstallDate { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatus { get; set; }       // [OperationalStatus]

        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusTime { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? LastPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? NextPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? WarrantyExpiration { get; set; }

        [CsvColumn(Name = "CommunicationsVersion", CanBeNull = true)]
        public string CommunicationsVersion { get; set; }

        [CsvColumn(Name = "SoftwareVersion", CanBeNull = true)]
        public string SoftwareVersion { get; set; }

        [CsvColumn(Name = "FirmwareVersion", CanBeNull = true)]
        public string FirmwareVersion { get; set; }
    }

    public class UploadCashboxModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }


        [CsvColumn(Name = "Name", CanBeNull = true)]
        public string CashBoxName { get; set; }


        [CsvColumn(Name = "Model", CanBeNull = true)]
        public string CashBoxModel { get; set; }            // [MechanismMaster]

        [CsvColumn(Name = "Location", CanBeNull = true)]
        public string CashBoxLocationType { get; set; }     // [CashboxLocationType]

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string CashBoxState { get; set; }            // [AssetState]

        [CsvColumn(Name = "Sequence", CanBeNull = true)]
        public int? CashBoxSeq { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? InstallDate { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatus { get; set; }       // [OperationalStatus]

        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusTime { get; set; }
        
        [CsvColumn(CanBeNull = true)]
        public DateTime? LastPreventativeMaintenance   { get; set; }
        
        [CsvColumn(CanBeNull = true)]
        public DateTime? NextPreventativeMaintenance   { get; set; }
        
        [CsvColumn(CanBeNull = true)]
        public DateTime? WarrantyExpiration     { get; set; }
        
        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusEndTime { get; set; }

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatusComment { get; set; }
    }

    public class UploadSpaceModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }

        [CsvColumn(Name = "BayNumber", CanBeNull = true)]
        public int BayNumber { get; set; } 

        [CsvColumn(Name = "BayName", CanBeNull = true)]
        public string BayName { get; set; }

        [CsvColumn(Name = "Latitude", CanBeNull = true)]
        public float? Latitude { get; set; }

        [CsvColumn(Name = "Longitude", CanBeNull = true)]
        public float? Longitude { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string State { get; set; }                   // [AssetState]   ParkingSpaces.SpaceStatus

        [CsvColumn(Name = "DemandZone", CanBeNull = true)]
        public string DemandZone { get; set; }              // [DemandZone]

        [CsvColumn(CanBeNull = true)]
        public int? AssociatedMeter { get; set; }           // [Meters]

        [CsvColumn(CanBeNull = true)]
        public int? AssociatedMeterAreaId { get; set; }     // [Meters]

        [CsvColumn(CanBeNull = true)]
        public int? AssociatedSensor { get; set; }           // [Sensors]

        [CsvColumn(CanBeNull = true)]
        public string OperationalStatus { get; set; }   // [OperationalStatus]

        [CsvColumn(CanBeNull = true)]
        public DateTime? OperationalStatusTime { get; set; }

        [CsvColumn(Name = "ParkingSpaceType", CanBeNull = true)]
        public string ParkingSpaceType { get; set; }              // [MeterGroup]
    }

    //todo - GTC: Mechanism Work
    //flesh out this object with the valid mechanism properties
    public class UploadMechanismModel
    {

        public int Id { get; set; }

        [CsvColumn(Name = "Name", CanBeNull = true)]
        public string Name { get; set; }

        [CsvColumn(Name = "LineNumber", CanBeNull = true)]
        public int LineNumber { get; set; }

        [CsvColumn(Name = "Model", CanBeNull = true)]
        public string Model { get; set; }

        [CsvColumn(Name = "SerialNumber", CanBeNull = true)]
        public string SerialNumber { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? ActivateDate { get; set; }

        [CsvColumn(Name = "State", CanBeNull = true)]
        public string State { get; set; }

         [CsvColumn(Name = "OperationalStatus", CanBeNull = true)]
        public string OperationalStatus { get; set; }   // [OperationalStatus]

         [CsvColumn(CanBeNull = true)]
         public DateTime? OperationalStatusTime { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? InstallDate { get; set; }
       
        [CsvColumn(CanBeNull = true)]
        public DateTime? LastPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? NextPreventativeMaintenance { get; set; }

        [CsvColumn(CanBeNull = true)]
        public DateTime? WarrantyExpiration { get; set; }
    }

    //todo - GTC: Datakey Work
    //flesh out this object with the valid datakey properties
    public class UploadDatakeyModel
    {
        public int Id { get; set; }

    }

    #region PAMConfiguration
    public class BulkUploadPAMConfigModel
    {
        public int SNo { get; set; }
        public int MeterID { get; set; }

        //[CsvColumn(Name = "Name", CanBeNull = true)]
        public string Description { get; set; }

    }
    #endregion

}
