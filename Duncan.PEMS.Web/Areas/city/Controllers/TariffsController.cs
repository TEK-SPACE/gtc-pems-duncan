using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Business.Tariffs;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Errors;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Tariffs;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using NLog;
using NPOI.HSSF.UserModel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class TariffsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Session Storage

        // Tariff editing makes extensive use of session storage.  This region contains the various session
        // storage constructs and property accessors to simplify use

        private const string SessionTariffReturnStackKey = "_SessionTariffReturnStack";

        private const string SessionTariffRatesKey = "_SessionTariffRates";
        private const string SessionTariffRateConfigurationKey = "_SessionTariffRateConfiguration";

        private const string SessionRateSchedulesKey = "_SessionRateSchedules";
        private const string SessionRateScheduleConfigurationKey = "_SessionRateScheduleConfiguration";

        private const string SessionHolidayRatesKey = "_SessionHolidayRates";
        private const string SessionHolidayRateConfigurationKey = "_SessionHolidayRateConfiguration";

        private Stack<string> SessionTariffReturnStack
        {
            get
            {
                Stack<string> list = Session[SessionTariffReturnStackKey] as Stack<string>;
                if (list == null)
                {
                    list = new Stack<string>();
                    Session[SessionTariffReturnStackKey] = list;
                }
                return list;
            }
        }

        private string SessionTariffReturnStackPop
        {
            get
            {
                string returnAction = "Create";

                if ( SessionTariffReturnStack.Any() )
                {
                    returnAction = SessionTariffReturnStack.Pop();
                }
                return returnAction;
            }
        }

        private string SessionTariffReturnStackPush
        {
            set
            {
                if(!string.IsNullOrWhiteSpace(value))
                    SessionTariffReturnStack.Push(value);
            }
        }


        /// <summary>
        /// A list of <see cref="TariffRateModel"/> instances that are presently being organized into
        /// a TariffRateConfiguration collection.
        /// </summary>
        private List<TariffRateModel> SessionTariffRates
        {
            get
            {
                List<TariffRateModel> list = Session[SessionTariffRatesKey] as List<TariffRateModel>;
                if (list == null)
                {
                    list = new List<TariffRateModel>();
                    Session[SessionTariffRatesKey] = list;
                }
                return list;
            }
        }

        /// <summary>
        /// Working model of a <see cref="TariffRateConfigurationModel"/> instance that is presently being used
        /// to create Session ScheduleRate instances and ultimately a ConfigProfile.
        /// </summary>
        private TariffRateConfigurationModel SessionTariffRateConfiguration
        {
            get
            {
                TariffRateConfigurationModel trc = Session[SessionTariffRateConfigurationKey] as TariffRateConfigurationModel;
                if (trc == null)
                {
                    trc = new TariffRateConfigurationModel()
                        {
                            CustomerId = CurrentCity.Id
                        };
                    Session[SessionTariffRateConfigurationKey] = trc;
                }
                return trc;
            }

            set
            {
                Session[SessionTariffRateConfigurationKey] = value;
            }
        }


        /// <summary>
        /// A list of <see cref="RateScheduleModel"/> instances that are presently being organized into
        /// a RateScheduleConfiguration collection.
        /// </summary>
        private List<RateScheduleModel> SessionRateSchedules
        {
            get
            {
                List<RateScheduleModel> list = Session[SessionRateSchedulesKey] as List<RateScheduleModel>;
                if (list == null)
                {
                    list = new List<RateScheduleModel>();
                    Session[SessionRateSchedulesKey] = list;
                }
                return list;
            }
        }

        /// <summary>
        /// Working model of <see cref="RateScheduleConfigurationModel"/> instance that is presently being used
        /// to create either HolidayRate instances or ConfigProfile.
        /// </summary>
        private RateScheduleConfigurationModel SessionRateScheduleConfiguration
        {
            get
            {
                RateScheduleConfigurationModel rsc = Session[SessionRateScheduleConfigurationKey] as RateScheduleConfigurationModel;
                if (rsc == null)
                {
                    rsc = new RateScheduleConfigurationModel();
                    Session[SessionRateScheduleConfigurationKey] = rsc;
                }
                return rsc;
            }

            set
            {
                Session[SessionRateScheduleConfigurationKey] = value;
            }
        }

        /// <summary>
        /// A list of <see cref="HolidayRateModel"/> instances that are presently being organized into
        /// a HolidayRateConfiguration collection.
        /// </summary>
        private List<HolidayRateModel> SessionHolidayRates
        {
            get
            {
                List<HolidayRateModel> list = Session[SessionHolidayRatesKey] as List<HolidayRateModel>;
                if (list == null)
                {
                    list = new List<HolidayRateModel>();
                    Session[SessionHolidayRatesKey] = list;
                }
                return list;
            }
        }

        /// <summary>
        /// Working model of <see cref="HolidayRateConfigurationModel"/> instance that is presently being used
        /// to create ConfigProfile.
        /// </summary>
        private HolidayRateConfigurationModel SessionHolidayRateConfiguration
        {
            get
            {
                HolidayRateConfigurationModel hrc = Session[SessionHolidayRateConfigurationKey] as HolidayRateConfigurationModel;
                if (hrc == null)
                {
                    hrc = new HolidayRateConfigurationModel();
                    Session[SessionHolidayRateConfigurationKey] = hrc;
                }
                return hrc;
            }

            set
            {
                Session[SessionHolidayRateConfigurationKey] = value;
            }
        }


        private void ClearSessionStorage()
        {
            SessionTariffReturnStack.Clear();
            SessionTariffRates.Clear();
            Session[SessionTariffRateConfigurationKey] = null;
            SessionRateSchedules.Clear();
            Session[SessionRateScheduleConfigurationKey] = null;
            SessionHolidayRates.Clear();
            Session[SessionHolidayRateConfigurationKey] = null;
        }


        #endregion



        #region Index

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetItems([DataSourceRequest] DataSourceRequest request, long startDateTicks, long endDateTicks, int customerId)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Tariffs");

            //get models
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetConfigProfileModels(startDateTicks, endDateTicks, customerId);

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


        #endregion

        #region "Exporting"

        public FileResult ExportToCsv([DataSourceRequest] DataSourceRequest request, long startDateTicks, long endDateTicks, int customerId)
        {
            //get role models
            var data = GetExportData(request, startDateTicks, endDateTicks, customerId);
            var output = new MemoryStream();
            var writer = new StreamWriter(output, Encoding.UTF8);
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Tariff Config"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Creation Date"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Rates"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Schedules"));
            AddCsvValue(writer, (new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Holidays"), true);
            foreach (ConfigProfileModel item in data)
            {
                //add a item for each property here
                AddCsvValue(writer, item.ConfigurationName);
                AddCsvValue(writer, item.CreatedOnDisplay);
                AddCsvValue(writer, item.TariffRatesCount.ToString() + " Rates");
                AddCsvValue(writer, item.RateSchedulesCount.ToString() + " Schedules");
                AddCsvValue(writer, item.HolidayRatesCount.ToString() + " Holidays", true);
            }
            writer.Flush();
            output.Position = 0;
            return File(output, "text/comma-separated-values","TariffsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, long startDateTicks, long endDateTicks, int customerId)
        {
            //get role models
            var data = GetExportData(request, startDateTicks, endDateTicks, customerId, Constants.Export.ExcelExportCount);

            //Create new Excel workbook
            var workbook = new HSSFWorkbook();
            //Create new Excel sheet
            var sheet = workbook.CreateSheet();

            //add the filtered values to the top of the excel file
            var rowNumber = AddFiltersToExcelSheet(request, sheet);

            //Create a header row
            var headerRow = sheet.CreateRow(rowNumber++);

            //Set the column names in the header row

            headerRow.CreateCell(0).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Tariff Config"));
            headerRow.CreateCell(1).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Creation Date"));
            headerRow.CreateCell(2).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Rates"));
            headerRow.CreateCell(3).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Schedules"));
            headerRow.CreateCell(4).SetCellValue((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Holidays"));

            //Populate the sheet with values from the grid data
            foreach (ConfigProfileModel item in data)
            {
                //Create a new row
                //use a variable for column number so we can easily conditionally add or NOT add rows and the file will still be generated correctly
                int columnNumber = 0;
                var row = sheet.CreateRow(rowNumber++);

                //Set values for the cells
                row.CreateCell(columnNumber++).SetCellValue(item.ConfigurationName);
                row.CreateCell(columnNumber++).SetCellValue(item.CreatedOnDisplay);
                row.CreateCell(columnNumber++).SetCellValue(item.TariffRatesCount.ToString()    + " Rates");
                row.CreateCell(columnNumber++).SetCellValue(item.RateSchedulesCount.ToString()  + " Schedules");
                row.CreateCell(columnNumber++).SetCellValue(item.HolidayRatesCount.ToString()   + " Holidays");
            }

            //Write the workbook to a memory stream
            var output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user

            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "TariffsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request, long startDateTicks, long endDateTicks, int customerId)
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
            var data = GetExportData(request, startDateTicks, endDateTicks, customerId, Constants.Export.PdfExportCount);


            var dataTable = new PdfPTable(5) { WidthPercentage = 100 };
            dataTable.DefaultCell.Padding = 3;
            dataTable.DefaultCell.BorderWidth = 2;
            dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Tariff Config"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Creation Date"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Rates"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Schedules"));
            dataTable.AddCell((new ResourceFactory()).GetLocalizedTitle(ResourceTypes.GridColumn, "Holidays"));
            dataTable.CompleteRow();
            dataTable.HeaderRows = 1;
            dataTable.DefaultCell.BorderWidth = 1;
            int rowIndex = 0;
            foreach (ConfigProfileModel item in data)
            {
                dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                rowIndex++;
                dataTable.AddCell(item.ConfigurationName);
                dataTable.AddCell(item.CreatedOnDisplay);
                dataTable.AddCell(item.TariffRatesCount.ToString() + " Rates");
                dataTable.AddCell(item.RateSchedulesCount.ToString() + " Schedules");
                dataTable.AddCell(item.HolidayRatesCount.ToString() + " Holidays");
                dataTable.CompleteRow();
            }

            // Add table to the document
            document.Add(dataTable);

            //This is important don't forget to close the document
            document.Close();

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "TariffsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        private IEnumerable GetExportData(DataSourceRequest request, long startDateTicks, long endDateTicks,
                                          int customerId, int size = 0)
        {
            //get the user models
            //no paging, we are bringing back all the items
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetConfigProfileModels(startDateTicks, endDateTicks, customerId);

            items = items.ApplyFiltering(request.Filters);
            items = items.ApplySorting(request.Groups, request.Sorts);
            items = items.ApplyPaging(request.Page, request.PageSize);
            IEnumerable data = items.ToList();
            return data;
        }

        #endregion

        #region ConfigProfile Details (Configured Tariff Set)

        [RequireRouteValues(new[] { "customerId", "configProfileId" })]
        public ActionResult Details(int customerId, Int64 configProfileId)
        {
            ConfigProfileSpaceViewModel model = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetConfigProfileSpaceViewModel(customerId, configProfileId);
            return View(model);
        }

        [RequireRouteValues(new[] { "customerId", "configProfileSpaceId", "spaceId" })]
        public ActionResult Details(int customerId, Int64 configProfileSpaceId, Int64 spaceId)
        {
            ConfigProfileSpaceViewModel model = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetConfigProfileSpaceViewModel(customerId, configProfileSpaceId, spaceId);
            return View(model);
        }


        #region Tariff Rates

        public ActionResult DetailsTariffRateConfig(int customerId, Int64 configProfileId)
        {
            ConfigProfileSpaceViewModel model = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetConfigProfileSpaceViewModel(customerId, configProfileId);
            return View(model);
        }

        #endregion

        #region Rate Schedules

        public ActionResult DetailsRateScheduleConfig(int customerId, Int64 configProfileId)
        {
            ConfigProfileSpaceViewModel model = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetConfigProfileSpaceViewModel(customerId, configProfileId);
            return View(model);
        }

        public ActionResult ScheduleDetails(int customerId, Int64 configProfileId)
        {
            return View();
        }

        #endregion

        #region Holiday Rates

        public ActionResult DetailsHolidayRateConfig(int customerId, Int64 configProfileId)
        {
            ConfigProfileSpaceViewModel model = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).GetConfigProfileSpaceViewModel(customerId, configProfileId);
            return View(model);
        }

        public ActionResult HolidayDetails(int customerId, Int64 configProfileId)
        {
            return View();
        }

        #endregion


        #endregion

        #region ConfigProfile Create (Configured Tariff Set)

        #region Config Profile Handlers

        /// <summary>
        /// Initializes session for ConfigProfile creation.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public ActionResult CreateInit(int customerId)
        {
            // Clear session storage
            ClearSessionStorage();

            // Create working copies of TariffRateConfiguration, RateScheduleConfiguration and HolidayRateConfiguration
            var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

            SessionTariffRateConfiguration = tariffFactory.SaveTariffRateConfiguration(customerId, new TariffRateConfigurationModel() { CustomerId = customerId }, SessionTariffRates);
            SessionRateScheduleConfiguration = tariffFactory.SaveRateScheduleConfiguration(customerId, new RateScheduleConfigurationModel() { CustomerId = customerId }, SessionRateSchedules);
            SessionHolidayRateConfiguration = tariffFactory.SaveHolidayRateConfiguration(customerId, new HolidayRateConfigurationModel() { CustomerId = customerId }, SessionHolidayRates);



            return RedirectToAction("Create", new { customerId = customerId });
        }


        public ActionResult Create(int customerId)
        {
            var model = new ConfigProfileModel() { CustomerId = customerId };

            model.TariffRateConfigurationId = SessionTariffRateConfiguration.TariffRateConfigurationId;
            model.TariffRateConfigurationName = SessionTariffRateConfiguration.Name;
            model.TariffRateConfigurationState = SessionTariffRateConfiguration.State;
            model.TariffRateConfigurationStateName = SessionTariffRateConfiguration.StateName;
            model.TariffRatesCount = SessionTariffRateConfiguration.TariffRateCount;

            model.RateScheduleConfigurationId = SessionRateScheduleConfiguration.RateScheduleConfigurationId;
            model.RateScheduleConfigurationName = SessionRateScheduleConfiguration.Name;
            model.RateScheduleConfigurationState = SessionRateScheduleConfiguration.State;
            model.RateScheduleConfigurationStateName = SessionRateScheduleConfiguration.StateName;
            model.RateSchedulesCount = SessionRateScheduleConfiguration.RateScheduleCount;
            for (int i = 0; i < SessionRateScheduleConfiguration.DayOfWeek.Count(); i++)
            {
                model.DayOfWeek[i] = SessionRateScheduleConfiguration.DayOfWeek[i];
            }

            model.HolidayRateConfigurationId = SessionHolidayRateConfiguration.HolidayRateConfigurationId;
            model.HolidayRateConfigurationName = SessionHolidayRateConfiguration.Name;
            model.HolidayRateConfigurationState = SessionHolidayRateConfiguration.State;
            model.HolidayRateConfigurationStateName = SessionHolidayRateConfiguration.StateName;
            model.HolidayRatesCount = SessionHolidayRateConfiguration.HolidayRateCount;

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(string submitButton, ConfigProfileModel model)
        {
            var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

            if (submitButton.Equals("CANCEL"))
            {
                // If any of the Configurations are still Pending or New then delete them.  User has already
                // had the chance to save the configuration that was pending.

                if ( SessionTariffRateConfiguration.State == TariffStateType.New || SessionTariffRateConfiguration.State == TariffStateType.Pending )
                {
                    tariffFactory.DeleteTariffRateConfiguration(model.CustomerId, SessionTariffRateConfiguration.TariffRateConfigurationId);
                }

                if (SessionRateScheduleConfiguration.State == TariffStateType.New || SessionRateScheduleConfiguration.State == TariffStateType.Pending)
                {
                    tariffFactory.DeleteRateScheduleConfiguration( model.CustomerId, SessionRateScheduleConfiguration.RateScheduleConfigurationId);
                }

                if (SessionHolidayRateConfiguration.State == TariffStateType.New || SessionHolidayRateConfiguration.State == TariffStateType.Pending)
                {
                    tariffFactory.DeleteHolidayRateConfiguration( model.CustomerId, SessionHolidayRateConfiguration.HolidayRateConfigurationId);
                }

                // Clear session storage
                ClearSessionStorage();

                // Return back to index page.
                return RedirectToAction("Index", new { customerId = model.CustomerId });
            }


            // Update model with Tariff Rates, Rate Schedules and optionally Holiday Rate Configuration information.
            model.TariffRateConfigurationId = SessionTariffRateConfiguration.TariffRateConfigurationId;
            model.TariffRateConfigurationName = SessionTariffRateConfiguration.Name;
            model.TariffRateConfigurationState = SessionTariffRateConfiguration.State;
            model.TariffRateConfigurationStateName = SessionTariffRateConfiguration.StateName;
            model.TariffRatesCount = SessionTariffRateConfiguration.TariffRateCount;

            model.RateScheduleConfigurationId = SessionRateScheduleConfiguration.RateScheduleConfigurationId;
            model.RateScheduleConfigurationName = SessionRateScheduleConfiguration.Name;
            model.RateScheduleConfigurationState = SessionRateScheduleConfiguration.State;
            model.RateScheduleConfigurationStateName = SessionRateScheduleConfiguration.StateName;
            model.RateSchedulesCount = SessionRateScheduleConfiguration.RateScheduleCount;
            for (int i = 0; i < SessionRateScheduleConfiguration.DayOfWeek.Count(); i++)
            {
                model.DayOfWeek[i] = SessionRateScheduleConfiguration.DayOfWeek[i];
            }

            model.HolidayRateConfigurationId = SessionHolidayRateConfiguration.HolidayRateConfigurationId;
            model.HolidayRateConfigurationName = SessionHolidayRateConfiguration.Name;
            model.HolidayRateConfigurationState = SessionHolidayRateConfiguration.State;
            model.HolidayRateConfigurationStateName = SessionHolidayRateConfiguration.StateName;
            model.HolidayRatesCount = SessionHolidayRateConfiguration.HolidayRateCount;


            if (SessionTariffRateConfiguration.State != TariffStateType.Current)
            {
                // Tariff rates must have been configured
                ModelState.AddModelError("RequireConfiguredTariffs",
                    "Cannot configure Tariff until have configured Tariff Rates.");
                AddError("1234", "Cannot configure Tariff until have configured Tariff Rates.");
            }

            if (SessionRateScheduleConfiguration.State != TariffStateType.Current)
            {
                // Rate Schedules must have been configured
                ModelState.AddModelError("RequireConfiguredSchedule",
                    "Cannot configure Tariff until have configured Rate Schedules.");
                AddError("1234", "Cannot configure Tariff until have configured Rate Schedules.");
            }

            if ( ModelState.IsValid )
            {
                // Create a ConfigProfile with the TariffRateConfiguration, RateScheduleConfiguration, and optionally the HolidayRateConfiguration.
                tariffFactory.CreateConfigProfile( model );

                // At this point only HolidayRateConfiguration could be in a state other than Current.
                if (SessionHolidayRateConfiguration.State == TariffStateType.New || SessionHolidayRateConfiguration.State == TariffStateType.Pending)
                {
                    tariffFactory.DeleteHolidayRateConfiguration(model.CustomerId, SessionHolidayRateConfiguration.HolidayRateConfigurationId);
                }

                // Clear session storage
                ClearSessionStorage();

                // Now redirect back to the Create process.
                return RedirectToAction( "Index", new {customerId = model.CustomerId} );
            }
            else
            {
                return View( model );
            }
        }

        #endregion

        #region Tariff Rate Configuration Handlers

        public ActionResult CreateTariffRateConfig(int customerId, string rtn)
        {
            SessionTariffReturnStackPush = rtn;

            return View(SessionTariffRateConfiguration);
        }

        /// <summary>
        /// Act on a Tariff Rate Configuration page action.
        /// RETURN - Return to previous page.  Ignore any changes.
        /// CANCEL - Remove any <see cref="TariffRateModel"/> that were not already saved in the present Session.
        /// CONFIGURE - Create a <see cref="TariffRateConfiguration"/> object from the <see cref="TariffRateConfigurationModel"/>
        /// and the <see cref="TariffRateModel"/> that are presently in the Session.  Once <see cref="TariffRateConfiguration"/> is created,
        /// configure the <see cref="TariffRateConfiguration"/>.  See <see cref="TariffFactory.ConfigureTariffRateConfiguration"/>.
        /// SAVE - Save a temporary copy of <see cref="TariffRateConfiguration"/> and the associated <see cref="TariffRateModel"/>
        /// in Session.  Mark the <see cref="TariffRateModel"/> in Session as IsSaved = true.
        /// </summary>
        /// <param name="submitButton">Action requested: RETURN, CANCEL, CONFIGURE, SAVE</param>
        /// <param name="model"><see cref="TariffRateConfigurationModel"/> instance from which to create or save a 
        /// <see cref="TariffRateConfiguration"/>.</param>
        /// <returns><see cref="ActionResult"/> for associated Action. </returns>
        [HttpPost]
        public ActionResult CreateTariffRateConfig(string submitButton, TariffRateConfigurationModel model)
        {
            if ( submitButton.Equals( "RETURN" ) )
            {
                return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
            }

            if (submitButton.Equals("CANCEL"))
            {
                // Remove any tariff rates that are not marked as used.
                List<TariffRateModel> deleteList = new List<TariffRateModel>();
                foreach (var tariffRate in SessionTariffRates)
                {
                    if (!tariffRate.IsSaved)
                    {
                        deleteList.Add(tariffRate);
                    }
                }
                // Now remove the ones in the deleteList
                foreach (var tariffRate in deleteList)
                {
                    SessionTariffRates.Remove( tariffRate );
                    // Remove these unused tariff rates from the backing storage.
                    (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                        .DeleteTariffRate( tariffRate );
                }
                // Now break any LinkedRates to any rates in deleteList
                foreach (var tariffRateModel in SessionTariffRates)
                {
                    var tr = deleteList.FirstOrDefault( m => m.TariffRateId == tariffRateModel.LinkedTariffRateId );
                    if ( tr != null )
                    {
                        tariffRateModel.LinkedTariffRateId = null;
                        tariffRateModel.LinkedTariffRateName = string.Empty;
                        tariffRateModel.IsChanged = true;
                    }
                }

                return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
            }


            if (!SessionTariffRates.Any())
            {
                // Must be at least one schedule day.
                ModelState.AddModelError("RequireATariff", "Minimum of one Tariff Rate required.");
                AddError( "1234", "Minimum of one Tariff Rate required." );
            }

            if ( submitButton.Equals( "CONFIGURE" ) )
            {
                if ( string.IsNullOrWhiteSpace( model.Name ) )
                {
                    // Must have a name
                    ModelState.AddModelError( "RequireName", "Name is required." );
                    AddError("1234", "Name is required.");
                }
            }


            if ( ModelState.IsValid )
            {
                var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                // Create a TariffRateConfiguration entry from SessionTariffRates
                model = tariffFactory.SaveTariffRateConfiguration(model.CustomerId, model, SessionTariffRates);

                // Take a different path if action == CONFIGURE
                if (submitButton.Equals("CONFIGURE"))
                {
                    // Set the State of the TariffRateConfiguration to 'Current'
                    model = tariffFactory.ConfigureTariffRateConfiguration( model.CustomerId, model );
                    SessionTariffRateConfiguration = model;
                    // Now redirect back to the Create process.
                    return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
                }
                else
                {
                    // Else stay on edit screen.
                    SessionTariffRateConfiguration = model;
                    return View(model);
                }
            }
            else
            {
                return View( model );
            }
        }


        public ActionResult SelectTariffRateConfig(int customerId)
        {
            TariffRateConfigurationModel model = new TariffRateConfigurationModel() { CustomerId = customerId };

            return View(model);
        }

        public ActionResult ImportTariffRateConfig(int customerId, Int64 tariffRateConfigurationId)
        {
            if (tariffRateConfigurationId != 0)
            {
                // Is this an import of another TariffRateConfiguration?
                if (tariffRateConfigurationId != SessionTariffRateConfiguration.TariffRateConfigurationId)
                {
                    var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

                    // This is an import.  Clone the list of TariffRates
                    List<TariffRateModel> cloneList = new List<TariffRateModel>();
                    tariffFactory.CloneTariffRateConfigurationList(customerId, tariffRateConfigurationId, cloneList);

                    // Need to rename these rates to avoid clashing with any existing tariff rate names.
                    // Find the largest RateNameIndex presently in the SessionTariffRates list.
                    int nextRateIndex = 1;

                    if (SessionTariffRates.Any())
                    {
                        nextRateIndex = SessionTariffRates.Max(m => m.RateNameIndex) + 1;
                    }

                    // For each rate in cloneList, rename the rate.  Then fix up the linked rate names.
                    foreach (var tariffRate in cloneList)
                    {
                        tariffRate.RateNameIndex = nextRateIndex++;
                        tariffRate.RateName = "Rate " + tariffRate.RateNameIndex;
                        tariffRate.IsChanged = true;
                    }
                    // Fix up LinkedRate Names
                    foreach (var tariffRate in cloneList)
                    {
                        if (tariffRate.LinkedTariffRateId.HasValue && tariffRate.LinkedTariffRateId > 0)
                        {
                            tariffRate.LinkedTariffRateName = cloneList.FirstOrDefault(m => m.TariffRateId == tariffRate.LinkedTariffRateId).RateName;
                        }
                    }

                    // Now have a list of cloned tariff rates ready to join the existing tariff rates in SessionTariffRates
                    SessionTariffRates.AddRange(cloneList);

                    SessionTariffRateConfiguration.TariffRateCount = SessionTariffRates.Count;
                }
            }


            return RedirectToAction("CreateTariffRateConfig", new { customerId = customerId });
        }



        #endregion

        #region Rate Schedule Configuration Handlers


        public ActionResult CreateRateScheduleConfig(int customerId, string rtn)
        {
            SessionTariffReturnStackPush = rtn;

            return View(SessionRateScheduleConfiguration);
        }

        [HttpPost]
        public ActionResult CreateRateScheduleConfig(string submitButton, RateScheduleConfigurationModel model)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
            }

            if (submitButton.Equals("CANCEL"))
            {
                // Remove any rate schedules that are not marked as used.
                List<RateScheduleModel> deleteList = new List<RateScheduleModel>();
                foreach (var rateSchedule in SessionRateSchedules)
                {
                    if (!rateSchedule.IsSaved)
                    {
                        deleteList.Add(rateSchedule);
                    }
                }
                // Now remove the ones in the deleteList
                foreach (var rateSchedule in deleteList)
                {
                    SessionRateSchedules.Remove(rateSchedule);
                    // Remove these unused rate schedules from the backing storage.
                    (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                        .DeleteRateSchedule(rateSchedule);
                }
                
                return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
            }

            // Check validity of model
            if (!SessionRateSchedules.Any())
            {
                // Must be at least one schedule day.
                ModelState.AddModelError("RequireSchedule", "Minimum of one Schedule day required.");
                AddError("1234", "Minimum of one Schedule day required.");
            }


            if ( submitButton.Equals( "CONFIGURE" ) )
            {
                if (SessionTariffRateConfiguration.State != TariffStateType.Current)
                {
                    // Must be at least one schedule day.
                    ModelState.AddModelError("RequireConfiguredTariffs", 
                        "Cannot configure Schedule Rates until have configured Tariff Rates.");
                    AddError("1234", "Cannot configure Schedule Rates until have configured Tariff Rates.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    // Must have a name
                    ModelState.AddModelError("RequireName", "Name is required.");
                    AddError("1234", "Name is required.");
                }

                // Were all days of the week covered?
                if ( !SessionRateScheduleConfiguration.AllDaysCovered )
                {
                    // All days of the week must be scheduled.
                    ModelState.AddModelError("ValidationRateSchedule", "All days of the week must be scheduled.");
                    AddError("1234", "All days of the week must be scheduled.");
                }

            }

            if ( ModelState.IsValid )
            {
                var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                // Create a RateScheduleConfiguration entry from SessionRateSchedules
                model = tariffFactory.SaveRateScheduleConfiguration(model.CustomerId, model, SessionRateSchedules);

                // Take a different path if action == CONFIGURE
                if (submitButton.Equals("CONFIGURE"))
                {
                    // Set the State of the RateScheduleConfiguration to 'Current'
                    model = tariffFactory.ConfigureRateScheduleConfiguration(model.CustomerId, model);
                    SessionRateScheduleConfiguration = model;
                    // Now redirect back to the Create process.
                    return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
                }
                else
                {
                    // Else stay on edit screen.
                    SessionRateScheduleConfiguration = model;
                    return View(model);
                }

            }
            else
            {
                return View( model );
            }
        }

        public ActionResult SelectRateScheduleConfig(int customerId)
        {
            RateScheduleConfigurationModel model = new RateScheduleConfigurationModel() { CustomerId = customerId };

            return View(model);
        }

        public ActionResult ImportRateScheduleConfig(int customerId, Int64 rateScheduleConfigurationId)
        {

            RateScheduleConfigurationModel model = null;

            if (rateScheduleConfigurationId != 0)
            {
                // Is this an import of another TariffRateConfiguration?
                if (rateScheduleConfigurationId != SessionRateScheduleConfiguration.RateScheduleConfigurationId)
                {
                    var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                    // This is an import.  Clone the list of RateSchedules
                    List<TariffRateModel> cloneTariffRateList = new List<TariffRateModel>();
                    List<RateScheduleModel> cloneRateScheduleList = new List<RateScheduleModel>();
                    tariffFactory.CloneRateScheduleConfigurationList(customerId, rateScheduleConfigurationId,
                        cloneTariffRateList, cloneRateScheduleList);

                    // Need to rename these rates to avoid clashing with any existing tariff rate names.
                    // Find the largest RateNameIndex presently in the SessionTariffRates list.
                    int nextRateIndex = 1;

                    if (SessionTariffRates.Any())
                    {
                        nextRateIndex = SessionTariffRates.Max(m => m.RateNameIndex) + 1;
                    }

                    // For each rate in cloneList, rename the rate.  Then fix up the linked rate names.
                    foreach (var tariffRate in cloneTariffRateList)
                    {
                        tariffRate.RateNameIndex = nextRateIndex++;
                        tariffRate.RateName = "Rate " + tariffRate.RateNameIndex;
                        tariffRate.IsChanged = true;
                    }
                    // Fix up LinkedRate Names
                    foreach (var tariffRate in cloneTariffRateList)
                    {
                        if (tariffRate.LinkedTariffRateId.HasValue && tariffRate.LinkedTariffRateId > 0)
                        {
                            tariffRate.LinkedTariffRateName = cloneTariffRateList.FirstOrDefault(m => m.TariffRateId == tariffRate.LinkedTariffRateId).RateName;
                        }
                    }

                    // Now have a list of cloned tariff rates ready to join the existing tariff rates in SessionTariffRates
                    SessionTariffRates.AddRange(cloneTariffRateList);

                    SessionTariffRateConfiguration.TariffRateCount = SessionTariffRates.Count;

                    // Add the corrected TariffRateName to the imported RateSchedule.
                    foreach (var rateSchedule in cloneRateScheduleList)
                    {
                        var tariffRate = SessionTariffRates.FirstOrDefault( m => m.TariffRateId == rateSchedule.TariffRateId );
                        rateSchedule.TariffRateName = tariffRate.RateName;
                    }

                    // Add the cloned rate schedules to the session.
                    SessionRateSchedules.AddRange(cloneRateScheduleList);
                    SessionRateScheduleConfiguration.RateScheduleCount = SessionRateSchedules.Count;

                    // Update SessionRateScheduleConfiguration stats
                    SessionRateScheduleConfiguration.UpdateStats(SessionRateSchedules);
                }

                model = SessionRateScheduleConfiguration;

            }

            return RedirectToAction("CreateRateScheduleConfig", new { customerId = customerId });
        }

        #endregion

        #region Holiday Rate Configuration Handlers


        public ActionResult CreateHolidayRateConfig(int customerId, string rtn)
        {
            SessionTariffReturnStackPush = rtn;

            return View(SessionHolidayRateConfiguration);
        }

        [HttpPost]
        public ActionResult CreateHolidayRateConfig(string submitButton, HolidayRateConfigurationModel model)
        {
            if (submitButton.Equals("RETURN"))
            {
                return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
            }

            if (submitButton.Equals("CANCEL"))
            {
                // Remove any holiday rates that are not marked as used.
                List<HolidayRateModel> deleteList = new List<HolidayRateModel>();
                foreach (var holidayRate in SessionHolidayRates)
                {
                    if (!holidayRate.IsSaved)
                    {
                        deleteList.Add(holidayRate);
                        // Remove these unused rate schedules from the backing storage.
                        (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                            .DeleteHolidayRate(holidayRate);
                    }
                }
                // Now remove the ones in the deleteList
                foreach (var holidayRate in deleteList)
                {
                    SessionHolidayRates.Remove(holidayRate);
                }

                return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
            }

            if (!SessionHolidayRates.Any())
            {
                // Must be at least one holiday.
                ModelState.AddModelError("RequireAHoliday", "Minimum of one Holiday required.");
                AddError("1234", "Minimum of one Holiday required.");
            }

            if (submitButton.Equals("CONFIGURE"))
            {
                if (SessionTariffRateConfiguration.State != TariffStateType.Current)
                {
                    // Tariff rates must have been configured
                    ModelState.AddModelError("RequireConfiguredTariffs",
                        "Cannot configure Holiday Rates until have configured Tariff Rates.");
                    AddError("1234", "Cannot configure Holiday Rates until have configured Tariff Rates.");
                }

                if (SessionRateScheduleConfiguration.State != TariffStateType.Current)
                {
                    // Rate Schedules must have been configured
                    ModelState.AddModelError("RequireConfiguredSchedule",
                        "Cannot configure Holiday Rates until have configured Rate Schedules.");
                    AddError("1234", "Cannot configure Holiday Rates until have configured Rate Schedules.");
                }

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    // Must have a name
                    ModelState.AddModelError("RequireName", "Name is required.");
                    AddError("1234", "Name is required.");
                }
            }


            if ( ModelState.IsValid )
            {
                var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                // Create a HolidayRateConfiguration entry from SessionHolidayRates
                model = tariffFactory.SaveHolidayRateConfiguration( model.CustomerId, model, SessionHolidayRates);

                // Take a different path if action == CONFIGURE
                if (submitButton.Equals("CONFIGURE"))
                {
                    // Set the State of the HolidayRateConfiguration to 'Current'
                    model = tariffFactory.ConfigureHolidayRateConfiguration( model.CustomerId, model);
                    SessionHolidayRateConfiguration = model;
                    // Now redirect back to the Create process.
                    return RedirectToAction(SessionTariffReturnStackPop, new { customerId = model.CustomerId });
                }
                else
                {
                    // Else stay on edit screen.
                    SessionHolidayRateConfiguration = model;
                    return View(model);
                }
            }
            else
            {
                return View( model );
            }
        }

        public ActionResult SelectHolidayRateConfig(int customerId)
        {
            HolidayRateConfigurationModel model = new HolidayRateConfigurationModel() { CustomerId = customerId };

            return View(model);
        }

        public ActionResult ImportHolidayRateConfig(int customerId, Int64 holidayRateConfigurationId)
        {
            HolidayRateConfigurationModel model = null;

            if (holidayRateConfigurationId != 0)
            {
                // Is this an import of another HolidayRateConfiguration?
                if (holidayRateConfigurationId != SessionHolidayRateConfiguration.HolidayRateConfigurationId)
                {
                    var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                    List<HolidayRateModel> holidayRateModelList = tariffFactory.GetHolidayRates( customerId, holidayRateConfigurationId );

                    // Clear out the day of week data
                    foreach (var holidayRateModel in holidayRateModelList)
                    {
                        holidayRateModel.DayOfWeek = -1;
                        holidayRateModel.DayOfWeekName = "";
                        holidayRateModel.RateScheduleConfigurationId = 0;
                    }

                    // Add this to the SessionHolidayRates
                    SessionHolidayRates.AddRange(holidayRateModelList);

                }
            }

            return RedirectToAction("CreateHolidayRateConfig", new { customerId = customerId });
        }


        #endregion

        #endregion

        #region Data Source Methods (AJAX)

        #region Tariff Rates

        [HttpGet]
        public ActionResult GetTariffRates([DataSourceRequest] DataSourceRequest request, int customerId, Int64 tariffRateConfigurationId)
        {
            // Get TariffRates for a configured TariffRateConfiguration
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetTariffRates(customerId, tariffRateConfigurationId);

            var sortedItems = TariffRateModelUtility.Sort(items);

            var result = new DataSourceResult
            {
                Data = sortedItems,
                Total = sortedItems.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetConfiguredTariffRates([DataSourceRequest] DataSourceRequest request, int customerId, Int64 configProfileId, Int64 tariffRateConfigurationId)
        {
            // Get TariffRates for a configured TariffRateConfiguration
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetConfiguredTariffRates(customerId, configProfileId, tariffRateConfigurationId);

            var sortedItems = TariffRateModelUtility.Sort( items );

            var result = new DataSourceResult
            {
                Data = sortedItems,
                Total = sortedItems.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetTariffRateConfigurationsForImport([DataSourceRequest] DataSourceRequest request, int customerId)
        {
            // Get a list of importable TariffRateConfigurations.
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetTariffRateConfigurationsForImport( customerId );

            // Remove any configurations that are already in session.
            var filteredList =
                items.Where( m => !m.Equals(SessionTariffRateConfiguration) ).ToList();

            var result = new DataSourceResult
            {
                Data = filteredList,
                Total = filteredList.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetTariffRateConfigurationsForSelection(int customerId)
        {
            // Get a list of importable TariffRateConfigurations.
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetTariffRateConfigurationsForImport(customerId);

            // Remove any configurations that are already in session.
            var filteredList =
                items.Where(m => !m.Equals(SessionTariffRateConfiguration)).ToList();

            return Json(filteredList, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Rate Schedules


        [HttpGet]
        public ActionResult GetConfiguredRateScheduleConfigurations([DataSourceRequest] DataSourceRequest request, int customerId, Int64 configProfileId)
        {
            ////get models
            //var items = (new TariffFactory((DateTime)Session[Constants.ViewData.CurrentLocalTime], Session[Constants.Security.ConnectionStringSessionVariableName].ToString()))
            //    .GetConfiguredRateScheduleConfigurations(customerId, configProfileId);

            var result = new DataSourceResult
            {
                Data = null, //items,
                Total = 0, //items.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Get a list of the rate schedules for this <see cref="rateScheduleConfigurationId"/>
        /// </summary>
        /// <param name="request">Kendo data source request instance</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="configProfileId">The Config Profile Id</param>
        /// <param name="rateScheduleConfigurationId">The configuration id for this set of rate schedules.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetConfiguredRateSchedules([DataSourceRequest] DataSourceRequest request, int customerId, Int64 configProfileId, Int64 rateScheduleConfigurationId, int dayOfWeek = -1)
        {
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetConfiguredRateSchedules(customerId, configProfileId, rateScheduleConfigurationId);
            List<RateScheduleModel> sortedItems = null;

            // Get the RateSchedules from session.  If dayOfWeek > -1 then get a specific day.
            if ( dayOfWeek < 0 )
            {
                sortedItems = items;
            }
            else
            {
                sortedItems = items.Where( m => m.DayOfWeek == dayOfWeek ).ToList();
            }

            sortedItems.Sort();

            var result = new DataSourceResult
            {
                Data = sortedItems,
                Total = sortedItems.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetRateScheduleConfigurationsForImport([DataSourceRequest] DataSourceRequest request, int customerId)
        {
            //get models
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetRateScheduleConfigurationsForImport( customerId );

            // Remove any configurations that are already in session.
            var filteredList =
                items.Where( m => !m.Equals(SessionRateScheduleConfiguration) ).ToList();

            var result = new DataSourceResult
            {
                Data = filteredList,
                Total = filteredList.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Holiday Rates


        [HttpGet]
        public ActionResult GetConfiguredHolidayRates([DataSourceRequest] DataSourceRequest request, int customerId, Int64 configProfileId, Int64 holidayRateConfigurationId)
        {
            //get models
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetConfiguredHolidayRates(customerId, configProfileId, holidayRateConfigurationId);

            items.Sort();

            var result = new DataSourceResult
            {
                Data = items,
                Total = items.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetHolidayRateConfigurationsForImport([DataSourceRequest] DataSourceRequest request, int customerId)
        {
            //get models
            var items = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetHolidayRateConfigurationsForImport(customerId);

            // Remove any configurations that are already in session.
            var filteredList =
                items.Where(m => !m.Equals(SessionHolidayRateConfiguration)).ToList();

            var result = new DataSourceResult
            {
                Data = filteredList,
                Total = filteredList.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        #endregion

        #endregion

        #region Tariff Components CRUD (AJAX)

        #region Edit Tariffs Methods (AJAX)



        [HttpGet]
        public ActionResult TariffRateRead([DataSourceRequest] DataSourceRequest request)
        {
            List<TariffRateModel> list = TariffRateModelUtility.Sort( SessionTariffRates );
            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TariffRateCreate([DataSourceRequest] DataSourceRequest request, TariffRateModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                // Is model valid?
                if ( ModelState.IsValid )
                {
                    // Create a tariff.
                    (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).CreateTariffRate(model);

                    // Add this model to the repository
                    SessionTariffRates.Add(model);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TariffRateUpdate([DataSourceRequest] DataSourceRequest request, TariffRateModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                if ( ModelState.IsValid )
                {


                    var target = SessionTariffRates.FirstOrDefault( m => m.TariffRateId == model.TariffRateId );
                    if ( target != null )
                    {
                        target.PerTimeValue = model.PerTimeValue;
                        target.PerTimeUnitId = model.PerTimeUnitId;
                        target.PerTimeUnitName = model.PerTimeUnitName;

                        target.MaxTimeValue = model.MaxTimeValue;
                        target.MaxTimeUnitId = model.MaxTimeUnitId;
                        target.MaxTimeUnitName = model.MaxTimeUnitName;

                        target.RateInCents = model.RateInCents;

                        target.RateDescription = model.RateDescription;

                        target.GracePeriodMinute = model.GracePeriodMinute;

                        target.LockMaxTime = model.LockMaxTime;

                        target.LinkedTariffRateId = model.LinkedTariffRateId;
                        target.LinkedTariffRateName = model.LinkedTariffRateName;

                        target.IsChanged = true;
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TariffRateDestroy([DataSourceRequest] DataSourceRequest request, TariffRateModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                var target = SessionTariffRates.FirstOrDefault(m => m.TariffRateId == model.TariffRateId);
                if (target != null)
                {
                    // Break any links to this tariff
                    foreach (var tariff in SessionTariffRates)
                    {
                        if ( tariff.LinkedTariffRateId == target.TariffRateId )
                        {
                            tariff.LinkedTariffRateId = 0;
                            tariff.LinkedTariffRateName = string.Empty;
                        }
                    }

                    // Remove this tariff from any RateSchedules that were using it.
                    foreach (var rateSchedule in SessionRateSchedules.Where( m => m.TariffRateId == target.TariffRateId ))
                    {
                        rateSchedule.TariffRateId = null;
                        rateSchedule.TariffRateName = "";
                    }

                    // Now remove this tariff.
                    SessionTariffRates.Remove(target);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }



        [HttpPost]
        public ActionResult GetTimeUnits(int customerId)
        {

            List<SelectListItemWrapper> list = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).TimeUnitsList(customerId);

            return Json(new { list });
        }

        /// <summary>
        /// Find any tariff rates that are not linked already and is not the tariff rate
        /// asking for the information.  At this point, any tariff rate besides the tariff
        /// rate asking is a candidate to be linked.
        /// </summary>
        /// <param name="customerId">Integer id of customer</param>
        /// <param name="currentRateId">Int64 id of the current tariff rate</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLinkableRates(int customerId, Int64 currentRateId)
        {
            List<TariffRateModel> unlinkedTariffs = TariffRateModelUtility.UnlinkedTariffs(SessionTariffRates);

            List<SelectListItemWrapper> list = (from tr in unlinkedTariffs
                                                where tr.TariffRateId != currentRateId
                                                select new SelectListItemWrapper()
                                                {
                                                    Selected = false,
                                                    Text = tr.RateName,
                                                    ValueInt64 = tr.TariffRateId
                                                }).ToList();
            
            // If the current rate has a linked tariff then add it to list and select it.
            var currentTariffRate = SessionTariffRates.FirstOrDefault( m => m.TariffRateId == currentRateId );
            if ( currentTariffRate != null && currentTariffRate.LinkedTariffRateId.HasValue && currentTariffRate.LinkedTariffRateId > 0 )
            {
                var linkedTariffRate = SessionTariffRates.FirstOrDefault( m => m.TariffRateId == currentTariffRate.LinkedTariffRateId );
                if ( linkedTariffRate != null )
                {
                    list.Insert(0, new SelectListItemWrapper()
                    {
                        Text = linkedTariffRate.RateName,
                        ValueInt64 = linkedTariffRate.TariffRateId
                    });
                    
                }
            }


            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "0"
            });

            return Json(new { list });
        }

        #endregion

        #region Edit Rate Schedule Methods (AJAX)

        [HttpGet]
        public ActionResult RateScheduleRead([DataSourceRequest] DataSourceRequest request)
        {
            SessionRateSchedules.Sort();
            return Json(SessionRateSchedules.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateScheduleCreate([DataSourceRequest] DataSourceRequest request, RateScheduleModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                // Is model valid?
                if (ModelState.IsValid)
                {
                    // Pull the selected days-of-the-week from the forms collection.
                    List<int> dayOfWeek = new List<int>();
                    // Walk the form fields and set any values in the model to values reflected by
                    // the form fields.  Looking for "[RateScheduleModel.ChkBoxPrepend][RateScheduleModel.ChkBoxDivider]#".  If found, grab the # 
                    foreach (var formValueKey in formColl.Keys)
                    {
                        string[] tokens = formValueKey.ToString().Split(RateScheduleModel.ChkBoxDivider);
                        // Make sure tokens[0] equals RateScheduleModel.ChkBoxPrepend
                        if (tokens.Length == 2 && tokens[0].Equals(RateScheduleModel.ChkBoxPrepend))
                        {
                            // Check the value of the checkbox to see if is actually checked.
                            if ( formColl[formValueKey.ToString()].Equals( "true", StringComparison.CurrentCultureIgnoreCase ) )
                            {
                                // tokens[1] is the day-of-week id.
                                dayOfWeek.Add( int.Parse( tokens[1] ) );
                            }
                        }
                    }

                    // Make sure at least one day was selected.
                    if ( !dayOfWeek.Any() )
                    {
                        ModelState.AddModelError("RequireDayOfWeek", "Rate Schedule requires at least one day.");
                    }
                    else
                    {
                        // Create the RateSchedules.
                        var tariffFactory = new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime);

                        foreach (var day in dayOfWeek)
                        {
                            model.DayOfWeek = day;
                            tariffFactory.CreateRateSchedule(model);

                            // Add this model to the repository
                            SessionRateSchedules.Add(new RateScheduleModel()
                            {
                                CustomerId = model.CustomerId,
                                RateScheduleConfigurationId = model.RateScheduleConfigurationId,
                                RateScheduleId = model.RateScheduleId,
                                DayOfWeek = model.DayOfWeek,
                                DayOfWeekName = model.DayOfWeekName,
                                StartTimeHour = model.StartTimeHour,
                                StartTimeMinute = model.StartTimeMinute,
                                OperationMode = model.OperationMode,
                                OperationModeName = model.OperationModeName,
                                CreatedOn = model.CreatedOn,
                                CreatedBy = model.CreatedBy,
                                UpdatedOn = model.UpdatedOn,
                                UpdatedBy = model.UpdatedBy,
                                TariffRateId = model.TariffRateId,
                                TariffRateName = model.TariffRateName
                            });
                        }

                        SessionRateSchedules.Sort();

                        // Update SessionRateScheduleConfiguration stats
                        SessionRateScheduleConfiguration.UpdateStats(SessionRateSchedules);
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateScheduleUpdate([DataSourceRequest] DataSourceRequest request, RateScheduleModel model, FormCollection formColl)
        {
            if ( model != null )
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if ( TryUpdateModel( model, formColl.ToValueProvider() ) )
                    UpdateModel( model, formColl.ToValueProvider() );

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel( model );

                if ( ModelState.IsValid )
                {


                    var target = SessionRateSchedules.FirstOrDefault( m => m.RateScheduleId == model.RateScheduleId );
                    if ( target != null )
                    {
                        target.DayOfWeekName = model.DayOfWeekName;
                        target.DayOfWeek = model.DayOfWeek;

                        target.StartTimeHour = model.StartTimeHour;
                        target.StartTimeMinute = model.StartTimeMinute;

                        target.OperationModeName = model.OperationModeName;
                        target.OperationMode = model.OperationMode;

                        //target.MessageSequence = model.MessageSequence;
                        //target.LockMaxTime = model.LockMaxTime;

                        target.TariffRateId = model.TariffRateId;
                        target.TariffRateName = model.TariffRateName;

                        target.IsChanged = true;

                        // Update SessionRateScheduleConfiguration stats
                        SessionRateScheduleConfiguration.UpdateStats(SessionRateSchedules);
                    }
                }

            }
            SessionRateSchedules.Sort();

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RateScheduleDestroy([DataSourceRequest] DataSourceRequest request, RateScheduleModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);


                var target = SessionRateSchedules.FirstOrDefault(m => m.RateScheduleId == model.RateScheduleId);
                if (target != null)
                {
                    // Now remove this rateSchedule.
                    SessionRateSchedules.Remove(target);
                }
            }

            SessionRateSchedules.Sort();

            // Update SessionRateScheduleConfiguration stats
            SessionRateScheduleConfiguration.UpdateStats(SessionRateSchedules);

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult GetDaysOfWeek(int customerId)
        {
            List<SelectListItemWrapper> list = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).DaysOfWeekList(customerId);

            return Json(new { list });
        }

        [HttpPost]
        public ActionResult GetOperationModes(int customerId)
        {
            List<SelectListItemWrapper> list = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).OperationModesList(customerId);

            return Json(new { list });
        }


        [HttpPost]
        public ActionResult GetMessageSequences(int customerId)
        {
            List<SelectListItemWrapper> list = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).TariffMessageList(customerId);

            return Json(new { list });
        }

        #endregion

        #region Edit Holiday Rate Methods (AJAX)


        [HttpGet]
        public ActionResult HolidayRateRead([DataSourceRequest] DataSourceRequest request)
        {
            SessionHolidayRates.Sort();
            return Json(SessionHolidayRates.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayRateCreate([DataSourceRequest] DataSourceRequest request, HolidayRateModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                // Is model valid?
                if (ModelState.IsValid)
                {
                    // Create a Holiday.
                    model.RateScheduleConfigurationId = SessionRateScheduleConfiguration.RateScheduleConfigurationId;
                    (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).CreateHolidayRate(model);

                    // Add this model to the repository
                    SessionHolidayRates.Add(model);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayRateUpdate([DataSourceRequest] DataSourceRequest request, HolidayRateModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                if (ModelState.IsValid)
                {
                    var target = SessionHolidayRates.FirstOrDefault(m => m.HolidayRateId == model.HolidayRateId);
                    if (target != null)
                    {
                        target.HolidayName = model.HolidayName;
                        target.HolidayDateTime = model.HolidayDateTime;

                        target.DayOfWeek = model.DayOfWeek;
                        target.DayOfWeekName = model.DayOfWeekName;

                        target.RateScheduleConfigurationId = model.RateScheduleConfigurationId;

                        target.IsChanged = true;
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayRateDestroy([DataSourceRequest] DataSourceRequest request, HolidayRateModel model, FormCollection formColl)
        {
            if (model != null)
            {
                // Rebind model since locale of thread has been set since original binding of model by MVC
                if (TryUpdateModel(model, formColl.ToValueProvider()))
                    UpdateModel(model, formColl.ToValueProvider());

                // Revalidate model after rebind.
                ModelState.Clear();
                TryValidateModel(model);

                var target = SessionHolidayRates.FirstOrDefault(m => m.HolidayRateId == model.HolidayRateId);
                if (target != null)
                {
                    // Now remove this Holiday Rate.
                    SessionHolidayRates.Remove(target);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }



        
        [HttpPost]
        public ActionResult GetSessionRateScheduleValidDayOfWeek(int customerId)
        {
            // Return only days where there are RateSchedules

            List<SelectListItemWrapper> dow = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime)).DaysOfWeekList(customerId);

            List<SelectListItemWrapper> list = new List<SelectListItemWrapper>();

            foreach (var selectListItemWrapper in dow)
            {
                int dowId = int.Parse( selectListItemWrapper.Value );
                if ( dowId >= 0 && SessionRateScheduleConfiguration.DayOfWeek[dowId] == 1)
                {
                    list.Add(selectListItemWrapper);
                }
            }

            list.Insert(0, new SelectListItemWrapper()
            {
                Selected = true,
                Text = "",
                Value = "-1"
            });


            return Json(new { list });
        }



        #endregion

        #region Data Source Methods For Edits (AJAX)

        #region Tariff Rates

        /// <summary>
        /// Determine the next Tariff Rate name based on tariff rates already in the session.
        /// </summary>
        /// <param name="customerId">Integer customer id.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSessionNextTariffRateName(int customerId)
        {
            // Find the largest RateNameIndex presently in the SessionTariffRates list.
            int nextRateIndex = 1;

            if ( SessionTariffRates.Any() )
            {
                nextRateIndex = SessionTariffRates.Max( m => m.RateNameIndex ) + 1;
            }

            return Json(new {RateName = "Rate " + nextRateIndex, RateNameIndex = nextRateIndex} );
        }


        [HttpGet]
        public ActionResult GetSessionTariffRates([DataSourceRequest] DataSourceRequest request, int customerId)
        {
            // Get the tariff rates that are presently in the session storage.
            List<TariffRateModel> list = TariffRateModelUtility.Sort( SessionTariffRates );

            var result = new DataSourceResult
            {
                Data = list,
                Total = list.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Rate Schedules



        /// <summary>
        /// Get a list of the rate schedules for this <see cref="rateScheduleConfigurationId"/>.  Optionally, get only the 
        /// <see cref="RateScheduleModel"/> for a specific day of the week.
        /// </summary>
        /// <param name="request">Kendo data source request instance</param>
        /// <param name="customerId">The customer id</param>
        /// <param name="rateScheduleConfigurationId">The configuration id for this set of rate schedules.</param>
        /// <param name="dayOfWeek">Optional day of week id.  0 = Sunday</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSessionRateSchedules([DataSourceRequest] DataSourceRequest request, int customerId, Int64 rateScheduleConfigurationId, int dayOfWeek = -1)
        {
            List<RateScheduleModel> items = null;

            // Get the RateSchedules from session.  If dayOfWeek > -1 then get a specific day.
            if ( dayOfWeek < 0 )
            {
                items = SessionRateSchedules;
            }
            else
            {
                items = SessionRateSchedules.Where(m => m.DayOfWeek == dayOfWeek).ToList();
            }
            
            items.Sort();

            var result = new DataSourceResult
            {
                Data = items,
                Total = items.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region Holiday Rates

        [HttpGet]
        public ActionResult GetSessionHolidayRates([DataSourceRequest] DataSourceRequest request, int customerId)
        {
            // Get list of HolidayRates from session.
            var items = SessionHolidayRates;

            items.Sort();

            var result = new DataSourceResult
            {
                Data = items,
                Total = items.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        
        [HttpGet]
        public ActionResult GetSessionHolidayRateSchedules([DataSourceRequest] DataSourceRequest request, int customerId, Int64 holidayRateConfigurationId, Int64 holidayRateId, Int64 rateScheduleConfigurationId)
        {
            //get models
            var itemSource = (new TariffFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString(), CurrentCity.LocalTime))
                .GetRateSchedules(customerId, rateScheduleConfigurationId);

            var items = ( from rs in itemSource
                          select new HolidayRateScheduleModel()
                              {
                                  HolidayRateConfigurationId = holidayRateConfigurationId,
                                  HolidayRateId = holidayRateId,
                                  RateScheduleConfigurationId = rateScheduleConfigurationId,
                                  RateScheduleId = rs.RateScheduleId,
                                  CustomerId = customerId,
                                  DayOfWeek = rs.DayOfWeek,
                                  DayOfWeekName = rs.DayOfWeekName,

                                  StartTimeHour = rs.StartTimeHour,
                                  StartTimeMinute = rs.StartTimeMinute,
                                  OperationMode = rs.OperationMode,
                                  OperationModeName = rs.OperationModeName,

                                  //MessageSequence = rs.MessageSequence,

                                  //LockMaxTime = rs.LockMaxTime,
                                  CreatedOn = rs.CreatedOn,
                                  CreatedBy = rs.CreatedBy,

                                  TariffRateId = rs.TariffRateId

                              } ).ToList();


            var result = new DataSourceResult
            {
                Data = items,
                Total = items.Count,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #endregion

        #endregion

        #region Utilities

        private void AddError(string errorCode, string errorMessage)
        {
            var ei = ViewData["__errorItem"] as ErrorItem;
            if ( ei == null )
            {
                ei = new ErrorItem()
                {
                    ErrorCode = errorCode,
                    ErrorMessage = errorMessage
                };
            }
            else
            {
                ei.ErrorCode += ", " + errorCode;
                ei.ErrorMessage += "  " + errorMessage;
            }

            ViewData["__errorItem"] = ei;
        }

        #endregion
    }
}
