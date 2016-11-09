using System.Web.Mvc;
namespace Duncan.PEMS.Web.Areas.maint.Controllers
{
    public class AccountController :  shared.Controllers.AccountController
    {
        // Post: /shared/Account/LogOff
        [ChildActionOnly]
        public ActionResult Header()
        {
            return PartialView();
        }
    }
}
