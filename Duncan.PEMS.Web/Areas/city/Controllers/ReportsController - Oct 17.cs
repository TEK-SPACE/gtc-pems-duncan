using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Reports;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.Reports;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Duncan.PEMS.Web.Controllers;
using NLog;
using WebMatrix.WebData;
using Duncan.PEMS.Entities.BatteryChange;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class ReportsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Report Categories

        public ActionResult ReportAssetStatus()
        {
            var model = ( new ReportsFactory() )
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.AssetStatus);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportAlarmsEvents()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.AlarmsEvents);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportAssetMaintenance()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.AssetMaintenance);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportFinancial()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.Financial);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportCollectionRoutes()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.CollectionRoutes);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportDashboard()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.Dashboard);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportAdHocReports()
        {
            return RedirectToAction("InstantReport", "Reporting");
        }

        public ActionResult ReportPEMSReports()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.PEMSUSA);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportEnforcement()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.Enforcement);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        public ActionResult ReportSensor()
        {
            var model = (new ReportsFactory())
                .GetReportsList(CurrentCity.Id, WebSecurity.CurrentUserName, ReportCategory.Sensor);
            Session[ReportLinkModel.SessionActiveReportGroup] = CurrentAction;
            return View("ReportList", model);
        }

        #endregion

        #region Report Frames - Place Holders
        /// <summary>
        /// Determine if report viewer is invoked via iFrames or via internal Izenda report viewer model.
        /// 
        /// Default to iFrames
        /// </summary>
        /// <returns>Returns <see cref="ReportViewerType"/> enumeration of report viewer type.</returns>
        private ReportViewerType TypeOfReportViewer()
        {
            ReportViewerType type = ReportViewerType.Iframes;
            string viewer = ConfigurationManager.AppSettings[Constants.Security.IzendaReportViewer];

            if ( !string.IsNullOrWhiteSpace( viewer ) )
            {
                if ( viewer.Equals( "iframes", StringComparison.CurrentCultureIgnoreCase ) )
                {
                    type = ReportViewerType.Iframes;
                }
                else if (viewer.Equals("internal", StringComparison.CurrentCultureIgnoreCase))
                {
                    type = ReportViewerType.Internal;
                }
                else
                {
                    type = ReportViewerType.Undefined;
                }
            }
            return type;
        }

        private ActionResult ReportRedirect(ReportLinkModel model)
        {
           Session[ReportLinkModel.SessionActiveReport] = model.ReportName;
           // Add Dashboards to path to allow dashboard reporting style to work
           if (model.Path.Contains("Dashboards"))
              return RedirectToAction("Dashboards", "Reporting", new { rn = model.ReportName });
           else
              return RedirectToAction("ReportViewer", "Reporting", new { rn = model.ReportName });
        }

        private ActionResult ViewReport()
        {
            switch (TypeOfReportViewer())
            {
                case ReportViewerType.Iframes:
                    var model = (new ReportsFactory()).GetReportLink(CurrentCity.Id, CurrentAction);
                    return View("ReportFrame", model);
                case ReportViewerType.Internal:
                    return ReportRedirect((new ReportsFactory()).GetReportLink(CurrentCity.Id, CurrentAction));
            }
            return View("ReportsViewerUndefined");
        }


        public ActionResult RptFrame1()
        {
            return ViewReport();
        }

        public ActionResult RptFrame2()
        {
            return ViewReport();
        }

        public ActionResult RptFrame3()
        {
            return ViewReport();
        }

        public ActionResult RptFrame4()
        {
            return ViewReport();
        }

        public ActionResult RptFrame5()
        {
            return ViewReport();
        }

        public ActionResult RptFrame6()
        {
            return ViewReport();
        }

        public ActionResult RptFrame7()
        {
            return ViewReport();
        }

        public ActionResult RptFrame8()
        {
            return ViewReport();
        }

        public ActionResult RptFrame9()
        {
            return ViewReport();
        }

        public ActionResult RptFrame10()
        {
            return ViewReport();
        }

        public ActionResult RptFrame11()
        {
            return ViewReport();
        }

        public ActionResult RptFrame12()
        {
            return ViewReport();
        }

        public ActionResult RptFrame13()
        {
            return ViewReport();
        }

        public ActionResult RptFrame14()
        {
            return ViewReport();
        }

        public ActionResult RptFrame15()
        {
            return ViewReport();
        }

        public ActionResult RptFrame16()
        {
            return ViewReport();
        }

        public ActionResult RptFrame17()
        {
            return ViewReport();
        }

        public ActionResult RptFrame18()
        {
            return ViewReport();
        }

        public ActionResult RptFrame19()
        {
            return ViewReport();
        }

        public ActionResult RptFrame20()
        {
            return ViewReport();
        }

        public ActionResult RptFrame21()
        {
            return ViewReport();
        }

        public ActionResult RptFrame22()
        {
            return ViewReport();
        }
        
        public ActionResult RptFrame23()
        {
            return ViewReport();
        }

        public ActionResult RptFrame24()
        {
            return ViewReport();
        }

        public ActionResult RptFrame25()
        {
            return ViewReport();
        }

        public ActionResult RptFrame26()
        {
            return ViewReport();
        }

        public ActionResult RptFrame27()
        {
            return ViewReport();
        }

        public ActionResult RptFrame28()
        {
            return ViewReport();
        }

        public ActionResult RptFrame29()
        {
            return ViewReport();
        }

        public ActionResult RptFrame30()
        {
            return ViewReport();
        }

        public ActionResult RptFrame31()
        {
            return ViewReport();
        }
        public ActionResult RptFrame32()
        {
            return ViewReport();
        }
        public ActionResult RptFrame33()
        {
            return ViewReport();
        }
        #endregion

        #region BattaryChange
        public ActionResult BatteryVoltageIndex()
        {
            BatteryChangeModel objGISModel = new BatteryChangeModel();

            try
            {
                ViewBag.CurrentCityID = CurrentCity.Id;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN BatteryVoltage method (Reports Ctrller)", ex);
            }

            return View(objGISModel);
        }

        public ActionResult BatteryChangeByDay()
        {
            BatteryChangeModel objGISModel = new BatteryChangeModel();

            try
            {
                ViewBag.CurrentCityID = CurrentCity.Id;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN BatteryChangeByDay method (Report Ctrller)", ex);
            }

            return View(objGISModel);
        }

        public ActionResult BatteryAnalysis()
        {
            BatteryChangeModel objGISModel = new BatteryChangeModel();

            try
            {
                ViewBag.CurrentCityID = CurrentCity.Id;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN BatteryAnalysis method (Report Ctrller)", ex);
            }

            return View(objGISModel);
        }

        public ActionResult OccRate()
        {
            BatteryChangeModel objGISModel = new BatteryChangeModel();

            try
            {
                ViewBag.CurrentCityID = CurrentCity.Id;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN OccRate method (Reports Ctrller)", ex);
            }

            return View(objGISModel);
        }

        public ActionResult sensorprofile()
        {
            BatteryChangeModel objGISModel = new BatteryChangeModel();

            try
            {
                ViewBag.CurrentCityID = CurrentCity.Id;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN sensor profiles method (Reports Ctrller)", ex);
            }

            return View(objGISModel);
        }
        #endregion
    }
}
