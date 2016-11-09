using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Help;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    public class HelpController : Controller
    {
        [ChildActionOnly]
        public ActionResult GetLink(string currentArea, string currentController, string currentAction, string currentLocale)
        {
            PEMRBACEntities rbacEntities = new PEMRBACEntities();
            HelpLinkModel model = null;

            Uri url = Request.Url;
            var baseUrl = Request.IsLocal
                          ? String.Format("{0}://{1}:{2}", url.Scheme, url.Host, url.Port)
                          : String.Format("{0}://{1}", url.Scheme, url.Host);

            var helpMap = rbacEntities.HelpMaps.FirstOrDefault(x => x.Area == currentArea &&
                x.Controller == currentController
                && x.Action == currentAction && x.CultureCode == currentLocale);
            if ( helpMap != null )
            {
                model = new HelpLinkModel()
                {
                    Server = helpMap.Server == null ? baseUrl :
                        helpMap.Server.EndsWith("/") ? helpMap.Server.Remove(helpMap.Server.Length - 1) : helpMap.Server,
                    File = helpMap.File.StartsWith("/") ? helpMap.File.Substring(1) : helpMap.File,
                    Topic = helpMap.Topic
                };
            }

            return PartialView(model);
        }
    }
}
