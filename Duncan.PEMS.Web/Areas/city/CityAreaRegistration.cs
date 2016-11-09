using System.Web.Mvc;

namespace Duncan.PEMS.Web.Areas.city
{
    public class CityAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "city";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "city_default",
                url: "{city}/pems/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Duncan.PEMS.Web.Areas.city.Controllers" }
            );
        }
    }
}
