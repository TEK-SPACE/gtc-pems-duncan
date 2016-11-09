using System.Web.Mvc;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using NLog;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    public class AccountController : BaseController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Logoff

        // Post: /shared/Account/LogOff
        [ChildActionOnly]
        public ActionResult LogOut()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult MobileLogOut()
        {
            return PartialView();
        }

        public ActionResult LogOff()
        {
            Logout();
            return SendToLoginPage();
        }

        #endregion

        #region "Change City"

        // Post: /shared/Account/ChangeCity
        [ChildActionOnly]
        [OutputCache(Duration = 900, VaryByParam = "userName")]
        public ActionResult ChangeCity(string userName)
        {
            //get a list of cities for this user
            var secMgr = new SecurityManager();
            //            var userCities = (new SecurityManager()).GetCitiesForUser((new UserFactory()).GetCurrentUsername());
            var userCities = (new SecurityManager()).GetCitiesForUser(userName);

            ViewBag.ShowChangeCityLink = false;

            //if they have more than one, display the change city link.
            if (userCities.Count > 1)
                ViewBag.ShowChangeCityLink = true;
            return PartialView();
        }

        // Post: /shared/Account/ChangeCity
        [OutputCache(Duration = 900, VaryByParam = "userName")]
        public ActionResult MobileChangeCity(string userName)
        {
            var userCities = (new SecurityManager()).GetCitiesForUser(userName);
            ViewBag.ShowChangeCityLink = false;

            //if they have more than one, display the change city link.
            if (userCities.Count > 1)
                ViewBag.ShowChangeCityLink = true;
            return PartialView();
        }


        public ActionResult ChooseCity()
        {
            DeleteCityCookie();
            return SendToLandingPage();
        }

        public ActionResult RedirectToHomepage()
        {
            return RedirectToAction("Index", "Home");
        }



        #endregion
    }
}