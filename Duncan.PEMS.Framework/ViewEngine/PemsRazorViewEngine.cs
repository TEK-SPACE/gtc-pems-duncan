using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Duncan.PEMS.Framework.Controller;

namespace Duncan.PEMS.Framework.ViewEngine
{
    public class PemsRazorViewEngine : RazorViewEngine
    {
        public PemsRazorViewEngine()
            : base()
        {

        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            if (ViewEngineUtility.IsPemsController(controllerContext))
            {
                return base.CreatePartialView(controllerContext, 
                    ViewEngineUtility.AreaPathFix(controllerContext, partialPath));
            }
            else
            {
                return base.CreatePartialView(controllerContext, partialPath);
            }
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (ViewEngineUtility.IsPemsController(controllerContext))
            {
                return base.CreateView( controllerContext,
                    ViewEngineUtility.AreaPathFix( controllerContext, viewPath ),
                    ViewEngineUtility.AreaPathFix( controllerContext, masterPath ) );
            }
            else
            {
                return base.CreateView(controllerContext, viewPath, masterPath);
            }
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            if (ViewEngineUtility.IsPemsController(controllerContext))
            {
                return base.FileExists( controllerContext,
                    ViewEngineUtility.AreaPathFix( controllerContext, virtualPath ) );
            }
            else
            {
                return base.FileExists(controllerContext, virtualPath);
            }

        }

    }

}