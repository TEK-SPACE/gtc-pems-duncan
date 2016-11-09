using System.Web.Mvc;

namespace Duncan.PEMS.Web.Areas.shared
{
    public class SharedAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "shared";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "shared_default",
                "shared/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
