
/******************* CHANGELOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             __________________________________________________________________________________________________
 * 7/27/2014          Sairam                 Enhancement done in GIS - Meters page: Allow Demand zones for all three layers
 * 8/14/2014          Sairam                 Implemented new page (Citation) under GIS
 * *****************************************************************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.GIS;
using Duncan.PEMS.Entities.BatteryChange;
using Duncan.PEMS.Framework.Controller;
using Kendo.Mvc.UI;
using System.Collections;
using System.Globalization;
using System.Web.Script.Serialization;
using Kendo.Mvc.Extensions;
using System.Data;
using System.Data.Objects.SqlClient;
using Duncan.PEMS.Business.GIS;
using Duncan.PEMS.Utilities;
using NLog;
using Duncan.PEMS.Business.Exports;
using Kendo.Mvc;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{


    public class GISController : PemsController
    {

        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Clicking the Meter Link under GIS menu navigates to the Meter page UI
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            GISModel objGISModel = new GISModel();

            try
            {
                //** Get the Lat/Lng coordinates of the customer
                var LatLngDetails = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLatLngOfCustomer(CurrentCity.Id);
                ViewBag.CurrentCityID = CurrentCity.Id;

                objGISModel.LocationTypeId = "Select a type";
                objGISModel.Latitude = LatLngDetails.FirstOrDefault().Latitude;
                objGISModel.Longitude = LatLngDetails.FirstOrDefault().Longitude;


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN Index method (Giscontroller)", ex);
            }

            return View(objGISModel);
        }

      

        /// <summary>
        /// Clicking the 'Officer Current Location' Link under GIS menu navigates to the Officer Current Location UI 
        /// </summary>
        /// <returns></returns>
        public ActionResult CurrentLocation()
        {
            GISModel objGISModel = new GISModel();

            try
            {
                //** Get the Lat/Lng coordinates of the customer
                var LatLngDetails = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLatLngOfCustomer(CurrentCity.Id);
                ViewBag.CurrentCityID = CurrentCity.Id;

                objGISModel.LocationTypeId = "Select a type";
                objGISModel.Latitude = LatLngDetails.FirstOrDefault().Latitude;
                objGISModel.Longitude = LatLngDetails.FirstOrDefault().Longitude;


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN CurrentLocation method (Giscontroller)", ex);
            }

            return View(objGISModel);
            // return View();
        }

        /// <summary>
        /// Clicking the 'Officer Route' Link under GIS menu navigates to the Officer Route UI 
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficerRoute()
        {
            GISModel objGISModel = new GISModel();

            try
            {
                //** Get the Lat/Lng coordinates of the customer
                var LatLngDetails = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLatLngOfCustomer(CurrentCity.Id);
                ViewBag.CurrentCityID = CurrentCity.Id;

                objGISModel.LocationTypeId = "Select a type";
                objGISModel.Latitude = LatLngDetails.FirstOrDefault().Latitude;
                objGISModel.Longitude = LatLngDetails.FirstOrDefault().Longitude;


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN OfficerRoute method (Giscontroller)", ex);
            }

            return View(objGISModel);

            // return View();
        }

        /// <summary>
        /// Clicking the 'Financial Report Type' Link under GIS menu navigates to the FinancialReportType UI 
        /// </summary>
        /// <returns></returns>
        public ActionResult FinancialReportType()
        {
            GISModel objGISModel = new GISModel();
            try
            {
                var LatLngDetails = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLatLngOfCustomer(CurrentCity.Id);

                ViewBag.CurrentCityID = CurrentCity.Id;

                objGISModel.LocationTypeId = "Select a type";
                objGISModel.Latitude = LatLngDetails.FirstOrDefault().Latitude;
                objGISModel.Longitude = LatLngDetails.FirstOrDefault().Longitude;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN Index method (Giscontroller)", ex);
            }
            //** Get the Lat/Lng coordinates of the customer



            return View(objGISModel);
        }

      

        /// <summary>
        /// Clicking the 'Citation' Link under GIS menu navigates to the Citation UI 
        /// </summary>
        /// <returns></returns>
        public ActionResult Citation()
        {
            GISModel objGISModel = new GISModel();
            try
            {
                var LatLngDetails = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLatLngOfCustomer(CurrentCity.Id);

                ViewBag.CurrentCityID = CurrentCity.Id;

                objGISModel.LocationTypeId = "Select a type";
                objGISModel.Latitude = LatLngDetails.FirstOrDefault().Latitude;
                objGISModel.Longitude = LatLngDetails.FirstOrDefault().Longitude;

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN Index method (Giscontroller)", ex);
            }
            //** Get the Lat/Lng coordinates of the customer



            return View(objGISModel);
            //return View();
        }

        /// <summary>
        /// Description: It is used to populate the various types of Assets available for financial revenue layer.
        /// Modified By: Sairam on July 27th 2014.        /// </summary>
        /// </summary>
        /// <param name="LayerID"></param>
        /// <returns></returns>
        public ActionResult GetAssetTypes(string LayerID)
        {
            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetTypes_FinancialRevenueLayer(LayerID, CurrentCity.Id);

            return Json(assets, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAssetTypes_HomePage(string LayerID)
        {
            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetTypes_HomePage(LayerID, CurrentCity.Id);

            return Json(assets, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// Description: It is used to populate the various types of Assets available for a given customer based on the layer selected.
        /// Modified By: Sairam on July 23rd 2014.
        /// </summary>
        /// <param name="LayerID"></param>
        /// <returns></returns>
        public ActionResult GetAssetTypes_Inventory(string LayerID)
        {
            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetTypes(LayerID, CurrentCity.Id);

            return Json(assets, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Description:This method is used to populate the Officer Names for the selected customer
        /// Modified By: Sairam on June 11th 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOfficerNames()
        {
            var officers = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerNames(CurrentCity.Id);
            return Json(officers, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetOfficerNames_List([DataSourceRequest] DataSourceRequest request)
        {
            var officers = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerNames(CurrentCity.Id);
            return Json(officers.ToDataSourceResult(request));
        }

        /// <summary>
        /// Description:This method is used to populate the Officer IDs for the selected customer
        /// Modified By: Sairam on June 12th 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOfficerIDS()
        {
            var officers = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerID(CurrentCity.Id);
            return Json(officers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to compute the no. of citations issued by an officer for the selected date.
        /// Modified By: Sairam on July 25th 2014
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        //public ActionResult GetCitationIssued(string startDate, string endDate, int? officerID)
        //{
        //    DateTime startTimeOutput = new DateTime();
        //    DateTime endTimeOutput = new DateTime();

        //    try
        //    {

        //        if (DateTime.TryParse(startDate, out startTimeOutput))
        //        {
        //            //** Valid start date 
        //        }

        //        if (DateTime.TryParse(endDate, out endTimeOutput))
        //        {
        //            //** Valid end date 
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetCitationIssued Method (GIS Reports - Citation)", ex);
        //    }


        //    var officers = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetNoOfCitationIssued(CurrentCity.Id, startTimeOutput, endTimeOutput, officerID);
        //    return Json(officers, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetCitationIssued(string startDate, string endDate, string officerID, int customerId)
        {
            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            //var officers = GetNoOfCitationIssued(customerId, startDate, endDate, officerID);
            var officers = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetNoOfCitationIssued(CurrentCity.Id, startDate, endDate, officerID);
            return Json(officers, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This method is used to populate the enabled demand zones for the given customer.
        /// Modified By: Sairam on July 14th 2014.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDemandZones()
        {

            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetDemandZones(CurrentCity.Id);

            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: It is used to populate the Asset Models for the selected Asset type in the financial revenue layer.
        /// Modified By: Sairam on 24th july 2014        /// </summary>
        /// </summary>
        /// <param name="assetTypeIs"></param>
        /// <returns></returns>
        public ActionResult GetAssetModels(int assetTypeIs)
        {

            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetModels(assetTypeIs, CurrentCity.Id);// GetAssetModels();

            return Json(assets, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Description:This method is used to populate the Status of Inventory
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAssetState()
        {

            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAssetState();


            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to populate the Status of Occupancy for the inventory layer
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOccupancyStatus()
        {

            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOccupancyStatus();


            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to populate the Compliance Status for the Inventory.
        /// Modified By: Sairam on May 21st 2014

        /// </summary>
        /// <returns></returns>
        public ActionResult GetComplianceStatus()
        {

            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetComplianceStatus();


            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description:This method is used to populate the Operational Status for the Asset Operational Layer.
        /// Modified By: Sairam on May 21st 2014

        /// </summary>
        /// <returns></returns>
        public ActionResult GetOperationalStatus()
        {

            var assets = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOperationalStatus();


            return Json(assets, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Description: This method is used to display the Asset IDs for the selected asset Type.
        /// Modified By: Sairam on June 3rd 2014
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="LayerID"></param>
        /// <param name="SearchBy"></param>
        /// <param name="TypedText"></param>
        /// <returns></returns>
        public ActionResult GetMasterAssetIds(string assetType, string LayerID, int SearchBy = 2, string TypedText = "")
        {
            int meterGroupId = -1;
            int sensorType = -1;
            int gateWayType = -1;
            int SpaceType = -1;
            List<GISModel> SSMGISModel = new List<GISModel>();
            List<GISModel> MeterGISModel = new List<GISModel>();
            List<GISModel> sensorTypeGISModel = new List<GISModel>();
            List<GISModel> gateWayTypeGISModel = new List<GISModel>();
            List<GISModel> SpaceTypeGISModel = new List<GISModel>();

            try
            {


                if (!string.IsNullOrEmpty(assetType))
                {
                    string[] array = assetType.Split(',');
                    meterGroupId = (array.Length > 0) ? Convert.ToInt32(array[0]) : -1;


                    if (meterGroupId == 0)
                    {
                        SSMGISModel = GetAllMetersTypes(0, SearchBy, TypedText);
                    }

                    if (meterGroupId == 1)
                    {
                        MeterGISModel = GetAllMetersTypes(1, SearchBy, TypedText);
                    }

                    if (assetType.Contains("10"))
                    {
                        // sensorTypeGISModel = GetAllSensorsTypes(10, SearchBy, TypedText);
                        sensorTypeGISModel = GetAllMetersTypes(10, SearchBy, TypedText);
                    }

                    if (assetType.Contains("13"))
                    {
                        // gateWayTypeGISModel = GetAllGatewaysTypes(13, SearchBy, TypedText);
                        gateWayTypeGISModel = GetAllMetersTypes(13, SearchBy, TypedText);
                    }

                    if (assetType.Contains("16"))
                    {
                        //SpaceTypeGISModel = GetAllSpaceTypes(20, SearchBy, TypedText);
                        SpaceTypeGISModel = GetAllMetersTypes(16, SearchBy, TypedText);
                    }

                    if (assetType.Contains("20"))
                    {
                        //SpaceTypeGISModel = GetAllSpaceTypes(20, CurrentCity.Id, SearchBy, TypedText);
                        SpaceTypeGISModel = GetAllMetersTypes(-1, SearchBy, TypedText);
                    }


                }
                else
                {//here getting all data.
                    if (LayerID == "2")
                    {
                        //  SpaceTypeGISModel = GetAllSpaceTypes(SpaceType, CurrentCity.Id, SearchBy, TypedText);
                        SpaceTypeGISModel = GetAllMetersTypes(-1, SearchBy, TypedText);
                    }
                    else
                    {
                        SSMGISModel = GetAllMetersTypes(meterGroupId, SearchBy, TypedText);
                        MeterGISModel = GetAllMetersTypes(meterGroupId, SearchBy, TypedText);
                        sensorTypeGISModel = GetAllMetersTypes(sensorType, SearchBy, TypedText);
                        gateWayTypeGISModel = GetAllMetersTypes(gateWayType, SearchBy, TypedText);
                        //  SpaceTypeGISModel = GetAllSpaceTypes(SpaceType, CurrentCity.Id, SearchBy, TypedText);
                        SpaceTypeGISModel = GetAllMetersTypes(-1, SearchBy, TypedText);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetMasterAssetIds Method (GIS Reports)", ex);
            }


            var result = SSMGISModel.Union(MeterGISModel).Union(sensorTypeGISModel).Union(gateWayTypeGISModel).Union(SpaceTypeGISModel);

            List<SelectListItem> lstAssets = new List<SelectListItem>();

            if (result != null)
            {
                foreach (var k in result)
                {
                    if (SearchBy == 1 && k.Text != null)
                    {
                        lstAssets.Add(new SelectListItem { Text = k.Text.ToString(), Value = k.Value.ToString() });

                    }
                    else if (SearchBy == 2 && k.Text != null)
                    {
                        lstAssets.Add(new SelectListItem { Text = k.Value.ToString(), Value = k.Value.ToString() });
                    }

                }
            }

            return Json(lstAssets, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Description:This method is used to populate the Meter groups for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="meterGroupId"></param>
        /// <param name="Searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllMetersTypes(int meterGroupId, int Searchby = 2, string typedtext = "")
        {
            return (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllMetersTypes(meterGroupId, CurrentCity.Id, Searchby, typedtext);
        }

        /// <summary>
        /// Description:This method is used to populate the Sensor Types for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="Searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllSensorsTypes(int sensorType, int Searchby = 2, string typedtext = "")
        {
            return (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllSensorsTypes(sensorType, Searchby, typedtext);
        }

        /// <summary>
        /// Description:This method is used to populate the Gateway Types for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="gateWayType"></param>
        /// <param name="Searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllGatewaysTypes(int gateWayType, int Searchby = 2, string typedtext = "")
        {

            return (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllGatewaysTypes(gateWayType, Searchby, typedtext);

        }

        /// <summary>
        /// Description:This method is used to populate the Parking Space Types for the typed text.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <param name="SpaceType"></param>
        /// <param name="Searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<GISModel> GetAllSpaceTypes(int SpaceType, int currentCity, int Searchby = 2, string typedtext = "")
        {

            return (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetAllSpaceTypes(SpaceType, currentCity, Searchby, typedtext);

        }

        /// <summary>
        /// Description:This method is used to populate the Locations such as area, zone and street.
        /// Modified By: Sairam on May 17th 2014
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLoacationTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Area", Value = "Area" });
            list.Add(new SelectListItem { Text = "Zone", Value = "Zone" });
            list.Add(new SelectListItem { Text = "Street", Value = "Street" });

            return Json(list, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Description:This method is used to populate the IDs for the given location types (Area/Zone/Street).
        /// Modified By: Sairam on May 19th 2014
        /// </summary>
        /// <param name="locationType"></param>
        /// <returns></returns>
        public ActionResult GetLocationTypeId(string locationType)
        {
            GISModel objGISModel = new GISModel();

            IEnumerable<GISModel> details = Enumerable.Empty<GISModel>();


            details = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetLocationTypeId(locationType, CurrentCity.Id);


            if (locationType == "Street" || locationType == "Suburb" || locationType == "Demand Area")
            {
                var allDetails = (from detail in details
                                  select new
                                  {
                                      Value = detail.Text

                                      //}).ToList();
                                  }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items

                return Json(allDetails, JsonRequestBehavior.AllowGet);
            }



            List<SelectListItem> lst = new List<SelectListItem>();


            if (details != null)
            {
                foreach (var k in details)
                {
                    //lst.Add(new SelectListItem { Text = k.Value.ToString(), Value = k.Value.ToString() });
                    lst.Add(new SelectListItem { Text = k.Text.ToString(), Value = k.Value.ToString() });
                }
            }

            return Json(lst, JsonRequestBehavior.AllowGet);
        }







        /// <summary>
        /// Description:This method is used to populate the Officer Last reported locations for the Officer Current Position GIS report.
        /// Modified By: Sairam on July 11th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        public ActionResult GetOfficerCurrentLocations([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {

            IEnumerable<GISOfficerLocationModel> lstCurrent = Enumerable.Empty<GISOfficerLocationModel>();

            int total = 0;
            string[] officerIDArr = officerID.Split(',');

            //** THis below list is important for converting IN operator to Linq while querying;

            List<string> ids = new List<string>();
            for (var i = 0; i < officerIDArr.Length; i++)
            {
                if (i == 0 && officerIDArr[0] == "All")
                {
                    ids.Add("-1");
                }
                else
                {
                    ids.Add(officerIDArr[i].Trim());
                }
                //ids.Add(Convert.ToInt32(officerIDArr[i]));
            }

            int officerIdStatus = (officerIDArr.Length > 0) ? Convert.ToInt32(officerIDArr[0]) : -1;

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                //** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }


            if (officerIdStatus != -1)
            {
                //** Other than 'ALL' option

                #region OfficerLocation

                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsOther(request, CurrentCity.Id, ids, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDateTime),
                                 activityDate = r.activityDateTime, //activityDateOff = Convert.ToString(r.activityDate),// r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude


                             };

                #endregion

            }
            else
            {
                //** 'ALL' option is chosen

                #region OfficerLocation
                //var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(CurrentCity.Id, startTimeOutput, endTimeOutput);
                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(request, CurrentCity.Id, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDateTime),
                                 activityDate = r.activityDateTime, //activityDateOff = Convert.ToString(r.activityDate),// r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }

            var finalresult = lstCurrent.OrderBy(x => x.officerID).ToList();

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            {
                Data = finalresult,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Description: This method is used to populate and display citations for the selected Officers in the grid for Citations GIS Layer
        /// Modified By: Sairam on 14th Aug 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        public ActionResult GetCitationForParkingOfficer([DataSourceRequest]DataSourceRequest request)
        {

            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "GISCitation");

            IEnumerable<GISCitationModel> lstCurrent = Enumerable.Empty<GISCitationModel>();

            int total = 0;

            var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCitationForParkingOfficer(request, CurrentCity.Id, out total);



            lstCurrent = (from r in lstCurrent_res
                          select new GISCitationModel
                          {
                              CustomerID = r.CustomerID,
                              officerID = r.officerID,
                              officerName = r.officerName,
                              IssueNo = r.IssueNo,
                              AssetID = r.AssetID,
                              MeterID = r.MeterID,
                              dateOnly = Convert.ToString(r.startDateTime.Value.ToShortDateString()),
                              startTime = Convert.ToString(r.startDateTime.Value.ToString("h:mm tt")),
                              endTime = Convert.ToString(r.endDateTime.Value.ToString("h:mm tt")),

                              Latitude = r.Latitude,
                              Longitude = r.Longitude
                          }).ToList();


            var finalresult = lstCurrent.ToList();

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            {
                Data = finalresult,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Description:This method is used to populate the grid with the route information (such as lat/lng, activity) travelled by the officer for the selected date
        /// Modified By: Sairam on July 15th 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="officerID"></param>
        /// <returns></returns>
        public ActionResult GetOfficerRouteDetails([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {

            IEnumerable<GISOfficerRouteModel> lstCurrent = Enumerable.Empty<GISOfficerRouteModel>();


            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();
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
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerRouteDetails Method (GIS Reports)", ex);
            }

            lstCurrent = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerRouteDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, officerID, request, out total);

            var finalresult = lstCurrent.ToList();

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            {
                Data = finalresult,
                Total = total,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public FileResult ExportToExcel_OfficerRoute([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            IEnumerable<GISOfficerRouteModel> lstCurrent = Enumerable.Empty<GISOfficerRouteModel>();


            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();
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
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerRouteDetails Method (GIS Reports)", ex);
            }


            lstCurrent = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerRouteDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, officerID, request, out total);


            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(lstCurrent, CurrentController, "GetOfficerRouteDetails", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "GIS_Officer_Route_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToCsv_OfficerRoute([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            IEnumerable<GISOfficerRouteModel> lstCurrent = Enumerable.Empty<GISOfficerRouteModel>();


            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();
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
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerRouteDetails Method (GIS Reports)", ex);
            }


            lstCurrent = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerRouteDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, officerID, request, out total);



            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(lstCurrent, CurrentController, "GetOfficerRouteDetails", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "GIS_Officer_Route_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToPdf_OfficerRoute([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            IEnumerable<GISOfficerRouteModel> lstCurrent = Enumerable.Empty<GISOfficerRouteModel>();


            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();
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
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetOfficerRouteDetails Method (GIS Reports)", ex);
            }


            lstCurrent = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerRouteDetails(CurrentCity.Id, startTimeOutput, endTimeOutput, officerID, request, out total);



            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(lstCurrent, CurrentController, "GetOfficerRouteDetails", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GIS_Officer_Route_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");


        }

        public FileResult ExportToExcel_Citation([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            var items = GetExportData_Citation(request);

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);
            var itemToRemove = filters.FirstOrDefault(r => r.Member == "OfficerID");
            var officerIDval = itemToRemove.Value;
            if (officerIDval == "-1")
            {
                filters.FirstOrDefault(r => r.Member == "OfficerID").Value = "All";
            }
            // if (itemToRemove != null) filters.Remove(itemToRemove);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetCitationForParkingOfficer", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "GIS_Citation_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToExcel_Meter([DataSourceRequest]DataSourceRequest request, string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs, string pageChosen)
        {
            //** Export options - Step 4

            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models
            int total = 0;
            IEnumerable<GISModel> items = null;

            long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
            int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

            int areaId = -1;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            int zoneId = -1;
            string street = string.Empty;
            string suburb = string.Empty;
            string demandArea = string.Empty;

            try
            {

                if (locationTypeName == "Area")
                {
                    // areaId = Convert.ToInt32(parallerLocationId);
                    areaNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Zone")
                {
                    //zoneId = Convert.ToInt32(parallerLocationId);
                    zoneNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeName == "Suburb")
                {
                    suburb = parallerLocationId;
                }
                else if (locationTypeName == "Demand Area")
                {
                    demandArea = parallerLocationId;
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert AssetStatus to int list type before invoking factory class
                string[] assetStatusArr = assetStatus.Split(',');
                List<int?> assetStatusIDs = new List<int?>();
                for (var i = 0; i < assetStatusArr.Length; i++)
                {
                    assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
                }

                //** Convert Occupancy Criteria to int list type before invoking factory class
                string[] specificCriteriaArr = specificCriteria.Split(',');
                List<int?> occupancyIDs = new List<int?>();
                for (var i = 0; i < specificCriteriaArr.Length; i++)
                {
                    occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
                }

                //** Convert Non-Compliance Criteria to int list type before invoking factory class
                string[] NonComplianceStatusArr = nonCompStatus.Split(',');
                List<int?> NonComplianceStatusIDs = new List<int?>();
                for (var i = 0; i < NonComplianceStatusArr.Length; i++)
                {
                    NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
                }

                //** Convert Opertional Status Criteria to int list type before invoking factory class
                string[] operationalCriteriaArr = specificCriteria.Split(',');
                List<int?> operationalIDs = new List<int?>();
                for (var i = 0; i < operationalCriteriaArr.Length; i++)
                {
                    operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
                }

                if (layerValue == "1")
                {
                    //** Inventory Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory_All(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                AssetId = Convert.ToString(r.AssetID),

                                //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,

                                MeterName = r.MeterName,
                                AssetStateDesc = r.AssetStateDesc,
                                Location = r.Location,
                                BayStart = r.BayStart,
                                BayEnd = r.BayEnd,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                MeterGroup = r.MeterGroup,
                            };

                }
                else if (layerValue == "2")
                {
                    //** Parking Space Layer;
                    if (complianceStatusIs == "All")
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {
                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc
                                };

                    }
                    else if (complianceStatusIs == "Compliant")
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_OnlyCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {

                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc

                                };
                    }
                    else
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_NonCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {

                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc

                                };
                    }


                }
                else
                {
                    //** Asset Operational Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = r.MeterGroup,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                // DemandZoneId = demandZone.DemandZoneId,
                                // DemandZoneDesc = demandZone.DemandZoneDesc,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                OperationalStatusId = r.OperationalStatusId,
                                OperationalStatusDesc = r.OperationalStatusDesc,
                                EventCode = r.EventCode,
                                EventDescVerbose = r.EventDescVerbose
                            };

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
            }

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);
            var output = (new ExportFactory()).GetExcelFileMemoryStream(items, CurrentController, "GetCustomerGridDetails_New", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "GIS_Meter_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        //public ActionResult updateSensorsOnDemand(string assetID, string Latitude, string Longitude)
        //{
        //    string result = string.Empty;
        //    int myAssetID = Convert.ToInt32(assetID);
        //    double lat = Convert.ToDouble(Latitude);
        //    double lng = Convert.ToDouble(Longitude);
        //    result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).updateSensorsOnDemand(myAssetID, lat, lng, CurrentCity.Id);
        //    return Content(result);
        //}


        //public ActionResult updateAllMarkersOnDemand(string assetID, string Latitude, string Longitude)
        //{
        //    string result = string.Empty;

        //    string[] assetIDArr = assetID.Split(',');
        //    List<int?> assetIDs = new List<int?>();
        //    for (var i = 0; i < assetIDArr.Length; i++)
        //    {
        //        assetIDs.Add(Convert.ToInt32(assetIDArr[i]));
        //    }

        //    string[] LatArr = Latitude.Split(',');
        //    List<double?> Lats = new List<double?>();
        //    for (var i = 0; i < LatArr.Length; i++)
        //    {
        //        Lats.Add(Convert.ToInt32(LatArr[i]));
        //    }

        //    string[] LngArr = Longitude.Split(',');
        //    List<double?> Lngs = new List<double?>();
        //    for (var i = 0; i < LngArr.Length; i++)
        //    {
        //        Lngs.Add(Convert.ToInt32(LngArr[i]));
        //    }

        //    result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).updateAllMarkersOnDemand(assetIDs, Lats, Lngs, CurrentCity.Id);
        //    return Content(result);
        //}

        //public ActionResult updateOnDemand(string assetID, string Latitude, string Longitude)
        //{
        //    string result = string.Empty;
        //    int myAssetID = Convert.ToInt32(assetID);
        //    double lat = Convert.ToDouble(Latitude);
        //    double lng = Convert.ToDouble(Longitude);
        //    result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).updateOnDemand(myAssetID, lat, lng, CurrentCity.Id);
        //    return Content(result);
        //}

        public FileResult ExportToCsv_Meter([DataSourceRequest]DataSourceRequest request, string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs, string pageChosen)
        {
            //** Export options - Step 4

            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models
            int total = 0;
            IEnumerable<GISModel> items = null;

            long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
            int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

            int areaId = -1;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            int zoneId = -1;
            string street = string.Empty;
            string suburb = string.Empty;
            string demandArea = string.Empty;

            try
            {

                if (locationTypeName == "Area")
                {
                    // areaId = Convert.ToInt32(parallerLocationId);
                    areaNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Zone")
                {
                    //zoneId = Convert.ToInt32(parallerLocationId);
                    zoneNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeName == "Suburb")
                {
                    suburb = parallerLocationId;
                }
                else if (locationTypeName == "Demand Area")
                {
                    demandArea = parallerLocationId;
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert AssetStatus to int list type before invoking factory class
                string[] assetStatusArr = assetStatus.Split(',');
                List<int?> assetStatusIDs = new List<int?>();
                for (var i = 0; i < assetStatusArr.Length; i++)
                {
                    assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
                }

                //** Convert Occupancy Criteria to int list type before invoking factory class
                string[] specificCriteriaArr = specificCriteria.Split(',');
                List<int?> occupancyIDs = new List<int?>();
                for (var i = 0; i < specificCriteriaArr.Length; i++)
                {
                    occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
                }

                //** Convert Non-Compliance Criteria to int list type before invoking factory class
                string[] NonComplianceStatusArr = nonCompStatus.Split(',');
                List<int?> NonComplianceStatusIDs = new List<int?>();
                for (var i = 0; i < NonComplianceStatusArr.Length; i++)
                {
                    NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
                }

                //** Convert Opertional Status Criteria to int list type before invoking factory class
                string[] operationalCriteriaArr = specificCriteria.Split(',');
                List<int?> operationalIDs = new List<int?>();
                for (var i = 0; i < operationalCriteriaArr.Length; i++)
                {
                    operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
                }

                if (layerValue == "1")
                {
                    //** Inventory Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory_All(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                AssetId = Convert.ToString(r.AssetID),

                                //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,

                                MeterName = r.MeterName,
                                AssetStateDesc = r.AssetStateDesc,
                                Location = r.Location,
                                BayStart = r.BayStart,
                                BayEnd = r.BayEnd,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                MeterGroup = r.MeterGroup,
                            };

                }
                else if (layerValue == "2")
                {
                    //** Parking Space Layer;
                    if (complianceStatusIs == "All")
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {
                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc
                                };

                    }
                    else if (complianceStatusIs == "Compliant")
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_OnlyCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {

                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc

                                };
                    }
                    else
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_NonCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {

                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc

                                };
                    }


                }
                else
                {
                    //** Asset Operational Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = r.MeterGroup,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                // DemandZoneId = demandZone.DemandZoneId,
                                // DemandZoneDesc = demandZone.DemandZoneDesc,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                OperationalStatusId = r.OperationalStatusId,
                                OperationalStatusDesc = r.OperationalStatusDesc,
                                EventCode = r.EventCode,
                                EventDescVerbose = r.EventDescVerbose
                            };

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
            }
            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);
            //var itemToRemove = filters.FirstOrDefault(r => r.Member == "OfficerID");
            //var officerIDval = itemToRemove.Value;
            //if (officerIDval == "-1")
            //{
            //    filters.FirstOrDefault(r => r.Member == "OfficerID").Value = "All";
            //}
            // if (itemToRemove != null) filters.Remove(itemToRemove);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetCustomerGridDetails_New", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "GIS_Meter_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToPdf_Meter([DataSourceRequest]DataSourceRequest request, string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs, string pageChosen)
        {
            //** Export options - Step 4

            //now save the sort filters
            if (request.Sorts.Count > 0)
                SetSavedSortValues(request.Sorts, "Assets");

            //get models
            int total = 0;
            IEnumerable<GISModel> items = null;

            long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
            int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

            int areaId = -1;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            int zoneId = -1;
            string street = string.Empty;
            string suburb = string.Empty;
            string demandArea = string.Empty;

            try
            {

                if (locationTypeName == "Area")
                {
                    // areaId = Convert.ToInt32(parallerLocationId);
                    areaNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Zone")
                {
                    //zoneId = Convert.ToInt32(parallerLocationId);
                    zoneNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeName == "Suburb")
                {
                    suburb = parallerLocationId;
                }
                else if (locationTypeName == "Demand Area")
                {
                    demandArea = parallerLocationId;
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert AssetStatus to int list type before invoking factory class
                string[] assetStatusArr = assetStatus.Split(',');
                List<int?> assetStatusIDs = new List<int?>();
                for (var i = 0; i < assetStatusArr.Length; i++)
                {
                    assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
                }

                //** Convert Occupancy Criteria to int list type before invoking factory class
                string[] specificCriteriaArr = specificCriteria.Split(',');
                List<int?> occupancyIDs = new List<int?>();
                for (var i = 0; i < specificCriteriaArr.Length; i++)
                {
                    occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
                }

                //** Convert Non-Compliance Criteria to int list type before invoking factory class
                string[] NonComplianceStatusArr = nonCompStatus.Split(',');
                List<int?> NonComplianceStatusIDs = new List<int?>();
                for (var i = 0; i < NonComplianceStatusArr.Length; i++)
                {
                    NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
                }

                //** Convert Opertional Status Criteria to int list type before invoking factory class
                string[] operationalCriteriaArr = specificCriteria.Split(',');
                List<int?> operationalIDs = new List<int?>();
                for (var i = 0; i < operationalCriteriaArr.Length; i++)
                {
                    operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
                }

                if (layerValue == "1")
                {
                    //** Inventory Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory_All(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                AssetId = Convert.ToString(r.AssetID),

                                //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,

                                MeterName = r.MeterName,
                                AssetStateDesc = r.AssetStateDesc,
                                Location = r.Location,
                                BayStart = r.BayStart,
                                BayEnd = r.BayEnd,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                MeterGroup = r.MeterGroup,
                            };

                }
                else if (layerValue == "2")
                {
                    //** Parking Space Layer;
                    if (complianceStatusIs == "All")
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {
                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc
                                };

                    }
                    else if (complianceStatusIs == "Compliant")
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_OnlyCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {

                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc

                                };
                    }
                    else
                    {
                        var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_NonCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                        items = from r in items_temp
                                select new GISModel
                                {

                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc

                                };
                    }


                }
                else
                {
                    //** Asset Operational Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = r.MeterGroup,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                // DemandZoneId = demandZone.DemandZoneId,
                                // DemandZoneDesc = demandZone.DemandZoneDesc,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                OperationalStatusId = r.OperationalStatusId,
                                OperationalStatusDesc = r.OperationalStatusDesc,
                                EventCode = r.EventCode,
                                EventDescVerbose = r.EventDescVerbose
                            };

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
            }
            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetCustomerGridDetails_New", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GIS_Meter_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");

        }



        public FileResult ExportToExcel_CurrentLocation([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            IEnumerable<GISOfficerLocationModel> lstCurrent = Enumerable.Empty<GISOfficerLocationModel>();

            int total = 0;
            string[] officerIDArr = officerID.Split(',');

            //** THis below list is important for converting IN operator to Linq while querying;

            List<string> ids = new List<string>();

            for (var i = 0; i < officerIDArr.Length; i++)
            {
                if (i == 0 && officerIDArr[0] == "All")
                {
                    ids.Add("-1");
                }
                else
                {
                    ids.Add(officerIDArr[i].Trim());
                }
                //ids.Add(Convert.ToInt32(officerIDArr[i]));
            }

            int officerIdStatus = (officerIDArr.Length > 0) ? Convert.ToInt32(officerIDArr[0]) : -1;

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                //** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }


            if (officerIdStatus != -1)
            {
                //** Other than 'ALL' option

                #region OfficerLocation

                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsOther(request, CurrentCity.Id, ids, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDateTime), // DateTime.TryParse("12-12-3020"), // Convert.ToDateTime(r.activityDate),// r.activityDate,
                                 activityDate = r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }
            else
            {
                //** 'ALL' option is chosen

                #region OfficerLocation
                //var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(CurrentCity.Id, startTimeOutput, endTimeOutput);
                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(request, CurrentCity.Id, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDate),// r.activityDate,
                                 activityDate = r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }


            //  var items = GetExportData_CurrentLocation(request);

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(lstCurrent, CurrentController, "GetOfficerCurrentLocations", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "GIS_Officer_Location_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToCsv_CurrentLocation([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            IEnumerable<GISOfficerLocationModel> lstCurrent = Enumerable.Empty<GISOfficerLocationModel>();

            int total = 0;
            string[] officerIDArr = officerID.Split(',');

            //** THis below list is important for converting IN operator to Linq while querying;

            List<string> ids = new List<string>();
            for (var i = 0; i < officerIDArr.Length; i++)
            {
                if (i == 0 && officerIDArr[0] == "All")
                {
                    ids.Add("-1");
                }
                else
                {
                    ids.Add(officerIDArr[i].Trim());
                }
                //ids.Add(Convert.ToInt32(officerIDArr[i]));
            }

            int officerIdStatus = (officerIDArr.Length > 0) ? Convert.ToInt32(officerIDArr[0]) : -1;

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                //** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }


            if (officerIdStatus != -1)
            {
                //** Other than 'ALL' option

                #region OfficerLocation

                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsOther(request, CurrentCity.Id, ids, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDate), // DateTime.TryParse("12-12-3020"), // Convert.ToDateTime(r.activityDate),// r.activityDate,
                                 activityDate = r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }
            else
            {
                //** 'ALL' option is chosen

                #region OfficerLocation
                //var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(CurrentCity.Id, startTimeOutput, endTimeOutput);
                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(request, CurrentCity.Id, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDate),// r.activityDate,
                                 activityDate = r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }


            //  var items = GetExportData_CurrentLocation(request);

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(lstCurrent, CurrentController, "GetOfficerCurrentLocations", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "GIS_Officer_Location_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdf_CurrentLocation([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            IEnumerable<GISOfficerLocationModel> lstCurrent = Enumerable.Empty<GISOfficerLocationModel>();

            int total = 0;
            string[] officerIDArr = officerID.Split(',');

            //** THis below list is important for converting IN operator to Linq while querying;

            List<string> ids = new List<string>();
            for (var i = 0; i < officerIDArr.Length; i++)
            {
                if (i == 0 && officerIDArr[0] == "All")
                {
                    ids.Add("-1");
                }
                else
                {
                    ids.Add(officerIDArr[i].Trim());
                }
                //ids.Add(Convert.ToInt32(officerIDArr[i]));
            }

            int officerIdStatus = (officerIDArr.Length > 0) ? Convert.ToInt32(officerIDArr[0]) : -1;

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                //** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }


            if (officerIdStatus != -1)
            {
                //** Other than 'ALL' option

                #region OfficerLocation

                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsOther(request, CurrentCity.Id, ids, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDate), // DateTime.TryParse("12-12-3020"), // Convert.ToDateTime(r.activityDate),// r.activityDate,
                                 activityDate = r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }
            else
            {
                //** 'ALL' option is chosen

                #region OfficerLocation
                //var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(CurrentCity.Id, startTimeOutput, endTimeOutput);
                var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetOfficerCurrentLocationsALL(request, CurrentCity.Id, out total);


                lstCurrent = from r in lstCurrent_res
                             select new GISOfficerLocationModel
                             {
                                 CustomerID = r.CustomerID,
                                 officerName = r.officerName,
                                 officerID = r.officerID,
                                 activityDateOff = Convert.ToString(r.activityDate),// r.activityDate,
                                 activityDate = r.activityDate,
                                 officerActivity = r.officerActivity,
                                 Latitude = r.Latitude,
                                 Longitude = r.Longitude

                             };

                #endregion

            }


            //  var items = GetExportData_CurrentLocation(request);

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(lstCurrent, CurrentController, "GetOfficerCurrentLocations", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GIS_Officer_Location_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }

        public FileResult ExportToCsv_Citation([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {
            //** Export options - Step 4

            var items = GetExportData_Citation(request);
            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);
            var itemToRemove = filters.FirstOrDefault(r => r.Member == "OfficerID");
            var officerIDval = itemToRemove.Value;
            if (officerIDval == "-1")
            {
                filters.FirstOrDefault(r => r.Member == "OfficerID").Value = "All";
            }
            // if (itemToRemove != null) filters.Remove(itemToRemove);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(items, CurrentController, "GetCitationForParkingOfficer", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "GIS_Citation_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");
        }

        public FileResult ExportToPdf_Citation([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string officerID)
        {

            //** Export options - Step 4

            var items = GetExportData_Citation(request);
            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);
            var itemToRemove = filters.FirstOrDefault(r => r.Member == "OfficerID");
            var officerIDval = itemToRemove.Value;
            if (officerIDval == "-1")
            {
                filters.FirstOrDefault(r => r.Member == "OfficerID").Value = "All";
            }
            // if (itemToRemove != null) filters.Remove(itemToRemove);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(items, CurrentController, "GetCitationForParkingOfficer", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GIS_Citation_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");
        }



        public IEnumerable<GISCitationModel> GetExportData_Citation([DataSourceRequest] DataSourceRequest request)
        {

            int total = 0;
            var lstCurrent_res = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetCitationForParkingOfficer(request, CurrentCity.Id, out total);

            IEnumerable<GISCitationModel> lstCurrent = Enumerable.Empty<GISCitationModel>();

            lstCurrent = (from r in lstCurrent_res
                          select new GISCitationModel
                          {
                              CustomerID = r.CustomerID,
                              officerID = r.officerID,
                              officerName = r.officerName,
                              IssueNo = r.IssueNo,
                              AssetID = r.AssetID,
                              MeterID = r.MeterID,
                              dateOnly = Convert.ToString(r.startDateTime.Value.ToShortDateString()),
                              startTime = Convert.ToString(r.startDateTime.Value.ToString("h:mm tt")),
                              endTime = Convert.ToString(r.endDateTime.Value.ToString("h:mm tt")),

                              Latitude = r.Latitude,
                              Longitude = r.Longitude
                          }).ToList();

            return lstCurrent;

        }

        /// <summary>
        /// Description: This method is used to populate the grid with Financial Revenue details for the given search criteria.
        /// Modified By: Sairam on July 20th 2014.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="assetType"></param>
        /// <param name="assetModel"></param>
        /// <param name="assetId"></param>
        /// <param name="locationTypeId"></param>
        /// <param name="parallerLocationId"></param>
        /// <param name="revCategory"></param>
        /// <param name="revSource"></param>
        /// <param name="demZone"></param>
        /// <param name="highRev"></param>
        /// <param name="medRev"></param>
        /// <param name="lowRev"></param>
        /// <returns></returns>
        public ActionResult GetRevenueGridDetails([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string assetType, string assetModel, string assetId, string locationTypeId, string parallerLocationId, string revCategory, string revSource, string demZone, string highRev, string medRev, string lowRev)
        {

            //** First clear the IEnumerable model instances;
            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            int total = 0;
            try
            {
                //** Initialize and Convert the required parameters to their respective data types
                long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
                //int areaId = -1;
                //int zoneId = -1;
                string areaNameIs = string.Empty;
                string zoneNameIs = string.Empty;
                string street = string.Empty;
                string suburb = string.Empty;

                //** Convert high/med/low to decimal type
                double highVal = Convert.ToDouble(highRev);
                double medVal = Convert.ToDouble(medRev);
                double lowVal = Convert.ToDouble(lowRev);

                //** Check the location type and assign the corresponding location IDs / names
                if (locationTypeId == "Area")
                {
                    areaNameIs = parallerLocationId.ToString().Trim();
                }
                else if (locationTypeId == "Zone")
                {
                    zoneNameIs = parallerLocationId.ToString().Trim();
                }
                else if (locationTypeId == "Street")
                {
                    street = parallerLocationId.ToString().Trim();
                }
                else if (locationTypeId == "Suburb")
                {
                    suburb = parallerLocationId.ToString().Trim();
                }

                //** Conversion of string representation of date to DateTime format
                DateTime tranStartDate;
                DateTime tranEndDate;

                if (DateTime.TryParse(startDate, out tranStartDate))
                {
                    //** Valid start date 
                }

                // DateTime tranEndDate = tranStartDate.AddDays(1);
                if (DateTime.TryParse(endDate, out tranEndDate))
                {
                    //** Valid start date 
                }

                //** Check if Start and End dates are SAME
                if (tranStartDate == tranEndDate)
                {
                    tranEndDate = tranStartDate.AddDays(1);
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert revCategory to int list type before invoking factory class
                string[] revCategoryArr = revCategory.Split(',');
                List<int> revCategoryIDS = new List<int>();
                for (var i = 0; i < revCategoryArr.Length; i++)
                {
                    revCategoryIDS.Add(Convert.ToInt32(revCategoryArr[i]));
                }

                //** Convert revSource to int list type before invoking factory class
                string[] revSourceArr = revSource.Split(',');
                List<int?> revSourceIDS = new List<int?>();
                for (var i = 0; i < revSourceArr.Length; i++)
                {
                    revSourceIDS.Add(Convert.ToInt32(revSourceArr[i]));
                }



                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Revenue(CurrentCity.Id, tranStartDate, tranEndDate, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, revCategoryIDS, revSourceIDS, demandZoneIDs, highVal, medVal, lowVal, request, out total);

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueGridDetails Method (GIS Reports - Financial Revenue)", ex);
            }

            var finalresult = result.ToList();

            DataSourceResult Fresult = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            {
                Data = finalresult,
                Total = total,
            };
            return Json(Fresult, JsonRequestBehavior.AllowGet);


        }

        public FileResult ExportToExcel_FinRev([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string assetType, string assetModel, string assetId, string locationTypeId, string parallerLocationId, string revCategory, string revSource, string demZone, string highRev, string medRev, string lowRev)
        {

            //******************

            //** First clear the IEnumerable model instances;
            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            int total = 0;
            try
            {
                //** Initialize and Convert the required parameters to their respective data types
                long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
                int areaId = -1;
                int zoneId = -1;
                string areaNameIs = string.Empty;
                string zoneNameIs = string.Empty;
                string street = string.Empty;
                string suburb = string.Empty;

                //** Convert high/med/low to decimal type
                double highVal = Convert.ToDouble(highRev);
                double medVal = Convert.ToDouble(medRev);
                double lowVal = Convert.ToDouble(lowRev);

                //** Check the location type and assign the corresponding location IDs / names
                if (locationTypeId == "Area")
                {
                    areaId = Convert.ToInt32(parallerLocationId);
                }
                else if (locationTypeId == "Zone")
                {
                    zoneId = Convert.ToInt32(parallerLocationId);
                }
                else if (locationTypeId == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeId == "Suburb")
                {
                    suburb = parallerLocationId;
                }

                //** Conversion of string representation of date to DateTime format
                DateTime tranStartDate;
                DateTime tranEndDate;

                if (DateTime.TryParse(startDate, out tranStartDate))
                {
                    //** Valid start date 
                }

                // DateTime tranEndDate = tranStartDate.AddDays(1);
                if (DateTime.TryParse(endDate, out tranEndDate))
                {
                    //** Valid start date 
                }

                //** Check if Start and End dates are SAME
                if (tranStartDate == tranEndDate)
                {
                    tranEndDate = tranStartDate.AddDays(1);
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert revCategory to int list type before invoking factory class
                string[] revCategoryArr = revCategory.Split(',');
                List<int> revCategoryIDS = new List<int>();
                for (var i = 0; i < revCategoryArr.Length; i++)
                {
                    revCategoryIDS.Add(Convert.ToInt32(revCategoryArr[i]));
                }

                //** Convert revSource to int list type before invoking factory class
                string[] revSourceArr = revSource.Split(',');
                List<int?> revSourceIDS = new List<int?>();
                for (var i = 0; i < revSourceArr.Length; i++)
                {
                    revSourceIDS.Add(Convert.ToInt32(revSourceArr[i]));
                }



                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Revenue(CurrentCity.Id, tranStartDate, tranEndDate, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, revCategoryIDS, revSourceIDS, demandZoneIDs, highVal, medVal, lowVal, request, out total);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueGridDetails Method (GIS Reports - Financial Revenue)", ex);
            }

            //***************

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetExcelFileMemoryStream(result, CurrentController, "GetRevenueGridDetails", CurrentCity.Id, filters);
            return File(output.ToArray(),   //The binary data of the XLS file
              "application/vnd.ms-excel", //MIME type of Excel files
              "GIS_Financial_Revenue_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".xls");     //Suggested file name in the "Save as" dialog which will be displayed to the end user
        }

        public FileResult ExportToCsv_FinRev([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string assetType, string assetModel, string assetId, string locationTypeId, string parallerLocationId, string revCategory, string revSource, string demZone, string highRev, string medRev, string lowRev)
        {

            //******************

            //** First clear the IEnumerable model instances;
            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            int total = 0;
            try
            {
                //** Initialize and Convert the required parameters to their respective data types
                long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
                int areaId = -1;
                int zoneId = -1;
                string areaNameIs = string.Empty;
                string zoneNameIs = string.Empty;
                string street = string.Empty;
                string suburb = string.Empty;

                //** Convert high/med/low to decimal type
                double highVal = Convert.ToDouble(highRev);
                double medVal = Convert.ToDouble(medRev);
                double lowVal = Convert.ToDouble(lowRev);

                //** Check the location type and assign the corresponding location IDs / names
                if (locationTypeId == "Area")
                {
                    areaId = Convert.ToInt32(parallerLocationId);
                }
                else if (locationTypeId == "Zone")
                {
                    zoneId = Convert.ToInt32(parallerLocationId);
                }
                else if (locationTypeId == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeId == "Suburb")
                {
                    suburb = parallerLocationId;
                }

                //** Conversion of string representation of date to DateTime format
                DateTime tranStartDate;
                DateTime tranEndDate;

                if (DateTime.TryParse(startDate, out tranStartDate))
                {
                    //** Valid start date 
                }

                // DateTime tranEndDate = tranStartDate.AddDays(1);
                if (DateTime.TryParse(endDate, out tranEndDate))
                {
                    //** Valid start date 
                }

                //** Check if Start and End dates are SAME
                if (tranStartDate == tranEndDate)
                {
                    tranEndDate = tranStartDate.AddDays(1);
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert revCategory to int list type before invoking factory class
                string[] revCategoryArr = revCategory.Split(',');
                List<int> revCategoryIDS = new List<int>();
                for (var i = 0; i < revCategoryArr.Length; i++)
                {
                    revCategoryIDS.Add(Convert.ToInt32(revCategoryArr[i]));
                }

                //** Convert revSource to int list type before invoking factory class
                string[] revSourceArr = revSource.Split(',');
                List<int?> revSourceIDS = new List<int?>();
                for (var i = 0; i < revSourceArr.Length; i++)
                {
                    revSourceIDS.Add(Convert.ToInt32(revSourceArr[i]));
                }



                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Revenue(CurrentCity.Id, tranStartDate, tranEndDate, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, revCategoryIDS, revSourceIDS, demandZoneIDs, highVal, medVal, lowVal, request, out total);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueGridDetails Method (GIS Reports - Financial Revenue)", ex);
            }

            //***************

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetCsvFileMemoryStream(result, CurrentController, "GetRevenueGridDetails", CurrentCity.Id);
            return File(output, "text/comma-separated-values", "GIS_Financial_Revenue_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".csv");

        }

        public FileResult ExportToPdf_FinRev([DataSourceRequest]DataSourceRequest request, string startDate, string endDate, string assetType, string assetModel, string assetId, string locationTypeId, string parallerLocationId, string revCategory, string revSource, string demZone, string highRev, string medRev, string lowRev)
        {

            //******************

            //** First clear the IEnumerable model instances;
            IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();
            int total = 0;
            try
            {
                //** Initialize and Convert the required parameters to their respective data types
                long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
                int areaId = -1;
                int zoneId = -1;
                string areaNameIs = string.Empty;
                string zoneNameIs = string.Empty;
                string street = string.Empty;
                string suburb = string.Empty;

                //** Convert high/med/low to decimal type
                double highVal = Convert.ToDouble(highRev);
                double medVal = Convert.ToDouble(medRev);
                double lowVal = Convert.ToDouble(lowRev);

                //** Check the location type and assign the corresponding location IDs / names
                if (locationTypeId == "Area")
                {
                    areaId = Convert.ToInt32(parallerLocationId);
                }
                else if (locationTypeId == "Zone")
                {
                    zoneId = Convert.ToInt32(parallerLocationId);
                }
                else if (locationTypeId == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeId == "Suburb")
                {
                    suburb = parallerLocationId;
                }

                //** Conversion of string representation of date to DateTime format
                DateTime tranStartDate;
                DateTime tranEndDate;

                if (DateTime.TryParse(startDate, out tranStartDate))
                {
                    //** Valid start date 
                }

                // DateTime tranEndDate = tranStartDate.AddDays(1);
                if (DateTime.TryParse(endDate, out tranEndDate))
                {
                    //** Valid start date 
                }

                //** Check if Start and End dates are SAME
                if (tranStartDate == tranEndDate)
                {
                    tranEndDate = tranStartDate.AddDays(1);
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert revCategory to int list type before invoking factory class
                string[] revCategoryArr = revCategory.Split(',');
                List<int> revCategoryIDS = new List<int>();
                for (var i = 0; i < revCategoryArr.Length; i++)
                {
                    revCategoryIDS.Add(Convert.ToInt32(revCategoryArr[i]));
                }

                //** Convert revSource to int list type before invoking factory class
                string[] revSourceArr = revSource.Split(',');
                List<int?> revSourceIDS = new List<int?>();
                for (var i = 0; i < revSourceArr.Length; i++)
                {
                    revSourceIDS.Add(Convert.ToInt32(revSourceArr[i]));
                }



                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Revenue(CurrentCity.Id, tranStartDate, tranEndDate, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, revCategoryIDS, revSourceIDS, demandZoneIDs, highVal, medVal, lowVal, request, out total);
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetRevenueGridDetails Method (GIS Reports - Financial Revenue)", ex);
            }

            //***************

            var filters = new List<Kendo.Mvc.FilterDescriptor>();
            filters.Add(new FilterDescriptor { Member = " ", Value = " " });
            filters = GetFilterDescriptors(request.Filters, filters);

            var output = (new ExportFactory()).GetPDFFileMemoryStream(result, CurrentController, "GetRevenueGridDetails", CurrentCity.Id, filters, 1);

            // send the memory stream as File
            return File(output.ToArray(), "application/pdf", "GIS_Financial_Revenue_Summary_" + FormatHelper.FormatDateTime(DateTime.Now) + ".pdf");



        }
        /// <summary>
        /// Description:This method is used to populate the Grid in the UI for all the 3 layers of Inventory for the selected search criteria
        /// Modified By: Sairam on May 21st 2014
        /// </summary>
        /// <param name="request"></param>
        /// <param name="assetType"></param>
        /// <param name="assetId"></param>
        /// <param name="locationTypeId"></param>
        /// <param name="locationTypeName"></param>
        /// <param name="parallerLocationId"></param>
        /// <param name="assetStatus"></param>
        /// <param name="demZone"></param>
        /// <param name="specificCriteria"></param>
        /// <param name="layerValue"></param>
        /// <param name="assetModel"></param>
        /// <param name="nonCompStatus"></param>
        /// <returns></returns>
        //public ActionResult GetCustomerGridDetails([DataSourceRequest]DataSourceRequest request, string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs)
        //{

        //    //** First clear the IEnumerable model instances;
        //    IEnumerable<GISModel> result = Enumerable.Empty<GISModel>();  

        //    long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
        //    int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

        //    int areaId = -1;
        //    string areaNameIs = string.Empty;
        //    string zoneNameIs = string.Empty;
        //    int zoneId = -1;
        //    string street = string.Empty;
        //    string suburb = string.Empty;
        //    string demandArea = string.Empty;

        //    try
        //    {

        //        if (locationTypeName == "Area")
        //        {
        //           // areaId = Convert.ToInt32(parallerLocationId);
        //            areaNameIs = parallerLocationId;
        //        }
        //        else if (locationTypeName == "Zone")
        //        {
        //            //zoneId = Convert.ToInt32(parallerLocationId);
        //            zoneNameIs = parallerLocationId;
        //        }
        //        else if (locationTypeName == "Street")
        //        {
        //            street = parallerLocationId;
        //        }
        //        else if (locationTypeName == "Suburb")
        //        {
        //            suburb = parallerLocationId;
        //        }
        //        else if (locationTypeName == "Demand Area")
        //        {
        //            demandArea = parallerLocationId;
        //        }


        //        //** Convert AssetModel to int list type before invoking factory class
        //        string[] assetModelArr = assetModel.Split(',');
        //        List<int?> assetModelIDs = new List<int?>();
        //        if (assetModel == "")
        //        {
        //            //** Asset Model dd is disabled
        //        }
        //        else
        //        {
        //            for (var i = 0; i < assetModelArr.Length; i++)
        //            {
        //                assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
        //            }
        //        }


        //        //** Convert AssetType to int list type before invoking factory class
        //        string[] assetTypeArr = assetType.Split(',');
        //        List<int?> assetTypeIDs = new List<int?>();
        //        for (var i = 0; i < assetTypeArr.Length; i++)
        //        {
        //            assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
        //        }

        //        //** Convert Demandzone to int list type before invoking factory class
        //        string[] demArr = demZone.Split(',');
        //        List<int?> demandZoneIDs = new List<int?>();
        //        for (var i = 0; i < demArr.Length; i++)
        //        {
        //            demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
        //        }

        //        //** Convert AssetStatus to int list type before invoking factory class
        //        string[] assetStatusArr = assetStatus.Split(',');
        //        List<int?> assetStatusIDs = new List<int?>();
        //        for (var i = 0; i < assetStatusArr.Length; i++)
        //        {
        //            assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
        //        }

        //        //** Convert Occupancy Criteria to int list type before invoking factory class
        //        string[] specificCriteriaArr = specificCriteria.Split(',');
        //        List<int?> occupancyIDs = new List<int?>();
        //        for (var i = 0; i < specificCriteriaArr.Length; i++)
        //        {
        //            occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
        //        }

        //        //** Convert Non-Compliance Criteria to int list type before invoking factory class
        //        string[] NonComplianceStatusArr = nonCompStatus.Split(',');
        //        List<int?> NonComplianceStatusIDs = new List<int?>();
        //        for (var i = 0; i < NonComplianceStatusArr.Length; i++)
        //        {
        //            NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
        //        }

        //        //** Convert Opertional Status Criteria to int list type before invoking factory class
        //        string[] operationalCriteriaArr = specificCriteria.Split(',');
        //        List<int?> operationalIDs = new List<int?>();
        //        for (var i = 0; i < operationalCriteriaArr.Length; i++)
        //        {
        //            operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
        //        }

        //        if (layerValue == "1")
        //        {
        //            //** Inventory Layer;
        //            //result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaId, zoneId, street, suburb, demandZoneIDs, assetStatusIDs, layervalue);
        //            result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue);
        //        }
        //        else if (layerValue == "2")
        //        {
        //            //** Parking Space Layer;
        //            if (complianceStatusIs == "All")
        //            {
        //                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs);
        //            }
        //            else if (complianceStatusIs == "Compliant")
        //            {
        //                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_OnlyCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs);
        //            }
        //            else
        //            {
        //                result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_NonCompliant(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs);
        //            }

        //        }
        //        else
        //        {
        //            //** Asset Operational Layer;
        //            result = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs);

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
        //    } 


        //    DataSourceResult finalResult = result.ToDataSourceResult(request);

        //   return new LargeJsonResult() { Data = finalResult, MaxJsonLength = int.MaxValue };

        //}



        public ActionResult GetCustomerGridDetails_New_MapAlone(string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs, string pageChosen)
        {


            int total = 0;
            IEnumerable<GISModel> items = null;

            long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
            int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

            int areaId = -1;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            int zoneId = -1;
            string street = string.Empty;
            string suburb = string.Empty;
            string demandArea = string.Empty;

            try
            {

                if (locationTypeName == "Area")
                {
                    // areaId = Convert.ToInt32(parallerLocationId);
                    areaNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Zone")
                {
                    //zoneId = Convert.ToInt32(parallerLocationId);
                    zoneNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeName == "Suburb")
                {
                    suburb = parallerLocationId;
                }
                else if (locationTypeName == "Demand Area")
                {
                    demandArea = parallerLocationId;
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert AssetStatus to int list type before invoking factory class
                string[] assetStatusArr = assetStatus.Split(',');
                List<int?> assetStatusIDs = new List<int?>();
                for (var i = 0; i < assetStatusArr.Length; i++)
                {
                    assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
                }

                //** Convert Occupancy Criteria to int list type before invoking factory class
                string[] specificCriteriaArr = specificCriteria.Split(',');
                List<int?> occupancyIDs = new List<int?>();
                for (var i = 0; i < specificCriteriaArr.Length; i++)
                {
                    occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
                }

                //** Convert Non-Compliance Criteria to int list type before invoking factory class
                string[] NonComplianceStatusArr = nonCompStatus.Split(',');
                List<int?> NonComplianceStatusIDs = new List<int?>();
                for (var i = 0; i < NonComplianceStatusArr.Length; i++)
                {
                    NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
                }

                //** Convert Opertional Status Criteria to int list type before invoking factory class
                string[] operationalCriteriaArr = specificCriteria.Split(',');
                List<int?> operationalIDs = new List<int?>();
                for (var i = 0; i < operationalCriteriaArr.Length; i++)
                {
                    operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
                }

                if (layerValue == "1")
                {
                    //** Inventory Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory_All_MapAlone(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                AssetId = Convert.ToString(r.AssetID),

                                //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,

                                MeterName = r.MeterName,
                                AssetStateDesc = r.AssetStateDesc,
                                Location = r.Location,
                                BayStart = r.BayStart,
                                BayEnd = r.BayEnd,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                MeterGroup = r.MeterGroup,
                            };
                    //return Json(items, JsonRequestBehavior.AllowGet);
                    return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };

                }
                else if (layerValue == "2")
                {
                    //** Parking Space Layer;
                    var items_raw = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_MapAlone(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, pageChosen);
                    items = from r in items_raw
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = 20,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                MeterID = r.MeterID,
                                BayID = r.BayID,
                                HasSensor = r.HasSensor,
                                PaymentType = r.PaymentType,
                                CurrentMeterTime = r.CurrentMeterTime,
                                NonSensorEventTime = r.NonSensorEventTime,
                                PaymentTime = r.PaymentTime,
                                BayNumber = r.BayNumber,
                                ViolationType = r.ViolationType,
                                amountInCent = r.amountInCent,
                                endDateTimeMob = r.endDateTimeMob,
                                startDateTimeMob = r.startDateTimeMob,

                                NonCommStatusDesc = r.NonCommStatusDesc, //** added on May 30th 2016 for finding NonCommSensor

                                OccupancyStatusID = r.OccupancyStatusID,
                                OccupancyStatusDesc = r.OccupancyStatusDesc,
                                NonCompliantStatus = r.NonCompliantStatus,  //***
                                NonCompliantStatusDesc = r.NonCompliantStatusDesc
                            };

                    return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };
                }
                else
                {
                    //** Asset Operational Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus_MapAlone(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = r.MeterGroup,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                // DemandZoneId = demandZone.DemandZoneId,
                                // DemandZoneDesc = demandZone.DemandZoneDesc,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                OperationalStatusId = r.OperationalStatusId,
                                OperationalStatusDesc = r.OperationalStatusDesc,
                                EventCode = r.EventCode,
                                EventDescVerbose = r.EventDescVerbose
                            };

                }
                return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
            }

            //DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            ////var result = new DataSourceResult
            //{
            //    Data = items,
            //    Total = total,
            //};



            return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };

        }

        public ActionResult GetCustomerGridDetails_New([DataSourceRequest]DataSourceRequest request, string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs, string pageChosen)
        {

            //now save the sort filters
            //if (request.Sorts.Count > 0)
            //    SetSavedSortValues(request.Sorts, "Meter");


            //get models
            int total = 0;
            IEnumerable<GISModel> items = null;

            long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
            int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

            int areaId = -1;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            int zoneId = -1;
            string street = string.Empty;
            string suburb = string.Empty;
            string demandArea = string.Empty;

            try
            {

                if (locationTypeName == "Area")
                {
                    // areaId = Convert.ToInt32(parallerLocationId);
                    areaNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Zone")
                {
                    //zoneId = Convert.ToInt32(parallerLocationId);
                    zoneNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeName == "Suburb")
                {
                    suburb = parallerLocationId;
                }
                else if (locationTypeName == "Demand Area")
                {
                    demandArea = parallerLocationId;
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert AssetStatus to int list type before invoking factory class
                string[] assetStatusArr = assetStatus.Split(',');
                List<int?> assetStatusIDs = new List<int?>();
                for (var i = 0; i < assetStatusArr.Length; i++)
                {
                    assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
                }

                //** Convert Occupancy Criteria to int list type before invoking factory class
                string[] specificCriteriaArr = specificCriteria.Split(',');
                List<int?> occupancyIDs = new List<int?>();
                for (var i = 0; i < specificCriteriaArr.Length; i++)
                {
                    occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
                }

                //** Convert Non-Compliance Criteria to int list type before invoking factory class
                string[] NonComplianceStatusArr = nonCompStatus.Split(',');
                List<int?> NonComplianceStatusIDs = new List<int?>();
                for (var i = 0; i < NonComplianceStatusArr.Length; i++)
                {
                    NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
                }

                //** Convert Opertional Status Criteria to int list type before invoking factory class
                string[] operationalCriteriaArr = specificCriteria.Split(',');
                List<int?> operationalIDs = new List<int?>();
                for (var i = 0; i < operationalCriteriaArr.Length; i++)
                {
                    operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
                }

                if (layerValue == "1")
                {
                    //** Inventory Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory_All(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                AssetId = Convert.ToString(r.AssetID),

                                //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,

                                MeterName = r.MeterName,
                                AssetStateDesc = r.AssetStateDesc,
                                Location = r.Location,
                                BayStart = r.BayStart,
                                BayEnd = r.BayEnd,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                MeterGroup = r.MeterGroup,
                            };

                }
                else if (layerValue == "2")
                {
                    //** Parking Space Layer;
                    var items_raw = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);
                    items = from r in items_raw
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = 20,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                MeterID = r.MeterID,
                                BayID = r.BayID,
                                HasSensor = r.HasSensor,
                                PaymentType = r.PaymentType,
                                CurrentMeterTime = r.CurrentMeterTime,
                                NonSensorEventTime = r.NonSensorEventTime,
                                PaymentTime = r.PaymentTime,
                                BayNumber = r.BayNumber,
                                ViolationType = r.ViolationType,
                                amountInCent = r.amountInCent,
                                endDateTimeMob = r.endDateTimeMob,
                                startDateTimeMob = r.startDateTimeMob,

                                NonCommStatusDesc = r.NonCommStatusDesc, //** added on May 30th 2016 for finding NonCommSensor

                                OccupancyStatusID = r.OccupancyStatusID,
                                OccupancyStatusDesc = r.OccupancyStatusDesc,
                                NonCompliantStatus = r.NonCompliantStatus,  //***
                                NonCompliantStatusDesc = r.NonCompliantStatusDesc
                            };


                }
                else
                {
                    //** Asset Operational Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs, request, out total, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = r.MeterGroup,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                // DemandZoneId = demandZone.DemandZoneId,
                                // DemandZoneDesc = demandZone.DemandZoneDesc,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                OperationalStatusId = r.OperationalStatusId,
                                OperationalStatusDesc = r.OperationalStatusDesc,
                                EventCode = r.EventCode,
                                EventDescVerbose = r.EventDescVerbose
                            };

                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
            }

            DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            //var result = new DataSourceResult
            {
                Data = items,
                Total = total,
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCustomerGridDetails_NewHome(string assetType, string assetId, string locationTypeId, string locationTypeName, string parallerLocationId, string assetStatus, string demZone, string specificCriteria, string layerValue, string assetModel, string nonCompStatus, string complianceStatusIs, string pageChosen)
        {

            //now save the sort filters
            //if (request.Sorts.Count > 0)
            //    SetSavedSortValues(request.Sorts, "Meter");


            //get models
            // int total = 0;
            IEnumerable<GISModel> items = null;

            long assetID = (!string.IsNullOrEmpty(assetId)) ? Convert.ToInt64(assetId) : -1;
            int layervalue = (!string.IsNullOrEmpty(layerValue)) ? Convert.ToInt32(layerValue) : -1;

            int areaId = -1;
            string areaNameIs = string.Empty;
            string zoneNameIs = string.Empty;
            int zoneId = -1;
            string street = string.Empty;
            string suburb = string.Empty;
            string demandArea = string.Empty;

            try
            {

                if (locationTypeName == "Area")
                {
                    // areaId = Convert.ToInt32(parallerLocationId);
                    areaNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Zone")
                {
                    //zoneId = Convert.ToInt32(parallerLocationId);
                    zoneNameIs = parallerLocationId;
                }
                else if (locationTypeName == "Street")
                {
                    street = parallerLocationId;
                }
                else if (locationTypeName == "Suburb")
                {
                    suburb = parallerLocationId;
                }
                else if (locationTypeName == "Demand Area")
                {
                    demandArea = parallerLocationId;
                }


                //** Convert AssetModel to int list type before invoking factory class
                string[] assetModelArr = assetModel.Split(',');
                List<int?> assetModelIDs = new List<int?>();
                if (assetModel == "")
                {
                    //** Asset Model dd is disabled
                }
                else
                {
                    for (var i = 0; i < assetModelArr.Length; i++)
                    {
                        assetModelIDs.Add(Convert.ToInt32(assetModelArr[i]));
                    }
                }


                //** Convert AssetType to int list type before invoking factory class
                string[] assetTypeArr = assetType.Split(',');
                List<int?> assetTypeIDs = new List<int?>();
                for (var i = 0; i < assetTypeArr.Length; i++)
                {
                    assetTypeIDs.Add(Convert.ToInt32(assetTypeArr[i]));
                }

                //** Convert Demandzone to int list type before invoking factory class
                string[] demArr = demZone.Split(',');
                List<int?> demandZoneIDs = new List<int?>();
                for (var i = 0; i < demArr.Length; i++)
                {
                    demandZoneIDs.Add(Convert.ToInt32(demArr[i]));
                }

                //** Convert AssetStatus to int list type before invoking factory class
                string[] assetStatusArr = assetStatus.Split(',');
                List<int?> assetStatusIDs = new List<int?>();
                for (var i = 0; i < assetStatusArr.Length; i++)
                {
                    assetStatusIDs.Add(Convert.ToInt32(assetStatusArr[i]));
                }

                //** Convert Occupancy Criteria to int list type before invoking factory class
                string[] specificCriteriaArr = specificCriteria.Split(',');
                List<int?> occupancyIDs = new List<int?>();
                for (var i = 0; i < specificCriteriaArr.Length; i++)
                {
                    occupancyIDs.Add(Convert.ToInt32(specificCriteriaArr[i]));
                }

                //** Convert Non-Compliance Criteria to int list type before invoking factory class
                string[] NonComplianceStatusArr = nonCompStatus.Split(',');
                List<int?> NonComplianceStatusIDs = new List<int?>();
                for (var i = 0; i < NonComplianceStatusArr.Length; i++)
                {
                    NonComplianceStatusIDs.Add(Convert.ToInt32(NonComplianceStatusArr[i]));
                }

                //** Convert Opertional Status Criteria to int list type before invoking factory class
                string[] operationalCriteriaArr = specificCriteria.Split(',');
                List<int?> operationalIDs = new List<int?>();
                for (var i = 0; i < operationalCriteriaArr.Length; i++)
                {
                    operationalIDs.Add(Convert.ToInt32(operationalCriteriaArr[i]));
                }

                if (layerValue == "1")
                {
                    //** Inventory Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_Inventory_All_Home(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, assetStatusIDs, layervalue);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                AssetId = Convert.ToString(r.AssetID),

                                //** The below two lines are assigned default values temporarily on 29th sep 2014 sairam. It has to be enabled later once demand zone is ready
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,

                                MeterName = r.MeterName,
                                AssetStateDesc = r.AssetStateDesc,
                                Location = r.Location,
                                BayStart = r.BayStart,
                                BayEnd = r.BayEnd,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                MeterGroup = r.MeterGroup,
                            };

                }
                else if (layerValue == "2")
                {
                    //** Parking Space Layer;
                    if (complianceStatusIs == "All")
                    {
                        var items_raw = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_Home(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs);
                        items = from r in items_raw
                                select new GISModel
                                {
                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    MeterID = r.MeterID,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc
                                };
                        return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };
                    }
                    else if (complianceStatusIs == "Compliant")
                    {
                        var tempResult = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_OnlyCompliant_MapAlone(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, pageChosen);
                        items = from r in tempResult
                                select new GISModel
                                {
                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    // NonCompliantStatusDesc = r.NonCompliantStatusDesc ?? "Compliant"  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc
                                };
                        return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };
                    }
                    else
                    {
                        var tempResult = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_HighDemandBays_NonCompliant_MapAlone(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, occupancyIDs, layervalue, NonComplianceStatusIDs, pageChosen);
                        items = from r in tempResult
                                select new GISModel
                                {
                                    CustomerID = r.CustomerID,
                                    AssetId = Convert.ToString(r.AssetID),
                                    MeterName = r.MeterName,
                                    ZoneID = r.ZoneID,
                                    ZoneName = r.ZoneName,
                                    AreaID = r.AreaID,
                                    AreaName = r.AreaName,
                                    Location = r.Location,
                                    //** The below two lines are used temporarily on 29th sep 2014 by sairam. It has to be commented later once join with parkingspacetype is ready
                                    MeterGroup = 20,
                                    MeterGroupDesc = "Parking Spaces",
                                    Latitude = r.Latitude,
                                    Longitude = r.Longitude,
                                    //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                    //  DemandZoneId = demandZone.DemandZoneId,
                                    //  DemandZoneDesc = demandZone.DemandZoneDesc,
                                    DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                    DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                    assetModelDesc = r.assetModelDesc,
                                    OccupancyStatusID = r.OccupancyStatusID,
                                    OccupancyStatusDesc = r.OccupancyStatusDesc,
                                    NonCompliantStatus = r.NonCompliantStatus,  //***
                                    NonCompliantStatusDesc = r.NonCompliantStatusDesc
                                };
                    }
                    return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };

                }
                else
                {
                    //** Asset Operational Layer;
                    var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetSearchMetersData_ParkingMeterOperationStatus_MapAlone(CurrentCity.Id, assetTypeIDs, assetModelIDs, assetID, areaNameIs, zoneNameIs, street, suburb, demandZoneIDs, operationalIDs, layervalue, NonComplianceStatusIDs, pageChosen);

                    items = from r in items_temp
                            select new GISModel
                            {
                                CustomerID = r.CustomerID,
                                AssetId = Convert.ToString(r.AssetID),
                                MeterName = r.MeterName,
                                ZoneID = r.ZoneID,
                                ZoneName = r.ZoneName,
                                AreaID = r.AreaID,
                                AreaName = r.AreaName,
                                Location = r.Location,
                                MeterGroup = r.MeterGroup,
                                MeterGroupDesc = r.MeterGroupDesc,
                                Latitude = r.Latitude,
                                Longitude = r.Longitude,
                                //** The below two lines are commented temporarily on 29th sep 2014 by sairam. It has to be enabled later once demand zone is ready
                                // DemandZoneId = demandZone.DemandZoneId,
                                // DemandZoneDesc = demandZone.DemandZoneDesc,
                                DemandZoneId = 1, //demandZone.DemandZoneId, [***** Currently set 1 as default 'High']
                                DemandZoneDesc = "High", //demandZone.DemandZoneDesc, [ **** Currently set 'High' as default ]
                                assetModelDesc = r.assetModelDesc,
                                OperationalStatusId = r.OperationalStatusId,
                                OperationalStatusDesc = r.OperationalStatusDesc,
                                EventCode = r.EventCode,
                                EventDescVerbose = r.EventDescVerbose
                            };

                }
                return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetCustomerGridDetails Method (GIS Reports)", ex);
            }

            //DataSourceResult result = new DataSourceResult()  //** Sairam made changes on dec 17th 2014
            ////var result = new DataSourceResult
            //{
            //    Data = items,
            //    Total = total,
            //};

            return new LargeJsonResult() { Data = items, MaxJsonLength = int.MaxValue };

        }

        public ActionResult GetOccupancyForMobile(string customerId, string areaID, string zoneID, string assetID, string pageChosen, string StatusIs)
        {

            List<GISModel> final = new List<GISModel>();

            var items_temp = (new GISFactory(Session[Constants.Security.ConnectionStringSessionVariableName].ToString())).GetMobileOccupancy(CurrentCity.Id);

            return new LargeJsonResult() { Data = items_temp, MaxJsonLength = int.MaxValue };
        }
    }

    /// <summary>
    /// Description : This Method is used to convert huge list values to JSON format
    /// Modified by : Sampath
    /// </summary>
    public class LargeJsonResult : JsonResult
    {
        const string JsonRequest_GetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";
        public LargeJsonResult()
        {
            MaxJsonLength = 2147483644;
            RecursionLimit = 100;
            JsonRequestBehavior = JsonRequestBehavior.AllowGet; //** Sairam added this on Oct 9th 2014 for Home page GIS

        }

        public int MaxJsonLength { get; set; }
        public int RecursionLimit { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(JsonRequest_GetNotAllowed);
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = MaxJsonLength, RecursionLimit = RecursionLimit };
                response.Write(serializer.Serialize(Data));
            }
        }



    }

}


