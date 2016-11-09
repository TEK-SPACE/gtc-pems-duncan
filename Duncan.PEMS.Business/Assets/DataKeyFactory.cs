using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.UI;

namespace Duncan.PEMS.Business.Assets
{
    //todo GTC: datakey Update this factory with any remaining properties that are needed for the views/auditing
    public class DataKeyFactory : AssetFactory
    {
        public DataKeyFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }

        private string GetOperationalStatus(int dataKeyId, out DateTime? OperationalStatusDateTime, out int? OperationalStatusId)
        {
            OperationalStatusId = null;
            OperationalStatusDateTime = null;
            string status = "Inactive";
            var datakey = PemsEntities.DataKeys.FirstOrDefault(m => m.DataKeyId == dataKeyId);
            if (datakey == null)
                return status;

            //if its plugged into a meter, its operational, otherwise it is inactive.
            //this means that if there is a meter map that isnt a datakey, something else is pointing to it (a SSM or MSM)
            var meterMap = datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey);
            if (meterMap != null)
                if (meterMap.Meter != null)
                {
                    status = meterMap.Meter.OperationalStatu != null ? meterMap.Meter.OperationalStatu.OperationalStatusDesc : "Inactive";
                    OperationalStatusId = meterMap.Meter.OperationalStatu != null ? meterMap.Meter.OperationalStatu.OperationalStatusId : (int?)null;
                    OperationalStatusDateTime = meterMap.Meter.OperationalStatusTime;
                }
            return status;
        }

        #region Index

        /// <summary>
        /// Get a queryable list of datakey summaries (<see cref="IQueryable{AssetListModel}"/>) for
        /// customer id of <paramref name="customerId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="gridType">The type of grid (<see cref="AssetListGridType"/>) that will be using the data</param>
        /// <returns>An instance of <see cref="IQueryable{AssetListModel}"/></returns>
        public IQueryable<AssetListModel> GetSummaryModels(int customerId, AssetListGridType gridType)
        {


            var items = from datakey in PemsEntities.DataKeys
                        where datakey.CustomerID == customerId
                        join meterMap in PemsEntities.MeterMaps on
                            new { DataKeyId = datakey.DataKeyId, CustomerId = datakey.CustomerID } equals
                            new { DataKeyId = meterMap.DataKeyId.Value, CustomerId = meterMap.Customerid } into l1
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
                                Type = "DataKey", //name of the details page (ViewDataKey)
                                AreaId = (int)AssetAreaId.DataKey,
                                CustomerId = customerId,

                                //-----------FILTERABLE ITEMS
                                AssetType = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Datakey).MeterGroupDesc,
                                AssetId = datakey.DataKeyIdNumber ?? mm.DataKeyId.Value,
                                AssetName = mm.Meter.MeterName,
                                InventoryStatus = datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey) == null ? " "
                                 : datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey).Meter.MeterState == null ? " "
                                 : datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey).Meter.AssetState.AssetStateDesc,
                               
                                //have to make sure we get the op status of the meter that this datakey is installed in, and NOT the "Meter" table entry for this datakey
                                OperationalStatus = datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey) == null ? " "
                                 : datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey).Meter.OperationalStatu == null ? " "
                                 : datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup != (int)MeterGroups.Datakey).Meter.OperationalStatu.OperationalStatusDesc,
                                Latitude = mm.Meter.Latitude,
                                Longitude = mm.Meter.Longitude,
                                AreaId2 = mm.AreaId2,
                                ZoneId = mm.ZoneId,
                                Suburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId) == null ? " "
                                    : PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == mm.CustomGroup1 && x.CustomerId == customerId).DisplayName,
                                Street = mm.Meter.Location ?? " ",
                                DemandStatus = PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == mm.Meter.DemandZone) == null ? "" : PemsEntities.DemandZones.FirstOrDefault(x => x.DemandZoneId == mm.Meter.DemandZone).DemandZoneDesc,

                                //------------COMMON COLUMNS
                                AssetModel = datakey.DataKeyDesc,

                                //------------SUMMARY GRID
                                //SpacesCount = PemsEntities.SensorMappings.Count(x => x.CustomerID == customerId && x.GatewayID == g.GateWayID),
                                Area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? ""
                                    : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                                Zone = PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId) == null ? ""
                                    : PemsEntities.Zones.FirstOrDefault(x => x.ZoneId == mm.ZoneId && x.customerID == customerId).ZoneName,

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
        /// <param name="dataKeyId"></param>
        /// <returns></returns>
        public DataKeyViewModel GetViewModel(int customerId, int dataKeyId)
        {
            var model = new DataKeyViewModel
            {
                CustomerId = customerId,
                AreaId = (int)AssetAreaId.DataKey,
                AssetId = dataKeyId
            };

            var datakey = PemsEntities.DataKeys.FirstOrDefault(m => m.CustomerID == customerId && m.DataKeyId == dataKeyId);

            if (datakey != null)
            {
                //get the meter map, should have this - test for the meter with the correct group
                var meterMap = datakey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Datakey && x.DataKeyId == datakey.DataKeyId);
                if (meterMap != null)
                {
                    //now go get the meter
                    var meter = meterMap.Meter;
                    model.GlobalId = meter.GlobalMeterId.ToString();

                    model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Datakey).MeterGroupDesc;
                    model.TypeId = (int)MeterGroups.Datakey;
                    model.AssetModel = GetMechanismDescription(model.CustomerId, datakey.MechType ?? -1);
                    model.AssetModelId = datakey.DataKeyTypeId;
                    model.Name = "todo";//todo GTC:datakey - set asset name here for datakey when the db script is added. This may be data desc...

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


                    // Operational status of meter (datakey).
                    DateTime? opStatusDatetime;
                    int? statusId;
                    model.Status = GetOperationalStatus(dataKeyId, out opStatusDatetime, out statusId);
                    model.StatusDate = opStatusDatetime;
                    model.StatusId = statusId ?? 0;
                    // See if item has pending changes.
                    model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

                    // Is asset active?
                    model.State = (AssetStateType)(meter.MeterState ?? 0);

                    // Get last update by information
                    CreateAndLastUpdate(model);

                    //custom DataKey properties
                    model.Street = model.Location = GetMechanismLocation(customerId, datakey.DataKeyId);
                    model.DataKeyTypeId = datakey.DataKeyTypeId;
                    model.StorageMBit = datakey.StorageMBit;
                    model.CityCode = datakey.CityCode;
                    model.ColorCode = datakey.ColorCode;
                }

            }

            return model;
        }

        public DataKeyViewModel GetPendingViewModel(int customerId, int datakeyId)
        {
            var model = GetViewModel(customerId, datakeyId);
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Datakey && m.MeterId == datakeyId);
            if (assetPending == null)
                return model;
            SetPropertiesOfPendingAssetModel(assetPending, model);

            // Version information
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

        public DataKeyEditModel GetEditModel(int customerId, int dataKeyId)
        {

            var model = new DataKeyEditModel
            {
                CustomerId = customerId,
                AreaId = (int)AssetAreaId.DataKey,
                AssetId = dataKeyId,
                Configuration =  new AssetConfigurationModel()
            };

            var dataKey = PemsEntities.DataKeys.FirstOrDefault(m => m.CustomerID == customerId && m.DataKeyIdNumber == dataKeyId);

            if (dataKey != null)
            {
                //get the meter map, should have this - test for the meter with the correct group
                var meterMap = dataKey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Datakey);
                if (meterMap != null)
                {
                    //now go get the meter
                    var meter = meterMap.Meter;
                    // First build the edit model from the existing 
                    model.GlobalId = meter.GlobalMeterId.ToString();

                    model.Type =PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int) MeterGroups.Datakey).MeterGroupDesc;
                    model.TypeId = MeterGroups.Datakey;
                    model.DataKeyIdNumber = dataKeyId;

                    model.AssetModelId = dataKey.DataKeyTypeId;
                    GetAssetModelList(model, (int)MeterGroups.Datakey);
                    model.Name = "todo";//todo gtc set this as the asset name when we get that field added to datakey "Name";

                    // Lat/Long
                    model.Latitude = meter.Latitude ?? 0.0;
                    model.Longitude = meter.Longitude ?? 0.0;

                    model.LastPrevMaint = meter.LastPreventativeMaintenance.HasValue ? meter.LastPreventativeMaintenance.Value.ToShortDateString() : "";
                    model.NextPrevMaint = meter.NextPreventativeMaintenance.HasValue ? meter.NextPreventativeMaintenance.Value : (DateTime?)null;
                    model.WarrantyExpiration = meter.WarrantyExpiration.HasValue ? meter.WarrantyExpiration.Value : (DateTime?)null;
                    model.InstallationDate = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?)null;
                    model.Configuration = new AssetConfigurationModel
                        {
                            DateInstalled = meter.InstallDate.HasValue ? meter.InstallDate.Value : (DateTime?) null
                        };
                    // Operational status of meter (datakey).
                    DateTime? opStatusDatetime;
                    int? opStatusId;
                    model.Status = GetOperationalStatus(dataKeyId, out opStatusDatetime, out opStatusId);
                    model.StatusDate = opStatusDatetime;
                    model.StatusId = opStatusId ?? 0;
                    //make sure this is using the meter assigned to the datakey and not the meter that represetnts the datakey
                    GetOperationalStatusDetails(model, model.StatusId, model.StatusDate.HasValue ? model.StatusDate.Value : DateTime.Now);

                    // Is asset active?
                     model.StateId = meter.MeterState.GetValueOrDefault();

                    //custom datakey properties
                     model.Street = model.Location = GetDataKeyLocation(customerId, dataKey.DataKeyId);
                }
            }

            // Is asset active?

            #endregion

            // If there is an asset pending record then update edit model with associated values.
            //metermapdatakeyid - todo - update this when the asset pending table
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Datakey && m.MeterId == dataKeyId);

            if (assetPending != null)
            {
                #region Update model from an assetPending

                // Get datakey models.
                model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;
                // Get name.
                //todo - gtc: datakey - get this working once they add the name ot the datakey table. This may be datakaydesc
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
                // Operational status of datakey.
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

        public void RefreshEditModel(DataKeyEditModel model)
        {
            GetAssetModelList(model, (int)MeterGroups.Datakey);
            GetAssetStateList(model);
        }

        public void SetEditModel(DataKeyEditModel model)
        {
            var otherModel = GetEditModel(model.CustomerId, (int)model.AssetId);
            (new PendingFactory(ConnectionStringName, Now)).Save(model, otherModel);
        }

        public DataKeyMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> dataKeyIds)
        {
            var editModel = new DataKeyMassEditModel();
            editModel.CustomerId = customerId;

            SetPropertiesOfAssetMassiveEditModel(editModel, dataKeyIds);
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

        public void RefreshMassEditModel(DataKeyMassEditModel editModel)
        {
            GetAssetStateList(editModel);
        }

        public void SetMassEditModel(DataKeyMassEditModel model)
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

            // Get original datakey and its associated Meter and MeterMap
            // Since this exists, these entities should exist also.
            DataKey dataKey = PemsEntities.DataKeys.FirstOrDefault(m => m.DataKeyId == assetId);

            if (dataKey != null)
            {
                MeterMap meterMap = dataKey.MeterMaps.FirstOrDefault(m => m.DataKeyId == dataKey.DataKeyId);
                if (meterMap != null)
                {

                    Meter meter = meterMap.Meter;

                    // Create the new meter that is associated with the datakey type of asset.  This is not a meter
                    // but rather a an asset in the meter table that represents a datakey.
                    Meter newMeter = CreateBaseAsset(meter.CustomerID, MeterGroups.Datakey, meter.AreaID);

                    // Create a datakey
                    var clonedItem = new DataKey()
                        {
                            //todo - GTC: DataKeys add datakeyname here when db field is added
                            DataKeyDesc = dataKey.DataKeyDesc,
                            DataKeyId = newMeter.MeterId,
                            //this is auto increment, so it wont save the value, its why we have the datakeyidnumber column.
                            DataKeyIdNumber = newMeter.MeterId, 
                            DataKeyType = dataKey.DataKeyType,
                            CustomerID = customerId
                        };

                    PemsEntities.DataKeys.Add(clonedItem);
                    PemsEntities.SaveChanges();

                    // Add AssetPending record
                    (new PendingFactory(ConnectionStringName, Now)).SetImportPending(
                        AssetTypeModel.AssetType.DataKey, SetToMidnight(Now), AssetStateType.Current, customerId,
                        clonedItem.DataKeyIdNumber.GetValueOrDefault(), (int)AssetStateType.Pending, (int)OperationalStatusType.Inactive, (int)AssetAreaId.DataKey);

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
                            DataKeyId = clonedItem.DataKeyId,
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

                    return clonedItem.DataKeyIdNumber.GetValueOrDefault();
                }
            }
            return 0;
        }

        public int Clone(DataKeyViewModel model)
        {
            return Clone(model.CustomerId, model.AssetId);
        }

        public int Clone(DataKeyEditModel model)
        {
            return Clone(model.CustomerId, model.AssetId);
        }

        public DataKey Create(int customerId)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(customerId, MeterGroups.Datakey);
            return CreateDataKey(customerId, newMeter);
        }

        public DataKey Create(int customerId, AssetTypeModel.EnterMode enterMode, int dataKeyId)
        {
            // Create a new Meters entry.
            Meter newMeter = CreateBaseAsset(enterMode, dataKeyId, customerId, MeterGroups.Datakey);
            return CreateDataKey(customerId, newMeter);
        }


        private DataKey CreateDataKey(int customerId, Meter newMeter)
        {
            // Get customer TimeZoneId
            var customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
            newMeter.TimeZoneID = customerProfile.TimeZoneID ?? 0;

            // Create an audit entry.
            Audit(newMeter);

            // Create a datakey
            var newItem = new DataKey()
                {
                    //todo - GTC: DataKeys add dataKeyname here when db field is added - might have to pass in the datakey name
                    DataKeyId = newMeter.MeterId,//this is auto increment field, so thats why we have the datakeyidnumber field
                    DataKeyIdNumber = newMeter.MeterId, 
                    CustomerID = customerId,

                };
            PemsEntities.DataKeys.Add(newItem);
            PemsEntities.SaveChanges();

            // Add audit record
            Audit(newItem);

            //  Need a [HousingMaster]
            HousingMaster housingMaster = PemsEntities.HousingMasters.FirstOrDefault(m => m.HousingName.Equals("Default"));

            // Create a MeterMap record to join datakey and Meter
            MeterMap meterMap = new MeterMap()
                {
                    Customerid = newMeter.CustomerID,
                    Areaid = newMeter.AreaID,
                    MeterId = newMeter.MeterId,
                    DataKeyId = newItem.DataKeyId,
                    HousingId = housingMaster.HousingId
                };

            PemsEntities.MeterMaps.Add(meterMap);
            PemsEntities.SaveChanges();
            return newItem;
        }

        #endregion

        #region History
        //todo - GTC - datakey history - finish this based on another factory example (gateway)
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
        public List<DataKeyViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {
            DateTime startDate;
            DateTime endDate;

            IQueryable<DataKeyAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.DataKeyAudits.Where(m => m.CustomerID == customerId && m.DataKeyId == assetId
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.DataKeyAudits.Where(m => m.CustomerID == customerId && m.DataKeyId == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.DataKeyAudits.Where(m => m.CustomerID == customerId && m.DataKeyId == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.DataKeyAudits.Where(m => m.CustomerID == customerId && m.DataKeyId == assetId).OrderBy(m => m.UpdateDateTime);
            }



            // Get the DataKeyViewModel for each audit record.
            var list = new List<DataKeyViewModel>();

            // Get present data
            var presentModel = GetViewModel(customerId, (int)assetId);
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data view models
            DataKeyViewModel previousModel = null;
            foreach (var dataKeyItem in query)
            {
                var activeModel = CreateHistoricViewModel(dataKeyItem);
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
        /// <param name="dataKeyId"></param>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public DataKeyViewModel GetHistoricViewModel(int customerId, int dataKeyId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.DataKeyAudits.FirstOrDefault(m => m.DataKeyAuditId == auditId));
        }

        // these methods will have to be updated to pull back the correct data
        public AssetHistoryModel GetHistoricListViewModel(int customerId, int dataKeyId)
        {
            var model = new AssetHistoryModel
            {
                CustomerId = customerId,
                AssetId = dataKeyId,
                TypeId = (int)MeterGroups.Datakey,
                Street = "-"
            };

            var type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == dataKeyId);
            model.Type = type == null
                ? DataKeyViewModel.DefaultType
                : type.MeterGroupDesc ?? DataKeyViewModel.DefaultType;

            return model;
        }

        /// <summary>
        ///  Description: This Method will create history summary madel  for particular audit
        ///   ModifiedBy: Santhosh  (26/June/2014 - 04/July/2014)    
        /// </summary>
        /// <param name="auditModel"></param>
        /// <returns></returns>

        private DataKeyViewModel CreateHistoricViewModel(DataKeyAudit auditModel)
        {
            var model = new DataKeyViewModel()
            {
                CustomerId = auditModel.CustomerID ?? 0,
                AreaId = (int)AssetAreaId.DataKey,
                AssetId = auditModel.DataKeyId ?? 0,

            };


            var dataKey = PemsEntities.DataKeys.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.DataKeyId == auditModel.DataKeyId);

            if (dataKey != null)
            {
                //get the meter map, should have this - test for the meter with the correct group
                var meterMap = dataKey.MeterMaps.FirstOrDefault(x => x.Meter != null && x.Meter.MeterGroup == (int)MeterGroups.Datakey && x.DataKeyId == dataKey.DataKeyId);
                if (meterMap != null)
                {
                    //now go get the meter
                    var meter = meterMap.Meter;
                    model.GlobalId = meter.GlobalMeterId.ToString();

                    model.DataKeyIdNumber = meterMap.DataKeyId;

                    model.Type = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == (int)MeterGroups.Datakey).MeterGroupDesc;
                    model.TypeId = (int)MeterGroups.Datakey;
                    model.AssetModel = GetMechanismDescription(model.CustomerId, dataKey.MechType ?? -1);
                    model.AssetModelId = dataKey.DataKeyTypeId;
                    model.Name = "todo";//todo GTC:datakey - set asset name here for datakey when the db script is added

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


                    // Operational status of meter (datakey).
                    DateTime? opStatusDatetime;
                    int? statusId;
                    model.Status = GetOperationalStatus(auditModel.DataKeyId ?? 0, out opStatusDatetime, out statusId);
                    model.StatusDate = opStatusDatetime;
                    model.StatusId = statusId ?? 0;
                    // See if item has pending changes.
                    model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

                    // Is asset active?
                    model.State = (AssetStateType)(meter.MeterState ?? 0);

                    // Get the full-spell change reason.
                    model.LastUpdatedReason = (AssetPendingReasonType)(auditModel.AssetPendingReasonId ?? 0);
                    model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;


                    //custom datakey properties
                    model.Street = model.Location = GetMechanismLocation(model.CustomerId, dataKey.DataKeyId);

                    // Get last update by information
                    CreateAndLastUpdate(model);

                    // Record date
                    model.RecordDate = auditModel.UpdateDateTime;

                    // Audit record id
                    model.AuditId = auditModel.DataKeyAuditId;
                }

            }


            return model;
        }


        #endregion
    }
}
