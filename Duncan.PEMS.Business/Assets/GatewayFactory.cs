/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 12/20/2013       Sergey Ostrerov                 Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 * 02/06/2014       R Howard                        JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 02/19/2014       Sergey Ostrerov                       DPTXPEMS-213 - Time formatting:Alarm Duration.
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

namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// Class encapsulating functionality for the gateway asset type.
    /// </summary>
    public class GatewayFactory : AssetFactory
    {
        /// <summary>
        /// Gateway Factory constructor taking a connection string name.
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
        public GatewayFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }

        #region Index

        /// <summary>
        /// Get a queryable list of gateway summaries (<see cref="IQueryable{AssetListModel}"/>) for
        /// customer id of <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gridType">The type of grid (<see cref="AssetListGridType"/>) that will be using the data</param>
        /// <returns>An instance of <see cref="IQueryable{AssetListModel}"/></returns>
        public IQueryable<AssetListModel> GetSummaryModels(int customerId, AssetListGridType gridType)
        {
            var items = from g in PemsEntities.Gateways
                        where g.CustomerID == customerId
                        join meterMap in PemsEntities.MeterMaps on
                            new { SensorId = g.GateWayID, CustomerId = g.CustomerID } equals
                            new { SensorId = meterMap.GatewayID.Value, CustomerId = meterMap.Customerid } into l1
                        from mm in l1.DefaultIfEmpty()
                        let aa = mm.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()

                         join profMeters in PemsEntities.VersionProfileMeters on
                            new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } equals
                          new { MeterId = profMeters.MeterId.Value, AreaId = profMeters.AreaId.Value, CustomerId = profMeters.CustomerId } into l4
                        from vpn in l4.DefaultIfEmpty()

                        select
                            new AssetListModel
                            {
                    //----------DETAIL LINKS
                    Type = "Gateway", //name of the details page (ViewCashbox)
                    AreaId = (int)AssetAreaId.Gateway,
                    CustomerId = customerId,

                    //-----------FILTERABLE ITEMS
                    AssetType = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == g.GatewayType) != null ? GatewayViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == g.GatewayType).MeterGroupDesc ?? GatewayViewModel.DefaultType,
                    AssetId = g.GateWayID,
                    AssetName = g.Description ?? " ",
                    InventoryStatus = g.GatewayState == null ? "" : g.AssetState.AssetStateDesc,
                    OperationalStatus = g.OperationalStatu == null ? " " : g.OperationalStatu.OperationalStatusDesc,
                    Latitude = g.Latitude ?? 0.0,
                    Longitude = g.Longitude ?? 0.0,
                    AreaId2 = mm.AreaId2,
                    ZoneId = mm.ZoneId,
                    Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId) == null ? " " : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId).DisplayName,
                    Street = g.Location ?? " ",
                    DemandStatus = PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == g.DemandZone) == null ? "" : PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == g.DemandZone).DemandZoneDesc,

                    //------------COMMON COLUMNS
                    AssetModel = g.MechanismMaster == null ? " "
                     : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == g.MechanismMaster.MechanismId && m.IsDisplay) == null ? " "
                     : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == g.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc,

                    //------------SUMMARY GRID
                    SpacesCount = PemsEntities.SensorMappings.Count(x => x.CustomerID == customerId && x.GatewayID == g.GateWayID),
                    Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                    Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId) == null ? "" : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId).ZoneName,  

                    //---------------CONFIGURATION GRID
                    DateInstalled = g.InstallDateTime,
                    ConfigurationId =  vpn == null ? (long?)null : vpn.VersionProfileMeterId,
                    //ConfigCreationDate
                    //ConfigScheduleDate
                    //ConfigActivationDate
                    //MpvVersion
                    //SoftwareVersion
                    //FirmwareVersion

                    //-------------OCCUPANCY GRID - only valid for spaces
                    //MeterName
                    //SensorName
                    OperationalStatusDate = g.OperationalStatusTime,
                    //OccupancyStatus
                    //OccupancyStatusDate
                    //NonComplianceStatus
                    //NonComplianceStatusDate

                    //------------FUNCTIONAL STATUS GRID
                    AlarmClass = aa.EventCode1.AlarmTier1.TierDesc,
                    AlarmCode = aa == null ? "" : aa.EventCode1 == null ? "" : aa.EventCode1.EventDescVerbose,
                    AlarmTimeOfOccurance = aa == null ? (DateTime?)null : aa.TimeOfOccurrance,
                    AlarmRepairTargetTime = aa == null ? (DateTime?)null : aa.SLADue,
                    LocalTime = Now
                };

            return items;
        }
        #endregion

        #region Get View Models


        public List<SelectListItemWrapper> GatewayList(int customerId, int selectedId = -1)
        {
            List<SelectListItemWrapper> list = (from gateways in PemsEntities.Gateways
                                                where gateways.CustomerID == customerId && gateways.GatewayState == (int)AssetStateType.Current
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = gateways.GateWayID == selectedId,
                                                    TextInt = gateways.GateWayID,
                                                    ValueInt = gateways.GateWayID
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }



        /// <summary>
        /// Returns an instance of <see cref="GatewayViewModel"/> reflecting the present state
        /// and settings of a <see cref="Gateway"/> for customer id of <paramref name="customerId"/>
        /// and a gateway id of <paramref name="gatewayId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gatewayId">The gateway id</param>
        /// <returns>An instance of <see cref="GatewayViewModel"/></returns>
        public GatewayViewModel GetViewModel(int customerId, int gatewayId)
        {
            var model = new GatewayViewModel
                {
                    CustomerId = customerId,
                    AreaId = (int)AssetAreaId.Gateway, 
                    AssetId = gatewayId
                };

            var gateway = PemsEntities.Gateways.FirstOrDefault( m => m.CustomerID == customerId && m.GateWayID == gatewayId );


            model.GlobalId = gateway.GateWayID.ToString();

            model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == gateway.GatewayType) == null ? GatewayViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == gateway.GatewayType).MeterGroupDesc ?? GatewayViewModel.DefaultType;
            model.TypeId = gateway.GatewayType ?? (int)MeterGroups.Gateway;
            model.AssetModel = gateway.MechanismMaster == null ? " "
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == gateway.MechanismMaster.MechanismId && m.IsDisplay) == null ? " "
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == gateway.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc;
            model.Name = gateway.Description ?? " ";

            model.Street = gateway.Location ?? " ";

            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.GatewayID == gateway.GateWayID);
            if (meterMap != null)
            {
                var area = PemsEntities.Areas.FirstOrDefault( m => m.AreaID == meterMap.AreaId2 && m.CustomerID == customerId );
                model.Area = area == null ? "" : area.AreaName;
                model.AreaId2 = area == null ? -1 : area.AreaID;
                model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                model.ZoneId = meterMap.Zone == null ? -1 : meterMap.Zone.ZoneId;
                model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;
                model.MaintenanceRoute = meterMap.MaintRouteId == null ? "-" : meterMap.MaintRoute.DisplayName ?? "-";
            }
            else
            {
                model.Area = "";
                model.AreaId2 = -1;
                model.Zone = "";
                model.ZoneId = -1;
                model.Suburb = "";
                model.MaintenanceRoute = "-";

            }

            // Lat/Long
            model.Latitude = gateway.Latitude ?? 0.0;
            model.Longitude = gateway.Longitude ?? 0.0;

            model.LastPrevMaint = gateway.LastPreventativeMaintenance.HasValue ? gateway.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = gateway.NextPreventativeMaintenance.HasValue ? gateway.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            model.WarrantyExpiration = gateway.WarrantyExpiration.HasValue ? gateway.WarrantyExpiration.Value.ToShortDateString() : "";

            // Configuration information
            model.Configuration = new GatewayConfigurationModel();
            model.Configuration.DateInstalled = gateway.InstallDateTime;
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.GatewayID == gateway.GateWayID);
            if (versionProfile != null)
            {
                model.Configuration.MPVVersion = versionProfile.CommunicationVersion ?? "";
                model.Configuration.SoftwareVersion = versionProfile.SoftwareVersion ?? "";
                model.Configuration.FirmwareVersion = versionProfile.HardwareVersion ?? "";
                model.Configuration.ConfigurationId = versionProfile.VersionProfileMeterId;
                model.Configuration.ConfigurationName = versionProfile.ConfigurationName ?? "";
            }


            model.Configuration.HardwareVersion = gateway.HWVersion ?? "";
            model.Configuration.PowerSource = gateway.PowerSource == null ? "" : gateway.PowerSource.ToString();
            model.Configuration.GatewayModel = gateway.GatewayModel == null ? "" : gateway.GatewayModel.ToString();
            model.Configuration.Manufacturer = gateway.Manufacturer ?? "";


            // Active alarm
            model.AlarmConfiguration = new AssetAlarmConfigModel();
            if ( meterMap != null && meterMap.Meter != null && meterMap.Meter.ActiveAlarms != null && meterMap.Meter.ActiveAlarms.Any() )
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

            // Operational status of gateway.
            model.Status = gateway.OperationalStatu == null ? "" : gateway.OperationalStatu.OperationalStatusDesc;
            model.StatusDate = gateway.OperationalStatusTime;

            // See if gateway has pending changes.
            model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

            // Is asset active?
            model.State = (AssetStateType)(gateway.GatewayState ?? 0);

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns an instance of <see cref="GatewayViewModel"/> reflecting the present and pending state
        /// and settings of a <see cref="Gateway"/> for customer id of <paramref name="customerId"/>
        /// and a gateway id of <paramref name="gatewayId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gatewayId">The gateway id</param>
        /// <returns>An instance of <see cref="GatewayViewModel"/></returns>
        public GatewayViewModel GetPendingViewModel(int customerId, int gatewayId)
        {
            GatewayViewModel model = GetViewModel( customerId, gatewayId );

            // Get the pending changes from [AssetPending] 
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapGatewayId == model.AssetId);

            // Get out if there are no pending changes.
            if ( assetPending == null )
            {
                return model;
            }

            // Determine which pending changes are valid.
            if (assetPending.AssetModel.HasValue)
            {
                model.AssetModel = PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == assetPending.AssetModel.Value && m.IsDisplay) == null ? ""
                    : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == assetPending.AssetModel.Value && m.IsDisplay).MechanismDesc;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.AssetName))
            {
                model.Name = assetPending.AssetName;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.LocationGateway))
            {
                model.Street = assetPending.LocationGateway;
            }

            if (assetPending.MeterMapAreaId2.HasValue)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == customerId &&  m.AreaID == assetPending.MeterMapAreaId2.Value);
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

            // Lat/Long
            if (assetPending.Latitude.HasValue)
            {
                model.Latitude = assetPending.Latitude.Value;
            }

            if (assetPending.Longitude.HasValue)
            {
                model.Longitude = assetPending.Longitude.Value;
            }

            if (assetPending.MeterMapMaintenanceRoute.HasValue)
            {
                model.MaintenanceRoute =
                    PemsEntities.MaintRoutes.FirstOrDefault( m => m.CustomerId == model.CustomerId 
                        && m.MaintRouteId == assetPending.MeterMapMaintenanceRoute.Value ).DisplayName ??
                    "-";
            }

            if (assetPending.LastPreventativeMaintenance.HasValue)
            {
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.Value.ToShortDateString();
            }

            if (assetPending.NextPreventativeMaintenance.HasValue)
            {
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance.Value.ToShortDateString();
            }

            if (assetPending.WarrantyExpiration.HasValue)
            {
                model.WarrantyExpiration = assetPending.WarrantyExpiration.Value.ToShortDateString();
            }

            // Sensor configuration details
            if (assetPending.DateInstalled.HasValue)
            {
                model.Configuration.DateInstalled = assetPending.DateInstalled.Value;
            }

            // Version information
            if (!string.IsNullOrWhiteSpace(assetPending.MPVFirmware))
            {
                model.Configuration.MPVVersion = assetPending.MPVFirmware;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.AssetSoftwareVersion))
            {
                model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.AssetFirmwareVersion))
            {
                model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion;
            }

            // Operational status of meter.
            if (assetPending.OperationalStatus.HasValue)
            {
                model.Status = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == assetPending.OperationalStatus.Value).OperationalStatusDesc;
                model.StatusDate = assetPending.OperationalStatusTime;
            }

            // Is asset active?
            model.State = AssetStateType.Pending;

            // Get target ActivationDate
            model.ActivationDate = assetPending.RecordMigrationDateTime.HasValue ? assetPending.RecordMigrationDateTime.Value.ToShortDateString() : "";

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns a <see cref="GatewayViewModel"/> based upon the indicated audit record id <paramref name="auditId"/>.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="gatewayId">The asset id</param>
        /// <param name="auditId">The audit record id</param>
        /// <returns>An instance of <see cref="GatewayViewModel"/></returns>
        public GatewayViewModel GetHistoricViewModel(int customerId, int gatewayId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.GatewaysAudits.FirstOrDefault(m => m.GatewaysAuditId == auditId));
        }

        /// <summary>
        /// Private method to create a <see cref="GatewayViewModel"/> from a <see cref="GatewaysAudit"/> record.
        /// </summary>
        /// <param name="auditModel">An instance of <see cref="GatewaysAudit"/></param>
        /// <returns>An instance of <see cref="GatewayViewModel"/></returns>
        private GatewayViewModel CreateHistoricViewModel(GatewaysAudit auditModel)
        {
            var model = new GatewayViewModel()
            {
                CustomerId = auditModel.CustomerID,
                AreaId = (int)AssetAreaId.Gateway,
                AssetId = auditModel.GateWayID
            };

            model.GlobalId = auditModel.GateWayID.ToString();

            model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.GatewayType) == null ? GatewayViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == auditModel.GatewayType).MeterGroupDesc ?? GatewayViewModel.DefaultType;
            model.TypeId = auditModel.GatewayType ?? (int)MeterGroups.Gateway;
            model.AssetModel = !auditModel.GatewayModel.HasValue ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == auditModel.GatewayModel && m.IsDisplay) == null ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == auditModel.GatewayModel && m.IsDisplay).MechanismDesc; 
            model.Name = auditModel.Description ?? " ";

            model.Street = auditModel.Location ?? " ";

            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == auditModel.CustomerID && m.GatewayID == auditModel.GateWayID);
            if (meterMap != null)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.AreaID == meterMap.AreaId2 && m.CustomerID == auditModel.CustomerID);
                model.Area = area == null ? "" : area.AreaName;
                model.AreaId2 = area == null ? -1 : area.AreaID;
                model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                model.ZoneId = meterMap.Zone == null ? -1 : meterMap.Zone.ZoneId;
                model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;
                model.MaintenanceRoute = meterMap.MaintRouteId == null ? "-" : meterMap.MaintRoute.DisplayName ?? "-";
            }
            else
            {
                model.Area = "";
                model.AreaId2 = -1;
                model.Zone = "";
                model.ZoneId = -1;
                model.Suburb = "";
                model.MaintenanceRoute = "-";

            }

            // Lat/Long
            model.Latitude = auditModel.Latitude ?? 0.0;
            model.Longitude = auditModel.Longitude ?? 0.0;

            model.LastPrevMaint = auditModel.LastPreventativeMaintenance.HasValue ? auditModel.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = auditModel.NextPreventativeMaintenance.HasValue ? auditModel.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            model.WarrantyExpiration = auditModel.WarrantyExpiration.HasValue ? auditModel.WarrantyExpiration.Value.ToShortDateString() : "";

            // Configuration information
            // See if there is a VersionProfileMeterAudit withing an hour of this audit record.
            DateTime fromDate = auditModel.UpdateDateTime.AddHours(-1);
            DateTime toDate = auditModel.UpdateDateTime.AddHours(1);
            var versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.GatewayID == auditModel.GateWayID
                && m.UpdateDateTime > fromDate && m.UpdateDateTime < toDate);
            // If no audit record then get audit record most recent in the past.
            if (versionProfileMeterAudit == null)
            {
                versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.GatewayID == auditModel.GateWayID
                      && m.UpdateDateTime < auditModel.UpdateDateTime);
            }

            // Is there still is not a versionProfileMeterAudit then get present versionProfileMeter, otherwise use versionProfileMeterAudit
            VersionProfileMeter versionProfileMeter = null;
            if (versionProfileMeterAudit == null)
            {
                versionProfileMeter = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == auditModel.CustomerID && m.GatewayID == auditModel.GateWayID);
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


            // Miscellaneous properties
            model.Configuration.HardwareVersion = auditModel.HWVersion ?? "";
            model.Configuration.PowerSource = auditModel.PowerSource == null ? "" : auditModel.PowerSource.ToString();
            model.Configuration.GatewayModel = auditModel.GatewayModel == null ? "" : auditModel.GatewayModel.ToString();
            model.Configuration.Manufacturer = auditModel.Manufacturer ?? "";


            // Active alarm
            model.AlarmConfiguration = new AssetAlarmConfigModel();

            // Operational status of gateway.
            model.Status = auditModel.OperationalStatus.HasValue
                               ? PemsEntities.OperationalStatus.FirstOrDefault( m => m.OperationalStatusId == auditModel.OperationalStatus.Value ).OperationalStatusDesc
                               : "";
            model.StatusDate = auditModel.OperationalStatusTime;

            // Is asset active?
            model.State = (AssetStateType)(auditModel.GatewayState ?? 0);

            // Updated by
            model.LastUpdatedById = auditModel.UserId;
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReason = (AssetPendingReasonType)(auditModel.AssetPendingReasonId ?? 0);
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;

            // Record date
            model.RecordDate = auditModel.UpdateDateTime;

            // Audit record id
            model.AuditId = auditModel.GatewaysAuditId;

            return model;
        }

        /// <summary>
        /// Gets an <see cref="AssetHistoryModel"/> for a given <paramref name="customerId"/> and <paramref name="gatewayId"/>.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gatewayId">The gateway id</param>
        /// <returns>An instance of an <see cref="AssetHistoryModel"/></returns>
        public AssetHistoryModel GetHistoricListViewModel(int customerId, int gatewayId)
        {
            var gateway = PemsEntities.Gateways.FirstOrDefault(m => m.CustomerID == customerId && m.GateWayID == gatewayId);

            var model = new AssetHistoryModel()
            {
                CustomerId = customerId,
                AssetId = gatewayId,
                Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == gateway.GatewayType) == null ? GatewayViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == gateway.GatewayType).MeterGroupDesc ?? GatewayViewModel.DefaultType,
                TypeId = gateway.GatewayType ?? (int)MeterGroups.Gateway,
                Name = gateway.Description,
                Street = gateway.Location ?? ""
            };

            return model;
        }

        #endregion

        #region Get Edit Models

        /// <summary>
        /// Returns an instance of <see cref="GatewayEditModel"/> reflecting the present and any pending state
        /// and settings of a <see cref="Gateway"/> for customer id of <paramref name="customerId"/>
        /// and a gateway id of <paramref name="gatewayId"/>. This model is used for edit page.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gatewayId">The gateway id</param>
        /// <returns>An instance of <see cref="GatewayEditModel"/></returns>
        public GatewayEditModel GetEditModel(int customerId, int gatewayId)
        {
            // Get the gateway from Gateways
            var gateway = PemsEntities.Gateways.FirstOrDefault(m => m.CustomerID == customerId && m.GateWayID == gatewayId);

            var model = new GatewayEditModel
            {
                CustomerId = customerId,
                AreaId = (int)AssetAreaId.Gateway,
                AssetId = gatewayId
            };

            // First build the edit model from the existing gateway.
            #region Build model from a gateway

            model.GlobalId = gateway.GateWayID.ToString();

            // Gateway Type
            model.Type = gateway.GatewayType == null ? GatewayViewModel.DefaultType : 
                (PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == gateway.GatewayType).MeterGroupDesc ?? GatewayViewModel.DefaultType);
            model.TypeId = (MeterGroups)(gateway.GatewayType ?? (int)MeterGroups.Gateway);

            // Get meter models.
            model.AssetModelId = gateway.GatewayModel ?? -1;

            // Get meter name.
            model.Name = gateway.Description ?? " ";

            // Get street name
            model.Street = gateway.Location ?? " ";

            // Get Zone and Suburb
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.GatewayID == gateway.GateWayID);
            if (meterMap != null)
            {
                model.AreaListId = meterMap.AreaId2 ?? 0;
                model.ZoneId = meterMap.ZoneId ?? 0;
                model.SuburbId = meterMap.CustomGroup1 ?? 0;

                // Maintenace Route
                model.MaintenanceRouteId = meterMap.MaintRouteId ?? 0;
            }
            else
            {
                model.AreaListId = 0;
                model.ZoneId = 0;
                model.SuburbId = 0;

                // Maintenace Route
                model.MaintenanceRouteId = 0;
            }

            // Lat/Long
            model.Latitude = gateway.Latitude ?? 0.0;
            model.Longitude = gateway.Longitude ?? 0.0;

            // Preventative maintenance dates
            model.LastPrevMaint = gateway.LastPreventativeMaintenance.HasValue ? gateway.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = gateway.NextPreventativeMaintenance.HasValue ? gateway.NextPreventativeMaintenance.Value : (DateTime?)null;

            // Warantry expiration
            model.WarrantyExpiration = gateway.WarrantyExpiration.HasValue ? gateway.WarrantyExpiration.Value : (DateTime?)null;

            // Version profile information
            model.Configuration = new AssetConfigurationModel();
            model.Configuration.DateInstalled = gateway.InstallDateTime;
            var versionProfile = PemsEntities.VersionProfileMeters.FirstOrDefault(m => m.CustomerId == customerId && m.GatewayID == gatewayId);
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

            // Operational status of gateway.
            GetOperationalStatusDetails(model, gateway.OperationalStatus ?? 0, gateway.OperationalStatusTime.HasValue ? gateway.OperationalStatusTime.Value : DateTime.Now);
//            GetOperationalStatusList(model, gateway.OperationalStatus ?? 0);

            // Is asset active?
            model.StateId = gateway.GatewayState ?? (int)AssetStateType.Pending;

            #endregion

            // If there is an asset pending record then update edit model with associated values.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapGatewayId == model.AssetId);

            if ( assetPending != null )
            {
                #region Update model from an assetPending

                // Get gateway models.
                model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;

                // Get gateway name.
                model.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? model.Name : assetPending.AssetName;

                // Get street name
                model.Street = string.IsNullOrWhiteSpace(assetPending.LocationGateway) ? model.Street : assetPending.LocationGateway;

                // Get Zone, Area and Suburb
                model.AreaListId = assetPending.MeterMapAreaId2 ?? model.AreaListId;
                model.ZoneId = assetPending.MeterMapZoneId ?? model.ZoneId;
                model.SuburbId = assetPending.MeterMapSuburbId ?? model.SuburbId;

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

                // Operational status of gateway.
                if ( assetPending.OperationalStatus != null )
                {
                    GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);
                }
//                GetOperationalStatusList(model, (int)(assetPending.OperationalStatus ?? gateway.OperationalStatus));

                // Asset state is pending
                model.StateId = (int)AssetStateType.Pending;

                // Activation Date
                model.ActivationDate = assetPending.RecordMigrationDateTime;

                #endregion
            }

            GetAssetModelList(model, (int)MeterGroups.Gateway);

            // Get zone list
            GetZoneList(model);

            // Get suburb list
            GetSuburbList(model);

            // Get Areas list
            GetAreaList(model);

            // Maintenace Route
            GetMaintenanceRouteList(model);

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
        /// Refreshes any lists in the <see cref="GatewayEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="GatewayEditModel"/> to refresh</param>
        public void RefreshEditModel(GatewayEditModel editModel)
        {
            // Get gateway models.
            GetAssetModelList(editModel, (int)MeterGroups.Gateway);

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

            // Get state list
            GetAssetStateList(editModel);
        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="GatewayEditModel"/> to the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of a <see cref="GatewayEditModel"/> with updates</param>
        public void SetEditModel(GatewayEditModel model)
        {
            // Note:  At this time all changes to the gateway are written to the [AssetPending] table.

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
            (new PendingFactory(ConnectionStringName, Now)).Save(model, otherModel);

        }


        /// <summary>
        /// Takes a list of <see cref="AssetIdentifier"/> pointing to a list of gateways and creates the model
        /// used to mass edit the list of gateways.  The model contains only fields that can be changed for all
        /// items in the list.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gatewayIds">A <see cref="List{AssetIdentifier}"/> indicating gateways to be edited</param>
        /// <returns>An instance of <see cref="GatewayMassEditModel"/></returns>
        public GatewayMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> gatewayIds)
        {
            var editModel = new GatewayMassEditModel
            {
                CustomerId = customerId,
                Zone = ZoneList(customerId, -1),
                ZoneId = -1,
                SuburbId = -1,
                Suburb = SuburbList(customerId, -1),
                AreaListId = -1,
                Area = AreaList(customerId, -1),
                LastPrevMaint = null,
                NextPrevMaint = null,
                MaintenanceRouteId = -1,
                MaintenanceRoute = MaintenanceRouteList(customerId, -1),
                WarrantyExpiration = null
            };

            SetPropertiesOfAssetMassiveEditModel(editModel, gatewayIds);

            return editModel;
        }

        /// <summary>
        /// Refreshes any lists in the <see cref="GatewayMassEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="GatewayMassEditModel"/> to refresh</param>
        public void RefreshMassEditModel(GatewayMassEditModel editModel)
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
        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="GatewayMassEditModel"/> to the [AssetPending] table for each
        /// gateway in the <see cref="GatewayMassEditModel.AssetIds"/> list.  Only values that have been changed are 
        /// stored.
        /// </summary>
        /// <param name="model">Instance of a <see cref="GatewayMassEditModel"/> with updates</param>
        public void SetMassEditModel(GatewayMassEditModel model)
        {
            var pendingFactory = new PendingFactory(this.ConnectionStringName, Now);
            foreach (var asset in model.AssetIds())
            {
                // Save changes to the [AssetPending] table.
                pendingFactory.Save(model, model.CustomerId, asset.AreaId, (int)asset.AssetId);
            }
        }


        #endregion

        #region Create/Clone


        /// <summary>
        /// Creates a clone (copy) of the <see cref="Gateway"/> refrenced by <paramref name="assetId"/> and <paramref name="customerId"/>.  This cloned
        /// <see cref="Gateway"/> is written to the [Gateways] table and the [AssetPending] table.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id (presently not used)</param>
        /// <param name="assetId">The gateway id to clone</param>
        /// <returns>Integer representing the newly cloned gateway id (<see cref="Gateway.GateWayID"/>)</returns>
        private int Clone(int customerId, int areaId, Int64 assetId)
        {
            // Get original gateway and its associated Meter and MeterMap
            // Since this gateway exists, these entities should exist also.
            Gateway gateway = PemsEntities.Gateways.FirstOrDefault(m => m.GateWayID == assetId && m.CustomerID == customerId);
            MeterMap meterMap = gateway.MeterMaps.FirstOrDefault(m => m.GatewayID == assetId && m.Customerid == customerId);
            Meter meter = meterMap.Meter;

            // Create the new meter that is associated with the gateway type of asset.  This is not a meter
            // but rather a an asset in the meter table that represents a gateway.
            Meter newMeter = CreateBaseAsset(meter.CustomerID, MeterGroups.Gateway, meter.AreaID);

            // Create a gateway
            Gateway newGateway = new Gateway()
            {
                GateWayID = NextGatewayId(customerId),
                CustomerID = customerId,
                GatewayState = (int)AssetStateType.Pending,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                GatewayType = newMeter.MeterGroup
            };
            newGateway.GatewayModel = gateway.GatewayModel;
            newGateway.WarrantyExpiration = gateway.WarrantyExpiration;
            newGateway.CAMID = gateway.CAMID;
            newGateway.CELID = gateway.CELID;
            newGateway.HWVersion = gateway.HWVersion;
            newGateway.Latitude = gateway.Latitude;
            newGateway.Longitude = gateway.Longitude;
            newGateway.Location = gateway.Location;
            newGateway.Manufacturer = gateway.Manufacturer;
            newGateway.PowerSource = gateway.PowerSource;
            newGateway.TimeZoneID = gateway.TimeZoneID;

            PemsEntities.Gateways.Add(newGateway);
            PemsEntities.SaveChanges();

            // Add AssetPending record
            (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Gateway,
                                                                       SetToMidnight(Now),
                                                                       AssetStateType.Current,
                                                                       customerId,
                                                                       newGateway.GateWayID,
                                                                       (int)AssetStateType.Pending, (int)OperationalStatusType.Inactive,
                                                                       (int)AssetAreaId.Gateway);


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
            newMeterMap.GatewayID = newGateway.GateWayID;


            PemsEntities.SaveChanges();

            // Set audit records.
            Audit(newMeter);
            Audit(newMeterMap);
            Audit(newGateway);

            return newGateway.GateWayID;
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Gateway"/> refrenced by <paramref name="model"/>.  
        /// This cloned <see cref="Gateway"/> is written to the [Gateways] table and the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of <see cref="GatewayViewModel"/> to clone</param>
        /// <returns>Integer representing the newly cloned gateway id (<see cref="Gateway.GateWayID"/>)</returns>
        public int Clone(GatewayViewModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="Gateway"/> refrenced by <paramref name="model"/>.  
        /// This cloned <see cref="Gateway"/> is written to the [Gateways] table and the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of <see cref="GatewayEditModel"/> to clone</param>
        /// <returns>Integer representing the newly cloned gateway id (<see cref="Gateway.GateWayID"/>)</returns>
        public int Clone(GatewayEditModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }


        /// <summary>
        /// Gets the next Gateway Id from the table [Gateways].  This will be used
        /// in the creation of a new Gateway.  It is entirely possible that this is the first gateway
        /// so check for this case.
        /// </summary>
        /// <param name="customerId">Integer id of customer</param>
        /// <returns>Next available Gateway id.</returns>
        private int NextGatewayId(int customerId)
        {
            int nextId = 1;

            Gateway gateway = PemsEntities.Gateways.FirstOrDefault(m => m.CustomerID == customerId);
            if (gateway != null)
            {
                nextId = PemsEntities.Gateways.Where(m => m.CustomerID == customerId).Max(m => m.GateWayID) + 1;
            }
            return nextId;
        }



        /// <summary>
        /// Create a new gateway and set it to <see cref="OperationalStatusType.Inactive"/> operational status (<see cref="Gateway.OperationalStatus"/>) 
        /// and a <see cref="AssetStateType.Pending"/> state (<see cref="Gateway.GatewayState"/>). Associate it with this <paramref name="customerId"/>.
        /// 
        /// Create an associated <see cref="MeterMap"/> and relate it to new <see cref="Gateway"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Instance of <see cref="Gateway"/> of new gateway.</returns>
        public Gateway Create(int customerId)
        {
            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault( m => m.CustomerId == customerId );

            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(customerId, MeterGroups.Gateway);
            newMeter.TimeZoneID = customerProfile.TimeZoneID ?? 0;

            // Create an audit entry.
            Audit( newMeter );

            // Create a gateway
            Gateway gateway = new Gateway()
                {
                    GateWayID = NextGatewayId( customerId ),
                    CustomerID = customerId,
                    GatewayState = (int) AssetStateType.Pending,
                    OperationalStatus = (int) OperationalStatusType.Inactive,
                    OperationalStatusTime = DateTime.Now,
                    GatewayType = newMeter.MeterGroup,
                    TimeZoneID = customerProfile.TimeZoneID
                };
            PemsEntities.Gateways.Add( gateway );
            PemsEntities.SaveChanges();

            // Add audit record
            Audit( gateway );

            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            // Create a MeterMap record to join Gateway and Meter
            MeterMap meterMap = new MeterMap()
                {
                    Customerid = newMeter.CustomerID,
                    Areaid = newMeter.AreaID,
                    MeterId = newMeter.MeterId,
                    GatewayID = gateway.GateWayID,
                    HousingId = housingMaster.HousingId
                };

            PemsEntities.MeterMaps.Add( meterMap );
            PemsEntities.SaveChanges();

            return gateway;
        }

        public Gateway Create(int customerId, AssetTypeModel.EnterMode enterMode, int gateWayID)
        {
            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);

            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(enterMode, gateWayID, customerId, MeterGroups.Gateway);
            newMeter.TimeZoneID = customerProfile.TimeZoneID ?? 0;

            // Create an audit entry.
            Audit(newMeter);

            // Create a gateway
            Gateway gateway = new Gateway()
            {
                //GateWayID = NextGatewayId(customerId),
                GateWayID = gateWayID,
                CustomerID = customerId,
                GatewayState = (int)AssetStateType.Pending,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                GatewayType = newMeter.MeterGroup,
                TimeZoneID = customerProfile.TimeZoneID
            };
            PemsEntities.Gateways.Add(gateway);
            PemsEntities.SaveChanges();

            // Add audit record
            Audit(gateway);

            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            // Create a MeterMap record to join Gateway and Meter
            MeterMap meterMap = new MeterMap()
            {
                Customerid = newMeter.CustomerID,
                Areaid = newMeter.AreaID,
                MeterId = newMeter.MeterId,
                GatewayID = gateway.GateWayID,
                HousingId = housingMaster.HousingId
            };

            PemsEntities.MeterMaps.Add(meterMap);
            PemsEntities.SaveChanges();

            return gateway;
        }


        #endregion

        #region Utilities

        /// <summary>
        /// Resets a gateway by writing a reset record into the [MeterResetSchedule] table.  The gateway
        /// is referenced by the <paramref name="assetIdentifier"/> and <paramref name="userId"/> indicates
        /// who requested the reset.
        /// </summary>
        /// <param name="assetIdentifier">Instance of <see cref="AssetIdentifier"/> indicating gateway to reset</param>
        /// <param name="userId">User id identifying requesting user</param>
        /// <returns>True indicating <see cref="MeterResetSchedule"/> was created and written to [MeterResetSchedule] table</returns>
        public bool Reset(AssetIdentifier assetIdentifier, int userId)
        {
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.GatewayID == assetIdentifier.AssetId && m.Customerid == assetIdentifier.CustomerId);
            if (meterMap != null)
            {
                // According to Duncan, the MeterMap maps to the gateway.  Use customer id, meter id and area id to reset gateway.
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

        #region History

        /// <summary>
        /// Gets a <see cref="List{GatewayViewModel}"/> representing the known history of a
        /// gateway referenced by gateway id of <paramref name="assetId"/> and customer id of <paramref name="customerId"/>.
        /// Return only a list that are between <paramref name="startDateTicks"/> and <paramref name="endDateTicks"/>
        /// </summary>
        /// <param name="request">Instance of <see cref="DataSourceRequest"/> from Kendo UI</param>
        /// <param name="total">Out parameter used to return total number of records being returned</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="assetId">The gateway id</param>
        /// <param name="startDateTicks">From date in ticks</param>
        /// <param name="endDateTicks">To date in ticks</param>
        /// <returns><see cref="List{GatewayViewModel}"/> instance</returns>
        public List<GatewayViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {
            DateTime startDate;
            DateTime endDate;

            IQueryable<GatewaysAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.GatewaysAudits.Where(m => m.CustomerID == customerId && m.GateWayID == assetId
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.GatewaysAudits.Where(m => m.CustomerID == customerId && m.GateWayID == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.GatewaysAudits.Where(m => m.CustomerID == customerId && m.GateWayID == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.GatewaysAudits.Where(m => m.CustomerID == customerId && m.GateWayID == assetId).OrderBy(m => m.UpdateDateTime);
            }

            // Get the GatewayViewModel for each audit record.
            var list = new List<GatewayViewModel>();

            // Get present data
            var presentModel = GetViewModel(customerId, (int)assetId);
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data
            GatewayViewModel previousModel = null;
            foreach (var gateway in query)
            {
                var activeModel = CreateHistoricViewModel(gateway);
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
