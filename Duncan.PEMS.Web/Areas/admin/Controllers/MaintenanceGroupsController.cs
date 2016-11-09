using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Business.MaintenanceGroups;
using Duncan.PEMS.Business.Utility.Audit;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Errors;
using Duncan.PEMS.Entities.MaintenanceGroups;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using NPOI.HSSF.UserModel;
using WebMatrix.WebData;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class MaintenanceGroupsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Index


        public ActionResult Index()
        {
            ViewBag.CurrentCityID = CurrentCity.Id;
            return View();
        }
        
        [HttpGet]
        public ActionResult GetMaintenanceGroups([DataSourceRequest] DataSourceRequest request)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "MaintenanceGroups");

            //use default conneciton here, since it doesnt matter
            IQueryable<ListMaintenanceGroupModel> items = (new MaintenanceGroupFactory()).GetMaintenanceGroupsList().AsQueryable();
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

        //Maintenance Groups
        public FileResult ExportMGToCsv([DataSourceRequest]DataSourceRequest request, string roleId)
        {
            //get role models
            var data = GetMGExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetCustomers", CurrentCity.Id);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Id"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Status"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Creation Date"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Created By"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Last Modified Date"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Last Modified By"), true);
            foreach (ListMaintenanceGroupModel item in data)
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

            return File(output, "text/comma-separated-values", "MaintenanceGroupsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportMGToExcel([DataSourceRequest]DataSourceRequest request, string roleId)
        {
            //get role models
            var data = GetMGExportData(request);
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
            headerRow.CreateCell(1).SetCellValue(GetLocalizedGridTitle(gridData, "Name"));
            headerRow.CreateCell(2).SetCellValue(GetLocalizedGridTitle(gridData, "Status"));
            headerRow.CreateCell(3).SetCellValue(GetLocalizedGridTitle(gridData, "Creation Date"));
            headerRow.CreateCell(4).SetCellValue(GetLocalizedGridTitle(gridData, "Created By"));
            headerRow.CreateCell(5).SetCellValue(GetLocalizedGridTitle(gridData, "Last Modified Date"));
            headerRow.CreateCell(6).SetCellValue(GetLocalizedGridTitle(gridData, "Last Modified By"));

            //Populate the sheet with values from the grid data
            foreach (ListMaintenanceGroupModel item in data)
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
                "MaintenanceGroupsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportMGToPdf([DataSourceRequest]DataSourceRequest request, string roleId)
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
            var data = GetMGExportData(request);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetCustomers", CurrentCity.Id);

            var dataTable = new PdfPTable(7) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Id"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Status"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Creation Date"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Created By"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Last Modified Date"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Last Modified By"));

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;

            foreach (ListMaintenanceGroupModel item in data)
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
            return File(output.ToArray(), "application/pdf", "MaintenanceGroupsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable GetMGExportData(DataSourceRequest request)
        {
            //get the models
            //no paging, we are bringing back all the items
            //sort, filter, group them
            //dont care about conneciton string, since this pulls from rbac
            IQueryable<ListMaintenanceGroupModel> items = (new MaintenanceGroupFactory()).GetMaintenanceGroupsList().AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items;
            return data;
        }
        
        #endregion


        #region Add Maintenance Group
        //Returns the view for the Add City functionality
        public ActionResult AddMaintenanceGroup()
        {
            var customerModel = new MaintenangeGroupCreateModel { ConnectionStrings = SettingsFactory.GetMaintenanceGroupConnectionStringNames() };
            return View(customerModel);
        }

        //Responds to the click of the Save button when creating a city
        [HttpPost]
        public ActionResult AddMaintenanceGroup(string submitButton, MaintenangeGroupCreateModel model)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var customerFactory = new CustomerFactory(model.ConnectionStringName);
                    // Validate that this model.Id has not yet been used.
                    if (customerFactory.CustomerIdExists(model.Id))
                        // Cannot use this id.
                        ModelState.AddModelError("Id", "Id already in use.");

                    if (customerFactory.CustomerNameExists(model.DisplayName))
                        // Cannot use this name.
                        ModelState.AddModelError("DisplayName", "Name already in use.");

                    if (!ModelState.IsValid)
                    {
                        model.ConnectionStrings = SettingsFactory.GetMaintenanceGroupConnectionStringNames();
                        return View(model);
                    }

                    // At this point, can create new customer.
                    // Get server-relative paths for create customer process.

                    string templateFolder = Server.MapPath(ConfigurationManager.AppSettings["rbac.menu.template.dir"]);
                    string workingFolder = Server.MapPath(ConfigurationManager.AppSettings["rbac.menu.template.upload"]);
                    //create the new maint group here
                    int customerId = (new MaintenanceGroupFactory()).CreateNewMaintananceGroup(model, templateFolder, workingFolder);
                    if (customerId == 0)
                    {
                        var ei = new ErrorItem()
                        {
                            ErrorCode = "1236",
                            ErrorMessage = "General failure creating new maintenance group."
                        };

                        ViewData["__errorItem"] = ei;
                        model.ConnectionStrings = SettingsFactory.GetMaintenanceGroupConnectionStringNames();
                        return View(model);
                    }

                    // Success creating new customer. Redirect to Edit
                    return RedirectToAction("EditMaintenanceGroup", new { customerId = model.Id });
                }
                model.ConnectionStrings = SettingsFactory.GetMaintenanceGroupConnectionStringNames();
                return View(model);
            }
            catch (Exception ex)
            {
                var ei = new ErrorItem()
                {
                    ErrorCode = "1237",
                    ErrorMessage = "General failure creating new maintenance group.  " + ex.Message
                };

                ViewData["__errorItem"] = ei;
                model.ConnectionStrings = SettingsFactory.GetMaintenanceGroupConnectionStringNames();
                return View(model);
            }
        }
        #endregion

        #region Edit MaintenanceGroup Identification

        public ActionResult EditMaintenanceGroup(int customerId)
        {
            MaintenanceGroupIdentificationModel model = (new MaintenanceGroupFactory()).GetIdentificationModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMaintenanceGroup(string submitButton, MaintenanceGroupIdentificationModel model)
        {
            // Was action RETURN?

            var mainFactory = new MaintenanceGroupFactory();
            var auditFactory = new AuditFactory();
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = "true" });
            }

            if (submitButton.Equals("ACTIVATE"))
            {
                mainFactory.Activate(model.CustomerId);
                auditFactory.ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
                return RedirectToAction("EditMaintenanceGroup", new { customerId = model.CustomerId });
            }

            if (submitButton.Equals("INACTIVATE"))
            {
                mainFactory.Inactivate(model.CustomerId);
                auditFactory.ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
                return RedirectToAction("EditMaintenanceGroup", new { customerId = model.CustomerId });
            }

            // Save values
            if (!ModelState.IsValid)
            {
                model = mainFactory.GetIdentificationModel(model.CustomerId, model);
                return View(model);
            }

            mainFactory.SetIdentificationModel(model.CustomerId, model);
            auditFactory.ModifiedBy("CustomerProfile", model.CustomerId, WebSecurity.CurrentUserId);
            model = mainFactory.GetIdentificationModel(model.CustomerId, model);
            if (submitButton.Equals("SAVE"))
            {
                // Saved and stay on same page.
                return View(model);
            }
            // Saved but move to next page.
            return RedirectToAction("EditMaintenanceGroupCustomers", new { customerId = model.CustomerId });
        }
        #endregion

        #region MaintenanceGroup Customers

        public ActionResult EditMaintenanceGroupCustomers(int customerId)
        {
            MaintenanceGroupCustomersModel model = (new MaintenanceGroupFactory()).GetMaintenanceGroupCustomersModel(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMaintenanceGroupCustomers(string submitButton, MaintenanceGroupCustomersModel model, FormCollection formValues)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
                return RedirectToAction("Index", new { rtn = "true" });

            // Rebuild CustomerAreasModel from form fields
            model = RebuildModel(model, formValues);

            // Save values
            if (!ModelState.IsValid)
                return View(model);

            (new MaintenanceGroupFactory()).SetMaintenanceGroupCustomersModel(model.CustomerId, model);

                // Saved and stay on same page.
            return RedirectToAction("EditMaintenanceGroupCustomers", new { customerId = model.CustomerId });
        }

        private MaintenanceGroupCustomersModel RebuildModel(MaintenanceGroupCustomersModel model, FormCollection formValues)
        {
            var tokenCharacter = new[] { model.Separator };

            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.
            foreach (var formValueKey in formValues.Keys)
            {
                var formKey = formValueKey.ToString();
                var formValue = formValues[formKey];
                if (formKey.Equals("NewCustomers"))
                    model.NewCustomers = formValue.Split(tokenCharacter, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return model;
        }

        #endregion


        #region Maintenance Group Activation
        public ActionResult ActivateMaintenanceGroup(int customerId)
        {
            MaintenanceGroupActivateModel model = (new MaintenanceGroupFactory()).CanActivate(customerId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ActivateMaintenanceGroup(string submitButton, CustomerActivateModel model)
        {
            // Was action RETURN?
            if (submitButton.Equals("RETURN"))
                return RedirectToAction("Index");

            // Activate customer and return to index page.
            (new MaintenanceGroupFactory()).Activate(model.CustomerId);
            return RedirectToAction("Index");
        }

        #endregion

    }
}
