using System.Web.Mvc;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Framework.Controller;

namespace Duncan.PEMS.Web.Areas.maint.Controllers
{
    public class HomeController : PemsController
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            var userFactory = new UserFactory();
          if (userFactory.RequiresPasswordReset(User.Identity.Name))
             return RedirectToAction("ChangePassword", "Profile");

            ViewBag.PWExpirationDays = userFactory.GetPasswordExpirationInDays();
            // This causes the id of the master page <body> tag to be set to 'client-inquiry'
            return View();
        }
    }
}
