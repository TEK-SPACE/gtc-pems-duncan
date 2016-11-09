using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Web;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.Events;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Duncan.PEMS.Entities.BatteryChange;
using NLog;
using System.Data.Objects;
using Duncan.PEMS.Business.Exports;
using Duncan.PEMS.Entities.News;
using System.Data.Entity.Infrastructure;

namespace Duncan.PEMS.Business.BatteryChange
{


    /// </summary>
    public class BatteryChangeFactory : BaseFactory
    {

        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public BatteryChangeFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }


        public List<BatteryChangeModel> GetAssetIDs(int CurrentCity, string areaNameIs, string zoneNameIs, string street, string suburb, string myTypedText)
        {
            int myAreaId = -1;
            int myZoneId = -1;
            string mySuburb = string.Empty;

            List<BatteryChangeModel> result = new List<BatteryChangeModel>();

            try
            {
                //** Retrieve the meter IDs based on area/zone/street/suburb selection
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (suburb == "")
                {
                    mySuburb = string.Empty;
                }
                else
                {
                    mySuburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomerId == CurrentCity && x.DisplayName == suburb).DisplayName;
                }

                var details_raw = (from meter in PemsEntities.BatteryVoltageVs

                                   //join mm in PemsEntities.MeterMaps on
                                   //     new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                                   //     new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap //** Sairam modified on 7th oct [ AreaID2 is used now instead of AreaID]
                                   //from l1 in metermap.DefaultIfEmpty()

                                   where meter.CustomerID == CurrentCity

                                   && (meter.AreaID == myAreaId || myAreaId == -1)
                                   && (meter.ZoneId == myZoneId || myZoneId == -1)
                                   && (meter.Street == street || street == string.Empty)
                                   && (meter.Suburb == mySuburb || mySuburb == string.Empty)
                                   && SqlFunctions.StringConvert((double)meter.AssetId).Trim().StartsWith(myTypedText)
                                   select new
                                   {
                                       AssetID = meter.AssetId
                                   }).ToList();

                result = (from i in details_raw
                          select new BatteryChangeModel
                          {
                              Text = i.AssetID.ToString()
                          }).ToList();

                return result;

            }
            catch (Exception e)
            {

            }

            return result;

        }

        public List<sensorProfileModel> GetLocationsForListBox([DataSourceRequest]DataSourceRequest request, int CurrentCity, string areaNameIs, string zoneNameIs, string street, string suburb, DateTime startTimeOutput, DateTime endTimeOutput, List<string> statusTypeIDs)
        {
            int myAreaId = -1;
            int myZoneId = -1;
            string mySuburb = string.Empty;

            List<sensorProfileModel> result = new List<sensorProfileModel>();

            try
            {
                //** Retrieve the meter IDs based on area/zone/street/suburb selection
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (suburb == "")
                {
                    mySuburb = string.Empty;
                }
                else
                {
                    mySuburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomerId == CurrentCity && x.DisplayName == suburb).DisplayName;
                }


                var details_raw = (from i in PemsEntities.sensorProfiles
                                   where i.CustomerId == CurrentCity &&
                                         i.LastUpdatedTS >= startTimeOutput &&
                                         i.LastUpdatedTS <= endTimeOutput &&
                                         statusTypeIDs.Contains(i.ProfileStatus) &&
                                         (i.AreaId == myAreaId || myAreaId == -1) &&
                                         (i.ZoneId == myZoneId || myZoneId == -1) &&
                                         (i.Street == street || street == string.Empty)
                                   select new
                                   {
                                       AssetID = i.MeterId
                                   }).Distinct().ToList();


                result = (from i in details_raw
                          select new sensorProfileModel
                          {
                              Text = i.AssetID.ToString()
                          }).ToList();

                return result;

            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN GetLocationsForListBox Method (Sensor Profile Reports)", e);
            }

            return result;

        }

        public List<BatteryChangeModel> GetPieChartData([DataSourceRequest]DataSourceRequest request, out int total, int CurrentCity, DateTime startDate, DateTime endDate, long myAssetId, string areaNameIs, string zoneNameIs, string street, string suburb, decimal voltageStart, decimal voltageEnd)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;


            List<BatteryChangeModel> result = new List<BatteryChangeModel>();

            int myAreaId = -1;
            int myZoneId = -1;
            total = 0;
            string mySuburb = string.Empty;
            double myVoltStart = Convert.ToDouble(voltageStart);
            double myVoltEnd = Convert.ToDouble(voltageEnd);

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                if (suburb == "")
                {
                    mySuburb = string.Empty;
                }
                else
                {
                    mySuburb = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomerId == CurrentCity && x.DisplayName == suburb).DisplayName;
                }
                var details_raw = (from i in PemsEntities.avgbatteryvoltages
                                   where i.customerid == CurrentCity
                                       //  && i.Time_of_Last_Status_Report >= startDate && i.Time_of_Last_Status_Report <= endDate
                                   && (i.AssetId == myAssetId || myAssetId == -1)
                                   && (i.areaid == myAreaId || myAreaId == -1)
                                   && (i.ZoneId == myZoneId || myZoneId == -1)
                                   && (i.Street == street || street == string.Empty)
                                   && (i.Suburb == mySuburb || mySuburb == string.Empty)
                                   && i.avgbatt >= myVoltStart && i.avgbatt <= myVoltEnd
                                   select new BatteryChangeModel
                                   {
                                       avgVoltage = i.avgbatt,
                                       AssetType = i.AssetType,
                                       TimeOfLastStatus_Date = i.Time_of_Last_Status_Report,
                                       Area = i.Area,
                                       Zone = i.Zone,
                                       Suburb = i.Suburb,
                                       Street = i.Street,
                                       MID = i.AssetId,
                                       MeterName = i.AssetName,
                                       category = "",
                                       value = 0,
                                       color = ""
                                   }
                           ).ToList();


                //** Add Chart data by calculating voltages for different sectors (Normal, Plan to change and Change)
                var normalCnt = 0;
                var planToChgCnt = 0;
                var changeCnt = 0;

                for (var i = 0; i < details_raw.Count(); i++)
                {

                    if (details_raw[i].avgVoltage >= 3.4)
                    {
                        //** Normal
                        normalCnt++;
                    }
                    else if (details_raw[i].avgVoltage >= 3.0 && details_raw[i].avgVoltage < 3.4)
                    {
                        //** Due for replacement / Plan to change
                        planToChgCnt++;
                    }
                    else if (details_raw[i].avgVoltage < 3.0)
                    {
                        //** Due for replacement / Plan to change
                        changeCnt++;
                    }
                }


                IQueryable<BatteryChangeModel> tempResult = details_raw.AsQueryable();
                tempResult = tempResult.ApplyFiltering(request.Filters);
                total = tempResult.Count();
                tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);

                var finalResult = tempResult.ToList();

                if (finalResult.Count() > 0)
                {
                    BatteryChangeModel inst_1 = new BatteryChangeModel();

                    inst_1.category = "Normal";
                    inst_1.value = normalCnt;
                    inst_1.color = "#3ec770";

                    finalResult[0].chartData.Add(inst_1);

                    BatteryChangeModel inst_2 = new BatteryChangeModel();

                    inst_2.category = "Plan to Change";
                    inst_2.value = planToChgCnt; ;
                    inst_2.color = "#ff6112";

                    finalResult[0].chartData.Add(inst_2);

                    BatteryChangeModel inst_3 = new BatteryChangeModel();

                    inst_3.category = "Change";
                    inst_3.value = changeCnt; ;
                    inst_3.color = "#DB2929";

                    finalResult[0].chartData.Add(inst_3);
                }


                return finalResult;


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetPieChartData Method (GIS Reports)", ex);
            }

            return result;
        }

        public List<BatteryAnalysisModel> GetBarChartData([DataSourceRequest]DataSourceRequest request, out int total, int CurrentCity, DateTime startDate, DateTime endDate, long myAssetId, string areaNameIs, string zoneNameIs, string street)
        {

            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<BatteryAnalysisModel> result = new List<BatteryAnalysisModel>();

            int myAreaId = -1;
            int myZoneId = -1;
            total = 0;
            string mySuburb = string.Empty;

            try
            {

                //** Sairam added on Oct 13th 2014 to filter by area or zone names
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }

                var result_raw = (from A in PemsEntities.BatteryChangeGrids
                                  where A.CustomerID == CurrentCity && A.ChangedDate >= startDate && A.ChangedDate <= endDate
                                  && (A.MeterID == myAssetId || myAssetId == -1)
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  group A by A.MeterID into grp
                                  select new
                                  {
                                      MID = grp.Key,
                                      MeterName = grp.FirstOrDefault().MeterName,
                                      AreaName = grp.FirstOrDefault().Area,
                                      AreaID = grp.FirstOrDefault().AreaID,
                                      ZoneID = grp.FirstOrDefault().ZoneID,
                                      ZoneName = grp.FirstOrDefault().Zone,
                                      Street = grp.FirstOrDefault().Street,
                                      firstSecComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      secToThirdComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      BatChangesCnt = grp.Count(),
                                      firstChange_dateFormat = grp.FirstOrDefault().ChangedDate,
                                      secondChange_dateFormat = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().ChangedDate : null),
                                      thirdChange_dateFormat = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().ChangedDate : null),
                                      SensorStatus = (grp.FirstOrDefault().HasSensor == true ? "Yes" : "No")

                                  }).ToList().Select(p => new BatteryAnalysisModel
                                  {
                                      MID = p.MID,
                                      MeterName = p.MeterName,
                                      Area = p.AreaName,
                                      Zone = p.ZoneName,
                                      Street = p.Street,
                                      chartData = new List<BatteryAnalysisModel>(),
                                      commsBtw_FirstChange_SecondChange = (p.firstSecComms == null) ? 0 : p.firstSecComms,
                                      commsBtw_SecondChange_ThirdChange = (p.secToThirdComms == null) ? 0 : p.secToThirdComms,
                                      BatChangesCnt = p.BatChangesCnt,
                                      firstChange = p.firstChange_dateFormat.ToString(),
                                      secondChange = p.secondChange_dateFormat.ToString(),
                                      thirdChange = p.thirdChange_dateFormat.ToString(),
                                      SensorStatus = p.SensorStatus
                                  }).OrderBy(x => x.MID);


                //** Add Chart data by calculating voltages for different sectors (Normal, Plan to change and Change)
                var changeCnt = 0;


                IQueryable<BatteryAnalysisModel> tempResult = result_raw.AsQueryable();
                tempResult = tempResult.ApplyFiltering(request.Filters);
                total = tempResult.Count();
                tempResult = tempResult.ApplySorting(request.Groups, request.Sorts);
                tempResult = tempResult.ApplyPaging(request.Page, request.PageSize);

                var finalResult = tempResult.ToList();

                if (finalResult.Count() > 0)
                {
                    BatteryAnalysisModel inst_1 = new BatteryAnalysisModel();

                    inst_1.category = "Normal";
                    inst_1.value = 0;
                    inst_1.color = "#3ec770";

                    finalResult[0].chartData.Add(inst_1);

                    BatteryAnalysisModel inst_2 = new BatteryAnalysisModel();

                    inst_2.category = "Plan to Change";
                    inst_2.value = 0; ;
                    inst_2.color = "#ff6112";

                    finalResult[0].chartData.Add(inst_2);

                    BatteryAnalysisModel inst_3 = new BatteryAnalysisModel();

                    inst_3.category = "Change";
                    inst_3.value = total; ;
                    inst_3.color = String.Format("{0:m}", startDate);

                    finalResult[0].chartData.Add(inst_3);
                }


                return finalResult;


            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetPieChartData Method (GIS Reports)", ex);
            }

            return result;
        }

        public IEnumerable<BatteryChangeModel> GetLocationTypeId(string locationType, int CurrentCity, string myTypedText)
        {
            IEnumerable<BatteryChangeModel> details = Enumerable.Empty<BatteryChangeModel>();

            try
            {

                if (locationType == "Area")
                {
                    var details_raw = (from area in PemsEntities.Areas
                                       where area.CustomerID == CurrentCity && area.AreaID != null && area.AreaName.StartsWith(myTypedText)
                                       select new
                                       {
                                           myvalue = area.AreaID,
                                           Text = area.AreaName
                                       }).ToList();

                    details = (from i in details_raw
                               select new BatteryChangeModel
                               {
                                   value = Convert.ToInt64(i.myvalue),
                                   Text = i.Text
                               }).ToList();


                }
                else if (locationType == "Zone")
                {

                    var details_raw = (from zone in PemsEntities.Zones
                                       where zone.customerID == CurrentCity && zone.ZoneId != null && zone.ZoneName.StartsWith(myTypedText)
                                       select new
                                       {
                                           myvalue = zone.ZoneId,
                                           Text = zone.ZoneName
                                       }).ToList();

                    details = (from i in details_raw
                               select new BatteryChangeModel
                               {
                                   value = Convert.ToInt64(i.myvalue),
                                   Text = i.Text
                               }).ToList();

                }
                else if (locationType == "Street")
                {
                    details = (from meter in PemsEntities.Meters
                               where meter.CustomerID == CurrentCity && meter.Location != null && meter.Location.StartsWith(myTypedText)
                               select new BatteryChangeModel
                               {
                                   Text = meter.Location,
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items
                }
                else if (locationType == "Suburb")
                {
                    details = (from customGroup in PemsEntities.CustomGroup1
                               where customGroup.CustomerId == CurrentCity && customGroup.DisplayName != null && customGroup.DisplayName.StartsWith(myTypedText)
                               select new BatteryChangeModel
                               {
                                   Text = customGroup.DisplayName
                               }).Distinct(); //** Sairam added this line on Oct 7th 2014 to produce unique items


                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetLocationTypeId Method (GIS Reports)", ex);
            }


            return details;
        }



        public List<BatteryChartModel> GetBatteryAnalysisChartForUniqueIDS(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, List<int?> assetIDS, string areaNameIs, string zoneNameIs, string street)
        {

            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<BatteryChartModel> result = new List<BatteryChartModel>();

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                var result_raw = (from A in PemsEntities.BatteryAnalysisCharts
                                  where A.CustomerID == CurrentCity && A.VoltageDate >= startTimeOutput && A.VoltageDate <= endTimeOutput
                                      // && (A.MeterID == myAssetId || myAssetId == -1)
                                  && (assetIDS.Contains(A.MeterID))
                                  && (A.AreaID == myAreaId || myAreaId == -1)
                                  && (A.ZoneID == myZoneId || myZoneId == -1)
                                  && (A.Street == street || street == string.Empty)
                                  select new
                                  {
                                      MID = A.MeterID,
                                      MeterName = A.MeterName,
                                      VoltageDate = A.VoltageDate,
                                      MinVoltage = A.MinVolt,
                                      MaxVoltage = A.MaxVolt,
                                      CommsCnt = A.CommsCnt


                                  }).ToList().Select(p => new BatteryChartModel
                                  {
                                      MID = p.MID,
                                      MeterName = p.MeterName,
                                      VoltageDate_Dt = p.VoltageDate,
                                      VoltageDate = p.VoltageDate.ToString(),
                                      minVoltage = p.MinVoltage,
                                      maxVoltage = p.MaxVoltage,
                                      CommsCnt = p.CommsCnt
                                  }).OrderBy(x => x.MID).ThenBy(y => y.VoltageDate_Dt);

                //** Also add the battery changed dates for the selected start and end dates and put it a field
                //var result_raw_Change = (from A in PemsEntities.BatteryChangeGrids
                //               where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
                //               select new
                //               {
                //                  MID = A.Me


                result = result_raw.ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryAnalysisChart factory Method", ex);
            }

            return result;
        }


        public List<BatteryChartModel> GetBatteryAnalysisChart(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, long myAssetId, string areaNameIs, string zoneNameIs, string street)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<BatteryChartModel> result = new List<BatteryChartModel>();

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {

                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                var result_raw = (from A in PemsEntities.BatteryAnalysisCharts
                                  where A.CustomerID == CurrentCity && A.VoltageDate >= startTimeOutput && A.VoltageDate <= endTimeOutput
                                  && (A.MeterID == myAssetId || myAssetId == -1)
                                  && (A.AreaID == myAreaId || myAreaId == -1)
                                  && (A.ZoneID == myZoneId || myZoneId == -1)
                                  && (A.Street == street || street == string.Empty)
                                  select new
                                  {
                                      MID = A.MeterID,
                                      MeterName = A.MeterName,
                                      VoltageDate = A.VoltageDate,
                                      MinVoltage = A.MinVolt,
                                      MaxVoltage = A.MaxVolt,
                                      CommsCnt = A.CommsCnt


                                  }).ToList().Select(p => new BatteryChartModel
                                  {
                                      MID = p.MID,
                                      MeterName = p.MeterName,
                                      VoltageDate = p.VoltageDate.ToString(),
                                      minVoltage = p.MinVoltage,
                                      maxVoltage = p.MaxVoltage,
                                      CommsCnt = p.CommsCnt
                                  }).OrderBy(x => x.MID).ThenBy(y => y.VoltageDate);

                //** Also add the battery changed dates for the selected start and end dates and put it a field
                //var result_raw_Change = (from A in PemsEntities.BatteryChangeGrids
                //               where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
                //               select new
                //               {
                //                  MID = A.Me


                result = result_raw.ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryAnalysisChart factory Method", ex);
            }

            return result;
        }



        public List<BatteryAnalysisModel> GetUniqueIDSFromAction(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, long myAssetId, string areaNameIs, string zoneNameIs, string street)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<BatteryAnalysisModel> result = new List<BatteryAnalysisModel>();

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                var result_raw = (from A in PemsEntities.BatteryChangeGrids
                                  where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
                                   && (A.MeterID == myAssetId || myAssetId == -1)
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  group A by A.MeterID into grp
                                  select new
                                  {
                                      MID = grp.Key,

                                  }).ToList().Select(p => new BatteryAnalysisModel
                                  {
                                      MID = p.MID,
                                  }).OrderBy(x => x.MID);




                var items = result_raw.AsQueryable();
                result = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetUniqueIDSFromAction factory Method", ex);
            }

            return result;

        }

        public List<BatteryAnalysisModel> GetCommsDataForUniqueIDS(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, List<int?> assetIDS, string areaNameIs, string zoneNameIs, string street)
        {

            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;
            List<BatteryAnalysisModel> result = new List<BatteryAnalysisModel>();

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }




                var result_raw = (from A in PemsEntities.BatteryChangeGrids
                                  where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
                                      // && (A.MeterID == myAssetId || myAssetId == -1)
                                   && (assetIDS.Contains(A.MeterID))
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  group A by A.MeterID into grp
                                  select new
                                  {
                                      MID = grp.Key,
                                      firstSecComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      secToThirdComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      BatChangesCnt = grp.Count(),

                                  }).ToList().Select(p => new BatteryAnalysisModel
                                  {
                                      MID = p.MID,
                                      commsBtw_FirstChange_SecondChange = (p.firstSecComms == null) ? 0 : p.firstSecComms,
                                      commsBtw_SecondChange_ThirdChange = (p.secToThirdComms == null) ? 0 : p.secToThirdComms,
                                      BatChangesCnt = p.BatChangesCnt,
                                  }).OrderBy(x => x.MID);




                var items = result_raw.AsQueryable();
                result = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", ex);
            }

            return result;

        }

        public List<BatteryAnalysisModel> GetCommsData(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, long myAssetId, string areaNameIs, string zoneNameIs, string street)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<BatteryAnalysisModel> result = new List<BatteryAnalysisModel>();

            int myAreaId = -1;
            int myZoneId = -1;

            try
            {
                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }


                var result_raw = (from A in PemsEntities.BatteryChangeGrids
                                  where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
                                   && (A.MeterID == myAssetId || myAssetId == -1)
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  group A by A.MeterID into grp
                                  select new
                                  {
                                      MID = grp.Key,
                                      firstSecComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      secToThirdComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      BatChangesCnt = grp.Count(),

                                  }).ToList().Select(p => new BatteryAnalysisModel
                                  {
                                      MID = p.MID,
                                      commsBtw_FirstChange_SecondChange = (p.firstSecComms == null) ? 0 : p.firstSecComms,
                                      commsBtw_SecondChange_ThirdChange = (p.secToThirdComms == null) ? 0 : p.secToThirdComms,
                                      BatChangesCnt = p.BatChangesCnt,
                                  }).OrderBy(x => x.MID);




                var items = result_raw.AsQueryable();
                result = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", ex);
            }

            return result;

        }

        //public List<BatteryAnalysisModel> GetCommsData(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput)
        //{
        //    List<BatteryAnalysisModel> result = new List<BatteryAnalysisModel>();



        //    try
        //    {

        //        var result_raw = (from A in PemsEntities.BatteryChangeGrids
        //                          where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
        //                          group A by A.MeterID into grp
        //                          select new
        //                          {
        //                              MID = grp.Key,
        //                              firstChange_dateFormat = grp.FirstOrDefault().ChangedDate,
        //                              secondChange_dateFormat = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().ChangedDate : null),
        //                              thirdChange_dateFormat = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().ChangedDate : null),

        //                          }).ToList().Select(p => new BatteryAnalysisModel
        //                          {
        //                              MID = p.MID,



        //                              commsForFirstChange = FetchComms_new(p.MID, CurrentCity, 
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime) p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime) p.secondChange_dateFormat, (DateTime) p.secondChange_dateFormat},
        //                              new DateTime[] { (DateTime) p.thirdChange_dateFormat, (DateTime) p.thirdChange_dateFormat},
        //                              new DateTime[] { (DateTime) startTimeOutput, (DateTime) p.firstChange_dateFormat},
        //                              new DateTime[] { (DateTime) p.firstChange_dateFormat, (DateTime) p.secondChange_dateFormat},
        //                              new DateTime[] { (DateTime) p.secondChange_dateFormat, (DateTime) p.thirdChange_dateFormat}
        //                              )[0],


        //                              commsForSecondChange = FetchComms_new(p.MID, CurrentCity,
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.thirdChange_dateFormat, (DateTime)p.thirdChange_dateFormat },
        //                              new DateTime[] { (DateTime)startTimeOutput, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.thirdChange_dateFormat }
        //                              )[1],


        //                              commsForThirdChange = FetchComms_new(p.MID, CurrentCity,
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.thirdChange_dateFormat, (DateTime)p.thirdChange_dateFormat },
        //                              new DateTime[] { (DateTime)startTimeOutput, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.thirdChange_dateFormat }
        //                              )[2],

        //                              commsBtw_Start_FirstChange = FetchComms_new(p.MID, CurrentCity,
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.thirdChange_dateFormat, (DateTime)p.thirdChange_dateFormat },
        //                              new DateTime[] { (DateTime)startTimeOutput, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.thirdChange_dateFormat }
        //                              )[3],

        //                              commsBtw_FirstChange_SecondChange = FetchComms_new(p.MID, CurrentCity,
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.thirdChange_dateFormat, (DateTime)p.thirdChange_dateFormat },
        //                              new DateTime[] { (DateTime)startTimeOutput, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.thirdChange_dateFormat }
        //                              )[4],

        //                              commsBtw_SecondChange_ThirdChange = FetchComms_new(p.MID, CurrentCity,
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.thirdChange_dateFormat, (DateTime)p.thirdChange_dateFormat },
        //                              new DateTime[] { (DateTime)startTimeOutput, (DateTime)p.firstChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.firstChange_dateFormat, (DateTime)p.secondChange_dateFormat },
        //                              new DateTime[] { (DateTime)p.secondChange_dateFormat, (DateTime)p.thirdChange_dateFormat }
        //                              )[5],

        //                              firstChange = p.firstChange_dateFormat.ToString(),
        //                              secondChange = p.secondChange_dateFormat.ToString(),
        //                              thirdChange = p.thirdChange_dateFormat.ToString(),

        //                          }).OrderBy(x => x.MID);




        //        var items = result_raw.AsQueryable();
        //        result = items.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", ex);
        //    }

        //    return result;

        //}

        public int FetchComms(int? MID, int CurrentCity, DateTime? startTimeOutput, DateTime? endTimeOutput)
        {
            var result_raw = (from A in PemsEntities.BatteryAnalysisCharts
                              where A.CustomerID == CurrentCity && A.VoltageDate >= startTimeOutput && A.VoltageDate <= endTimeOutput && A.MeterID == MID
                              select new
                              {
                                  commsCnt = A.CommsCnt
                              }).ToList();

            return (int)result_raw.AsEnumerable().Sum(o => o.commsCnt);
        }

        public int[] FetchComms_new(int? MID, int CurrentCity, DateTime[] firstpair, DateTime[] secondpair, DateTime[] thirdpair, DateTime[] fourthtpair, DateTime[] fifthpair, DateTime[] sixthpair)
        {

            DateTime startTimeOutput = new DateTime();
            DateTime endTimeOutput = new DateTime();

            int[] result = new int[] { };

            try
            {

                if (DateTime.TryParse(firstpair[0].ToString(), out startTimeOutput))
                {
                    // ** Valid start date 
                }

                if (DateTime.TryParse(firstpair[1].ToString(), out endTimeOutput))
                {
                    // ** Valid end date 
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetTrellisChartData Method", ex);
            }

            var result_raw = (from A in PemsEntities.BatteryAnalysisCharts
                              where A.CustomerID == CurrentCity && A.VoltageDate >= startTimeOutput && A.VoltageDate <= endTimeOutput && A.MeterID == MID
                              select new
                              {
                                  commsCnt = A.CommsCnt
                              }).ToList();

            //result (int)result_raw.AsEnumerable().Sum(o => o.commsCnt);
            return result;
        }


        public List<sensorProfileModel> GetCustomers([DataSourceRequest]DataSourceRequest request)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<sensorProfileModel> result = new List<sensorProfileModel>();

            try
            {
                var result_raw = (from i in PemsEntities.Customers
                                  select new
                                  {
                                      MID = i.CustomerID
                                  }).Distinct().ToList();

                result = (from j in result_raw
                          select new sensorProfileModel
                          {
                              Text = j.MID.ToString(),
                              Value = Convert.ToInt32(j.MID)
                          }).OrderBy(a => a.Value).ToList();
                return result;
            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN GetCustomers factory Method", e);
            }
            return result;
        }


        public List<sensorProfileModel> GetProfileData([DataSourceRequest]DataSourceRequest request, out int total, int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, List<Int32?> myAssetId, List<string> myProfileStatus)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<sensorProfileModel> result = new List<sensorProfileModel>();
            total = 0;

            try
            {


                var resultRaw = (from i in PemsEntities.sensorProfiles
                                 where i.CustomerId == CurrentCity && i.LastUpdatedTS >= startTimeOutput
                                 && i.LastUpdatedTS <= endTimeOutput && myAssetId.Contains(i.MeterId) && myProfileStatus.Contains(i.ProfileStatus)

                                 select new
                                 {
                                     CustomerID = i.CustomerId,
                                     LastUpdatedTS = i.LastUpdatedTS,
                                     AreaID = i.AreaId,
                                     AreaName = i.AreaName,
                                     ZoneID = i.ZoneId,
                                     ZoneName = i.ZoneName,
                                     Street = i.Street,
                                     MeterID = i.MeterId,
                                     TMP = i.Tmp,
                                     TMPDesc = i.TmpDesc,
                                     RU = i.RU,
                                     RUDesc = i.RUDesc,
                                     AT = i.AT,
                                     ATDesc = i.ATDesc,
                                     FSW = i.FSW,
                                     SW = i.SW,
                                     BL = i.BL,
                                     TRPK = i.TRPK,
                                     TRPKDesc = i.TRPKDesc,
                                     LastStatus = i.LastStatus,
                                     OccupancyStatus = i.OccupancyStatus,
                                     ProfileStatus = i.ProfileStatus,
                                     D0 = i.D0,
                                     D1 = i.D1,
                                     D2 = i.D2,
                                     D3 = i.D3,
                                     D4 = i.D4,
                                     D5 = i.D5,
                                     D6 = i.D6,
                                     D7 = i.D7,
                                     D8 = i.D8,
                                     D9 = i.D9,

                                     D10 = i.D10,
                                     D11 = i.D11,
                                     D12 = i.D12,
                                     D13 = i.D13,
                                     D14 = i.D14,
                                     D15 = i.D15,
                                     D16 = i.D16,
                                     D17 = i.D17,
                                     D18 = i.D18,
                                     D19 = i.D19,

                                     D20 = i.D20,
                                     D21 = i.D21,
                                     D22 = i.D22,
                                     D23 = i.D23,
                                     D24 = i.D24,
                                     D25 = i.D25,
                                     D26 = i.D26,
                                     D27 = i.D27,
                                     D28 = i.D28,
                                     D29 = i.D29,

                                     D30 = i.D30,
                                     D31 = i.D31,
                                     D32 = i.D32,
                                     D33 = i.D33,
                                     D34 = i.D34,
                                     D35 = i.D35,
                                     D36 = i.D36,
                                     D37 = i.D37,
                                     D38 = i.D38,
                                     D39 = i.D39,

                                     D40 = i.D40,
                                     D41 = i.D41,
                                     D42 = i.D42,
                                     D43 = i.D43,
                                     D44 = i.D44,
                                     D45 = i.D45,
                                     D46 = i.D46,
                                     D47 = i.D47,
                                     D48 = i.D48,
                                     D49 = i.D49,

                                     D50 = i.D50,
                                     D51 = i.D51,
                                     D52 = i.D52,
                                     D53 = i.D53,
                                     D54 = i.D54,
                                     D55 = i.D55,
                                     D56 = i.D56,
                                     D57 = i.D57,
                                     D58 = i.D58,
                                     D59 = i.D59,

                                     T0 = i.T0,
                                     T1 = i.T1,
                                     T2 = i.T2,
                                     T3 = i.T3,
                                     T4 = i.T4,
                                     T5 = i.T5,
                                     T6 = i.T6,
                                     T7 = i.T7,
                                     T8 = i.T8,
                                     T9 = i.T9,

                                     T10 = i.T10,
                                     T11 = i.T11,
                                     T12 = i.T12,
                                     T13 = i.T13,
                                     T14 = i.T14,
                                     T15 = i.T15,
                                     T16 = i.T16,
                                     T17 = i.T17,
                                     T18 = i.T18,
                                     T19 = i.T19,

                                     T20 = i.T20,
                                     T21 = i.T21,
                                     T22 = i.T22,
                                     T23 = i.T23,
                                     T24 = i.T24,
                                     T25 = i.T25,
                                     T26 = i.T26,
                                     T27 = i.T27,
                                     T28 = i.T28,
                                     T29 = i.T29,

                                     T30 = i.T30,
                                     T31 = i.T31,
                                     T32 = i.T32,
                                     T33 = i.T33,
                                     T34 = i.T34,
                                     T35 = i.T35,
                                     T36 = i.T36,
                                     T37 = i.T37,
                                     T38 = i.T38,
                                     T39 = i.T39,

                                     T40 = i.T40,
                                     T41 = i.T41,
                                     T42 = i.T42,
                                     T43 = i.T43,
                                     T44 = i.T44,
                                     T45 = i.T45,
                                     T46 = i.T46,
                                     T47 = i.T47,
                                     T48 = i.T48,
                                     T49 = i.T49,

                                     T50 = i.T50,
                                     T51 = i.T51,
                                     T52 = i.T52,
                                     T53 = i.T53,
                                     T54 = i.T54,
                                     T55 = i.T55,
                                     T56 = i.T56,
                                     T57 = i.T57,
                                     T58 = i.T58,
                                     T59 = i.T59
                                 }).ToList().Select(p => new sensorProfileModel
                                 {
                                     CustomerID = p.CustomerID,
                                     LastUpdatedTS = p.LastUpdatedTS.ToString(),
                                     Area = p.AreaName,
                                     Zone = p.ZoneName,
                                     Street = p.Street,
                                     MID = p.MeterID,
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
                                     ProfileStatus = p.ProfileStatus,
                                     Diag = new int?[] { p.D0, p.D1, p.D2, p.D3, p.D4, p.D5, p.D6, p.D7, p.D8, p.D9, p.D10, p.D11, p.D12, p.D13, p.D14, p.D15, p.D16, p.D17, p.D18, p.D19, p.D20, p.D21, p.D22, p.D23, p.D24, p.D25, p.D26, p.D27, p.D28, p.D29, p.D30, p.D31, p.D32, p.D33, p.D34, p.D35, p.D36, p.D37, p.D38, p.D39, p.D40, p.D41, p.D42, p.D43, p.D44, p.D45, p.D46, p.D47, p.D48, p.D49, p.D50, p.D51, p.D52, p.D53, p.D54, p.D55, p.D56, p.D57, p.D58, p.D59 },
                                     Threshold = new int?[] { p.T0, p.T1, p.T2, p.T3, p.T4, p.T5, p.T6, p.T7, p.T8, p.T9, p.T10, p.T11, p.T12, p.T13, p.T14, p.T15, p.T16, p.T17, p.T18, p.T19, p.T20, p.T21, p.T22, p.T23, p.T24, p.T25, p.T26, p.T27, p.T28, p.T29, p.T30, p.T31, p.T32, p.T33, p.T34, p.T35, p.T36, p.T37, p.T38, p.T39, p.T40, p.T41, p.T42, p.T43, p.T44, p.T45, p.T46, p.T47, p.T48, p.T49, p.T50, p.T51, p.T52, p.T53, p.T54, p.T55, p.T56, p.T57, p.T58, p.T59 }
                                 }).ToList();

                var items = resultRaw.AsQueryable();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();

                return result;

            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", e);
            }

            return result;
        }



        public List<sensorProfileModel> GetProfileDataForChart(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, List<Int32?> myAssetId, List<string> myProfileStatus)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;

            List<sensorProfileModel> result = new List<sensorProfileModel>();
            try
            {


                var resultRaw = (from i in PemsEntities.sensorProfiles
                                 where i.CustomerId == CurrentCity && i.LastUpdatedTS >= startTimeOutput
                                 && i.LastUpdatedTS <= endTimeOutput && myAssetId.Contains(i.MeterId) && myProfileStatus.Contains(i.ProfileStatus)

                                 select new
                                 {
                                     CustomerID = i.CustomerId,
                                     LastUpdatedTS = i.LastUpdatedTS,
                                     AreaID = i.AreaId,
                                     AreaName = i.AreaName,
                                     ZoneID = i.ZoneId,
                                     ZoneName = i.ZoneName,
                                     Street = i.Street,
                                     MeterID = i.MeterId,
                                     TMP = i.Tmp,
                                     TMPDesc = i.TmpDesc,
                                     RU = i.RU,
                                     RUDesc = i.RUDesc,
                                     AT = i.AT,
                                     ATDesc = i.ATDesc,
                                     FSW = i.FSW,
                                     SW = i.SW,
                                     BL = i.BL,
                                     TRPK = i.TRPK,
                                     TRPKDesc = i.TRPKDesc,
                                     LastStatus = i.LastStatus,
                                     OccupancyStatus = i.OccupancyStatus,
                                     ProfileStatus = i.ProfileStatus,
                                     D0 = i.D0,
                                     D1 = i.D1,
                                     D2 = i.D2,
                                     D3 = i.D3,
                                     D4 = i.D4,
                                     D5 = i.D5,
                                     D6 = i.D6,
                                     D7 = i.D7,
                                     D8 = i.D8,
                                     D9 = i.D9,

                                     D10 = i.D10,
                                     D11 = i.D11,
                                     D12 = i.D12,
                                     D13 = i.D13,
                                     D14 = i.D14,
                                     D15 = i.D15,
                                     D16 = i.D16,
                                     D17 = i.D17,
                                     D18 = i.D18,
                                     D19 = i.D19,

                                     D20 = i.D20,
                                     D21 = i.D21,
                                     D22 = i.D22,
                                     D23 = i.D23,
                                     D24 = i.D24,
                                     D25 = i.D25,
                                     D26 = i.D26,
                                     D27 = i.D27,
                                     D28 = i.D28,
                                     D29 = i.D29,

                                     D30 = i.D30,
                                     D31 = i.D31,
                                     D32 = i.D32,
                                     D33 = i.D33,
                                     D34 = i.D34,
                                     D35 = i.D35,
                                     D36 = i.D36,
                                     D37 = i.D37,
                                     D38 = i.D38,
                                     D39 = i.D39,

                                     D40 = i.D40,
                                     D41 = i.D41,
                                     D42 = i.D42,
                                     D43 = i.D43,
                                     D44 = i.D44,
                                     D45 = i.D45,
                                     D46 = i.D46,
                                     D47 = i.D47,
                                     D48 = i.D48,
                                     D49 = i.D49,

                                     D50 = i.D50,
                                     D51 = i.D51,
                                     D52 = i.D52,
                                     D53 = i.D53,
                                     D54 = i.D54,
                                     D55 = i.D55,
                                     D56 = i.D56,
                                     D57 = i.D57,
                                     D58 = i.D58,
                                     D59 = i.D59,

                                     T0 = i.T0,
                                     T1 = i.T1,
                                     T2 = i.T2,
                                     T3 = i.T3,
                                     T4 = i.T4,
                                     T5 = i.T5,
                                     T6 = i.T6,
                                     T7 = i.T7,
                                     T8 = i.T8,
                                     T9 = i.T9,

                                     T10 = i.T10,
                                     T11 = i.T11,
                                     T12 = i.T12,
                                     T13 = i.T13,
                                     T14 = i.T14,
                                     T15 = i.T15,
                                     T16 = i.T16,
                                     T17 = i.T17,
                                     T18 = i.T18,
                                     T19 = i.T19,

                                     T20 = i.T20,
                                     T21 = i.T21,
                                     T22 = i.T22,
                                     T23 = i.T23,
                                     T24 = i.T24,
                                     T25 = i.T25,
                                     T26 = i.T26,
                                     T27 = i.T27,
                                     T28 = i.T28,
                                     T29 = i.T29,

                                     T30 = i.T30,
                                     T31 = i.T31,
                                     T32 = i.T32,
                                     T33 = i.T33,
                                     T34 = i.T34,
                                     T35 = i.T35,
                                     T36 = i.T36,
                                     T37 = i.T37,
                                     T38 = i.T38,
                                     T39 = i.T39,

                                     T40 = i.T40,
                                     T41 = i.T41,
                                     T42 = i.T42,
                                     T43 = i.T43,
                                     T44 = i.T44,
                                     T45 = i.T45,
                                     T46 = i.T46,
                                     T47 = i.T47,
                                     T48 = i.T48,
                                     T49 = i.T49,

                                     T50 = i.T50,
                                     T51 = i.T51,
                                     T52 = i.T52,
                                     T53 = i.T53,
                                     T54 = i.T54,
                                     T55 = i.T55,
                                     T56 = i.T56,
                                     T57 = i.T57,
                                     T58 = i.T58,
                                     T59 = i.T59
                                 }).ToList().Select(p => new sensorProfileModel
                                 {
                                     CustomerID = p.CustomerID,
                                     LastUpdatedTS = p.LastUpdatedTS.ToString(),
                                     Area = p.AreaName,
                                     Zone = p.ZoneName,
                                     Street = p.Street,
                                     MID = p.MeterID,
                                     TMPDesc = p.TMPDesc,
                                     RUDesc = p.RUDesc,
                                     ATDesc = p.ATDesc,
                                     FSW = p.FSW.ToString(),
                                     SW = p.SW.ToString(),
                                     SWDesc = p.FSW.ToString() + ", " + p.SW.ToString(),
                                     BL = p.BL.ToString(),
                                     TRPKDesc = p.TRPKDesc,
                                     LastStatus = p.LastStatus,
                                     OccupancyStatus = p.OccupancyStatus,
                                     ProfileStatus = p.ProfileStatus,
                                     Diag = new int?[] { p.D0, p.D1, p.D2, p.D3, p.D4, p.D5, p.D6, p.D7, p.D8, p.D9, p.D10, p.D11, p.D12, p.D13, p.D14, p.D15, p.D16, p.D17, p.D18, p.D19, p.D20, p.D21, p.D22, p.D23, p.D24, p.D25, p.D26, p.D27, p.D28, p.D29, p.D30, p.D31, p.D32, p.D33, p.D34, p.D35, p.D36, p.D37, p.D38, p.D39, p.D40, p.D41, p.D42, p.D43, p.D44, p.D45, p.D46, p.D47, p.D48, p.D49, p.D50, p.D51, p.D52, p.D53, p.D54, p.D55, p.D56, p.D57, p.D58, p.D59 },
                                     Threshold = new int?[] { p.T0, p.T1, p.T2, p.T3, p.T4, p.T5, p.T6, p.T7, p.T8, p.T9, p.T10, p.T11, p.T12, p.T13, p.T14, p.T15, p.T16, p.T17, p.T18, p.T19, p.T20, p.T21, p.T22, p.T23, p.T24, p.T25, p.T26, p.T27, p.T28, p.T29, p.T30, p.T31, p.T32, p.T33, p.T34, p.T35, p.T36, p.T37, p.T38, p.T39, p.T40, p.T41, p.T42, p.T43, p.T44, p.T45, p.T46, p.T47, p.T48, p.T49, p.T50, p.T51, p.T52, p.T53, p.T54, p.T55, p.T56, p.T57, p.T58, p.T59 }
                                 }).ToList().OrderBy(x => x.LastUpdatedTS);

                var items = resultRaw.AsQueryable();
                //items = items.ApplySorting(request.Groups, request.Sorts);
                // items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();

                return result;

            }
            catch (Exception e)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", e);
            }

            return result;
        }


        public List<OccupancyRateModel> GetOccupancyRateDetails(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, long myAssetId, string areaNameIs, string zoneNameIs, string street, [DataSourceRequest]DataSourceRequest request, out int total)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;
            List<OccupancyRateModel> result = new List<OccupancyRateModel>();

            int myAreaId = -1;
            int myZoneId = -1;
            total = 0;
            string mySuburb = string.Empty;

            string paramValues = string.Empty;

            // var spParams = GetSpParams(request, "MeterID desc", out paramValues);
            //string startDate = spParams[3].Value.ToString();
            //string endDate = spParams[4].Value.ToString();

            try
            {

                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }



                var result_raw = (from A in PemsEntities.OccRateDeriveds
                                  where A.CustomerID == CurrentCity && A.LastUpdatedTime >= startTimeOutput && A.LastUpdatedTime <= endTimeOutput
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  select new
                                  {
                                      LastUpdatedDateTime = A.LastUpdatedTime,
                                      OccupiedCnt = A.OccupiedCnt,
                                      VacantCnt = A.VacantCnt,
                                      ViolatedCnt = A.ViolatedCnt,
                                      MetersCnt = A.MetersCnt,
                                      OccupiedPercent = A.PercentOccupied,
                                      VacantPercent = A.PercentVacant,
                                      ViolatedPercent = A.PercentViolated,
                                      AreaName = A.Area,
                                      AreaID = A.AreaID,
                                      ZoneID = A.ZoneID,
                                      ZoneName = A.Zone,
                                      Street = A.Street


                                  }).OrderBy(x => x.LastUpdatedDateTime).ToList().Select(p => new OccupancyRateModel
                                  {
                                      LastUpdatedDateTime = p.LastUpdatedDateTime.ToString(),
                                      MetersCnt = p.MetersCnt,
                                      OccupiedCnt = p.OccupiedCnt,
                                      VacantCnt = p.VacantCnt,
                                      ViolatedCnt = p.ViolatedCnt,
                                      OccupiedPercent = p.OccupiedPercent + "%",
                                      VacantPercent = p.VacantPercent + "%",
                                      VioaltedPercent = p.ViolatedPercent + "%",
                                      Area = p.AreaName,
                                      Zone = p.ZoneName,
                                      Street = p.Street

                                  }).ToList();


                //** Add Chart data by calculating voltages for different sectors (Normal, Plan to change and Change)
                var totalPercentOccupied = 0;
                var totalSpacesCnt = 0;

                //for (var i = 0; i < result_raw.Count(); i++)
                //{
                //    var OccPerNoSymbol = result_raw[i].OccupiedPercent.ToString().Split('%')[0];
                //    int perOcc = Convert.ToInt32(OccPerNoSymbol);
                //    int spaceOcc = Convert.ToInt32(result_raw[i].OccupiedCnt);

                //    if (perOcc > 0)
                //    {
                //        //** total percent Occupied
                //        totalPercentOccupied = totalPercentOccupied + perOcc;
                //    }
                //    if (spaceOcc > 0)
                //    {
                //        //** total Spaces Occupied Count
                //        totalSpacesCnt = totalSpacesCnt + spaceOcc;
                //    }
                //}

                var totalAreasCnt = result_raw.Select(x => x.Area).Distinct().Count();

                var items = result_raw.AsQueryable();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();

                var finalResult = items.ToList();

                if (finalResult.Count() > 0)
                {
                    OccupancyRateModel inst_1 = new OccupancyRateModel();

                    inst_1.totalPercentOccupied = totalPercentOccupied;
                    inst_1.totalSpacesCnt = totalSpacesCnt;
                    inst_1.totalAreasCnt = totalAreasCnt;

                    finalResult[0].overAllData.Add(inst_1);

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", ex);
            }

            return result;
        }

        public List<OccupancyRateModel> GetOccupancyRateDetailsForChart(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, long myAssetId, string areaNameIs, string zoneNameIs, string street, [DataSourceRequest]DataSourceRequest request, out int total)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;
            List<OccupancyRateModel> result = new List<OccupancyRateModel>();

            int myAreaId = -1;
            int myZoneId = -1;
            total = 0;
            string mySuburb = string.Empty;

            string paramValues = string.Empty;

            // var spParams = GetSpParams(request, "MeterID desc", out paramValues);
            //string startDate = spParams[3].Value.ToString();
            //string endDate = spParams[4].Value.ToString();

            try
            {

                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }



                var result_raw = (from A in PemsEntities.OccRateDeriveds
                                  where A.CustomerID == CurrentCity && A.LastUpdatedTime >= startTimeOutput && A.LastUpdatedTime <= endTimeOutput
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  select new
                                  {
                                      LastUpdatedDateTime = A.LastUpdatedTime,
                                      OccupiedCnt = A.OccupiedCnt,
                                      VacantCnt = A.VacantCnt,
                                      ViolatedCnt = A.ViolatedCnt,
                                      MetersCnt = A.MetersCnt,
                                      OccupiedPercent = A.PercentOccupied,
                                      VacantPercent = A.PercentVacant,
                                      ViolatedPercent = A.PercentViolated,
                                      AreaName = A.Area,
                                      AreaID = A.AreaID,
                                      ZoneID = A.ZoneID,
                                      ZoneName = A.Zone,
                                      Street = A.Street


                                  }).OrderBy(x => x.LastUpdatedDateTime).ToList().Select(p => new OccupancyRateModel
                                  {
                                      LastUpdatedDateTime = p.LastUpdatedDateTime.ToString(),
                                      MetersCnt = p.MetersCnt,
                                      OccupiedCnt = p.OccupiedCnt,
                                      VacantCnt = p.VacantCnt,
                                      ViolatedCnt = p.ViolatedCnt,
                                      OccupiedPercent = p.OccupiedPercent + "%",
                                      VacantPercent = p.VacantPercent + "%",
                                      VioaltedPercent = p.ViolatedPercent + "%",
                                      Area = p.AreaName,
                                      Zone = p.ZoneName,
                                      Street = p.Street

                                  }).ToList();


                //** Add Chart data by calculating voltages for different sectors (Normal, Plan to change and Change)
                var totalPercentOccupied = 0;
                var totalSpacesCnt = 0;

                //for (var i = 0; i < result_raw.Count(); i++)
                //{
                //    var OccPerNoSymbol = result_raw[i].OccupiedPercent.ToString().Split('%')[0];
                //    int perOcc = Convert.ToInt32(OccPerNoSymbol);
                //    int spaceOcc = Convert.ToInt32(result_raw[i].OccupiedCnt);

                //    if (perOcc > 0)
                //    {
                //        //** total percent Occupied
                //        totalPercentOccupied = totalPercentOccupied + perOcc;
                //    }
                //    if (spaceOcc > 0)
                //    {
                //        //** total Spaces Occupied Count
                //        totalSpacesCnt = totalSpacesCnt + spaceOcc;
                //    }
                //}

                var totalAreasCnt = result_raw.Select(x => x.Area).Distinct().Count();

                var items = result_raw.AsQueryable();
                total = items.Count();
                //items = items.ApplySorting(request.Groups, request.Sorts);
                //items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();

                var finalResult = items.ToList();

                if (finalResult.Count() > 0)
                {
                    OccupancyRateModel inst_1 = new OccupancyRateModel();

                    inst_1.totalPercentOccupied = totalPercentOccupied;
                    inst_1.totalSpacesCnt = totalSpacesCnt;
                    inst_1.totalAreasCnt = totalAreasCnt;

                    finalResult[0].overAllData.Add(inst_1);

                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", ex);
            }

            return result;
        }

        public List<BatteryAnalysisModel> GetBatteryAnalysisDetails(int CurrentCity, DateTime startTimeOutput, DateTime endTimeOutput, long myAssetId, string areaNameIs, string zoneNameIs, string street, [DataSourceRequest]DataSourceRequest request, out int total)
        {
            ((IObjectContextAdapter)PemsEntities).ObjectContext.CommandTimeout = 3600;
            List<BatteryAnalysisModel> result = new List<BatteryAnalysisModel>();

            int myAreaId = -1;
            int myZoneId = -1;
            total = 0;
            string mySuburb = string.Empty;

            string paramValues = string.Empty;

            var spParams = GetSpParams(request, "MeterID desc", out paramValues);
            string startDate = spParams[3].Value.ToString();
            string endDate = spParams[4].Value.ToString();

            try
            {

                if (areaNameIs == "")
                {
                    myAreaId = -1;
                }
                else
                {
                    myAreaId = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == CurrentCity && x.AreaName == areaNameIs).AreaID;
                }

                if (zoneNameIs == "")
                {
                    myZoneId = -1;
                }
                else
                {
                    myZoneId = PemsEntities.Zones.FirstOrDefault(x => x.customerID == CurrentCity && x.ZoneName == zoneNameIs).ZoneId;
                }



                var result_raw = (from A in PemsEntities.BatteryChangeGrids
                                  where A.CustomerID == CurrentCity && A.ChangedDate >= startTimeOutput && A.ChangedDate <= endTimeOutput
                                  && (A.MeterID == myAssetId || myAssetId == -1)
                                   && (A.AreaID == myAreaId || myAreaId == -1)
                                   && (A.ZoneID == myZoneId || myZoneId == -1)
                                   && (A.Street == street || street == string.Empty)
                                  group A by A.MeterID into grp
                                  select new
                                  {
                                      MID = grp.Key,
                                      MeterName = grp.FirstOrDefault().MeterName,
                                      AreaName = grp.FirstOrDefault().Area,
                                      AreaID = grp.FirstOrDefault().AreaID,
                                      ZoneID = grp.FirstOrDefault().ZoneID,
                                      ZoneName = grp.FirstOrDefault().Zone,
                                      Street = grp.FirstOrDefault().Street,
                                      firstSecComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      secToThirdComms = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().PrevToChangedComms : null),
                                      BatChangesCnt = grp.Count(),
                                      firstChange_dateFormat = grp.FirstOrDefault().ChangedDate,
                                      secondChange_dateFormat = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(1).Take(1).FirstOrDefault().ChangedDate : null),
                                      thirdChange_dateFormat = (grp.Count() > 1 ? grp.OrderBy(x => x.MeterID).Skip(2).Take(1).FirstOrDefault().ChangedDate : null),
                                      SensorStatus = (grp.FirstOrDefault().HasSensor == true ? "Yes" : "No")

                                  }).ToList().Select(p => new BatteryAnalysisModel
                                  {
                                      MID = p.MID,
                                      MeterName = p.MeterName,
                                      Area = p.AreaName,
                                      Zone = p.ZoneName,
                                      Street = p.Street,
                                      commsBtw_FirstChange_SecondChange = (p.firstSecComms == null) ? 0 : p.firstSecComms,
                                      commsBtw_SecondChange_ThirdChange = (p.secToThirdComms == null) ? 0 : p.secToThirdComms,
                                      BatChangesCnt = p.BatChangesCnt,
                                      firstChange = p.firstChange_dateFormat.ToString(),
                                      secondChange = p.secondChange_dateFormat.ToString(),
                                      thirdChange = p.thirdChange_dateFormat.ToString(),
                                      SensorStatus = p.SensorStatus
                                  }).OrderBy(x => x.MID);




                var items = result_raw.AsQueryable();
                total = items.Count();
                items = items.ApplySorting(request.Groups, request.Sorts);
                items = items.ApplyPaging(request.Page, request.PageSize);
                result = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetBatteryChangeDetails factory Method", ex);
            }

            return result;
        }



    }

}
