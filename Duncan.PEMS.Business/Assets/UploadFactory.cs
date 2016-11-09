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
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using LINQtoCSV;
using NLog;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Business.Customers;


namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// Class encapsulating functionality for the upload of bulk asset data and the associated processing.
    /// Also provides the directions and sample files for asset bulk upload.
    /// </summary>
    public class UploadFactory : AssetFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Upload Factory constructor taking a connection string name.  Used to handle all of the 
        /// asset import functionality.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        /// <param name="customerNow">
        /// <see cref="DateTime"/> indication of 'now' for the factory.  Generally used to ensure that
        /// the factory is set to the customer's timezone-adjusted 'now'.
        /// </param>
        public UploadFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }

        private CsvContext _csvContext;
        private CsvFileDescription _csvFileDescription;

        #region Asset Upload Processing

        /// <summary>
        /// Process a csv file for bulk import of assets.  This is the main method for processing
        /// bulk asset imports.  This method makes an instance of the csv reader context and calls the appropriate
        /// asset processing chain.
        /// </summary>
        /// <param name="customerId">Id of target customer</param>
        /// <param name="assetType"><see cref="AssetTypeModel.AssetType"/> type represented in the csv file</param>
        /// <param name="file">Fully-qualified file name and path of csv file</param>
        /// <returns><see cref="AssetsUploadResultsModel"/> instance which contains the results of the csv processing</returns>
        public AssetsUploadResultsModel Process(int customerId, AssetTypeModel.AssetType assetType, string file)
        {

            AssetsUploadResultsModel model = new AssetsUploadResultsModel()
            {
                CustomerId = customerId,
                UploadedFileName = file
            };

            // Prep the LINQtoCSV context.

            _csvContext = new CsvContext();
            _csvFileDescription = new CsvFileDescription()
            {
                MaximumNbrExceptions = 100,
                SeparatorChar = ','
            };

            try
            {
                switch (assetType)
                {
                    case AssetTypeModel.AssetType.Cashbox:
                        ProcessCashbox(model);
                        break;
                    case AssetTypeModel.AssetType.Gateway:
                        ProcessGateway(model);
                        break;
                    case AssetTypeModel.AssetType.MultiSpaceMeter:
                        ProcessMultiSpaceMeter(model);
                        break;
                    case AssetTypeModel.AssetType.Sensor:
                        ProcessSensor(model);
                        break;
                    case AssetTypeModel.AssetType.SingleSpaceMeter:
                        ProcessSingleSpaceMeter(model);
                        break;
                    case AssetTypeModel.AssetType.Smartcard:
                        model.Errors.Add("Smartcard is presently unsupported.");
                        break;
                    case AssetTypeModel.AssetType.ParkingSpaces: //** Sairam added this on oct 1st 2014
                        ProcessSpace(model);
                        break;
                    //todo - GTC: Mechanism work
                    case AssetTypeModel.AssetType.Mechanism:
                        ProcessMechanism(model);
                        break;
                    //todo - GTC: Datakey work
                    case AssetTypeModel.AssetType.DataKey:
                        ProcessDatakey(model);
                        break;
                }
            }
            catch (AggregatedException ae)
            {
                // Process all exceptions generated while processing the file
                var innerExceptionsList =
                    (List<Exception>)ae.Data["InnerExceptionsList"];

                foreach (Exception e in innerExceptionsList)
                {
                    model.Errors.Add(e.Message);
                }

            }
            catch (Exception ex)
            {
                model.Errors.Add("General exception");
                model.Errors.Add(ex.Message);
            }


            return model;
        }


        private void ProcessGateway(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadGatewayModel> assets =
                _csvContext.Read<UploadGatewayModel>(model.UploadedFileName, _csvFileDescription);

            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == model.CustomerId);
            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));
            foreach (var asset in assets)
            {
                bool canCreateItem = true;
                if (asset.LineNumber != 0)
                {
                    bool res = AssetIDExistence
                        (
                        asset.LineNumber.ToString(),
                        model.CustomerId.ToString(),
                      (int)AssetAreaId.Gateway,
                        (int)MeterGroups.Gateway
                        );
                    if (res)
                    {
                        model.Errors.Add(string.Format("Record {0}, LineNumber '{1}' is already used.", asset.Id, asset.LineNumber));
                        canCreateItem = false;
                    }
                }
                // First, validate that certain data, if present, resolves to the 
                // associated referential table.

                // Model == [MechanismMaster]
                var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(
                    m => m.MechanismDesc.Equals(asset.Model, StringComparison.CurrentCultureIgnoreCase) && m.MeterGroupId == (int)MeterGroups.Gateway);
                if (mechanismMaster == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Model '{1}' is invalid.", asset.Id, asset.Model));
                    canCreateItem = false;
                }

                // Area == [Areas]
                var area = PemsEntities.Areas.FirstOrDefault(
                    m => m.AreaName.Equals(asset.Area, StringComparison.CurrentCultureIgnoreCase) && m.CustomerID == model.CustomerId);
                if (area == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Area '{1}' is invalid.", asset.Id, asset.Area));
                    canCreateItem = false;
                }

                // Zone == [Zones]
                var zone = PemsEntities.Zones.FirstOrDefault(
                    m => m.ZoneName.Equals(asset.Zone, StringComparison.CurrentCultureIgnoreCase) && m.customerID == model.CustomerId);
                if (zone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Zone '{1}' is invalid.", asset.Id, asset.Zone));
                    canCreateItem = false;
                }

                // Suburb == [CustomGroup1]
                var suburb = PemsEntities.CustomGroup1.FirstOrDefault(
                    m => m.DisplayName.Equals(asset.Suburb, StringComparison.CurrentCultureIgnoreCase) && m.CustomerId == model.CustomerId);
                if (suburb == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Suburb '{1}' is invalid.", asset.Id, asset.Suburb));
                    canCreateItem = false;
                }

                // State == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.State, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, State '{1}' is invalid.", asset.Id, asset.State));
                    canCreateItem = false;
                }

                // DemandZone == [DemandZone]
                DemandZone demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneDesc.Equals(asset.DemandZone, StringComparison.CurrentCultureIgnoreCase));
                if (demandZone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, DemandZone '{1}' is invalid.", asset.Id, asset.DemandZone));
                    canCreateItem = false;
                }

                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }

                // If a Gateway cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }
                Gateway gatewayProxy = null;
                if (asset.LineNumber == 0)
                {
                    gatewayProxy = (new GatewayFactory(ConnectionStringName, Now)).Create(model.CustomerId);
                    //meterProxy = (new MeterFactory(ConnectionStringName)).Create(model.CustomerId, true, bayNumber, bayNumber);
                }
                else
                {
                    //AssetTypeModel.EnterMode enterMode, int meterId
                    gatewayProxy = (new GatewayFactory(ConnectionStringName, Now)).Create(model.CustomerId, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber);
                    //gatewayProxy = (new CashboxFactory(ConnectionStringName)).Create(model.CustomerId, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber);

                }
                    // Now should be able to create a Gateway
                    // For each asset, create a new Gateway
                    //Gateway gatewayProxy = (new GatewayFactory(ConnectionStringName, Now)).Create(model.CustomerId);

                int OperationalStatus;
                int State;
               

                    var gateway = PemsEntities.Gateways.FirstOrDefault(m => m.GateWayID == gatewayProxy.GateWayID);
                    gateway.OperationalStatus = (int)OperationalStatusType.Inactive;
                    if (assetState.AssetStateId == (int)AssetStateType.Current || assetState.AssetStateId == (int)AssetStateType.Historic)
                        OperationalStatus = (int)OperationalStatusType.Operational;
                    else
                        OperationalStatus = (int)OperationalStatusType.Inactive;


                    gateway.Description = asset.Name;
                    gateway.GatewayModel = mechanismMaster.MechanismId;
                    gateway.Location = asset.Street;
                    State = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                    gateway.GatewayState = (int)AssetStateType.Pending;
                  //  gateway.GatewayState = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                   // gateway.OperationalStatus = operationalStatus == null ? (int?)null : operationalStatus.OperationalStatusId;
                    gateway.OperationalStatusTime = DateTime.Now;//asset.OperationalStatusTime;
                    gateway.TimeZoneID = customerProfile.TimeZoneID;

                    gateway.DemandZone = demandZone.DemandZoneId;

                    gateway.LastPreventativeMaintenance = asset.LastPreventativeMaintenance;
                    gateway.NextPreventativeMaintenance = asset.NextPreventativeMaintenance;
                    gateway.InstallDateTime = asset.InstallDate;
                    gateway.WarrantyExpiration = asset.WarrantyExpiration;

                    gateway.Latitude = asset.Latitude;
                    gateway.Longitude = asset.Longitude;


                    var meterMap = (from m in PemsEntities.Meters
                                    join mm in PemsEntities.MeterMaps on m.MeterId equals mm.MeterId
                                    where m.MeterGroup == (int)MeterGroups.Gateway
                                    where m.CustomerID == model.CustomerId
                                    where m.AreaID == (int)AssetAreaId.Gateway
                                    where mm.GatewayID == gateway.GateWayID
                                    select mm).FirstOrDefault();

                    meterMap.ZoneId = zone.ZoneId;
                    meterMap.AreaId2 = area.AreaID;
                    meterMap.CustomGroup1 = suburb.CustomGroupId;


                    var versionProfileMeter = new VersionProfileMeter()
                    {
                        CustomerId = model.CustomerId,
                        GatewayID = gateway.GateWayID,
                        ConfigurationName = "",
                        HardwareVersion = asset.HardwareVersion,
                        SoftwareVersion = asset.SoftwareVersion,
                        MeterGroup = (int)MeterGroups.Gateway
                    };
                    PemsEntities.VersionProfileMeters.Add(versionProfileMeter);
                    PemsEntities.SaveChanges();

                    Audit(gateway);
                    Audit(meterMap);
                    Audit(versionProfileMeter);

                    // Add AssetPending record
                    (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Gateway,
                                                                               asset.ActivateDate,
                                                                               (AssetStateType)assetState.AssetStateId,
                                                                               model.CustomerId,
                                                                               gateway.GateWayID,
                                                                               State,OperationalStatus,
                                                                               (int)AssetAreaId.Gateway
                                                                               );

                    model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", asset.Id, asset.Name));
               
            }
        }

        private void ProcessSensor(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadSensorModel> assets =
                _csvContext.Read<UploadSensorModel>(model.UploadedFileName, _csvFileDescription);
            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));
            foreach (var asset in assets)
            {
                bool canCreateItem = true;
                // asset.LineNumber.ToString(), model.CustomerId.ToString(),"", (int)AssetAreaId.Gateway, (int)MeterGroups.Gateway
                if (asset.LineNumber != 0)
                {
                    bool res = AssetIDExistence
                        (
                        asset.LineNumber.ToString(),
                        model.CustomerId.ToString(),
                      (int)AssetAreaId.Sensor,
                        (int)MeterGroups.Sensor
                        );
                    if (res)
                    {
                        model.Errors.Add(string.Format("Record {0}, LineNumber '{1}' is already used.", asset.Id, asset.LineNumber));
                        canCreateItem = false;
                    }
                }
                // First, validate that certain data, if present, resolves to the 
                // associated referential table.

                // Model == [MechanismMaster]
                var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(
                    m => m.MechanismDesc.Equals(asset.Model, StringComparison.CurrentCultureIgnoreCase) && m.MeterGroupId == (int)MeterGroups.Sensor);
                if (mechanismMaster == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Model '{1}' is invalid.", asset.Id, asset.Model));
                    canCreateItem = false;
                }

                // Area == [Areas]
                var area = PemsEntities.Areas.FirstOrDefault(
                    m => m.AreaName.Equals(asset.Area, StringComparison.CurrentCultureIgnoreCase) && m.CustomerID == model.CustomerId);
                if (area == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Area '{1}' is invalid.", asset.Id, asset.Area));
                    canCreateItem = false;
                }

                // Zone == [Zones]
                var zone = PemsEntities.Zones.FirstOrDefault(
                    m => m.ZoneName.Equals(asset.Zone, StringComparison.CurrentCultureIgnoreCase) && m.customerID == model.CustomerId);
                if (zone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Zone '{1}' is invalid.", asset.Id, asset.Zone));
                    canCreateItem = false;
                }

                // Suburb == [CustomGroup1]
                var suburb = PemsEntities.CustomGroup1.FirstOrDefault(
                    m => m.DisplayName.Equals(asset.Suburb, StringComparison.CurrentCultureIgnoreCase) && m.CustomerId == model.CustomerId);
                if (suburb == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Suburb '{1}' is invalid.", asset.Id, asset.Suburb));
                    canCreateItem = false;
                }

                // State == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.State, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, State '{1}' is invalid.", asset.Id, asset.State));
                    canCreateItem = false;
                }

                // DemandZone == [DemandZone]
                DemandZone demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneDesc.Equals(asset.DemandZone, StringComparison.CurrentCultureIgnoreCase));
                if (demandZone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, DemandZone '{1}' is invalid.", asset.Id, asset.DemandZone));
                    canCreateItem = false;
                }

                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }

                // PrimaryGateway in [Gateways]
                Gateway primaryGateway = null;
                AssetState primaryGatewayState = null;
                if (asset.PrimaryGateway != null)
                {
                    primaryGateway = PemsEntities.Gateways.FirstOrDefault(m => m.GateWayID == asset.PrimaryGateway && m.CustomerID == model.CustomerId);
                    if (primaryGateway == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, PrimaryGateway '{1}' is invalid.", asset.Id, asset.PrimaryGateway));
                        canCreateItem = false;
                    }
                    // Has a PrimaryGatewayState also been given?
                    if (asset.PrimaryGatewayState == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, PrimaryGateway '{1}' requires a PrimaryGatewayState.", asset.Id, asset.PrimaryGateway));
                        canCreateItem = false;
                    }
                    // PrimaryGatewayState in [AssetState]
                    primaryGatewayState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.PrimaryGatewayState, StringComparison.CurrentCultureIgnoreCase));
                    if (primaryGatewayState == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, PrimaryGatewayState '{1}' is invalid.", asset.Id, asset.PrimaryGatewayState));
                        canCreateItem = false;
                    }
                }

                // SecondaryGateway in [Gateways]
                Gateway secondaryGateway = null;
                AssetState secondaryGatewayState = null;
                if (asset.SecondaryGateway != null)
                {
                    secondaryGateway = PemsEntities.Gateways.FirstOrDefault(m => m.GateWayID == asset.SecondaryGateway && m.CustomerID == model.CustomerId);
                    if (secondaryGateway == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, SecondaryGateway '{1}' is invalid.", asset.Id, asset.SecondaryGateway));
                        canCreateItem = false;
                    }
                    // Has a SecondaryGatewayState also been given?
                    if (asset.SecondaryGatewayState == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, SecondaryGateway '{1}' requires a SecondaryGatewayState.", asset.Id, asset.SecondaryGateway));
                        canCreateItem = false;
                    }
                    // SecondaryGatewayState in [AssetState]
                    secondaryGatewayState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.SecondaryGatewayState, StringComparison.CurrentCultureIgnoreCase));
                    if (secondaryGatewayState == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, SecondaryGatewayState '{1}' is invalid.", asset.Id, asset.SecondaryGatewayState));
                        canCreateItem = false;
                    }
                }

                // AssociatedMeter in [Meters]
                // First verify have both AssociatedMeter and AssociatedMeterAreaId
                if (asset.AssociatedMeter != null && asset.AssociatedMeterAreaId == null)
                {
                    model.Errors.Add(string.Format("Record {0}, AssociatedMeter '{1}' requires an AssociatedMeterAreaId.",
                        asset.Id, asset.AssociatedMeter));
                    canCreateItem = false;
                }

                Meter associatedMeter = null;
                if (asset.AssociatedMeter != null)
                {
                    associatedMeter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == asset.AssociatedMeter && m.CustomerID == model.CustomerId && m.AreaID == asset.AssociatedMeterAreaId);
                    if (associatedMeter == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, AssociatedMeter '{1}' and/or AssociatedMeterAreaId '{2}' are invalid.",
                            asset.Id, asset.AssociatedMeter, asset.AssociatedMeterAreaId));
                        canCreateItem = false;
                    }
                    //else
                    //{
                    //    // Does this meter already have a sensor?
                    //    var sensorMeterMap = associatedMeter.MeterMaps.FirstOrDefault(m => m.SensorID != null);
                    //    if (sensorMeterMap != null)
                    //    {
                    //        model.Errors.Add(string.Format("Record {0}, AssociatedMeter '{1}' already has a sensor.", asset.Id, asset.AssociatedMeter));
                    //        canCreateItem = false;
                    //    }
                    //}
                }


                // AssociatedSpace in [ParkingSpaces]
                ParkingSpace associatedSpace = null;
                if (asset.AssociatedSpace != null)
                {
                    associatedSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == asset.AssociatedSpace && m.CustomerID == model.CustomerId);
                    if (associatedSpace == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, AssociatedSpace '{1}' is invalid.",
                            asset.Id, asset.AssociatedSpace));
                        canCreateItem = false;
                    }
                    //else
                    //{
                    //    // Does space already have a sensor?
                    //    if (associatedSpace.HasSensor == true)
                    //    {
                    //        model.Errors.Add(string.Format("Record {0}, AssociatedSpace '{1}' already has a sensor.", asset.Id, asset.AssociatedSpace));
                    //        canCreateItem = false;
                    //    }
                    //}
                }
                else
                {
                    asset.AssociatedSpace = 0;
                }


                // If a Sensor cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }
                Sensor sensorProxy = null;
                if (asset.LineNumber == 0)
                {
                    sensorProxy = (new SensorFactory(ConnectionStringName, Now)).Create(model.CustomerId, 0, (Int64)asset.AssociatedSpace);
                    //meterProxy = (new MeterFactory(ConnectionStringName)).Create(model.CustomerId, true, bayNumber, bayNumber);
                }
                else
                {
                    //AssetTypeModel.EnterMode enterMode, int meterId
                    sensorProxy = (new SensorFactory(ConnectionStringName, Now)).Create(AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber, model.CustomerId, 0,(Int64) asset.AssociatedSpace);
                    //gatewayProxy = (new CashboxFactory(ConnectionStringName)).Create(model.CustomerId, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber);
                }

                // Now should be able to create a Sensor
                // For each asset, create a new Sensor
                //Sensor sensorProxy = (new SensorFactory(ConnectionStringName, Now)).Create(model.CustomerId);


                int OperationalStatus;
                int State;
                var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.SensorID == sensorProxy.SensorID && m.CustomerID == sensorProxy.CustomerID);

                sensor.OperationalStatus = (int)OperationalStatusType.Inactive;
                if (assetState.AssetStateId == (int)AssetStateType.Current || assetState.AssetStateId == (int)AssetStateType.Historic)
                    OperationalStatus = (int)OperationalStatusType.Operational;
                else
                    OperationalStatus = (int)OperationalStatusType.Inactive;

                sensor.SensorName = asset.Name;
                sensor.SensorModel = mechanismMaster.MechanismId;
                sensor.Location = asset.Street;
              //  sensor.OperationalStatus = operationalStatus == null ? (int?)null : operationalStatus.OperationalStatusId;
                sensor.OperationalStatusTime = DateTime.Now;//asset.OperationalStatusTime;
                State = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                sensor.SensorState =  (int)AssetStateType.Pending;
                //sensor.SensorState = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                sensor.DemandZone = demandZone.DemandZoneId;

                sensor.LastPreventativeMaintenance = asset.LastPreventativeMaintenance;
                sensor.NextPreventativeMaintenance = asset.NextPreventativeMaintenance;
                sensor.InstallDateTime = asset.InstallDate ?? sensor.InstallDateTime;
                sensor.WarrantyExpiration = asset.WarrantyExpiration;

                sensor.Latitude = asset.Latitude ?? sensor.Latitude;
                sensor.Longitude = asset.Longitude ?? sensor.Longitude;
                sensor.SensorType = (int)MeterGroups.Sensor;




                // Get Sensor's MeterMap
                var meterMap = sensor.MeterMaps.FirstOrDefault(m => m.SensorID == sensor.SensorID);

                meterMap.ZoneId = zone.ZoneId;
                meterMap.AreaId2 = area.AreaID;
                meterMap.CustomGroup1 = suburb.CustomGroupId;


                var versionProfileMeter = new VersionProfileMeter()
                {
                    CustomerId = model.CustomerId,
                    SensorID = sensor.SensorID,
                    ConfigurationName = "",
                    CommunicationVersion = asset.CommunicationsVersion,
                    HardwareVersion = asset.FirmwareVersion,
                    SoftwareVersion = asset.SoftwareVersion,
                    MeterGroup = (int)MeterGroups.Sensor
                };
                PemsEntities.VersionProfileMeters.Add(versionProfileMeter);
                PemsEntities.SaveChanges();

                 //Primary Gateway - Is there one?
                if (primaryGateway != null)
                {
                    SensorMapping sensorMapping = new SensorMapping()
                    {
                        CustomerID = model.CustomerId,
                        SensorID = sensor.SensorID,
                        ParkingSpaceID = asset.AssociatedSpace,
                        GatewayID = primaryGateway.GateWayID,
                        IsPrimaryGateway = true,
                        MappingState = primaryGatewayState.AssetStateId
                    };
                    PemsEntities.SensorMappings.Add(sensorMapping);
                    PemsEntities.SaveChanges();
                    Audit(sensorMapping);
                }

                // Secondary Gateway - Is there one?
                if (secondaryGateway != null)
                {
                    SensorMapping sensorMapping = new SensorMapping()
                    {
                        CustomerID = model.CustomerId,
                        SensorID = sensor.SensorID,
                        ParkingSpaceID = asset.AssociatedSpace,
                        GatewayID = secondaryGateway.GateWayID,
                        IsPrimaryGateway = false,
                        MappingState = secondaryGatewayState.AssetStateId
                    };
                    PemsEntities.SensorMappings.Add(sensorMapping);
                    PemsEntities.SaveChanges();
                    Audit(sensorMapping);
                }

                // Associated Space
                if (associatedSpace != null)
                {
                    sensor.ParkingSpaceId = associatedSpace.ParkingSpaceId;
                    associatedSpace.HasSensor = true;
                }

                 //Associated Meter
                //if (associatedMeter != null)
                //{
                //    var associatedMeterMap = associatedMeter.MeterMaps.FirstOrDefault(m => m.SensorID == null);
                //    associatedMeterMap.SensorID = sensor.SensorID;
                //    PemsEntities.SaveChanges();
                //    Audit(associatedMeterMap);
                //}

                // Save all the changes.
                PemsEntities.SaveChanges();

                // Write any audit records
                Audit(associatedSpace);
                Audit(associatedMeter);
                Audit(meterMap);
                Audit(sensor);
                Audit(versionProfileMeter);

                // Add AssetPending record
                (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Sensor,
                                                                           asset.ActivateDate,
                                                                           (AssetStateType)assetState.AssetStateId,
                                                                           model.CustomerId,
                                                                           sensor.SensorID,State,OperationalStatus,
                                                                           (int)AssetAreaId.Sensor);

                model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", asset.Id, asset.Name));

            }
        }

        private void ProcessSingleSpaceMeter(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadSingleSpaceMeterModel> assets =
                _csvContext.Read<UploadSingleSpaceMeterModel>(model.UploadedFileName, _csvFileDescription);
            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == model.CustomerId);
            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));
            foreach (var asset in assets)
            {
                bool canCreateItem = true;
                if (asset.LineNumber != 0)
                {
                    bool res = AssetIDExistence
                        (
                        asset.LineNumber.ToString(),
                        model.CustomerId.ToString(),
                      (int)AssetAreaId.Meter,
                        (int)MeterGroups.SingleSpaceMeter
                        );
                    if (res)
                    {
                        model.Errors.Add(string.Format("Record {0}, LineNumber '{1}' is already used.", asset.Id, asset.LineNumber));
                        canCreateItem = false;
                    }
                }

                 

                // First, validate that certain data, if present, resolves to the 
                // associated referential table.

                // Model == [MechanismMaster]
                var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(
                    m => m.MechanismDesc.Equals(asset.Model, StringComparison.CurrentCultureIgnoreCase) && m.MeterGroupId == (int)MeterGroups.SingleSpaceMeter);
                if (mechanismMaster == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Model '{1}' is invalid.", asset.Id, asset.Model));
                    canCreateItem = false;
                }

                // Area == [Areas]
                var area = PemsEntities.Areas.FirstOrDefault(
                    m => m.AreaName.Equals(asset.Area, StringComparison.CurrentCultureIgnoreCase) && m.CustomerID == model.CustomerId);
                if (area == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Area '{1}' is invalid.", asset.Id, asset.Area));
                    canCreateItem = false;
                }

                // Zone == [Zones]
                var zone = PemsEntities.Zones.FirstOrDefault(
                    m => m.ZoneName.Equals(asset.Zone, StringComparison.CurrentCultureIgnoreCase) && m.customerID == model.CustomerId);
                if (zone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Zone '{1}' is invalid.", asset.Id, asset.Zone));
                    canCreateItem = false;
                }

                // Suburb == [CustomGroup1]
                var suburb = PemsEntities.CustomGroup1.FirstOrDefault(
                    m => m.DisplayName.Equals(asset.Suburb, StringComparison.CurrentCultureIgnoreCase) && m.CustomerId == model.CustomerId);
                if (suburb == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Suburb '{1}' is invalid.", asset.Id, asset.Suburb));
                    canCreateItem = false;
                }

                // State == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.State, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, State '{1}' is invalid.", asset.Id, asset.State));
                    canCreateItem = false;
                }

                // DemandZone == [DemandZone]
                DemandZone demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneDesc.Equals(asset.DemandZone, StringComparison.CurrentCultureIgnoreCase));
                if (demandZone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, DemandZone '{1}' is invalid.", asset.Id, asset.DemandZone));
                    canCreateItem = false;
                }

                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }


                // LocationID == [LocationID]

                if (asset.LocationID == null || asset.LocationID == "")
                {
                    model.Errors.Add(string.Format("Record {0}, LocationID '{1}' is invalid.", asset.Id, asset.LocationID));
                    canCreateItem = false;
                }
                else
                {
                    var existingLocation = PemsEntities.HousingMasters.FirstOrDefault(x => x.Customerid == model.CustomerId && x.HousingName == asset.LocationID);
                    if (existingLocation != null)
                    {
                        model.Errors.Add(string.Format("Record {0}, LocationID '{1}' is already use.", asset.Id, asset.LocationID));
                        canCreateItem = false;
                    }
                   
                }

                // MechSerialNumber == [MechSerialNumber]
                if (asset.MechSerialNumber != null && asset.MechSerialNumber != "")
                {
                  //  var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.Customerid == model.CustomerId && x.MechSerial == asset.MechSerialNumber);
                    var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x =>  x.MechSerial == asset.MechSerialNumber);
                    if (existingMechanism == null)
                    {

                        model.Errors.Add(string.Format("Record {0}, MechSerialNumber '{1}' is invalid.", asset.Id, asset.MechSerialNumber));
                        canCreateItem = false;
                    }
                }

                // If a Meter cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }


                // Now should be able to create a Meter
                // For each asset, create a new Meter
                // Determine bay number
                int bayNumber = asset.BayNumber ?? 1;

                // AssetID change
                Meter meterProxy = null;
                if (asset.LineNumber == 0)
                {
                    meterProxy = (new MeterFactory(ConnectionStringName, Now)).Create(model.CustomerId, true, bayNumber, bayNumber, asset.MechSerialNumber, asset.LocationID);
                }
                else
                {
                    meterProxy = (new MeterFactory(ConnectionStringName, Now)).Create(model.CustomerId, true, bayNumber, bayNumber, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber, asset.MechSerialNumber, asset.LocationID);
                }

             
                //Meter meterProxy = (new MeterFactory(ConnectionStringName)).Create(model.CustomerId, true, bayNumber, bayNumber);

                var meter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == meterProxy.MeterId && m.CustomerID == meterProxy.CustomerID && m.AreaID == meterProxy.AreaID);
                int OperationalStatusID;
                int State;
                meter.OperationalStatusID = (int)OperationalStatusType.Inactive;
                if (assetState.AssetStateId == (int)AssetStateType.Current || assetState.AssetStateId == (int)AssetStateType.Historic)
                    OperationalStatusID = (int)OperationalStatusType.Operational;
                else
                    OperationalStatusID = (int)OperationalStatusType.Inactive;

                meter.MeterName = asset.Name;
                meter.TimeZoneID = customerProfile.TimeZoneID ?? 0;
                meter.SMSNumber = asset.PhoneNumber;
                meter.Location = asset.Street;
                meter.MeterType = mechanismMaster.MechanismId;
               // meter.OperationalStatusID = operationalStatus == null ? (int?)null : operationalStatus.OperationalStatusId;
                meter.OperationalStatusTime = DateTime.Now;//asset.OperationalStatusTime;
                State = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                meter.MeterState = (int)AssetStateType.Pending;
                //meter.MeterState = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                meter.DemandZone = demandZone.DemandZoneId;

                meter.LastPreventativeMaintenance = asset.LastPreventativeMaintenance;
                meter.NextPreventativeMaintenance = asset.NextPreventativeMaintenance;
                meter.InstallDate = asset.InstallDate;
                meter.WarrantyExpiration = asset.WarrantyExpiration;

                meter.Latitude = asset.Latitude;
                meter.Longitude = asset.Longitude;


                var meterMap = meter.MeterMaps.Where(m => m.GatewayID == null && m.CashBoxID == null && m.SensorID == null).FirstOrDefault();

                meterMap.ZoneId = zone.ZoneId;
                meterMap.AreaId2 = area.AreaID;
                meterMap.CustomGroup1 = suburb.CustomGroupId;


                var versionProfileMeter = new VersionProfileMeter()
                {
                    CustomerId = model.CustomerId,
                    MeterId = meter.MeterId,
                    AreaId = meter.AreaID,
                    ConfigurationName = "",
                    CommunicationVersion = asset.CommunicationsVersion,
                    HardwareVersion = asset.FirmwareVersion,
                    SoftwareVersion = asset.SoftwareVersion,
                    MeterGroup = (int)MeterGroups.SingleSpaceMeter
                };
                PemsEntities.VersionProfileMeters.Add(versionProfileMeter);
                PemsEntities.SaveChanges();

                Audit(meter);
                Audit(meterMap);
                Audit(versionProfileMeter);

                // Add AssetPending record
                (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.SingleSpaceMeter,
                                                                           asset.ActivateDate,
                                                                           (AssetStateType)assetState.AssetStateId,
                                                                           model.CustomerId,
                                                                           meter.MeterId,State,OperationalStatusID,
                                                                           meter.AreaID);

                var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.MeterId == meter.MeterId && m.CustomerId == model.CustomerId && m.AreaId == meter.AreaID);


                assetPending.MeterMapMechId = meterMap.MechId;
                assetPending.MeterMapHousingId = meterMap.HousingId;
                PemsEntities.SaveChanges();
               
                model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", asset.Id, asset.Name));
            }
        }

        private void ProcessMultiSpaceMeter(AssetsUploadResultsModel model)
        {

            IEnumerable<UploadMultiSpaceMeterModel> assets =
                _csvContext.Read<UploadMultiSpaceMeterModel>(model.UploadedFileName, _csvFileDescription);

            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == model.CustomerId);
            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));
            foreach (var asset in assets)
            {
                bool canCreateItem = true;
                if (asset.LineNumber != 0)
                {
                    bool res = AssetIDExistence
                        (
                        asset.LineNumber.ToString(),
                        model.CustomerId.ToString(),
                      (int)AssetAreaId.Meter,
                        (int)MeterGroups.MultiSpaceMeter
                        );
                    if (res)
                    {
                        model.Errors.Add(string.Format("Record {0}, LineNumber '{1}' is already used.", asset.Id, asset.LineNumber));
                        canCreateItem = false;
                    }
                }


                // First, validate that certain data, if present, resolves to the 
                // associated referential table.

                // Model == [MechanismMaster]
                var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(
                    m => m.MechanismDesc.Equals(asset.Model, StringComparison.CurrentCultureIgnoreCase) && m.MeterGroupId == (int)MeterGroups.MultiSpaceMeter);
                if (mechanismMaster == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Model '{1}' is invalid.", asset.Id, asset.Model));
                    canCreateItem = false;
                }

                // Area == [Areas]
                var area = PemsEntities.Areas.FirstOrDefault(
                    m => m.AreaName.Equals(asset.Area, StringComparison.CurrentCultureIgnoreCase) && m.CustomerID == model.CustomerId);
                if (area == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Area '{1}' is invalid.", asset.Id, asset.Area));
                    canCreateItem = false;
                }

                // Zone == [Zones]
                var zone = PemsEntities.Zones.FirstOrDefault(
                    m => m.ZoneName.Equals(asset.Zone, StringComparison.CurrentCultureIgnoreCase) && m.customerID == model.CustomerId);
                if (zone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Zone '{1}' is invalid.", asset.Id, asset.Zone));
                    canCreateItem = false;
                }

                // Suburb == [CustomGroup1]
                var suburb = PemsEntities.CustomGroup1.FirstOrDefault(
                    m => m.DisplayName.Equals(asset.Suburb, StringComparison.CurrentCultureIgnoreCase) && m.CustomerId == model.CustomerId);
                if (suburb == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Suburb '{1}' is invalid.", asset.Id, asset.Suburb));
                    canCreateItem = false;
                }

                // State == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.State, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                    canCreateItem = false;
                }

                // DemandZone == [DemandZone]
                DemandZone demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneDesc.Equals(asset.DemandZone, StringComparison.CurrentCultureIgnoreCase));
                if (demandZone == null)
                {
                    model.Errors.Add(string.Format("Record {0}, DemandZone '{1}' is invalid.", asset.Id, asset.DemandZone));
                    canCreateItem = false;
                }

                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }


                // Check if asset.BayStart or asset.NumberOfBays exists without the other.
                if (asset.NumberOfBays != null && asset.BayStart == null)
                {
                    model.Errors.Add(string.Format("Record {0}, NumberOfBays requires BayStart value.", asset.Id));
                    canCreateItem = false;
                }

                // If a Meter cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }


                // Now should be able to create a Meter
                // For each asset, create a new Meter

                // Determine bay start and end numbers
                int bayStart = asset.BayStart ?? 1;
                int bayEnd = bayStart + (asset.NumberOfBays ?? 1) - 1;
                //Meter meterProxy = (new MeterFactory(ConnectionStringName, Now)).Create(model.CustomerId, false, bayStart, bayEnd);

                // AssetID change
                Meter meterProxy = null;
                if (asset.LineNumber == 0)
                {
                    meterProxy = (new MeterFactory(ConnectionStringName, Now)).Create(model.CustomerId, false, bayStart, bayEnd, "", "");
                }
                else
                {
                    meterProxy = (new MeterFactory(ConnectionStringName, Now)).Create(model.CustomerId, false, bayStart, bayEnd, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber, "", "");
                }


                int OperationalStatusID;
                int State;
                var meter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == meterProxy.MeterId && m.CustomerID == meterProxy.CustomerID && m.AreaID == meterProxy.AreaID);
                meter.OperationalStatusID = (int)OperationalStatusType.Inactive;
                if (assetState.AssetStateId == (int)AssetStateType.Current || assetState.AssetStateId == (int)AssetStateType.Historic)
                    OperationalStatusID = (int)OperationalStatusType.Operational;
                else
                    OperationalStatusID = (int)OperationalStatusType.Inactive;
                meter.MeterName = asset.Name;
                meter.TimeZoneID = customerProfile.TimeZoneID ?? 0;
                meter.SMSNumber = asset.PhoneNumber;
                meter.Location = asset.Street;
                meter.MeterType = mechanismMaster.MechanismId;
                //  meter.OperationalStatusID = operationalStatus == null ? (int?)null : operationalStatus.OperationalStatusId;
                meter.OperationalStatusTime = DateTime.Now;//asset.OperationalStatusTime;
                State = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                meter.MeterState = (int)AssetStateType.Pending;
                //meter.MeterState = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;

                meter.DemandZone = demandZone.DemandZoneId;
                meter.LastPreventativeMaintenance = asset.LastPreventativeMaintenance;
                meter.NextPreventativeMaintenance = asset.NextPreventativeMaintenance;
                meter.InstallDate = asset.InstallDate;
                meter.WarrantyExpiration = asset.WarrantyExpiration;

                meter.Latitude = asset.Latitude;
                meter.Longitude = asset.Longitude;


                var meterMap = meter.MeterMaps.Where(m => m.GatewayID == null && m.CashBoxID == null && m.SensorID == null).FirstOrDefault();

                meterMap.ZoneId = zone.ZoneId;
                meterMap.AreaId2 = area.AreaID;
                meterMap.CustomGroup1 = suburb.CustomGroupId;


                var versionProfileMeter = new VersionProfileMeter()
                {
                    CustomerId = model.CustomerId,
                    MeterId = meter.MeterId,
                    AreaId = meter.AreaID,
                    ConfigurationName = "",
                    CommunicationVersion = asset.CommunicationsVersion,
                    HardwareVersion = asset.FirmwareVersion,
                    SoftwareVersion = asset.SoftwareVersion,
                    MeterGroup = (int)MeterGroups.SingleSpaceMeter
                };
                PemsEntities.VersionProfileMeters.Add(versionProfileMeter);
                PemsEntities.SaveChanges();

                Audit(meter);
                Audit(meterMap);
                Audit(versionProfileMeter);

                // Add AssetPending record
                (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.MultiSpaceMeter,
                                                                           asset.ActivateDate,
                                                                           (AssetStateType)assetState.AssetStateId,
                                                                           model.CustomerId,
                                                                           meter.MeterId, State,
                                                                           OperationalStatusID,
                                                                           meter.AreaID);

                model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", asset.Id, asset.Name));

            }
        }

        private void ProcessCashbox(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadCashboxModel> assets =
                _csvContext.Read<UploadCashboxModel>(model.UploadedFileName, _csvFileDescription);


            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));

            foreach (var asset in assets)
            {
                bool canCreateItem = true;

                // First, validate that certain data, if present, resolves to the 
                // associated referential table.


                if (asset.LineNumber != 0)
                {
                    bool res = AssetIDExistence
                        (
                        asset.LineNumber.ToString(),
                        model.CustomerId.ToString(),
                      (int)AssetAreaId.Cashbox,
                        (int)MeterGroups.Cashbox
                        );
                    if (res)
                    {
                        model.Errors.Add(string.Format("Record {0}, LineNumber '{1}' is already used.", asset.Id, asset.LineNumber));
                        canCreateItem = false;
                    }
                }

                // CashBoxModel == [MechanismMaster]
                var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(
                    m => m.MechanismDesc.Equals(asset.CashBoxModel, StringComparison.CurrentCultureIgnoreCase) && m.MeterGroupId == (int)MeterGroups.Cashbox);
                if (mechanismMaster == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Model '{1}' is invalid.", asset.Id, asset.CashBoxModel));
                    canCreateItem = false;
                }

                // CashBoxLocationType == [CashboxLocationType]
                var cashBoxLocationType =
                    PemsEntities.CashBoxLocationTypes.FirstOrDefault(
                        m => m.CashBoxLocationTypeDesc.Equals(asset.CashBoxLocationType, StringComparison.CurrentCultureIgnoreCase));
                if (cashBoxLocationType == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Location '{1}' is invalid.", asset.Id, asset.CashBoxLocationType));
                    canCreateItem = false;
                }

                // CashBoxState == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.CashBoxState, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, State '{1}' is invalid.", asset.Id, asset.CashBoxState));
                    canCreateItem = false;
                }


                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }

                // If a CashBox cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }


                // Now should be able to create a Cashbox
                // For each uploadCashboxModel, create a new CashBox

                // AssetID change
                CashBox cashboxProxy = null;
                if (asset.LineNumber == 0)
                {
                    cashboxProxy = (new CashboxFactory(ConnectionStringName, Now)).Create(model.CustomerId);
                    //meterProxy = (new MeterFactory(ConnectionStringName)).Create(model.CustomerId, true, bayNumber, bayNumber);
                }
                else
                {
                    cashboxProxy = (new CashboxFactory(ConnectionStringName, Now)).Create(model.CustomerId, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber);
                    //meterProxy = (new MeterFactory(ConnectionStringName)).Create(model.CustomerId, false, bayNumber, bayNumber, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber);
                }
                //CashBox cashboxProxy = (new CashboxFactory(ConnectionStringName)).Create(model.CustomerId);

                var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CashBoxID == cashboxProxy.CashBoxID);
                
                int OperationalStatus;
                cashbox.OperationalStatus = (int)OperationalStatusType.Inactive;

                 if (assetState.AssetStateId == (int)AssetStateType.Current || assetState.AssetStateId == (int)AssetStateType.Historic)
                     OperationalStatus = (int)OperationalStatusType.Operational;
                else
                     OperationalStatus = (int)OperationalStatusType.Inactive;
               
                cashbox.CashBoxName = asset.CashBoxName;
                cashbox.CashBoxModel = mechanismMaster.MechanismId;
                cashbox.CashBoxLocationTypeId = cashBoxLocationType.CashBoxLocationTypeId;
                cashbox.CashBoxState = (int)AssetStateType.Pending;
                int State = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
              //  cashbox.CashBoxState = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;

             //   cashbox.CashBoxSeq = asset.CashBoxSeq ?? 1;

                cashbox.InstallDate = asset.InstallDate;

              

                cashbox.OperationalStatusTime = DateTime.Now;//asset.OperationalStatusTime;

                cashbox.LastPreventativeMaintenance = asset.LastPreventativeMaintenance;
                cashbox.NextPreventativeMaintenance = asset.NextPreventativeMaintenance;
                cashbox.WarrantyExpiration = asset.WarrantyExpiration;
                cashbox.OperationalStatusEndTime = asset.OperationalStatusEndTime;
                cashbox.OperationalStatusComment = asset.OperationalStatusComment;

                PemsEntities.SaveChanges();            
			 
			  
                // Add AssetPending record
                (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Cashbox,
                                                                           asset.ActivateDate,
                                                                           (AssetStateType)assetState.AssetStateId,
                                                                           model.CustomerId,
                                                                           cashbox.CashBoxID,
                                                                           State,
                                                                           OperationalStatus, (int)AssetAreaId.Cashbox);

                model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", asset.Id, asset.CashBoxName));
            }
        }

        private void ProcessSpace(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadSpaceModel> assets =
                _csvContext.Read<UploadSpaceModel>(model.UploadedFileName, _csvFileDescription);
            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));
            foreach (var asset in assets)
            {
                bool canCreateItem = true;

                // First, validate that certain data, if present, resolves to the 
                // associated referential table.


                // State == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.State, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, State '{1}' is invalid.", asset.Id, asset.State));
                    canCreateItem = false;
                }

                // DemandZone == [DemandZone]
                if (asset.DemandZone != null)
                {
                    var demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneDesc.Equals(asset.DemandZone, StringComparison.CurrentCultureIgnoreCase));
                    if (demandZone == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, DemandZone '{1}' is invalid.", asset.Id, asset.DemandZone));
                        canCreateItem = false;
                    }
                }

                // AssociatedMeter == [Meters]
                // AssociatedMeterAreaId == [Meters]
                // First verify have both AssociatedMeter and AssociatedMeterAreaId
                if (asset.AssociatedMeter != null && asset.AssociatedMeterAreaId == null)
                {
                    model.Errors.Add(string.Format("Record {0}, AssociatedMeter '{1}' requires an AssociatedMeterAreaId.",
                        asset.Id, asset.AssociatedMeter));
                    canCreateItem = false;
                }

                Meter associatedMeter = null;
                if (asset.AssociatedMeter != null)
                {
                    associatedMeter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == asset.AssociatedMeter && m.CustomerID == model.CustomerId && m.AreaID == asset.AssociatedMeterAreaId);
                    if (associatedMeter == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, AssociatedMeter '{1}' and/or AssociatedMeterAreaId '{2}' are invalid.",
                            asset.Id, asset.AssociatedMeter, asset.AssociatedMeterAreaId));
                        canCreateItem = false;
                    }

                    // Is this meter a single-space or a multi-space?
                    if (associatedMeter.MeterGroup == (int)MeterGroups.SingleSpaceMeter)
                    {
                        // Does this meter already have an associated space?
                        if (associatedMeter.ParkingSpaces.Any())
                        {
                            model.Errors.Add(string.Format("Record {0}, AssociatedMeter '{1}' is single-space and already has a parking space assigned to it.", asset.Id, asset.AssociatedMeter));
                            canCreateItem = false;
                        }
                    }
                    else
                    {
                        // Multi-space meter
                    }
                }

                // AssociatedSensor == [Sensors]
                Sensor sensor = null;
                if (asset.AssociatedSensor != null)
                {
                    sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.SensorID == asset.AssociatedSensor);
                    if (sensor == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, AssociatedSensor '{1}' is invalid.", asset.Id, asset.AssociatedSensor));
                        canCreateItem = false;
                    }

                    // Is this sensor already connected to another space?
                    if (sensor.ParkingSpaceId != null)
                    {
                        model.Errors.Add(string.Format("Record {0}, AssociatedSensor '{1}' is already associated with parking space '{2}'.",
                            asset.Id, asset.AssociatedSensor, sensor.ParkingSpaceId));
                        canCreateItem = false;
                    }
                }

                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }

                // If a Space cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }

                model.Results.Add(string.Format("Record {0}, Bay Number '{1}' added successfully.", asset.Id, asset.BayNumber));
            }
        }

        //todo - GTC: Mechanism Work
        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        private void ProcessMechanism(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadMechanismModel> assets =
                _csvContext.Read<UploadMechanismModel>(model.UploadedFileName, _csvFileDescription);

            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == model.CustomerId);
            if (assets.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));
            foreach (var asset in assets)
            {
                bool canCreateItem = true;
                if (asset.LineNumber != 0)
                {
                    bool res = AssetIDExistence
                        (
                        asset.LineNumber.ToString(),
                        model.CustomerId.ToString(),
                      (int)AssetAreaId.Mechanism,
                        (int)MeterGroups.Mechanism
                        );
                    if (res)
                    {
                        model.Errors.Add(string.Format("Record {0}, LineNumber '{1}' is already used.", asset.Id, asset.LineNumber));
                        canCreateItem = false;
                    }
                }
                // First, validate that certain data, if present, resolves to the 
                // associated referential table.

                // Model == [MechanismMaster]
                var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(
                    m => m.MechanismDesc.Equals(asset.Model, StringComparison.CurrentCultureIgnoreCase) && m.MeterGroupId == (int)MeterGroups.Mechanism);
                if (mechanismMaster == null)
                {
                    model.Errors.Add(string.Format("Record {0}, Model '{1}' is invalid.", asset.Id, asset.Model));
                    canCreateItem = false;
                }


                // OperationalStatus == [OperationalStatus]
                OperationalStatu operationalStatus = null;
                if (asset.OperationalStatus != null)
                {
                    operationalStatus = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusDesc.Equals(asset.OperationalStatus, StringComparison.CurrentCultureIgnoreCase));
                    if (operationalStatus == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, OperationalStatus '{1}' is invalid.", asset.Id, asset.OperationalStatus));
                        canCreateItem = false;
                    }
                }

                // SerialNumber == [SerialNumber]
                if (asset.SerialNumber == null || asset.SerialNumber =="")
                {
                    model.Errors.Add(string.Format("Record {0}, SerialNumber not found.", asset.Id));
                    canCreateItem = false;
                }

                // SerialNumber == [SerialNumber]
                var area = PemsEntities.MechMasters.FirstOrDefault(
                    m => m.MechSerial.Equals(asset.SerialNumber, StringComparison.CurrentCultureIgnoreCase) && m.Customerid == model.CustomerId);
                if (area != null)
                {
                    model.Errors.Add(string.Format("Record {0}, SerialNumber '{1}' is already exists.", asset.Id, asset.SerialNumber));
                    canCreateItem = false;
                }

                // State == [AssetState]
                var assetState = PemsEntities.AssetStates.FirstOrDefault(m => m.AssetStateDesc.Equals(asset.State, StringComparison.CurrentCultureIgnoreCase));
                if (assetState == null)
                {
                    model.Errors.Add(string.Format("Record {0}, State '{1}' is invalid.", asset.Id, asset.State));
                    canCreateItem = false;
                }
                
                // If a Meter cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }

              

                // AssetID change
                MechMaster mechanism = null;
                if (asset.LineNumber == 0)
                {
                    mechanism = (new MechanismFactory(ConnectionStringName, Now)).Create(model.CustomerId, asset.SerialNumber);
                }
                else
                {
                    mechanism = (new MechanismFactory(ConnectionStringName, Now)).Create(model.CustomerId, AssetTypeModel.EnterMode.ManuallyGenerated, asset.LineNumber, asset.SerialNumber);
                }


                int OperationalStatusID;
                int State;
                var meter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == mechanism.MechIdNumber && m.CustomerID == model.CustomerId && m.AreaID == (int)AssetAreaId.Mechanism);
                meter.OperationalStatusID = (int)OperationalStatusType.Inactive;
                
                if (assetState.AssetStateId == (int)AssetStateType.Current || assetState.AssetStateId == (int)AssetStateType.Historic)
                    OperationalStatusID = (int)OperationalStatusType.Operational;
                else
                    OperationalStatusID = (int)OperationalStatusType.Inactive;


                meter.MeterName = asset.Name;
                meter.MeterType = mechanismMaster.MechanismId;
                meter.TimeZoneID = customerProfile.TimeZoneID ?? 0;       
                meter.LastPreventativeMaintenance = asset.LastPreventativeMaintenance;
                meter.NextPreventativeMaintenance = asset.NextPreventativeMaintenance;
                meter.InstallDate = asset.InstallDate;
                meter.WarrantyExpiration = asset.WarrantyExpiration;
              //  meter.OperationalStatusID = operationalStatus == null ? (int?)null : operationalStatus.OperationalStatusId;
                meter.OperationalStatusTime = DateTime.Now;//asset.OperationalStatusTime;
                State = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                meter.MeterState = (int)AssetStateType.Pending;
                //meter.MeterState = assetState == null ? (int)AssetStateType.Pending : assetState.AssetStateId;
                PemsEntities.SaveChanges();

                Audit(meter);
               
                // Add AssetPending record
                (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Mechanism,
                                                                           asset.ActivateDate,
                                                                           (AssetStateType)assetState.AssetStateId,
                                                                           model.CustomerId,
                                                                           meter.MeterId,
                                                                           State,
                                                                           OperationalStatusID,
                                                                           meter.AreaID);
                model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", asset.Id, asset.Name));

              
            }
        }

        //todo - GTC: Datakey Work
        /// <summary>
        /// </summary>
        /// <param name="model"></param>
        private void ProcessDatakey(AssetsUploadResultsModel model)
        {
            IEnumerable<UploadDatakeyModel> assets =
                _csvContext.Read<UploadDatakeyModel>(model.UploadedFileName, _csvFileDescription);

            foreach (var asset in assets)
            {
                bool canCreateItem = true;

                // First, validate that certain data, if present, resolves to the 
                // associated referential table.
                //todo - GTC: Datakey Work - fill out this method based on the other assets as an example
            }
        }

        #endregion

        #region Constraints for Upload Files

        private const string ConstraintPad = "  ";
        private const string ConstraintEmpty = "[empty record]";
        private const string ConstraintReturn = "\n";
        private const string ConstraintSpacing = "\n\n";
        private const string FieldRequired = " - Required";
        private const string FieldOptional = " - Optional";


        /// <summary>
        /// Return a string that list the various constrained fields and the allowable data for each field.  This
        /// is used in conjunction with the following classes.
        /// UploadGatewayModel, UploadSensorModel, UploadSingleSpaceMeterModel, UploadMultiSpaceMeterModel, UploadCashboxModel
        /// </summary>
        /// <param name="customerId">Customer Id</param>
        /// <param name="assetType">The <see cref="AssetTypeModel.AssetType"/> that is being described.</param>
        /// <returns>String containing the constraints for the <see cref="AssetTypeModel.AssetType"/></returns>
        public string ConstraintsList(int customerId, AssetTypeModel.AssetType assetType)
        {
            string constraints = "";

            switch (assetType)
            {
                case AssetTypeModel.AssetType.Cashbox:
                    constraints = ConstraintsForCashbox(customerId);
                    break;
                case AssetTypeModel.AssetType.Gateway:
                    constraints = ConstraintsForGateway(customerId);
                    break;
                case AssetTypeModel.AssetType.MultiSpaceMeter:
                    constraints = ConstraintsForMultiSpaceMeter(customerId);
                    break;
                case AssetTypeModel.AssetType.Sensor:
                    constraints = ConstraintsForSensor(customerId);
                    break;
                case AssetTypeModel.AssetType.SingleSpaceMeter:
                    constraints = ConstraintsForSingleSpaceMeter(customerId);
                    break;
                case AssetTypeModel.AssetType.Smartcard:
                    break;
                case AssetTypeModel.AssetType.ParkingSpaces:  //** Sairam added on Oct 1st 2014
                    break;
                case AssetTypeModel.AssetType.Mechanism:
                    //todo - GTC: Mechanism Work
                    constraints = ConstraintsForMechanism(customerId);
                    break;
                case AssetTypeModel.AssetType.DataKey:
                    //todo - GTC: Datakey Work
                    constraints = ConstraintsForDatakey(customerId);
                    break;
            }

            return constraints;
        }


        public string FieldsList(int customerId, AssetTypeModel.AssetType assetType)
        {
            string constraints = "";

            switch (assetType)
            {
                case AssetTypeModel.AssetType.Cashbox:
                    constraints = FieldsForCashbox(customerId);
                    break;
                case AssetTypeModel.AssetType.Gateway:
                    constraints = FieldsForGateway(customerId);
                    break;
                case AssetTypeModel.AssetType.MultiSpaceMeter:
                    constraints = FieldsForMultiSpaceMeter(customerId);
                    break;
                case AssetTypeModel.AssetType.Sensor:
                    constraints = FieldsForSensor(customerId);
                    break;
                case AssetTypeModel.AssetType.SingleSpaceMeter:
                    constraints = FieldsForSingleSpaceMeter(customerId);
                    break;
                case AssetTypeModel.AssetType.Smartcard:
                    break;
                case AssetTypeModel.AssetType.ParkingSpaces:  //** Sairam added on Oct 1st 2014
                    break;
                case AssetTypeModel.AssetType.Mechanism:
                    //todo - GTC: Mechanism Work
                    constraints = FieldsForMechanism(customerId);
                    break;
                case AssetTypeModel.AssetType.DataKey:
                    //todo - GTC: Datakey Work
                    constraints = FieldsForDatakey(customerId);
                    break;
            }

            return constraints;
        }


        private string ConstraintsForCashbox(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.Cashbox
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Location == [CashboxLocationType]
            sb.Append("Field: Location").Append(ConstraintReturn);
            foreach (var row in PemsEntities.CashBoxLocationTypes)
            {
                sb.Append(ConstraintPad).Append(row.CashBoxLocationTypeDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // State == [AssetState]
            sb.Append("Field: State").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            //// OperationalStatus == [OperationalStatus]
            //sb.Append("Field: OperationalStatus").Append(ConstraintReturn).Append(ConstraintPad)
            //    .Append(ConstraintEmpty).Append(ConstraintReturn);
            //foreach (var row in PemsEntities.OperationalStatus)
            //{
            //    sb.Append(ConstraintPad).Append(row.OperationalStatusDesc).Append(ConstraintReturn);
            //}
            //sb.Append(ConstraintSpacing);

            return sb.ToString();
        }

        private string ConstraintsForGateway(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.Gateway
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);
            sb.Append("Field: LineNumber").Append(ConstraintReturn);
            // Area == [Areas]
            sb.Append("Field: Area").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Areas.Where(m => m.CustomerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.AreaName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Zone == [Zones]
            sb.Append("Field: Zone").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Zones.Where(m => m.customerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.ZoneName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Suburb == [CustomGroup1]
            sb.Append("Field: Suburb").Append(ConstraintReturn);
            foreach (var row in PemsEntities.CustomGroup1.Where(m => m.CustomerId == customerId))
            {
                sb.Append(ConstraintPad).Append(row.DisplayName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // State == [AssetState]
            sb.Append("Field: State").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // DemandZone == [DemandZone]
            sb.Append("Field: DemandZone").Append(ConstraintReturn);

            var query = from dz in PemsEntities.DemandZones
                        join dzc in PemsEntities.DemandZoneCustomers on dz.DemandZoneId equals dzc.DemandZoneId
                        where dzc.CustomerId == customerId
                        where dzc.IsDisplay != null
                        where (bool)dzc.IsDisplay == true
                        orderby dz.DemandZoneDesc
                        select new { dz.DemandZoneDesc};


            foreach (var dzDesc in query)
            {
                sb.Append(ConstraintPad).Append(dzDesc.DemandZoneDesc).Append(ConstraintReturn);
            }

            sb.Append(ConstraintSpacing);

            //// OperationalStatus == [OperationalStatus]
            //sb.Append("Field: OperationalStatus").Append(ConstraintReturn).Append(ConstraintPad)
            //    .Append(ConstraintEmpty).Append(ConstraintReturn);
            //foreach (var row in PemsEntities.OperationalStatus)
            //{
            //    sb.Append(ConstraintPad).Append(row.OperationalStatusDesc).Append(ConstraintReturn);
            //}
            //sb.Append(ConstraintSpacing);

            return sb.ToString();
        }

        private string ConstraintsForMultiSpaceMeter(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.MultiSpaceMeter
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);
            sb.Append("Field: LineNumber").Append(ConstraintReturn);
            // Area == [Areas]
            sb.Append("Field: Area").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Areas.Where(m => m.CustomerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.AreaName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Zone == [Zones]
            sb.Append("Field: Zone").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Zones.Where(m => m.customerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.ZoneName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Suburb == [CustomGroup1]
            sb.Append("Field: Suburb").Append(ConstraintReturn);
            foreach (var row in PemsEntities.CustomGroup1.Where(m => m.CustomerId == customerId))
            {
                sb.Append(ConstraintPad).Append(row.DisplayName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // State == [AssetState]
            sb.Append("Field: State").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // DemandZone == [DemandZone]
            sb.Append("Field: DemandZone").Append(ConstraintReturn);

            var query = from dz in PemsEntities.DemandZones
                        join dzc in PemsEntities.DemandZoneCustomers on dz.DemandZoneId equals dzc.DemandZoneId
                        where dzc.CustomerId == customerId
                        where dzc.IsDisplay != null
                        where (bool)dzc.IsDisplay == true
                        orderby dz.DemandZoneDesc
                        select new { dz.DemandZoneDesc };


            foreach (var dzDesc in query)
            {
                sb.Append(ConstraintPad).Append(dzDesc.DemandZoneDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            //// OperationalStatus == [OperationalStatus]
            //sb.Append("Field: OperationalStatus").Append(ConstraintReturn).Append(ConstraintPad)
            //    .Append(ConstraintEmpty).Append(ConstraintReturn);
            //foreach (var row in PemsEntities.OperationalStatus)
            //{
            //    sb.Append(ConstraintPad).Append(row.OperationalStatusDesc).Append(ConstraintReturn);
            //}
            //sb.Append(ConstraintSpacing);

            return sb.ToString();
        }

        private string ConstraintsForSensor(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.Sensor
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            sb.Append("Field: LineNumber").Append(ConstraintReturn);
            // Area == [Areas]
            sb.Append("Field: Area").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Areas.Where(m => m.CustomerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.AreaName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Zone == [Zones]
            sb.Append("Field: Zone").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Zones.Where(m => m.customerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.ZoneName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Suburb == [CustomGroup1]
            sb.Append("Field: Suburb").Append(ConstraintReturn);
            foreach (var row in PemsEntities.CustomGroup1.Where(m => m.CustomerId == customerId))
            {
                sb.Append(ConstraintPad).Append(row.DisplayName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // State == [AssetState]
            sb.Append("Field: State").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // DemandZone == [DemandZone]
            sb.Append("Field: DemandZone").Append(ConstraintReturn);


            var query = from dz in PemsEntities.DemandZones
                        join dzc in PemsEntities.DemandZoneCustomers on dz.DemandZoneId equals dzc.DemandZoneId
                        where dzc.CustomerId == customerId
                        where dzc.IsDisplay != null
                        where (bool)dzc.IsDisplay == true
                        orderby dz.DemandZoneDesc
                        select new { dz.DemandZoneDesc };


            foreach (var dzDesc in query)
            {
                sb.Append(ConstraintPad).Append(dzDesc.DemandZoneDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            //// OperationalStatus == [OperationalStatus]
            //sb.Append("Field: OperationalStatus").Append(ConstraintReturn).Append(ConstraintPad)
            //    .Append(ConstraintEmpty).Append(ConstraintReturn);
            //foreach (var row in PemsEntities.OperationalStatus)
            //{
            //    sb.Append(ConstraintPad).Append(row.OperationalStatusDesc).Append(ConstraintReturn);
            //}
            //sb.Append(ConstraintSpacing);

            // PrimaryGateway == [Gateways]
            // SecondaryGateway == [Gateways]
            sb.Append("Field: PrimaryGateway").Append(ConstraintReturn);
            sb.Append("Field: SecondaryGateway").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append(ConstraintEmpty).Append(ConstraintReturn);
            foreach (var row in PemsEntities.Gateways.Where(m => m.CustomerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.GateWayID).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);


            // PrimaryGatewayState == [AssetState]
            // SecondaryGatewayState == [AssetState]
            sb.Append("Field: PrimaryGatewayState").Append(ConstraintReturn);
            sb.Append("Field: SecondaryGatewayState").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append(ConstraintEmpty).Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);



            // AssociatedMeter == [Meters]
            sb.Append("Fields: AssociatedMeter and AssociatedMeterAreaId").Append(ConstraintReturn).Append(ConstraintPad)
                .Append(ConstraintEmpty).Append(", ").Append(ConstraintEmpty).Append(ConstraintReturn);
            var meterQuery = from m in PemsEntities.Meters
                             join mm in PemsEntities.MeterMaps on m.MeterId equals mm.MeterId
                             where m.CustomerID == customerId && m.MeterName != null && m.AreaID == (int)AssetAreaId.Meter
                             where mm.Customerid == customerId && mm.Areaid == (int)AssetAreaId.Meter
                             where mm.SensorID == null
                             select m;
            foreach (var row in meterQuery)
            {
                sb.Append(ConstraintPad).Append(row.MeterId).Append(", ").Append(row.AreaID).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);


            // AssociatedSpace == [ParkingSpaces]
            sb.Append("Field: AssociatedSpace").Append(ConstraintReturn).Append(ConstraintPad)
                .Append(ConstraintEmpty).Append(ConstraintReturn);
            //   foreach (var row in PemsEntities.ParkingSpaces.Where(m => m.CustomerID == customerId && (m.HasSensor == false || m.HasSensor == null)))
            foreach (var row in PemsEntities.ParkingSpaces.Where(m => m.CustomerID == customerId))
            {
                if (row.Meter.MeterName != null)
                {
                    var IsExist = PemsEntities.Sensors.FirstOrDefault(m => m.ParkingSpaceId == row.ParkingSpaceId);
                    if (IsExist == null)
                        sb.Append(ConstraintPad).Append(row.ParkingSpaceId).Append(" :  ").Append("New").Append(ConstraintReturn);
                    else
                        sb.Append(ConstraintPad).Append(row.ParkingSpaceId).Append(" :  ").Append("Assigned").Append(ConstraintReturn);
                }

            }
            sb.Append(ConstraintSpacing);


            return sb.ToString();
        }

        private string ConstraintsForSingleSpaceMeter(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.SingleSpaceMeter
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);
            sb.Append("Field: LineNumber").Append(ConstraintReturn);
            // Area == [Areas]
            sb.Append("Field: Area").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Areas.Where(m => m.CustomerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.AreaName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Zone == [Zones]
            sb.Append("Field: Zone").Append(ConstraintReturn);
            foreach (var row in PemsEntities.Zones.Where(m => m.customerID == customerId))
            {
                sb.Append(ConstraintPad).Append(row.ZoneName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Suburb == [CustomGroup1]
            sb.Append("Field: Suburb").Append(ConstraintReturn);
            foreach (var row in PemsEntities.CustomGroup1.Where(m => m.CustomerId == customerId))
            {
                sb.Append(ConstraintPad).Append(row.DisplayName).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // State == [AssetState]
            sb.Append("Field: State").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // DemandZone == [DemandZone]
            sb.Append("Field: DemandZone").Append(ConstraintReturn);

            var query = from dz in PemsEntities.DemandZones
                        join dzc in PemsEntities.DemandZoneCustomers on dz.DemandZoneId equals dzc.DemandZoneId
                        where dzc.CustomerId == customerId
                        where dzc.IsDisplay != null
                        where (bool)dzc.IsDisplay == true
                        orderby dz.DemandZoneDesc
                        select new { dz.DemandZoneDesc };


            foreach (var dzDesc in query)
            {
                sb.Append(ConstraintPad).Append(dzDesc.DemandZoneDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            //// OperationalStatus == [OperationalStatus]
            //sb.Append("Field: OperationalStatus").Append(ConstraintReturn).Append(ConstraintPad)
            //    .Append(ConstraintEmpty).Append(ConstraintReturn);
            //foreach (var row in PemsEntities.OperationalStatus)
            //{
            //    sb.Append(ConstraintPad).Append(row.OperationalStatusDesc).Append(ConstraintReturn);
            //}
            //sb.Append(ConstraintSpacing);


            // MechSerialNumber == [MechSerialNumber]
            sb.Append("Field: CustermerID , MechSerialNumber, Status").Append(ConstraintReturn);

             var query1 = from mm in PemsEntities.MechMasters
                        where mm.Customerid == customerId

                        select new { mm.MechSerial,mm.Customerid,mm.MechId };


            foreach (var mmDesc in query1)
            {
                var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                           where s.MechId == mmDesc.MechId  
                                           select s).FirstOrDefault();
                var Status="New";
                if (mechMasterMeter != null)
                    Status = "Already used";

                sb.Append(ConstraintPad).Append(mmDesc.Customerid + " , " + mmDesc.MechSerial + " , " + Status).Append(ConstraintReturn);
            }
            var query2 = from mm in PemsEntities.MechMasters
                          where mm.Customerid != customerId

                         select new { mm.MechSerial, mm.Customerid, mm.MechId };


            foreach (var mmDesc in query2)
            {
                var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                       where s.MechId == mmDesc.MechId
                                       select s).FirstOrDefault();

               

                var Status = "New";
                if (mechMasterMeter != null)
                {
                    Status = "Already used";
                    sb.Append(ConstraintPad).Append(mmDesc.Customerid + " , " + mmDesc.MechSerial + " , " + Status).Append(ConstraintReturn);
                }
                else
                {
                    var mechserial = (from s in PemsEntities.MeterMaps
                                      join m in PemsEntities.MechMasters on s.MechId equals m.MechId
                                      where m.MechSerial == mmDesc.MechSerial && s.Customerid != mmDesc.Customerid
                                      select s).ToList();
                    if (mechserial == null)
                        sb.Append(ConstraintPad).Append(mmDesc.Customerid + " , " + mmDesc.MechSerial + " , " + Status).Append(ConstraintReturn);
                }

                
            }


            sb.Append(ConstraintSpacing);

            return sb.ToString();
        }

        //todo - GTC: Mechanism Work
        private string ConstraintsForMechanism(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.Mechanism
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // State == [AssetState]
            sb.Append(" Field: State").Append(ConstraintReturn);
            foreach (var row in PemsEntities.AssetStates)
            {
                sb.Append(ConstraintPad).Append(row.AssetStateDesc).Append(ConstraintReturn);
            }

            sb.Append(ConstraintSpacing);



            //// OperationalStatus == [OperationalStatus]
            //sb.Append(" Field: OperationalStatus").Append(ConstraintReturn).Append(ConstraintPad)
            //    .Append(ConstraintEmpty).Append(ConstraintReturn);
            //foreach (var row in PemsEntities.OperationalStatus)
            //{
            //    sb.Append(ConstraintPad).Append(row.OperationalStatusDesc).Append(ConstraintReturn);
            //}
            //sb.Append(ConstraintSpacing);           

            return sb.ToString();
        }
        //todo - GTC: Datakey Work -  flesh this out based on the Datakey asset type. use the other assets as an example
        private string ConstraintsForDatakey(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: Model").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters
                             join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                             where mm.MeterGroupId == (int)MeterGroups.Mechanism
                             where mmc.CustomerId == customerId
                             where mmc.IsDisplay
                             select mm;
            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MechanismDesc).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);
            return sb.ToString();
        }

        private string FieldsForCashbox(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LineNumber").Append(", Asset ID Manually Entered by User. If blank - Automatic assignment").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Name").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Model").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Location").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("ActivateDate").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("State").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Sequence").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("InstallDate").Append(FieldOptional).Append(ConstraintReturn);
           // sb.Append(ConstraintPad).Append("OperationalStatus").Append(FieldOptional).Append(ConstraintReturn);
           // sb.Append(ConstraintPad).Append("OperationalStatusTime").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LastPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("NextPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("WarrantyExpiration").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("OperationalStatusEndTime").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("OperationalStatusComment").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }

        private string FieldsForGateway(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LineNumber").Append(", Asset ID Manually Entered by User. If blank - Automatic assignment").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Name").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Model").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Street").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Area").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Zone").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Suburb").Append(FieldRequired).Append(ConstraintReturn);

          

            sb.Append(ConstraintPad).Append("ActivateDate").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("State").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("DemandZone").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Latitude").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Longitude").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("InstallDate").Append(FieldOptional).Append(ConstraintReturn);

            //sb.Append(ConstraintPad).Append("OperationalStatus").Append(FieldOptional).Append(ConstraintReturn);
           // sb.Append(ConstraintPad).Append("OperationalStatusTime").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LastPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("NextPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("WarrantyExpiration").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("HardwareVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SoftwareVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("FirmwareVersion").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }

        private string FieldsForMultiSpaceMeter(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LineNumber").Append(", Asset ID Manually Entered by User. If blank - Automatic assignment").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Name").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Model").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Street").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Area").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Zone").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Suburb").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("ActivateDate").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("State").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("DemandZone").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Latitude").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Longitude").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("PhoneNumber").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("NumberOfBays").Append(FieldOptional).Append(", Defaults to 1 if BayStart exists").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("BayStart").Append(FieldOptional).Append(", If exists, also requires NumberOfBays").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("InstallDate").Append(FieldOptional).Append(ConstraintReturn);

           // sb.Append(ConstraintPad).Append("OperationalStatus").Append(FieldOptional).Append(ConstraintReturn);
           // sb.Append(ConstraintPad).Append("OperationalStatusTime").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LastPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("NextPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("WarrantyExpiration").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("CommunicationsVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SoftwareVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("FirmwareVersion").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }

        private string FieldsForSensor(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LineNumber").Append(", Asset ID Manually Entered by User. If blank - Automatic assignment").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Name").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Model").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Street").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Area").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Zone").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Suburb").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("ActivateDate").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("State").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("DemandZone").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Latitude").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Longitude").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("PrimaryGateway").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("PrimaryGatewayState").Append(FieldOptional).Append(", Required if PrimaryGateway exists").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("SecondaryGateway").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SecondaryGatewayState").Append(FieldOptional).Append(", Required if SecondaryGateway exists").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("AssociatedMeter").Append(FieldOptional).Append(", If exists, also requires AssociatedMeterAreaId").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("AssociatedMeterAreaId").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("AssociatedSpace").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("InstallDate").Append(FieldOptional).Append(ConstraintReturn);

           // sb.Append(ConstraintPad).Append("OperationalStatus").Append(FieldOptional).Append(ConstraintReturn);
           // sb.Append(ConstraintPad).Append("OperationalStatusTime").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LastPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("NextPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("WarrantyExpiration").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("HardwareVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SoftwareVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("FirmwareVersion").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }

        private string FieldsForSingleSpaceMeter(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LineNumber").Append(", Asset ID Manually Entered by User. If blank - Automatic assignment").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Name").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Model").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Street").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Area").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Zone").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Suburb").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("LocationID").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("MechSerialNumber").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("ActivateDate").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("State").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("DemandZone").Append(FieldRequired).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("Latitude").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Longitude").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("PhoneNumber").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("BayNumber").Append(FieldOptional).Append(ConstraintReturn);

            sb.Append(ConstraintPad).Append("InstallDate").Append(FieldOptional).Append(ConstraintReturn);

          //  sb.Append(ConstraintPad).Append("OperationalStatus").Append(FieldOptional).Append(ConstraintReturn);
          //  sb.Append(ConstraintPad).Append("OperationalStatusTime").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("LastPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("NextPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("WarrantyExpiration").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("CommunicationsVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SoftwareVersion").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("FirmwareVersion").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }

        //todo - GTC: Mechanism Work
        private string FieldsForMechanism(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);
         
            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);          
            sb.Append(ConstraintPad).Append("LineNumber").Append(", Asset ID Manually Entered by User. If blank - Automatic assignment").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Name").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Model").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SerialNumber").Append(FieldRequired).Append(" Enter New SerialNumber .").Append(ConstraintReturn);         

            sb.Append(ConstraintPad).Append("ActivateDate").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("State").Append(FieldRequired).Append(ConstraintReturn);
            // sb.Append(ConstraintPad).Append("OperationalStatus").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("InstallDate").Append(FieldOptional).Append(ConstraintReturn);         
            sb.Append(ConstraintPad).Append("LastPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("NextPreventativeMaintenance").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("WarrantyExpiration").Append(FieldOptional).Append(ConstraintReturn);
        
          

            return sb.ToString();
        }

        //todo - GTC: Datakey Work -  flesh this out based on the Datakey asset type. use the other assets as an example
        private string FieldsForDatakey(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Id").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="customerID"></param>
        /// <param name="areaID"></param>
        /// <param name="gatewayareaID"></param>
        /// <param name="assetTpeId"></param>
        /// <returns></returns>
        //public string AssetIDExistence(string assetID, string customerID, string areaID, int gatewayareaID, int assetTpeId)
        //{
        //    string assetIDNew = "0";
        //    string defaultAreaID = string.Empty; //** Sairam added code on Oct 1st 2014 

        //    try
        //    {

        //        int customerId = Int32.Parse(customerID);
        //        int areaId = 0; //** Sairam added code on Oct 1st 2014 

        //        //int areaId = Int32.Parse(areaID);  //** Sairam commented code on Oct 1st 2014 as it throws error.

        //        //** Sairam added code on Oct 1st 2014 
        //        if (string.IsNullOrEmpty(areaID))
        //        {
        //            defaultAreaID = "-1";
        //        }
        //        else
        //        {
        //            areaId = Int32.Parse(areaID);
        //        }


        //        int asID = 0;
        //        int astTpeId = 0;
        //        var device = new object();

        //        PEMEntities pe = new PEMEntities();
        //        Meter meter = null;
        //        CashBox cashbox = null;
        //        Gateway gteway = null;
        //        MechMaster mechMaster = null;
        //        DataKey dataKey = null;
        //        device = null;

        //        if (!String.IsNullOrEmpty(assetID))
        //        {
        //            asID = Int32.Parse(assetID);
        //        }

        //        astTpeId = assetTpeId;

        //        switch (assetTpeId)
        //        {
        //            case (int)MeterGroups.MultiSpaceMeter:
        //                meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && (m.AreaID == areaId || defaultAreaID == "-1")); //** Sairam added code on Oct 1st 2014 
        //                device = meter;
        //                break;
        //            case (int)MeterGroups.SingleSpaceMeter:
        //                meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && (m.AreaID == areaId || defaultAreaID == "-1")); //** Sairam added code on Oct 1st 2014 
        //                device = meter;
        //                break;
        //            case (int)MeterGroups.Cashbox:
        //                cashbox = pe.CashBoxes.FirstOrDefault(c => c.CustomerID == customerId && c.CashBoxID == asID);
        //                break;
        //            case (int)MeterGroups.Gateway:
        //                gteway = pe.Gateways.FirstOrDefault(g => g.CustomerID == customerId && g.GateWayID == asID);
        //                device = gteway;
        //                break;
        //            case (int)MeterGroups.Sensor:
        //                meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && (m.AreaID == areaId || defaultAreaID == "-1")); //** Sairam added code on Oct 1st 2014 
        //                device = meter;
        //                break;
        //            case (int)MeterGroups.Mechanism:
        //                mechMaster = pe.MechMasters.FirstOrDefault(g => g.Customerid == customerId && g.MechIdNumber == asID);
        //                device = mechMaster;
        //                break;
        //            case (int)MeterGroups.Datakey:
        //                dataKey = pe.DataKeys.FirstOrDefault(m => m.CustomerID == customerId && m.DataKeyId == asID);
        //                device = dataKey;
        //                break;
        //        }

        //        if (device != null)
        //        {
        //            assetIDNew = "Invalid";
        //        }
        //        else if (device == null)
        //        {
        //            assetIDNew = "Valid";
        //        }

        //        if (device != null)
        //        {
        //            assetIDNew = "Meter " + assetID + " already in use";
        //        }
        //        else
        //        {
        //            assetIDNew = "Meter " + assetID + " Valid";
        //        }

        //    }
        //    catch (Exception ex) //** Sairam added code on Oct 1st 2014 
        //    {

        //    }

        //    return assetIDNew;
        //    //Json(assetIDNew, JsonRequestBehavior.AllowGet);

        //}


        protected bool AssetIDExistence(string assetID, string customerID, int areaId, int astTpeId)
        {
            int customerId = Int32.Parse(customerID);
            
            string assetIDNew = "0";
            int asID = 0;
          
            var device = new object();

            PEMEntities pe = new PEMEntities();
            Meter meter = null;
            CashBox cashbox = null;
            Gateway gteway = null;
            MechMaster mechanism = null;
            DataKey dataKey = null;
            device = null;

            if (!String.IsNullOrEmpty(assetID))
            {
                asID = Int32.Parse(assetID);
            }

            switch (astTpeId)
            {
                case (int)MeterGroups.MultiSpaceMeter:
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.SingleSpaceMeter:
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.Cashbox:
                    cashbox = pe.CashBoxes.FirstOrDefault(c => c.CustomerID == customerId && c.CashBoxSeq == asID);
                    device = cashbox;
                    break;
                case (int)MeterGroups.Gateway:
                    gteway = pe.Gateways.FirstOrDefault(g => g.CustomerID == customerId && g.GateWayID == asID);
                    device = gteway;
                    break;
                case (int)MeterGroups.Sensor:
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.Mechanism:
                    // mechanism = pe.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechIdNumber == asID);
                    meter = pe.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == asID && m.AreaID == areaId);
                    device = meter;
                    break;
                case (int)MeterGroups.Datakey:
                    dataKey = pe.DataKeys.FirstOrDefault(m => m.CustomerID == customerId && m.DataKeyIdNumber == asID);
                    device = dataKey;
                    break;
            }

           
            if (device != null)
            {
                return true;
            }
            
            return false;
        }



        #endregion

        #region PAM CONFIGURATION
        public AssetsUploadResultsModel ProcessPAMBulkUpload(int customerId, string file)
        {

            AssetsUploadResultsModel model = new AssetsUploadResultsModel()
            {
                CustomerId = customerId,
                UploadedFileName = file
            };

            // Prep the LINQtoCSV context.

            _csvContext = new CsvContext();
            _csvFileDescription = new CsvFileDescription()
            {
                MaximumNbrExceptions = 100,
                SeparatorChar = ','
            };

            try
            {
                ProcessPAMBulkUpload(model);

            }
            catch (AggregatedException ae)
            {
                // Process all exceptions generated while processing the file
                var innerExceptionsList =
                    (List<Exception>)ae.Data["InnerExceptionsList"];

                foreach (Exception e in innerExceptionsList)
                {
                    model.Errors.Add(e.Message);
                }

            }
            catch (Exception ex)
            {
                model.Errors.Add("General exception");
                model.Errors.Add(ex.Message);
            }


            return model;
        }

        private void ProcessPAMBulkUpload(AssetsUploadResultsModel model)
        {
            IEnumerable<BulkUploadPAMConfigModel> PAMConfig =
                _csvContext.Read<BulkUploadPAMConfigModel>(model.UploadedFileName, _csvFileDescription);


            if (PAMConfig.Count() <= 0)
                model.Errors.Add(string.Format("No records found to add"));

            foreach (var PAMsConfig in PAMConfig)
            {
                bool canCreateItem = true;

                // First, validate that certain data, if present, resolves to the 
                // associated referential table.

                Meter associatedMeter = null;
                if (PAMsConfig.MeterID <= 0)
                {
                    model.Errors.Add(string.Format("No valid records found to add. Pleae provide the valid PAM Configuration."));
                    canCreateItem = false;
                }
              
                if (PAMsConfig.MeterID > 0)
                {                    
                    associatedMeter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == PAMsConfig.MeterID && m.CustomerID == model.CustomerId && m.MeterGroup == (int)MeterGroups.SingleSpaceMeter);
                    if (associatedMeter == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, Meter '{1}' is a invalid meter.",
                            PAMsConfig.SNo, PAMsConfig.MeterID));
                        canCreateItem = false;
                    }

                }

                // If a CashBox cannot be created continue on to next record.
                if (!canCreateItem)
                {
                    continue;
                }


                // Now should be able to create a Cashbox
                // For each uploadPamConfig, create a new CashBox
                PAMClusters BulkPamConfig = new PAMClusters();
                {
                    BulkPamConfig.Customerid = model.CustomerId;
                    BulkPamConfig.MeterId = PAMsConfig.MeterID;
                    BulkPamConfig.Description = PAMsConfig.Description;
                }
                //PAMCluster PamBulkUpload = null;
                if (BulkPamConfig.MeterId != null)
                {
                    //PamBulkUpload = (new CustomerFactory(ConnectionStringName)).AddPAMClusterMeter(BulkPamConfig, model.CustomerId);
                    var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
                    (new CustomerFactory(connStringName)).AddPAMClusterMeter(BulkPamConfig, model.CustomerId);
                }

                model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", PAMsConfig.SNo, PAMsConfig.MeterID));
            }
        }

        public string ConstraintsListForPAMConfig(int customerId)
        {
            StringBuilder sb = new StringBuilder();

            // Model == [MechanismMaster]
            sb.Append("Field: MeterID").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.Meters
                             where mm.CustomerID == customerId &&
                             mm.MeterGroup == (int)MeterGroups.SingleSpaceMeter
                             select mm;

            foreach (var row in mechanisms)
            {
                sb.Append(ConstraintPad).Append(row.MeterId).Append(ConstraintReturn);
            }
            sb.Append(ConstraintSpacing);

            // Location == [CashboxLocationType]

            return sb.ToString();
        }

        public string FieldsListForPAMConfig()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Field Notes:").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("SNo").Append(FieldRequired).Append(", Record row number used to report on results.").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("MeterID").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Description").Append(FieldOptional).Append(ConstraintReturn);

            return sb.ToString();
        }
        #endregion

    }
}
