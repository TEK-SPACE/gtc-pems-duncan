using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Roles;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using NPOI.HSSF.UserModel;
using WebMatrix.WebData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CustomerSettingsFactory = Duncan.PEMS.Business.Customers.SettingsFactory;
using UserSettingsFactory = Duncan.PEMS.Business.Users.SettingsFactory;


namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class UsersController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private readonly SecurityManager _secMgr = new SecurityManager();
        #region Index
        public ActionResult Index()
        {
            ViewBag.CurrentCityID = CurrentCity.Id;
            return View();

        }

        [HttpGet]
        public ActionResult GetUsers([DataSourceRequest] DataSourceRequest request, string roleId)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Users");

            //get the user models
            var userModels = (new UserFactory()).GetUsersListModels(CurrentCity);

            //sort, page, filter, group them
            var items = userModels.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            var total = items.Count();
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.ApplyGrouping(request.Groups);

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = data,
                Total = total
            };

            return Json(result,  JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Exporting"

        public FileResult ExportToCsv([DataSourceRequest]DataSourceRequest request, string roleId)
        {
            //get role models
            var data = GetExportData(request, roleId);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetUsers", CurrentCity.Id);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Username"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "First Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Last Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Creation Date"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Role Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Status"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Bad Login Count") );
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Last Login Date"), true);
            foreach (ListUserModel item in data)
            {
                //add a item for each property here
                AddCsvValue(writer, item.Username);
                AddCsvValue(writer, item.FirstName);
                AddCsvValue(writer, item.LastName);
                AddCsvValue(writer, item.CreationDateDisplay);
                AddCsvValue(writer, item.Role);
                AddCsvValue(writer, item.Status);
                AddCsvValue(writer, item.BadLoginCount.ToString());
                AddCsvValue(writer, item.LastLoginDateDisplay, true);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "UsersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

       


        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, string roleId)
        {
            //get role models
            var data = GetExportData(request, roleId, Constants.Export.ExcelExportCount);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetUsers", CurrentCity.Id);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue(GetLocalizedGridTitle(gridData, "Username"));
            headerRow.CreateCell(1).SetCellValue(GetLocalizedGridTitle(gridData, "First Name"));
            headerRow.CreateCell(2).SetCellValue(GetLocalizedGridTitle(gridData, "Last Name"));
            headerRow.CreateCell(3).SetCellValue(GetLocalizedGridTitle(gridData, "Creation Date"));
            headerRow.CreateCell(4).SetCellValue(GetLocalizedGridTitle(gridData, "Role Name"));
            headerRow.CreateCell(5).SetCellValue(GetLocalizedGridTitle(gridData, "Status"));
            headerRow.CreateCell(6).SetCellValue(GetLocalizedGridTitle(gridData, "Bad Login Count"));
            headerRow.CreateCell(7).SetCellValue(GetLocalizedGridTitle(gridData, "Last Login Date"));

            //Populate the sheet with values from the grid data
            foreach (ListUserModel item in data)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.Username);
                row.CreateCell(columnNumber++).SetCellValue(item.FirstName);
                row.CreateCell(columnNumber++).SetCellValue(item.LastName);
                row.CreateCell(columnNumber++).SetCellValue(item.CreationDateDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.Role);
                row.CreateCell(columnNumber++).SetCellValue(item.Status);
                row.CreateCell(columnNumber++).SetCellValue(item.BadLoginCount);
                row.CreateCell(columnNumber++).SetCellValue(item.LastLoginDateDisplay);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "UsersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
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
            document =  AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));
            var data = GetExportData(request, roleId, Constants.Export.PdfExportCount);

            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetUsers", CurrentCity.Id);

            var dataTable = new PdfPTable(8) {WidthPercentage = 100};
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Username"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "First Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Last Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Creation Date"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Role Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Status"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Bad Login Count"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Last Login Date"));
            dataTable.CompleteRow();
            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;
            foreach (ListUserModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.Username);
                dataTable.AddCell(item.FirstName);
                dataTable.AddCell(item.LastName);
                dataTable.AddCell(item.CreationDateDisplay);
                dataTable.AddCell(item.Role);
                dataTable.AddCell(item.Status);
                dataTable.AddCell(item.BadLoginCount.ToString());
                dataTable.AddCell(item.LastLoginDateDisplay);
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "UsersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable GetExportData(DataSourceRequest request, string roleId, int size = 0)
        {
            //get the user models
            //no paging, we are bringing back all the items
            var userModels = (new UserFactory()).GetUsersListModels(CurrentCity);
            //sort, filter, group them
            var items = userModels.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            items = items.ApplySorting(request.Groups, request.Sorts);
            //if (size > 0)
            //    items = items.Take(size);
            //export change order udpate
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.ApplyGrouping(request.Groups);
            return data;
        }
        #endregion

        #region "Create"
        public ActionResult Create()
        {
            //get the groups for the current city so we can display a selectable list of groups for that city. 
            //This way, when a user is created, we can assign that user to those groups
            var model = new UserModel { Groups = GetGroups() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserModel model, string status, string submitButton)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Cancel":
                    return RedirectToAction("Index", "Users");
            }

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    var usrMgr = new UserFactory();

                    //go get the default password for the current city.
                    var settings = (new CustomerSettingsFactory()).Get("DefaultPassword", CurrentCity.Id);
                    string defaultPassword;
                    if (settings.Any())
                        defaultPassword = settings[0].Value;
                    else
                    {
                        ModelState.AddModelError("DefaultPassword", "No default password defined for customer.");
                        model.Groups = GetGroups();
                        return View(model);
                    }

                    //if the middle name is empty, set it to a default
                    if (string.IsNullOrEmpty(model.MiddleInitial))
                        model.MiddleInitial = Constants.User.DefaultMiddleName;
                   
                    //get the phone to empty string, since int he DB it shouldnt be null
                    if (string.IsNullOrEmpty(model.PhoneNumber))
                        model.PhoneNumber = "";

                    bool active = status == "Active";
                    //create the user, set their profile information, and add them to the default group for this store
                    string username = usrMgr.CreateUser(model, CurrentCity.InternalName, defaultPassword, active);

                    //set if they are a tech right off the bat
                    var settingsFactory = new SettingsFactory();
                    int userId = usrMgr.GetUserId(model.Username);
                    settingsFactory.Set(userId, Constants.User.IsTechnician, model.IsTechnician.ToString());

                    (new TechnicianFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).SetTechnician(model, CurrentCity);
                    return RedirectToAction("Index", "Users");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", HelperMethods.ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups();
            return View(model);
        }

        #endregion

        #region "Edit"

        public ActionResult Edit(string username)
        {
            var userFactory = new UserFactory();
            //now check to se if the requested user exist int he system
            if (!userFactory.DoesUserExist(username))
                return RedirectToAction("Index", "Users");

            //get the model
            var model = userFactory.GetProfileModel(username, CurrentCity);
            if (model == null)
                return RedirectToAction("Index", "Users");


            //go find the maint group - can only be part of one.
            var existingMGCustomer = (new PEMRBACEntities()).MaintenanceGroupCustomers.FirstOrDefault(x => x.CustomerId == CurrentCity.Id);
            ViewBag.IsMaintenanceGroupCustomer = existingMGCustomer != null;


           return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProfileModel model, string submitButton, string password, string status, FormCollection formColl)
        {
            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            //we dont care about the validation for questions and answers, so remove them from the model state
            ModelState.Remove("Question1.Question");
            ModelState.Remove("Question1.Answer");
            ModelState.Remove("Question2.Question");
            ModelState.Remove("Question2.Answer");
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Change Password":
                    return (ResetPassword(model));
                case "Clear Password History":
                    return (ClearPasswordHistory(model));
                case "Unlock User":
                    return (UnlockUser(model));
                    //check the other submit buttons and act on them, or continue
                case "Cancel":
                    return RedirectToAction("Index", "Users");
                case "Customer Roles":
                    return RedirectToAction("CustomerRoles", "Users", new { username = model.Username });
            }

            //case "Save":
            ModelState.Remove("Password.NewPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var usrMgr = new UserFactory();
                    // update the users profile
                    
                    bool active = status == "Active";
                    usrMgr.UpdateUserProfile(model, active);
                    //now we have to check to see if they are a technician. if they are, create a technician in the pems db
                    var settingsFactory = new SettingsFactory();
                    int userId = usrMgr.GetUserId(model.Username);
                    settingsFactory.Set(userId, Constants.User.IsTechnician, model.IsTechnician.ToString());
                    (new TechnicianFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).SetTechnician(model, CurrentCity);
                    //clear all their group permissions
                    CurrentAuthorizationManager.RemoveMemberGroups(model.Username);

                    // now add them to the specific group for this store they will have access to
                    CurrentAuthorizationManager.AddGroupMember(model.Role, model.Username);

                    // send them back to the user listing page.
                    return RedirectToAction("Index", "Users");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not update users profile.");
                }
            }
            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups(model.Username);
            return View(model);
        }

        [ChildActionOnly]
        protected ActionResult ResetPassword(ProfileModel model)
        {
            // If we got this far, something failed, redisplay form
            TryValidateModel(model.Password);
            model.Groups = GetGroups(model.Username);
            if (ModelState.IsValid)
            {
                // check to make sure the passwords have values
                if (string.IsNullOrEmpty(model.Password.NewPassword) ||
                    string.IsNullOrEmpty(model.Password.ConfirmPassword))
                {
                    ModelState.AddModelError("Password",
                                             (new ResourceFactory()).GetLocalizedTitle(
                                                 ResourceTypes.ErrorMessage,
                                                 "Password must be set"));
                    return View(model);
                }

                //force the pw reset as admin, ignore pw reset rules, etc.
                try
                {
                    var pwMGr = new PasswordManager(model.Username);
                    pwMGr.ChangePassword(model.Password.NewPassword, true);
                    // send them back to the user listing page.
                    ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage, "Password Changed Successfully"));
                    return Edit(model.Username);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not reset users password.");
                }
            }

            //otherwise, rol lthoreugh and add the errors
            List<string> errorsToAdd = ModelState.Values.SelectMany(modelValue => modelValue.Errors).Select(modelError => modelError.ErrorMessage).ToList();

            foreach (var error in errorsToAdd)
                ModelState.AddModelError("Password", (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage, error));


            return View("Edit", model);
        }

        [ChildActionOnly]
        protected ActionResult ClearPasswordHistory(ProfileModel model)
        {
                try
                {
                    var pwMGr = new PasswordManager(model.Username);
                    pwMGr.ClearPasswordHistory();

                    // send them back to the user listing page.
                    return RedirectToAction("Index", "Users");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not reset users password.");
                }

            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups(model.Username);
            return View("Edit", model);
        }


        [ChildActionOnly]
        protected ActionResult UnlockUser(ProfileModel model)
        {
            try
            {
                //all this does is reset the number of failed password attemps since last successful login.
                ModelState.Clear();
                var manager = new UserFactory();
                manager.UnLockUser(model.Username);
                ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,(new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,"User Unlocked Successfully"));
               //now return them to the edit page again, refresh it since we jsut changed the user
                return Edit(model.Username);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Could not reset users password.");
            }

            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups(model.Username);
            return View("Edit", model);
        }

        #endregion

        #region "Delete"

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string username)
        {
            if (ModelState.IsValid)
            {
                var usrMgr = new UserFactory();

                //the first thing we have to do is check to see if this user is part of the current city. if it is not, send them back to the index page.
                if (!_secMgr.CheckUserAccessForCity(username, CurrentCity))
                    return RedirectToAction("Index", "Users");

                //now we have to check to make sure a user cant delete themselves
                if (username.ToLower().Trim() == WebSecurity.CurrentUserName.ToLower().Trim())
                    return RedirectToAction("Index", "Users");

                try
                {
                    //delete the user
                    usrMgr.DeleteUser(username);

                    //send them back to the user listing page.
                    return RedirectToAction("Index", "Users");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not delete the user.");
                }
            }
            return RedirectToAction("Index", "Users");
        }

        #endregion

        #region "Details"
        
        public ActionResult Details(string username)
        {
          
            var usrMgr = new UserFactory();
            //now check to se if the requested user exist int he system
            if (!usrMgr.DoesUserExist(username))
                return RedirectToAction("Index", "Users");

            //get the model
            ProfileModel model = usrMgr.GetProfileModel(username, CurrentCity);
            if (model == null)
                return RedirectToAction("Index", "Users");


            //go find the maint group - can only be part of one.
            var existingMGCustomer = (new PEMRBACEntities()).MaintenanceGroupCustomers.FirstOrDefault(x => x.CustomerId == CurrentCity.Id);
            ViewBag.IsMaintenanceGroupCustomer = existingMGCustomer != null;



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(UserStatsModel model, string submitButton)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Edit":
                    return RedirectToAction("Edit", "Users", new{username = model.Username});
                case "Return":
                    return RedirectToAction("Index", "Users");
                case "Customer Roles":
                    return RedirectToAction("CustomerRoles", "Users", new { username = model.Username });
            }

            return Details(model.Username);
        }


        #endregion

        #region "Helper Methods"
        public List<CheckBoxItem> GetGroups()
        {
            return (new RoleFactory()).GetGroups(CurrentCity);
        }
        public List<CheckBoxItem> GetGroups(string username)
        {
            return (new RoleFactory()).GetGroups(username, CurrentCity);
        }
        public JsonResult GetRoles(string cid)
        {
            var items = (new RoleFactory()).GetRoleListItems(cid);
            return Json(items);
        }
        #endregion

        #region "Actions"

        public ActionResult Action(string ddlActionValue)
        {
            //get the userIds out of session
            var model = new BulkUpdateModel
                {
                    Groups = GetGroups(),
                    ChangeType = ddlActionValue == "ChangeRole" ? ChangeType.Role : ChangeType.Status
                };
            //save the change type
            Session["__SavedChangeType"] = model.ChangeType;
            var userNames = GetActionableUsernames();
            if (userNames != null)
                model.UserNames = userNames;

            return View("BulkUpdate", model);
        }

        public JsonResult SetActionableUsernames(string[] uns)
        {
            Session["__SavedActionableUsernames"] = uns;
            return null;
        }

        private string[] GetActionableUsernames()
        {
            if (Session["__SavedActionableUsernames"] != null)
            {
                var retVal = Session["__SavedActionableUsernames"] as string[];
                //clear the session
                return retVal;
            }
            return null;
        }

        [HttpPost]
        public ActionResult BulkUpdate(BulkUpdateModel model, string submitButton)
        {
            //check the other submit buttons and act on them, or continue
            switch (submitButton)
            {
                case "Cancel":
                    return RedirectToAction("Index", "Users");
            }

            //case "Save":
            if (ModelState.IsValid)
            {

                var changeType = ChangeType.Status;
                if (Session["__SavedChangeType"] != null)
                {
                   changeType = Session["__SavedChangeType"] is ChangeType ? (ChangeType) Session["__SavedChangeType"] : ChangeType.Status;
                    //clear the session
                }

                try
                {
                    //get the users to act on
                    var usrMgr = new UserFactory();
                    //determine the action
                    if (changeType == ChangeType.Status)
                    {
                        //if status, update status for those users
                        bool active = model.Action == "Active";
                        foreach (var userName in   GetActionableUsernames() )
                            usrMgr.UpdateUserStatus(userName, active);
                    }
                    else
                    {
                        //if roles, update roles for those users - role is the current action
                        var roleName = model.Action;
                        //clear all first, then update
                        foreach (var userName in GetActionableUsernames())
                          {
                              //clear all their group permissions
                              CurrentAuthorizationManager.RemoveMemberGroups(userName);

                              // now add them to the specific group for this store they will have access to
                              CurrentAuthorizationManager.AddGroupMember(roleName, userName);
                          }
                    }
                    // send them back to the user listing page.
                    return RedirectToAction("Index", "Users");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Could not update users profile.");
                }
            }
            // If we got this far, something failed, redisplay form
            model.Groups = GetGroups();
            return View(model);
        }

        #endregion
    }
}