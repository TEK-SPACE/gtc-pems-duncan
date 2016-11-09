using System.Collections.Generic;
using Kendo.Mvc.Extensions;
using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using iTextSharp.text;
using Duncan.PEMS.Entities.PayByCell;
using System.Web;
using Duncan.PEMS.Business.PayByCell;
using System.Web.Script.Serialization;
using Kendo.Mvc;
using Duncan.PEMS.Business.Exports;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{

    /// <summary>
    /// Controller to allow control of users for the administrative function.  This
    /// inherits from the shared <see cref="shared.Controllers.UsersController"/>.  Do
    /// not modify this controller.
    /// </summary>
    public class PayByCellController : PemsController
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
            //ViewBag.CurrentCityID = CurrentCity.Id;
            return View();

        }

        /// <summary>
        /// Description: This method will populate Vendor names in the autocomplete control, based on the selection of the Vendor ID through the Filter Panel in Index page.        
        /// Modified:Prita()
        /// </summary>
        /// <param name="VendorID"></param>
        /// <returns></returns>
        public ActionResult GetVendorName(string VendorID)
        {
            string[] VendorName = VendorID.Split(',');

            string vendorName = string.Empty;

            string meterId = VendorName[0].ToString();


            if (!string.IsNullOrEmpty(meterId))
            {
                int outNum;
                bool isNum = int.TryParse(meterId, out outNum);
                if (isNum)
                {
                    vendorName = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVendorName(outNum);
                }

            }

            return Json(vendorName, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:In Index page filter panel when vendor Name is selected, respective vendor name will be populated on the corresponding autocomplete accordingly by this method
        /// Modified:Prita()
        /// </summary>
        /// <param name="VendorID"></param>
        /// <returns></returns>
        public ActionResult GetVendorID(string VendorName)
        {
            string[] AssetName = VendorName.Split(',');

            int vendorID = 0;
            string VendorID = string.Empty;

            string meterName = AssetName[0].ToString();


            if (!string.IsNullOrEmpty(meterName))
            {

                vendorID = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVendorID(meterName);
                VendorID = Convert.ToString(vendorID);
            }

            return Json(VendorID, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method will call 'GetAssetTypes' method and retrieve the data in a list. This model is then used to bind Filter panel vendor ID and name autocomplete control of index page
        /// Modeified:Prita()
        /// </summary>
        /// <param name="TypedText"></param>
        /// <returns></returns>
        public ActionResult GetVendorIds( string TypedText)
        {
            List<PayByCellModelAdmin> FDModel = new List<PayByCellModelAdmin>();
            FDModel = GetAssetTypes(TypedText);
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
       /// It will return vendor IDs and names fetched by factory method 'GetAllVendors' from database.
       /// Modeified:Prita()
       /// </summary>
       /// <param name="typedtext"></param>
       /// <returns></returns>

        public List<PayByCellModelAdmin> GetAssetTypes(string typedtext)
        {

            return (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllVendors(CurrentCity.Id, typedtext);

        }

        /// <summary>
        /// This method will send the factory method 'GetPayByCellSummary' with filter panel data and get the output of the query to fill the grid in index page
        /// Modeified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ActionResult GetPayByCellSummary([DataSourceRequest]DataSourceRequest request)
        {
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "FileDownload");

            IEnumerable<PayByCellModelAdmin> PayByCellSummaryResult = Enumerable.Empty<PayByCellModelAdmin>();
            int total = 0;
            var PayByCellSummaryResult_raw = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPayByCellSummary(request, out total);

            PayByCellSummaryResult = from r in PayByCellSummaryResult_raw
                                     select new PayByCellModelAdmin
                                {
                                    VendorID = r.VendorID,
                                    VendorName = r.VendorName,
                                    FileTypeName = r.FileTypeName,
                                    CreatedOnDate = r.CreatedOnDate,
                                    CreatedByID = r.CreatedByID,
                                    CreatedByName = r.CreatedByName,
                                    CustomerID = r.CustomerID, //** added by sai
                                    Deprecated=r.Deprecated
                                                                      
                                };
            var result = new DataSourceResult
            {
                Data = PayByCellSummaryResult,
                Total = total,
            };

            return new LargeJsonResult() { Data = result, MaxJsonLength = int.MaxValue };

        }
        /// <summary>
        /// Description:This method will check whether the vendor inthe grid is active.If active,then onclick edit page will dispaly will corresponding data for the vendor.
        /// Modified:Prita()
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="Active"></param>
        /// <returns></returns>
        public ActionResult EditPayByCell(int vendorID, bool Active, int CustID)
        {
            PayByCellModelAdmin model = new PayByCellModelAdmin();

            if (Active == false) { return RedirectToAction("Index", "PayByCell", new { DataSourceRequest = model }); }
            else
            {

                var Usable = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckIfUsable(vendorID);
                if (Usable == false)
                {
                    var html = "This vendor is no longer available.";
                    return Content(html, "text/html");
                }

                var vendorName = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVendorName(vendorID);
                model.VendorID = vendorID;
                model.VendorName = vendorName;
                model.CustomerID = CustID;
                return View(model);
            };
        }
    #endregion

        #region Add New Vendor
        public ActionResult AddPayByCell()
        {
            ViewData["MaxVendorID"] = GetMaxVendorID();
            return View();
        }

        /// <summary>
        ///Description:This method will call the factory method 'GetMaxVendorID' that will generate the next VendorID to create a new vendor
        ///Modified:Prita()
        /// </summary>
        /// <returns></returns>
        public int GetMaxVendorID()
        {
            return (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetMaxVendorID();
        }

        public ActionResult GetCustomerList()
        {
            IQueryable<PayByCellModelAdmin> Result = Enumerable.Empty<PayByCellModelAdmin>().AsQueryable();

            Result = new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString()).GetCustomerList(CurrentCity.Id);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
 
 
        /// <summary>
        ///Description: This method will add the vendor records and the customer and duncan property to the database by passing the data to 'SaveVendorDetails' method of factory 
        ///Modified: Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="DuncanPropGriditems"></param>
        /// <param name="CustPropGriditems"></param>
        /// <param name="vendorID"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddNewVendorRecords([DataSourceRequest] DataSourceRequest request, DuncanPropertyModel[] DuncanPropGriditems, CustomerPropertyModel[] CustPropGriditems, int vendorID, string VendorName, int CustID)
        {
            try{
                //var VendorExists = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckIfExist(vendorID, VendorName, CurrentCity.Id);
                var VendorExists = (new PayByCellFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CheckIfExist(vendorID, VendorName, CustID);
                if (VendorExists == true)
                {
                    var html = "This vendor Already Exist.";
                    return Json(html, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //(new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveVendorDetails(vendorID, VendorName, CurrentCity.Id, DuncanPropGriditems, CustPropGriditems);
                    (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveVendorDetails(vendorID, VendorName, CustID, DuncanPropGriditems, CustPropGriditems);
                    var html1 = "Record saved successfully";
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

        #region Edit Vendor

        /// <summary>
        /// Description:This method will use the data in both the grid of 'EditPayByCell.cshtml' to edit existing data in the database for the particular vendor
        /// Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="DuncanPropGriditems"></param>
        /// <param name="CustPropGriditems"></param>
        /// <param name="vendorID"></param>
        /// <param name="VendorName"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditVendorRecords([DataSourceRequest] DataSourceRequest request, DuncanPropertyModel[] DuncanPropGriditems, CustomerPropertyModel[] CustPropGriditems, int vendorID, string VendorName)
        {
            try
            {
            (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).UpdateVendorDetails(vendorID, VendorName, CurrentCity.Id, DuncanPropGriditems, CustPropGriditems);
             var html1 = "Record updated successfully";
             return Json(html1, JsonRequestBehavior.AllowGet);       
            }
         catch (Exception ex)
            {
                var html = ex.Message;
                return Json(html, JsonRequestBehavior.AllowGet);
            }
        }
       

        public JsonResult GetFilterValues(string TypedText = "")
        {
            var PayByCellFactory = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString()));


            var VendorIds = PayByCellFactory.GetAllVendors(CurrentCity.Id, TypedText);


            var filterValues = new { VendorIds };

            // return as JSON
            return Json(filterValues, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Description:This method will call the factory method to deactivate the VendorID selected.
        /// Modified:Prita()
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public ActionResult DepricateVendorRecords(int vendorID)
        {
            try
            {
                (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).DepricateVendorRecords(vendorID);
                var html1 = "This vendor is deprecated and cannot be used or modified any more.";
                return Json(html1, JsonRequestBehavior.AllowGet); 
            }
            catch (Exception ex)
            {
                var html = ex.Message;
                return Json(html, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Description:This will call the factory method to fetch ripnetproperties from database for that particular vendor. The data fetched will be displayed in the 'Duncan Property' grid of EditPayByCell page
        /// Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public ActionResult GetDuncanProperties([DataSourceRequest] DataSourceRequest request, int vendorid, int custID)
        {
            IEnumerable<DuncanPropertyModel> DuncanProperties = Enumerable.Empty<DuncanPropertyModel>();
            int total = 0;
            var DuncanPropertiesList_raw = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDuncanProp(custID, vendorid);
            //var DuncanPropertiesList_raw = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDuncanProp(CurrentCity.Id, vendorid);

            DuncanProperties = from r in DuncanPropertiesList_raw
                                     select new DuncanPropertyModel
                                {
                                    KeyText=r.KeyText,
                                    ValueText=r.ValueText,
                                    KeyTextOld=r.KeyTextOld,
                                    ValueTextOld=r.ValueTextOld
                                  
                                };

            return new LargeJsonResult() { Data = DuncanProperties.ToDataSourceResult(request), MaxJsonLength = int.MaxValue };

        }

        /// <summary>
        /// Description:This will call the factory method to fetch ripnetproperties from database for that particular vendor. The data fetched will be displayed in the 'Customer Property' grid of EditPayByCell page
        /// Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        public ActionResult GetCustomerProperties([DataSourceRequest] DataSourceRequest request, int vendorid, int custID)
        {
            IEnumerable<CustomerPropertyModel> CustomerProperties = Enumerable.Empty<CustomerPropertyModel>();
            int total = 0;
           // var CustPropertiesList_raw = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCustomerProp(CurrentCity.Id, vendorid);
            var CustPropertiesList_raw = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCustomerProp(custID, vendorid);

            CustomerProperties = from r in CustPropertiesList_raw
                               select new CustomerPropertyModel
                               {
                                   KeyText = r.KeyText,
                                   ValueText = r.ValueText,
                                   KeyTextOld=r.KeyTextOld,
                                    ValueTextOld=r.ValueTextOld

                               };
          
            return new LargeJsonResult() { Data = CustomerProperties.ToDataSourceResult(request), MaxJsonLength = int.MaxValue };

        }
        #endregion
        
        #region export options
        public FileResult ExportToExcel_PBC([DataSourceRequest]DataSourceRequest request)
        {
            int total = 0;
            var items = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPayByCellSummary(request, out total);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetPayByCellSummary", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "PayByCellVendorsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToCsv_PBC([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            int total = 0;
            var items = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPayByCellSummary(request, out total);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetPayByCellSummary", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "PayByCellVendorsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdf_PBC([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            int total = 0;
            var items = (new PayByCellFactory(Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPayByCellSummary(request, out total);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetPayByCellSummary", CurrentCity.Id, filters, 1);
            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "PayByCellVendorsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        #endregion
        
        #region Convert List to JSon 
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
    }
}
