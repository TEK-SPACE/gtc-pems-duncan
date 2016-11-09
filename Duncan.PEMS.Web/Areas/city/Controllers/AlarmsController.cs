/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 01/23//2014      Sergey Ostrerov            DPTXPEMS-107 - PEMS: Alarm Inquiry >PDF export file has 2 rows in the header.
 * 
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.Business.Alarms;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Entities.Alarms;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
     [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class AlarmsController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region "Listing"
        
        public ActionResult Index()
        {
            ViewBag.CurrentCityID = CurrentCity.Id;
            return View();
        }

        [HttpGet]
        public ActionResult GetItems([DataSourceRequest] DataSourceRequest request)
        {
            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Alarm");

            //get models
            var items = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarms(request, "TimeOfOccurrance desc");

            int total = 0;
            if (items.Any())
            {
                total = items.First().Count;

                //additional requirement, if the time til > 0, just set it as 0. we are going to do it here and NOT in the view

                foreach (var spAlarmModel in items)
                {
                    if (spAlarmModel.TotalMinutes < 0)
                        spAlarmModel.TotalMinutes = 0;

                    spAlarmModel.TotalMinutesDisplay = FormatHelper.FormatTimeFromMinutes(spAlarmModel.TimeRemainingUntilTarget, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HHH_MM);
                }


            }
            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Filter Values"

        public JsonResult GetAssetTypes( int cityId )
        {
            var assetTypes = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmAssetTypes(cityId);
            return Json( assetTypes, JsonRequestBehavior.AllowGet ); 
        }

        /// <summary>
        /// Gets the list items in the AlarmStatus dropdown list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAlarmStatuses()
        {
            var values = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmStatusValues();

            return Json(values, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the list items in the AlarmStatus dropdown list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTargetServices(int cityId)
        {
            var values = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTargetServices(cityId);
            return Json(values, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of items for the Asset State dropdown filter
        /// </summary>
        /// <returns><see cref="JsonResult"/> formatted list of asset statuses </returns>
        public JsonResult GetAssetStates()
        {
            var assets = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetStates();
            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of items for the Event Sources filter
        /// </summary>
        /// <returns><see cref="JsonResult"/> formatted list of alarm sources </returns>
        public JsonResult GetAlarmSources()
        {
            var sources = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmSources();
            return Json(sources, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Gets a list of items in the TimeTypes dropdown list
        /// </summary>
        /// <returns><see cref="JsonResult"/> formatted list of time types </returns>
        public JsonResult GetTimeTypes()
        {
            var timeTypes = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmTimeTypes();
            return Json(timeTypes, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>
        /// Gets a JSON-formatted list of alarm severities.
        /// </summary>
        /// <returns><see cref="JsonResult"/> formatted list of alarm severities </returns>
        public JsonResult GetAlarmSeverities()
        {
            var severities = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmServities();
            return Json(severities, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of Technicians for the TechnicianID dropdown
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTechnicianDetails()
        {
            var techs = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmTechnicians();
            return Json(techs, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Gets a list of items for the Alarm filter
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAlarmFilters(int customerId)
        {
            var items = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmFilters(customerId);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of Areas for the Areas filter
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAlarmAreas(int cityId)
        {
            var areas = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmAreas(cityId);
            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of items for the Zones filter
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAlarmZones(int cityId)
        {
            var zones = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmZones(cityId);
            return Json(zones, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a list of items for the Suburbs dropdown list
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAlarmSubAreas(int cityId)
        {
            var areas = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarmSubAreas(cityId);
            return Json(areas, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region "Exporting"

        public FileResult ExportToCsv([DataSourceRequest]DataSourceRequest request)
        {
            var items = GetExportData(request);
            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetItems", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "AlarmsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToExcel([DataSourceRequest]DataSourceRequest request, string customerId, string startDate, string endDate, string timeType)
        {
            var items = GetExportData(request);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetItems", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "AlarmsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToPdf([DataSourceRequest]DataSourceRequest request, string customerId, string startDate, string endDate, string timeType)
        {
            var items = GetExportData(request);
            var filters = new List<FilterDescriptor>();
            //pass in an empty list
            filters = GetFilterDescriptors(request.Filters, filters);

            var itemToRemove = filters.FirstOrDefault(r => r.Member == "CustomerId");
            if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetItems", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "AlarmsExport_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }
        private IEnumerable<SpAlarmModel> GetExportData(DataSourceRequest request)
        {
            var items = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAlarms(request, "TimeOfOccurrance desc");
            //additional requirement, if the time til > 0, just set it as 0. we are going to do it here and NOT in the view
            foreach (var spAlarmModel in items)
            {
                if (spAlarmModel.TotalMinutes < 0)
                    spAlarmModel.TotalMinutes = 0;
            }

            return items;
        }
        #endregion

        #region "Details"
        
        // GET: /city/Alarms/Details/5

        public ActionResult Details(int areaID, int meterID, int eventCode, int eventSource, long occuranceTimeTicks, bool isClosed, int customerId)
         {
             var occuranceTime = new DateTime(occuranceTimeTicks);
             //if it is open, get the active alarm
             if (!isClosed)
             {
                 var localTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;

                 AlarmModel alarm = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetActiveAlarmDetails(customerId, areaID, meterID, eventCode, eventSource, occuranceTime, localTime);
                 alarm.ClosureNotification = localTime;
                 alarm.TimeCleared = localTime;

                 return View(alarm);
             }
             //otherwise its a historical
             AlarmModel historicalAlarm = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetHistoricAlarmDetails( customerId, areaID, meterID, eventCode, eventSource, occuranceTime );
             return View(historicalAlarm);
         }

         public ActionResult AlarmDetails( int eventId, int customerId, bool isClosed)
         {
             //if it is open, get the active alarm
             if( !isClosed )
             {
                 var localTime = CurrentCity == null ? DateTime.Now : CurrentCity.LocalTime;
                 AlarmModel alarm = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetActiveAlarmDetails(customerId, eventId, localTime);
                 //set the time cleared and closure notificaiton time here
                 alarm.ClosureNotification = localTime;
                 alarm.TimeCleared = localTime;
                 return View( "Details", alarm );
             }

             //otherwise its a historical
             AlarmModel historicalAlarm = (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetHistoricAlarmDetails( customerId, eventId );
             return View( "Details", historicalAlarm );
         }
         

         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Details(AlarmModel model, string submitButton, FormCollection formColl, long OccuranceTimeTicks)
         {
             //check the other submit buttons and act on them, or continue
             switch (submitButton)
             {
                 case "Cancel":
                 case "Return":
                     return RedirectToAction("Index", "Alarms", new { rtn = "true" });
             }

             // Rebind model since locale of thread has been set since original binding of model by MVC
             if (TryUpdateModel(model, formColl.ToValueProvider()))
                 UpdateModel(model, formColl.ToValueProvider());

             // Revalidate model after rebind.
             ModelState.Clear();
             TryValidateModel(model);

             //clear the alarm and send them back to the index page
             if (ModelState.IsValid)
             {
                 //update the model to use the ticks, since miliseconds are lost in the hiddenfor.
                 var occuranceTime = new DateTime(OccuranceTimeTicks);
                 model.TimeOccured = occuranceTime;
                 (new AlarmFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).ClearAlarm(model);
                 return RedirectToAction("Index", "Alarms", new { rtn = "true" });
             }
           
             //might need to rebuild the time types here
             return View(model);
         }
         #endregion
    }
}
