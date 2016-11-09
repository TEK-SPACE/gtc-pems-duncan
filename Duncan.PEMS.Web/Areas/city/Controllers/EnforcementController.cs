using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.Enforcement;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Business.Enforcement;
using Duncan.PEMS.Business.Exports;
using Kendo.Mvc.Extensions;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NLog;
using System.IO;
using System.Web.UI.WebControls;
using Kendo.Mvc;
using Duncan.PEMS.DataAccess.OracleDB;  //** Step 1 


namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class EnforcementController : PemsController
    {

        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private const string AutoIssueSvcInternalSessionKey = "Sz8tTtrh4CYkD88jXOjp6LpFAXU11BlGYFYdGkvK0EU=";
        private int citMaxLen;


        #region Enforcement Summary Action


        public ActionResult Index()
        {
            EnforcementModel objModel = new EnforcementModel();
            return View(objModel);
        }

        #endregion Index Action

        #region Enforcement Details Action

        /// <summary>
        ///  Description: This Method will return view 
        ///   ModifiedBy: Santhosh  (19/Feb/2014 - 27/Feb/2014) 
        /// </summary>
        /// <returns></returns>
        public ActionResult EnforceDet()
        {
            return View();
        }

        /// <summary>
        ///  Description: This will add enforcement note to database
        ///   ModifiedBy: Santhosh  (25/AUG/2014) 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="FileComment"></param>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Submit(HttpPostedFileBase files, string FileComment, long CitationID)
        {
            // HttpPostedFileBase files = Request.Files[0];
            List<string> errors = new List<string>();
            string errormsg = "";
            try
            {
                if (files != null)
                {


                    var UploadBy = CurrentCity.Id;
                    var fileName = Path.GetFileName(files.FileName);
                    BinaryReader b = new BinaryReader(files.InputStream);
                    uint filesize = Convert.ToUInt32(files.InputStream.Length);
                    byte[] filedata = new byte[filesize];
                    Array.Copy(b.ReadBytes(Convert.ToInt32(filesize)), 0, filedata, 0, filesize);
                    var fileExt = Path.GetExtension(files.FileName).Substring(1).ToUpper();

                    string MultimediaNoteDataType = "";

                    var ContentType = files.ContentType;

                    if (ContentType.Contains("image"))
                    {
                        MultimediaNoteDataType = "mmPicture";
                    }
                    else if (ContentType.Contains("audio"))
                    {
                        MultimediaNoteDataType = "mmAudio";
                    }
                    else if (ContentType.Contains("video"))
                    {
                        MultimediaNoteDataType = "mmVideo";
                    }
                    else
                        MultimediaNoteDataType = "mmNone";


                    int suceess = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateFile(CitationID, filedata, UploadBy, MultimediaNoteDataType, FileComment);


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

        /// <summary>
        ///  Description: This Method will retrive list of attachment files for particular CitationID
        ///   ModifiedBy: Santhosh  (19/Feb/2014 - 27/Feb/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        public ActionResult GetNotes([DataSourceRequest] DataSourceRequest request, int CitationID)
        {


            IEnumerable<NoteModel> enforcementnoteResult = Enumerable.Empty<NoteModel>();

            var enforcementNoteResult_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetNotes(CitationID);
            enforcementnoteResult = from r in enforcementNoteResult_raw
                                    select new NoteModel
                                    {
                                        ParkNoteId = r.ParkNoteId,
                                        ParkingId = r.ParkingId,
                                        NoteDateTime = r.NoteDateTime,
                                        NotesMemo = r.NotesMemo,
                                        MultimediaNoteDataType = r.MultimediaNoteDataType,
                                        CustomerId = r.CustomerId,
                                        CustomerName = r.CustomerName
                                    };
            DataSourceResult finalResult = enforcementnoteResult.ToDataSourceResult(request);

            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        }


        /// <summary>
        ///  Description: This Method will retrive attachment file
        ///   ModifiedBy: Santhosh  (19/Feb/2014 - 27/Feb/2014) 
        /// </summary>
        /// <param name="ParkNoteId"></param>
        /// <returns></returns>
        public ActionResult NoteAttachment(int ParkNoteId)
        {
            var enforcementNoteResult_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).NoteAttachment(ParkNoteId);
            byte[] MultimediaNoteData = enforcementNoteResult_raw[0].MultimediaNoteData;
            string MultimediaNoteDataType = enforcementNoteResult_raw[0].MultimediaNoteDataType;

            if (MultimediaNoteData != null)
            {
                if (MultimediaNoteDataType == "mmPicture")
                    return new FileContentResult(MultimediaNoteData, "image/jpeg");
                else if (MultimediaNoteDataType == "mmAudio")
                    return new FileContentResult(MultimediaNoteData, "audio/mp4");
                else if (MultimediaNoteDataType == "mmVideo")
                    return new FileContentResult(MultimediaNoteData, "video/mp4");
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///  Description: This function is used to export the data to excel
        ///  Modified By: Sairam  (4th Nov 2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="gridType"></param>
        /// <param name="customerId"></param>
        /// <param name="assetType"></param>
        /// <returns></returns>
        public FileResult ExportToExcel([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType)
        {

            int total = 0;
            //var items = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementsummary_Export(request, startDate, endDate, licensePlate, licenseState, licenseType, vin, classes, prefix, startCitationNo, endCitationNo, suffix, code, officerName, agency, beat, meterNo, officerID, vioDesc, parkingStatus, parkingType, CurrentCity.Id, out total);
            var items = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementsummary(startDate, endDate, licensePlate, licenseState, licenseType, vin, classes, prefix, startCitationNo, endCitationNo, suffix, code, officerName, agency, beat, meterNo, officerID, vioDesc, parkingStatus, parkingType, CurrentCity.Id);

            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "Citation Start Date", Value = startDate });
            filters.Add(new FilterDescriptor { Member = "Citation End Date", Value = endDate });
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);


            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetEnforcementsummary", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Enforcement_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");

        }

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType)
        {
            int total = 0;
            var items = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementsummary(startDate, endDate, licensePlate, licenseState, licenseType, vin, classes, prefix, startCitationNo, endCitationNo, suffix, code, officerName, agency, beat, meterNo, officerID, vioDesc, parkingStatus, parkingType, CurrentCity.Id);

            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "Citation Start Date", Value = startDate });
            filters.Add(new FilterDescriptor { Member = "Citation End Date", Value = endDate });
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetEnforcementsummary", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Enforcement_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType)
        {

            int total = 0;
            var items = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementsummary(startDate, endDate, licensePlate, licenseState, licenseType, vin, classes, prefix, startCitationNo, endCitationNo, suffix, code, officerName, agency, beat, meterNo, officerID, vioDesc, parkingStatus, parkingType, CurrentCity.Id);

            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "Citation Start Date", Value = startDate });
            filters.Add(new FilterDescriptor { Member = "Citation End Date", Value = endDate });
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetEnforcementsummary", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Enforcement_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }



        /// <summary>
        ///  Description: This will change the status of a citation that has been previously been voided to active
        ///   ModifiedBy: Santhosh  (25/aUG/2014)  
        /// </summary>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        public int SetREINSTATE(long CitationID)
        {
            int Changes = 0;
            Changes = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SetREINSTATE(CitationID);
            return Changes;
        }

        /// <summary>
        ///  Description: This will change the status of an active citation to voided
        ///   ModifiedBy: Santhosh  (25/AUG/2014)  
        /// </summary>
        /// <param name="CitationID"></param>
        /// <returns></returns>
        public int SetVOID(long CitationID)
        {
            int Changes = 0;
            //AutoIssueSvcNewOrleans.AutoISSUEHostService svc = new AutoIssueSvcNewOrleans.AutoISSUEHostService();
            
            Changes = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SetVOID(CitationID);
            return Changes;
        }
        #endregion

        #region Filters

        #region Enforcement VehLic No Method

        /// <summary>
        /// Description:This method is used to populate the LicenseNo for enforcement page for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="vehLICNoIs"></param>
        /// <returns></returns>
        public JsonResult EnforcementVehLicNo(string vehLICNoIs)
        {
            var assets = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementVehLicNo(CurrentCity.Id, vehLICNoIs);
            return Json(assets, JsonRequestBehavior.AllowGet);
        }


        #endregion Enforcement VehLic No Method

        #region Enforcement License State Method


        //public JsonResult EnforcementLicenseState()
        //{
        //    var assets = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementLicenseState(CurrentCity.Id);
        //    return Json(assets, JsonRequestBehavior.AllowGet);
        //}
        #endregion Enforcement License State Method

        /// <summary>
        /// Enforcement license type
        /// </summary>
        /// <returns></returns>

        //public JsonResult EnforcementLicenseType()
        //{
        //    var licenseType = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementLicenseType(CurrentCity.Id);
        //    return Json(licenseType, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Description:This method is used to populate the VehicleVIN for enforcement page for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <returns></returns>
        public JsonResult EnforcementVehicleVIN(string vehVINIs)
        {
            var vehicleVIN = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementVehicleVIN(CurrentCity.Id, vehVINIs);
            return Json(vehicleVIN, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to populate the IssueNumber Prefix items for the typed prefix in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <returns></returns>
        public JsonResult EnforcementIssueNumberPrefix(string prefixIs)
        {
            var issueNoPrefix = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementIssueNumberPrefix(CurrentCity.Id, prefixIs);
            return Json(issueNoPrefix, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Enforcement Agency
        /// </summary>
        /// <returns></returns>

        //public JsonResult EnforcementAgency()
        //{
        //    var agency = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementAgency(CurrentCity.Id);
        //    return Json(agency, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Description:This method is used to populate the IssueNumbers for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <returns></returns>
        public JsonResult EnforcementIssueNumber(string citIssueNumberIs)
        {
            var issueNumber = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementIssueNumber(CurrentCity.Id, citIssueNumberIs);
            var issueNumber_final = from r in issueNumber
                                    select new EnforcementModel
                                    {
                                        IssueNo_final = Convert.ToString(r.IssueNo),
                                    };
            return Json(issueNumber_final, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Enforcement Beat
        /// </summary>
        /// <returns></returns>

        //public JsonResult EnforcementBeat()
        //{
        //    var Beat = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementBeat(CurrentCity.Id);
        //    return Json(Beat, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult EnforcementStatus()
        //{
        //    var Status = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementStatus(CurrentCity.Id);
        //    return Json(Status, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult EnforcementClass()
        //{
        //    var classes = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementClass(CurrentCity.Id);
        //    return Json(classes, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult EnforcementCode()
        //{
        //    var code = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementCode(CurrentCity.Id);
        //    return Json(code, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult EnforcementVioDesc()
        //{
        //    var vioDesc = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementVioDesc(CurrentCity.Id);
        //    return Json(vioDesc, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult EnforcementType()
        //{
        //    var type = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementType(CurrentCity.Id);
        //    return Json(type, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Description:This method is used to populate the IssueNumberSuffix for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <returns></returns>
        public JsonResult EnforcementIssueNumberSuffix(string suffixIs)
        {
            var issueNumberSuffix = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementIssueNumberSuffix(CurrentCity.Id, suffixIs);
            return Json(issueNumberSuffix, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to populate the MeterID for enforcement page in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <returns></returns>
        public JsonResult EnforcementMeterId(string meterIDIs)
        {
            var meterId = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementMeterId(CurrentCity.Id, meterIDIs);
            return Json(meterId, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Enforcement officer id
        /// </summary>
        /// <returns></returns>

        //public JsonResult EnforcementOfficerId()
        //{
        //    var officeId = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementOfficerId(CurrentCity.Id);
        //    return Json(officeId, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Description:This method is used to populate the Street for enforcement page for the typed text in the autocomplete text box.
        /// Modified By: Sairam on June 19th 2014
        /// </summary>
        /// <param name="streetIs"></param>
        /// <returns></returns>
        //public JsonResult EnforcementStreet(string streetIs)
        //{
        //    var street = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementStreet(CurrentCity.Id, streetIs);
        //    return Json(street, JsonRequestBehavior.AllowGet);
        //}



        //public JsonResult EnforcementOfficerNames()
        //{
        //    var OfficerName = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementOfficerNames(CurrentCity.Id);
        //    return Json(OfficerName, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult EnforcementCitationDetails(long citationID)
        {
            var enforceModel = new EnforcementModel();
            var enforcementCitation = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).EnforcementCitationDetails(citationID);
            enforceModel.IssueNo = enforcementCitation[0].IssueNo;
            enforceModel.IssueDateTime = enforcementCitation[0].IssueDateTime;
            enforceModel.UnitSerial = enforcementCitation[0].UnitSerial;
            enforceModel.Beat = enforcementCitation[0].Beat;
            enforceModel.OfficerID = enforcementCitation[0].OfficerID;
            enforceModel.OfficerName = enforcementCitation[0].OfficerName;
            enforceModel.Agency = enforcementCitation[0].Agency;
            enforceModel.VehLicNo = enforcementCitation[0].VehLicNo;
            enforceModel.VehLicState = enforcementCitation[0].VehLicState;
            enforceModel.VEHLICTYPE = enforcementCitation[0].VEHLICTYPE;
            enforceModel.VehMake = enforcementCitation[0].VehMake;
            enforceModel.VehModel = enforcementCitation[0].VehModel;
            enforceModel.VehBodyStyle = enforcementCitation[0].VehBodyStyle;
            enforceModel.VEHCHECKDIGIT = enforcementCitation[0].VEHCHECKDIGIT;
            enforceModel.VehColor1 = enforcementCitation[0].VehColor1;
            enforceModel.VehColor2 = enforcementCitation[0].VehColor2;
            enforceModel.VehLicExpDate = enforcementCitation[0].VehLicExpDate;
            enforceModel.VehVIN = enforcementCitation[0].VehVIN;
            enforceModel.VehVIN4 = enforcementCitation[0].VehVIN4;
            enforceModel.LocBlock = enforcementCitation[0].LocBlock;
            enforceModel.LocCity = enforcementCitation[0].LocCity;
            enforceModel.LocCrossStreet1 = enforcementCitation[0].LocCrossStreet1;
            enforceModel.LocCrossStreet2 = enforcementCitation[0].LocCrossStreet2;
            enforceModel.LocDescriptor = enforcementCitation[0].LocDescriptor;
            enforceModel.LocDirection = enforcementCitation[0].LocDirection;
            enforceModel.LocLot = enforcementCitation[0].LocLot;
            enforceModel.LOCSIDEOFSTREET = enforcementCitation[0].LOCSIDEOFSTREET;
            enforceModel.LocState = enforcementCitation[0].LocState;
            enforceModel.LocStreet = enforcementCitation[0].LocStreet;
            enforceModel.LocSuburb = enforcementCitation[0].LocSuburb;
            enforceModel.LOCZIP = enforcementCitation[0].LOCZIP;
            enforceModel.MeterBayNo = enforcementCitation[0].MeterBayNo;
            enforceModel.MeterID = enforcementCitation[0].MeterID;
            enforceModel.MeterNo = enforcementCitation[0].MeterNo;
            enforceModel.PermitNo = enforcementCitation[0].PermitNo;
            enforceModel.Remark1 = enforcementCitation[0].Remark1;
            enforceModel.Remark2 = enforcementCitation[0].Remark2;
            enforceModel.Remark3 = enforcementCitation[0].Remark3;
            enforceModel.VioCode = enforcementCitation[0].VioCode;
            enforceModel.VioDescription1 = enforcementCitation[0].VioDescription1;
            enforceModel.VioFine = enforcementCitation[0].VioFine;
            enforceModel.VioLateFee1 = enforcementCitation[0].VioLateFee1;
            enforceModel.VioLateFee2 = enforcementCitation[0].VioLateFee2;
            enforceModel.ViolationDescription = enforcementCitation[0].ViolationDescription;
            enforceModel.ParkingId = enforcementCitation[0].ParkingId;
            ViewData["citationUnqKey"] = enforcementCitation[0].ParkingId;
            //ViewData["citationUnqKey"] = 5;
            ViewData["cno"] = citationID;
            enforceModel.myCitationID = citationID;
            return View("EnforceDet", enforceModel);

        }

        public ActionResult GetEnforcementViolation([DataSourceRequest] DataSourceRequest request, int CitationID)
        {
            IEnumerable<EnforcementModel> Violation = Enumerable.Empty<EnforcementModel>();
            var Violation_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementViolation(CitationID);

            Violation = from r in Violation_raw
                        select new EnforcementModel
                        {
                            VioCode = r.VioCode,
                            VioDescription1 = r.VioDescription1,
                            VioFine = r.VioFine,
                            VioLateFee1 = r.VioLateFee1,
                            VioLateFee2 = r.VioLateFee2,

                        };

            DataSourceResult finalResult = Violation.ToDataSourceResult(request);

            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };

        }

        public ActionResult GetEnforcementRecordHistory([DataSourceRequest] DataSourceRequest request, int CitationID)
        {
            IEnumerable<EnforcementModel> AuditHistory = Enumerable.Empty<EnforcementModel>();
            var AuditHistory_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementRecordHistory(CitationID);

            AuditHistory = from r in AuditHistory_raw
                           select new EnforcementModel
                           {
                               OfficerName = r.OfficerName,
                               StatusValue = r.StatusValue,
                               StatusDateTime = r.StatusDateTime,
                               StatusReason = r.StatusReason,
                               OfficerID = r.OfficerID,

                           };

            DataSourceResult finalResult = AuditHistory.ToDataSourceResult(request);

            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };

        }

        //public ActionResult GetEnforcementExportHistory([DataSourceRequest] DataSourceRequest request, int CitationID)
        //{
        //    IEnumerable<EnforcementModel> AuditHistory = Enumerable.Empty<EnforcementModel>();
        //    var AuditHistory_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementExportHistory(CitationID);
        //    AuditHistory = from r in AuditHistory_raw
        //                   select new EnforcementModel
        //                   {
        //                       ExportDateTime = r.ExportDateTime,
        //                       ExportId = r.ExportId,
        //                       ExportType = r.ExportType,
        //                       Count = r.Count
        //                   };
        //    DataSourceResult finalResult = AuditHistory.ToDataSourceResult(request);

        //    return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        //}



        /// <summary>
        /// Description: This method fetches data for all the search criteria filters displayed in the Enforcement Summary (AI Enquiry) screen;
        /// Modified By: Santhosh (May 21st 2014);
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFilterValues()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));

            //var EnforcementLicenseStateassets = enforcementFactory.EnforcementLicenseState(CurrentCity.Id);



           // var licenseTypes = enforcementFactory.EnforcementLicenseType(CurrentCity.Id);

            //var agencys = enforcementFactory.EnforcementAgency(CurrentCity.Id);

            //var Beats = enforcementFactory.EnforcementBeat(CurrentCity.Id);

            //var StatusA = enforcementFactory.EnforcementStatus(CurrentCity.Id);

            //var officeIds = enforcementFactory.EnforcementOfficerId(CurrentCity.Id);

           // var classesa = enforcementFactory.EnforcementClass(CurrentCity.Id);

            //var codes = enforcementFactory.EnforcementCode(CurrentCity.Id);

          //  var OfficerNames = enforcementFactory.EnforcementOfficerNames(CurrentCity.Id);

            //var vioDescs = enforcementFactory.EnforcementVioDesc(CurrentCity.Id);

            //var types = enforcementFactory.EnforcementType(CurrentCity.Id);

            // Concatenate the data into an anonymous typed object            
            //var filterValues = new { EnforcementLicenseStateassets, licenseTypes, agencys, Beats, StatusA, classesa, codes, vioDescs, types, officeIds, OfficerNames };
            var filterValues = new {};

            // return as JSON
            return Json(filterValues, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementOfficerNames()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var OfficerNames = enforcementFactory.EnforcementOfficerNames(CurrentCity.Id);
            return Json(OfficerNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementOfficerId()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var officeIds = enforcementFactory.EnforcementOfficerId(CurrentCity.Id);
            return Json(officeIds, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementLicenseState()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var EnforcementLicenseStateassets = enforcementFactory.EnforcementLicenseState(CurrentCity.Id);
            return Json(EnforcementLicenseStateassets, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementLicenseType()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var licenseTypes = enforcementFactory.EnforcementLicenseType(CurrentCity.Id);
            return Json(licenseTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementAgency()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var agencys = enforcementFactory.EnforcementAgency(CurrentCity.Id);
            return Json(agencys, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementBeat()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var Beats = enforcementFactory.EnforcementBeat(CurrentCity.Id);
            return Json(Beats, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementStatus()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var StatusA = enforcementFactory.EnforcementStatus(CurrentCity.Id);
            return Json(StatusA, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementClass()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var classesa = enforcementFactory.EnforcementClass(CurrentCity.Id);
            return Json(classesa, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementCode()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var codes = enforcementFactory.EnforcementCode(CurrentCity.Id);
            return Json(codes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementVioDesc()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var vioDescs = enforcementFactory.EnforcementVioDesc(CurrentCity.Id);
            return Json(vioDescs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnforcementType()
        {
            var enforcementFactory = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));
            var types = enforcementFactory.EnforcementType(CurrentCity.Id);
            return Json(types, JsonRequestBehavior.AllowGet);
        }
        
        

        #endregion Filters

        /// <summary>
        /// Description:This method is used to populate the Grid with the Enforcement Summary details.
        /// Modified By: Sairam on June 26th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="licensePlate"></param>
        /// <param name="licenseState"></param>
        /// <param name="licenseType"></param>
        /// <param name="vin"></param>
        /// <param name="classes"></param>
        /// <param name="prefix"></param>
        /// <param name="startCitationNo"></param>
        /// <param name="endCitationNo"></param>
        /// <param name="suffix"></param>
        /// <param name="code"></param>
        /// <param name="officerName"></param>
        /// <param name="agency"></param>
        /// <param name="beat"></param>
        /// <param name="meterNo"></param>
        /// <param name="officerID"></param>
        /// <param name="vioDesc"></param>
        /// <param name="parkingStatus"></param>
        /// <param name="parkingType"></param>
        /// <returns></returns>
        #region Get Enforcement Summary
        public ActionResult GetEnforcementsummary([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType)
        {

            IEnumerable<EnforcementModel> enforcementSummaryResult = Enumerable.Empty<EnforcementModel>();

            var enforcementSummaryResult_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementsummary(startDate, endDate, licensePlate, licenseState, licenseType, vin, classes, prefix, startCitationNo, endCitationNo, suffix, code, officerName, agency, beat, meterNo, officerID, vioDesc, parkingStatus, parkingType, CurrentCity.Id);


            enforcementSummaryResult = from r in enforcementSummaryResult_raw
                                       select new EnforcementModel
                                       {
                                           OfficerID = r.OfficerID,
                                           OfficerName = r.OfficerName,
                                           IssueNo = r.IssueNo,
                                           VehLicNo = r.VehLicNo,
                                           Beat = r.Beat,
                                           MeterNo = r.MeterNo,
                                           VioCode = r.VioCode,
                                           IssueDateTime_final = Convert.ToString(r.IssueDateTime),
                                           IssueNoWithPfxSfx = String.Concat(Convert.ToString(r.IssueNoPfx), GetIssueNoWithLeftPaddedZeroes(r.IssueNo), Convert.ToString(r.IssueNoSfx)),
                                           Status = r.Status,
                                           isAutoBindReqd = r.isAutoBindReqd
                                       };

            DataSourceResult finalResult = enforcementSummaryResult.ToDataSourceResult(request);

            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        }
        #endregion

        #region Export Enforcement Summary Data to Excel and PDF
        //public List<EnforcementModel> GetEnforcementsummary_Export(DataSourceRequest request, string startDate, string endDate, string licensePlate, string licenseState, string licenseType, string vin, string classes, string prefix, string startCitationNo, string endCitationNo, string suffix, string code, string officerName, string agency, string beat, string meterNo, string officerID, string vioDesc, string parkingStatus, string parkingType, out int total)
        //{

        //    List<EnforcementModel> enforceSummaryResultList = new List<EnforcementModel>();

        //    var enforcementSummaryResult_raw = (new EnforcementFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforcementsummary(startDate, endDate, licensePlate, licenseState, licenseType, vin, classes, prefix, startCitationNo, endCitationNo, suffix, code, officerName, agency, beat, meterNo, officerID, vioDesc, parkingStatus, parkingType, CurrentCity.Id);


        //    enforceSummaryResultList = (from r in enforcementSummaryResult_raw
        //                               select new EnforcementModel
        //                               {
        //                                   OfficerID = r.OfficerID,
        //                                   OfficerName = r.OfficerName,
        //                                   IssueNo = r.IssueNo,
        //                                   VehLicNo = r.VehLicNo,
        //                                   Beat = r.Beat,
        //                                   MeterNo = r.MeterNo,
        //                                   VioCode = r.VioCode,
        //                                   IssueDateTime_final = Convert.ToString(r.IssueDateTime),
        //                                   IssueNoWithPfxSfx = String.Concat(Convert.ToString(r.IssueNoPfx), GetIssueNoWithLeftPaddedZeroes(r.IssueNo), Convert.ToString(r.IssueNoSfx)),
        //                                   Status = r.Status,
        //                                   isAutoBindReqd = r.isAutoBindReqd
        //                               }).ToList();

        //    DataSourceResult finalResult = enforceSummaryResultList.ToDataSourceResult(request);

        //    return enforceSummaryResultList;
        //}
        #endregion

        /// <summary>
        /// Description: This method is used to display the Front and Back side of the Citation Image in the Citation Tab of enforcement page.
        /// Modified By: Sairam on June 14th 2014
        /// </summary>
        /// <param name="id"></param>
        /// <param name="structure"></param>
        /// <returns></returns>
        #region Citation Image
        public ActionResult CitationImage(long id, string structure)
        {
            var stream = new MemoryStream();
            try
            {
                //AutoIssueSvc.AutoISSUEHostService svc = new AutoIssueSvc.AutoISSUEHostService();

                AutoIssueSvcNewOrleans.AutoISSUEHostService svc = new AutoIssueSvcNewOrleans.AutoISSUEHostService();

                var image = svc.ApiGetCitationImage(structure, id, EnforcementController.AutoIssueSvcInternalSessionKey);
                stream = new MemoryStream(image.ToArray());
                stream.Position = 0;


            }
            catch (Exception ex)
            {
                //throw;
            }
            return new FileStreamResult(stream, "image/jpeg");
        }

        public string GetIssueNoWithLeftPaddedZeroes(long issueNo)
        {

            string issueNoIs = Convert.ToString(issueNo);
            return issueNoIs.PadLeft(citMaxLen, '0');
        }

        #endregion
    }



}