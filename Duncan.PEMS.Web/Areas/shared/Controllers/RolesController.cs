
/******************* CHANGE LOG **********************************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 01/09/2014       Sergey Ostrerov            Issue: DPTXPEMS-137 - DuncanAdmin Exported file names (pdf, csv, xls)include AM/PM Time as well 
 *                                                                   but not NorthSydney Exported files.
 *                                                                   Changed DateTime format to include current Time to the file name.
 * 
 * ****************************************************************************************************************************************************************/

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Business.Roles;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Business.Utility.Audit;
using Duncan.PEMS.Entities.Roles;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using NPOI.HSSF.UserModel;
using WebMatrix.WebData;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class RolesController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        //
        // GET: /shared/Roles/
        #region "Role Inquiry"

        public ActionResult Index()
        {
            ViewBag.CurrentCityID = CurrentCity.Id;
            return View();
        }

        [HttpGet]
        public ActionResult GetRoles([DataSourceRequest] DataSourceRequest request, string roleId, string customerId)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Roles");

            //get role models
            var listItems = (new RoleFactory()).GetRoleListModels(roleId, customerId);
            var items = listItems.AsQueryable();

            //now that we have our list of filtered items, we can apply the paging, sorting, etc.
            //Get the data representing the current grid state - page, sort and filter

            items = items.ApplyFiltering(request.Filters);
            int total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.ApplyGrouping(request.Groups);
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = data,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult GetRoles(string cid)
        {
            var items = (new RoleFactory()).GetRoleListItems(cid);
            return Json(items);
        }
      
        public JsonResult GetClients()
        {
            var secMgr = new SecurityManager();
            var clients = secMgr.GetCitiesForUser(WebSecurity.CurrentUserName);
            var items = clients.Select(client => new SelectListItem { Text = client.DisplayName, Value = client.Id.ToString() }).ToList();
            return Json(items);
        }
        #endregion

        #region "Exporting"
        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string roleId, string customerId)
        {
            //get role models
            var data = GetExportData( request, roleId, customerId );
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetRoles", CurrentCity.Id);

            var output = new MemoryStream();
            var writer = new StreamWriter( output, Encoding.UTF8 );

            // Write column headers
            AddCsvValue( writer, GetLocalizedGridTitle( gridData, "Role Name" ) );
            AddCsvValue( writer, GetLocalizedGridTitle( gridData, "Last Modified On" ) );
            AddCsvValue( writer, GetLocalizedGridTitle( gridData, "Last Modified By" ), lastCellInRow: true );

            // Write rows
            foreach (ListRoleModel item in data)
            {
                AddCsvValue( writer, item.RoleName );
                AddCsvValue( writer, item.LastModifiedOn.ToString() );
                AddCsvValue( writer, item.LastModifiedBy, lastCellInRow: true );
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "RolesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, string roleId, string customerId)
        {

            //get role models
            var data = GetExportData(request, roleId, customerId, Constants.Export.ExcelExportCount);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetRoles", CurrentCity.Id);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row
            headerRow.CreateCell(0).SetCellValue(GetLocalizedGridTitle(gridData,"Role Name"));
            headerRow.CreateCell(1).SetCellValue(GetLocalizedGridTitle(gridData,"Last Modified On"));
            headerRow.CreateCell(2).SetCellValue(GetLocalizedGridTitle(gridData,"Last Modified By"));

            //Populate the sheet with values from the grid data
            foreach (ListRoleModel item  in data)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.RoleName);
                row.CreateCell(columnNumber++).SetCellValue(item.LastModifiedOn.ToString());
                row.CreateCell(columnNumber++).SetCellValue(item.LastModifiedBy);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "RolesExport_"+FormatHelper.FormatDateTime(DateTime.Now)+".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request,  string roleId, string customerId)
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
            var data = GetExportData(request, roleId, customerId, Constants.Export.PdfExportCount);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetRoles", CurrentCity.Id);

            var dataTable = new PdfPTable(3) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell(GetLocalizedGridTitle(gridData,"Role Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData,"Last Modified On"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData,"Last Modified By"));
            dataTable.CompleteRow();
            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;

            foreach (ListRoleModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.RoleName);
                dataTable.AddCell(item.LastModifiedOn.ToString());
                dataTable.AddCell(item.LastModifiedBy);
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "RolesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }



        private  IEnumerable GetExportData(DataSourceRequest request, string roleId, string customerId, int size = 0)
        {
            //get role models
            //no paging, we are bringing back all the items
            var listItems = (new RoleFactory()).GetRoleListModels(roleId, customerId);
            var items = listItems.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            items = items.ApplySorting(request.Groups, request.Sorts);
           // if (size > 0)
                //items = items.Take(size);//export change order udpate
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.ApplyGrouping(request.Groups);
            return data;
        }
        #endregion



        #region "Edit"

        public ActionResult Edit(string roleName, string customerInternalName, string customerName)
        {
            var roleModel = new EditRoleModel
                {
                    CustomerName = customerName,
                    RoleName = roleName,
                    CustomerInternalName = customerInternalName
                };
            var authMan = new AuthorizationManager(customerInternalName);
            var permissions = authMan.GetAuthorizationList(roleName);
            roleModel.Items = permissions;
            return View(roleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditRoleModel model, string submitButton, int[] checkedNodes)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Copy Role":
                    return RedirectToAction("Copy", new { roleName = model.RoleName });
                case "Return to Previous Page":
                    return RedirectToAction("Index", "Roles");

            }

            //case "Update Role":
            var authMan = new AuthorizationManager(model.CustomerInternalName);
            var permissions = authMan.GetAuthorizationList(model.RoleName);
            if (ModelState.IsValid)
            {
                try
                {

                    //update the role and send them back to the list page
                    //get all the permissions for a user

                    //for each one, set to false unless its in our list of checked nodes.
                    foreach (var authorizationItem in permissions)
                    {
                        authorizationItem.Authorized = checkedNodes.Contains(authorizationItem.Id);
                        foreach (var subAuthItem in authorizationItem)
                            subAuthItem.Authorized = checkedNodes.Contains(subAuthItem.Id);
                    }

                    //then save it back to the DB
                    int groupID = authMan.SaveAuthorizationList(model.RoleName, permissions);
                    int userID = WebSecurity.CurrentUserId;
                    (new AuditFactory()).ModifiedBy(Constants.Audits.RoleTableName, groupID, userID);
                    return RedirectToAction("Index", "Roles");

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not update role permissions.");
                }
            }

            //// If we got this far, something failed, redisplay form
            model.Items = permissions;
            return View(model);
        }
        #endregion
        
        #region "Create"
        public ActionResult Create()
        {
            var roleModel = new CreateRoleModel
                {
                    CustomerName = CurrentCity.DisplayName,
                    CustomerInternalName = CurrentCity.InternalName
                };
            var authMan = new AuthorizationManager(CurrentCity.InternalName);
            var permissions = authMan.GetAuthorizationList();
            roleModel.Items = permissions;
            return View(roleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateRoleModel model, string submitButton, int[] checkedNodes)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Cancel":
                    return RedirectToAction("Index", "Roles");

            }

            //case "Update Role":
            //get a blank one, then save it
            var authMan = new AuthorizationManager(model.CustomerInternalName);
            var permissions = authMan.GetAuthorizationList();
            if (ModelState.IsValid)
            {
                try
                {
                    //create the role and send them back to the list page
                    //get all the permissions for a user

                    //for each one, set to false unless its in our list of checked nodes.
                    foreach (var authorizationItem in permissions)
                    {
                        authorizationItem.Authorized = checkedNodes.Contains(authorizationItem.Id);
                        foreach (var subAuthItem in authorizationItem)
                            subAuthItem.Authorized = checkedNodes.Contains(subAuthItem.Id);
                    }

                    //then save it back to the DB
                    int groupID = authMan.CreateAuthorizationList(model.RoleName, permissions);
                    int userID = WebSecurity.CurrentUserId;
                    (new AuditFactory()).CreatedBy(Constants.Audits.RoleTableName, groupID, userID);
                    (new AuditFactory()).ModifiedBy(Constants.Audits.RoleTableName, groupID, userID);
                    return RedirectToAction("Index", "Roles");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not update role permissions.");
                }
            }

            //// If we got this far, something failed, redisplay form
            model.Items = permissions;
            return View(model);
        }

        public JsonResult DoesRoleExist(string roleName, string customerInternalName)
        {
            bool roleExist = (new RoleFactory()).DoesRoleExist(roleName, customerInternalName);
            return Json(roleExist);
        }
        #endregion

        #region "Copy"
        public ActionResult Copy(string roleName)
        {
            ViewBag.RoleName = roleName;
            var roleModel = new CopyRoleModel
                {
                    CustomerName = CurrentCity.DisplayName,
                    CustomerId = CurrentCity.Id,
                    CustomerInternalName = CurrentCity.InternalName
                };

            return View(roleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(CopyRoleModel model, string submitButton, string ddlRoleName)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Cancel":
                    return RedirectToAction("Index", "Roles");

            }

            //case "Copy Role":
            try
            {
                var authMan = new AuthorizationManager(model.CustomerInternalName);
                int groupID = authMan.CloneAuthorizationList(ddlRoleName, model.ToRoleName);

                //then save it back to the DB
                int userID = WebSecurity.CurrentUserId;
                (new AuditFactory()).CreatedBy(Constants.Audits.RoleTableName, groupID, userID);

                return RedirectToAction("Index", "Roles");

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Could not copy role .");
            }
            //// If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

    }
}
