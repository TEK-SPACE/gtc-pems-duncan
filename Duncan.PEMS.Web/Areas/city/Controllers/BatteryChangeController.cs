using Duncan.PEMS.Entities.BatteryChange;
using System.Data.Objects;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Framework.Controller;
using Kendo.Mvc.UI;
using System.Collections;
using System.Globalization;
using System.Web.Script.Serialization;
using Kendo.Mvc.Extensions;
using System.Data;
using System.Data.Objects.SqlClient;
using Duncan.PEMS.Business.BatteryChange;
using Duncan.PEMS.Utilities;
using NLog;
using Duncan.PEMS.Business.Exports;
using Kendo.Mvc;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class BatteryChangeController : PemsController
    {

        #region NLog Logger

        //<summary>
        //NLog logger instance.
        //</summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        public ActionResult Index()
        {
            BatteryChangeModel objGISModel = new BatteryChangeModel();

            try
            {
                ViewBag.CurrentCityID = CurrentCity.Id;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN Index method (Battery Change Ctrller)", ex);
            }

            return View(objGISModel);
        }






        public ActionResult GetAssetIds(string myLocationTypeId, string myParallerLocationId, string myTypedText)
        {
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            List<BatteryChangeModel> result = new List<BatteryChangeModel>();

            try
            {

                //** Check the location type and assign the corresponding location IDs / names
                if (myLocationTypeId == "Area")
                {
                    areaNameIs = myParallerLocationId.ToString().Trim();
                }
                else if (myLocationTypeId == "Zone")
                {
                    zoneNameIs = myParallerLocationId.ToString().Trim();
                }
                else if (myLocationTypeId == "Street")
                {
                    street = myParallerLocationId.ToString().Trim();
                }
                else if (myLocationTypeId == "Suburb")
                {
                    suburb = myParallerLocationId.ToString().Trim();
                }

                result = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetIDs(CurrentCity.Id, areaNameIs, zoneNameIs, street, suburb, myTypedText);



                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetLocationTypeId(string customerID, string locationType, string myTypedText)
        {
            int CityID = Convert.ToInt32(customerID);

            BatteryChangeModel objGISModel = new BatteryChangeModel();
            IEnumerable<BatteryChangeModel> details = Enumerable.Empty<BatteryChangeModel>();


            details = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId(locationType, CityID, myTypedText);


            if (locationType == "Street" || locationType == "Suburb" || locationType == "Demand Area")
            {
                var allDetails = (from detail in details
                                  select new
                                  {
                                      Text = detail.Text,
                                      Value = detail.value.ToString()
                                  }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items

                return Json(allDetails, JsonRequestBehavior.AllowGet);
            }



            List<SelectListItem> lst = new List<SelectListItem>();


            if (details != null)
            {
                foreach (var k in details)
                {
                    lst.Add(new SelectListItem { Text = k.Text.ToString(), Value = k.value.ToString() });
                }
            }

            return Json(lst, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetTrellisChartDataForUniqueIDS(string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            List<BatteryChartModel> result = new List<BatteryChartModel>();


            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            // long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            //** Convert AssetId to int list type before invoking factory class
            string[] assetTypeArr = AssetId.Split(',');
            List<int?> assetTypeIDs = new List<int?>();
            for (var i = 0; i < assetTypeArr.Length; i++)
            {
                assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
            }

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                result = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryAnalysisChartForUniqueIDS(CurrentCity.Id, startTimeOutput, endTimeOutput, assetTypeIDs, areaNameIs, zoneNameIs, street);
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetTrellisChartData Method", ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTrellisChartData(string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            List<BatteryChartModel> result = new List<BatteryChartModel>();


            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                result = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryAnalysisChart(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetTrellisChartData Method", ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBatteryChangeByDay([DataSourceRequest]DataSourceRequest request, int customerId, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId, decimal voltageStart, decimal voltageEnd)
        {

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();

            int total = 0;

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBarChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);

                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   chartData = p.chartData,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();



            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetPieChartData Method", ex);
            }


            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            {
                Data = finalResult,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetBatteryChangeDetails([DataSourceRequest]DataSourceRequest request, int customerId, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId, decimal voltageStart, decimal voltageEnd)
        {

            IEnumerable<BatteryChangeModel> finalResult = Enumerable.Empty<BatteryChangeModel>();

            int total = 0;

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryChangeModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPieChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, suburb, voltageStart, voltageEnd);

                finalResult = (from r in details_raw
                               select new BatteryChangeModel
                               {
                                   avgVoltage = r.avgVoltage,
                                   AssetType = r.AssetType,
                                   TimeOfLastStatus = r.TimeOfLastStatus_Date.ToString(),
                                   Area = r.Area,
                                   Zone = r.Zone,
                                   chartData = r.chartData,
                                   Suburb = r.Suburb,
                                   Street = r.Street,
                                   MID = r.MID,
                                   MeterName = r.MeterName,
                                   category = r.category,
                                   value = r.value,
                                   color = r.color
                               }).ToList();



            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetPieChartData Method", ex);
            }


            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            {
                Data = finalResult,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationsForListBox([DataSourceRequest] DataSourceRequest request, string myLocationTypeId, string myParallerLocationId, string customerID, string startDate, string endDate, string statusList)
        {
            int CID = Convert.ToInt32(customerID);
            List<sensorProfileModel> AllData = new List<sensorProfileModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            try
            {

                //** Check the location type and assign the corresponding location IDs / names
                if (myLocationTypeId == "Area")
                {
                    areaNameIs = myParallerLocationId.ToString().Trim();
                }
                else if (myLocationTypeId == "Zone")
                {
                    zoneNameIs = myParallerLocationId.ToString().Trim();
                }
                else if (myLocationTypeId == "Street")
                {
                    street = myParallerLocationId.ToString().Trim();
                }
                else if (myLocationTypeId == "Suburb")
                {
                    suburb = myParallerLocationId.ToString().Trim();
                }

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }

                //** Convert StatusList to int list type before invoking factory class
                string[] statusTypeArr = statusList.Split(',');
                List<string> statusTypeIDs = new List<string>();
                for (var i = 0; i < statusTypeArr.Length; i++)
                {
                    statusTypeIDs.Add((statusTypeArr[i]));
                }

                IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationsForListBox(request, CID, areaNameIs, zoneNameIs, street, suburb, startTimeOutput, endTimeOutput, statusTypeIDs);

                return Json(details_raw.ToDataSourceResult(request));

            }
            catch (Exception e)
            {

            }


            return Json(AllData, JsonRequestBehavior.AllowGet);



        }

        public ActionResult GetCustomers([DataSourceRequest] DataSourceRequest request)
        {
            List<sensorProfileModel> final = new List<sensorProfileModel>();
            IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCustomers(request);
            final = details_raw.ToList();

            return new LargeJsonResult() { Data = final, MaxJsonLength = int.MaxValue };
        }


        public ActionResult GetStatusProfileForListBox([DataSourceRequest] DataSourceRequest request, string customerID, DateTime startDate, DateTime endDate, string statusList)
        {
            int? CID = Convert.ToInt32(customerID);
            List<sensorProfileModel> AllData = new List<sensorProfileModel>();
            List<sensorProfileModel> result = new List<sensorProfileModel>();

            try
            {
                int[] statusID = { 1, 2, 3, 4, 5 };
                string[] statusName = { "Corrupted", "Occupied", "Small object", "Suspicious", "Vacant" };


                for (var i = 0; i < statusID.Length; i++)
                {
                    sensorProfileModel inst = new sensorProfileModel();
                    inst.Text = statusName[i];
                    inst.Value = statusID[i];
                    result.Add(inst);
                }
                return Json(result.ToDataSourceResult(request));

            }
            catch (Exception e)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetProfileData([DataSourceRequest]DataSourceRequest request, string customerID, string startDate, string endDate, string sensorIDS, string statusList)
        {
            int cityID = Convert.ToInt32(customerID);
            int total = 0;

            List<sensorProfileModel> final = new List<sensorProfileModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }

                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = sensorIDS.Split(',');
                List<Int32?> assetTypeIDs = new List<Int32?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert StatusList to int list type before invoking factory class
                string[] statusTypeArr = statusList.Split(',');
                List<string> statusTypeIDs = new List<string>();
                for (var i = 0; i < statusTypeArr.Length; i++)
                {
                    statusTypeIDs.Add((statusTypeArr[i]));
                }

                IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetProfileData(request, out total, cityID, startTimeOutput, endTimeOutput, assetTypeIDs, statusTypeIDs);

                final = details_raw.ToList();

                DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
                {
                    Data = final,
                    Total = total,
                };

                return new LargeJsonResult() { Data = result, MaxJsonLength = int.MaxValue };
            }
            catch (Exception e)
            {

            }



            return new LargeJsonResult() { Data = final, MaxJsonLength = int.MaxValue };
        }


        public ActionResult GetProfileDataForChart(string customerID, string startDate, string endDate, string sensorIDS, string statusList)
        {
            int CityID = Convert.ToInt32(customerID);
            List<sensorProfileModel> final = new List<sensorProfileModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }

                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = sensorIDS.Split(',');
                List<Int32?> assetTypeIDs = new List<Int32?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert StatusList to int list type before invoking factory class
                string[] statusTypeArr = statusList.Split(',');
                List<string> statusTypeIDs = new List<string>();
                for (var i = 0; i < statusTypeArr.Length; i++)
                {
                    statusTypeIDs.Add((statusTypeArr[i]));
                }

                IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetProfileDataForChart(CityID, startTimeOutput, endTimeOutput, assetTypeIDs, statusTypeIDs);

                final = details_raw.ToList();

                return new LargeJsonResult() { Data = final, MaxJsonLength = int.MaxValue };
            }
            catch (Exception e)
            {

            }



            return new LargeJsonResult() { Data = final, MaxJsonLength = int.MaxValue };
        }

        public ActionResult GetOccupancyRateDetails([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {

            IEnumerable<OccupancyRateModel> lstCurrent = Enumerable.Empty<OccupancyRateModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyRateDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);

                var finalresult = lstCurrent.ToList();

                DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
                {
                    Data = finalresult,
                    Total = total,
                };

                return new LargeJsonResult() { Data = result, MaxJsonLength = int.MaxValue };

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails Method", ex);
            }


            return Json(lstCurrent, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOccupancyRateDetailsForChart([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {

            IEnumerable<OccupancyRateModel> lstCurrent = Enumerable.Empty<OccupancyRateModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyRateDetailsForChart(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);

                var finalresult = lstCurrent.ToList();

                //DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
                //{
                //    Data = finalresult,
                //    Total = total,
                //};

                //return Json(finalresult, JsonRequestBehavior.AllowGet);
                //** Use LargeJsonResult class as the no. of data is huge (i.e. more than 6000 records which resulted in
                //** Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property
                return new LargeJsonResult() { Data = finalresult, MaxJsonLength = int.MaxValue };

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails Method", ex);
            }


            return Json(lstCurrent, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetBatteryAnalysisDetails([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {

            IEnumerable<BatteryAnalysisModel> lstCurrent = Enumerable.Empty<BatteryAnalysisModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;

            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryAnalysisDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);

                var finalresult = lstCurrent.ToList();

                DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
                {
                    Data = finalresult,
                    Total = total,
                };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails Method", ex);
            }


            return Json(lstCurrent, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetUniqueIDSFromAction(string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {

            IEnumerable<BatteryAnalysisModel> lstCurrent = Enumerable.Empty<BatteryAnalysisModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;


            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {
                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetUniqueIDSFromAction(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);

                var finalresult = lstCurrent.ToList();

                return Json(finalresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCommsData Method", ex);
            }

            return Json(lstCurrent, JsonRequestBehavior.AllowGet);

        }



        public ActionResult GetCommsDataForUniqueIDS(string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {

            IEnumerable<BatteryAnalysisModel> lstCurrent = Enumerable.Empty<BatteryAnalysisModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;


            // long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            //** Convert AssetId to int list type before invoking factory class
            string[] assetTypeArr = AssetId.Split(',');
            List<int?> assetTypeIDs = new List<int?>();
            for (var i = 0; i < assetTypeArr.Length; i++)
            {
                assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
            }


            try
            {
                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCommsDataForUniqueIDS(CurrentCity.Id, startTimeOutput, endTimeOutput, assetTypeIDs, areaNameIs, zoneNameIs, street);

                var finalresult = lstCurrent.ToList();

                return Json(finalresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCommsData Method", ex);
            }

            return Json(lstCurrent, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetCommsData(string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {

            IEnumerable<BatteryAnalysisModel> lstCurrent = Enumerable.Empty<BatteryAnalysisModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int total = 0;

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;


            long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

            try
            {
                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    // ** Valid end date 
                }

                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCommsData(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);

                var finalresult = lstCurrent.ToList();

                return Json(finalresult, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCommsData Method", ex);
            }

            return Json(lstCurrent, JsonRequestBehavior.AllowGet);

        }


        public FileResult ExportToExcel_BatteryChange([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId, decimal voltageStart, decimal voltageEnd)
        {
            // ** Export options - Step 4

            IEnumerable<BatteryChangeModel> finalResult = Enumerable.Empty<BatteryChangeModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryChangeModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPieChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, suburb, voltageStart, voltageEnd);

                finalResult = (from r in details_raw
                               select new BatteryChangeModel
                               {
                                   avgVoltage = r.avgVoltage,
                                   AssetType = r.AssetType,
                                   TimeOfLastStatus = r.TimeOfLastStatus_Date.ToString(),
                                   Area = r.Area,
                                   Zone = r.Zone,
                                   Suburb = r.Suburb,
                                   Street = r.Street,
                                   MID = r.MID,
                                   MeterName = r.MeterName,
                                   category = r.category,
                                   value = r.value,
                                   color = r.color
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChange Method (Battery Change Reports)", ex);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(finalResult, CurrentController, "GetBatteryChangeDetails", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Battery_Voltage_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user


        }

        public FileResult ExportToExcel_SensorProfile([DataSourceRequest]DataSourceRequest request, string customerID, string startDate, string endDate, string sensorIDS, string statusList)
        {
            // ** Export options - Step 4

            int total = 0;

            int CityID = Convert.ToInt32(customerID);

            List<sensorProfileModel> final = new List<sensorProfileModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }

                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = sensorIDS.Split(',');
                List<Int32?> assetTypeIDs = new List<Int32?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert StatusList to int list type before invoking factory class
                string[] statusTypeArr = statusList.Split(',');
                List<string> statusTypeIDs = new List<string>();
                for (var i = 0; i < statusTypeArr.Length; i++)
                {
                    statusTypeIDs.Add((statusTypeArr[i]));
                }

                IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetProfileData(request, out total, CityID, startTimeOutput, endTimeOutput, assetTypeIDs, statusTypeIDs);

                final = (from p in details_raw
                         select new sensorProfileModel
                         {
                             CustomerID = p.CustomerID,
                             LastUpdatedTS = p.LastUpdatedTS.ToString(),
                             Area = p.Area,
                             Zone = p.Zone,
                             Street = p.Street,
                             MID = p.MID,
                             TMPDesc = p.TMPDesc,
                             RUDesc = p.RUDesc,
                             ATDesc = p.ATDesc,
                             FSW = p.FSW.ToString(),
                             SW = p.SW.ToString(),
                             SWDesc = p.FSW.ToString() + " " + p.SW.ToString(),
                             BL = p.BL.ToString(),
                             TRPKDesc = p.TRPKDesc,
                             LastStatus = p.LastStatus,
                             OccupancyStatus = p.OccupancyStatus,
                             ProfileStatus = p.ProfileStatus
                         }).ToList();



            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN ExportToEXCEL_SensorProfile Method (Battery Change Reports)", e);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(final, CurrentController, "GetProfileData", CityID, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Sensor_Profile_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user


        }

        public FileResult ExportToExcel_BatteryAnalysisReport([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            // ** Export options - Step 4

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;



            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryAnalysisDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);

                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryAnalysis Method (Battery Change Reports)", ex);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(finalResult, CurrentController, "GetBatteryAnalysisDetails", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Battery_Analysis_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user


        }

        public FileResult ExportToExcel_OccupancyRateReport([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            // ** Export options - Step 4

            List<OccupancyRateModel> lstCurrent = new List<OccupancyRateModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            int total = 0;



            try
            {

                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }


                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyRateDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);




            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOccRate Method (Battery Change Reports)", ex);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(lstCurrent, CurrentController, "GetOccupancyRateDetails", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Occupancy_Rate_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user


        }

        public FileResult ExportToExcel_BatteryChangeByDay([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            // ** Export options - Step 4

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBarChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);

                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   chartData = p.chartData,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeByDay Method (Battery Change Reports)", ex);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(finalResult, CurrentController, "GetBatteryChangeByDay", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "Battery_Change_By_Day_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user


        }


        public FileResult ExportToCSV_BatteryChange([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId, decimal voltageStart, decimal voltageEnd)
        {
            //** Export options - Step 4

            IEnumerable<BatteryChangeModel> finalResult = Enumerable.Empty<BatteryChangeModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryChangeModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPieChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, suburb, voltageStart, voltageEnd);

                finalResult = (from r in details_raw
                               select new BatteryChangeModel
                               {
                                   avgVoltage = r.avgVoltage,
                                   AssetType = r.AssetType,
                                   TimeOfLastStatus = r.TimeOfLastStatus_Date.ToString(),
                                   Area = r.Area,
                                   Zone = r.Zone,
                                   Suburb = r.Suburb,
                                   Street = r.Street,
                                   MID = r.MID,
                                   MeterName = r.MeterName,
                                   category = r.category,
                                   value = r.value,
                                   color = r.color
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChange Method (Battery Change Reports)", ex);
            }






            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(finalResult, CurrentController, "GetBatteryChangeDetails", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Battery_Voltage_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToCSV_SensorProfile([DataSourceRequest]DataSourceRequest request, string customerID, string startDate, string endDate, string sensorIDS, string statusList)
        {
            // ** Export options - Step 4

            int total = 0;

            int CityID = Convert.ToInt32(customerID);

            List<sensorProfileModel> final = new List<sensorProfileModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }

                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = sensorIDS.Split(',');
                List<Int32?> assetTypeIDs = new List<Int32?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert StatusList to int list type before invoking factory class
                string[] statusTypeArr = statusList.Split(',');
                List<string> statusTypeIDs = new List<string>();
                for (var i = 0; i < statusTypeArr.Length; i++)
                {
                    statusTypeIDs.Add((statusTypeArr[i]));
                }

                IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetProfileData(request, out total, CityID, startTimeOutput, endTimeOutput, assetTypeIDs, statusTypeIDs);

                // final = details_raw.ToList();

                final = (from p in details_raw
                         select new sensorProfileModel
                         {
                             CustomerID = p.CustomerID,
                             LastUpdatedTS = p.LastUpdatedTS.ToString(),
                             Area = p.Area,
                             Zone = p.Zone,
                             Street = p.Street,
                             MID = p.MID,
                             TMPDesc = p.TMPDesc,
                             RUDesc = p.RUDesc,
                             ATDesc = p.ATDesc,
                             FSW = p.FSW.ToString(),
                             SW = p.SW.ToString(),
                             SWDesc = p.FSW.ToString() + " " + p.SW.ToString(),
                             BL = p.BL.ToString(),
                             TRPKDesc = p.TRPKDesc,
                             LastStatus = p.LastStatus,
                             OccupancyStatus = p.OccupancyStatus,
                             ProfileStatus = p.ProfileStatus
                         }).ToList();



            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN ExportToPDF_SensorProfile Method (Battery Change Reports)", e);
            }



            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(final, CurrentController, "GetProfileData", CityID);
            return File(output, "text/comma-separated-values", "Sensor_Profile_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToCSV_OccupancyRateReport([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            //** Export options - Step 4

            List<OccupancyRateModel> lstCurrent = new List<OccupancyRateModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            int total = 0;



            try
            {

                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }


                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyRateDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChange Method (Battery Change Reports)", ex);
            }






            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(lstCurrent, CurrentController, "GetOccupancyRateDetails", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Occupancy_Rate_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToCSV_BatteryAnalysisReport([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            //** Export options - Step 4

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;



            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }



                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryAnalysisDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);
                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();




            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryAnalysis Method (Battery Change Reports)", ex);
            }


            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(finalResult, CurrentController, "GetBatteryAnalysisDetails", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Battery_Analysis_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToCSV_BatteryChangeByDay([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            //** Export options - Step 4

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBarChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);

                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   chartData = p.chartData,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeByDay Method (Battery Change Reports)", ex);
            }






            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(finalResult, CurrentController, "GetBatteryChangeByDay", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "Battery_Change_By_Day_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToPDF_BatteryChange([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId, decimal voltageStart, decimal voltageEnd)
        {
            // ** Export options - Step 4

            IEnumerable<BatteryChangeModel> finalResult = Enumerable.Empty<BatteryChangeModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Suburb")
                {
                    suburb = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryChangeModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetPieChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, suburb, voltageStart, voltageEnd);

                finalResult = (from r in details_raw
                               select new BatteryChangeModel
                               {
                                   avgVoltage = r.avgVoltage,
                                   AssetType = r.AssetType,
                                   TimeOfLastStatus = r.TimeOfLastStatus_Date.ToString(),
                                   Area = r.Area,
                                   Zone = r.Zone,
                                   Suburb = r.Suburb,
                                   Street = r.Street,
                                   MID = r.MID,
                                   MeterName = r.MeterName,
                                   category = r.category,
                                   value = r.value,
                                   color = r.color
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChange Method (Battery Change Reports)", ex);
            }


            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(finalResult, CurrentController, "GetBatteryChangeDetails", CurrentCity.Id, filters, 1);

            //  send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Battery_Voltage_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");


        }

        public FileResult ExportToPDF_SensorProfile([DataSourceRequest]DataSourceRequest request, string customerID, string startDate, string endDate, string sensorIDS, string statusList)
        {
            // ** Export options - Step 4

            int total = 0;
            int CityID = Convert.ToInt32(customerID);

            List<sensorProfileModel> final = new List<sensorProfileModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }

                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = sensorIDS.Split(',');
                List<Int32?> assetTypeIDs = new List<Int32?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert StatusList to int list type before invoking factory class
                string[] statusTypeArr = statusList.Split(',');
                List<string> statusTypeIDs = new List<string>();
                for (var i = 0; i < statusTypeArr.Length; i++)
                {
                    statusTypeIDs.Add((statusTypeArr[i]));
                }

                IEnumerable<sensorProfileModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetProfileData(request, out total, CityID, startTimeOutput, endTimeOutput, assetTypeIDs, statusTypeIDs);

                //final = details_raw.ToList();

                final = (from p in details_raw
                         select new sensorProfileModel
                         {
                             CustomerID = p.CustomerID,
                             LastUpdatedTS = p.LastUpdatedTS.ToString(),
                             Area = p.Area,
                             Zone = p.Zone,
                             Street = p.Street,
                             MID = p.MID,
                             TMPDesc = p.TMPDesc,
                             RUDesc = p.RUDesc,
                             ATDesc = p.ATDesc,
                             FSW = p.FSW.ToString(),
                             SW = p.SW.ToString(),
                             SWDesc = p.FSW.ToString() + " " + p.SW.ToString(),
                             BL = p.BL.ToString(),
                             TRPKDesc = p.TRPKDesc,
                             LastStatus = p.LastStatus,
                             OccupancyStatus = p.OccupancyStatus,
                             ProfileStatus = p.ProfileStatus
                         }).ToList();



            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN ExportToPDF_SensorProfile Method (Battery Change Reports)", e);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(final, CurrentController, "GetProfileData", CityID, filters, 1);

            //  send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Sensor_Profile_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");


        }

        public FileResult ExportToPDF_OccupancyRateReport([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            // ** Export options - Step 4

            List<OccupancyRateModel> lstCurrent = new List<OccupancyRateModel>();

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            int total = 0;



            try
            {

                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }


                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                lstCurrent = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyRateDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);




            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChange Method (Battery Change Reports)", ex);
            }


            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(lstCurrent, CurrentController, "GetOccupancyRateDetails", CurrentCity.Id, filters, 1);

            //  send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Occupancy_Rate_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");


        }

        public FileResult ExportToPDF_BatteryAnalysisReport([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            // ** Export options - Step 4

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }


                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBatteryAnalysisDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street, request, out total);

                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryAnalysis Method (Battery Change Reports)", ex);
            }


            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(finalResult, CurrentController, "GetBatteryAnalysisDetails", CurrentCity.Id, filters, 1);

            //  send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Battery_Analysis_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");


        }

        public FileResult ExportToPDF_BatteryChangeByDay([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string AssetId, string LocationTypeId, string LocationTypeName, string LocationId)
        {
            // ** Export options - Step 4

            IEnumerable<BatteryAnalysisModel> finalResult = Enumerable.Empty<BatteryAnalysisModel>();
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            string street = string.Empty;
            string suburb = string.Empty;


            int total = 0;

            try
            {

                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    //** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //** Valid end date 
                }



                long assetID = (!string.IsNullOrEmpty(AssetId)) ? Convert.ToInt64(AssetId) : -1;


                if (DateTime.TryParse(startDate, out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(endDate, out endTimeOutput))
                {
                    //  ** Valid end date 
                }

                //** Check the location type and assign the corresponding location IDs / names
                if (LocationTypeId == "Area")
                {
                    areaNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Zone")
                {
                    zoneNameIs = LocationId.ToString().Trim();
                }
                else if (LocationTypeId == "Street")
                {
                    street = LocationId.ToString().Trim();
                }

                IEnumerable<BatteryAnalysisModel> details_raw = (new BatteryChangeFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetBarChartData(request, out total, CurrentCity.Id, startTimeOutput, endTimeOutput, assetID, areaNameIs, zoneNameIs, street);

                finalResult = (from p in details_raw
                               select new BatteryAnalysisModel
                               {
                                   MID = p.MID,
                                   MeterName = p.MeterName,
                                   Area = p.Area,
                                   Zone = p.Zone,
                                   Street = p.Street,
                                   chartData = p.chartData,
                                   commsBtw_FirstChange_SecondChange = p.commsBtw_FirstChange_SecondChange,
                                   commsBtw_SecondChange_ThirdChange = p.commsBtw_SecondChange_ThirdChange,
                                   BatChangesCnt = p.BatChangesCnt,
                                   firstChange = p.firstChange.ToString(),
                                   secondChange = p.secondChange.ToString(),
                                   thirdChange = p.thirdChange.ToString(),
                                   SensorStatus = p.SensorStatus
                               }).ToList();


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeByDay Method (Battery Change Reports)", ex);
            }


            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(finalResult, CurrentController, "GetBatteryChangeByDay", CurrentCity.Id, filters, 1);

            //  send the memory stream as File
            return File(output.ToArray(), "application/pdf", "Battery_Change_By_Day_Report_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");


        }


    }
}
