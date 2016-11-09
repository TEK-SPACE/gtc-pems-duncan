using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Business.News;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Entities.News;
using Duncan.PEMS.Framework.Controller;
using NLog;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
     [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class HomeController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        //
        // GET: /admin/Home/
        public ActionResult Index()
        {
            _logger.Trace("Enter");

            var userFactory = new UserFactory();
            if ( userFactory.RequiresPasswordReset( User.Identity.Name ) )
            {
                _logger.Trace("Exit");
                return RedirectToAction("Edit", "Profile");
            }

            ViewBag.PWExpirationDays = userFactory.GetPasswordExpirationInDays();
            // This causes the id of the master page <body> tag to be set to 'client-inquiry'
            ViewData["ShowLandingPage"] = "true";
            var newsMgr = new NewsManager();
            List<NewsItem> newsItems = newsMgr.GetNewsItems(CurrentCity.Id, CurrentCity.Locale.ToString(), CurrentCity.LocalTime);

            _logger.Trace("Exit");
            return View(newsItems);
        }

    }
}
