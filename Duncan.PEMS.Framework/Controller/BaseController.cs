using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Business.MaintenanceGroups;
using Duncan.PEMS.Entities.Grids;
using Duncan.PEMS.Entities.Sessions;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Security;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;
using NPOI.SS.UserModel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Duncan.PEMS.Framework.Controller
{
    public abstract class BaseController : System.Web.Mvc.Controller
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region "Routing Helpers"

        private ActionResult SendToRoute(RouteValueDictionary values)
        {
            if (values.ContainsKey("city"))
                return SendToCityRoute(values);

            return RedirectToRoute( Constants.Routing.DefaultRouteName, values );
        }
        
        private ActionResult SendToErrorRoute(RouteValueDictionary values)
        {
            if (values.ContainsKey("city"))
                return SendToCityRoute(values);

            return RedirectToRoute(Constants.Routing.ErrorRouteName, values);
        }

        private ActionResult SendToCityRoute(RouteValueDictionary values)
        {
            //get the city name
            if (values.ContainsKey("city"))
            {
                var userCity = values["city"].ToString();

                //if its admin, then just send them to the admin route
                if (userCity.Equals("admin", StringComparison.CurrentCultureIgnoreCase))
                    return RedirectToRoute(Constants.Routing.AdminRouteName, values);

                //if it isnt admin, then we need to check to see if this city is a maintenance group. 
                //if it is NOT (they are logging in to the customer, not the maintenance group, then we need to check to see if this user is a technician and either send them to the maint route or the city route
                if (!MaintenanceGroupFactory.IsCustomerMaintenanceGroup(userCity))
                {
                    ////its not a maint group, it can be a maint customer, check here to see if we need to send them to the admin or maint version of hte site
                    var cityCookie = GetCityCookie();
                    if (cityCookie != null)
                    {
                        //now lets check to see if they are logging in for maintenance or customer
                        if (cityCookie.Value.Split('|')[2] == CustomerLoginType.MaintenanceGroupCustomer.ToString())
                            return RedirectToRoute(Constants.Routing.MaintRouteName, values);
                    }
                }
            }

            //if it is a maintenance group, just send the user to the city homepage
            return RedirectToRoute(Constants.Routing.CityRouteName, values);
        }

        protected ActionResult SendToCityHomePage(string city)
        {
            var values = new RouteValueDictionary
                {
                    {"city", city},
                    {"controller", "Home"},
                    {"action", "Index"}
                };
            return SendToRoute(values);
        }

        protected ActionResult SendToErrorPage(string errorCode, string errorMessage)
        {
            var values = new RouteValueDictionary
                             {
                                 {"controller", "Home"},
                                 {"action", "Error"}
                             };

            ViewData["__errorCode"] = errorCode;
            ViewData["__errorMessage"] = errorMessage;

            return SendToErrorRoute(values);

        }


        protected ActionResult SendToDefaultHomePage()
        {
            var values = new RouteValueDictionary
                             {
                                 {"controller", "Home"},
                                 {"action", "Index"}
                             };

            return SendToRoute( values );
        }

        protected ActionResult SendToLandingPage()
        {
            return RedirectToRoute(Constants.Routing.LandingRouteName);
        }

        protected ActionResult SendToLoginPage()
        {
            var values = new RouteValueDictionary
                             {
                                 {"controller", "Account"},
                                 {"action", "LogIn"}
                             };

            return SendToRoute( values );
        }

        #endregion

        #region "Cookies"

        /// <summary>
        /// Sets an encrypted cookie for the user.  Cookie contains user name, user type and city/customer
        /// </summary>
        /// <param name="value"></param>
        protected void SetCityCookie(string value)
        {
            //cookie will be username|cityinternalname|loginType so "Bob|Admin|Customer" or "Bob|NorthSydneyCouncil|Maintenance"
            //remove the cookie if it already exists
            DeleteCityCookie();

            //now add the new one
            var cookie = new HttpCookie( Constants.Security.UserCityCookieName, value );
            //var strCookieExpMinutes = ConfigurationManager.AppSettings[Constants.Security.CityCookieExpirationAppSettingName];
            //int cookieExpInMinutes;
            //int.TryParse(strCookieExpMinutes, out cookieExpInMinutes);
            ////default it to an hour if this is not set correctly
            //if (cookieExpInMinutes < 1)
            //    cookieExpInMinutes = 60;
            HttpCookie encodedCookie = CookieManager.Encode( cookie );
           // encodedCookie.Expires = DateTime.Now.AddMinutes(cookieExpInMinutes);
            Response.Cookies.Add(encodedCookie);
        }

        /// <summary>
        /// Deletes the users city cookie. 
        /// </summary>
        public void DeleteCityCookie()
        {
            //check to see if the user is logged in, and if they are, set it to name|none|Customer, otherwise expire the cookie
            if (User.Identity.IsAuthenticated)
            {
                //default it to the customer login
                var cookie = new HttpCookie(Constants.Security.UserCityCookieName,
                                            User.Identity.Name + "|None|" + CustomerLoginType.Unknown);
                HttpCookie encodedCookie = CookieManager.Encode(cookie);
                Response.Cookies.Set(encodedCookie);

            }
            else
            {
                var cookie = new HttpCookie(Constants.Security.UserCityCookieName)
                    {
                        Expires = DateTime.Now.AddDays(-1) // or any other time in the past
                    };
                Response.Cookies.Set(cookie);
            }
        }

        /// <summary>
        /// Gets and decrypts a cookie for the users city
        /// </summary>
        /// <returns></returns>
        protected HttpCookie GetCityCookie()
        {
            HttpCookie cookie = Request.Cookies.Get(Constants.Security.UserCityCookieName);
            if ( cookie != null )
            {
                var decodedCookie = CookieManager.Decode( cookie );
                return decodedCookie;
            }

            return null;
        }

        #endregion

        #region "RedirectToLogin"

        protected ActionResult ReturnLoginRedirectView(string message, string pageTitle)
        {
            var lrModel = new LoginRedirectModel() {Message = message, SubTitle = pageTitle};
            return View( "ReturnToLogin", lrModel );
        }

        #endregion

        #region "User Management"

        protected void Logout()
        {
            SecurityManager.Logout();
            DeleteCityCookie();
        }

        #endregion

        #region "Session Management"

        public void SetSavedSortValues(IList<SortDescriptor> sorts, string indexType)
        {
            var sortValues = new List<SortValue>();
            foreach (SortDescriptor sort in sorts)
            {
                var asldjf = sort.Serialize();
                var sortValue = new SortValue
                                    {
                                        field = sort.Member,
                                        dir = sort.SortDirection == ListSortDirection.Ascending ? "asc" : "desc"
                                    };
                sortValues.Add( sortValue );
            }
            Session["__SavedSortValues" + indexType] = sortValues;
        }

        public JsonResult GetSavedSortValues(string indexType = "") 
        {
            if (Session["__SavedSortValues" + indexType] != null)
            {
                var retVal = Json(Session["__SavedSortValues" + indexType] as List<SortValue>);
                //clear the session
                Session.Remove("__SavedSortValues" + indexType);
                return retVal;
            }
            return Json(string.Empty);
        }

        #endregion

        #region "Filtering"

        /// <summary>
        /// Recursivly add all the sub filters to get a flat list of filters for the export functionality
        /// </summary>
        /// <param name="filterDescriptors"></param>
        /// <param name="filters"></param>
        public List<FilterDescriptor> GetFilterDescriptors(IList<IFilterDescriptor> filterDescriptors, List<FilterDescriptor> filters)
        {
            foreach (IFilterDescriptor filter in filterDescriptors)
            {
                if ( filter is CompositeFilterDescriptor )
                    filters = GetFilterDescriptors( ( (CompositeFilterDescriptor) filter ).FilterDescriptors, filters );
                else
                    filters.Add( (FilterDescriptor) filter );
            }
            return filters;
        }

        #endregion

        #region "Grid Overrides"

        public JsonResult GetGridData(int cid, string cont, string act)
        {
            List<GridData> items = ( new GridFactory() ).GetGridData( cont, act, cid );
            if ( items != null )
                return Json( items );
            return Json(string.Empty);
        }

        #endregion

        #region "Exporting"

        protected static void AddCsvValue(StreamWriter writer, string value, bool lastCellInRow = false)
        {
            //last one doesnt get a comma at the end!
            string formatted = String.Format( "{0}{1}{2}{3}",
                                              Constants.Export.EscapeChar,
                                              value,
                                              Constants.Export.EscapeChar,
                                              lastCellInRow ? "" : Constants.Export.DelimiterChar );

            writer.Write( formatted );

            if ( lastCellInRow )
            {
                writer.WriteLine();
            }
        }

        protected static string GetLocalizedGridTitle(IEnumerable<GridData> gridData, string originalTitle)
        {
            var firstOrDefault = gridData.FirstOrDefault( x => x.OriginalTitle == originalTitle );
            if ( firstOrDefault != null )
                return firstOrDefault.Title;
            return originalTitle;
        }
        
        protected int AddFiltersToExcelSheet(ISheet sheet, List<FilterDescriptor> customFilters)
        {
            int rowNumber = 0;
            //filters can be filter descriptor, or composit filter descriptor, so we need to recursively generate this
            if (customFilters.Any())
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;

                //Create a header row
                var filterRow = sheet.CreateRow(rowNumber++);

                //Set the column names in the header row
                filterRow.CreateCell(0).SetCellValue("Filter Name");
                filterRow.CreateCell(1).SetCellValue("Filter Value");
                foreach (var item in customFilters)
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        int columnNumber = 0;
                        var row = sheet.CreateRow(rowNumber++);
                        var friendlyName = HelperMethods.GetFriendlyLocalizedName(item.Member, textInfo);
                        row.CreateCell(columnNumber++).SetCellValue(friendlyName);
                        row.CreateCell(columnNumber++).SetCellValue(item.Value.ToString());
                    }
                }

                //if we added filters, lets add a blank row for readability
                var blankRow = sheet.CreateRow(rowNumber++);
                blankRow.CreateCell(0).SetCellValue(" ");
            }
            return rowNumber;
        }
        
        protected int AddFiltersToExcelSheet(DataSourceRequest request, ISheet sheet, List<FilterDescriptor> additionalFilters = null)
        {
            int rowNumber = 0;
            //filters can be filter descriptor, or composit filter descriptor, so we need to recursively generate this
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors( request.Filters, filters );
            //add any additional filters passed in as well
            if (additionalFilters != null)
                filters.AddRange(additionalFilters);
            if ( filters.Any() )
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                //Create a header row
                var filterRow = sheet.CreateRow( rowNumber++ );

                //Set the column names in the header row
                filterRow.CreateCell( 0 ).SetCellValue( "Filter Name" );
                filterRow.CreateCell( 1 ).SetCellValue( "Filter Value" );
                foreach (var item in filters)
                {
                    if ( !string.IsNullOrEmpty( item.Value.ToString() ) )
                    {
                        int columnNumber = 0;
                        var row = sheet.CreateRow( rowNumber++ );
                        var friendlyName = HelperMethods.GetFriendlyLocalizedName(item.Member, textInfo);
                        row.CreateCell(columnNumber++).SetCellValue(friendlyName);
                        row.CreateCell( columnNumber++ ).SetCellValue( item.Value.ToString() );
                    }
                }

                //if we added filters, lets add a blank row for readability
                var blankRow = sheet.CreateRow( rowNumber++ );
                blankRow.CreateCell( 0 ).SetCellValue( " " );
            }
            return rowNumber;
        }

        protected static void AddHeaderToPdf(Document document, PdfWriter writer, string headerText)
        {
            Rectangle rectangle = document.PageSize;
            PdfPTable header = new PdfPTable( 1 )
                                   {
                                       TotalWidth = rectangle.Width
                                   };
            Phrase phrase = new Phrase( headerText );
            PdfPCell cell = new PdfPCell( phrase )
                                {
                                    Border = Rectangle.NO_BORDER,
                                    VerticalAlignment = Element.ALIGN_TOP,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                };
            header.AddCell( cell );
            header.WriteSelectedRows(
                // first/last row; -1 writes all rows
                0,
                -1,
                // left offset
                0,
                // ** bottom** yPos of the table
                rectangle.Height - document.TopMargin + header.TotalHeight + 20,
                writer.DirectContent
                );

            document.Add( header );
        }

        protected Document AddFiltersToPdf( Document document, List<FilterDescriptor> customFilters)
        {
            var filterTable = new PdfPTable(2);
            filterTable.DefaultCell.Padding = 3;
            filterTable.DefaultCell.BorderWidth = 2;
            filterTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            filterTable.AddCell("Filter Name");
            filterTable.AddCell("Filter Value");
            filterTable.CompleteRow();
            filterTable.HeaderRows = 1;
            filterTable.DefaultCell.BorderWidth = 1;

            if (customFilters.Any())
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                foreach (var item in customFilters)
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        //get the friendly name for the filter (Asset Status instead of assetStatus)
                        var friendlyName = HelperMethods.GetFriendlyLocalizedName(item.Member, textInfo);
                        filterTable.AddCell(friendlyName);
                        filterTable.AddCell(item.Value.ToString());
                    }
                }
                // Add table to the document
                document.Add(new Paragraph("Filters:  "));
                document.Add(filterTable);
                document.Add(new Paragraph(" "));
            }
            return document;
        }

        protected Document AddFiltersToPdf(DataSourceRequest request, Document document, List<FilterDescriptor> additionalFilters = null)
        {
            var filterTable = new PdfPTable( 2 );
            filterTable.DefaultCell.Padding = 3;
            filterTable.DefaultCell.BorderWidth = 2;
            filterTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Adding headers
            filterTable.AddCell( "Filter Name" );
            filterTable.AddCell( "Filter Value" );
            filterTable.CompleteRow();
            filterTable.HeaderRows = 1;
            filterTable.DefaultCell.BorderWidth = 1;

            var filters = new List<FilterDescriptor>();
            // filters can be filter descriptor, or composite filter descriptor, so we need to recursively generate this
            // pass in an empty list
            filters = GetFilterDescriptors( request.Filters, filters );

            //add any additional filters passed in as well
            if ( additionalFilters != null )
                filters.AddRange( additionalFilters );

            if ( filters.Any() )
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                foreach (var item in filters)
                {
                    if ( !string.IsNullOrEmpty( item.Value.ToString() ) )
                    {
                        //get the friendly name for the filter (Asset Status instead of assetStatus)
                        var friendlyName = HelperMethods.GetFriendlyLocalizedName( item.Member, textInfo );
                        filterTable.AddCell( friendlyName );
                        filterTable.AddCell( item.Value.ToString() );
                    }
                }
                // Add table to the document
                document.Add( new Paragraph( "Filters:  " ) );
                document.Add( filterTable );
                document.Add( new Paragraph( " " ) );
            }
            return document;
        }


        #endregion

        #region Error Handling

        protected override void OnException(ExceptionContext context)
        {
            /*
            ErrorItem error = new ErrorItem();
            string errorMessage = string.Empty;
            if (context.Exception.Message != null)
            {
                errorMessage = context.Exception.Message;
            }
            if (context.Exception.InnerException != null)
            {
                errorMessage += "INNER EXCEPTION: " + context.Exception.InnerException;
            }
            System.Web.HttpContext ctx = System.Web.HttpContext.Current;
            ctx.Response.Clear();
            RequestContext rc = ((MvcHandler)ctx.CurrentHandler).RequestContext;
            //Get the name of the controller and action that caused the error
            string controllerName = rc.RouteData.GetRequiredString("controller");
            string actionName = rc.RouteData.GetRequiredString("action");
            
            //Set the error code to <CONTROLLER>.<ACTION>
            error.ErrorCode = controllerName + "." + actionName;
            error.ErrorMessage = errorMessage;

            SendToErrorPage(error.ErrorCode, error.ErrorMessage);
            Server.ClearError();
            ctx.Response.End();
            */

        }


        #endregion
    }
}