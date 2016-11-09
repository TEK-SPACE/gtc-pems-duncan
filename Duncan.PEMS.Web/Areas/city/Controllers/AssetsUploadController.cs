using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Framework.Controller;
using NLog;
using Duncan.PEMS.Entities.Customers;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class AssetsUploadController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const string SessionFilePath = "PemsAssetsBulkUpdate";
        private const string SessionFileName = "PemsAssetsBulkUpdateFile";
        private const string SessionFileType = "PemsAssetsBulkUpdateType";

        #region Upload page handlers

        /// <summary>
        /// Asset upload file select page.
        /// </summary>
        /// <param name="customerId">The integer id of the customer</param>
        /// <returns><see cref="ActionResult"/> with a <see cref="AssetTypesModel"/> model.</returns>
        public ActionResult Upload(int customerId)
        {
            var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
            return View((new AssetFactory(connStringName)).GetAssetTypesModel(customerId));
        }

        /// <summary>
        /// Handles the submission of the asset upload file.
        /// </summary>
        /// <param name="files">List of files uploaded.  Only care about first file.</param>
        /// <param name="model"><see cref="AssetTypesModel"/> instance so know asset type selected</param>
        /// <returns></returns>
        public ActionResult UploadSubmit(IEnumerable<HttpPostedFileBase> files, AssetTypesModel model)
        {
            // Clear the session variables as this is a new upload.
            this.Session.Remove(SessionFilePath);
            this.Session.Remove(SessionFileName);
            this.Session.Remove(SessionFileType);

            foreach (var file in files)
            {
                // Some browsers send file names with full path. We only care about the file name.
                var uniqueFileName = Guid.NewGuid().ToString() + ".csv";
                var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.upload"]), uniqueFileName);

                file.SaveAs(destinationPath);

                this.Session[SessionFilePath] = destinationPath;
                this.Session[SessionFileName] = Path.GetFileName(file.FileName);
                this.Session[SessionFileType] = model.AssetTypesId;

                // Only care about first file.
                break;
            }

            // Pass processing on to UploadProcess handler.
            return RedirectToAction("UploadProcess", new {customerId = model.CustomerId});
        }

        /// <summary>
        /// Handle actual submission and processing of asset upload.  Return results of
        /// upload process.
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <returns>An <see cref="ActionResult"/> with an instance of <see cref="AssetsUploadResultsModel"/> containing results</returns>
        public ActionResult UploadProcess(int customerId)
        {
            AssetsUploadResultsModel model = null;

            string assetsFile = Session[SessionFilePath] as string;
            string fileName = Session[SessionFileName] as string;
            int? assetTypeId = Session[SessionFileType] as int?;

            if (string.IsNullOrEmpty(assetsFile) || assetTypeId == null)
            {
                model = new AssetsUploadResultsModel()
                {
                    CustomerId = customerId
                };
                model.Errors.Add("Unable to process uploaded file");
            }
            else
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
                // Process the file.
                model = (new UploadFactory(connStringName, CurrentCity.LocalTime)).Process(customerId, (AssetTypeModel.AssetType)assetTypeId, assetsFile);


                // Note name of file in Results log.
                if(model.Errors.Count > 0)               
                 model.Results.Insert(0, "File Processed with errors: " + fileName);
                else
                model.Results.Insert(0, "File Processed: " + fileName);

                // Clear the session variable.
                this.Session.Remove(SessionFilePath);
                this.Session.Remove(SessionFileName);
                this.Session.Remove(SessionFileType);

                // Delete the file.
                System.IO.File.Delete(assetsFile);
            }
            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();
            return View(model);
        }

        #endregion

        #region AJAX Support for Examples and Instructions Files

        /// <summary>
        /// Return a <see cref="FileResult"/> of an example file for the appropriate resource type.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="assetType">Asset type identifier (See <see cref="AssetTypeModel.AssetType"/>)</param>
        /// <returns>Return a <see cref="FileResult"/> instance</returns>
        public FileResult DownloadExample(int customerId, int assetType)
        {
            var fileName = ( (AssetTypeModel.AssetType) assetType ).ToString() + ".csv";
            var fullFilePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.samples"]), fileName);
            return File(fullFilePath, "text/csv", fileName);
        }

        /// <summary>
        /// Return a <see cref="FileResult"/> of an instruction file for the appropriate resource type.
        /// </summary>
        /// <param name="customerId">Customer id</param>
        /// <param name="assetType">Asset type identifier (See <see cref="AssetTypeModel.AssetType"/>)</param>
        /// <returns>Return a <see cref="FileResult"/> instance</returns>
        public FileResult DownloadInstructions(int customerId, int assetType)
        {
            // Make a copy of the appropriate instrunction template file.
            var fileName = ((AssetTypeModel.AssetType)assetType).ToString() + ".txt";
            var sourcePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.samples"]), fileName);

            var uniqueFileName = Guid.NewGuid().ToString() + ".txt";
            var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.upload"]), uniqueFileName);

            System.IO.File.Copy(sourcePath, destinationPath, true);

            // Open the new file
            using (StreamWriter sw = System.IO.File.AppendText(destinationPath))
            {
                // Append the date to it.
                sw.WriteLine();
                sw.WriteLine("Created: " + DateTime.Now.ToShortDateString());
                sw.WriteLine();

                // Add appropriate constraints.
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
                var uploadFactory = new UploadFactory(connStringName, CurrentCity.LocalTime);

                sw.WriteLine(uploadFactory.ConstraintsList(customerId, ((AssetTypeModel.AssetType)assetType)));
                sw.WriteLine();

                // Add appropriate field notes
                sw.WriteLine();
                sw.WriteLine(uploadFactory.FieldsList(customerId, ((AssetTypeModel.AssetType)assetType)));

                // Save file.
                sw.Close();
            }


            return File(destinationPath, "text/txt", fileName);
        }

        #endregion

        #region PAM CONFIGURATION Upload
        public ActionResult PAMConfigBulkUploadSubmit(IEnumerable<HttpPostedFileBase> files, CustomerPAMConfigurationModel model)
        {
            // Clear the session variables as this is a new upload.
            this.Session.Remove(SessionFilePath);
            this.Session.Remove(SessionFileName);
            this.Session.Remove(SessionFileType);

            foreach (var file in files)
            {
                // Some browsers send file names with full path. We only care about the file name.
                var uniqueFileName = Guid.NewGuid().ToString() + ".csv";
                var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.upload"]), uniqueFileName);

                file.SaveAs(destinationPath);

                this.Session[SessionFilePath] = destinationPath;
                this.Session[SessionFileName] = Path.GetFileName(file.FileName);
                //this.Session[SessionFileType] = model.AssetTypesId;

                // Only care about first file.
                break;
            }
          
            // Pass processing on to UploadProcess handler.
            return RedirectToAction("UploadPAMConfigProcess", new { customerId = model.CustomerId });
        }

        /// <summary>
        /// Handle actual submission and processing of asset upload.  Return results of
        /// upload process.
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <returns>An <see cref="ActionResult"/> with an instance of <see cref="AssetsUploadResultsModel"/> containing results</returns>
        public ActionResult UploadPAMConfigProcess(int customerId)
        {
            AssetsUploadResultsModel model = null;

            string assetsFile = Session[SessionFilePath] as string;
            string fileName = Session[SessionFileName] as string;
            //int? assetTypeId = Session[SessionFileType] as int?;

            if (string.IsNullOrEmpty(assetsFile))
            {
                model = new AssetsUploadResultsModel()
                {
                    CustomerId = customerId
                };
                model.Errors.Add("Unable to process uploaded file");
            }
            else
            {
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
                // Process the file.
                model = (new UploadFactory(connStringName, CurrentCity.LocalTime)).ProcessPAMBulkUpload(customerId, assetsFile);


                // Note name of file in Results log.
                if (model.Errors.Count > 0)
                    model.Results.Insert(0, "File Processed with errors: " + fileName);
                else
                    model.Results.Insert(0, "File Processed: " + fileName);

                // Clear the session variable.
                this.Session.Remove(SessionFilePath);
                this.Session.Remove(SessionFileName);
                this.Session.Remove(SessionFileType);

                // Delete the file.
                System.IO.File.Delete(assetsFile);
            }
            var JavaRequest = new city.Controllers.AssetsController();
            JavaRequest.JavaGetRequestToURL();
            return View(model);
        }

        public FileResult DownloadPAMUploadExample(int customerId)
        {
            var fileName = "PAMConfigBulkUploadExample.csv";
            var fullFilePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.samples"]), fileName);
            return File(fullFilePath, "text/csv", fileName);
        }
        public FileResult DownloadPAMUploadInstruction(int customerId)
        {
            //var fileName = "PAMConfig.txt";
            //var fullFilePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.samples"]), fileName);
            //return File(fullFilePath, "text/txt", fileName);
            // Make a copy of the appropriate instrunction template file.
            var fileName = "PAMConfig.txt";
            var sourcePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.samples"]), fileName);

            var uniqueFileName = Guid.NewGuid().ToString() + ".txt";
            var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.asset.upload"]), uniqueFileName);

            System.IO.File.Copy(sourcePath, destinationPath, true);

            // Open the new file
            using (StreamWriter sw = System.IO.File.AppendText(destinationPath))
            {
                // Append the date to it.
                sw.WriteLine();
                sw.WriteLine("Created: " + DateTime.Now.ToShortDateString());
                sw.WriteLine();

                // Add appropriate constraints.
                var connStringName = (new SettingsFactory()).GetCustomerConnectionStringName(customerId);
                var uploadFactory = new UploadFactory(connStringName, CurrentCity.LocalTime);

                sw.WriteLine(uploadFactory.ConstraintsListForPAMConfig(customerId));
                sw.WriteLine();

                // Add appropriate field notes
                sw.WriteLine();
                sw.WriteLine(uploadFactory.FieldsListForPAMConfig());

                // Save file.
                sw.Close();
            }


            return File(destinationPath, "text/txt", fileName);
        }

        #endregion
    }
}
