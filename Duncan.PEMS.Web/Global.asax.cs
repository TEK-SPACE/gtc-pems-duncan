using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Duncan.PEMS.Entities.Errors;
using Duncan.PEMS.Framework.ViewEngine;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using Izenda.AdHoc;

using RBACToolbox;
using RBACProvider;

using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;
using System.Threading.Tasks;


namespace Duncan.PEMS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static AppRBACInterface RBAC = null;

        protected void Application_Start()
        {
            RouteTable.Routes.MapPageRoute("blank_rs_aspx_request",
                            "{*aspx}",//catch all url 
                            "~/Reporting/blank_rs_aspx.html",
                            false,
                            null,
                            new RouteValueDictionary { { "aspx", new BlankIFrameRouteConstraint() } }
                           );


            // This route is exlicity for Izenda and routing calls to their rs.aspx "server".
            // This route entry must be very early in the routing dictonary.
            RouteTable.Routes.MapPageRoute("all",
                            "{*aspx}",//catch all url 
                            "~/Reporting/rs.aspx",
                            false,
                            null,
                            new RouteValueDictionary { { "aspx", new AspxRouteConstraint() } }
                           );




            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RouteTable.Routes.IgnoreRoute("{*allelrte}", new { allelrte = @".*elrte(/.*)?" });

            RouteTable.Routes.IgnoreRoute("App_Data/{*relpath}");
            RouteTable.Routes.IgnoreRoute("Reporting/{*relpath}");
            RouteTable.Routes.IgnoreRoute("Content/{*relpath}");
            RouteTable.Routes.IgnoreRoute("Help/{*relpath}");
            RouteTable.Routes.IgnoreRoute("css/{*relpath}");
            RouteTable.Routes.IgnoreRoute("Images/{*relpath}");
            RouteTable.Routes.IgnoreRoute("Scripts/{*relpath}");


            // This forces MVC to handle routing for all files whether or not they match existing files.
            RouteTable.Routes.RouteExistingFiles = true;


            AreaRegistration.RegisterAllAreas();

            //// Call Izenda's routing 
            //IzendaRouteConfig.RegisterRoutes(RouteTable.Routes);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // Register View Engines
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new PemsRazorViewEngine());
            ViewEngines.Engines.Add(new PemsWebFormViewEngine());

            // Initialize membership database connection
            SecurityManager.InitializeMembershipConnection();

            // Add cache provider
            ModelMetadataProviders.Current = new CachedDataAnnotationsModelMetadataProvider();
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;



            if (ConfigurationManager.AppSettings["PaymentXMLSource"] == null)
                CustomerLogic.CustomerManager.Instance.PaymentXMLSource = "";
            else
                CustomerLogic.CustomerManager.Instance.PaymentXMLSource = ConfigurationManager.AppSettings["PaymentXMLSource"];

            if (ConfigurationManager.AppSettings["PAMServerTCPAddress"] == null)
                CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress = "";
            else
                CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress = ConfigurationManager.AppSettings["PAMServerTCPAddress"];

            if (ConfigurationManager.AppSettings["PAMServerTCPPort"] == null)
                CustomerLogic.CustomerManager.Instance.PAMServerTCPPort = "";
            else
                CustomerLogic.CustomerManager.Instance.PAMServerTCPPort = ConfigurationManager.AppSettings["PAMServerTCPPort"];


            //string appDataPhysPath = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data");
           // // Launch the OverstayVio DB task in seperate thread so we can appear more responsive
           // Task taskOverStayDBInit = Task.Factory.StartNew(() => OverstayVioActionDBManager.Instance.Initialize(appDataPhysPath));

        }

        //void Application_AcquireRequestState(object sender, EventArgs e)
        //{
        //    CustomAdHocConfig.InitializeReporting();
        //} 

        //void Session_Start(object sender, EventArgs e)
        //{
        //    AdHocSettings.LicenseKey = ConfigurationManager.AppSettings["IzendaLicKey"];
        //    AdHocSettings.SqlServerConnectionString = ConfigurationManager.AppSettings["MsConnectionString"];
        //    AdHocSettings.GenerateThumbnails = true;
        //    AdHocSettings.DefaultReportBorderColor = "#FFFFFF";
        //    AdHocSettings.ShowMapTab = true;
        //    AdHocSettings.NumChartTabs = 3;
        //    AdHocSettings.TabsCssUrl = "~/Reporting/Resources/css/tabs.css";
        //    AdHocSettings.ReportCssUrl = "~/Reporting/Resources/css/Report.css";
        //    AdHocSettings.ShowBetweenDateCalendar = true;
        //    AdHocSettings.ShowSaveInReportViewer = true;
        //    AdHocSettings.ShowSimpleModeViewer = true;
        //    AdHocSettings.PrintMode = PrintMode.Html2PdfAndHtml;

        //    AdHocSettings.AdHocConfig = new CustomAdHocConfig();
        //}

    }

    //[Serializable]
    //public class CustomAdHocConfig : FileSystemAdHocConfig
    //{
    //    public static void InitializeReporting()
    //    {
    //        if (HttpContext.Current.Session == null || HttpContext.Current.Session["ReportingInitialized"] != null)
    //            return;
    //        if (HttpContext.Current.Request == null || HttpContext.Current.Request.RequestContext == null)
    //            return;
    //        string reportingPath = null;
    //        try
    //        {
    //            UrlHelper uh = new UrlHelper(HttpContext.Current.Request.RequestContext);
    //            foreach (RouteBase rb in uh.RouteCollection)
    //            {
    //                if (rb is Route)
    //                {
    //                    Route route = (Route)rb;
    //                    if (route.DataTokens != null && route.DataTokens.ContainsKey("RouteName") && route.DataTokens["RouteName"].ToString() == "IzendaReportingRoute")
    //                    {
    //                        string url = route.Url;
    //                        if (!url.ToLower().Contains("{controller}"))
    //                        {
    //                            reportingPath = string.Format("{0}{1}", "/", route.Defaults["controller"]) + "/";
    //                        }
    //                        else
    //                        {
    //                            url = "/" + url.Substring(0, url.ToLower().IndexOf("{controller}"));
    //                            url += route.Defaults["controller"];
    //                            reportingPath = url + "/";
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch
    //        {
    //            reportingPath = null;
    //        }
    //        if (String.IsNullOrEmpty(reportingPath))
    //            reportingPath = "/";
    //        AdHocSettings.LicenseKey = ConfigurationManager.AppSettings["IzendaLicKey"];
    //        AdHocSettings.SqlServerConnectionString = ConfigurationManager.AppSettings["MsConnectionString"];
    //        AdHocSettings.AdHocConfig = new FileSystemAdHocConfig();
    //        string absUri = HttpContext.Current.Request.Url.AbsoluteUri;
    //        string pathAndQ = HttpContext.Current.Request.Url.PathAndQuery;
    //        string appPath = HttpContext.Current.Request.ApplicationPath;
    //        int pqStart = absUri.LastIndexOf(pathAndQ);
    //        string siteUrl = absUri.Substring(0, pqStart) + appPath;
    //        if (siteUrl.EndsWith("/"))
    //            siteUrl = siteUrl.Substring(0, siteUrl.Length - 1);
    //        string reportingUrl = siteUrl + reportingPath;
    //        AdHocSettings.ReportViewer = reportingUrl + "ReportViewer";
    //        AdHocSettings.InstantReport = reportingUrl + "InstantReport";
    //        AdHocSettings.DashboardViewer = reportingUrl + "Dashboards";
    //        AdHocSettings.ReportDesignerUrl = reportingUrl + "ReportDesigner";
    //        AdHocSettings.DashboardDesignerUrl = reportingUrl + "DashboardDesigner";
    //        AdHocSettings.ReportList = reportingUrl + "ReportList";
    //        AdHocSettings.FormDesignerUrl = reportingUrl + "FormDesigner";
    //        AdHocSettings.SettingsPageUrl = reportingUrl + "Settings";
    //        AdHocSettings.ParentSettingsUrl = reportingUrl + "Settings";
    //        AdHocSettings.ResponseServer = reportingUrl + "rs.aspx";
    //        AdHocSettings.ReportsPath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "Reporting\\Reports");
    //        AdHocSettings.ShowHelpButton = true;
    //        AdHocSettings.AllowMultilineHeaders = true;

    //        AdHocSettings.ReportViewerDefaultPreviewResults = 100000;
    //        AdHocSettings.DefaultPreviewResults = 100000;

    //        AdHocSettings.Formats["AU Date"] = "{0:dd/MM/yyyy}";
    //        AdHocSettings.Formats["AU Datetime"] = "{0:dd/MM/yyy hh:mm:ss tt}";

    //        AdHocSettings.ShowSideHelp = true;
    //        AdHocSettings.GenerateThumbnails = false;
    //        AdHocSettings.NumChartTabs = 3;
    //        AdHocSettings.TabsCssUrl = "Resources/css/tabs.css";
    //        AdHocSettings.ReportCssUrl = "Resources/css/Report.css";
    //        AdHocSettings.ShowBetweenDateCalendar = true;
    //        AdHocSettings.ShowSimpleModeViewer = true;
    //        AdHocSettings.PrintMode = PrintMode.Html2PdfAndHtml;
    //        AdHocSettings.ShowPoweredByLogo = false;
    //        HttpContext.Current.Session["ReportingInitialized"] = true;
    //    }
    //}

    public class ExtendedHandleErrorInfo : System.Web.Mvc.HandleErrorInfo
    {
        public string MessageTxt { get; set; }

        public bool NeverShowStackTrace { get; set; }

        public ExtendedHandleErrorInfo(Exception exception, string controllerName, string actionName, string messageText)
            : base(exception, controllerName, actionName)
        {
            this.MessageTxt = messageText;
            NeverShowStackTrace = false;
        }
    }

}