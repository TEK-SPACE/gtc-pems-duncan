
/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 12/20/2013       Sergey Ostrerov                 Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 * 02/06/2014       R Howard                    JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NPOI.SS.Formula.Functions;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Assets"/> namespace contains classes for managing assets.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// This class is the base class for all asset-handling factory classes.  Any class that handles
    /// asset business functionality should inherit from this class.
    /// </summary>
    public class AssetFactory : BaseFactory
    {

        /// <summary>
        /// This is the common <see cref="DateTime"/> used for any configuration/tariff 
        /// creation and other customer-centric time uses.  By default it is set to DateTime.Now.  
        /// Alternative DateTimes can be passed via constructor.
        /// </summary>
        protected DateTime Now = DateTime.Now;


        /// <summary>
        /// This is the base factory constructor that underlays all asset business function factories.  It takes a 
        /// a name of a connection string, <paramref name="connectionStringName"/>, so as to point to a specific instance of the PEMS database.  Also takes
        /// a <see cref="DateTime"/> <paramref name="customerNow"/> to reference the time at the customer's physical location.
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
        public AssetFactory(string connectionStringName, DateTime customerNow)
        {
            ConnectionStringName = connectionStringName;
            Now = customerNow;
        }

        /// <summary>
        /// This is the base factory constructor that underlays all business function factories.  It takes a 
        /// a name of a connection string, <paramref name="connectionStringName"/>, so as to point to a specific instance of the PEMS database.
        /// This vrsion of the constructor will default to <see cref="DateTime.Now"/> for time.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public AssetFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Private version of default constructor to prevent any instantiation of an AssetFactory without a connection string name.
        /// </summary>
        private AssetFactory()
        {

        }

        public static List<ConfigurationAssetListModel> CastToDerived(IEnumerable<AssetListModel> originalItems, List<ConfigurationAssetListModel> newItems)
        {
            newItems.AddRange(from originalItem in originalItems let newItem = new ConfigurationAssetListModel() select ExportFactory.CopyFrom(newItem, originalItem));
            return newItems;
        }
        public static List<SummaryAssetListModel> CastToDerived(IEnumerable<AssetListModel> originalItems, List<SummaryAssetListModel> newItems)
        {
            newItems.AddRange(from originalItem in originalItems let newItem = new SummaryAssetListModel() select ExportFactory.CopyFrom(newItem, originalItem));
            return newItems;
        }
        public static List<FunctionalStatusAssetListModel> CastToDerived(IEnumerable<AssetListModel> originalItems, List<FunctionalStatusAssetListModel> newItems)
        {
            newItems.AddRange(from originalItem in originalItems let newItem = new FunctionalStatusAssetListModel() select ExportFactory.CopyFrom(newItem, originalItem));
            return newItems;
        }
        public static List<OccupancyAssetListModel> CastToDerived(IEnumerable<AssetListModel> originalItems, List<OccupancyAssetListModel> newItems)
        {
            newItems.AddRange(from originalItem in originalItems let newItem = new OccupancyAssetListModel() select ExportFactory.CopyFrom(newItem, originalItem));
            return newItems;
        }

        public static AssetClass GetAssetClass(int? meterGroupId)
        {
            if (meterGroupId.HasValue)
            {
                switch (meterGroupId)
                {
                    case (int)MeterGroups.SingleSpaceMeter:
                    case (int)MeterGroups.MultiSpaceMeter:
                        return AssetClass.Meter;
                    case (int)MeterGroups.Cashbox:
                        return AssetClass.Cashbox;
                    case (int)MeterGroups.Sensor:
                        return AssetClass.Sensor;
                    case (int)MeterGroups.Gateway:
                        return AssetClass.Gateway;
                    case (int)MeterGroups.Smartcard:
                        return AssetClass.Smartcard;
                    case (int)MeterGroups.Mechanism:
                        return AssetClass.Mechanism;
                    case (int)MeterGroups.Datakey:
                        return AssetClass.DataKey;
                }
            }

            return AssetClass.Unknown;
        }

        public static MeterGroups GetMeterGroup(string assetType)
        {
            MeterGroups meterGroup;
            Enum.TryParse(assetType.Trim().Replace(" ", ""), out meterGroup);
            return meterGroup;
        }


        public static AssetClass GetAssetClass(string assetType)
        {
            MeterGroups meterGroup;
            //get the meter group based on the asset type passed in
            bool parsed = Enum.TryParse(assetType.Trim().Replace(" ", ""), out meterGroup);
            if (parsed)
            {
                switch (meterGroup)
                {
                    case MeterGroups.SingleSpaceMeter:
                    case MeterGroups.MultiSpaceMeter:
                        return AssetClass.Meter;
                    case MeterGroups.Cashbox:
                        return AssetClass.Cashbox;
                    case MeterGroups.Sensor:
                        return AssetClass.Sensor;
                    case MeterGroups.Gateway:
                        return AssetClass.Gateway;
                    case MeterGroups.Smartcard:
                        return AssetClass.Smartcard;
                    case MeterGroups.Mechanism:
                        return AssetClass.Mechanism;
                    case MeterGroups.Datakey:
                        return AssetClass.DataKey;
                }
            }
            return AssetClass.Unknown;
        }

        /// <summary>
        /// Gets the customized asset type name for a customer.
        /// </summary>
        /// <param name="meterGroupId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public string GetAssetTypeDescription(int? meterGroupId, int customerId)
        {
            //first try to get it out of the asset type table for this customer
            var assetType = PemsEntities.AssetTypes.FirstOrDefault(x => x.MeterGroupId == meterGroupId && x.CustomerId == customerId);
            if (assetType != null)
            {
                //check to make sure we are supposed to be displaying this field. if not, return nothing
                if (assetType.IsDisplay == false)
                    return string.Empty;

                //return the value
                return assetType.MeterGroupDesc;
            }

            //couldnt find it in the asset type table, return nothing
            return string.Empty;
        }

        public AssetConfigurationEditModel GetAssetConfigurationEditModel(int meterGroupId)
        {
            AssetConfigurationEditModel model = new AssetConfigurationEditModel();
            SelectListItem listItem;

            // VersionGroup.MPV
            model.MPVVersion = new List<SelectListItem>
                {
                    new SelectListItem()
                        {
                            Selected = true,
                            Text = "-",
                            Value = "-1"
                        }
                };
            model.MPVVersionId = -1;

            var query = from fdftmg in PemsEntities.FDFileTypeMeterGroups
                        join fdft in PemsEntities.FDFileTypes on fdftmg.FileType equals fdft.FileType
                        join avmm in PemsEntities.AssetVersionMasters on fdftmg.FDFileTypeMeterGroupID equals avmm.FDFileTypeMeterGroupID
                        where fdftmg.MeterGroup == meterGroupId
                        where fdftmg.VersionGroup == (int)VersionGroupType.MPV
                        orderby avmm.CreateDate
                        select new { fdft.FileExtension, avmm.AssetVersionMasterId, avmm.VersionName };
            foreach (var version in query)
            {
                model.MPVVersion.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = version.VersionName + " (" + version.FileExtension + ")",
                    Value = version.AssetVersionMasterId.ToString()
                });
            }

            // VersionGroup.Software
            model.SoftwareVersion = new List<SelectListItem>
                {
                    new SelectListItem()
                        {
                            Selected = true,
                            Text = "-",
                            Value = "-1"
                        }
                };
            model.SoftwareVersionId = -1;

            query = from fdftmg in PemsEntities.FDFileTypeMeterGroups
                    join fdft in PemsEntities.FDFileTypes on fdftmg.FileType equals fdft.FileType
                    join avmm in PemsEntities.AssetVersionMasters on fdftmg.FDFileTypeMeterGroupID equals avmm.FDFileTypeMeterGroupID
                    where fdftmg.MeterGroup == meterGroupId
                    where fdftmg.VersionGroup == (int)VersionGroupType.Software
                    orderby avmm.CreateDate
                    select new { fdft.FileExtension, avmm.AssetVersionMasterId, avmm.VersionName };
            foreach (var version in query)
            {
                model.SoftwareVersion.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = version.VersionName + " (" + version.FileExtension + ")",
                    Value = version.AssetVersionMasterId.ToString()
                });
            }

            // VersionGroup.Hardware
            model.HardwareVersion = new List<SelectListItem>
                {
                    new SelectListItem()
                        {
                            Selected = true,
                            Text = "-",
                            Value = "-1"
                        }
                };
            model.HardwareVersionId = -1;

            query = from fdftmg in PemsEntities.FDFileTypeMeterGroups
                    join fdft in PemsEntities.FDFileTypes on fdftmg.FileType equals fdft.FileType
                    join avmm in PemsEntities.AssetVersionMasters on fdftmg.FDFileTypeMeterGroupID equals avmm.FDFileTypeMeterGroupID
                    where fdftmg.MeterGroup == meterGroupId
                    where fdftmg.VersionGroup == (int)VersionGroupType.Hardware
                    orderby avmm.CreateDate
                    select new { fdft.FileExtension, avmm.AssetVersionMasterId, avmm.VersionName };
            foreach (var version in query)
            {
                model.SoftwareVersion.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = version.VersionName + " (" + version.FileExtension + ")",
                    Value = version.AssetVersionMasterId.ToString()
                });
            }

            // VersionGroup.Firmware
            model.FirmwareVersion = new List<SelectListItem>
                {
                    new SelectListItem()
                        {
                            Selected = true,
                            Text = "-",
                            Value = "-1"
                        }
                };
            model.FirmwareVersionId = -1;

            query = from fdftmg in PemsEntities.FDFileTypeMeterGroups
                    join fdft in PemsEntities.FDFileTypes on fdftmg.FileType equals fdft.FileType
                    join avmm in PemsEntities.AssetVersionMasters on fdftmg.FDFileTypeMeterGroupID equals avmm.FDFileTypeMeterGroupID
                    where fdftmg.MeterGroup == meterGroupId
                    where fdftmg.VersionGroup == (int)VersionGroupType.Firmware
                    orderby avmm.CreateDate
                    select new { fdft.FileExtension, avmm.AssetVersionMasterId, avmm.VersionName };
            foreach (var version in query)
            {
                model.SoftwareVersion.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = version.VersionName + " (" + version.FileExtension + ")",
                    Value = version.AssetVersionMasterId.ToString()
                });
            }



            return model;
        }


        #region Edit Model Helpers - Common to all assets

        protected List<SelectListItemWrapper> ZoneList(int customerId, int selectedId)
        {
            List<SelectListItemWrapper> list = (from z in PemsEntities.Zones
                                                where z.customerID == customerId
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = z.ZoneId == selectedId,
                                                    Text = z.ZoneName,
                                                    ValueInt = z.ZoneId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        protected List<SelectListItemWrapper> SuburbList(int customerId, int selectedId)
        {
            List<SelectListItemWrapper> list = (from cg1 in PemsEntities.CustomGroup1
                                                where cg1.CustomerId == customerId
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = cg1.CustomGroupId == selectedId,
                                                    Text = cg1.DisplayName,
                                                    ValueInt = cg1.CustomGroupId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        protected List<SelectListItemWrapper> MaintenanceRouteList(int customerId, int selectedId)
        {
            List<SelectListItemWrapper> list = (from mr in PemsEntities.MaintRoutes
                                                where mr.CustomerId == customerId
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = mr.MaintRouteId == selectedId,
                                                    Text = mr.DisplayName,
                                                    ValueInt = mr.MaintRouteId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        protected List<SelectListItemWrapper> AreaList(int customerId, int selectedId)
        {

            //we need to ignor the default areas (99,98,97,1)
            List<SelectListItemWrapper> list = (from a in PemsEntities.Areas
                                                where a.CustomerID == customerId
                                                where (a.AreaID != 99 && a.AreaID != 98 && a.AreaID != 97 && a.AreaID != 1)
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = a.AreaID == selectedId,
                                                    Text = a.AreaName,
                                                    ValueInt = a.AreaID
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        public List<SelectListItemWrapper> DemandStatusList(int customerId, int selectedId)
        {

             var list = (from dzc in PemsEntities.DemandZoneCustomers
                                  join dz in PemsEntities.DemandZones on dzc.DemandZoneId equals dz.DemandZoneId
                         where dzc.CustomerId == customerId
                                  where dzc.IsDisplay != null
                                  where (bool)dzc.IsDisplay == true
                                  where dz.DemandZoneDesc !=null
                                  select new SelectListItemWrapper()
                                    {
                                        Selected = dzc.DemandZoneId == selectedId,
                                        Text = dz.DemandZoneDesc,
                                        ValueInt = dzc.DemandZoneId
                                    }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }

        protected List<SelectListItemWrapper> ConfigProfileList(int customerId, int selectedId)
        {
            List<SelectListItemWrapper> list = (from cp in PemsEntities.ConfigProfiles
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = cp.ConfigProfileId == selectedId,
                                                    Text = cp.ConfigurationName,
                                                    ValueInt64 = cp.ConfigProfileId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }



        protected List<SelectListItemWrapper> SpaceTypeList(int selectedId)
        {
            List<SelectListItemWrapper> list = (from st in PemsEntities.SpaceTypes
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = st.SpaceTypeId == selectedId,
                                                    Text = st.SpaceTypeDesc,
                                                    ValueInt = st.SpaceTypeId
                                                }).ToList();

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = selectedId == -1,
                Text = "",
                Value = "-1"
            });

            return list;
        }




        protected void GetZoneList(AssetEditBaseModel assetEditBaseModel)
        {
            assetEditBaseModel.Zone = ZoneList(assetEditBaseModel.CustomerId, assetEditBaseModel.ZoneId);
        }

        protected void GetSuburbList(AssetEditBaseModel assetEditBaseModel)
        {
            assetEditBaseModel.Suburb = SuburbList(assetEditBaseModel.CustomerId, assetEditBaseModel.SuburbId);
        }


        protected void GetMaintenanceRouteList(AssetEditBaseModel assetEditBaseModel)
        {
            assetEditBaseModel.MaintenanceRoute = MaintenanceRouteList(assetEditBaseModel.CustomerId, assetEditBaseModel.MaintenanceRouteId);
        }

        protected void GetAreaList(AssetEditBaseModel assetEditBaseModel)
        {
            assetEditBaseModel.Area = AreaList(assetEditBaseModel.CustomerId, assetEditBaseModel.AreaListId);
        }


        protected void GetOperationalStatusDetails(AssetEditBaseModel assetEditBaseModel, int operationalStatusId, DateTime operationalStatusDate)
        {
            assetEditBaseModel.StatusId = operationalStatusId;
            assetEditBaseModel.StatusDate = operationalStatusDate;

            assetEditBaseModel.Status = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == operationalStatusId).OperationalStatusDesc;

            assetEditBaseModel.StatusOperationalId = (int)OperationalStatusType.Operational;
            assetEditBaseModel.StatusOperational = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == (int)OperationalStatusType.Operational).OperationalStatusDesc;

            assetEditBaseModel.StatusInactiveId = (int)OperationalStatusType.Inactive;
            assetEditBaseModel.StatusInactive= PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == (int)OperationalStatusType.Inactive).OperationalStatusDesc;
            


        }

        /// <summary>
        /// Gets the id of the mechanism master for this customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="mechanismMasterId"></param>
        /// <returns></returns>
        public string GetMechanismMasterCustomerDescription(int customerId, int? mechanismMasterId)
        {
            //first get the emchanism form the mechanismmaster table
            var mmCustomer = PemsEntities.MechanismMasterCustomers.FirstOrDefault(x => x.CustomerId == customerId && x.MechanismId == mechanismMasterId);
            if (mmCustomer != null)
                return mmCustomer.MechanismDesc;

            //if there is not a customer specific vrsion, return the mechanism description
            var mMaster = PemsEntities.MechanismMasters.FirstOrDefault(x => x.MechanismId == mechanismMasterId);
            if (mMaster != null)
                return mMaster.MechanismDesc;

            //nothing was found, return empty string
            return string.Empty;

        }

        protected void GetAssetModelList(AssetEditBaseModel assetEditBaseModel, int? meterGroupId)
        {
            assetEditBaseModel.AssetModel = (from mmc in PemsEntities.MechanismMasterCustomers
                                             join mm in PemsEntities.MechanismMasters
                                             on mmc.MechanismId equals mm.MechanismId
                                             where mmc.CustomerId == assetEditBaseModel.CustomerId && mmc.IsDisplay && mm.MeterGroupId == meterGroupId
                                             select new SelectListItemWrapper()
                                             {
                                                 Selected = mmc.MechanismId == assetEditBaseModel.AssetModelId,
                                                 //default it to the base model name if it isnt set correctly (blank)
                                                 Text = mmc.MechanismDesc != string.Empty? mmc.MechanismDesc : mm.MechanismDesc,
                                                 ValueInt = mmc.MechanismId
                                             }).ToList();

            assetEditBaseModel.AssetModel.Insert(0, new SelectListItemWrapper()
            {
                Selected = assetEditBaseModel.AssetModelId == -1,
                Text = "",
                Value = "-1"
            });
        }


        protected void GetAssetStateList(AssetEditBaseModel assetEditBaseModel)
        {
            assetEditBaseModel.State = (from asc in PemsEntities.AssetStateCustomers
                                        where asc.CustomerId == assetEditBaseModel.CustomerId && asc.IsDisplayed
                                        select new SelectListItemWrapper()
                                        {
                                            Selected = asc.AssetStateId == assetEditBaseModel.StateId,
                                            Text = asc.AssetStateDesc,
                                            ValueInt = asc.AssetStateId
                                        }).ToList();
        }

        protected void GetAssetStateList(AssetMassUpdateBaseModel assetMassUpdateBaseModel)
        {
            assetMassUpdateBaseModel.State = (from asc in PemsEntities.AssetStateCustomers
                                              where asc.CustomerId == assetMassUpdateBaseModel.CustomerId && asc.IsDisplayed
                                              select new SelectListItemWrapper()
                                              {
                                                  Selected = asc.AssetStateId == assetMassUpdateBaseModel.StateId,
                                                  Text = asc.AssetStateDesc,
                                                  ValueInt = asc.AssetStateId
                                              }).ToList();

            assetMassUpdateBaseModel.State.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "-1"
            });


        }





        #endregion

        #region Asset Lists for Grids
        public List<AssetListModel> GetSummaryModels(int customerId, string assetType, AssetListGridType gridType, [DataSourceRequest] DataSourceRequest request, out int total)
        {
            IQueryable<AssetListModel> items = null;
            total = 0;
            var at = AssetTypeModel.GetAssetType(assetType);

            switch (at)
            {
                case AssetTypeModel.AssetType.Cashbox:
                    items = (new CashboxFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    break;
                case AssetTypeModel.AssetType.Gateway:
                    items = (new GatewayFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    break;
                case AssetTypeModel.AssetType.SingleSpaceMeter:
                case AssetTypeModel.AssetType.MultiSpaceMeter:
                    var meterGroup = GetMeterGroup(assetType);
                    items = (new MeterFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType, meterGroup);
                    break;
                case AssetTypeModel.AssetType.Sensor:
                    items = (new SensorFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    break;
                //case AssetTypeModel.AssetType.Space:  //** Sairam commented this on 1st oct 2014
                case AssetTypeModel.AssetType.ParkingSpaces: //** Sairam added this on 1st oct 2014
                    items = (new SpaceFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    break;
                case AssetTypeModel.AssetType.Smartcard:
                    //we have no mapping or logic for smartcards, return nothing
                    return new List<AssetListModel>();
                case AssetTypeModel.AssetType.Mechanism:
                    items = (new MechanismFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    break;
                case AssetTypeModel.AssetType.DataKey:
                    items = (new DataKeyFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    break;
                default:
                    //group them all, filter them first, then you can sort and page on all the items
                    var cashboxes = (new CashboxFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    cashboxes = cashboxes.ApplyFiltering(request.Filters);
                    var gateways = (new GatewayFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    gateways = gateways.ApplyFiltering(request.Filters);
                    var ssms = (new MeterFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType, MeterGroups.SingleSpaceMeter);
                    ssms = ssms.ApplyFiltering(request.Filters);
                    var msms = (new MeterFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType, MeterGroups.MultiSpaceMeter);
                    msms = msms.ApplyFiltering(request.Filters);
                    var sensors = (new SensorFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    sensors = sensors.ApplyFiltering(request.Filters);
                    var spaces = (new SpaceFactory(ConnectionStringName, Now)).GetSummaryModels(customerId, gridType);
                    spaces = spaces.ApplyFiltering(request.Filters);
                    var mechanisms = (new MechanismFactory(ConnectionStringName, Now)).GetSummaryModels(customerId,
                        gridType).ApplyFiltering(request.Filters);
                    var datakeys = (new DataKeyFactory(ConnectionStringName, Now)).GetSummaryModels(customerId,
                        gridType).ApplyFiltering(request.Filters);

                    var finalItems = cashboxes.ToList();
                    finalItems.AddRange(gateways.ToList());
                    finalItems.AddRange(ssms.ToList());
                    finalItems.AddRange(msms.ToList());
                    finalItems.AddRange(sensors.ToList());
                    finalItems.AddRange(spaces.ToList());
                    finalItems.AddRange(mechanisms.ToList());
                    finalItems.AddRange(datakeys.ToList());
                    items = finalItems.AsQueryable();
                    total = items.Count();
                    items = items.ApplySorting(request.Groups, request.Sorts);
                    items = items.ApplyPaging(request.Page, request.PageSize);
                    return items.ToList();
            }
            if (items != null)
            {
                items = items.ApplyFiltering(request.Filters);
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                return items.ToList();
            }
            return new List<AssetListModel>();
        }

        #endregion

        #region Get AssetTypeModel (used for creating assets)

        public AssetTypesModel GetAssetTypesModel(int customerId)
        {
            var model = new AssetTypesModel()
            {
                CustomerId = customerId
            };

            AssetTypeModel.AssetType? assetType = null;

            //// Make an entry for a space
            // Cannot create a stand-alone spce RJH 06/26/2013
            //model.AssetTypes.Add(new AssetTypeModel()
            //{
            //    Name = "Space",
            //    Type = AssetTypeModel.AssetType.Space
            //});

            // Make an entry for anything that can be mapped between enumerations MeterGroups and AssetType

            // MeterGroup->AssetType->



            var meterGroups = (from at in PemsEntities.AssetTypes
                               where at.CustomerId == customerId && at.IsDisplay == true
                               join mm in PemsEntities.MechanismMasters on at.MeterGroupId equals mm.MeterGroupId
                               join mmc in PemsEntities.MechanismMasterCustomers on mm.MechanismId equals mmc.MechanismId
                               where mmc.IsDisplay == true && mmc.CustomerId == customerId
                               select new { mm.MeterGroupId, at.MeterGroupDesc }).Distinct();


            foreach (var meterGroup in meterGroups)
            {
                if (AssetTypeModel.ConvertType((MeterGroups)meterGroup.MeterGroupId, out assetType))
                {
                    model.AssetTypes.Add(new AssetTypeModel()
                    {
                        Name = meterGroup.MeterGroupDesc,
                        Type = (AssetTypeModel.AssetType)assetType
                    });
                }
            }

       

            return model;
        }


        #endregion

        #region Creation of an Asset

        /// <summary>
        /// Gets the next unused Meter Id from the table [Meters].  This table is actually more of 
        /// an "asset" table so next Meter Id is actually "next asset id".  This will be used
        /// in the creation of a new asset.  It is entirely possible that this is the first asset
        /// so check for this case.
        /// </summary>
        /// <param name="customerId">Integer id of customer</param>
        /// <param name="meterGroup">The <see cref="MeterGroups"/> type of asset.</param>
        /// <param name="areaId">Optional AreaId of asset</param>
        /// <returns>Next asset available id.</returns>
        private int GetNextAssetId(int customerId, MeterGroups meterGroup, int areaId = 0)
        {
            int nextAssetId = 1;

            int nextAssetAreaId = areaId;

            if (nextAssetAreaId == 0)
            {
                // Determine appropriate AreaId
                nextAssetAreaId = (int)AssetAreaId.Meter;
                switch (meterGroup)
                {
                    case MeterGroups.Cashbox:
                        nextAssetAreaId = (int)AssetAreaId.Cashbox;
                        break;
                    case MeterGroups.Gateway:
                        nextAssetAreaId = (int)AssetAreaId.Gateway;
                        break;
                    case MeterGroups.Sensor:
                        nextAssetAreaId = (int)AssetAreaId.Sensor;
                        break;
                    case MeterGroups.Mechanism:
                        nextAssetAreaId = (int)AssetAreaId.Mechanism;
                        break;
                    case MeterGroups.Datakey:
                        nextAssetAreaId = (int) AssetAreaId.DataKey;
                        break;
                }
            }

            Meter meter = PemsEntities.Meters.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == nextAssetAreaId);
            if (meter != null)
            {
                nextAssetId = PemsEntities.Meters.Where(m => m.CustomerID == customerId && m.AreaID == nextAssetAreaId).Max(m => m.MeterId) + 1;
            }
            return nextAssetId;
        }

        public class GlobalMeterId
        {
            public Int64 Id { get; set; }
        }

        /// <summary>
        /// Generates the GlobalMeterId used by several tables.
        /// </summary>
        /// <param name="customerId">Integer customer id</param>
        /// <param name="areaId">Integer area id.  Should always be 1.</param>
        /// <param name="meterId">Integer meter id</param>
        /// <param name="bayNumber">Optional bay number</param>
        /// <returns></returns>
        public Int64? GetGlobalMeterId(int customerId, int areaId, int meterId, int bayNumber = 0)
        {
            DbCommand com = PemsEntities.Database.Connection.CreateCommand();

            com.CommandText = "select dbo.GenGlobalID(@cid, @aid, @mid, @baynum)";
            com.Parameters.Add(new SqlParameter("cid", customerId));
            com.Parameters.Add(new SqlParameter("aid", areaId));
            com.Parameters.Add(new SqlParameter("mid", meterId));
            com.Parameters.Add(new SqlParameter("baynum", bayNumber));

            if (com.Connection.State == ConnectionState.Closed) com.Connection.Open();

            var result = com.ExecuteScalar();

            return result != null ? (Int64?)result : (Int64?)null;
        }


        /// <summary>
        /// Create a basic asset in the [Meters] table.  Also creates an associated [MeterMap] entry.  
        /// </summary>
        /// <param name="customerId">Integer id of customer.</param>
        /// <param name="meterGroup"><see cref="MeterGroups"/> enumeration of meter group.</param>
        /// <param name="areaId">Optional area id to assign to new asset.  By default, the area id will be 
        /// determined from the <see cref="meterGroup"/></param>
        /// <returns>Returns an instance of a <see cref="Meter"/></returns>
        protected Meter CreateBaseAsset(int customerId, MeterGroups meterGroup, int areaId = 0)
        {

            CustomerProfile customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
            Customer customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);

            // Get the AssetType from the [AssetType] table for this customer.
            int slaMinutes = customer.SLAMinutes ?? 300;
            AssetType assetType = PemsEntities.AssetTypes.FirstOrDefault(m => m.CustomerId == customerId && m.MeterGroupId == (int)meterGroup);
            if (assetType != null)
            {
                slaMinutes = assetType.SLAMinutes ?? slaMinutes;
            }

            // Determine area id to use for new asset.
            int newAssetAreaId = areaId;

            //If no area id was passed, determine area id from meter group.
            if (newAssetAreaId == 0)
            {
                // Determine appropriate AreaId
                newAssetAreaId = (int)AssetAreaId.Meter;
                switch (meterGroup)
                {
                    case MeterGroups.Cashbox:
                        newAssetAreaId = (int)AssetAreaId.Cashbox;
                        break;
                    case MeterGroups.Gateway:
                        newAssetAreaId = (int)AssetAreaId.Gateway;
                        break;
                    case MeterGroups.Sensor:
                        newAssetAreaId = (int)AssetAreaId.Sensor;
                        break;
                    case MeterGroups.Mechanism:
                        newAssetAreaId = (int)AssetAreaId.Mechanism;
                        break;
                    case MeterGroups.Datakey:
                        newAssetAreaId = (int) AssetAreaId.DataKey;
                        break;
                }
            }

            // Get next asset id for given asset type and area
            int newAssetId = GetNextAssetId(customerId, meterGroup, newAssetAreaId);


            // Get the GlobalMeterId
            Int64? globalMeterId = GetGlobalMeterId(customerId, newAssetAreaId, newAssetId);

            var newMeter = new Meter()
            {
                GlobalMeterId = globalMeterId ?? null,
                CustomerID = customerId,
                AreaID = newAssetAreaId,
                MeterId = newAssetId,
                MeterStatus = (int)AssetStateType.Pending,
                MeterState = (int)AssetStateType.Pending,
                OperationalStatusID = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                TimeZoneID = customerProfile.TimeZoneID ?? 0,
                MeterRef = newAssetId,
                GSMNumber = "12345",
                SchedServTime = slaMinutes,
                MeterGroup = (int)meterGroup,
                DemandZone = 1,
            };

            PemsEntities.Meters.Add(newMeter);
            PemsEntities.SaveChanges();

            return newMeter;
        }

        protected Meter CreateBaseAsset(AssetTypeModel.EnterMode enterMode, int meterId, int customerId, MeterGroups meterGroup, int areaId = 0)
        {
            CustomerProfile customerProfile = RbacEntities.CustomerProfiles.FirstOrDefault(m => m.CustomerId == customerId);
            Customer customer = PemsEntities.Customers.FirstOrDefault(m => m.CustomerID == customerId);

            // Get the AssetType from the [AssetType] table for this customer.
            int slaMinutes = customer.SLAMinutes ?? 300;
            AssetType assetType = PemsEntities.AssetTypes.FirstOrDefault(m => m.CustomerId == customerId && m.MeterGroupId == (int)meterGroup);
            if (assetType != null)
            {
                slaMinutes = assetType.SLAMinutes ?? slaMinutes;
            }

            // Determine area id to use for new asset.
            int newAssetAreaId = areaId;

            //If no area id was passed, determine area id from meter group.
            if (newAssetAreaId == 0)
            {
                // Determine appropriate AreaId
                newAssetAreaId = (int)AssetAreaId.Meter;
                switch (meterGroup)
                {
                    case MeterGroups.Cashbox:
                        newAssetAreaId = (int)AssetAreaId.Cashbox;
                        break;
                    case MeterGroups.Gateway:
                        newAssetAreaId = (int)AssetAreaId.Gateway;
                        break;
                    case MeterGroups.Sensor:
                        newAssetAreaId = (int)AssetAreaId.Sensor;
                        break;
                    case MeterGroups.Mechanism:
                        newAssetAreaId = (int)AssetAreaId.Mechanism;
                        break;
                    case MeterGroups.Datakey:
                        newAssetAreaId = (int) AssetAreaId.DataKey;
                        break;
                }
            }

            // Get next asset id for given asset type and area
            int newAssetId = 0;
            switch ((int)enterMode)
            {
                case 1:
                    newAssetId = GetNextAssetId(customerId, meterGroup, newAssetAreaId);
                    break;
                case 0:
                    newAssetId = meterId;
                    break;
            }

            // Get the GlobalMeterId
            Int64? globalMeterId = GetGlobalMeterId(customerId, newAssetAreaId, newAssetId);

            Meter newMeter = new Meter()
            {
                GlobalMeterId = globalMeterId ?? null,
                CustomerID = customerId,
                AreaID = newAssetAreaId,
                MeterId = newAssetId,
                MeterStatus = (int)AssetStateType.Pending,
                MeterState = (int)AssetStateType.Pending,
                OperationalStatusID = (int)OperationalStatusType.Inactive,
                OperationalStatusTime = DateTime.Now,
                TimeZoneID = customerProfile.TimeZoneID ?? 0,
                MeterRef = newAssetId,
                GSMNumber = "12345",
                SchedServTime = slaMinutes,
                MeterGroup = (int)meterGroup,
                DemandZone=1,
            };

            PemsEntities.Meters.Add(newMeter);
            PemsEntities.SaveChanges();

            return newMeter;
        }

        #endregion

        #region Create Audit Entry

        /// <summary>
        /// Adds an audit record to [MetersAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="Meter"/> instance to add to audit table.</param>
        protected void Audit(Meter itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            MetersAudit audit = new MetersAudit();

            audit.GlobalMeterId = itemToAudit.GlobalMeterId;
            audit.CustomerID = itemToAudit.CustomerID;
            audit.AreaID = itemToAudit.AreaID;
            audit.MeterId = itemToAudit.MeterId;
            audit.SMSNumber = itemToAudit.SMSNumber;
            audit.MeterStatus = itemToAudit.MeterStatus;
            audit.TimeZoneID = itemToAudit.TimeZoneID;
            audit.MeterRef = itemToAudit.MeterRef;
            audit.EmporiaKey = itemToAudit.EmporiaKey;
            audit.MeterName = itemToAudit.MeterName;
            audit.Location = itemToAudit.Location;
            audit.BayStart = itemToAudit.BayStart;
            audit.BayEnd = itemToAudit.BayEnd;
            audit.Description = itemToAudit.Description;
            audit.GSMNumber = itemToAudit.GSMNumber;
            audit.SchedServTime = itemToAudit.SchedServTime;
            audit.RSFName = itemToAudit.RSFName;
            audit.RSFDateTime = itemToAudit.RSFDateTime;
            audit.BarCode = itemToAudit.BarCode;
            audit.Latitude = itemToAudit.Latitude;
            audit.Longitude = itemToAudit.Longitude;
            audit.ProgramName = itemToAudit.ProgramName;
            audit.MaxBaysEnabled = itemToAudit.MaxBaysEnabled;
            audit.MeterType = itemToAudit.MeterType;
            audit.MeterGroup = itemToAudit.MeterGroup;
            audit.MParkID = itemToAudit.MParkID;
            audit.MeterState = itemToAudit.MeterState;
            audit.DemandZone = itemToAudit.DemandZone;
            audit.TypeCode = itemToAudit.TypeCode;
            //meterAudit.OperationalStatus = itemToAudit.OperationalStatus;
            audit.InstallDate = itemToAudit.InstallDate;
            audit.OperationalStatusID = itemToAudit.OperationalStatusID;
            audit.FreeParkingMinute = itemToAudit.FreeParkingMinute;
            audit.RegulatedStatusID = itemToAudit.RegulatedStatusID;
            audit.WarrantyExpiration = itemToAudit.WarrantyExpiration;
            audit.OperationalStatusTime = itemToAudit.OperationalStatusTime;
            audit.LastPreventativeMaintenance = itemToAudit.LastPreventativeMaintenance;
            audit.NextPreventativeMaintenance = itemToAudit.NextPreventativeMaintenance;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;

            PemsEntities.MetersAudits.Add(audit);
            PemsEntities.SaveChanges();
        }


        /// <summary>
        /// Adds an audit record to [MeterMapAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="MeterMap"/> instance to add to audit table.</param>
        protected void Audit(MeterMap itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            MeterMapAudit audit = new MeterMapAudit();

            audit.Customerid = itemToAudit.Customerid;
            audit.Areaid = itemToAudit.Areaid;
            audit.MeterId = itemToAudit.MeterId;
            audit.ZoneId = itemToAudit.ZoneId;
            audit.HousingId = itemToAudit.HousingId;
            audit.MechId = itemToAudit.MechId;
            audit.DataKeyId = itemToAudit.DataKeyId;
            audit.AreaId2 = itemToAudit.AreaId2;
            audit.CollRouteId = itemToAudit.CollRouteId;
            audit.EnfRouteId = itemToAudit.EnfRouteId;
            audit.MaintRouteId = itemToAudit.MaintRouteId;
            audit.CustomGroup1 = itemToAudit.CustomGroup1;
            audit.CustomGroup2 = itemToAudit.CustomGroup2;
            audit.CustomGroup3 = itemToAudit.CustomGroup3;
            audit.SubAreaID = itemToAudit.SubAreaID;
            audit.GatewayID = itemToAudit.GatewayID;
            audit.SensorID = itemToAudit.SensorID;
            audit.CashBoxID = itemToAudit.CashBoxID;
            audit.CollectionRunId = itemToAudit.CollectionRunId;

            audit.AuditDateTime = DateTime.Now;
            audit.UserId = WebSecurity.CurrentUserId;

            PemsEntities.MeterMapAudits.Add(audit);
            PemsEntities.SaveChanges();
        }


        /// <summary>
        /// Adds an audit record to [ParkingSpacesAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="ParkingSpace"/> instance to add to audit table.</param>
        protected void Audit(ParkingSpace itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            ParkingSpacesAudit audit = new ParkingSpacesAudit();

            audit.ParkingSpaceId = itemToAudit.ParkingSpaceId;
            audit.ServerID = itemToAudit.ServerID;
            audit.CustomerID = itemToAudit.CustomerID;
            audit.AreaId = itemToAudit.AreaId;
            audit.MeterId = itemToAudit.MeterId;
            audit.BayNumber = itemToAudit.BayNumber;
            audit.AddedDateTime = itemToAudit.AddedDateTime;
            audit.Latitude = itemToAudit.Latitude;
            audit.Longitude = itemToAudit.Longitude;
            audit.HasSensor = itemToAudit.HasSensor;
            audit.SpaceStatus = itemToAudit.SpaceStatus;
            audit.DateActivated = itemToAudit.DateActivated;
            audit.Comments = itemToAudit.Comments;
            audit.DisplaySpaceNum = itemToAudit.DisplaySpaceNum;
            audit.DemandZoneId = itemToAudit.DemandZoneId;
            audit.InstallDate = itemToAudit.InstallDate;
            audit.ParkingSpaceType = itemToAudit.ParkingSpaceType;
            audit.OperationalStatus = itemToAudit.OperationalStatus;
            audit.OperationalStatusTime = itemToAudit.OperationalStatusTime;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;

            PemsEntities.ParkingSpacesAudits.Add(audit);
            PemsEntities.SaveChanges();
        }

        /// <summary>
        /// Adds an audit record to [SensorAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="Sensor"/> instance to add to audit table.</param>
        protected void Audit(Sensor itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            SensorsAudit audit = new SensorsAudit();

            audit.CustomerID = itemToAudit.CustomerID;
            audit.SensorID = itemToAudit.SensorID;
            audit.BarCodeText = itemToAudit.BarCodeText;
            audit.Description = itemToAudit.Description;
            audit.GSMNumber = itemToAudit.GSMNumber;
            audit.GlobalMeterID = itemToAudit.GlobalMeterID;
            audit.Latitude = itemToAudit.Latitude;
            audit.Longitude = itemToAudit.Longitude;
            audit.Location = itemToAudit.Location;
            audit.SensorName = itemToAudit.SensorName;
            audit.SensorState = itemToAudit.SensorState;
            audit.SensorType = itemToAudit.SensorType;
            audit.InstallDateTime = itemToAudit.InstallDateTime;
            audit.DemandZone = itemToAudit.DemandZone;
            audit.Comments = itemToAudit.Comments;
            audit.RoadWayType = itemToAudit.RoadWayType;
            audit.ParkingSpaceId = itemToAudit.ParkingSpaceId;
            audit.SensorModel = itemToAudit.SensorModel;
            audit.OperationalStatus = itemToAudit.OperationalStatus;
            audit.WarrantyExpiration = itemToAudit.WarrantyExpiration;
            audit.OperationalStatusTime = itemToAudit.OperationalStatusTime;
            audit.LastPreventativeMaintenance = itemToAudit.LastPreventativeMaintenance;
            audit.NextPreventativeMaintenance = itemToAudit.NextPreventativeMaintenance;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;

            PemsEntities.SensorsAudits.Add(audit);
            PemsEntities.SaveChanges();
        }

        /// <summary>
        /// Adds an audit record to [SensorMappingAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="SensorMapping"/> instance to add to audit table.</param>
        protected void Audit(SensorMapping itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            SensorMappingAudit audit = new SensorMappingAudit();

            audit.SensorMappingID = itemToAudit.SensorMappingID;
            audit.CustomerID = itemToAudit.CustomerID;
            audit.SensorID = itemToAudit.SensorID;
            audit.ParkingSpaceID = itemToAudit.ParkingSpaceID;
            audit.GatewayID = Convert.ToInt32(itemToAudit.GatewayID);
            audit.IsPrimaryGateway = itemToAudit.IsPrimaryGateway;

            audit.ChangeDate = DateTime.Now;
            audit.UserId = WebSecurity.CurrentUserId;

            PemsEntities.SensorMappingAudits.Add(audit);
            PemsEntities.SaveChanges();
        }


        /// <summary>
        /// Adds an audit record to [GatewaysAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="Gateway"/> instance to add to audit table.</param>
        protected void Audit(Gateway itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            GatewaysAudit audit = new GatewaysAudit();

            audit.GateWayID = itemToAudit.GateWayID;
            audit.CustomerID = itemToAudit.CustomerID;
            audit.Description = itemToAudit.Description;
            audit.Latitude = itemToAudit.Latitude;
            audit.Longitude = itemToAudit.Longitude;
            audit.Location = itemToAudit.Location;
            audit.GatewayState = itemToAudit.GatewayState;
            audit.GatewayType = itemToAudit.GatewayType;
            audit.InstallDateTime = itemToAudit.InstallDateTime;
            audit.TimeZoneID = itemToAudit.TimeZoneID;
            audit.DemandZone = itemToAudit.DemandZone;
            audit.CAMID = itemToAudit.CAMID;
            audit.CELID = itemToAudit.CELID;
            audit.PowerSource = itemToAudit.PowerSource;
            audit.HWVersion = itemToAudit.HWVersion;
            audit.Manufacturer = itemToAudit.Manufacturer;
            audit.GatewayModel = itemToAudit.GatewayModel;
            audit.OperationalStatus = itemToAudit.OperationalStatus;
            audit.OperationalStatusTime = itemToAudit.OperationalStatusTime;
            audit.LastPreventativeMaintenance = itemToAudit.LastPreventativeMaintenance;
            audit.NextPreventativeMaintenance = itemToAudit.NextPreventativeMaintenance;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;

            PemsEntities.GatewaysAudits.Add(audit);
            PemsEntities.SaveChanges();
        }



        //todo - GTC: Mechanism work
        //need to update this to audit mechanisms when we get the db scripts ready
        /// <summary>
        /// Adds an audit record to [MechMaster] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="MechMaster"/> instance to add to audit table.</param>
        protected void Audit(MechMaster itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;
            //we have to generate a new ID since the id for mechmasteraudit isnt auto increment
            long nextMechAuditId = 1;
            
            if (PemsEntities.MechMasterAudits.Any())
                nextMechAuditId = PemsEntities.MechMasterAudits.Max(m => m.MechMasterAuditId) + 1;
            var audit = new MechMasterAudit();
            audit.MechMasterAuditId = nextMechAuditId;
            
            // todo - GTC: set mechanism audit values here - test these
           // todo - GTC: Mechanisms make sure mech asset name is stored as well. Currently the db field doesnt exist
            audit.MechType = itemToAudit.MechType;
            audit.Customerid = itemToAudit.Customerid;
            audit.SimNo = itemToAudit.SimNo;
            audit.MechSerial = itemToAudit.MechSerial;
            audit.Notes = itemToAudit.Notes;
            audit.MechId = itemToAudit.MechIdNumber.GetValueOrDefault();
            audit.CreateDate = itemToAudit.CreateDate;
            audit.InactiveRemarkID = itemToAudit.InactiveRemarkID;
            audit.InsertedDate = itemToAudit.InsertedDate;
            audit.IsActive = itemToAudit.IsActive;
            audit.CreateDate = itemToAudit.CreateDate;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;
            PemsEntities.MechMasterAudits.Add(audit);
            PemsEntities.SaveChanges();
        }


        /// <summary>
        /// Adds an audit record to [CashBoxAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="CashBox"/> instance to add to audit table.</param>
        protected void Audit(CashBox itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            CashBoxAudit audit = new CashBoxAudit();

            audit.CashBoxID = itemToAudit.CashBoxID;
            audit.CustomerID = itemToAudit.CustomerID;
            audit.CashBoxSeq = itemToAudit.CashBoxSeq;
            audit.CashBoxState = itemToAudit.CashBoxState;
            audit.InstallDate = itemToAudit.InstallDate;
            audit.CashBoxModel = itemToAudit.CashBoxModel;
            audit.CashBoxType = itemToAudit.CashBoxType;
            audit.OperationalStatus = itemToAudit.OperationalStatus;
            audit.OperationalStatusTime = itemToAudit.OperationalStatusTime;
            audit.LastPreventativeMaintenance = itemToAudit.LastPreventativeMaintenance;
            audit.NextPreventativeMaintenance = itemToAudit.NextPreventativeMaintenance;
            audit.CashBoxName = itemToAudit.CashBoxName;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;

            PemsEntities.CashBoxAudits.Add(audit);
            PemsEntities.SaveChanges();
        }


        /// <summary>
        /// Adds an audit record to [VersionProfileMeterAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="VersionProfileMeter"/> instance to add to audit table.</param>
        protected void Audit(VersionProfileMeter itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            VersionProfileMeterAudit audit = new VersionProfileMeterAudit();

            audit.VersionProfileMeterId = itemToAudit.VersionProfileMeterId;
            audit.ConfigurationName = itemToAudit.ConfigurationName;
            audit.HardwareVersion = itemToAudit.HardwareVersion;
            audit.SoftwareVersion = itemToAudit.SoftwareVersion;
            audit.CommunicationVersion = itemToAudit.CommunicationVersion;
            audit.Version1 = itemToAudit.Version1;
            audit.Version2 = itemToAudit.Version2;
            audit.Version3 = itemToAudit.Version3;
            audit.Version4 = itemToAudit.Version4;
            audit.Version5 = itemToAudit.Version5;
            audit.Version6 = itemToAudit.Version6;
            audit.CustomerId = itemToAudit.CustomerId;
            audit.AreaId = itemToAudit.AreaId;
            audit.MeterId = itemToAudit.MeterId;
            audit.MeterGroup = itemToAudit.MeterGroup;
            audit.SensorID = itemToAudit.SensorID;
            audit.GatewayID = itemToAudit.GatewayID;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;

            PemsEntities.VersionProfileMeterAudits.Add(audit);
            PemsEntities.SaveChanges();
        }



        /// <summary>
        /// Adds an audit record to [HousingAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="HousingMaster"/> instance to add to audit table.</param>
        protected void Audit(HousingMaster itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            HousingAudit audit = new HousingAudit();

            audit.HousingId = itemToAudit.HousingId;
            audit.HousingName = itemToAudit.HousingName;
            audit.Customerid = itemToAudit.Customerid;
            audit.Block = itemToAudit.Block;
            audit.StreetName = itemToAudit.StreetName;
            audit.StreetType = itemToAudit.StreetType;
            audit.StreetDirection = itemToAudit.StreetDirection;
            audit.StreetNotes = itemToAudit.StreetNotes;
            audit.HousingTypeID = itemToAudit.HousingTypeID;
            audit.DoorLockId = itemToAudit.DoorLockId;
            audit.MechLockId = itemToAudit.MechLockId;
            audit.IsActive = itemToAudit.IsActive;
            audit.InactiveRemarkID = itemToAudit.InactiveRemarkID;
            audit.CreateDate = itemToAudit.CreateDate;
            audit.Notes = itemToAudit.Notes;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.AuditTS = DateTime.Now;

            PemsEntities.HousingAudits.Add(audit);
            PemsEntities.SaveChanges();
        }



        /// <summary>
        /// Adds an audit record to [ConfigProfileSpaceAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="ConfigProfileSpace"/> instance to add to audit table.</param>
        protected void Audit(ConfigProfileSpace itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            ConfigProfileSpaceAudit audit = new ConfigProfileSpaceAudit();

            audit.ConfigProfileSpaceId = itemToAudit.ConfigProfileSpaceId;
            audit.ConfigProfileId = itemToAudit.ConfigProfileId;
            audit.CustomerId = itemToAudit.CustomerId;
            audit.ParkingSpaceId = itemToAudit.ParkingSpaceId;
            audit.ScheduledDate = itemToAudit.ScheduledDate;
            audit.ActivationDate = itemToAudit.ActivationDate;
            audit.CreationDate = itemToAudit.CreationDate;
            audit.EndDate = itemToAudit.EndDate;
            audit.CreateDatetime = itemToAudit.CreateDatetime;
            audit.ConfigStatus = itemToAudit.ConfigStatus;

            audit.UserId = WebSecurity.CurrentUserId;
            audit.AuditDatetime = DateTime.Now;

            PemsEntities.ConfigProfileSpaceAudits.Add(audit);
            PemsEntities.SaveChanges();
        }

        //todo - GTC: DataKey work
        //need to update this to audit datakeys when we get the db scripts ready
        /// <summary>
        /// Adds an audit record to [DataKeyAudit] table.
        /// </summary>
        /// <param name="itemToAudit"><see cref="DataKey"/> instance to add to audit table.</param>
        protected void Audit(DataKey itemToAudit)
        {
            // Get out if nothing to audit.
            if (itemToAudit == null) return;

            var audit = new DataKeyAudit();

            // todo - GTC: set datakey audit values here - test these
            // todo - GTC: datakey make sure mech asset name is stored as well. Currently the db field doesnt exist
            audit.CustomerID = itemToAudit.CustomerID;
            audit.DataKeyDesc = itemToAudit.DataKeyDesc;
            audit.DataKeyTypeId = itemToAudit.DataKeyTypeId;
            audit.StorageMBit = itemToAudit.StorageMBit;
            audit.CityCode = itemToAudit.CityCode;
            audit.ColorCode = itemToAudit.ColorCode;
            audit.UserId = WebSecurity.CurrentUserId;
            audit.UpdateDateTime = DateTime.Now;
            PemsEntities.DataKeyAudits.Add(audit);
            PemsEntities.SaveChanges();
        }


        #endregion

        public List<string> GetOperationalStatuses()
        {
            IQueryable<string> statusQuery = from mg in PemsEntities.OperationalStatus
                                             select mg.OperationalStatusDesc;
            List<string> operationalStatuss = statusQuery.ToList();
            return operationalStatuss;
        }

        public List<AssetTypeDDLModel> GetResetTypes()
        {
            return new SpecialActions().GetResetTypes();
        }

        public List<AssetTypeDDLModel> GetHasSensorTypes()
        {
            return new HasSensor().GetHasSensorTypes();
        }

        public List<AssetTypeDDLModel> GetAssetTypesDdlItems(int customerId)
        {

            var assetTypesQuery = from at in PemsEntities.AssetTypes
                                  where at.CustomerId == customerId && at.IsDisplay == true
                                  select new AssetTypeDDLModel { Name = at.MeterGroupDesc, Value = at.MeterGroupId };

            var assetTypes = assetTypesQuery.ToList();
            //var spaceType = new AssetTypeDDLModel { Name = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.Label, "Space"), Value = -1 };
            //assetTypes.Add(spaceType);
            return assetTypes;
        }


        public List<SelectListItemWrapper> GetReasonsForChange()
        {
            return (from apr in PemsEntities.AssetPendingReasons
                    select
                        new SelectListItemWrapper()
                        {
                            Text = apr.AssetPendingReasonDesc,
                            ValueInt = apr.AssetPendingReasonId
                        }).ToList();
        }


        #region Time Utilities

        protected DateTime SetToMidnight(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 001);
        }

        #endregion


        #region Greated/Last Updated On/By/Reason Methods

        protected void CreateAndLastUpdate(MeterViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            // Get most recent MeterAudit audit record.
            var audits = PemsEntities.MetersAudits.Where(m => m.CustomerID == model.CustomerId && m.AreaID == model.AreaId && m.MeterId == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get most recent MeterMapAudit for meter
            var meterMapAudits = PemsEntities.MeterMapAudits.Where(m => m.Customerid == model.CustomerId && m.Areaid == model.AreaId && m.MeterId == model.AssetId).OrderBy(m => m.AuditDateTime).ToList();
            // Get information from last record
            if (meterMapAudits.Any())
            {
                if (model.CreatedOn == null || meterMapAudits.First().AuditDateTime < model.CreatedOn)
                {
                    model.CreatedOn = meterMapAudits.First().AuditDateTime;
                    model.CreatedById = meterMapAudits.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || meterMapAudits.Last().AuditDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = meterMapAudits.Last().AuditDateTime;
                    model.LastUpdatedById = meterMapAudits.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(meterMapAudits.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the most recent VersionProfileMeterAudit for meter
            var versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.Where(m => m.CustomerId == model.CustomerId && m.AreaId == model.AreaId && m.MeterId == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (versionProfileMeterAudit.Any())
            {
                if (model.CreatedOn == null || versionProfileMeterAudit.First().UpdateDateTime < model.CreatedOn)
                {
                    model.CreatedOn = versionProfileMeterAudit.First().UpdateDateTime;
                    model.CreatedById = versionProfileMeterAudit.First().UserId;
                }
                if (model.LastUpdatedOn == null || versionProfileMeterAudit.Last().UpdateDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = versionProfileMeterAudit.Last().UpdateDateTime;
                    model.LastUpdatedById = versionProfileMeterAudit.Last().UserId;
                }
            }


            // Get the full-spell user name
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }

        protected void CreateAndLastUpdate(SensorViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            // Get most recent SensorsAudit audit record.
            var audits = PemsEntities.SensorsAudits.Where(m => m.CustomerID == model.CustomerId && m.SensorID == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get most recent MeterMapAudit for sensor
            var meterMapAudits = PemsEntities.MeterMapAudits.Where(m => m.Customerid == model.CustomerId && m.SensorID == model.AssetId).OrderBy(m => m.AuditDateTime).ToList();
            // Get information from last record
            if (meterMapAudits.Any())
            {
                if (model.CreatedOn == null || meterMapAudits.First().AuditDateTime < model.CreatedOn)
                {
                    model.CreatedOn = meterMapAudits.First().AuditDateTime;
                    model.CreatedById = meterMapAudits.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || meterMapAudits.Last().AuditDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = meterMapAudits.Last().AuditDateTime;
                    model.LastUpdatedById = meterMapAudits.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(meterMapAudits.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the most recent VersionProfileMeterAudit for sensor
            var versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.Where(m => m.CustomerId == model.CustomerId && m.SensorID == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (versionProfileMeterAudit.Any())
            {
                if (model.CreatedOn == null || versionProfileMeterAudit.First().UpdateDateTime < model.CreatedOn)
                {
                    model.CreatedOn = versionProfileMeterAudit.First().UpdateDateTime;
                    model.CreatedById = versionProfileMeterAudit.First().UserId;
                }
                if (model.LastUpdatedOn == null || versionProfileMeterAudit.Last().UpdateDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = versionProfileMeterAudit.Last().UpdateDateTime;
                    model.LastUpdatedById = versionProfileMeterAudit.Last().UserId;
                }
            }

            // Get the most recent SensorMappingAudit for sensor
            var sensorMappingAudits = PemsEntities.SensorMappingAudits.Where(m => m.CustomerID == model.CustomerId && m.SensorID == model.AssetId).ToList();
            // Get information from last record
            if (sensorMappingAudits.Any())
            {
                if (model.CreatedOn == null || sensorMappingAudits.First().ChangeDate < model.CreatedOn)
                {
                    model.CreatedOn = sensorMappingAudits.First().ChangeDate;
                    model.CreatedById = sensorMappingAudits.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || sensorMappingAudits.Last().ChangeDate > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = sensorMappingAudits.Last().ChangeDate;
                    model.LastUpdatedById = sensorMappingAudits.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(sensorMappingAudits.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the full-spell user name
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }

        protected void CreateAndLastUpdate(GatewayViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            // Get most recent SensorsAudit audit record.
            var audits = PemsEntities.GatewaysAudits.Where(m => m.CustomerID == model.CustomerId && m.GateWayID == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get most recent MeterMapAudit for gateway
            var meterMapAudits = PemsEntities.MeterMapAudits.Where(m => m.Customerid == model.CustomerId && m.GatewayID == model.AssetId).OrderBy(m => m.AuditDateTime).ToList();
            // Get information from last record
            if (meterMapAudits.Any())
            {
                if (model.CreatedOn == null || meterMapAudits.First().AuditDateTime < model.CreatedOn)
                {
                    model.CreatedOn = meterMapAudits.First().AuditDateTime;
                    model.CreatedById = meterMapAudits.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || meterMapAudits.Last().AuditDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = meterMapAudits.Last().AuditDateTime;
                    model.LastUpdatedById = meterMapAudits.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(meterMapAudits.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the most recent VersionProfileMeterAudit for gateway
            var versionProfileMeterAudit = PemsEntities.VersionProfileMeterAudits.Where(m => m.CustomerId == model.CustomerId && m.GatewayID == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (versionProfileMeterAudit.Any())
            {
                if (model.CreatedOn == null || versionProfileMeterAudit.First().UpdateDateTime < model.CreatedOn)
                {
                    model.CreatedOn = versionProfileMeterAudit.First().UpdateDateTime;
                    model.CreatedById = versionProfileMeterAudit.First().UserId;
                }
                if (model.LastUpdatedOn == null || versionProfileMeterAudit.Last().UpdateDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = versionProfileMeterAudit.Last().UpdateDateTime;
                    model.LastUpdatedById = versionProfileMeterAudit.Last().UserId;
                }
            }

            // Get the full-spell user name
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }

        protected void CreateAndLastUpdate(CashboxViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;

            // Get most recent CashBoxAudit audit record.
            //var audits = PemsEntities.CashBoxAudits.Where(m => m.CustomerID == model.CustomerId && m.CashBoxID == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            var audits = PemsEntities.CashBoxAudits.Where(m => m.CustomerID == model.CustomerId && m.CashBoxSeq == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get the full-spell user name
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }

        protected void CreateAndLastUpdate(SpaceViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            // Get most recent ParkingSpacesAudit audit record.
            var audits = PemsEntities.ParkingSpacesAudits.Where(m => m.CustomerID == model.CustomerId && m.ParkingSpaceId == model.AssetId).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get most recent ConfigProfileSpaceAudit for space
            var configProfileSpaceAuditList = PemsEntities.ConfigProfileSpaceAudits.Where(m => m.ParkingSpaceId == model.AssetId).ToList();
            // Get information from last record
            if (configProfileSpaceAuditList.Any())
            {
                if (model.CreatedOn == null || configProfileSpaceAuditList.First().AuditDatetime < model.CreatedOn)
                {
                    model.CreatedOn = configProfileSpaceAuditList.First().AuditDatetime;
                    model.CreatedById = configProfileSpaceAuditList.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || configProfileSpaceAuditList.Last().AuditDatetime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = configProfileSpaceAuditList.Last().AuditDatetime;
                    model.LastUpdatedById = configProfileSpaceAuditList.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(configProfileSpaceAuditList.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the full-spell user name
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }

        protected void CreateAndLastUpdate(MechanismViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            //todo - GTC: Mechanism Work - verify this is correct
            // Get most recent mechaudit audit record.
            var audits = PemsEntities.MechMasterAudits.Where(m => m.Customerid == model.CustomerId && m.MechId == model.MechIdNumber).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get most recent MeterMapAudit for mechanism
            var meterMapAudits = PemsEntities.MeterMapAudits.Where(m => m.Customerid == model.CustomerId && m.MechId == model.MechIdNumber).OrderBy(m => m.AuditDateTime).ToList();
            // Get information from last record
            if (meterMapAudits.Any())
            {
                if (model.CreatedOn == null || meterMapAudits.First().AuditDateTime < model.CreatedOn)
                {
                    model.CreatedOn = meterMapAudits.First().AuditDateTime;
                    model.CreatedById = meterMapAudits.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || meterMapAudits.Last().AuditDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = meterMapAudits.Last().AuditDateTime;
                    model.LastUpdatedById = meterMapAudits.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(meterMapAudits.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the full-spell user name
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }

        protected void CreateAndLastUpdate(DataKeyViewModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            //todo - GTC: DataKey Work - verify this is correct
            // Get most recent mechaudit audit record.
            var audits = PemsEntities.DataKeyAudits.Where(m => m.CustomerID == model.CustomerId && m.DataKeyId == model.DataKeyIdNumber).OrderBy(m => m.UpdateDateTime).ToList();
            // Get information from last record
            if (audits.Any())
            {
                model.CreatedOn = audits.First().UpdateDateTime;
                model.CreatedById = audits.First().UserId;
                model.LastUpdatedOn = audits.Last().UpdateDateTime;
                model.LastUpdatedById = audits.Last().UserId;
                model.LastUpdatedReason = (AssetPendingReasonType)(audits.Last().AssetPendingReasonId ?? 0);
            }

            // Get most recent MeterMapAudit for mechanism
            var meterMapAudits = PemsEntities.MeterMapAudits.Where(m => m.Customerid == model.CustomerId && m.DataKeyId == model.DataKeyIdNumber).OrderBy(m => m.AuditDateTime).ToList();
            // Get information from last record
            if (meterMapAudits.Any())
            {
                if (model.CreatedOn == null || meterMapAudits.First().AuditDateTime < model.CreatedOn)
                {
                    model.CreatedOn = meterMapAudits.First().AuditDateTime;
                    model.CreatedById = meterMapAudits.First().UserId ?? 0;
                }
                if (model.LastUpdatedOn == null || meterMapAudits.Last().AuditDateTime > model.LastUpdatedOn)
                {
                    model.LastUpdatedOn = meterMapAudits.Last().AuditDateTime;
                    model.LastUpdatedById = meterMapAudits.Last().UserId ?? 0;
                    model.LastUpdatedReason = (AssetPendingReasonType)(meterMapAudits.Last().AssetPendingReasonId ?? (int)model.LastUpdatedReason);
                }
            }

            // Get the full-spell user name
            model.CreatedBy = (new UserFactory()).GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = (new UserFactory()).GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            model.LastUpdatedReasonDisplay = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason).AssetPendingReasonDesc;
        }


        protected void CreateAndLastUpdate(AssetBaseModel model)
        {
            model.CreatedOn = null;
            model.CreatedById = 0;
            model.LastUpdatedOn = null;
            model.LastUpdatedById = 0;
            model.LastUpdatedReason = AssetPendingReasonType.General;

            // Get the full-spell user name
            var userFactory = new UserFactory();

            model.CreatedBy = userFactory.GetUserById(model.CreatedById).FullName();
            model.LastUpdatedBy = userFactory.GetUserById(model.LastUpdatedById).FullName();

            // Get the full-spell change reason.
            var pendingReason = PemsEntities.AssetPendingReasons.FirstOrDefault(m => m.AssetPendingReasonId == (int)model.LastUpdatedReason);
            if (pendingReason != null)
                model.LastUpdatedReasonDisplay = pendingReason.AssetPendingReasonDesc;
        }

        #endregion

        /// <summary>
        /// Gets a list of distinct customer/area/meter id in the assets table
        /// returns a lsit of asset identifiers that represent these items.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public List<AssetIdentifier> GetAllAssetsForCustomer(int customerId)
        {
            var items = new List<AssetIdentifier>();
            //get all distinct items out of hte meters table (area, meter, and customer Id)

            var distinctItems = PemsEntities.Meters.Where(x => x.CustomerID == customerId).Select(y => new { y.MeterId, y.AreaID }).Distinct();

            if (distinctItems.Any())
            {
                items.AddRange(distinctItems.Select(distinctItem => new AssetIdentifier
                {
                    AreaId = distinctItem.AreaID,
                    AssetId = distinctItem.MeterId,
                    CustomerId = customerId
                }));
            }

            return items;
        }

        public Meter GetAsset(int customerId, int areaId, int meterId)
        {
            var asset =
                PemsEntities.Meters.FirstOrDefault(
                    x => x.AreaID == areaId && x.CustomerID == customerId && x.MeterId == meterId);

            return asset;
        }


        public List<AssetIdentifier> GetWorkOrderAssetsForCustomer(int customerId, bool includeMeterMapInfo = true)
        {
            var items = GetAllAssetsForCustomer(customerId);
            //now for each one we need to populate the area name field. 
            //meter > metermap >areaid2 > area > name

            if (includeMeterMapInfo)
            {
                foreach (var item in items)
                {
                    //fix up the areaname to use areaid2
                    var meterMap =
                        PemsEntities.MeterMaps.FirstOrDefault(
                            m => m.Customerid == customerId && m.Areaid == item.AreaId && m.MeterId == item.AssetId);
                    if (meterMap != null)
                    {
                        var area =
                            PemsEntities.Areas.FirstOrDefault(
                                m => m.CustomerID == customerId && m.AreaID == meterMap.AreaId2);
                        item.AreaName = area == null ? "" : area.AreaName;
                    }
                }

            }
            return items;
        }

        /// <summary>
        /// Gets the name of the area assigned to the AreaId2 field for an asset by passing in the original customer id, meter id, and the AreadId on the asset (from the meters table)
        /// The areaid passed in is NOT the areaID2, but the areaid on the asset (meter table)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="areaId"></param>
        /// <param name="meterId"></param>
        /// <returns></returns>
        public string GetAssetAreaName(int customerId, int areaId, int meterId)
        {
            var meterMap = PemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == customerId && m.Areaid == areaId && m.MeterId == meterId);
            if (meterMap != null)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == customerId && m.AreaID == meterMap.AreaId2);
                return area == null ? "" : area.AreaName;
            }
            return string.Empty;
        }

        public void SetPropertiesOfPendingAssetModel( AssetPending assetPending, AssetBaseModel model)
        {
            // Determine which pending changes are valid.
            if (assetPending.AssetModel.HasValue)
                model.AssetModel = GetMechanismDescription(model.CustomerId, assetPending.AssetModel.Value);

            if (!string.IsNullOrWhiteSpace(assetPending.AssetName))
                model.Name = assetPending.AssetName;

            if (!string.IsNullOrWhiteSpace(assetPending.LocationMeters))
                model.Street = assetPending.LocationMeters;
            if (!string.IsNullOrWhiteSpace(assetPending.LocationGateway))
                model.Street = assetPending.LocationGateway;
            if (!string.IsNullOrWhiteSpace(assetPending.LocationSensors))
                model.Street = assetPending.LocationSensors;

            // Area, Zone and Suburb
            if (assetPending.MeterMapAreaId2.HasValue)
            {
                var area = PemsEntities.Areas.FirstOrDefault(m => m.CustomerID == model.CustomerId && m.AreaID == assetPending.MeterMapAreaId2.Value);
                model.Area = area == null ? "" : area.AreaName;
                model.AreaId2 = area == null ? 0 : area.AreaID;
            }

            if (assetPending.MeterMapZoneId.HasValue)
            {
                var zone =PemsEntities.Zones.FirstOrDefault(m => m.ZoneId == assetPending.MeterMapZoneId.Value && m.customerID == model.CustomerId);
                model.Zone = zone == null ? "" : zone.ZoneName ?? "";
            }

            if (assetPending.MeterMapSuburbId.HasValue)
            {
                var suburb =PemsEntities.CustomGroup1.FirstOrDefault(m => m.CustomGroupId == assetPending.MeterMapSuburbId.Value && m.CustomerId == model.CustomerId);
                model.Suburb = suburb == null ? "" : suburb.DisplayName ?? "";
            }

            if (assetPending.MeterMapMechId.HasValue)
            {
                var mechanism = PemsEntities.MechMasters.FirstOrDefault(m => m.MechIdNumber == assetPending.MeterMapMechId.Value && m.Customerid == model.CustomerId);
                //todo -  GTC: Mechanism Work - add mechanism name here
                model.Mechanism = mechanism == null ? "" : "todo - db";// mechanism.Name ?? "";
                model.MechanismId = assetPending.MeterMapMechId;
            }

            if (assetPending.MeterMapDataKeyId.HasValue)
            {
                var datakey = PemsEntities.DataKeys.FirstOrDefault(m => m.DataKeyIdNumber == assetPending.MeterMapDataKeyId.Value && m.CustomerID == model.CustomerId);
                //todo -  GTC: Mechanism Work - add datakey name here
                model.Mechanism = datakey == null ? "" : "todo - db";// datakey.Name ?? "";
                model.MechanismId = assetPending.MeterMapDataKeyId;
            }

            // Lat/Long
            if (assetPending.Latitude.HasValue)
                model.Latitude = assetPending.Latitude.Value;

            if (assetPending.Longitude.HasValue)
                model.Longitude = assetPending.Longitude.Value;

            if (assetPending.MeterMapMaintenanceRoute.HasValue)
                model.MaintenanceRoute =PemsEntities.MaintRoutes.FirstOrDefault(m => m.CustomerId == model.CustomerId&& m.MaintRouteId == assetPending.MeterMapMaintenanceRoute.Value).DisplayName ?? "-";

            if (assetPending.LastPreventativeMaintenance.HasValue)
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.Value.ToShortDateString();

            if (assetPending.NextPreventativeMaintenance.HasValue)
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance.Value.ToShortDateString();

            // Operational status of meter.
            if (assetPending.OperationalStatus.HasValue)
            {
                model.Status = PemsEntities.OperationalStatus.FirstOrDefault(m => m.OperationalStatusId == assetPending.OperationalStatus.Value).OperationalStatusDesc;
                model.StatusDate = assetPending.OperationalStatusTime;
            }
            
            // Get target ActivationDate
            model.ActivationDate = assetPending.RecordMigrationDateTime.HasValue ? assetPending.RecordMigrationDateTime.Value.ToShortDateString() : "";
        }

        protected void SetPropertiesOfPendingAssetEditModel(AssetPending assetPending, AssetEditBaseModel model)
        {
            if (model == null)
                return;
            model.StateId = (int)AssetStateType.Pending;
            if (assetPending == null)
                return;

            model.AssetModelId = assetPending.AssetModel ?? model.AssetModelId;

            if (!string.IsNullOrWhiteSpace(assetPending.AssetName))
                model.Name = assetPending.AssetName;

            if (assetPending.LastPreventativeMaintenance.HasValue)
                model.LastPrevMaint = assetPending.LastPreventativeMaintenance.Value.ToShortDateString();
            if (assetPending.NextPreventativeMaintenance != null)
                model.NextPrevMaint = assetPending.NextPreventativeMaintenance;
            if (assetPending.WarrantyExpiration != null)
                model.WarrantyExpiration = assetPending.WarrantyExpiration;
            if (assetPending.OperationalStatus != null)
                GetOperationalStatusDetails(model, assetPending.OperationalStatus.Value, assetPending.OperationalStatusTime.HasValue ? assetPending.OperationalStatusTime.Value : DateTime.Now);

            model.ActivationDate = assetPending.RecordMigrationDateTime.HasValue
                ? assetPending.RecordMigrationDateTime.Value
                : Now;
            model.ActivationDate = SetToMidnight(model.ActivationDate.Value);
        }

        protected void SetPropertiesOfAssetMassiveEditModel(AssetMassUpdateBaseModel model, List<AssetIdentifier> assetIds)
        {
            model.SetList(assetIds);
            model.StateId = -1;
            GetAssetStateList(model);
            model.ActivationDate = SetToMidnight(Now);
        }

        /// <summary>
        /// Gets the customer specific mechanism description. If none is set, it will automatically populate it based on the corrosponding Mechanism Master.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="mechanismId"></param>
        /// <returns></returns>
        public string GetMechanismDescription(int customerId, int mechanismId)
        {

            //get the mech master and customer specific version
            var mechanismMasterCustomer = PemsEntities.MechanismMasterCustomers.FirstOrDefault(m => m.CustomerId == customerId && m.MechanismId == mechanismId);
            var mechanismMaster = PemsEntities.MechanismMasters.FirstOrDefault(x => x.MechanismId == mechanismId);

            if (mechanismMaster != null)
            {
                //if it doesnt exist for the customer, insert it into the customer and return that value, insert one for this customer and return that value
                if (mechanismMasterCustomer == null)
                {
                    mechanismMasterCustomer = new MechanismMasterCustomer()
                    {
                        CustomerId = customerId,
                        IsDisplay = true,
                        MechanismId = mechanismMaster.MechanismId,
                        MechanismDesc = mechanismMaster.MechanismDesc,
                        PreventativeMaintenanceScheduleDays = 180,
                        SLAMinutes = 300
                    };
                    PemsEntities.MechanismMasterCustomers.Add(mechanismMasterCustomer);
                    //now return the value of the mech master
                    return mechanismMaster.MechanismDesc;
                }

                //if it does exist but the description is empty
                if (!string.IsNullOrEmpty(mechanismMasterCustomer.MechanismDesc))
                {
                    mechanismMasterCustomer.MechanismDesc = mechanismMaster.MechanismDesc;
                    PemsEntities.SaveChanges();
                    return mechanismMasterCustomer.MechanismDesc;
                }

                //it exist and has a value, lets use that
                return mechanismMasterCustomer.MechanismDesc;
            }

            //the mechanism master doesnt exist, return nothing
            return string.Empty;
        }

        public string GetMechanismLocation(int customerId, int mechanismId)
        {
            const string defaultLocation = "Inventory";

            var meterMap =
                PemsEntities.MeterMaps.FirstOrDefault(
                    m => m.Customerid == customerId && m.MechId == mechanismId && m.Areaid == 1);
            if (meterMap == null)
                return defaultLocation;
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == meterMap.MeterId);
            return meter == null ? defaultLocation : meter.Location;
        }

        public string GetDataKeyLocation(int customerId, int dataKeyId)
        {
            const string defaultLocation = "Inventory";

            var meterMap =
                PemsEntities.MeterMaps.FirstOrDefault(
                    m => m.Customerid == customerId && m.DataKeyId == dataKeyId && m.Areaid == 1);
            if (meterMap == null)
                return defaultLocation;
            var meter = PemsEntities.Meters.FirstOrDefault(m => m.MeterId == meterMap.MeterId);
            return meter == null ? defaultLocation : meter.Location;
        }

    }
}
