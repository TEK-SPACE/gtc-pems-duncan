/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 02/10/2014       Sergey Ostrerov            DPTXPEMS-237 - Edit Screen for Event Code Crashes
 *                                                            Create/Update Event Code   
 * 
 * *****************************************************************************************************************************************************/
using System.Web.Mvc;
using Duncan.PEMS.Business.Events;
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using NLog;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class EventCodesController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Create Event Code

        public ActionResult Details(int customerId, int eventSourceId, int eventCodeId)
        {
            var model = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodeViewModel(customerId, eventSourceId, eventCodeId);

            return View(model);
        }

        #endregion

        #region Create Event Code

        public ActionResult Create(int customerId)
        {
            var model = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodeEditModel(customerId);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EventCodeEditModel model)
        {
            // Save values
            if (!ModelState.IsValid)
            {
                (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).RefreshEventCodeEditModel(model);
                return View(model);
            }
            var eventCodesFactory = new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString());
            // Does the requested EventCode settings indicae an
            // EventCode that already exists?
            if ( eventCodesFactory.CanCreateEventCode( model ) )
            {
                ModelState.AddModelError("Exists", "Event Code with this same Event Source, Event Type, Event Category and Asset Type already exists.");
                (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).RefreshEventCodeEditModel(model);
                return View(model);
            }
            (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).CreateEventCode(model);
            return RedirectToAction("EditEventCodes", "Customers", new { customerId = model.CustomerId });
        }

        #endregion


        #region Edit Event Code 

        public ActionResult Edit(int customerId, int eventSourceId, int eventCodeId, bool fromDetailsPage = false)
        {
            var model = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEventCodeEditModel(customerId, eventSourceId, eventCodeId);

            model.FromDetailsPage = fromDetailsPage;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EventCodeEditModel model)
        {
            // Save values
            if (!ModelState.IsValid)
            {
                (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).RefreshEventCodeEditModel(model);
                return View(model);
            }
            (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).SetEventCodeEditModel(model);

            // Saved and stay on same page.
            if ( model.FromDetailsPage )
                return RedirectToAction("Details", new { customerId = model.CustomerId, eventSourceId = model.SourceId, eventCodeId = model.Id });
            return RedirectToAction("EditEventCodes", "Customers", new { customerId = model.CustomerId });
        }

        #endregion


        #region Ajax Support

        public ActionResult GetAssetTypeSLA(int customerId, int meterGroupId, int ? currSlaMinutes, int currAssetTypeID)
        {
            // Get the default SLA minutes of this asset type for this customer.  This can be 
            // found in [AssetType] table.
            if (meterGroupId == -1 && meterGroupId != currAssetTypeID)
            {
                return Json(new { DefaultSLA = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetTypeDefaultSLA(customerId, meterGroupId) });
            }
            else if (meterGroupId != -1 && meterGroupId != currAssetTypeID)
            {
                return Json(new { DefaultSLA = (new EventCodesFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetTypeDefaultSLA(customerId, meterGroupId) });
            }
            return Json(new { DefaultSLA = currSlaMinutes});
        }

        #endregion

    }
}
