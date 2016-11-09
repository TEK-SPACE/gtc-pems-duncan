using System.Web.Mvc;
using Duncan.PEMS.Utilities;

namespace Duncan.PEMS.Web.Areas.maint
{
    public class MaintAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "maint";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               name: Constants.Routing.MaintRouteName,
               url: "{city}/maint/{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "Duncan.PEMS.Web.Areas.maint.Controllers" }
           );
        }
    }
}
