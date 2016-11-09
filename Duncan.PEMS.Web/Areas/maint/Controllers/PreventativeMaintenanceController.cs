using System.Web.Mvc;
using Duncan.PEMS.Framework.Controller;

namespace Duncan.PEMS.Web.Areas.maint.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class PreventativeMaintenanceController : PemsController
    {
        /// <summary>
        /// List of the preventative maintenance assets in the system (for this customer)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}
