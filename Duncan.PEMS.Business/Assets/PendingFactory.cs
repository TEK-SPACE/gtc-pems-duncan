/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/15/2014       Sergey Ostrerov                 DPTXPEMS-124 - Can't update Asset Information with new values.
 * 01/15/2014       Sergey Ostrerov                 DPTXPEMS-8 - Can't create new TX meter through PEMS UI.
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
using NLog;
using WebMatrix.WebData;


namespace Duncan.PEMS.Business.Assets
{
    public class PendingFactory : AssetFactory
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
        public PendingFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
        }

        //public PendingFactory(string connectionStringName)
        //    : base(connectionStringName)
        //{
        //}

        #region Check for Pending Changes

        public bool Pending(MeterViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterId == model.AssetId)
                != null;
        }

        public bool Pending(MeterEditModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterId == model.AssetId)
                != null;
        }

        public bool Pending(CashboxViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapCashBoxId == model.AssetId)
                != null;
        }

        public bool Pending(GatewayViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapGatewayId == model.AssetId)
                != null;
        }

        public bool Pending(SensorViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapSensorId == model.AssetId)
                != null;
        }

        public bool Pending(SpaceViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == model.AssetId && m.AssetType == (int)MeterGroups.Space)
                != null;
        }

        public bool Pending(MechanismViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == model.AssetId && m.AssetType == (int)MeterGroups.Mechanism)
                != null;
        }

        public bool Pending(DataKeyViewModel model)
        {
            return
                PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == model.AssetId && m.AssetType == (int)MeterGroups.Datakey)
                != null;
        }

        #endregion

        #region Set Pending State Only

        /// <summary>
        /// Set an [AssetPending] record indication asset is ready to activate.
        /// </summary>
        /// <param name="assetType"><see cref="AssetTypeModel.AssetType"/> type of asset</param>
        /// <param name="activateDate">Date requested for activation</param>
        /// <param name="assetState">Asset state <see cref="AssetStateType"/></param>
        /// <param name="customerId">Customer Id</param>
        /// <param name="assetId">Asset Id</param>
        /// <param name="areaId">Optional Area Id</param>
        public void SetImportPending(AssetTypeModel.AssetType assetType, DateTime? activateDate, AssetStateType assetState, int customerId, Int64 assetId, int AssetState, int operationalstatus, int areaId = 0)
        {
            AssetPending assetPending = null;

            switch (assetType)
            {
                case AssetTypeModel.AssetType.Cashbox:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterMapCashBoxId == assetId);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            AreaId = areaId,
                            MeterMapCashBoxId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.Cashbox
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.Gateway:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterMapGatewayId == assetId);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            MeterId = (int)assetId,
                            AreaId = areaId,
                            MeterMapGatewayId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.Gateway
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.MultiSpaceMeter:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == assetId);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            AreaId = areaId,
                            MeterId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.MultiSpaceMeter
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.Sensor:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterMapSensorId == assetId);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            AreaId = areaId,
                            MeterId = (int)assetId,
                            MeterMapSensorId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.Sensor
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.SingleSpaceMeter:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == assetId);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            AreaId = areaId,
                            MeterId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.SingleSpaceMeter
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.ParkingSpaces: //** Sairam added on Oct 1st 2014
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AssetId == assetId && m.AssetType == (int)MeterGroups.Space);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            MeterId = 0,
                            AreaId = areaId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.Space
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.Mechanism:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AssetId == assetId && m.AssetType == (int)MeterGroups.Mechanism);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            MeterId = (int)assetId,
                            AreaId = areaId,
                            MeterMapMechId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.Mechanism
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
                case AssetTypeModel.AssetType.DataKey:
                    assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AssetId == assetId && m.AssetType == (int)MeterGroups.Datakey);
                    if (assetPending == null)
                    {
                        assetPending = new AssetPending()
                        {
                            CustomerId = customerId,
                            MeterId = 0,
                            AreaId = areaId,
                            MeterMapDataKeyId = (int)assetId,
                            AssetId = assetId,
                            AssetType = (int)MeterGroups.Datakey
                        };
                        PemsEntities.AssetPendings.Add(assetPending);
                    }
                    break;
            }

            if (assetPending == null) return;

            // Now update assetPending
            // TODO  Check: What are the correct settings for an imported asset in the AssetPending table.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            //As per  	DTPEMS-192 RecordMigrationDateTime changes to NOW
            // assetPending.RecordMigrationDateTime = SetToMidnight(activateDate.HasValue ? activateDate.Value : Now);
            assetPending.RecordMigrationDateTime = activateDate.HasValue ? activateDate.Value : Now;
            assetPending.AssetState = AssetState;
            assetPending.OperationalStatus = operationalstatus; //(int) OperationalStatusType.Operational;          
            assetPending.OperationalStatusTime = assetPending.RecordMigrationDateTime;

            PemsEntities.SaveChanges();
        }

        #endregion

        #region Meters

        public void Save(MeterEditModel model, MeterEditModel otherModel)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterId == model.AssetId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    MeterId = (int)model.AssetId,
                    AssetId = model.AssetId,
                    AssetType = (int)otherModel.TypeId
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Now save the actual data associated with the meter.

            // Asset model.
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.AssetName = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Street
            if (model.PropertyChanged(otherModel, "Street"))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Phone Number
            // if (model.PropertyChanged(otherModel, "PhoneNumber"))
            {
                assetPending.PhoneNumber = model.PhoneNumber;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.PropertyChanged(otherModel, "AreaListId"))
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "ZoneId"))
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "SuburbId"))
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Lat/Long
            if (model.PropertyChanged(otherModel, "Latitude"))
            {
                assetPending.Latitude = model.Latitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "Longitude"))
            {
                assetPending.Longitude = model.Longitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            {
                assetPending.NextPreventativeMaintenance = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "NextPrevMaint"))
                {
                    assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Maintenace Route
            if (model.MaintenanceRouteId > -1)
            {
                if (model.PropertyChanged(otherModel, "MaintenanceRouteId"))
                {
                    assetPending.MeterMapMaintenanceRoute = model.MaintenanceRouteId;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Warantry expiration
            if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            {
                assetPending.WarrantyExpiration = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
                {
                    assetPending.WarrantyExpiration = model.WarrantyExpiration;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            if (model.Configuration != null)
            {
                // Installation Date
                if (!model.Configuration.DateInstalled.HasValue || model.Configuration.DateInstalled == DateTime.MinValue)
                {
                    assetPending.DateInstalled = null;
                }
                else
                {
                    if (model.Configuration.DateInstalled != otherModel.Configuration.DateInstalled)
                    {
                        assetPending.DateInstalled = model.Configuration.DateInstalled;
                        changeType |= (int)AssetPendingReasonType.InfoOnly;
                    }
                }

                // Version profile information
                if (model.Configuration.MPVVersion != otherModel.Configuration.MPVVersion)
                {
                    assetPending.MPVFirmware = model.Configuration.MPVVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.SoftwareVersion != otherModel.Configuration.SoftwareVersion)
                {
                    assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.FirmwareVersion != otherModel.Configuration.FirmwareVersion)
                {
                    assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }
            }

            // Operational status of assetPending.
            //  if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            {
                assetPending.OperationalStatus = (int)Enum.Parse(typeof(OperationalStatusType), model.Status); // model.StatusId;
                assetPending.OperationalStatusTime = model.StatusDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;

            }


            if (model.Type == "Single Space Meter")
            {
                HousingMaster housingMaster = (from s in PemsEntities.HousingMasters
                                               where s.HousingName == model.LocationName && s.Customerid == model.CustomerId
                                               select s).FirstOrDefault();

                if (housingMaster == null)
                {
                    HousingMaster housingMasterObject = new HousingMaster();

                    housingMasterObject.Customerid = model.CustomerId;
                    housingMasterObject.HousingName = model.LocationName;
                    housingMasterObject.Block = "Default";
                    housingMasterObject.StreetName = "Default";
                    housingMasterObject.StreetType = "Default";
                    housingMasterObject.StreetDirection = "Default";
                    housingMasterObject.IsActive = false;
                    housingMasterObject.CreateDate = DateTime.Now;

                    PemsEntities.HousingMasters.Add(housingMasterObject);
                    PemsEntities.SaveChanges();

                }
                housingMaster = (from s in PemsEntities.HousingMasters
                                 where s.HousingName == model.LocationName && s.Customerid == model.CustomerId
                                 select s).FirstOrDefault();
                //Changed By Rajesh on October/20/2015
                if (housingMaster != null)
                {
                    housingMaster.Customerid = model.CustomerId;
                    if (model.Status == "Inactive")
                    {
                        housingMaster.IsActive = false;
                    }
                    else
                    {
                        housingMaster.IsActive = true;
                    }
                    PemsEntities.SaveChanges();
                }
                MechMaster mechMaster = new MechMaster();
                if (model.MechSerialNo != "" && model.MechSerialNo != null)
                {
                    model.MechSerialNo = model.MechSerialNo.Trim();
                    var mechMasterlist = (from s in PemsEntities.MechMasters
                                          where s.MechSerial == model.MechSerialNo //&& s.Customerid == model.CustomerId
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
                        //var MechMeter = (from s in PemsEntities.Meters
                        //                 where s.MeterId == mechmasterlistrow.MechIdNumber && s.MeterGroup == (int)MeterGroups.Mechanism //&& s.CustomerID == model.CustomerId
                        //                 select s).FirstOrDefault();

                        var mechMasterMeter = (from s in PemsEntities.MeterMaps
                                               where s.MechId == mechmasterlistrow.MechId && s.MeterId != model.AssetId  //&& s.Customerid == model.CustomerId// s.Areaid != MechMeter.AreaID//s.MeterId != mechMaster.MechIdNumber 
                                               select s).ToList();

                        if (mechMasterMeter != null)
                        {
                            mechMasterMeter.ForEach(a => a.MechId = null);
                            // mechMasterMeter.MechId = null;
                            PemsEntities.SaveChanges();
                        }
                    }

                    mechMaster = (from s in PemsEntities.MechMasters
                                  where s.MechSerial == model.MechSerialNo && s.Customerid == model.CustomerId
                                  select s).FirstOrDefault();

                    if (mechMaster == null) //&& mechMaster.Customerid != model.CustomerId)
                    {
                        mechMaster = (from s in PemsEntities.MechMasters
                                      where s.MechSerial == model.MechSerialNo
                                      select s).FirstOrDefault();

                        var MechMeter = (from s in PemsEntities.Meters
                                         where s.MeterId == mechMaster.MechIdNumber && s.MeterGroup == (int)MeterGroups.Mechanism //&& s.CustomerID == model.CustomerId
                                         select s).FirstOrDefault();

                        // mechMaster.MechSerial = null;
                        //  PemsEntities.SaveChanges();                     
                        Meter newMechMeter = CreateBaseAsset(model.CustomerId, MeterGroups.Mechanism);
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
                if (mechMaster.MechId != 0)
                {

                    assetPending.MeterMapMechId = mechMaster.MechId;
                }
                else
                {
                    assetPending.MeterMapMechId = null;
                }
                assetPending.MeterMapHousingId = housingMaster.HousingId;



            }


            // Save state selected
            assetPending.AssetState = model.StateId;

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();

            //** Sairam added on sep 23rd 2014
            //** The 'Meters' table need to be populated with the required fields. Saving the details entered during creation of ssm in meters table
            //** Santhosh commented ..it will do affter sp_assetpending job completion
            //var fetchAssetName = (from i in PemsEntities.Meters
            //                      where i.MeterId == model.AssetId && i.CustomerID == model.CustomerId && i.AreaID == model.AreaId
            //                      select i).FirstOrDefault();
            //fetchAssetName.MeterName = model.Name;
            //fetchAssetName.Latitude = model.Latitude;
            //fetchAssetName.Longitude = model.Longitude;
            //fetchAssetName.Location = model.Street;
            //fetchAssetName.MeterType = model.AssetModelId;

            //PemsEntities.SaveChanges();
        }

        public void Save(MeterMassEditModel model, int customerId, int areaId, int meterId)
        {
            int changeType = 0;
            Meter meter = null;


            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == customerId && m.AreaId == areaId && m.MeterId == meterId);
            if (assetPending == null)
            {
                meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == meterId);

                assetPending = new AssetPending()
                {
                    CustomerId = customerId,
                    AreaId = areaId,
                    MeterId = meterId,
                    AssetId = meterId,
                    AssetType = meter == null ? (int)MeterGroups.SingleSpaceMeter : (meter.MeterGroup ?? (int)MeterGroups.SingleSpaceMeter)
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;


            // Now save the actual data associated with the meter.

            // Street
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.AreaListId > -1)
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.ZoneId > -1)
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.SuburbId > -1)
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Maintenace Route
            if (model.MaintenanceRouteId > -1)
            {
                assetPending.MeterMapMaintenanceRoute = model.MaintenanceRouteId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Last maintenance date
            if (model.LastPrevMaint != null)
            {
                assetPending.LastPreventativeMaintenance = model.LastPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (model.NextPrevMaint != null)
            {
                assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Warantry expiration
            if (model.WarrantyExpiration != null)
            {
                assetPending.WarrantyExpiration = model.WarrantyExpiration;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Version profile information
            if (!string.IsNullOrWhiteSpace(model.Configuration.MPVVersion))
            {
                assetPending.MPVFirmware = model.Configuration.MPVVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.SoftwareVersion))
            {
                assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.FirmwareVersion))
            {
                assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            // See if new AssetState has been set. 
            if (model.StateId != -1)
            {
                if (meter == null)
                {
                    meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == areaId && m.MeterId == meterId);
                }

                // Check sensor to see if OperationalStatus == OperationalStatusType.Inactive and AssetState is being set to AssetStateType.Current
                // If so, set OperationalState to OperationalStatusType.Operational
                if ((!meter.OperationalStatusID.HasValue || meter.OperationalStatusID.Value == (int)OperationalStatusType.Inactive) && model.StateId == (int)AssetStateType.Current)
                {
                    assetPending.OperationalStatus = (int)OperationalStatusType.Operational;
                    assetPending.OperationalStatusTime = model.ActivationDate;
                }

                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }

        #endregion

        #region Gateways

        public void Save(GatewayEditModel model, GatewayEditModel otherModel)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapGatewayId == model.AssetId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    MeterId = (int)model.AssetId,
                    MeterMapGatewayId = (int)model.AssetId,
                    AssetId = model.AssetId,
                    AssetType = (int)model.TypeId
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Asset model.
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.AssetName = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Street
            if (model.PropertyChanged(otherModel, "Street"))
            {
                assetPending.LocationGateway = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.PropertyChanged(otherModel, "AreaListId"))
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.PropertyChanged(otherModel, "ZoneId"))
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.PropertyChanged(otherModel, "SuburbId"))
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Lat/Long
            if (model.PropertyChanged(otherModel, "Latitude"))
            {
                assetPending.Latitude = model.Latitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.PropertyChanged(otherModel, "Longitude"))
            {
                assetPending.Longitude = model.Longitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            {
                assetPending.NextPreventativeMaintenance = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "NextPrevMaint"))
                {
                    assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Maintenace Route
            if (model.MaintenanceRouteId > -1)
            {
                if (model.PropertyChanged(otherModel, "MaintenanceRouteId"))
                {
                    assetPending.MeterMapMaintenanceRoute = model.MaintenanceRouteId;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Warantry expiration
            if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            {
                assetPending.WarrantyExpiration = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
                {
                    assetPending.WarrantyExpiration = model.WarrantyExpiration;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            if (model.Configuration != null)
            {
                // Installation Date
                if (!model.Configuration.DateInstalled.HasValue || model.Configuration.DateInstalled == DateTime.MinValue)
                {
                    assetPending.DateInstalled = null;
                }
                else
                {
                    if (model.Configuration.DateInstalled != otherModel.Configuration.DateInstalled)
                    {
                        assetPending.DateInstalled = model.Configuration.DateInstalled;
                        changeType |= (int)AssetPendingReasonType.InfoOnly;
                    }
                }

                // Version profile information
                if (model.Configuration.MPVVersion != otherModel.Configuration.MPVVersion)
                {
                    assetPending.MPVFirmware = model.Configuration.MPVVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.SoftwareVersion != otherModel.Configuration.SoftwareVersion)
                {
                    assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.FirmwareVersion != otherModel.Configuration.FirmwareVersion)
                {
                    assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }
            }

            // Operational status of assetPending.
            if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            {
                assetPending.OperationalStatus = model.StatusId;
                assetPending.OperationalStatusTime = model.StatusDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            assetPending.AssetState = model.StateId;

            // Save changes
            PemsEntities.SaveChanges();
        }

        public void Save(GatewayMassEditModel model, int customerId, int areaId, int gatewayId)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == areaId && m.MeterMapGatewayId == gatewayId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = areaId,
                    MeterId = gatewayId,
                    MeterMapGatewayId = gatewayId,
                    AssetId = gatewayId,
                    AssetType = (int)MeterGroups.Gateway

                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Now save the actual data associated with the gateway.

            // Street
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.AreaListId > -1)
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.ZoneId > -1)
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.SuburbId > -1)
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Maintenace Route
            if (model.MaintenanceRouteId > -1)
            {
                assetPending.MeterMapMaintenanceRoute = model.MaintenanceRouteId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Last maintenance date
            if (model.LastPrevMaint != null)
            {
                assetPending.LastPreventativeMaintenance = model.LastPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (model.NextPrevMaint != null)
            {
                assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Warantry expiration
            if (model.WarrantyExpiration != null)
            {
                assetPending.WarrantyExpiration = model.WarrantyExpiration;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Version profile information
            if (!string.IsNullOrWhiteSpace(model.Configuration.MPVVersion))
            {
                assetPending.MPVFirmware = model.Configuration.MPVVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.SoftwareVersion))
            {
                assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.FirmwareVersion))
            {
                assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            // See if new AssetState has been set. 
            if (model.StateId != -1)
            {
                var gateway = PemsEntities.Gateways.FirstOrDefault(m => m.CustomerID == customerId && m.GateWayID == gatewayId);

                // Check sensor to see if OperationalStatus == OperationalStatusType.Inactive and AssetState is being set to AssetStateType.Current
                // If so, set OperationalState to OperationalStatusType.Operational
                if ((!gateway.OperationalStatus.HasValue || gateway.OperationalStatus.Value == (int)OperationalStatusType.Inactive) && model.StateId == (int)AssetStateType.Current)
                {
                    assetPending.OperationalStatus = (int)OperationalStatusType.Operational;
                    assetPending.OperationalStatusTime = model.ActivationDate;
                }

                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }

        #endregion

        #region Sensors

        public void Save(SensorEditModel model, SensorEditModel otherModel)
        {
            int changeType = 0;

            //making deallocating ParkingSpaceId 
            var oldAssest = PemsEntities.AssetPendings.FirstOrDefault(m => m.AssocidateSpaceId == model.AssociatedSpaceId);
            if (oldAssest != null)
            {
                oldAssest.AssocidateSpaceId = null;
                PemsEntities.SaveChanges();

            }

            //making deallocating ParkingSpaceId and sensor
            var oldSensor = (from m in PemsEntities.Sensors
                             where m.ParkingSpaceId == model.AssociatedSpaceId && m.CustomerID == model.CustomerId && m.SensorID != model.AssetId
                             select m).ToList();
            oldSensor.ForEach(a => a.ParkingSpaceId = null);
            PemsEntities.SaveChanges();


            var oldSensorMap = (from m in PemsEntities.SensorMappings
                                where m.ParkingSpaceID == model.AssociatedSpaceId && m.CustomerID == model.CustomerId && m.SensorID != model.AssetId
                                select m).ToList();
            oldSensorMap.ForEach(a => a.ParkingSpaceID = null);
            PemsEntities.SaveChanges();




            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapSensorId == model.AssetId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    MeterId = (int)model.AssetId,
                    MeterMapSensorId = (int)model.AssetId,
                    AssetId = model.AssetId,
                    AssetType = (int)model.TypeId
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Asset model.
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.AssetName = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            //// Street
            //if (model.PropertyChanged(otherModel, "Street"))
            //{
            //    assetPending.LocationSensors = model.Street;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Set Zone, Area and Suburb
            //if (model.PropertyChanged(otherModel, "AreaListId"))
            //{
            //    assetPending.MeterMapAreaId2 = model.AreaListId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}
            //if (model.PropertyChanged(otherModel, "ZoneId"))
            //{
            //    assetPending.MeterMapZoneId = model.ZoneId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}
            //if (model.PropertyChanged(otherModel, "SuburbId"))
            //{
            //    assetPending.MeterMapSuburbId = model.SuburbId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Lat/Long
            //if (model.PropertyChanged(otherModel, "Latitude"))
            //{
            //    assetPending.Latitude = model.Latitude;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}
            //if (model.PropertyChanged(otherModel, "Longitude"))
            //{
            //    assetPending.Longitude = model.Longitude;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}           
            assetPending.LocationSensors = model.Street;

            assetPending.MeterMapAreaId2 = model.AreaListId;

            assetPending.MeterMapZoneId = model.ZoneId;


            assetPending.MeterMapSuburbId = model.SuburbId;

            assetPending.Latitude = model.Latitude;

            assetPending.Longitude = model.Longitude;
            changeType |= (int)AssetPendingReasonType.InfoOnly;


            if (model.PropertyChanged(otherModel, "AssociatedSpaceId"))
            {
                //var parkingspaceID = (from s in PemsEntities.ParkingSpaces
                //       where s.ParkingSpaceId == model.AssociatedSpaceId && s.CustomerID == model.CustomerId
                //        select s).FirstOrDefault();

                assetPending.AssocidateSpaceId = model.AssociatedSpaceId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.PropertyChanged(otherModel, "AssociatedMeterId"))
            {

                assetPending.AssociatedMeterId = model.AssociatedMeterId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            {
                assetPending.NextPreventativeMaintenance = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "NextPrevMaint"))
                {
                    assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Maintenace Route
            if (model.MaintenanceRouteId > -1)
            {
                if (model.PropertyChanged(otherModel, "MaintenanceRouteId"))
                {
                    assetPending.MeterMapMaintenanceRoute = model.MaintenanceRouteId;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Warantry expiration
            if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            {
                assetPending.WarrantyExpiration = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
                {
                    assetPending.WarrantyExpiration = model.WarrantyExpiration;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            if (model.Configuration != null)
            {
                // Installation Date
                if (!model.Configuration.DateInstalled.HasValue || model.Configuration.DateInstalled == DateTime.MinValue)
                {
                    assetPending.DateInstalled = null;
                }
                else
                {
                    if (model.Configuration.DateInstalled != otherModel.Configuration.DateInstalled)
                    {
                        assetPending.DateInstalled = model.Configuration.DateInstalled;
                        changeType |= (int)AssetPendingReasonType.InfoOnly;
                    }
                }

                // Version profile information
                if (model.Configuration.MPVVersion != otherModel.Configuration.MPVVersion)
                {
                    assetPending.MPVFirmware = model.Configuration.MPVVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.SoftwareVersion != otherModel.Configuration.SoftwareVersion)
                {
                    assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.FirmwareVersion != otherModel.Configuration.FirmwareVersion)
                {
                    assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }
            }

            // Set primary Gateway
            var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.SensorID == model.AssetId);

            SensorMapping currentSensorMapping;
            SensorMapping pendingSensorMapping;
            if (model.Configuration.PrimaryGatewayId > -1)
            {
                if (model.Configuration.PrimaryGatewayId != otherModel.Configuration.PrimaryGatewayId)
                {
                    // Write the pending PrimaryGateway 
                    assetPending.PrimaryGateway = model.Configuration.PrimaryGatewayId;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;

                    // Get existing current primary gateway if there is one.
                    currentSensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                                                                                      && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);

                    // Get existing pending primary gateway if there is one.
                    pendingSensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                                                                                      && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Pending);

                    // Is the current or pending primary gateway already the selected gateway?  If so, do nothing.
                    // Otherwise create a pending record.
                    if (!((currentSensorMapping != null && currentSensorMapping.GatewayID == model.Configuration.PrimaryGatewayId)
                            ||
                            (pendingSensorMapping != null && pendingSensorMapping.GatewayID == model.Configuration.PrimaryGatewayId)
                          ))
                    {
                        // Add a pending SensorMapping
                        pendingSensorMapping = new SensorMapping()
                        {
                            CustomerID = model.CustomerId,
                            SensorID = sensor.SensorID,
                            GatewayID = model.Configuration.PrimaryGatewayId,
                            IsPrimaryGateway = true,
                            MappingState = (int)AssetStateType.Pending

                        };
                        PemsEntities.SensorMappings.Add(pendingSensorMapping);
                        Audit(pendingSensorMapping);
                    }
                }
            }

            // Set secondary Gateway
            if (model.Configuration.SecondaryGatewayId > -1)
            {
                if (model.Configuration.SecondaryGatewayId != otherModel.Configuration.SecondaryGatewayId)
                {
                    // Write the pending SecondaryGateway 
                    assetPending.SecondaryGateway = model.Configuration.SecondaryGatewayId;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;

                    // Get existing current secondary gateway if there is one.
                    currentSensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                                                                                      && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);

                    // Get existing pending secondary gateway if there is one.
                    pendingSensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                                                                                      && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Pending);

                    // Is the current or pending secondary gateway already the selected gateway?  If so, do nothing.
                    // Otherwise create a pending record.
                    if (!((currentSensorMapping != null && currentSensorMapping.GatewayID == model.Configuration.SecondaryGatewayId)
                            ||
                            (pendingSensorMapping != null && pendingSensorMapping.GatewayID == model.Configuration.SecondaryGatewayId)
                          ))
                    {
                        // Add a pending SensorMapping
                        pendingSensorMapping = new SensorMapping()
                        {
                            CustomerID = model.CustomerId,
                            SensorID = sensor.SensorID,
                            GatewayID = model.Configuration.SecondaryGatewayId,
                            IsPrimaryGateway = false,
                            MappingState = (int)AssetStateType.Pending
                        };
                        PemsEntities.SensorMappings.Add(pendingSensorMapping);
                        Audit(pendingSensorMapping);
                    }
                }
            }
            if (model.AssociatedSpaceId > 0)
            {
                var space = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == model.AssociatedSpaceId);
                if (space != null)
                {
                    space.HasSensor = true;
                    PemsEntities.SaveChanges();
                    Audit(space);
                }
            }

            // Operational status of assetPending.
            if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            {
                assetPending.OperationalStatus = model.StatusId;
                assetPending.OperationalStatusTime = model.StatusDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            assetPending.AssetState = model.StateId;



            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }

        public void Save(SensorMassEditModel model, int customerId, int areaId, int sensorId)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == areaId && m.MeterMapSensorId == sensorId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = areaId,
                    MeterId = sensorId,
                    MeterMapSensorId = sensorId,
                    AssetId = sensorId,
                    AssetType = (int)MeterGroups.Sensor
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Now save the actual data associated with the meter.

            // Street
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.AreaListId > -1)
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.ZoneId > -1)
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.SuburbId > -1)
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Maintenace Route
            if (model.MaintenanceRouteId > -1)
            {
                assetPending.MeterMapMaintenanceRoute = model.MaintenanceRouteId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            // Last maintenance date
            if (model.LastPrevMaint != null)
            {
                assetPending.LastPreventativeMaintenance = model.LastPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (model.NextPrevMaint != null)
            {
                assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Warantry expiration
            if (model.WarrantyExpiration != null)
            {
                assetPending.WarrantyExpiration = model.WarrantyExpiration;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Version profile information
            if (!string.IsNullOrWhiteSpace(model.Configuration.MPVVersion))
            {
                assetPending.MPVFirmware = model.Configuration.MPVVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.SoftwareVersion))
            {
                assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.FirmwareVersion))
            {
                assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            // Primary and secondary gateways
            var sensor = PemsEntities.Sensors.FirstOrDefault(m => m.CustomerID == customerId && m.SensorID == sensorId);

            SensorMapping currentSensorMapping;
            if (model.Configuration.PrimaryGatewayId > -1)
            {
                // Save Primary gateway
                assetPending.PrimaryGateway = model.Configuration.PrimaryGatewayId;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;

                // Get existing current primary gateway if there is one.
                currentSensorMapping = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                    && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);

                // Get existing pending primary gateway if there is one.
                SensorMapping primaryGatewayMap = sensor.SensorMappings.FirstOrDefault(m => m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                                                                                            && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Pending);

                // Is the current or pending primary gateway already the selected gateway?  If so, do nothing.
                // Otherwise create a pending record.
                if (!((currentSensorMapping != null && currentSensorMapping.GatewayID == model.Configuration.PrimaryGatewayId)
                          ||
                       (primaryGatewayMap != null && primaryGatewayMap.GatewayID == model.Configuration.PrimaryGatewayId)
                 ))
                {
                    // Add a pending SensorMapping
                    primaryGatewayMap = new SensorMapping()
                    {
                        CustomerID = model.CustomerId,
                        SensorID = sensor.SensorID,
                        GatewayID = model.Configuration.PrimaryGatewayId,
                        IsPrimaryGateway = true,
                        MappingState = (int)AssetStateType.Pending
                    };
                    PemsEntities.SensorMappings.Add(primaryGatewayMap);
                }
            }

            // Set secondary Gateway
            if (model.Configuration.SecondaryGatewayId > -1)
            {
                // Save secondary gateway
                assetPending.SecondaryGateway = model.Configuration.SecondaryGatewayId;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;

                // Get existing current secondary gateway if there is one.
                currentSensorMapping = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                    && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Current);

                // Get existing pending secondary gateway if there is one.
                SensorMapping secondaryGatewayMap = sensor.SensorMappings.FirstOrDefault(m => !m.IsPrimaryGateway && m.CustomerID == model.CustomerId
                                                                                              && m.SensorID == sensor.SensorID && m.MappingState == (int)AssetStateType.Pending);

                // Is the current or pending secondary gateway already the selected gateway?  If so, do nothing.
                // Otherwise create a pending record.
                if (!((currentSensorMapping != null && currentSensorMapping.GatewayID == model.Configuration.SecondaryGatewayId)
                          ||
                       (secondaryGatewayMap != null && secondaryGatewayMap.GatewayID == model.Configuration.SecondaryGatewayId)
                 ))
                {
                    // Add a pending SensorMapping
                    secondaryGatewayMap = new SensorMapping()
                    {
                        CustomerID = model.CustomerId,
                        SensorID = sensor.SensorID,
                        GatewayID = model.Configuration.SecondaryGatewayId,
                        IsPrimaryGateway = false,
                        MappingState = (int)AssetStateType.Pending
                    };
                    PemsEntities.SensorMappings.Add(secondaryGatewayMap);
                    Audit(secondaryGatewayMap);
                }
            }

            // See if new AssetState has been set. 
            if (model.StateId != -1)
            {
                // Check sensor to see if OperationalStatus == OperationalStatusType.Inactive and AssetState is being set to AssetStateType.Current
                // If so, set OperationalState to OperationalStatusType.Operational
                if ((!sensor.OperationalStatus.HasValue || sensor.OperationalStatus.Value == (int)OperationalStatusType.Inactive) && model.StateId == (int)AssetStateType.Current)
                {
                    assetPending.OperationalStatus = (int)OperationalStatusType.Operational;
                    assetPending.OperationalStatusTime = model.ActivationDate;
                }

                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }


        #endregion

        #region Cashboxes

        public void Save(CashboxEditModel model, CashboxEditModel otherModel)
        {
            int changeType = 0;


            var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.CashBoxSeq == model.AssetId);
            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapCashBoxId == cashbox.CashBoxID);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    MeterId = 0,
                    MeterMapCashBoxId = cashbox.CashBoxID,
                    AssetId = cashbox.CashBoxID,
                    AssetType = (int)model.TypeId

                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Asset model.
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.AssetName = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Location
            if (model.PropertyChanged(otherModel, "LocationId"))
            {
                assetPending.CashboxLocationId = model.LocationId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            {
                assetPending.NextPreventativeMaintenance = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "NextPrevMaint"))
                {
                    assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Warantry expiration
            if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            {
                assetPending.WarrantyExpiration = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
                {
                    assetPending.WarrantyExpiration = model.WarrantyExpiration;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }


            // Operational status of assetPending.
            if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            {
                assetPending.OperationalStatus = model.StatusId;
                assetPending.OperationalStatusTime = model.StatusDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            // Asset is pending?
            assetPending.AssetState = model.StateId;

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            PemsEntities.SaveChanges();
        }

        public void Save(CashboxMassEditModel model, int customerId, int areaId, int cashboxId)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == areaId && m.MeterMapCashBoxId == cashboxId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = areaId,
                    MeterId = cashboxId,
                    MeterMapCashBoxId = cashboxId,
                    AssetId = cashboxId,
                    AssetType = (int)MeterGroups.Cashbox

                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Location
            if (model.LocationId >= 0)
            {
                assetPending.CashboxLocationId = model.LocationId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Last maintenance date
            if (model.LastPrevMaint != null)
            {
                assetPending.LastPreventativeMaintenance = model.LastPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (model.NextPrevMaint != null)
            {
                assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Warantry expiration
            if (model.WarrantyExpiration != null)
            {
                assetPending.WarrantyExpiration = model.WarrantyExpiration;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // See if new AssetState has been set. 
            if (model.StateId != -1)
            {
                var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == customerId && m.CashBoxID == cashboxId);

                // Check sensor to see if OperationalStatus == OperationalStatusType.Inactive and AssetState is being set to AssetStateType.Current
                // If so, set OperationalState to OperationalStatusType.Operational
                if ((!cashbox.OperationalStatus.HasValue || cashbox.OperationalStatus.Value == (int)OperationalStatusType.Inactive) && model.StateId == (int)AssetStateType.Current)
                {
                    assetPending.OperationalStatus = (int)OperationalStatusType.Operational;
                    assetPending.OperationalStatusTime = model.ActivationDate;
                }

                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            PemsEntities.SaveChanges();
        }

        #endregion

        #region Spaces

        public void Save(SpaceEditModel model, SpaceEditModel otherModel)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == model.AssetId && m.AssetType == (int)MeterGroups.Space);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    // MeterId = 0,
                    MeterId = model.AssociatedMeterId,
                    AssetId = model.AssetId,
                    AssetType = (int)model.TypeId
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.DisplaySpaceNum = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Parking space "Model"
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            // Street
            // TODO: Does the associated meter get updated?
            //parkingSpace.Location = editModel.Street;

            // Set Zone and Suburb
            // Set Zone, Area and Suburb
            if (model.PropertyChanged(otherModel, "AreaListId"))
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "ZoneId"))
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "SuburbId"))
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Lat/Long
            if (model.PropertyChanged(otherModel, "Latitude"))
            {
                assetPending.Latitude = model.Latitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "Longitude"))
            {
                assetPending.Longitude = model.Longitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Demand Zone
            if (model.PropertyChanged(otherModel, "DemandStatusId"))
            {
                assetPending.DemandStatus = model.DemandStatusId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Associated meter. Note:  This is a required field and will not be equal to 0.
            if (model.PropertyChanged(otherModel, "AssociatedMeterId"))
            {
                assetPending.AssociatedMeterId = model.AssociatedMeterId;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            // Associated sensor. A space can have a sensor or no sensor.
            if (model.PropertyChanged(otherModel, "AssociatedSensorId"))
            {
                assetPending.MeterMapSensorId = model.AssociatedSensorId;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            // Is asset active?
            if (model.PropertyChanged(otherModel, "StateId"))
            {
                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save changes
            PemsEntities.SaveChanges();
        }

        public void Save(SpaceMassEditModel model, int customerId, Int64 spaceId)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == spaceId && m.AssetType == (int)MeterGroups.Space);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = 0,
                    MeterId = 0,
                    AssetId = spaceId,
                    AssetType = (int)MeterGroups.Space
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Save remaining fields if required.

            // Street
            if (!string.IsNullOrWhiteSpace(model.Street))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            // Set Zone, Area and Suburb
            if (model.AreaListId > -1)
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.ZoneId > -1)
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            if (model.SuburbId > -1)
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Demand Status
            if (model.DemandStatusId > -1)
            {
                assetPending.DemandStatus = model.DemandStatusId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // See if new AssetState has been set. 
            if (model.StateId != -1)
            {
                var space = PemsEntities.ParkingSpaces.FirstOrDefault(m => m.ParkingSpaceId == spaceId);

                // Check sensor to see if OperationalStatus == OperationalStatusType.Inactive and AssetState is being set to AssetStateType.Current
                // If so, set OperationalState to OperationalStatusType.Operational
                if ((!space.OperationalStatus.HasValue || space.OperationalStatus.Value == (int)OperationalStatusType.Inactive) && model.StateId == (int)AssetStateType.Current)
                {
                    assetPending.OperationalStatus = (int)OperationalStatusType.Operational;
                    assetPending.OperationalStatusTime = model.ActivationDate;
                }

                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save changes
            PemsEntities.SaveChanges();
        }

        #endregion

        #region Mechanisms
        /// <summary>
        /// saves the pending mechanism to the system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="otherModel"></param>
        public void Save(MechanismEditModel model, MechanismEditModel otherModel)
        {
            #region Comment 22 Sept
            //int changeType = 0;

            //// If there is a pending record then use it else make one.

            //var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Mechanism && m.MeterId == model.MechIdNumber);
            //if (assetPending == null)
            //{
            //    assetPending = new AssetPending()
            //    {
            //        CustomerId = model.CustomerId,
            //        AreaId = model.AreaId,
            //        MeterMapMechId = (int) model.AssetId,
            //        MeterId = (int)model.AssetId,
            //        AssetId = model.AssetId,
            //        AssetType = (int)MeterGroups.Mechanism
            //    };
            //    PemsEntities.AssetPendings.Add(assetPending);
            //}


            //// Save pertinent information.
            //assetPending.CreateUserId = WebSecurity.CurrentUserId;
            //assetPending.RecordCreateDateTime = Now;
            //assetPending.RecordMigrationDateTime = model.ActivationDate;

            //// Now save the actual data associated with the meter.

            ////serial number
            //if (model.PropertyChanged(otherModel, "SerialNumber"))
            //{
            //    assetPending.MechSerialNumber = model.SerialNumber;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}


            //// Asset model.
            //if (model.PropertyChanged(otherModel, "AssetModelId"))
            //{
            //    assetPending.AssetModel = model.AssetModelId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Asset name
            //if (model.PropertyChanged(otherModel, "Name"))
            //{
            //    assetPending.AssetName = model.Name;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Street
            //if (model.PropertyChanged(otherModel, "Street"))
            //{
            //    assetPending.LocationMeters = model.Street;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Set Zone, Area and Suburb
            //if (model.PropertyChanged(otherModel, "AreaListId"))
            //{
            //    assetPending.MeterMapAreaId2 = model.AreaListId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}
            //if (model.PropertyChanged(otherModel, "ZoneId"))
            //{
            //    assetPending.MeterMapZoneId = model.ZoneId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}
            //if (model.PropertyChanged(otherModel, "SuburbId"))
            //{
            //    assetPending.MeterMapSuburbId = model.SuburbId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Lat/Long
            //if (model.PropertyChanged(otherModel, "Latitude"))
            //{
            //    assetPending.Latitude = model.Latitude;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}
            //if (model.PropertyChanged(otherModel, "Longitude"))
            //{
            //    assetPending.Longitude = model.Longitude;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Preventative maintenance date
            //if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            //{
            //    assetPending.NextPreventativeMaintenance = null;
            //}
            //else
            //{
            //    if (model.PropertyChanged(otherModel, "NextPrevMaint"))
            //    {
            //        assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
            //        changeType |= (int)AssetPendingReasonType.InfoOnly;
            //    }
            //}

            //// Warantry expiration
            //if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            //{
            //    assetPending.WarrantyExpiration = null;
            //}
            //else
            //{
            //    if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
            //    {
            //        assetPending.WarrantyExpiration = model.WarrantyExpiration;
            //        changeType |= (int)AssetPendingReasonType.InfoOnly;
            //    }
            //}
            //if (model.PropertyChanged(otherModel, "InstallationDate"))
            //{
            //    assetPending.DateInstalled = model.InstallationDate;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}


            //if (model.Configuration != null)
            //{
            //    // Installation Date
            //    if (!model.Configuration.DateInstalled.HasValue || model.Configuration.DateInstalled == DateTime.MinValue)
            //    {
            //        assetPending.DateInstalled = null;
            //    }
            //    else
            //    {
            //        if (model.Configuration.DateInstalled != otherModel.Configuration.DateInstalled)
            //        {
            //            assetPending.DateInstalled = model.Configuration.DateInstalled;
            //            changeType |= (int)AssetPendingReasonType.InfoOnly;
            //        }
            //    }

            //    // Version profile information
            //    if (model.Configuration.MPVVersion != otherModel.Configuration.MPVVersion)
            //    {
            //        assetPending.MPVFirmware = model.Configuration.MPVVersion;
            //        changeType |= (int)AssetPendingReasonType.ConfigOnly;
            //    }

            //    if (model.Configuration.SoftwareVersion != otherModel.Configuration.SoftwareVersion)
            //    {
            //        assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
            //        changeType |= (int)AssetPendingReasonType.ConfigOnly;
            //    }

            //    if (model.Configuration.FirmwareVersion != otherModel.Configuration.FirmwareVersion)
            //    {
            //        assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
            //        changeType |= (int)AssetPendingReasonType.ConfigOnly;
            //    }
            //}

            //// Operational status of assetPending.
            //if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            //{
            //    assetPending.OperationalStatus = model.StatusId;
            //    assetPending.OperationalStatusTime = model.StatusDate;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Save state selected
            //// Is asset active?
            //if (model.PropertyChanged(otherModel, "StateId"))
            //{
            //    assetPending.AssetState = model.StateId;
            //    changeType |= (int)AssetPendingReasonType.InfoOnly;
            //}

            //// Save change reason
            //assetPending.AssetPendingReasonId = changeType;

            //// Save any changes
            //PemsEntities.SaveChanges();
            #endregion

            int changeType = 0;

            // If there is a pending record then use it else make one.

            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Mechanism && m.MeterId == model.AssetId);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    MeterMapMechId = (int)model.AssetId,
                    MeterId = (int)model.AssetId,
                    AssetId = model.AssetId,
                    AssetType = (int)MeterGroups.Mechanism
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }


            //todo - GTC: Mechanism work - make sure these values being added to asset pending are correct for mechanism
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Now save the actual data associated with the meter.

            //serial number
            if (model.PropertyChanged(otherModel, "SerialNumber"))
            {
                assetPending.MechSerialNumber = model.SerialNumber;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            // Asset model.
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            assetPending.AssetModel = model.AssetModelId;  //** Sairam added on 21st sep 2014

            //todo - GTC: Mechanism work -  duncan to add the name to the mechmaster table, still todo. currently "Name" doesnt exist
            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.AssetName = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            assetPending.AssetName = model.Name; //****************** sairam finally added on 21st sep 2014
            // Street
            if (model.PropertyChanged(otherModel, "Street"))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.PropertyChanged(otherModel, "AreaListId"))
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "ZoneId"))
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "SuburbId"))
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Lat/Long
            if (model.PropertyChanged(otherModel, "Latitude"))
            {
                assetPending.Latitude = model.Latitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "Longitude"))
            {
                assetPending.Longitude = model.Longitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            {
                assetPending.NextPreventativeMaintenance = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "NextPrevMaint"))
                {
                    assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Warantry expiration
            if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            {
                assetPending.WarrantyExpiration = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
                {
                    assetPending.WarrantyExpiration = model.WarrantyExpiration;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }
            if (model.PropertyChanged(otherModel, "InstallationDate"))
            {
                assetPending.DateInstalled = model.InstallationDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            if (model.Configuration != null)
            {
                // Installation Date
                if (!model.Configuration.DateInstalled.HasValue || model.Configuration.DateInstalled == DateTime.MinValue)
                {
                    assetPending.DateInstalled = null;
                }
                else
                {
                    if (model.Configuration.DateInstalled != otherModel.Configuration.DateInstalled)
                    {
                        assetPending.DateInstalled = model.Configuration.DateInstalled;
                        changeType |= (int)AssetPendingReasonType.InfoOnly;
                    }
                }

                // Version profile information
                if (model.Configuration.MPVVersion != otherModel.Configuration.MPVVersion)
                {
                    assetPending.MPVFirmware = model.Configuration.MPVVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.SoftwareVersion != otherModel.Configuration.SoftwareVersion)
                {
                    assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.FirmwareVersion != otherModel.Configuration.FirmwareVersion)
                {
                    assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }
            }

            // Operational status of assetPending.
            if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            {
                assetPending.OperationalStatus = model.StatusId;
                assetPending.OperationalStatusTime = model.StatusDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save state selected
            // Is asset active?
            if (model.PropertyChanged(otherModel, "StateId"))
            {
                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();

            //** Sairam added on sep 21st 
            // it will do after asset pending job completion
            //var fetchAssetName = (from i in PemsEntities.Meters
            //                      where i.MeterId == model.AssetId && i.CustomerID == model.CustomerId && i.AreaID== model.AreaId
            //                      select i).FirstOrDefault();
            //fetchAssetName.MeterName = model.Name;
            //PemsEntities.SaveChanges();

        }

        public void Save(MechanismMassEditModel model, int customerId, int mechanismId)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == mechanismId && m.AssetType == (int)MeterGroups.Mechanism);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    MeterMapMechId = mechanismId,
                    AreaId = 0,
                    MeterId = 0,
                    AssetId = mechanismId,
                    AssetType = (int)MeterGroups.Mechanism
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            //todo - GTC: Mechanism work - verify that these properties are complete for mechnisms. might have to add name? if they can mass edit name.
            // Now save the actual data associated with the item.

            // Serial Number
            if (!string.IsNullOrWhiteSpace(model.SerialNumber))
            {
                assetPending.MechSerialNumber = model.SerialNumber;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            // Street
            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                assetPending.LocationMeters = model.Location;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Last maintenance date
            if (model.LastPrevMaint != null)
            {
                assetPending.LastPreventativeMaintenance = model.LastPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (model.NextPrevMaint != null)
            {
                assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Warantry expiration
            if (model.WarrantyExpiration != null)
            {
                assetPending.WarrantyExpiration = model.WarrantyExpiration;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.Configuration == null)
                model.Configuration = new AssetConfigurationModel();
            // Version profile information
            if (!string.IsNullOrWhiteSpace(model.Configuration.MPVVersion))
            {
                assetPending.MPVFirmware = model.Configuration.MPVVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.SoftwareVersion))
            {
                assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.FirmwareVersion))
            {
                assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }
            //we need to pass in the mechid and NOT the user defined (or system generated) asset iD, but the auto increment id
            //so, we have to go get the metermap,or the mechanism (mechid) cause that is where this id exists
            var mechanism = PemsEntities.MechMasters.FirstOrDefault(x => x.MechIdNumber == mechanismId);
            var mechId = -1;
            if (mechanism != null)
                mechId = mechanism.MechId;

            model.Location = GetMechanismLocation(customerId, mechId);

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }

        #endregion

        #region DataKeys
        /// <summary>
        /// saves the pending mechanism to the system.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="otherModel"></param>
        public void Save(DataKeyEditModel model, DataKeyEditModel otherModel)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.

            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.AssetType == (int)MeterGroups.Datakey && m.MeterId == model.DataKeyIdNumber);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    AreaId = model.AreaId,
                    MeterMapDataKeyId = (int)model.AssetId,
                    MeterId = (int)model.AssetId,
                    AssetId = model.AssetId,
                    AssetType = (int)MeterGroups.Datakey
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }


            //todo - GTC: DataKey work - make sure these values being added to asset pending are correct for datakey
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            // Now save the actual data associated with the meter.

            // Asset model.
            if (model.PropertyChanged(otherModel, "AssetModelId"))
            {
                assetPending.AssetModel = model.AssetModelId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            //todo - GTC: DataKey work -  duncan to add the name to the datakey table, still todo. currently "Name" doesnt exist
            // Asset name
            if (model.PropertyChanged(otherModel, "Name"))
            {
                assetPending.AssetName = model.Name;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Street
            if (model.PropertyChanged(otherModel, "Street"))
            {
                assetPending.LocationMeters = model.Street;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Set Zone, Area and Suburb
            if (model.PropertyChanged(otherModel, "AreaListId"))
            {
                assetPending.MeterMapAreaId2 = model.AreaListId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "ZoneId"))
            {
                assetPending.MeterMapZoneId = model.ZoneId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "SuburbId"))
            {
                assetPending.MeterMapSuburbId = model.SuburbId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Lat/Long
            if (model.PropertyChanged(otherModel, "Latitude"))
            {
                assetPending.Latitude = model.Latitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.PropertyChanged(otherModel, "Longitude"))
            {
                assetPending.Longitude = model.Longitude;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (!model.NextPrevMaint.HasValue || model.NextPrevMaint == DateTime.MinValue)
            {
                assetPending.NextPreventativeMaintenance = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "NextPrevMaint"))
                {
                    assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }

            // Warantry expiration
            if (!model.WarrantyExpiration.HasValue || model.WarrantyExpiration == DateTime.MinValue)
            {
                assetPending.WarrantyExpiration = null;
            }
            else
            {
                if (model.PropertyChanged(otherModel, "WarrantyExpiration"))
                {
                    assetPending.WarrantyExpiration = model.WarrantyExpiration;
                    changeType |= (int)AssetPendingReasonType.InfoOnly;
                }
            }
            if (model.PropertyChanged(otherModel, "InstallationDate"))
            {
                assetPending.DateInstalled = model.InstallationDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }


            if (model.Configuration != null)
            {
                // Installation Date
                if (!model.Configuration.DateInstalled.HasValue || model.Configuration.DateInstalled == DateTime.MinValue)
                {
                    assetPending.DateInstalled = null;
                }
                else
                {
                    if (model.Configuration.DateInstalled != otherModel.Configuration.DateInstalled)
                    {
                        assetPending.DateInstalled = model.Configuration.DateInstalled;
                        changeType |= (int)AssetPendingReasonType.InfoOnly;
                    }
                }

                // Version profile information
                if (model.Configuration.MPVVersion != otherModel.Configuration.MPVVersion)
                {
                    assetPending.MPVFirmware = model.Configuration.MPVVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.SoftwareVersion != otherModel.Configuration.SoftwareVersion)
                {
                    assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }

                if (model.Configuration.FirmwareVersion != otherModel.Configuration.FirmwareVersion)
                {
                    assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                    changeType |= (int)AssetPendingReasonType.ConfigOnly;
                }
            }

            // Operational status of assetPending.
            if (otherModel.StatusId == (int)OperationalStatusType.Inactive && model.StatusId != (int)OperationalStatusType.Inactive)
            {
                assetPending.OperationalStatus = model.StatusId;
                assetPending.OperationalStatusTime = model.StatusDate;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save state selected
            // Is asset active?
            if (model.PropertyChanged(otherModel, "StateId"))
            {
                assetPending.AssetState = model.StateId;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }

        public void Save(DataKeyMassEditModel model, int customerId, int mechanismId)
        {
            int changeType = 0;

            // If there is a pending record then use it else make one.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AssetId == mechanismId && m.AssetType == (int)MeterGroups.Datakey);
            if (assetPending == null)
            {
                assetPending = new AssetPending()
                {
                    CustomerId = model.CustomerId,
                    MeterMapDataKeyId = mechanismId,
                    AreaId = 0,
                    MeterId = 0,
                    AssetId = mechanismId,
                    AssetType = (int)MeterGroups.Datakey
                };
                PemsEntities.AssetPendings.Add(assetPending);
            }
            // Save pertinent information.
            assetPending.CreateUserId = WebSecurity.CurrentUserId;
            assetPending.RecordCreateDateTime = Now;
            assetPending.RecordMigrationDateTime = model.ActivationDate;

            //todo - GTC: DataKey work - verify that these properties are complete for DataKeys. might have to add name? if they can mass edit name.
            // Now save the actual data associated with the item.

            // Street
            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                assetPending.LocationMeters = model.Location;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Last maintenance date
            if (model.LastPrevMaint != null)
            {
                assetPending.LastPreventativeMaintenance = model.LastPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Preventative maintenance date
            if (model.NextPrevMaint != null)
            {
                assetPending.NextPreventativeMaintenance = model.NextPrevMaint;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }

            // Warantry expiration
            if (model.WarrantyExpiration != null)
            {
                assetPending.WarrantyExpiration = model.WarrantyExpiration;
                changeType |= (int)AssetPendingReasonType.InfoOnly;
            }
            if (model.Configuration == null)
                model.Configuration = new AssetConfigurationModel();
            // Version profile information
            if (!string.IsNullOrWhiteSpace(model.Configuration.MPVVersion))
            {
                assetPending.MPVFirmware = model.Configuration.MPVVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.SoftwareVersion))
            {
                assetPending.AssetSoftwareVersion = model.Configuration.SoftwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            if (!string.IsNullOrWhiteSpace(model.Configuration.FirmwareVersion))
            {
                assetPending.AssetFirmwareVersion = model.Configuration.FirmwareVersion;
                changeType |= (int)AssetPendingReasonType.ConfigOnly;
            }

            //we need to pass in the datakeyid and NOT the user defined (or system generated) asset iD, but the auto increment id
            //so, we have to go get the metermap,or the datakey (datakeyid) cause that is where this id exists
            var datakey = PemsEntities.DataKeys.FirstOrDefault(x => x.DataKeyIdNumber == mechanismId);
            var datakeyId = -1;
            if (datakey != null)
                datakeyId = datakey.DataKeyId;

            model.Location = GetMechanismLocation(customerId, datakeyId);

            // Save change reason
            assetPending.AssetPendingReasonId = changeType;

            // Save any changes
            PemsEntities.SaveChanges();
        }

        #endregion
    }
}
