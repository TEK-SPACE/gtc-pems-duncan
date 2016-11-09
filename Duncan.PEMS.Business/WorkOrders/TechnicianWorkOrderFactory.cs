using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.WorkOrders.Technician;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.WorkOrders
{
    /// <summary>
    /// Factory that handles all of the data access and business rules for mobile Work orders
    /// </summary>
    public class TechnicianWorkOrderFactory : BaseWorkOrderFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// This is the base factory constructor that underlays all business function factories.  It takes a 
        /// a name of a connection string, <paramref name="maintenanceConnectionStringName"/>, so as to point to a specific instance of the Maintenance database.
        /// This vrsion of the constructor will default to <see cref="DateTime.Now"/> for time.
        /// </summary>
        /// <param name="maintenanceConnectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        /// <param name="connectionStringName"></param>
        public TechnicianWorkOrderFactory(string maintenanceConnectionStringName, string connectionStringName)
        {
            MaintenanceConnectionStringName = maintenanceConnectionStringName;
            ConnectionStringName = connectionStringName;
        }

        #region Technician Screens

        /// <summary>
        /// gets all the work orders for the current customer and technician (current user logged in)
        /// </summary>
        /// <returns></returns>
        public List<TechnicianWorkOrderListItem> GetTechnicianWorkOrders(int customerId, int technicianId)
        {
            var techWorkOrders = new List<TechnicianWorkOrderListItem>();
            //get all the work orders for the customer and assigned to this tech
            var workOrders =MaintenanceEntities.FMWorkOrders.Where(x => x.CustomerId == customerId && x.TechnicianId == technicianId && x.AssignmentState == (int)AssignmentState.Assigned 
                &&    (x.WorkOrderStatusId == (int)WorkOrderStatus.Open ||
                    x.WorkOrderStatusId == (int)WorkOrderStatus.Incomplete ));
            if (workOrders.Any())
            {
                foreach (var workOrder in workOrders)
                {
                    var techWorkOrder = new TechnicianWorkOrderListItem
                        {
                            WorkOrderAsset = MakeWorkOrderAsset(workOrder, customerId),
                            Deadline = workOrder.SLADue,
                            WorkOrderId = workOrder.WorkOrderId,
                            NotificationTime = workOrder.AssignedDate,
                            Location = workOrder.Location
                        };
                    techWorkOrders.Add(techWorkOrder);
                }
            }
            return techWorkOrders;
        }


        /// <summary>
        /// gets all the pm work orders for the current customer 
        /// </summary>
        /// <returns></returns>
        public List<TechnicianWorkOrderListItem> GetTechnicianPMWorkOrders(int customerId)
        {
            var techWorkOrders = new List<TechnicianWorkOrderListItem>();
            //get all the work orders for the customer and for the pm event code, all open or incomplete ones. ignore closed, suspended, and rejected ones

            var settingFactory = new SettingsFactory();
            var pmEventCodeString = settingFactory.GetValue("PMEventCode", customerId);
            if (pmEventCodeString == null)
            {
                //if they havent set this, then just return an empty list
                settingFactory.Set(customerId, "PMEventCode", string.Empty);
                return techWorkOrders;
            }
            int pmEventCode;
            if (int.TryParse(pmEventCodeString, out pmEventCode))
            {
                var workOrders =
                    MaintenanceEntities.FMWorkOrders.Where(
                        x => x.CustomerId == customerId && x.WorkOrderEvents.Any(y => y.EventCode == pmEventCode)
                             &&
                             (x.WorkOrderStatusId == (int) WorkOrderStatus.Open ||
                              x.WorkOrderStatusId == (int) WorkOrderStatus.Incomplete));
                if (workOrders.Any())
                {
                    foreach (var workOrder in workOrders)
                    {
                        var techWorkOrder = new TechnicianWorkOrderListItem
                            {
                                WorkOrderAsset = MakeWorkOrderAsset(workOrder, customerId),
                                Deadline = workOrder.SLADue,
                                WorkOrderId = workOrder.WorkOrderId,
                                NotificationTime = workOrder.AssignedDate,
                                Location = workOrder.Location,

                            };
                        techWorkOrders.Add(techWorkOrder);
                    }
                }
            }

            return techWorkOrders;
        }

        /// <summary>
        /// Gets the mobile version of the work order details screen
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="customerId"></param>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public TechnicianWorkOrderDetailsModel GetWorkOrderDetails(long workOrderId, int customerId, DateTime localTime)
        {
            var model = new TechnicianWorkOrderDetailsModel();
            //get the work order
            var workOrder = MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
            if (workOrder != null)
            {
                //convert it to our enetity
                model.Deadline = workOrder.SLADue;
                model.Location = workOrder.Location;
                model.Status = workOrder.WorkOrderStatu.WorkOrderStatusDesc;
                model.WorkOrderId = workOrder.WorkOrderId;
                model.Notes = workOrder.Notes;
                model.CreationDate = workOrder.CreateDateTime;
                model.CrossStreet = workOrder.CrossStreet;
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
                var tier = PemsEntities.AlarmTiers.FirstOrDefault(x => x.Tier ==  workOrder.HighestSeverity);
                model.PriorityDisplay = tier != null ? tier.TierDesc : HttpContext.Current.GetLocaleResource(ResourceTypes.Label, "Minor");
                
                //list of all past work orders inthe last 14 days (this migh have to be configurable per client todo)
                var pastDatetime = localTime.AddDays(-14);
                var pastWorkOrders =
                    MaintenanceEntities.FMWorkOrders.Where(
                        x => x.MeterId == workOrder.MeterId && x.WorkOrderId != workOrderId && x.CreateDateTime >= pastDatetime);
                model.PastWorkOrders = pastWorkOrders.Any()? pastWorkOrders.Count().ToString(): "0";


                //set the flag based on the current customer to determine if they need to close the entire work order out or per events
                var settingFactory = new SettingsFactory();
                var workOrderCloseEventSetting = settingFactory.GetValue("CloseWorkOrderEvents", customerId);
                //if it doenst exist, create it
                if (workOrderCloseEventSetting == null)
                    settingFactory.Set(customerId, "CloseWorkOrderEvents", false.ToString());
                workOrderCloseEventSetting = settingFactory.GetValue("CloseWorkOrderEvents", customerId);
                model.CloseWorkOrderEvents = bool.Parse(workOrderCloseEventSetting);


            }
            return model;
        }

       
        #endregion

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
                                    DateTime localTime)
        {
            //we have to check to see if htey asset already has a work order. if it does, use that work order.
            //we add it to any open work order, even if it is assigned ot another technician (in practice this will almost never happen)
            var existingWorkOrder =
                MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.MeterId == assetId &&
                    (x.WorkOrderStatusId == (int)WorkOrderStatus.Open ||
                    x.WorkOrderStatusId == (int)WorkOrderStatus.Incomplete ||
                    x.WorkOrderStatusId == (int)WorkOrderStatus.Rejected));
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
                    AssignedDate = localTime,
                    CreateDateTime = localTime,
                    CreatedById = WebSecurity.CurrentUserId,
                    CustomerId = customerId,
                    HighestSeverity = 0,
                    Location = asset.Location,
                    Mechanism = asset.MeterType,
                    MeterGroup = asset.MeterGroup ?? 1,
                    MeterId = assetId,
                    Notes = string.Empty,
                    ReportingUserId = WebSecurity.CurrentUserId,
                    SLADue = localTime.AddHours(2),
                    TechnicianId = WebSecurity.CurrentUserId,
                    WorkOrderStatusId = (int) WorkOrderStatus.Open,
                    AssignmentState = (int)AssignmentState.Assigned, //assigned to the existing user
                    ZoneId = asset.MeterMaps.FirstOrDefault() != null ? asset.MeterMaps.FirstOrDefault().ZoneId : (int?) null
                };
            //fmWorkOrder.CompletedDate; //not set here, set upon resolution
            //fmWorkOrder.CrossStreet; - ?? //not used for mobile, will be a field in the dispatchers screens
            //fmWorkOrder.ParkingSpaceId; //not used, dont have to set parkingspace ID
            //fmWorkOrder.ResolutionCode; - nothing on creation, when resolving, these are maintenance codes
            //fmWorkOrder.ResolutionDesc; -  nothing on creation, when resolving, these are maintenance codes
            MaintenanceEntities.FMWorkOrders.Add(fmWorkOrder);
            MaintenanceEntities.SaveChanges();
            return fmWorkOrder.WorkOrderId;
        }
        #endregion

        #region Events

        /// <summary>
        /// Gets a work order event.
        /// </summary>
        /// <param name="workOrderEventId"></param>
        /// <returns></returns>
        public TechnicianEventDetailsModel GetWorkOrderEvent(int workOrderEventId, int customerId)
        {
            //get the work order event
            var fmWorkOrderEvent =
                MaintenanceEntities.FMWorkOrderEvents.FirstOrDefault(x => x.WorkOrderEventId == workOrderEventId);
            if (fmWorkOrderEvent == null)
            return null;

            //now get the work order for this event, make the asset, work order event, and images for this work order and work order event.
            var eventModel = new TechnicianEventDetailsModel();
            eventModel.WorkOrderAsset = MakeWorkOrderAsset(fmWorkOrderEvent.WorkOrder, customerId);
            eventModel.WorkOrderEvent = MakeWorkOrderEvent(fmWorkOrderEvent);
            eventModel.WorkOrderImages = MakeWorkOrderImages(fmWorkOrderEvent.WorkOrder.WorkOrderImages);

            //assumption is that the event source id is 10 (PEMS). So we need to default it to PEMS and then get based on the eventsourceid property of the base factory, see if its int he pems db
            eventModel.Source = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.Glossary, "PEMS");

           //now get it from pems to see what the description should be (in case it changed
            var eventSource = PemsEntities.EventSources.FirstOrDefault(x => x.EventSourceCode == WorkOrderEventSourceId);
            if (eventSource != null)
                eventModel.Source = eventSource.EventSourceDesc;

            return eventModel;
        }

        #endregion

        #region Parts
        /// <summary>
        /// Updates work order part quantities on all of the work order parts for a work order.
        /// The only data fora work order part that it is expecting is workOrderPartId and Quantity for a work order part.
        /// </summary>
        /// <param name="model"></param>
        public void UpdateWorkOrderPartQuantities(TechnicianWorkOrderDetailsModel model)
        {
            UpdatePartQuantities(model.WorkOrderParts, model.WorkOrderId);
        }
        #endregion
    }
}
