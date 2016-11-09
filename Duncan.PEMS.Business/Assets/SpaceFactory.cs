/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________        __________________________________________________________________________________________________
 * 02/06/2014       R Howard                    JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Tariffs;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// Class encapsulating functionality for the parking space psudo-asset type.
    /// </summary>
    public class SpaceFactory : AssetFactory
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
        public SpaceFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }

        #region "Index

        /// <summary>
        /// Get a queryable list of parking space summaries (<see cref="IQueryable{AssetListModel}"/>) for
        /// customer id of <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gridType">The type of grid (<see cref="AssetListGridType"/>) that will be using the data</param>
        /// <returns>An instance of <see cref="IQueryable{AssetListModel}"/></returns>
        public IQueryable<AssetListModel> GetSummaryModels(int customerId, AssetListGridType gridType)
        {
            //NOTE: we are intentionally leaving some fields blank - per the spec.
            //get the meters with the meter map



            var items = from parkingSpace in PemsEntities.ParkingSpaces
                        where parkingSpace.CustomerID == customerId
                        join metermap in PemsEntities.MeterMaps on
                            new { MeterId = parkingSpace.MeterId, AreaId = parkingSpace.AreaId, CustomerId = parkingSpace.CustomerID } equals
                            new { MeterId = metermap.MeterId, AreaId = metermap.Areaid, CustomerId = metermap.Customerid } into l1
                        from mm in l1.DefaultIfEmpty()
                        join parkingspaceDetails in PemsEntities.ParkingSpaceDetails on
                            new { parkingSpace.ParkingSpaceId } equals
                           new { parkingspaceDetails.ParkingSpaceId } into l2
                        from psd in l2.DefaultIfEmpty()
                        join configSpace in PemsEntities.ConfigProfileSpaces on
                           new { parkingSpace.ParkingSpaceId } equals
                          new { configSpace.ParkingSpaceId } into l3
                        from cps in l3.DefaultIfEmpty()

                        // RJH:  Space does not have an entry in VersionProfileMeter.

                        //join profMeters in PemsEntities.VersionProfileMeters on
                        //    new { MeterId = parkingSpace.MeterId, AreaId = parkingSpace.AreaId, CustomerId = parkingSpace.CustomerID } equals
                        //   new { MeterId = profMeters.MeterId.Value, AreaId = profMeters.AreaId.Value, CustomerId = profMeters.CustomerId } into l4
                        //from vpn in l4.DefaultIfEmpty()
                        join senspaytrancurr in PemsEntities.SensorPaymentTransactionCurrents on
                         new { ParkingSpaceId = parkingSpace.ParkingSpaceId } equals
                        new { ParkingSpaceId = senspaytrancurr.ParkingSpaceId } into l5
                        from sptc in l5.DefaultIfEmpty()
                        let aa = parkingSpace.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()
                        // let pso = parkingSpace.ParkingSpaceOccupancies.FirstOrDefault()
                        select
                            new AssetListModel
                            {
                                //----------DETAIL LINKS
                                Type = "Space", //name of the details page (ViewCashbox)
                                AreaId = parkingSpace.AreaId, //m.areaid maybe?
                                CustomerId = customerId,

                                //-----------FILTERABLE ITEMS
                                AssetType = parkingSpace.MeterGroup == null ? SpaceViewModel.DefaultType : parkingSpace.MeterGroup.MeterGroupDesc ?? SpaceViewModel.DefaultType,
                                AssetId = parkingSpace.ParkingSpaceId,
                                AssetName = parkingSpace.DisplaySpaceNum ?? " ",
                               
                               // InventoryStatus = parkingSpace.SpaceStatus == null ? "" : parkingSpace.AssetState.AssetStateDesc,
                              //  OperationalStatus = parkingSpace.OperationalStatu == null ? "" : parkingSpace.OperationalStatu.OperationalStatusDesc,

                                InventoryStatus = mm.Meter.MeterState == null ? "" : mm.Meter.AssetState.AssetStateDesc,

                                OperationalStatus = mm.Meter.OperationalStatu.OperationalStatusDesc,
                                
                                Latitude = parkingSpace.Latitude ?? 0.0,
                                Longitude = parkingSpace.Longitude ?? 0.0,
                                AreaId2 = mm.AreaId2,
                                ZoneId = mm.ZoneId,
                                Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId) == null ? " " : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId).DisplayName,
                                Street = parkingSpace.Meter == null ? " " : parkingSpace.Meter.Location ?? " ",
                                DemandStatus = parkingSpace.DemandZone == null ? " " : parkingSpace.DemandZone.DemandZoneDesc ?? " ",

                                //------------COMMON COLUMNS
                                AssetModel = psd == null ? "" : psd.SpaceType1 == null ? "" : psd.SpaceType1.SpaceTypeDesc,

                                //------------SUMMARY GRID
                                SpacesCount = 1,
                                Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                                Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId).ZoneName,

                                //---------------CONFIGURATION GRID
                                DateInstalled = parkingSpace.InstallDate,
                                ConfigurationId = cps == null ? (long?)null : cps.ConfigProfileId,
                                ConfigCreationDate = cps == null ? (DateTime?)null : cps.CreationDate,
                                ConfigScheduleDate = cps == null ? (DateTime?)null : cps.ScheduledDate,
                                ConfigActivationDate = cps == null ? (DateTime?)null : cps.ActivationDate,

                                // RJH:  Space does not have an entry in VersionProfileMeter.

                                //MpvVersion = vpn == null ? "" : vpn.CommunicationVersion,
                                //SoftwareVersion = vpn == null ? "" : vpn.SoftwareVersion,
                                //FirmwareVersion = vpn == null ? "" : vpn.HardwareVersion,

                                //-------------OCCUPANCY GRID - only valid for spaces - 
                                MeterName = parkingSpace.Meter.MeterName ?? " ",
                                SensorName = parkingSpace.Sensors.FirstOrDefault() == null ? "" : parkingSpace.Sensors.FirstOrDefault().SensorName,
                                OperationalStatusDate = parkingSpace.OperationalStatusTime,
                                OccupancyStatus = sptc.OccupancyStatu == null ? "" : sptc.OccupancyStatu.StatusDesc,
                                OccupancyStatusDate = sptc.OccupancyDate,
                                NonComplianceStatus = sptc.NonCompliantStatu == null ? "" : sptc.NonCompliantStatu.NonCompliantStatusDesc,
                                NonComplianceStatusDate = sptc.DepartureTime,

                                //------------FUNCTIONAL STATUS GRID  - these are set after paging, filtering, etc
                                AlarmClass = aa.EventCode1.AlarmTier1.TierDesc,
                                AlarmCode = aa.EventCode1.EventDescVerbose,
                                AlarmTimeOfOccurance = aa.TimeOfOccurrance,
                                AlarmRepairTargetTime = aa.SLADue,
                                LocalTime = Now

                            };
            return items;
        }

        #endregion

        #region View Models

        public List<SimpleSpaceModel> GetAllAssets(int customerId)
        {

            return (from Meters in PemsEntities.Meters
                    where Meters.CustomerID == customerId && Meters.MeterName != null && Meters.AreaID == (int)AssetAreaId.Meter//&& spaces.HasSensor == false
                    select new SimpleSpaceModel()
                    {
                        MeterID = Meters.MeterId,
                        MeterName = Meters.MeterName
                    }).ToList();
        }


        public List<SimpleSpaceModel> SpacesWithoutSensors(int customerId, int meterID)
        {

            return (from spaces in PemsEntities.ParkingSpaces
                    where spaces.CustomerID == customerId && spaces.MeterId == meterID //&& spaces.HasSensor == false
                    select new SimpleSpaceModel()
                    {
                        SpaceId = spaces.ParkingSpaceId,
                        BayNumber = spaces.BayNumber
                    }).ToList();
        }



        /// <summary>
        /// Create and populate a <see cref="SpaceListModel"/>.  A SpaceListModel includes reference to 
        /// the associated sensor if on exists as well as bay number and space status.
        /// </summary>
        /// <param name="spaceId">Long int id of the space.</param>
        /// <returns>A populated <see cref="SpaceListModel"/></returns>
        public SpaceListModel GetSpaceListModel(long spaceId)
        {
            var spaceListModel = new SpaceListModel { AssetId = spaceId };

            var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);
            if (parkingSpace != null)
            {
                spaceListModel.MeterId = parkingSpace.MeterId;
                spaceListModel.MeterAreaId = parkingSpace.AreaId;

                spaceListModel.Name = parkingSpace.DisplaySpaceNum ?? "";

                spaceListModel.Latitude = parkingSpace.Latitude ?? 0.0;
                spaceListModel.Longitude = parkingSpace.Longitude ?? 0.0;

                spaceListModel.DemandStatus = parkingSpace.DemandZone == null ? " " : parkingSpace.DemandZone.DemandZoneDesc ?? " ";
                spaceListModel.DemandStatusId = parkingSpace.DemandZoneId ?? -1;

                spaceListModel.OperationalStatus = parkingSpace.OperationalStatus ?? (int)OperationalStatusType.Inactive;
                spaceListModel.OrgOperationalStatus = spaceListModel.OperationalStatus;

                spaceListModel.Street = parkingSpace.Meter == null ? " " : parkingSpace.Meter.Location ?? " ";

                if (parkingSpace.HasSensor ?? false)
                {
                    var sensor = (new SensorFactory(ConnectionStringName, Now)).GetViewModel(spaceId);
                    spaceListModel.Sensor = sensor;
                    spaceListModel.SensorId = sensor == null ? "" : sensor.AssetId.ToString();
                }
                else
                {
                    spaceListModel.Sensor = null;
                    spaceListModel.SensorId = "";
                }

                spaceListModel.BayNumber = parkingSpace.BayNumber;
                spaceListModel.BayName = parkingSpace.DisplaySpaceNum;

                spaceListModel.State = (AssetStateType)(parkingSpace.SpaceStatus ?? 0);

                // Indicates that this is a model from db and has not been edited by ui
                spaceListModel.IsChanged = false;

                // False indicates that this model is from db.  There are other possible properties that may indicate
                // this but a flag is the most fool-proof.
                spaceListModel.IsNew = false;
            }

            return spaceListModel;
        }

        public SpaceViewModel GetViewModel(long spaceId, SpaceViewModel model = null)
        {
            var viewModel = model == null ? new SpaceViewModel { AssetId = spaceId } : model;

            var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);

            viewModel.GlobalId = parkingSpace.ParkingSpaceId.ToString();
            viewModel.Type = parkingSpace.MeterGroup == null ? SpaceViewModel.DefaultType : parkingSpace.MeterGroup.MeterGroupDesc ?? SpaceViewModel.DefaultType;
            viewModel.TypeId = (int)MeterGroups.Space;

            var parkingSpaceDetails = parkingSpace.ParkingSpaceDetails.FirstOrDefault(m => m.ParkingSpaceId == parkingSpace.ParkingSpaceId);
            viewModel.AssetModel = parkingSpaceDetails == null ? "" : parkingSpaceDetails.SpaceType1 == null ? "" : parkingSpaceDetails.SpaceType1.SpaceTypeDesc;
            viewModel.Name = parkingSpace.DisplaySpaceNum ?? "";

            viewModel.BayNumber = parkingSpace.BayNumber;

            viewModel.CustomerId = parkingSpace.CustomerID;
            viewModel.AreaId = parkingSpace.AreaId;

            viewModel.Latitude = parkingSpace.Latitude ?? 0.0;
            viewModel.Longitude = parkingSpace.Longitude ?? 0.0;

            viewModel.DemandStatus = parkingSpace.DemandZone == null ? " " : parkingSpace.DemandZone.DemandZoneDesc ?? " ";

            viewModel.Street = parkingSpace.Meter == null ? " " : parkingSpace.Meter.Location ?? " ";

            var meterMap =
                PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == parkingSpace.CustomerID && m.Areaid == parkingSpace.AreaId && m.MeterId == parkingSpace.MeterId);

            if (meterMap != null)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == parkingSpace.CustomerID && m.AreaID == meterMap.AreaId2);
                viewModel.Area = area == null ? "" : area.AreaName;
                viewModel.AreaId2 = area == null ? 0 : area.AreaID;

                viewModel.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                viewModel.ZoneId = meterMap.Zone == null ? 0 : meterMap.Zone.ZoneId;

                viewModel.Suburb = meterMap.CustomGroup11 == null ? "" : meterMap.CustomGroup11.DisplayName ?? "";
            }
            else
            {
                viewModel.Area = "";
                viewModel.AreaId2 = 0;
                viewModel.Zone = "";
                viewModel.ZoneId = 0;
                viewModel.Suburb = "";
            }

            // Get associated meter.
            viewModel.Meter = parkingSpace.Meter == null ? new AssetIdentifier() : new AssetIdentifier()
            {
                CustomerId = parkingSpace.Meter.CustomerID,
                AreaId = parkingSpace.Meter.AreaID,
                AssetId = parkingSpace.Meter.MeterId
            };

            // Get associated sensor.
            if (parkingSpace.HasSensor ?? false)
            {
                var sensor = (new SensorFactory(ConnectionStringName, Now)).GetViewModel(spaceId);
                viewModel.Sensor = sensor ?? new AssetIdentifier();
            }
            else
            {
                viewModel.Sensor = new AssetIdentifier();
            }

            // Get configuration details
            viewModel.Configuration = new SpaceConfigurationModel();

            // Date "Installed"
            viewModel.Configuration.DateInstalled = parkingSpace.InstallDate;

            // Get active tariff
            DateTime activeTariffDate = DateTime.Now;
            var configProfileSpace = parkingSpace.ConfigProfileSpaces.FirstOrDefault(m => m.ConfigStatus == (int)ConfigStatus.Active);
            if (configProfileSpace != null)
            {
                viewModel.Configuration.ActiveTariff = configProfileSpace.ConfigProfile == null ? "-" : configProfileSpace.ConfigProfile.ConfigurationName;
                viewModel.Configuration.TariffConfigProfileId = configProfileSpace.ConfigProfileSpaceId;
                activeTariffDate = configProfileSpace.ActivationDate ?? activeTariffDate;

                viewModel.Configuration.ActiveTariffDateActivated = configProfileSpace.ActivationDate;
                viewModel.Configuration.ActiveTariffDateCreated = configProfileSpace.CreationDate;
                viewModel.Configuration.ActiveTariffDateScheduled = configProfileSpace.ScheduledDate;
            }
            else
            {
                viewModel.Configuration.ActiveTariff = "";
                viewModel.Configuration.TariffConfigProfileId = 0;
            }

            // Get pending tariff (if any)
            var pendingConfigProfileSpace = parkingSpace.ConfigProfileSpaces.Where(m => m.ConfigStatus == (int)ConfigStatus.Active && m.ScheduledDate > activeTariffDate).OrderBy(m => m.ScheduledDate).FirstOrDefault();
            if (pendingConfigProfileSpace != null)
            {
                viewModel.Configuration.PendingTariff = pendingConfigProfileSpace.ConfigProfile == null ? "-" : pendingConfigProfileSpace.ConfigProfile.ConfigurationName;
                viewModel.Configuration.PendingTariffConfigProfileId = pendingConfigProfileSpace.ConfigProfileSpaceId;
                // Pending to active date
                viewModel.Configuration.PendingTariffDateScheduled = pendingConfigProfileSpace.ScheduledDate;
                viewModel.Configuration.PendingTariffDateCreated = pendingConfigProfileSpace.CreationDate;
            }
            else
            {
                viewModel.Configuration.PendingTariff = "";
                viewModel.Configuration.PendingTariffConfigProfileId = 0;
            }

            // Get occupancy status
            viewModel.Occupancy = new SpaceStatusModel();

            viewModel.Occupancy.OperationalStatus = parkingSpace.OperationalStatu == null ? "" : parkingSpace.OperationalStatu.OperationalStatusDesc;
            viewModel.Occupancy.OperationalStatusDate = parkingSpace.OperationalStatusTime;

            var sensorPaymentTransaction = parkingSpace.SensorPaymentTransactions.FirstOrDefault(m => m.ParkingSpaceId == parkingSpace.ParkingSpaceId);
            if (sensorPaymentTransaction != null)
            {
                viewModel.Occupancy.OccupancyStatus = sensorPaymentTransaction.OccupancyStatu == null ? "" : sensorPaymentTransaction.OccupancyStatu.StatusDesc ?? "";
                viewModel.Occupancy.OccupancyStatusDate = sensorPaymentTransaction.OccupancyDate;
                viewModel.Occupancy.NonComplianceStatus = sensorPaymentTransaction.NonCompliantStatu == null ? "" : sensorPaymentTransaction.NonCompliantStatu.NonCompliantStatusDesc ?? "";
                viewModel.Occupancy.NonComplianceStatusDate = sensorPaymentTransaction.DepartureTime;
            }
            else
            {
                viewModel.Occupancy.OccupancyStatus = "";
                viewModel.Occupancy.NonComplianceStatus = "";
            }

            // Is asset active?
            viewModel.State = (AssetStateType)(parkingSpace.SpaceStatus ?? 0);

            // Get last update by information
            CreateAndLastUpdate(viewModel);

            return viewModel;
        }

        /// <summary>
        /// Update a <see cref="SpaceViewModel"/> with any pending changes.
        /// </summary>
        /// <param name="model">A <see cref="SpaceViewModel"/> instance to update.</param>
        /// <returns>The updated <see cref="SpaceViewModel"/> model.</returns>
        public SpaceViewModel GetPendingViewModel(long spaceId, SpaceViewModel existingModel = null)
        {
            SpaceViewModel model = GetViewModel(spaceId, existingModel);


            // Get the pending changes from [AssetPending] 
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == model.AssetId && m.AssetType == model.TypeId);

            // Get out if there are no pending changes.
            if (assetPending == null)
            {
                return model;
            }

            // Determine which pending changes are valid.
            if (assetPending.AssetModel.HasValue)
            {
                model.AssetModel = PemsEntities.SpaceTypes.FirstOrDefault(m => m.SpaceTypeId == assetPending.AssetModel.Value).SpaceTypeDesc;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.DisplaySpaceNum))
            {
                model.Name = assetPending.DisplaySpaceNum;
            }

            // Lat/Long
            if (assetPending.Latitude.HasValue)
            {
                model.Latitude = assetPending.Latitude.Value;
            }

            if (assetPending.Longitude.HasValue)
            {
                model.Longitude = assetPending.Longitude.Value;
            }

            if (assetPending.DemandStatus.HasValue)
            {
                var demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneId == assetPending.DemandStatus.Value);
                model.DemandStatus = demandZone == null ? string.Empty : demandZone.DemandZoneDesc;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.LocationMeters))
            {
                model.Street = assetPending.LocationMeters;
            }

            // Area, Zone and Suburb
            if (assetPending.MeterMapAreaId2.HasValue)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.AreaID == assetPending.MeterMapAreaId2.Value);
                model.Area = area == null ? "" : area.AreaName;
                model.AreaId2 = area == null ? 0 : area.AreaID;
            }

            if (assetPending.MeterMapZoneId.HasValue)
            {
                var zone = PemsEntities.Zones.FirstOrDefault(m => m.ZoneId == assetPending.MeterMapZoneId.Value && m.customerID == model.CustomerId);
                model.Zone = zone == null ? "" : zone.ZoneName ?? "";
            }

            if (assetPending.MeterMapSuburbId.HasValue)
            {
                var suburb = PemsEntities.CustomGroup1.FirstOrDefault(m => m.CustomGroupId == assetPending.MeterMapSuburbId.Value && m.CustomerId == model.CustomerId);
                model.Suburb = suburb == null ? "" : suburb.DisplayName ?? "";
            }

            // Get associated meter.
            if (assetPending.AssociatedMeterId.HasValue)
            {
                if (model.Meter == null)
                {
                    model.Meter = new AssetIdentifier();
                }
                model.Meter.CustomerId = model.CustomerId;
                model.Meter.AreaId = assetPending.AssociatedMeterAreaId ?? 0;
                model.Meter.AssetId = assetPending.AssociatedMeterId ?? 0;
            }

            // Get associated sensor.
            if (assetPending.MeterMapSensorId.HasValue)
            {
                if (model.Sensor == null)
                {
                    model.Sensor = new AssetIdentifier();
                }
                model.Sensor.CustomerId = model.CustomerId;
                model.Sensor.AreaId = (int)AssetAreaId.Sensor;
                model.Sensor.AssetId = assetPending.MeterMapSensorId ?? 0;
            }

            // Get configuration details
            // Date "Installed"
            if (assetPending.DateInstalled.HasValue)
            {
                model.Configuration.DateInstalled = assetPending.DateInstalled.Value;
            }

            // Asset state?
            if (assetPending.OperationalStatus.HasValue)
            {
                model.Status = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == assetPending.OperationalStatus.Value).OperationalStatusDesc;
                model.StatusDate = assetPending.OperationalStatusTime;
            }

            // Get target ActivationDate
            model.ActivationDate = assetPending.RecordMigrationDateTime.HasValue ? assetPending.RecordMigrationDateTime.Value.ToShortDateString() : "";

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns a <see cref="SpaceViewModel"/> based upon the indicated audit record.
        /// </summary>
        /// <param name="spaceId">The asset id</param>
        /// <param name="auditId">The audit record id</param>
        /// <returns></returns>
        public SpaceViewModel GetHistoricViewModel(long spaceId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.ParkingSpacesAudits.FirstOrDefault(m => m.ParkingSpacesAuditId == auditId));
        }

        /// <summary>
        /// Private method to create a <see cref="SpaceViewModel"/> from a <see cref="SensorsAudit"/> record.
        /// </summary>
        /// <param name="auditModel">An instance of <see cref="SensorsAudit"/></param>
        /// <returns>An instance of <see cref="SpaceViewModel"/></returns>
        private SpaceViewModel CreateHistoricViewModel(ParkingSpacesAudit auditModel)
        {
            var model = new SpaceViewModel()
            {
                CustomerId = auditModel.CustomerID,
                AreaId = 0,
                AssetId = auditModel.ParkingSpaceId
            };

            model.GlobalId = auditModel.ParkingSpaceId.ToString();

            model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.ParkingSpaceType) == null ? GatewayViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.ParkingSpaceType).MeterGroupDesc ?? GatewayViewModel.DefaultType;
            model.TypeId = auditModel.ParkingSpaceType ?? (int)MeterGroups.Space;

            model.Name = auditModel.DisplaySpaceNum ?? "";

            model.BayNumber = auditModel.BayNumber;

            model.CustomerId = auditModel.CustomerID;
            model.AreaId = auditModel.AreaId;

            model.Latitude = auditModel.Latitude ?? 0.0;
            model.Longitude = auditModel.Longitude ?? 0.0;

            var demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneId == auditModel.DemandZoneId);
            model.DemandStatus = demandZone == null ? "" : demandZone.DemandZoneDesc ?? "";


            // Need to see if there is a PemsEntities.MeterMapAudits for this sensor id within an hour.
            DateTime fromDate = auditModel.UpdateDateTime.AddHours(-1);
            DateTime toDate = auditModel.UpdateDateTime.AddHours(1);

            var meterMapAudit = PemsEntities.MeterMapAudits.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.Areaid == auditModel.AreaId
                && m.AuditDateTime > fromDate && m.AuditDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if (meterMapAudit == null)
            {
                meterMapAudit = PemsEntities.MeterMapAudits.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.Areaid == auditModel.AreaId
                                                                                 && m.AuditDateTime <= auditModel.UpdateDateTime);
            }

            // Is there still is not a meterMapAudit then get present MeterMap, otherwise use meterMapAudit
            MeterMap meterMap = null;
            if (meterMapAudit == null)
            {
                meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.Areaid == auditModel.AreaId);
            }

            // Now get Zone, Area and Suburb and Maintenance Route
            // Area
            model.AreaId2 = meterMapAudit == null ? (meterMap == null ? 0 : meterMap.AreaId2) : meterMapAudit.AreaId2;
            var area = model.AreaId2.HasValue ? PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.AreaID == model.AreaId2) : null;
            model.Area = area == null ? "" : area.AreaName;

            // Zone
            model.ZoneId = meterMapAudit == null ? (meterMap == null ? null : meterMap.ZoneId) : meterMapAudit.ZoneId;
            var zone = PemsEntities.Zones.FirstOrDefault(m => m.customerID == auditModel.CustomerID && m.ZoneId == model.ZoneId);
            model.Zone = zone == null ? "" : zone.ZoneName ?? "";

            // Suburb
            model.SuburbId = meterMapAudit == null ? (meterMap == null ? null : meterMap.CustomGroup1) : meterMapAudit.CustomGroup1;
            var suburb = PemsEntities.CustomGroup1.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.CustomGroupId == model.SuburbId);
            model.Suburb = suburb == null ? "" : suburb.DisplayName ?? "";


            // Get associated meter.
            model.Meter = new AssetIdentifier()
            {
                CustomerId = auditModel.CustomerID,
                AreaId = auditModel.AreaId,
                AssetId = auditModel.MeterId
            };

            // Get street based on meter id at audit time.
            var meterAudit = PemsEntities.MetersAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.AreaID == auditModel.AreaId
                && m.UpdateDateTime > fromDate && m.UpdateDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if (meterAudit == null)
            {
                meterAudit = PemsEntities.MetersAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.AreaID == auditModel.AreaId
                                                                                 && m.UpdateDateTime <= auditModel.UpdateDateTime);
            }

            // Is there still is not a meterAudit then get present Meter, otherwise use meterAudit
            Meter meter = null;
            if (meterAudit == null)
            {
                meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.AreaID == auditModel.AreaId);
            }

            model.Street = meterAudit == null ? (meter == null ? "" : meter.Location ?? "") : meterAudit.Location ?? "";

            // Get associated sensor.
            if (auditModel.HasSensor ?? false)
            {
                // Get sensor at audit record time.
                var sensorAudit = PemsEntities.SensorsAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.ParkingSpaceId == auditModel.ParkingSpaceId
                    && m.UpdateDateTime > fromDate && m.UpdateDateTime < toDate);
                // If no audit record then get audit record most recent in the past.
                if (sensorAudit == null)
                {
                    sensorAudit = PemsEntities.SensorsAudits.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.ParkingSpaceId == auditModel.ParkingSpaceId
                                                                                     && m.UpdateDateTime <= auditModel.UpdateDateTime);
                }

                // Is there still is not a meterMapAudit then get present MeterMap, otherwise use meterMapAudit
                Sensor sensor = null;
                if (sensorAudit == null)
                {
                    sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == auditModel.CustomerID && m.ParkingSpaceId == auditModel.ParkingSpaceId);
                }

                model.Sensor = new AssetIdentifier()
                {
                    CustomerId = sensorAudit == null ? (sensor == null ? 0 : sensor.CustomerID) : sensorAudit.CustomerID,
                    AreaId = (int)AssetAreaId.Sensor,
                    AssetId = sensorAudit == null ? (sensor == null ? 0 : sensor.SensorID) : sensorAudit.SensorID
                };
            }
            else
            {
                model.Sensor = new AssetIdentifier();
            }


            SpaceTypeType spaceType = SpaceTypeType.Undefined;
            if (model.Meter.AssetId > 0 && model.Sensor.AssetId > 0)
            {
                spaceType = SpaceTypeType.MeteredWithSensor;
            }
            else if (model.Meter.AssetId > 0 && model.Sensor.AssetId == 0)
            {
                spaceType = SpaceTypeType.MeteredSpace;
            }
            else if (model.Meter.AssetId == 0 && model.Sensor.AssetId > 0)
            {
                spaceType = SpaceTypeType.SensorOnly;
            }

            if (spaceType == SpaceTypeType.Undefined)
            {
                model.AssetModel = "";
            }
            else
            {
                model.AssetModel = PemsEntities.SpaceTypes.FirstOrDefault(m => m.SpaceTypeId == (int)spaceType).SpaceTypeDesc;
            }


            // Get configuration details
            model.Configuration = new SpaceConfigurationModel();

            // Date "Installed"
            model.Configuration.DateInstalled = auditModel.InstallDate;

            // Get active tariff at time of audit record
            DateTime activeTariffDate = auditModel.UpdateDateTime;

            var configProfileSpaceAuditList = PemsEntities.ConfigProfileSpaceAudits.Where(
                m => m.ParkingSpaceId == auditModel.ParkingSpaceId &&
                    m.CustomerId == auditModel.CustomerID && m.ActivationDate <= auditModel.UpdateDateTime && m.ConfigStatus == (int)ConfigStatus.Active)
                                                          .OrderBy(m => m.ActivationDate).ToList();

            // Were any records found?
            if (configProfileSpaceAuditList.Any())
            {
                // Use that last configProfileSpaceAuditList which is the most recent.
                model.Configuration.TariffConfigProfileId = configProfileSpaceAuditList.Last().ConfigProfileSpaceId;
                model.Configuration.ActiveTariffDateActivated = configProfileSpaceAuditList.Last().ActivationDate;
                model.Configuration.ActiveTariffDateCreated = configProfileSpaceAuditList.Last().CreationDate;
                model.Configuration.ActiveTariffDateScheduled = configProfileSpaceAuditList.Last().ScheduledDate;

                // Get the name of the configuration.
                var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault(m => m.ConfigProfileId == model.Configuration.TariffConfigProfileId);
                model.Configuration.ActiveTariff = configProfile == null ? "" : configProfile.ConfigurationName ?? "";
            }
            else
            {
                // Get the last active ConfigProfileSpace for the 
                var configProfileSpaceList = PemsEntities.ConfigProfileSpaces.Where(
                    m => m.ParkingSpaceId == auditModel.ParkingSpaceId &&
                        m.CustomerId == auditModel.CustomerID && m.ActivationDate <= auditModel.UpdateDateTime && m.ConfigStatus == (int)ConfigStatus.Active)
                                                              .OrderBy(m => m.ActivationDate).ToList();

                // Were any records found?
                if (configProfileSpaceList.Any())
                {
                    // Use that last configProfileSpaceAuditList which is the most recent.
                    model.Configuration.TariffConfigProfileId = configProfileSpaceList.Last().ConfigProfileSpaceId;
                    model.Configuration.ActiveTariffDateActivated = configProfileSpaceList.Last().ActivationDate;
                    model.Configuration.ActiveTariffDateCreated = configProfileSpaceList.Last().CreationDate;
                    model.Configuration.ActiveTariffDateScheduled = configProfileSpaceList.Last().ScheduledDate;

                    // Get the name of the configuration.
                    var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault(m => m.ConfigProfileId == model.Configuration.TariffConfigProfileId);
                    model.Configuration.ActiveTariff = configProfile == null ? "" : configProfile.ConfigurationName ?? "";
                }
            }
            model.ConfigurationName = model.Configuration.ActiveTariff;

            // Skip getting pending tariff at time of audit record
            model.Configuration.PendingTariff = "";
            model.Configuration.PendingTariffConfigProfileId = 0;

            // Get status
            model.Occupancy = new SpaceStatusModel();
            model.Occupancy.OperationalStatus = auditModel.OperationalStatus.HasValue
                               ? PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == auditModel.OperationalStatus.Value).OperationalStatusDesc
                               : "";
            model.Occupancy.OperationalStatusDate = auditModel.OperationalStatusTime;

            // Updated by
            model.LastUpdatedById = auditModel.UserId;
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReason = (AssetPendingReasonType)(auditModel.AssetPendingReasonId ?? 0);
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;

            // Record date
            model.RecordDate = auditModel.UpdateDateTime;

            // Audit record id
            model.AuditId = auditModel.ParkingSpacesAuditId;

            return model;
        }

        public AssetHistoryModel GetHistoricListViewModel(Int64 spaceId)
        {
            var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);

            var model = new AssetHistoryModel()
            {
                CustomerId = parkingSpace.CustomerID,
                AssetId = spaceId,
                Type = parkingSpace.MeterGroup == null ? SpaceViewModel.DefaultType : parkingSpace.MeterGroup.MeterGroupDesc ?? SpaceViewModel.DefaultType,
                TypeId = (int)MeterGroups.Space,
                Name = parkingSpace.DisplaySpaceNum ?? "",
                Street = ""
            };

            return model;
        }

        /// <summary>
        /// Get a list of all spaces that do not have a sensor assigned.
        /// </summary>
        /// <param name="customerId">Integer id of customer.</param>
        /// <returns></returns>
        public List<SpaceListModel> GetUnsensoredSpaces(int customerId)
        {
            List<SpaceListModel> list = new List<SpaceListModel>();

            var spaces = PemsEntities.ParkingSpaces.Where(m => m.CustomerID == customerId && m.HasSensor == false);
            foreach (var space in spaces)
            {
                list.Add(GetSpaceListModel(space.ParkingSpaceId));
            }

            return list;
        }

        #endregion

        #region Edit Models

        public SpaceEditModel GetEditModel(Int64 spaceId)
        {
            var space = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);

            var model = BuildSpaceEditModel(space);

            // If there is an asset pending record then update edit model with associated values.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == spaceId && m.AssetType == (int)MeterGroups.Space);

            if (assetPending != null)
            {
                AddAssetPendingToSpaceEditModel(assetPending, model, space);
            }


            // Get types of spaces list.
            model.AssetModel = SpaceTypeList(model.AssetModelId);

            GetZoneList(model);

            GetSuburbList(model);

            GetAreaList(model);

            //model.DemandStatus = (from dz in PemsEntities.DemandZones
            model.DemandStatus = (from dzc in PemsEntities.DemandZoneCustomers
                                  join dz in PemsEntities.DemandZones on dzc.DemandZoneId equals dz.DemandZoneId
                                  where dzc.CustomerId == model.CustomerId
                                  where dzc.IsDisplay != null
                                  where (bool)dzc.IsDisplay == true
                                  where dz.DemandZoneDesc !=null
                                  //where (dzc.IsDisplay != null && (bool)dzc.IsDisplay && dzc.CustomerId == model.CustomerId && dzc.DemandZone.DemandZoneDesc != "")
                                  select new SelectListItemWrapper()
                                  {
                                      Selected = dzc.DemandZoneId == model.DemandStatusId,
                                      Text = dz.DemandZoneDesc,
                                      ValueInt = dzc.DemandZoneId
                                  }).ToList();
            model.DemandStatus.Insert(0, new SelectListItemWrapper()
            {
                Selected = model.DemandStatusId == 0,
                Text = " ",
                Value = "0"
            });

            // Get list of possible meters.
            model.AssociatedMeter = (from mm in PemsEntities.MeterMaps
                                     where mm.Customerid == model.CustomerId
                                     where mm.Areaid == model.AreaId
                                     select new SelectListItemWrapper()
                                     {
                                         Selected = mm.MeterId == model.AssociatedMeterId,
                                         TextInt = mm.MeterId,
                                         ValueInt = mm.MeterId
                                     }).ToList();
            // Add a "No Meter" selection
            model.AssociatedMeter.Insert(0, new SelectListItemWrapper()
            {
                Selected = model.AssociatedMeterId == 0,
                Text = "No Meter",
                Value = "0"
            });

            // Get list of possible sensors
            model.AssociatedSensor = (from s in PemsEntities.Sensors
                                      where s.CustomerID == model.CustomerId
                                      select new SelectListItemWrapper()
                                      {
                                          Selected = s.SensorID == model.AssociatedSensorId,
                                          Text = s.Description,
                                          ValueInt = s.SensorID
                                      }).ToList();
            // Add a "No Sensor" selection
            model.AssociatedSensor.Insert(0, new SelectListItemWrapper()
            {
                Selected = model.AssociatedSensorId == 0,
                Text = "No Sensor",
                Value = "0"
            });

            // Get types of state list
            GetAssetStateList(model);
            // Initialize ActivationDate if needed and set to midnight.
            if (!model.ActivationDate.HasValue)
            {
                model.ActivationDate = Now;
            }
            model.ActivationDate = SetToMidnight(model.ActivationDate.Value);
            return model;
        }

        public void RefreshEditModel(SpaceEditModel editModel)
        {
            var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == editModel.AssetId);


            // Get sensor models.
            editModel.AssetModel = new List<SelectListItemWrapper>();
            editModel.AssetModel.Add(new SelectListItemWrapper()
            {
                Selected = true,
                Text = "Space",
                Value = "0"
            });


            // Demand Zone
            //editModel.DemandStatus = (from dz in PemsEntities.DemandZones
            editModel.DemandStatus = (from dzc in PemsEntities.DemandZoneCustomers
                                      join dz in PemsEntities.DemandZones on dzc.DemandZoneId equals dz.DemandZoneId
                                      where dzc.CustomerId == editModel.CustomerId
                                      where dzc.IsDisplay != null
                                      where (bool)dzc.IsDisplay == true
                                      where dz.DemandZoneDesc != null
                                      select new SelectListItemWrapper()
                                      {
                                          Selected = dzc.DemandZoneId == editModel.DemandStatusId,
                                          Text = dz.DemandZoneDesc,
                                          ValueInt = dzc.DemandZoneId
                                      }).ToList();

            editModel.DemandStatus.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.DemandStatusId == -1,
                Text = " ",
                Value = "-1"
            });


            // Zone and suburb
            var meterMap =
                PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == parkingSpace.CustomerID && m.Areaid == parkingSpace.AreaId && m.MeterId == parkingSpace.MeterId);

            // Get zone list
            GetZoneList(editModel);

            // Get suburb list
            GetSuburbList(editModel);

            // Get Areas list
            GetAreaList(editModel);


            // Get list of possible meters.
            editModel.AssociatedMeter = (from mm in PemsEntities.MeterMaps
                                         where mm.Customerid == editModel.CustomerId
                                         where mm.Areaid == editModel.AreaId
                                         select new SelectListItemWrapper()
                                         {
                                             Selected = mm.MeterId == editModel.AssociatedMeterId,
                                             TextInt = mm.MeterId,
                                             ValueInt = mm.MeterId
                                         }).ToList();
            // Add a "No Meter" selection
            editModel.AssociatedMeter.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.AssociatedMeterId == -1,
                Text = "No Meter",
                Value = "-1"
            });


            // Get list of possible sensors
            editModel.AssociatedSensor = (from s in PemsEntities.Sensors
                                          where s.CustomerID == editModel.CustomerId
                                          select new SelectListItemWrapper()
                                          {
                                              Selected = s.SensorID == editModel.AssociatedSensorId,
                                              Text = s.Description,
                                              ValueInt = s.SensorID
                                          }).ToList();
            // Add a "No Sensor" selection
            editModel.AssociatedSensor.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.AssociatedSensorId == -1,
                Text = "No Sensor",
                Value = "-1"
            });

            // Get list of potential active tariffs
            editModel.Configuration.PendingTariff = (from cp in PemsEntities.ConfigProfiles
                                                     select new SelectListItemWrapper()
                                                     {
                                                         Selected = cp.ConfigProfileId == editModel.Configuration.PendingTariffConfigProfileId,
                                                         Text = cp.ConfigurationName,
                                                         ValueInt64 = cp.ConfigProfileId
                                                     }).ToList();
            // Add a "No Sensor" selection
            editModel.Configuration.PendingTariff.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.Configuration.PendingTariffConfigProfileId == -1,
                Text = "No Pending Tariff",
                Value = "-1"
            });

            //// Operational status of space.
            //GetOperationalStatusList(editModel, editModel.Status.StatusId);

            // Get state list
            GetAssetStateList(editModel);

        }

        public void SetEditModel(SpaceEditModel model)
        {
            // Note:  At this time most changes to the space are written to the [AssetPending] table.

            // Get the existing model so can determine what has changed
            var originalModel = GetEditModel(model.AssetId);

            // Write changes to [AssetPending] table.
            (new PendingFactory(ConnectionStringName, Now)).Save(model, originalModel);

            // Note:  The following is commented out but remains since am unsure that the AssetPending table
            // will do what system requires.

            // Note:  Pending tariff is not supported in [AssetPending] table.  It is supported in the table
            // [ConfigProfileSpace].  Those updates remain in this method.


            // Get the parking space.
            var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == model.AssetId);

            //// Asset name
            //parkingSpace.DisplaySpaceNum = editModel.Name;


            //// Parking space "Model"
            //var parkingSpaceDetail = parkingSpace.ParkingSpaceDetails.FirstOrDefault(m => m.ParkingSpaceId == parkingSpace.ParkingSpaceId);
            //if ( parkingSpaceDetail != null )
            //{
            //    parkingSpaceDetail.SpaceType = editModel.AssetModelId;
            //}
            //else
            //{
            //    PemsEntities.ParkingSpaceDetails.Add( new ParkingSpaceDetail()
            //        {
            //            ParkingSpaceId = parkingSpace.ParkingSpaceId,
            //            SpaceType = editModel.AssetModelId,
            //            HBInterval = 15,
            //            ExpiryLevel = "5,-5",
            //            PercentLevel = "30,60"
            //        } );
            //}


            //// Street
            ////parkingSpace.Location = editModel.Street;

            //// Set Zone and Suburb
            //var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == editModel.CustomerId && m.Areaid == editModel.AreaId && m.MeterId == editModel.AssetId);
            //if (meterMap != null)
            //{
            //    meterMap.AreaId2 = editModel.AreaListId;
            //    meterMap.ZoneId = editModel.ZoneId;
            //    meterMap.CustomGroup1 = editModel.SuburbId;
            //}

            //// Lat/Long
            //parkingSpace.Latitude = editModel.Latitude;
            //parkingSpace.Longitude = editModel.Longitude;

            //// Demand Zone
            //parkingSpace.DemandZoneId = editModel.DemandStatusId == 0 ? null : editModel.DemandStatusId;

            //// Associated meter. Note:  This is a required field and will not be equal to 0.
            //parkingSpace.MeterId = editModel.AssociatedMeterId;


            //// Associated sensor. A space can have a sensor or no sensor.
            //if (editModel.AssociatedSensorId == 0)
            //{
            //    // Remove any associated sensor if there is one.
            //    parkingSpace.HasSensor = false;
            //}
            //else
            //{
            //    var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == editModel.CustomerId && m.SensorID == editModel.AssociatedSensorId);
            //    if (sensor != null)
            //    {
            //        sensor.ParkingSpaceId = editModel.AssetId;
            //        parkingSpace.HasSensor = true;
            //    }

            //}

            // Active tariff
            var configProfileSpace = parkingSpace.ConfigProfileSpaces.FirstOrDefault(m => m.ConfigStatus == (int)ConfigStatus.Active);
            DateTime activeTariffDate = DateTime.Now;
            if (configProfileSpace != null)
            {
                activeTariffDate = configProfileSpace.ActivationDate ?? activeTariffDate;
            }
            //if (editModel.Configuration.TariffConfigProfileId <= 0)
            //{
            //    // Remove any active tariff
            //    if (configProfileSpace != null)
            //    {
            //        configProfileSpace.ConfigStatus = (int)TariffStatus.Inactive;
            //    }
            //}
            //else
            //{
            //    // If there is no active Tariff then create one.
            //    if (configProfileSpace == null)
            //    {
            //        // Create new configProfileSpace
            //        configProfileSpace = new ConfigProfileSpace()
            //        {
            //            ConfigProfileId = editModel.Configuration.TariffConfigProfileId,
            //            CustomerId = editModel.CustomerId,
            //            ParkingSpaceId = parkingSpace.ParkingSpaceId,
            //            CreateDatetime = DateTime.Now
            //        };
            //        PemsEntities.ConfigProfileSpaces.Add(configProfileSpace);
            //        configProfileSpace.ConfigStatus = (int)TariffStatus.Active;
            //        configProfileSpace.ActivationDate = DateTime.Now;
            //    }
            //    else
            //    {
            //        if (configProfileSpace.ConfigProfileId != editModel.Configuration.TariffConfigProfileId)
            //        {
            //            // If active tariff is not last active tariff, set last active tariff ConfigStatus 

            //            // Set old "Active Tariff" to inactive.
            //            configProfileSpace.ConfigStatus = (int)TariffStatus.Inactive;

            //            // Find new tariff and set to active.
            //            configProfileSpace = parkingSpace.ConfigProfileSpaces.FirstOrDefault(m => m.ConfigProfileId == editModel.Configuration.TariffConfigProfileId
            //                                                                                       && m.CustomerId == editModel.CustomerId &&
            //                                                                                       m.ParkingSpaceId == parkingSpace.ParkingSpaceId);
            //            if (configProfileSpace == null)
            //            {
            //                // Create new configProfileSpace
            //                configProfileSpace = new ConfigProfileSpace()
            //                {
            //                    ConfigProfileId = editModel.Configuration.TariffConfigProfileId,
            //                    CustomerId = editModel.CustomerId,
            //                    ParkingSpaceId = parkingSpace.ParkingSpaceId,
            //                    CreateDatetime = DateTime.Now
            //                };
            //                PemsEntities.ConfigProfileSpaces.Add(configProfileSpace);
            //            }
            //            configProfileSpace.ConfigStatus = (int)TariffStatus.Active;
            //            configProfileSpace.ActivationDate = DateTime.Now;
            //        }
            //        else
            //        {
            //            activeTariffDate = configProfileSpace.ActivationDate.HasValue ? configProfileSpace.ActivationDate.Value : DateTime.Now;
            //        }
            //    }
            //}


            // Pending tariff.
            if (model.Configuration.PendingTariffConfigProfileId > -1)
            {
                // If this pending tariff alread exists as pending then adjust the schedule date.
                // If it does not exist then add it as a pending tariff.
                var pendingConfigProfileSpace = parkingSpace.ConfigProfileSpaces.FirstOrDefault(m => m.ConfigStatus != 1
                    && m.ScheduledDate > activeTariffDate && m.ConfigProfileId == model.Configuration.PendingTariffConfigProfileId);

                if (pendingConfigProfileSpace == null)
                {
                    pendingConfigProfileSpace = new ConfigProfileSpace()
                    {
                        ConfigProfileId = model.Configuration.PendingTariffConfigProfileId,
                        CustomerId = model.CustomerId,
                        ParkingSpaceId = model.AssetId,
                        CreationDate = DateTime.Now,
                        CreateDatetime = DateTime.Now,
                        ConfigStatus = (int)ConfigStatus.Active,
                        UserId = WebSecurity.CurrentUserId
                    };
                    PemsEntities.ConfigProfileSpaces.Add(pendingConfigProfileSpace);
                }
                pendingConfigProfileSpace.ScheduledDate = model.Configuration.PendingToActiveDate;

                PemsEntities.SaveChanges();
                Audit(pendingConfigProfileSpace);
            }

            //// Is asset active?
            //parkingSpace.SpaceStatus = (int)(editModel.StateId == 1 ? AssetStateType.Current : AssetStateType.Historic);

            // Save changes
            PemsEntities.SaveChanges();
        }


        public SpaceMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> spaceIds)
        {
            SpaceMassEditModel editModel = new SpaceMassEditModel()
            {
                CustomerId = customerId
            };
            editModel.SetList(spaceIds);

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

            // Pending to Active dates
            editModel.PendingToActiveDate = (DateTime?)null;

            // Demand Status
            editModel.DemandStatusId = -1;
            editModel.DemandStatus = DemandStatusList(customerId, -1);

            // Active Tariff Configurations
            editModel.ActiveTariffId = -1;
            editModel.ActiveTariff = ConfigProfileList(customerId, editModel.ActiveTariffId);

            // Pending Tariff Configurations
            editModel.PendingTariffId = -1;
            editModel.PendingTariff = ConfigProfileList(customerId, editModel.PendingTariffId);

            return editModel;
        }

        public void RefreshMassEditModel(SpaceMassEditModel editModel)
        {
            // Get State list
            GetAssetStateList(editModel);

            // Get zone list
            editModel.Zone = ZoneList(editModel.CustomerId, editModel.ZoneId);

            // Get suburb list
            editModel.Suburb = SuburbList(editModel.CustomerId, editModel.SuburbId);

            // Get Areas list
            editModel.Area = AreaList(editModel.CustomerId, editModel.AreaListId);

            // Demand Status
            editModel.DemandStatus = DemandStatusList(editModel.CustomerId, -1);

            // Active Tariff Configurations
            editModel.ActiveTariff = ConfigProfileList(editModel.CustomerId, editModel.ActiveTariffId);

            // Pending Tariff Configurations
            editModel.PendingTariff = ConfigProfileList(editModel.CustomerId, editModel.PendingTariffId);
        }

        public void SetMassEditModel(SpaceMassEditModel model)
        {
            var pendingFactory = new PendingFactory(ConnectionStringName, Now);
            foreach (var asset in model.AssetIds())
            {
                // Save changes to the [AssetPending] table.
                pendingFactory.Save(model, model.CustomerId, asset.AssetId);

                // Tariff changes are saved to a separate table.
                if (model.PendingTariffId > -1)
                {
                    // Is this tariff already pending?
                    var configProfileSpace = PemsEntities.ConfigProfileSpaces.FirstOrDefault(
                        m => m.CustomerId == model.CustomerId && m.ConfigProfileId == model.PendingTariffId && m.ConfigStatus == (int)ConfigStatus.Inactive);
                    if (configProfileSpace != null)
                    {
                        configProfileSpace.ScheduledDate = model.PendingToActiveDate;
                        PemsEntities.SaveChanges();
                        Audit(configProfileSpace);
                    }
                    else
                    {
                        // Add this pending tariff.
                        var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault(m => m.ConfigProfileId == model.PendingTariffId);
                        if (configProfile != null)
                        {
                            configProfileSpace = new ConfigProfileSpace()
                            {
                                ConfigProfileId = model.PendingTariffId,
                                CustomerId = model.CustomerId,
                                ParkingSpaceId = asset.AssetId,
                                ScheduledDate = model.PendingToActiveDate,
                                CreationDate = DateTime.Now,
                                CreateDatetime = DateTime.Now,
                                ConfigStatus = (int)ConfigStatus.Active,
                                UserId = WebSecurity.CurrentUserId
                            };
                            PemsEntities.ConfigProfileSpaces.Add(configProfileSpace);
                            PemsEntities.SaveChanges();
                            Audit(configProfileSpace);
                        }
                    }
                }
            }

            // Note:  The following is commented out but remains since am unsure that the AssetPending table
            // will do what system requires.



            //List<AssetIdentifier> assets = model.AssetIds();

            //foreach (var asset in assets)
            //{
            //    var space = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.CustomerID == editModel.CustomerId && m.ParkingSpaceId == asset.AssetId);

            //    ParkingSpace spaceAudit = null;
            //    MeterMap meterMapAudit = null;

            //    // Street
            //    if ( !string.IsNullOrWhiteSpace( editModel.Street ) )
            //    {
            //        if ( space.Meter != null )
            //        {
            //            space.Meter.Location = editModel.Street;
            //            spaceAudit = space;
            //        }
            //    }

            //    // Set Zone, Area and Suburb
            //    var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == space.CustomerID && m.Areaid == space.AreaId && m.MeterId == space.MeterId);
            //    if (meterMap != null)
            //    {
            //        if (editModel.AreaListId > -1)
            //        {
            //            meterMap.AreaId2 = editModel.AreaListId;
            //            meterMapAudit = meterMap;
            //        }

            //        if (editModel.ZoneId > -1)
            //        {
            //            meterMap.ZoneId = editModel.ZoneId;
            //            meterMapAudit = meterMap;
            //        }

            //        if (editModel.SuburbId > -1)
            //        {
            //            meterMap.CustomGroup1 = editModel.SuburbId;
            //            meterMapAudit = meterMap;
            //        }
            //    }

            //    // Demand Status
            //    if ( editModel.DemandStatusId > -1 )
            //    {
            //        space.DemandZoneId = editModel.DemandStatusId;
            //        spaceAudit = space;
            //    }


            //    //  Tariff Configurations
            //    // Active Tariff 
            //    if ( editModel.ActiveTariffId > -1 )
            //    {
            //        // Set any active tariff to inactive if it is not the selected tariff
            //        var configProfileSpace = PemsEntities.ConfigProfileSpaces.FirstOrDefault( 
            //            m => m.CustomerId == editModel.CustomerId && m.ParkingSpaceId == asset.AssetId && m.ConfigStatus == (int) ConfigStatus.Active );
            //        if ( configProfileSpace == null )
            //        {
            //            // Add ConfigProfileSpaces record
            //            // Add this configProfile
            //            var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault(m => m.ConfigProfileId == editModel.ActiveTariffId);
            //            if (configProfile != null)
            //            {
            //                configProfileSpace = new ConfigProfileSpace()
            //                {
            //                    ConfigProfileId = editModel.ActiveTariffId,
            //                    CustomerId = editModel.CustomerId,
            //                    ParkingSpaceId = asset.AssetId,
            //                    ActivationDate = DateTime.Now,
            //                    CreationDate = DateTime.Now,
            //                    CreateDatetime = DateTime.Now,
            //                    ConfigStatus = (int)ConfigStatus.Active,
            //                    UserId = WebSecurity.CurrentUserId
            //                };
            //                PemsEntities.ConfigProfileSpaces.Add(configProfileSpace);
            //                PemsEntities.SaveChanges();
            //                Audit(configProfileSpace);
            //            }
            //        }
            //        else
            //        {
            //            // Is this Tariff already the active one?
            //            if ( configProfileSpace.ConfigProfileId != editModel.ActiveTariffId )
            //            {
            //                configProfileSpace.ConfigStatus = (int) ConfigStatus.Inactive;
            //                PemsEntities.SaveChanges();
            //                Audit(configProfileSpace);

            //                // Create new active tariff
            //                configProfileSpace = PemsEntities.ConfigProfileSpaces.FirstOrDefault(
            //                    m => m.CustomerId == editModel.CustomerId && m.ParkingSpaceId == asset.AssetId && m.ConfigStatus == (int)ConfigStatus.Active
            //                    && m.ConfigProfileId == editModel.ActiveTariffId );

            //                // If found, set as active.
            //                if ( configProfileSpace != null )
            //                {
            //                    configProfileSpace.ConfigStatus = (int)ConfigStatus.Active;
            //                    configProfileSpace.ActivationDate = DateTime.Now;
            //                    PemsEntities.SaveChanges();
            //                    Audit(configProfileSpace);
            //                }
            //                else
            //                {
            //                    // Add this configProfile
            //                    var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault( m => m.ConfigProfileId == editModel.ActiveTariffId );
            //                    if ( configProfile != null )
            //                    {
            //                        configProfileSpace = new ConfigProfileSpace()
            //                            {
            //                                ConfigProfileId = editModel.ActiveTariffId,
            //                                CustomerId = editModel.CustomerId,
            //                                ParkingSpaceId = asset.AssetId,
            //                                ActivationDate = DateTime.Now,
            //                                CreationDate = DateTime.Now,
            //                                CreateDatetime = DateTime.Now,
            //                                ConfigStatus = (int)ConfigStatus.Active,
            //                                UserId = WebSecurity.CurrentUserId
            //                            };
            //                        PemsEntities.ConfigProfileSpaces.Add( configProfileSpace );
            //                        PemsEntities.SaveChanges();
            //                        Audit(configProfileSpace);
            //                    }
            //                }

            //            }
            //        }
            //    }

            //    if ( editModel.PendingTariffId > -1 )
            //    {
            //        // Is this tariff already pending?
            //        var configProfileSpace = PemsEntities.ConfigProfileSpaces.FirstOrDefault( 
            //            m => m.CustomerId == editModel.CustomerId && m.ConfigProfileId == editModel.PendingTariffId && m.ConfigStatus == (int) ConfigStatus.Inactive );
            //        if ( configProfileSpace != null )
            //        {
            //            configProfileSpace.ScheduledDate = editModel.PendingToActiveDate;
            //            PemsEntities.SaveChanges();
            //            Audit(configProfileSpace);
            //        }
            //        else
            //        {
            //            // Add this pending tariff.
            //            var configProfile = PemsEntities.ConfigProfiles.FirstOrDefault(m => m.ConfigProfileId == editModel.PendingTariffId);
            //            if (configProfile != null)
            //            {
            //                configProfileSpace = new ConfigProfileSpace()
            //                {
            //                    ConfigProfileId = editModel.PendingTariffId,
            //                    CustomerId = editModel.CustomerId,
            //                    ParkingSpaceId = asset.AssetId,
            //                    ScheduledDate = editModel.PendingToActiveDate,
            //                    CreationDate = DateTime.Now,
            //                    CreateDatetime = DateTime.Now,
            //                    ConfigStatus = (int)ConfigStatus.Inactive,
            //                    UserId = WebSecurity.CurrentUserId
            //                };
            //                PemsEntities.ConfigProfileSpaces.Add(configProfileSpace);
            //                PemsEntities.SaveChanges();
            //                Audit(configProfileSpace);
            //            }
            //        }
            //    }

            //    // Save any changes
            //    PemsEntities.SaveChanges();

            //    Audit(meterMapAudit);
            //    Audit(spaceAudit);

            //}
        }

        private SpaceEditModel BuildSpaceEditModel(ParkingSpace space)
        {
            var model = new SpaceEditModel
            {
                AssetId = space.ParkingSpaceId,
                GlobalId = space.ParkingSpaceId.ToString(),
                // Space bay number.
                BayNumber = space.BayNumber,
                // Space Type
                Type = space.MeterGroup == null ? SpaceViewModel.DefaultType : space.MeterGroup.MeterGroupDesc ?? SpaceViewModel.DefaultType,
                TypeId = MeterGroups.Space,
                // Parking space name.
                Name = space.DisplaySpaceNum ?? "",
                // Get AreaId and CustomerId from parking space.
                AreaId = space.AreaId,
                CustomerId = space.CustomerID,
                // Latitude and Longitude
                Latitude = space.Latitude ?? 0.0,
                Longitude = space.Longitude ?? 0.0,
                // Street location
                Street = space.Meter == null ? " " : space.Meter.Location ?? " "
            };



            // Get Space models.
            var parkingSpaceDetail = space.ParkingSpaceDetails.FirstOrDefault(m => m.ParkingSpaceId == space.ParkingSpaceId);
            model.AssetModelId = parkingSpaceDetail == null ? -1 : parkingSpaceDetail.SpaceType;

            // Demand Zone
            model.DemandStatusId = space.DemandZone == null ? 0 : space.DemandZoneId ?? 0;

            // Zone and suburb
            var meterMap =
                PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == space.CustomerID && m.Areaid == space.AreaId && m.MeterId == space.MeterId);

            model.AreaListId = meterMap == null ? 0 : meterMap.AreaId2 ?? 0;
            model.ZoneId = meterMap == null ? 0 : meterMap.ZoneId ?? 0;
            model.SuburbId = meterMap == null ? 0 : meterMap.CustomGroup1 ?? 0;


            // Get associated meter.
            model.AssociatedMeterId = space.Meter == null ? 0 : space.Meter.MeterId;

            // Get associated sensor.
            if (space.HasSensor ?? false)
            {
                var sensor = (new SensorFactory(ConnectionStringName, Now)).GetViewModel(space.ParkingSpaceId);
                model.AssociatedSensorId = sensor == null ? 0 : (int)sensor.AssetId;
            }
            else
            {
                model.AssociatedSensorId = 0;
            }

            // Get configuration details
            model.Configuration = new SpaceConfigurationEditModel();

            // Get active tariff
            DateTime activeTariffDate = DateTime.Now;
            var configProfileSpace = space.ConfigProfileSpaces.FirstOrDefault(m => m.ConfigStatus == (int)ConfigStatus.Active);
            if (configProfileSpace != null)
            {
                model.Configuration.TariffConfigProfileId = configProfileSpace.ConfigProfileId;
                model.Configuration.ActiveTariff = configProfileSpace.ConfigProfile.ConfigurationName;
                activeTariffDate = configProfileSpace.ActivationDate ?? activeTariffDate;
            }
            else
            {
                model.Configuration.TariffConfigProfileId = 0;
                model.Configuration.ActiveTariff = "No Active Tariff";
            }

            // Get pending tariff (if any)
            var pendingConfigProfileSpace = space.ConfigProfileSpaces.FirstOrDefault(m => m.ConfigStatus != 1 && m.ScheduledDate > activeTariffDate);
            if (pendingConfigProfileSpace != null)
            {
                model.Configuration.PendingTariffConfigProfileId = pendingConfigProfileSpace.ConfigProfileId;
                // Pending to active date
                model.Configuration.PendingToActiveDate = pendingConfigProfileSpace.ScheduledDate.HasValue ? pendingConfigProfileSpace.ScheduledDate.Value : (DateTime?)null;
            }
            else
            {
                model.Configuration.PendingTariffConfigProfileId = -1;
                // Pending to active date
                model.Configuration.PendingToActiveDate = (DateTime?)null;
            }
            // Get list of potential active tariffs
            model.Configuration.PendingTariff = (from cp in PemsEntities.ConfigProfiles
                                                 select new SelectListItemWrapper()
                                                 {
                                                     Selected = cp.ConfigProfileId == model.Configuration.PendingTariffConfigProfileId,
                                                     Text = cp.ConfigurationName,
                                                     ValueInt64 = cp.ConfigProfileId
                                                 }).ToList();
            // Add a "No Pending Tariff" selection
            model.Configuration.PendingTariff.Insert(0, new SelectListItemWrapper()
            {
                Selected = model.Configuration.PendingTariffConfigProfileId == -1,
                Text = "No Pending Tariff",
                Value = "-1"
            });



            // Operational status of meter.
            GetOperationalStatusDetails(model, space.OperationalStatus ?? 0, space.OperationalStatusTime.HasValue ? space.OperationalStatusTime.Value : DateTime.Now);

            // Is asset active?
            model.StateId = space.SpaceStatus ?? (int)AssetStateType.Pending;

            return model;
        }

        private void AddAssetPendingToSpaceEditModel(AssetPending assetPending, SpaceEditModel model, ParkingSpace space)
        {
            if (assetPending == null || model == null)
                return;

            model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;

            model.Name = assetPending.DisplaySpaceNum ?? model.Name;

            model.AreaId = assetPending.AreaId;

            model.Latitude = assetPending.Latitude ?? model.Latitude;
            model.Longitude = assetPending.Longitude ?? model.Longitude;

            model.DemandStatusId = assetPending.DemandStatus ?? model.DemandStatusId;

            model.Street = space.Meter == null ? " " : space.Meter.Location ?? " ";

            model.AreaListId = assetPending.MeterMapAreaId2 ?? model.AreaListId;
            model.ZoneId = assetPending.MeterMapZoneId ?? model.ZoneId;
            model.SuburbId = assetPending.MeterMapSuburbId ?? model.SuburbId;

            model.AssociatedMeterId = (int)(assetPending.AssociatedMeterId ?? model.AssociatedMeterId);

            model.AssociatedSensorId = (int)(assetPending.MeterMapSensorId ?? model.AssociatedSensorId);

            if (assetPending.OperationalStatus != null)
            {
                GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);
            }

            // Is asset active?
            model.StateId = assetPending.AssetState ?? model.StateId;
        }

        #endregion

        #region Create/Clone

        private int Clone(int customerId, int areaId, Int64 assetId)
        {
            //// Get original space 
            //// Since this space exists, these entities should exist also.
            //ParkingSpace space = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.CustomerID == customerId && m.ParkingSpaceId == assetId);

            //// Create a gateway
            //ParkingSpace newSpace = new ParkingSpace();
            //{
            //    GateWayID = NextGatewayId(customerId),
            //    CustomerID = customerId,
            //    GatewayState = (int)AssetStateType.Pending,
            //    OperationalStatus = (int)OperationalStatusType.Inactive,
            //    OperationalStatusTime = DateTime.Now,
            //    GatewayType = newMeter.MeterGroup
            //};
            //newGateway.GatewayModel = gateway.GatewayModel;
            //newGateway.WarrantyExpiration = gateway.WarrantyExpiration;
            //newGateway.CAMID = gateway.CAMID;
            //newGateway.CELID = gateway.CELID;
            //newGateway.HWVersion = gateway.HWVersion;
            //newGateway.Latitude = gateway.Latitude;
            //newGateway.Longitude = gateway.Longitude;
            //newGateway.Location = gateway.Location;
            //newGateway.Manufacturer = gateway.Manufacturer;
            //newGateway.PowerSource = gateway.PowerSource;
            //newGateway.TimeZoneID = gateway.TimeZoneID;

            //PemsEntities.Gateways.Add(newGateway);
            //PemsEntities.SaveChanges();


            //// Align clonable meter fields.
            //newMeter.Location = meter.Location;
            //newMeter.DemandZone = meter.DemandZone;
            //newMeter.FreeParkingMinute = meter.FreeParkingMinute;
            //newMeter.MeterGroup = meter.MeterGroup;
            //newMeter.MeterName = meter.MeterName;
            //newMeter.MeterType = meter.MeterType;
            //newMeter.TimeZoneID = meter.TimeZoneID;
            //newMeter.TypeCode = meter.TypeCode;
            //newMeter.WarrantyExpiration = meter.WarrantyExpiration;
            //newMeter.NextPreventativeMaintenance = meter.NextPreventativeMaintenance;

            //// Create meter map for cloned gateway.
            ////  Need a [HousingMaster]
            //HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            //// Create a [MeterMap] entry
            //MeterMap newMeterMap = new MeterMap()
            //{
            //    Customerid = newMeter.CustomerID,
            //    Areaid = newMeter.AreaID,
            //    MeterId = newMeter.MeterId,
            //    HousingId = housingMaster.HousingId
            //};
            //PemsEntities.MeterMaps.Add(newMeterMap);
            //PemsEntities.SaveChanges();

            //newMeterMap.AreaId2 = meterMap.AreaId2;
            //newMeterMap.ZoneId = meterMap.ZoneId;
            //newMeterMap.SubAreaID = meterMap.SubAreaID;
            //newMeterMap.MechId = meterMap.MechId;
            //newMeterMap.DataKeyId = meterMap.DataKeyId;
            //newMeterMap.CustomGroup1 = meterMap.CustomGroup1;
            //newMeterMap.CustomGroup2 = meterMap.CustomGroup2;
            //newMeterMap.CustomGroup3 = meterMap.CustomGroup3;
            //newMeterMap.MaintRouteId = meterMap.MaintRouteId;
            //newMeterMap.GatewayID = newGateway.GateWayID;


            //PemsEntities.SaveChanges();

            //// Set audit records.
            //Audit(newMeter);
            //Audit(newMeterMap);
            //Audit(newGateway);

            return 0;
        }

        public Int64 Clone(SpaceViewModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }

        public Int64 Clone(SpaceEditModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }

        public Int64 Create(Meter meter, int bayNumber)
        {
            Int64? globalMeterId = GetGlobalMeterId(meter.CustomerID, meter.AreaID, meter.MeterId, bayNumber);

            ServerInstance serverInstance = PemsEntities.ServerInstances.FirstOrDefault();

            var space = new ParkingSpace()
            {
                ParkingSpaceId = globalMeterId ?? 0,
                ServerID = serverInstance == null ? 0 : serverInstance.Instanceid,
                CustomerID = meter.CustomerID,
                AreaId = meter.AreaID,
                MeterId = meter.MeterId,
                BayNumber = bayNumber,
                AddedDateTime = DateTime.Now,
                SpaceStatus = (int)AssetStateType.Pending,
                OperationalStatus = (int)OperationalStatusType.Inactive
            };


            PemsEntities.ParkingSpaces.Add(space);
            PemsEntities.SaveChanges();

            // Add audit record
            Audit(space);

            // Add parking space detail record.
            PemsEntities.ParkingSpaceDetails.Add(new ParkingSpaceDetail()
            {
                ParkingSpaceId = globalMeterId ?? 0,
                SpaceType = 1,
                HBInterval = 15,
                ExpiryLevel = "5,-5",
                PercentLevel = "30,60"
            });
            PemsEntities.SaveChanges();

            return space.ParkingSpaceId;
        }

        /// <summary>
        /// Add one or more parking spaces (bays) to an existing meter.
        /// </summary>
        /// <param name="customerId">Id of customer</param>
        /// <param name="areaId">Area id of meter</param>
        /// <param name="assetId">Meter id</param>
        /// <param name="bayStart">Starting bay number</param>
        /// <param name="bayEnd">Ending bay number</param>
        /// <param name="error">Output parameter to return any error description.</param>
        /// <returns>True if all bays were added.</returns>
        public bool AddSpacesToMeter(int customerId, int areaId, int assetId, int bayStart, int bayEnd, out string error)
        {
            int baysAdded = 0;
            int maxBayNumber = 0;

            // Error log
            error = string.Empty;

            // Get the meter associated with the new spaces.
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == assetId);

            for (int bayNumber = bayStart; bayNumber <= bayEnd; bayNumber++)
            {
                // Does this bay number already exist for this meter?
                var space = meter.ParkingSpaces.FirstOrDefault(m => m.BayNumber == bayNumber);
                if (space != null)
                {
                    error += "Bay number " + bayNumber.ToString() + " already exists. ";
                    continue;
                }

                // Space does not exist.  Create one.
                Int64 spaceId = Create(meter, bayNumber);
                if (spaceId == 0)
                {
                    error += "Unable to create bay number " + bayNumber.ToString() + ". ";
                }
                else
                {
                    maxBayNumber = bayNumber;
                    baysAdded++;
                }
            }


            // Update meter if required.
            if (baysAdded > 0)
            {
                meter.MaxBaysEnabled += baysAdded;
                meter.BayEnd = maxBayNumber;
                PemsEntities.SaveChanges();
                Audit(meter);
            }

            return baysAdded > 0;
        }



        #endregion



        #region Spaces Utilities

        /// <summary>
        /// Determines if a given bay number has already been assigned to a parking space for the given meter.
        /// </summary>
        /// <param name="customerId">Integer id of customer</param>
        /// <param name="meterId">Integer id of meter.</param>
        /// <param name="areaId">Integer id of area</param>
        /// <param name="bayNumber">Bay number being checked</param>
        /// <returns>True if bay number has already been assigned to meter</returns>
        public bool IsBayNumberUsed(int customerId, int meterId, int areaId, int bayNumber)
        {
            var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(
                m => m.CustomerID == customerId && m.MeterId == meterId && m.AreaId == areaId && m.BayNumber == bayNumber);

            return parkingSpace != null;
        }


        /// <summary>
        /// Determines next bay number in order that is unassigned to this meter.
        /// </summary>
        /// <param name="customerId">Integer id of customer</param>
        /// <param name="meterId">Integer id of meter.</param>
        /// <param name="areaId">Integer id of area</param>
        /// <returns>Next bay number</returns>
        public int NextBayNumber(int customerId, int meterId, int areaId)
        {
            int nextBayNumber = 1;

            var parkingSpaces = PemsEntities.ParkingSpaces.Where(
                m => m.CustomerID == customerId && m.MeterId == meterId && m.AreaId == areaId);
            if (parkingSpaces.Any())
            {
                nextBayNumber = parkingSpaces.Max(m => m.BayNumber) + 1;
            }

            return nextBayNumber;
        }


        #endregion

        #region History

        public List<SpaceViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {
            DateTime startDate;
            DateTime endDate;

            IQueryable<ParkingSpacesAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.ParkingSpacesAudits.Where(m => m.CustomerID == customerId && m.ParkingSpaceId == assetId
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.ParkingSpacesAudits.Where(m => m.CustomerID == customerId && m.ParkingSpaceId == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.ParkingSpacesAudits.Where(m => m.CustomerID == customerId && m.ParkingSpaceId == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.ParkingSpacesAudits.Where(m => m.CustomerID == customerId && m.ParkingSpaceId == assetId).OrderBy(m => m.UpdateDateTime);
            }

            // Get the SpaceViewModel for each audit record.
            var list = new List<SpaceViewModel>();

            // Get present data
            var presentModel = GetViewModel(assetId);
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data
            SpaceViewModel previousModel = null;
            foreach (var space in query)
            {
                var activeModel = CreateHistoricViewModel(space);
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
