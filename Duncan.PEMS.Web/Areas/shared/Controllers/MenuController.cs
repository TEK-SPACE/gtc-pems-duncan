using System;
using System.Runtime.InteropServices.ComTypes;
using System.Web.Mvc;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Security.Menu;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    public class MenuController : Controller
    {
        [ChildActionOnly]
        //[OutputCache(Duration = 600, VaryByParam = "currentCity;currentUserName")]
        public ActionResult Index(string currentCity, string currentUserName)
        {
            //first, try to get the menu from cache 
            var existingMenu = GetMenuFromCache(currentCity, currentUserName);
            if (existingMenu != null)
                return PartialView(existingMenu);

            //if we get this far, we need to go get the menu, store it
            Uri url = Request.Url;
            var baseUrl = "";
            baseUrl = Request.IsLocal
                          ? String.Format("{0}://{1}:{2}/{3}/pems/", url.Scheme, url.Host, url.Port, RouteData.Values["city"])
                          : String.Format("{0}://{1}/{2}/pems/", url.Scheme, url.Host, RouteData.Values["city"]);
            string menuUrl = String.Format("/{0}/pems/", RouteData.Values["city"]);
            var displayFullMenu = DisplayFullMenu(GetCurrentCustomerId());
            var menu = new PemsMenu(RouteData.Values["city"].ToString(), User.Identity.Name, baseUrl, string.Empty, displayFullMenu);
            SaveMenuToCache(menu, currentCity, currentUserName);

            return PartialView(menu);
        }

        private int GetCurrentCustomerId()
        {
            var idString = Session[Constants.ViewData.CurrentCityId].ToString();
            int cityId;
            int.TryParse(idString, out cityId);
            return cityId;
        }

        private bool DisplayFullMenu(int customerId)
        {
            try
            {
                return bool.Parse(new SettingsFactory().GetValue("DisplayFullMenu", customerId));
            }
            catch
            { return false; }
        }

        private void SaveMenuToCache(PemsMenu item, string currentCity, string currentUserName)
        {
            HttpContext.Cache.Insert("__PemsMenu" + currentCity.Trim() + currentUserName.Trim(), item, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0));
        }

        private PemsMenu GetMenuFromCache(string currentCity, string currentUserName)
        {
            var retVal = HttpContext.Cache.Get("__PemsMenu" + currentCity.Trim() + currentUserName.Trim()) as PemsMenu;
            return retVal;
        }
    }
}
