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
using Duncan.PEMS.Entities.FileUpload;
using System.Collections.Generic;
using Kendo.Mvc.Extensions;
using Duncan.PEMS.Web.Areas.city.Controllers;
using System.Web;
using Duncan.PEMS.Business.FileUpload;
using System.Security.Cryptography;
using Duncan.PEMS.Business.Exports;
using Kendo.Mvc;
using Duncan.PEMS.Business.Alarms;
using System.Web.Script.Serialization;
using Duncan.PEMS.Entities.GIS;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class FileUploadController : PemsController
    {
        //  List<FDFilesModel> SummaryResultList = new List<FDFilesModel>();
        // FileUploadModel t1 = new FileUploadModel();

        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region File Upload Summary

        /// <summary>
        /// Description: This Method will returns the view 
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.CurrentCityID = CurrentCity.Id;
            ViewBag.FileTypes = new SelectList((new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFileTypesForUpload1(CurrentCity.InternalName), " Value", "Text");
            return View();
        }

        /// <summary>
        /// Description: This Method will returns list  of filenames with respect to filetype
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="fileTypeVal"></param>
        /// <returns></returns>
        public ActionResult GetListOfFileNames(int fileTypeVal)
        {

            var Files = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFileNames(fileTypeVal);
            return Json(Files, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This Method will addition of files to the system. 
        /// When in Duncan Admin these files will be binary files for assets which are system wide.
        /// When in a customer the PGM, RPG, CCF, and MPBConfig files can be added.
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)   
        /// </summary>
        /// <param name="files"></param>
        /// <param name="FileType"></param>
        /// <param name="FileTypeExt"></param>
        /// <param name="FileComment"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Submit(HttpPostedFileBase files, int FileType, string FileTypeExt, string FileComment, string FileVersion)
        {
            // HttpPostedFileBase files = Request.Files[0];
            List<string> errors = new List<string>();
            string errormsg = "";
            try
            {
                if (files != null)
                {
                    int fileindex = FileTypeExt.IndexOf("(.") + 2;
                    FileTypeExt = FileTypeExt.Substring(fileindex, (FileTypeExt.IndexOf(')') - fileindex));
                    int tempFileInActive = 0;   // checking file is inactive for any meter                
                    var UserId = WebSecurity.CurrentUserId;
                    var fileName = Path.GetFileName(files.FileName);
                    BinaryReader b = new BinaryReader(files.InputStream);
                    uint filesize = Convert.ToUInt32(files.InputStream.Length);
                    byte[] filedata = new byte[filesize];
                    Array.Copy(b.ReadBytes(Convert.ToInt32(filesize)), 0, filedata, 0, filesize);
                    var filehash = CalHasValue(filedata);
                    var fileExt = Path.GetExtension(files.FileName).Substring(1).ToUpper();
                    var temp = 0; // checking for any error msg 

                    if (fileName.Length > 19)
                    {
                        // File length validation
                        errors.Add("ERROR:'" + fileName + "' File name length must be less than 20");
                        temp = 1;
                    }

                    if (FileTypeExt != fileExt)
                    {
                        // show error msg for invalid filetype
                        errors.Add("ERROR:'" + fileName + "'Invalid type. Only " + FileTypeExt + " Type supported.");
                        temp = 1;
                    }


                    if (temp == 0)
                    {
                        // check already exixts 
                        var FDFilesValues = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).FileExists(filehash, CurrentCity.Id);

                        if (FDFilesValues.FileID > 0)
                        {
                            // If File is Active or No Longer Active an error message will be displayed to the client
                            errors.Add("MSG:'" + fileName + "'File Name already exists and is " + FDFilesValues.Activestaus + " State");
                            var values = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFileInfo(FDFilesValues.FileID);

                            foreach (var value in values)
                            {
                                if (value.Active == 1)
                                {
                                    errors.Add("MSG:'" + fileName + "' for Meter '" + value.MeterID + "' is Active State..");
                                }
                                else if (value.Active == 0)
                                {

                                    errors.Add("ERROR:'" + fileName + "' for Meter '" + value.MeterID + "' is InActive State..");
                                    tempFileInActive = 1;
                                }
                            }
                        }

                        if (FDFilesValues.FileID <= 0 || tempFileInActive == 1) // checking file exists or no longer active
                        {

                            long FileID = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UploadFile(fileName, filedata, filehash, FileType, UserId, FileComment);

                            if (FileID > 0)
                            {
                                errors.Add("MSG:'" + fileName + "' Successfuly Uploded Server.");
                            }
                        }

                    }
                }
                else
                {
                    errors.Add("ERROR: Unable to read File Try again.");
                }

                foreach (var err in errors)
                    errormsg = errormsg + "####" + err;
            }
            catch
            {
                errors.Add("ERROR: Unable to read File Try again.");

                foreach (var err in errors)
                    errormsg = errormsg + "####" + err;
            }


            return Json(new { status = errormsg }, "text/plain");
        }


        public bool FileIsActive(long FileID)
        {
            var values = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).FileIsActive(FileID);
            if (values != null)
                return true;
            return false;

        }

        /// <summary>
        ///  Description: This Method will return list of files uploaded to system with details
        ///  When in Duncan Admin these files will be binary files for assets which are system wide. 
        ///  When in a customer the PGM, RPG, CCF, and MPBConfig files 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetFileDownloadsummary([DataSourceRequest]DataSourceRequest request)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "FileUpload");

            IEnumerable<FDFilesModel> SummaryResult = Enumerable.Empty<FDFilesModel>();
            int total = 0;
            var SummaryResult_raw = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFDFileDetails(request, CurrentCity.InternalName, out total);

            //********* end of webservice code ****************

            SummaryResult = from r in SummaryResult_raw
                            select new FDFilesModel
                            {
                                FileID = r.FileID,
                                FileName = r.FileName,

                                FileTypeText = r.FileTypeText,

                                FileComments = r.FileComments,

                                Activestaus = r.Activestaus,
                                FileAdditionDate = r.FileAdditionDate,
                                //FileDate = r.FileAdditionDate.ToString(),
                                UploadedBy = r.Userid == 0 ? "" : (new UserFactory()).GetUserById(r.Userid).FullName(),

                            };



            // DataSourceResult finalResult = SummaryResult.ToDataSourceResult(request);
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = SummaryResult,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///  Description: This Method will update file state with comments for particular file    
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="FileStatus"></param>
        /// <param name="fileIdChecked">If it true then will update</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileUploadGrid_Update([DataSourceRequest] DataSourceRequest request, FDFilesModel GridRowdata, string FileStatus, bool fileIdChecked)
        {
            var UserId = WebSecurity.CurrentUserId;
            if (GridRowdata != null && fileIdChecked)
            {
                if (FileStatus == "1")
                {
                    //Checking file already exists or not
                    var FDFilesValues = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).FileActiveExists(GridRowdata);

                    if (FDFilesValues.FileID > 0)
                    {
                        return null;
                    }

                }
                (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateFile(GridRowdata, FileStatus, UserId);

            }

            return Json(ModelState.ToDataSourceResult());
        }

        #endregion

        /// <summary>
        /// Description: This controller action is used to get timezone info about the customer and set time in the schedule FD page accordingly.
        /// Modified By: Sairam on 25th sep 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleFD()
        {


            List<DateTime> TimeZoneDateTime = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeZoneNames(CurrentCity.Id);

            GISModel inst = new GISModel();

            if (TimeZoneDateTime.Count() > 0)
            {
                string finalTime = TimeZoneDateTime[0].ToString();

                ViewBag.customerDateIs = finalTime;
                inst.Text = finalTime;
                inst.errorMsg = "";
            }
            else
            {
                //** If no records, that indicates time zone info is not found and hence take local time.
                ViewBag.customerDateIs = DateTime.Now.ToString();
                inst.Text = DateTime.Now.ToString();
                inst.errorMsg = "";
            }



            return View(inst);
        }

        #region Caliculate Hash Value
        /// <summary>
        /// Description: This Method will calculate hashvalue for given file data
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="filedata"></param>
        /// <returns></returns>
        public string CalHasValue(byte[] filedata)
        {
            string HashValue = "";

            //  using (BufferedStream bs = new BufferedStream(files.InputStream))
            // {
            using (SHA1Managed sha12 = new SHA1Managed())
            {
                byte[] hash2 = sha12.ComputeHash(filedata);
                StringBuilder formatted = new StringBuilder(2 * hash2.Length);
                foreach (byte b in hash2)
                {
                    HashValue = "";
                    HashValue += formatted.AppendFormat("{0:X2}", b);
                }
            }
            // }

            return HashValue.ToLower();
        }
        #endregion     
              
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ScheduleAssetJobs(string chosenAsset, string locTypeVal, string assetSelected, string fileSelected, string dateScheduled, string dateActivated)
        {
            List<string> errors = new List<string>();
            string errormsg = "";

            //**To send output to View
            List<FileAsset> assetList = new List<FileAsset>();
            List<string> FilesAre = new List<string>();

            //** Conversion of string type dates to datetime type
            DateTime scheduledDate;
            DateTime activatedDate;

            //** Get the array of assets and files from parameters
            string[] assets = assetSelected.Split(',');
            string[] files = fileSelected.Split(',');

            scheduledDate = Convert.ToDateTime(dateScheduled);
            activatedDate = Convert.ToDateTime(dateActivated);

            //** Check all the files in the fileSelected Array for ACtive Status
            for (var i = 0; i < files.Length; i++)
            {

                try
                {

                    //** Get the File Active Status as well as file related info to store it in FDSummary table.

                    //** Get the FileHash for the corresponding FileID so that it can be passed to FilExists controller method for active status checking and other file details
                    long fileID = Convert.ToInt64(files[i]);
                    //var getFileHash = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFileHash(fileID);

                    //** Get the file active status and other file related details
                    //var FDFilesValues = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).FileExists(getFileHash.FileHash, CurrentCity.Id);

                    var FDFilesValues = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).FileExists1(fileID, CurrentCity.Id);

                    if (FDFilesValues.Activestaus == "Active")
                    {
                        //** The current file is 'Active', then only allow to schedule that file.

                        //** Get the user name who has scheduled the job.
                        var UserId = WebSecurity.CurrentUserId;
                        List<string> FileID = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).ScheduleJob(chosenAsset, locTypeVal, assets, FDFilesValues.FileID, scheduledDate, activatedDate, FDFilesValues.FileName, FDFilesValues.FileSizeBytes, FDFilesValues.FileHash, FDFilesValues.FileType, FDFilesValues.Activestaus, FDFilesValues.FileAdditionDate, UserId, CurrentCity.Id);

                        //** After getting successful assset scheduled, store the asset ID in the List

                        for (var j = 0; j < FileID.Count; j++)
                        {
                            FileAsset FA = new FileAsset();
                            FA.AssetIDIs = FileID[j];
                            FA.FileNameIs = FDFilesValues.FileName;
                            assetList.Add(FA);
                        }

                        //** After getting successful File scheduled, store the File name in the List
                        FilesAre.Add(FDFilesValues.FileName);



                    }
                    else
                    {
                        //** File is No Longer Active
                        errormsg = FDFilesValues.FileName + " is No Longer Active and hence cannot be scheduled.";
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("ERROR IN ScheduleAssetJobs Method (Asset Configuration - Schedule File Download)", ex);
                }

            }


            return Json(assetList.Distinct(), JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Description: This method is used to populate the 'Selection Method' dropdown menu in the Schedule FD screen;
        /// Modified By: Sairam on July 19th 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGroupingElements()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Asset Name", Value = "AssetName" });
            list.Add(new SelectListItem { Text = "Asset ID", Value = "AssetID" });
            list.Add(new SelectListItem { Text = "Area", Value = "Area" });
            list.Add(new SelectListItem { Text = "Zone", Value = "Zone" });
            list.Add(new SelectListItem { Text = "Street", Value = "Street" });
            list.Add(new SelectListItem { Text = "Suburb", Value = "Suburb" });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to get the locations for the 'Available Assets' list box in Schedule FD screen;
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="locationType"></param>
        /// <param name="meterGrpID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLocationsForListBox([DataSourceRequest] DataSourceRequest request, string locationType, int meterGrpID)
        {

            if (locationType.Length == 0)
            {
                return Content("");
            }

            IEnumerable<FDFilesModel> details = Enumerable.Empty<FDFilesModel>();

            details = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeIdListBox(locationType, CurrentCity.Id, meterGrpID);

            if (locationType == "AssetID")
            {
                var allDetails = (from detail in details
                                  select new
                                  {
                                      Text = detail.Value.ToString()

                                  }).ToList();
                var DistinctItems = allDetails.GroupBy(x => x.Text).Select(y => y.First());
                return Json(DistinctItems.ToDataSourceResult(request));
                // return Json(allDetails, JsonRequestBehavior.AllowGet);  //** This is used only when $.ajax method is applied

            }
            else if (locationType == "Street" || locationType == "Suburb" || locationType == "Demand Area" || locationType == "Area" || locationType == "Zone" || locationType == "AssetName")
            {
                var allDetails = (from detail in details
                                  select new
                                  {
                                      Text = detail.Text

                                  }).ToList();
                var DistinctItems = allDetails.GroupBy(x => x.Text).Select(y => y.First());
                return Json(DistinctItems.ToDataSourceResult(request));
            }


            return Json(details.ToList().ToDataSourceResult(request));
        }

        /// <summary>
        /// Description:This method is used to get the locations for the 'Selected Assets' list box in Schedule FD screen;
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="locationType"></param>
        /// <param name="groupItems"></param>
        /// <param name="assetTypeVal"></param>
        /// <param name="targetItems"></param>
        /// <returns></returns>
        public ActionResult GetTargetListLocations([DataSourceRequest] DataSourceRequest request, string locationType, string groupItems, int assetTypeVal, string targetItems)
        {

            if (locationType.Length == 0)
            {
                return Content("");
            }

            IEnumerable<FDFilesModel> details = Enumerable.Empty<FDFilesModel>();

            details = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTargetGroupLocations(locationType, CurrentCity.Id, groupItems, assetTypeVal);


            if (locationType == "Area" || locationType == "Zone" || locationType == "AssetID" || locationType == "Street" || locationType == "Suburb")
            {
                var allDetails = (from detail in details
                                  select new
                                  {
                                      Text = detail.Value.ToString()

                                  }).ToList();


                //** Add the result to allItems List;
                List<SelectListItem> allItems = new List<SelectListItem>();

                for (var i = 0; i < allDetails.Count; i++)
                {
                    allItems.Add(new SelectListItem { Text = allDetails[i].Text });
                }

                string[] targetListData = targetItems.Split(',');

                //** Check if the target list has some data, then append those to the new result;
                if (targetListData.Length > 0 && allItems.Count > 0)
                {
                    for (var i = 0; i < targetListData.Length; i++)
                    {
                        allItems.Add(new SelectListItem { Text = Convert.ToString(targetListData[i]) });
                    }


                }
                else if (targetListData.Length > 0 && allItems.Count == 0)
                {
                    for (var i = 0; i < targetListData.Length; i++)
                    {
                        allItems.Add(new SelectListItem { Text = Convert.ToString(targetListData[i]) });
                    }
                }

                //** Gets the Distinct List
                var DistinctItems = allItems.GroupBy(x => x.Text).Select(y => y.First());
                return Json(DistinctItems.ToDataSourceResult(request));
                // return Json(allDetails, JsonRequestBehavior.AllowGet);  //** This is used only when $.ajax method is applied

            }



            return Json(details.ToList().ToDataSourceResult(request));
        }


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

        #region "Exporting"
        /// <summary>
        ///  Description: This Method will export csv format (FileUplad details)     
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014) 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        public FileResult ExportToCsv([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {

            var items = GetExportData(request, startDate, endDate, FileType, FileStatus);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetFileDownloadsummary", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "FileSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        /// <summary>
        ///  Description: This Method will export Excel format  (FileUplad details)    
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {
            var items = GetExportData(request, startDate, endDate, FileType, FileStatus);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetFileDownloadsummary", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "FileSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        /// <summary>
        ///  Description: This Method will export PDF format (FileUplad details)   
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {
            var items = GetExportData(request, startDate, endDate, FileType, FileStatus);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetFileDownloadsummary", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "FileSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        /// <summary>
        ///  Description: This Method will get list of files and with details to export   
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)   
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        private IEnumerable<FDFilesModel> GetExportData(DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {
            IEnumerable<FDFilesModel> SummaryResult = Enumerable.Empty<FDFilesModel>();
            //var FileSummaryResult_raw = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFilesummary(startDate, endDate, licensePlate, FileType, FileStatus, vin, classes, prefix, issueNo, suffix, code, agency, street, beat, meterNo, officerID, officerName, vioDesc, CurrentCity.Id);

            int total = 0;
            var SummaryResult_raw = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFDFileDetails(request, CurrentCity.InternalName, out total);


            SummaryResult = from r in SummaryResult_raw
                            select new FDFilesModel
                            {
                                FileID = r.FileID,
                                FileName = r.FileName,

                                FileTypeText = r.FileTypeText,

                                FileComments = r.FileComments,

                                Activestaus = r.Activestaus,
                                FileDate = r.FileAdditionDate.ToString("dd/MM/yyyy hh:mm tt"),
                                //  FileDate = r.FileAdditionDate.ToString(),
                                UploadedBy = r.Userid == 0 ? "" : (new UserFactory()).GetUserById(r.Userid).FullName(),

                            };
            return SummaryResult;
        }

        /// <summary>
        ///  Description: This Method will export Excel format  (FileDownload details)    
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        public FileResult FileSummaryExportToCsv([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {

            var items = GetFileSummaryExportData(request);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetFileSummary", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "FileSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        /// <summary>
        ///  Description: This Method will export Excel format  (FileDownload details)    
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014) 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        public FileResult FileSummaryExportToExcel([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {
            var items = GetFileSummaryExportData(request);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetFileSummary", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "FileSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        /// <summary>
        ///  Description: This Method will export Excel format  (FileDownload details)    
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014) 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="FileType"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>
        public FileResult FileSummaryExportToPdf([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string FileType, string FileStatus)
        {
            var items = GetFileSummaryExportData(request);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetFileSummary", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "FileSummaryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        /// <summary>
        ///  Description: This Method will get list of files and with details to export   
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014) 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private IEnumerable<FDFilesModel1> GetFileSummaryExportData(DataSourceRequest request)
        {
            IEnumerable<FDFilesModel1> FileSummaryResult = Enumerable.Empty<FDFilesModel1>();
            //var FileSummaryResult_raw = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFilesummary(startDate, endDate, licensePlate, FileType, FileStatus, vin, classes, prefix, issueNo, suffix, code, agency, street, beat, meterNo, officerID, officerName, vioDesc, CurrentCity.Id);
            int total = 0;
            var FileSummaryResult_raw = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFilesummary(request, CurrentCity.Id, CurrentCity.InternalName, out total);




            FileSummaryResult = from r in FileSummaryResult_raw
                                select new FDFilesModel1
                                {

                                    FileID = r.FileID,
                                    JobID = r.JobID,
                                    FileName = r.FileName,
                                    FileComments = r.JobStatusDesc,
                                    FileType = r.FileType,
                                    JobStatusID = r.JobStatusID,
                                    Active = r.Active,
                                    JobStatusDesc = r.StatusDesc,
                                    StatusDesc = r.JobStatusDesc,
                                    FileTypeName = r.FileTypeName,
                                    FileAdditionDateFinal = Convert.ToString(r.FileAdditionDate),
                                    DateCompletedFinal = Convert.ToString(r.DateCompleted),
                                    MeterID = r.MeterID,
                                    MeterName = r.MeterName,
                                    UploadedBy = r.UploadedBy,
                                    CanceledBy = r.CanceledById == 0 ? "" : (new UserFactory()).GetUserById(r.CanceledById).FullName(),
                                    FileCancelationDateFinal = Convert.ToString(r.FileCancelationDate)



                                };
            return FileSummaryResult;
        }

        #endregion

        public ActionResult FileUploadGrid_Index_Update_Active([DataSourceRequest] DataSourceRequest request, FDFilesModel GridRowdata, string FileStatus)
        {
            if (GridRowdata != null && ModelState.IsValid)
            {
                (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateIndexFile(GridRowdata, FileStatus);
                ViewData["Result"] = "Success";
            }
            // return Json(new { status = "MSG"},"text/plain");
            return Json(ModelState.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #region FD Inquiry
        public ActionResult FDInquiry()
        {
            ViewBag.Companies = GetAllComments();
            return View();
        }

        /// <summary>
        /// Description:This method will return all the file types derived from the 'GetFileTypes' factory method to bind with file type dropdown in FDInquiry
        /// Modified:Prita()
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFileTypes()
        {

            var Files = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFileTypes(CurrentCity.InternalName);
            return Json(Files, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetFileTypesForUpload()
        {

            var Files = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFileTypesForUpload(CurrentCity.InternalName);
            return Json(Files, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Description:FileUploadGrid_Update_Active method will save any changes made to the grid by calling the 'UpdateFile_Active' facory method.
        /// This is mainly to save comment for file download cancellation.
        /// Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="FileStatus"></param>
        /// <returns></returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileUploadGrid_Update_Active([DataSourceRequest] DataSourceRequest request, FDFilesModel1 GridRowdata, string FileStatus)
        {
            var UserId = WebSecurity.CurrentUserId;
            if (GridRowdata != null && ModelState.IsValid)
            {
                (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateFile_Active(GridRowdata, FileStatus, UserId);
                ViewData["Result"] = "Success";
            }
            // return Json(new { status = "MSG"},"text/plain");
            return Json(ModelState.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to bind file scheduled data retrieved from 'GetFilesummary' method in facory to the grid in 'FDInquiry' page.
        /// Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetFileSummary([DataSourceRequest] DataSourceRequest request)
        {
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "FileDownload");

            IEnumerable<FDFilesModel1> FileSummaryResult = Enumerable.Empty<FDFilesModel1>();
            int total = 0;
            var FileSummaryResult_raw = (new FileUploadFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetFilesummary(request, CurrentCity.Id, CurrentCity.InternalName, out total);

            FileSummaryResult = from r in FileSummaryResult_raw
                                select new FDFilesModel1
                                {
                                    FileID = r.FileID,
                                    JobID = r.JobID,
                                    FileName = r.FileName,
                                    FileComments = r.FileComments,
                                    FileType = r.FileType,
                                    Active = r.Active,
                                    JobStatusID = r.JobStatusID,
                                    JobStatusDesc = r.StatusDesc,
                                    FileTypeName = r.FileTypeName,
                                    FileAdditionDate = r.FileAdditionDate,
                                    DateCompleted = r.DateCompleted,
                                    FileCancelationDate = r.FileCancelationDate,
                                    MeterID = r.MeterID,
                                    MeterName = r.MeterName,
                                    UploadedBy = r.UploadedBy,
                                    CanceledBy = r.CanceledById == 0 ? "" : (new UserFactory()).GetUserById(r.CanceledById).FullName(),

                                };
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = FileSummaryResult,
                Total = total,
            };

            return new LargeJsonResult() { Data = result, MaxJsonLength = int.MaxValue };

        }


        /// <summary>
        /// Description: This method will get the 'meter name' and bind it to the 'Asset name' auto complete when a asset ID is selected in asset ID auto complete of FDinquiry page.
        /// Modified:Prita()
        /// </summary>
        /// <param name="MeterId"></param>
        /// <returns></returns>
        public ActionResult GetAssetName(string MeterId)
        {
            string[] AssetName = MeterId.Split(',');

            string assetName = string.Empty;

            string meterId = AssetName[0].ToString();


            if (!string.IsNullOrEmpty(meterId))
            {
                int outNum;
                bool isNum = int.TryParse(meterId, out outNum);
                if (isNum)
                {
                    assetName = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetName(outNum);
                }

            }

            return Json(assetName, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This method will get the 'meter ID' and bind it to the 'Asset ID' auto complete when a Asset NAme is selected in Asset Name auto complete of FDinquiry page.
        /// Modified:Prita()
        /// </summary>
        /// <param name="MeterName"></param>
        /// <returns></returns>  
        public ActionResult GetAssetID(string MeterName)
        {
            string[] AssetName = MeterName.Split(',');

            int assetID = 0;
            string AssetID = string.Empty;

            string meterName = AssetName[0].ToString();


            if (!string.IsNullOrEmpty(meterName))
            {

                assetID = (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetID(meterName);
                AssetID = Convert.ToString(assetID);
            }

            return Json(AssetID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method will call 'GetAssetTypes' to fetch data from facoty and bind all the meterIDs to model 'FDFilesModel1'.
        /// This method is used to bind data to AssetID and name Dropdown in FDInquiry page.It will return the data as list
        /// Modified:Prita()
        /// </summary>
        /// <param name="FileType"></param>
        /// <param name="SearchBy"></param>
        /// <param name="TypedText"></param>
        /// <returns></returns>
        public ActionResult GetAssetIds(string FileType, int SearchBy = 2, string TypedText = "")
        {
            List<FDFilesModel1> FDModel = new List<FDFilesModel1>();
            FDModel = GetAssetTypes(FileType, SearchBy, TypedText);
            var result = FDModel;

            List<SelectListItem> lstAssets = new List<SelectListItem>();

            if (result != null)
            {
                foreach (var k in result)
                {
                    if (k.Text != null && k.Text != null)
                    {
                        lstAssets.Add(new SelectListItem { Text = k.Text.ToString(), Value = k.Value.ToString() });
                    }

                }
            }

            return Json(lstAssets, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Description:GetAssetTypes will call GetAllAssetTypes in factory method to get all the meters matching the typed character in the autocomplete box and filter the record accordingly
        /// Modified:Prita()
        /// </summary>
        /// <param name="Ftype"></param>
        /// <param name="Searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<FDFilesModel1> GetAssetTypes(string Ftype, int Searchby = 2, string typedtext = "")
        {

            return (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllAssetTypes(Ftype, CurrentCity.Id, CurrentCity.InternalName, Searchby, typedtext);

        }

        /// <summary>
        /// This will get list of comments that can be selected for canceling a file download. These comments will be visible in the grid as drop down when a file is selected to be canceled
        /// Modified:Prita()
        /// </summary>
        /// <returns></returns>
        public List<FDFilesModel1> GetAllComments()
        {
            return (new FileUploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllJobComments();
        }
        #endregion

    }
}
