using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Collections;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Entities.Collections;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using NLog;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NPOI.HSSF.UserModel;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
     [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class CollectionsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region "List pages"


        public ActionResult Index()
        {
            return View();
        }

         public ActionResult Aggregation()
         {
             return View();

         }

         public ActionResult GroupedAggregation()
         {
             //TODO - GTC: Pems USA Collection Aggregation Update work
             return View();
         }


         public ActionResult VendorCoinEntry()
         {
             return View();
         }

         [HttpGet]
        public ActionResult GetCollectionRoutes ([DataSourceRequest] DataSourceRequest request, string customerId, string meterId, string meterName)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Collection");

            //get the user models
            int total;
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionsListModels(request, out total, customerId, meterId, meterName, true);
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
             //var result = new DataSourceResult
            {
                Data = items,
                Total = total
            };

            return Json(result,  JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetAggregationRoutes([DataSourceRequest] DataSourceRequest request, string customerId, string meterId, string meterName, string area, string zone, string suburb, string startDate, string endDate)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Aggregation");

            //get the user models
            int total;
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationListModels(request, out total, customerId, meterId, meterName, area, zone, suburb, startDate, endDate, true);
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total
            };

            return Json(result,  JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetGroupedAggregationRoutes([DataSourceRequest] DataSourceRequest request, string customerId, string meterId, string meterName, string area, string zone, string suburb, string startDate, string endDate)
        {
            //TODO - GTC: Pems USA Collection Aggregation Update work
            //the filters might be different, pending the documentaiton from duncan. This will meant hta you will have to update the signature of htis method and the values passed in from the view.

            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "GroupedAggregation");

            //get the user models
            int total;
            //todo gtc: - will have to create a getgroupedaggregationlistmodels method in the collections factory
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationListModels(request, out total, customerId, meterId, meterName, area, zone, suburb, startDate, endDate, true);
            
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total
            };

            return Json(result, JsonRequestBehavior.AllowGet);
            //todo gtc: will have to create and return a model for the grouped aggregation items.
        }

        
        #endregion

        #region "Filters"
              /// <summary>
        /// Gets the list items in the AlarmStatus dropdown list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetConfigurations(int cityId)
        {
            var values = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetConfigurations(cityId);
            return Json(values, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of items for the Asset State dropdown filter
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStatuses()
        {
            var assets = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetStatuses();
            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of items for the Suburbs dropdown list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSuburbs(int cityId)
        {
            var areas = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSuburbs(cityId);
            return Json(areas, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Exporting"

         //Collection Meters
        public FileResult ExportMetersToCsv(long routeId, int customerId)
         {
             var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(routeId, customerId);
             var output = new MemoryStream();
             var writer = new StreamWriter(output, Encoding.UTF8);
             AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Meter Name"), true);
             foreach (var item in items.Meters)
                 //add a item for each property here
                 AddCsvValue(writer, item.MeterName, true);
             writer.Flush();
             output.Position = 0;
             return File(output, "text/comma-separated-values",
                         "CollectionRouteMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
         }

        public FileResult ExportMetersToExcel(long routeId, int customerId)
        {
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(routeId, customerId);
            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();
            //add the filtered values to the top of the excel file
            // Add the filtered values to the top of the excel file
            var customFilters = new List<FilterDescriptor>{new FilterDescriptor {Member = "Configuration Id", Value = routeId}};
            var rowNumber = AddFiltersToExcelSheet(sheet, customFilters);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Meter Name"));
            //Populate the sheet with values from the grid data
            foreach (var item in items.Meters)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);
                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.MeterName);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user
            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "CollectionRouteMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportMetersToPdf(long routeId, int customerId)
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
            var customFilters = new List<FilterDescriptor> { new FilterDescriptor { Member = "Configuration Id", Value = routeId } };
            document = AddFiltersToPdf(document, customFilters);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(routeId, customerId);

            var dataTable = new PdfPTable(1) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Meter Name"));
            dataTable.CompleteRow();

            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;
            foreach (var item in items.Meters)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.MeterName);
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "CollectionRouteMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }


         //Collections
        public FileResult ExportToCsv([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName)
        {
            var items = GetExportData(request, customerId, meterId, meterName, Constants.Export.CsvExportCount);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetRoutes", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "CollectionRoutesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request,  string customerId, string meterId, string meterName)
        {
            var items = GetExportData(request, customerId, meterId, meterName, Constants.Export.ExcelExportCount);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            //we have to add meter ID and meter name
            if (!string.IsNullOrEmpty(meterId))
                filters.Add(new FilterDescriptor { Member = "Meter ID", Value = meterId });
            if (!string.IsNullOrEmpty(meterName))
                filters.Add(new FilterDescriptor { Member = "Meter Name", Value = meterName });

            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetRoutes", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "CollectionRoutesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName)
        {
            var items = GetExportData(request, customerId, meterId, meterName, Constants.Export.PdfExportCount);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            //we have to add meter ID and meter name
            if (!string.IsNullOrEmpty(meterId))
                filters.Add(new FilterDescriptor { Member = "Meter ID", Value = meterId });
            if (!string.IsNullOrEmpty(meterName))
                filters.Add(new FilterDescriptor { Member = "Meter Name", Value = meterName });
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetRoutes", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "CollectionRoutesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable<CollectionListModel> GetExportData(DataSourceRequest request, string customerId, string meterId, string meterName, int size)
        {
            //get the models
            int total = 0;
            //we are forcing the size to be -1 so we get back what the user is viewing per the export Change Order
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionsListModels(request, out total, customerId, meterId, meterName, true, -1);
            return items;
        }

         //Aggregations
        public FileResult ExportToCsvAgg([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            var items = GetExportDataAgg(request, customerId, meterId, meterName, areaname, zone, suburb, startDate, endDate, Constants.Export.CsvExportCount);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetAggregations", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "AggregationRoutesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcelAgg([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            var items = GetExportDataAgg(request, customerId, meterId, meterName, areaname, zone, suburb, startDate, endDate, Constants.Export.ExcelExportCount);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            //add the filters that werent addedto the request object
            //Meter id, meter name, area, start date, end date, zone, suburb
            if (!string.IsNullOrEmpty(meterId))
                filters.Add(new FilterDescriptor { Member = "Meter ID", Value = meterId });
            if (!string.IsNullOrEmpty(meterName))
                filters.Add(new FilterDescriptor { Member = "Meter Name", Value = meterName });
            if (!string.IsNullOrEmpty(areaname))
                filters.Add(new FilterDescriptor { Member = "Area", Value = areaname });
            if (!string.IsNullOrEmpty(zone))
                filters.Add(new FilterDescriptor { Member = "Zone", Value = zone });
            if (!string.IsNullOrEmpty(suburb))
                filters.Add(new FilterDescriptor { Member = "Suburb", Value = suburb });

            //start date and end date checkt he colldatasumm table
            //parse the dates
            var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
            sDate = sDate.ToLocalTime();
            var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
            eDate = eDate.ToLocalTime();
            filters.Add(new FilterDescriptor { Member = "Start Date", Value = sDate });
            filters.Add(new FilterDescriptor { Member = "End Date", Value = eDate });

            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetAggregations", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "AggregationRoutesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
            }

         public FileResult ExportToPdfAgg([DataSourceRequest] DataSourceRequest request, string customerId,string meterId, string meterName, string areaname, string zone, string suburb,string startDate, string endDate)
         {
             var items = GetExportDataAgg(request, customerId, meterId, meterName, areaname, zone, suburb, startDate,endDate, Constants.Export.PdfExportCount);
             var filters = new List<FilterDescriptor>();
             //pass in an empty list
             filters = GetFilterDescriptors(request.Filters, filters);

             //add the filters that werent addedto the request object
             //Meter id, meter name, area, start date, end date, zone, suburb
             if (!string.IsNullOrEmpty(meterId))
                 filters.Add(new FilterDescriptor { Member = "Meter ID", Value = meterId });
             if (!string.IsNullOrEmpty(meterName))
                 filters.Add(new FilterDescriptor { Member = "Meter Name", Value = meterName });
             if (!string.IsNullOrEmpty(areaname))
                 filters.Add(new FilterDescriptor { Member = "Area", Value = areaname });
             if (!string.IsNullOrEmpty(zone))
                 filters.Add(new FilterDescriptor { Member = "Zone", Value = zone });
             if (!string.IsNullOrEmpty(suburb))
                 filters.Add(new FilterDescriptor { Member = "Suburb", Value = suburb });

             //start date and end date checkt he colldatasumm table
             //parse the dates
             var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
             sDate = sDate.ToLocalTime();
             var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
             eDate = eDate.ToLocalTime();
             filters.Add(new FilterDescriptor { Member = "Start Date", Value = sDate });
             filters.Add(new FilterDescriptor { Member = "End Date", Value = eDate });



             var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetAggregations", CurrentCity.Id, filters, 2);

             // send the memory stream as File
             return File(output.ToArray(), "application/pdf", "AggregationRoutesExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
         }

         private IEnumerable<AggregationListModel> GetExportDataAgg(DataSourceRequest request, string customerId, string meterId, string meterName, string area, string zone, string suburb, string startDate, string endDate, int size)
        {
            //get the models
            int total = 0;
            //we are forcing the size to be -1 so we get back what the user is viewing per the export Change Order
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationListModels(request, out total, customerId, meterId, meterName, area, zone, suburb, startDate, endDate, true, -1);
            return items;
        }
        
         //Aggregation Meters
         //public FileResult ExportAggMetersToCsv(string routeId, string customerId, long dateTimeTicks)
         //{
         //    var collectionDate = new DateTime(dateTimeTicks);
         //    var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate);
         //    var output = (new ExportFactory()).GetCsvFileMemoryStream(items.Meters, CurrentController, "GetAggMeters", CurrentCity.Id);
         //    return File(output, "text/comma-separated-values", "AggregationMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
         //}

         //public FileResult ExportAggMetersToExcel(string routeId, string customerId, long dateTimeTicks)
         //{
         //    var collectionDate = new DateTime(dateTimeTicks);
         //    var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate);
         //    var filters = new List<FilterDescriptor> { new FilterDescriptor { Member = "Collection ID", Value = routeId } };
         //    var output = (new ExportFactory()).GetExcelFileMemoryStream(items.Meters, CurrentController, "GetAggMeters", CurrentCity.Id, filters);
         //    return File(output.ToArray(),   //The binary data of the XLS file
         //      "application/vnd.ms-excel", //MIME type of Excel files
         //      "AggregationMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
         //}

         //public FileResult ExportAggMetersToPdf(string routeId, string customerId, long dateTimeTicks)
         //{

         //    var collectionDate = new DateTime(dateTimeTicks);
         //    var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate);
         //    var filters = new List<FilterDescriptor> { new FilterDescriptor { Member = "Collection ID", Value = routeId } };
         //    var output = (new ExportFactory()).GetPDFFileMemoryStream(items.Meters, CurrentController, "GetAggMeters", CurrentCity.Id, filters, 1);

         //    // send the memory stream as File
         //    return File(output.ToArray(), "application/pdf", "AggregationMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
         //}

         public FileResult ExportAggMetersToCsv(string routeId, string customerId, long dateTimeTicks, long crrId)
         {
             var collectionDate = new DateTime(dateTimeTicks);
             var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate, crrId);
             var output = (new ExportFactory()).GetCsvFileMemoryStream(items.Meters, CurrentController, "GetAggMeters", CurrentCity.Id);
             return File(output, "text/comma-separated-values", "AggregationMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
         }

         public FileResult ExportAggMetersToExcel(string routeId, string customerId, long dateTimeTicks, long crrId)
         {
             var collectionDate = new DateTime(dateTimeTicks);
             var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate, crrId);
             var filters = new List<FilterDescriptor> { new FilterDescriptor { Member = "Collection ID", Value = routeId } };
             var output = (new ExportFactory()).GetExcelFileMemoryStream(items.Meters, CurrentController, "GetAggMeters", CurrentCity.Id, filters);
             return File(output.ToArray(),   //The binary data of the XLS file
               "application/vnd.ms-excel", //MIME type of Excel files
               "AggregationMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
         }

         public FileResult ExportAggMetersToPdf(string routeId, string customerId, long dateTimeTicks, long crrId)
         {

             var collectionDate = new DateTime(dateTimeTicks);
             var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate, crrId);
             var filters = new List<FilterDescriptor> { new FilterDescriptor { Member = "Collection ID", Value = routeId } };
             var output = (new ExportFactory()).GetPDFFileMemoryStream(items.Meters, CurrentController, "GetAggMeters", CurrentCity.Id, filters, 1);

             // send the memory stream as File
             return File(output.ToArray(), "application/pdf", "AggregationMetersExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
         }

        
        //TODO - GTC: Get the Grouped Aggregation export working
        //TODO - GTC: Get the Grouped Aggregation meters export working
        //TODO - GTC: Get the Grouped Aggregation single space meters export working

         //Vendor Coin Entry
        public FileResult ExportVCEToCsv([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, int customerId)
        {
            //get role models
            var data = GetVCEExportData(request, startDate,endDate, customerId);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetVCEs", CurrentCity.Id);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Date"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Total Meter Amount Reported"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Total Amount Counted"), true);
            foreach (VCEListModel item in data)
            {
                //add a item for each property here
                AddCsvValue(writer, item.CollectionRunName);
                AddCsvValue(writer, item.ActivationDateDisplay);
                AddCsvValue(writer, item.TotalAmountReportedByMeter.ToString());
                AddCsvValue(writer, item.TotalAmountCounted.ToString(), true);
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values", "VendorCoinEntryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportVCEToExcel([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, int customerId)
        {
            //get role models
            var data = GetVCEExportData(request, startDate, endDate, customerId);
            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetVCEs", CurrentCity.Id);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            if (!string.IsNullOrEmpty(startDate))
            {
                var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
                sDate = sDate.ToLocalTime();
                request.Filters.Add(new FilterDescriptor { Member = "Start Date", Value = sDate.ToShortDateString() });
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
                eDate = eDate.ToLocalTime();
                request.Filters.Add(new FilterDescriptor { Member = "End Date", Value = eDate.ToShortDateString() });
            }

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);
            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue(GetLocalizedGridTitle(gridData, "Name"));
            headerRow.CreateCell(1).SetCellValue(GetLocalizedGridTitle(gridData, "Date"));
            headerRow.CreateCell(2).SetCellValue(GetLocalizedGridTitle(gridData, "Total Meter Amount Reported"));
            headerRow.CreateCell(3).SetCellValue(GetLocalizedGridTitle(gridData, "Total Amount Counted"));

            //Populate the sheet with values from the grid data
            foreach (VCEListModel item in data)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.CollectionRunName);
                row.CreateCell(columnNumber++).SetCellValue(item.ActivationDateDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.TotalAmountReportedByMeter.ToString());
                row.CreateCell(columnNumber++).SetCellValue(item.TotalAmountCounted.ToString());
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "VendorCoinEntryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportVCEToPdf([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, int customerId)
        {
            var data = GetVCEExportData(request, startDate, endDate, customerId);
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            //step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            //step 3: we open the document
            document.Open();

            //step 4: we add content to the document
            if (!string.IsNullOrEmpty(startDate))
            {
                var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
                sDate = sDate.ToLocalTime();
                request.Filters.Add(new FilterDescriptor { Member = "Start Date", Value = sDate.ToShortDateString() });
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
                eDate = eDate.ToLocalTime();
                request.Filters.Add(new FilterDescriptor { Member = "End Date", Value = eDate.ToShortDateString() });
            }
            //first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));
         

            var gridData = (new GridFactory()).GetGridData(CurrentController, "GetVCEs", CurrentCity.Id);

            var dataTable = new PdfPTable(4) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Date"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Total Meter Amount Reported"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Total Amount Counted"));
            dataTable.CompleteRow();
            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;
            foreach (VCEListModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.CollectionRunName);
                dataTable.AddCell(item.ActivationDateDisplay);
                dataTable.AddCell(item.TotalAmountReportedByMeter.ToString());
                dataTable.AddCell(item.TotalAmountCounted.ToString());
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "VendorCoinEntryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable GetVCEExportData(DataSourceRequest request, string startDate, string endDate, int customerId)
        {
            //get models
            var vceModels = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVCEIndexModels(customerId, startDate, endDate);

            //sort, filter, group them
            var items = vceModels.AsQueryable();
            items = items.ApplyFiltering(request.Filters);
            items = items.ApplySorting(request.Groups, request.Sorts);
            //export change order udpate
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.AsEnumerable();
            return data;
        }

        public FileResult ExportToExcelGroupedAgg([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            int total;
            //var StartDate = new DateTime(dateTimeTicks);
            //var items = GetGroupedAggregationRoutes(request,customerId,meterId,meterName,areaname,zone,suburb,startDate,endDate); //(new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate);
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetGroupedAggregationListModels(request, out total, customerId, meterId, meterName, areaname, zone, suburb, startDate, endDate);
            var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
            sDate = sDate.ToLocalTime();
            var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
            eDate = eDate.ToLocalTime();
            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "CustomerID", Value = customerId });
            filters.Add(new FilterDescriptor { Member = "MeterID", Value = meterId });
            filters.Add(new FilterDescriptor { Member = "MeterName", Value = meterName });
            filters.Add(new FilterDescriptor { Member = "AreaName", Value = areaname });
            filters.Add(new FilterDescriptor { Member = "Zone", Value = zone });
            filters.Add(new FilterDescriptor { Member = "Suburb", Value = suburb });
            filters.Add(new FilterDescriptor { Member = "StartDate", Value = sDate });
            filters.Add(new FilterDescriptor { Member = "EndDate", Value = eDate });
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetGroupedAggregationRoutes", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "GroupedAggregationExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToCsvGroupedAgg([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            int total;
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetGroupedAggregationListModels(request, out total, customerId, meterId, meterName, areaname, zone, suburb, startDate, endDate);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetGroupedAggregationRoutes", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "GroupedAggregationExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdfGroupedAgg([DataSourceRequest]DataSourceRequest request, string customerId, string meterId, string meterName, string areaname, string zone, string suburb, string startDate, string endDate)
        {
            int total;
            var items = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetGroupedAggregationListModels(request, out total, customerId, meterId, meterName, areaname, zone, suburb, startDate, endDate);
            var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
            sDate = sDate.ToLocalTime();
            var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
            eDate = eDate.ToLocalTime();
            var filters = new List<FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = "CustomerID", Value = customerId });
            filters.Add(new FilterDescriptor { Member = "MeterID", Value = meterId });
            filters.Add(new FilterDescriptor { Member = "MeterName", Value = meterName });
            filters.Add(new FilterDescriptor { Member = "AreaName", Value = areaname });
            filters.Add(new FilterDescriptor { Member = "Zone", Value = zone });
            filters.Add(new FilterDescriptor { Member = "Suburb", Value = suburb });
            filters.Add(new FilterDescriptor { Member = "StartDate", Value = sDate });
            filters.Add(new FilterDescriptor { Member = "EndDate", Value = eDate });
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetGroupedAggregationRoutes", CurrentCity.Id, filters, 1);
            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GroupedAggregationExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }


        #endregion

        #region "Aggregate Details"

        ////Comment ad new crrId(collectionrunreportId added)
        //public ActionResult AggregationDetails(string routeId, string customerId, long dateTimeTicks)
        //{
        //    var collectionDate = new DateTime(dateTimeTicks);
        //    var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate);
        //    return View(item);
        //}

        public ActionResult AggregationDetails(string routeId, string customerId, long dateTimeTicks, long crrId)
        {
            var collectionDate = new DateTime(dateTimeTicks);
            var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate, crrId);
            return View(item);
        }

         [HttpPost]
         public ActionResult AggregationDetails(AggregationDetails model)
         {
             //only button is return, so send them back to the aggregation listing page
             return RedirectToAction("Aggregation", "Collections");
         }
     
         public ActionResult MeterDetails(int meterId, string customerId, long dateTimeTicks, long collectionDateTimeTicks, int areaId, long collectionRunId)
        {
            var datetime = new DateTime(dateTimeTicks);
            var collectionDateTime = new DateTime(collectionDateTimeTicks);
            var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationMeterDetails(meterId, customerId, datetime, collectionDateTime, areaId, collectionRunId);
            return View(item);
        }

         [HttpPost]
         public ActionResult MeterDetails(AggregationMeterDetails model, string submitButton,  FormCollection formColl)
         {
             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);
             //return is the only thing they can do - make sure we send them to the aggregation details page
             return RedirectToAction("AggregationDetails", "Collections", new {routeId = model.CollectionRunId, customerId = model.CustomerId, dateTimeTicks = model.CollectionDate.Ticks});
         }
        #endregion

         //TODO - GTC: update this for grouped aggregation details
         #region "Grouped Aggregate Details"

         public ActionResult GroupedAggregationDetails(string routeId, string customerId, long dateTimeTicks)
         {
             var collectionDate = new DateTime(dateTimeTicks);
             //todo - gtc: update to get grouped details
             var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationDetails(routeId, customerId, collectionDate);
             return View(item);
         }

         [HttpPost]
         public ActionResult GroupedAggregationDetails(AggregationDetails model)
         {
             //todo - gtc: update to get grouped details

             //only button is return, so send them back to the aggregation listing page
             return RedirectToAction("Aggregation", "Collections");
         }

         public ActionResult SingleSpaceMeterDetails(int meterId, string customerId, long dateTimeTicks, long collectionDateTimeTicks, int areaId, long collectionRunId)
         {
             //todo - gtc: update to get grouped single space meter details

             var datetime = new DateTime(dateTimeTicks);
             var collectionDateTime = new DateTime(collectionDateTimeTicks);
             var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAggregationMeterDetails(meterId, customerId, datetime, collectionDateTime, areaId, collectionRunId);
             return View(item);
         }

         [HttpPost]
         public ActionResult SingleSpaceMeterDetails(AggregationMeterDetails model, string submitButton, FormCollection formColl)
         {
             //todo - gtc: update to get grouped single space meter details

             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);
             //return is the only thing they can do - make sure we send them to the aggregation details page
             return RedirectToAction("GroupedAggregationDetails", "Collections", new { routeId = model.CollectionRunId, customerId = model.CustomerId, dateTimeTicks = model.CollectionDate.Ticks });
         }
         #endregion

         #region "Collection Details"
          public ActionResult Collection(long routeId, int customerId)
        {
            var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(routeId, customerId);
            return View(item);
        }
         [HttpPost]
         public ActionResult Collection(CollectionConfiguration model, string submitButton)
         {

             //check the other submit buttons and act on them, or continue
             switch (submitButton)
             {
                 case "Update":
                     return RedirectToAction("EditCollection", "Collections", new {routeId = model.CollectionId, customerId = model.CustomerId});
                 case "Copy":
                     return RedirectToAction("CopyCollection", "Collections", new { routeId = model.CollectionId, customerId = model.CustomerId });
                 case "Delete":
                     //delete it here - make sure we are setting delete time to the local city time.
                     (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).DeleteCollection(model.CollectionId);
                     return RedirectToAction("Index", "Collections");
             }
             //Return
             return RedirectToAction("Index", "Collections");
         }
         #endregion

         #region "Collection Edit"
         public ActionResult EditCollection(long routeId, int customerId)
         {
             var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(routeId, customerId, true, true);
             item.LocalTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;
             return View(item);
         }

         //GetAllMeters


         public ActionResult GetMetersForCollection([DataSourceRequest]DataSourceRequest request, string locationtype, string locationvalue)
         {
             //** First clear the IEnumerable model instances;
             IEnumerable<CollectionMeter> result = Enumerable.Empty<CollectionMeter>();
             int total = 0;


             result = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetMetersForCollection(CurrentCity.Id, locationtype, locationvalue);

             var finalresult = result.ToList();

             DataSourceResult Fresult = new DataSourceResult()
             {
                 Data = finalresult,
                 Total = result.Count(),
             };
             return Json(Fresult, JsonRequestBehavior.AllowGet);

         }

         [HttpPost]
         public ActionResult EditCollection(CollectionConfiguration model, string submitButton, FormCollection formColl)
         {
             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);
             //check the other submit buttons and act on them, or continue
             switch (submitButton)
             {
                 case "Return":
                     return RedirectToAction("Collection", "Collections", new { routeId = model.CollectionId, customerId = model.CustomerId });
             }

             //Save //Save a new one
             if (ModelState.IsValid)
             {
                 //now we have to validate the activation date. since they can create it with a activation date, then wait a week until they update it, we cant force the patepicker mindate like we did on the create page, so we do a check here
                 if (model.ActivationDate.GetValueOrDefault().Date < model.LocalTime.Date)
                 {
                     ModelState.AddModelError("ActivationDate",
                                              (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage,
                                                                                        "Activation Date cannot be in the past"));
                     return EditCollection(model.CollectionId, model.CustomerId);
                 }
                 var collFactory = new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

                 collFactory.EditCollection(model);

                 //now remove all the meters for this colleciton run and add the ones they want.
                 collFactory.RemoveCollectionRunMeters(model.CollectionId);
                 //now add all the ones we need to.
                 var metersToAdd = GetMeters();
                 if (metersToAdd != null)
                 {
                     foreach (var meterToAdd in metersToAdd)
                     {
                         //its int he following format MeterId|AreaId
                         var meterInfo = meterToAdd.Split('|');
                         var meterId = Convert.ToInt32(meterInfo[0]);
                         var areaId = Convert.ToInt32(meterInfo[1]);
                         var status = collFactory.AddMeterToCollection(meterId, areaId, model.CollectionId,model.CustomerId,model.VendorId);
                     }
                 }
                 //save the model and re-display the values(send them back to the edit)
                 ModelState.Clear();
                 ModelState.AddModelError(Constants.ViewData.ModelStateStatusKey,
                                          (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.StatusMessage,
                                                                                    "Collection Route Saved"));
             }
             return EditCollection(model.CollectionId, model.CustomerId);
         }


       public ActionResult GetArea()
        {
            var Area = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId("Area",CurrentCity.Id);
            return Json(Area, JsonRequestBehavior.AllowGet);
        }

       public ActionResult GetZone()
       {
           var Zone = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId("Zone",CurrentCity.Id);
           return Json(Zone, JsonRequestBehavior.AllowGet);
       }

       public ActionResult GetStreet()
       {
           var Street = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId("Street",CurrentCity.Id);
           return Json(Street, JsonRequestBehavior.AllowGet);
       }

       public ActionResult GetSuburb()
       {
           var Suburb = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId("Suburb",CurrentCity.Id);
           return Json(Suburb, JsonRequestBehavior.AllowGet);
       }

         #endregion

        #region "Copy Collection"
         public ActionResult CopyCollection(long routeId, int customerId)
         {
             var item = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(routeId, customerId, true);
             //clear the name and activation date
             item.CollectionName = string.Empty;
             item.ActivationDate = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;
             item.Status = (new ResourceFactory() ).GetLocalizedTitle(ResourceTypes.Glossary,"Pending" );
             item.StatusId = 2;
             //add validation for these items as well
             return View(item);
         }

         [HttpPost]
         public ActionResult CopyCollection(CollectionConfiguration model, string submitButton,  FormCollection formColl)
         {

             //check the other submit buttons and act on them, or continue
             switch (submitButton)
             {
                 case "Return":
                     return RedirectToAction("Index", "Collections");
             }

             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);

             var collFactory = new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

             //Save a new one
             if (ModelState.IsValid)
             {

                 //check to make sure name and activation date are set, otehrwise send them back
                 //now we have to validate the activation date.
                 var localTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;
                 if (model.ActivationDate.GetValueOrDefault().Date < localTime.Date)
                 {
                     ModelState.AddModelError("ActivationDate", (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage, "Activation Date cannot be in the past"));
                     var newModel = collFactory.GetCollectionConfiguration(model.CollectionId, model.CustomerId, true);
                     newModel.ActivationDate = model.ActivationDate;
                     newModel.CollectionName = model.CollectionName;
                     return View(newModel);
                 }

                 //Per AU, we do not need to check dupliate names now.
                 ////now check to see if the name exists, if it does, show error and return
                 //var collectionAlreadyExists = collFactory.DoesCollectionExist(model.CollectionName, model.CustomerId);
                 //if (collectionAlreadyExists)
                 //{
                 //    ModelState.AddModelError("CollectionName", (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage, "Duplicate Collection Name"));
                 //    var newModel = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCollectionConfiguration(model.CollectionId, model.CustomerId, true);
                 //    newModel.ActivationDate = model.ActivationDate;
                 //    newModel.CollectionName = model.CollectionName;
                 //    return View(newModel);
                 //}

                 //copy the collection
                 var newCollection = collFactory.CopyCollection(model);
                 //send the user to the edit version of this page so they can modify the status, meters, etc.
                 return RedirectToAction("EditCollection", "Collections", new { routeId = newCollection.CollectionId, customerId = newCollection.CustomerId });
             }
           
           
             //save the model and re-display the values(send them back to the copy page)
            //if it was successful, 
             var newModel2 = collFactory.GetCollectionConfiguration(model.CollectionId, model.CustomerId, true);
             newModel2.ActivationDate = model.ActivationDate;
             newModel2.CollectionName = model.CollectionName;
             return View(newModel2);
         }
        #endregion

         #region "Create Collection"
         public ActionResult CreateCollection(int customerId)
         {
             var item = new CollectionConfiguration
                 {
                     ActivationDate = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime,
                     Status =  (new ResourceFactory() ).GetLocalizedTitle(ResourceTypes.Glossary,"Pending" ),
                     StatusId = Constants.CollectionRoutes.PendingStatus,
                     CustomerId = customerId,
                     VendorOptions = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString()).GetVendors(customerId))
                 };
             //clear the name and activation date
             //add validation for these items as well
             return View(item);
         }

         [HttpPost]
         public ActionResult CreateCollection(CollectionConfiguration model, string submitButton, FormCollection formColl)
         {
             //check the other submit buttons and act on them, or continue
             switch (submitButton)
             {
                 case "Return":
                     return RedirectToAction("Index", "Collections");
             }
             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);
             //Save a new one
             var collFactory = new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());
             if (ModelState.IsValid)
             {
                 //check to make sure name and activation date are set, otehrwise send them back
                 var localTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;
                 if (model.ActivationDate.GetValueOrDefault().Date < localTime.Date)
                 {
                     ModelState.AddModelError("ActivationDate", (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage, "Activation Date cannot be in the past"));
                     model.VendorOptions = collFactory.GetVendors(model.CustomerId);
                     return View(model);
                 }

                 //Per AU, we do not need to check dupliate names now.
                 ////now check to see if the name exists, if it does, show error and return
                 //var collectionAlreadyExists = collFactory.DoesCollectionExist(model.CollectionName, model.CustomerId);
                 //if (collectionAlreadyExists)
                 //{
                 //    ModelState.AddModelError("CollectionName", (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.ErrorMessage, "Duplicate Collection Name"));
                 //    model.VendorOptions = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString()).GetVendors(model.CustomerId));
                 //    return View(model);
                 //}
                 //copy the collection
                 var newCollection = collFactory.CreateCollection(model);
                 //send the user to the edit version of this page so they can modify the status, meters, etc.
                 return RedirectToAction("EditCollection", "Collections", new { routeId = newCollection.CollectionId, customerId = newCollection.CustomerId });
             }
             model.VendorOptions = collFactory.GetVendors(model.CustomerId);
             //save the model and re-display the values
             return View(model);
         }

         public ActionResult VendorList(int customerId)
         {
             var VendorOptions = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString()).GetVendors(customerId));
             return Json(VendorOptions, JsonRequestBehavior.AllowGet);
         }
         public string AddVendorname(int customerId, string vendorname)
         {
             return (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString()).AddVendorname(customerId, vendorname));
            
         }
         
         #endregion

         public ActionResult GetMeterIds(int customerId)
         {
             var meters = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime).GetDistinctMeterIds(customerId));
             return Json(meters, JsonRequestBehavior.AllowGet);
         }

         public JsonResult SetMeters(string[] uns)
         {
             Session["__CRSavedMeters"] = uns;
             return null;
         }

         private string[] GetMeters()
         {
             if (Session["__CRSavedMeters"] != null)
             {
                 var retVal = Session["__CRSavedMeters"] as string[];
                 //clear the session
                 Session.Remove("__CRSavedMeters");
                 return retVal;
             }
             return null;
         }



        #region Vendor Coin Entry
         public ActionResult GetVendorCoinEntry([DataSourceRequest] DataSourceRequest request, string startDate, string endDate, int customerId)
         {
             //now save the sort filters
             if (request.Sorts.Count > 0)
                 SetSavedSortValues(request.Sorts, "VCEs");

             //get models
             var vceModels = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVCEIndexModels( customerId, startDate, endDate);

             var items = vceModels.AsQueryable();
             //sort, page, filter, group them
             items = items.ApplyFiltering(request.Filters);
             var total = items.Count();
             items = items.ApplySorting(request.Groups, request.Sorts);
             items = items.ApplyPaging(request.Page, request.PageSize);
             IEnumerable data = items.ToList();
             DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
             //var result = new DataSourceResult
             {
                 Data = data,
                 Total = total,
             };
             return Json(result, JsonRequestBehavior.AllowGet);
         }




         public ActionResult VendorCoinEntryDetails(int collectionRunId, int customerId)
         {
             var model = (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVCEDetailModel(customerId, collectionRunId);
             return View(model);
         }



         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult VendorCoinEntryDetails(VCEDetailsModel model, string submitButton, FormCollection formColl)
         {
             //check the other submit buttons and act on them, or continue
             switch (submitButton)
             {
                 case "Cancel":
                 case "Return":
                     return RedirectToAction("VendorCoinEntry", "Collections", new { rtn = "true" });
             }
             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);

             //clear the alarm and send them back to the index page
             if (ModelState.IsValid)
             {
                 (new CollectionsFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).SaveVCEDetailsModel(model);
                 return RedirectToAction("VendorCoinEntry", "Collections", new { rtn = "true" });
             }

             return View(model);
         }


         #endregion



        ////not using this currently, leaving it in since we have commented and uncommented this 3 times already
         /// it is the rules fr adding meterrs. currently it is done client side, but if we need to apply the validation rules, then we need to use this.
         //public ActionResult AddMeters(CollectionConfiguration model)
         //{
         //    //get the meters to add and the current collection route ID
         //    var collFactory = new CollectionsFactory();
         //    var metersToAdd = GetMeters();
         //    foreach (var meterToAdd in metersToAdd)
         //    {
         //        //its int he following format MeterId|AreaId
         //        var meterInfo = meterToAdd.Split('|');
         //        var meterId = Convert.ToInt32(meterInfo[0]);
         //        var areaId = Convert.ToInt32(meterInfo[1]);

         //        var status = collFactory.AddMeterToCollection(meterId, areaId, model.CollectionId, model.CustomerId, model.VendorId);
                
         //        //NOTE: we are not doing validation anymore, duncan decided to jsut add meter and have backend process handle it, so im leaving this stuff in here in case we need it in the future, but should always bring back success
         //        ////now that we have our status, do something with it! display nothing for success, or message for others
         //        ////add the model state "Errors" 
         //       //-----------------------------------------------------------------------------------------------------------
         //        //NOTE: 6/18/2013 - Duncan decided they wanted to ptu back in the validation jsut for the current collection run
         //        //---------------------------------------------------------------------------------------------------------------
         //        var resFActory = new ResourceFactory();

         //        //var localizedActive = resFActory.GetLocalizedTitle(ResourceTypes.ErrorMessage, "MeterId {0} is assigned to the Active Collection Route {1}");
         //        var localizedCurrent = resFActory.GetLocalizedTitle(ResourceTypes.ErrorMessage, "Meter Id {0} is assigned to the Current Collection Route: {1}");
         //        //var localizedPending = resFActory.GetLocalizedTitle(ResourceTypes.ErrorMessage, "MeterId {0} is assigned to the Pending Collection Route {1}");
         //        var unknownError = resFActory.GetLocalizedTitle(ResourceTypes.ErrorMessage, "Meter Id {0} could not be added - Unknown Error");

         //        switch (status.Result)
         //        {
         //          //  case CollectionRouteResult.ExistsActive:
         //              //  ModelState.AddModelError("MeterAdditionStatus", string.Format(localizedActive, meterId, status.ConflictingCollectionId));
         //            //    break;
         //            case CollectionRouteResult.ExistsCurrent:
         //                ModelState.AddModelError("MeterAdditionStatus", string.Format(localizedCurrent, meterId, status.ConflictingCollectionId));
         //                break;
         //          //  case CollectionRouteResult.ExistsPending:
         //           //     ModelState.AddModelError("MeterAdditionStatus", string.Format(localizedPending, meterId, status.ConflictingCollectionId));
         //           //     break;
         //            case CollectionRouteResult.UnknownError:
         //               ModelState.AddModelError("MeterAdditionStatus", string.Format(unknownError, meterId));
         //               break;
         //        }
         //    }
         //    // //now that we have either added or alerted the user, send them back tot he edit page again with the new data - do this if we need to add status messages
         //    var item = (new CollectionsFactory()).GetCollectionConfiguration(model.CollectionId, model.CustomerId, true, true);
         //    return View("EditCollection", item);
         // //   return RedirectToAction("EditCollection", "Collections", new { routeId = model.CollectionId, customerId = model.CustomerId });
         //}
    }
}
