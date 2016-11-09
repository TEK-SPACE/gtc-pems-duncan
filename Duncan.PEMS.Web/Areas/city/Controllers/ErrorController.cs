using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /Error/
        public ActionResult Index()
        {
            /*return View();*/
            // Use a fully qualified view name because the route data might not be set
            return View("~/Views/Error/index.aspx");
        }

        // GET: /Error/NotFound
        public ActionResult NotFound()
        {
            return View();
        }

        // GET: /Error/AccessDenied
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
