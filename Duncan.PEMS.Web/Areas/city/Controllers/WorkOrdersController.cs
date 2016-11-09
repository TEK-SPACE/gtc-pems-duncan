using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Utility;
using Duncan.PEMS.Business.WebServices;
using Duncan.PEMS.Business.WorkOrders;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.WebServices.FieldMaintenance;
using Duncan.PEMS.Entities.WorkOrders;
using Duncan.PEMS.Entities.WorkOrders.Dispatcher;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;
using NPOI.HSSF.UserModel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
     [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class WorkOrdersController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Listing
         public ActionResult Index()
         {
             ViewBag.CurrentCityID = CurrentCity.Id;
             return View();
         }

         [HttpGet]
         public ActionResult GetWorkOrders([DataSourceRequest] DataSourceRequest request, int alarmCode = -1)
         {
             //now save the sort filters
             if (request.Sorts.Count > 0)
                 SetSavedSortValues(request.Sorts, "WorkOrder");

             //get models
             var models = (new DispatcherWorkOrderFactory(CurrentCity)).GetWorkOrders(alarmCode);


             //sort, page, filter, group them
             var items = models.AsQueryable();
             items = items.ApplyFiltering(request.Filters);
             var total = items.Count();
             items = items.ApplySorting(request.Groups, request.Sorts);
             items = items.ApplyPaging(request.Page, request.PageSize);
             IEnumerable data = items.ToList();

             DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
             //var result = new DataSourceResult
             {
                 Data = data,
                 Total = total
             };

             return Json(result, JsonRequestBehavior.AllowGet);
         }
       
        #endregion

        #region "Filter Values"
         /// <summary>
         /// Gets all of the possible asset tyeps for all customers in the current maintenance group
         /// </summary>
         /// <returns></returns>
         public JsonResult GetAssetTypes()
         {
             //get all of the unique asset types for each customer in the maintenance group
             //get the maintenance group
             var maintGroup = new PemsCity(Session[Constants.ViewData.CurrentCityId].ToString());
             var items = (new DispatcherWorkOrderFactory(maintGroup)).GetWorkOrderAssetTypes();
             return Json(items, JsonRequestBehavior.AllowGet);
         }

         /// <summary>
         /// Gets all of the possible assignment States for all customers in the current maintenance group
         /// </summary>
         /// <returns></returns>
         public JsonResult GetAssignmentStates()
         {
             //get all of the unique assignment states for each customer in the maintenance group
             //get the maintenance group
             var maintGroup = new PemsCity(Session[Constants.ViewData.CurrentCityId].ToString());
             var items = (new DispatcherWorkOrderFactory(maintGroup)).GetWorkOrderAssignmentStates();
             return Json(items, JsonRequestBehavior.AllowGet);
         }

         public JsonResult GetTechnicians()
         {
             //get all of the technicains for each customer in the maintenance group
             //get the maintenance group
             var maintGroup = new PemsCity(Session[Constants.ViewData.CurrentCityId].ToString());
             var items = (new DispatcherWorkOrderFactory(maintGroup)).GetWorkOrderTechnicians();
             return Json(items, JsonRequestBehavior.AllowGet);
         }

         /// <summary>
         /// Gets all the asset types int he systme
         /// </summary>
         /// <returns></returns>
        public JsonResult GetPartAssetTypes()
        {
            //get a unique list of meter groups in the system for all of the customers in a maintenance group.
            var items = (new DispatcherWorkOrderFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString())).GetAllMeterGroups();
            return Json(items, JsonRequestBehavior.AllowGet);
        }
         /// <summary>
         /// Get all the asset sub-types (categories, or mechanism masters) in the system
         /// </summary>
         /// <returns></returns>
        public JsonResult GetPartCategories()
        {
            //get a unique list of meter groups in the system for all of the customers in a maintenance group.
            var items = (new DispatcherWorkOrderFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString())).GetAllMechanisms();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Work Order Exporting

        public FileResult ExportToCsv([DataSourceRequest]DataSourceRequest request, int alarmCode = -1)
        {
            var items = GetExportData(request, alarmCode);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetWorkOrders", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "WorkOrdersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, int alarmCode = -1)
        {
            var items = GetExportData(request, alarmCode);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            //we have to add alarm code
            if (alarmCode > -1)
                filters.Add(new FilterDescriptor { Member = "Alarm Code", Value = alarmCode.ToString() });

            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetWorkOrders", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "WorkOrdersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request, int alarmCode = -1)
        {
            var items = GetExportData(request, alarmCode);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            //we have to add alarm code
            if (alarmCode > -1)
                filters.Add(new FilterDescriptor { Member = "Alarm Code", Value = alarmCode.ToString() });
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetWorkOrders", CurrentCity.Id, filters, 2);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "WorkOrdersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable<DispatcherWorkOrderListModel> GetExportData(DataSourceRequest request, int alarmCode = -1)
        {
            //get models
            var models = (new DispatcherWorkOrderFactory(CurrentCity)).GetWorkOrders(alarmCode);

            //sort, page, filter, group them
            var items = models.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            var total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            return items.ToList();
        }

        #endregion

        #region "Parts Exporting"

        public FileResult ExportPartsToCsv([DataSourceRequest]DataSourceRequest request)
        {
            var items = GetPartsExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetAvailableParts", CurrentCity.Id);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Part ID"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Part Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Asset Type"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Category"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Part Description"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Retail Cost"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Part Status"), true);
            foreach (AvailablePart item in items)
            {
                //add a item for each property here
                AddCsvValue(writer, item.PartId.ToString());
                AddCsvValue(writer, item.PartName);
                AddCsvValue(writer, item.MeterGroupDisplay);
                AddCsvValue(writer, item.CategoryDisplay);
                AddCsvValue(writer, item.PartDesc);
                AddCsvValue(writer, item.CostInCentsDisplay);
                AddCsvValue(writer, item.StatusDisplay, true);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "PartsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportPartsToExcel([DataSourceRequest]DataSourceRequest request)
        {
            //get role models
            var data = GetPartsExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetAvailableParts", CurrentCity.Id);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue(GetLocalizedGridTitle(gridData, "Part ID"));
            headerRow.CreateCell(1).SetCellValue(GetLocalizedGridTitle(gridData, "Part Name"));
            headerRow.CreateCell(2).SetCellValue(GetLocalizedGridTitle(gridData, "Asset Type"));
            headerRow.CreateCell(3).SetCellValue(GetLocalizedGridTitle(gridData, "Category"));
            headerRow.CreateCell(4).SetCellValue(GetLocalizedGridTitle(gridData, "Part Description"));
            headerRow.CreateCell(5).SetCellValue(GetLocalizedGridTitle(gridData, "Retail Cost"));
            headerRow.CreateCell(6).SetCellValue(GetLocalizedGridTitle(gridData, "Part Status"));

            //Populate the sheet with values from the grid data
            foreach (AvailablePart item in data)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.PartId.ToString());
                row.CreateCell(columnNumber++).SetCellValue(item.PartName);
                row.CreateCell(columnNumber++).SetCellValue(item.MeterGroupDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.CategoryDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.PartDesc);
                row.CreateCell(columnNumber++).SetCellValue(item.CostInCentsDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.StatusDisplay);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "PartsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
            }

        public FileResult ExportPartsToPdf([DataSourceRequest]DataSourceRequest request)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            //step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            //step 3: we open the document
            document.Open();

            //step 4: we add content to the document
            //first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));
            var data = GetPartsExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetAvailableParts", CurrentCity.Id);

            var dataTable = new PdfPTable(7) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Part ID"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Part Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Asset Type"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Category"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Part Description"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Retail Cost"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Part Status"));
            dataTable.CompleteRow();
            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;

            foreach (AvailablePart item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.PartId.ToString());
                dataTable.AddCell(item.PartName);
                dataTable.AddCell(item.MeterGroupDisplay);
                dataTable.AddCell(item.CategoryDisplay);
                dataTable.AddCell(item.PartDesc);
                dataTable.AddCell(item.CostInCentsDisplay);
                dataTable.AddCell(item.StatusDisplay);
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "PartsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        private IEnumerable<AvailablePart> GetPartsExportData(DataSourceRequest request)
        {
            //get models
            var parts = (new DispatcherWorkOrderFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString())).GetAllAvailableParts();

            //sort, page, filter, group them
            var items = parts.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            var total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            return items.ToList();
        }
        #endregion

        #region Create Event
        public ActionResult CreateEventId()
        {
            var createModel = new DispatcherCreateEventModel();

            //we have to pull a facotory for each customer, since their conneciton string could be different.
            foreach (var maintenanceCustomer in CurrentCity.MaintenanceCustomers)
            {
               var woFactory = (new DispatcherWorkOrderFactory(maintenanceCustomer));
               createModel.FaultDescriptions.AddRange(woFactory.GetFaultDescriptions(maintenanceCustomer.Id));
                var customerAssetOptions = woFactory.GetAssetsForCustomer(maintenanceCustomer.Id);
                //fix up all the customer names for the asset options
                customerAssetOptions.ForEach(x=>x.CustomerName = maintenanceCustomer.DisplayName);
                createModel.AssetIdOptions.AddRange(customerAssetOptions);
            }
            return View(createModel);
        }

        [HttpPost]
        public ActionResult CreateEventId(DispatcherCreateEventModel model)
        {
            //message / error handling if needed
            //once it all checks out, create the work order, create the event id,  then send to the listing page
            //break out the SelectedAssetKey of the model  = CustomerId | AreaId | AssetId
            var ids = model.SelectedAssetKey.Split('|');

            var customerId = ids[0];
            var areaid = ids[1];
            var assetid = ids[2];
            int customerID = int.Parse(customerId);
            //instantiate the factory with the current maintenance group and the selected city for the asset

            var selectedCustomer = CurrentCity.MaintenanceCustomers.FirstOrDefault(x => x.Id == customerID);
            if (selectedCustomer != null)
            {
                var workOrderFactory = (new DispatcherWorkOrderFactory(selectedCustomer.PemsConnectionStringName, CurrentCity.MaintenanceConnectionStringName));
                //create the work order event in RBAC - 
                //get the selected event code the user chose
                var eventCode = workOrderFactory.GetEventCode(model.SelectedFaultDescription, selectedCustomer.Id);
                if (eventCode == null)
                {
                    //if we got this far, something failed. add error to model and display to user
                    ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                             (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,
                                                                                       "Event Code not found. Try again."));
                    //repopulate the options for the lists
                    //we have to pull a facotory for each customer, since their conneciton string could be different.
                    foreach (var maintenanceCustomer in CurrentCity.MaintenanceCustomers)
                    {
                        var woFactory = (new DispatcherWorkOrderFactory(maintenanceCustomer));
                        model.FaultDescriptions.AddRange(woFactory.GetFaultDescriptions(maintenanceCustomer.Id));
                        var customerAssetOptions = woFactory.GetAssetsForCustomer(maintenanceCustomer.Id);
                        //fix up all the customer names for the asset options
                        customerAssetOptions.ForEach(x => x.CustomerName = maintenanceCustomer.DisplayName);
                        model.AssetIdOptions.AddRange(customerAssetOptions);
                    }
                    //send them back to the view with the error
                    return View("CreateEventId", model);
                }
                
                int areaID = int.Parse(areaid);
                int assetID = int.Parse(assetid);
                //create work order
                var workOrderId = workOrderFactory.CreateWorkOrder(assetID, model.SelectedFaultDescription,selectedCustomer.Id, areaID,selectedCustomer.LocalTime, model.CrossStreet,model.Notes);

                //get event ID
                var webServiceFactory = new WebServiceFactory();
                //create the event  via the web service factory. 
                //create the Create Alarm Request object to pass to the web services
                var createAlarmRequest = new CreateAlarmRequest
                    {
                        AreaId = areaID,
                        AssetId = assetID,
                        CustomerId = selectedCustomer.Id,
                        EventCode = eventCode.EventCode1,
                        EventSource = eventCode.EventSource,
                        LocalTime = selectedCustomer.LocalTime,
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
                    //we have to pull a facotory for each customer, since their conneciton string could be different.
                    foreach (var maintenanceCustomer in CurrentCity.MaintenanceCustomers)
                    {
                        var woFactory = (new DispatcherWorkOrderFactory(maintenanceCustomer));
                        model.FaultDescriptions.AddRange(woFactory.GetFaultDescriptions(maintenanceCustomer.Id));
                        var customerAssetOptions = woFactory.GetAssetsForCustomer(maintenanceCustomer.Id);
                        //fix up all the customer names for the asset options
                        customerAssetOptions.ForEach(x => x.CustomerName = maintenanceCustomer.DisplayName);
                        model.AssetIdOptions.AddRange(customerAssetOptions);
                    }
                    //send them back to the view with the error
                    return View("CreateEventId", model);
                }

                //we found the event code, continue. just set the sla to 2 hours int he future, it will be updated below.
                var workOrderEventId = workOrderFactory.CreateWorkOrderEvent(createAlarmResponse.EventUID, workOrderId,eventCode.EventCode1, CurrentCity.LocalTime,eventCode.EventDescVerbose,createAlarmResponse.SlaDue,eventCode.AlarmTier, model.Notes, false,false, (int) WorkOrderEventStatus.Open);

                //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
                workOrderFactory.UpdateEventAggregateInfo(workOrderId);

                //now that it is created correctly, send them to the listing page so they can see thier new owrk order.
                return RedirectToAction("WorkOrderDetail", "WorkOrders", new {workOrderId});
            }

            //fail here, no customer found, refresh page, add error, whatever
            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Customer Not Found. Try again."));
            //repopulate the options for the lists
            //we have to pull a facotory for each customer, since their conneciton string could be different.
            foreach (var maintenanceCustomer in CurrentCity.MaintenanceCustomers)
            {
                var woFactory = (new DispatcherWorkOrderFactory(maintenanceCustomer));
                model.FaultDescriptions.AddRange(woFactory.GetFaultDescriptions(maintenanceCustomer.Id));
                var customerAssetOptions = woFactory.GetAssetsForCustomer(maintenanceCustomer.Id);
                //fix up all the customer names for the asset options
                customerAssetOptions.ForEach(x => x.CustomerName = maintenanceCustomer.DisplayName);
                model.AssetIdOptions.AddRange(customerAssetOptions);
            }
            return View("CreateEventId", model);
        }

        #endregion

        #region Work Order Details
        /// <summary>
        /// Gets the work order details
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <returns></returns>
        public ActionResult WorkOrderDetail(int workOrderId)
        {
            var model = (new DispatcherWorkOrderFactory(CurrentCity)).GetWorkOrderDetails(workOrderId, CurrentCity.LocalTime);
            return View(model);
        }

         /// <summary>
         /// Action method called on button click of the work order details page (assign, unassign)
         /// </summary>
         /// <param name="model"></param>
         /// <param name="submitButton"></param>
         /// <returns></returns>
         [HttpPost]
        public ActionResult WorkOrderDetail(DispatcherWorkOrderDetailsModel model, string submitButton, IEnumerable<HttpPostedFileBase> files, FormCollection formValues)
         {
             var factory = (new DispatcherWorkOrderFactory(CurrentCity));
             switch (submitButton)
             {
                 case "Assign":
                     // assign the work order to the technicianid of the model (SelectedTechnician) and redirect back to the details page
                     factory.AssignWorkOrder(model.WorkOrderId, model.SelectedTechnicianId, CurrentCity.LocalTime);
                     break;
                 case "Unassign":
                     // unassign the work order  and redirect back to the details page
                     factory.UnassignWorkOrder(model.WorkOrderId);
                     break;
                 case "SaveImages":
                     //now we need to add the images to the work order (if there are any)
                     if (files != null)
                         foreach (var file in files)
                             factory.CreateWorkOrderImage(model.WorkOrderId,
                                                          ImageFactory.StreamToByteArray(file.InputStream),
                                                          CurrentCity.LocalTime);
                     break;
                 case "AddPart":
                     //add the available part to the work order as a work order part
                     //get the selectd part ID
                     long partID;
                     //poart will be PartId | PartName
                     var partIdentifier = model.SelectedPart.Split('|');
                     bool parsed = long.TryParse(partIdentifier[0], out partID);
                     if (parsed)
                         factory.AddPartToWorkOrder(partID, model.WorkOrderId, model.PartNote);
                     break;
                 case "UpdateQuantities":
                     //rebuild the model with the work order parts and quantities passed in from the forms colleciton
                     model = RebuildTechnicianWorkOrderDetailsModel(model, formValues);
                     //now pass it to the factory to update part quantities
                     //save all of the part values and then send them back to this page again
                     factory.UpdateWorkOrderPartQuantities(model);
                     break;
             }
             //redirect back to the details page so the changes wil be reflected.
             return RedirectToAction("WorkOrderDetail", new { model.WorkOrderId });
         }

         private DispatcherWorkOrderDetailsModel RebuildTechnicianWorkOrderDetailsModel(DispatcherWorkOrderDetailsModel model, FormCollection formValues)
         {
             // Prep the Model.Coins class 
             if (model.WorkOrderParts == null)
                 model.WorkOrderParts = new List<WorkOrderPart>();

             // Walk the form fields and set any values in the model to values reflected by
             // the form fields.
             foreach (var formValueKey in formValues.Keys)
             {
                 string[] tokens = formValueKey.ToString().Split(model.Separator);
                 var formValue = formValues[formValueKey.ToString()];


                 //token is going to be "partId_" then the id of the part
                 if (tokens.Length != 2)
                     continue;
                 // Get model id
              
                 int woPartID;
                 if (int.TryParse(tokens[1], out woPartID))
                 {
                     if (tokens[0].Equals(DispatcherWorkOrderDetailsModel.PartIdPrefix))
                     {
                         //this is a textbox that represents a number (quantity). try to part it as an int.
                         int quantity;
                         var parsed = int.TryParse(formValue, out quantity);
                         if (parsed)
                         {
                             //create a work order part and add it to the list
                             var woPart = new WorkOrderPart();
                             woPart.WorkOrderPartId = woPartID;
                             woPart.Quantity = quantity;
                             model.WorkOrderParts.Add(woPart);
                         }
                         else
                         {
                             //if we cant parse it, they either didnt put anything there, or put a nonsensical value (a), so set it to 0 so we can remove it.
                             var woPart = new WorkOrderPart();
                             woPart.WorkOrderPartId = woPartID;
                             woPart.Quantity = 0;
                             model.WorkOrderParts.Add(woPart);
                         }
                     }
                 }
             }
             return model;
         }

        #endregion

         #region Work Order Event Details
         /// <summary>
         /// Gets the event details for a work order
         /// </summary>
         /// <param name="eventId"></param>
         /// <param name="workOrderId"></param>
         /// <returns></returns>
         public ActionResult EventDetail(int eventId, int workOrderId)
         {
             var factory = (new DispatcherWorkOrderFactory(CurrentCity));

             var workOrderEvent = factory.GetWorkOrderEvent(eventId);
             if (workOrderEvent == null)
                 RedirectToAction("WorkOrderDetail", new { workOrderId });
             return View(workOrderEvent);
         }
         #endregion

         #region Parts
        public ActionResult PartsIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetAvailableParts([DataSourceRequest] DataSourceRequest request)
        {
            //get models
            var parts = (new DispatcherWorkOrderFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), Session[Constants.Security.MaintenanceConnectionStringSessionVariableName].ToString())).GetAllAvailableParts();

            //sort, page, filter, group them
            var items = parts.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            var total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.ToList();

            var result = new DataSourceResult
            {
                Data = data,
                Total = total
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
         public ActionResult PartsImport()
         {
             return View();
         }


         private const string SessionFilePath = "PemsPartsBulkUpdate";
         private const string SessionFileName = "PemsPartsBulkUpdateFile";


         /// <summary>
         /// Handles the submission of the asset upload file.
         /// </summary>
         /// <param name="files">List of files uploaded.  Only care about first file.</param>
         /// <returns></returns>
         public ActionResult UploadParts(IEnumerable<HttpPostedFileBase> files)
         {
             // Clear the session variables as this is a new upload.
             Session.Remove(SessionFilePath);
             Session.Remove(SessionFileName);

             if (files != null)
             {
                 foreach (var file in files)
                 {
                     // Some browsers send file names with full path. We only care about the file name.
                     var uniqueFileName = Guid.NewGuid().ToString() + ".csv";
                     var destinationPath =
                         Path.Combine(
                             Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.parts.upload"]),
                             uniqueFileName);
                     file.SaveAs(destinationPath);

                     Session[SessionFilePath] = destinationPath;
                     Session[SessionFileName] = Path.GetFileName(file.FileName);

                     // Only care about first file.
                     break;
                 }

                 // Pass processing on to UploadProcess handler.
                 return RedirectToAction("ProcessParts");
             }
             return RedirectToAction("PartsImport");
         }

         public ActionResult ProcessParts()
         {
             DispatcherPartUploadResultModel model = null;

             string partsFile = Session[SessionFilePath] as string;
             string fileName = Session[SessionFileName] as string;

             if (string.IsNullOrEmpty(partsFile))
             {
                 model = new DispatcherPartUploadResultModel();
                 model.Errors.Add("Unable to process uploaded file");
             }
             else
             {
                 // Process the file.
                 var factory = (new DispatcherWorkOrderFactory(CurrentCity));

                 model = factory.ProcessPartImport(partsFile);

                 // Note name of file in Results log.
                 model.Results.Insert(0, "File Processed: " + fileName);

                 // Clear the session variable.
                 this.Session.Remove(SessionFilePath);
                 this.Session.Remove(SessionFileName);

                 // Delete the file.
                 System.IO.File.Delete(partsFile);
             }
             return View(model);
         }


         #endregion

         #region AJAX Support for Examples and Instructions Files

         /// <summary>
         /// Return a <see cref="FileResult"/> of an example file for the parts.
         /// </summary>
         public FileResult DownloadExample()
         {
             var fileName = "Parts.csv";
             var fullFilePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.parts.samples"]), fileName);
             return File(fullFilePath, "text/csv", fileName);
         }

         /// <summary>
         /// Return a <see cref="FileResult"/> of an instruction file for the appropriate resource type.
         /// </summary>
         public FileResult DownloadInstructions()
         {
             // Make a copy of the appropriate instrunction template file.
             var fileName = "Parts.txt";
             var sourcePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.parts.samples"]), fileName);

             var uniqueFileName = Guid.NewGuid().ToString() + ".txt";
             var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.upload"]), uniqueFileName);

             System.IO.File.Copy(sourcePath, destinationPath, true);

             // Open the new file
             using (StreamWriter sw = System.IO.File.AppendText(destinationPath))
             {
                 // Append the date to it.
                 sw.WriteLine();
                 sw.WriteLine("Created: " + DateTime.Now.ToShortDateString());
                 sw.WriteLine();

                 // Add appropriate constraints.
                 var factory = (new DispatcherWorkOrderFactory(CurrentCity));
                 sw.WriteLine(factory.PartsConstraintsList());
                 sw.WriteLine();

                 // Add appropriate field notes
                 sw.WriteLine();
                 sw.WriteLine(factory.PartsFieldsList());

                 // Save file.
                 sw.Close();
             }


             return File(destinationPath, "text/txt", fileName);
         }

         #endregion

         #region Bulk Updates

         public JsonResult SetActionableWorkOrderIds(string[] uns)
        {
            Session["__SavedActionableWorkOrderIds"] = uns;
            return null;
        }

        public JsonResult ClearActionableWorkOrderIds()
        {
            Session["__SavedActionableWorkOrderIds"] = null;
            return null;
        }

        private string[] GetActionableWorkOrderIds()
        {
            if (Session["__SavedActionableWorkOrderIds"] != null)
            {
                var retVal = Session["__SavedActionableWorkOrderIds"] as string[];
                //clear the session
                return retVal;
            }
            return null;
        }

        public ActionResult WorkOrderBulkAction(string ddlActionValue)
        {
            switch (ddlActionValue)
            {
                case "Assign":
                    return RedirectToAction("MassAssign");
                case "UnAssign":
                    return RedirectToAction("MassUnassign");
            }
            //nothing happened. we verify this has a field, so just default them to the assign.
            return RedirectToAction("MassAssign");
        }

        public ActionResult MassAssign()
        {
            ////get the saved work order ids that we are working with
            var model = new DispatcherBulkUpdateModel();
            model.WorkOrderIds = GetActionableWorkOrderIds() ;
            if (model.WorkOrderIds == null)
                return RedirectToAction("Index");
            model.AvailableTechnicians = (new DispatcherWorkOrderFactory(CurrentCity)).GetMaintenanceGroupTechnicians(CurrentCity);
            return View(model);
        }

         [HttpPost]
        public ActionResult MassAssign(DispatcherBulkUpdateModel model)
        {
          //assign all of the actionable items to the selected technician
            var factory = (new DispatcherWorkOrderFactory(CurrentCity));
             var woIds = GetActionableWorkOrderIds();
             if (woIds != null)
             {
                 foreach (var woId in woIds)
                 {
                     int woID;
                     if (int.TryParse(woId, out woID))
                     {
                         factory.AssignWorkOrder(woID, model.SelectedTechnicianId, CurrentCity.LocalTime);
                     }
                 }
             }
             //now clear actionablework order ids
             ClearActionableWorkOrderIds();
             //send them back to the indexx page
             return RedirectToAction("Index");
        }

        public ActionResult MassUnassign()
        {
            var model = new DispatcherBulkUpdateModel();
            model.WorkOrderIds = GetActionableWorkOrderIds();
            if (model.WorkOrderIds == null)
                return RedirectToAction("Index");
            return View(model);
        }
        [HttpPost]
        public ActionResult MassUnassign(DispatcherBulkUpdateModel model)
        {
            //unassign all of the actionable items 
            var factory = (new DispatcherWorkOrderFactory(CurrentCity));
            var woIds = GetActionableWorkOrderIds();
            if (woIds != null)
            {
                foreach (var woId in woIds)
                {
                    int woID;
                    if (int.TryParse(woId, out woID))
                        factory.UnassignWorkOrder(woID);
                }
            }
            //now clear actionablework order ids
            ClearActionableWorkOrderIds();
            //send them back to the indexx page
            return RedirectToAction("Index");
        }

        #endregion

        #region Work Load
        /// <summary>
        /// Loads the work load and filters it down to a specific technician (if the id is passed in)
        /// </summary>
        /// <param name="techId"></param>
        /// <returns></returns>
        public ActionResult WorkLoad(int techId = -1)
        {
            //get a list of unique technicians for all fo the customers for the current maintenance group

            //aggergate the counts of completed (for today) and assigned
            var model = (new DispatcherWorkOrderFactory(CurrentCity)).GetWorkLoadListingModel();

            //if the technician id passed in exists, then filter down to display only that technician
            if (techId > 0)
            {
                var singleModel = model.Where(x => x.TechnicianID == techId);
                return View(singleModel);
            }

            return View(model);
        }
        #endregion



        #region Resolve Work Order

        #region Resolution
        /// <summary>
        /// Resolve a work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResolveWorkOrder(long workOrderId, string selectedSort, string sortDirection)
        {
            var model = (new DispatcherWorkOrderFactory(CurrentCity)).GetWorkOrderDetails(workOrderId, CurrentCity.LocalTime);
            model.RepairDescriptions.Insert(0, new SelectListItem { Text = (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.DropDownDefault, "Select One"), Value = "-1" });
            //we have to add the initial (select one) text
            return View(model);
        }

        /// <summary>
        /// Resolve a work order.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResolveWorkOrder(DispatcherWorkOrderDetailsModel model,  string submitButton, string ddlVandalism)
        {
            var factory = (new DispatcherWorkOrderFactory(CurrentCity));
            //get the two
            factory.UpdateNotes(model.WorkOrderId, model.Notes);
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Close":
                    //have to get hte vandalism flag and the resolution code from the user selections
                    int resCode;
                    int.TryParse(model.SelectedRepairDescription, out resCode);
                    bool vandalism = ddlVandalism == "Yes";
                    factory.ResolveWorkOrder(model.WorkOrderId, resCode, vandalism);

                    //check to see if the work order evetns closed correctly.
                    if (!string.IsNullOrEmpty(factory.ErrorMessage))
                    {
                        //if we got this far, something failed. add error to model and display to user
                        ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, factory.ErrorMessage);
                        var tWorkOrder = factory.GetWorkOrderDetails(model.WorkOrderId, CurrentCity.LocalTime);
                        return View("ResolveWorkOrder", tWorkOrder);
                    }
                    //now send them back to the details.
                    return RedirectToAction("WorkOrderDetail", new { workOrderId = model.WorkOrderId});
            }
            
            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));

            var techWorkOrder = factory.GetWorkOrderDetails(model.WorkOrderId,CurrentCity.LocalTime);
            return View("ResolveWorkOrder", techWorkOrder);
        }

        /// <summary>
        /// Resolve multiple events (alarms) for a work order.
        /// </summary>
        /// <returns></returns>
        public ActionResult ResolveEvents(long workOrderId)
        {
            var model = (new DispatcherWorkOrderFactory(CurrentCity)).GetWorkOrderDetails(workOrderId, CurrentCity.LocalTime);
            return View(model);
        }

        [HttpGet]
        public ActionResult DPResolveEvents(long workOrderId, long[] eventIds, string submitButton)
        {
            var factory = (new DispatcherWorkOrderFactory(CurrentCity));
            //get the two
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Resolve":
                    factory.ResolveEvents(eventIds, CurrentCity.LocalTime);
                    //every time a event is created, resolved , etc, we have to update the sladue and highest severity, so lets do that now
                    factory.UpdateEventAggregateInfo(workOrderId);

                    //check to see if the work order evetns closed correctly.
                    if (!string.IsNullOrEmpty(factory.ErrorMessage))
                    {
                        //if we got this far, something failed. add error to model and display to user
                        ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Some events did not close. Try again later."));
                        ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, factory.ErrorMessage));

                        var tWorkOrder = factory.GetWorkOrderDetails(workOrderId,  CurrentCity.LocalTime);
                        return View("ResolveEvents", tWorkOrder);
                    }
                    //now send them back to the details.
                    return RedirectToAction("WorkOrderDetail", new { workOrderId});
            }

            //if we got this far, something failed. add error to model and display to user
            ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "An error occurred. Try again."));

            var techWorkOrder = factory.GetWorkOrderDetails(workOrderId,CurrentCity.LocalTime);
            return View("ResolveEvents", techWorkOrder);
        }
        #endregion
        #endregion
    }
}
