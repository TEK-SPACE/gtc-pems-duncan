using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Reports;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Business.Reports
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Reports"/> namespace contains classes for managing Izenda Reports lists display and navigation
    /// from a descriptive URL to the actual Izenda report viewer url.  Organizes reports into categories and presents descriptive
    /// names and urls that can be presented on a web page.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Class to create list of reports for a particular report category (<see cref="ReportCategory"/>).  Provides report discription text
    /// and navigable URL to the actual Izenda report viewer page.  Organizes reports into categories and presents descriptive
    /// names and urls that can be presented on a web page.
    /// </summary>
    public class ReportsFactory : RbacBaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Returns a <see cref="ReportsListViewModel"/> that contains a <see cref="List{ReportViewModel}"/> of reports with
        /// descriptions and URLs for Izenda report viewer.  
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="userName">User name making request.  Not presently used.</param>
        /// <param name="reportCategory">Enumeration <see cref="ReportCategory"/> of the requested report category</param>
        /// <returns>Returns a <see cref="ReportsListViewModel"/> instance</returns>
        public ReportsListViewModel GetReportsList(int customerId, string userName, ReportCategory reportCategory)
        {
            var model = new ReportsListViewModel()
                {
                    CustomerId = customerId,
                    Category = reportCategory,
                    CategoryName =
                        HttpContext.Current.GetLocaleResource(ResourceTypes.ReportCat,
                        RbacEntities.CustomerReportsCategories.FirstOrDefault(m => m.CustomerReportsCategoryId == (int)reportCategory).Name).ToString(),
                    Reports = new List<ReportViewModel>()
                };


            foreach (var report in RbacEntities.CustomerReports.Where(m => m.CustomerId == customerId && m.CustomerReportsCategoryId == (int)reportCategory))
            {
                model.Reports.Add(new ReportViewModel()
                    {
                        Name = HttpContext.Current.GetLocaleResource(ResourceTypes.ReportName, report.ReportAction).ToString(),
                        Description = HttpContext.Current.GetLocaleResource(ResourceTypes.ReportDesc, report.ReportAction).ToString(),
                        Action = report.ReportAction
                    });
            }

            return model;
        }

        /// <summary>
        /// Gets an instance of <see cref="ReportLinkModel"/> that represents the URL of the report on an external site.  This
        /// is used for the case where an Izenda report is displayed in an iFrame from an external site.  The <paramref name="customerId"/>
        /// is appended to the URL as a parameter customerId such that external site can optionally act on that value.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <param name="reportAction">String that represents one of the place-holder Actions in the MVC Controller Duncan.PEMS.Web.Areas.city.Controllers.ReportsController</param>
        /// <returns>An instance of <see cref="ReportLinkModel"/></returns>
        public ReportLinkModel GetReportLink(int customerId, string reportAction)
        {

            var report = RbacEntities.CustomerReports.FirstOrDefault(m => m.CustomerId == customerId && m.ReportAction.Equals(reportAction, StringComparison.CurrentCultureIgnoreCase));


            var model = new ReportLinkModel()
                {
                    Host = report.Host,
                    Path = report.Path,
                    Parameters = report.Parameters
                };

            model.Parameters += "&customerId=" + customerId;


            // Parse out report name from report.Parameter

            string[] tokens = report.Parameters.Split(new char[] { '=', '&' });
            model.ReportName = string.Empty;

            if (tokens.Length == 1)
            {
                model.ReportName = tokens[0];
            }
            if (tokens.Length % 2 == 0)
            {
                // Find the parameter that was "rn", take next token.
                for (int i = 0; i < tokens.Length; i += 2)
                {
                    if (tokens[i].Equals("rn", StringComparison.CurrentCultureIgnoreCase))
                    {
                        model.ReportName = tokens[i + 1];
                        break;
                    }
                }
            }

            return model;
        }


        /// <summary>
        /// Returns a string array of all of the allowable reporting sources for Izenda.
        /// At present, these are the views allowed for each customer.
        /// </summary>
        /// <param name="customerId">Id of the customer</param>
        /// <returns>String array of the allowable sources</returns>
        public string[] AllowableReportingSources(int customerId)
        {
            // TODO:  Pull these from table for each client.
            return new string[] { "ActiveAlarms_SP", "ActiveAlarmsV", "AssetEventListV", "HistoricalAlarmsV", "MaintenanceEventsV", 
                "AssetOperationalStatusV", "EventsAllAlarmsV", "EventsCollectionCommEventV", "EventsCollectionCBRV", "EventsGSMConnectionLogsV", 
                "EventsTransactionsV", "CollReconDetCBRCOMMSsubV", "CreditCardReconciliationV", "LastCollectioNDateTimeV", "CurrentMeterAmountsV", 
                "CustomerPaymentTransactionV", "DailyFinancialTransactionV", "MeterUptimeV", "MSM_Sensor_Gateway_AttribStatExceptsSummV", 
                "OccupancyRateSummaryV", "TotalIncomeSummaryV", "OccupancyRateSummary_SP", "A_EventsAlarmsTransactionsV", "A_LastCollectionAndGSMConnectionV",
                "Low_Battery_AlarmV","MeterCurrentStatusReportV","MeterConfigurationReportV","NoCommunicationV","AssetEventListReportV","MeterHitListReportV",
                "ServiceAuditReportV","MeterUtilizationReportV","VoidReasonSummaryV","NOLAViolationSummaryV","ViolationSummaryByAreaV","ActivityLogSummaryV",
                "DeviceUsageSummaryV","AssetAnalysisV","MeterVoltageReportV","IzendaBatteryReport","TotalIncomeSummaryVPerf", "latestmeterdataV","GeneralSyncreportV",
                "NOLAActivityLogSummaryV","NOLAViolationSummaryV","NOLADeviceUsageSummaryV","NOLAVoidReasonSummaryV","NOLAViolationSummaryByAreaV"};
        }


        /// <summary>
        /// Returns a string array of all of the allowable ad hoc reporting sources for Izenda.
        /// At present, these are the views allowed for each customer.
        /// </summary>
        /// <param name="customerId">Id of the customer</param>
        /// <returns>String array of the allowable sources</returns>
        public string[] AllowableAdHocSources(int customerId)
        {
            string[] sources = (from crah in RbacEntities.CustomerReportsAdHocs where crah.CustomerId == customerId select crah.ReportSource).ToArray();
            return sources;

            //return new string[] { "ActiveAlarms_SP", "ActiveAlarmsV", "AssetEventListV", "HistoricalAlarmsV", "MaintenanceEventsV", 
            //    "AssetOperationalStatusV", "EventsAllAlarmsV", "EventsCollectionCommEventV", "EventsCollectionCBRV", "EventsGSMConnectionLogsV", 
            //    "EventsTransactionsV", "CollReconDetCBRCOMMSsubV", "CreditCardReconciliationV", "LastCollectioNDateTimeV", "CurrentMeterAmountsV", 
            //    "CustomerPaymentTransactionV", "DailyFinancialTransactionV", "MeterUptimeV", "MSM_Sensor_Gateway_AttribStatExceptsSummV", 
            //    "OccupancyRateSummaryV", "TotalIncomeSummaryV", "OccupancyRateSummary_SP", "A_EventsAlarmsTransactionsV", "A_LastCollectionAndGSMConnectionV" };
        }

    }
}
