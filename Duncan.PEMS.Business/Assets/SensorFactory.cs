/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________        __________________________________________________________________________________________________
 * 12/20/2013       Sergey Ostrerov                Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 * 02/06/2014       R Howard                    JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 02/19/2014       Sergey Ostrerov                   DPTXPEMS-213 - Time formatting:Alarm Duration.
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// Class encapsulating functionality for the sensor asset type.
    /// </summary>
    public class SensorFactory : AssetFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
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
        public SensorFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }

        #region Index

        /// <summary>
        /// Get a queryable list of sensor summaries (<see cref="IQueryable{AssetListModel}"/>) for
        /// customer id of <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gridType">The type of grid (<see cref="AssetListGridType"/>) that will be using the data</param>
        /// <returns>An instance of <see cref="IQueryable{AssetListModel}"/></returns>
        public IQueryable<AssetListModel> GetSummaryModels(int customerId, AssetListGridType gridType)
        {
            //NOTE: we are intentionally leaving some fields blank - per the spec.
            var items = from sensor in PemsEntities.Sensors
                        where sensor.CustomerID == customerId
                        join mm in PemsEntities.MeterMaps on
                            new { SensorId = sensor.SensorID, CustomerId = sensor.CustomerID } equals
                            new { SensorId = mm.SensorID.Value, CustomerId = mm.Customerid } into l1
                        from meterMap in l1.DefaultIfEmpty()
                        join vpn in PemsEntities.VersionProfileMeters on
                            new { SensorId = sensor.SensorID, CustomerId = sensor.CustomerID } equals
                            new { SensorId = vpn.SensorID.Value, CustomerId = vpn.CustomerId } into l2
                        from vpns in l2.DefaultIfEmpty()
                        join ps in PemsEntities.ParkingSpaces on sensor.ParkingSpaceId equals ps.ParkingSpaceId into l3
                        from parkingSpace in l2.DefaultIfEmpty()
                        let aa = meterMap.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()
                        select
                            new AssetListModel
                            {


                                //----------DETAIL LINKS
                                Type = "Sensor", //name of the details page (ViewCashbox)
                                AreaId = (int)AssetAreaId.Sensor,
                                CustomerId = customerId,

                                //-----------FILTERABLE ITEMS
                                AssetType = sensor.MeterGroup == null ? SensorViewModel.DefaultType : sensor.MeterGroup.MeterGroupDesc ?? SensorViewModel.DefaultType,
                                AssetId = sensor.SensorID,
                                AssetName = sensor.SensorName ?? " ",
                                InventoryStatus = sensor.SensorState == null ? "" : sensor.AssetState.AssetStateDesc,
                                OperationalStatus = sensor.OperationalStatu == null ? " " : sensor.OperationalStatu.OperationalStatusDesc,
                                Latitude = sensor.Latitude,
                                Longitude = sensor.Longitude,
                                AreaId2 = meterMap.AreaId2,
                                ZoneId = meterMap.ZoneId,
                                Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == meterMap.CustomGroup1 && x.CustomerId == customerId) == null ? " " : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == meterMap.CustomGroup1 && x.CustomerId == customerId).DisplayName,
                                Street = sensor.Location ?? " ",
                                DemandStatus = PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == sensor.DemandZone) == null ? "" : PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == sensor.DemandZone).DemandZoneDesc,

                                //------------COMMON COLUMNS
                                AssetModel = sensor.MechanismMaster == null ? " "
                                   : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == customerId && z.MechanismId == sensor.MechanismMaster.MechanismId && z.IsDisplay) == null ? " "
                                   : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == customerId && z.MechanismId == sensor.MechanismMaster.MechanismId && z.IsDisplay).MechanismDesc,

                                //------------SUMMARY GRID
                                SpacesCount = 1,
                                Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == meterMap.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == meterMap.AreaId2).AreaName,
                                Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == meterMap.ZoneId && x.customerID == customerId) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == meterMap.ZoneId && x.customerID == customerId).ZoneName,  

                                //---------------CONFIGURATION GRID
                                DateInstalled = sensor.InstallDateTime,
                                ConfigurationId = vpns == null ? (long?)null : vpns.VersionProfileMeterId,
                                //ConfigCreationDate
                                //ConfigScheduleDate
                                //ConfigActivationDate
                                MpvVersion = vpns == null ? "" : vpns.CommunicationVersion,
                                SoftwareVersion = vpns == null ? "" : vpns.SoftwareVersion,
                                FirmwareVersion = vpns == null ? "" : vpns.HardwareVersion,

                                //-------------OCCUPANCY GRID - only valid for spaces
                                //MeterName
                                //SensorName
                                OperationalStatusDate= sensor.OperationalStatusTime,
                                //OccupancyStatus
                                //OccupancyStatusDate 
                                //NonComplianceStatus
                                //NonComplianceStatusDate

                                //------------FUNCTIONAL STATUS GRID
                                AlarmClass = aa.EventCode1.AlarmTier1.TierDesc,
                                AlarmCode =  aa.EventCode1.EventDescVerbose,
                                AlarmTimeOfOccurance =   aa.TimeOfOccurrance,
                                AlarmRepairTargetTime = aa.SLADue,
                                LocalTime = Now

                            };
         
            return items;
        }

        #endregion

        #region Get View Models

        /// <summary>
        /// Returns an instance of <see cref="SensorViewModel"/> reflecting the present state
        /// and settings of a <see cref="Sensor"/> for customer id of <paramref name="customerId"/>
        /// and a sensor id of <paramref name="sensorId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="sensorId">The sensor id</param>
        /// <returns>An instance of <see cref="SensorViewModel"/></returns>
        public SensorViewModel GetViewModel(int customerId, int sensorId)
        {

            var sensor = PemsEntities.Sensors.FirstOrDefault( m => m.CustomerID == customerId && m.SensorID == sensorId );

            var model = new SensorViewModel
            {
                CustomerId = customerId,
                AreaId = (int)AssetAreaId.Sensor,
                AssetId = sensorId
            };

            model.GlobalId = sensor.GlobalMeterID.ToString() ;

            model.Type = sensor.MeterGroup == null ? SensorViewModel.DefaultType : sensor.MeterGroup.MeterGroupDesc ?? SensorViewModel.DefaultType;
            model.TypeId = sensor.SensorType ?? (int)MeterGroups.Sensor;
            model.AssetModel = sensor.MechanismMaster == null ? " "
             : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == sensor.MechanismMaster.MechanismId && m.IsDisplay) == null ? ""
             : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == sensor.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc;
            model.Name = sensor.SensorName ?? " ";

            model.Street = sensor.Location ?? " ";

//            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.Areaid == (int)AssetAreaId.Sensor && m.SensorID == sensorId);
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.SensorID == sensorId);
            if (meterMap != null)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.AreaID == meterMap.AreaId2);
                model.Area = area == null ? "" : area.AreaName;
                model.AreaId2 = area == null ? 0 : area.AreaID;
                model.AreaId = meterMap.Areaid;
                model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;

                // Maintenace Route
                if ( meterMap.MaintRouteId != null )
                {
                    model.MaintenanceRoute = meterMap.MaintRoute == null ? "" : meterMap.MaintRoute.DisplayName ?? "";
                }
                else
                {
                    model.MaintenanceRoute = "-";
                }
            }
            else
            {
                model.Area = "";
                model.AreaId2 = -1;
                model.AreaId = (int)AssetAreaId.Sensor;
                model.Zone = "";
                model.Suburb = " ";

                // Associated meter.
                model.MeterId = new AssetIdentifier();

                model.MaintenanceRoute = "-";
            }

            // Lat/Long
            model.Latitude = sensor.Latitude;
            model.Longitude = sensor.Longitude;

            // Associated Space
            model.SpaceId = new AssetIdentifier();
            if (sensor.ParkingSpace != null)
            {
                model.SpaceId.AreaId = 0;
                model.SpaceId.AssetId = sensor.ParkingSpace.ParkingSpaceId;
                model.SpaceId.CustomerId = customerId;
            }

            // Associated meter.
            model.MeterId = new AssetIdentifier();
            if (sensor.ParkingSpace != null && sensor.ParkingSpace.Meter != null)
            {
                model.MeterId.AreaId = sensor.ParkingSpace.Meter.AreaID;
                model.MeterId.AssetId = sensor.ParkingSpace.Meter.MeterId;
                model.MeterId.CustomerId = customerId;
            }


            // Space Type - metered or sensor only.
            if (sensor.ParkingSpaceId == null || sensor.ParkingSpaceId == 0)
            {
                model.SpaceType = "";
            }
            else
            {
                var parkingSpaceDetail = sensor.ParkingSpace.ParkingSpaceDetails.FirstOrDefault();
                model.SpaceType = parkingSpaceDetail == null ? "" : (parkingSpaceDetail.SpaceType1 == null ? "" : parkingSpaceDetail.SpaceType1.SpaceTypeDesc ?? "");
            }

            model.LastPrevMaint = sensor.LastPreventativeMaintenance.HasValue ? sensor.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = sensor.NextPreventativeMaintenance.HasValue ? sensor.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            model.WarrantyExpiration = sensor.WarrantyExpiration;

            // Sensor configuration details
            model.Configuration = new SensorConfigurationModel();

            model.Configuration.DateInstalled = sensor.InstallDateTime;

            // Version profile information
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId  && m.SensorID == sensor.SensorID);
            if (versionProfile != null)
            {
                model.Configuration.MPVVersion = versionProfile.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfile.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfile.HardwareVersion ?? "";
                model.Configuration.ConfigurationId = versionProfile.VersionProfileMeterId;
                model.Configuration.ConfigurationName = versionProfile.ConfigurationName ?? "";
            }

            // Get primary Gateway
            var sensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == customerId
                && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);
            model.Configuration.PrimaryGateway = sensorMapping == null ? new AssetIdentifier() : new AssetIdentifier()
                {
                    AreaId = (int)AssetAreaId.Gateway,
                    AssetId = Convert.ToInt64(sensorMapping.GatewayID),
                    CustomerId = customerId
                };

            // Get secondary Gateway
            sensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == customerId
                && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);
            model.Configuration.SecondaryGateway = sensorMapping == null ? new AssetIdentifier() : new AssetIdentifier()
                {
                    AreaId = (int)AssetAreaId.Gateway,
                    AssetId = Convert.ToInt64(sensorMapping.GatewayID),
                    CustomerId = customerId
                };


            // Active alarm
            model.AlarmConfiguration = new AssetAlarmConfigModel();
            if ( meterMap != null && meterMap.Meter != null )
            {
                var activeAlarm = meterMap.Meter.ActiveAlarms.OrderByDescending( m => m.TimeOfOccurrance ).FirstOrDefault();
                if ( activeAlarm != null )
                {
                    model.AlarmConfiguration.Class = activeAlarm.EventCode1.AlarmTier1.TierDesc;
                    model.AlarmConfiguration.Code = activeAlarm.EventCode1.EventDescVerbose ?? " ";
                    DateTime now = DateTime.Now;
                    int totalMinutes = (int) ( activeAlarm.TimeOfOccurrance > now
                                                    ? ( activeAlarm.TimeOfOccurrance - now ).TotalMinutes
                                                    : ( now - activeAlarm.TimeOfOccurrance ).TotalMinutes );
                    model.AlarmConfiguration.Duration = FormatHelper.FormatTimeFromMinutes(totalMinutes, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM);
                    model.AlarmConfiguration.RepairTargetTime = activeAlarm.SLADue == null ? " " : activeAlarm.SLADue.Value.ToString();
                }
            }

            // Operational status of meter.
            model.Status = sensor.OperationalStatu == null ? "" : sensor.OperationalStatu.OperationalStatusDesc;
            model.StatusDate = sensor.OperationalStatusTime;

            // See if sensor has pending changes.
            model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

            // Is asset active?
            model.State = (AssetStateType)sensor.SensorState;

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns an instance of <see cref="SensorViewModel"/> reflecting the present and any pending state
        /// and settings of a <see cref="Sensor"/> for customer id of <paramref name="customerId"/>
        /// and a sensor id of <paramref name="sensorId"/>. 
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="sensorId">The sensor id</param>
        /// <returns>An instance of <see cref="SensorViewModel"/></returns>
        public SensorViewModel GetPendingViewModel(int customerId, int sensorId)
        {
            SensorViewModel model = GetViewModel( customerId, sensorId );

            // Get the pending changes from [AssetPending] table.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault( m => m.CustomerId == model.CustomerId && m.MeterMapSensorId == model.AssetId );

            // Get out if there are no pending changes.
            if (assetPending == null)
                return model;

            SetPropertiesOfPendingAssetModel(assetPending, model);

            // Associated Space
            if ( assetPending.AssocidateSpaceId.HasValue )
            {
                model.SpaceId.AreaId = 0;
                model.SpaceId.AssetId = assetPending.AssocidateSpaceId.Value;
                model.SpaceId.CustomerId = model.CustomerId;

                var parkingSpaceDetail = PemsEntities.ParkingSpaceDetails.FirstOrDefault(m => m.ParkingSpaceId == model.SpaceId.AssetId);
                model.SpaceType = parkingSpaceDetail == null ? "" : (parkingSpaceDetail.SpaceType1 == null ? "" : parkingSpaceDetail.SpaceType1.SpaceTypeDesc ?? "");
            }
            else
            {
                model.SpaceId.AreaId = 0;
                model.SpaceId.AssetId = 0;
                model.SpaceId.CustomerId = 0;
                model.SpaceType = "";
            }

            // Associated meter.
            if (assetPending.AssociatedMeterId.HasValue)
            {
                model.MeterId.AreaId = assetPending.AssociatedMeterAreaId.Value;
                model.MeterId.AssetId = assetPending.AssociatedMeterId.Value;
                model.MeterId.CustomerId = model.CustomerId;
            }

            if ( assetPending.WarrantyExpiration.HasValue )
                model.WarrantyExpiration = assetPending.WarrantyExpiration.Value;

            // Sensor configuration details
            if ( assetPending.DateInstalled.HasValue )
                model.Configuration.DateInstalled = assetPending.DateInstalled.Value;

            // Version information
            if ( !string.IsNullOrWhiteSpace( assetPending.MPVFirmware ) )
                model.Configuration.MPVVersion = assetPending.MPVFirmware;

            if (!string.IsNullOrWhiteSpace(assetPending.AssetSoftwareVersion))
                model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion;

            if (!string.IsNullOrWhiteSpace(assetPending.AssetFirmwareVersion))
                model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion;

            // Get primary Gateway
            if ( assetPending.PrimaryGateway.HasValue )
            {
                model.Configuration.PrimaryGateway.AssetId = assetPending.PrimaryGateway.Value;
                model.Configuration.PrimaryGateway.AreaId = (int)AssetAreaId.Gateway;
                model.Configuration.PrimaryGateway.CustomerId = model.CustomerId;
            }

            // Get secondary Gateway
            if (assetPending.SecondaryGateway.HasValue)
            {
                model.Configuration.SecondaryGateway.AssetId = assetPending.SecondaryGateway.Value;
                model.Configuration.SecondaryGateway.AreaId = (int)AssetAreaId.Gateway;
                model.Configuration.SecondaryGateway.CustomerId = model.CustomerId;
            }

            // Is asset active?
            model.State = AssetStateType.Pending;

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns a <see cref="SensorViewModel"/> based upon the indicated audit record id <paramref name="auditId"/>.
        /// </summary>
        /// <param name="customerId">The customer id (presently not used)</param>
        /// <param name="sensorId">The sensor id (presently not used)</param>
        /// <param name="auditId">The audit record id</param>
        /// <returns>An instance of <see cref="SensorViewModel"/></returns>
        public SensorViewModel GetHistoricViewModel(int customerId, int sensorId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.SensorsAudits.FirstOrDefault(m => m.SensorsAuditId == auditId));
        }

        /// <summary>
        /// Private method to create a <see cref="SensorViewModel"/> from a <see cref="SensorsAudit"/> record.
        /// </summary>
        /// <param name="auditModel">An instance of <see cref="SensorsAudit"/></param>
        /// <returns>An instance of <see cref="SensorViewModel"/></returns>
        private SensorViewModel CreateHistoricViewModel(SensorsAudit auditModel)
        {
            var model = new SensorViewModel()
            {
                CustomerId = auditModel.CustomerID,
                AreaId = (int)AssetAreaId.Sensor,
                AssetId = auditModel.SensorID
            };

            model.GlobalId = auditModel.GlobalMeterID.ToString();

            model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.SensorType) == null ? GatewayViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.SensorType).MeterGroupDesc ?? GatewayViewModel.DefaultType;
            model.TypeId = auditModel.SensorType ?? (int)MeterGroups.Sensor;
            model.AssetModel = !auditModel.SensorModel.HasValue ? ""
              : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == auditModel.SensorModel && m.IsDisplay) == null ? ""
              : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == auditModel.SensorModel && m.IsDisplay).MechanismDesc; 
            model.Name = auditModel.SensorName ?? " ";
            model.Street = auditModel.Location ?? " ";

            // Need to see if there is a PemsEntities.MeterMapAudits for this sensor id within an hour.
            DateTime fromDate = auditModel.UpdateDateTime.AddHours(-1);
            DateTime toDate = auditModel.UpdateDateTime.AddHours(1);

            var meterMapAudit = PemsEntities.MeterMapAudits.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                && m.AuditDateTime > fromDate && m.AuditDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if ( meterMapAudit == null )
            {
                meterMapAudit = PemsEntities.MeterMapAudits.FirstOrDefault( m => m.Customerid == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                                                                                 && m.AuditDateTime <= auditModel.UpdateDateTime);
            }

            // Is there still is not a meterMapAudit then get present MeterMap, otherwise use meterMapAudit
            MeterMap meterMap = null;
            if ( meterMapAudit == null )
            {
                meterMap = PemsEntities.MeterMaps.FirstOrDefault( m => m.Customerid == auditModel.CustomerID && m.SensorID == auditModel.SensorID );
            }

            // Now get Zone, Area and Suburb and Maintenance Route
            // Area
            model.AreaId2 = meterMapAudit == null ? (meterMap == null ? 0 : meterMap.AreaId2) : meterMapAudit.AreaId2;
            var area = model.AreaId2.HasValue ? PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.AreaID == model.AreaId2) : null;
            model.Area = area == null ? "" : area.AreaName;

            // AreaId
            model.AreaId = meterMapAudit == null ? (meterMap == null ? (int)AssetAreaId.Sensor : meterMap.Areaid) : meterMapAudit.Areaid;

            // Zone
            model.ZoneId = meterMapAudit == null ? (meterMap == null ? null : meterMap.ZoneId) : meterMapAudit.ZoneId;
            var zone = PemsEntities.Zones.FirstOrDefault( m => m.customerID == auditModel.CustomerID && m.ZoneId == model.ZoneId );
            model.Zone = zone == null ? "" : zone.ZoneName ?? "";

            // Suburb
            model.SuburbId = meterMapAudit == null ? (meterMap == null ? null : meterMap.CustomGroup1) : meterMapAudit.CustomGroup1;
            var suburb = PemsEntities.CustomGroup1.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.CustomGroupId == model.SuburbId);
            model.Suburb = suburb == null ? "" : suburb.DisplayName ?? "";

            // Maintenance Route
            int? maintRouteId = meterMapAudit == null ? (meterMap == null ? null : meterMap.MaintRouteId) : meterMapAudit.MaintRouteId;
            var maintRoute = PemsEntities.MaintRoutes.FirstOrDefault( m => m.CustomerId == auditModel.CustomerID && m.MaintRouteId == maintRouteId );
            model.MaintenanceRoute = maintRoute == null ? "" : maintRoute.DisplayName ?? "";

            // Lat/Long
            model.Latitude = auditModel.Latitude;
            model.Longitude = auditModel.Longitude;

            // Associated Space - See if there is an associated space about the same time as the audit model.
            model.SpaceId = new AssetIdentifier();
            model.SpaceId.AreaId = 0;
            model.SpaceId.AssetId = auditModel.ParkingSpaceId ?? 0;
            model.SpaceId.AssetId = auditModel.ParkingSpaceId.HasValue ? auditModel.CustomerID : 0;

            // Associated meter.
            model.MeterId = new AssetIdentifier();
            // Associated meter would be meter associated with parking space above.
            if ( model.SpaceId.AssetId > 0 )
            {
                // See if there is a parking space audit record around time of audit.
                var spaceAudit = PemsEntities.ParkingSpacesAudits.FirstOrDefault(m => m.ParkingSpaceId == model.SpaceId.AssetId
                    && m.UpdateDateTime > fromDate && m.UpdateDateTime < toDate);
                // If no audit record then get audit record most recent in the past.
                if (spaceAudit == null)
                {
                    spaceAudit = PemsEntities.ParkingSpacesAudits.FirstOrDefault(m => m.ParkingSpaceId == model.SpaceId.AssetId
                        && m.UpdateDateTime <= auditModel.UpdateDateTime);
                }

                // Was a space audit found?  If not then get space with this space id and get MeterId from it.
                if ( spaceAudit != null )
                {
                    model.MeterId.AreaId = spaceAudit.AreaId;
                    model.MeterId.AssetId = spaceAudit.MeterId;
                    model.MeterId.CustomerId = auditModel.CustomerID;
                }
                else
                {
                    var space = PemsEntities.ParkingSpaces.FirstOrDefault( m => m.ParkingSpaceId == model.SpaceId.AssetId );
                    if ( space != null )
                    {
                        model.MeterId.AreaId = space.AreaId;
                        model.MeterId.AssetId = space.MeterId;
                        model.MeterId.CustomerId = auditModel.CustomerID;
                    }
                }
            }

            // Space Type - metered or sensor only.
            SpaceTypeType spaceType = SpaceTypeType.Undefined;
            if ( model.MeterId.AssetId > 0 && model.SpaceId.AssetId > 0 )
            {
                spaceType = SpaceTypeType.MeteredWithSensor;
            }
            else if (model.MeterId.AssetId > 0 && model.SpaceId.AssetId == 0)
            {
                spaceType = SpaceTypeType.MeteredSpace;
            }
            else if (model.MeterId.AssetId == 0 && model.SpaceId.AssetId > 0)
            {
                spaceType = SpaceTypeType.SensorOnly;
            }

            if ( spaceType == SpaceTypeType.Undefined )
            {
                model.SpaceType = "";
            }
            else
            {
                model.SpaceType = PemsEntities.SpaceTypes.FirstOrDefault( m => m.SpaceTypeId == (int) spaceType ).SpaceTypeDesc;
            }

            // Maint. Dates
            model.LastPrevMaint = auditModel.LastPreventativeMaintenance.HasValue ? auditModel.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = auditModel.NextPreventativeMaintenance.HasValue ? auditModel.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            // Warranty Date
            model.WarrantyExpiration = auditModel.WarrantyExpiration;

            // Sensor configuration details
            model.Configuration = new SensorConfigurationModel();

            model.Configuration.DateInstalled = auditModel.InstallDateTime;

            // Version profile information
            // See if there is a VersionProfileMeterAudit withing an hour of this audit record.
            var versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                && m.UpdateDateTime > fromDate && m.UpdateDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if (versionProfileMeterAudit == null)
            {
                versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                      && m.UpdateDateTime < auditModel.UpdateDateTime);
            }

            // Is there still is not a versionProfileMeterAudit then get present versionProfileMeter, otherwise use versionProfileMeterAudit
            VersionProfileMeter versionProfileMeter = null;
            if (versionProfileMeterAudit == null)
            {
                versionProfileMeter = PemsEntities.VersionProfileMeters.FirstOrDefault( m => m.CustomerId == auditModel.CustomerID && m.SensorID == auditModel.SensorID );
            }


            if ( versionProfileMeterAudit == null && versionProfileMeter == null )
            {
                model.Configuration.MPVVersion = "";
                model.Configuration.SoftwareVersion = "";
                model.Configuration.FirmwareVersion = "";
                model.Configuration.ConfigurationId = 0;
                model.Configuration.ConfigurationName = "";
            }
            else if ( versionProfileMeterAudit == null )
            {
                model.Configuration.MPVVersion = versionProfileMeter.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfileMeter.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfileMeter.HardwareVersion ?? "";
                model.Configuration.ConfigurationId = versionProfileMeter.VersionProfileMeterId;
                model.Configuration.ConfigurationName = versionProfileMeter.ConfigurationName ?? "";
            }
            else
            {
                model.Configuration.MPVVersion = versionProfileMeterAudit.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfileMeterAudit.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfileMeterAudit.HardwareVersion ?? "";
                model.Configuration.ConfigurationId = versionProfileMeterAudit.VersionProfileMeterId;
                model.Configuration.ConfigurationName = versionProfileMeterAudit.ConfigurationName ?? "";
            }
            model.ConfigurationName = model.Configuration.ConfigurationName;



            // Get primary Gateway
            // Check if there is a [SensorMappingAudit] withing an hour of this audit model
            var sensorMappingAudit = PemsEntities.SensorMappingAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                && m.ChangeDate > fromDate && m.ChangeDate < toDate && m.IsPrimaryGateway);
            // If no audit record then get audit record most recent in the past.
            if (sensorMappingAudit == null)
            {
                sensorMappingAudit = PemsEntities.SensorMappingAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                    && m.ChangeDate < auditModel.UpdateDateTime && m.IsPrimaryGateway);
            }

            // Is there still is not a versionProfileMeterAudit then get present versionProfileMeter, otherwise use versionProfileMeterAudit
            SensorMapping sensorMapping = null;
            if (sensorMappingAudit == null)
            {
                sensorMapping = PemsEntities.SensorMappings.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.SensorID == auditModel.SensorID 
                    && m.IsPrimaryGateway && m.MappingState == (int)AssetStateType.Current);
            }


            model.Configuration.PrimaryGateway = new AssetIdentifier()
            {
                AreaId = (sensorMappingAudit != null || sensorMapping != null) ? (int)AssetAreaId.Gateway : 0,
                AssetId = sensorMappingAudit == null ? (sensorMapping == null ? 0 : Convert.ToInt64(sensorMapping.GatewayID) ) : Convert.ToInt64(sensorMappingAudit.GatewayID),
                CustomerId = auditModel.CustomerID
            };

            // Get secondary Gateway
            // Check if there is a [SensorMappingAudit] withing an hour of this audit model
            sensorMappingAudit = PemsEntities.SensorMappingAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                && m.ChangeDate > fromDate && m.ChangeDate < toDate && !m.IsPrimaryGateway);
            // If no audit record then get audit record most recent in the past.
            if (sensorMappingAudit == null)
            {
                sensorMappingAudit = PemsEntities.SensorMappingAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                    && m.ChangeDate < auditModel.UpdateDateTime && !m.IsPrimaryGateway);
            }

            // Is there still is not a versionProfileMeterAudit then get present versionProfileMeter, otherwise use versionProfileMeterAudit
            sensorMapping = null;
            if (sensorMappingAudit == null)
            {
                sensorMapping = PemsEntities.SensorMappings.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.SensorID == auditModel.SensorID
                    && !m.IsPrimaryGateway && m.MappingState == (int)AssetStateType.Current);
            }


            model.Configuration.SecondaryGateway = new AssetIdentifier()
            {
                AreaId = (sensorMappingAudit != null || sensorMapping != null) ? (int)AssetAreaId.Gateway : 0,
                AssetId = sensorMappingAudit == null ? (sensorMapping == null ? 0 : Convert.ToInt64(sensorMapping.GatewayID)) : Convert.ToInt64(sensorMappingAudit.GatewayID),
                CustomerId = auditModel.CustomerID
            };

            // Active alarm
            model.AlarmConfiguration = new AssetAlarmConfigModel();

            // Operational status of sensor.
            model.Status = auditModel.OperationalStatus.HasValue
                               ? PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == auditModel.OperationalStatus.Value).OperationalStatusDesc
                               : "";
            model.StatusDate = auditModel.OperationalStatusTime;

            // Updated by
            model.LastUpdatedById = auditModel.UserId;
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReason = (AssetPendingReasonType)(auditModel.AssetPendingReasonId ?? 0);
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;

            // Record date
            model.RecordDate = auditModel.UpdateDateTime;

            // Audit record id
            model.AuditId = auditModel.SensorsAuditId;

            return model;
        }

        /// <summary>
        /// Gets an <see cref="AssetHistoryModel"/> for a given <paramref name="customerId"/> and <paramref name="sensorId"/>.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="sensorId">The sensor id</param>
        /// <returns>An instance of an <see cref="AssetHistoryModel"/></returns>
        public AssetHistoryModel GetHistoricListViewModel(int customerId, int sensorId)
        {
            var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == customerId && m.SensorID == sensorId);

            var model = new AssetHistoryModel()
            {
                CustomerId = customerId,
                AssetId = sensorId,
                Type = sensor.MeterGroup == null ? SensorViewModel.DefaultType : sensor.MeterGroup.MeterGroupDesc ?? SensorViewModel.DefaultType,
                TypeId = sensor.SensorType ?? (int)MeterGroups.Sensor,
                Name = sensor.SensorName ?? " ",
                Street = sensor.Location ?? " "
            };

            return model;
        }



        /// <summary>
        /// Get a <see cref="SensorViewModel"/> instance for a particular space.
        /// </summary>
        /// <param name="spaceId">Space Id (64-bit)</param>
        /// <returns>Returns <see cref="SensorEditModel"/> if exists, else null.</returns>
        public SensorViewModel GetViewModel(Int64 spaceId)
        {
            SensorViewModel model = null;

            var sensor = PemsEntities.Sensors.FirstOrDefault( m => m.ParkingSpaceId == spaceId );

            if ( sensor != null )
            {
                model = new SensorViewModel { CustomerId = sensor.CustomerID, AssetId = sensor.SensorID };
            }

            return model;
        }

        #endregion

        #region Get Edit Models

        /// <summary>
        /// Returns an instance of <see cref="SensorEditModel"/> reflecting the present and any pending state
        /// and settings of a <see cref="Sensor"/> for customer id of <paramref name="customerId"/>
        /// and a sensor id of <paramref name="sensorId"/>. This model is used for edit page.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="sensorId">The sensor id</param>
        /// <returns>An instance of <see cref="SensorEditModel"/></returns>
        public SensorEditModel GetEditModel(int customerId, int sensorId)
        {
            SensorEditModel model = new SensorEditModel()
            {
                CustomerId = customerId,
                AreaId = (int)AssetAreaId.Sensor,
                AssetId = sensorId
            };

            var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == customerId && m.SensorID == sensorId);

            #region Build model from a sensor

            model.GlobalId = (sensor.GlobalMeterID ?? 0).ToString();

            // Sensor Type
            model.Type = sensor.MeterGroup == null ? SensorViewModel.DefaultType : sensor.MeterGroup.MeterGroupDesc ?? SensorViewModel.DefaultType;
            model.TypeId = (MeterGroups)(sensor.SensorType ?? (int) MeterGroups.Sensor);

            // Get sensor models.
            model.AssetModelId = sensor.SensorModel ?? -1;

            // Get meter name.
            model.Name = sensor.SensorName ?? " ";

            // Get street name
            model.Street = sensor.Location ?? " ";

            // Associated meter
            model.AssociatedMeterId = sensor.ParkingSpaceId == null ? -1 : sensor.ParkingSpace.Meter.MeterId;
            var AssociatedmeterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.MeterId == model.AssociatedMeterId && m.Areaid == (int)AssetAreaId.Meter);

            // Get Zone and Suburb
            //            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.Areaid == (int)AssetAreaId.Sensor && m.SensorID == sensorId);
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.SensorID == sensorId);
            if (meterMap != null)
            {
                if (sensor.ParkingSpaceId != null)
                {
                    model.AreaListId = meterMap.AreaId2 ?? AssociatedmeterMap.AreaId2 ?? -1;
                    model.ZoneId = meterMap.ZoneId ?? AssociatedmeterMap.ZoneId ?? -1;
                    model.SuburbId = meterMap.CustomGroup1 ?? AssociatedmeterMap.CustomGroup1 ?? -1;

                    // Get area id of associated meter.
                    model.AreaId = meterMap.Areaid;

                    // Maintenace Route
                    model.MaintenanceRouteId = meterMap.MaintRouteId ?? AssociatedmeterMap.MaintRouteId ?? -1;
                    if (model.Street == "")
                    {
                        model.Street = sensor.ParkingSpace.Meter.Location ?? " ";

                    }
                }
                else
                {
                    //DTEPEMS  177
                    model.AreaListId = meterMap.AreaId2  ?? -1;
                    model.ZoneId = meterMap.ZoneId  ?? -1;
                    model.SuburbId = meterMap.CustomGroup1  ?? -1;

                    // Get area id of associated meter.
                    model.AreaId = meterMap.Areaid;

                    // Maintenace Route
                    model.MaintenanceRouteId = meterMap.MaintRouteId ?? -1;

                }
               
            }


            else
            {
                if (sensor.ParkingSpaceId != null)
                {
                    model.AreaListId = AssociatedmeterMap.AreaId2 ?? -1;
                    model.ZoneId = AssociatedmeterMap.ZoneId ?? -1;
                    model.SuburbId = AssociatedmeterMap.CustomGroup1 ?? -1;

                    // Maintenace Route
                    model.MaintenanceRouteId = AssociatedmeterMap.MaintRouteId ?? -1;
                    if (model.Street == "")
                    {
                        model.Street = sensor.ParkingSpace.Meter.Location ?? " ";

                    }
                }
                else
                {
                    model.AreaListId = -1;
                    model.ZoneId = -1;
                    model.SuburbId = -1;

                    // Maintenace Route
                    model.MaintenanceRouteId = -1;
                }

            }


            // Lat/Long
            if (sensor.ParkingSpaceId == null)
            { 
                 model.Latitude = sensor.Latitude;
                 model.Longitude = sensor.Longitude;
            }
            else
            {
                model.Latitude = sensor.Latitude == 0.0 ? sensor.ParkingSpace.Meter.Latitude ?? 0.0 : sensor.Latitude;
                model.Longitude = sensor.Longitude == 0.0 ? sensor.ParkingSpace.Meter.Longitude ?? 0.0 : sensor.Longitude;

            }




            // Get list of possible meters.
            //model.AssociatedMeter = (
            //                                from m in PemsEntities.Meters
            //                                join mm in PemsEntities.MeterMaps
            //                                    on new { CI = m.CustomerID, AI = m.AreaID, MI = m.MeterId } equals new { CI = mm.Customerid, AI = mm.Areaid, MI = mm.MeterId }
            //                                where m.CustomerID == customerId
            //                                where m.MeterGroup == (int)MeterGroups.SingleSpaceMeter || m.MeterGroup == (int)MeterGroups.MultiSpaceMeter
            //                                select new SelectListItemWrapper()
            //                                {
            //                                    Selected = m.MeterId == model.AssociatedMeterId,
            //                                    TextInt = m.MeterId,
            //                                    ValueInt = m.MeterId
            //                                }).ToList();


            //model.AssociatedMeter.Insert(0,
            //                                  new SelectListItemWrapper()
            //                                  {
            //                                      Selected = model.AssociatedMeterId == -1,
            //                                      Text = "",
            //                                      Value = "-1"
            //                                  });


            // Associated space
            model.AssociatedSpaceId = sensor.ParkingSpaceId ?? -1;


            //// Get list of possible parking spaces without Sensors.
            //model.AssociatedSpace = (from ps in PemsEntities.ParkingSpaces
            //                         where ps.CustomerID == customerId //&& ps.HasSensor == false
            //                         select new SelectListItemWrapper()
            //                         {
            //                             Selected = ps.ParkingSpaceId == model.AssociatedSpaceId,
            //                             TextInt64 = ps.ParkingSpaceId,
            //                             ValueInt64 = ps.ParkingSpaceId
            //                         }).ToList();

            //if (sensor.ParkingSpace != null)
            //{
            //    model.AssociatedSpace.Insert(0,
            //                                      new SelectListItemWrapper()
            //                                      {
            //                                          Selected = true,
            //                                          TextInt64 = sensor.ParkingSpace.ParkingSpaceId,
            //                                          ValueInt64 = sensor.ParkingSpace.ParkingSpaceId
            //                                      });
            //}

            //model.AssociatedSpace.Insert(0,
            //                                  new SelectListItemWrapper()
            //                                  {
            //                                      Selected = model.AssociatedMeterId == -1,
            //                                      Text = "",
            //                                      Value = "-1"
            //                                  });




            // Space Type - metered or sensor only.
            if (sensor.ParkingSpaceId == null)
            {
                model.SpaceType = "";
            }
            else
            {
                var parkingSpaceDetail = sensor.ParkingSpace.ParkingSpaceDetails.FirstOrDefault();
                model.SpaceType = parkingSpaceDetail == null ? "" : (parkingSpaceDetail.SpaceType1 == null ? "" : parkingSpaceDetail.SpaceType1.SpaceTypeDesc ?? "");
            }


            // Get Sensor configuration attributes.
            model.Configuration = new SensorConfigurationEditModel();

            // Get primary Gateway
            var sensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == customerId
                                                                           && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);
            model.Configuration.PrimaryGatewayId = sensorMapping == null ? -1 : Convert.ToInt32(sensorMapping.GatewayID);

            // Get secondary Gateway
            sensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == customerId
                                                                       && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);
            model.Configuration.SecondaryGatewayId = sensorMapping == null ? -1 : Convert.ToInt32(sensorMapping.GatewayID);

            // Preventative maintenance dates
            model.LastPrevMaint = sensor.LastPreventativeMaintenance.HasValue ? sensor.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = sensor.NextPreventativeMaintenance.HasValue ? sensor.NextPreventativeMaintenance.Value : (DateTime?)null;

            // Maintenace Route
            GetMaintenanceRouteList(model);

            // Warantry expiration
            model.WarrantyExpiration = sensor.WarrantyExpiration.HasValue ? sensor.WarrantyExpiration.Value : (DateTime?)null;

            // Version profile information
            model.Configuration.DateInstalled = sensor.InstallDateTime;
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.SensorID == sensorId);
            if (versionProfile == null)
            {
                model.Configuration.MPVVersion = "";
                model.Configuration.SoftwareVersion = "";
                model.Configuration.FirmwareVersion = "";
            }
            else
            {
                model.Configuration.MPVVersion = versionProfile.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfile.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfile.HardwareVersion ?? "";
            }

            // Operational status of sensor.
            GetOperationalStatusDetails(model, sensor.OperationalStatus ?? 0, sensor.OperationalStatusTime.HasValue ? sensor.OperationalStatusTime.Value : DateTime.Now);
//            GetOperationalStatusList(model, sensor.OperationalStatus ?? 0);

            // Is asset active?
            model.StateId = sensor.SensorState;
            GetAssetStateList(model);

            #endregion

            // If there is a pending asset record then use it to update the model.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == (int)AssetAreaId.Sensor && m.MeterMapSensorId == sensorId);
            if (assetPending != null)
            {
                #region Build model from an assetPending

                // Get sensor models.
                model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;

                // Get meter name.
                model.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? model.Name : assetPending.AssetName;

                // Get street name
                model.Street = string.IsNullOrWhiteSpace(assetPending.LocationMeters) ? model.Street : assetPending.LocationMeters;

                // Get Zone and Suburb
                model.AreaListId = assetPending.MeterMapAreaId2 ?? model.AreaListId;
                model.ZoneId = assetPending.MeterMapZoneId ?? model.ZoneId;
                model.SuburbId = assetPending.MeterMapSuburbId ?? model.SuburbId;

                // Lat/Long
                model.Latitude = assetPending.Latitude ?? model.Latitude;
                model.Longitude = assetPending.Longitude ?? model.Longitude;

                // Associated meter
                model.AssociatedMeterId = (int)(assetPending.AssociatedMeterId ?? model.AssociatedMeterId);
                    
                // Associated space
                model.AssociatedSpaceId = assetPending.AssocidateSpaceId ?? model.AssociatedSpaceId;


                //// Space Type - metered or sensor only.
                //if (model.AssociatedSpaceId <= 0)
                //{
                //    model.SpaceType = "";
                //}
                //else
                //{
                //    var parkingSpaceDetail = sensor.ParkingSpace.ParkingSpaceDetails.FirstOrDefault();
                //    model.SpaceType = parkingSpaceDetail == null ? "" : (parkingSpaceDetail.SpaceType1 == null ? "" : parkingSpaceDetail.SpaceType1.SpaceTypeDesc ?? "");
                //}


                // Get Sensor configuration attributes.
                model.Configuration = new SensorConfigurationEditModel();

                // Get primary Gateway
                model.Configuration.PrimaryGatewayId = (int)(assetPending.PrimaryGateway ?? model.Configuration.PrimaryGatewayId);

                // Get secondary Gateway
                model.Configuration.SecondaryGatewayId = (int)(assetPending.SecondaryGateway ?? model.Configuration.SecondaryGatewayId);

                // Maint. Route
                model.MaintenanceRouteId = assetPending.MeterMapMaintenanceRoute ?? model.MaintenanceRouteId;

                // Preventative maintenance dates
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.HasValue ? assetPending.LastPreventativeMaintenance.Value.ToShortDateString() : model.LastPrevMaint;
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance ?? model.NextPrevMaint;

                // Warantry expiration
                model.WarrantyExpiration = assetPending.WarrantyExpiration ?? model.WarrantyExpiration;

                // Version profile information
                model.Configuration.MPVVersion = assetPending.MPVFirmware ?? model.Configuration.MPVVersion;
                model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion ?? model.Configuration.SoftwareVersion;
                model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion ?? model.Configuration.FirmwareVersion;

                
                // Operational status of meter.
                if ( assetPending.OperationalStatus.HasValue )
                {
                    GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);
//                    GetOperationalStatusList(model, assetPending.OperationalStatus.Value);
                }

                // Asset state is pending
                model.StateId = (int)AssetStateType.Pending;

                // Activation Date
                model.ActivationDate = assetPending.RecordMigrationDateTime;

                #endregion
            }
            model.AssociatedMeter = (
                                            from m in PemsEntities.Meters
                                            join mm in PemsEntities.MeterMaps
                                                on new { CI = m.CustomerID, AI = m.AreaID, MI = m.MeterId } equals new { CI = mm.Customerid, AI = mm.Areaid, MI = mm.MeterId }
                                            where m.CustomerID == customerId && m.MeterName != null
                                            where m.MeterGroup == (int)MeterGroups.SingleSpaceMeter || m.MeterGroup == (int)MeterGroups.MultiSpaceMeter
                                            select new SelectListItemWrapper()
                                            {
                                                Selected = m.MeterId == model.AssociatedMeterId,
                                                Text = m.MeterName,
                                                ValueInt = m.MeterId
                                            }).ToList();


            model.AssociatedMeter.Insert(0,
                                              new SelectListItemWrapper()
                                              {
                                                  Selected = model.AssociatedMeterId == -1,
                                                  Text = "",
                                                  Value = "-1"
                                              });

            // Get list of possible parking spaces without Sensors.
            model.AssociatedSpace = (from ps in PemsEntities.ParkingSpaces
                                     where ps.CustomerID == customerId && ps.MeterId == model.AssociatedMeterId //&& ps.HasSensor == false
                                     select new SelectListItemWrapper()
                                     {
                                         Selected = ps.ParkingSpaceId == model.AssociatedSpaceId,
                                         TextInt64 = ps.ParkingSpaceId,
                                         ValueInt64 = ps.ParkingSpaceId
                                     }).ToList();

            if (sensor.ParkingSpace != null)
            {
                model.AssociatedSpace.Insert(0,
                                                  new SelectListItemWrapper()
                                                  {
                                                      Selected = true,
                                                      TextInt64 = sensor.ParkingSpace.ParkingSpaceId,
                                                      ValueInt64 = sensor.ParkingSpace.ParkingSpaceId
                                                  });
            }

            model.AssociatedSpace.Insert(0,
                                              new SelectListItemWrapper()
                                              {
                                                  Selected = model.AssociatedMeterId == -1,
                                                  Text = "",
                                                  Value = "-1"
                                              });


            // Get list of possible models.
            GetAssetModelList(model, (int)MeterGroups.Sensor);

            // Get zone list
            GetZoneList(model);

            // Get suburb list
            GetSuburbList(model);

            // Get Areas list
            GetAreaList(model);

            // Maintenace Route
            GetMaintenanceRouteList(model);

            // Get the list of possible Gateways that sensor could be using.
            GatewayFactory gatewayFactory = new GatewayFactory(ConnectionStringName, Now);
            model.Configuration.PrimaryGateway = gatewayFactory.GatewayList(customerId, model.Configuration.PrimaryGatewayId);
            model.Configuration.SecondaryGateway = gatewayFactory.GatewayList(customerId, model.Configuration.SecondaryGatewayId);

            GetAssetStateList(model);

            // Initialize ActivationDate if needed and set to midnight.
            if (!model.ActivationDate.HasValue)
            {
                model.ActivationDate = Now;
            }
            model.ActivationDate = SetToMidnight(model.ActivationDate.Value);

            return model;
        }

        /// <summary>
        /// Refreshes any lists in the <see cref="SensorEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="SensorEditModel"/> to refresh</param>
        public void RefreshEditModel(SensorEditModel editModel)
        {
            var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == editModel.CustomerId && m.SensorID == editModel.AssetId);

            // Get sensor models.
            GetAssetModelList(editModel, (int)MeterGroups.Sensor);

            // Get zone list
            GetZoneList(editModel);

            // Get suburb list
            GetSuburbList(editModel);

            // Get Areas list
            GetAreaList(editModel);


            // Associated meter
            // Get list of possible meters.
            //editModel.AssociatedMeter = (from mm in PemsEntities.MeterMaps
            //                                   where mm.Customerid == editModel.CustomerId
            //                                   where mm.Areaid == editModel.AreaId
            //                                   select new SelectListItemWrapper()
            //                                   {
            //                                       Selected = mm.MeterId == editModel.AssociatedMeterId,
            //                                       TextInt = mm.MeterId,
            //                                       ValueInt = mm.MeterId
            //                                   }).ToList();
          



            // Get list of possible parking spaces.
            //editModel.AssociatedSpace = (from ps in PemsEntities.ParkingSpaces
            //                                   where ps.CustomerID == editModel.CustomerId                                              
            //                                   select new SelectListItemWrapper()
            //                                   {
            //                                       Selected = ps.ParkingSpaceId == editModel.AssociatedSpaceId,
            //                                       TextInt64 = ps.ParkingSpaceId,
            //                                       ValueInt64 = ps.ParkingSpaceId
            //                                   }).ToList();


            // Get Sensor configuration attributes.
            editModel.Configuration = new SensorConfigurationEditModel();

            // Get the list of possible Gateways that sensor could be using.
            //GatewayFactory gatewayFactory = new GatewayFactory(ConnectionStringName, Now);
            //editModel.Configuration.PrimaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.PrimaryGatewayId);
            //editModel.Configuration.SecondaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.SecondaryGatewayId);

            // Maintenace Route
            GetMaintenanceRouteList( editModel );

            //// Operational status of meter.
            //GetOperationalStatusList(editModel, editModel.Status.StatusId);

            // If there is a pending asset record then use it to update the model.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == editModel.CustomerId && m.AreaId == (int)AssetAreaId.Sensor && m.MeterMapSensorId == editModel.AssetId);
            if (assetPending != null)
            {
                #region Build model from an assetPending

                // Get sensor models.
                editModel.AssetModelId = assetPending.AssetModel ?? editModel.AssetModelId;

                // Get meter name.
                editModel.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? editModel.Name : assetPending.AssetName;

                // Get street name
                editModel.Street = string.IsNullOrWhiteSpace(assetPending.LocationMeters) ? editModel.Street : assetPending.LocationMeters;

                // Get Zone and Suburb
                editModel.AreaListId = assetPending.MeterMapAreaId2 ?? editModel.AreaListId;
                editModel.ZoneId = assetPending.MeterMapZoneId ?? editModel.ZoneId;
                editModel.SuburbId = assetPending.MeterMapSuburbId ?? editModel.SuburbId;

                // Lat/Long
                editModel.Latitude = assetPending.Latitude ?? editModel.Latitude;
                editModel.Longitude = assetPending.Longitude ?? editModel.Longitude;

                // Associated meter
                editModel.AssociatedMeterId = (int)(assetPending.AssociatedMeterId ?? editModel.AssociatedMeterId);

                // Associated space
                editModel.AssociatedSpaceId = assetPending.AssocidateSpaceId ?? editModel.AssociatedSpaceId;


                //// Space Type - metered or sensor only.
                //if (model.AssociatedSpaceId <= 0)
                //{
                //    model.SpaceType = "";
                //}
                //else
                //{
                //    var parkingSpaceDetail = sensor.ParkingSpace.ParkingSpaceDetails.FirstOrDefault();
                //    model.SpaceType = parkingSpaceDetail == null ? "" : (parkingSpaceDetail.SpaceType1 == null ? "" : parkingSpaceDetail.SpaceType1.SpaceTypeDesc ?? "");
                //}


                // Get Sensor configuration attributes.
                editModel.Configuration = new SensorConfigurationEditModel();

                // Get primary Gateway
                editModel.Configuration.PrimaryGatewayId = (int)(assetPending.PrimaryGateway ?? editModel.Configuration.PrimaryGatewayId);

                // Get secondary Gateway
                editModel.Configuration.SecondaryGatewayId = (int)(assetPending.SecondaryGateway ?? editModel.Configuration.SecondaryGatewayId);

                // Maint. Route
                editModel.MaintenanceRouteId = assetPending.MeterMapMaintenanceRoute ?? editModel.MaintenanceRouteId;

                // Preventative maintenance dates
                editModel.LastPrevMaint = assetPending.LastPreventativeMaintenance.HasValue ? assetPending.LastPreventativeMaintenance.Value.ToShortDateString() : editModel.LastPrevMaint;
                editModel.NextPrevMaint = assetPending.NextPreventativeMaintenance ?? editModel.NextPrevMaint;

                // Warantry expiration
                editModel.WarrantyExpiration = assetPending.WarrantyExpiration ?? editModel.WarrantyExpiration;

                // Version profile information
                editModel.Configuration.MPVVersion = assetPending.MPVFirmware ?? editModel.Configuration.MPVVersion;
                editModel.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion ?? editModel.Configuration.SoftwareVersion;
                editModel.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion ?? editModel.Configuration.FirmwareVersion;


                // Operational status of meter.
                if (assetPending.OperationalStatus.HasValue)
                {
                    GetOperationalStatusDetails(editModel, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);
                    //                    GetOperationalStatusList(model, assetPending.OperationalStatus.Value);
                }

                // Asset state is pending
                editModel.StateId = (int)AssetStateType.Pending;

                // Activation Date
                editModel.ActivationDate = assetPending.RecordMigrationDateTime;

                #endregion
            }

            editModel.AssociatedMeter = (
                                         from m in PemsEntities.Meters
                                         join mm in PemsEntities.MeterMaps
                                             on new { CI = m.CustomerID, AI = m.AreaID, MI = m.MeterId } equals new { CI = mm.Customerid, AI = mm.Areaid, MI = mm.MeterId }
                                         where m.CustomerID == editModel.CustomerId
                                         where m.MeterGroup == (int)MeterGroups.SingleSpaceMeter || m.MeterGroup == (int)MeterGroups.MultiSpaceMeter
                                         select new SelectListItemWrapper()
                                         {
                                             Selected = m.MeterId == editModel.AssociatedMeterId,
                                             TextInt = m.MeterId,
                                             ValueInt = m.MeterId
                                         }).ToList();


            editModel.AssociatedMeter.Insert(0,
                                              new SelectListItemWrapper()
                                              {
                                                  Selected = editModel.AssociatedMeterId == -1,
                                                  Text = "",
                                                  Value = "-1"
                                              });


            // Get list of possible parking spaces without Sensors.
            editModel.AssociatedSpace = (from ps in PemsEntities.ParkingSpaces
                                     where ps.CustomerID == editModel.CustomerId //&& ps.HasSensor == false
                                     select new SelectListItemWrapper()
                                     {
                                         Selected = ps.ParkingSpaceId == editModel.AssociatedSpaceId,
                                         TextInt64 = ps.ParkingSpaceId,
                                         ValueInt64 = ps.ParkingSpaceId
                                     }).ToList();

            if (sensor.ParkingSpace != null)
            {
                editModel.AssociatedSpace.Insert(0,
                                                  new SelectListItemWrapper()
                                                  {
                                                      Selected = true,
                                                      TextInt64 = sensor.ParkingSpace.ParkingSpaceId,
                                                      ValueInt64 = sensor.ParkingSpace.ParkingSpaceId
                                                  });
            }

            editModel.AssociatedSpace.Insert(0,
                                              new SelectListItemWrapper()
                                              {
                                                  Selected = editModel.AssociatedMeterId == -1,
                                                  Text = "",
                                                  Value = "-1"
                                              });

            // Get the list of possible Gateways that sensor could be using.
            GatewayFactory gatewayFactory = new GatewayFactory(ConnectionStringName, Now);
            editModel.Configuration.PrimaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.PrimaryGatewayId);
            editModel.Configuration.SecondaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.SecondaryGatewayId);

            // Get state list
            GetAssetStateList(editModel);
        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="SensorEditModel"/> to the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of a <see cref="SensorEditModel"/> with updates</param>
        public void SetEditModel(SensorEditModel model)
        {

            // Note:  At this time all changes to the sensor are written to the [AssetPending] table.

            var pendingFactory = new PendingFactory(ConnectionStringName, Now);

            // Use installation date to determine asset state
            if (model.Configuration.DateInstalled.HasValue)
            {
                // If meter.InstallDate is <= customer "now" then set meter to  AssetStateType.Current
                if (model.Configuration.DateInstalled < Now)
                {
                    model.StateId = (int)AssetStateType.Current;
                }
            }

            var otherModel = GetEditModel( model.CustomerId, (int) model.AssetId );

            // Write changes to [AssetPending] table.
            pendingFactory.Save(model, otherModel);


        }

        /// <summary>
        /// Takes a list of <see cref="AssetIdentifier"/> pointing to a list of sensors and creates the model
        /// used to mass edit the list of sensors.  The model contains only fields that can be changed for all
        /// items in the list.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="sensorIds">A <see cref="List{AssetIdentifier}"/> indicating sensors to be edited</param>
        /// <returns>An instance of <see cref="SensorMassEditModel"/></returns>
        public SensorMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> sensorIds)
        {
            SensorMassEditModel editModel = new SensorMassEditModel()
            {
                CustomerId = customerId
            };
            editModel.SetList(sensorIds);

            // Get State list
            editModel.StateId = -1;
            GetAssetStateList(editModel);

            // Get zone list
            editModel.ZoneId = -1;
            editModel.Zone = ZoneList(customerId, -1);

            // Get suburb list
            editModel.SuburbId = -1;
            editModel.Suburb = SuburbList(customerId, -1);

            // Get Areas list
            editModel.AreaListId = -1;
            editModel.Area = AreaList(customerId, -1);

            // Preventative maintenance dates
            editModel.LastPrevMaint = (DateTime?)null;
            editModel.NextPrevMaint = (DateTime?)null;

            // Maintenace Route
            editModel.MaintenanceRouteId = -1;
            editModel.MaintenanceRoute = MaintenanceRouteList(customerId, -1);

            // Warantry expiration
            editModel.WarrantyExpiration = (DateTime?)null;

            // Get Sensor configuration attributes.
            editModel.Configuration = new SensorConfigurationEditModel();

            editModel.Configuration.PrimaryGatewayId = -1;
            editModel.Configuration.SecondaryGatewayId = -1;

            // Get the list of possible Gateways that sensor could be using.
            GatewayFactory gatewayFactory = new GatewayFactory(ConnectionStringName, Now);
            editModel.Configuration.PrimaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.PrimaryGatewayId);
            editModel.Configuration.SecondaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.SecondaryGatewayId);

            // Initialize ActivationDate and set to midnight.
            editModel.ActivationDate = SetToMidnight(Now);

            return editModel;
        }

        /// <summary>
        /// Refreshes any lists in the <see cref="SensorMassEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="SensorMassEditModel"/> to refresh</param>
        public void RefreshMassEditModel(SensorMassEditModel editModel)
        {
            // Get State list
            GetAssetStateList(editModel);

            // Get zone list
            editModel.Zone = ZoneList(editModel.CustomerId, editModel.ZoneId);

            // Get suburb list
            editModel.Suburb = SuburbList(editModel.CustomerId, editModel.SuburbId);

            // Get Areas list
            editModel.Area = AreaList(editModel.CustomerId, editModel.AreaListId);

            // Maintenace Route
            editModel.MaintenanceRoute = MaintenanceRouteList(editModel.CustomerId, editModel.MaintenanceRouteId);

            // Get Sensor configuration attributes.
            editModel.Configuration = new SensorConfigurationEditModel();

            // Get the list of possible Gateways that sensor could be using.
            GatewayFactory gatewayFactory = new GatewayFactory(ConnectionStringName, Now);
            editModel.Configuration.PrimaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.PrimaryGatewayId);
            editModel.Configuration.SecondaryGateway = gatewayFactory.GatewayList(editModel.CustomerId, editModel.Configuration.SecondaryGatewayId);
        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="SensorMassEditModel"/> to the [AssetPending] table for each
        /// sensor in the <see cref="SensorMassEditModel.AssetIds"/> list.  Only values that have been changed are 
        /// stored.
        /// </summary>
        /// <param name="model">Instance of a <see cref="SensorMassEditModel"/> with updates</param>
        public void SetMassEditModel(SensorMassEditModel model)
        {
            var pendingFactory = new PendingFactory(ConnectionStringName, Now);
            foreach (var asset in model.AssetIds())
            {
                // Save changes to the [AssetPending] table.
                pendingFactory.Save(model, model.CustomerId, asset.AreaId, (int)asset.AssetId);

            }
        }


        public bool IsExistsAssociatedSpaceId(long AssociatedSpaceId, int CustomerId)
        {
            var IsExist = (from m in PemsEntities.Sensors
                           where m.ParkingSpaceId == AssociatedSpaceId && m.CustomerID == CustomerId
                           select m).FirstOrDefault();

            if (IsExist != null)
            {
                return true;
            }
            return false;
        }



        public MeterMap GetdetailsAssociatedMeterMap(int AssociatedSpaceId, int CustomerId)
        {      
            var AssociatedmeterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == CustomerId && m.MeterId == AssociatedSpaceId && m.Areaid == (int)AssetAreaId.Meter);
            return AssociatedmeterMap;
        }


        public Meter GetdetailsAssociatedMeter(int AssociatedSpaceId, int CustomerId)
        {
            var AssociatedmeterMap = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == CustomerId && m.MeterId == AssociatedSpaceId && m.AreaID == (int)AssetAreaId.Meter);
            return AssociatedmeterMap;
        }

        #endregion

        #region Create/Clone

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Sensor"/> refrenced by <paramref name="assetId"/> and <paramref name="customerId"/>.  This cloned
        /// <see cref="Sensor"/> is written to the [Sensor] table and the [AssetPending] table.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id (presently not used)</param>
        /// <param name="assetId">The sensor id to clone</param>
        /// <returns>Integer representing the new sensor id (<see cref="Sensor.SensorID"/>)</returns>
        private int Clone(int customerId, int areaId, Int64 assetId)
        {
            SensorMapping primaryGatewayMap = null;
            SensorMapping secondaryGatewayMap = null;


            // Get original sensor and its associated Meter and MeterMap
            // Since this sensor exists, these entities should exist also.
            Sensor sensor = PemsEntities.Sensors.FirstOrDefault(m => m.SensorID == assetId && m.CustomerID == customerId);
            MeterMap meterMap = sensor.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.SensorID == assetId);
            Meter meter = meterMap.Meter;

            // Create the new meter that is associated with the sensor type of asset.  This is not a meter
            // but rather a an asset in the meter table that represents a sensor.
            Meter newMeter = CreateBaseAsset(meter.CustomerID, MeterGroups.Sensor, meter.AreaID);

            // Create a default barcode of "XXXX"
            byte[] barCodeText = new byte[] { 0x58, 0x58, 0x58, 0x58, 0x58 };

            // Create a Sensor
            Sensor newSensor = new Sensor()
            {
                SensorID = NextSensorId(customerId),
                CustomerID = customerId,
                BarCodeText = barCodeText,
                Description = "",
                Latitude = 0.0,
                Longitude = 0.0,
                Location = sensor.Location,
                SensorName = "",
                SensorState = (int)AssetStateType.Pending,
                ParkingSpaceId = (long?)null,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                SensorType = newMeter.MeterGroup,
                InstallDateTime = DateTime.Now,
                GlobalMeterID = newMeter.GlobalMeterId
            };

            newSensor.DemandZone = sensor.DemandZone;
            newSensor.SensorModel = sensor.SensorModel;
            newSensor.WarrantyExpiration = sensor.WarrantyExpiration;

            PemsEntities.Sensors.Add(newSensor);
            PemsEntities.SaveChanges();

            // Add AssetPending record
            (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Sensor,
                                                                       SetToMidnight(Now),
                                                                       AssetStateType.Current,
                                                                       customerId,
                                                                       newSensor.SensorID,
                                                                       (int)AssetStateType.Pending, (int)OperationalStatusType.Inactive,
                                                                       (int)AssetAreaId.Sensor);

            // Align clonable meter fields.
            newMeter.Location = meter.Location;
            newMeter.DemandZone = meter.DemandZone;
            newMeter.FreeParkingMinute = meter.FreeParkingMinute;
            newMeter.MeterGroup = meter.MeterGroup;
            newMeter.MeterName = meter.MeterName;
            newMeter.MeterType = meter.MeterType;
            newMeter.TimeZoneID = meter.TimeZoneID;
            newMeter.TypeCode = meter.TypeCode;
            newMeter.WarrantyExpiration = meter.WarrantyExpiration;
            newMeter.NextPreventativeMaintenance = meter.NextPreventativeMaintenance;

            // Create meter map for cloned gateway.
            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            // Create a [MeterMap] entry
            MeterMap newMeterMap = new MeterMap()
            {
                Customerid = newMeter.CustomerID,
                Areaid = newMeter.AreaID,
                MeterId = newMeter.MeterId,
                HousingId = housingMaster.HousingId
            };
            PemsEntities.MeterMaps.Add(newMeterMap);
            PemsEntities.SaveChanges();

            newMeterMap.AreaId2 = meterMap.AreaId2;
            newMeterMap.ZoneId = meterMap.ZoneId;
            newMeterMap.SubAreaID = meterMap.SubAreaID;
            newMeterMap.MechId = meterMap.MechId;
            newMeterMap.DataKeyId = meterMap.DataKeyId;
            newMeterMap.CustomGroup1 = meterMap.CustomGroup1;
            newMeterMap.CustomGroup2 = meterMap.CustomGroup2;
            newMeterMap.CustomGroup3 = meterMap.CustomGroup3;
            newMeterMap.MaintRouteId = meterMap.MaintRouteId;
            newMeterMap.SensorID = newSensor.SensorID;


            PemsEntities.SaveChanges();



            // Set primary Gateway
            SensorMapping currentPrimaryActiveSensorMapping;
            SensorMapping currentPrimaryPendingSensorMapping;

            // Get existing current primary gateway for sensor that is being cloned if there is one.
            currentPrimaryActiveSensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == customerId
                && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);

            // Get existing pending primary gateway if there is one.
            currentPrimaryPendingSensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == customerId
                && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Pending);

            // Does the original sensor have a current primary gateway?  If so, make it the pending primary gateway for the cloned sensor.
            if ( currentPrimaryActiveSensorMapping != null )
            {
                // Add a pending SensorMapping
                primaryGatewayMap = new SensorMapping()
                {
                    CustomerID = customerId,
                    SensorID = newSensor.SensorID,
                    GatewayID = currentPrimaryActiveSensorMapping.GatewayID,
                    IsPrimaryGateway = true,
                    MappingState = (int)AssetStateType.Pending
                };
                PemsEntities.SensorMappings.Add(primaryGatewayMap);
            }
            else
            {
                // If there was a pending primary gateway for the sensor being cloned then
                // make it the pending primary gateway for the cloned sensor.
                if ( currentPrimaryPendingSensorMapping != null )
                {
                    // Add a pending SensorMapping
                    primaryGatewayMap = new SensorMapping()
                    {
                        CustomerID = customerId,
                        SensorID = newSensor.SensorID,
                        GatewayID = currentPrimaryPendingSensorMapping.GatewayID,
                        IsPrimaryGateway = true,
                        MappingState = (int)AssetStateType.Pending
                    };
                    PemsEntities.SensorMappings.Add(primaryGatewayMap);
                }
            }

            // Set secondary Gateway
            // Get existing current secondary gateway for sensor that is being cloned if there is one.
            currentPrimaryActiveSensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == customerId
                && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);

            // Get existing pending secondary gateway if there is one.
            currentPrimaryPendingSensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == customerId
                && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Pending);

            // Does the original sensor have a current secondary gateway?  If so, make it the pending secondary gateway for the cloned sensor.
            if (currentPrimaryActiveSensorMapping != null)
            {
                // Add a pending SensorMapping
                secondaryGatewayMap = new SensorMapping()
                {
                    CustomerID = customerId,
                    SensorID = newSensor.SensorID,
                    GatewayID = currentPrimaryActiveSensorMapping.GatewayID,
                    IsPrimaryGateway = false,
                    MappingState = (int)AssetStateType.Pending
                };
                PemsEntities.SensorMappings.Add(secondaryGatewayMap);
            }
            else
            {
                // If there was a pending secondary gateway for the sensor being cloned then
                // make it the pending secondary gateway for the cloned sensor.
                if (currentPrimaryPendingSensorMapping != null)
                {
                    // Add a pending SensorMapping
                    secondaryGatewayMap = new SensorMapping()
                    {
                        CustomerID = customerId,
                        SensorID = newSensor.SensorID,
                        GatewayID = currentPrimaryPendingSensorMapping.GatewayID,
                        IsPrimaryGateway = false,
                        MappingState = (int)AssetStateType.Pending
                    };
                    PemsEntities.SensorMappings.Add(secondaryGatewayMap);
                }
            }

            // Clone the VersionProfileMeter if exists.
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == sensor.CustomerID && m.SensorID == sensor.SensorID);
            if (versionProfile != null)
            {
                // Make a new VersionProfileMeters
                VersionProfileMeter newVersionProfileMeter = new VersionProfileMeter()
                {
                    ConfigurationName = versionProfile.ConfigurationName,
                    CustomerId = customerId,
                    AreaId = (int)AssetAreaId.Sensor,
                    SensorID = newSensor.SensorID,
                    MeterGroup = (int)MeterGroups.Sensor,
                    HardwareVersion = versionProfile.HardwareVersion,
                    SoftwareVersion = versionProfile.SoftwareVersion,
                    CommunicationVersion = versionProfile.CommunicationVersion,
                    Version1 = versionProfile.Version1,
                    Version2 = versionProfile.Version2,
                    Version3 = versionProfile.Version3,
                    Version4 = versionProfile.Version4,
                    Version5 = versionProfile.Version5,
                    Version6 = versionProfile.Version6

                };

                PemsEntities.VersionProfileMeters.Add(newVersionProfileMeter);
                Audit(newVersionProfileMeter);
            }


            // Set audit records.
            Audit(primaryGatewayMap);
            Audit(secondaryGatewayMap);
            Audit(newMeter);
            Audit(newMeterMap);
            Audit(newSensor);

            return newSensor.SensorID;
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Sensor"/> refrenced by <paramref name="model"/>.  
        /// This cloned <see cref="Sensor"/> is written to the [Sensor] table and the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of <see cref="SensorViewModel"/> to clone</param>
        /// <returns>Integer representing the newly cloned sensor id (<see cref="Sensor.SensorID"/>)</returns>
        public int Clone(SensorViewModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Sensor"/> refrenced by <paramref name="model"/>.  
        /// This cloned <see cref="Sensor"/> is written to the [Sensor] table and the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of <see cref="SensorEditModel"/> to clone</param>
        /// <returns>Integer representing the newly cloned sensor id (<see cref="Sensor.SensorID"/>)</returns>
        public int Clone(SensorEditModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }


        /// <summary>
        /// Gets the next Sensor Id from the table [Sensor].  This will be used
        /// in the creation of a new sensor.  It is entirely possible that this is the first sensor
        /// so check for this case.
        /// </summary>
        /// <param name="customerId">Integer id of customer</param>
        /// <returns>Next available sensor id.</returns>
        private int NextSensorId(int customerId)
        {
            int nextSensorId = 1;

            Sensor sensor = PemsEntities.Sensors.FirstOrDefault( m => m.CustomerID == customerId );
            if ( sensor != null )
            {
                nextSensorId = PemsEntities.Sensors.Where(m => m.CustomerID == customerId).Max(m => m.SensorID) + 1;
            }
            return nextSensorId;
        }


        /// <summary>
        /// Create a new sensor and set it to <see cref="OperationalStatusType.Inactive"/> operational status (<see cref="CashBox.OperationalStatus"/>) 
        /// and a <see cref="AssetStateType.Pending"/> state (<see cref="CashBox.CashBoxState"/>). Associate it with this <paramref name="customerId"/>.
        /// 
        /// If there is a non-zero <paramref name="gatewayId"/>, make a relationship to the new sensor and the <paramref name="gatewayId"/> in
        /// [SensorMapping] table.
        /// 
        /// If there is a non-zero <paramref name="spaceId"/>, make a relationship to the new sensor and the <paramref name="spaceId"/> in
        /// [Sensors] table entry and set <see cref="ParkingSpace.HasSensor"/> to true in the associated [ParkingSpaces] table row for
        /// the associated parking space.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gatewayId">The associated gateway id [optional]</param>
        /// <param name="spaceId">The associated parking space id [optional]</param>
        /// <returns>Instance of <see cref="Sensor"/> of new sensor.</returns>
        public Sensor Create(int customerId, int gatewayId = 0, Int64 spaceId = 0)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(customerId, MeterGroups.Sensor);

            // Create an audit entry.
            Audit(newMeter);

            // Create a Sensor record.
            int sensorId = NextSensorId( customerId );

            // Create a default barcode of "XXXX"
            byte[] barCodeText = new byte[] { 0x58, 0x58, 0x58, 0x58, 0x58 };

            //making deallocating ParkingSpaceId and sensor
           

                var oldSensor = (from s in PemsEntities.Sensors
                                    where s.ParkingSpaceId == spaceId
                                    select s).ToList();
                oldSensor.ForEach(a => a.ParkingSpaceId = null); // set no longer active                
                PemsEntities.SaveChanges();
               

                var oldSensorMap = (from s in PemsEntities.SensorMappings                                    
                                      where  s.ParkingSpaceID == spaceId
                                      select s).ToList();
                oldSensorMap.ForEach(a => a.ParkingSpaceID = null); // set no longer active                
                PemsEntities.SaveChanges();

            
            Sensor sensor = new Sensor()
                {
                    CustomerID = customerId,
                    SensorID =  sensorId,
                    BarCodeText = barCodeText,
                    Latitude = 0.0,
                    Longitude = 0.0,
                    Location = "",
                    Description = "",
                    SensorName = "",
                    SensorState = (int)AssetStateType.Pending,
                    OperationalStatus = (int)OperationalStatusType.Inactive,
                    OperationalStatusTime = DateTime.Now,
                    InstallDateTime = DateTime.Now,
                    ParkingSpaceId = spaceId <= 0 ? (long?)null : spaceId,
                    GlobalMeterID = newMeter.GlobalMeterId
                };

            PemsEntities.Sensors.Add( sensor );
            PemsEntities.SaveChanges();

            // Add audit record.
            Audit(sensor);

            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            // Create a MeterMap record to join Sensor and Meter
            MeterMap meterMap = new MeterMap()
            {
                Customerid = newMeter.CustomerID,
                Areaid = newMeter.AreaID,
                MeterId = newMeter.MeterId,
                SensorID = sensor.SensorID,
                HousingId = housingMaster.HousingId
            };

            PemsEntities.MeterMaps.Add(meterMap);
            PemsEntities.SaveChanges();

            Audit(meterMap);

            // If there is a gateway, create a SensorMapping entry.
            if ( gatewayId > 0 )
            {

                 //Add row to SensorMapping table.
                SensorMapping sensorMapping = new SensorMapping()
                {
                    CustomerID = customerId,
                    SensorID = sensor.SensorID,
                    ParkingSpaceID = spaceId <= 0 ? (long?)null : spaceId,
                    GatewayID = gatewayId,
                    IsPrimaryGateway = true,
                    MappingState = (int)AssetStateType.Pending
                };
                PemsEntities.SensorMappings.Add(sensorMapping);
                PemsEntities.SaveChanges();

                // Add audit record.
                Audit(sensorMapping);
                
            }

            // If a parking space is being assgned, update ParkingSpace.HasSensor
            if ( spaceId > 0 )
            {
                var space = PemsEntities.ParkingSpaces.FirstOrDefault( m => m.ParkingSpaceId == spaceId );
                if ( space != null )
                {
                    space.HasSensor = true;
                    PemsEntities.SaveChanges();
                    Audit(space);
                }
            }



            return sensor;
        }

        // AssetID Change
        public Sensor Create(AssetTypeModel.EnterMode enterMode, int meterId, int customerId, int gatewayId = 0, Int64 spaceId = 0)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(enterMode, meterId,customerId, MeterGroups.Sensor);
         
            // Create an audit entry.
            Audit(newMeter);

            // Create a Sensor record.
            //int sensorId = NextSensorId(customerId);
            int sensorId = newMeter.MeterId;

            // Create a default barcode of "XXXX"
            byte[] barCodeText = new byte[] { 0x58, 0x58, 0x58, 0x58, 0x58 };


            //making deallocating ParkingSpaceId and sensor
            var oldSensor = (from s in PemsEntities.Sensors
                             where s.ParkingSpaceId == spaceId
                             select s).ToList();
            oldSensor.ForEach(a => a.ParkingSpaceId = null); // set no longer active                
            PemsEntities.SaveChanges();


            var oldSensorMap = (from s in PemsEntities.SensorMappings
                                where s.ParkingSpaceID == spaceId
                                select s).ToList();
            oldSensorMap.ForEach(a => a.ParkingSpaceID = null); // set no longer active                
            PemsEntities.SaveChanges();

            Sensor sensor = new Sensor()
            {
                CustomerID = customerId,
                SensorID = sensorId,
                BarCodeText = barCodeText,
                Latitude = 0.0,
                Longitude = 0.0,
                Location = "",
                Description = "",
                SensorName = "",
                SensorState = (int)AssetStateType.Pending,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                InstallDateTime = DateTime.Now,
                ParkingSpaceId = spaceId <= 0 ? (long?)null : spaceId,
                GlobalMeterID = newMeter.GlobalMeterId
            };

            PemsEntities.Sensors.Add(sensor);
            PemsEntities.SaveChanges();

            // Add audit record.
            Audit(sensor);

            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));
                        

            // Create a MeterMap record to join Sensor and Meter
            MeterMap meterMap = new MeterMap()
            {
                Customerid = newMeter.CustomerID,
                Areaid = newMeter.AreaID,
                MeterId = newMeter.MeterId,
                SensorID = sensor.SensorID,
                HousingId = housingMaster.HousingId
            };

            PemsEntities.MeterMaps.Add(meterMap);
            PemsEntities.SaveChanges();

            Audit(meterMap);

            // If there is a gateway, create a SensorMapping entry.
            if (gatewayId > 0)
            {

                // Add row to SensorMapping table.
                SensorMapping sensorMapping = new SensorMapping()
                {
                    CustomerID = customerId,
                    SensorID = sensor.SensorID,
                    ParkingSpaceID = spaceId <= 0 ? (long?)null : spaceId,
                    GatewayID = gatewayId,
                    IsPrimaryGateway = true,
                    MappingState = (int)AssetStateType.Pending
                };
                PemsEntities.SensorMappings.Add(sensorMapping);
                PemsEntities.SaveChanges();

                // Add audit record.
                Audit(sensorMapping);

            }

            // If a parking space is being assgned, update ParkingSpace.HasSensor
            if (spaceId > 0)
            {
                var space = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);
                if (space != null)
                {
                    space.HasSensor = true;
                    PemsEntities.SaveChanges();
                    Audit(space);
                }
            }



            return sensor;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Resets a sensor by writing a reset record into the [MeterResetSchedule] table.  The sensor
        /// is referenced by the <paramref name="assetIdentifier"/> and <paramref name="userId"/> indicates
        /// who requested the reset.
        /// </summary>
        /// <param name="assetIdentifier">Instance of <see cref="AssetIdentifier"/> indicating sensor to reset</param>
        /// <param name="userId">User id identifying requesting user</param>
        /// <returns>True indicating <see cref="MeterResetSchedule"/> was created and written to [MeterResetSchedule] table</returns>
        public bool Reset(AssetIdentifier assetIdentifier, int userId)
        {

            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.SensorID == assetIdentifier.AssetId && m.Customerid == assetIdentifier.CustomerId);
            if (meterMap != null)
            {
                // According to Duncan, the MeterMap maps to the sensor.  Use customer id, meter id and area id to reset sensor.
                var meterResetSchedule = new MeterResetSchedule();

                meterResetSchedule.CustomerId = meterMap.Customerid;
                meterResetSchedule.AreaId = meterMap.Areaid;
                meterResetSchedule.MeterId = meterMap.MeterId;

                meterResetSchedule.ResetDateTime = DateTime.Now;

                meterResetSchedule.UserId = userId;

                PemsEntities.MeterResetSchedules.Add(meterResetSchedule);

                return PemsEntities.SaveChanges() > 0;
            }
            return false;
        }


        #endregion

        #region Grid Lists

       

        //public void GetGridList(int customerId, List<AssetGridOccupancy> list)
        //{
        //    var query = from s in PemsEntities.Sensors
        //                join ps in PemsEntities.ParkingSpaces on s.ParkingSpaceId equals ps.ParkingSpaceId
        //                join spt in PemsEntities.SensorPaymentTransactions on ps.ParkingSpaceId equals spt.ParkingSpaceId
        //                where s.CustomerID == customerId
        //                select new { spt, s, ps };


        //    foreach (var row in query)
        //    {
        //        AssetGridOccupancy model = new AssetGridOccupancy();

        //        // Operational status of gateway.
        //        model.Status.Status = row.s.OperationalStatu == null ? "" : row.s.OperationalStatu.OperationalStatusDesc;
        //        model.Status.StatusDate = row.s.OperationalStatu == null ?DateTime.MinValue: row.s.OperationalStatusTime?? DateTime.MinValue;

        //        // Occupancy Status
        //        model.Occupancy.OccupancyStatus = row.spt.OccupancyStatu == null ? "" : row.spt.OccupancyStatu.StatusDesc ?? "";
        //        model.Occupancy.OccupancyStatusDate = row.spt.OccupancyDate;
        //        model.Occupancy.NonComplianceStatus = row.spt.NonCompliantStatu == null ? "" : row.spt.NonCompliantStatu.NonCompliantStatusDesc ?? "";
        //        model.Occupancy.NonComplianceStatusDate = row.spt.DepartureTime;

        //        // Names
        //        model.AssetId = row.ps.ParkingSpaceId;
        //        model.AssetName = (row.s.SensorModel == null ? "" : row.s.SensorModel.ToString());
        //        model.MeterName = row.ps.DisplaySpaceNum ?? "";
        //        model.SensorName = row.s.SensorName ?? "";

        //        list.Add(model);
        //    }
        //}
        #endregion

        #region History

        /// <summary>
        /// Gets a <see cref="List{SensorViewModel}"/> representing the known history of a
        /// sensor referenced by sensor id of <paramref name="assetId"/> and customer id of <paramref name="customerId"/>.
        /// Return only a list that are between <paramref name="startDateTicks"/> and <paramref name="endDateTicks"/>
        /// </summary>
        /// <param name="request">Instance of <see cref="DataSourceRequest"/> from Kendo UI</param>
        /// <param name="total">Out parameter used to return total number of records being returned</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="assetId">The sensor id</param>
        /// <param name="startDateTicks">From date in ticks</param>
        /// <param name="endDateTicks">To date in ticks</param>
        /// <returns><see cref="List{SensorViewModel}"/> instance</returns>
        public List<SensorViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {
            DateTime startDate;
            DateTime endDate;

            IQueryable<SensorsAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.SensorsAudits.Where(m => m.CustomerID == customerId && m.SensorID == assetId
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.SensorsAudits.Where(m => m.CustomerID == customerId && m.SensorID == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.SensorsAudits.Where(m => m.CustomerID == customerId && m.SensorID == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.SensorsAudits.Where(m => m.CustomerID == customerId && m.SensorID == assetId).OrderBy(m => m.UpdateDateTime);
            }

            // Get the SensorViewModel for each audit record.
            var list = new List<SensorViewModel>();

            // Get present data
            var presentModel = GetViewModel(customerId, (int)assetId);
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data
            SensorViewModel previousModel = null;
            foreach (var sensor in query)
            {
                var activeModel = CreateHistoricViewModel(sensor);
                list.Add(activeModel);
                if (previousModel != null)
                {
                    previousModel.RecordSuperceededDate = activeModel.RecordDate;
                }
                previousModel = activeModel;
            }

            // Now filter, sort and page.
            var items = list.AsQueryable();

            items = items.ApplyFiltering(request.Filters);
            total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            return items.ToList();
        }

        #endregion
    
    }
}
