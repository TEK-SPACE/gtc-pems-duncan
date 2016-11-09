using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.Utilities;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Security;
using NLog;
using WebMatrix.WebData;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    public class UtilitiesController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region RBAC Bulk Update


        public ActionResult RbacBulkUpdateSet()
        {
            var model = new RightsMenusGroupsAndAppsModel();

            var xmlFile = Session["RbacBulkUpdate"] as string;

            if ( !string.IsNullOrEmpty(xmlFile)  )
            {
                var authorizationManager = new AuthorizationManager();

                try
                {
                    // Process the xml file.
                    authorizationManager.SetConfiguration(xmlFile);
                }
                catch (Exception ex)
                {
                    // Do nothing.
                }

                // Delete the file
                try
                {
                    System.IO.File.Delete(xmlFile);
                }
                catch (Exception)
                {
                    // Do nothing.
                }
                
                // Create the log and error lists.
                model.Logs.Add("File Processed: " + xmlFile);

                model.ModelIsResults = true;
                foreach (var errorText in authorizationManager.XmlProcessErrors)
                {
                    model.Errors.Add(errorText);
                }
                foreach (var logText in authorizationManager.XmlProcessLogs)
                {
                    model.Logs.Add(logText);
                }

                Session.Remove("RbacBulkUpdate");
            }
            
            return View(model);
        }

        public ActionResult RbacBulkUpdateSetSubmit(IEnumerable<HttpPostedFileBase> files)
        {
            Session.Remove("RbacBulkUpdate");

            foreach (var file in files)
            {
                // Some browsers send file names with full path. We only care about the file name.
                var fileName = Path.GetFileName(file.FileName);
                var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["rbac.menu.template.upload"]), fileName);

                file.SaveAs(destinationPath);

                Session["RbacBulkUpdate"] = destinationPath;
            }

            return RedirectToAction("RbacBulkUpdateSet");
        }

        public ActionResult RbacBulkUpdateGet()
        {
            var model = new RbacCustomerListModel()
                {
                    Customers = ( new AuthorizationManager() ).GetAuthorizedCities( WebSecurity.CurrentUserName )
                };

            return View(model);
        }

        
        public FileResult RbacBulkUpdateGetExisting(int customerId)
        {
            var authorizationManager = new AuthorizationManager(new PemsCity(customerId.ToString()));

            string file = authorizationManager.GetConfiguration(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["rbac.menu.template.upload"]));
            string fileName = Path.GetFileName( file );
            FileResult fileResult = File( file, "text/xml", fileName );

            return fileResult;
        }

        #endregion
    }
}
