/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________        __________________________________________________________________________________________________
 * 02/06/2014       R Howard                    JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Technicians;
using Duncan.PEMS.Entities.WorkOrders.Dispatcher;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using LINQtoCSV;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.WorkOrders
{
    /// <summary>
    /// Factory that handles all of the data access and business rules for Work orders for the Dispatch screens
    /// this class has different logic for most method implementation, so needs to be seperate. the mobile screens will use the mobile work order factory and the dispatcher will use these screens
    /// </summary>
    public class DispatcherWorkOrderFactory : BaseWorkOrderFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Represents the current maintenance group the user is logged in as (as a dispatcher)
        /// </summary>
        private PemsCity _maintenanceGroup { get; set; }
        public PemsCity MaintenanceGroup
        {
            get{return _maintenanceGroup;}
            set{_maintenanceGroup = value;}
        }


        /// <summary>
        /// Represents the lsit of cities for this maintenance group.
        /// </summary>
        private List<PemsCity> _maintenanceGroupCustomers { get; set; }
        public List<PemsCity> MaintenanceGroupCustomers
        {
            get{return _maintenanceGroupCustomers;}
            set{_maintenanceGroupCustomers = value;}
        }

        /// <summary>
        /// force this constructor, so they have to be working in the context of a specific city.
        ///  All work orders  should be city specific. 
        /// this allows us to iterate through all the work orders for a entire group if we have to
        /// </summary>
        /// <param name="currentMaintenanceGroup"></param>
        public DispatcherWorkOrderFactory(PemsCity currentMaintenanceGroup)
       {
           MaintenanceGroup = currentMaintenanceGroup;
            MaintenanceGroupCustomers = currentMaintenanceGroup.MaintenanceCustomers;
            MaintenanceConnectionStringName = currentMaintenanceGroup.MaintenanceConnectionStringName;
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerConnectionString"></param>
        /// <param name="maintenanceGroupConnectionString"></param>
        public DispatcherWorkOrderFactory(string customerConnectionString, string maintenanceGroupConnectionString)
        {
            MaintenanceConnectionStringName = maintenanceGroupConnectionString;
            ConnectionStringName = customerConnectionString;
        }

        #region Work Order Creation

        /// <summary>
        /// Creates a work order in the system for a mobile user (technician)
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="faultDescription"></param>
        /// <param name="notes"></param>
        /// <param name="customerId"></param>
        /// <param name="areaId"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public long CreateWorkOrder(int assetId, string faultDescription, int customerId, int areaId,
                                    DateTime localTime, string crossStreet, string notes)
        {
            //we have to check to see if htey asset already has a work order. if it does, use that work order. This includes any open, incomplete or rejected work orders.
            //we add it to any open work order, even if it is assigned ot another technician (in practice this will almost never happen)
            var existingWorkOrder =
                MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.MeterId == assetId && 
                    (x.WorkOrderStatusId == (int)WorkOrderStatus.Open ||
                    x.WorkOrderStatusId == (int)WorkOrderStatus.Incomplete 
                    || x.WorkOrderStatusId == (int)WorkOrderStatus.Rejected));
            if (existingWorkOrder != null)
                return existingWorkOrder.WorkOrderId;
            //first we have to go to PEMS db to get the asset
            var assetFactory = (new AssetFactory(ConnectionStringName));

            var asset = assetFactory.GetAsset(customerId, areaId, assetId);
            //fail state
            if (asset == null)
                return -1;

            //create the work order now. commented out properties we ignore upon creation.
            var fmWorkOrder = new FMWorkOrder
                {
                    AreaId = areaId,
                  //  AssignedDate = localTime, - not set until it is assigned
                    CreateDateTime = localTime,
                    CreatedById = WebSecurity.CurrentUserId,
                    CrossStreet = crossStreet,
                    CustomerId = customerId,
                    HighestSeverity = 0,
                    Location = asset.Location,
                    Mechanism = asset.MeterType,
                    MeterGroup = asset.MeterGroup ?? 1,
                    MeterId = assetId,
                    Notes = notes,
                    ReportingUserId = WebSecurity.CurrentUserId,
                    SLADue = localTime.AddHours(2),
                    //TechnicianId = WebSecurity.CurrentUserId, - assign it to nothing since the dispatch is creating this
                    WorkOrderStatusId = (int) WorkOrderStatus.Open,
                    AssignmentState =  (int)AssignmentState.Unassigned,
                    ZoneId =
                        asset.MeterMaps.FirstOrDefault() != null ? asset.MeterMaps.FirstOrDefault().ZoneId : (int?) null
                };
            fmWorkOrder.AssignmentState = (int) AssignmentState.Unassigned;
            //fmWorkOrder.CompletedDate; //not set here, set upon resolution
            // fmWorkOrder.ParkingSpaceId; //not used, dont have to set parkingspace ID
            //fmWorkOrder.ResolutionCode; - nothing on creation, when resolving, these are maintenance codes
            //fmWorkOrder.ResolutionDesc; -  nothing on creation, when resolving, these are maintenance codes
            MaintenanceEntities.FMWorkOrders.Add(fmWorkOrder);
            MaintenanceEntities.SaveChanges();
            return fmWorkOrder.WorkOrderId;
        }
        #endregion

        #region Inquiry Page Filters

        /// <summary>
        /// Gets all of the unique asset types for all of the customers for teh current maintenance group
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetWorkOrderAssetTypes()
        {
            var ddlItems = new List<SelectListItem>();
            //for each customer
            foreach (var maintenanceGroupCustomer in MaintenanceGroupCustomers)
            {
                using (var pemsEntities = new PEMEntities(maintenanceGroupCustomer.PemsConnectionStringName))
                {
                    //get this customer asset types
                    var assetTypes =
                        pemsEntities.AssetTypes.Where(
                            x => x.CustomerId == maintenanceGroupCustomer.Id && x.IsDisplay == true);
                    if (assetTypes.Any())
                    {
                        foreach (var assetType in assetTypes)
                        {
                            //dont add it twice
                            if (ddlItems.All(x => x.Text != assetType.MeterGroupDesc))
                                ddlItems.Add(new SelectListItem {Text = assetType.MeterGroupDesc});
                        }
                    }
                }
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets all of the unique assignment states for the maintenance group customers
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetWorkOrderAssignmentStates()
        {
            var ddlItems = new List<SelectListItem>();
            var factory = new ResourceFactory();
            ddlItems.Add(new SelectListItem { Text = factory.GetLocalizedTitle(ResourceTypes.Label, "Assigned"), Value = ((int)AssignmentState.Assigned).ToString() });
            ddlItems.Add(new SelectListItem { Text = factory.GetLocalizedTitle(ResourceTypes.Label, "Suspended"), Value = ((int)AssignmentState.Suspended).ToString() });
            ddlItems.Add(new SelectListItem { Text = factory.GetLocalizedTitle(ResourceTypes.Label, "Unassigned"), Value = ((int)AssignmentState.Unassigned).ToString() });
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of all the technicians for a maintenance group
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetWorkOrderTechnicians()
        {
            //get all the users for the current maintenance group that have the flag of IsTechnician set.
            var ddlItems = new List<SelectListItem>();

            var techs = GetMaintenanceGroupTechnicians(MaintenanceGroup);

            //for each profile in this city (the current maintenance group)
            foreach (var tech in techs)
            {
                ddlItems.Add(new SelectListItem { Text = tech.TechnicianName , Value =tech.TechnicianName });
            }
            return ddlItems;
        }

        #endregion

        #region Work Order Listing / Details

        /// <summary>
        /// Gets all of hte work orders for the dispatch inquiry screen. Filters on eventcodeID if it is passed in.
        /// </summary>
        /// <returns></returns>
        public List<DispatcherWorkOrderListModel> GetWorkOrders(int eventCodeId = -1)
        {
            var workOrders = new List<DispatcherWorkOrderListModel>();
            //get all the work orders that are part of any of the maintenance group customers
            var maintCustomerIds = MaintenanceGroupCustomers.Select(x => x.Id).ToList();
            var fmWorkOrders = MaintenanceEntities.FMWorkOrders.Where(x => maintCustomerIds.Any(y => y == x.CustomerId));

            //filter by event codes here
            if (eventCodeId > -1)
                fmWorkOrders = fmWorkOrders.Where(x => x.WorkOrderEvents.Any(y => y.EventCode == eventCodeId));

            foreach (var fmworkOrder in fmWorkOrders)
            {
                //get the asset factory that is specific to this customer
                var customer = MaintenanceGroupCustomers.FirstOrDefault(x => x.Id == fmworkOrder.CustomerId);
                //pems entities for this customer - used for assets, meters, etc anything we have to go back to pems to get
                using (var pemsEntities = new PEMEntities(ConnectionStringName))
                {
                    var workOrder = new DispatcherWorkOrderListModel();

                    //first try to get it out of the asset type table for this customer
                    var assetType = pemsEntities.AssetTypes.FirstOrDefault(x => x.MeterGroupId == fmworkOrder.MeterGroup && x.CustomerId == fmworkOrder.CustomerId );
                    if (assetType != null)
                        //check to make sure we are supposed to be displaying this field. if not, return nothing
                        workOrder.AssetType = assetType.IsDisplay == false ? string.Empty : assetType.MeterGroupDesc;
                  
                    //Direct work order fields 
                    workOrder.AssignedDate = fmworkOrder.AssignedDate;
                    workOrder.AssignmentState = fmworkOrder.AssignmentState;
                    workOrder.CompletedDate = fmworkOrder.CompletedDate;
                    workOrder.CreationDate = fmworkOrder.CreateDateTime;
                    workOrder.Deadline = fmworkOrder.SLADue;
                    workOrder.EventCount = fmworkOrder.WorkOrderEvents.Count();
                    workOrder.WorkOrderId = fmworkOrder.WorkOrderId;
                    workOrder.WorkOrderState = fmworkOrder.WorkOrderStatu.WorkOrderStatusDesc;
                    workOrder.Street = fmworkOrder.Location ?? string.Empty;
                    workOrder.Customer = customer.DisplayName ?? string.Empty; //assuming this customer exist, otherwise we wouldnt be here
                    workOrder.AssetId = fmworkOrder.MeterId;
                   
                    //Highest Priority
                    workOrder.HighestPriorityId = fmworkOrder.HighestSeverity;
                    var tier = pemsEntities.AlarmTiers.FirstOrDefault(x => x.Tier == fmworkOrder.HighestSeverity);
                    workOrder.HighestPriorityDisplay = tier != null? tier.TierDesc: "";
                  
                    //asset location - get meter map to get this information
                    var meterMap = pemsEntities.MeterMaps.FirstOrDefault(m => m.Customerid == fmworkOrder.CustomerId && m.Areaid == fmworkOrder.AreaId && m.MeterId == fmworkOrder.MeterId);
                    if (meterMap != null)
                    {
                        //get the area name based ont he areaid2 field
                        var area = pemsEntities.Areas.FirstOrDefault(m => m.CustomerID == fmworkOrder.CustomerId && m.AreaID == meterMap.AreaId2);
                        workOrder.Area = area == null ? "" : area.AreaName;

                        //use FK relationshit for zone
                        workOrder.Zone =  meterMap.Zone == null ? "" :  meterMap.Zone.ZoneName;
                        
                        //suburb - use customgroup1
                       var suburb =  pemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == meterMap.CustomGroup1 && x.CustomerId == fmworkOrder.CustomerId);
                        workOrder.Suburb = suburb == null ? " " : suburb.DisplayName;
                    }

                   
                    //Technician
                    var dbTech = pemsEntities.TechnicianDetails.FirstOrDefault(x => x.TechnicianId == fmworkOrder.TechnicianId);
                    workOrder.Technician = dbTech == null ? "" : dbTech.Name;

                    workOrders.Add(workOrder);
                }
            }
            
         


            return workOrders;
        }

        /// <summary>
        /// Gets the mobile version of the work order details screen
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="customerId"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public DispatcherWorkOrderDetailsModel GetWorkOrderDetails(long workOrderId, DateTime localTime)
        {
            var model = new DispatcherWorkOrderDetailsModel();
            //get the work order
            var workOrder = MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
            if (workOrder != null)
            {
                var customerId = workOrder.CustomerId;
                //convert it to our enetity
                model.Deadline = workOrder.SLADue;
                model.Location = workOrder.Location;
                model.Status = workOrder.WorkOrderStatu.WorkOrderStatusDesc;
                model.StatusId = workOrder.WorkOrderStatusId;
                model.WorkOrderId = workOrder.WorkOrderId;
                model.Notes = workOrder.Notes;
                model.CreationDate = workOrder.CreateDateTime;
                model.CrossStreet = workOrder.CrossStreet;
                model.AreaId = workOrder.AreaId;
                model.AreaId2Display = GetAreaName(customerId, workOrder.AreaId, workOrder.MeterId);

                //create the asset for this work order
                model.WorkOrderAsset = MakeWorkOrderAsset(workOrder, customerId);

                //create a list of images
                model.WorkOrderImages = MakeWorkOrderImages(workOrder.WorkOrderImages);

                //create a list of events
                model.WorkOrderEvents = MakeWorkOrderEvents(workOrder.WorkOrderEvents);

                //Repair Descriptions (used for closing the work order)
                model.RepairDescriptions = GetRepairDescriptions();

                //available parts for this work order (based ont he asset id - metergroup of hte work order)
                model.AvailableParts = GetAvailableParts(workOrder.MeterGroup, workOrder.Mechanism);

                //list of parts assigned to this work order
                model.WorkOrderParts = MakeWorkOrderParts(workOrder.WorkOrderParts);

                //highest severity of all the work order events
                model.Priority = workOrder.HighestSeverity;

                //fix up alarm tier (priority) display - have to go back to pems for this info.
                var tier = PemsEntities.AlarmTiers.FirstOrDefault(x => x.Tier == workOrder.HighestSeverity);
                model.PriorityDisplay = tier != null? tier.TierDesc: "";

                //list of all past work orders inthe last 14 days (this migh have to be configurable per client todo - make this configurable)
                var pastDatetime = localTime.AddDays(-14);
                var pastWorkOrders =
                    MaintenanceEntities.FMWorkOrders.Where(
                        x =>
                        x.MeterId == workOrder.MeterId && x.WorkOrderId != workOrderId &&
                        x.CreateDateTime >= pastDatetime);
                model.PastWorkOrders = pastWorkOrders.Any() ? pastWorkOrders.Count().ToString() : "0";

                //set the flag based on the current customer to determine if they need to close the entire work order out or per events
                var settingFactory = new SettingsFactory();
                var workOrderCloseEventSetting = settingFactory.GetValue("CloseWorkOrderEvents", customerId);
                //if it doenst exist, create it
                if (workOrderCloseEventSetting == null)
                    settingFactory.Set(customerId, "CloseWorkOrderEvents", false.ToString());
                workOrderCloseEventSetting = settingFactory.GetValue("CloseWorkOrderEvents", customerId);
                model.CloseWorkOrderEvents = bool.Parse(workOrderCloseEventSetting);

                //technicina details
                model.Technician = MakeTechnicianFromWorkOrder(workOrder);

                //get all the techs in the system
                model.AvailableTechnicians = GetMaintenanceGroupTechnicians(MaintenanceGroup);

            }
            return model;
        }
        
        #endregion

        #region Technicians
        public List<Technician> GetWorkLoadListingModel()
        {
            //get a list of unique technicians for all fo the customers for the current maintenance group
            var techs = GetMaintenanceGroupTechnicians(MaintenanceGroup);
            foreach (var tech in techs)
            {
                    //fix up the completed for today
                    //get a count of completed work orders today (used int he work load screen)
                //get completed work orders
                var completedWorkOrders =MaintenanceEntities.FMWorkOrders.Where(x => x.TechnicianId == tech.TechnicianID && x.WorkOrderStatusId == (int) WorkOrderStatus.Closed).ToList();
                tech.TechnicianCompletedWorkOrderCount = completedWorkOrders.Count(x => x.CompletedDate != null && x.CompletedDate.Value.DayOfYear == MaintenanceGroup.LocalTime.DayOfYear);
            }
            return techs;
        }

        /// <summary>
        /// Gets a list of unique technicians for all the customers for a specific maintenance group.
        /// Filters by cutomer access as well.
        /// </summary>
        /// <param name="maintGroup"></param>
        /// <returns></returns>
        public List<Technician> GetMaintenanceGroupTechnicians(PemsCity maintGroup)
        {
            var allTechs = new List<Technician>();
            var ucaManager = new UserCustomerAccessManager();

            foreach (var maintenanceCustomer in maintGroup.MaintenanceCustomers)
            {
                var allTechsForCustomer =
                    (new DispatcherWorkOrderFactory(maintenanceCustomer.PemsConnectionStringName,
                                                    maintGroup.MaintenanceConnectionStringName)).MakeAvailableTechnicians();
                foreach (var techForCustomer in allTechsForCustomer)
                    if (allTechs.All(x => x.TechnicianID != techForCustomer.TechnicianID))
                        allTechs.Add(techForCustomer);
            }

            //now that we have all the unique technicians for all the maintneannce customers,
            //we need to make sure they have access (since the techdetails brought back is for the data source and is not customer specific.
            //we do this by checking the UserCustomerAccess table
            //call the GetCustomersIds method and check to see if they have access to the maintenancecustomers for this maintneance group.
            var filteredTechsWithAccess = new List<Technician>();
            var maintCustomerIds = maintGroup.MaintenanceCustomers.Select(x => x.Id).ToList();

            //assuming there is only unique techs in this list
            foreach (var technician in allTechs)
            {
                var customerIds = ucaManager.GetCustomersIds(technician.TechnicianID);
                bool hasMatch = customerIds.Any(x => maintCustomerIds.Any(y => y == x));
                if (hasMatch)
                    filteredTechsWithAccess.Add(technician);
            }
            return filteredTechsWithAccess;
        }
        #endregion

        #region Work Order Events
        /// <summary>
        /// Gets a work order event.
        /// </summary>
        /// <param name="workOrderEventId"></param>
        /// <returns></returns>
        public DispatcherEventDetailsModel GetWorkOrderEvent(int workOrderEventId)
        {
            //get the work order event
            var fmWorkOrderEvent =
                MaintenanceEntities.FMWorkOrderEvents.FirstOrDefault(x => x.WorkOrderEventId == workOrderEventId);
            if (fmWorkOrderEvent == null)
                return null;

            //now get the work order for this event, make the asset, work order event, and images for this work order and work order event.
            var eventModel = new DispatcherEventDetailsModel();
            eventModel.WorkOrderAsset = MakeWorkOrderAsset(fmWorkOrderEvent.WorkOrder, fmWorkOrderEvent.WorkOrder.CustomerId);
            eventModel.WorkOrderEvent = MakeWorkOrderEvent(fmWorkOrderEvent);
            eventModel.WorkOrderImages = MakeWorkOrderImages(fmWorkOrderEvent.WorkOrder.WorkOrderImages);

            //assumption is that the event source id is 10 (PEMS). So we need to default it to PEMS and then get based on the eventsourceid property of the base factory, see if its int he pems db
            eventModel.Source = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.Glossary, "PEMS");

            //now get it from pems to see what the description should be (in case it changed
            var eventSource = PemsEntities.EventSources.FirstOrDefault(x => x.EventSourceCode == WorkOrderEventSourceId);
            if (eventSource != null)
                eventModel.Source = eventSource.EventSourceDesc;

            //now we need to set the dispatch specific stuff (repari code, etc)
            eventModel.RepairCode = fmWorkOrderEvent.WorkOrder.ResolutionCode;
            eventModel.RepairDescription = fmWorkOrderEvent.WorkOrder.ResolutionDesc;
            //go get the technician that closed the work order
            var technician = RbacEntities.UserProfiles.FirstOrDefault(x => x.UserId == fmWorkOrderEvent.WorkOrder.TechnicianId);
            if (technician != null)
                eventModel.ClosedBy = technician.UserName;
            
            eventModel.ClosureNotes = fmWorkOrderEvent.WorkOrder.Notes;
            eventModel.IsVandalism = fmWorkOrderEvent.WorkOrder.ResolutionCode == null ? (bool?) null : fmWorkOrderEvent.Vandalism;
            eventModel.TimeCleared = fmWorkOrderEvent.WorkOrder.CompletedDate;
            return eventModel;
        }
        #endregion

        #region Helpers / Filters
        
        /// <summary>
        /// Gets all the meter groups in the system for the parts index page.
        /// Need to use the connection string instantiation here and not the pems city constructor
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllMeterGroups()
        {
            //get all meter gorups int he system. use the default connection string when instantiating the factory
            var list = new List<SelectListItem>();
            var meterGroups = PemsEntities.MeterGroups.ToList();
            foreach (var meterGroup in meterGroups)
            {
               list.Add(new SelectListItem()
                    {
                        Selected = false,
                        Text = meterGroup.MeterGroupDesc,
                        Value = meterGroup.MeterGroupId.ToString()
                    });
            }
            return list;
        }

        /// <summary>
        /// Gets all fo the mechanism masters in the system. need to use the connection string constructors isntead of the pemscity constructor
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllMechanisms()
        {
            //get all meter gorups int he system. use the default connection string when instantiating the factory
            var list = new List<SelectListItem>();
            var items = PemsEntities.MechanismMasters.ToList();
            foreach (var item in items)
            {
                list.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = item.MechanismDesc,
                    Value = item.MechanismId.ToString()
                });
            }
            return list;
        }

        #endregion

        #region Upload (Parts)
        private const string ConstraintPad = "  ";
        private const string ConstraintEmpty = "[empty record]";
        private const string ConstraintReturn = "\n";
        private const string ConstraintSpacing = "\n\n";
        private const string FieldRequired = " - Required";
        private const string FieldOptional = " - Optional";

        /// <summary>
        /// Gets a list of constraints for  aparts import
        /// </summary>
        /// <returns></returns>
        public string PartsConstraintsList()
        {
            var sb = new StringBuilder();
            //metergroup
            sb.Append("Field: MeterGroup").Append(ConstraintReturn);
            var metergroups = from mm in PemsEntities.MeterGroups select mm;
            foreach (var row in metergroups)
                sb.Append(ConstraintPad).Append(row.MeterGroupDesc +": " + row.MeterGroupId).Append(ConstraintReturn);
            sb.Append(ConstraintSpacing);
            
            //category
            sb.Append("Field: Category").Append(ConstraintReturn);
            var mechanisms = from mm in PemsEntities.MechanismMasters select mm;
            foreach (var row in mechanisms)
                sb.Append(ConstraintPad).Append(row.MechanismDesc + ": " + row.MechanismId ).Append(ConstraintReturn);
            sb.Append(ConstraintSpacing);
            
            //status
            sb.Append("Field: Status").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Active: " + 1).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("InActive: " + 0).Append(ConstraintReturn);
            sb.Append(ConstraintSpacing);
            return sb.ToString();
        }

        /// <summary>
        /// Gets a list of fields for a parts inport
        /// </summary>
        /// <returns></returns>
        public string PartsFieldsList()
        {
            var sb = new StringBuilder();
            sb.Append("Field Notes:").Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Part Name").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Meter Group").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Category").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Part Description").Append(FieldOptional).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Cost In Cents").Append(" - Will default to 0 if not set").Append(FieldRequired).Append(ConstraintReturn);
            sb.Append(ConstraintPad).Append("Status").Append(FieldRequired).Append(ConstraintReturn);
            return sb.ToString();
        }

        //UploadParts
        private CsvContext _csvContext;
        private CsvFileDescription _csvFileDescription;
        public DispatcherPartUploadResultModel ProcessPartImport(string file)
        {
            var model = new DispatcherPartUploadResultModel()
            {
                UploadedFileName = file
            };
            try
            {
               
                // Prep the LINQtoCSV context.
                _csvContext = new CsvContext();
                _csvFileDescription = new CsvFileDescription()
                {
                    MaximumNbrExceptions = 100,
                    SeparatorChar = ','
                };

                IEnumerable<UploadPartModel> items = _csvContext.Read<UploadPartModel>(file, _csvFileDescription);

                foreach (var item in items)
                {
                    bool canCreateItem = true;

                    // First, validate that certain data, if present, resolves to the 
                    // associated referential table.

                    //check name first
                    if (string.IsNullOrEmpty(item.PartName))
                    {
                        model.Errors.Add(string.Format("Record {0}, Part Name is required.", item.Id));
                        canCreateItem = false;
                    }

                    //metergroup
                    var meterGroup = PemsEntities.MeterGroups.FirstOrDefault(m => m.MeterGroupId == item.MeterGroup);
                    if (meterGroup == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, MeterGroup '{1}' is invalid.", item.Id, item.MeterGroup));
                        canCreateItem = false;
                    }

                    //Category - mech master
                    var category = PemsEntities.MechanismMasters.FirstOrDefault(m => m.MechanismId == item.Category);
                    if (category == null)
                    {
                        model.Errors.Add(string.Format("Record {0}, Category '{1}' is invalid.", item.Id, item.Category));
                        canCreateItem = false;
                    }

                    if (item.CostInCents < 0)
                    {
                        model.Errors.Add(string.Format("Record {0}, CostInCents '{1}' is invalid. Defaulting to 0.", item.Id, item.CostInCents));
                        item.CostInCents = 0;
                    }

                    if (item.Status != 1 && item.Status != 0)
                    {
                        model.Errors.Add(string.Format("Record {0}, Status '{1}' is invalid. Defaulting to 1.", item.Id, item.Status));
                        item.Status = 1;
                    }

                    item.PartDesc = item.PartDesc.Trim();
                    item.PartName = item.PartName.Trim();

                    // If a CashBox cannot be created continue on to next record.
                    if (!canCreateItem)
                        continue;

                    // Now should be able to create the item
                    // For each uplaoded model, create a new item o r update an existing one
                    //this is an update for create based on part name / meter group, cateegory. 
                    var fmPart = MaintenanceEntities.FMParts.FirstOrDefault(x => x.MeterGroup == item.MeterGroup && x.Category == item.Category && x.PartName == item.PartName);
                    if (fmPart == null)
                    {
                        fmPart = CreateAvailablePart(item.Category, item.MeterGroup, item.CostInCents, item.PartDesc, item.PartName, item.Status);
                        model.Results.Add(string.Format("Record {0}, '{1}' added successfully.", item.Id, item.PartName));
                    }
                    //otherwise we want to update it - metergroup, cate, and name wont change
                    else
                    {
                        fmPart.CostInCents = item.CostInCents;
                        fmPart.PartDesc = item.PartDesc;
                        fmPart.Status = item.Status;
                        MaintenanceEntities.SaveChanges();
                        model.Results.Add(string.Format("Record {0}, '{1}' updated successfully.", item.Id, item.PartName));
                    }
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

        #endregion


        #region Parts
        /// <summary>
        /// Updates work order part quantities on all of the work order parts for a work order.
        /// The only data fora work order part that it is expecting is workOrderPartId and Quantity for a work order part.
        /// </summary>
        /// <param name="model"></param>
        public void UpdateWorkOrderPartQuantities(DispatcherWorkOrderDetailsModel model)
        {
            UpdatePartQuantities(model.WorkOrderParts, model.WorkOrderId);
        }
        #endregion
    }
}