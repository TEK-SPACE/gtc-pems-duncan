using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Duncan.PEMS.Framework.Controller;

namespace Duncan.PEMS.Framework.ViewEngine
{
    class ViewEngineUtility
    {
        /// <summary>
        /// Determine if controller is a Pems controller.  Does it have an associated city?
        /// </summary>
        /// <param name="controllerContext">An instance of <see cref="ControllerContext"/> pointing to active controller.</param>
        /// <returns>True if this controller has a related city.</returns>
        static public bool IsPemsController(ControllerContext controllerContext)
        {
            return controllerContext.RouteData.Values.ContainsKey( "city" );
        }

        /// <summary>
        /// Fix up the path to the view file resource.
        /// </summary>
        /// <param name="controllerContext">An instance of <see cref="ControllerContext"/> pointing to active controller.</param>
        /// <param name="path">The path to fix.</param>
        /// <returns>A path pointing to the correct view resource.</returns>
        static public string AreaPathFix(ControllerContext controllerContext, string path)
        {
            // Is this controller directly inherited from PemsController?
            string baseClassName = controllerContext.Controller.GetType().BaseType.Name;

            if (baseClassName.Equals(typeof(PemsController).Name) || baseClassName.Equals(typeof(BaseController).Name))
            {
                string city = controllerContext.RouteData.Values["city"] as string;
                return
                    city == null ? path :
                    (city.Equals("admin", StringComparison.CurrentCultureIgnoreCase) ?
                    path.Replace("~/Views", "~/Areas/admin/Views") :
                    path.Replace("~/Views", "~/Areas/city/Views"));
            }
            else
            {
                string controllerName = baseClassName.Replace( "Controller", "" );

                if (controllerName.Length > 0 && path.StartsWith("~/Views"))
                {
                    // Point to shared view.
                    string[] tokens = path.Split( '/' );
                    if ( tokens.Length > 2 )
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append( "~/Areas/shared/Views/" ).Append( controllerName )
                          .Append( '/' ).Append( tokens[tokens.Length - 1] );
                        return sb.ToString();
                    }
                }

            }

            // Controller context was of type Controller.
            return path;
        }
    
    }
}
