using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Business.WebServices;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Technicians;
using Duncan.PEMS.Entities.WebServices.FieldMaintenance;
using Duncan.PEMS.Entities.WorkOrders;
using Duncan.PEMS.Utilities;
using SettingsFactory = Duncan.PEMS.Business.Users.SettingsFactory;
using WorkOrderEvent = Duncan.PEMS.Entities.WorkOrders.WorkOrderEvent;
using WorkOrderImage = Duncan.PEMS.Entities.WorkOrders.WorkOrderImage;
using WorkOrderPart = Duncan.PEMS.Entities.WorkOrders.WorkOrderPart;

namespace Duncan.PEMS.Business.WorkOrders
{
    /// <summary>
    /// Base work order factory that handles all of the make methods, data access classes, get ddl items, common helpers, etc
    /// </summary>
    public abstract class BaseWorkOrderFactory : BaseFactory
    {
        public string ErrorMessage { get; set; }
        public string ErrorMessageReason { get; set; }

        /// <summary>
        /// Instance of <see cref="MaintenanceEntities"/> for factory use.
        /// </summary>
        private MaintenanceEntities _maintenanceEntities;
        /// <summary>
        /// Protected property to get/create instance of <see cref="MaintenanceEntities"/>
        /// </summary>
        protected MaintenanceEntities MaintenanceEntities
        {
            get { return _maintenanceEntities ?? (_maintenanceEntities = new MaintenanceEntities(MaintenanceConnectionStringName)); }
        }

        private string _maintenanceConnectionStringName { get; set; }
        /// <summary>
        /// This represents the name of the connection string for accessing the Maintenance database instance.  This 
        /// name is used to look up the connection string in the web.config or connectionStrings.config.
        /// </summary>
        public string MaintenanceConnectionStringName
        {
            get
            {
                // Default it to the default connection string if it hasnt been set
                if (string.IsNullOrEmpty(_maintenanceConnectionStringName))
                    _maintenanceConnectionStringName = ConfigurationManager.AppSettings[Constants.Security.DefaultMaintConnectionStringName];
                return _maintenanceConnectionStringName;
            }
            set
            {
                _maintenanceConnectionStringName = value;
            }
        }

        /// <summary>
        /// Flag to determine if this is occuring in the mobile version or not. Mobile versions only care abouyt the currently logged in customer
        /// Admins care about all customers wthint he maintenance group.
        /// </summary>
        protected int WorkOrderEventSourceId = Constants.FieldMaintenance.EventSourceId;

        #region Helper Methods
        /// <summary>
        /// Gets all of the asset identifiers for a customer
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<AssetIdentifier> GetAssetsForCustomer(int customerID, bool includeMeterMapInfo = true)
        {
            return (new AssetFactory(ConnectionStringName).GetWorkOrderAssetsForCustomer(customerID, includeMeterMapInfo));
        }

        /// <summary>
        ///Calls the Asset Factory to:
        ///  Gets the name of the area assigned to the AreaId2 field for an asset by passing in the original customer id, meter id, and the AreadId on the asset (from the meters table)
        /// The areaid passed in is NOT the areaID2, but the areaid on the asset (meter table)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="areaId"></param>
        /// <param name="meterId"></param>
        /// <returns></returns>
        public string GetAreaName(int customerId, int areaId, int meterId)
        {
            return (new AssetFactory(ConnectionStringName).GetAssetAreaName(customerId, areaId, meterId));
        }

        /// <summary>
        /// Updates the notes for this work order
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="notes"></param>
        public void UpdateNotes(long workOrderId, string notes)
        {
            //get the work order
            var workOrder = MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
            if (workOrder != null)
            {
                workOrder.Notes = notes;
                MaintenanceEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a work order sla and highest severity base don the open events assigned to this work order, then saves that data.
        /// </summary>
        /// <param name="workOrderId"></param>
       public void UpdateEventAggregateInfo(long workOrderId)
       {
           //get the work order
            var workOrder = MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
           if (workOrder != null)
           {
               //now update the sla and highest severity (event alarm tier)

               //SLA - most recent - get the date
               var eventWithSoonestSLA = workOrder.WorkOrderEvents.Where(x => x.Status == (int)WorkOrderEventStatus.Open).OrderByDescending(x => x.SLADue).FirstOrDefault();
               if (eventWithSoonestSLA != null)
                   workOrder.SLADue = eventWithSoonestSLA.SLADue;

               //highest severity (alarm tier) - the alarm tiers are 0=Severe, 1 Major, - minor, so sort asc
               var eventWithHighestSeverity =workOrder.WorkOrderEvents.Where(x => x.Status == (int) WorkOrderEventStatus.Open).OrderBy(x => x.AlarmTier).FirstOrDefault();
               if (eventWithHighestSeverity != null)
                   workOrder.HighestSeverity = eventWithHighestSeverity.AlarmTier;

               MaintenanceEntities.SaveChanges();
           }
       }
        #endregion
       
        #region Events
       /// <summary>
       /// Gets an event code based on the string event code that is passed in.
       /// </summary>
       /// <returns></returns>
       public EventCode GetEventCode(string selectedFaultCode, int customerId)
       {
           //selected fault code is Event code |  event source, since this is part of the primary key 
           string[] items = selectedFaultCode.Split('|');
           if (items.Length == 2)
           {
               //get the event code by customerId, FM event type, and selected fault code
               int eventCode;
               int eventSource;
               if (int.TryParse(items[0], out eventCode) && int.TryParse(items[1], out eventSource))
               {
                   var ec = PemsEntities.EventCodes.FirstOrDefault(x => x.CustomerID == customerId
                       && x.EventSource == eventSource
                       && x.EventCode1 == eventCode
                       && x.EventType == Constants.FieldMaintenance.EventType);
                   if (ec != null)
                       return ec;
               }
           }

           //if nothing was found, return null
           return null;
       }

       /// <summary>
       /// Update the status of each event to be "Rejected" WorkOrderEventStatus.Rejected
       /// </summary>
       /// <param name="eventIds"></param>
       public void ResolveEvents(long[] eventIds,DateTime localTime, bool vandalism = false)
       {
           if (eventIds == null)
               return;
           //for each event, set the status to closed., as they are closing out individual events on a work order
           if (eventIds.Any())
           {
               var webServiceFactory = new WebServiceFactory();
               foreach (var eventId in eventIds)
               {
                   var fmworkOrderEvent =
                       MaintenanceEntities.FMWorkOrderEvents.FirstOrDefault(x => x.WorkOrderEventId == eventId);
                   if (fmworkOrderEvent != null)
                   {
                       fmworkOrderEvent.Vandalism = vandalism;
                       //now that we have closed them on our side, we need to close them on Duncans side, so for each event, send the request off to duncan to close this alarm as well.
                       //create the event  via the web service factory. 
                       //create the close Alarm Request object to pass to the web services
                       var closeAlarmRequest = new CloseAlarmRequest
                       {
                           AreaId = fmworkOrderEvent.WorkOrder.AreaId,
                           AssetId = fmworkOrderEvent.WorkOrder.MeterId,
                           CustomerId = fmworkOrderEvent.WorkOrder.CustomerId,
                           EventCode = fmworkOrderEvent.EventCode,
                           EventUID = fmworkOrderEvent.EventId,
                           LocalTime =  localTime,
                       };
                       var closeAlarmResponse = webServiceFactory.CloseAlarm(closeAlarmRequest);
                       //set the status and if there were errors, report them
                       //first, check to see if it is valid. if it is, then 

                       if (closeAlarmResponse.Closed)
                           fmworkOrderEvent.Status = (int)WorkOrderEventStatus.Closed;
                       else
                       {
                           fmworkOrderEvent.Status = (int)WorkOrderEventStatus.Open;
                           var eventResource = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.Glossary, "Event(s) not closed:");
                           if (string.IsNullOrEmpty(ErrorMessage))
                               ErrorMessage = string.Format(eventResource + " {0}", fmworkOrderEvent.WorkOrderEventId);
                           else
                               ErrorMessage += string.Format(", {0}", fmworkOrderEvent.WorkOrderEventId);
                       }
                   }
               }
               MaintenanceEntities.SaveChanges();
           }
       }

       /// <summary>
       /// Creates a work order event for a given work order and returns the ID of the generated event.
       /// </summary>
       /// <param name="eventId"></param>
       /// <param name="workOrderId"></param>
       /// <param name="eventCode"></param>
       /// <param name="eventDateTime"></param>
       /// <param name="eventDescription"></param>
       /// <param name="slaDue"></param>
       /// <param name="alarmTier"></param>
       /// <param name="notes"></param>
       /// <param name="automated"></param>
       /// <param name="vandalism"></param>
       /// <param name="status"></param>
       /// <returns></returns>
       public long CreateWorkOrderEvent(int eventId, long workOrderId, int eventCode, DateTime eventDateTime,
                                        string eventDescription, DateTime slaDue, int alarmTier, string notes,
                                        bool automated, bool vandalism, int status)
       {
           var fmWorkOrderEvent = new FMWorkOrderEvent();
           fmWorkOrderEvent.AlarmTier = alarmTier; // alarm tier of the event code
           fmWorkOrderEvent.Automated = automated;
           fmWorkOrderEvent.EventCode = eventCode;
           fmWorkOrderEvent.EventDateTime = eventDateTime;
           fmWorkOrderEvent.EventDesc = eventDescription;
           fmWorkOrderEvent.EventId = eventId;
           fmWorkOrderEvent.Notes = notes;
           fmWorkOrderEvent.SLADue = slaDue;
           fmWorkOrderEvent.Status = status;
           fmWorkOrderEvent.Vandalism = vandalism;
           fmWorkOrderEvent.WorkOrderId = workOrderId;

           MaintenanceEntities.FMWorkOrderEvents.Add(fmWorkOrderEvent);
           MaintenanceEntities.SaveChanges();
           return fmWorkOrderEvent.WorkOrderEventId;
       }
       /// <summary>
        /// Creates a list of work order events based on the db events passed in.
        /// </summary>
        /// <param name="fmWorkOrderEvents"></param>
        /// <returns></returns>
       protected List<WorkOrderEvent> MakeWorkOrderEvents(IEnumerable<FMWorkOrderEvent> fmWorkOrderEvents)
        {
            //check for null
            if (fmWorkOrderEvents == null)
                return new List<WorkOrderEvent>();

            var workOrderEvents = new List<WorkOrderEvent>();
            //foreach one, create our version of the work order image
            foreach (var fmWorkOrderEvent in fmWorkOrderEvents)
            {
                if (fmWorkOrderEvent.Status == (int) WorkOrderEventStatus.Open)
                {
                    var workOrderEvent = MakeWorkOrderEvent(fmWorkOrderEvent);
                    if (workOrderEvent != null)
                        workOrderEvents.Add(workOrderEvent);
                }
            }
            return workOrderEvents;
        }

        /// <summary>
        /// Creates a Work order event item base don the DB version.
        /// </summary>
        /// <param name="fmWorkOrderEvent"></param>
        /// <returns></returns>
       protected WorkOrderEvent MakeWorkOrderEvent(FMWorkOrderEvent fmWorkOrderEvent)
        { //check for null
            if (fmWorkOrderEvent == null)
                return null;

            var workOrderEvent = new WorkOrderEvent
            {
                AlarmTier = fmWorkOrderEvent.AlarmTier,
                Automated = fmWorkOrderEvent.Automated,
                EventCode = fmWorkOrderEvent.EventCode,
                EventDateTime = fmWorkOrderEvent.EventDateTime,
                EventDesc = fmWorkOrderEvent.EventDesc,
                EventId = fmWorkOrderEvent.EventId,
                Notes = fmWorkOrderEvent.Notes,
                SLADue = fmWorkOrderEvent.SLADue,
                Status = fmWorkOrderEvent.Status,
                StatusDisplay = fmWorkOrderEvent.Status == (int)WorkOrderEventStatus.Open ? HttpContext.Current.GetLocaleResource(ResourceTypes.Label, "Open") : @HttpContext.Current.GetLocaleResource(ResourceTypes.Label, "Closed"),
                Vandalism = fmWorkOrderEvent.Vandalism,
                WorkOrderEventId = fmWorkOrderEvent.WorkOrderEventId,
                Severity = fmWorkOrderEvent.AlarmTier,
                SeverityDisplay =  HttpContext.Current.GetLocaleResource(ResourceTypes.Label, "Severe"), //default this to something - is set below.
                WorkOrderId = fmWorkOrderEvent.WorkOrderId,
            };

           //fix up alarm tier display - have to go back to pems for this info.
            var tier = PemsEntities.AlarmTiers.FirstOrDefault(x => x.Tier == workOrderEvent.AlarmTier);
            if (tier != null)
                workOrderEvent.SeverityDisplay = tier.TierDesc;
            return workOrderEvent;
        }
       #endregion

        #region Images

       /// <summary>
       /// Creates an associated work order image for the work order id passed in.
       /// </summary>
       /// <param name="workOrderId"></param>
       /// <param name="imageData"></param>
       /// <returns></returns>
       public long CreateWorkOrderImage(long workOrderId, byte[] imageData, DateTime dateTaken)
       {
           var fmWorkOrderImage = new FMWorkOrderImage();
           fmWorkOrderImage.DateTaken = dateTaken;
           fmWorkOrderImage.ImageData = imageData;
           fmWorkOrderImage.WorkOrderId = workOrderId;
           MaintenanceEntities.FMWorkOrderImages.Add(fmWorkOrderImage);
           MaintenanceEntities.SaveChanges();
           return fmWorkOrderImage.WorkOrderImageId;
       }

       /// <summary>
        /// Creates a list of work order image items from the db list passed in.
        /// </summary>
        /// <param name="fmWorkOrderImages"></param>
        /// <returns></returns>
       protected List<WorkOrderImage> MakeWorkOrderImages(IEnumerable<FMWorkOrderImage> fmWorkOrderImages)
        {
            //check for null
            if (fmWorkOrderImages == null)
                return new List<WorkOrderImage>();

            var workOrderImages = new List<WorkOrderImage>();
            //foreach one, create our version of the work order image
            foreach (var fmWorkOrderImage in fmWorkOrderImages)
            {
                var workOrderImage = MakeWorkOrderImage(fmWorkOrderImage);
                if (workOrderImage != null)
                    workOrderImages.Add(workOrderImage);
            }

            return workOrderImages;
        }

        /// <summary>
        /// Make a work order image based on the DB version
        /// </summary>
        /// <param name="fmWorkOrderImage"></param>
        /// <returns></returns>
       protected WorkOrderImage MakeWorkOrderImage(FMWorkOrderImage fmWorkOrderImage)
        {
            //check for null
            if (fmWorkOrderImage == null)
                return null;

            var workOrderImage = new WorkOrderImage();
            workOrderImage.DateTaken = fmWorkOrderImage.DateTaken;
            workOrderImage.ImageData = fmWorkOrderImage.ImageData;
            workOrderImage.WorkOrderId = fmWorkOrderImage.WorkOrderId;
            workOrderImage.WorkOrderImageId = fmWorkOrderImage.WorkOrderImageId;
            return workOrderImage;
        }

       #endregion
       
        #region Parts
        /// <summary>
        /// Updates the part quantities for the parts assigned ot a work order.
        /// </summary>
        /// <param name="workOrderParts"></param>
        /// <param name="workOrderId"></param>
       protected void UpdatePartQuantities(List<WorkOrderPart> workOrderParts, long workOrderId)
       {
           //get the work order
           //update quantities for all the parts. if it is 0, then delete it from the work order entirely.
           foreach (var workOrderPart in workOrderParts)
           {
               //get the fmWork Order Part
               var fmWorkOrderPart =
                   MaintenanceEntities.FMWorkOrderParts.FirstOrDefault(
                       x => x.WorkOrderPartId == workOrderPart.WorkOrderPartId && x.WorkOrderId == workOrderId);
               if (fmWorkOrderPart != null)
               {
                   //if 0, delete it from the work order parts
                   if (workOrderPart.Quantity == 0)
                       MaintenanceEntities.FMWorkOrderParts.Remove(fmWorkOrderPart);
                   else
                       //get it from main entities, update quantity, and save
                       fmWorkOrderPart.Quantity = workOrderPart.Quantity;
               }
           }
           MaintenanceEntities.SaveChanges();
       }

       /// <summary>
       /// Adds a FMPart to a work order as a workorderpart db entry
       /// </summary>
       /// <param name="partId"></param>
       /// <param name="workOrderId"></param>
       /// <param name="notes"></param>
       public void AddPartToWorkOrder(long partId, long workOrderId, string notes)
       {
           //get the part 
           var fmPart = MaintenanceEntities.FMParts.FirstOrDefault(x => x.PartId == partId);
           if (fmPart == null)
               return;

           //we only add unique when the part is notht he"Other" part. if it is other, then we want to add it with new notes no matter what
           if (fmPart.PartName != Constants.FieldMaintenance.OtherPartName)
           {
               //if they are adding a part that already exist, just increment it by one
               //check to see if the work order part isnt already part of this work order. 
               //if it exist, get it from the maint entities, increment once, then save
               var fmwoPart = MaintenanceEntities.FMWorkOrderParts.FirstOrDefault(x => x.PartId == partId && x.WorkOrderId == workOrderId);
               if (fmwoPart != null)
               {
                   fmwoPart.Quantity = fmwoPart.Quantity + 1;
                   //update the notes as well, only if htere is a value for it. we dont want to override the existing notes
                   if (!string.IsNullOrEmpty(notes))
                       fmwoPart.Notes = notes;
                   MaintenanceEntities.SaveChanges();
                   return;
               }
           }

           //now create a work order part if is doesnt already exist for this work order.
           var fmWordOrderPart = new FMWorkOrderPart
           {
               Notes = notes ?? string.Empty,
               PartId = partId,
               Quantity = 1,
               WorkOrderId = workOrderId
           };
           MaintenanceEntities.FMWorkOrderParts.Add(fmWordOrderPart);
           MaintenanceEntities.SaveChanges();
       }

       /// <summary>
       /// Creates a list of work order parts based ont he db items.
     /// </summary>
     /// <param name="dbItems"></param>
     /// <returns></returns>
        public List<WorkOrderPart> MakeWorkOrderParts(ICollection<FMWorkOrderPart> dbItems)
        {
          //check for null
            if (dbItems == null)
                return new List<WorkOrderPart>();

            var items = new List<WorkOrderPart>();
            //foreach one, create our version of the work order image
            foreach (var dbItem in dbItems)
            {
                var item = MakeWorkOrderPart(dbItem);
                if (item != null)
                    items.Add(item);
            }

            return items;

        }

        /// <summary>
        /// Make a work order part based on the db version
        /// </summary>
        /// <param name="dbItem"></param>
        /// <returns></returns>
        protected WorkOrderPart MakeWorkOrderPart(FMWorkOrderPart dbItem)
        {
            //check for null
            if (dbItem == null)
                return null;
           
            var item = new WorkOrderPart
                {
                    Notes = dbItem.Notes,
                    PartId = dbItem.PartId,
                    Quantity = dbItem.Quantity,
                    WorkOrderId = dbItem.WorkOrderId,
                    WorkOrderPartId = dbItem.WorkOrderPartId,
                    PartDesc = dbItem.Part.PartDesc,
                    PartName = dbItem.Part.PartName,
                    CostInCents = dbItem.Part.CostInCents
                    
                };
            return item;
        }
       #endregion

        #region Assets
       protected WorkOrderAsset MakeWorkOrderAsset(FMWorkOrder workOrder, int customerId)
       {

           var assetFactory = (new AssetFactory(ConnectionStringName));
           var asset = assetFactory.GetAsset(customerId, workOrder.AreaId, workOrder.MeterId);
            var workOrderAsset = new WorkOrderAsset
            {
                AssetId = workOrder.MeterId,
                //subtype - call asset factory and pass in mechanismid and customer id to the GetMechanismMasterCustomerDescription - will return the customer specific, or default to the desc of hte mechanism
                AssetSubType = assetFactory.GetMechanismMasterCustomerDescription(customerId, workOrder.Mechanism ),
                AssetType = assetFactory.GetAssetTypeDescription(workOrder.MeterGroup, customerId),
                AssetAreaName = assetFactory.GetAssetAreaName(customerId, workOrder.AreaId, workOrder.MeterId),
                AssetName = asset.MeterName,
                AreaId = asset.AreaID,
                AssetLastPMDate =  asset.LastPreventativeMaintenance
            };
            return workOrderAsset;
        }
       #endregion

        #region Work Orders

        /// <summary>
        /// Resolves a work order and assigns the associated maintenance code to the resoluction for the work order
        /// This will also close out any exisiting events for this work order
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="maintCodeId"></param>
        /// <param name="vandalism"></param>
        public void ResolveWorkOrder(long workOrderId, int maintCodeId, bool vandalism)
        {
            //for each event, set the status to resolved (completed).
            var fmworkorder = MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
            if (fmworkorder != null)
            {
                ErrorMessage = string.Empty;
                //now lets close out all the work order events
                var localTime = (new Customers.SettingsFactory().GetCustomerLocalTime(fmworkorder.CustomerId));
                //only try to close the non -closed ones. if they are already closed, then the web service will return (incorrectly) that it was not closed.
                if (fmworkorder.WorkOrderEvents.Any(x=>x.Status != (int)WorkOrderStatus.Closed))
                    ResolveEvents(fmworkorder.WorkOrderEvents.Where(x => x.Status != (int)WorkOrderStatus.Closed).Select(x => x.WorkOrderEventId).ToArray(), localTime, vandalism);

                //only close it out if the events were closed.
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    var woResource = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.Glossary, "Work Order was not closed:");
                    var woTryAgain = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.Glossary, "Try again later.");
                    ErrorMessage = woResource + " " + ErrorMessage + ". " + woTryAgain;
                }
                else
                {
                    fmworkorder.WorkOrderStatusId = (int) WorkOrderStatus.Closed;
                    fmworkorder.CompletedDate = localTime;
                    //fmworkorder.AssignmentState = - no change to assignment state one resolution
                    //now we have to get the resolution code (maintenance code) from pems
                    var maintenanceCode =
                        PemsEntities.MaintenanceCodes.FirstOrDefault(x => x.MaintenanceCode1 == maintCodeId);
                    if (maintenanceCode != null)
                    {
                        fmworkorder.ResolutionCode = maintenanceCode.MaintenanceCode1;
                        fmworkorder.ResolutionDesc = maintenanceCode.Description;
                    }
                    MaintenanceEntities.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Reject the work order and assign it back to the empty pool so dispatch can see that it is unassigned and rejected
        /// </summary>
        /// <param name="workOrderId"></param>
       public void RejectWorkOrder(long workOrderId)
       {
           //for each event, set the status to rejected.
           var fmworkorder =
               MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
           if (fmworkorder != null)
           {
               //change the status to rejected and clear out the technician
               fmworkorder.WorkOrderStatusId = (int)WorkOrderStatus.Rejected;
               fmworkorder.AssignmentState = (int)AssignmentState.Unassigned;
               fmworkorder.TechnicianId = (int?)null;
               MaintenanceEntities.SaveChanges();
           }
       }

       /// <summary>
       /// Changes the state of the work order to unassigned and removes the technicianid
       /// </summary>
       /// <param name="workOrderId"></param>
       public void SuspendWorkOrder(long workOrderId)
       {
           //get the work order
           var fmworkorder =
               MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
           if (fmworkorder != null)
           {
               fmworkorder.WorkOrderStatusId = (int)WorkOrderStatus.Suspended;
               fmworkorder.AssignmentState = (int)AssignmentState.Suspended;
               fmworkorder.TechnicianId = (int?)null;
           }
           MaintenanceEntities.SaveChanges();
       }

       /// <summary>
       /// Changes the state of the work order to unassigned and removes the technicianid
       /// </summary>
       /// <param name="workOrderId"></param>
       public void UnassignWorkOrder(long workOrderId)
       {
           //get the work order
           var fmworkorder =
               MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
           if (fmworkorder != null)
           {
               //fmworkorder.WorkOrderStatusId = no change on unnasignment
               fmworkorder.AssignmentState = (int)AssignmentState.Unassigned;
               fmworkorder.AssignedDate = (DateTime?) null;
               fmworkorder.TechnicianId = (int?)null;
           }
           MaintenanceEntities.SaveChanges();
       }

        /// <summary>
        /// Changes the state of the work order to assigned and adds the technicianid
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <param name="technicianId"></param>
        public void AssignWorkOrder(long workOrderId, int technicianId, DateTime localTime)
       {
           //get the work order
           var fmworkorder =
               MaintenanceEntities.FMWorkOrders.FirstOrDefault(x => x.WorkOrderId == workOrderId);
           if (fmworkorder != null)
           {
               //if the work order status is rejected, set it back to incomplete.
               //this way they will be able to see it in the work order list. otherwise they wont, and they shouldnt see rejected work orders.
               if (fmworkorder.WorkOrderStatusId == (int)WorkOrderStatus.Rejected)
                   fmworkorder.WorkOrderStatusId = (int)WorkOrderStatus.Incomplete;
               fmworkorder.AssignmentState = (int)AssignmentState.Assigned;
               
               fmworkorder.TechnicianId = technicianId;
               fmworkorder.AssignedDate = localTime;
           }
           MaintenanceEntities.SaveChanges();
       }

       #endregion

        #region Fault / Repair Descriptions
       /// <summary>
       /// Gets a list of fault descriptions for the customer
       /// this gets all fo the events that are of the Field Maintenance Event Types set int he constants
       /// </summary>
       /// <returns></returns>
       public List<SelectListItem> GetFaultDescriptions(int customerId)
       {
           //have to get fault descriptions
           var items = new List<SelectListItem>();
           var eventCodes =
               PemsEntities.EventCodes.Where(
                   x => x.CustomerID == customerId && x.EventType == Constants.FieldMaintenance.EventType);
           //do not convert to a linq expression, it doesnt like the ToString()
           if (eventCodes.Any())
               foreach (var eventCode in eventCodes)
                   items.Add(new SelectListItem { Text = eventCode.EventDescVerbose, Value = eventCode.EventCode1 + "|" + eventCode.EventSource });

           return items;
       }
      
       /// <summary>
       /// Gets a list of repair descriptions (maintenance codes)
       /// </summary>
       /// <returns></returns>
       public List<SelectListItem> GetRepairDescriptions()
       {
           var items = new List<SelectListItem>();
           //this has to go tot hte pems db and get the repair codes (maintenance codes)
           var maintCodes = PemsEntities.MaintenanceCodes.ToList();
           if (maintCodes.Any())
               items.AddRange(maintCodes.Select(maintenanceCode => new SelectListItem {Text = maintenanceCode.Description, Value = maintenanceCode.MaintenanceCode1.ToString()}));
           return items;
       }

       #endregion

        #region Available Parts
       
       /// <summary>
        /// Gets the available parts for the work order based ont he metergroup of the asset and the category (mechanism)
        /// </summary>
        /// <param name="meterGroupId"></param>
        /// <param name="mechanismId"></param>
        /// <returns></returns>
        protected List<AvailablePart> GetAvailableParts(int meterGroupId, int? mechanismId)
       {
           //get all of the parts int he system for a specific meter group
            //get all the items with the correct active status for this meter group. 
            //the mechanismId is the category on a part in the db
           var dbItems = MaintenanceEntities.FMParts.Where(x => x.Status == (int)AvailablePartStatus.Active
               && x.MeterGroup == meterGroupId && x.Category == mechanismId); 

           //foreach one, create our version of the work order image
           return MakeAvailableParts(dbItems.AsEnumerable());
       }

        /// <summary>
        /// Gets all of the avaialble parts int he db. use the connection string construtor for this one.
        /// </summary>
        /// <returns></returns>
        public List<AvailablePart> GetAllAvailableParts()
        {
            //get all of the parts int he system 
            //get all the items with the correct active status 
            var dbItems = MaintenanceEntities.FMParts; 
            //foreach one, create our version of the work order image
            return MakeAvailableParts(dbItems.AsEnumerable());
        }


        /// <summary>
        /// Creates a list of work order image items from the db list passed in.
        /// </summary>
        /// <returns></returns>
        protected List<AvailablePart> MakeAvailableParts(IEnumerable<FMPart> fmParts)
        {
            //check for null
            if (fmParts == null)
                return new List<AvailablePart>();

            var parts = new List<AvailablePart>();
            //foreach one, create our version of the work order image
            foreach (var fmPart in fmParts)
            {
                var availablePart = MakeAvailablePart(fmPart);
                if (availablePart != null)
                    parts.Add(availablePart);
            }
            return parts;
        }

        /// <summary>
        /// Make a work order part based on the DB version
        /// </summary>
        /// <param name="fmPart"></param>
        /// <returns></returns>
        protected AvailablePart MakeAvailablePart(FMPart fmPart)
        {
            //check for null
            if (fmPart == null)
                return null;

            var item = new AvailablePart
                {
                    PartId = fmPart.PartId,
                    PartName = fmPart.PartName,
                    MeterGroup = fmPart.MeterGroup,
                    MeterGroupDisplay =   GetMeterGroupDescription(fmPart.MeterGroup),
                    Category = fmPart.Category,
                    CategoryDisplay = GetMechanismDescription(fmPart.Category),
                    PartDesc = fmPart.PartDesc,
                    CostInCents = fmPart.CostInCents,
                    Status = fmPart.Status,
                    StatusDisplay = fmPart.Status == (int)AvailablePartStatus.Active ? "Active" : "Inactive"
                };
            return item;
        }

        /// <summary>
        /// Public method to add an available part ot hte parts system
        /// </summary>
        public FMPart CreateAvailablePart(int category, int metergroup, int costInCents, string description, string name, int status)
        {
            //create a part
            var fmPart = new FMPart
                {
                    Category = category,
                    MeterGroup = metergroup,
                    CostInCents = costInCents,
                    PartDesc = description,
                    PartName = name,
                    Status = status
                };
            MaintenanceEntities.FMParts.Add(fmPart);
            MaintenanceEntities.SaveChanges();
            return fmPart;
        }

        /// <summary>
        /// Gets the desccription for a mechanism. used when determineing name of a category in the work order dispatch screens
        ///  </summary>
        /// <param name="mechamismId"></param>
        /// <returns></returns>
        private string GetMechanismDescription(int mechamismId)
        {
            var item = PemsEntities.MechanismMasters.FirstOrDefault(x => x.MechanismId == mechamismId);
            return item == null ? "" : item.MechanismDesc;
        }

        /// <summary>
        /// Gets the description for a meter group. 
        /// </summary>
        /// <param name="meterGroupId"></param>
        /// <returns></returns>
        private string GetMeterGroupDescription(int meterGroupId)
        {
            var item = PemsEntities.MeterGroups.FirstOrDefault(x => x.MeterGroupId == meterGroupId);
            return item == null ? "" : item.MeterGroupDesc;
        }


       #endregion

        #region Technicians

        /// <summary>
        /// Make a work order part based on the db version
        /// </summary>
        /// <param name="dbItem"></param>
        /// <returns></returns>
        protected Technician MakeTechnicianFromWorkOrder(FMWorkOrder workOrder)
        {
            //check for null
            if (workOrder == null)
                return new Technician();
            //technicina details
            var tech = new Technician();
            tech.TechnicianID = workOrder.TechnicianId ?? -1;
            
            if (tech.TechnicianID > 0)
                tech = MakeTechnician(tech.TechnicianID);
            return tech;
        }

        /// <summary>
        /// Make a work order part based on the db version
        /// </summary>
        /// <param name="dbItem"></param>
        /// <returns></returns>
        protected Technician MakeTechnician(int technicianId)
        {
            //technicina details
            var tech = new Technician();
            tech.TechnicianID = technicianId;

            if (tech.TechnicianID > 0)
            {
                //get the etechnician details for this user
                var dbTech = PemsEntities.TechnicianDetails.FirstOrDefault(x => x.TechnicianId == tech.TechnicianID);

                //set the name for the technician
                tech.TechnicianName = dbTech == null ? "" : dbTech.Name;
                tech.TechnicianContact = dbTech == null ? "" : dbTech.Contact;
                tech.TechnicianKeyID = dbTech == null ? (int?)null : dbTech.TechnicianKeyID;

                //get a count of exisitng assigned work orders for this tech.
                tech.TechnicianAssignedWorkOrderCount = MaintenanceEntities.FMWorkOrders.Count(x => x.TechnicianId == tech.TechnicianID && (x.WorkOrderStatusId == (int)WorkOrderStatus.Open || x.WorkOrderStatusId == (int)WorkOrderStatus.Incomplete));
              
            }
            return tech;
        }

        /// <summary>
        /// Gets a list of technicians based ont he current pems connections string. 
        /// Maintenance will need to loop thorugh the maintenance gorup customers for a maintenance group and add a range for each of these.
        /// </summary>
        /// <param name="workOrder"></param>
        /// <returns></returns>
        public List<Technician> MakeAvailableTechnicians()
        {
            var userFactory = new UserFactory();
            //get a lsit of techencians for the current customer
            var dbtechs = PemsEntities.TechnicianDetails.ToList();
            var techs = new List<Technician>();
            //make sure they are active
            foreach (var dbtechnicianDetail in dbtechs)
            {
                //make sure they are active users
                if (userFactory.IsUserActive(dbtechnicianDetail.TechnicianId))
                    techs.Add(MakeTechnician(dbtechnicianDetail.TechnicianId));
            }
            return techs;
        }

        /// <summary>
        /// Gets all the active user profiles  in the system that are technicians by using the usersetting factory 
        /// </summary>
        /// <returns></returns>
        public List<Technician> GetAllTechnicians()
        {
            var techs = new List<Technician>();
            //get all the users that are techs from the settings factory
            var allTechsInSystem = (new SettingsFactory()).GetAllTechnicianProfiles();
            var userFactory = new UserFactory();
            foreach (var userProfile in allTechsInSystem)
            {
                //make sure they are active users
                if (userFactory.IsUserActive(userProfile.UserId))
                    techs.Add(MakeTechnician(userProfile.UserId));
            }
            return techs;
        }
        #endregion
    }
}