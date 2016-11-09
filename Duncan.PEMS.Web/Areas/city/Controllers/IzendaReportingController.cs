using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Routing;
using System.Web.UI.WebControls;
using Duncan.PEMS.Business.ConditionalValues;
using Duncan.PEMS.Business.Reports;
using Duncan.PEMS.Business.Resources;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Reports;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Duncan.PEMS.Web.Reporting;
using Izenda.AdHoc;
using System.Text;
using NLog;
using Filter = Izenda.AdHoc.Filter;


namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class ReportingController : PemsController
	{
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion




        public ActionResult ReportDesigner()
        {
            InitializeReporting(ControllerContext);
            return View();
        }

        public ActionResult ReportList()
        {
            InitializeReporting(ControllerContext);
            return View();
        }

        public ActionResult Settings()
        {
            InitializeReporting(ControllerContext);
            return View();
        }

        public ActionResult Dashboards()
        {
            InitializeReporting(ControllerContext);
            return View();
        }

        public ActionResult ReportViewer()
        {
            // Check requested report against actual report being displayed.
            string reportNameViaURL = ControllerContext.HttpContext.Request.QueryString["rn"] as string;
            string reportNameRequested = Session[ReportLinkModel.SessionActiveReport] as string;

            // TODO:  Restore this check after all reports show correctly and testing is complete.
            
            // If reportNameViaURL == reportNameRequested then ok to display report.
            if ( true ) //reportNameViaURL.Equals( reportNameRequested ) )
            {
                InitializeReporting(ControllerContext);
                InitializeAllowableReportingSources();
                string strSQLConnectionString = AdHocSettings.SqlServerConnectionString;
                if (reportNameViaURL == Constants.Reporting.ActivityLogSummary || reportNameViaURL == Constants.Reporting.ViolationSummary || reportNameViaURL == Constants.Reporting.ViolationSummaryByArea || reportNameViaURL == Constants.Reporting.ViolationSummaryByOfficer || reportNameViaURL == Constants.Reporting.VoidReasonSummary || reportNameViaURL == Constants.Reporting.DeviceUsageSummary)
                {
                    //AdHocSettings.OracleConnectionString = "Data Source=AIPRO2;Persist Security Info=True;User ID=VGANESAN;password=atOu!8%2n;Unicode=True;";
                    AdHocSettings.OracleConnectionString = "Data Source=AIPRO2;Persist Security Info=True;User ID=CITATIONSDATA;password=Citation3ata;Unicode=True;";
                    AdHocSettings.ExtendedFunctions = new string[0];
                }
                if(reportNameViaURL == ConfigurationManager.AppSettings[Constants.Reporting.ActivityLogSummary])
                {

                }

                AdHocSettings.ShowSimpleModeViewer = true;
                //AdHocSettings.SqlServerConnectionString = strSQLConnectionString;
                return View();
            }
            else
            {
                var model = new ReportErrorModel
                    {
                        ReturnAction = Session[ReportLinkModel.SessionActiveReportGroup] as string,
                        ReturnController = "Reports",
                        ErrorDescription = "Invalid report requested."
                    };

                return View( "InvalidReportRequest", model );
            }
        }

        public ActionResult InstantReport()
        {
            InitializeReporting(ControllerContext);
            InitializeAllowableAdHocSources();
            return View();
        }

        public ActionResult DashboardDesigner()
        {
            InitializeReporting(ControllerContext);
            InitializeAllowableAdHocSources();
            return View();
        }

        public ActionResult ShowAdHoc()
        {
            var model = new IzendaAdHocModel();

            var hiddens = AdHocSettings.HiddenFilters.Keys;
            foreach (var hidden in hiddens)
            {
                model.HiddenFilters.Add(hidden.ToString(), AdHocSettings.HiddenFilters[hidden.ToString()].ToString());
            }

            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult FormDesigner()
        {
            string templateName = Request.Params["rn"];
            if (String.IsNullOrEmpty(templateName))
                return this.Content("Template name not specified");
            templateName = templateName.Replace("\\\\", "\\");

            string saveData = Request.Params["editor"];
            if (!String.IsNullOrEmpty(saveData))
            {
                bool doNotDoAnything = false;
                string goCancel = Request.Params["gocancel"];
                if (!String.IsNullOrEmpty(goCancel) && goCancel == "1")
                    doNotDoAnything = true;
                if (!doNotDoAnything)
                    AdHocSettings.AdHocConfig.SetVolatileTemplate(templateName, saveData);
                string goBack = Request.Params["goback"];//save clicked
            }
            string delete = Request.Params["delete"];
            if (!String.IsNullOrEmpty(delete) && delete == "1")
                AdHocSettings.AdHocConfig.DeleteTemplateInternal(templateName);
            string templateData = AdHocSettings.AdHocConfig.GetVolatileTemplate(templateName);

            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append("<script src=\"/Reporting/elrte/js/jquery-1.6.1.min.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            sb.Append("<script src=\"/Reporting/elrte/js/jquery-ui-1.8.13.custom.min.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            sb.Append("<link rel=\"stylesheet\" href=\"/Reporting/elrte/css/smoothness/jquery-ui-1.8.13.custom.css\" type=\"text/css\" media=\"screen\" charset=\"utf-8\">");
            sb.Append("<script src=\"/Reporting/elrte/js/elrte.full.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            sb.Append("<link rel=\"stylesheet\" href=\"/Reporting/elrte/css/elrte.min.css\" type=\"text/css\" media=\"screen\" charset=\"utf-8\">");
            sb.Append("<script src=\"/Reporting/elrte/js/i18n/elrte.ru.js\" type=\"text/javascript\" charset=\"utf-8\"></script>");
            sb.Append("<script type=\"text/javascript\" charset=\"utf-8\">	  $().ready(function() { var opts = { cssClass: 'el-rte', width:950, height: 350, toolbar: 'maxi', cssfiles: ['/Reporting/elrte/css/elrte-inner.css'] }; $('#editor').elrte(opts); if ($(\".docstructure\") && $(\".docstructure\").attr(\"class\").indexOf(\"active\") != -1) $(\".docstructure\").click(); $(\".formatblock\").click(function(e){if ($(this).attr(\"class\").indexOf(\"disabled\") != -1){e.preventDefault();e.stopPropagation();this.onclick=undefined;return false;}else return true;});$(\".el-rte .toolbar .panel-direction\").hide(); });</script>");
            sb.Append("<script type=\"text/javascript\" charset=\"utf-8\">	  function reinitialize() {var act = document.getElementById('saveForm').action; var gbPos = act.indexOf('&goback'); if (gbPos >= 0) {act = act.substr(0, gbPos);} var gcPos = act.indexOf('&gocancel'); if (gcPos >= 0) {act = act.substr(0, gcPos);} document.getElementById('saveForm').action = act;}</script>");
            sb.Append("</head><body>");
            sb.Append("<form id=\"saveForm\" method=\"POST\" action=\"./FormDesigner?rn=" + Request.Params["rn"] + "\">");
            sb.Append("<input id=\"cswBackButton\" type=\"submit\" value=\"OK\" onclick=\"document.getElementById('saveForm').action += '&goback=1';parent.closeIFrame();\" style=\"background-color:LightGray;border:1px solid DarkGray;\" />&nbsp;");
            sb.Append("<input id=\"cswCancelButton\" type=\"submit\" value=\"Cancel\" onclick=\"document.getElementById('saveForm').action += '&gocancel=1';parent.closeIFrame();\" style=\"background-color:LightGray;border:1px solid DarkGray;\" />&nbsp;");
            sb.Append("<input id=\"cswDeleteButton\" type=\"submit\" value=\"Delete\" onclick=\"document.getElementById('saveForm').action += '&delete=1';parent.closeIFrame();\" style=\"background-color:LightGray;border:1px solid DarkGray;\" /><br /><br />");
            string message = "";
            bool showMsg = false;
            HttpBrowserCapabilitiesBase brObject = Request.Browser;
            if (brObject.Type.ToLower().StartsWith("ie"))
            {
                float ver;
                try
                {
                    ver = float.Parse(brObject.Version);
                }
                catch
                {
                    ver = 0;
                }
                if (ver < 9)
                    showMsg = true;
            }
            if (showMsg)
                message = "<br />The Forms Designer works requires at least Internet Exporer 9.0 an HTML5 compatible browser";

            sb.Append("<div id=\"editor\">" + templateData + "</div></form>" + message + "</body></html>");

            return this.Content(sb.ToString());
        }


        private void InitializeReporting(ControllerContext context)
        {
            // Set hidden filter for customer id
            // TODO:  Need to add the appropriate hidden filters here to filter on customer id.  The way Izenda works is that:
            // If the data source is a table or view, then the hidden filter will be a where clause (if filter key matches column name)
            // If the data source is a store procedure then the hidden filter will be a parameter (if filter key matches parameter name)
            // 
            // If you are changing this I suggest you do some research on how Izenda deals with hidden filters.
            // http://www.izenda.com/Site/CodeSample/CodeSample.aspx?setting=HiddenFilters
            if ( CurrentCity != null && CurrentCity.Id > 0 )
            {
                AdHocSettings.HiddenFilters["CustomerId"] = CurrentCity.Id; // One form of customer id column name.
                AdHocSettings.HiddenFilters["CustomerID"] = CurrentCity.Id; // Another form of customer id column name.
            }

        

            // Get the Izenda license key.
            AdHocSettings.LicenseKey = ConfigurationManager.AppSettings[Constants.Security.IzendaLicenseKey];

            // Get the Izenda database connection string
            // The connection string for the reporting sql server is specific to a customer.  Set it here.
            // If no reporting server then fall back to default reporting server
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[CurrentCity.ReportingConnectionStringName];
            string connectionString = connectionStringSettings != null ? connectionStringSettings.ConnectionString : string.Empty;

            // Finally, if connection string could not be determined fall back to default Izenda reporting server connection string.
            if ( string.IsNullOrEmpty( connectionString ) )
            {
                string defaultConnectionName = ConfigurationManager.AppSettings[Constants.Security.DefaultReportingConnectionStringName];
                if ( !string.IsNullOrWhiteSpace( defaultConnectionName ) )
                {
                    connectionStringSettings = ConfigurationManager.ConnectionStrings[defaultConnectionName];
                    connectionString = connectionStringSettings != null ? connectionStringSettings.ConnectionString : string.Empty;
                }
            }
            AdHocSettings.SqlServerConnectionString = connectionString;
            //AdHocSettings.OracleConnectionString = "Data Source=AIPRO2;Persist Security Info=True;User ID=CITATIONSDATA;password=Citation3ata;Unicode=True;";
            // If reporting already initialized for this session then exit.
            

            // General Izenda configuration
            AdHocSettings.AdHocConfig = new FileSystemAdHocConfig();

            // AdHocSettings.CurrentUserTenantId - Segregates ReportsController saved by user 
            // Special TenantId = "_global_" points to the common "user"
            
            // Determine the locations of all of the functions that Izenda controller supports.
            string absUri = context.HttpContext.Request.Url.AbsoluteUri;
            string pathAndQ = context.HttpContext.Request.Url.PathAndQuery;
            string appPath = context.HttpContext.Request.ApplicationPath;
            int pqStart = absUri.LastIndexOf(pathAndQ);
            string siteUrl = absUri.Substring(0, pqStart) + appPath;
            if (siteUrl.EndsWith("/"))
                siteUrl = siteUrl.Substring(0, siteUrl.Length - 1);

            int actionOffset = absUri.LastIndexOf(context.RequestContext.RouteData.Values["action"] as string);
            string uriWithController = absUri.Substring( 0, actionOffset );

            AdHocSettings.ReportViewer = uriWithController + "ReportViewer";
            AdHocSettings.InstantReport = uriWithController + "InstantReport";
            AdHocSettings.DashboardViewer = uriWithController + "Dashboards";
            AdHocSettings.ReportDesignerUrl = uriWithController + "ReportDesigner";
            AdHocSettings.DashboardDesignerUrl = uriWithController + "DashboardDesigner";
            AdHocSettings.ReportList = uriWithController + "ReportList";
            AdHocSettings.FormDesignerUrl = uriWithController + "FormDesigner";
            AdHocSettings.SettingsPageUrl = uriWithController + "Settings";
            AdHocSettings.ParentSettingsUrl = uriWithController + "Settings";

            var hiddenColumns = GetCustomerHiddenColumns(CurrentCity.Id);
            AdHocSettings.HiddenColumns = hiddenColumns.ToArray();

            // Get the location of the Izenda response server
            AdHocSettings.ResponseServer = siteUrl + "/Reporting/rs.aspx";

            // Location of reports.
            AdHocSettings.ReportsPath = Server.MapPath(ConfigurationManager.AppSettings[Constants.Security.IzendaReportLocation]);
            AdHocSettings.ShowHelpButton = true;
            AdHocSettings.AllowMultilineHeaders = true;
            AdHocSettings.ReportViewerDefaultPreviewResults = 1000;
            AdHocSettings.DefaultPreviewResults = 1000;

            // TODO:  To limit data sources to a subset of what is on db server.
            //AdHocSettings.VisibleDataSources = new string[];

            // TODO:  Use the locale format strings to set up an Izenda report to display correct date/time formatting.
            // Common report date and date/time formats
            AdHocSettings.Formats["PEMS ShortDate"] = "{0:" + CurrentCity.Locale.DateTimeFormat.ShortDatePattern + "}";
            AdHocSettings.Formats["PEMS DateTime"] = "{0:" + CurrentCity.Locale.DateTimeFormat.ShortDatePattern + " " + CurrentCity.Locale.DateTimeFormat.LongTimePattern + "}";
            AdHocSettings.Formats["AU Date"] = "{0:dd/MM/yyyy}";
            AdHocSettings.Formats["AU Datetime"] = "{0:dd/MM/yyy hh:mm:ss tt}";

            AdHocSettings.ShowSideHelp = true;
            AdHocSettings.GenerateThumbnails = false;
            AdHocSettings.NumChartTabs = 3;
            AdHocSettings.TabsCssUrl = siteUrl + "/Reporting/Resources/css/tabs.css";
            AdHocSettings.ReportCssUrl = siteUrl + "/Reporting/Resources/css/Report.css";
            AdHocSettings.ShowBetweenDateCalendar = true;
            AdHocSettings.ShowSimpleModeViewer = true;
            AdHocSettings.PrintMode = PrintMode.Html2PdfAndHtml;
            AdHocSettings.ShowPoweredByLogo = false;

            AdHocSettings.DataCacheInterval = 0;
            AdHocSettings.SqlCommandTimeout = 120;

            AdHocSettings.VisibleDataSources = new string[] { "ActiveAlarms_SP", "ActiveAlarmsV", "AssetEventListV", "HistoricalAlarmsV", "MaintenanceEventsV", 
                "AssetOperationalStatusV", "EventsAllAlarmsV", "EventsCollectionCommEventV", "EventsCollectionCBRV", "EventsGSMConnectionLogsV", 
                "EventsTransactionsV", "CollReconDetCBRCOMMSsubV", "CreditCardReconciliationV", "LastCollectioNDateTimeV", "CurrentMeterAmountsV", 
                "CustomerPaymentTransactionV", "DailyFinancialTransactionV", "MeterUptimeV", "MSM_Sensor_Gateway_AttribStatExceptsSummV", "OccupancyRateSummaryV",
                "TotalIncomeSummaryV", "OccupancyRateSummary_SP", "A_EventsAlarmsTransactionsV", "A_LastCollectionAndGSMConnectionV", "Low_Battery_AlarmV", 
                "MeterCurrentStatusReportV", "MeterConfigurationReportV", "NoCommunicationV", "AssetEventListReportV","ServiceAuditReportV","MeterHitListReportV",
                "MeterUtilizationReportV","VoidReasonSummaryV","ViolationSummaryByAreaV","ActivityLogSummaryV",
                "DeviceUsageSummaryV","AssetAnalysisV","MeterVoltageReportV","IzendaBatteryReport","TotalIncomeSummaryVPerf","latestmeterdataV","GeneralSyncreportV",
                "NOLAActivityLogSummaryV","NOLAViolationSummaryV","NOLADeviceUsageSummaryV","NOLAVoidReasonSummaryV","NOLAViolationSummaryByAreaV"};
            AdHocSettings.ExtendedFunctions = new string[] { "ufn_AllSpacesEvents" };
            AdHocSettings.AllowEqualsSelectForStoredProcedures = true;

            AdHocSettings.ReportSetEventWatchers.Add(new FiltersReportSetEventWatcher());

            // Custom labels support.
            var nvc = ResourceFactory.GetCustomerLocaleResources(ResourceTypes.GridColumn, CurrentCity.Locale.Name, CurrentCity.Id) as Dictionary<string, string>;
            if (nvc != null)
            {
                foreach (var nvp in nvc)
                {
                    if (!string.IsNullOrWhiteSpace(nvp.Key) && !string.IsNullOrWhiteSpace(nvp.Value))
                        AdHocSettings.FieldAliases.Add(nvp.Key, nvp.Value);
                }
            }
            nvc = ResourceFactory.GetCustomerLocaleResources(ResourceTypes.Label, CurrentCity.Locale.Name, CurrentCity.Id) as Dictionary<string, string>;
            if (nvc != null)
            {
                foreach (var nvp in nvc)
                {
                    if (!string.IsNullOrWhiteSpace(nvp.Key) && !string.IsNullOrWhiteSpace(nvp.Value))
                        AdHocSettings.FieldAliases.Add(nvp.Key, nvp.Value);
                }
            }

            
            // Mark Izenda Reporting as initilized.
            Session[Constants.Reporting.ReportingInitializedForSession + CurrentCity.Id] = true;
            //if (Session[Constants.Reporting.ReportingInitializedForSession + CurrentCity.Id] != null)
            //{
            //    return;
            //}
        }

        public JsonResult GetCustomerHiddenFilters(int customerId)
        {
            var hiddenColumns = GetCustomerHiddenColumns(customerId);
            return Json(hiddenColumns);
        }


        /// <summary>
        /// Generate a list of columns that need to be hidden for this customer
        /// </summary>
        /// <returns></returns>
        public List<string> GetCustomerHiddenColumns(int customerId)
        {
            //this is how to hide a column. do the checks here
            var hiddenColumns = new List<string>();

            //Zone
            if (!ConditionalValueFactory.DisplayField(customerId, Constants.HiddenFields.FieldZone))
            {
             //  hiddenColumns.Add("Zone");
             //   hiddenColumns.Add("ZoneId");
            }

            //Demand Zone, Demand Area,
            if (!ConditionalValueFactory.DisplayField(customerId, Constants.HiddenFields.FieldDemandArea))
            {
              //  hiddenColumns.Add("Demand Zone");
              //  hiddenColumns.Add("DemandZone");
             //   hiddenColumns.Add("Demand Area");
              //  hiddenColumns.Add("DemandArea");
            }
            //Custom Group 1, Suburb
            if (!ConditionalValueFactory.DisplayField(customerId, Constants.HiddenFields.FieldCG1))
                hiddenColumns.Add("Suburb");

            //Customer Group 2
            if (!ConditionalValueFactory.DisplayField(customerId, Constants.HiddenFields.FieldCG2))
            {
                //add custom group 2
            }

            //Custom Group 3
            if (!ConditionalValueFactory.DisplayField(customerId, Constants.HiddenFields.FieldCG3))
            {
                //add custom group 3
            }

            //Discount Scheme
            //Custom Group 3
            if (!ConditionalValueFactory.DisplayField(customerId, Constants.HiddenFields.FieldDiscountScheme))
            {
                hiddenColumns.Add("Discount Scheme");
                hiddenColumns.Add("DiscountScheme");
            }
            return hiddenColumns;
        }
		
        /// <summary>
        /// Sets the Izenda visible data sources for report viewing.
        /// </summary>
        private void InitializeAllowableReportingSources()
        {
            AdHocSettings.VisibleDataSources = ( new ReportsFactory() ).AllowableReportingSources( CurrentCity.Id );            
            AdHocSettings.ExtendedFunctions = new string[] { "ufn_AllSpacesEvents" };
        }

        /// <summary>
        /// Sets the Izenda visible data sources for ad hoc.
        /// </summary>
        private void InitializeAllowableAdHocSources()
        {
            AdHocSettings.VisibleDataSources = (new ReportsFactory()).AllowableAdHocSources( CurrentCity.Id);
            AdHocSettings.ExtendedFunctions = new string[] { "ufn_AllSpacesEvents" };
        }
		
	}

    [Serializable]
    public class FiltersReportSetEventWatcher : IReportSetEventWatcher
    {
        private void CleanAdditionalFilters(ReportSet reportSet)
        {
            FilterCollection filters = new FilterCollection();
            foreach (Filter filter in reportSet.Filters)
                if (!(filter is AdditionalFilter))
                    filters.Add(filter);
            reportSet.Filters.Clear();
            reportSet.Filters.AddRange(filters.ToArray());


            // Make sure accessing reportSet.ReportName does not throw an exception.
            string reportName = null;
            try
            {
                reportName = Utility.GetWebReportName(reportSet);
            }
            catch (Exception)
            {
                // Do nothing.
            }

            if (reportName != null)
            {
                AdditionalFiltersStorage.CleanAdditionalFilterFromSession(reportName);
            }
            
        }

        private void ResetDescription(ReportSet reportSet)
        {
            if (reportSet.GetVolatileOption("DefaultDescription") != null)
            {
                reportSet.Description = reportSet.GetVolatileOption("DefaultDescription") as string;
                reportSet.RemoveVolatileOption("DefaultDescription");
            }
        }

        private void PopulateFiltersDescription(ReportSet reportSet)
        {
            reportSet.ShowReportParameters = false;
            ResetDescription(reportSet);
            if (reportSet.Filters.Count > 0)
            {
                reportSet.SetVolatileOption("DefaultDescription", reportSet.Description);

                string result = reportSet.Description ?? "";
                foreach (Izenda.AdHoc.Filter filter in reportSet.Filters)
                    if (filter != null && !filter.Hidden && !string.IsNullOrEmpty(filter.Column) && (filter.Value != null && filter.Value != "..." && filter.Value != "" || filter.Values.GetValue(0) != null && filter.Values.GetValue(1) != null))
                    {
                        string columnAlias = null;

                        if (reportSet.Detail != null && reportSet.Detail.Fields != null)
                        {
                            foreach (Field field in reportSet.Detail.Fields)
                            {
                                if (field.ColumnName == filter.dbColumn.FullName)
                                {
                                    columnAlias = field.ClearDescription;
                                }
                            }
                        }
                        columnAlias = filter.Description;

                        if (filter.dbColumn != null && columnAlias == null)
                        {
                            columnAlias = AdHocSettings.FieldAliases[filter.dbColumn.Name] ?? AdHocSettings.FieldAliases[filter.dbColumn.FullName];
                            if (columnAlias == null)
                            {
                                columnAlias = filter.dbColumn.Name;
                                foreach (string str in AdHocSettings.FieldAliases.AllKeys)
                                    if (columnAlias.Contains(str))
                                        columnAlias = columnAlias.Replace(str, AdHocSettings.FieldAliases[str]);
                            }
                        }
                        if (columnAlias == null)
                            columnAlias = filter.dbColumn == null ? filter.Description : filter.dbColumn.Name;
                        result += Environment.NewLine;
                        result += columnAlias;
                        result += "  ";
                        string operatorName = Utility.GetOperatorName(filter.Operator, filter.Not);

                        result += operatorName;
                        if (filter.Value != null)
                        {
                            string val = filter.Value.ToString();
                            switch (filter.Operator)
                            {
                                case OperatorTypes.InTimePeriod:
                                    ListItemCollection lic = ResponseServer.GetPeriodListCollection();
                                    foreach (ListItem li in lic)
                                    {
                                        if (li.Value == val)
                                            result += "  " + li.Text;
                                    }
                                    break;
                                default:
                                    result += "  " + filter.Value;
                                    break;
                            }
                        }
                        else if (filter.Values.Length == 2 && filter.Values.GetValue(0) != null && filter.Values.GetValue(1) != null)
                            result += "  " + filter.Values.GetValue(0) + "  " + filter.Values.GetValue(1);
                    }
                reportSet.Description = result;
            }
        }

        public void PreExecuteReportSet(ReportSet reportSet)
        {
            // Make sure accessing reportSet.ReportName does not throw an exception.
            string reportName = null;
            try
            {
                reportName = Utility.GetWebReportName(reportSet);
            }
            catch (Exception)
            {
                // Do nothing.
            }

            if (reportName != null)
            {
                FilterCollection additionalFilters = AdditionalFiltersStorage.GetAdditionalFiltersFromSession( reportName );
                RsDuncan.AddRangeSafe( reportSet.Filters, additionalFilters );
            }

            PopulateFiltersDescription(reportSet);
        }

        public void PostExecuteReportSet(ReportSet reportSet)
        {
            CleanAdditionalFilters(reportSet);
        }

        public void PreSaveReportSet(string name, ReportSet reportSet)
        {
            CleanAdditionalFilters(reportSet);
            ResetDescription(reportSet);
        }

        public void PostSaveReportSet(string name, ReportSet reportSet)
        {
        }

        public void PreLoadReportSet(string name)
        {
        }

        public void PostLoadReportSet(string name, ReportSet reportSet)
        {

        }
    }



}
