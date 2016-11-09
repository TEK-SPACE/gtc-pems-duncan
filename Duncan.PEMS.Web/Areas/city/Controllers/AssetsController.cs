/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 12/20/2013       Sergey Ostrerov            Enhancement/Issue DPTXPEMS-14 - AssetID Change: Allow manually entering AssetID
 * 
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Errors;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Grids;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.Extensions;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;
using NPOI.HSSF.UserModel;
using WebMatrix.WebData;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class AssetsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const string SavedActionableMassEditItemsId = "__SavedActionableMassEditItems";
        private const string SavedAssetIdentifiersId = "__SavedAssetIdentifiers";

        public static int isNewAssetStatus = 0;

        public ActionResult Index()
        {
            return View();
        }

        #region "Listings"

        public JsonResult SetActionableMassEditItems(long[] uns)
        {
            Session[SavedActionableMassEditItemsId] = uns;
            return null;
        }

        private long[] GetActionableMassEditItems()
        {
            if (Session[SavedActionableMassEditItemsId] != null)
            {
                var retVal = Session[SavedActionableMassEditItemsId] as long[];
                //clear the session
                return retVal;
            }
            return null;
        }

        public JsonResult SaveActionableMassEditItems(List<AssetIdentifier> items)
        {
            Session[SavedAssetIdentifiersId] = items;
            return null;
        }

        private List<AssetIdentifier> GetSavedAssetIdentifiers()
        {
            if (Session[SavedAssetIdentifiersId] != null)
            {
                var retVal = Session[SavedAssetIdentifiersId] as List<AssetIdentifier>;
                //clear the session
                return retVal;
            }
            return null;
        }

       // public ActionResult Action(int AssetTypeId, int CustomerID)
        public ActionResult Action(FormCollection forms)
        {
            ///////////////////////////////
            //FormCollection forms = new FormCollection();
            int AssetTypeId = Convert.ToInt32(forms["AssetTypeId"].ToString());
            int CustomerID = Convert.ToInt32(forms["CustomerID"].ToString());
            string value = forms["ddlActionValue"].ToString();
            int? specialAction = null;
            switch (value)
            {
                case "1":
                    specialAction = 1;
                    break;
                case "241":
                    specialAction = 241;
                    break;
                case "242":
                    specialAction = 242;
                    break;
                case "243":
                    specialAction = 243;
                    break;
                case "-1":
                    specialAction = -1;
                    break;
                default:
                    break;
            }


            if (specialAction.HasValue)
            {
                if (AssetTypeId == 0)
                {
                    var idsLong = GetActionableMassEditItems();
                    int[] intArray = Array.ConvertAll<long, Int32>(idsLong, delegate(long i)
                    {
                        return (Int32)i;
                    });


                    (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                     .SetSpecialAction(intArray, CustomerID, (Int32)AssetAreaId.Meter, specialAction.Value);
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            ///////////////////////////////


            //get the type base don the meter group id. if it is -1, it is spaces
            var assetClass = AssetFactory.GetAssetClass(AssetTypeId);
            if (AssetTypeId == -1)
                assetClass = AssetClass.Space;

            //create a list of asset identifiyers 
            var ids = GetActionableMassEditItems();
            string viewName = "MassEdit" + assetClass;
            var assetIds = new List<AssetIdentifier>();
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var assetId = new AssetIdentifier { CustomerId = CustomerID, AssetId = id };
                    switch (assetClass)
                    {
                        case AssetClass.Meter:
                            assetId.AreaId = (int)AssetAreaId.Meter;
                            break;
                        case AssetClass.Cashbox:
                            assetId.AreaId = (int)AssetAreaId.Cashbox;
                            break;
                        case AssetClass.Gateway:
                            assetId.AreaId = (int)AssetAreaId.Gateway;
                            break;
                        case AssetClass.Sensor:
                            assetId.AreaId = (int)AssetAreaId.Sensor;
                            break;
                        case AssetClass.Mechanism:
                            assetId.AreaId = (int)AssetAreaId.Mechanism;
                            break;
                        case AssetClass.Space:
                            assetId.AreaId = -1;
                            break;
                        case AssetClass.DataKey:
                            assetId.AreaId = (int) AssetAreaId.DataKey;
                            break;
                    }
                    assetIds.Add(assetId);
                }
            }
            SaveActionableMassEditItems(assetIds);
            return RedirectToAction(viewName);
        }

        public ActionResult GetSummaryItems([DataSourceRequest] DataSourceRequest request, int customerId, string assetType)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models
            int total = 0;

            var items = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetSummaryModels(customerId, assetType, AssetListGridType.Summary, request, out total);
            
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetConfigurationItems([DataSourceRequest] DataSourceRequest request, int customerId, string assetType)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models
            int total = 0;
            var items = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetSummaryModels(customerId, assetType, AssetListGridType.Configuration, request, out total);
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOccupancyItems([DataSourceRequest] DataSourceRequest request, int customerId, string assetType)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models
            //get models
            int total = 0;
            var items = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetSummaryModels(customerId, assetType, AssetListGridType.Occupancy, request, out total);
            
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFunctionalItems([DataSourceRequest] DataSourceRequest request, int customerId, string assetType)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models

            //get models
            int total = 0;
            var items = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetSummaryModels(customerId, assetType, AssetListGridType.FunctionalStatus, request, out total);
            
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Gets requested Kendo grid
        /// GET: Events/GetGrid/
        /// </summary>
        public ActionResult GetGrid(string gridType)
        {
            string viewName;
            JsonResult customGridData;
            switch (gridType)
            {
                case "Summary":
                    viewName = "_SummaryGrid";
                    customGridData = GetGridData(CurrentCity.Id, CurrentController, "GetSummaryAssets");
                    break;
                case "Configuration":
                    viewName = "_ConfigurationGrid";
                    customGridData = GetGridData(CurrentCity.Id, CurrentController, "GetConfigurationAssets");
                    break;
                case "Occupancy":
                    viewName = "_OccupancyGrid";
                    customGridData = GetGridData(CurrentCity.Id, CurrentController, "GetOccupancyAssets");
                    break;
                case "Functional Status":
                    viewName = "_FunctionalStatusGrid";
                    customGridData = GetGridData(CurrentCity.Id, CurrentController, "GetFunctionalStatusAssets");
                    break;
                default:
                    return null;
            }

            // Get the partial view and render the html to a string
            string html = string.Empty;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                html = sw.GetStringBuilder().ToString();
            }

            // Concatenate the data into an anonymous typed object
            var data = new
            {
                view = html,
                customGridData
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFilterValues()
        {
            var assetFactory = new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());
            var assetTypes = assetFactory.GetAssetTypesDdlItems(CurrentCity.Id);
            var operationalStatuss = assetFactory.GetOperationalStatuses();
            var resetTypes = assetFactory.GetResetTypes();
            var hasSensorTypes = assetFactory.GetHasSensorTypes();
            // Concatenate the data into an anonymous typed object
            var filterValues = new { assetTypes, operationalStatuss, resetTypes, hasSensorTypes };

            // return as JSON
            return Json(filterValues, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetReasonForChangeValues()
        {
            var reasonsForChange = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetReasonsForChange();

            // return as JSON
            return Json(reasonsForChange, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region "Exporting
        private IEnumerable<AssetListModel> GetExportData(DataSourceRequest request, int size, int customerId, string assetType, AssetListGridType gridType)
        {
            //get the user models
            //no paging, we are bringing back all the items
            //request.Page = 1;
            //request.PageSize = size;
            int total = 0;
            var items = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetSummaryModels(customerId, assetType, gridType, request, out total);
            return items;
        }

        #region Excel


        public FileResult ExportToExcel([DataSourceRequest] DataSourceRequest request, string gridType, int customerId, string assetType)
        {
            switch (gridType)
            {
                case "Summary":
                    return ExportSummaryGridToExcel(request, customerId, assetType, AssetListGridType.Summary);
                case "Configuration":
                    return ExporConfigurationGridToExcel(request, customerId, assetType, AssetListGridType.Configuration);
                case "Occupancy":
                    return ExportOccupancyGridToExcel(request, customerId, assetType, AssetListGridType.Occupancy);
                case "Functional Status":
                    return ExportFunctionalGridToExcel(request, customerId, assetType, AssetListGridType.FunctionalStatus);
                default:
                    return null;
            }
        }

        private FileResult ExportSummaryGridToExcel([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.ExcelExportCount, customerId, assetType, gridType);
            var items = new List<SummaryAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetSummaryAssets", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Assets_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        private FileResult ExporConfigurationGridToExcel([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.ExcelExportCount, customerId, assetType, gridType);
            var items = new List<ConfigurationAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetConfigurationAssets", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Assets_Configuration_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        private FileResult ExportOccupancyGridToExcel([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.ExcelExportCount, customerId, assetType, gridType);
            var items = new List<OccupancyAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetOccupancyAssets", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Assets_Occupancy_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        private FileResult ExportFunctionalGridToExcel([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.ExcelExportCount, customerId, assetType, gridType);
            var items = new List<FunctionalStatusAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetFunctionalStatusAssets", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Assets_Functional_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        #region Export History to Excel

        private void AddHistoryColumnHeaderForExcel(NPOI.SS.UserModel.IRow headerRow, List<GridData> gridData)
        {
            int columnNumber = 0;
            headerRow.CreateCell(columnNumber++).SetCellValue(GetLocalizedGridTitle(gridData, "Activation Date"));
            headerRow.CreateCell(columnNumber++).SetCellValue(GetLocalizedGridTitle(gridData, "Deactivation Date"));
            headerRow.CreateCell(columnNumber++).SetCellValue(GetLocalizedGridTitle(gridData, "Configuration Name"));
            headerRow.CreateCell(columnNumber++).SetCellValue(GetLocalizedGridTitle(gridData, "User Name"));
            headerRow.CreateCell(columnNumber++).SetCellValue(GetLocalizedGridTitle(gridData, "Reason"));
        }

        private void AddHistoryRowForExcel(NPOI.SS.UserModel.IRow row, AssetBaseModel model)
        {
            int columnNumber = 0;
            row.CreateCell(columnNumber++).SetCellValue(model.RecordDateDisplay);
            row.CreateCell(columnNumber++).SetCellValue(model.RecordSuperceededDateDisplay);
            row.CreateCell(columnNumber++).SetCellValue(model.ConfigurationName);
            row.CreateCell(columnNumber++).SetCellValue(model.LastUpdatedBy);
            row.CreateCell(columnNumber++).SetCellValue(model.LastUpdatedReasonDisplay);
        }

        public FileResult ExportCashboxHistoryToExcel([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {

            // Get history data
            int total;
            var data = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            // Create a header row
            AddHistoryColumnHeaderForExcel(sheet.CreateRow(rowNumber++), (new GridFactory()).GetGridData(CurrentController, "ViewCashboxHistory", CurrentCity.Id));

            //Populate the sheet with values from the grid data
            foreach (CashboxViewModel item in data)
            {
                // Create a new row
                AddHistoryRowForExcel(sheet.CreateRow(rowNumber++), item);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "CashboxHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportSensorHistoryToExcel([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {

            // Get history data
            int total;
            var data = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            // Create a header row
            AddHistoryColumnHeaderForExcel(sheet.CreateRow(rowNumber++), (new GridFactory()).GetGridData(CurrentController, "ViewSensorHistory", CurrentCity.Id));

            //Populate the sheet with values from the grid data
            foreach (SensorViewModel item in data)
            {
                // Create a new row
                AddHistoryRowForExcel(sheet.CreateRow(rowNumber++), item);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "SensorHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportGatewayHistoryToExcel([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {

            // Get history data
            int total;
            var data = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            // Create a header row
            AddHistoryColumnHeaderForExcel(sheet.CreateRow(rowNumber++), (new GridFactory()).GetGridData(CurrentController, "ViewGatewayHistory", CurrentCity.Id));

            //Populate the sheet with values from the grid data
            foreach (GatewayViewModel item in data)
            {
                // Create a new row
                AddHistoryRowForExcel(sheet.CreateRow(rowNumber++), item);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "GatewayHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportMeterHistoryToExcel([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {

            // Get history data
            int total;
            var data = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            // Create a header row
            AddHistoryColumnHeaderForExcel(sheet.CreateRow(rowNumber++), (new GridFactory()).GetGridData(CurrentController, "ViewMeterHistory", CurrentCity.Id));

            //Populate the sheet with values from the grid data
            foreach (MeterViewModel item in data)
            {
                // Create a new row
                AddHistoryRowForExcel(sheet.CreateRow(rowNumber++), item);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "MeterHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportSpaceHistoryToExcel([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {

            // Get history data
            int total;
            var data = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            // Create a header row
            AddHistoryColumnHeaderForExcel(sheet.CreateRow(rowNumber++), (new GridFactory()).GetGridData(CurrentController, "ViewSpaceHistory", CurrentCity.Id));

            //Populate the sheet with values from the grid data
            foreach (SpaceViewModel item in data)
            {
                // Create a new row
                AddHistoryRowForExcel(sheet.CreateRow(rowNumber++), item);
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "SpaceHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        #endregion

        #endregion

        #region PDF

        public FileResult ExportToPdf([DataSourceRequest] DataSourceRequest request, string gridType, int customerId, string assetType)
        {
            switch (gridType)
            {
                case "Summary":
                    return ExportSummaryGridToPdf(request, customerId, assetType, AssetListGridType.Summary);
                case "Configuration":
                    return ExporConfigurationGridToPdf(request, customerId, assetType, AssetListGridType.Configuration);
                case "Occupancy":
                    return ExportOccupancyGridToPdf(request, customerId, assetType, AssetListGridType.Occupancy);
                case "Functional Status":
                    return ExportFunctionalGridToPdf(request, customerId, assetType, AssetListGridType.FunctionalStatus);
                default:
                    return null;
            }
        }

        private FileResult ExportSummaryGridToPdf([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.PdfExportCount, customerId, assetType, gridType);
            var items = new List<SummaryAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetSummaryAssets", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Assets_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExporConfigurationGridToPdf([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.PdfExportCount, customerId, assetType, gridType);
            var items = new List<ConfigurationAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetConfigurationAssets", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Assets_Configuration_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportOccupancyGridToPdf([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.PdfExportCount, customerId, assetType, gridType);
            var items = new List<OccupancyAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetOccupancyAssets", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Assets_Occupancy_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private FileResult ExportFunctionalGridToPdf([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.PdfExportCount, customerId, assetType, gridType);
            var items = new List<FunctionalStatusAssetListModel>();
            items = AssetFactory.CastToDerived(data, items);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);
            filters.Add(new FilterDescriptor { Member = "Asset Type", Value = assetType });
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetFunctionalStatusAssets", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Assets_Functional_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        #region Export History to PDF

        private PdfPTable HistoryTableForPdf()
        {
            var dataTable = new PdfPTable(5) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            return dataTable;
        }

        private void AddHistoryColumnHeaderForPdf(PdfPTable dataTable, List<GridData> gridData)
        {
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Activation Date"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Deactivation Date"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Configuration Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "User Name"));
            dataTable.AddCell(GetLocalizedGridTitle(gridData, "Reason"));
            dataTable.CompleteRow();
            dataTable.HeaderRows = 1;
        }

        private void AddHistoryRowForPdf(PdfPTable dataTable, AssetBaseModel model)
        {
            dataTable.AddCell(model.RecordDateDisplay);
            dataTable.AddCell(model.RecordSuperceededDateDisplay);
            dataTable.AddCell(model.ConfigurationName);
            dataTable.AddCell(model.LastUpdatedBy);
            dataTable.AddCell(model.LastUpdatedReasonDisplay);
            dataTable.CompleteRow();
        }

        public FileResult ExportCashboxHistoryToPdf([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            // step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            // first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));

            int total;
            var data = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            // Create history table for pdf
            var dataTable = HistoryTableForPdf();

            // Adding headers
            AddHistoryColumnHeaderForPdf(dataTable, (new GridFactory()).GetGridData(CurrentController, "ViewCashboxHistory", CurrentCity.Id));

            dataTable.DefaultCell.BorderWidth = 1;

            int rowIndex = 0;
            foreach (CashboxViewModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                AddHistoryRowForPdf(dataTable, item);
            }

            // Add table to the document
            document.Add(dataTable);

            // This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "CashboxHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        public FileResult ExportSensorHistoryToPdf([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            // step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            // first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));

            int total;
            var data = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            // Create history table for pdf
            var dataTable = HistoryTableForPdf();

            // Adding headers
            AddHistoryColumnHeaderForPdf(dataTable, (new GridFactory()).GetGridData(CurrentController, "ViewSensorHistory", CurrentCity.Id));

            dataTable.DefaultCell.BorderWidth = 1;

            int rowIndex = 0;
            foreach (SensorViewModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                AddHistoryRowForPdf(dataTable, item);
            }

            // Add table to the document
            document.Add(dataTable);

            // This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "SensorHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        public FileResult ExportGatewayHistoryToPdf([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            // step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            // first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));

            int total;
            var data = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            // Create history table for pdf
            var dataTable = HistoryTableForPdf();

            // Adding headers
            AddHistoryColumnHeaderForPdf(dataTable, (new GridFactory()).GetGridData(CurrentController, "ViewGatewayHistory", CurrentCity.Id));

            dataTable.DefaultCell.BorderWidth = 1;

            int rowIndex = 0;
            foreach (GatewayViewModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                AddHistoryRowForPdf(dataTable, item);
            }

            // Add table to the document
            document.Add(dataTable);

            // This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GatewayHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        public FileResult ExportMeterHistoryToPdf([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            // step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            // first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));

            int total;
            var data = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            // Create history table for pdf
            var dataTable = HistoryTableForPdf();

            // Adding headers
            AddHistoryColumnHeaderForPdf(dataTable, (new GridFactory()).GetGridData(CurrentController, "ViewMeterHistory", CurrentCity.Id));

            dataTable.DefaultCell.BorderWidth = 1;

            int rowIndex = 0;
            foreach (MeterViewModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                AddHistoryRowForPdf(dataTable, item);
            }

            // Add table to the document
            document.Add(dataTable);

            // This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "MeterHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        public FileResult ExportSpaceHistoryToPdf([DataSourceRequest]DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // step 1: creation of a document-object
            var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

            // step 2: we create a memory stream that listens to the document
            var output = new MemoryStream();
            PdfWriter.GetInstance(document, output);

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            // first we have to add the filters they used
            document = AddFiltersToPdf(request, document);

            //then we add the data
            document.Add(new Paragraph("Results:  "));
            document.Add(new Paragraph(" "));

            int total;
            var data = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            // Create history table for pdf
            var dataTable = HistoryTableForPdf();

            // Adding headers
            AddHistoryColumnHeaderForPdf(dataTable, (new GridFactory()).GetGridData(CurrentController, "ViewSpaceHistory", CurrentCity.Id));

            dataTable.DefaultCell.BorderWidth = 1;

            int rowIndex = 0;
            foreach (SpaceViewModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                AddHistoryRowForPdf(dataTable, item);
            }

            // Add table to the document
            document.Add(dataTable);

            // This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "SpaceHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        #endregion


        #endregion

        #region CSV

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, string gridType, int customerId, string assetType)
        {
            switch (gridType)
            {
                case "Summary":
                    return ExportSummaryGridToCsv(request, customerId, assetType, AssetListGridType.Summary);
                case "Configuration":
                    return ExporConfigurationGridToCSV(request, customerId, assetType, AssetListGridType.Configuration);
                case "Occupancy":
                    return ExportOccupancyGridToCSV(request, customerId, assetType, AssetListGridType.Occupancy);
                case "Functional Status":
                    return ExportFunctionalGridToCSV(request, customerId, assetType, AssetListGridType.FunctionalStatus);
                default:
                    return null;
            }
        }

        private FileResult ExportSummaryGridToCsv([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.CsvExportCount, customerId, assetType, gridType);
            var configAssetModels = new List<SummaryAssetListModel>();
            configAssetModels = AssetFactory.CastToDerived(data, configAssetModels);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(configAssetModels, CurrentController, "GetSummaryAssets", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Assets_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExporConfigurationGridToCSV([DataSourceRequest] DataSourceRequest request, int customerId, string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.CsvExportCount, customerId, assetType, gridType);
            var configAssetModels = new List<ConfigurationAssetListModel>();
            configAssetModels = AssetFactory.CastToDerived(data, configAssetModels);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(configAssetModels, CurrentController, "GetConfigurationAssets", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Assets_Configuration_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportOccupancyGridToCSV([DataSourceRequest] DataSourceRequest request, int customerId,
                                                    string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.CsvExportCount, customerId, assetType, gridType);
            var configAssetModels = new List<OccupancyAssetListModel>();
            configAssetModels = AssetFactory.CastToDerived(data, configAssetModels);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(configAssetModels, CurrentController, "GetOccupancyAssets", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Assets_Occupancy_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private FileResult ExportFunctionalGridToCSV([DataSourceRequest] DataSourceRequest request, int customerId,
                                                     string assetType, AssetListGridType gridType)
        {
            var data = GetExportData(request, Constants.Export.CsvExportCount, customerId, assetType, gridType);
            var configAssetModels = new List<FunctionalStatusAssetListModel>();
            configAssetModels = AssetFactory.CastToDerived(data, configAssetModels);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(configAssetModels, CurrentController, "GetFunctionalStatusAssets", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Assets_Functional_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        private void AddHistoryColumnHeaderForCsv(StreamWriter writer, List<GridData> gridData)
        {
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Activation Date"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Deactivation Date"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Configuration Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "User Name"));
            AddCsvValue(writer, GetLocalizedGridTitle(gridData, "Reason"), true);
        }

        private void AddHistoryRowForCsv(StreamWriter writer, AssetBaseModel model)
        {
            AddCsvValue(writer, model.RecordDateDisplay);
            AddCsvValue(writer, model.RecordSuperceededDateDisplay);
            AddCsvValue(writer, model.ConfigurationName);
            AddCsvValue(writer, model.LastUpdatedBy);
            AddCsvValue(writer, model.LastUpdatedReasonDisplay, true);
        }

        public FileResult ExportCashboxHistoryToCsv([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // Get history records
            int total;
            var data = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);

            // Write column headers
            AddHistoryColumnHeaderForCsv(writer, (new GridFactory()).GetGridData(CurrentController, "ViewCashboxHistory", CurrentCity.Id));

            // Write rows
            foreach (CashboxViewModel item in data)
            {
                AddHistoryRowForCsv(writer, item);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "CashboxHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportSensorHistoryToCsv([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // Get history records
            int total;

            var data = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);

            // Write column headers
            AddHistoryColumnHeaderForCsv(writer, (new GridFactory()).GetGridData(CurrentController, "ViewSensorHistory", CurrentCity.Id));

            // Write rows
            foreach (SensorViewModel item in data)
            {
                AddHistoryRowForCsv(writer, item);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "SensorHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportGatewayHistoryToCsv([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // Get history records
            int total;

            var data = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);

            // Write column headers
            AddHistoryColumnHeaderForCsv(writer, (new GridFactory()).GetGridData(CurrentController, "ViewGatewayHistory", CurrentCity.Id));

            // Write rows
            foreach (GatewayViewModel item in data)
            {
                AddHistoryRowForCsv(writer, item);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "GatewayHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportMeterHistoryToCsv([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // Get history records
            int total;

            var data = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);

            // Write column headers
            AddHistoryColumnHeaderForCsv(writer, (new GridFactory()).GetGridData(CurrentController, "ViewMeterHistory", CurrentCity.Id));

            // Write rows
            foreach (MeterViewModel item in data)
            {
                AddHistoryRowForCsv(writer, item);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "MeterHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportSpaceHistoryToCsv([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            // Get history records
            int total;

            var data = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);

            // Write column headers
            AddHistoryColumnHeaderForCsv(writer, (new GridFactory()).GetGridData(CurrentController, "ViewSpaceHistory", CurrentCity.Id));

            // Write rows
            foreach (SpaceViewModel item in data)
            {
                AddHistoryRowForCsv(writer, item);
            }
            writer.Flush();
            output.Position = 0;

            return File(output, "text/comma-separated-values", "SpaceHistoryExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }


        #endregion

        #endregion


        #region Create Asset


        public ActionResult Create(int customerId)
        {
            AssetTypesModel model = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetAssetTypesModel(customerId);

            return View(model);
        }

        public void JavaGetRequestToURL()
        {
            try
            {
                string URI = ConfigurationSettings.AppSettings["JavaInternalBus"].ToString();
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
                req.Proxy = new System.Net.WebProxy(URI, true); //true means no proxy
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());

                string URIripnet = ConfigurationSettings.AppSettings["ripnet"].ToString();
                System.Net.WebRequest reqripnet = System.Net.WebRequest.Create(URIripnet);
                reqripnet.Proxy = new System.Net.WebProxy(URIripnet, true); //true means no proxy
                System.Net.WebResponse ripnet = reqripnet.GetResponse();
                System.IO.StreamReader srripnet = new System.IO.StreamReader(ripnet.GetResponseStream());

                string URIPAMEngine = ConfigurationSettings.AppSettings["PAMEngine"].ToString();
                System.Net.WebRequest reqPAMEngine = System.Net.WebRequest.Create(URIPAMEngine);
                reqPAMEngine.Proxy = new System.Net.WebProxy(URIPAMEngine, true); //true means no proxy
                System.Net.WebResponse respPAMEngine = reqPAMEngine.GetResponse();
                System.IO.StreamReader srPAMEngine = new System.IO.StreamReader(respPAMEngine.GetResponseStream());
                
            }
            catch (Exception ex)
            {
                string Error = ex.Message.ToString();
            }
        }

        [HttpPost]
        public ActionResult Create(string submitButton, AssetTypesModel model, FormCollection formValues)
        {
            isNewAssetStatus = 1;

            AssetTypeModel.EnterMode modeAssetEntered = (AssetTypeModel.EnterMode)int.Parse(formValues[AssetTypesModel.GroupNameMode].ToString());

            Meter newSMeter = null;
            Meter newMMeter = null;
            if (submitButton.Equals("CREATE"))
            {
                switch ((AssetTypeModel.AssetType)int.Parse(formValues[AssetTypesModel.GroupName].ToString()))
                {
                    case AssetTypeModel.AssetType.SingleSpaceMeter:
                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                newSMeter = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, true, model.StartBay, model.StartBay, model.MechSerialNo, model.LocationName);
                                break;
                            case 0:
                                newSMeter = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, true, model.StartBay, model.StartBay, modeAssetEntered, int.Parse(model.AssetID), model.MechSerialNo,model.LocationName);
                                break;
                        }
                        if (newSMeter != null) { JavaGetRequestToURL(); }
                        return RedirectToAction("EditMeter", new
                        {
                            customerId = CurrentCity.Id,
                            areaId = newSMeter.AreaID,
                            assetId = newSMeter.MeterId,
                            parentAssetId = 0,
                        });
                    case AssetTypeModel.AssetType.MultiSpaceMeter:

                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                newMMeter = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, false, model.StartBay, model.EndBay,"","");
                                break;
                            case 0:
                                newMMeter = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, false, model.StartBay, model.EndBay, modeAssetEntered, int.Parse(model.AssetID),"","");
                                break;
                        }
                        if (newSMeter != null) { JavaGetRequestToURL(); }
                        return RedirectToAction("EditMeter", new
                        {
                            customerId = CurrentCity.Id,
                            areaId = newMMeter.AreaID,
                            assetId = newMMeter.MeterId,
                            parentAssetId = 0,
                        });
                    case AssetTypeModel.AssetType.Cashbox:
                        CashBox cashBox = null;
                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                cashBox = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id);
                                break;
                            case 0:
                                cashBox = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, modeAssetEntered, int.Parse(model.AssetID));
                                break;
                        }
                        if (cashBox != null) { JavaGetRequestToURL(); }
                        return RedirectToAction("EditCashbox", new
                        {
                            customerId = CurrentCity.Id,
                            assetId = cashBox.CashBoxSeq,
                            parentAssetId = 0
                        });
                    case AssetTypeModel.AssetType.Gateway:
                        Gateway gateway = null;
                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                gateway = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id);
                                break;
                            case 0:
                                gateway = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, modeAssetEntered, int.Parse(model.AssetID));
                                break;
                        }
                        if (gateway != null) { JavaGetRequestToURL(); }

                        return RedirectToAction("EditGateway", new
                        {
                            customerId = CurrentCity.Id,
                            assetId = gateway.GateWayID,
                            parentAssetId = 0
                        });
                    case AssetTypeModel.AssetType.Mechanism:
                        MechMaster mechanism = null;
                        var mechanismFactory =new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(),CurrentCity.LocalTime);

                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                mechanism = mechanismFactory.Create(CurrentCity.Id, model.SerialNumber);
                                break;
                            case 0:
                                mechanism = mechanismFactory.Create(CurrentCity.Id, modeAssetEntered, int.Parse(model.AssetID), model.SerialNumber);
                                break;
                        }
                        if (mechanism != null) { JavaGetRequestToURL(); }
                        return RedirectToAction("EditMechanism", new
                        {
                            customerId = CurrentCity.Id,
                            assetId = mechanism == null ? 0 : mechanism.MechIdNumber,
                            parentAssetId = 0
                        });
                    case AssetTypeModel.AssetType.DataKey:
                        DataKey dataKey = null;
                        var dataKeyFactory = new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                dataKey = dataKeyFactory.Create(CurrentCity.Id);
                                break;
                            case 0:
                                dataKey = dataKeyFactory.Create(CurrentCity.Id, modeAssetEntered, int.Parse(model.AssetID));
                                break;
                        }
                        return RedirectToAction("EditDataKey", new
                        {
                            customerId = CurrentCity.Id,
                            assetId = dataKey == null ? 0 : dataKey.DataKeyIdNumber,
                            parentAssetId = 0
                        });


                    case AssetTypeModel.AssetType.Sensor:
                        // Was a gateway or space selected?
                        int gatewayId = int.Parse(formValues["SensorGatewayList"].ToString());
                        Int64 spaceId = Int64.Parse(formValues["SensorSpaceList"].ToString());
                        Sensor sensor = null;

                        switch ((int)modeAssetEntered)
                        {
                            case 1:
                                sensor = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(CurrentCity.Id, gatewayId, spaceId);
                                break;
                            case 0:
                                sensor = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Create(modeAssetEntered, int.Parse(model.AssetID), CurrentCity.Id, gatewayId, spaceId);
                                break;
                        }
                        if (sensor != null) { JavaGetRequestToURL(); }
                        return RedirectToAction("EditSensor", new
                        {
                            customerId = CurrentCity.Id,
                            assetId = sensor.SensorID,
                            parentAssetId = 0
                        });
                    default:
                        return RedirectToAction("Index", new { rtn = true });
                }


            }
            else if (submitButton.Equals("IMPORT"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }
            else //(submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }
        }


        //[HttpPost]
        //public ActionResult GetSensorSpacesAndGateways(int customerId)
        //{
        //    // Get parking spaces without sensor.
         //   var spaces = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SpacesWithoutSensors(customerId);

        //    // Get gateways
        //    var gateways = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GatewayList(customerId);

        //   // return Json(new { Spaces = spaces, Gateways = gateways });

        //    var jsonResult = Json(new { Spaces = spaces, Gateways = gateways });
        //    jsonResult.MaxJsonLength = int.MaxValue;
        //    return jsonResult;
        //}

        [HttpPost]
        public ActionResult GetSensorSpacesAndGateways(int customerId)
        {
            // Get meters.
            var meters = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetAllAssets(customerId);
            // Get parking spaces without sensor.

            // Get gateways
            var gateways = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GatewayList(customerId);

            // return Json(new { Spaces = spaces, Gateways = gateways });

            var jsonResult = Json(new { Meters = meters, Gateways = gateways });
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetAssociatedSpaceIDs(int customerId, int meterID)
        {
            var spaces = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SpacesWithoutSensors(customerId, meterID);
            var jsonResult = Json(new { Spaces = spaces });
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Meters

        public ActionResult ViewMeterPending(int customerId, int areaId, int assetId)
        {
            MeterViewModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(customerId, areaId, assetId);
            return View(model);
        }


        public ActionResult ViewMeterHistory(int customerId, int areaId, int assetId)
        {
            AssetHistoryModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(customerId, areaId, assetId);
            return View(model);
        }

        public ActionResult ViewMeterHistoryDetail(int customerId, int areaId, int assetId, Int64 auditId)
        {
            MeterViewModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(customerId, areaId, assetId, auditId);
            return View(model);
        }

        public ActionResult ViewMeter(int customerId, int areaId, int assetId, bool viewOnly = false, Int64 activeSpaceId = 0, int activeSensorId = 0)
        {
            MeterViewModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(customerId, areaId, assetId);
            model.ViewOnly = viewOnly;
            model.ActiveSpace = activeSpaceId;
            model.ActiveSensor = activeSensorId;
            return View(model);
        }


        [HttpPost]
        public ActionResult ViewMeter(string submitButton, MeterViewModel model, int startBay, int endBay)
        {
            if (submitButton.Equals("COPY"))
            {
                int clonedMeterId = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model, startBay, endBay);

                if (clonedMeterId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    return View((new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(model.CustomerId, model.AreaId, (int)model.AssetId));
                }

                return RedirectToAction("EditMeter", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedMeterId, parentAssetId = (int)model.AssetId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }



        public ActionResult ViewMeterToSpaceMapping(int customerId, int areaId, int assetId, bool viewOnly = false)
        {
            MeterSpacesModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetMeterSpacesModel(customerId, areaId, assetId);
            model.ViewOnly = viewOnly;
            return View(model);
        }

        public ActionResult EditMeterToSpaceMapping(int customerId, int areaId, int assetId)
        {
            MeterSpacesModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetMeterSpacesModel(customerId, areaId, assetId);

            // Store the present list of spaces in Session[SessionSpacesRepositoryKey]
            SessionSpacesRepository.Clear();
            SessionSpacesRepository.AddRange(model.Spaces);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditMeterToSpaceMapping(MeterSpacesModel model)
        {
            // Add the sesson-based list of spaces to model.
            model.Spaces.AddRange(SessionSpacesRepository);

            // Save the changed and new spaces.
            (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetMeterSpacesModel(model);

            // Get the updated model and return to view.
            model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetMeterSpacesModel(model.CustomerId, model.AreaId, (int)model.AssetId);

            // Store the present list of spaces in Session[SessionSpacesRepositoryKey]
            SessionSpacesRepository.Clear();
            SessionSpacesRepository.AddRange(model.Spaces);

            return View(model);
        }


        private MeterSpacesModel RebuildMeterSpacesModel(FormCollection formValues)
        {
            // Determine  CustomerId, AreaId and MeterId from form collection

            int customerId = int.Parse(formValues["CustomerId"].ToString());
            int areaId = int.Parse(formValues["AreaId"].ToString());
            int assetId = int.Parse(formValues["AssetId"].ToString());

            // Get an original copy of the CustomerAssetsModel.  This will ensure that any items that are
            // not in the formValues collection are still accounted for.

            MeterSpacesModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetMeterSpacesModel(customerId, areaId, assetId);

            // Set all of the model.Spaces IsActive flag to false.  This may be set true from form collection checkboxes.
            foreach (var space in model.Spaces)
            {
                space.State = AssetStateType.Pending;
            }

            // Walk the form fields and set any values in the model to values reflected by
            // the form fields.  Looking for "CHK_######".  If found, set the model.Spaces[#######] IsActive to true 
            foreach (var formValueKey in formValues.Keys)
            {
                string[] tokens = formValueKey.ToString().Split('_');
                if (tokens.Length == 2)
                {
                    // token [1] is the space id
                    // This should always return a space
                    var space = model.Spaces.FirstOrDefault(m => m.SpaceId.Equals(tokens[1]));
                    if (space != null)
                    {
                        space.State = AssetStateType.Current;
                    }
                }
            }
            return model;
        }



        [HttpPost]
        public ActionResult ResetMeter(int customerId, int areaId, int assetId)
        {
            // Reset the meter.  Return text string status
            bool success = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Reset(
                new AssetIdentifier() { CustomerId = customerId, AreaId = areaId, AssetId = assetId }, WebSecurity.CurrentUserId);

            string text = success
                              ? "Meter " + assetId.ToString() + " has been reset."
                              : "Failed to reset meter " + assetId.ToString() + "for area " + areaId.ToString() + "!";

            return Json(new { Success = success, Description = text });
        }

        [HttpPost]
        public ActionResult AddTimeToMeter(int customerId, int areaId, int assetId, int minutesToAdd, int bayNumber)
        {
            // Add time to the meter.  Return text string status
            bool success = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).AddTime(new AssetIdentifier() { CustomerId = customerId, AreaId = areaId, AssetId = assetId },
                minutesToAdd, bayNumber, WebSecurity.CurrentUserId);

            string text = success
                              ? "Time has been added to meter " + assetId.ToString() + ", bay number " + bayNumber.ToString() + "."
                              : "Failed to add time to meter " + assetId.ToString() + "!";

            return Json(new { Success = success, Description = text });
        }


        [HttpPost]
        public ActionResult AddSpacesToMeter(int customerId, int areaId, int assetId, int bayStart, int bayEnd)
        {
            string error;

            // Add spaces to the meter.
            return Json(new
            {
                Success = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).AddSpacesToMeter(customerId, areaId, assetId, bayStart, bayEnd, out error),
                ErrorDescription = error
            });
        }




        [HttpPost]
        public ActionResult AddSpaceToMeter(int customerId, int areaId, int assetId, int bayNumber)
        {
            string error;

            // Add space to the meter.
            return Json(new
            {
                Success = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).AddSpacesToMeter(customerId, areaId, assetId, bayNumber, bayNumber, out error),
                ErrorDescription = error
            });
        }




        [HttpPost]
        public ActionResult GetMeterBays(int customerId, int areaId, int assetId)
        {
            // Get bay ids associated with meter.
            var meterSpacesModel = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetMeterSpacesModel(customerId, areaId, assetId);

            return Json(new { Success = meterSpacesModel != null, Model = meterSpacesModel });
        }


        public ActionResult EditMeter(int customerId, int areaId, int assetId, int parentAssetId)
        {
            MeterEditModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetEditModel(customerId, areaId, assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = areaId,
                    AssetId = parentAssetId

                };
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMeter(string submitButton, MeterEditModel model, FormCollection formColl, string returnUrl, int startBay, int endBay)
        {
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("SPACES"))
            {
                return RedirectToAction("EditMeterToSpaceMapping",
                    new { customerId = model.CustomerId, areaId = model.AreaId, assetId = model.AssetId });
            }
            else if (submitButton.Equals("COPY"))
            {
                int clonedMeterId = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model, startBay, endBay);
                if (clonedMeterId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem()
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditMeter", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedMeterId, parentAssetId = (int)model.AssetId });
            }



            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);


            #region Work-Around for MVC Handling of Null DateTime Elements
            // If DateTime controls were left blank they will be represented in the model by DateTime.MinValue
            // In this case remove the validation error since we allow "blanks" in these dates.
            // THis is fundamentally a work-around for how MVC treates "null dates".
            if (model.NextPrevMaint == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("NextPrevMaint"))
                    ModelState["NextPrevMaint"].Errors.Clear();
            }
            if (model.WarrantyExpiration == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("WarrantyExpiration"))
                    ModelState["WarrantyExpiration"].Errors.Clear();
            }
            if (model.Configuration.DateInstalled == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("Configuration.DateInstalled"))
                    ModelState["Configuration.DateInstalled"].Errors.Clear();
            }
            #endregion


            // Save values
            if (!ModelState.IsValid)
            {
                // Refresh list values
                (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                model.Saved = false;
                isNewAssetStatus = 0;
                return View(model);
            }
            else
            {
                // Save updates.
                (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetEditModel(model);
                
                JavaGetRequestToURL();

                if (model.ClonedFromAsset != null)
                {
                    // Refresh list values
                    (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
                else
                {

                    (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
            }
        }

        public ActionResult MassEditMeter()
        {
            MeterMassEditModel model = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());
            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditMeter(string submitButton, MeterMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);


            // Save values
            if (!ModelState.IsValid)
            {
                (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshMassEditModel(model);
                return View(model);
            }
            else
            {
                // Saved and stay on same page.
                (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetMassEditModel(model);
                return RedirectToAction("Index", new { rtn = true });
            }
        }



        #endregion

        #region Spaces

        public ActionResult ViewSpacePending(int customerId, Int64 assetId)
        {
            SpaceViewModel model = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(assetId);
            return View(model);
        }

        public ActionResult ViewSpaceHistory(int customerId, Int64 assetId)
        {
            AssetHistoryModel model = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(assetId);
            return View(model);
        }

        public ActionResult ViewSpaceHistoryDetail(int customerId, Int64 assetId, Int64 auditId)
        {
            SpaceViewModel model = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(assetId, auditId);
            return View(model);
        }


        public ActionResult ViewSpace(int customerId, Int64 assetId, bool viewOnly = false, int activeSensorId = 0)
        {
            SpaceViewModel model = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(assetId);
            model.ViewOnly = viewOnly;
            model.ActiveSensor = activeSensorId;
            return View(model);
        }


        [HttpPost]
        public ActionResult ViewSpace(string submitButton, SpaceViewModel model)
        {
            if (submitButton.Equals("COPY"))
            {
                Int64 clonedSpaceId = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);

                if (clonedSpaceId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying space."
                    };

                    return View((new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(model.AssetId));
                }

                return RedirectToAction("EditSpace", new { assetId = clonedSpaceId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }



        public ActionResult EditSpace(int customerId, Int64 assetId, Int64 parentAssetId)
        {
            SpaceEditModel model = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetEditModel(assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = 0,
                    AssetId = parentAssetId
                };
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSpace(string submitButton, SpaceEditModel model, FormCollection formColl)
        {
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("COPY"))
            {
                Int64 clonedSpaceId = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);
                if (clonedSpaceId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditSpace", new { customerId = model.CustomerId, assetId = clonedSpaceId, parentAssetId = (int)model.AssetId });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            // Save values
            if (!ModelState.IsValid)
            {
                // Refresh list values
                (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                model.Saved = false;
                return View(model);
            }
            else
            {
                // Save updates.
                (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetEditModel(model);

                if (model.ClonedFromAsset != null)
                {
                    // Refresh list values
                    (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
                else
                {
                    (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                    //return RedirectToAction("ViewSpace", new { customerId = model.CustomerId, assetId = model.AssetId });
                }
            }
        }

        public ActionResult MassEditSpace()
        {
            SpaceMassEditModel model = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());
            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditSpace(string submitButton, SpaceMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }

            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            // Save values
            if (!ModelState.IsValid)
            {
                (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshMassEditModel(model);
                return View(model);
            }
            else
            {
                // Saved and stay on same page.
                (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetMassEditModel(model);
                return RedirectToAction("Index", new { rtn = true });
            }
        }


        #endregion

        #region Cashboxes

        public ActionResult ViewCashboxPending(int customerId, int areaId, int assetId)
        {
            CashboxViewModel model = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult ViewCashboxHistory(int customerId, int areaId, int assetId)
        {
            AssetHistoryModel model = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult ViewCashboxHistoryDetail(int customerId, int areaId, int assetId, Int64 auditId)
        {
            CashboxViewModel model = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(customerId, assetId, auditId);
            return View(model);
        }

        public ActionResult ViewCashbox(int customerId, int assetId)
        {
            CashboxViewModel model = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(customerId, assetId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ViewCashbox(string submitButton, CashboxViewModel model)
        {
            if (submitButton.Equals("COPY"))
            {
                int clonedCashboxId = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);

                if (clonedCashboxId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying cashbox."
                    };

                    return View((new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(model.CustomerId, (int)model.AssetId));
                }

                return RedirectToAction("EditCashbox", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedCashboxId, parentAssetId = (int)model.AssetId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }


        public ActionResult EditCashbox(int customerId, int assetId, int parentAssetId)
        {
            CashboxEditModel model = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetEditModel(customerId, assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = 0,
                    AssetId = parentAssetId
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCashbox(string submitButton, CashboxEditModel model, FormCollection formColl, string returnUrl)
        {
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("COPY"))
            {
                int clonedCashboxId = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);
                if (clonedCashboxId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditCashbox", new { customerId = model.CustomerId, assetId = clonedCashboxId, parentAssetId = (int)model.AssetId });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);


            #region Work-Around for MVC Handling of Null DateTime Elements
            // If DateTime controls were left blank they will be represented in the model by DateTime.MinValue
            // In this case remove the validation error since we allow "blanks" in these dates.
            // THis is fundamentally a work-around for how MVC treates "null dates".
            if (model.NextPrevMaint == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("NextPrevMaint"))
                    ModelState["NextPrevMaint"].Errors.Clear();
            }
            if (model.WarrantyExpiration == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("WarrantyExpiration"))
                    ModelState["WarrantyExpiration"].Errors.Clear();
            }
            #endregion


            // Save values
            if (!ModelState.IsValid)
            {
                // Refresh list values
                (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                model.Saved = false;
                return View(model);
            }
            else
            {
                // Save updates.
                (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetEditModel(model);
                JavaGetRequestToURL();
                if (model.ClonedFromAsset != null)
                {
                    // Refresh list values
                    (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
                else
                {
                    //if there is a returnUrl defined, we need to send there instead of the vew meter. this takes affect if they come from the create asset, etc.
                    //if (!string.IsNullOrEmpty(returnUrl))
                    //    return Redirect(returnUrl);
                    //return RedirectToAction("ViewCashbox", new { customerId = model.CustomerId, assetId = model.AssetId });

                    (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
            }
        }

        public ActionResult MassEditCashbox()
        {
            CashboxMassEditModel model = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());

            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditCashbox(string submitButton, CashboxMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);


            // Save values
            if (!ModelState.IsValid)
            {
                (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshMassEditModel(model);
                return View(model);
            }
            else
            {
                // Saved and stay on same page.
                (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetMassEditModel(model);
                return RedirectToAction("Index", new { rtn = true });
            }
        }



        #endregion

        #region Gateways

        public ActionResult ViewGatewayPending(int customerId, int areaId, int assetId)
        {
            GatewayViewModel model = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult ViewGatewayHistory(int customerId, int areaId, int assetId)
        {
            AssetHistoryModel model = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult ViewGatewayHistoryDetail(int customerId, int areaId, int assetId, Int64 auditId)
        {
            GatewayViewModel model = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(customerId, assetId, auditId);
            return View(model);
        }

        public ActionResult ViewGateway(int customerId, int assetId)
        {
            GatewayViewModel model = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(customerId, assetId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ViewGateway(string submitButton, GatewayViewModel model)
        {
            if (submitButton.Equals("COPY"))
            {
                int clonedGatewayId = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);

                if (clonedGatewayId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying gateway."
                    };

                    return View((new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(model.CustomerId, (int)model.AssetId));
                }

                return RedirectToAction("EditGateway", new { customerId = model.CustomerId, assetId = clonedGatewayId, parentAssetId = model.AssetId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }


        public ActionResult EditGateway(int customerId, int assetId, int parentAssetId)
        {
            GatewayEditModel model = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetEditModel(customerId, assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = model.AreaId,
                    AssetId = parentAssetId
                };
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditGateway(string submitButton, GatewayEditModel model, FormCollection formColl, string returnUrl)
        {
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("COPY"))
            {
                int clonedGatewayId = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);
                if (clonedGatewayId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditGateway", new { customerId = model.CustomerId, assetId = clonedGatewayId, parentAssetId = (int)model.AssetId });
            }

            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);



            #region Work-Around for MVC Handling of Null DateTime Elements
            // If DateTime controls were left blank they will be represented in the model by DateTime.MinValue
            // In this case remove the validation error since we allow "blanks" in these dates.
            // THis is fundamentally a work-around for how MVC treates "null dates".
            if (model.NextPrevMaint == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("NextPrevMaint"))
                    ModelState["NextPrevMaint"].Errors.Clear();
            }
            if (model.WarrantyExpiration == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("WarrantyExpiration"))
                    ModelState["WarrantyExpiration"].Errors.Clear();
            }
            if (model.Configuration.DateInstalled == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("Configuration.DateInstalled"))
                    ModelState["Configuration.DateInstalled"].Errors.Clear();
            }
            #endregion


            // Save values
            if (!ModelState.IsValid)
            {
                // Refresh list values
                (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                model.Saved = false;
                return View(model);
            }
            else
            {
                // Save updates.
                (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetEditModel(model);
                JavaGetRequestToURL();
                if (model.ClonedFromAsset != null)
                {
                    // Refresh list values
                    (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
                else
                {
                    //if there is a returnUrl defined, we need to send there instead of the vew meter. this takes affect if they come from the create asset, etc.
                    //if (!string.IsNullOrEmpty(returnUrl))
                    //    return Redirect(returnUrl);
                    //return RedirectToAction("ViewGateway", new { customerId = model.CustomerId, assetId = model.AssetId });
                    (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
            }
        }

        [HttpPost]
        public ActionResult ResetGateway(int customerId, int assetId)
        {
            // Reset the meter.  Return text string status
            bool success = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Reset(
                new AssetIdentifier() { CustomerId = customerId, AssetId = assetId }, WebSecurity.CurrentUserId);

            string text = success
                              ? "Gateway " + assetId.ToString() + " has been reset."
                              : "Failed to reset gateway " + assetId.ToString() + "!";

            return Json(new { Success = success, Description = text });
        }



        public ActionResult MassEditGateway()
        {
            GatewayMassEditModel model = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());

            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditGateway(string submitButton, GatewayMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            // Save values
            if (!ModelState.IsValid)
            {
                (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshMassEditModel(model);
                return View(model);
            }
            else
            {
                // Saved and stay on same page.
                (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetMassEditModel(model);
                return RedirectToAction("Index", new { rtn = true });
            }
        }


        #endregion

        #region Sensors

        public ActionResult ViewSensorPending(int customerId, int areaId, int assetId)
        {
            SensorViewModel model = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult ViewSensorHistory(int customerId, int areaId, int assetId)
        {
            AssetHistoryModel model = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult ViewSensorHistoryDetail(int customerId, int areaId, int assetId, Int64 auditId)
        {
            SensorViewModel model = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(customerId, assetId, auditId);
            return View(model);
        }

        public ActionResult ViewSensor(int customerId, int assetId)
        {
            SensorViewModel model = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(customerId, assetId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ViewSensor(string submitButton, SensorViewModel model)
        {
            if (submitButton.Equals("COPY"))
            {
                int clonedSensorId = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);

                if (clonedSensorId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying sensor."
                    };

                    return View((new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(model.CustomerId, (int)model.AssetId));
                }

                return RedirectToAction("EditSensor", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedSensorId, parentAssetId = (int)model.AssetId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }


        public ActionResult EditSensor(int customerId, int assetId, int parentAssetId)
        {
            SensorEditModel model = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetEditModel(customerId, assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = 0,
                    AssetId = parentAssetId
                };
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSensor(string submitButton, SensorEditModel model, FormCollection formColl, string returnUrl)
        {
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("COPY"))
            {
                int clonedMeterId = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Clone(model);
                if (clonedMeterId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditSensor", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedMeterId, parentAssetId = (int)model.AssetId });
            }

            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);


            #region Work-Around for MVC Handling of Null DateTime Elements
            // If DateTime controls were left blank they will be represented in the model by DateTime.MinValue
            // In this case remove the validation error since we allow "blanks" in these dates.
            // THis is fundamentally a work-around for how MVC treates "null dates".
            if (model.NextPrevMaint == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("NextPrevMaint"))
                    ModelState["NextPrevMaint"].Errors.Clear();
            }
            if (model.WarrantyExpiration == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("WarrantyExpiration"))
                    ModelState["WarrantyExpiration"].Errors.Clear();
            }
            if (model.Configuration.DateInstalled == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("Configuration.DateInstalled"))
                    ModelState["Configuration.DateInstalled"].Errors.Clear();
            }
            #endregion


            // Save values
            if (!ModelState.IsValid)
            {
                // Refresh list values
                (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                model.Saved = false;
                return View(model);
            }
            else
            {
                // Save updates.
                (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetEditModel(model);
                JavaGetRequestToURL();
                if (model.ClonedFromAsset != null)
                {
                    // Refresh list values
                    (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
                else
                {
                    //if there is a returnUrl defined, we need to send there instead of the vew meter. this takes affect if they come from the create asset, etc.
                    //if (!string.IsNullOrEmpty(returnUrl))
                    //    return Redirect(returnUrl);
                    //return RedirectToAction("ViewSensor", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = model.AssetId });

                    (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshEditModel(model);
                    model.Saved = true;
                    isNewAssetStatus = 0;
                    return View(model);
                }
            }
        }


        [HttpPost]
        public ActionResult ResetSensor(int customerId, int assetId)
        {
            // Reset the meter.  Return text string status
            bool success = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).Reset(
                new AssetIdentifier() { CustomerId = customerId, AssetId = assetId }, WebSecurity.CurrentUserId);

            string text = success
                              ? "Sensor " + assetId.ToString() + " has been reset."
                              : "Failed to reset sensor " + assetId.ToString() + "!";

            return Json(new { Success = success, Description = text });
        }





        public ActionResult MassEditSensor()
        {

            SensorMassEditModel model = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());

            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditSensor(string submitButton, SensorMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }

            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            // Save values

            if (!ModelState.IsValid)
            {
                (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).RefreshMassEditModel(model);
                return View(model);
            }
            else
            {
                // Saved and stay on same page.
                (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SetMassEditModel(model);
                return RedirectToAction("Index", new { rtn = true });
            }
        }


        public JsonResult IsExistsAssociatedSpaceId(long AssociatedSpaceId, int CustomerId)
        {
            var IsExist = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).IsExistsAssociatedSpaceId(AssociatedSpaceId, CustomerId);
        
            return Json(IsExist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetdetailsAssociatedMeterId(int AssociatedMeterId, int CustomerId)
        {
            var Meter = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetdetailsAssociatedMeter(AssociatedMeterId, CustomerId);
            var MeterMaps = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetdetailsAssociatedMeterMap(AssociatedMeterId, CustomerId);
            var spaces = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SpacesWithoutSensors(CustomerId, AssociatedMeterId);

            
            var Street = "";
            var AreaListId = -1;
            var ZoneId = -1;
            var SuburbId = -1;
            var Latitude = 0.0;
            var Longitude = 0.0;
            if (Meter != null)
            {
                Street = Meter.Location;
                Latitude = Meter.Latitude ?? 0.0;
                Longitude = Meter.Longitude ?? 0.0;
            }
            if (MeterMaps != null)
            {
                AreaListId = MeterMaps.AreaId2 ?? -1;
                ZoneId = MeterMaps.ZoneId ?? -1;
                SuburbId = MeterMaps.CustomGroup1 ?? -1;
            }
            var filterValues = new { Street, AreaListId, ZoneId, SuburbId, Latitude, Longitude, spaces };

            return Json(filterValues, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Mechanisms


        public ActionResult ViewMechanism(int customerId, int assetId)
        {
            var model = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(customerId, assetId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ViewMechanism(string submitButton, MechanismViewModel model)
        {
            if (submitButton.Equals("COPY"))
            {
                var mechanismFactory = new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                int clonedCashboxId = mechanismFactory.Clone(model);

                if (clonedCashboxId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying mechanism."
                    };

                    return View(mechanismFactory.GetViewModel(model.CustomerId, (int)model.AssetId));
                }

                return RedirectToAction("EditMechanism", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedCashboxId, parentAssetId = (int)model.AssetId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }

        public ActionResult ViewMechanismPending(int customerId, int areaId, int assetId)
        {
            var model = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult EditMechanism(int customerId, int assetId, int parentAssetId)
        {
            var model = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetEditModel(customerId, assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = model.AreaId,
                    AssetId = parentAssetId
                };
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMechanism(string submitButton, MechanismEditModel model, FormCollection formColl, string returnUrl)
        {
            var mechanismFactory =
                new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(),
                    CurrentCity.LocalTime);
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("COPY"))
            {
                var clonedMechanismId = mechanismFactory.Clone(model);
                if (clonedMechanismId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    mechanismFactory.RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditMechanism", new { customerId = model.CustomerId, assetId = clonedMechanismId, parentAssetId = (int)model.AssetId });
            }

            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);



            #region Work-Around for MVC Handling of Null DateTime Elements
            // If DateTime controls were left blank they will be represented in the model by DateTime.MinValue
            // In this case remove the validation error since we allow "blanks" in these dates.
            // THis is fundamentally a work-around for how MVC treates "null dates".
            if (model.NextPrevMaint == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("NextPrevMaint"))
                    ModelState["NextPrevMaint"].Errors.Clear();
            }
            if (model.WarrantyExpiration == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("WarrantyExpiration"))
                    ModelState["WarrantyExpiration"].Errors.Clear();
            }
            var IsExist = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .SerialNumberExistance(CurrentCity.Id,model.SerialNumber,(int)model.AssetId);
            if (IsExist)        
            {
                ModelState.AddModelError("SerialNumber", "SerialNumber already used");              
               
            }
            #endregion


            // Save values

            if (!ModelState.IsValid)
            {
                // Refresh list values
                mechanismFactory.RefreshEditModel(model);
                model.Saved = false;
                return View(model);
            }
            // Save updates.
            mechanismFactory.SetEditModel(model);

            JavaGetRequestToURL();

            if (model.ClonedFromAsset != null)
            {
                // Refresh list values
                mechanismFactory.RefreshEditModel(model);
                model.Saved = true;
                isNewAssetStatus = 0;
                return View(model);
            }

            mechanismFactory.RefreshEditModel(model);
            model.Saved = true;
            isNewAssetStatus = 0;
            return View(model);
        }


        public ActionResult MassEditMechanism()
        {
            var model = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());

            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditMechanism(string submitButton, MechanismMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            var mechanismFactory =
                new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(),
                    CurrentCity.LocalTime);

            if (!ModelState.IsValid)
            {
                mechanismFactory.RefreshMassEditModel(model);
                return View(model);
            }

            mechanismFactory.SetMassEditModel(model);
            return RedirectToAction("Index", new { rtn = true });
        }

        //TODO - GTC work : mechanism history 
        public ActionResult ViewMechanismHistory(int customerId, int areaId, int assetId)
        {
            var model = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(customerId, assetId);
            return View(model);
        }
        //TODO - GTC work : mechanism history 
        public ActionResult ViewMechanismHistoryDetail(int customerId, int areaId, int assetId, Int64 auditId)
        {
            var model = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(customerId, assetId, auditId);
            return View(model);
        }

        #endregion

        #region DataKeys


        public ActionResult ViewDataKey(int customerId, int assetId)
        {
            var model = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetViewModel(customerId, assetId);
            return View(model);
        }

        [HttpPost]
        public ActionResult ViewDataKey(string submitButton, DataKeyViewModel model)
        {
            if (submitButton.Equals("COPY"))
            {
                var dataKeyFactory = new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                int clonedDataKeyId = dataKeyFactory.Clone(model);

                if (clonedDataKeyId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying DataKey."
                    };

                    return View(dataKeyFactory.GetViewModel(model.CustomerId, (int)model.AssetId));
                }

                return RedirectToAction("EditDataKey", new { customerId = model.CustomerId, areaId = model.AreaId, assetId = clonedDataKeyId, parentAssetId = (int)model.AssetId });
            }

            return RedirectToAction("Index", new { rtn = true });
        }

        public ActionResult ViewDataKeyPending(int customerId, int areaId, int assetId)
        {
            var model = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetPendingViewModel(customerId, assetId);
            return View(model);
        }

        public ActionResult EditDataKey(int customerId, int assetId, int parentAssetId)
        {
            var model = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetEditModel(customerId, assetId);
            model.isNewAsset = isNewAssetStatus;
            // Is this an edit of a cloned asset?
            if (parentAssetId > 0 && parentAssetId != assetId)
            {
                model.ClonedFromAsset = new AssetIdentifier
                {
                    CustomerId = customerId,
                    AreaId = model.AreaId,
                    AssetId = parentAssetId
                };
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditDataKey(string submitButton, DataKeyEditModel model, FormCollection formColl, string returnUrl)
        {
            var dataKeyFactory =
                new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(),
                    CurrentCity.LocalTime);
            model.isNewAsset = isNewAssetStatus;
            if (submitButton.Equals("COPY"))
            {
                var clonedDataKeyId = dataKeyFactory.Clone(model);
                if (clonedDataKeyId == 0)
                {
                    ViewData["__errorItem"] = new ErrorItem
                    {
                        ErrorCode = "1234",
                        ErrorMessage = "General failure copying meter."
                    };

                    dataKeyFactory.RefreshEditModel(model);
                    model.Saved = false;
                    return View(model);
                }

                return RedirectToAction("EditDataKey", new { customerId = model.CustomerId, assetId = clonedDataKeyId, parentAssetId = (int)model.AssetId });
            }

            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);



            #region Work-Around for MVC Handling of Null DateTime Elements
            // If DateTime controls were left blank they will be represented in the model by DateTime.MinValue
            // In this case remove the validation error since we allow "blanks" in these dates.
            // THis is fundamentally a work-around for how MVC treates "null dates".
            if (model.NextPrevMaint == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("NextPrevMaint"))
                    ModelState["NextPrevMaint"].Errors.Clear();
            }
            if (model.WarrantyExpiration == DateTime.MinValue)
            {
                if (ModelState.ContainsKey("WarrantyExpiration"))
                    ModelState["WarrantyExpiration"].Errors.Clear();
            }

            #endregion


            // Save values

            if (!ModelState.IsValid)
            {
                // Refresh list values
                dataKeyFactory.RefreshEditModel(model);
                model.Saved = false;
                return View(model);
            }
            // Save updates.
            dataKeyFactory.SetEditModel(model);

            if (model.ClonedFromAsset != null)
            {
                // Refresh list values
                dataKeyFactory.RefreshEditModel(model);
                model.Saved = true;
                isNewAssetStatus = 0;
                return View(model);
            }

            dataKeyFactory.RefreshEditModel(model);
            model.Saved = true;
            isNewAssetStatus = 0;
            return View(model);
        }


        public ActionResult MassEditDataKey()
        {
            var model = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetMassEditModel(CurrentCity.Id, GetSavedAssetIdentifiers());

            return View(model);
        }

        [HttpPost]
        public ActionResult MassEditDataKey(string submitButton, DataKeyMassEditModel model, FormCollection formColl)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction("Index", new { rtn = true });
            }


            // Rebind model since locale of thread has been set since original binding of model by MVC
            if (TryUpdateModel(model, formColl.ToValueProvider()))
                UpdateModel(model, formColl.ToValueProvider());

            // Revalidate model after rebind.
            ModelState.Clear();
            TryValidateModel(model);

            var dataKeyFactory =
                new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(),
                    CurrentCity.LocalTime);

            if (!ModelState.IsValid)
            {
                dataKeyFactory.RefreshMassEditModel(model);
                return View(model);
            }

            dataKeyFactory.SetMassEditModel(model);
            return RedirectToAction("Index", new { rtn = true });
        }

        //TODO - GTC work : datakey history 
        public ActionResult ViewDataKeyHistory(int customerId, int areaId, int assetId)
        {
            var model = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricListViewModel(customerId, assetId);
            return View(model);
        }
        //TODO - GTC work : datakey history 
        public ActionResult ViewDataKeyHistoryDetail(int customerId, int areaId, int assetId, Int64 auditId)
        {
            var model = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetHistoricViewModel(customerId, assetId, auditId);
            return View(model);
        }

        #endregion




        #region Edit Meter-to-Spaces Mappings

        private const string SessionSpacesRepositoryKey = "_SessionSpacesRepository";

        private List<SpaceListModel> SessionSpacesRepository
        {
            get
            {
                List<SpaceListModel> list = Session[SessionSpacesRepositoryKey] as List<SpaceListModel>;
                if (list == null)
                {
                    list = new List<SpaceListModel>();
                    Session[SessionSpacesRepositoryKey] = list;
                }
                return list;
            }
        }

        /// <summary>
        /// Determine if the bay number in this model is either already used by an existing parking space
        /// or has been assigned to a pending new parking space.
        /// </summary>
        /// <param name="model"><see cref="SpaceListModel"/> instance</param>
        /// <returns>True if model's bay number has already been assigned</returns>
        private bool IsBayNumberUsed(SpaceListModel model)
        {
            SpaceFactory spaceFactory = new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);
            bool alreadyUsed = spaceFactory.IsBayNumberUsed(model.CustomerId, model.MeterId, model.MeterAreaId, model.BayNumber);

            if (!alreadyUsed)
            {
                // Check session bays that have been added.
                var sessionSpace = SessionSpacesRepository.FirstOrDefault(m => m.BayNumber == model.BayNumber);
                alreadyUsed = sessionSpace != null;
            }

            return alreadyUsed;
        }


        /// <summary>
        /// Determine and return next available bay number for this customer, meter and area id
        /// </summary>
        /// <param name="customerId">Integer customer id</param>
        /// <param name="meterId">Integer meter id</param>
        /// <param name="areaId">Integer area id</param>
        /// <returns>Integer value of next bay number</returns>
        private int NextBayNumber(int customerId, int meterId, int areaId)
        {
            int nextBayNumber;

            // See if there are already new spaces added to this session.
            var newSpace = SessionSpacesRepository.FirstOrDefault(m => m.IsNew);
            if (newSpace != null)
            {
                nextBayNumber = SessionSpacesRepository.Where(m => m.IsNew).Max(m => m.BayNumber) + 1;
            }
            else
            {
                // Get next bay number from database.

                var spaceFactory = new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);
                nextBayNumber = spaceFactory.NextBayNumber(customerId, meterId, areaId);
            }

            return nextBayNumber;
        }


        [HttpGet]
        public ActionResult MeterSpacesRead([DataSourceRequest] DataSourceRequest request)
        {
            return Json(SessionSpacesRepository.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MeterSpacesCreate([DataSourceRequest] DataSourceRequest request, SpaceListModel model)
        {
            if (model != null)
            {


                var spaceFactory = new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);
                // Check/assign bay number.  Get next available one if no bay number was assigned. 
                if (model.BayNumber == 0)
                {
                    // Create new bay number.
                    model.BayNumber = NextBayNumber(model.CustomerId, model.MeterId, model.MeterAreaId);
                }
                else
                {
                    // Check if bay number was already used.
                    if (IsBayNumberUsed(model))
                    {
                        ModelState.AddModelError("BayNumberExists", "Bay Number " + model.BayNumber.ToString() + " already exists");
                    }
                }

                // See if we can create a new space in our session storage.
                if (ModelState.IsValid)
                {
                    model.IsNew = true;
                    model.IsChanged = true;
                    model.OperationalStatus = (int)OperationalStatusType.Operational;
                    model.State = AssetStateType.Pending;
                    SessionSpacesRepository.Add(model);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MeterSpacesUpdate([DataSourceRequest] DataSourceRequest request, SpaceListModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var target = SessionSpacesRepository.FirstOrDefault(m => m.BayNumber == model.BayNumber);
                if (target != null)
                {
                    target.Name = model.Name;
                    target.BayName = model.BayName;
                    target.Latitude = model.Latitude;
                    target.Longitude = model.Longitude;
                    target.DemandStatusId = model.DemandStatusId;
                    target.DemandStatus = model.DemandStatus;
                    target.IsChanged = true;
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MeterSpacesDestroy([DataSourceRequest] DataSourceRequest request, SpaceListModel model)
        {
            if (model != null)
            {
                var target = SessionSpacesRepository.FirstOrDefault(m => m.BayNumber == model.BayNumber);
                if (target != null)
                {
                    // Mark space as Inactive
                    target.IsChanged = true;
                    target.OperationalStatus = (int)OperationalStatusType.Inactive;
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }




        [HttpPost]
        public ActionResult GetDemandZones(int customerId, int selectedDemandZoneId)
        {
            List<SelectListItemWrapper> list = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).DemandStatusList(customerId, selectedDemandZoneId);

            return Json(new { list });
        }

       // [HttpPost]
        //public ActionResult GetDemandZones(int customerId, int selectedDemandZoneId, bool isDisplay)
        //{
        //    var list = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(),
        //            CurrentCity.LocalTime)).DemandStatusList(customerId, selectedDemandZoneId);

        //    return Json(new { list });
        //}

        [HttpPost]
        public ActionResult GetNextBayNumber(int customerId, int meterId, int areaId)
        {
            int nextBayNumber = NextBayNumber(customerId, meterId, areaId);

            return Json(new { BayNumber = nextBayNumber });
        }

        #endregion

        #region PAM Configuration

        /// <summary>
        /// Description: This Method will returns the view for the PAMConfiguration with PAM active details
        /// ModifiedBy: Santhosh  (04/Aug/2014 - 07/Aug/2014)        
        /// </summary>
        /// <param name="customerId">Selected Customer ID</param>
        /// <returns></returns>
        public ActionResult PAMConfiguration()
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CurrentCity.Id);
            CustomerPAMConfigurationModel model = (new CustomerFactory(connStringName)).GetPAMConfigurationModel(CurrentCity.Id);
            return View(model);
        }


        /// <summary>
        /// Description:This Method will get details of Clusters  with details
        ///  ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)           
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult GetGracePeriod([DataSourceRequest] DataSourceRequest request, int CustomerId)
        {
            IEnumerable<PAMGracePeriodModel> SummaryResult = Enumerable.Empty<PAMGracePeriodModel>();
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var PAMConfigResult_raw = (new CustomerFactory(connStringName)).GetPAMGrace(CustomerId);
            SummaryResult = from r in PAMConfigResult_raw
                            select new PAMGracePeriodModel
                            {
                                Clusterid = r.Clusterid,
                                GracePeriod = r.GracePeriod

                            };
            DataSourceResult finalResult = SummaryResult.ToDataSourceResult(request);
            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        }

        /// <summary>
        /// Description: This Method will Set PAM Active /InActive with respected parameters
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014) 
        /// ModifiedDate: 28/Aug/2014
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="pamCustactive"></param>
        /// <param name="ChkExpTimeByPAM"></param>
        /// <param name="ChkResetImin"></param>
        public void SetPAMActive(int customerId, bool pamCustactive, bool ChkExpTimeByPAM, bool ChkResetImin)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            (new CustomerFactory(connStringName)).SetPAMActive(customerId, pamCustactive, ChkExpTimeByPAM, ChkResetImin);
        }

        /// <summary>
        /// Description: This Method will ADD Clusterid and graceperiod
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)           
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddGracePeriod([DataSourceRequest] DataSourceRequest request, PAMGracePeriodModel GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).AddGracePeriod(GridRowdata, CustomerId);
            }
            return Json(ModelState.ToDataSourceResult());
        }

        /// <summary>
        /// Description: This Method will Delete Existing Clusterid and graceperiod
        ///  ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteGracePeriod([DataSourceRequest] DataSourceRequest request, PAMGracePeriodModel GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).DeleteGracePeriod(GridRowdata);

            }

            return Json(ModelState.ToDataSourceResult());
        }
        /// <summary>
        /// Description: This Method will retrive all ClusterID list created for customer.
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)                   
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult ClusterIDList(int CustomerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var clusteridlist = (new CustomerFactory(connStringName)).ClusterIDList(CustomerId);
            return Json(clusteridlist, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Description: This Method will retrive all MeterID list given customer.
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult MeterIDList(int CustomerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var MeterIDList = (new CustomerFactory(connStringName)).MeterIDList(CustomerId);
            return Json(MeterIDList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MeterIDListforSSM(int CustomerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var MeterIDList = (new CustomerFactory(connStringName)).MeterIDListforSSM(CustomerId);
            return Json(MeterIDList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This Method will retrive Meters are assigned to a cluster with a bay range 
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)         
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult GetPAMClusterMeter([DataSourceRequest] DataSourceRequest request, int CustomerId)
        {

            IEnumerable<PAMClusters> SummaryResult = Enumerable.Empty<PAMClusters>();
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
            var PAMConfigResult_raw = (new CustomerFactory(connStringName)).GetPAMClusterMeter(CustomerId);
            SummaryResult = from r in PAMConfigResult_raw
                            select new PAMClusters
                            {
                                Clusterid = r.Clusterid,
                                Description = r.Description,
                                Hostedbayend = r.Hostedbayend,
                                Hostedbaystart = r.Hostedbaystart,
                                MeterId = r.MeterId
                            };

            DataSourceResult finalResult = SummaryResult.ToDataSourceResult(request);
            return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };
        }

        /// <summary>
        /// Description: This Method will ADD new //Update Meter assigned to a cluster with a bay range 
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddPAMClusterMeter([DataSourceRequest] DataSourceRequest request, PAMClusters GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).AddPAMClusterMeter(GridRowdata, CustomerId);

            }
            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();
            return Json(ModelState.ToDataSourceResult());
        }

        /// <summary>
        /// Description: This Method will Unassign Meter from cluster bay range 
        /// ModifiedBy: Santhosh (04/Aug/2014 - 07/Aug/2014)          
        /// </summary>
        /// <param name="request"></param>
        /// <param name="GridRowdata"></param>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeletePAMClusterMeter([DataSourceRequest] DataSourceRequest request, PAMClusters GridRowdata, int CustomerId)
        {
            if (GridRowdata != null)
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(CustomerId);
                (new CustomerFactory(connStringName)).DeletePAMClusterMeter(GridRowdata);

            }
            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();
            return Json(ModelState.ToDataSourceResult());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public int GetGroupID(int MeterId, int CustomerId)
        {
            return (new CustomerFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetGroupID(MeterId, CustomerId);
        }

        #endregion


        #region Asset History Support


        [HttpPost]
        public ActionResult GetReasonsForChangeTypes()
        {
            //List<SelectListItemWrapper> list = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetReasonsForChange();

            List<SelectListItemWrapper> list = (new AssetFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetReasonsForChange();

            return Json(new { list });
        }

        public ActionResult GetItemsCashboxHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new CashboxFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsMechanismHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetItemsDataKeyHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new DataKeyFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);
            
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
           // var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetItemsSensorHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new SensorFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsGatewayHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new GatewayFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetItemsMeterHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new MeterFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsSpaceHistory([DataSourceRequest] DataSourceRequest request, int customerId, int areaId, int assetId, long startDateTicks, long endDateTicks)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "AssetHistory");

            //get models
            int total;

            var items = (new SpaceFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHistory(request, out total, customerId, areaId, assetId, startDateTicks, endDateTicks);

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        /// <summary>
        /// Check AssetID entered by User
        /// </summary>
        /// <param name="assetID"></param>
        /// <param name="customerID"></param>
        /// <param name="areaID"></param>
        /// <param name="assetTpeId"></param>
        /// <returns></returns>
        public ActionResult AssetIDExistence(string assetID, string customerID, string areaID, string assetTpeId)
        {
            return base.AssetIDExistence(assetID, customerID, areaID, assetTpeId);
        }

        public ActionResult SerialNumberIDExistence(string serialNumber, string customerID)
        {
            int customerId = Int32.Parse(customerID);
            string serialIdNew = "0";
            var serialResult = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SerialNumberExistance(customerId, serialNumber.Trim());
            return Json(serialResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SingleSpaceSerialNumberExistance(string serialNumber, string customerID)
        {
            int customerId = Int32.Parse(customerID);
            string serialIdNew = "0";
            var serialResult = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SingleSpaceSerialNumberExistance(customerId, serialNumber.Trim());
            return Json(serialResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LocationNameExistence(string locationname, string customerID)
        {
            int customerId = Int32.Parse(customerID);
            var serialResult = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).LocationIdExistance(customerId, locationname);
            return Json(serialResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SingleSpaceSerialNumberExistanceMeter(string serialNumber, string customerID, string MeterId)
        {
            int customerId = Int32.Parse(customerID);
            int MeterID = Int32.Parse(MeterId);
            string serialIdNew = "0";
            var serialResult = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).SingleSpaceSerialNumberExistanceMeter(customerId, serialNumber.Trim(), MeterID);
            return Json(serialResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LocationNameExistenceMeter(string locationname, string customerID,string MeterId)
        {
            int customerId = Int32.Parse(customerID);
            int MeterID = Int32.Parse(MeterId);
            var serialResult = (new MechanismFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).LocationIdExistanceMeter(customerId, locationname, MeterID);
            return Json(serialResult, JsonRequestBehavior.AllowGet);
        }


    }
}
