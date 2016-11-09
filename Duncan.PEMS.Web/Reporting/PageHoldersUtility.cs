using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Izenda.AdHoc;

namespace MVC.Reporting
{
    public class PageHolders : System.Web.Mvc.ViewMasterPage
    {
        public string Header { get; set; }
        public string Content { get; set; }
    }

    public class PageHoldersUtility
    {
        public static PageHolders GetPageHoldersFromPageName(string pageName)
        {
            return GetPageHoldersFromFiles(HttpContext.Current.Server.MapPath(string.Format("~/Reporting/Resources/html/{0}-Head.ascx", pageName))
                                         , HttpContext.Current.Server.MapPath(string.Format("~/Reporting/Resources/html/{0}-Body.ascx", pageName)));
        }

        public static PageHolders GetPageHoldersFromFiles(string headFile, string bodyFile)
        {
            PageHolders ph = new PageHolders();
            try
            {
                // Read files content
                string headData = File.ReadAllText(headFile);
                string bodyData = File.ReadAllText(bodyFile);
                // Remove ASCX WebForms headers
                headData = headData.Substring(headData.IndexOf(Environment.NewLine) + 2);
                bodyData = bodyData.Substring(bodyData.IndexOf(Environment.NewLine) + 2);
                // Fix relative URLs
                headData = ClearContentPaths(headData);
                bodyData = ClearContentPaths(bodyData);

                ph.Header = headData;
                ph.Content = bodyData;
            }
            catch { }
            return ph;
        }

        private static string ClearContentPaths(string content)
        {
            string basePath = GetBaseUrl();
            return content.Replace("./rs.aspx", AdHocSettings.ResponseServer)
                          .Replace("\"rs.aspx", "\"" + AdHocSettings.ResponseServer)
                          .Replace("'rs.aspx", "'" + AdHocSettings.ResponseServer)
                          .Replace("Dashboards.aspx", basePath + "Reporting/Dashboards")
                          .Replace("DashboardDesigner.aspx", AdHocSettings.DashboardDesignerUrl)
                          .Replace("DataSources.aspx", basePath + "Reporting/DataSources")
                          .Replace("InstantReport.aspx", basePath + "Reporting/InstantReport")
                          .Replace("ReportList.aspx", AdHocSettings.ReportList)
                          .Replace("ReportViewer.aspx", AdHocSettings.ReportViewer)
                          .Replace("\"img/", "\"" + basePath + "Resources/img/")
                          .Replace("'img/", "'" + basePath + "Resources/img/")
                          .Replace("\"Resources/", "\"" + basePath + "Resources/");
        }

        private static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;
            var baseUrl = string.Format("{0}://{1}{2}/", request.Url.Scheme, request.Url.Authority, appUrl);
            return baseUrl;
        }
    }
}
