using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    public class VersionsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Asset Version Maintenance

        public ActionResult AssetVersion()
        {
            AssetVersionViewModel model = (new VersionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetVersionViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AssetVersion(AssetVersionViewModel model, FormCollection formCollection)
        {
            //if (submitButton.Equals("RETURN"))
            //    return RedirectToAction("Index");

            //// Save values
            //if (!ModelState.IsValid)
            //    return View((new AssetFactory()).GetAssetVersionFilterModel((new UserFactory()).GetUserId(User.Identity.Name)));
            //// Saved and stay on same page.
            return
                View((new VersionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetVersionViewModel(model));
        }

        [HttpPost]
        public ActionResult GetVersionList(int fileType, int meterGroup, int versionGroup)
        {
            // Get version list for fileType, meterGroup, versionGroup
            var versionList = (new VersionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetVersionSetModel(fileType, meterGroup, versionGroup);

            var result = new DataSourceResult
            {
                Data = versionList.Versions,
                Total = versionList.Versions.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddVersion([DataSourceRequest] DataSourceRequest request,
                 [Bind(Prefix = "models")]IEnumerable<AssetVersionModel> versions, int fileType, int meterGroup, int versionGroup)
        {
            var versionFactory = new VersionFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());


            // Get new version
            var newVersion = versions.FirstOrDefault( m => m.Id == 0 );

            if ( newVersion != null )
            {
                // Does this version already exist?
                if ( versionFactory.VersionExists( newVersion.Name, fileType, meterGroup, versionGroup ) )
                {
                    // Return an error to caller.
                    ModelState.AddModelError("Version", "This version already exists.");
                }
                else
                {
                    // Save new version.
                    versionFactory.AddVersion( newVersion.Name, fileType, meterGroup, versionGroup );
                }
            }

            if ( ModelState.IsValid )
            {
                var versionList = versionFactory.GetVersionSetModel(fileType, meterGroup, versionGroup);

                var result = new DataSourceResult
                {
                    Data = versionList.Versions,
                    Total = versionList.Versions.Count
                };

                // Return new list.
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            // Return the error
            return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}
