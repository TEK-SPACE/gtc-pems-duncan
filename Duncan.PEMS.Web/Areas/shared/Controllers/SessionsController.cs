using System.Web.Mvc;
using Duncan.PEMS.Entities.Sessions;
using NLog;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    public class SessionsController : Controller
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        public JsonResult Save(TabStrip item)
        {
            Session["Session_TabStrip"] = item;
            return Json(item);
        }

        public JsonResult Load()
        {
            var item = new TabStrip { SelectedTab = "Default" };
            if (Session["Session_TabStrip"] != null)
                item = Session["Session_TabStrip"] as TabStrip;
            return Json(item);
        }

        public void SaveUserFilters(UserFilters item)
        {
            Session["__UserFilters"] = item;
        }

        public JsonResult GetUserFilters()
        {
            if (Session["__UserFilters"] != null)
            {
                var retVal = Json(Session["__UserFilters"] as UserFilters);
                //clear the session
                Session.Remove("__UserFilters");
                return retVal;
            }
            return Json(string.Empty);
        }

        public void SaveRoleFilters(RoleFilters item)
        {
            Session["__RoleFilters"] = item;
        }

        public JsonResult GetRoleFilters()
        {
            if (Session["__RoleFilters"] != null)
            {
                var retVal = Json(Session["__RoleFilters"] as RoleFilters);
                //clear the session
                Session.Remove("__RoleFilters");
                return retVal;
            }
            return Json(string.Empty);
        }



        public void SaveCustomerFilters(CustomerFilters item)
        {
            Session["__CustomerFilters"] = item;
        }

        public JsonResult GetCustomerFilters()
        {
            if (Session["__CustomerFilters"] != null)
            {
                var retVal = Json(Session["__CustomerFilters"] as CustomerFilters);
                //clear the session
                Session.Remove("__CustomerFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


        public void SaveMaintenanceGroupFilters(MaintenanceGroupFilters item)
        {
            Session["__CustomerFilters"] = item;
        }

        public JsonResult GetMaintenanceGroupFilters()
        {
            if (Session["__MaintenanceGroupFilters"] != null)
            {
                var retVal = Json(Session["__MaintenanceGroupFilters"] as MaintenanceGroupFilters);
                //clear the session
                Session.Remove("__MaintenanceGroupFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


        public void SaveAlarmFilters(AlarmFilters item)
        {
            Session["_AlarmFilters"] = item;
        }

        public JsonResult GetAlarmFilters()
        {
            if (Session["_AlarmFilters"] != null)
            {
                var retVal = Json(Session["_AlarmFilters"] as AlarmFilters);
                //clear the session
               // Session.Remove("_AlarmFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


        public void SaveCollectionFilters(CollectionFilters item)
        {
            Session["_CollectionFilters"] = item;
        }

        /// <summary>
        /// Used to keep client session alive during long periods where client 
        /// does not move to new page.  For example, reviewing rows returned on a 
        /// report.
        /// </summary>
        /// <returns>String of "OK".</returns>
        public JsonResult PingServer()
        {
            // This does nothing but force a ping of the server to keep session alive
            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCollectionFilters()
        {
            if (Session["_CollectionFilters"] != null)
            {
                var retVal = Json(Session["_CollectionFilters"] as CollectionFilters);
                //clear the session
                Session.Remove("_CollectionFilters");
                return retVal;
            }
            return Json(string.Empty);
        }

        public void SaveAggregationFilters(AggregationFilters item)
        {
            Session["_AggregationFilters"] = item;
        }

        public JsonResult GetAggregationFilters()
        {
            if (Session["_AggregationFilters"] != null)
            {
                var retVal = Json(Session["_AggregationFilters"] as AggregationFilters);
                //clear the session
                Session.Remove("_AggregationFilters");
                return retVal;
            }
            return Json(string.Empty);
        }

        public void SaveDiscountSummaryFilters(DiscountSummaryFilters item)
        {
            Session["_DiscountSummaryFilters"] = item;
        }

        public JsonResult GetDiscountSummaryFilters()
        {
            if (Session["_DiscountSummaryFilters"] != null)
            {
                var retVal = Json(Session["_DiscountSummaryFilters"] as DiscountSummaryFilters);
                //clear the session
                Session.Remove("_DiscountSummaryFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


        public void SaveTariffFilters(TariffFilters item)
        {
            Session["_TariffFilters"] = item;
        }

        public JsonResult GetTariffFilters()
        {
            if (Session["_TariffFilters"] != null)
            {
                var retVal = Json(Session["_TariffFilters"] as TariffFilters);
                //clear the session
                Session.Remove("_TariffFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


        public void SaveAssetHistoryFilters(AssetHistoryFilters item)
        {
            Session["_AssetHistoryFilters"] = item;
        }

        public JsonResult GetAssetHistoryFilters()
        {
            if (Session["_AssetHistoryFilters"] != null)
            {
                var retVal = Json(Session["_AssetHistoryFilters"] as AssetHistoryFilters);
                //clear the session
                Session.Remove("_AssetHistoryFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


        //todo - GTC: DataKey Work - add in ability to get session values for DataKey just like the code below
        public JsonResult GetDataKeyHistoryFilters()
        {
            //if (Session["_AssetHistoryFilters"] != null)
            //{
            //    var retVal = Json(Session["_AssetHistoryFilters"] as AssetHistoryFilters);
            //    //clear the session
            //    Session.Remove("_AssetHistoryFilters");
            //    return retVal;
            //}
            return Json(string.Empty);
        }


        public void SaveVCEFilters(VCEFilters item)
        {
            Session["_VCEFilters"] = item;
        }

        public JsonResult GetVCEFilters()
        {
            if (Session["_VCEFilters"] != null)
            {
                var retVal = Json(Session["_VCEFilters"] as VCEFilters);
                //clear the session
                Session.Remove("_VCEFilters");
                return retVal;
            }
            return Json(string.Empty);
        }


    }
}
