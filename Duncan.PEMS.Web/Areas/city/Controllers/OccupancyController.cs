using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Business.Occupancy;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Entities.Occupancy;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class OccupancyController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Occupancy Inquiry

        private const string HistoricalSpName = "sp_GetOccupancy";
        private const string CurrentSpName = "sp_GetOccupancyCurrent";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(long spaceid)
        {
            return View((new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDetails(CurrentCity.Id, spaceid));
        }

        private List<OccupancyInquiryItem> GetListItems([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            if ( request.Sorts.Count > 0 )
                SetSavedSortValues( request.Sorts, "Occupancy" );
            //get models
            var spName = gridType == "Historical" ? HistoricalSpName : CurrentSpName;
            var items = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyItems(request, "ArrivalTime", spName);
            return items;
        }

        [HttpGet]
        public ActionResult GetItems([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            //now save the sort filters
            if ( request.Sorts.Count > 0 )
                SetSavedSortValues(request.Sorts, "Occupancy");

            //get models
            var spName = gridType == "Historical" ? HistoricalSpName : CurrentSpName;
            var items = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyItems( request, "ArrivalTime", spName);
           
            int total = 0;
            if (items.Any())
            {
                total = items.First().Count;

                foreach (var item in items)
                {
                    if (item.TotalOccupiedMinute < 0)
                        item.TotalOccupiedMinute = 0;

                    //item.TotalOccupiedMinuteDisplay = FormatHelper.FormatTimeFromMinutes(item.TotalOccupiedMinute, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM);
                    //item.TotalTimePaidMinuteDisplay = FormatHelper.FormatTimeFromMinutes(item.TotalTimePaidMinute, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM);
                    item.TotalOccupiedMinuteDisplay = FormatHelper.FormatTimeFromMinutesTwoDigit(item.TotalOccupiedMinute, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM_SS);
                    item.TotalTimePaidMinuteDisplay = FormatHelper.FormatTimeFromSecondsToHHMMSS(item.TotalTimePaidMinute);
                }
            }
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            
            return Json( result, JsonRequestBehavior.AllowGet );
        }

        #endregion

        public JsonResult GetTimeTypes()
        {
            var result = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTimeTypes();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOperationalStatuses()
        {
            var result = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOperationalStatuses();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOccupancyStatuses()
        {
            var result = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyStatuses();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetTypes()
        {
            var result = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetTypes(CurrentCity.Id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNoncompliantStatuses()
        {
            var result = (new OccupancyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetNoncompliantStatuses();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region "Exporting"

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            //request.Page = 1;
            //request.PageSize = Constants.Export.CsvExportCount;
            var data = GetListItems(request, gridType);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetItems", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Occupancies_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            //get summary events
            //request.Page = 1;
            //request.PageSize = Constants.Export.ExcelExportCount;
            var data = GetListItems(request, gridType);

            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Grid Type", Value = gridType });
            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            //need to remove start and end date if it is historical
            if (gridType != "Historical")
            {
                var startDate = filters.FirstOrDefault(r => r.Member == "StartDate");
                if (startDate != null) filters.Remove(startDate);
                var endDate = filters.FirstOrDefault(r => r.Member == "EndDate");
                if (endDate != null) filters.Remove(endDate);
            }
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetItems", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "Occupancy_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");  
        }

        public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string gridType)
        {

            //get summary events
           // request.Page = 1;
            //request.PageSize = Constants.Export.PdfExportCount;
            var data = GetListItems(request, gridType);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Grid Type", Value = gridType });
            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            //need to remove start and end date if it is historical
            if (gridType != "Historical")
            {
                var startDate = filters.FirstOrDefault(r => r.Member == "StartDate");
                if (startDate != null) filters.Remove(startDate);
                var endDate = filters.FirstOrDefault(r => r.Member == "EndDate");
                if (endDate != null) filters.Remove(endDate);
            }
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetItems", CurrentCity.Id, filters, 2);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Occupancy_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        #endregion
    }
}