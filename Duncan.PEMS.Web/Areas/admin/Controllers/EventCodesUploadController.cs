using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Events;
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using NLog;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    public class EventCodesUploadController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        private const string SessionFilePath = "PemsEventCodesBulkUpdate";
        private const string SessionFileName = "PemsEventCodesBulkUpdateFile";


        public ActionResult Upload(int customerId)
        {
            EventCodesCustomerModel model = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodesCustomerModel(customerId);

            return View(model);
        }


        public ActionResult UploadSubmit(IEnumerable<HttpPostedFileBase> files, EventCodesCustomerModel model)
        {
            Session.Remove(SessionFilePath);
            Session.Remove(SessionFileName);

            foreach (var file in files)
            {
                // Some browsers send file names with full path. We only care about the file name.
                var uniqueFileName = Guid.NewGuid().ToString() + ".csv";
                var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.eventcodes.upload"]), uniqueFileName);

                file.SaveAs(destinationPath);

                Session[SessionFilePath] = destinationPath;
                Session[SessionFileName] = Path.GetFileName(file.FileName);
            }

            return RedirectToAction("UploadProcess", new { customerId = model.CustomerId });
        }



        public ActionResult UploadProcess(int customerId)
        {
            EventCodesUploadResultsModel model;

            var eventCodesFile = Session[SessionFilePath] as string;
            var fileName = Session[SessionFileName] as string;

            if (string.IsNullOrEmpty(eventCodesFile))
            {
                model = new EventCodesUploadResultsModel
                    {
                    CustomerId = customerId
                };
                model.Errors.Add("Unable to process uploaded file");
            }
            else
            {
                // Process the file.
                model = (new UploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).Process(customerId, eventCodesFile);

                // Note name of file in Results log.
                model.Results.Insert(0, "File Processed: " + fileName);

                // Clear the session variable.
                Session.Remove(SessionFilePath);
                Session.Remove(SessionFileName);

                // Delete the file.
                System.IO.File.Delete(eventCodesFile);

            }
            return View(model);
        }

        public FileResult DownloadExample(int customerId)
        {
            const string fileName = "eventcodes.csv";
            var fullFilePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.eventcodes.samples"]), fileName);
            return File(fullFilePath, "text/csv", fileName);
        }

        public FileResult DownloadInstructions(int customerId)
        {
            // Make a copy of the appropriate instrunction template file.
            const string fileName = "eventcodes.txt";
            var sourcePath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.eventcodes.samples"]), fileName);

            var uniqueFileName = Guid.NewGuid().ToString() + ".txt";
            var destinationPath = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["pems.eventcodes.upload"]), uniqueFileName);

            System.IO.File.Copy(sourcePath, destinationPath, true);

            // Open the new file
            using (StreamWriter sw = System.IO.File.AppendText(destinationPath))
            {
                // Append the date to it.
                sw.WriteLine();
                sw.WriteLine("Created: " + DateTime.Now.ToShortDateString());
                sw.WriteLine();

                // Add appropriate constraints.
                var uploadFactory = new UploadFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());

                sw.WriteLine(uploadFactory.ConstraintsList(customerId));
                sw.WriteLine();

                // Add appropriate field notes
                sw.WriteLine();
                sw.WriteLine(uploadFactory.FieldsList(customerId));

                // Save file.
                sw.Close();
            }


            return File(destinationPath, "text/txt", fileName);
        }



    }
}
