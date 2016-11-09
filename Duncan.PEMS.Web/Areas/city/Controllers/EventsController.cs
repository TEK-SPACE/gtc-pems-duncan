using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Events;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class EventsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const string defaultOrderBy = "DateTime desc";

        #region Event Inquiry

        // GET: /Events/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets requested Kendo grid
        /// GET: Events/GetGrid/
        /// </summary>
        public ActionResult GetGrid(string gridType)
        {
            PartialViewResult view;
            string viewName;
            JsonResult customGridData;
            switch (gridType)
            {
                case "Summary":
                    viewName = "_SummaryGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetSummaryEvents" );
                    break;
                case "Functional Status":
                    viewName = "_DiagnosticsGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetDiagnosticEvents" );
                    break;
                case "Alarms":
                    viewName = "_AlarmsGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetAlarmEvents" );
                    break;
                case "Connections":
                    viewName = "_ConnectionsGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetConnectionEvents" );
                    break;
                case "Transactions":
                    viewName = "_TransactionsGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetTransactionEvents" );
                    break;
                case "Collection Comm":
                    viewName = "_CollectionCommGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetCollectionCommEvents" );
                    break;
                case "Collection CBR":
                    viewName = "_CollectionCBRGrid";
                    customGridData = GetGridData( CurrentCity.Id, CurrentController, "GetCollectionCBREvents" );
                    break;
                default:
                    return null;
            }

            // Get the partial view and render the html to a string
            string html = string.Empty;
            using( StringWriter sw = new StringWriter() )
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView( ControllerContext, viewName );
                ViewContext viewContext = new ViewContext( ControllerContext, viewResult.View, ViewData, TempData, sw );
                viewResult.View.Render( viewContext, sw );

                html = sw.GetStringBuilder().ToString();
            }

            // Concatenate the data into an anonymous typed object
            var data = new
                           {
                               view = html,
                               customGridData
                           };

            // return as JSON
            return Json( data, JsonRequestBehavior.AllowGet );
        }

        public ActionResult GetSummaryEvents([DataSourceRequest] DataSourceRequest request)
        {
            //get models
            List<SummaryEventModel> items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSummaryEventModels(request, defaultOrderBy);

            int total = 0;
            if (items.Any())
                total = items.First().Count;

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiagnosticEvents([DataSourceRequest] DataSourceRequest request, string customerId)
        {
            var items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDiagnosticEventModels(request, defaultOrderBy);
            int total = 0;
            if (items.Any())
                total = items.First().Count;
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAlarmEvents([DataSourceRequest] DataSourceRequest request, string customerId)
        {
            var items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmEventModels(request, defaultOrderBy);

            int total = 0;
            if ( items.Any() )
                total = items.First().Count;

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetConnectionEvents([DataSourceRequest] DataSourceRequest request, string customerId)
        {
            var items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetConnectionEventModels(request, defaultOrderBy);
            int total = 0;

            if (items.Any())
                total = items.First().Count;

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTransactionEvents([DataSourceRequest] DataSourceRequest request, string customerId)
        {
            var items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactionEventModels(request, defaultOrderBy);
            int total = 0;
            if (items.Any())
                total = items.First().Count;
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCollectionCommEvents([DataSourceRequest] DataSourceRequest request, string customerId)
        {
            var items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCommEventModels(request, defaultOrderBy);
            int total = 0;
            if (items.Any())
                total = items.First().Count;
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCollectionCBREvents([DataSourceRequest] DataSourceRequest request, string customerId)
        {
            var items = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCBREventModels(request, defaultOrderBy);
            int total = 0;

            if (items.Any())
                total = items.First().Count
                    ;
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFilterValues()
        {
            var eventFactory = new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());
            // Execute the queries
            var softwareVersions = eventFactory.GetSoftwareVersionFilterItems(CurrentCity.Id);
            var assetTypes = eventFactory.GetAssetTypeFilterItems(CurrentCity.Id);
            var eventTypes = eventFactory.GetEventTypesFilterItems();
            var timeTypes = eventFactory.GetTimeTypesFilterItems();

            // Concatenate the data into an anonymous typed object
            var filterValues = new { timeTypes, softwareVersions, assetTypes, eventTypes };

            // return as JSON
            return Json( filterValues, JsonRequestBehavior.AllowGet );
        }

        #endregion

        #region Event Details

        public ActionResult Details(int id)
        {
            EventDetails eventDetails = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventDetails(id, CurrentCity.Id);

            if ( eventDetails == null )
            {
                ModelState.AddModelError( "InvalidEventId", "Event Id not found" );
            }

            return View( eventDetails );
        }

        #endregion

        #region "Exporting"

        private List<FilterDescriptor> GetFilterDescriptors( IList<IFilterDescriptor> filterDescriptors, List<FilterDescriptor> filters )
        {
            List<FilterDescriptor> descriptors = base.GetFilterDescriptors( filterDescriptors, filters );
            descriptors.RemoveAll(x => x.Member.ToLower() == "customerid");
            //we are also removing the view name
            descriptors.RemoveAll(x => x.Member.ToLower() == "viewname");

            //FilterDescriptor viewFilter = descriptors.FirstOrDefault( x => x.Member.ToLower() == "viewname" );
            //if ( viewFilter != null )
            //{
            //    string viewName = viewFilter.Value.ToString();
            //    viewName = viewName.Replace( "pv_Events", "" );
            //    viewFilter.Value = viewName;
            //}
            return descriptors;
        }

        #region CSV

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            switch (gridType)
            {
                case "Summary":
                    return ExportSummaryGridToCSV( request);
                case "Functional Status":
                    return ExportDiagnosticsGridToCSV( request);
                case "Alarms":
                    return ExportAlarmsGridToCSV( request);
                case "Connections":
                    return ExportConnectionsGridToCSV( request);
                case "Transactions":
                    return ExportTransactionsGridToCSV( request);
                case "Collection Comm":
                    return ExportCollectionCommsGridToCSV( request);
                case "Collection CBR":
                    return ExportCollectionCBRGridToCSV( request);
                default:
                    return null;
            }
        }

        private FileResult ExportSummaryGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
            //request.Page = 1;
            //request.PageSize = Constants.Export.CsvExportCount;

            var data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSummaryEventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetSummaryEvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "SummaryEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportDiagnosticsGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            // Get diagnostic events
          //  request.Page = 1;
           // request.PageSize = Constants.Export.CsvExportCount;
            var data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDiagnosticEventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetDiagnosticEvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "FunctionalStatusEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportAlarmsGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            //get alarm events
            //request.Page = 1;
            //request.PageSize = Constants.Export.CsvExportCount;
            List<AlarmsEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmEventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetAlarmEvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "AlarmEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportConnectionsGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            //get connection events
            //request.Page = 1;
           // request.PageSize = Constants.Export.CsvExportCount;
            List<ConnectionEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetConnectionEventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetConnectionEvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "ConnectionEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportTransactionsGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
            //request.Page = 1;
            //request.PageSize = Constants.Export.CsvExportCount;
            List<TransactionEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactionEventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetTransactionEvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "TransactionEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportCollectionCommsGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
            //request.Page = 1;
           // request.PageSize = Constants.Export.CsvExportCount;
            List<CollectionCommEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCommEventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetCollectionCommEvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "CollectionCommEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportCollectionCBRGridToCSV([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.CsvExportCount;
            List<CollectionCBREventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCBREventModels(request, defaultOrderBy);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(data, CurrentController, "GetCollectionCBREvents", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "CollectionCBREvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        #endregion

        #region Excel

        public FileResult ExportToExcel([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            switch (gridType)
            {
                case "Summary":
                    return ExportSummaryGridToExcel( request);
                case "Functional Status":
                    return ExportDiagnosticsGridToExcel( request);
                case "Alarms":
                    return ExportAlarmsGridToExcel( request);
                case "Connections":
                    return ExportConnectionsGridToExcel( request);
                case "Transactions":
                    return ExportTransactionsGridToExcel( request);
                case "Collection Comm":
                    return ExportCollectionCommsGridToExcel( request);
                case "Collection CBR":
                    return ExportCollectionCBRGridToExcel( request);
                default:
                    return null;
            }
        }

        private FileResult ExportSummaryGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.ExcelExportCount;
            List<SummaryEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSummaryEventModels(request, defaultOrderBy);

            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetSummaryEvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "SummaryEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        private FileResult ExportDiagnosticsGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            //get diagnostic events
           // request.Page = 1;
            //request.PageSize = Constants.Export.ExcelExportCount;
            List<DiagnosticsEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDiagnosticEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetDiagnosticEvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "FunctionalStatusEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        private FileResult ExportAlarmsGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            //get alarm events
            //request.Page = 1;
            //request.PageSize = Constants.Export.ExcelExportCount;
            List<AlarmsEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetAlarmEvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "AlarmEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        private FileResult ExportConnectionsGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            //get connection events
           // request.Page = 1;
            //request.PageSize = Constants.Export.ExcelExportCount;
            List<ConnectionEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetConnectionEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetConnectionEvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "ConnectionEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        private FileResult ExportTransactionsGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
           // request.Page = 1;
            //request.PageSize = Constants.Export.ExcelExportCount;
            List<TransactionEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactionEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetTransactionEvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "TransactionEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        private FileResult ExportCollectionCommsGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.ExcelExportCount;
            List<CollectionCommEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCommEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetCollectionCommEvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "CollectionCommEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        private FileResult ExportCollectionCBRGridToExcel([DataSourceRequest] DataSourceRequest request)
        {
            //get summary events
          //  request.Page = 1;
          //  request.PageSize = Constants.Export.ExcelExportCount;
            List<CollectionCBREventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCBREventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(data, CurrentController, "GetCollectionCBREvents", CurrentCity.Id, filters);
            return File(output.ToArray(), "application/vnd.ms-excel", "CollectionCBREvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");   
        }

        #endregion

        #region Pdf

        public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string gridType)
        {
            switch (gridType)
            {
                case "Summary":
                    return ExportSummaryGridToPdf( request );
                case "Functional Status":
                    return ExportDiagnosticsGridToPdf( request );
                case "Alarms":
                    return ExportAlarmsGridToPdf( request );
                case "Connections":
                    return ExportConnectionsGridToPdf( request );
                case "Transactions":
                    return ExportTransactionsGridToPdf( request );
                case "Collection Comm":
                    return ExportCollectionCommsGridToPdf( request );
                case "Collection CBR":
                    return ExportCollectionCBRGridToPdf( request );
                default:
                    return null;
            }
        }

        private FileResult ExportSummaryGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.PdfExportCount;
            List<SummaryEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSummaryEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetSummaryEvents", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "SummaryEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportDiagnosticsGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // step 1: creation of a document-object
            // Get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.PdfExportCount;
            List<DiagnosticsEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDiagnosticEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetDiagnosticEvents", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "FunctionalStatusEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportAlarmsGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.PdfExportCount;
            List<AlarmsEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetAlarmEvents", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "AlarmEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportConnectionsGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
           // request.Page = 1;
           // request.PageSize = Constants.Export.PdfExportCount;
            List<ConnectionEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetConnectionEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetConnectionEvents", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "ConnectionEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportTransactionsGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
          //  request.Page = 1;
          //  request.PageSize = Constants.Export.PdfExportCount;
            List<TransactionEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTransactionEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetTransactionEvents", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "TransactionEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportCollectionCommsGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
         //   request.Page = 1;
          //  request.PageSize = Constants.Export.PdfExportCount;
            List<CollectionCommEventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCommEventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetCollectionCommEvents", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "CollectionCommEvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportCollectionCBRGridToPdf([DataSourceRequest] DataSourceRequest request)
        {
            // Get summary events
            //request.Page = 1;
          //  request.PageSize = Constants.Export.PdfExportCount;
            List<CollectionCBREventModel> data = (new EventModelFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionCBREventModels(request, defaultOrderBy);
            var filters = new List<FilterDescriptor>();
            filters = GetFilterDescriptors(request.Filters, filters); //pass in an empty list
            var output = (new ExportFactory()).GetPDFFileMemoryStream(data, CurrentController, "GetCollectionCBREvents", CurrentCity.Id, filters, 2);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "CollectionCBREvents_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        #endregion

        #endregion
    }
}