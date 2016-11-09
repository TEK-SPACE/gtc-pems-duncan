using System;
using Kendo.Mvc.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Entities.PayByCell;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using Duncan.PEMS.Business.PayByCell;
using System.Web.Script.Serialization;
using System.Web;
using Kendo.Mvc;
using Duncan.PEMS.Business.Exports;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class PayByCellController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private readonly SecurityManager _secMgr = new SecurityManager();
   
        public ActionResult SpaceMgmt()
        {
           // ViewBag.CurrentCityID = CurrentCity.Id;
            return View();

        }

        /// <summary>
        /// Description:This method will call 'GetVendors' method of factory class to fill vendor's dropdown in Spacemgmt.cshtml from data in database
        /// Modified:Prita(6-Aug-2014 to 12-Aug-2014)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVendors()
        {

            var Files = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVendors(CurrentCity.Id);
            return Json(Files, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        ///Description:This method will take the location type selected by user and call 'GetLocationTypeId' method of factory class to fill Area,Street,Zone and SubUrb
        ///             dropdown in Spacemgmt.cshtml's filter panel.
        /// Modified: Prita(6-Aug-2014 to 12-Aug-2014)
        /// </summary>
        /// <param name="locationType"></param>
        /// <returns></returns>
        public ActionResult GetLocs(string locationType)
        {

            var details = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId(locationType, CurrentCity.Id);
            return Json(details, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Description:This method will accept data from the filter panel and call 'GetSpaceManagementsummary' factory method to fill the grid in Spacemgmt.cshtml
        /// Modified: Prita(6-Aug-2014 to 12-Aug-2014)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="VendorID"></param>
        /// <param name="AreaID"></param>
        /// <param name="ZoneID"></param>
        /// <param name="SubUrbID"></param>
        /// <param name="Street"></param>
        /// <returns></returns>

        public ActionResult GetPayByCellSummary([DataSourceRequest] DataSourceRequest request, string VendorID, string AreaID, string ZoneID, string SubUrbID, string Street)
        {
            IEnumerable<PayByCellModel> FileSummaryResult = Enumerable.Empty<PayByCellModel>();
            var FileSummaryResult_raw = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSpaceManagementsummary(request, CurrentCity.Id, VendorID, AreaID, ZoneID, SubUrbID, Street);

            FileSummaryResult = from r in FileSummaryResult_raw
                                select new PayByCellModel
                                {
                                    MeterID = r.MeterID,
                                    AreaName = r.AreaName,
                                    Street = r.Street,
                                    ZoneName = r.ZoneName,
                                    SubUrbName = r.SubUrbName,
                                    VendorID = r.VendorID,
                                    VendorName = r.VendorName,
                                    AreaID = r.AreaID
                                };
            var result = FileSummaryResult.ToList();
            DataSourceResult finalResult = result.ToDataSourceResult(request);
            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SpaceManagement_Update([DataSourceRequest] DataSourceRequest request, PayByCellModel[] GridRowdata)
        {
            //var UserId = WebSecurity.CurrentUserId;
            if (GridRowdata != null && ModelState.IsValid)
            {
                (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateFile_Active(GridRowdata, CurrentCity.Id);
                ViewData["Result"] = "Success";
            }
            // return Json(new { status = "MSG"},"text/plain");
            return Json(ModelState.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #region exportoptions
        public FileResult ExportToExcel_SpacMgmt([DataSourceRequest]DataSourceRequest request, string VendorID, string AreaID, string ZoneID, string SubUrbID, string Street, string VendorName, string AreaName, string ZoneName, string SuburbName)
        {
            var items = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSpaceManagementsummary(request, CurrentCity.Id, VendorID, AreaID, ZoneID, SubUrbID, Street);
            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "Vendor", Value = VendorName });
            filters.Add(new FilterDescriptor { Member = "Area", Value = AreaName });
            filters.Add(new FilterDescriptor { Member = "Zone", Value = ZoneName });
            filters.Add(new FilterDescriptor { Member = "SubUrb", Value = SuburbName });
            filters.Add(new FilterDescriptor { Member = "Street", Value = Street });
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetPayByCellSummary", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "PayByCell_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");
        }

        public FileResult ExportToCsv_SpacMgmt([DataSourceRequest] DataSourceRequest request, string VendorID, string AreaID, string ZoneID, string SubUrbID, string Street, string VendorName, string AreaName, string ZoneName, string SuburbName)
        {
            var items = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSpaceManagementsummary(request, CurrentCity.Id, VendorID, AreaID, ZoneID, SubUrbID, Street);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetPayByCellSummary", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "PayByCell_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdf_SpacMgmt([DataSourceRequest] DataSourceRequest request, string VendorID, string AreaID, string ZoneID, string SubUrbID, string Street, string VendorName, string AreaName, string ZoneName, string SuburbName)
        {

            var items = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSpaceManagementsummary(request, CurrentCity.Id, VendorID, AreaID, ZoneID, SubUrbID, Street);
            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "Vendor", Value = VendorName });
            filters.Add(new FilterDescriptor { Member = "Area", Value = AreaName });
            filters.Add(new FilterDescriptor { Member = "Zone", Value = ZoneName });
            filters.Add(new FilterDescriptor { Member = "SubUrb", Value = SuburbName });
            filters.Add(new FilterDescriptor { Member = "Street", Value = Street });
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetPayByCellSummary", CurrentCity.Id, filters, 1);
            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "PayByCell_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
    
        #endregion

    }
}