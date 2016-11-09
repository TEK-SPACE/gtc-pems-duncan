using System.Web.Mvc;

namespace Duncan.PEMS.Web.Areas.admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "admin_default",
                url: "{city}/pems/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Duncan.PEMS.Web.Areas.admin.Controllers" },
                constraints: new { city = "Admin" }
            );
        }
    }
}
