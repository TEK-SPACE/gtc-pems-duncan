using System.Collections.Generic;
using System.Web.Mvc;
using Duncan.PEMS.Business.News;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.Entities.News;
using Duncan.PEMS.Framework.Controller;
using Duncan.PEMS.Utilities;
using NLog;
using System.Linq;
using System;
using Duncan.PEMS.Entities.GIS;
using Duncan.PEMS.Business.GIS;
using Duncan.PEMS.SpaceStatus.DataSuppliers;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class HomeController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            CustomerLogic result = new CustomerLogic();
            result.CustomerId = CurrentCity.Id;
            string dbConnectStr = Session[Duncan.PEMS.Utilities.Constants.Security.ConnectionStringSessionVariableName].ToString();
            result.SharedSqlDao = new SqlServerDAO(dbConnectStr, result.LogMessage);
            result.GatherAllInventory();

            if (result != null)
            {
                if (Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.ContainsKey(result.CustomerId) == false)
                    Duncan.PEMS.SpaceStatus.DataSuppliers.CustomerLogic.CustomerManager.Instance._Customers.Add(result.CustomerId, result);
            }

            var userFactory = new UserFactory();
            if (userFactory.RequiresPasswordReset(User.Identity.Name))
                return RedirectToAction("Edit", "Profile");

            ViewBag.PWExpirationDays = userFactory.GetPasswordExpirationInDays();
            // This causes the id of the master page <body> tag to be set to 'client-inquiry'
            ViewData["ShowLandingPage"] = "true";
            var newsMgr = new NewsManager();
            List<NewsItem> newsItems = newsMgr.GetNewsItems(CurrentCity.Id, CurrentCity.Locale.ToString(), CurrentCity.LocalTime);

            //********************************************************************
            //** Code added on Oct 8th 2014 for plotting map for the selected client
            try
            {
                //** First get the Lat/Lng coordinates of the customer using the GIS factory method
                IQueryable<GISModel> LatLngDetails = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLatLngOfCustomer(CurrentCity.Id);

                ViewBag.Latitude = LatLngDetails.FirstOrDefault().Latitude;
                ViewBag.Longitude = LatLngDetails.FirstOrDefault().Longitude;

                //** Then assign Lat and Lng coordinates to the very first item of the List for plotting map on the home page of the client
                for (var i = 0; i < newsItems.Count(); i++)
                {
                    newsItems[i].Latitude = LatLngDetails.FirstOrDefault().Latitude;
                    newsItems[i].Longitude = LatLngDetails.FirstOrDefault().Longitude;
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN Index method (Homecontroller)", ex);
            }

            /////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////
            Dashboard objDash = null;
            objDash = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDashBoradInventory(CurrentCity.Id);

            double[] key;
            objDash.Key = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyDetails(CurrentCity.Id, CurrentCity.LocalTime);

            objDash.myCustomerID = CurrentCity.Id;
            /////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////


            ////********************************************************************

            ////********************************************************************
            ////********************************************************************



            string mon, date, mon2, date2, mon3, date3, mon4, date4, mon5, date5;
            DateTime month = DateTime.Today.AddDays(-1);
            DateTime day = DateTime.Today.AddDays(-1);
            mon = month.ToString("MMM");
            date = day.ToString("dd");

            DateTime month2 = DateTime.Today.AddDays(-2);
            DateTime day2 = DateTime.Today.AddDays(-2);
            mon2 = month2.ToString("MMM");
            date2 = day2.ToString("dd");

            DateTime month3 = DateTime.Today.AddDays(-3);
            DateTime day3 = DateTime.Today.AddDays(-3);
            mon3 = month3.ToString("MMM");
            date3 = day3.ToString("dd");

            DateTime month4 = DateTime.Today.AddDays(-4); ;
            DateTime day4 = DateTime.Today.AddDays(-4); ;
            mon4 = month4.ToString("MMM");
            date4 = day4.ToString("dd");

            DateTime month5 = DateTime.Today.AddDays(-5); ;
            DateTime day5 = DateTime.Today.AddDays(-5); ;
            mon5 = month5.ToString("MMM");
            date5 = day5.ToString("dd");

            //********************************************************************
            objDash.Month = mon + " " + date;
            objDash.Month2 = mon2 + " " + date2;
            objDash.Month3 = mon3 + " " + date3;
            objDash.Month4 = mon4 + " " + date4;
            objDash.Month5 = mon5 + " " + date5;

            return View(objDash);
        }

        //public ActionResult GetRevenueDetails()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    DashboardRevenue objDash = null;
        //    objDash = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetRevenueDetails(CurrentCity.Id);
        //    return Json(objDash, JsonRequestBehavior.AllowGet);

        //}

        ////********************************************************************
        ////********************************************************************

        public ActionResult GetBatteryRep()
        {
            //int customerId = Int32.Parse(customerID);

            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryReport(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetChangeBatteryRep()
        {
            //int customerId = Int32.Parse(customerID);

            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetChangeBatteryReport(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNoMeterCommRep()
        {
            //int customerId = Int32.Parse(customerID);

            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetNoMeterCommReport(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSensorCommRep()
        {
            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSensorComm(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSpacestatusSensorRep()
        {
            //var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSpacestatusSensor(CurrentCity.Id, CurrentCity.LocalTime);
            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyStatus(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaymentRep()
        {
            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPaymentStatus(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEnforceableSpaceRep()
        {
            //var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforceableSpace(CurrentCity.Id, CurrentCity.LocalTime);
            var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetEnforceStatus(CurrentCity.Id);
            return Json(mCount, JsonRequestBehavior.AllowGet);
        }
        ////********************************************************************
        ////********************************************************************


        //public ActionResult GetTotalAssetCount()
        //{
        //    NewsItem objTotalCount = new NewsItem();

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetTotalMeterCount(CurrentCity.Id);
        //    //return Json(mCount, JsonRequestBehavior.AllowGet);
        //    objTotalCount.TotalCount = mCount;
        //    return View(objTotalCount);
        //}
        //public ActionResult GetActAssetCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetMeterCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetInActAssetCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetInActMeterCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetActAlarmCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetActAlarmCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        ////public ActionResult GetInActAlarmCount()
        ////{
        ////    //int customerId = Int32.Parse(customerID);

        ////    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetInActAlarmCount(CurrentCity.Id);
        ////    return Json(mCount, JsonRequestBehavior.AllowGet);

        ////}
        ///// <summary>
        ///// ////////////////////////////////Alarms Report
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetSeverAlarmCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSevAlarmCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetMajorAlarmCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetMajAlarmCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetMinorAlarmCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetMinAlarmCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetInActAlarmCount()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetInActAlarmCount(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        ///// <summary>
        ///// ///////////////////////////////////////////////////////////////////////////////////////////////
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetCommMeters()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetActCommMeters(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetNonCommMeters()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetActNonCommMeters(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetBttrChangeRep()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBttrChange(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
        //public ActionResult GetBttrPlanChangeRep()
        //{
        //    //int customerId = Int32.Parse(customerID);

        //    var mCount = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBttrPlanChange(CurrentCity.Id);
        //    return Json(mCount, JsonRequestBehavior.AllowGet);

        //}
    }
}