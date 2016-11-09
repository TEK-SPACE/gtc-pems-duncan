using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.UI;

namespace Duncan.PEMS.Business.Assets
{
    public class MechanismFactory : AssetFactory
    {
        public MechanismFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }
        
        private string GetOperationalStatus(int CustId,int mechanismId, out DateTime? OperationalStatusDateTime, out int? OperationalStatusId)
        {
            OperationalStatusId =null;
            OperationalStatusDateTime = null;
            string status = "Inactive";
            var mech = PemsEntities.MechMasters.FirstOrDefault(m => m.MechIdNumber == mechanismId && m.Customerid == CustId);
            if (mech == null)
                return status;
          
            //if its plugged into a meter, its operational, otherwise it is inactive.
            //this means that if there is a meter map that isnt a mechanism, something else is pointing to it (a SSM or MSM)
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(x => x.Meter.MeterGroup != (int)MeterGroups.Mechanism && x.MeterId == mech.MechIdNumber && x.Customerid == CustId);
            if (meterMap != null)
                if (meterMap.Meter != null)
                {
                    status = meterMap.Meter.OperationalStatu != null ? meterMap.Meter.OperationalStatu.OperationalStatusDesc : "Inactive";
                    OperationalStatusId = meterMap.Meter.OperationalStatu != null ? meterMap.Meter.OperationalStatu.OperationalStatusId : (int?)null;
                    OperationalStatusDateTime = meterMap.Meter.OperationalStatusTime ;
                }
            return status;
        }

        #region Index

        /// <summary>
        /// Get a queryable list of cashbox summaries (<see cref="IQueryable{AssetListModel}"/>) for
        /// customer id of <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gridType">The type of grid (<see cref="AssetListGridType"/>) that will be using the data</param>
        /// <returns>An instance of <see cref="IQueryable{AssetListModel}"/></returns>
        public IQueryable<AssetListModel> GetSummaryModels(int customerId, AssetListGridType gridType)
        {
            #region Commented 22 Sep 2014

            //var items = from mechanism in PemsEntities.MechMasters
            //            where mechanism.Customerid == customerId
            //            join meterMap in PemsEntities.MeterMaps on
            //                new { MechId = mechanism.MechId, CustomerId = mechanism.Customerid.Value } equals
            //                new { MechId = meterMap.MechId.Value, CustomerId = meterMap.Customerid } into l1
            //            from mm in l1.DefaultIfEmpty()
            //            let aa = mm.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()
            //            join profMeters in PemsEntities.VersionProfileMeters on
            //              new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } equals
            //            new { MeterId = profMeters.MeterId.Value, AreaId = profMeters.AreaId.Value, CustomerId = profMeters.CustomerId } into l4
            //            from vpn in l4.DefaultIfEmpty()
            //            select
            //                new AssetListModel
            //                {
            //                    //----------DETAIL LINKS
            //                    Type = "Mechanism", //name of the details page (ViewCashbox)
            //                    AreaId = (int)AssetAreaId.Mechanism,
            //                    CustomerId = customerId,

            //                    //-----------FILTERABLE ITEMS
            //                    AssetType = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Mechanism).MeterGroupDesc,
            //                    AssetId = mechanism.MechIdNumber ?? mm.MechId.Value, //this used to ne mechid, but we cant remove the auto increment id, so we are using the mechidnumber instead
            //                    AssetName = " todo db ",
            //                    //have to make sure we get the op status of the meter that this mechanism is installed in, and NOT the "Meter" table entry for this mechanism
            //                    OperationalStatus =mechanism.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int) MeterGroups.Mechanism) == null ? " "
            //                     : mechanism.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int) MeterGroups.Mechanism).Meter.OperationalStatu == null ? " "
            //                     : mechanism.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int) MeterGroups.Mechanism).Meter.OperationalStatu.OperationalStatusDesc,
            //                    Latitude = mm.Meter.Latitude ,
            //                    Longitude = mm.Meter.Longitude ,
            //                    AreaId2 = mm.AreaId2,
            //                    ZoneId = mm.ZoneId,
            //                    Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId) == null ? " " 
            //                        : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId).DisplayName,
            //                    Street = mm.Meter.Location ?? " ",
            //                    DemandStatus = PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == mm.Meter.DemandZone) == null ? "" : PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == mm.Meter.DemandZone).DemandZoneDesc,

            //                    //------------COMMON COLUMNS
            //                    AssetModel = mechanism.MechanismMaster == null ? "" : mechanism.MechanismMaster.MechanismDesc,

            //                    //------------SUMMARY GRID
            //                    //SpacesCount = PemsEntities.SensorMappings.Count(x => x.CustomerID == customerId && x.GatewayID == g.GateWayID),
            //                    Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" 
            //                        : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
            //                    Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId) == null ? "" 
            //                        : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId).ZoneName,

            //                    //---------------CONFIGURATION GRID
            //                    //DateInstalled 
            //                   // ConfigurationId 
            //                    //ConfigCreationDate
            //                    //ConfigScheduleDate
            //                    //ConfigActivationDate
            //                    //MpvVersion
            //                    //SoftwareVersion
            //                    //FirmwareVersion

            //                    //-------------OCCUPANCY GRID - only valid for spaces
            //                    //MeterName
            //                    //SensorName
            //                   // OperationalStatusDate 
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

            var items = from mechanism in PemsEntities.MechMasters
                        where mechanism.Customerid == customerId
                        //join meterMap in PemsEntities.MeterMaps on
                        //    new { MechId = mechanism.MechId, CustomerId = mechanism.Customerid.Value } equals
                        //    new { MechId = meterMap.MechId.Value, CustomerId = meterMap.Customerid } into l1
                        join meterMap in PemsEntities.MeterMaps on
                            new { MechId = mechanism.MechIdNumber.Value, CustomerId = mechanism.Customerid.Value } equals
                            new { MechId = meterMap.MeterId, CustomerId = meterMap.Customerid } into l1
                        from mm in l1.DefaultIfEmpty()
                        where mm.Meter.MeterGroup == (int)MeterGroups.Mechanism   // To get only one Mechanism in Asset Inquiry
                        let aa = mm.Meter.ActiveAlarms.OrderByDescending(x => x.TimeOfOccurrance).FirstOrDefault()
                        join profMeters in PemsEntities.VersionProfileMeters on
                          new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } equals
                        new { MeterId = profMeters.MeterId.Value, AreaId = profMeters.AreaId.Value, CustomerId = profMeters.CustomerId } into l4
                        from vpn in l4.DefaultIfEmpty()
                        select
                            new AssetListModel
                            {
                                //----------DETAIL LINKS
                                Type = "Mechanism", //name of the details page (ViewCashbox)
                                AreaId = (int)AssetAreaId.Mechanism,
                                CustomerId = customerId,

                                //-----------FILTERABLE ITEMS
                                AssetType = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Mechanism).MeterGroupDesc,
                                AssetId = mechanism.MechIdNumber ?? mm.MechId.Value, //this used to ne mechid, but we cant remove the auto increment id, so we are using the mechidnumber instead

                                //** Sairam added on sep 21st
                                //AssetName = PemsEntities.Meters.FirstOrDefault(x => x.MeterId == (int)(mechanism.MechIdNumber ?? mm.MechId.Value)).MeterName,

                                //** Sairam added on sep 21st Santhosh Commented Sairam code for showing only one Mechanism in Asset Inquiry Page
                                AssetName = mm.Meter.MeterName, //PemsEntities.Meters.FirstOrDefault(x => x.MeterId == (int)(mechanism.MechIdNumber ?? mm.MechId.Value) && x.MeterGroup == (int)MeterGroups.Mechanism).MeterName,

                                //AssetName = mechanism.MeterName,  //** sairam done this on sep 21st for grid asset name display (asset inquiry page)
                                //have to make sure we get the op status of the meter that this mechanism is installed in, and NOT the "Meter" table entry for this mechanism
                                //OperationalStatus = mechanism.MeterMaps.FirstOrDefault(x=> x.Customerid ==customerId &&  x.MeterId == mm.Meter.MeterId && x.Meter.MeterGroup != (int)MeterGroups.Mechanism) == null ? " "
                                // : mechanism.MeterMaps.FirstOrDefault(x => x.Customerid == customerId && x.MeterId == mm.Meter.MeterId && x.Meter.MeterGroup != (int)MeterGroups.Mechanism).Meter.OperationalStatu == null ? " "
                                // : mechanism.MeterMaps.FirstOrDefault(x => x.Customerid == customerId & x.MeterId == mm.Meter.MeterId && x.Meter.MeterGroup != (int)MeterGroups.Mechanism).Meter.OperationalStatu.OperationalStatusDesc,
                                InventoryStatus = mm.Meter.MeterState == null ? "" : mm.Meter.AssetState.AssetStateDesc,
                                
                                OperationalStatus=mm.Meter.OperationalStatu.OperationalStatusDesc,
                                Latitude = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? 0.0
                                   : PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism).Meter.Latitude,

                                Longitude = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? 0.0
                               : PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism).Meter.Longitude,
                             //   Latitude = mm.Meter.Latitude,
                                //Longitude = mm.Meter.Longitude,
                                AreaId2 = mm.AreaId2,
                                ZoneId = mm.ZoneId,
                               
                                //Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId) == null ? " "
                                //    : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId).DisplayName,
                                
                                    Suburb = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? ""
                               : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomerId == customerId && x.CustomGroupId == (PemsEntities.MeterMaps.FirstOrDefault(y => y.MechId == mechanism.MechId && y.Customerid == customerId && y.Areaid != (int)AssetAreaId.Mechanism).CustomGroup1)).DisplayName,

                                   // Street = mm.Meter.Location ?? " ",

                                Street = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? ""
                        : PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism).Meter.Location,

                              //  DemandStatus = PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == mm.Meter.DemandZone) == null ? "" : PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == mm.Meter.DemandZone).DemandZoneDesc,

                                DemandStatus = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? ""
                                                           : PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == (PemsEntities.MeterMaps.FirstOrDefault(y => y.MechId == mechanism.MechId && y.Customerid == customerId && y.Areaid != (int)AssetAreaId.Mechanism).Meter.DemandZone)).DemandZoneDesc,

                                
                                //------------COMMON COLUMNS
                                //AssetModel = mechanism.MechanismMaster == null ? "" : mechanism.MechanismMaster.MechanismDesc,

                                //** Sairam added on 22nd sep *****
                                AssetModel = PemsEntities.MechanismMasterCustomers.FirstOrDefault(x => x.CustomerId == mm.Customerid && x.MechanismId == (int)MeterGroups.Mechanism).MechanismDesc,

                                //------------SUMMARY GRID
                                //SpacesCount = PemsEntities.SensorMappings.Count(x => x.CustomerID == customerId && x.GatewayID == g.GateWayID),
                                //Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? ""
                                //    : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,


                                Area = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? ""
                               : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID ==(PemsEntities.MeterMaps.FirstOrDefault(y => y.MechId == mechanism.MechId && y.Customerid == customerId && y.Areaid != (int)AssetAreaId.Mechanism).AreaId2)).AreaName,


                                //Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId) == null ? ""
                                //    : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId).ZoneName,

                                Zone = PemsEntities.MeterMaps.FirstOrDefault(x => x.MechId == mechanism.MechId && x.Customerid == customerId && x.Areaid != (int)AssetAreaId.Mechanism) == null ? ""
                                                             : PemsEntities.Zones.FirstOrDefault(x => x.customerID == customerId && x.ZoneId == (PemsEntities.MeterMaps.FirstOrDefault(y => y.MechId == mechanism.MechId && y.Customerid == customerId && y.Areaid != (int)AssetAreaId.Mechanism).ZoneId)).ZoneName,


                                //---------------CONFIGURATION GRID
                                //DateInstalled 
                                // ConfigurationId 
                                //ConfigCreationDate
                                //ConfigScheduleDate
                                //ConfigActivationDate
                                //MpvVersion
                                //SoftwareVersion
                                //FirmwareVersion

                                //-------------OCCUPANCY GRID - only valid for spaces
                                //MeterName
                                //SensorName
                                // OperationalStatusDate 
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

        /// <summary>
        /// Gets the view model
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="mechanismId"></param>
        /// <returns></returns>
        public MechanismViewModel GetViewModel(int customerId, int mechanismId)
        {
            #region Comment
            //var model = new MechanismViewModel
            //{
            //    CustomerId = customerId,
            //    AreaId = (int)AssetAreaId.Gateway,
            //    AssetId = mechanismId
            //};

            //var mechanism = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechIdNumber == mechanismId);

            //if (mechanism != null)
            //{
            //    //get the meter map, should have this - test for the meter with the correct group
            //    var meterMap = mechanism.MeterMaps.FirstOrDefault(x=>x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Mechanism && x.MechId == mechanism.MechId);
            //    if (meterMap != null)
            //    {
            //     //now go get the meter
            //        var meter = meterMap.Meter;
            //        model.GlobalId = meter.GlobalMeterId.ToString();
                    
            //        model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Mechanism).MeterGroupDesc;
            //        model.TypeId =(int)MeterGroups.Mechanism;
            //        model.AssetModel = GetMechanismDescription(customerId, mechanism.MechType ?? -1);
            //        model.AssetModelId = mechanism.MechType ?? -1;
            //        model.Name ="todo";//todo GTC:Mechanism - set asset name here for mechmaster when the db script is added

            //        var area = PemsEntities.Areas.FirstOrDefault(m => m.AreaID == meterMap.AreaId2 && m.CustomerID == customerId);
            //        model.Area = area == null ? "" : area.AreaName;
            //        model.AreaId2 = area == null ? -1 : area.AreaID;
            //        model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
            //        model.ZoneId = meterMap.Zone == null ? -1 : meterMap.Zone.ZoneId;
            //        model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;
            //        model.MaintenanceRoute = meterMap.MaintRouteId == null ? "-" : meterMap.MaintRoute.DisplayName ?? "-";

            //        // Lat/Long
            //        model.Latitude = meter.Latitude ?? 0.0;
            //        model.Longitude = meter.Longitude ?? 0.0;

            //        model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            //        model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value.ToShortDateString() : "";
            //        model.InstallationDate = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?) null;
            //        model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;


            //        // Operational status of meter (mechanism).
            //        DateTime? opStatusDatetime;
            //        int? statusId;
            //        model.Status = GetOperationalStatus(mechanismId, out opStatusDatetime, out statusId);
            //        model.StatusDate = opStatusDatetime;
            //        model.StatusId = statusId ?? 0;
            //        // See if item has pending changes.
            //        model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);
                    
            //        // Is asset active?
            //        model.State = (AssetStateType)(meter.MeterState ?? 0);

            //        // Get last update by information
            //        CreateAndLastUpdate(model);

            //        //custom mechanism properties
                   
            //        model.SerialNumber = mechanism.MechSerial;
            //        model.Street = model.Location = GetMechanismLocation(customerId, mechanism.MechId);
            //    }

            //}

            //return model;
            #endregion

            var model = new MechanismViewModel
            {
                CustomerId = customerId,
                AreaId = (int)AssetAreaId.Gateway,
                AssetId = mechanismId
            };

            var mechanism = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechIdNumber == mechanismId);



            if (mechanism != null)
            {
                //get the meter map, should have this - test for the meter with the correct group
                var meterMap = PemsEntities.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Mechanism && x.MeterId == mechanism.MechIdNumber && x.Customerid == customerId);
                if (meterMap != null)
                {
                    //now go get the meter
                    var meter = meterMap.Meter;
                    model.GlobalId = meter.GlobalMeterId.ToString();

                    model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Mechanism).MeterGroupDesc;
                    model.TypeId = (int)MeterGroups.Mechanism;
                    // model.AssetModel = GetMechanismDescription(customerId, mechanism.MechType ?? -1);
                    model.MechIdNumber = mechanism.MechIdNumber;
                    //*** Final fix by sairam on sep 21st afternoon;
                    var FetchAssetModel = PemsEntities.MechanismMasters.FirstOrDefault(m => m.MeterGroupId == (int)MeterGroups.Mechanism);
                    model.AssetModel = FetchAssetModel.MechanismDesc;
                    //** end of final fix

                    model.AssetModelId = mechanism.MechType ?? -1;

                    //*** Final fix by sairam on sep 21st afternoon;
                    //var meterFetch = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == mechanismId);

                    // Santhosh on 24 Sept 14 for display asset name in View Mechanism
                    var meterFetch = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.MeterId == mechanismId && m.MeterGroup == (int)MeterGroups.Mechanism);

                    //model.Name ="todo";//todo GTC:Mechanism - set asset name here for mechmaster when the db script is added
                    model.Name = meterFetch.MeterName;
                    //** end of final fix

                    var area = PemsEntities.Areas.FirstOrDefault(m => m.AreaID == meterMap.AreaId2 && m.CustomerID == customerId);
                    model.Area = area == null ? "" : area.AreaName;
                    model.AreaId2 = area == null ? -1 : area.AreaID;
                    model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                    model.ZoneId = meterMap.Zone == null ? -1 : meterMap.Zone.ZoneId;
                    model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;
                    model.MaintenanceRoute = meterMap.MaintRouteId == null ? "-" : meterMap.MaintRoute.DisplayName ?? "-";

                    // Lat/Long
                    model.Latitude = meter.Latitude ?? 0.0;
                    model.Longitude = meter.Longitude ?? 0.0;

                    model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
                    model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value.ToShortDateString() : "";
                    model.InstallationDate = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?)null;
                    model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;


                    // Operational status of meter (mechanism).
                    DateTime? opStatusDatetime;
                    int? statusId;
                    model.Status = GetOperationalStatus(customerId,mechanismId, out opStatusDatetime, out statusId);
                    model.StatusDate = opStatusDatetime;
                    model.StatusId = statusId ?? 0;
                    // See if item has pending changes.
                    model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

                    // Is asset active?
                    model.State = (AssetStateType)(meter.MeterState ?? 0);

                    // Get last update by information
                    CreateAndLastUpdate(model);

                    //custom mechanism properties

                    model.SerialNumber = mechanism.MechSerial;
                    model.Street = model.Location = GetMechanismLocation(customerId, mechanism.MechId);
                }

            }
            // Get last update by information
          

            return model;
        }

        public MechanismViewModel GetPendingViewModel(int customerId, int mechanismId)
        {
            var model = GetViewModel(customerId, mechanismId);
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Mechanism && m.MeterId == mechanismId);
            if (assetPending == null)
                return model;
            SetPropertiesOfPendingAssetModel(assetPending, model);

            // Version information
            if (!string.IsNullOrWhiteSpace(assetPending.MechSerialNumber))
                model.SerialNumber = assetPending.MechSerialNumber;
            if (model.Configuration == null)
                model.Configuration = new AssetConfigurationModel();
            model.Configuration.DateInstalled = model.InstallationDate = assetPending.DateInstalled ?? (model.Configuration == null ? (DateTime?)null : model.Configuration.DateInstalled);
            //asset specific properties here
            if (assetPending.WarrantyExpiration.HasValue)
                model.WarrantyExpiration = assetPending.WarrantyExpiration.Value;

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
            CreateAndLastUpdate(model);
           
            return model;
        }

        #endregion

        #region Get Edit Models

        public MechanismEditModel GetEditModel(int customerId, int mechanismId)
        {
            #region 22 Sept 2014
            //var model = new MechanismEditModel
            //{
            //    CustomerId = customerId,
            //    AreaId = (int)AssetAreaId.Gateway,
            //    AssetId = mechanismId,
            //    Configuration =  new AssetConfigurationModel()
            //};

            //var mechanism = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechIdNumber == mechanismId);

            //if (mechanism != null)
            //{
            //    //get the meter map, should have this - test for the meter with the correct group
            //    var meterMap = mechanism.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Mechanism);
            //    if (meterMap != null)
            //    {
            //        //now go get the meter
            //        var meter = meterMap.Meter;
            //        // First build the edit model from the existing 
            //        model.GlobalId = meter.GlobalMeterId.ToString();

            //        model.Type =PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int) MeterGroups.Mechanism).MeterGroupDesc;
            //        model.TypeId = MeterGroups.Mechanism;
            //        model.MechIdNumber = mechanismId;
                    
            //        model.AssetModelId = mechanism.MechType ?? -1;
            //        GetAssetModelList(model, (int)MeterGroups.Mechanism);
            //        model.Name = "todo";//todo set this as the asset name when we get that field added to mechmaster "Name";

            //        // Lat/Long
            //        model.Latitude = meter.Latitude ?? 0.0;
            //        model.Longitude = meter.Longitude ?? 0.0;

            //        model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            //        model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value : (DateTime?)null;
            //        model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;
            //        model.InstallationDate = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?)null;
            //        model.Configuration = new AssetConfigurationModel
            //            {
            //                DateInstalled = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?) null
            //            };
            //        // Operational status of meter (mechanism).
            //        DateTime? opStatusDatetime;
            //        int? opStatusId;
            //        model.Status = GetOperationalStatus(mechanismId, out opStatusDatetime, out opStatusId);
            //        model.StatusDate = opStatusDatetime;
            //        model.StatusId = opStatusId ?? 0;
            //        //make sure this is using the meter assigned to the mech and not the meter that represetnts the mech
            //        GetOperationalStatusDetails(model, model.StatusId, model.StatusDate.HasValue ? model.StatusDate.Value : DateTime.Now);

            //        // Is asset active?
            //         model.StateId = meter.MeterState.GetValueOrDefault();

            //        //custom mechanism properties
            //        model.SerialNumber = mechanism.MechSerial;
            //        model.Street = model.Location = GetMechanismLocation(customerId, mechanism.MechId);
            //    }
            //}

            //// Is asset active?

           

            //// If there is an asset pending record then update edit model with associated values.
            //var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Mechanism && m.MeterId == mechanismId);

            //if (assetPending != null)
            //{
            //    #region Update model from an assetPending

            //    // Get cashbox models.
            //    model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;
            //    if (!string.IsNullOrEmpty(assetPending.MechSerialNumber))
            //        model.SerialNumber = assetPending.MechSerialNumber;
            //    // Get name.
            //    model.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? model.Name : assetPending.AssetName;
            //    model.Street = model.Location = string.IsNullOrWhiteSpace(assetPending.LocationMeters) ? model.Street : assetPending.LocationMeters;
            //    // Get Zone, Area and Suburb
            //    model.AreaListId = assetPending.MeterMapAreaId2 ?? model.AreaListId;
            //    model.ZoneId = assetPending.MeterMapZoneId ?? model.ZoneId;
            //    model.SuburbId = assetPending.MeterMapSuburbId ?? model.SuburbId;
            //    // Preventative maintenance dates
            //    model.LastPrevMaint = assetPending.LastPreventativeMaintenance.HasValue ? assetPending.LastPreventativeMaintenance.Value.ToShortDateString() : model.LastPrevMaint;
            //    model.NextPrevMaint = assetPending.NextPreventativeMaintenance ?? model.NextPrevMaint;
            //    // Lat/Long
            //    model.Latitude = assetPending.Latitude ?? model.Latitude;
            //    model.Longitude = assetPending.Longitude ?? model.Longitude;
            //    // Warantry expiration
            //    model.WarrantyExpiration = assetPending.WarrantyExpiration ?? model.WarrantyExpiration;
            //    // Version profile information

            //    model.Configuration.DateInstalled = model.InstallationDate = assetPending.DateInstalled ?? (model.Configuration == null ? (DateTime?)null : model.Configuration.DateInstalled);
            //    model.Configuration.MPVVersion = assetPending.MPVFirmware ?? (model.Configuration == null ? "" : model.Configuration.MPVVersion);
            //    model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion ?? (model.Configuration == null ? "" : model.Configuration.SoftwareVersion);
            //    model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion ?? (model.Configuration == null ? "" : model.Configuration.FirmwareVersion);
            //    // Operational status of cashbox.
            //    if (assetPending.OperationalStatus != null)
            //        GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);

            //    // Asset state is pending
            //    model.StateId = (int)AssetStateType.Pending;

            //    // Activation Date
            //    model.ActivationDate = assetPending.RecordMigrationDateTime;

            //    #endregion
            //}

            //GetAssetStateList(model);

            //// Initialize ActivationDate if needed and set to midnight.
            //if (!model.ActivationDate.HasValue)
            //    model.ActivationDate = Now;
            //model.ActivationDate = SetToMidnight(model.ActivationDate.Value);

            //return model;
        #endregion

            var model = new MechanismEditModel
            {
                CustomerId = customerId,                
                AreaId = (int)AssetAreaId.Mechanism,
                AssetId = mechanismId,
                Configuration = new AssetConfigurationModel()
            };

            var mechanism = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == customerId && m.MechIdNumber == mechanismId);

            if (mechanism != null)
            {
                //get the meter map, should have this - test for the meter with the correct group
                var meterMap = PemsEntities.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Mechanism && x.MeterId == mechanism.MechIdNumber && x.Customerid == customerId);
                if (meterMap != null)
                {
                    //now go get the meter
                    var meter = meterMap.Meter;
                    // First build the edit model from the existing 
                    model.GlobalId = meter.GlobalMeterId.ToString();

                    model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Mechanism).MeterGroupDesc;
                    model.TypeId = MeterGroups.Mechanism;
                    model.MechIdNumber = mechanismId;

                    model.AssetModelId = mechanism.MechType ?? -1;
                    GetAssetModelList(model, (int)MeterGroups.Mechanism);

                    //** Sairam added on sep 22nd
                    model.Name = meterMap.Meter.MeterName;
                    model.AssetModelName = "";

                   model.AssetModelId = meter.MeterType ?? -1;

          

                    // Lat/Long
                    model.Latitude = meter.Latitude ?? 0.0;
                    model.Longitude = meter.Longitude ?? 0.0;

                    model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
                    model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value : (DateTime?)null;
                    model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;
                    model.InstallationDate = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?)null;
                    model.Configuration = new AssetConfigurationModel
                    {
                        DateInstalled = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?)null
                    };
                    // Operational status of meter (mechanism).
                    DateTime? opStatusDatetime;
                    int? opStatusId;
                    model.Status = GetOperationalStatus(customerId,mechanismId, out opStatusDatetime, out opStatusId);
                    model.StatusDate = opStatusDatetime;
                    model.StatusId = opStatusId ?? 0;
                    //make sure this is using the meter assigned to the mech and not the meter that represetnts the mech
                    GetOperationalStatusDetails(model, model.StatusId, model.StatusDate.HasValue ? model.StatusDate.Value : DateTime.Now);

                    // Is asset active?
                    model.StateId = meter.MeterState.GetValueOrDefault();

                    //custom mechanism properties
                    model.SerialNumber = mechanism.MechSerial;
                    model.Street = model.Location = GetMechanismLocation(customerId, mechanism.MechId);
                }
            }

            // Is asset active?

        #endregion

            // If there is an asset pending record then update edit model with associated values.
            //metermapmechid - todo - update this when the asset pending table
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Mechanism && m.MeterId == mechanismId);

            if (assetPending != null)
            {
                #region Update model from an assetPending

                // Get cashbox models.
                model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;
                if (!string.IsNullOrEmpty(assetPending.MechSerialNumber))
                    model.SerialNumber = assetPending.MechSerialNumber;
                // Get name.
                //todo - gtc: Mechanisms - get this working once they add the name ot the mechmaster table
                model.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? model.Name : assetPending.AssetName;
                model.Street = model.Location = string.IsNullOrWhiteSpace(assetPending.LocationMeters) ? model.Street : assetPending.LocationMeters;
                // Get Zone, Area and Suburb
                model.AreaListId = assetPending.MeterMapAreaId2 ?? model.AreaListId;
                model.ZoneId = assetPending.MeterMapZoneId ?? model.ZoneId;
                model.SuburbId = assetPending.MeterMapSuburbId ?? model.SuburbId;
                // Preventative maintenance dates
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.HasValue ? assetPending.LastPreventativeMaintenance.Value.ToShortDateString() : model.LastPrevMaint;
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance ?? model.NextPrevMaint;
                // Lat/Long
                model.Latitude = assetPending.Latitude ?? model.Latitude;
                model.Longitude = assetPending.Longitude ?? model.Longitude;
                // Warantry expiration
                model.WarrantyExpiration = assetPending.WarrantyExpiration ?? model.WarrantyExpiration;
                // Version profile information

                model.Configuration.DateInstalled = model.InstallationDate = assetPending.DateInstalled ?? (model.Configuration == null ? (DateTime?)null : model.Configuration.DateInstalled);
                model.Configuration.MPVVersion = assetPending.MPVFirmware ?? (model.Configuration == null ? "" : model.Configuration.MPVVersion);
                model.Configuration.SoftwareVersion = assetPending.AssetSoftwareVersion ?? (model.Configuration == null ? "" : model.Configuration.SoftwareVersion);
                model.Configuration.FirmwareVersion = assetPending.AssetFirmwareVersion ?? (model.Configuration == null ? "" : model.Configuration.FirmwareVersion);
                // Operational status of cashbox.
                if (assetPending.OperationalStatus != null)
                    GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);

                // Asset state is pending
                model.StateId = (int)AssetStateType.Pending;

                // Activation Date
                model.ActivationDate = assetPending.RecordMigrationDateTime;

                #endregion
            }

            GetAssetStateList(model);

            // Initialize ActivationDate if needed and set to midnight.
            if (!model.ActivationDate.HasValue)
                model.ActivationDate = Now;
            model.ActivationDate = SetToMidnight(model.ActivationDate.Value);

            return model;
        }

        public void RefreshEditModel(MechanismEditModel model)
        {
            GetAssetModelList(model, (int)MeterGroups.Mechanism);
            GetAssetStateList(model);
        }

        public void SetEditModel(MechanismEditModel model)
        {
            var otherModel = GetEditModel(model.CustomerId, (int)model.AssetId);
            (new PendingFactory(ConnectionStringName, Now)).Save(model, otherModel);
        }

        public MechanismMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> mechanismIds)
        {
            var editModel = new MechanismMassEditModel
            {
                CustomerId = customerId
            };
            SetPropertiesOfAssetMassiveEditModel(editModel, mechanismIds);
            // Get State list
            editModel.StateId = -1;
            GetAssetStateList(editModel);

            // Preventative maintenance dates
            editModel.LastPrevMaint = (DateTime?)null;
            editModel.NextPrevMaint = (DateTime?)null;

            // Warantry expiration
            editModel.WarrantyExpiration = (DateTime?)null;

            // Initialize ActivationDate and set to midnight.
            editModel.ActivationDate = SetToMidnight(Now);
            return editModel;
        }

        public void RefreshMassEditModel(MechanismMassEditModel editModel)
        {
            GetAssetStateList(editModel);
        }

        public void SetMassEditModel(MechanismMassEditModel model)
        {
            var pendingFactory = new PendingFactory(ConnectionStringName, Now);
            foreach (var asset in model.AssetIds())
            {
                pendingFactory.Save(model, model.CustomerId, (int)asset.AssetId);
            }
        }


        #region Create/Clone

        private int Clone(int customerId, Int64 assetId)
        {

            // Get original mechanism and its associated Meter and MeterMap
            // Since this exists, these entities should exist also.
            MechMaster mechMaster = PemsEntities.MechMasters.FirstOrDefault(m => m.MechIdNumber == assetId && m.Customerid == customerId);


            if (mechMaster != null)
            {
                MeterMap meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.MeterId == assetId && m.Customerid == customerId && m.Areaid == (int)AssetAreaId.Mechanism);
                if (meterMap != null)
                {

                    Meter meter = meterMap.Meter;

                    // Create the new meter that is associated with the gateway type of asset.  This is not a meter
                    // but rather a an asset in the meter table that represents a gateway.
                    Meter newMeter = CreateBaseAsset(meter.CustomerID, MeterGroups.Mechanism, meter.AreaID);

                    // Create a gateway

                    string newSerialNumber = GenerateNewSerialNumber(mechMaster.MechSerial, 1);

                    var clonedItem = new MechMaster()
                        {
                            //todo - GTC: Mechanisms add mechname here when db field is added
                            MechId = newMeter.MeterId,
                            //this is auto increment, so it wont save the value, its why we have the mechidnumber column.
                            MechIdNumber = newMeter.MeterId,
                            MechSerial = newSerialNumber,
                            MechType = mechMaster.MechType,
                            Customerid = customerId,
                            SimNo = mechMaster.SimNo,
                            IsActive = mechMaster.IsActive,
                            CreateDate = DateTime.Now,
                            InsertedDate = DateTime.Now,

                        };

                    PemsEntities.MechMasters.Add(clonedItem);
                    PemsEntities.SaveChanges();

                    // Add AssetPending record
                    (new PendingFactory(ConnectionStringName, Now)).SetImportPending(
                        AssetTypeModel.AssetType.Mechanism, SetToMidnight(Now), AssetStateType.Current, customerId,
                        clonedItem.MechIdNumber.GetValueOrDefault(), (int)AssetStateType.Pending, (int)OperationalStatusType.Inactive, (int)AssetAreaId.Mechanism);

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

                    // Create meter map for cloned item.
                    //  Need a [HousingMaster]
                    HousingMaster housingMaster =
                        PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

                    // Create a [MeterMap] entry
                    MeterMap newMeterMap = new MeterMap()
                        {
                            Customerid = newMeter.CustomerID,
                            Areaid = newMeter.AreaID,
                            MeterId = newMeter.MeterId,
                           // MechId = clonedItem.MechId,
                            HousingId = housingMaster.HousingId
                        };
                    PemsEntities.MeterMaps.Add(newMeterMap);
                    PemsEntities.SaveChanges();

                    newMeterMap.AreaId2 = meterMap.AreaId2;
                    newMeterMap.ZoneId = meterMap.ZoneId;
                    newMeterMap.SubAreaID = meterMap.SubAreaID;
                    newMeterMap.CustomGroup1 = meterMap.CustomGroup1;
                    newMeterMap.CustomGroup2 = meterMap.CustomGroup2;
                    newMeterMap.CustomGroup3 = meterMap.CustomGroup3;
                    newMeterMap.MaintRouteId = meterMap.MaintRouteId;

                    PemsEntities.SaveChanges();

                    // Set audit records.
                    Audit(newMeter);
                    Audit(newMeterMap);
                    Audit(clonedItem);

                    return clonedItem.MechIdNumber.GetValueOrDefault();
                }
            }
            return 0;
        }

        /// <summary>
        /// Generates a unique serial number for this customer. 
        /// </summary>
        /// <param name="currentSerial"></param>
        /// <returns></returns>
        private string GenerateNewSerialNumber(string currentSerial, int serialIterator)
        {
           //taket he serial, add _1 to it, then if that exist, add _2, etc until you find a valid new serial number

            var newSerial = currentSerial + "_" + serialIterator.ToString();
            if (!PemsEntities.MechMasters.Any(x => x.MechSerial == newSerial))
                return newSerial;

            return GenerateNewSerialNumber(currentSerial, serialIterator + 1);
        }

        public int Clone(MechanismViewModel model)
        {
            return Clone(model.CustomerId, model.AssetId);
        }

        public int Clone(MechanismEditModel model)
        {
            return Clone(model.CustomerId, model.AssetId);
        }

        public MechMaster Create(int customerId, string serialNumber)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(customerId, MeterGroups.Mechanism);
            return CreateMechMaster(customerId, newMeter,  serialNumber);
        }

        public MechMaster Create(int customerId, AssetTypeModel.EnterMode enterMode, int mechanismId, string serialNumber)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(enterMode, mechanismId, customerId, MeterGroups.Mechanism);
            return CreateMechMaster(customerId, newMeter, serialNumber);
        }


        private MechMaster CreateMechMaster(int customerId, Meter newMeter, string serialNumber)
        {
            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
            newMeter.TimeZoneID = customerProfile.TimeZoneID ?? 0;

            // Create an audit entry.
            Audit(newMeter);

            // Create a mechanism
            var newItem = new MechMaster()
                {
                    //todo - GTC: Mechanisms add mechname here when db field is added - might have to pass in the mech name
                    MechId = newMeter.MeterId,//this is auto increment field, so thats why we have the mechidnumber field
                    MechIdNumber = newMeter.MeterId,
                    Customerid = customerId,
                    IsActive = false,
                    CreateDate = DateTime.Now,
                    MechSerial = serialNumber,
                    InsertedDate = DateTime.Now,
                   
                };
            PemsEntities.MechMasters.Add(newItem);
            PemsEntities.SaveChanges();

            // Add audit record
            Audit(newItem);

            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            // Create a MeterMap record to join mechanism and Meter
            MeterMap meterMap = new MeterMap()
                {
                    Customerid = newMeter.CustomerID,
                    Areaid = newMeter.AreaID,
                    MeterId = newMeter.MeterId,
                 //   MechId = newItem.MechId,
                    HousingId = housingMaster.HousingId
                };

            PemsEntities.MeterMaps.Add(meterMap);
            PemsEntities.SaveChanges();
            return newItem;
        }

        #endregion

        #region History
        //todo - GTC - mechanism history -finish this based on another factory example (gateway)
        /// <summary>
        ///  Description: This Method will retrive histtry summary
        ///   ModifiedBy: Santhosh  (26/June/2014 - 04/July/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="total"></param>
        /// <param name="customerId"></param>
        /// <param name="areaId"></param>
        /// <param name="assetId"></param>
        /// <param name="startDateTicks"></param>
        /// <param name="endDateTicks"></param>
        /// <returns></returns>
        public List<MechanismViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {

            DateTime startDate;
            DateTime endDate;

            IQueryable<MechMasterAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.MechMasterAudits.Where(m => m.Customerid == customerId && m.MechId == assetId
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.MechMasterAudits.Where(m => m.Customerid == customerId && m.MechId == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.MechMasterAudits.Where(m => m.Customerid == customerId && m.MechId == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.MechMasterAudits.Where(m => m.Customerid == customerId && m.MechId == assetId).OrderBy(m => m.UpdateDateTime);
            }



            // Get the CashboxViewModel for each audit record.
            var list = new List<MechanismViewModel>();

            // Get present data
            var presentModel = GetViewModel(customerId, (int)assetId);
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data view models
            MechanismViewModel previousModel = null;
            foreach (var mech in query)
            {
                var activeModel = CreateHistoricViewModel(mech);
                list.Add(activeModel);
                if (previousModel != null)
                {
                    previousModel.RecordSuperceededDate = activeModel.RecordDate;
                }
                previousModel = activeModel;
            }

            // Now filter, sort and page.
            var items = list.AsQueryable();

            //items = items.ApplyFiltering(request.Filters);
            total = items.Count();
            //items = items.ApplySorting(request.Groups, request.Sorts);
            //items = items.ApplyPaging(request.Page, request.PageSize);
            return items.ToList();
        }

        /// <summary>
        ///  Description: This Method will retrive history summary details for particular audit
        ///   ModifiedBy: Santhosh  (26/June/2014 - 04/July/2014)   
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="mechanismId"></param>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public MechanismViewModel GetHistoricViewModel(int customerId, int mechanismId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.MechMasterAudits.FirstOrDefault(m => m.MechMasterAuditId == auditId));
        }

        //TODO - GTC work : mechanism history 
        // these methods will have to be updated to pull back the correct data
        public AssetHistoryModel GetHistoricListViewModel(int customerId, int mechanismId)
        {
            var model = new AssetHistoryModel
            {
                CustomerId = customerId,
                AssetId = mechanismId,
                TypeId = (int)MeterGroups.Mechanism,
                Street = "-"
            };

            var type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == mechanismId);
            model.Type = type == null
                ? MechanismViewModel.DefaultType
                : type.MeterGroupDesc ?? MechanismViewModel.DefaultType;

            return model;
        }

        //TODO - GTC work : mechanism history 
        private MechanismViewModel createHistoricViewModel()
        {
            var model = new MechanismViewModel();

            return model;
        }
        /// <summary>
        ///  Description: This Method will create history summary madel  for particular audit
        ///   ModifiedBy: Santhosh  (26/June/2014 - 04/July/2014)    
        /// </summary>
        /// <param name="auditModel"></param>
        /// <returns></returns>

        private MechanismViewModel CreateHistoricViewModel(MechMasterAudit auditModel)
        {
            var model = new MechanismViewModel()
            {
                CustomerId = auditModel.Customerid ?? 0,
                AreaId = (int)AssetAreaId.Mechanism,
                AssetId = auditModel.MechId,

            };


            var mechanism = PemsEntities.MechMasters.FirstOrDefault(m => m.Customerid == model.CustomerId && m.MechIdNumber == auditModel.MechId);

            if (mechanism != null)
            {
                //get the meter map, should have this - test for the meter with the correct group
                var meterMap = PemsEntities.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Mechanism && x.MeterId == mechanism.MechIdNumber && x.Customerid == model.CustomerId);
                if (meterMap != null)
                {
                    //now go get the meter
                    var meter = meterMap.Meter;
                    model.GlobalId = meter.GlobalMeterId.ToString();

                    model.MechIdNumber = meterMap.MechId;

                    model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Mechanism).MeterGroupDesc;
                    model.TypeId = (int)MeterGroups.Mechanism;
                    model.AssetModel = GetMechanismDescription(model.CustomerId, mechanism.MechType ?? -1);
                    model.AssetModelId = mechanism.MechType ?? -1;
                    model.Name = "todo";//todo GTC:Mechanism - set asset name here for mechmaster when the db script is added

                    var area = PemsEntities.Areas.FirstOrDefault(m => m.AreaID == meterMap.AreaId2 && m.CustomerID == model.CustomerId);
                    model.Area = area == null ? "" : area.AreaName;
                    model.AreaId2 = area == null ? -1 : area.AreaID;
                    model.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName ?? "";
                    model.ZoneId = meterMap.Zone == null ? -1 : meterMap.Zone.ZoneId;
                    model.Suburb = meterMap.CustomGroup11 == null ? " " : meterMap.CustomGroup11.DisplayName;
                    model.MaintenanceRoute = meterMap.MaintRouteId == null ? "-" : meterMap.MaintRoute.DisplayName ?? "-";

                    // Lat/Long
                    model.Latitude = meter.Latitude ?? 0.0;
                    model.Longitude = meter.Longitude ?? 0.0;

                    model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
                    model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value.ToShortDateString() : "";
                    model.InstallationDate = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?)null;
                    model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;


                    // Operational status of meter (mechanism).
                    DateTime? opStatusDatetime;
                    int? statusId;
                    model.Status = GetOperationalStatus(model.CustomerId, auditModel.MechId, out opStatusDatetime, out statusId);
                    model.StatusDate = opStatusDatetime;
                    model.StatusId = statusId ?? 0;
                    // See if item has pending changes.
                    model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

                    // Is asset active?
                    model.State = (AssetStateType)(meter.MeterState ?? 0);



                    //custom mechanism properties

                    model.SerialNumber = mechanism.MechSerial;
                    model.Street = model.Location = GetMechanismLocation(model.CustomerId, mechanism.MechId);

                    // Get last update by information
                    CreateAndLastUpdate(model);

                    // Record date
                    model.RecordDate = auditModel.UpdateDateTime;

                    // Audit record id
                    model.AuditId = auditModel.MechMasterAuditId;
                }

            }


            return model;
        }


        #endregion


        public string SerialNumberExistance(int customerId, string serialNumber)
        {
            var serialIdNew = "";
            var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.Customerid == customerId && x.MechSerial == serialNumber);
          //  var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.MechSerial == serialNumber);
            if (existingMechanism != null)
                serialIdNew = "Serial " + serialNumber + " already in use";
            else
                serialIdNew = "Serial " + serialNumber + " Valid";
            return serialIdNew;
        }

        public bool SerialNumberExistance(int customerId, string serialNumber, int meterid)
        {
            var serialIdNew = "";
            var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.Customerid == customerId && x.MechSerial == serialNumber);
            if (existingMechanism != null && existingMechanism.MechIdNumber != meterid)
                return true;
           
            return false;
        }
        public string SingleSpaceSerialNumberExistance(int customerId, string serialNumber)
        {
            var serialIdNew = "";

            var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.MechSerial == serialNumber);
            if (existingMechanism != null)
            {
                serialIdNew = "Serial " + serialNumber + "Valid";
                var query1 = from mm in PemsEntities.MechMasters
                             where mm.MechSerial == serialNumber

                             select new { mm.MechSerial, mm.Customerid, mm.MechId, mm.MechIdNumber };


                foreach (var mmDesc in query1)
                {
                    var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                           where s.MechId == mmDesc.MechId
                                           select s).FirstOrDefault();

                    if (mechMasterMeter != null)
                    {
                        serialIdNew = "Serial " + serialNumber + " already in use";
                        break;
                    }
                }


            }
            else
                serialIdNew = "Serial " + serialNumber + "Not Valid use";
            return serialIdNew;
        }

        public string LocationIdExistance(int customerId, string locationname)
        {
            var serialIdNew = "";
            var existingMechanism = PemsEntities.HousingMasters.FirstOrDefault(x => x.Customerid == customerId && x.HousingName == locationname);
            if (existingMechanism != null)
                serialIdNew = "Location ID " + locationname + " already in use";
            else
                serialIdNew = "Location ID " + locationname + " Valid";
            return serialIdNew;
        }


        public string SingleSpaceSerialNumberExistanceMeter(int customerId, string serialNumber, int MeterId)
        {
            var serialIdNew = "";



            var existingMechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.MechSerial == serialNumber);
            if (existingMechanism != null)
            {
                serialIdNew = "Serial " + serialNumber + "Valid";
                var query1 = from mm in PemsEntities.MechMasters
                             where mm.MechSerial == serialNumber

                             select new { mm.MechSerial, mm.Customerid, mm.MechId, mm.MechIdNumber };


                foreach (var mmDesc in query1)
                {
                    var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                           where s.MechId == mmDesc.MechId
                                           select s).FirstOrDefault();

                    if (mechMasterMeter != null)
                    {
                        if (mechMasterMeter.MeterId != MeterId)
                            serialIdNew = "Serial " + serialNumber + " already in use";
                        break;
                    }
                }


            }
            else
                serialIdNew = "Serial " + serialNumber + "Not Valid use";          
            return serialIdNew;
        }

        public string LocationIdExistanceMeter(int customerId, string locationname, int MeterId)
        {
            var serialIdNew = "";
            var existingHousing = PemsEntities.HousingMasters.FirstOrDefault(x => x.Customerid == customerId && x.HousingName == locationname );
            if (existingHousing != null)
            {
                var Meterexisting = PemsEntities.MeterMaps.FirstOrDefault(x => x.Customerid == customerId && x.HousingId == existingHousing.HousingId && x.MeterId != MeterId);
                if (Meterexisting != null)
                serialIdNew = "Location ID " + locationname + " already in use";
                else
                    serialIdNew = "Location ID " + locationname + " Valid";
            }
            else
            {
                serialIdNew = "Location ID " + locationname + " Valid";
            }
            return serialIdNew;
        }


    }
}
