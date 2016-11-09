using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Utility;
using Duncan.PEMS.Business.WebServices;
using Duncan.PEMS.Business.WorkOrders;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.WebServices.FieldMaintenance;
using Duncan.PEMS.Entities.WorkOrders;
using Duncan.PEMS.Entities.WorkOrders.Technician;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using WebMatrix.WebData;
#pragma warning disable 1591

namespace Duncan.PEMS.Web.Areas.maint.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class WorkOrdersController : PemsController
    {
        #region Work Order Listing
        private const string DefaultSort = "Deadline";
        private const string DefaultSortDirection = "ASC";
        private const string DefaultAssetId = "";

        /// <summary>
        /// Get work orders with optional sort and direction parameters
        /// all requests are new ones to this method, they are all GETS. we pass the correct stuff to the details page so we can come back without hte page refresh issue 
        /// </summary>
        /// <param name="selectedSort"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public ActionResult WorkOrders(string selectedSort = null, string sortDirection = null)
        {
            if (selectedSort == null)
                selectedSort = DefaultSort;
            if (sortDirection == null)
                sortDirection = DefaultSortDirection;
            var model = GetSortedWorkOrders(selectedSort, sortDirection);
            return View( model);
        }

        public TechnicianWorkOrderListModel GetSortedWorkOrders(string selectedSort, string sortDirection)
        {
            if (string.IsNullOrEmpty(sortDirection))
                sortDirection = DefaultSortDirection;
            var model = new TechnicianWorkOrderListModel { WorkOrders = new List<TechnicianWorkOrderListItem>(), SortOptions = GetSortOptions(), SelectedSort = selectedSort };
            var techWorkOrders = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), 
                Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTechnicianWorkOrders(CurrentCity.Id, WebSecurity.CurrentUserId);

            //now we have to sort based on thier selection. if nothing was selected, sort by deadline
            //determine the sort direction we need 
            //always sort byt he default sort after the initial sort
            techWorkOrders = techWorkOrders.OrderBy(selectedSort + " " + sortDirection).ToList();

            //add the sorted ones back to the model
            model.WorkOrders = techWorkOrders;
            //set the sorts for back purposes from the details page
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            return model;
        }

        /// <summary>
        /// Gets a list of localized sort options
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSortOptions()
        {
            var items = new List<SelectListItem>
                {
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Work Order" ), Value = "WorkOrderId"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Location" ), Value = "Location"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Deadline" ), Value = "Deadline"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Asset Type" ), Value = "AssetType"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Notification Time" ), Value = "NotificationTime"}
                };
            return items;
        }
        #endregion


        #region PM Work Orders

        /// <summary>
        /// Get work orders with optional sort and direction parameters
        /// all requests are new ones to this method, they are all GETS. we pass the correct stuff to the details page so we can come back without hte page refresh issue 
        /// </summary>
        /// <param name="selectedSort"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public ActionResult PMWorkOrders(string selectedSort = null, string sortDirection = null, string assetId = null)
        {
            if (selectedSort == null)
                selectedSort = DefaultSort;
            if (sortDirection == null)
                sortDirection = DefaultSortDirection;
            if (assetId == null)
                assetId = DefaultAssetId;
            var model = GetSortedPMWorkOrders(selectedSort, sortDirection, assetId);

            model.AssetIdOptions = GetAssetIdOptions();
            return View( model);
        }

        public List<AssetIdentifier> GetAssetIdOptions(int customerId = -1)
        {
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var options = new List<AssetIdentifier>();
            int customerIdToUse = CurrentCity.Id;
            if (customerId != -1)
                customerIdToUse = customerId;
            //need to put this in session, so it doesnt take forever to load each time.
            if (Session["__PMCustomerAssets" + customerIdToUse] != null)
                options = Session["__PMCustomerAssets" + customerIdToUse] as List<AssetIdentifier>;
            else
            {
                var assetOptions = mobileWorkOrderFactory.GetAssetsForCustomer(customerIdToUse);
                Session["__PMCustomerAssets" + customerIdToUse] = assetOptions;
                options = assetOptions;
            }
            return options;
        }

        public TechnicianWorkOrderListModel GetSortedPMWorkOrders(string selectedSort, string sortDirection, string assetId)
        {
            if (string.IsNullOrEmpty(sortDirection))
                sortDirection = DefaultSortDirection;
            var model = new TechnicianWorkOrderListModel { WorkOrders = new List<TechnicianWorkOrderListItem>(), SortOptions = GetPMSortOptions(), SelectedSort = selectedSort, SelectedAssetKey = assetId};
            var techWorkOrders = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), 
                Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTechnicianPMWorkOrders(CurrentCity.Id);

            //now we have to sort based on thier selection. if nothing was selected, sort by deadline
            //determine the sort direction we need 
            //always sort byt he default sort after the initial sort
            techWorkOrders = techWorkOrders.OrderBy(selectedSort + " " + sortDirection).ToList();

            //if the assetId is passed in, filter by those items
            if (!string.IsNullOrEmpty(assetId))
            {
                var ids = assetId.Split('|');
                //make sure we have the values we expect.
                if (ids.Count() == 3)
                {
                    var areaid = ids[1];
                    var assetid = ids[2];
                    //split it here, make sure they match the area/asset selected
                    techWorkOrders.RemoveAll(x => !(x.AssetId == areaid && x.AreaId == assetid));
                }
            }
            //add the sorted / filtered ones back to the model
            model.WorkOrders = techWorkOrders;
            //set the sorts for back purposes from the details page
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            ViewBag.AssetId = assetId;
            return model;
        }

        /// <summary>
        /// Gets a list of localized sort options
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPMSortOptions()
        {
            var items = new List<SelectListItem>
                {
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Asset ID" ), Value = "AssetId"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Asset Name" ), Value = "AssetName"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Area ID" ), Value = "AreaName"},
                    new SelectListItem {Text = System.Web.HttpContext.Current.GetLocaleResource( ResourceTypes.Glossary, "Deadline" ), Value = "Deadline"},
                };
            return items;
        }

        #endregion

        #region Work Site Safe
        /// <summary>
        /// Details for the work order with a lsit of all events for that work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkSiteSafe(long workOrderId, string selectedSort, string sortDirection)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString()
                , Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View(techWorkOrder);
        }

        [HttpPost]
        public ActionResult WorkSiteSafe(TechnicianWorkOrderDetailsModel model, string selectedSort, string sortDirection, string submitButton, IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //get the two
            //check the other submit buttons and act on them, or continue
            //update the notes for this work order
            mobileWorkOrderFactory.UpdateNotes(model.WorkOrderId, model.Notes);
            //now we need to add the images to the work order (if there are any)
            if (files != null)
                foreach (var file in files)
                    mobileWorkOrderFactory.CreateWorkOrderImage(model.WorkOrderId, ImageFactory.StreamToByteArray(file.InputStream), CurrentCity.LocalTime);

            switch (submitButton)
            {
                case "Next":
                    //send them to the work order details page
                    return RedirectToAction("WorkOrderDetails", new { model.WorkOrderId, selectedSort, sortDirection });
                case "Suspend":
                    //put it back in the unassigned pool
                    mobileWorkOrderFactory.SuspendWorkOrder(model.WorkOrderId);
                    ////now send them back to the listing page
                    return RedirectToAction("WorkOrders", new { selectedSort, sortDirection });
                    break;
            }

            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));

            var techWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(model.WorkOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View("WorkSiteSafe", techWorkOrder);
        }
        #endregion

        #region Work Order Details
        /// <summary>
        /// Details for the work order with a lsit of all events for that work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkOrderDetails(long workOrderId, string selectedSort, string sortDirection)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString()
                , Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);

            //if this work order has no events assigned to it, then we must send the user to the resolve page.
            if(!techWorkOrder.WorkOrderEvents.Any())
                return RedirectToAction("ResolveWorkOrder", new { workOrderId, selectedSort, sortDirection });
              //  return View("ResolveWorkOrder", techWorkOrder);
            
            return View(techWorkOrder);
        }
        [HttpGet]
        public ActionResult GetWorkOrderDetails(long workOrderId, string selectedSort, string sortDirection, string submitButton)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //get the two
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Reject":
                    mobileWorkOrderFactory.RejectWorkOrder(workOrderId);
                    //now send them back to the details.
                    return RedirectToAction("WorkOrders", new {  selectedSort, sortDirection });
                    break;
                case "Resolve":
                    //send them tot he resolve work order page
                    return RedirectToAction("ResolveWorkOrder", new { workOrderId, selectedSort, sortDirection });
            }

            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));

            var techWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View("WorkOrderDetails", techWorkOrder);
        }

       #endregion

        #region Create work order
        /// <summary>
        /// Creates an event. Needs to call a web service ad it will return if there is an existing work order for this asset, otherwise needs to create work order, then event.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateEvent()
        {
            var mobileWorkOrderFactory =(new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var createModel = new TechnicianCreateEventModel
                {
                    FaultDescriptions = mobileWorkOrderFactory.GetFaultDescriptions(CurrentCity.Id),
                    AssetIdOptions = GetAssetIdOptions()
                };
            return View(createModel);
        }

        [HttpPost]
        public ActionResult CreateEvent(TechnicianCreateEventModel model, IEnumerable<HttpPostedFileBase> files, FormCollection coll)
        {
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(),
               Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //create the work order event in RBAC - 
            //get the selected event code the user chose
            var eventCode = mobileWorkOrderFactory.GetEventCode(model.SelectedFaultDescription, CurrentCity.Id);
            if (eventCode == null)
            {
                //if we got this far, something failed. add error to model and display to user
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Event Code not found. Try again."));

                //repopulate the options for the lists
                model.FaultDescriptions = mobileWorkOrderFactory.GetFaultDescriptions(CurrentCity.Id);
                model.AssetIdOptions = GetAssetIdOptions();
                //send them back to the view with the error
                return View("CreateEvent", model);
            }

            //first check to see if the selected asset key is valid
            //message / error handling if needed

            bool assetIsValid = true;
            if (string.IsNullOrEmpty(model.SelectedAssetKey) || model.SelectedAssetKey.Split('|').Count() != 3)
            {
                //if we got this far, something failed. add error to model and display to user
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Invalid Asset."));

                //repopulate the options for the lists
                model.FaultDescriptions = mobileWorkOrderFactory.GetFaultDescriptions(CurrentCity.Id);
                model.AssetIdOptions = GetAssetIdOptions();
                //send them back to the view with the error
                return View("CreateEvent", model);
            }

            //once it all checks out, create the work order, create the event id, and add the images, then send to the listing page
            //break out the SelectedAssetKey of the model  = CustomerId | AreaId | AssetId
            var ids = model.SelectedAssetKey.Split('|');
         
            
            var areaid = ids[1];
            var assetid = ids[2];
           
            int areaID = int.Parse(areaid);
            int assetID = int.Parse(assetid);
            //create work order
            var workOrderId = mobileWorkOrderFactory.CreateWorkOrder(assetID, model.SelectedFaultDescription, CurrentCity.Id, areaID,CurrentCity.LocalTime);

            //get event ID
            var webServiceFactory = new WebServiceFactory();
            //create the event  via the web service factory. 
            //create the Create Alarm Request object to pass to the web services
            var createAlarmRequest = new CreateAlarmRequest
            {
                AreaId = areaID,
                AssetId = assetID,
                CustomerId = CurrentCity.Id,
                EventCode = eventCode.EventCode1,
                EventSource = eventCode.EventSource,
                LocalTime = CurrentCity.LocalTime,
                Notes = model.Notes,
                WorkOrderId = workOrderId
            };

            var createAlarmResponse = webServiceFactory.CreateAlarm(createAlarmRequest);
            //check the response, if it failed, (eventUID is -1 or slaDue is minvalue
            if (!createAlarmResponse.IsValid)
            {
                //if we got this far, something failed. add error to model and display to user
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Event could not be created: " + createAlarmResponse.ErrorMessage));
                //repopulate the options for the lists
                model.FaultDescriptions = mobileWorkOrderFactory.GetFaultDescriptions(CurrentCity.Id);
                model.AssetIdOptions = GetAssetIdOptions();
                //send them back to the view with the error
                return View("CreateEvent", model);
            }

            //we found the event code, continue. just set the sla to 2 hours int he future, it will be updated below.
            var workOrderEventId = mobileWorkOrderFactory.CreateWorkOrderEvent(createAlarmResponse.EventUID, workOrderId, eventCode.EventCode1, CurrentCity.LocalTime, eventCode.EventDescVerbose,
                                                                           createAlarmResponse.SlaDue, eventCode.AlarmTier, model.Notes, false, false, (int)WorkOrderEventStatus.Open); 

            //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
            mobileWorkOrderFactory.UpdateEventAggregateInfo(workOrderId);

            //now we need to add the images to the work order (if there are any)
            if (files != null)
                foreach (var file in files)
                    mobileWorkOrderFactory.CreateWorkOrderImage(workOrderId, ImageFactory.StreamToByteArray(file.InputStream), CurrentCity.LocalTime);
            
            //now that it is created correctly, send them to the listing page so they can see thier new owrk order.
            return RedirectToAction("WorkOrders");
        }
        #endregion

        #region Event Details
        /// <summary>
        /// Get the details for a specific event / alarm
        /// </summary>
        /// <returns></returns>
        public ActionResult EventDetails(int eventId, int workOrderId , string selectedSort, string sortDirection)
        {
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(),
                Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));

            var workOrderEvent = mobileWorkOrderFactory.GetWorkOrderEvent(eventId, CurrentCity.Id);
            if (workOrderEvent == null)
                RedirectToAction("WorkOrderDetails", new {workOrderId,selectedSort, sortDirection});
            return View(workOrderEvent);
        }
        #endregion

        #region Work order images
        /// <summary>
        /// List of images for a work order (images are on the work order level)
        /// </summary>
        /// <returns></returns>
        public ActionResult Images(long workOrderId, string selectedSort, string sortDirection)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(),
                Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View(techWorkOrder);
        }

        [HttpPost]
        public ActionResult Images(TechnicianWorkOrderDetailsModel model, string selectedSort, string sortDirection, IEnumerable<HttpPostedFileBase> files)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //now we need to add the images to the work order (if there are any)
            if (files != null)
                foreach (var file in files)
                    mobileWorkOrderFactory.CreateWorkOrderImage(model.WorkOrderId, ImageFactory.StreamToByteArray(file.InputStream), CurrentCity.LocalTime);
            return RedirectToAction("ResolveWorkOrder", new { model.WorkOrderId, selectedSort, sortDirection });
        }
        #endregion

        #region Parts
        /// <summary>
        /// List of parts for this work order (parts are on the work order level)
        /// </summary>
        /// <returns></returns>
        public ActionResult Parts(long workOrderId, string selectedSort, string sortDirection)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(),
                Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
           
            //add the "select one" here
            techWorkOrder.AvailableParts.Insert(0, new AvailablePart { PartDesc = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Select One"), PartId = -1, PartName = "-1"});
            //add the "other" part here. this way the view knows when to force validation on the notes field
            //techWorkOrder.AvailableParts.Add(new AvailablePart { PartDesc = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Other"), PartId = -2, PartName = Constants.FieldMaintenance.OtherPartName});
            return View(techWorkOrder);
        }

        [HttpPost]
        public ActionResult Parts(TechnicianWorkOrderDetailsModel model, string selectedSort, string sortDirection, string submitButton, FormCollection formValues)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory =(new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            
            //determine if we need to add parts or update values for parts
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Add":
                    //add the available part to the work order as a work order part
                    //get the selectd part ID
                    long partID;
                    //poart will be PartId | PartName
                    var partIdentifier = model.SelectedPart.Split('|');
                    bool parsed = long.TryParse(partIdentifier[0], out partID);
                    if (parsed)
                        mobileWorkOrderFactory.AddPartToWorkOrder(partID, model.WorkOrderId, model.PartNote);
                    break;
                case "Save":
                    //rebuild the model with the work order parts and quantities passed in from the forms colleciton
                    model = RebuildTechnicianWorkOrderDetailsModel(model, formValues);
                    //now pass it to the factory to update part quantities
                    //save all of the part values and then send them back to this page again
                    mobileWorkOrderFactory.UpdateWorkOrderPartQuantities(model);
                    break;
            }
            //now send them back to this page so the user can see the modifications
            return RedirectToAction("Parts", new {workOrderId = model.WorkOrderId, selectedSort, sortDirection});
        }

        private TechnicianWorkOrderDetailsModel RebuildTechnicianWorkOrderDetailsModel(TechnicianWorkOrderDetailsModel model, FormCollection formValues)
        {
            // Prep the Model.Coins class 
            if (model.WorkOrderParts == null)
                model.WorkOrderParts =  new List<WorkOrderPart>();

            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.
            foreach (var formValueKey in formValues.Keys)
            {
                string[] tokens = formValueKey.ToString().Split(model.Separator);
                var formValue = formValues[formValueKey.ToString()];


                //token is going to be "partId_" then the id of the part
                if (tokens.Length < 2)
                    continue;
                // Get model id
                int woPartId = int.Parse(tokens[1]);

                if (tokens[0].Equals(TechnicianWorkOrderDetailsModel.PartIdPrefix))
                {
                    //this is a textbox that represents a number (quantity). try to part it as an int.
                    int quantity;
                    var parsed =int.TryParse(formValue, out quantity);
                    if (parsed)
                    {
                        //create a work order part and add it to the list
                        var woPart = new WorkOrderPart();
                        woPart.WorkOrderPartId = woPartId;
                        woPart.Quantity = quantity;
                        model.WorkOrderParts.Add(woPart);
                    }
                    else
                    {
                        //if we cant parse it, they either didnt put anything there, or put a nonsensical value (a), so set it to 0 so we can remove it.
                        var woPart = new WorkOrderPart();
                        woPart.WorkOrderPartId = woPartId;
                        woPart.Quantity = 0;
                        model.WorkOrderParts.Add(woPart);
                    }
                }
            }
            return model;
        }
        #endregion

        #region Reject Work Order
        /// <summary>
        /// Resolve a work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult RejectWorkOrder(long workOrderId, string selectedSort, string sortDirection)
        {
            ////every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
            //mobileWorkOrderFactory.UpdateEventAggregateInfo(workOrderId);
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString()
                , Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View(techWorkOrder);
        }

        /// <summary>
        /// Resolve a work order.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RejectWorkOrder(TechnicianWorkOrderDetailsModel model, string selectedSort, string sortDirection, string submitButton)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //get the two
            mobileWorkOrderFactory.UpdateNotes(model.WorkOrderId, model.Notes);
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Reject":
                    mobileWorkOrderFactory.RejectWorkOrder(model.WorkOrderId);
                    //now send them back to the details.
                    return RedirectToAction("WorkOrders", new { selectedSort, sortDirection });
                    break;
            }

            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));
            var techWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(model.WorkOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View("RejectWorkOrder", techWorkOrder);
        }

        #endregion


        #region Resolution
        /// <summary>
        /// Resolve a work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResolveWorkOrder(long workOrderId, string selectedSort, string sortDirection)
        {
            ////every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
            //mobileWorkOrderFactory.UpdateEventAggregateInfo(workOrderId);
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString()
                , Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            techWorkOrder.RepairDescriptions.Insert(0,  new SelectListItem{Text = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Select One") , Value = "-1" });

            //we have to add the initial (select one) text
            return View(techWorkOrder);
        }

        /// <summary>
        /// Resolve a work order.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResolveWorkOrder(TechnicianWorkOrderDetailsModel model, string selectedSort, string sortDirection, string submitButton, string ddlVandalism)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(), Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //get the two
            mobileWorkOrderFactory.UpdateNotes(model.WorkOrderId, model.Notes);
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Parts":
                    return RedirectToAction("Parts", new {workOrderId = model.WorkOrderId, selectedSort, sortDirection });
                case "Images":
                    return RedirectToAction("Images", new { workOrderId = model.WorkOrderId, selectedSort, sortDirection });
                case "Close":
                    //have to get hte vandalism flag and the resolution code from the user selections
                    int resCode;
                    int.TryParse(model.SelectedRepairDescription, out resCode);
                    bool vandalism = ddlVandalism == "Yes";
                    mobileWorkOrderFactory.ResolveWorkOrder(model.WorkOrderId, resCode, vandalism);

                    //check to see if the work order evetns closed correctly.
                    if (!string.IsNullOrEmpty(mobileWorkOrderFactory.ErrorMessage))
                    {
                        //if we got this far, something failed. add error to model and display to user
                        ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, mobileWorkOrderFactory.ErrorMessage));
                        var tWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(model.WorkOrderId, CurrentCity.Id, CurrentCity.LocalTime);
                        return View("ResolveWorkOrder", tWorkOrder);
                    }

                    //now send them back to the details.
                    return RedirectToAction("WorkOrders", new {selectedSort, sortDirection });
            }

            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));

            var techWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(model.WorkOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View("ResolveWorkOrder", techWorkOrder);
        }

        /// <summary>
        /// Resolve multiple events (alarms) for a work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResolveEvents(long workOrderId, string selectedSort, string sortDirection)
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var techWorkOrder = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(),
                Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View(techWorkOrder);
        }

        [HttpGet]
        public ActionResult GetResolveEvents(long workOrderId, string selectedSort, string sortDirection, long[] eventIds,string submitButton )
        {
            ViewBag.SortOrder = selectedSort;
            ViewBag.SortDirection = sortDirection;
            var mobileWorkOrderFactory = (new TechnicianWorkOrderFactory(Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString(),Session[Constants.Security.ConnectionStringSessionVariableName].ToString()));
            //get the two
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Resolve":
                      mobileWorkOrderFactory.ResolveEvents(eventIds,CurrentCity.LocalTime);
                      //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
                      mobileWorkOrderFactory.UpdateEventAggregateInfo(workOrderId);

                    //check to see if the work order evetns closed correctly.
                    if (!string.IsNullOrEmpty(mobileWorkOrderFactory.ErrorMessage))
                    {
                        //if we got this far, something failed. add error to model and display to user
                        ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Some events did not close. Try again later."));
                        ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, mobileWorkOrderFactory.ErrorMessage));

                        var tWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
                        return View("ResolveEvents", tWorkOrder);
                    }

                    //now send them back to the details.
                    return RedirectToAction("WorkOrderDetails", new { workOrderId,selectedSort, sortDirection });
            }

            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));

            var techWorkOrder = mobileWorkOrderFactory.GetWorkOrderDetails(workOrderId, CurrentCity.Id, CurrentCity.LocalTime);
            return View("ResolveEvents", techWorkOrder);
        }
        #endregion
    }
}
