/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________          ___________________        __________________________________________________________________________________________________
 * 12/20/2013       Sergey Ostrerov                 Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 * 
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
    /// Class encapsulating functionality for the cashbox asset type.
    /// </summary>
    public class CashboxFactory : AssetFactory
    {
        /// <summary>
        /// Factory constructor taking a connection string name and a time.
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
        public CashboxFactory(string connectionStringName, DateTime customerNow)
            : base(connectionStringName, customerNow)
        {
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
            //dont return anything if this is for occupancy
            if (gridType != AssetListGridType.Occupancy)
            {
                //NOTE: we are intentionally leaving some fields blank - per the spec.
                IQueryable<AssetListModel> items = PemsEntities.CashBoxes.Where(m => m.CustomerID == customerId).Select(
                    item =>
                    new AssetListModel
                    {
                        //----------DETAIL LINKS
                        Type = "Cashbox", //name of the details page (ViewCashbox)
                        AreaId = (int)AssetAreaId.Cashbox,
                        CustomerId = customerId,

                        //-----------FILTERABLE ITEMS
                        AssetType = item.MeterGroup == null ? CashboxViewModel.DefaultType : item.MeterGroup.MeterGroupDesc ?? CashboxViewModel.DefaultType,
                        AssetId = item.CashBoxSeq, //item.CashBoxID,
                        AssetName = item.CashBoxName,
                        InventoryStatus= item.CashBoxState == null ? "": item.AssetState.AssetStateDesc,
                        OperationalStatus = item.OperationalStatu == null ? " " : item.OperationalStatu.OperationalStatusDesc,
                        //Latitude
                        //Longitude
                        //AreaId2
                        //ZoneId
                        Suburb = "",
                        Street  = "",
                        DemandStatus= "",

                        //------------COMMON COLUMNS
                        AssetModel =item.MechanismMaster == null ? " " 
                            : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == item.MechanismMaster.MechanismId && m.IsDisplay) == null ? " "
                            : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == item.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc,

                        //------------SUMMARY GRID
                        //SpacesCount
                         Area = "",
                          Zone  ="",

                        //---------------CONFIGURATION GRID
                        DateInstalled = item.InstallDate,
                        //ConfigurationId
                        //ConfigCreationDate
                        //ConfigScheduleDate
                        //ConfigActivationDate
                        //MpvVersion
                        //SoftwareVersion
                        //FirmwareVersion

                        //-------------OCCUPANCY GRID - only valid for spaces
                        //MeterName
                        //SensorName
                        OperationalStatusDate= item.OperationalStatusTime,
                        //OccupancyStatus
                        //OccupancyStatusDate
                        //NonComplianceStatus
                        //NonComplianceStatusDate

                        //------------FUNCTIONAL STATUS GRID
                        //AlarmClass 
                        //AlarmCode
                        //AlarmTimeOfOccurance
                        //AlarmRepairTargetTime
                        LocalTime = Now
                    });
                return items;
            }
            return  new List<AssetListModel>().AsQueryable();
        }
       
        #endregion

        #region Get View Models

        /// <summary>
        /// Returns an instance of <see cref="CashboxViewModel"/> reflecting the present state
        /// and settings of a <see cref="CashBox"/> for customer id of <paramref name="customerId"/>
        /// and a cashbox id of <paramref name="cashboxId"/>
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="cashboxId">The cashbox id</param>
        /// <returns>An instance of <see cref="CashboxViewModel"/></returns>
        public CashboxViewModel GetViewModel(int customerId, int cashboxId)
        {
            var model = new CashboxViewModel { CustomerId = customerId, AreaId = 0, AssetId = cashboxId };

            var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == customerId && m.CashBoxSeq == cashboxId);

            model.GlobalId = cashbox.CashBoxID.ToString();

            model.Type = cashbox.MeterGroup == null ? CashboxViewModel.DefaultType : cashbox.MeterGroup.MeterGroupDesc ?? CashboxViewModel.DefaultType;
            model.TypeId = cashbox.CashBoxType ?? (int)MeterGroups.Cashbox;

            model.AssetModel = cashbox.MechanismMaster == null ? " " :
                PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == cashbox.MechanismMaster.MechanismId && m.IsDisplay) == null ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == cashbox.MechanismMaster.MechanismId && m.IsDisplay).MechanismDesc;
            model.Name = cashbox.CashBoxName ?? " ";

            model.Location = cashbox.CashBoxLocationType == null ? "" : cashbox.CashBoxLocationType.CashBoxLocationTypeDesc ?? "";


            // Current meter associated with cashbox
            var collDataSumm = PemsEntities.CollDataSumms
                .Where(m => m.NewCashBoxID == cashbox.CashBoxName && m.CustomerId == customerId).OrderBy(m => m.CollDateTime).FirstOrDefault();
            model.CurrentMeterName = collDataSumm == null ? "" : (collDataSumm.Meter == null ? "" : collDataSumm.Meter.Location ?? "");

            // Preventive maint. dates.
            model.LastPrevMaint = cashbox.LastPreventativeMaintenance.HasValue ?  cashbox.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = cashbox.NextPreventativeMaintenance.HasValue ? cashbox.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            // Warranty Expiration 
            model.WarrantyExpiration = cashbox.WarrantyExpiration.HasValue ? cashbox.WarrantyExpiration.Value.ToShortDateString() : "";

            // Cashbox install date.
            model.InstallDate = cashbox.InstallDate.HasValue ? cashbox.InstallDate.Value.ToShortDateString() : "";

            // Operational status of cashbox.
            model.Status = cashbox.OperationalStatu == null ? "" : cashbox.OperationalStatu.OperationalStatusDesc ?? "";
            model.StatusDate = cashbox.OperationalStatusTime;

            // Get last meter associated with this cash box.
            collDataSumm = PemsEntities.CollDataSumms
                .Where( m => m.OldCashBoxID == cashbox.CashBoxName && m.CustomerId == customerId ).OrderBy( m => m.CollDateTime ).FirstOrDefault();
            if (collDataSumm != null)
            {
                model.LastMeterId = collDataSumm.MeterId;
                model.LastCollected = collDataSumm.CollDateTime.ToString();
                model.LastCollectedValue = collDataSumm.Amount;
            }

            // See if cashbox has pending changes.
            model.HasPendingChanges = (new PendingFactory(ConnectionStringName, Now)).Pending(model);

            // Is asset active?
            model.State = (AssetStateType)cashbox.CashBoxState;

            // Get last update by information
            CreateAndLastUpdate(model);

            return model;
        }

        /// <summary>
        /// Returns an instance of <see cref="CashboxViewModel"/> reflecting the present and any pending state
        /// and settings of a <see cref="CashBox"/> for customer id of <paramref name="customerId"/>
        /// and a cashbox id of <paramref name="cashboxId"/>. 
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="cashboxId">The cashbox id</param>
        /// <returns>An instance of <see cref="CashboxViewModel"/></returns>
        public CashboxViewModel GetPendingViewModel(int customerId, int cashboxId)
        {
            CashboxViewModel model = GetViewModel(customerId, cashboxId);

            var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.CashBoxSeq == cashboxId);
            // Get the pending changes from [AssetPending] 
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapCashBoxId == cashbox.CashBoxID);

            if (assetPending.AssetModel.HasValue)
            {
                model.AssetModel = PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == assetPending.AssetModel.Value && m.IsDisplay) == null ? ""
                    : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.MechanismId == assetPending.AssetModel.Value && m.IsDisplay).MechanismDesc;
            }

            if (!string.IsNullOrWhiteSpace(assetPending.AssetName))
            {
                model.Name = assetPending.AssetName;
            }

            // Preventive maint. dates.
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

            // Cashbox install date.
            if (assetPending.DateInstalled.HasValue)
            {
                model.InstallDate = assetPending.DateInstalled.Value.ToShortDateString();
            }

            // Operational status of cashbox.
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
        /// Returns a <see cref="CashboxViewModel"/> based upon the indicated audit record id <paramref name="auditId"/>.
        /// </summary>
        /// <param name="customerId">The customer id (presently not used)</param>
        /// <param name="cashboxId">The cashbox id (presently not used)</param>
        /// <param name="auditId">The audit record id</param>
        /// <returns>An instance of <see cref="CashboxViewModel"/></returns>
        public CashboxViewModel GetHistoricViewModel(int customerId, int cashboxId, Int64 auditId)
        {
            return CreateHistoricViewModel(PemsEntities.CashBoxAudits.FirstOrDefault(m => m.CashBoxAuditId == auditId));
        }

        /// <summary>
        /// Private method to create a <see cref="CashboxViewModel"/> from a <see cref="CashBoxAudit"/> record.
        /// </summary>
        /// <param name="auditModel">An instance of <see cref="CashBoxAudit"/></param>
        /// <returns>An instance of <see cref="CashboxViewModel"/></returns>
        private CashboxViewModel CreateHistoricViewModel(CashBoxAudit auditModel)
        {
            var model = new CashboxViewModel()
            {
                CustomerId = auditModel.CustomerID,
                AreaId = 0,
                AssetId = auditModel.CashBoxID,
                GlobalId = auditModel.CashBoxID.ToString()
            };


            model.Type = auditModel.CashBoxType == null ? CashboxViewModel.DefaultType : PemsEntities.MeterGroups.FirstOrDefault(m => m.MeterGroupId == auditModel.CashBoxType).MeterGroupDesc ?? CashboxViewModel.DefaultType;
            model.TypeId = auditModel.CashBoxType ?? (int)MeterGroups.Cashbox;

            model.AssetModel = auditModel.CashBoxModel == null ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.MechanismId == auditModel.CashBoxModel && m.IsDisplay) == null ? ""
                : PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.MechanismId == auditModel.CashBoxModel && m.IsDisplay).MechanismDesc;

            model.Name = auditModel.CashBoxName ?? " ";

            model.Location = auditModel.CashBoxLocationTypeId == null
                                 ? ""
                                 : PemsEntities.CashBoxLocationTypes.FirstOrDefault(m => m.CashBoxLocationTypeId == auditModel.CashBoxLocationTypeId).CashBoxLocationTypeDesc ??
                                   "";

            // Note:  These values are not recorded in an audit table.
            model.CurrentMeterName = "";

            // Preventive maint. dates.
            model.LastPrevMaint = auditModel.LastPreventativeMaintenance.HasValue ? auditModel.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = auditModel.NextPreventativeMaintenance.HasValue ? auditModel.NextPreventativeMaintenance.Value.ToShortDateString() : "";

            // Warranty Expiration 
            model.WarrantyExpiration = auditModel.WarrantyExpiration.HasValue ? auditModel.WarrantyExpiration.Value.ToShortDateString() : "";

            // Cashbox install date.
            model.InstallDate = auditModel.InstallDate.HasValue ? auditModel.InstallDate.Value.ToShortDateString() : "";

            // Operational status of cashbox.
            model.Status = auditModel.OperationalStatus == null
                               ? ""
                               : PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == auditModel.OperationalStatus).OperationalStatusDesc ?? "";
            model.StatusDate = auditModel.OperationalStatusTime;

            // Get last meter associated with this cash box.
            // Note:  These values are not recorded in an audit table.
            model.LastMeterId = 0;
            model.LastCollected = null;
            model.LastCollectedValue = 0;

            // Asset state
            model.State = (AssetStateType)auditModel.CashBoxState;

            // Updated by
            model.LastUpdatedById = auditModel.UserId;
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReason = (AssetPendingReasonType)(auditModel.AssetPendingReasonId ?? 0);
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;

            // Record date
            model.RecordDate = auditModel.UpdateDateTime;

            // Audit record id
            model.AuditId = auditModel.CashBoxAuditId;

            return model;
        }

        /// <summary>
        /// Gets an <see cref="AssetHistoryModel"/> for a given <paramref name="customerId"/> and <paramref name="cashboxId"/>.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="cashboxId">The cashbox id</param>
        /// <returns>An instance of an <see cref="AssetHistoryModel"/></returns>
        public AssetHistoryModel GetHistoricListViewModel(int customerId, int cashboxId)
        {
            var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == customerId && m.CashBoxSeq == cashboxId);

            var model = new AssetHistoryModel()
                {
                    CustomerId = customerId, 
                    AssetId = cashboxId,
                    Type = cashbox == null ? CashboxViewModel.DefaultType : cashbox.MeterGroup == null ? CashboxViewModel.DefaultType : cashbox.MeterGroup.MeterGroupDesc ?? CashboxViewModel.DefaultType,
                    TypeId = cashbox == null ? (int)MeterGroups.Cashbox : cashbox.CashBoxType ?? (int)MeterGroups.Cashbox,
                    Name = cashbox == null ? "" : cashbox.CashBoxName ?? "",
                    Street = "-"
                };

            return model;
        }

        #endregion

        #region Get Edit Models

        /// <summary>
        /// Returns an instance of <see cref="CashboxEditModel"/> reflecting the present and any pending state
        /// and settings of a <see cref="CashBox"/> for customer id of <paramref name="customerId"/>
        /// and a cashbox id of <paramref name="cashboxId"/>. This model is used for edit page.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="cashboxId">The cashbox id</param>
        /// <returns>An instance of <see cref="CashboxEditModel"/></returns>
        public CashboxEditModel GetEditModel(int customerId, int cashboxId)
        {
            var cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == customerId && m.CashBoxSeq == cashboxId);

            var model = new CashboxEditModel
                {
                    CustomerId = customerId, 
                    AreaId = (int)AssetAreaId.Cashbox, 
                    AssetId =  cashboxId
                };


            // First build the edit model from the existing cashbox.
            #region Build model from a cashbox

            model.GlobalId = cashbox.CashBoxID.ToString();

            // Cashbox Type
            model.Type = cashbox.MeterGroup == null ? CashboxViewModel.DefaultType : cashbox.MeterGroup.MeterGroupDesc ?? CashboxViewModel.DefaultType;
            model.TypeId = (MeterGroups)(cashbox.CashBoxType ?? (int)MeterGroups.Cashbox);

            // Get sensor models.
            model.AssetModelId = cashbox.CashBoxModel ?? -1;
            GetAssetModelList(model, (int)MeterGroups.Cashbox);

            // Get meter name.
            model.Name = cashbox.CashBoxName ?? "";

            // Cashbox location
            model.LocationId = cashbox.CashBoxLocationTypeId ?? -1;

            // Preventative maintenance dates
            model.LastPrevMaint = cashbox.LastPreventativeMaintenance.HasValue ? cashbox.LastPreventativeMaintenance.Value.ToShortDateString() : "";
            model.NextPrevMaint = cashbox.NextPreventativeMaintenance.HasValue ? cashbox.NextPreventativeMaintenance.Value : (DateTime?)null;

            // Warantry expiration
            model.WarrantyExpiration = cashbox.WarrantyExpiration.HasValue ? cashbox.WarrantyExpiration.Value : (DateTime?)null;

            // Operational status of meter.
            GetOperationalStatusDetails(model, cashbox.OperationalStatus ?? 0, cashbox.OperationalStatusTime.HasValue ? cashbox.OperationalStatusTime.Value : DateTime.Now);
//            GetOperationalStatusList(model, cashbox.OperationalStatus ?? -1);

            // Is asset active?
            model.StateId = cashbox.CashBoxState;

            #endregion

          
            // If there is an asset pending record then update edit model with associated values.
            var assetPending = PemsEntities.AssetPendings.FirstOrDefault(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterMapGatewayId == cashbox.CashBoxID);

            if ( assetPending != null )
            {
                #region Update model from an assetPending

                // Get cashbox models.
                model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;

                // Get cashbox name.
                model.Name = string.IsNullOrWhiteSpace(assetPending.AssetName) ? model.Name : assetPending.AssetName;

                // Get location
                model.LocationId = assetPending.CashboxLocationId ?? model.LocationId;

                // Preventative maintenance dates
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.HasValue ? assetPending.LastPreventativeMaintenance.Value.ToShortDateString() : model.LastPrevMaint;
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance ?? model.NextPrevMaint;

                // Warantry expiration
                model.WarrantyExpiration = assetPending.WarrantyExpiration ?? model.WarrantyExpiration;

                // Operational status of cashbox.
                if ( assetPending.OperationalStatus != null )
                {
                    GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);
                }
//                GetOperationalStatusList(model, (int)(assetPending.OperationalStatus ?? cashbox.OperationalStatus));

                // Asset state is pending
                model.StateId = (int)AssetStateType.Pending;

                // Activation Date
                model.ActivationDate = assetPending.RecordMigrationDateTime;

                #endregion
            }

            // Possible cashbox locations
            model.Location = (from cbl in PemsEntities.CashBoxLocationTypes
                              select new SelectListItemWrapper()
                              {
                                  Selected = cbl.CashBoxLocationTypeId == model.LocationId,
                                  Text = cbl.CashBoxLocationTypeDesc,
                                  ValueInt = cbl.CashBoxLocationTypeId
                              }).ToList();
            model.Location.Insert(0, new SelectListItemWrapper()
            {
                Selected = model.LocationId == -1,
                Text = "",
                Value = "-1"
            });


            GetAssetStateList(model);

            // Initialize ActivationDate if needed and set to midnight.
            if ( !model.ActivationDate.HasValue )
            {
                model.ActivationDate = Now;
            }
            model.ActivationDate = SetToMidnight( model.ActivationDate.Value );

            return model;
        }

        /// <summary>
        /// Refreshes any lists in the <see cref="CashboxEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="CashboxEditModel"/> to refresh</param>
        public void RefreshEditModel(CashboxEditModel editModel)
        {
            // Get sensor models.
            GetAssetModelList(editModel, (int)MeterGroups.Cashbox);

            // Cashbox location
            editModel.Location = (from cbl in PemsEntities.CashBoxLocationTypes
                                  select new SelectListItemWrapper()
                                  {
                                      Selected = cbl.CashBoxLocationTypeId == editModel.LocationId,
                                      Text = cbl.CashBoxLocationTypeDesc,
                                      ValueInt = cbl.CashBoxLocationTypeId
                                  }).ToList();
            editModel.Location.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.LocationId == -1,
                Text = "",
                Value = "-1"
            });

            //// Operational status of meter.
            //GetOperationalStatusList(editModel, editModel.Status.StatusId);

            // Get state list
            GetAssetStateList(editModel);

        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="CashboxEditModel"/> to the [AssetPending] table.
        /// </summary>
        /// <param name="model">Instance of a <see cref="CashboxEditModel"/> with updates</param>
        public void SetEditModel(CashboxEditModel model)
        {
            // Note:  At this time all changes to the gateway are written to the [AssetPending] table.

            var otherModel = GetEditModel( model.CustomerId, (int)model.AssetId );

            // Write changes to [AssetPending] table.
            (new PendingFactory(ConnectionStringName, Now)).Save(model, otherModel);

        }


        /// <summary>
        /// Takes a list of <see cref="AssetIdentifier"/> pointing to a list of cashboxes and creates the model
        /// used to mass edit the list of cashboxes.  The model contains only fields that can be changed for all
        /// items in the list.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="cashboxIds">A <see cref="List{AssetIdentifier}"/> indicating cashboxes to be edited</param>
        /// <returns>An instance of <see cref="CashboxMassEditModel"/></returns>
        public CashboxMassEditModel GetMassEditModel(int customerId, List<AssetIdentifier> cashboxIds)
        {
            CashboxMassEditModel editModel = new CashboxMassEditModel()
            {
                CustomerId = customerId
            };
            SetPropertiesOfAssetMassiveEditModel(editModel, cashboxIds);
            
            // Cashbox location
            editModel.LocationId = -1;
            editModel.Location = (from cbl in PemsEntities.CashBoxLocationTypes
                                  select new SelectListItemWrapper()
                                  {
                                      Selected = cbl.CashBoxLocationTypeId == editModel.LocationId,
                                      Text = cbl.CashBoxLocationTypeDesc,
                                      ValueInt = cbl.CashBoxLocationTypeId
                                  }).ToList();
            editModel.Location.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.LocationId == -1,
                Text = "",
                Value = "-1"
            });

            // Preventative maintenance dates
            editModel.LastPrevMaint = (DateTime?)null;
            editModel.NextPrevMaint = (DateTime?)null;

            // Warantry expiration
            editModel.WarrantyExpiration = (DateTime?)null;

         
            return editModel;
        }


        /// <summary>
        /// Refreshes any lists in the <see cref="CashboxMassEditModel"/> instance.  This is used
        /// where a model has been submitted from a web page but the contents of the dropdown lists were not passed back.
        /// </summary>
        /// <param name="editModel">The instance of the <see cref="CashboxMassEditModel"/> to refresh</param>
        public void RefreshMassEditModel(CashboxMassEditModel editModel)
        {
            // Get State list
            GetAssetStateList(editModel);

            // Cashbox location
            editModel.LocationId = -1;
            editModel.Location = (from cbl in PemsEntities.CashBoxLocationTypes
                                  select new SelectListItemWrapper()
                                  {
                                      Selected = cbl.CashBoxLocationTypeId == editModel.LocationId,
                                      Text = cbl.CashBoxLocationTypeDesc,
                                      ValueInt = cbl.CashBoxLocationTypeId
                                  }).ToList();
            editModel.Location.Insert(0, new SelectListItemWrapper()
            {
                Selected = editModel.LocationId == -1,
                Text = "",
                Value = "-1"
            });

        }

        /// <summary>
        /// Saves the updates of an instance of <see cref="CashboxMassEditModel"/> to the [AssetPending] table for each
        /// cashbox in the <see cref="CashboxMassEditModel.AssetIds"/> list.  Only values that have been changed are 
        /// stored.
        /// </summary>
        /// <param name="model">Instance of a <see cref="CashboxMassEditModel"/> with updates</param>
        public void SetMassEditModel(CashboxMassEditModel model)
        {
            var pendingFactory = new PendingFactory(ConnectionStringName, Now);
            foreach (var asset in model.AssetIds())
            {
                // Save changes to the [AssetPending] table.
                pendingFactory.Save(model, model.CustomerId, asset.AreaId, (int)asset.AssetId);
            }
        }


        #endregion

        #region Create/Clone

        /// <summary>
        /// Creates a clone (copy) of the <see cref="CashBox"/> refrenced by <paramref name="assetId"/> and <paramref name="customerId"/>.  This cloned
        /// <see cref="CashBox"/> is written to the [CashBox] table and the [AssetPending] table.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id (presently not used)</param>
        /// <param name="assetId">The cashbox id to clone</param>
        /// <returns>Integer representing the new cashbox id (<see cref="CashBox.CashBoxID"/>)</returns>
        private int Clone(int customerId, int areaId, Int64 assetId)
        {
            // Get original cashbox 
            // Since this cashbox exists, this entity should exist also.
            CashBox cashbox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CashBoxSeq == assetId && m.CustomerID == customerId);

            // Create a cashbox
            CashBox newCashbox = new CashBox()
            {
               // CashBoxID = NextCashBoxId(customerId),
                CustomerID = customerId,
                CashBoxSeq = NextCashBoxId(customerId), //cashbox.CashBoxSeq,
                CashBoxState = (int)AssetStateType.Pending,
                CashBoxModel = cashbox.CashBoxModel,
                CashBoxType = cashbox.CashBoxType,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                NextPreventativeMaintenance = cashbox.NextPreventativeMaintenance,
                CashBoxName = "",
                WarrantyExpiration = cashbox.WarrantyExpiration,
                CashBoxLocationTypeId = cashbox.CashBoxLocationTypeId
                
            };

            PemsEntities.CashBoxes.Add(newCashbox);
            PemsEntities.SaveChanges();

            // Set audit records.
            Audit(newCashbox);

            // Add AssetPending record
            (new PendingFactory(ConnectionStringName, Now)).SetImportPending(AssetTypeModel.AssetType.Cashbox,
                                                                       SetToMidnight(Now),
                                                                       AssetStateType.Current,
                                                                       customerId,
                                                                       newCashbox.CashBoxID,
                                                                       (int)AssetStateType.Pending,(int)OperationalStatusType.Inactive,
                                                                       (int)AssetAreaId.Cashbox);

            return newCashbox.CashBoxSeq;
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="CashBox"/> refrenced by <paramref name="model"/>.  This cloned
        /// <see cref="CashBox"/> is written to the [CashBox] table and the [AssetPending] table.
        /// </summary>
        /// <param name="model">An instance of <see cref="CashboxViewModel"/> referencinf cashbox to be cloned</param>
        /// <returns>Integer representing the new cashbox id. <see cref="CashBox.CashBoxID"/></returns>
        public int Clone(CashboxViewModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }

        /// <summary>
        /// Creates a clone (copy) of the <see cref="CashBox"/> refrenced by <paramref name="model"/>.  This cloned
        /// <see cref="CashBox"/> is written to the [CashBox] table and the [AssetPending] table.  
        /// </summary>
        /// <param name="model">An instance of <see cref="CashboxViewModel"/> referencinf cashbox to be cloned</param>
        /// <returns>Integer representing the new cashbox id. <see cref="CashBox.CashBoxID"/></returns>
        public int Clone(CashboxEditModel model)
        {
            return Clone(model.CustomerId, model.AreaId, model.AssetId);
        }

        /// <summary>
        /// Gets the next CashBox Id from the table [CashBox].  This will be used
        /// in the creation of a new CashBox.  Called from <see cref="CashboxFactory.Create"/> and <see cref="CashboxFactory.Clone(int, int, Int64)"/>.  
        /// It is entirely possible that this is the first CashBox so check for this case.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Next available CashBox id</returns>
        private int NextCashBoxId(int customerId)
        {
            int nextId = 1;

            CashBox cashBox = PemsEntities.CashBoxes.FirstOrDefault(m => m.CustomerID == customerId);
            if (cashBox != null)
            {
                nextId = PemsEntities.CashBoxes.Where(m => m.CustomerID == customerId).Max(m => m.CashBoxSeq) + 1;
            }
            return nextId;
        }


        /// <summary>
        /// Create a new cashbox and set it to "Inactive" operational status (<see cref="CashBox.OperationalStatus"/>) 
        /// and a "Current" state (<see cref="CashBox.CashBoxState"/>). Associate it with this <paramref name="customerId"/>.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Instance of <see cref="CashBox"/> of new cashbox.</returns>
        public CashBox Create(int customerId)
        {
            // Create new cashbox
            CashBox cashbox = new CashBox()
            {
               // CashBoxID = NextCashBoxId(customerId),
                CustomerID = customerId,
                CashBoxName = "",
                CashBoxSeq = NextCashBoxId(customerId),
                CashBoxState = (int)AssetStateType.Pending,
                CashBoxType = (int)MeterGroups.Cashbox,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                CashBoxLocationTypeId = (int)Duncan.PEMS.Entities.Enumerations.CashBoxLocationType.Inventory
            };
            PemsEntities.CashBoxes.Add(cashbox);
            PemsEntities.SaveChanges();

            // Create audit record.
            Audit(cashbox);

            return cashbox;
        }

        public CashBox Create(int customerId, AssetTypeModel.EnterMode enterMode, int cashBoxID)
        {
            // Create new cashbox
            int areaId = (int)AssetAreaId.Cashbox;
            CashBox cashbox = new CashBox()
            {
                //CashBoxID = NextCashBoxId(customerId),
              //  CashBoxID = cashBoxID,
                CustomerID = customerId,
                CashBoxName = "",
                CashBoxSeq = cashBoxID,
                CashBoxState = (int)AssetStateType.Pending,
                CashBoxType = (int)MeterGroups.Cashbox,
                OperationalStatus = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                CashBoxLocationTypeId = (int)Duncan.PEMS.Entities.Enumerations.CashBoxLocationType.Inventory
            };
            PemsEntities.CashBoxes.Add(cashbox);
            PemsEntities.SaveChanges();

            // Create audit record.
            Audit(cashbox);

            return cashbox;
        }

        #endregion

        #region History

        /// <summary>
        /// Gets a <see cref="List{CashboxViewModel}"/> of <see cref="CashboxViewModel"/> representing the known history of a
        /// cashbox referenced by cashbox id of <paramref name="assetId"/> and customer id of <paramref name="customerId"/>.
        /// Return only a list that are between <paramref name="startDateTicks"/> and <paramref name="endDateTicks"/>
        /// </summary>
        /// <param name="request">Instance of <see cref="DataSourceRequest"/> from Kendo UI</param>
        /// <param name="total">Out parameter used to return total number of records being returned</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="areaId">The area id</param>
        /// <param name="assetId">The cashbox id</param>
        /// <param name="startDateTicks">From date in ticks</param>
        /// <param name="endDateTicks">To date in ticks</param>
        /// <returns><see cref="List{CashboxViewModel}"/> instance</returns>
        public List<CashboxViewModel> GetHistory([DataSourceRequest] DataSourceRequest request, out int total, int customerId, int areaId, Int64 assetId, long startDateTicks, long endDateTicks)
        {
            DateTime startDate;
            DateTime endDate;

            IQueryable<CashBoxAudit> query = null;

            if (startDateTicks > 0 && endDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.CashBoxAudits.Where( m => m.CustomerID == customerId && m.CashBoxID == assetId 
                    && m.UpdateDateTime >= startDate && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (startDateTicks > 0)
            {
                startDate = (new DateTime(startDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.CashBoxAudits.Where(m => m.CustomerID == customerId && m.CashBoxID == assetId
                    && m.UpdateDateTime >= startDate).OrderBy(m => m.UpdateDateTime);
            }
            else if (endDateTicks > 0)
            {
                endDate = (new DateTime(endDateTicks, DateTimeKind.Utc)).ToLocalTime();
                query = PemsEntities.CashBoxAudits.Where(m => m.CustomerID == customerId && m.CashBoxID == assetId
                    && m.UpdateDateTime <= endDate).OrderBy(m => m.UpdateDateTime);
            }
            else
            {
                query = PemsEntities.CashBoxAudits.Where(m => m.CustomerID == customerId && m.CashBoxID == assetId).OrderBy(m => m.UpdateDateTime);
            }

            // Get the CashboxViewModel for each audit record.
            var list = new List<CashboxViewModel>();

            // Get present data
            var presentModel = GetViewModel( customerId, (int) assetId );
            presentModel.RecordDate = Now;
            list.Add(presentModel);

            // Get historical data view models
            CashboxViewModel previousModel = null;
            foreach (var cashbox in query)
            {
                var activeModel = CreateHistoricViewModel(cashbox);
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
