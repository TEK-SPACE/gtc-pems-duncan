using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Discounts;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Entities.Discounts;
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
    public class DiscountsController : PemsController
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
        public ActionResult GetAccounts([DataSourceRequest] DataSourceRequest request, string scheme,
                                        string schemeStatusId, string customerId)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "DiscountSummary");

            //get the user models
            int total;
            var items = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSummaryAccounts(request, out total, scheme, schemeStatusId,
                                                                   customerId, true);
            
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
                {
                    Data = items,
                    Total = total
                };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region "User Account Export"
         public FileResult ExportUserDiscountsToCsv(int userId)
         {
             var userAccount = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetUserAcount(userId);
             var output = new MemoryStream();
             var writer = new StreamWriter(output, Encoding.UTF8);
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Scheme Name"));
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Date"));
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Status"));
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Status Date"));
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Expiration Date"));
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Last Editied By"), true);
             foreach (var item in userAccount.DiscountSchemes)
             {
                 //add a item for each property here
                 AddCsvValue(writer, item.SchemeName);
                 AddCsvValue(writer, item.ApplicationDateDisplay);
                 AddCsvValue(writer, item.ApplicationStatus);
                 AddCsvValue(writer, item.ApplicationStatusDisplay);
                 AddCsvValue(writer, item.ExpirationDateDisplay);
                 AddCsvValue(writer, item.LastEditiedByUserName, true);
             }
             writer.Flush();
             output.Position = 0;
             return File(output, "text/comma-separated-values",
                         "UserDiscountSchemeExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
         }

         public FileResult ExportUserDiscountsToExcel(int userId)
        {
            var userAccount = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetUserAcount(userId);
            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();
            //add the filtered values to the top of the excel file
            // Add the filtered values to the top of the excel file
            var customFilters = new List<FilterDescriptor>();
            var rowNumber = AddFiltersToExcelSheet(sheet, customFilters);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Scheme Name"));
            headerRow.CreateCell(1).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Date"));
            headerRow.CreateCell(2).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Status"));
            headerRow.CreateCell(3).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Status Date"));
            headerRow.CreateCell(4).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Expiration Date"));
            headerRow.CreateCell(5).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Last Editied By"));
            //Populate the sheet with values from the grid data
            foreach (var item in userAccount.DiscountSchemes)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);
                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.SchemeName);
                row.CreateCell(columnNumber++).SetCellValue(item.ApplicationDateDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.ApplicationStatus);
                row.CreateCell(columnNumber++).SetCellValue(item.ApplicationStatusDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.ExpirationDateDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.LastEditiedByUserName);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user
            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "UserDiscountSchemeExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportUserDiscountsToPDF(int userId)
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
            var customFilters = new List<FilterDescriptor>();
            document = AddFiltersToPdf(document, customFilters);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));
            var userAccount = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetUserAcount(userId);
            //Create new Excel workbook

            var dataTable = new PdfPTable(6) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Scheme Name"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Date"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Status"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Application Status Date"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Expiration Date"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Last Editied By"));
            dataTable.CompleteRow();

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;
            foreach (var item in userAccount.DiscountSchemes)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.SchemeName);
                dataTable.AddCell(item.ApplicationDateDisplay);
                dataTable.AddCell(item.ApplicationStatus);
                dataTable.AddCell(item.ApplicationStatusDisplay);
                dataTable.AddCell(item.ExpirationDateDisplay);
                dataTable.AddCell(item.LastEditiedByUserName);
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "UserDiscountSchemeExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        #endregion


        #region "User Account Details"

        public ActionResult Details(int userId)
        {
            var userAccount = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetUserAcount(userId);
            return View(userAccount);
        }

        #endregion

        #region "User Scheme Details"

        public ActionResult UserSchemeDetails(int userId, int userSchemeId)
        {
            AccountSchemeDetails accountSchemeDetails = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAccountSchemeDetails(userId,
                                                                                                        userSchemeId);
            return View(accountSchemeDetails);
        }

        [HttpPost]
        public ActionResult ApproveApplication(int userId, int userSchemeId, string notes)
        {
            var localTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;
            (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).ApproveApplication(userId, userSchemeId, notes, localTime);
            return RedirectToAction("UserSchemeDetails", "Discounts", new {userId = userId, userSchemeId = userSchemeId});
        }

        [HttpPost]
        public ActionResult RejectApplication(int userId, int userSchemeId, string notes)
        {
            var localTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;

            (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).RejectApplication(userId, userSchemeId, notes, localTime);
            return RedirectToAction("UserSchemeDetails", "Discounts", new {userId = userId, userSchemeId = userSchemeId});
        }

        #endregion

        #region "Edit User Account"

        public ActionResult EditAccount(int userId)
        {
            var userAccount = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetUserAcount(userId);
            return View(userAccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(UserAccountDetails model, string submitButton, FormCollection formColl)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Terminate":
                case "Activate":
                case "Unlock":
                    return RedirectToAction("Index", "Alarms");
            }
            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);
            //default is save
            if (ModelState.IsValid)
            {
                //make sure that email doesnt exist in the system already. (expect for the current user). otherwise show an error.
                var emailAlreadyExists = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckExistingEmail(model.UserId, model.Email,
                                                                                    model.CustomerId);
                if (emailAlreadyExists)
                {
                    ModelState.AddModelError("Email",
                                             (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage,
                                                                                       "That email is already in use."));
                    return View(model);
                }

                (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveUserAcount(model);
                ModelState.Clear();
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                         (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,
                                                                                   "Account Saved"));
                return EditAccount(model.UserId);
                //return RedirectToAction("EditAccount", "Discounts", new {userId = model.UserId});
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult TerminateAccount(int userID, string notes)
        {
            (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).TerminateUserAccount(userID, notes);
            return RedirectToAction("EditAccount", "Discounts", new {userId = userID});
        }

        [HttpPost]
        public ActionResult ReactivateAccount(int userID, string notes)
        {
            (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).ReactivateUserAccount(userID, notes);
            return RedirectToAction("EditAccount", "Discounts", new {userId = userID});
        }

        [HttpPost]
        public ActionResult UnlockAccount(int userID, string notes)
        {
            (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).UnlockUserAccount(userID, notes);
            return RedirectToAction("EditAccount", "Discounts", new {userId = userID});
        }

        #endregion

        #region "Exporting"

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string scheme,
                                      string schemeStatusId, string customerId)
        {
            var items = GetExportData(request, scheme, schemeStatusId, customerId);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetItems", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "DiscountSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest] DataSourceRequest request, string scheme,
                                        string schemeStatusId, string customerId)
        {
            var items = GetExportData(request, scheme, schemeStatusId, customerId);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            //add the scheme and status if selected
            if (!string.IsNullOrEmpty(schemeStatusId))
                filters.Add(new FilterDescriptor { Member = "Discount Status", Value = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSchemeStatus(schemeStatusId) });
            if (!string.IsNullOrEmpty(scheme))
                filters.Add(new FilterDescriptor { Member = "Discount Scheme", Value = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSchemeName(scheme) });

            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetItems", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "DiscountSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string scheme,
                                      string schemeStatusId, string customerId)
        {

            var items = GetExportData(request, scheme, schemeStatusId, customerId);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            //add the scheme and status if selected
            if (!string.IsNullOrEmpty(schemeStatusId))
                filters.Add(new FilterDescriptor { Member = "Discount Status", Value = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSchemeStatus(schemeStatusId) });
            if (!string.IsNullOrEmpty(scheme))
                filters.Add(new FilterDescriptor { Member = "Discount Scheme", Value = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSchemeName(scheme) });

            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetItems", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "DiscountSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable<UserAccountListModel> GetExportData(DataSourceRequest request, string scheme, string schemeStatusId,
                                          string customerId)
        {
            //get the user models
            int total;
            var items = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSummaryAccounts(request, out total, scheme, schemeStatusId,
                                                                   customerId, true);
            return items;
        }

        #endregion

        #region "Filter Values"

        public JsonResult GetAccountStates()
        {
            var assetTypes = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAccountStates();
            return Json(assetTypes, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDiscountSchemes(int cityId)
        {
            var assetTypes = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDiscountSchemes(cityId);
            return Json(assetTypes, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDiscountStates()
        {
            var assetTypes = (new DiscountFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDiscountStates();
            return Json(assetTypes, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
