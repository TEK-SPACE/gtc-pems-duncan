/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 12/20/2013       Sergey Ostrerov                 Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 * 01/15/2014       Sergey Ostrerov                 DPTXPEMS-8 - Can't create new TX meter through PEMS UI.
 * 02/06/2014       R Howard                        JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 02/19/2014       Sergey Ostrerov                 DPTXPEMS-213 - Time formatting:Alarm Duration.
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Collections;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using NPOI.SS.Formula.PTG;

namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// Class encapsulating functionality for the meter asset type.
    /// </summary>
    public class MeterFactory : AssetFactory
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
        public MeterFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {

        }

        #region Index

        /// <summary>
        /// Get a queryable list of meter summaries (<see cref="IQueryable{AssetListModel}"/>) for
        /// customer id of <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gridType">The type of grid (<see cref="AssetListGridType"/>) that will be using the data</param>
        /// <returns>An instance of <see cref="IQueryable{AssetListModel}"/></returns>
        public IQueryable<AssetListModel> GetSummaryModels(int customerId, AssetListGridType gridType, MeterGroups meterGroup)
        {
            #region 22 Sep 2014
            ////NOTE: we are intentionally leaving some fields blank - per the spec.
            ////get the meters with the meter map
            //var items = from m in PemsEntities.Meters
            //            where m.CustomerID == customerId
            //            where m.MeterGroup == (int)meterGroup
            //            join mm in PemsEntities.MeterMaps on
            //                new { MeterId = m.MeterId, AreaId = m.AreaID, CustomerId = m.CustomerID } equals
            //                new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
            //            from l1 in metermap.DefaultIfEmpty()
            //            join vpn in PemsEntities.VersionProfileMeters on
            //                new { MeterId = m.MeterId, AreaId = m.AreaID, CustomerId = m.CustomerID } equals
            //               new { MeterId = vpn.MeterId.Value, AreaId = vpn.AreaId.Value, CustomerId = vpn.CustomerId } into vpns
            //            from l3 in vpns.DefaultIfEmpty()
            //            let aa = m.ActiveAlarms.FirstOrDefault()
            //            select
            //                new AssetListModel
            //                {
            //                    //----------DETAIL LINKS
            //                    Type = "Meter", //name of the details page (ViewCashbox)
            //                    AreaId = m.AreaID,
            //                    CustomerId = customerId,

            //                    //-----------FILTERABLE ITEMS
            //                    AssetType = m.MeterGroup1 == null ? MeterViewModel.DefaultType : m.MeterGroup1.MeterGroupDesc ?? MeterViewModel.DefaultType,
            //                    AssetId = m.MeterId,
            //                    AssetName = m.MeterName ?? " ",
            //                    OperationalStatus = m.OperationalStatu == null ? " " : m.OperationalStatu.OperationalStatusDesc,
            //                    Latitude = m.Latitude ?? 0.0,
            //                    Longitude = m.Longitude ?? 0.0,
            //                    AreaId2 = l1.AreaId2,
            //                    ZoneId = l1.ZoneId,
            //                    Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == l1.CustomGroup1 && x.CustomerId == customerId) == null ? " " : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == l1.CustomGroup1 && x.CustomerId == customerId).DisplayName,

            //                    Street = m.Location ?? " ",
            //                    DemandStatus = m.DemandZone1 == null ? " " : m.DemandZone1.DemandZoneDesc ?? " ",

            //                    //------------COMMON COLUMNS
            //                    AssetModel = m.MechanismMaster == null ? " "
            //                        : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == customerId && z.MechanismId == m.MechanismMaster.MechanismId && z.IsDisplay) == null ? " "
            //                        : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == customerId && z.MechanismId == m.MechanismMaster.MechanismId && z.IsDisplay).MechanismDesc,


            //                    //------------SUMMARY GRID
            //                    SpacesCount = m.MaxBaysEnabled ?? 0,
            //                    Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == l1.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == l1.AreaId2).AreaName,
            //                    Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == customerId) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == customerId).ZoneName,

            //                    //---------------CONFIGURATION GRID
            //                    DateInstalled = m.InstallDate,
            //                    ConfigurationId = l3 == null ? (long?)null : l3.VersionProfileMeterId,
            //                    //ConfigCreationDate
            //                    //ConfigScheduleDate
            //                    //ConfigActivationDate
            //                    MpvVersion = l3 == null ? "" : l3.CommunicationVersion,
            //                    SoftwareVersion = l3 == null ? "" : l3.SoftwareVersion,
            //                    FirmwareVersion = l3 == null ? "" : l3.HardwareVersion,

            //                    //-------------OCCUPANCY GRID - only valid for spaces
            //                    //MeterName
            //                    //SensorName
            //                    OperationalStatusDate = m.OperationalStatusTime,
            //                    //OccupancyStatus
            //                    //OccupancyStatusDate 
            //                    //NonComplianceStatus
            //                    //NonComplianceStatusDate

            //                    //------------FUNCTIONAL STATUS GRID
            //                    AlarmClass = aa.EventCode1.AlarmTier1.TierDesc,
            //                    AlarmCode = aa == null ? "" : aa.EventCode1 == null ? "" : aa.EventCode1.EventDescVerbose,
            //                    AlarmTimeOfOccurance = aa == null ? (DateTime?)null : aa.TimeOfOccurrance,
            //                    AlarmRepairTargetTime = aa == null ? (DateTime?)null : aa.SLADue,
            //                    LocalTime = Now
            //                };

            //return items;
            #endregion

            //NOTE: we are intentionally leaving some fields blank - per the spec.
            //get the meters with the meter map
            var items = from m in PemsEntities.Meters
                        where m.CustomerID == customerId
                        where m.MeterGroup == (int)meterGroup
                        join mm in PemsEntities.MeterMaps on
                            new { MeterId = m.MeterId, AreaId = m.AreaID, CustomerId = m.CustomerID } equals
                            new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                        from l1 in metermap.DefaultIfEmpty()
                        join vpn in PemsEntities.VersionProfileMeters on
                            new { MeterId = m.MeterId, AreaId = m.AreaID, CustomerId = m.CustomerID } equals
                           new { MeterId = vpn.MeterId.Value, AreaId = vpn.AreaId.Value, CustomerId = vpn.CustomerId } into vpns
                        from l3 in vpns.DefaultIfEmpty()
                        let aa = m.ActiveAlarms.FirstOrDefault()
                        select
                            new AssetListModel
                            {
                                //----------DETAIL LINKS
                                Type = "Meter", //name of the details page (ViewCashbox)
                                AreaId = m.AreaID,
                                CustomerId = customerId,

                                //-----------FILTERABLE ITEMS
                                AssetType = m.MeterGroup1 == null ? MeterViewModel.DefaultType : m.MeterGroup1.MeterGroupDesc ?? MeterViewModel.DefaultType,
                                AssetId = m.MeterId,
                                //AssetName = m.MeterName ?? " ",
                                AssetName = m.MeterName,  //** sairam done this on sep 21st for grid asset name display (asset inquiry page)
                                InventoryStatus = m.MeterState == null ? "" : m.AssetState.AssetStateDesc,
                                OperationalStatus = m.OperationalStatu == null ? " " : m.OperationalStatu.OperationalStatusDesc,
                                //Latitude = m.Latitude ?? 0.0,
                                //Longitude = m.Longitude ?? 0.0,
                                //** Sairam added on 23rd sep *****
                                Latitude = m.Latitude,
                                Longitude = m.Longitude,
                                AreaId2 = l1.AreaId2,
                                ZoneId = l1.ZoneId,
                                Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == l1.CustomGroup1 && x.CustomerId == customerId) == null ? " " : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == l1.CustomGroup1 && x.CustomerId == customerId).DisplayName,

                                //Street = m.Location ?? " ",
                                //** Sairam added on 23rd sep *****
                                Street = m.Location,
                                DemandStatus = m.DemandZone1 == null ? " " : m.DemandZone1.DemandZoneDesc ?? " ",

                                //------------COMMON COLUMNS
                                //AssetModel = m.MechanismMaster == null ? ""
                                //    : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == customerId && z.MechanismId == m.MechanismMaster.MechanismId && z.IsDisplay) == null ? " "
                                //    : PemsEntities.MechanismMasterCustomers.FirstOrDefault(z => z.CustomerId == customerId && z.MechanismId == m.MechanismMaster.MechanismId && z.IsDisplay).MechanismDesc,

                                //** Sairam added on 23rd sep *****
                                AssetModel = PemsEntities.MechanismMasterCustomers.FirstOrDefault(x => x.CustomerId == m.CustomerID && x.MechanismId == (PemsEntities.MechanismMasters.FirstOrDefault(y => y.MeterGroupId == m.MeterGroup).MechanismId)).MechanismDesc,

                                //------------SUMMARY GRID
                                //SpacesCount = m.MaxBaysEnabled ?? 0,
                                //** Sairam added on 23rd sep *****
                                SpacesCount = m.MaxBaysEnabled,

                                //Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == l1.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == l1.AreaId2).AreaName,
                                //** Sairam added on 23rd sep *****
                                //Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == (PemsEntities.MeterMaps.FirstOrDefault(u => u.MeterId == m.MeterId)).Areaid).AreaName,
                                Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == l1.AreaId2).AreaName,

                                //Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == customerId) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == l1.ZoneId && x.customerID == customerId).ZoneName,
                                //** Sairam added on 23rd sep *****
                                //Zone = PemsEntities.Zones.FirstOrDefault(x => x.customerID == customerId && x.ZoneId == (PemsEntities.MeterMaps.FirstOrDefault(u => u.MeterId == m.MeterId)).ZoneId).ZoneName,
                                Zone = PemsEntities.Zones.FirstOrDefault(x => x.customerID == customerId && x.ZoneId == l1.ZoneId).ZoneName,

                                //---------------CONFIGURATION GRID
                                DateInstalled = m.InstallDate,
                                ConfigurationId = l3 == null ? (long?)null : l3.VersionProfileMeterId,
                                //ConfigCreationDate
                                //ConfigScheduleDate
                                //ConfigActivationDate
                                MpvVersion = l3 == null ? "" : l3.CommunicationVersion,
                                SoftwareVersion = l3 == null ? "" : l3.SoftwareVersion,
                                FirmwareVersion = l3 == null ? "" : l3.HardwareVersion,

                                //-------------OCCUPANCY GRID - only valid for spaces
                                //MeterName
                                //SensorName
                                OperationalStatusDate = m.OperationalStatusTime,
                                //OccupancyStatus
                                //OccupancyStatusDate 
                                //NonComplianceStatus
                                //NonComplianceStatusDate

                                //------------FUNCTIONAL STATUS GRID
                                AlarmClass = aa.EventCode1.AlarmTier1.TierDesc,
                                AlarmCode = aa == null ? "" : aa.EventCode1 == null ? "" : aa.EventCode1.EventDescVerbose,
                                AlarmTimeOfOccurance = aa == null ? (DateTime?)null : aa.TimeOfOccurrance,
                                AlarmRepairTargetTime = aa == null ? (DateTime?)null : aa.SLADue,
                                LocalTime = Now,

                                ///Special Action
                                SpecialActionId = PemsEntities.LatestMeterDatas.FirstOrDefault(x => x.CID == customerId && x.MID.Value == m.MeterId && x.AID == m.AreaID).SpecialAction,
                                HasSensorId = PemsEntities.ParkingSpaces.FirstOrDefault(x => x.CustomerID == customerId && x.MeterId == m.MeterId && x.AreaId == m.AreaID).HasSensor == null ? false : PemsEntities.ParkingSpaces.FirstOrDefault(x => x.CustomerID == customerId && x.MeterId == m.MeterId && x.AreaId == m.AreaID).HasSensor.Value
                               // HasSensorId = false
                            };

            return items;
        }

        #endregion

        #region Get View Models

        /// <summary>
        /// Returns an instance of <see cref="MeterViewModel"/> reflecting the present state
        /// and settings of a <see cref="Meter"/> for customer id of <paramref name="customerId"/>,
        /// an area id of <paramref name="areaId"/> and a meter id of <paramref name="meterId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="meterId">The meter id</param>
        /// <returns>An instance of <see cref="MeterViewModel"/></returns>
        public MeterViewModel GetViewModel(int customerId, int areaId, int meterId)
        {

            // Get the meter from Meters
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == meterId);

            var model = new MeterViewModel
                {
                    CustomerId = customerId,
                    AreaId = areaId,
                    AssetId = meterId
                };

            model.GlobalId = meter.GlobalMeterId.ToString();

            model.Type = meter.MeterGroup1 == null ? MeterViewModel.DefaultType : meter.MeterGroup1.MeterGroupDesc ?? MeterViewModel.DefaultType;
            model.TypeId = meter.MeterGroup ?? (int)MeterGroups.SingleSpaceMeter;
            model.AssetModel = meter.MechanismMaster == null ? " "
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == meter.MechanismMaster.MechanismId && m.IsDisplay) == null ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == meter.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc;
            model.Name = meter.MeterName ?? "";
            if (meter.MechanismMaster != null)
                model.MechanismId = meter.MechanismMaster.MechanismId;

            model.Street = meter.Location ?? "";

            model.PhoneNumber = meter.SMSNumber ?? "";

            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.Areaid == areaId && m.MeterId == meterId);
            if (meterMap != null)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == meterMap.AreaId2);
                model.Area = area == null ? "" : area.AreaName;
                model.AreaId2 = area == null ? -1 : area.AreaID;
                model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                model.ZoneId = meterMap.Zone == null ? -1 : meterMap.Zone.ZoneId;
                model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;

                // Collection Route
                if (meterMap.CollRouteId != null)
                {
                    var collectionRun = PemsEntities.CollectionRuns
                        .FirstOrDefault(m => m.CustomerId == customerId && m.CollectionRunId == meterMap.CollRouteId);
                    model.CollectionRoute = collectionRun == null ? "" : collectionRun.CollectionRunName;
                }
                else
                {
                    model.CollectionRoute = "";
                }

                // Maintenace Route
                if (meterMap.MaintRouteId != null)
                {
                    model.MaintenanceRoute = meterMap.MaintRoute == null ? "" : meterMap.MaintRoute.DisplayName ?? "";
                }
                else
                {
                    model.MaintenanceRoute = "";
                }
                var location = PemsEntities.HousingMasters.FirstOrDefault(m => m.Customerid == customerId && m.HousingId == meterMap.HousingId);
                if (location != null)
                    model.LocationName = location.HousingName;
                else
                    model.LocationName = "";


                var Mechmaster = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechId == meterMap.MechId);
                if (Mechmaster != null)
                    model.MechSerialNo = Mechmaster.MechSerial;
                else
                    model.MechSerialNo = "";
            }
            else
            {
                model.Area = "";
                model.AreaId2 = -1;
                model.Zone = "";
                model.ZoneId = -1;
                model.Suburb = " ";

                // Collection Route
                model.CollectionRoute = "";

                // Maintenace Route - 
                model.MaintenanceRoute = "";

                model.LocationName = "";
                model.MechSerialNo = "";
            }

            // Lat/Long
            model.Latitude = meter.Latitude ?? 0.0;
            model.Longitude = meter.Longitude ?? 0.0;

            model.Spaces = meter.MaxBaysEnabled ?? 0;

            model.DemandStatus = meter.DemandZone1 == null ? " " : meter.DemandZone1.DemandZoneDesc ?? " ";

            model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value.ToShortDateString() : "";


            // Version profile information
            model.Configuration = new AssetConfigurationModel();
            model.Configuration.DateInstalled = meter.InstallDate;
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == meterId);
            if (versionProfile != null)
            {
                model.Configuration.MPVVersion = versionProfile.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfile.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfile.HardwareVersion ?? "";
                model.Configuration.ConfigurationId = versionProfile.VersionProfileMeterId;
                model.Configuration.ConfigurationName = versionProfile.ConfigurationName ?? "-";
            }


            // Active alarm
            model.AlarmConfiguration = new AssetAlarmConfigModel();
            var activeAlarm = meter.ActiveAlarms.OrderByDescending(m => m.TimeOfOccurrance).FirstOrDefault();
            if (activeAlarm != null)
            {
                model.AlarmConfiguration.Class = activeAlarm.EventCode1.AlarmTier1.TierDesc;
                model.AlarmConfiguration.Code = activeAlarm.EventCode1.EventDescVerbose ?? " ";
                DateTime now = DateTime.Now;
                int totalMinutes = (int)(activeAlarm.TimeOfOccurrance > now
                                                ? (activeAlarm.TimeOfOccurrance - now).TotalMinutes
                                                : (now - activeAlarm.TimeOfOccurrance).TotalMinutes);

                model.AlarmConfiguration.Duration = FormatHelper.FormatTimeFromMinutes(totalMinutes, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM);
                model.AlarmConfiguration.RepairTargetTime = activeAlarm.SLADue == null ? " " : activeAlarm.SLADue.Value.ToString();
            }

            // Operational status of meter.
            model.Status = meter.OperationalStatu == null ? "" : meter.OperationalStatu.OperationalStatusDesc;
            model.StatusDate = meter.OperationalStatusTime;

            // See if meter has pending changes.
            model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

            // Is asset active?
            model.State = (AssetStateType)(meter.MeterState ?? 0);

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }


        /// <summary>
        /// Returns an instance of <see cref="MeterViewModel"/> reflecting the present and pending state
        /// and settings of a <see cref="Meter"/> for customer id of <paramref name="customerId"/>,
        /// an area id of <paramref name="areaId"/> and a meter id of <paramref name="meterId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="meterId">The meter id</param>
        /// <returns>An instance of <see cref="MeterViewModel"/></returns>
        public MeterViewModel GetPendingViewModel(int customerId, int areaId, int meterId)
        {
            MeterViewModel model = GetViewModel(customerId, areaId, meterId);

            // Get the pending changes from [AssetPending] 
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterId == model.AssetId);

            // Get out if there are no pending changes.
            if (assetPending == null)
                return model;

            //Asset base model values
            SetPropertiesOfPendingAssetModel(assetPending, model);

            //asset specific values
            if (!string.IsNullOrWhiteSpace(assetPending.PhoneNumber))
                model.PhoneNumber = assetPending.PhoneNumber;
            if (assetPending.DemandStatus.HasValue)
                model.DemandStatus = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneId == assetPending.DemandStatus.Value).DemandZoneDesc;

            if (assetPending.WarrantyExpiration.HasValue)
                model.WarrantyExpiration = assetPending.WarrantyExpiration.Value.ToShortDateString();

            // Sensor configuration details
            if (assetPending.DateInstalled.HasValue)
                model.Configuration.DateInstalled = assetPending.DateInstalled.Value;

            // Version information
            if (!string.IsNullOrWhiteSpace(assetPending.MPVFirmware))
                model.Configuration.MPVVersion = assetPending.MPVFirmware;

            if (!string.IsNullOrWhiteSpace(assetPending.AssetSoftwareVersion))
                model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion;

            if (!string.IsNullOrWhiteSpace(assetPending.AssetFirmwareVersion))
                model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion;

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns a <see cref="MeterViewModel"/> based upon the indicated audit record id <paramref name="auditId"/>.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="meterId">The asset id</param>
        /// <param name="auditId">The audit record id</param>
        /// <returns>An instance of <see cref="MeterViewModel"/></returns>
        public MeterViewModel GetHistoricViewModel(int customerId, int areaId, int meterId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.MetersAudits.FirstOrDefault(m => m.MetersAuditId == auditId));
        }

        /// <summary>
        /// Private method to create a <see cref="MeterViewModel"/> from a <see cref="MetersAudit"/> record.
        /// </summary>
        /// <param name="auditModel">An instance of <see cref="MetersAudit"/></param>
        /// <returns>An instance of <see cref="MeterViewModel"/></returns>
        private MeterViewModel CreateHistoricViewModel(MetersAudit auditModel)
        {
            var model = new MeterViewModel()
            {
                CustomerId = auditModel.CustomerID,
                AreaId = auditModel.AreaID,
                AssetId = auditModel.MeterId,
                GlobalId = auditModel.GlobalMeterId.ToString()
            };

            model.GlobalId = auditModel.GlobalMeterId.ToString();

            model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.MeterGroup) == null ? MeterViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.MeterGroup).MeterGroupDesc ?? GatewayViewModel.DefaultType;
            model.TypeId = auditModel.MeterGroup ?? (int)MeterGroups.SingleSpaceMeter;

            model.AssetModel = !auditModel.MeterType.HasValue ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == auditModel.MeterType && m.IsDisplay) == null ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == auditModel.MeterType && m.IsDisplay).MechanismDesc;
            model.Name = auditModel.MeterName ?? "";

            model.Street = auditModel.Location ?? "";

            model.PhoneNumber = auditModel.SMSNumber ?? "";

            // Need to see if there is a PemsEntities.MeterMapAudits for this sensor id within an hour.
            DateTime fromDate = auditModel.UpdateDateTime.AddHours(-1);
            DateTime toDate = auditModel.UpdateDateTime.AddHours(1);

            var meterMapAudit = PemsEntities.MeterMapAudits.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.Areaid == auditModel.AreaID
                && m.AuditDateTime > fromDate && m.AuditDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if (meterMapAudit == null)
            {
                meterMapAudit = PemsEntities.MeterMapAudits.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.Areaid == auditModel.AreaID
                                                                                 && m.AuditDateTime <= auditModel.UpdateDateTime);
            }

            // Is there still is not a meterMapAudit then get present MeterMap, otherwise use meterMapAudit
            MeterMap meterMap = null;
            if (meterMapAudit == null)
            {
                meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.Areaid == auditModel.AreaID);
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
            var zone = PemsEntities.Zones.FirstOrDefault(m => m.customerID == auditModel.CustomerID && m.ZoneId == model.ZoneId);
            model.Zone = zone == null ? "" : zone.ZoneName ?? "";

            // Suburb
            model.SuburbId = meterMapAudit == null ? (meterMap == null ? null : meterMap.CustomGroup1) : meterMapAudit.CustomGroup1;
            var suburb = PemsEntities.CustomGroup1.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.CustomGroupId == model.SuburbId);
            model.Suburb = suburb == null ? "" : suburb.DisplayName ?? "";

            // Maintenance Route
            int? maintRouteId = meterMapAudit == null ? (meterMap == null ? null : meterMap.MaintRouteId) : meterMapAudit.MaintRouteId;
            var maintRoute = PemsEntities.MaintRoutes.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.MaintRouteId == maintRouteId);
            model.MaintenanceRoute = maintRoute == null ? "" : maintRoute.DisplayName ?? "";

            // Collection Route
            int? collRouteId = meterMapAudit == null ? (meterMap == null ? null : meterMap.CollRouteId) : meterMapAudit.CollRouteId;
            var collectionRun = PemsEntities.CollectionRuns.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.CollectionRunId == collRouteId);
            model.CollectionRoute = collectionRun == null ? "" : collectionRun.CollectionRunName ?? "";

            // Lat/Long
            model.Latitude = auditModel.Latitude ?? 0.0;
            model.Longitude = auditModel.Longitude ?? 0.0;

            model.Spaces = auditModel.MaxBaysEnabled ?? 0;

            var demandZone = PemsEntities.DemandZones.FirstOrDefault(m => m.DemandZoneId == auditModel.DemandZone);
            model.DemandStatus = demandZone == null ? "" : demandZone.DemandZoneDesc ?? "";

            model.LastPrevMaint = auditModel.LastPreventativeMaintenance.HasValue ? auditModel.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = auditModel.NextPreventativeMaintenance.HasValue ? auditModel.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            model.WarrantyExpiration = auditModel.WarrantyExpiration.HasValue ? auditModel.WarrantyExpiration.Value.ToShortDateString() : "";


            // Version profile information
            model.Configuration = new AssetConfigurationModel();
            model.Configuration.DateInstalled = auditModel.InstallDate;


            // See if there is a VersionProfileMeterAudit withing an hour of this audit record.
            var versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.AreaId == auditModel.AreaID
                && m.UpdateDateTime > fromDate && m.UpdateDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if (versionProfileMeterAudit == null)
            {
                versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.AreaId == auditModel.AreaID
                      && m.UpdateDateTime < auditModel.UpdateDateTime);
            }

            // Is there still is not a versionProfileMeterAudit then get present versionProfileMeter, otherwise use versionProfileMeterAudit
            VersionProfileMeter versionProfileMeter = null;
            if (versionProfileMeterAudit == null)
            {
                versionProfileMeter = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.MeterId == auditModel.MeterId && m.AreaId == auditModel.AreaID);
            }

            if (versionProfileMeterAudit == null && versionProfileMeter == null)
            {
                model.Configuration.MPVVersion = "";
                model.Configuration.SoftwareVersion = "";
                model.Configuration.FirmwareVersion = "";
                model.Configuration.ConfigurationId = 0;
                model.Configuration.ConfigurationName = "";
            }
            else if (versionProfileMeterAudit == null)
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


            // Active alarm
            model.AlarmConfiguration = new AssetAlarmConfigModel();

            // Operational status of meter.
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
            model.AuditId = auditModel.MetersAuditId;

            return model;
        }

        /// <summary>
        /// Gets an <see cref="AssetHistoryModel"/> for a given <paramref name="customerId"/>, <paramref name="areaId"/> and <paramref name="meterId"/>.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="meterId">The meter id</param>
        /// <returns>An instance of an <see cref="AssetHistoryModel"/></returns>
        public AssetHistoryModel GetHistoricListViewModel(int customerId, int areaId, int meterId)
        {
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == meterId);

            var model = new AssetHistoryModel()
            {
                CustomerId = customerId,
                AssetId = meterId,
                AreaId = areaId,
                Type = meter.MeterGroup1 == null ? MeterViewModel.DefaultType : meter.MeterGroup1.MeterGroupDesc ?? MeterViewModel.DefaultType,
                TypeId = meter.MeterGroup ?? (int)MeterGroups.SingleSpaceMeter,
                Name = meter.MeterName,
                Street = meter.Location ?? ""
            };

            return model;
        }



        public MeterSpacesModel GetMeterSpacesModel(int customerId, int areaId, int meterId)
        {
            var meterSpacesModel = new MeterSpacesModel { CustomerId = customerId, AreaId = areaId, AssetId = meterId };

            var meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == meterId);

            meterSpacesModel.GlobalId = (meter.GlobalMeterId ?? 0).ToString();

            meterSpacesModel.Type = meter.MeterGroup1 == null ? " " : meter.MeterGroup1.MeterGroupDesc ?? " ";
            meterSpacesModel.TypeId = meter.MeterGroup ?? (int)MeterGroups.SingleSpaceMeter;

            meterSpacesModel.AssetModel = meter.MechanismMaster == null ? " "
              : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == meter.MechanismMaster.MechanismId && m.IsDisplay) == null ? ""
              : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == meter.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc;
            meterSpacesModel.Name = meter.MeterName ?? " ";
            meterSpacesModel.Street = meter.Location ?? "";


            SpaceFactory spaceFactory = new SpaceFactory(ConnectionStringName, Now);

            // Walk the list of ParkingSpaces
            foreach (var parkingSpace in meter.ParkingSpaces)
            {
                meterSpacesModel.Spaces.Add(spaceFactory.GetSpaceListModel(parkingSpace.ParkingSpaceId));
            }

            return meterSpacesModel;
        }


        #endregion

        #region Set MeterSpacesModel

        public void SetMeterSpacesModel(MeterSpacesModel model)
        {
            // Get the Meter associated with these spaces.
            Meter meter = PemsEntities.Meters.FirstOrDefault(
                m => m.CustomerID == model.CustomerId && m.MeterId == model.AssetId && m.AreaID == model.AreaId);

            // Create a spaces factory.
            SpaceFactory spaceFactory = new SpaceFactory(ConnectionStringName, Now);



            //  Walk the Spaces collection
            //    Add new spaces
            //    Save updated spaces
            foreach (var space in model.Spaces)
            {
                // Is this a new space?
                if (space.IsNew)
                {
                    // Was this space added then deleted?  If so, then space.OperationalStatus would be set to Inactive
                    // If so, then skip this space.
                    if (space.OperationalStatus == (int)OperationalStatusType.Inactive)
                    {
                        continue;
                    }

                    // Space is new.  Add this space (bay)
                    Int64 spaceId = spaceFactory.Create(meter, space.BayNumber);
                    ParkingSpace newParkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);

                    // Add the remaining attributes.
                    newParkingSpace.Latitude = space.Latitude;
                    newParkingSpace.Longitude = space.Longitude;
                    newParkingSpace.DemandZoneId = space.DemandStatusId;
                    newParkingSpace.DisplaySpaceNum = space.BayName;

                    newParkingSpace.OperationalStatus = space.OperationalStatus;
                    newParkingSpace.SpaceStatus = (int)space.State;

                    Audit(newParkingSpace);
                }
                else if (space.IsChanged)
                {
                    // space data was changed.

                    // This should always return a parking space.
                    var parkingSpace = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == space.AssetId);
                    if (parkingSpace != null)
                    {
                        parkingSpace.Latitude = space.Latitude;
                        parkingSpace.Longitude = space.Longitude;
                        parkingSpace.DemandZoneId = space.DemandStatusId;
                        parkingSpace.DisplaySpaceNum = space.BayName;
                        parkingSpace.OperationalStatus = space.OperationalStatus;

                        Audit(parkingSpace);
                    }
                }
            }

            PemsEntities.SaveChanges();


            // Update meter with number of meters (Meters.MaxBaysEnabled), min and max bay numbers (Meters.BayStart, Meters.BayEnd)
            meter.BayStart = int.MaxValue;
            meter.BayEnd = int.MinValue;
            meter.MaxBaysEnabled = 0;
            var activeSpaces = from s in meter.ParkingSpaces
                               where s.OperationalStatus != (int)OperationalStatusType.Inactive
                               where s.SpaceStatus != (int)AssetStateType.Historic
                               select s;
            foreach (var activeSpace in activeSpaces)
            {
                meter.MaxBaysEnabled++;
                meter.BayStart = activeSpace.BayNumber < meter.BayStart ? activeSpace.BayNumber : meter.BayStart;
                meter.BayEnd = activeSpace.BayNumber > meter.BayEnd ? activeSpace.BayNumber : meter.BayEnd;
            }

            Audit(meter);
            PemsEntities.SaveChanges();

        }


        #endregion


        #region Get Edit Models

        /// <summary>
        /// Returns an instance of <see cref="MeterEditModel"/> reflecting the present and pending state
        /// and settings of a <see cref="Meter"/> for customer id of <paramref name="customerId"/>,
        /// an area id of <paramref name="areaId"/> and a meter id of <paramref name="meterId"/>.
        ///  This model is used for edit page.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="meterId">The meter id</param>
        /// <returns>An instance of <see cref="MeterEditModel"/></returns>
        public MeterEditModel GetEditModel(int customerId, int areaId, int meterId)
        {
            var model = new MeterEditModel
            {
                CustomerId = customerId,
                AreaId = areaId,
                AssetId = meterId
            };

            // Get the meter from Meters
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == meterId);

            if (meter.MechanismMaster != null)
                model.MechanismId = meter.MechanismMaster.MechanismId;

            // First build the edit model from the existing meter.
            #region Build model from a meter

            model.GlobalId = meter.GlobalMeterId.HasValue ? meter.GlobalMeterId.ToString() : "";

            // Get Meter Type
            model.Type = meter.MeterGroup1 == null ? MeterViewModel.DefaultType : meter.MeterGroup1.MeterGroupDesc ?? MeterViewModel.DefaultType;
            //model.TypeId = (MeterGroups)(meter.MeterType ?? (int) MeterGroups.SingleSpaceMeter);
            model.TypeId = (MeterGroups)meter.MeterGroup;

            // Get meter models.
            model.AssetModelId = meter.MeterType ?? -1;

            // Get meter name.
            model.Name = meter.MeterName ?? "";

            // Get street name
            model.Street = meter.Location ?? "";

            // Get phone number
            model.PhoneNumber = meter.SMSNumber ?? "";

            // Get Zone and Suburb
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.Areaid == areaId && m.MeterId == meterId);
            if (meterMap != null)
            {
                model.AreaListId = meterMap.AreaId2 ?? -1;
                model.ZoneId = meterMap.ZoneId ?? -1;
                model.SuburbId = meterMap.CustomGroup1 ?? -1;

                var location = PemsEntities.HousingMasters.FirstOrDefault(m => m.Customerid == customerId && m.HousingId == meterMap.HousingId);
                if (location != null)
                    model.LocationName = location.HousingName;
                else
                    model.LocationName = "";


                var Mechmaster = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechId == meterMap.MechId);
                if (Mechmaster != null)
                    model.MechSerialNo = Mechmaster.MechSerial;
                else
                    model.MechSerialNo = "";

                // Collection Route
                if (meterMap.CollRouteId != null)
                {
                    model.CollectionRoute = meterMap.CollRoute == null ? "" : meterMap.CollRoute.DisplayName ?? "";
                }
                else
                {
                    model.CollectionRoute = "";
                }

                // Maintenace Route
                model.MaintenanceRouteId = meterMap.MaintRouteId ?? -1;
            }
            else
            {
                model.AreaListId = -1;
                model.ZoneId = -1;
                model.SuburbId = -1;

                // Collection Route
                model.CollectionRoute = "";

                // Maintenace Route
                model.MaintenanceRouteId = -1;
            }


            // Lat/Long
            model.Latitude = meter.Latitude ?? 0.0;
            model.Longitude = meter.Longitude ?? 0.0;

            // Number of spaces
            model.Spaces = meter.MaxBaysEnabled ?? 0;

            // Preventative maintenance dates
            model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value : (DateTime?)null;

            // Warantry expiration
            model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;

            // Version profile information
            model.Configuration = new AssetConfigurationModel();
            model.Configuration.DateInstalled = meter.InstallDate;
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == meterId);
            if (versionProfile != null)
            {
                model.Configuration.MPVVersion = versionProfile.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfile.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfile.HardwareVersion ?? "";
                model.Configuration.ConfigurationId = versionProfile.VersionProfileMeterId;
                model.Configuration.ConfigurationName = versionProfile.ConfigurationName ?? "";
            }

            // Operational status of meter.
            GetOperationalStatusDetails(model, meter.OperationalStatusID ?? 0, meter.OperationalStatusTime.HasValue ? meter.OperationalStatusTime.Value : DateTime.Now);

            //GetOperationalStatusList(model, meter.OperationalStatusID ?? 0);

            // Is asset active?
            model.StateId = meter.MeterState ?? (int)AssetStateType.Pending;

            #endregion


            // If there is an asset pending record then update edit model with associated values.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == meterId);

            if (assetPending != null)
            {
                #region Build model from an assetPending

                // Get meter models.
                model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;

                // Get meter name.
                model.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? model.Name : assetPending.AssetName;

                // Get street name
                model.Street = string.IsNullOrWhiteSpace(assetPending.LocationMeters) ? model.Street : assetPending.LocationMeters;

                // Get phone number
                model.PhoneNumber = string.IsNullOrWhiteSpace(assetPending.PhoneNumber) ? model.PhoneNumber : assetPending.PhoneNumber;

                // Get Zone, Area and Suburb
                model.AreaListId = assetPending.MeterMapAreaId2 ?? model.AreaListId;
                model.ZoneId = assetPending.MeterMapZoneId ?? model.ZoneId;
                model.SuburbId = assetPending.MeterMapSuburbId ?? model.SuburbId;

                meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.Areaid == areaId && m.MeterId == meterId);
                if (meterMap != null)
                {
                    // Collection Route
                    if (meterMap.CollRouteId != null)
                    {
                        model.CollectionRoute = meterMap.CollRoute == null ? "" : meterMap.CollRoute.DisplayName ?? "";
                    }
                    else
                    {
                        model.CollectionRoute = "";
                    }

                    // Maintenace Route
                    model.MaintenanceRouteId = meterMap.MaintRouteId ?? -1;
                }
                else
                {
                    // Collection Route
                    model.CollectionRoute = "";

                    // Maintenace Route
                    model.MaintenanceRouteId = -1;
                }


                // Lat/Long
                model.Latitude = assetPending.Latitude ?? model.Latitude;
                model.Longitude = assetPending.Longitude ?? model.Longitude;

                // Maint. Route
                model.MaintenanceRouteId = assetPending.MeterMapMaintenanceRoute ?? model.MaintenanceRouteId;

                // Preventative maintenance dates
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.HasValue ? assetPending.LastPreventativeMaintenance.Value.ToShortDateString() : model.LastPrevMaint;
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance ?? model.NextPrevMaint;

                // Warantry expiration
                model.WarrantyExpiration = assetPending.WarrantyExpiration ?? model.WarrantyExpiration;

                // Version profile information
                model.Configuration.DateInstalled = assetPending.DateInstalled ?? model.Configuration.DateInstalled;
                model.Configuration.MPVVersion = assetPending.MPVFirmware ?? model.Configuration.MPVVersion;
                model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion ?? model.Configuration.SoftwareVersion;
                model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion ?? model.Configuration.FirmwareVersion;

                // Operational status of meter.
                if (assetPending.OperationalStatus != null)
                {
                    GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);
                }
                //                GetOperationalStatusList(model, (int)(assetPending.OperationalStatus ?? meter.OperationalStatusID));

                // Asset state is pending
                model.StateId = (int)AssetStateType.Pending;

                // Activation Date
                model.ActivationDate = assetPending.RecordMigrationDateTime;

                #endregion
            }

            // Get asset model list
            GetAssetModelList(model, meter.MeterGroup);

            // Get zone list
            GetZoneList(model);

            // Get suburb list
            GetSuburbList(model);

            // Get Areas list
            GetAreaList(model);

            // Maintenace Route
            GetMaintenanceRouteList(model);

            // Is asset active?
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
        /// Refreshes any lists in the <see cref="MeterEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="MeterEditModel"/> to refresh</param>
        public void RefreshEditModel(MeterEditModel editModel)
        {
            // Get meter models.
            var meterGroup = PemsEntities.MeterGroups.FirstOrDefault(m => m.MeterGroupDesc.Equals(editModel.Type, StringComparison.InvariantCultureIgnoreCase));

            GetAssetModelList(editModel, meterGroup == null ? (int)MeterGroups.MultiSpaceMeter : meterGroup.MeterGroupId);

            // Get zone list
            GetZoneList(editModel);

            // Get suburb list
            GetSuburbList(editModel);

            // Get Areas list
            GetAreaList(editModel);

            // Maintenace Route
            GetMaintenanceRouteList(editModel);

            //// Operational status of meter.
            //GetOperationalStatusList(editModel, editModel.Status.StatusId);

            // Get list of states
            GetAssetStateList(editModel);
        }


        /// <summary>
        /// Saves the updates of an instance of <see cref="MeterEditModel"/> to the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of a <see cref="MeterEditModel"/> with updates</param>
        public void SetEditModel(MeterEditModel model)
        {
            // Note:  At this time all changes to the meter are written to the [AssetPending] table.

            // Use installation date to determine asset state
            if (model.Configuration.DateInstalled.HasValue)
            {
                // If meter.InstallDate is <= customer "now" then set meter to  AssetStateType.Current
                if (model.Configuration.DateInstalled < Now)
                {
                    model.StateId = (int)AssetStateType.Current;
                }
            }

            // Get the existing model so can determine what has changed
            var originalModel = GetEditModel(model.CustomerId, model.AreaId, (int)model.AssetId);

            // Write changes to [AssetPending] table.
            (new PendingFactory(ConnectionStringName, Now)).Save(model, originalModel);

        }

        /// <summary>
        /// Takes a list of <see cref="AssetIdentifier"/> pointing to a list of meters and creates the model
        /// used to mass edit the list of meters.  The model contains only fields that can be changed for all
        /// items in the list.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="meterIds">A <see cref="List{AssetIdentifier}"/> indicating meters to be edited</param>
        /// <returns>An instance of <see cref="MeterMassEditModel"/></returns>
        public MeterMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> meterIds)
        {
            MeterMassEditModel editModel = new MeterMassEditModel()
                {
                    CustomerId = customerId
                };
            editModel.SetList(meterIds);

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

            // Initialize ActivationDate and set to midnight.
            editModel.ActivationDate = SetToMidnight(Now);

            return editModel;
        }

        /// <summary>
        /// Refreshes any lists in the <see cref="MeterMassEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="model">The instance of the <see cref="MeterMassEditModel"/> to refresh</param>
        public void RefreshMassEditModel(MeterMassEditModel model)
        {
            // Get State list
            GetAssetStateList(model);

            // Get zone list
            model.Zone = ZoneList(model.CustomerId, model.ZoneId);

            // Get suburb list
            model.Suburb = SuburbList(model.CustomerId, model.SuburbId);

            // Get Areas list
            model.Area = AreaList(model.CustomerId, model.AreaListId);

            // Maintenace Route
            model.MaintenanceRoute = MaintenanceRouteList(model.CustomerId, model.MaintenanceRouteId);
        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="MeterMassEditModel"/> to the [AssetPending] table for each
        /// meter in the <see cref="MeterMassEditModel.AssetIds"/> list.  Only values that have been changed are 
        /// stored.
        /// </summary>
        /// <param name="model">Instance of a <see cref="MeterMassEditModel"/> with updates</param>
        public void SetMassEditModel(MeterMassEditModel model)
        {
            var pendingFactory = new PendingFactory(ConnectionStringName, Now);
            foreach (var asset in model.AssetIds())
            {
                // Save changes to the [AssetPending] table.
                pendingFactory.Save(model, model.CustomerId, asset.AreaId, (int)asset.AssetId);
            }
        }


        public void SetSpecialAction(int[] ids, int customerId, int assetareaId, int specailAction)
        {
            foreach (var item in ids)
            {
                var lmd = (from s in PemsEntities.LatestMeterDatas
                           where s.MID.Value == item && s.CID.Value == customerId && s.AID.Value == assetareaId
                           select s).FirstOrDefault();

                if (lmd == default(LatestMeterData))
                {
                    //check If it has meter data .Else ignore
                    var meter = (from s in PemsEntities.Meters
                                 where s.MeterId == item && s.CustomerID == customerId && s.AreaID == assetareaId
                                 select s).FirstOrDefault();

                    if (meter != default(Meter))
                    {
                        lmd = new LatestMeterData() { CID = customerId, AID = assetareaId, MID = item, SpecialAction = specailAction == -1 ? (int?)null : specailAction , SpecialActionUpdateDateTime = DateTime.Now};
                        PemsEntities.LatestMeterDatas.Add(lmd);

                        ////////////////
                        LatestMeterDataAudit lmda = new LatestMeterDataAudit()
                        {
                            // latestmeterdataID = lmd.ID,
                            CID = lmd.CID,
                            AID = lmd.AID,
                            MID = lmd.MID,
                            SpecialAction = lmd.SpecialAction,
                            Auditdate = DateTime.Now,

                            Voltage = lmd.Voltage,
                            VoltTS = lmd.VoltTS,
                            IPPort = lmd.IPPort,
                            IpAddress = lmd.IpAddress,
                            IPSyncTS = lmd.IPSyncTS,
                            DK2MTR_MPBBin_Name = lmd.DK2MTR_MPBBin_Name,
                            DK2MTR_MPBCfg_Name = lmd.DK2MTR_MPBCfg_Name,
                            DK2MTR_MBPgm_Name = lmd.DK2MTR_MBPgm_Name,
                            DK2MTR_Ccf_Name = lmd.DK2MTR_Ccf_Name,
                            DK2MTR_RPG_Name = lmd.DK2MTR_RPG_Name,
                            DK2MTR_SNSR_Name = lmd.DK2MTR_SNSR_Name,
                            DK2MTR_MPBBin_Ts = lmd.DK2MTR_MPBBin_Ts,
                            DK2MTR_MPBCfg_Ts = lmd.DK2MTR_MPBCfg_Ts,
                            DK2MTR_MBPgm_Ts = lmd.DK2MTR_MBPgm_Ts,
                            DK2MTR_Ccf_Ts = lmd.DK2MTR_Ccf_Ts,
                            DK2MTR_RPG_Ts = lmd.DK2MTR_RPG_Ts,
                            DK2MTR_SNSR_Ts = lmd.DK2MTR_SNSR_Ts,
                            MTR2DK_MPBBin_Name = lmd.MTR2DK_MPBBin_Name,
                            MTR2DK_MPBCfg_Name = lmd.MTR2DK_MPBCfg_Name,
                            MTR2DK_MBPgm_Name = lmd.MTR2DK_MBPgm_Name,
                            MTR2DK_Ccf_Name = lmd.MTR2DK_Ccf_Name,
                            MTR2DK_RPG_Name = lmd.MTR2DK_RPG_Name,
                            MTR2DK_SNSR_Name = lmd.MTR2DK_SNSR_Name,
                            MTR2DK_MPBBin_Ts = lmd.MTR2DK_MPBBin_Ts,
                            MTR2DK_MPBCfg_Ts = lmd.MTR2DK_MPBCfg_Ts,
                            MTR2DK_MBPgm_Ts = lmd.MTR2DK_MBPgm_Ts,
                            MTR2DK_Ccf_Ts = lmd.MTR2DK_Ccf_Ts,
                            MTR2DK_RPG_Ts = lmd.MTR2DK_RPG_Ts,
                            MTR2DK_SNSR_Ts = lmd.MTR2DK_SNSR_Ts,
                            FD_MPBBin_FileId = lmd.FD_MPBBin_FileId,
                            FD_MPBCfg_FileId = lmd.FD_MPBCfg_FileId,
                            FD_MBPgm_FileId = lmd.FD_MBPgm_FileId,
                            FD_MBCcf_FileId = lmd.FD_MBCcf_FileId,
                            FD_MBRpg_FileId = lmd.FD_MBRpg_FileId,
                            FD_SNSR_FileId = lmd.FD_SNSR_FileId,
                            FD_MPBBin_TS = lmd.FD_MPBBin_TS,
                            FD_MPBCfg_TS = lmd.FD_MPBCfg_TS,
                            FD_MBPgm_TS = lmd.FD_MBPgm_TS,
                            FD_MBCcf_TS = lmd.FD_MBCcf_TS,
                            FD_MBRpg_TS = lmd.FD_MBRpg_TS,
                            FD_SNSR_TS = lmd.FD_SNSR_TS,
                            dk2meter_xfer_ts = lmd.dk2meter_xfer_ts,
                            meter2dk_xfer_ts = lmd.meter2dk_xfer_ts
                            //,
                            //FD_MPBBin_FileStatus =  lmd.FD_MPBBin_FileStatus ,
                            //FD_MPBCfg_FileStatus =  lmd.FD_MPBCfg_FileStatus ,
                            //FD_MBPgm_FileStatus =  lmd.FD_MBPgm_FileStatus ,
                            //FD_MBCcf_FileStatus =  lmd.FD_MBCcf_FileStatus ,
                            //FD_MBRpg_FileStatus =  lmd.FD_MBRpg_FileStatus ,
                            //FD_SNSR_FileStatus =  lmd.FD_SNSR_FileStatus 

                        };

                        PemsEntities.LatestMeterDataAudits.Add(lmda);
                        ///////////////////////////
                        PemsEntities.SaveChanges();
                    }
                }
                else
                {
                    //lmd.SpecialAction = specailAction == -1 ? (int?)null : specailAction;
                    lmd.SpecialAction = specailAction == -1 ? 0 : specailAction;
                    lmd.SpecialActionUpdateDateTime = DateTime.Now;
                    PemsEntities.SaveChanges();
                    ///////////////////////////
                    LatestMeterDataAudit lmda = new LatestMeterDataAudit()
                    {
                        latestmeterdataID = lmd.ID,
                        CID = lmd.CID,
                        AID = lmd.AID,
                        MID = lmd.MID,
                        SpecialAction = lmd.SpecialAction,
                        Auditdate = DateTime.Now
                    };
                    PemsEntities.LatestMeterDataAudits.Add(lmda);
                    PemsEntities.SaveChanges();
                    ////////////////////


                    ////Insert to audit table
                    //LatestMeterDataAudit lmda = new LatestMeterDataAudit()
                    //{
                    //    latestmeterdataID = lmd.ID,
                    //    CID = lmd.CID,
                    //    AID = lmd.AID,
                    //    MID = lmd.MID,
                    //    SpecialAction = lmd.SpecialAction

                    //};
                    //PemsEntities.LatestMeterDataAudits.Add(lmda);
                    //PemsEntities.SaveChanges();
                }
            }
        }
        #endregion

        #region Create/Clone

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Meter"/> refrenced by <paramref name="assetId"/>,
        /// <paramref name="areaId"/>, and <paramref name="customerId"/>.  This cloned
        /// <see cref="Meter"/> is written to the [Meters] table and the [AssetPending] table.
        /// If <paramref name="bayStart"/> and <paramref name="bayEnd"/> are given, create associated spaces
        /// and join to the meter that is created by the cloning operation.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="assetId">The meter id to clone</param>
        /// <param name="bayStart">Integer indicating first bay number [optional]</param>
        /// <param name="bayEnd">Integer indicating last bay number [optional]</param>
        /// <returns>Integer representing the newly clone meter id (<see cref="Meter.MeterId"/>)</returns>
        private int Clone(int customerId, int areaId, Int64 assetId, int bayStart = 0, int bayEnd = 0)
        {
            // Get original meter.
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == assetId);

            // Create a new meter with the same area id.
            Meter newMeter = CreateBaseAsset(meter.CustomerID, (MeterGroups)meter.MeterGroup, meter.AreaID);

            // Align clonable meter fields.
            newMeter.Location = meter.Location;
            newMeter.DemandZone = meter.DemandZone;
            newMeter.FreeParkingMinute = meter.FreeParkingMinute;
            newMeter.MeterGroup = meter.MeterGroup;
            newMeter.MeterName = "";
            newMeter.MeterType = meter.MeterType;
            newMeter.TimeZoneID = meter.TimeZoneID;
            newMeter.TypeCode = meter.TypeCode;
            newMeter.WarrantyExpiration = meter.WarrantyExpiration;
            newMeter.NextPreventativeMaintenance = meter.NextPreventativeMaintenance;
            if (bayStart > 0 && bayEnd > 0 && bayStart <= bayEnd)
            {
                var spaceFactory = new SpaceFactory(ConnectionStringName, Now);
                newMeter.BayStart = bayStart;
                newMeter.BayEnd = bayEnd;

                int baysEnabled = 0;
                //now, for each of these bays, create a space using SpaceFactory.Create
                for (int bayNumber = bayStart; bayNumber <= bayEnd; bayNumber++)
                {
                    baysEnabled++;
                    spaceFactory.Create(newMeter, bayNumber);

                }
                //get the max bays by end - start unless they are equal
                newMeter.MaxBaysEnabled = baysEnabled;
            }

            // Align meter map clonable fields.
            var meterMap = meter.MeterMaps.FirstOrDefault();

            // Create meter map for cloned meter.
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
            newMeterMap.CustomGroup1 = meterMap.CustomGroup1;
            newMeterMap.CustomGroup2 = meterMap.CustomGroup2;
            newMeterMap.CustomGroup3 = meterMap.CustomGroup3;
            newMeterMap.MaintRouteId = meterMap.MaintRouteId;

            PemsEntities.SaveChanges();


            // Create a [VesionProfileMete] entry
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == assetId);
            if (versionProfile != null)
            {
                // Does an entry exist?
                var newVersionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == newMeter.MeterId);
                if (newVersionProfile == null)
                {
                    newVersionProfile = new VersionProfileMeter()
                        {
                            CustomerId = customerId,
                            AreaId = newMeter.AreaID,
                            MeterId = newMeter.MeterId,
                            MeterGroup = (int)newMeter.MeterGroup
                        };
                }

                newVersionProfile.ConfigurationName = versionProfile.ConfigurationName;
                newVersionProfile.CommunicationVersion = versionProfile.CommunicationVersion;
                newVersionProfile.SoftwareVersion = versionProfile.SoftwareVersion;
                newVersionProfile.HardwareVersion = versionProfile.HardwareVersion;

                PemsEntities.VersionProfileMeters.Add(newVersionProfile);
                Audit(newVersionProfile);
            }


            // Set audit records.
            Audit(newMeter);
            Audit(newMeterMap);


            // Add AssetPending record
            (new PendingFactory(ConnectionStringName, Now)).SetImportPending(newMeter.MeterGroup == (int)MeterGroups.SingleSpaceMeter ?
                AssetTypeModel.AssetType.SingleSpaceMeter : AssetTypeModel.AssetType.MultiSpaceMeter,
                                                                       SetToMidnight(Now),
                                                                       AssetStateType.Current,
                                                                       customerId,
                                                                       newMeter.MeterId,
                                                                       (int)AssetStateType.Pending, (int)OperationalStatusType.Inactive,
                                                                       newMeter.AreaID);


            return newMeter.MeterId;
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Meter"/> refrenced by <paramref name="model"/>.
        /// This cloned <see cref="Meter"/> is written to the [Meters] table and the [AssetPending] table.
        /// If <paramref name="bayStart"/> and <paramref name="bayEnd"/> are given, create associated spaces
        /// and join to the meter that is created by the cloning operation.
        /// </summary>
        /// <param name="model">Instance of <see cref="MeterViewModel"/> to clone from</param>
        /// <param name="bayStart">Integer indicating first bay number [optional]</param>
        /// <param name="bayEnd">Integer indicating last bay number [optional]</param>
        /// <returns>Integer representing the newly clone meter id (<see cref="Meter.MeterId"/>)</returns>
        public int Clone(MeterViewModel model, int bayStart, int bayEnd)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId, bayStart, bayEnd);
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Meter"/> refrenced by <paramref name="model"/>.
        /// This cloned <see cref="Meter"/> is written to the [Meters] table and the [AssetPending] table.
        /// If <paramref name="bayStart"/> and <paramref name="bayEnd"/> are given, create associated spaces
        /// and join to the meter that is created by the cloning operation.
        /// </summary>
        /// <param name="model">Instance of <see cref="MeterEditModel"/> to clone from</param>
        /// <param name="bayStart">Integer indicating first bay number [optional]</param>
        /// <param name="bayEnd">Integer indicating last bay number [optional]</param>
        /// <returns>Integer representing the newly clone meter id (<see cref="Meter.MeterId"/>)</returns>
        public int Clone(MeterEditModel model, int bayStart, int bayEnd)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId, bayStart, bayEnd);
        }

        /// <summary>
        /// Create a new meter and set it to <see cref="OperationalStatusType.Inactive"/> operational status (<see cref="Meter.OperationalStatusID"/>) 
        /// and a <see cref="AssetStateType.Pending"/> state (<see cref="Meter.MeterState"/>). Associate it with this <paramref name="customerId"/>.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="isSingleSpaceMeter">True if single space meter type (<see cref="MeterGroups.SingleSpaceMeter"/>), 
        /// otherwise multi-space meter type (<see cref="MeterGroups.MultiSpaceMeter"/>)</param>
        /// <param name="bayStart">Integer indicating first bay number</param>
        /// <param name="bayEnd">Integer indicating last bay number</param>
        /// <returns>Instance of <see cref="Meter"/> of new meter.</returns>
        public Meter Create(int customerId, bool isSingleSpaceMeter, int bayStart, int bayEnd, string MechSerialNo, string LocationName)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(customerId, isSingleSpaceMeter ? MeterGroups.SingleSpaceMeter : MeterGroups.MultiSpaceMeter);


            // they can set no bays for TX Meters, so if start and end are 0, react accordingly.

            // Update the bays
            newMeter.BayStart = bayStart;
            newMeter.BayEnd = bayEnd;
            int maxbays = 0;
            if (bayEnd > 0)
                maxbays = bayEnd - bayStart + 1;
            newMeter.MaxBaysEnabled = maxbays;

            // Create an audit entry.
            Audit(newMeter);
            MechMaster mechMaster = new MechMaster();
            if (MechSerialNo != "" && MechSerialNo != null)
            {
                MechSerialNo = MechSerialNo.Trim();
                var mechMasterlist = (from s in PemsEntities.MechMasters
                                      where s.MechSerial == MechSerialNo //&& s.Customerid == model.CustomerId
                                      select s).ToList();
                foreach (var mechmasterlistrow in mechMasterlist)
                {
                    var MechAssetpending = (from s in PemsEntities.AssetPendings
                                            where s.MeterMapMechId == mechmasterlistrow.MechId //&& s.CustomerId == model.CustomerId
                                            select s).ToList();
                    if (MechAssetpending != null)
                    {
                        MechAssetpending.ForEach(a => a.MeterMapMechId = null);
                        PemsEntities.SaveChanges();
                    }
                    var MechMeter = (from s in PemsEntities.Meters
                                     where s.MeterId == mechmasterlistrow.MechIdNumber && s.MeterGroup == (int)MeterGroups.Mechanism //&& s.CustomerID == model.CustomerId
                                     select s).FirstOrDefault();


                    var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                           where s.MechId == mechmasterlistrow.MechId  //&& s.Customerid == model.CustomerId// s.Areaid != MechMeter.AreaID//s.MeterId != mechMaster.MechIdNumber 
                                           select s).ToList();

                    if (mechMasterMeter != null)
                    {
                        mechMasterMeter.ForEach(a => a.MechId = null);
                        // mechMasterMeter.MechId = null;
                        PemsEntities.SaveChanges();
                    }
                }

                mechMaster = (from s in PemsEntities.MechMasters
                              where s.MechSerial == MechSerialNo && s.Customerid == customerId
                              select s).FirstOrDefault();

                if (mechMaster == null) //&& mechMaster.Customerid != model.CustomerId)
                {
                    mechMaster = (from s in PemsEntities.MechMasters
                                  where s.MechSerial == MechSerialNo
                                  select s).FirstOrDefault();

                    var MechMeter = (from s in PemsEntities.Meters
                                     where s.MeterId == mechMaster.MechIdNumber && s.MeterGroup == (int)MeterGroups.Mechanism //&& s.CustomerID == model.CustomerId
                                     select s).FirstOrDefault();

                    // mechMaster.MechSerial = null;
                    //  PemsEntities.SaveChanges();                     
                    Meter newMechMeter = CreateBaseAsset(customerId, MeterGroups.Mechanism);
                    newMechMeter.MeterStatus = (int)AssetStateType.Current;
                    newMechMeter.MeterState = (int)AssetStateType.Current;
                    newMechMeter.OperationalStatusID = (int)OperationalStatusType.Operational;
                    newMechMeter.OperationalStatusTime = DateTime.Now;
                    newMechMeter.MeterName = "AUTO CREATED";
                    PemsEntities.SaveChanges();


                    mechMaster.MechIdNumber = newMechMeter.MeterId;
                    mechMaster.Customerid = newMechMeter.CustomerID;
                    PemsEntities.SaveChanges();

                    var mechMasterMetermap = (from s in PemsEntities.MeterMaps
                                              where s.MeterId == MechMeter.MeterId && s.Customerid == MechMeter.CustomerID && s.Areaid == MechMeter.AreaID//s.MeterId != mechMaster.MechIdNumber 
                                              select s).FirstOrDefault();
                    mechMasterMetermap.Customerid = newMechMeter.CustomerID;
                    mechMasterMetermap.MeterId = newMechMeter.MeterId;
                    PemsEntities.SaveChanges();

                }

            }
            HousingMaster housingMasterObject = new HousingMaster();
            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            if (isSingleSpaceMeter == false)
                housingMasterObject = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));
            else
            {
                // //  Create a [HousingMaster] for single space



                housingMasterObject.Customerid = customerId;
                housingMasterObject.HousingName = LocationName;
                housingMasterObject.Block = "Default";
                housingMasterObject.StreetName = "Default";
                housingMasterObject.StreetType = "Default";
                housingMasterObject.StreetDirection = "Default";
                housingMasterObject.IsActive = false;
                housingMasterObject.CreateDate = DateTime.Now;

                PemsEntities.HousingMasters.Add(housingMasterObject);
                PemsEntities.SaveChanges();




                housingMasterObject = (from s in PemsEntities.HousingMasters
                                       where s.HousingName == LocationName && s.Customerid == customerId
                                       select s).FirstOrDefault();
            }


            // Create a [MeterMap] entry
            MeterMap newMeterMap = new MeterMap();

            newMeterMap.Customerid = newMeter.CustomerID;
            newMeterMap.Areaid = newMeter.AreaID;
            newMeterMap.MeterId = newMeter.MeterId;
            newMeterMap.HousingId = housingMasterObject.HousingId;
            if (isSingleSpaceMeter == true && mechMaster.MechId != 0)
            {
                newMeterMap.MechId = mechMaster.MechId;
            }

            PemsEntities.MeterMaps.Add(newMeterMap);
            PemsEntities.SaveChanges();

            // Creat audit entry
            Audit(newMeterMap);

            // Create the parking spaces. - only do this if the max bays are greater than zero, otherwise we dont want any spaces.
            if (maxbays > 0)
            {
                SpaceFactory spaceFactory = new SpaceFactory(ConnectionStringName, Now);
                for (int bayNumber = bayStart; bayNumber <= bayEnd; bayNumber++)
                {
                    spaceFactory.Create(newMeter, bayNumber);
                }
            }

            //// Create the parking spaces. - only do this if the max bays are greater than zero, otherwise we dont want any spaces.
            //if (maxbays > 0)
            //{
            //    spaceFactory.Create( newMeter, bayNumber );
            //}


            return newMeter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="isSingleSpaceMeter"></param>
        /// <param name="bayStart"></param>
        /// <param name="bayEnd"></param>
        /// <param name="enterMode"></param>
        /// <param name="meterId"></param>
        /// <returns></returns>
        public Meter Create(int customerId, bool isSingleSpaceMeter, int bayStart, int bayEnd, AssetTypeModel.EnterMode enterMode, int meterId, string MechSerialNo, string LocationName)
        {
            Meter newMeter = CreateBaseAsset(enterMode, meterId, customerId, isSingleSpaceMeter ? MeterGroups.SingleSpaceMeter : MeterGroups.MultiSpaceMeter);

            // Update the bays
            newMeter.BayStart = bayStart;
            newMeter.BayEnd = bayEnd;
            int maxbays = 0;
            if (bayEnd > 0)
                maxbays = bayEnd - bayStart + 1;
            newMeter.MaxBaysEnabled = maxbays;

            //// Update the bays
            //newMeter.BayStart = bayStart;
            //newMeter.BayEnd = bayEnd;
            //newMeter.MaxBaysEnabled = bayEnd - bayStart + 1;

            // Create an audit entry.
            Audit(newMeter);
            MechMaster mechMaster = new MechMaster();
            if (MechSerialNo != "" && MechSerialNo != null)
            {
                MechSerialNo = MechSerialNo.Trim();
                var mechMasterlist = (from s in PemsEntities.MechMasters
                                      where s.MechSerial == MechSerialNo //&& s.Customerid == model.CustomerId
                                      select s).ToList();
                foreach (var mechmasterlistrow in mechMasterlist)
                {
                    var MechAssetpending = (from s in PemsEntities.AssetPendings
                                            where s.MeterMapMechId == mechmasterlistrow.MechId //&& s.CustomerId == model.CustomerId
                                            select s).ToList();
                    if (MechAssetpending != null)
                    {
                        MechAssetpending.ForEach(a => a.MeterMapMechId = null);
                        PemsEntities.SaveChanges();
                    }
                    var MechMeter = (from s in PemsEntities.Meters
                                     where s.MeterId == mechmasterlistrow.MechIdNumber && s.MeterGroup == (int)MeterGroups.Mechanism //&& s.CustomerID == model.CustomerId
                                     select s).FirstOrDefault();

                    var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                           where s.MechId == mechmasterlistrow.MechId  //&& s.Customerid == model.CustomerId// s.Areaid != MechMeter.AreaID//s.MeterId != mechMaster.MechIdNumber 
                                           select s).ToList();

                    if (mechMasterMeter != null)
                    {
                        mechMasterMeter.ForEach(a => a.MechId = null);
                        // mechMasterMeter.MechId = null;
                        PemsEntities.SaveChanges();
                    }
                }

                mechMaster = (from s in PemsEntities.MechMasters
                              where s.MechSerial == MechSerialNo && s.Customerid == customerId
                              select s).FirstOrDefault();

                if (mechMaster == null) //&& mechMaster.Customerid != model.CustomerId)
                {
                    mechMaster = (from s in PemsEntities.MechMasters
                                  where s.MechSerial == MechSerialNo
                                  select s).FirstOrDefault();

                    var MechMeter = (from s in PemsEntities.Meters
                                     where s.MeterId == mechMaster.MechIdNumber && s.MeterGroup == (int)MeterGroups.Mechanism //&& s.CustomerID == model.CustomerId
                                     select s).FirstOrDefault();

                    // mechMaster.MechSerial = null;
                    //  PemsEntities.SaveChanges();                     
                    Meter newMechMeter = CreateBaseAsset(customerId, MeterGroups.Mechanism);
                    newMechMeter.MeterStatus = (int)AssetStateType.Current;
                    newMechMeter.MeterState = (int)AssetStateType.Current;
                    newMechMeter.OperationalStatusID = (int)OperationalStatusType.Operational;
                    newMechMeter.OperationalStatusTime = DateTime.Now;
                    newMechMeter.MeterName = "AUTO CREATED";
                    PemsEntities.SaveChanges();


                    mechMaster.MechIdNumber = newMechMeter.MeterId;
                    mechMaster.Customerid = newMechMeter.CustomerID;
                    PemsEntities.SaveChanges();

                    //var mechMasterMetermap = (from s in PemsEntities.MeterMaps
                    //                          where s.MechId == mechMaster.MechId && s.MeterId == MechMeter.MeterId //&& s.Customerid == model.CustomerId// s.Areaid != MechMeter.AreaID//s.MeterId != mechMaster.MechIdNumber 
                    //                          select s).FirstOrDefault();
                    var mechMasterMetermap = (from s in PemsEntities.MeterMaps
                                              where s.MeterId == MechMeter.MeterId && s.Customerid == MechMeter.CustomerID && s.Areaid == MechMeter.AreaID//s.MeterId != mechMaster.MechIdNumber 
                                              select s).FirstOrDefault();
                    mechMasterMetermap.Customerid = newMechMeter.CustomerID;
                    mechMasterMetermap.MeterId = newMechMeter.MeterId;
                    PemsEntities.SaveChanges();

                }

            }
            HousingMaster housingMasterObject = new HousingMaster();
            //  Need a [HousingMaster]

            if (isSingleSpaceMeter == false)
                housingMasterObject = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));
            else
            {
                // //  Create a [HousingMaster] for single space



                housingMasterObject.Customerid = customerId;
                housingMasterObject.HousingName = LocationName;
                housingMasterObject.Block = "Default";
                housingMasterObject.StreetName = "Default";
                housingMasterObject.StreetType = "Default";
                housingMasterObject.StreetDirection = "Default";
                housingMasterObject.IsActive = false;
                housingMasterObject.CreateDate = DateTime.Now;

                PemsEntities.HousingMasters.Add(housingMasterObject);
                PemsEntities.SaveChanges();


                housingMasterObject = (from s in PemsEntities.HousingMasters
                                       where s.HousingName == LocationName && s.Customerid == customerId
                                       select s).FirstOrDefault();
            }


            // Create a [MeterMap] entry
            MeterMap newMeterMap = new MeterMap();

            newMeterMap.Customerid = newMeter.CustomerID;
            newMeterMap.Areaid = newMeter.AreaID;
            newMeterMap.MeterId = newMeter.MeterId;
            newMeterMap.HousingId = housingMasterObject.HousingId;
            if (isSingleSpaceMeter == true && mechMaster.MechId != 0)
            {
                newMeterMap.MechId = mechMaster.MechId;
            }

            PemsEntities.MeterMaps.Add(newMeterMap);
            PemsEntities.SaveChanges();

            // Creat audit entry
            Audit(newMeterMap);

            // Create the parking spaces. - only do this if the max bays are greater than zero, otherwise we dont want any spaces.
            if (maxbays > 0)
            {
                SpaceFactory spaceFactory = new SpaceFactory(ConnectionStringName, Now);
                for (int bayNumber = bayStart; bayNumber <= bayEnd; bayNumber++)
                {
                    spaceFactory.Create(newMeter, bayNumber);
                }
            }

            //// Create the parking spaces.
            //SpaceFactory spaceFactory = new SpaceFactory(ConnectionStringName);
            //for (int bayNumber = bayStart; bayNumber <= bayEnd; bayNumber++)
            //{
            //    spaceFactory.Create(newMeter, bayNumber);
            //}


            return newMeter;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Resets a meter by writing a reset record into the [MeterResetSchedule] table.  The meter
        /// is referenced by the <paramref name="assetIdentifier"/> and <paramref name="userId"/> indicates
        /// who requested the reset.
        /// </summary>
        /// <param name="assetIdentifier">Instance of <see cref="AssetIdentifier"/> indicating meter to reset</param>
        /// <param name="userId">User id identifying requesting user</param>
        /// <returns>True indicating <see cref="MeterResetSchedule"/> was created and written to [MeterResetSchedule] table</returns>
        public bool Reset(AssetIdentifier assetIdentifier, int userId)
        {
            var meterResetSchedule = new MeterResetSchedule();

            meterResetSchedule.CustomerId = assetIdentifier.CustomerId;
            meterResetSchedule.AreaId = assetIdentifier.AreaId;
            meterResetSchedule.MeterId = (int)assetIdentifier.AssetId;

            meterResetSchedule.ResetDateTime = DateTime.Now;

            meterResetSchedule.UserId = userId;

            PemsEntities.MeterResetSchedules.Add(meterResetSchedule);

            return PemsEntities.SaveChanges() > 0;
        }


        /// <summary>
        /// Adds time to a meter writing a time record into the [MeterPushSchedule] table.  The meter
        /// is referenced by the <paramref name="assetIdentifier"/> and <paramref name="userId"/> indicates
        /// who requested the reset.
        /// 
        /// <paramref name="minutesToAdd"/> indicates the time to add in minutes.  <paramref name="bayNumber"/> indicates
        /// the particular bay to which to add time.
        /// </summary>
        /// <param name="assetIdentifier">Instance of <see cref="AssetIdentifier"/> indicating sensor to reset</param>
        /// <param name="minutesToAdd">Minutes to add</param>
        /// <param name="bayNumber">Bay number</param>
        /// <param name="userId">User id identifying requesting user</param>
        /// <returns>True indicating <see cref="MeterPushSchedule"/> was created and written to [MeterPushSchedule] table</returns>
        public bool AddTime(AssetIdentifier assetIdentifier, int minutesToAdd, int bayNumber, int userId)
        {
            var meterPushSchedule = new MeterPushSchedule();

            meterPushSchedule.CustomerId = assetIdentifier.CustomerId;
            meterPushSchedule.AreaId = assetIdentifier.AreaId;
            meterPushSchedule.MeterId = (int)assetIdentifier.AssetId;
            meterPushSchedule.BayNumber = bayNumber;

            meterPushSchedule.CreatedTime = DateTime.Now;
            meterPushSchedule.ExpiryTime = DateTime.Now.AddHours(1);

            meterPushSchedule.UserId = userId;

            PemsEntities.MeterPushSchedules.Add(meterPushSchedule);

            return PemsEntities.SaveChanges() > 0;
        }


        /// <summary>
        /// Gets a <see cref="List{CollectionMeter}"/> of all meters for a <paramref name="customerId"/>.  
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns><see cref="List{CollectionMeter}"/> of meters</returns>
        public List<CollectionMeter> GetAllMeters(int customerId)
        {
            var items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         select new CollectionMeter
                             {
                                 MeterId = m.MeterId,
                                 MeterName = m.MeterName,
                                 Street = m.Location,
                                 CustomerId = m.CustomerID,
                                 AreaId = mm.Areaid,
                                 AreaId2 = mm.AreaId2,
                                 Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                                 Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                                 ZoneId = mm.ZoneId,
                                 Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                                 CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                             }).ToList();
            return items;
        }

        /// <summary>
        /// Gets a <see cref="List{Int32}"/> of distinct meter ids for a <paramref name="customerId"/>.  
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns><see cref="List{Int32}"/> of distinct meter ids</returns>
        public List<int> GetDistinctMeterIds(int customerId)
        {
            var items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         select m.MeterId).Distinct().ToList();
            return items;
        }


        #endregion

        #region History

        /// <summary>
        /// Gets a <see cref="List{MeterViewModel}"/> representing the known history of a
        /// meter referenced by meter id of <paramref name="assetId"/>, <paramref name="areaId"/>, and customer id of <paramref name="customerId"/>.
        /// Return only a list that are between <paramref name="startDateTicks"/> and <paramref name="endDateTicks"/>
        /// </summary>
        /// <param name="request">Instance of <see cref="DataSourceRequest"/> from Kendo UI</param>
        /// <param name="total">Out parameter used to return total number of records being returned</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="assetId">The meter id</param>
        /// <param name="startDateTicks">From date in ticks</param>
        /// <param name="endDateTicks">To date in ticks</param>
        /// <returns><see cref="List{MeterViewModel}"/> instance</returns>
        public List<MeterViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {
            DateTime startDate;
            DateTime endDate;

            IQueryable<MetersAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.MetersAudits.Where(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == assetId
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.MetersAudits.Where(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.MetersAudits.Where(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.MetersAudits.Where(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == assetId).OrderBy(m => m.UpdateDateTime);
            }

            // Get the MeterViewModel for each audit record.
            var list = new List<MeterViewModel>();

            // Get present data
            var presentModel = GetViewModel(customerId, areaId, (int)assetId);
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data
            MeterViewModel previousModel = null;
            foreach (var meter in query)
            {
                var activeModel = CreateHistoricViewModel(meter);
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
