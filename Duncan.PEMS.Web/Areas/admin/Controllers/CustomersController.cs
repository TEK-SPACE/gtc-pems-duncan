/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/30/2014       Sergey Ostrerov                 DPTXPEMS- 45 Reopened - Can't create new customer
 * *******************************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Business.Events;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Utility.Audit;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Errors;
using Duncan.PEMS.Entities.Grids;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using NPOI.HSSF.UserModel;
using WebMatrix.WebData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DayOfWeek = Duncan.PEMS.Entities.Customers.DayOfWeek;
using System.Web;
using System.Web.Script.Serialization;
using Kendo.Mvc.Extensions;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    public class CityStructure
    {
        private string _name;
        public string Name { get; set; }
    }

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class CustomersController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Index - Main inquiry page

        public ActionResult Index()
        {
            
            ViewBag.CurrentCityID = CurrentCity.Id;

            return View();
        }

        [HttpGet]
        public ActionResult GetCustomers([DataSourceRequest] DataSourceRequest request)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Customers");

            //use default conneciton here, since it doesnt matter
            IQueryable<ListCustomerModel> items = (new CustomerFactory(ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName])).GetCustomersList().AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            var total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            // IEnumerable data = items.ApplyGrouping(request.Groups);
            IEnumerable data = items;
            var result = new DataSourceResult
            {
                Data = data,
                Total = total
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Exporting"

        //Customers
        public FileResult ExportToCsv([DataSourceRequest]DataSourceRequest request, string roleId)
        {
            //get role models
            var data = GetExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetCustomers", CurrentCity.Id);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Id"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Display Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Status"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Created On"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Created By"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Updated On"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Updated By"), true);
            foreach (ListCustomerModel item in data)
            {
                //add a item for each property here
                AddCsvValue(writer, item.Id.ToString());
                AddCsvValue(writer, item.DisplayName);
                AddCsvValue(writer, item.Status);
                AddCsvValue(writer, item.CreatedOn.ToString("d"));
                AddCsvValue(writer, item.CreatedBy);
                AddCsvValue(writer, item.UpdatedOn.ToString("d"));
                AddCsvValue(writer, item.UpdatedBy, true);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "CustomersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, string roleId)
        {
            //get role models
            var data = GetExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetCustomers", CurrentCity.Id);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue(GetLocalizedGridTitle(gridData, "Id"));
            headerRow.CreateCell(1).SetCellValue(GetLocalizedGridTitle(gridData, "Display Name"));
            headerRow.CreateCell(2).SetCellValue(GetLocalizedGridTitle(gridData, "Status"));
            headerRow.CreateCell(3).SetCellValue(GetLocalizedGridTitle(gridData, "Created On"));
            headerRow.CreateCell(4).SetCellValue(GetLocalizedGridTitle(gridData, "Created By"));
            headerRow.CreateCell(5).SetCellValue(GetLocalizedGridTitle(gridData, "Updated Date"));
            headerRow.CreateCell(6).SetCellValue(GetLocalizedGridTitle(gridData, "Updated By"));

            //Populate the sheet with values from the grid data
            foreach (ListCustomerModel item in data)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.Id.ToString());
                row.CreateCell(columnNumber++).SetCellValue(item.DisplayName);
                row.CreateCell(columnNumber++).SetCellValue(item.Status);
                row.CreateCell(columnNumber++).SetCellValue(item.CreatedOn.ToString("d"));
                row.CreateCell(columnNumber++).SetCellValue(item.CreatedBy);
                row.CreateCell(columnNumber++).SetCellValue(item.UpdatedOn.ToString("d"));
                row.CreateCell(columnNumber++).SetCellValue(item.UpdatedBy);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "CustomersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request, string roleId)
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
            var data = GetExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetCustomers", CurrentCity.Id);

            var dataTable = new PdfPTable(7) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Id"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Display Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Status"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Created On"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Created By"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Updated On"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Updated By"));

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            foreach (ListCustomerModel item in data)
            {
                dataTable.AddCell(item.Id.ToString());
                dataTable.AddCell(item.DisplayName);
                dataTable.AddCell(item.Status);
                dataTable.AddCell(item.CreatedOn.ToString("d"));
                dataTable.AddCell(item.CreatedBy);
                dataTable.AddCell(item.UpdatedOn.ToString("d"));
                dataTable.AddCell(item.UpdatedBy);
        }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "CustomersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable GetExportData(DataSourceRequest request)
        {
            //get the models
            //no paging, we are bringing back all the items
            //sort, filter, group them
            //dont care about conneciton string, since this pulls from rbac
            IQueryable<ListCustomerModel> items = (new CustomerFactory(ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName])).GetCustomersList().AsQueryable();

            items = items.ApplyFiltering(request.Filters);
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items;
            return data;
        }

        //Meters
        public FileResult ExportEventCodesToCsv( int customerId)
        {
            var items = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodesViewModel(customerId);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Event Code"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Alarm Severity"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Description"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "SLA Duration"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Event Type"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Asset Type"), true);
            foreach (var item in items.Codes)
            {
                //add a item for each property here
                AddCsvValue(writer, item.Id.ToString());
                AddCsvValue(writer, item.AlarmTier);
                AddCsvValue(writer, item.DescAbbrev);
                AddCsvValue(writer, item.SLAMinutes.ToString());
                AddCsvValue(writer, item.Type);
                AddCsvValue(writer, item.AssetType, true);
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values",
                        "EventCodesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportEventCodesToExcel([DataSourceRequest]DataSourceRequest request, int customerId)
        {
            var items = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodesViewModel(customerId);
            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();
            //add the filtered values to the top of the excel file
            // Add the filtered values to the top of the excel file
            var rowNumber = 0;

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);
            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Event Code"));
            headerRow.CreateCell(1).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Alarm Severity"));
            headerRow.CreateCell(2).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Description"));
            headerRow.CreateCell(3).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "SLA Duration"));
            headerRow.CreateCell(4).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Event Type"));
            headerRow.CreateCell(5).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Asset Type"));
            //Populate the sheet with values from the grid data
            foreach (var item in items.Codes)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);
                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.Id.ToString());
                row.CreateCell(columnNumber++).SetCellValue(item.AlarmTier);
                row.CreateCell(columnNumber++).SetCellValue(item.DescAbbrev);
                row.CreateCell(columnNumber++).SetCellValue(item.SLAMinutes.ToString());
                row.CreateCell(columnNumber++).SetCellValue(item.Type);
                row.CreateCell(columnNumber++).SetCellValue(item.AssetType);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user
            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "EventCodesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportEventCodesToPdf( int customerId)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            //step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            //step 3: we open the document
            document.Open();
            //step 4: we add content to the document

            ////first we have to add the filters they used
            //var customFilters = new List<FilterDescriptor> { new FilterDescriptor { Member = "Configuration Id", Value = routeId } };
            //document = AddFiltersToPdf(document, customFilters);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));
            var items = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodesViewModel(customerId);

            var dataTable = new PdfPTable(1) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Event Code"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Alarm Severity"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Description"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "SLA Duration"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Event Type"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Asset Type"));
            dataTable.CompleteRow();

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;
            foreach (var item in items.Codes)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.Id.ToString());
                dataTable.AddCell(item.AlarmTier);
                dataTable.AddCell(item.DescAbbrev);
                dataTable.AddCell(item.SLAMinutes.ToString());
                dataTable.AddCell(item.Type);
                dataTable.AddCell(item.AssetType);
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "EventCodesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        #endregion

        #region Add City functions
        //Returns the view for the Add City functionality
        public ActionResult Add()
        {
            var customerModel = new CustomerModel
                {
                    ConnectionStrings = SettingsFactory.GetCustomerConnectionStringNames(),
                    ConnectionStringName = ConfigurationManager.AppSettings[Constants.Security.DefaultPemsConnectionStringName],
                    ReportingConnectionStrings = SettingsFactory.GetReportingConnectionStringNames(),
                    ReportingConnectionStringName = ConfigurationManager.AppSettings[Constants.Security.DefaultReportingConnectionStringName]
                };
            return View(customerModel);
        }

        //Responds to the click of the Save button when creating a city
        [HttpPost]
        public ActionResult Add(string submitButton, CustomerModel model)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }
            
            try
            {
                // Refresh the connection names lists in the model.
                model.ConnectionStrings = SettingsFactory.GetCustomerConnectionStringNames();
                model.ReportingConnectionStrings = SettingsFactory.GetReportingConnectionStringNames();

                if (ModelState.IsValid)
                {
                    var customerFactory = new CustomerFactory(model.ConnectionStringName);

                    // Validate that this model.Id has not yet been used.
                    if (customerFactory.CustomerIdExists(model.Id))
                    {
                        // Cannot use this id.
                        ModelState.AddModelError("Id", "Customer Id already in use.");
                    }

                    if (customerFactory.CustomerNameExists(model.DisplayName))
                    {
                        // Cannot use this name.
                        ModelState.AddModelError("DisplayName", "Customer Name already in use.");
                    }

                    if (customerFactory.CustomerInternalNameExists(model.InternalName))
                    {
                        // Cannot use this internal name.
                        ModelState.AddModelError("InternalName", "Customer Internal Name already in use.");
                    }

                    if ( !ModelState.IsValid )
                    {
                        return View(model);
                    }

                    // At this point, can create new customer.
                    // Get server-relative paths for create customer process.

                    string templateFolder = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["rbac.menu.template.dir"]);
                    string workingFolder = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["rbac.menu.template.upload"]);
                    //int customerId = customerFactory.CreateNewCustomer(model, templateFolder, workingFolder);
                    int customerId = customerFactory.CreateNewCustomerOptimzed(model, templateFolder, workingFolder);
                    
                    var JavaRequest = new city.Controllers.AssetsController();
                    JavaRequest.JavaGetRequestToURL();

                    if (customerId == 0)
                    {
                        var ei = new ErrorItem()
                        {
                            ErrorCode = "1234",
                            ErrorMessage = "General failure creating new customer."
                        };

                        ViewData["__errorItem"] = ei;
                        return View(model);
                    }

                    // Success creating new customer. Redirect to Edit
                    return RedirectToAction("EditCustomer", new { customerId = model.Id });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ViewData["__errorItem"] = new ErrorItem( "1235", ex );
                return View(model);
            }
        }

        #endregion

        #region Edit Customer Identification

        public ActionResult ViewCustomer(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerIdentificationModel model = (new CustomerFactory(connStringName)).GetCustomerIdentificationModel(customerId);
            return View(model);
        }

        public ActionResult EditCustomer(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerIdentificationModel model = (new CustomerFactory(connStringName)).GetCustomerIdentificationModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditCustomer(string submitButton, CustomerIdentificationModel model)
        {
            // Was action RETURN?

            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            var custFactory = new CustomerFactory(connStringName);
            var auditFactory = new AuditFactory();
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new {rtn = "true"});
            }
           
            if (submitButton.Equals("ACTIVATE"))
            {
                custFactory.Activate(model.CustomerId);
                auditFactory.ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
                return RedirectToAction("EditCustomer", new {customerId = model.CustomerId});
            }
           
            if (submitButton.Equals("INACTIVATE"))
            {
                custFactory.Inactivate(model.CustomerId);
                auditFactory.ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
                return RedirectToAction("EditCustomer", new {customerId = model.CustomerId});
            }

            // Save values
            if (!ModelState.IsValid)
            {
                model = custFactory.GetCustomerIdentificationModel(model.CustomerId, model);
                return View(model);
            }
            
            custFactory.SetCustomerIdentificationModel(model.CustomerId, model);
            auditFactory.ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
            model = custFactory.GetCustomerIdentificationModel(model.CustomerId, model);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return View(model);
            }
            // Saved but move to next page.
            return RedirectToAction("EditAssets", new {customerId = model.CustomerId});
        }

        #endregion

        #region Edit Customer Maintenance Schedule
          public ActionResult EditMaintenanceSchedule(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerMaintenanceScheduleModel model = (new CustomerFactory(connStringName)).GetCustomerMaintenanceScheduleModel(customerId);
            return View(model);
        }

        [HttpPost]
          public ActionResult EditMaintenanceSchedule(string submitButton, CustomerMaintenanceScheduleModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }
            // Rebuild CustomerAssetsModel from form fields
            model = RebuildMaintenanceScheduleModel(model, formValues);

            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
             (new CustomerFactory(connStringName)).SetCustomerMaintenanceScheduleModel(model.CustomerId, model);
            (new AuditFactory()).ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditMaintenanceSchedule", new { customerId = model.CustomerId });
            }

            // Saved but move to next page.
            return RedirectToAction("ActivateCustomer", new { customerId = model.CustomerId });
           // return RedirectToAction("PAMConfiguration", new { customerId = model.CustomerId });
        }

        private CustomerMaintenanceScheduleModel RebuildMaintenanceScheduleModel(CustomerMaintenanceScheduleModel model, FormCollection formValues)
        {
            // Prep the Model.Coins class 
            if (model.DaysOfWeek == null)
                model.DaysOfWeek = new List<DayOfWeek>();

            //set all the nohours to false
            foreach (var dw in model.DaysOfWeek)
                dw.NoHours = false;

            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.
            foreach (var formValueKey in formValues.Keys)
            {
                string[] tokens = formValueKey.ToString().Split(model.Separator);
                var formValue = formValues[formValueKey.ToString()];

                if (tokens.Length < 2)
                    continue;
                // Get model id
                int dayOfWeekId = int.Parse(tokens[1]);
                // Get/create a model for this form field
                DayOfWeek dayModel = model.DaysOfWeek.FirstOrDefault(m => m.DayOfWeekId == dayOfWeekId);
                if (dayModel == null)
                {
                    dayModel = new DayOfWeek() { DayOfWeekId = dayOfWeekId };
                    model.DaysOfWeek.Add(dayModel);
                }
                //NO HOURS
                if (tokens[0].Equals(CustomerMaintenanceScheduleModel.NameNoHoursPrefix))
                {
                    //this is a checkbox, so if it exist, then it is set.
                       dayModel.NoHours = true;
                }
                    //START TIME
                else if (tokens[0].Equals(CustomerMaintenanceScheduleModel.NameStartTimePrefix))
                {
                  //the time will be a datetime, we need ot get a datetime from it
                    var startTime = DateTime.Parse(formValue);
                    dayModel.StartMinute = (startTime.Hour*60) + startTime.Minute;
                }
                else if (tokens[0].Equals(CustomerMaintenanceScheduleModel.NameEndTimePrefix))
                {
                    //the time will be a datetime, we need ot get a datetime from it
                    var endTime = DateTime.Parse(formValue);
                    dayModel.EndMinute = (endTime.Hour * 60) + endTime.Minute;
                }
            }
            return model;
        }
        #endregion

        #region Edit Customer Assets

        public ActionResult ViewAssets(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerAssetsModel model = (new CustomerFactory(connStringName)).GetCustomerAssetsModel(customerId);
            return View(model);
        }

        public ActionResult EditAssets(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerAssetsModel model = (new CustomerFactory(connStringName)).GetCustomerAssetsModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAssets(string submitButton, CustomerAssetsModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }
            // Rebuild CustomerAssetsModel from form fields
            model = RebuildCustomerAssetModel(model.CustomerId, formValues);

            // Force validation on model.
            var errors = model.Validate(new ValidationContext(model, null, null));
            foreach (var error in errors)
            {
                foreach (var memberName in error.MemberNames)
                    ModelState.AddModelError( memberName, error.ErrorMessage );
            }


            // Save values
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            (new CustomerFactory(connStringName)).SetCustomerAssetsModel(model.CustomerId, model);
            (new AuditFactory()).ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditAssets", new { customerId = model.CustomerId });
            }
            
            // Saved but move to next page.
            return RedirectToAction("EditPayments", new { customerId = model.CustomerId });
        }

        private CustomerAssetsModel RebuildCustomerAssetModel(int customerId, FormCollection formValues)
        {
            // Get an original copy of the CustomerAssetsModel.  This will ensure that any items that are
            // not in the formValues collection are still accounted for.
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerAssetsModel model = (new CustomerFactory(connStringName)).GetCustomerAssetsModel(customerId);

            // Set all "Active" flags to false.  This is the default state if the form checkbox was not checked.
            // (No form field comes back)
            model.DisableAssets();

            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.
            foreach (var formValueKey in formValues.Keys)
            {
                string[] tokens = formValueKey.ToString().Split(model.Separator);
                var formValue = formValues[formValueKey.ToString()];

                if ( tokens.Length == 5 )
                {
                    int groupId = int.Parse( tokens[1] );
                    int parentId = int.Parse( tokens[3] );
                    int childId = int.Parse( tokens[4] );

                    // Get/create the CustomerAssetGroup
                    CustomerAssetGroupModel customerAssetGroupModel = model.GetGroup( groupId );

                    // Determine which type of asset model we are dealing with: CustomerAssetTypeModel or CustomerAssetModel
                    if ( tokens[2].Equals( CustomerAssetTypeModel.UiCode ) )
                    {
                        CustomerAssetTypeModel customerAssetTypeModel = customerAssetGroupModel.GetAssetType( childId );

                        // Now add the particular SLA value
                        // Add appropriate property value to customerAssetModel
                        switch (tokens[0])
                        {
                            case CustomerAssetsModel.NameChkBoxPrefix:
                                customerAssetTypeModel.Active = true;
                                break;
                            case CustomerAssetsModel.NamePmsPrefix:
                                customerAssetTypeModel.PreventativeMaintenanceSlaDays = formValue.Equals("") ? -1 : int.Parse(formValue);
                                break;
                            case CustomerAssetsModel.NameSlaHoursPrefix:
                                customerAssetTypeModel.MaintenanceSlaHours = formValue.Equals("") ? -1 : int.Parse(formValue);
                                break;
                            case CustomerAssetsModel.NameSlaDaysPrefix:
                                customerAssetTypeModel.MaintenanceSlaDays = formValue.Equals("") ? -1 : int.Parse(formValue);
                                break;
                        }
                    }
                    else if ( tokens[2].Equals( CustomerAssetModel.UiCode ) )
                    {
                        CustomerAssetModel customerAssetModel = customerAssetGroupModel.GetAssetType(parentId).GetAsset(childId);

                        // Now add the particular SLA value
                        // Add appropriate property value to customerAssetModel
                        switch (tokens[0])
                        {
                            case CustomerAssetsModel.NameChkBoxPrefix:
                                customerAssetModel.Active = true;
                                break;
                            case CustomerAssetsModel.NamePmsPrefix:
                                customerAssetModel.PreventativeMaintenanceSlaDays = formValue.Equals("") ? -1 : int.Parse(formValue);
                                break;
                            case CustomerAssetsModel.NameSlaHoursPrefix:
                                customerAssetModel.MaintenanceSlaHours = formValue.Equals("") ? -1 : int.Parse(formValue);
                                break;
                            case CustomerAssetsModel.NameSlaDaysPrefix:
                                customerAssetModel.MaintenanceSlaDays = formValue.Equals("") ? -1 : int.Parse(formValue);
                                break;
                        }
                    }
                }
            }
            return model;
        }

        #endregion

        #region Edit Customer Areas, Zones and Suburbs

        public ActionResult ViewAreas(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerAreasModel model = (new CustomerFactory(connStringName)).GetCustomerAreasModel(customerId);
            return View(model);
        }

        public ActionResult EditAreas(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerAreasModel model = (new CustomerFactory(connStringName)).GetCustomerAreasModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditAreas(string submitButton, CustomerAreasModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            // Rebuild CustomerAreasModel from form fields
            model = RebuildCustomerAreasModel(model, formValues);

            // Save values
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            (new CustomerFactory(connStringName)).SetCustomerAreasModel(model.CustomerId, model);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditAreas", new { customerId = model.CustomerId });
            }
            // Saved but move to next page.
            //return RedirectToAction("EditMaintenanceSchedule", new { customerId = model.CustomerId });
            return RedirectToAction("EditDemandZones", new { customerId = model.CustomerId });
           
        }

        private CustomerAreasModel RebuildCustomerAreasModel(CustomerAreasModel model, FormCollection formValues)
        {
            char[] tokenCharacter = new char[] { model.Separator };

            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.
            foreach (var formValueKey in formValues.Keys)
            {
                var formKey = formValueKey.ToString();
                var formValue = formValues[formKey];

                if ( formKey.Equals( "NewAreas" ) )
                {
                    model.NewAreas = formValue.Split( tokenCharacter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else if (formKey.Equals("NewZones"))
                {
                    model.NewZones = formValue.Split(tokenCharacter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else if (formKey.Equals("NewCustomGroup1s"))
                {
                    model.NewCustomGroup1s = formValue.Split(tokenCharacter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else if (formKey.Equals("NewCustomGroup2s"))
                {
                    model.NewCustomGroup2s = formValue.Split(tokenCharacter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else if (formKey.Equals("NewCustomGroup3s"))
                {
                    model.NewCustomGroup3s = formValue.Split(tokenCharacter, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }
            return model;
        }



        #endregion

        #region Edit Demand Zones


        /// <summary>
        /// Description:EditDemandZones will call 'GetCustomerDemandZonesModel' from factory to fetch demand zone 
        /// details for the selected customer into model and paas in on to the view to display in the grid.
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public ActionResult EditDemandZones(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            var model = (new CustomerFactory(connStringName)).GetCustomerDemandZonesModel(customerId);
            return View(model);
        }

        /// <summary>
        /// Description: This method will get called when any of the 3 submit button of 'EditDemandZone' tabpage is clicked.According to the submit button 
        /// clicked, and passed to it as parameter, necessary action will be taken.
        /// Modified:Prita()
        /// </summary>
        /// <param name="submitButton"></param>
        /// <param name="model"></param>
        /// <param name="formValues"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDemandZones(string submitButton, CustomerDemandZonesModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            // Save values
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            model = (new CustomerFactory(connStringName)).GetCustomerDemandZonesModel(model.CustomerId);

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.

                //changed by prita to save the isdisplay changes if any
                //return RedirectToAction("SaveChanges", new { customerId = model.CustomerId });

                //(new CustomerFactory(connStringName)).SetCustomerDemandZonesModel(model.CustomerId, model); 
                return RedirectToAction("EditDemandZones", new { customerId = model.CustomerId });
            }
            // Saved but move to next page.
            return RedirectToAction("EditMaintenanceSchedule", new { customerId = model.CustomerId });

        }

        /// <summary>
        /// RemoveDemandZoneCustomer is called when the cross button in every grid row is clicked.This will pass the control to factory
        /// method 'DeleteDemandZoneCustomers' with demand zoneID of the selected row and customer ID to remove it from database.
        /// Prior to that it will check if this demand zoneID is being used by any meters by 'CheckIfExist' method in factory.If not then only it can be deleted else an alert message will be passed to view.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="DemandZoneId"></param>
        /// <returns></returns>
        public ActionResult RemoveDemandZoneCustomer(int customerId, int DemandZoneId)
        {
            var AssetExists = (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckIfExist(customerId, DemandZoneId);
            if (AssetExists == true)
            {
                var html = "Asset with selected demand zone exists for selected customer,Hence cannot be deleted";
                return Content(html, "text/html");
            }
            else
            {
                try
                {
                    (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).DeleteDemandZoneCustomers(customerId, DemandZoneId);
                }
                catch
                { }
                (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCustomerDemandZonesModel(customerId);
                return RedirectToAction("EditDemandZones", "Customers", new { customerId = customerId });
            }
        }
        /// <summary>
        /// Description:This method is called when a row is clicked in editDemandZone page.When clicked it will display a different page called 'EditDemand'.
        /// This page contain the detail of selected demand zone and option to enable and disable the same.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="DemandZoneId"></param>
        /// <returns></returns>
        public ActionResult EditDemand(int customerId, int DemandZoneId)
        {

            var model = (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDemandZoneEditModel(customerId, DemandZoneId);

            return View(model);
        }


        /// <summary>
        /// Description:This method will be called on click on submit buttons in 'EditDemand' page.On save it will ask the factory method to 
        /// save the changes done in the page.and return to 'EditDemandZones' page
        /// Modified:Prita()
        /// </summary>
        /// <param name="submitButton"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditDemand(string submitButton, CustomerDemandZone model)
        {
            var p = model.IsDisplay;
            if (submitButton.Equals("SAVE"))
            {

                (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).SetCustomerDemandZonesModel(model);

                var JavaRequest = new city.Controllers.AssetsController();
                JavaRequest.JavaGetRequestToURL();
                return RedirectToAction("EditDemandZones", "Customers", new { customerId = model.CustomerId });
            }

            if (submitButton.Equals("BACK"))
            {
                return RedirectToAction("EditDemandZones", new { customerId = model.CustomerId });
            }
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }
            return RedirectToAction("EditMaintenanceSchedule", "Customers", new { customerId = model.CustomerId });

        }

        public ActionResult GetDemandZones()
        {
            var DemandZonesList = (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).DemandZonesList();
            return Json(DemandZonesList, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddNewDemandZone(int CustID, int DemandZoneID)
        {
            try
            {
                //var VendorExists = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckIfExist(vendorID, VendorName, CurrentCity.Id);
                var VendorExists = (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckIfDemandZoneExist(CustID, DemandZoneID);
                if (VendorExists == true)
                {
                    var html = "Selected demand zone already exist for this customer.";
                    return Json(html, JsonRequestBehavior.AllowGet);
                    //return json(html, "text/html");
                }
                else
                {
                    //(new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveVendorDetails(vendorID, VendorName, CurrentCity.Id, DuncanPropGriditems, CustPropGriditems);
                    (new CustomerFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveDemandZone(CustID, DemandZoneID);
                    var html1 = "Record saved successfully";
                    //return Content(html1, "text/html");
                    return Json(html1, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception ex)
            {
                var html = "Error occured while saving data.Please try saving the record once again.";
                //return Content(html, "text/html");
                return Json(html, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Edit Pay By Cell

        /// <summary>
        /// Description:Redirects to EditCustPayByCell tab with selected customerId
        /// Modified:Prita()
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public ActionResult EditCustPayByCell(int customerId)
        {
            CustomerPayByCellModel objGISModel = new CustomerPayByCellModel();
            try
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
                objGISModel = (new CustomerFactory(connStringName)).GetCustomerPayByCellModel(customerId);
            }
            catch (Exception ex)
            {

                _logger.ErrorException("ERROR IN EditCustPayByCell method (Customercontroller)", ex);
            }
            return View(objGISModel);
        }


        /// <summary>
        /// Description:Depending on the submit button clicked in 'EditCustPayByCell' Tabpage  necessary action will be taken here
        /// Modified:Prita()
        /// </summary>
        /// <param name="submitButton"></param>
        /// <param name="model"></param>
        /// <param name="formValues"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCustPayByCell(string submitButton, CustomerPayByCellModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditCustPayByCell", new { customerId = model.CustomerId });
            }
            // Saved but move to next page.
            return RedirectToAction("EditRules", new { customerId = model.CustomerId });

        }


        /// <summary>
        /// Description:Depending on the 'vendor ID' selected in first grid of 'EditCustPayByCell' page 2nd grid will be populated here.
        /// Selected VendorIDs are maintained in 'selectedvendor'
        /// Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CustID"></param>
        /// <param name="selectedvendor"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateRipnetGrid([DataSourceRequest] DataSourceRequest request, int CustID, string selectedvendor)
        {

            IEnumerable<RipnetProp> CustomerProperties = Enumerable.Empty<RipnetProp>();
            int total = 0;
            var CustPropertiesList_raw = (new CustomerFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRipnetProperties(CustID, selectedvendor);

            CustomerProperties = from r in CustPropertiesList_raw
                                 select new RipnetProp
                                 {
                                     KeyText = r.KeyText,
                                     ValueText = r.ValueText,

                                 };

            return new LargeJsonResult() { Data = CustomerProperties.ToDataSourceResult(request), MaxJsonLength = int.MaxValue };

        }

        /// <summary>
        /// Save vendorcustomer mapping into vendorcustomerpaybycellmap table
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CustID"></param>
        /// <param name="selectedvendor"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveVendorCustPayByCellMapping([DataSourceRequest] DataSourceRequest request, int CustID, string selectedvendor)
        {
            try
            {
                var SavedSuccessfully = (new CustomerFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveVendorCustomerMapping(CustID, selectedvendor);
                if (SavedSuccessfully == true)
                {
                    var html1 = "Record saved successfully";
                    return Json(html1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var html1 = "Error occured while saving data.Please try saving the record once again.";
                    return Json(html1, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var html = "Error occured while saving data.Please try saving the record once again.";
                return Json(html, JsonRequestBehavior.AllowGet);
            }


        }



        #endregion       

        #region Edit Customer Payments

        public ActionResult ViewPayments(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerPaymentsModel model = (new CustomerFactory(connStringName)).GetCustomerPaymentsModel(customerId);
            return View(model);
        }

        public ActionResult EditPayments(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerPaymentsModel model = (new CustomerFactory(connStringName)).GetCustomerPaymentsModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditPayments(string submitButton, CustomerPaymentsModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            // Rebuild CustomerAssetsModel from form fields
            model = RebuildCustomerPaymentsModel(model, formValues);

            // Force validation on model.
            var errors = model.Validate(new ValidationContext(model, null, null));
            foreach (var error in errors)
            {
                foreach (var memberName in error.MemberNames)
                    ModelState.AddModelError(memberName, error.ErrorMessage);
            }

            // Save values
            if (!ModelState.IsValid)
                return View(model);

            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            (new CustomerFactory(connStringName)).SetCustomerPaymentsModel(model.CustomerId, model);
            (new AuditFactory()).ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
            
            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditPayments", new { customerId = model.CustomerId });
            }
            
            // Saved but move to next page.
            //return RedirectToAction("EditRules", new { customerId = model.CustomerId });

            return RedirectToAction("EditCustPayByCell", new { customerId = model.CustomerId });
        }


        private CustomerPaymentsModel RebuildCustomerPaymentsModel(CustomerPaymentsModel model, FormCollection formValues)
        {
            // Prep the Model.Coins class 
            if ( model.Coins == null )
            {
                model.Coins = new CustomerPaymentsCoinsModel();
            }


            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.
            foreach (var formValueKey in formValues.Keys)
            {
                string[] tokens = formValueKey.ToString().Split(model.Separator);
                var formValue = formValues[formValueKey.ToString()];

                if ( tokens.Length < 2 )
                    continue;

                if ( tokens[0].Equals( CustomerPaymentsCoinModel.CoinPrefix ) )
                {
                    // It is a coin selection.
                    if ( tokens.Length == 3 )
                    {
                        // Get coin id
                        int coinId = int.Parse( tokens[2] );

                        // Get/create a coin for this form field
                        CustomerPaymentsCoinModel coinModel = model.Coins.Coins.FirstOrDefault( m => m.Id == coinId );
                        if ( coinModel == null )
                        {
                            coinModel = new CustomerPaymentsCoinModel()
                                {
                                    Id = coinId
                                };
                            model.Coins.Coins.Add(coinModel);
                        }
                        
                        // Update the associated property in the coin model
                        // from the form field.
                        if ( tokens[1].Equals( Duncan.PEMS.Entities.Customers.CustomerPaymentsCoinModel.CoinCheck ) )
                        {
                            // This is a check box.
                            coinModel.Enabled = formValue.StartsWith( "true", StringComparison.CurrentCultureIgnoreCase );
                        }
                        else if (tokens[1].Equals(Duncan.PEMS.Entities.Customers.CustomerPaymentsCoinModel.CoinText))
                        {
                            // This is a text box.
                            coinModel.Name = formValue;
                        }
                    }
                }
                else if (tokens[0].Equals(CustomerPaymentsCreditDebitModel.CardLabelsPrefix))
                {
                    // It is a credit card selection.
                    int cardId = int.Parse( tokens[1] );

                    // Add the credit card entry.
                    model.CreditDebit.Cards.Add(new SelectListItem()
                        {
                            Selected = formValue.StartsWith("true", StringComparison.CurrentCultureIgnoreCase),
                            Value = tokens[1]
                        });
                }
            }
            return model;
        }


        [HttpPost]
        public ActionResult SetCoinDenominationSource(int customerId, string coinCountryId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            var customerFactory = new CustomerFactory(connStringName);
            if ( customerFactory.ValidateCoinCountry( coinCountryId ) )
            {
                // Set the coin country.
                customerFactory.SetCustomerCoinCountry( customerId, coinCountryId );

                // Get list of coins
                var customerPaymentsModel = new CustomerPaymentsModel();
                customerFactory.GetCustomerPaymentsCoinsModel( customerId, customerPaymentsModel );

                // Return list of coins.
                return Json(new { Success = "True", customerPaymentsModel.Coins.Coins });
            }
            return Json(new { Success = "False", Description = "Select a valid coin country." });
        }

        #endregion

        #region Edit Customer Rules

        public ActionResult ViewRules(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerRulesModel model = (new CustomerFactory(connStringName)).GetCustomerRulesModel(customerId, (DateTime)Session[Constants.ViewData.CurrentLocalTime]);
            return View(model);
        }

        public ActionResult EditRules(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerRulesModel model = (new CustomerFactory(connStringName)).GetCustomerRulesModel(customerId, (DateTime)Session[Constants.ViewData.CurrentLocalTime]);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditRules(string submitButton, CustomerRulesModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            // Rebuild CustomerRulesModel from form fields
            RebuildCustomerRulesModel(model, formValues);

            // Force validation on model.
            var errors = model.Validate(new ValidationContext(model, null, null));
            foreach (var error in errors)
            {
                foreach (var memberName in error.MemberNames)
                    ModelState.AddModelError(memberName, error.ErrorMessage);
            }

            // Save values
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            if (!ModelState.IsValid)
            {
                model = (new CustomerFactory(connStringName)).GetCustomerRulesModel(model.CustomerId, model, (DateTime)Session[Constants.ViewData.CurrentLocalTime]);
                return View(model);
            }

            (new CustomerFactory(connStringName)).SetCustomerRulesModel(model.CustomerId, model, (DateTime)Session[Constants.ViewData.CurrentLocalTime]);
            (new AuditFactory()).ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            //clear out the session variable, since they could have changed the filters and fields for the reports this needs to be updated
            Session.Remove(Constants.Reporting.ReportingInitializedForSession + model.CustomerId);

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditRules", new { customerId = model.CustomerId });
            }
            
            // Saved but move to next page.
            return RedirectToAction("EditCustomLabels", new { customerId = model.CustomerId });
        }

        private void RebuildCustomerRulesModel(CustomerRulesModel model, FormCollection formValues)
        {
            model.OperationalTimes = new CustomerRulesOperationalTimeModel();
            
            // First build the asset group model then process the ALarm SLAs.
            foreach (var formValueKey in formValues.Keys)
            {
                string[] tokens = formValueKey.ToString().Split(model.Separator);
                var formValue = formValues[formValueKey.ToString()];

                if ( tokens.Length == 1 )
                {

                    if (tokens[0].Equals("ChkZeroOutMeter"))
                        model.ZeroOutMeter = true;
                    else if (tokens[0].Equals("ChkBlackListCC"))
                        model.BlacklistCC = true;
                    else if (tokens[0].Equals("ChkStreetline"))
                        model.Streetline = true;
                    else if (tokens[0].Equals("ChkCloseWorkOrderEvents"))
                        model.CloseWorkOrderEvents = true;
                    else if (tokens[0].Equals("ChkOperationTime"))
                        model.OperationalTimes.HasOperationTime = true;
                    else if (tokens[0].Equals("ChkPeakTime"))
                        model.OperationalTimes.HasPeakTime = true;
                    else if (tokens[0].Equals("ChkEveningTime"))
                        model.OperationalTimes.HasEveningTime = true;
                    else if (tokens[0].Equals("OperationTimeStart"))
                        model.OperationalTimes.OperationStart = DateTime.Parse( formValue.ToString() );
                    else if (tokens[0].Equals("OperationTimeEnd"))
                        model.OperationalTimes.OperationEnd = DateTime.Parse( formValue.ToString() );
                    else if (tokens[0].Equals("PeakTimeStart"))
                        model.OperationalTimes.PeakStart = DateTime.Parse( formValue.ToString() );
                    else if (tokens[0].Equals("PeakTimeEnd"))
                        model.OperationalTimes.PeakEnd = DateTime.Parse( formValue.ToString() );
                    else if (tokens[0].Equals("EveningTimeStart"))
                        model.OperationalTimes.EveningStart = DateTime.Parse( formValue.ToString() );
                    else if (tokens[0].Equals("EveningTimeEnd"))
                        model.OperationalTimes.EveningEnd = DateTime.Parse( formValue.ToString() );
                    else if (tokens[0].Equals("ChkDisplayFullMenu"))
                        model.DisplayFullMenu = true;
                    else if (tokens[0].Equals("ChkHFieldDemandArea"))
                        model.FieldDemandArea = true;
                    else if (tokens[0].Equals("ChkHFieldZone"))
                        model.FieldZone = true;
                    else if (tokens[0].Equals("ChkHFieldDiscountScheme"))
                        model.FieldDiscountScheme = true;
                    else if (tokens[0].Equals("ChkHFieldCG1"))
                        model.FieldCG1 = true;
                    else if (tokens[0].Equals("ChkHFieldCG2"))
                        model.FieldCG2 = true;
                    else if (tokens[0].Equals("ChkHFieldCG3"))
                        model.FieldCG3 = true;
                }
                else if ( tokens.Length == 2 )
                {

                    if ( tokens[0].Equals( model.SchemaPrefix ) )
                    {
                        // An existing schema
                        model.DiscountSchemas.Add( new CustomerRulesDiscountSchemaModel()
                            {
                                Enabled = formValue.StartsWith( "true", StringComparison.CurrentCultureIgnoreCase ),
                                Id = int.Parse( tokens[1] )
                            } );
                    }
                }

            }
        }

        #endregion

        #region Edit Customer Custom Labels

        public ActionResult ViewCustomLabels(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerLabelsModel model = (new CustomerFactory(connStringName)).GetCustomerLabelsModel(customerId);
            return View(model);
        }

        public ActionResult EditCustomLabels(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerLabelsModel model = (new CustomerFactory(connStringName)).GetCustomerLabelsModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditCustomLabels(string submitButton, CustomerLabelsModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
                return RedirectToAction("Index", new { rtn = "true" });
            
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            // Save values
            if (!ModelState.IsValid)
            {
                model = (new CustomerFactory(connStringName)).GetCustomerLabelsModel(model.CustomerId, model);
                return View(model);
            }
            
            // Rebuild model from FormCollection formValues
            RebuildLabelLists(model, formValues);

            // Save results.
            (new CustomerFactory(connStringName)).SetCustomerLabelsModel(model.CustomerId, model);
            (new AuditFactory()).ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditCustomLabels", new { customerId = model.CustomerId });
            }
            
            // Saved but move to next page.
            return RedirectToAction("EditCustomGrids", new { customerId = model.CustomerId });
        }

        private void RebuildLabelLists(CustomerLabelsModel model, FormCollection formValues)
        {
            string[] tokens;
            foreach (var formValueKey in formValues.Keys)
            {
                tokens = formValueKey.ToString().Split( model.Separator );
                if ( tokens.Length != 3 )
                    continue;

                var formValue = formValues[formValueKey.ToString()];

                // Create a CustomerLabel
                CustomerLabel customerLabel = new CustomerLabel();
                customerLabel.LabelId = int.Parse( tokens[2] );

                // Is this a text or list box?
                if ( tokens[1].Equals( customerLabel.TextType ) )
                {
                    customerLabel.CustomLabel = formValue;
                }
                else
                {
                    customerLabel.CustomLabelList = new List<SelectListItem>();
                    customerLabel.CustomLabelList.Add(new SelectListItem()
                    {
                        Value = formValue
                    });
                }

                // Add this to the appropriate List<CustomerLabel> in model
                switch (tokens[0])
                {
                    case CustomerLabelsModel.FieldLabelsPrefix:
                        if ( !model.LabelGroups.ContainsKey( ResourceTypes.Label ) )
                        {
                            model.LabelGroups.Add(ResourceTypes.Label, new List<CustomerLabel>());
                        }
                        model.LabelGroups[ResourceTypes.Label].Add(customerLabel);
                        break;
                    case CustomerLabelsModel.GridLabelsPrefix:
                        if ( !model.LabelGroups.ContainsKey( ResourceTypes.GridColumn ) )
                        {
                            model.LabelGroups.Add(ResourceTypes.GridColumn, new List<CustomerLabel>());
                        }
                        model.LabelGroups[ResourceTypes.GridColumn].Add(customerLabel);
                        break;
                    default:
                        break;
                }

            }

        }
        #endregion

        #region Edit Customer Custom Grids

        //todo - GTC: Custom Grid Work - this page needs to be re-done according to the document

        public ActionResult ViewCustomGrids(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerGridsModel model = (new CustomerFactory(connStringName)).GetCustomerGridsModel(customerId);
            return View(model);
        }
        //todo - GTC: Custom Grid Work - this page needs to be re-done according to the document

        /// <summary>
        /// Description: This Method will returns the view with  customer details 
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014)  
        /// </summary>
        /// <param name="customerId">Selected CustomerId</param>
        /// <returns></returns>
        public ActionResult EditCustomGrids(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerGridsModel model = (new CustomerFactory(connStringName)).GetCustomerGridsModel(customerId);
            ViewData["CustomerId"] = customerId;
            ViewData["DisplayName"] = model.DisplayName;
            ViewData["Status"] = model.Status.Status;
            ViewData["StatusId"] = model.Status.StatusId;
            ViewData["StatusDate"] = model.Status.StatusDate;
            ViewData["ModifiedBy"] = model.Status.ModifiedBy;
            ViewData["ModifiedOn"] = model.Status.ModifiedOn;
            ViewData["CreatedBy"] = model.Status.CreatedBy;
            ViewData["CreatedOn"] = model.Status.CreatedOn;
            return View();
        }
        //todo - GTC: Custom Grid Work - this page needs to be re-done according to the document

        /// <summary>
        /// 
        /// </summary>
        /// <param name="submitButton"></param>
        /// <param name="model"></param>
        /// <param name="formValues"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCustomGrids(string submitButton, CustomerGridsModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            // Save values
            if (!ModelState.IsValid)
            {
                model = (new CustomerFactory(connStringName)).GetCustomerGridsModel(model.CustomerId);
                return View(model);
            }

            // Rebuild model from FormCollection formValues
            RebuildGridsLists(model, formValues);

            (new CustomerFactory(connStringName)).SetCustomerGridsModel(model.CustomerId, model);
            (new AuditFactory()).ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditCustomGrids", new { customerId = model.CustomerId });
            }

            // Saved but move to next page.
            return RedirectToAction("EditEventCodes", new { customerId = model.CustomerId });
        }
        //todo - GTC: Custom Grid Work - this page needs to be re-done according to the document

        private void RebuildGridsLists(CustomerGridsModel model, FormCollection formValues)
        {
            string[] tokens;
            string[] values;
            foreach (var formValueKey in formValues.Keys)
            {
                tokens = formValueKey.ToString().Split(model.Separator);
                if (tokens.Length != 2)
                    continue;

                // Make sure model has TemplateSets 
                if (model.TemplateSets == null)
                    model.TemplateSets = new List<GridTemplateSet>();

                // Get the template Id selected.
                var formValue = formValues[formValueKey.ToString()];
                values = formValue.Split(model.Separator);
                var gts = new GridTemplateSet()
                {
                    Controller = tokens[0],
                    Action = tokens[1],
                    Version = int.Parse(values[1]),
                };

                gts.Add(new GridTemplate()
                {
                    Controller = tokens[0],
                    Action = tokens[1],
                    Version = int.Parse(values[1]),
                    GridId = int.Parse(values[0]),
                    Selected = true
                });

                model.TemplateSets.Add(gts);
            }
        }

        /// <summary>
        /// Description: This Method will reorder postion , title and should be hidden or not
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <param name="BoolUpdate">Check Gridrow will update or not</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UpdateCustmerGrid([DataSourceRequest] DataSourceRequest request, GridController GridRowdata, int CustomerId, bool BoolUpdate)
        {
            if (GridRowdata != null && BoolUpdate == true)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).UpdateCustomGridDetails(GridRowdata);
            }
            return null;
        }

        /// <summary>
        /// Description: This Method will retrive controller list given to customer
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public ActionResult GetController(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            var Controller = (new CustomerFactory(connStringName)).GetController(customerId);
            return Json(Controller, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This Method will retrive action list given to customer and Controllername
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014) 
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="Controllername"></param>
        /// <returns></returns>
        public ActionResult GetActionnames(int CustomerID, string Controllername)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerID);
            var Actionname = (new CustomerFactory(connStringName)).GetActionnames(CustomerID, Controllername);
            return Json(Actionname, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Description: This Method will retrive grid postion, title, Ishidden
        /// ModifiedBy: Santhosh  (28/July/2014 - 04/Aug/2014)   
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <param name="Controllername"></param>
        /// <param name="Actionname"></param>
        /// <returns></returns>
        public ActionResult GetCustmerGrid(int CustomerID, string Controllername, string Actionname)
        {
            IEnumerable<GridController> SummaryResult = Enumerable.Empty<GridController>();
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerID);
            var Controllergrid = (new CustomerFactory(connStringName)).GetCustmerGrid(CustomerID, Controllername, Actionname);
            SummaryResult = from r in Controllergrid
                            select new GridController
                            {
                                CustomerGridsId = r.CustomerGridsId,
                                Controller = r.Controller,
                                Action = r.Action,
                                Title = r.Title,
                                Position = r.Position,
                                OriginalTitle = r.OriginalTitle,
                                OriginalPosition = r.OriginalPosition,
                                IsHidden = r.IsHidden

                            };
            var result = new DataSourceResult
            {
                Data = SummaryResult,

            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit Customer Event Codes

        public ActionResult ViewEventCodes(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerEventCodesModel model = (new CustomerFactory(connStringName)).GetCustomerEventCodesModel(customerId);
            return View(model);
        }

        public ActionResult EditEventCodes(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            var model = (new EventCodesFactory(connStringName)).GetEventCodesViewModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEventCodes(string submitButton, CustomerEventCodesModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
                return RedirectToAction("Index", new { rtn = "true" });

            // Save values
            if (!ModelState.IsValid)
            {
//                model = CustomerFactory.GetCustomerGridsModel(model.CustomerId);
                return View(model);
            }
            
            //// Rebuild model from FormCollection formValues
            //RebuildGridsLists(model, formValues);

            //CustomerFactory.SetCustomerGridsModel(model.CustomerId, model);
            //AuditFactory.ModifiedBy("CustomerProfile", model.CustomerId, UserFactory.GetCurrentUserId());

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return RedirectToAction("EditEventCodes", new { customerId = model.CustomerId });
            }
            
            // Saved but move to next page.
            return RedirectToAction("EditAreas", new { customerId = model.CustomerId });
        }


        #endregion

        #region Customer Activation

        public ActionResult ActivateCustomer(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerActivateModel model = (new CustomerFactory(connStringName)).CanActivate(customerId);
            if (model.Status.StatusId == CustomerStatus.Active)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ActivateCustomer(string submitButton, CustomerActivateModel model)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index");
            }

            // Activate customer and return to index page.
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(model.CustomerId);
            (new CustomerFactory(connStringName)).Activate(model.CustomerId);

            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();

            return RedirectToAction("Index");
        }

        #endregion

        #region City Authorization Groups

        /// <summary>
        /// Displays the Add Group Interface
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        public ActionResult AddCityGroup(int cityId)
        {
            ViewData.Add("cityId", cityId);
            return View();
        }

        /// <summary>
        /// Responds to the form to add a group to a city
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCityGroup(int cityId, FormCollection collection)
        {
//            azManGroupMgr.CreateStoreGroup(cityToEdit, collection["txtCityGroupName"], collection["txtCityGroupDescription"]);
            ViewData.Add("cityId", cityId);
            return RedirectToAction("Edit", new { cityId = cityId });
        }


        /// <summary>
        /// Add a group record for a city
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCityGroup(int cityId, string groupToEdit, FormCollection collection)
        {
            string newGroupName = collection["txtCityGroupName"];
            string newGroupDescription = collection["txtCityGroupDescription"];
//            azManGroupMgr.UpdateStoreGroup(cityToEdit, groupToEdit, newGroupName, newGroupDescription);
            ViewData["cityId"] = cityId;
            ViewData["groupToEdit"] = newGroupName;
            return RedirectToAction("Edit", new { cityId = cityId });
        }

        public ActionResult EditCityGroup(int cityId, string groupToEdit)
        {
            ViewData["cityId"] = cityId;
            ViewData["groupToEdit"] = groupToEdit;
            return View();
        }


        public ActionResult AddGroupMemberError(int cityId, string groupToEdit, string es)
        {
            ViewData["errorMessage"] = "Username Already Exists";
            ViewData["cityId"] = cityId;
            ViewData["groupToEdit"] = groupToEdit;
            return View("AddGroupMember");
        }

        public ActionResult AddGroupMember(string cityToEdit, string groupToEdit)
        {
            ViewData["cityToEdit"] = cityToEdit;
            ViewData["groupToEdit"] = groupToEdit;
            return View();
        }

        /// <summary>
        /// Responds to the action of adding a member to a group
        /// </summary>
        /// <param name="cityToEdit"></param>
        /// <param name="groupToEdit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddGroupMember(string cityToEdit, string groupToEdit, FormCollection collection)
        {
            ViewData["cityToEdit"] = cityToEdit;
            ViewData["groupToEdit"] = groupToEdit;
            string username = collection["txtGroupUserName"];

            //if (azManGroupMgr.GetUser(username) != null)
            //{
            //    azManGroupMgr.CreateStoreGroupMember(cityToEdit, groupToEdit, username);
            //    return RedirectToAction("EditCityGroup", new { cityToEdit = cityToEdit, groupToEdit = groupToEdit });
            //}
            //else
            //{
            //    return RedirectToAction("AddGroupMemberError", new { cityToEdit = cityToEdit, groupToEdit = groupToEdit, es = 1});
            //}
            
            return RedirectToAction("AddGroupMemberError", new { cityToEdit = cityToEdit, groupToEdit = groupToEdit, es = 1});
        }

        #endregion

        #region Show City functions

        //Returns the view for the Show City functionality
        public ActionResult Details(int customerId)
        {
            return View();
        }


        #endregion

        #region JSON Result used in PayByCell Methods
        public class LargeJsonResult : JsonResult
        {
            const string JsonRequest_GetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";
            public LargeJsonResult()
            {
                MaxJsonLength = 2147483644;
                RecursionLimit = 100;
            }

            public int MaxJsonLength { get; set; }
            public int RecursionLimit { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
                if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                    String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(JsonRequest_GetNotAllowed);
                }

                HttpResponseBase response = context.HttpContext.Response;

                if (!String.IsNullOrEmpty(ContentType))
                {
                    response.ContentType = ContentType;
                }
                else
                {
                    response.ContentType = "application/json";
                }
                if (ContentEncoding != null)
                {
                    response.ContentEncoding = ContentEncoding;
                }
                if (Data != null)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = MaxJsonLength, RecursionLimit = RecursionLimit };
                    response.Write(serializer.Serialize(Data));
                }
            }



        }
        #endregion

        #region PAM Configuration

        /// <summary>
        /// Description: This Method will returns the view for the PAMConfiguration with PAM active details
        /// ModifiedBy: Santhosh  (04/Aug/2014 - 07/Aug/2014)        
        /// </summary>
        /// <param name="customerId">Selected Customer ID</param>
        /// <returns></returns>
        public ActionResult PAMConfiguration(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            CustomerPAMConfigurationModel model = (new CustomerFactory(connStringName)).GetPAMConfigurationModel(customerId);
            return View(model);
        }


        /// <summary>
        /// Description:This Method will get details of Clusters  with details
        ///  ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)           
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult GetGracePeriod([DataSourceRequest] DataSourceRequest request, int CustomerId)
        {
            IEnumerable<PAMGracePeriodModel> SummaryResult = Enumerable.Empty<PAMGracePeriodModel>();
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var PAMConfigResult_raw = (new CustomerFactory(connStringName)).GetPAMGrace(CustomerId);
            SummaryResult = from r in PAMConfigResult_raw
                            select new PAMGracePeriodModel
                            {
                                Clusterid = r.Clusterid,
                                GracePeriod = r.GracePeriod

                            };
            DataSourceResult finalResult = SummaryResult.ToDataSourceResult(request);
            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        }

        /// <summary>
        /// Description: This Method will Set PAM Active /InActive with respected parameters
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014) 
        /// ModifiedDate: 28/Aug/2014
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="pamCustactive"></param>
        /// <param name="ChkExpTimeByPAM"></param>
        /// <param name="ChkResetImin"></param>
        public void SetPAMActive(int customerId, bool pamCustactive, bool ChkExpTimeByPAM, bool ChkResetImin)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            (new CustomerFactory(connStringName)).SetPAMActive(customerId, pamCustactive, ChkExpTimeByPAM, ChkResetImin);
        }

        /// <summary>
        /// Description: This Method will ADD Clusterid and graceperiod
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)           
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddGracePeriod([DataSourceRequest] DataSourceRequest request, PAMGracePeriodModel GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).AddGracePeriod(GridRowdata, CustomerId);

                var JavaRequest = new city.Controllers.AssetsController();
                JavaRequest.JavaGetRequestToURL();

            }
            return Json(ModelState.ToDataSourceResult());
        }

        /// <summary>
        /// Description: This Method will Delete Existing Clusterid and graceperiod
        ///  ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteGracePeriod([DataSourceRequest] DataSourceRequest request, PAMGracePeriodModel GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).DeleteGracePeriod(GridRowdata);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        /// <summary>
        /// Description: This Method will retrive all ClusterID list created for customer.
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)                   
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult ClusterIDList(int CustomerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var clusteridlist = (new CustomerFactory(connStringName)).ClusterIDList(CustomerId);
            return Json(clusteridlist, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Description: This Method will retrive all MeterID list given customer.
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult MeterIDList(int CustomerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var MeterIDList = (new CustomerFactory(connStringName)).MeterIDList(CustomerId);
            return Json(MeterIDList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This Method will retrive Meters are assigned to a cluster with a bay range 
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult GetPAMClusterMeter([DataSourceRequest] DataSourceRequest request, int CustomerId)
        {

            IEnumerable<PAMClusters> SummaryResult = Enumerable.Empty<PAMClusters>();
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var PAMConfigResult_raw = (new CustomerFactory(connStringName)).GetPAMClusterMeter(CustomerId);
            SummaryResult = from r in PAMConfigResult_raw
                            select new PAMClusters
                            {
                                Clusterid = r.Clusterid,
                                Description = r.Description,
                                Hostedbayend = r.Hostedbayend,
                                Hostedbaystart = r.Hostedbaystart,
                                MeterId = r.MeterId
                            };

            DataSourceResult finalResult = SummaryResult.ToDataSourceResult(request);
            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        }

        /// <summary>
        /// Description: This Method will ADD new //Update Meter assigned to a cluster with a bay range 
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddPAMClusterMeter([DataSourceRequest] DataSourceRequest request, PAMClusters GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).AddPAMClusterMeter(GridRowdata, CustomerId);

                var JavaRequest = new city.Controllers.AssetsController();
                JavaRequest.JavaGetRequestToURL();

            }

            return Json(ModelState.ToDataSourceResult());
        }

        /// <summary>
        /// Description: This Method will Unassign Meter from cluster bay range 
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletePAMClusterMeter([DataSourceRequest] DataSourceRequest request, PAMClusters GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).DeletePAMClusterMeter(GridRowdata);

            }
            return Json(ModelState.ToDataSourceResult());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public int GetGroupID(int MeterId, int CustomerId)
        {
            return (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetGroupID(MeterId,CustomerId);
        }

        #endregion
    }
}
