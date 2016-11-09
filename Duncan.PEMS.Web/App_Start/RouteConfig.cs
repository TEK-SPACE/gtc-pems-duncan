using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Web
{
    public class AspxRouteConstraint : IRouteConstraint
    {
        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return
                values["aspx"] != null && values["aspx"].ToString().Contains("rs.aspx"); // && !values["aspx"].ToString().Contains("Reporting/rs.aspx");
        }

        #endregion
    }

    public class BlankIFrameRouteConstraint : IRouteConstraint
    {
        #region IRouteConstraint Members

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if ( values["aspx"] != null && values["aspx"].ToString().Contains( "rs.aspx" ) )
            {
                if ( httpContext.Request.QueryString["p"] != null && httpContext.Request.QueryString["p"].ToString().Contains( "BLANK" ) )
                {
                    return true;
                }
                return false;

            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                name: Constants.Routing.AdminRouteName,
                url: "{city}/pems/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Duncan.PEMS.Web.Areas.admin.Controllers" },
                constraints: new {city="Admin"}
            );



            routes.MapRoute(
                name: Constants.Routing.CityRouteName,
                url: "{city}/pems/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Duncan.PEMS.Web.Areas.city.Controllers" }
            );

            routes.MapRoute(
                name: Constants.Routing.DefaultRouteName,
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "Duncan.PEMS.Web.Controllers" }
            );


            //map for error page
            routes.MapRoute(
                name: Constants.Routing.ErrorRouteName,
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Error" },
                namespaces: new[] { "Duncan.PEMS.Web.Controllers" }
            );

            routes.MapRoute(Constants.Routing.LandingRouteName, "Home/Landing");
        }
    }

    public class SpecificFileRouterConstraint : IRouteConstraint
    {
        private string extensionToBeRouted = null;
        private string fileToBeRouted = null;

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return !String.IsNullOrEmpty(extensionToBeRouted) && !String.IsNullOrEmpty(fileToBeRouted) && values[extensionToBeRouted] != null && values[extensionToBeRouted].ToString().ToLower().Contains(fileToBeRouted);
        }

        public SpecificFileRouterConstraint() { }

        public SpecificFileRouterConstraint(string extension, string fileName)
        {
            extensionToBeRouted = extension.ToLower();
            fileToBeRouted = fileName.ToLower();
        }
    }

    public class IzendaResourceConstraint : IRouteConstraint
    {
        private string extensionToBeRouted = null;

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (String.IsNullOrEmpty(extensionToBeRouted) || values[extensionToBeRouted] == null)
                return false;
            string requestUrl = values[extensionToBeRouted].ToString().ToLower();
            bool result = false;
            if (extensionToBeRouted == "js")
            {
                result = result || requestUrl.Contains("reportviewerfilters.js");
                result = result || requestUrl.Contains("data-sources.js");
                result = result || requestUrl.Contains("data-sources-preview.js");
                result = result || requestUrl.Contains("charts.js");
                result = result || requestUrl.Contains("datasources-search.js");
                result = result || requestUrl.Contains("jquery-1.6.1.min.js");
                result = result || requestUrl.Contains("jquery-ui-1.8.13.custom.min.js");
                result = result || requestUrl.Contains("elrte.full.js");
                result = result || requestUrl.Contains("elrte.ru.js");
            }
            if (extensionToBeRouted == "css")
            {
                result = result || requestUrl.Contains("tabs.css");
                result = result || requestUrl.Contains("report.css");
                result = result || requestUrl.Contains("filters.css");
                result = result || requestUrl.Contains("base.css");
                result = result || requestUrl.Contains("report-list-modal.css");
                result = result || requestUrl.Contains("data-sources.css");
                result = result || requestUrl.Contains("charts.css");
                result = result || requestUrl.Contains("jquery-ui-1.8.13.custom.css");
                result = result || requestUrl.Contains("elrte.min.css");
                result = result || requestUrl.Contains("elrte-inner.css");
            }
            if (extensionToBeRouted == "png")
            {
                result = result || requestUrl.Contains("elrtebg.png");
                result = result || requestUrl.Contains("elrte-toolbar.png");
            }
            if (extensionToBeRouted == "gif")
            {
                result = result || requestUrl.Contains("elrte/images/pixel.gif");
            }
            return result;
        }

        public IzendaResourceConstraint() { }

        public IzendaResourceConstraint(string extension)
        {
            extensionToBeRouted = extension.ToLower();
        }
    }


    //public class IzendaRouteConfig
    //{
    //    public static void RegisterRoutes(RouteCollection routes)
    //    {
    //        routes.MapRoute("IzendaJsResources", "{*js}", new { controller = "IzendaStaticResources", action = "Index" }, new { irc = new IzendaResourceConstraint("js") });
    //        routes.MapRoute("IzendaCssResources", "{*css}", new { controller = "IzendaStaticResources", action = "Index" }, new { irc = new IzendaResourceConstraint("css") });
    //        routes.MapRoute("IzendaPngResources", "{*png}", new { controller = "IzendaStaticResources", action = "Index" }, new { irc = new IzendaResourceConstraint("png") });
    //        routes.MapRoute("IzendaGifResources", "{*gif}", new { controller = "IzendaStaticResources", action = "Index" }, new { irc = new IzendaResourceConstraint("gif") });
    //        //var reportingRoute = routes.MapRoute("IzendaReportingRoute", "{controller}/{action}/{id}", new { controller = "Reporting", action = "ReportList", id = UrlParameter.Optional });
    //        //reportingRoute.DataTokens.Add("RouteName", "IzendaReportingRoute");
    //    }
    //}


}