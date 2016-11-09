/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________        __________________________________________________________________________________________________
 * 02/06/2014       R Howard                    JIRA: DPTXPEMS-225  Added CustomerId to where clause for any selects from PEMS Areas table
 * 
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using Duncan.PEMS.Business.Assets;
using Duncan.PEMS.Business.Users;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Alarms;
using Duncan.PEMS.Entities.Collections;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;
using WebMatrix.WebData;


namespace Duncan.PEMS.Business.Collections
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Collections"/> namespace contains classes for managing collections.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    public class CollectionsFactory : BaseFactory
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
        public CollectionsFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        #region Collection Run Constants

        /// <summary>
        /// This enumerations reflects the data in table [CollectionRunStatus] table.
        /// </summary>
        public enum Status
        {
            Inactive = 0,
            Active = 1,
            Pending = 2
        };

        #endregion

        #region "Collection Routes List"

        /// <summary>
        /// Gets a list of all user models in the system
        /// </summary>
        /// <returns></returns>
        public List<CollectionListModel> GetCollectionsListModels([DataSourceRequest] DataSourceRequest request,
                                                                  out int total, string customerId, string meterId,
                                                                  string meterName, bool applyPaging = false,
                                                                  int exportCount = -1)
        {
            int custID;
            int.TryParse(customerId, out custID);
            // Step 1: Build initial query

            // if there is a meterid, then 
            int meterID = 0;
            bool parsed = int.TryParse(meterId, out meterID);
            if (parsed && meterID > 0)
                return GetCollectionsListModels(request, out total, applyPaging, exportCount, custID, meterID, meterId);

            //if there is a meterName, then
            if (!string.IsNullOrEmpty(meterName))
                return GetCollectionsListModels(request, out total, applyPaging, exportCount, custID, meterName);


            return GetCollectionsListModels(request, out total, applyPaging, exportCount, custID);
        }

        /// <summary>
        /// Gets a list of collection routes,  Fitlers based on metername
        /// </summary>
        private List<CollectionListModel> GetCollectionsListModels(DataSourceRequest request, out int total,
                                                                   bool applyPaging, int exportCount, int custID,
                                                                   string meterName)
        {
            var query = from cr in PemsEntities.CollectionRuns
                        join crm in PemsEntities.CollectionRunMeters on cr.CollectionRunId equals crm.CollectionRunId
                        join crs in PemsEntities.CollectionRunStatus on cr.CollectionRunStatus equals
                            crs.CollectionRunStatusId
                        //we are adding the metername filter here!
                        from m in PemsEntities.Meters
                        where crm.MeterId == m.MeterId
                        where crm.CustomerId == m.CustomerID
                        where crm.AreaId == m.AreaID
                        where crm.CustomerId == custID
                        where m.MeterName.Contains(meterName)
                        group cr by
                            new
                                {
                                    cr.CollectionRunId,
                                    cr.CollectionRunName,
                                    cr.DateCreated,
                                    crs.CollectionRunStatusDesc,
                                    cr.ActivationDate,
                                    cr.CollectionRunStatus
                                }
                            into g
                            select new CollectionListModel
                                {
                                    NumberOfMeters =
                                        PemsEntities.CollectionRunMeters.Count(
                                            x => x.CollectionRunId == g.Key.CollectionRunId),
                                    RouteId = g.Key.CollectionRunId,
                                    RouteName = g.Key.CollectionRunName,
                                    Status = g.Key.CollectionRunStatusDesc,
                                    StatusId = g.Key.CollectionRunStatus,
                                    DateCreated = g.Key.DateCreated,
                                    DateActivated = g.Key.ActivationDate
                                };

            // Step 2: Filter data
            query = query.ApplyFiltering(request.Filters);
            total = query.Count();


            // Step 3: Get count of filtered items
            total = query.Count();

            // Step 4: Apply sorting
            query = query.ApplySorting(request.Groups, request.Sorts);

            // Step 5: Apply paging
            if (applyPaging)
                query = exportCount > -1 ? query.Take(exportCount) : query.ApplyPaging(request.Page, request.PageSize);


            // Step 6: Get the data!
            return query.ToList();
        }

        /// <summary>
        /// Gets a list of collection routes,  Fitlers based on meterid
        /// </summary>
        private List<CollectionListModel> GetCollectionsListModels(DataSourceRequest request, out int total,
                                                                   bool applyPaging, int exportCount, int custID,
                                                                   int meterID, string meterIDString)
        {
            var query = from cr in PemsEntities.CollectionRuns
                        join crm in PemsEntities.CollectionRunMeters on cr.CollectionRunId equals crm.CollectionRunId
                        join crs in PemsEntities.CollectionRunStatus on cr.CollectionRunStatus equals
                            crs.CollectionRunStatusId
                        where cr.CustomerId == custID
                        //we are adding the meterid filter here!
                        //UPDATE we are doinga  starts with on the meter id instead of equals.
                        where SqlFunctions.StringConvert((double)crm.MeterId).TrimStart().StartsWith(meterIDString)
                        //  where crm.MeterId.StartsWith(meterIDString.Trim())
                        group cr by
                            new
                                {
                                    cr.CollectionRunId,
                                    cr.CollectionRunName,
                                    cr.DateCreated,
                                    crs.CollectionRunStatusDesc,
                                    cr.ActivationDate,
                                    cr.CollectionRunStatus
                                }
                            into g
                            select new CollectionListModel
                                {
                                    NumberOfMeters =
                                        PemsEntities.CollectionRunMeters.Count(
                                            x => x.CollectionRunId == g.Key.CollectionRunId),
                                    RouteId = g.Key.CollectionRunId,
                                    RouteName = g.Key.CollectionRunName,
                                    Status = g.Key.CollectionRunStatusDesc,
                                    StatusId = g.Key.CollectionRunStatus,
                                    DateCreated = g.Key.DateCreated,
                                    DateActivated = g.Key.ActivationDate
                                };

            // Step 2: Filter data
            query = query.ApplyFiltering(request.Filters);


            // Step 3: Get count of filtered items
            total = query.Count();

            // Step 4: Apply sorting
            query = query.ApplySorting(request.Groups, request.Sorts);

            // Step 5: Apply paging
            if (applyPaging)
                query = exportCount > -1 ? query.Take(exportCount) : query.ApplyPaging(request.Page, request.PageSize);


            // Step 6: Get the data!
            return query.ToList();
        }

        /// <summary>
        /// Gets a list of collection routes with no custom filters
        /// </summary>
        private List<CollectionListModel> GetCollectionsListModels(DataSourceRequest request, out int total,
                                                                   bool applyPaging, int exportCount,
                                                                   int custID)
        {
            var query = from cr in PemsEntities.CollectionRuns
                        // join crm in PemsEntities.CollectionRunMeters on cr.CollectionRunId equals crm.CollectionRunId
                        join crs in PemsEntities.CollectionRunStatus on cr.CollectionRunStatus equals
                            crs.CollectionRunStatusId
                        where cr.CustomerId == custID
                        group cr by
                            new
                                {
                                    cr.CollectionRunId,
                                    cr.CollectionRunName,
                                    cr.DateCreated,
                                    crs.CollectionRunStatusDesc,
                                    cr.ActivationDate,
                                    cr.CollectionRunStatus
                                }
                            into g
                            select new CollectionListModel
                                {
                                    NumberOfMeters =
                                        PemsEntities.CollectionRunMeters.Count(
                                            x => x.CollectionRunId == g.Key.CollectionRunId),
                                    RouteId = g.Key.CollectionRunId,
                                    RouteName = g.Key.CollectionRunName,
                                    Status = g.Key.CollectionRunStatusDesc,
                                    StatusId = g.Key.CollectionRunStatus,
                                    DateCreated = g.Key.DateCreated,
                                    DateActivated = g.Key.ActivationDate
                                };

            // Step 2: Filter data
            query = query.ApplyFiltering(request.Filters);
            total = query.Count();


            // Step 3: Get count of filtered items
            total = query.Count();

            // Step 4: Apply sorting
            query = query.ApplySorting(request.Groups, request.Sorts);

            // Step 5: Apply paging
            if (applyPaging)
                query = exportCount > -1 ? query.Take(exportCount) : query.ApplyPaging(request.Page, request.PageSize);

            // Step 6: Get the data!
            return query.ToList();
        }


        #endregion

        #region "Collection Aggregation List"

        /// <summary>
        /// Get a list of Collection Route Aggregations
        /// </summary>
        public List<AggregationListModel> GetAggregationListModels([DataSourceRequest] DataSourceRequest request,
                                                                   out int total, string customerId, string meterId,
                                                                   string meterName, string area, string zone,
                                                                   string suburb, string startDate, string endDate,
                                                                   bool applyPaging = false, int exportCount = -1)
        {
            int custID;
            int.TryParse(customerId, out custID);
            //for this listpage, we need to generate a list of all colleciton run IDs bsed on thefilters passed in. then we can use the regular filters to filter on routename, routeid, and variance 
            var collRunReports = GenerateCollectionRunReports(meterId, meterName, area, zone, suburb, startDate, endDate,
                                                              custID);
            bool toInclude = true;
            //need to generate a list of collection runs, for each dy
            var allItems = new List<AggregationListModel>();
            foreach (var collRunReport in collRunReports)
            {
                toInclude = true;
                //make sure there are meters to collect, and any have been collected
                if (collRunReport.CollectionRun == null || collRunReport.CollectionRun.CollectionRunMeters == null ||
                    !collRunReport.CollectionRun.CollectionRunMeters.Any()) continue;
                var aggReport = new AggregationListModel
                    {
                        DateTime = collRunReport.CollectionDate,
                        RouteId = collRunReport.CollectionRunId,
                        RouteName = collRunReport.CollectionRun.CollectionRunName,
                        VendorId = collRunReport.VendorId,
                        UnScheduled = collRunReport.CollectionRun.UnScheduled,
                        VendorName =
                            collRunReport.VendorId == null
                                ? ""
                                : PemsEntities.CollectionRunVendors.FirstOrDefault(
                                    x => x.CollectionRunVendorId == collRunReport.VendorId) == null
                                      ? ""
                                      : PemsEntities.CollectionRunVendors.FirstOrDefault(
                                          x => x.CollectionRunVendorId == collRunReport.VendorId)
                                                    .CollectionRunVendorName,
                        MetersToCollect = collRunReport.CollectionRun.CollectionRunMeters.Count(),
                        MetersCollected = collRunReport.TotalMeterCount,
                        TotalCollectedMeter = collRunReport.TotalMeterCashAmt == 0 ? 0.0 : (double)collRunReport.TotalMeterCashAmt / 100.0,
                        TotalCollectedChip = collRunReport.TotalByChip == 0 || collRunReport.TotalByChip == null ? 0.0 : (double)collRunReport.TotalByChip / 100.0,
                        TotalCollectedVendor = collRunReport.TotalManualCashAmt == 0 ? 0.0 : (double)collRunReport.TotalManualCashAmt / 100.0,
                        Variance = (collRunReport.TotalMeterCashAmt - collRunReport.TotalManualCashAmt) == 0 ? 0 : 1,
                        AmountDiff = (collRunReport.TotalMeterCashAmt - collRunReport.TotalManualCashAmt) == 0 ? 0 : (float)(collRunReport.TotalMeterCashAmt - collRunReport.TotalManualCashAmt) / 100,
                        CustomerId = custID,
                        CollectionRunReportId = collRunReport.CollectionRunReportId

                    };

                if(aggReport.UnScheduled.HasValue)
                {
                    if(aggReport.UnScheduled.Value == true && aggReport.TotalCollectedMeter.Value <= 0.0 && aggReport.MetersCollected <= 0)
                    {
                        toInclude = false;//Don't add 
                    }
                }
               
                if(toInclude)
                    allItems.Add(aggReport);
            }

            //now we build a list of CR aggregate items needed
            var query = allItems.AsQueryable();

            // Step 2: Filter data
            query = query.ApplyFiltering(request.Filters);

            // Step 3: Get count of filtered items
            total = query.Count();

            // Step 4: Apply sorting
            query = query.ApplySorting(request.Groups, request.Sorts);

            // Step 5: Apply paging
            if (applyPaging)
                query = exportCount > -1 ? query.Take(exportCount) : query.ApplyPaging(request.Page, request.PageSize);

            // Step 6: Get the data!
            return query.ToList();
        }



        /// <summary>
        /// Get a list of collection run reports that match any of the criteria passed in. 
        /// </summary>
        private IEnumerable<CollectionRunReport> GenerateCollectionRunReports(string meterId, string meterName,
                                                                              string area, string zone,
                                                                              string suburb, string startDate,
                                                                              string endDate,
                                                                              int custID)
        {
            var meterIds = new List<int>();
            var crIds = new List<long>();
            bool testForMeterIDs = false;
            //METER ID
            if (!string.IsNullOrEmpty(meterId))
            {
                testForMeterIDs = true;
                int meterID;
                bool parsed = int.TryParse(meterId, out meterID);
                if (parsed && meterID > 0)
                    //add the meter ID to the list
                    meterIds.Add(meterID);
            }

            //METER NAME
            if (!string.IsNullOrEmpty(meterName))
            {
                testForMeterIDs = true;
                var items = PemsEntities.Meters.Where(x => x.CustomerID == custID && x.MeterName.Contains(meterName));
                if (items.Any())
                    foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                        meterIds.Add(item.MeterId);
            }

            //we will compare area, zone, and suburb (customgroup1) to the meter map, and get a list of relevant meter ids

            //AREA
            if (!string.IsNullOrEmpty(area))
            {
                testForMeterIDs = true;
                //get all theareans that match this name
                var areas = PemsEntities.Areas.Where(x => x.AreaName.Contains(area) && x.CustomerID == custID);
                if (areas.Any())
                {
                    //now get all the meter maps where the areaID2 is in this list of areas
                    List<int> areaIds = areas.Select(x => x.AreaID).ToList();
                    var items =
                        PemsEntities.MeterMaps.Where(x => x.Customerid == custID && areaIds.Contains(x.AreaId2.Value));
                    if (items.Any())
                        foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                            meterIds.Add(item.MeterId);
                }
            }

            //ZONE
            if (!string.IsNullOrEmpty(zone))
            {
                testForMeterIDs = true;
                //get all theareans that match this name
                var zones = PemsEntities.Zones.Where(x => x.ZoneName.Contains(zone) && x.customerID == custID);
                if (zones.Any())
                {
                    //now get all the meter maps where the areaID2 is in this list of areas
                    var ids = zones.Select(x => x.ZoneId).ToList();
                    var items = PemsEntities.MeterMaps.Where(x => x.Customerid == custID && ids.Contains(x.ZoneId.Value));
                    if (items.Any())
                        foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                            meterIds.Add(item.MeterId);
                }
            }

            //SUBURB
            if (!string.IsNullOrEmpty(suburb))
            {

                testForMeterIDs = true;
                //get all theareans that match this name
                var suburbs =
                    PemsEntities.CustomGroup1.Where(x => x.DisplayName.Contains(suburb) && x.CustomerId == custID);
                if (suburbs.Any())
                {
                    //now get all the meter maps where the areaID2 is in this list of areas
                    var ids = suburbs.Select(x => x.CustomGroupId).ToList();
                    var items =
                        PemsEntities.MeterMaps.Where(x => x.Customerid == custID && ids.Contains(x.CustomGroup1.Value));
                    if (items.Any())
                        foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                            meterIds.Add(item.MeterId);
                }
            }

            //now that we have our list of meterids to check, go to the colldatasumm table, get all the CR ids that have matching meterids, then build the list of CRs from those ids

            //start date and end date checkt he colldatasumm table
            //parse the dates
            var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
            sDate = sDate.ToLocalTime();
            var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
            eDate = eDate.ToLocalTime();

            var collRunReports =
                PemsEntities.CollectionRunReports.Where(
                    x => x.CustomerId == custID && x.CollectionDate >= sDate && x.CollectionDate <= eDate).ToList();
            //if the meter IDS have any values, then filter on those, otherwise, bring back all for this date range.
            if (testForMeterIDs)
            //if (meterIds.Any())
            {
                //   collRunReports =collRunReports.Where(x =>meterIds.Intersect(x.CollectionRun.CollectionRunMeters.Select(y => y.MeterId).Distinct()).Any());
                collRunReports =
                    collRunReports.Where(
                        x =>
                        x.CollectionRun.CollectionRunMeters.Any(
                            y => meterIds.Any(z => y.MeterId.ToString().StartsWith(z.ToString())))).ToList();

            }
            return collRunReports;
        }

        #endregion

        //TODO - GTC: update this section.

        #region Grouped Collection Aggregation

        /// <summary>
        /// Get a list of Collection Route Grouped Aggregations
        /// </summary>
        public List<GroupedAggregationListModel> GetGroupedAggregationListModels(
            [DataSourceRequest] DataSourceRequest request, out int total, string customerId, string meterId,
            string meterName, string area, string zone, string suburb, string startDate, string endDate,
            bool applyPaging = false, int exportCount = -1)
        {
            //todo - GTC: verify this functionality.
            //these pull from differnt tables, so 
            //might not need the same signature, etc.

            int custID;
            int.TryParse(customerId, out custID);
            //for this listpage, we need to generate a list of all colleciton run IDs bsed on thefilters passed in. then we can use the regular filters to filter on routename, routeid, and variance 
            var collRunReports = GenerateGroupedCollectionRunReports(meterId, meterName, area, zone, suburb, startDate,
                                                                     endDate, custID);

            //need to generate a list of collection runs, for each dy
            var allItems = new List<GroupedAggregationListModel>();
            foreach (var collRunReport in collRunReports)
            {
                //make sure there are meters to collect, and any have been collected
                if (collRunReport.CollectionRun == null || collRunReport.CollectionRun.CollectionRunMeters == null ||
                    !collRunReport.CollectionRun.CollectionRunMeters.Any()) continue;
                var aggReport = new GroupedAggregationListModel
                    {
                        DateTime = collRunReport.CollectionDate,
                        RouteId = collRunReport.CollectionRunId,
                        RouteName = collRunReport.CollectionRun.CollectionRunName,
                        VendorId = collRunReport.VendorId,
                        VendorName =
                            collRunReport.VendorId == null
                                ? ""
                                : PemsEntities.CollectionRunVendors.FirstOrDefault(
                                    x => x.CollectionRunVendorId == collRunReport.VendorId) == null
                                      ? ""
                                      : PemsEntities.CollectionRunVendors.FirstOrDefault(
                                          x => x.CollectionRunVendorId == collRunReport.VendorId)
                                                    .CollectionRunVendorName,
                        MetersToCollect = collRunReport.CollectionRun.CollectionRunMeters.Count(),
                        MetersCollected = collRunReport.TotalMeterCount,
                        //todo - gtc: verify this
                        TotalCollected =
                            collRunReport.TotalMeterCashAmt + collRunReport.TotalByChip +
                            collRunReport.TotalManualCashAmt,
                        Variance = (collRunReport.TotalMeterCashAmt - collRunReport.TotalManualCashAmt) == 0 ? 0 : 1,
                        AmountDiff = collRunReport.TotalMeterCashAmt - collRunReport.TotalManualCashAmt,
                        CustomerId = custID
                    };
                allItems.Add(aggReport);
            }

            //now we build a list of CR aggregate items needed
            var query = allItems.AsQueryable();

            // Step 2: Filter data
            query = query.ApplyFiltering(request.Filters);

            // Step 3: Get count of filtered items
            total = query.Count();

            // Step 4: Apply sorting
            query = query.ApplySorting(request.Groups, request.Sorts);

            // Step 5: Apply paging
            if (applyPaging)
                query = exportCount > -1 ? query.Take(exportCount) : query.ApplyPaging(request.Page, request.PageSize);

            // Step 6: Get the data!
            return query.ToList();
        }


        /// <summary>
        /// Get a list of collection run reports that match any of the criteria passed in. 
        /// </summary>
        private IEnumerable<CollectionRunReport> GenerateGroupedCollectionRunReports(string meterId, string meterName,
                                                                                     string area, string zone,
                                                                                     string suburb, string startDate,
                                                                                     string endDate,
                                                                                     int custID)
        {
            //todo - GTC: update this to pull the grouped collection aggregation data from the correct tables, etc.
            var meterIds = new List<int>();
            var crIds = new List<long>();
            bool testForMeterIDs = false;
            //METER ID
            if (!string.IsNullOrEmpty(meterId))
            {
                testForMeterIDs = true;
                int meterID;
                bool parsed = int.TryParse(meterId, out meterID);
                if (parsed && meterID > 0)
                    //add the meter ID to the list
                    meterIds.Add(meterID);
            }

            //METER NAME
            if (!string.IsNullOrEmpty(meterName))
            {
                testForMeterIDs = true;
                var items = PemsEntities.Meters.Where(x => x.CustomerID == custID && x.MeterName.Contains(meterName));
                if (items.Any())
                    foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                        meterIds.Add(item.MeterId);
            }

            //we will compare area, zone, and suburb (customgroup1) to the meter map, and get a list of relevant meter ids

            //AREA
            if (!string.IsNullOrEmpty(area))
            {
                testForMeterIDs = true;
                //get all theareans that match this name
                var areas = PemsEntities.Areas.Where(x => x.AreaName.Contains(area) && x.CustomerID == custID);
                if (areas.Any())
                {
                    //now get all the meter maps where the areaID2 is in this list of areas
                    List<int> areaIds = areas.Select(x => x.AreaID).ToList();
                    var items =
                        PemsEntities.MeterMaps.Where(x => x.Customerid == custID && areaIds.Contains(x.AreaId2.Value));
                    if (items.Any())
                        foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                            meterIds.Add(item.MeterId);
                }
            }

            //ZONE
            if (!string.IsNullOrEmpty(zone))
            {
                testForMeterIDs = true;
                //get all theareans that match this name
                var zones = PemsEntities.Zones.Where(x => x.ZoneName.Contains(zone) && x.customerID == custID);
                if (zones.Any())
                {
                    //now get all the meter maps where the areaID2 is in this list of areas
                    var ids = zones.Select(x => x.ZoneId).ToList();
                    var items = PemsEntities.MeterMaps.Where(x => x.Customerid == custID && ids.Contains(x.ZoneId.Value));
                    if (items.Any())
                        foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                            meterIds.Add(item.MeterId);
                }
            }

            //SUBURB
            if (!string.IsNullOrEmpty(suburb))
            {

                testForMeterIDs = true;
                //get all theareans that match this name
                var suburbs =
                    PemsEntities.CustomGroup1.Where(x => x.DisplayName.Contains(suburb) && x.CustomerId == custID);
                if (suburbs.Any())
                {
                    //now get all the meter maps where the areaID2 is in this list of areas
                    var ids = suburbs.Select(x => x.CustomGroupId).ToList();
                    var items =
                        PemsEntities.MeterMaps.Where(x => x.Customerid == custID && ids.Contains(x.CustomGroup1.Value));
                    if (items.Any())
                        foreach (var item in items.Where(item => !meterIds.Contains(item.MeterId)))
                            meterIds.Add(item.MeterId);
                }
            }

            //now that we have our list of meterids to check, go to the colldatasumm table, get all the CR ids that have matching meterids, then build the list of CRs from those ids

            //start date and end date checkt he colldatasumm table
            //parse the dates
            var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
            sDate = sDate.ToLocalTime();
            var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
            eDate = eDate.ToLocalTime();

            var collRunReports =
                PemsEntities.CollectionRunReports.Where(
                    x => x.CustomerId == custID && x.CollectionDate >= sDate && x.CollectionDate <= eDate).ToList();
            //if the meter IDS have any values, then filter on those, otherwise, bring back all for this date range.
            if (testForMeterIDs)
            //if (meterIds.Any())
            {
                //   collRunReports =collRunReports.Where(x =>meterIds.Intersect(x.CollectionRun.CollectionRunMeters.Select(y => y.MeterId).Distinct()).Any());
                collRunReports =
                    collRunReports.Where(
                        x =>
                        x.CollectionRun.CollectionRunMeters.Any(
                            y => meterIds.Any(z => y.MeterId.ToString().StartsWith(z.ToString())))).ToList();

            }
            return collRunReports;
        }


        #endregion

        #region "Filters"

        /// <summary>
        /// Gets the available list of Colleciton Run Status's
        /// </summary>
        public List<StatusDDLModel> GetStatuses()
        {
            var ddlItems = new List<StatusDDLModel>();
            try
            {
                var items = PemsEntities.CollectionRunStatus;
                if (items.Any())
                    ddlItems.AddRange(
                        items.Select(
                            item =>
                            new StatusDDLModel
                                {
                                    Value = item.CollectionRunStatusDesc,
                                    Text = item.CollectionRunStatusDesc
                                }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetStatuses", ex);
                ddlItems = new List<StatusDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of colleciton run vendors for this customer
        /// </summary>
        /// <param name="customerId"></param>
        public List<VendorDDLModel> GetVendors(int customerId)
        {
            var ddlItems = new List<VendorDDLModel>();
            try
            {
                var items = PemsEntities.CollectionRunVendors.Where(x => x.CustomerId == customerId);
                if (items.Any())
                    ddlItems.AddRange(
                        items.Select(
                            item =>
                            new VendorDDLModel
                                {
                                    Value = item.CollectionRunVendorId,
                                    Text = item.CollectionRunVendorName
                                }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetVendors", ex);
                ddlItems = new List<VendorDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets the values for the Alarm Status dropdown on the listing page
        /// </summary>
        public List<ConfigDDLModel> GetConfigurations(int customerID)
        {
            var ddlItems = new List<ConfigDDLModel>();
            try
            {
                var items = (from cr in PemsEntities.CollectionRuns
                             where cr.CustomerId == customerID
                             select new ConfigDDLModel
                                 {
                                     Value = cr.CollectionRunId,
                                     Text = cr.CollectionRunName
                                 }).Distinct();
                if (items.Any())
                    ddlItems = items.ToList();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetConfigurations", ex);
                ddlItems = new List<ConfigDDLModel>();
            }
            return ddlItems;
        }

        /// <summary>
        /// Gets a list of suburbs for this customer (customGroup1)
        /// </summary>
        public List<AlarmDDLModel> GetSuburbs(int customerID)
        {
            var ddlItems = new List<AlarmDDLModel>();
            try
            {
                var items = PemsEntities.CustomGroup1.Where(x => x.CustomerId == customerID);
                if (items.Any())
                    ddlItems.AddRange(
                        items.Select(item => new AlarmDDLModel { Value = item.CustomGroupId, Text = item.DisplayName }));
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetSuburbs", ex);
                ddlItems = new List<AlarmDDLModel>();
            }
            return ddlItems;
        }

        public string AddVendorname(int customerId, string vendorname)
        {
            var items = (from cr in PemsEntities.CollectionRunVendors
                         where cr.CustomerId == customerId && cr.CollectionRunVendorName == vendorname
                         select cr).FirstOrDefault();

            if (items == null)
            {
                CollectionRunVendor vendor = new CollectionRunVendor();
                vendor.CustomerId = customerId;
                vendor.CollectionRunVendorName = vendorname;
                PemsEntities.CollectionRunVendors.Add(vendor);
                PemsEntities.SaveChanges();

                items = (from cr in PemsEntities.CollectionRunVendors
                         where cr.CustomerId == customerId && cr.CollectionRunVendorName == vendorname
                         select cr).FirstOrDefault();
                if (items == null)
                    return "0";
                else
                    return "1";
            }
            return "-1";

        }

        #endregion

        #region "Aggregation Details"

        /// <summary>
        /// Gets the details for the collection aggregation
        /// </summary>
        public AggregationDetails GetAggregationDetails(string routeId, string customerId, DateTime collectionDate)
        {
            //get the basic aggregation information
            int custID;
            int.TryParse(customerId, out custID);
            long routeID;
            long.TryParse(routeId, out routeID);
            var collRun = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == routeID);
            if (collRun != null)
            {
                var details = new AggregationDetails
                    {
                        DateTime = collectionDate,
                        RouteId = collRun.CollectionRunId,
                        RouteName = collRun.CollectionRunName,
                        VendorId = collRun.VendorId,
                        VendorName = collRun.CollectionRunVendor.CollectionRunVendorName,
                        CustomerId = custID
                    };

                //now lets get the aggregation info
                //need to get a list of CollectionRunMeters and colldatasumminfo for the aggregated data. 
                var currentStartDate = collectionDate;
                var currentEndDate = currentStartDate.AddDays(1).AddMilliseconds(-1);
                //CollectionRunMeters
                var crms =
                    PemsEntities.CollectionRunMeters.Where(
                        x => x.CustomerId == custID && x.CollectionRunId == collRun.CollectionRunId);
                var meterIds = crms.Select(x => x.MeterId).Distinct().ToList();


                //To determine the meters we need for this run, there will NOT be "CollectionRunID" values in the CollDataSumms and CashCoxDataImports table.
                //So, we will get all the meters for the collection run (crms), then get all the items collected for this date. any that have the correct meter ids in the crms are the meters we are looking for. 
                //to determine the meter information, get all the CashBoxData Imports, and for each one of those, get the first corrosponding colldatasumm (if it exists) for that date. 
                //the assumption is that each meter will be only collected ONCE per day. 

                //need to get the aggregatied data from the CollecitonRunReport table. where customerid, runid, and collectiondate match
                var collectionRunReport =
                    PemsEntities.CollectionRunReports.FirstOrDefault(
                        x =>
                        x.CustomerId == custID && x.CollectionRunId == routeID && x.CollectionDate >= currentStartDate &&
                        x.CollectionDate <= currentEndDate);

                //CollDataSumm
                var cds =
                    PemsEntities.CollDataSumms.Where(
                        x =>
                        x.CustomerId == custID && meterIds.Contains(x.MeterId) && x.CollDateTime >= currentStartDate &&
                        x.CollDateTime <= currentEndDate).ToList();

                //CashBoxDataImport - no not include unscheduled meters in this list
                var cdis =
                    PemsEntities.CashBoxDataImports.Where(
                        x =>
                        x.CustomerId == custID && meterIds.Contains(x.MeterId) && x.DateTimeRead >= currentStartDate &&
                        x.DateTimeRead <= currentEndDate && !x.UnscheduledFlag).ToList();

                //set this now, since we have the data to get the value. get all collections for htis customer and this day that have the flag set (UnscheduledFlag)
                details.TotalUnscheduledMeterCollected =
                    PemsEntities.CashBoxDataImports.Count(
                        x =>
                        x.CustomerId == custID && x.DateTimeRead >= currentStartDate && x.DateTimeRead <= currentEndDate &&
                        x.UnscheduledFlag);

                //populate all the aggregated data for this collection route
                details = PopulateAggregationData(details, crms, collectionRunReport, cdis, cds);

                //and finally, lets get a list of meters for this aggregation.
                details = PopulateAggregationMeters(details, routeID, cds, cdis, custID, collectionDate);

                return details;
            }
            return new AggregationDetails();
        }

        public AggregationDetails GetAggregationDetails(string routeId, string customerId, DateTime collectionDate, long collectionRunReportId)
        {
            //get the basic aggregation information
            int custID;
            int.TryParse(customerId, out custID);
            long routeID;
            long.TryParse(routeId, out routeID);
            var collRun = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == routeID);
            if (collRun != null)
            {
                var details = new AggregationDetails
                {
                    DateTime = collectionDate,
                    RouteId = collRun.CollectionRunId,
                    RouteName = collRun.CollectionRunName,
                    VendorId = collRun.VendorId,
                    VendorName = collRun.CollectionRunVendor.CollectionRunVendorName,
                    CustomerId = custID
                };

                //now lets get the aggregation info
                //need to get a list of CollectionRunMeters and colldatasumminfo for the aggregated data. 
                var currentStartDate = collectionDate;
                var currentEndDate = currentStartDate.AddDays(1).AddMilliseconds(-1);
                //CollectionRunMeters
                var crms =
                    PemsEntities.CollectionRunMeters.Where(
                        x => x.CustomerId == custID && x.CollectionRunId == collRun.CollectionRunId);
                var meterIds = crms.Select(x => x.MeterId).Distinct().ToList();


                //To determine the meters we need for this run, there will NOT be "CollectionRunID" values in the CollDataSumms and CashCoxDataImports table.
                //So, we will get all the meters for the collection run (crms), then get all the items collected for this date. any that have the correct meter ids in the crms are the meters we are looking for. 
                //to determine the meter information, get all the CashBoxData Imports, and for each one of those, get the first corrosponding colldatasumm (if it exists) for that date. 
                //the assumption is that each meter will be only collected ONCE per day. 

                //need to get the aggregatied data from the CollecitonRunReport table. where customerid, runid, and collectiondate match
                var collectionRunReport =
                    PemsEntities.CollectionRunReports.FirstOrDefault(
                        x =>
                        x.CustomerId == custID && x.CollectionRunId == routeID && x.CollectionDate >= currentStartDate &&
                        x.CollectionDate <= currentEndDate);


                /////////////////////////////////
                //CollDataSumm
                var cds =
                    PemsEntities.CollDataSumms.Where(
                        x =>
                        x.CustomerId == custID && meterIds.Contains(x.MeterId) && x.CollDateTime >= currentStartDate &&
                        x.CollDateTime <= currentEndDate).ToList();

                //CashBoxDataImport - no not include unscheduled meters in this list
                var cdis =
                    PemsEntities.CashBoxDataImports.Where(
                        x =>
                        x.CustomerId == custID && meterIds.Contains(x.MeterId) && x.DateTimeRead >= currentStartDate &&
                        x.DateTimeRead <= currentEndDate && !x.UnscheduledFlag).ToList();

                //set this now, since we have the data to get the value. get all collections for htis customer and this day that have the flag set (UnscheduledFlag)
                details.TotalUnscheduledMeterCollected =
                    PemsEntities.CashBoxDataImports.Count(
                        x =>
                        x.CustomerId == custID && x.DateTimeRead >= currentStartDate && x.DateTimeRead <= currentEndDate &&
                        x.UnscheduledFlag);

                /////////////////////////////////////

                //populate all the aggregated data for this collection route
                details = PopulateAggregationData(details, crms, collectionRunReport, cdis, cds);

                //and finally, lets get a list of meters for this aggregation.
                //details = PopulateAggregationMeters(details, routeID, cds, cdis, custID, collectionDate); Original
                details = PopulateAggregationMeters(details, routeID, custID, collectionDate, collectionRunReportId);
                details.CollectionRunReportID = collectionRunReportId;
                return details;
            }
            return new AggregationDetails();
        }

        /// <summary>
        /// Populates a list of meters for the collection aggregation. Uses the Collection data summ and cash box data import to determine meters to use.
        /// </summary>
        private AggregationDetails PopulateAggregationMeters(AggregationDetails details,
                                                             long collectionRouteID,
                                                             List<CollDataSumm> cds,
                                                             List<CashBoxDataImport> cdis,
                                                             int custID, DateTime collectionDateTime)
        {
            //List<CashBoxDataImport> RouteMetersDetailsV = PemsEntities.RouteMetersDetailsVs.FirstOrDefault(x => x.CollectionRunId == collectionRouteID);

            var RouteMetersDetailsV =
                 PemsEntities.RouteMetersDetailsVs.Where(
                        x => x.CollectionRunId == collectionRouteID).ToList();


            var aggMeters = new List<AggregationMeter>();

            foreach (var cdi in RouteMetersDetailsV)
            {

                var meterId = cdi.MeterId;
                var meter = PemsEntities.Meters.FirstOrDefault(x => x.MeterId == meterId);
                if (meter != null)
                {
                    var aggMeter = new AggregationMeter
                        {
                            MeterId = meter.MeterId,
                            MeterName = meter.MeterName,
                            DateTime = cdi.DateTime,
                            CollectionDateTime = collectionDateTime,
                            AreaId = meter.AreaID,
                            CustomerId = custID
                        };
                    //now get the meter map so we can get zone and suburb
                    var meterMap =
                        PemsEntities.MeterMaps.FirstOrDefault(
                            x => x.MeterId == meterId && x.Customerid == custID && x.Areaid == meter.AreaID);
                    if (meterMap != null)
                    {
                        aggMeter.ZoneId = meterMap.ZoneId;
                        aggMeter.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName;
                        aggMeter.AreaId2 = meterMap.AreaId2;
                        aggMeter.Area = meterMap.AreaId2 == null
                                            ? ""
                                            : PemsEntities.Areas.FirstOrDefault(
                                                x => x.CustomerID == custID && x.AreaID == meterMap.AreaId2) == null
                                                  ? ""
                                                  : PemsEntities.Areas.FirstOrDefault(
                                                      x => x.CustomerID == custID && x.AreaID == meterMap.AreaId2)
                                                                .AreaName;
                        var customGroup =
                            PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == meterMap.CustomGroup1);
                        if (customGroup != null)
                            aggMeter.Suburb = customGroup.DisplayName;
                    }
                    aggMeter.CollectionRunId = collectionRouteID;
                    aggMeter.Street = meter.Location;
                    aggMeter.AmtMeter = cdi.AmountMeter ?? 0;

                    aggMeter.AmtChip = cdi.AmountChip ?? 0;
                    //aggMeter.AmtVendor = cdi.AmtManual ?? 0;
                    //difference flag is if any of the following is different (amount, amt manual, amt auto)
                    aggMeter.DifferenceFlag = !AllEqual(aggMeter.AmtMeter, aggMeter.AmtChip, aggMeter.AmtVendor);

                    //get the corrosponding colldatasum for this day (first one, if exists) where date and meterID match. we already filtered on Date, so dont need o do that here
                    var collDataSum =
                        cds.FirstOrDefault(
                            x => x.MeterId == cdi.MeterId && x.AreaId == cdi.AreaId && x.CustomerId == cdi.CustomerID);

                    if (collDataSum != null) aggMeter.AmtMeter = cdi.AmountMeter;// collDataSum.Amount;
                    aggMeters.Add(aggMeter);
                }
            }

            details.Meters = aggMeters;
            return details;
        }

        /// <summary>
        /// Populates a list of meters for the collection aggregation. Uses the Collection data summ and cash box data import to determine meters to use.
        /// </summary>
        private AggregationDetails PopulateAggregationMeters(AggregationDetails details,
                                                             long collectionRouteID,
                                                             int custID, DateTime collectionDateTime, long collectionRunReportId)
        {
            var RouteMetersDetailsV =
                PemsEntities.RouteMetersDetailsV2.Where(
                       x => x.CollectionRunReportId == collectionRunReportId).ToList();

            var aggMeters = new List<AggregationMeter>();
            foreach (var cdi in RouteMetersDetailsV)
            {
                var aggMeter = new AggregationMeter
                {
                    MeterId = cdi.MeterId,
                    MeterName = cdi.MeterName,
                    Street = cdi.Street,
                    DateTime = cdi.DateTime,
                    CollectionDateTime = collectionDateTime,
                    CustomerId = custID,
                    AreaId = cdi.AreaId,
                    CollectionRunId = cdi.CollectionRunId,
                    Area = cdi.Area,
                    Zone = cdi.Zone,
                    Suburb = cdi.Suburb,
                    //AmtMeter = cdi.AmountMeter ?? 0,
                    AmtMeter = ConvertCentToDollar(cdi.AmountMeter),
                    AmtChip = cdi.AmountChip ?? 0,
                };
                aggMeter.DifferenceFlag = !AllEqual(aggMeter.AmtMeter, aggMeter.AmtChip, aggMeter.AmtVendor);
               
                aggMeters.Add(aggMeter);
            }

            details.Meters = aggMeters;
            return details;
        }

        private double? ConvertCentToDollar(int? cents)
        {
            if(cents.HasValue)
            {
                return (double)cents.Value / 100;
            }
            else
            {
                return 0;
            }
           
        }

        /// <summary>
        /// Populate the aggregation summary information for a colleciton aggregation based on the cashbox data import and Coll data summ passed in.
        /// </summary>
        private static AggregationDetails PopulateAggregationData(AggregationDetails details,
                                                                  IQueryable<CollectionRunMeter> crms,
                                                                  CollectionRunReport collectionRunReport,
                                                                  List<CashBoxDataImport> cdis,
                                                                  List<CollDataSumm> cds)
        {
            details.MetersToCollect = crms.Count();
            if (collectionRunReport != null)
            {
                details.TotalCashCollected = collectionRunReport.TotalManualCashAmt == 0 ? 0.0 : collectionRunReport.TotalManualCashAmt / 100.0;
                details.TotalMetersCollected = collectionRunReport.TotalMeterCount;
                details.TotalMetersNotCollected = details.MetersToCollect - details.TotalMetersCollected;

                //totals
                details.TotalReportedByMeter = collectionRunReport.TotalMeterCashAmt == 0 ? 0.0 : collectionRunReport.TotalMeterCashAmt / 100.0;
                details.TotalReportedByVendor = collectionRunReport.TotalManualCashAmt == 0 ? 0.0 : collectionRunReport.TotalManualCashAmt / 100.0;
                details.TotalReportedByChip = collectionRunReport.TotalByChip == 0 ? 0.0 : collectionRunReport.TotalByChip / 100.0;

            }

            //differences
            details.TotalDifferenceMeterToChip = (details.TotalReportedByMeter - details.TotalReportedByChip) ?? 0;
            details.TotalDifferenceMeterToVendor = (details.TotalReportedByMeter - details.TotalReportedByVendor) ?? 0;
            details.TotalDifferenceVendorToChip = (details.TotalReportedByVendor - details.TotalReportedByChip) ?? 0;
            //details.MaxAmtCollectedChip = collectionRunReport.MaxAmt_Chip??0;
            
            if (details.TotalMetersCollected > 0)
            {
                //averages
                details.AverageReportedByMeter = details.TotalReportedByMeter / details.TotalMetersCollected;
                details.AverageReportedByVendor = details.TotalReportedByVendor / details.TotalMetersCollected;
                details.AverageReportedByChip = details.TotalReportedByChip / details.TotalMetersCollected;

                //average differences
                details.AverageDifferenceMeterToChip = (details.TotalDifferenceMeterToChip / details.TotalMetersCollected) ??
                                                       0;
                details.AverageDifferenceMeterToVendor = (details.TotalDifferenceMeterToVendor /
                                                          details.TotalMetersCollected) ?? 0;
                details.AverageDifferenceVendorToChip = (details.TotalDifferenceVendorToChip /
                                                         details.TotalMetersCollected) ?? 0;
            }
            //details.MaxAmtCollectedChip = collectionRunReport.MaxAmt_Chip ?? 0;
            //details.MaxAmtCollectedMeter = collectionRunReport.MaxAmt_Meter ?? 0.0;
            //Maximums
            //double maxMeter = 0;
            //if (cds.Any())
            //   maxMeter = cds.Max(x => x.Amount);
            if (collectionRunReport != null) //Changed By Rajesh on 16/7/2015
            {
                details.MaxAmtCollectedMeter = collectionRunReport.MaxAmt_Meter ?? 0.0;
            }
            double maxChip = 0;
            double maxVendor = 0;
            if (cdis.Any())
            {
                maxChip = cdis.Max(x => x.AmtAuto) ?? 0;
                maxVendor = cdis.Max(x => x.AmtManual) ?? 0;

            }
            if (collectionRunReport != null) //Changed By Rajesh on 16/7/2015
            {
                details.MaxAmtCollectedChip = collectionRunReport.MaxAmt_Chip ?? 0;// maxChip;
            }
            details.MaxAmtCollectedVendor = maxVendor;
            
            return details;
        }

        /// <summary>
        /// Gets the collection aggregations meter information for a specific meter.
        /// </summary>
        public AggregationMeterDetails GetAggregationMeterDetails(int meterId, string customerId, DateTime dateTime,
                                                                  DateTime collectionDateTime, int areaId,
                                                                  long collectionRunId)
        {
            int custID;
            int.TryParse(customerId, out custID);
            //required - if we dont have this item, then we dont ahve anything. we must get the CashBoxDataIMport to use as the main table
            var cdi =
                PemsEntities.CashBoxDataImports.FirstOrDefault(
                    x =>
                   x.CustomerId == custID && x.DateTimeRead == dateTime && x.AreaId == areaId && x.MeterId == meterId);
            if (cdi != null)
            {
                //get the meter
                var meter = PemsEntities.Meters.FirstOrDefault(x => x.MeterId == meterId);
                if (meter != null)
                {
                    //set basic meter information
                    var aggMeter = new AggregationMeterDetails
                        {
                            MeterId = meter.MeterId,
                            MeterName = meter.MeterName,
                            DateTime = cdi.DateTimeRead,
                            AreaId = meter.AreaID,
                            CustomerId = custID,
                            Street = meter.Location,
                            Latitude = meter.Latitude,
                            Longitude = meter.Longitude,
                            CollectionDate = collectionDateTime,
                            CollectionRunId = collectionRunId
                        };

                    //now set meter map info
                    var meterMap =
                        PemsEntities.MeterMaps.FirstOrDefault(
                            x => x.MeterId == meterId && x.Customerid == custID && x.Areaid == meter.AreaID);
                    if (meterMap != null)
                    {
                        aggMeter.ZoneId = meterMap.ZoneId;
                        aggMeter.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName;
                        aggMeter.AreaId2 = meterMap.AreaId2;
                        aggMeter.Area = meterMap.AreaId2 == null
                                            ? ""
                                            : PemsEntities.Areas.FirstOrDefault(
                                                x => x.CustomerID == custID && x.AreaID == meterMap.AreaId2) == null
                                                  ? ""
                                                  : PemsEntities.Areas.FirstOrDefault(
                                                      x => x.CustomerID == custID && x.AreaID == meterMap.AreaId2)
                                                                .AreaName;
                        var customGroup =
                            PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == meterMap.CustomGroup1);
                        if (customGroup != null)
                            aggMeter.Suburb = customGroup.DisplayName;
                    }

                    //now try to get the colldatasum for this entry. (first and if exists, cause it might not alwyas be there)
                    //linq doesnt support Date, so will have to generate a start and end date
                    var currentStartDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                    //this will get the start of the day
                    var currentEndDate = currentStartDate.AddDays(1).AddMilliseconds(-1);
                    var collDataSum =
                        PemsEntities.CollDataSumms.FirstOrDefault(
                            x =>
                            x.CustomerId == custID && x.CollDateTime >= currentStartDate &&
                            x.CollDateTime <= currentEndDate && x.AreaId == areaId && x.MeterId == meterId);

                    //colleciton information
                    if (collDataSum != null)
                    {
                        aggMeter.CashBoxIdInserted = collDataSum.NewCashBoxID;
                        aggMeter.CashBoxIdRemoved = collDataSum.OldCashBoxID;
                        aggMeter.InsertionTimeMeter = collDataSum.InsertionDateTime;
                        aggMeter.RemovalTimeMeter = collDataSum.CollDateTime;
                        aggMeter.NewCashboxIdInserted = collDataSum.NewCashBoxID;
                        aggMeter.CollectorId = collDataSum.CollectorId ?? 0;
                        aggMeter.MeterAmount = collDataSum.Amount;
                    }

                    aggMeter.InsertionTimeChip = cdi.DateTimeIns;
                    aggMeter.RemovalTimeChip = cdi.DateTimeRem;
                    aggMeter.ReadTime = cdi.DateTimeRead;
                    aggMeter.TransactionLogFileName = cdi.FileName;
                    aggMeter.TimeActive = cdi.TimeActive ?? 0;
                    aggMeter.SequenceNumber = cdi.CashboxSequenceNo;
                    aggMeter.PercentageFull = cdi.PercentFull ?? 0;
                    aggMeter.Version = cdi.FirmwareVer ?? 0;
                    aggMeter.OperatorId = cdi.OperatorId;

                    //Revenue Collected
                    aggMeter.ChipAmount = cdi.AmtAuto ?? 0;
                    aggMeter.VendorAmount = cdi.AmtManual ?? 0;
                    aggMeter.DifferenceMeterChip = (aggMeter.MeterAmount - aggMeter.ChipAmount) ?? 0;
                    aggMeter.DifferenceMeterVendor = (aggMeter.MeterAmount - aggMeter.VendorAmount) ?? 0;
                    aggMeter.DifferenceVendorChip = (aggMeter.VendorAmount - aggMeter.ChipAmount) ?? 0;

                    //display coin demonination information for this customer

                    //get  a list of coin denominations for this customer
                    var customerCoinsDenoms = PemsEntities.CoinDenominationCustomers.Where(x => x.CustomerId == custID);

                    //for each of those, set the displaytyepX to true if isdisplay is true, otherwise false. then set the display name for the fields that we care about
                    foreach (var coinDenominationCustomer in customerCoinsDenoms.Where(x => x.IsDisplay))
                    {
                        //try to get the correct coin ttype
                        var denomNameEnum = CashBoxCoinDenomination.Unknown;
                        Enum.TryParse(coinDenominationCustomer.CoinDenomination.CashBoxDataImportMap.Trim(),
                                      out denomNameEnum);

                        //switch, and set display name and value and isdisplay
                        switch (denomNameEnum)
                        {
                            case CashBoxCoinDenomination.Dollar2Coins:
                                aggMeter.DisplayType1 = true;
                                aggMeter.DisplayNameType1 = coinDenominationCustomer.CoinName;
                                break;
                            case CashBoxCoinDenomination.Dollar1Coins:
                                aggMeter.DisplayType2 = true;
                                aggMeter.DisplayNameType2 = coinDenominationCustomer.CoinName;
                                break;
                            case CashBoxCoinDenomination.Cents50Coins:
                                aggMeter.DisplayType3 = true;
                                aggMeter.DisplayNameType3 = coinDenominationCustomer.CoinName;
                                break;
                            case CashBoxCoinDenomination.Cents20Coins:
                                aggMeter.DisplayNameType4 = coinDenominationCustomer.CoinName;
                                aggMeter.DisplayType4 = true;
                                break;
                            case CashBoxCoinDenomination.Cents10Coins:
                                aggMeter.DisplayType5 = true;
                                aggMeter.DisplayNameType5 = coinDenominationCustomer.CoinName;
                                break;
                            case CashBoxCoinDenomination.Cents5Coins:
                                aggMeter.DisplayType6 = true;
                                aggMeter.DisplayNameType6 = coinDenominationCustomer.CoinName;
                                break;
                        }
                    }

                    //set the values no matter what, incase we need to pass this model off to another action
                    aggMeter.CoinType1Count = cdi.Dollar2Coins ?? 0;
                    aggMeter.CoinType2Count = cdi.Dollar1Coins ?? 0;
                    aggMeter.CoinType3Count = cdi.Cents50Coins ?? 0;
                    aggMeter.CoinType4Count = cdi.Cents20Coins ?? 0;
                    aggMeter.CoinType5Count = cdi.Cents10Coins ?? 0;
                    aggMeter.CoinType6Count = cdi.Cents5Coins ?? 0;

                    return aggMeter;
                }
            }
            //Changed By Rajesh On 22 July 2015
            if (cdi == null)
            {
                //ananga commented
                //var cdiCustomer =
                //     PemsEntities.RouteMetersDetailsVs.Where(
                //            x => x.CustomerID == custID && x.AreaId == areaId && x.MeterId == meterId && x.DateTime == dateTime).ToList();

                var cdiCustomer =
                    PemsEntities.RouteMetersDetailsV2.Where(
                           x => x.CustomerID == custID && x.AreaId == areaId && x.MeterId == meterId && x.DateTime == dateTime).ToList();


                AggregationMeterDetails aggMeterdetail = new AggregationMeterDetails();
                if (cdiCustomer != null)
                {
                    var meter = PemsEntities.Meters.FirstOrDefault(x => x.MeterId == meterId);

                    if (meter != null)
                    {
                        aggMeterdetail.MeterId = meter.MeterId;
                        aggMeterdetail.MeterName = meter.MeterName;
                        aggMeterdetail.DateTime = dateTime;
                        aggMeterdetail.AreaId = meter.AreaID;
                        aggMeterdetail.CustomerId = meter.CustomerID;
                        aggMeterdetail.Street = meter.Location;
                        aggMeterdetail.Latitude = meter.Latitude;
                        aggMeterdetail.Longitude = meter.Longitude;
                        aggMeterdetail.CollectionDate = collectionDateTime;
                        aggMeterdetail.CollectionRunId = collectionRunId;
                    };
                    var meterMap =
                           PemsEntities.MeterMaps.FirstOrDefault(
                               x => x.MeterId == meterId && x.Customerid == custID && x.Areaid == meter.AreaID);
                    if (meterMap != null)
                    {
                        aggMeterdetail.ZoneId = meterMap.ZoneId;
                        aggMeterdetail.Zone = meterMap.Zone == null ? "" : meterMap.Zone.ZoneName;
                        aggMeterdetail.AreaId2 = meterMap.AreaId2;
                        aggMeterdetail.Area = meterMap.AreaId2 == null
                                            ? ""
                                            : PemsEntities.Areas.FirstOrDefault(
                                                x => x.CustomerID == custID && x.AreaID == meterMap.AreaId2) == null
                                                  ? ""
                                                  : PemsEntities.Areas.FirstOrDefault(
                                                      x => x.CustomerID == custID && x.AreaID == meterMap.AreaId2)
                                                                .AreaName;
                        var customGroup =
                            PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomGroupId == meterMap.CustomGroup1);
                        if (customGroup != null)
                            aggMeterdetail.Suburb = customGroup.DisplayName;
                    }
                    //now try to get the colldatasum for this entry. (first and if exists, cause it might not alwyas be there)
                    //linq doesnt support Date, so will have to generate a start and end date
                    var currentStartDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                    //this will get the start of the day
                    var currentEndDate = currentStartDate.AddDays(1).AddMilliseconds(-1);
                    var collDataSum =
                        PemsEntities.CollDataSumms.FirstOrDefault(
                            x =>
                            x.CustomerId == custID && x.CollDateTime >= currentStartDate &&
                            x.CollDateTime <= currentEndDate && x.AreaId == areaId && x.MeterId == meterId);

                    //colleciton information
                    if (collDataSum != null)
                    {
                        aggMeterdetail.CashBoxIdInserted = collDataSum.NewCashBoxID;
                        aggMeterdetail.CashBoxIdRemoved = collDataSum.OldCashBoxID;
                        aggMeterdetail.InsertionTimeMeter = collDataSum.InsertionDateTime;
                        aggMeterdetail.RemovalTimeMeter = collDataSum.CollDateTime;
                        aggMeterdetail.NewCashboxIdInserted = collDataSum.NewCashBoxID;
                        aggMeterdetail.CollectorId = collDataSum.CollectorId ?? 0;
                        aggMeterdetail.MeterAmount = collDataSum.Amount;
                    }
                    if (cdi != null)
                    {
                        aggMeterdetail.InsertionTimeChip = cdi.DateTimeIns;
                        aggMeterdetail.InsertionTimeChip = cdi.DateTimeIns;
                        aggMeterdetail.RemovalTimeChip = cdi.DateTimeRem;
                        aggMeterdetail.ReadTime = cdi.DateTimeRead;
                        aggMeterdetail.TransactionLogFileName = cdi.FileName;
                        aggMeterdetail.TimeActive = cdi.TimeActive ?? 0;
                        aggMeterdetail.SequenceNumber = cdi.CashboxSequenceNo;
                        aggMeterdetail.PercentageFull = cdi.PercentFull ?? 0;
                        aggMeterdetail.Version = cdi.FirmwareVer ?? 0;
                        aggMeterdetail.OperatorId = cdi.OperatorId;

                        //Revenue Collected
                        aggMeterdetail.ChipAmount = cdi.AmtAuto ?? 0;
                        aggMeterdetail.VendorAmount = cdi.AmtManual ?? 0;
                        aggMeterdetail.DifferenceMeterChip = (aggMeterdetail.MeterAmount - aggMeterdetail.ChipAmount) ?? 0;
                        aggMeterdetail.DifferenceMeterVendor = (aggMeterdetail.MeterAmount - aggMeterdetail.VendorAmount) ?? 0;
                        aggMeterdetail.DifferenceVendorChip = (aggMeterdetail.VendorAmount - aggMeterdetail.ChipAmount) ?? 0;
                    }
                    foreach (var cdiCust in cdiCustomer)
                    {
                        aggMeterdetail.ChipAmount = cdiCust.AmountChip ?? 0;
                        aggMeterdetail.MeterAmount = cdiCust.AmountMeter ?? 0;
                        aggMeterdetail.DifferenceMeterChip = (aggMeterdetail.MeterAmount - aggMeterdetail.ChipAmount) ?? 0;
                    }
                    //display coin demonination information for this customer
                    //get  a list of coin denominations for this customer
                    if (cdi != null)
                    {
                        var customerCoinsDenoms = PemsEntities.CoinDenominationCustomers.Where(x => x.CustomerId == custID);
                        //for each of those, set the displaytyepX to true if isdisplay is true, otherwise false. then set the display name for the fields that we care about
                        foreach (var coinDenominationCustomer in customerCoinsDenoms.Where(x => x.IsDisplay))
                        {
                            //try to get the correct coin ttype
                            var denomNameEnum = CashBoxCoinDenomination.Unknown;
                            Enum.TryParse(coinDenominationCustomer.CoinDenomination.CashBoxDataImportMap.Trim(),
                                          out denomNameEnum);

                            //switch, and set display name and value and isdisplay
                            switch (denomNameEnum)
                            {
                                case CashBoxCoinDenomination.Dollar2Coins:
                                    aggMeterdetail.DisplayType1 = true;
                                    aggMeterdetail.DisplayNameType1 = coinDenominationCustomer.CoinName;
                                    break;
                                case CashBoxCoinDenomination.Dollar1Coins:
                                    aggMeterdetail.DisplayType2 = true;
                                    aggMeterdetail.DisplayNameType2 = coinDenominationCustomer.CoinName;
                                    break;
                                case CashBoxCoinDenomination.Cents50Coins:
                                    aggMeterdetail.DisplayType3 = true;
                                    aggMeterdetail.DisplayNameType3 = coinDenominationCustomer.CoinName;
                                    break;
                                case CashBoxCoinDenomination.Cents20Coins:
                                    aggMeterdetail.DisplayNameType4 = coinDenominationCustomer.CoinName;
                                    aggMeterdetail.DisplayType4 = true;
                                    break;
                                case CashBoxCoinDenomination.Cents10Coins:
                                    aggMeterdetail.DisplayType5 = true;
                                    aggMeterdetail.DisplayNameType5 = coinDenominationCustomer.CoinName;
                                    break;
                                case CashBoxCoinDenomination.Cents5Coins:
                                    aggMeterdetail.DisplayType6 = true;
                                    aggMeterdetail.DisplayNameType6 = coinDenominationCustomer.CoinName;
                                    break;
                            }
                        }
                    }
                    //set the values no matter what, incase we need to pass this model off to another action
                    if (cdi != null)
                    {
                        aggMeterdetail.CoinType1Count = cdi.Dollar2Coins ?? 0;
                        aggMeterdetail.CoinType2Count = cdi.Dollar1Coins ?? 0;
                        aggMeterdetail.CoinType3Count = cdi.Cents50Coins ?? 0;
                        aggMeterdetail.CoinType4Count = cdi.Cents20Coins ?? 0;
                        aggMeterdetail.CoinType5Count = cdi.Cents10Coins ?? 0;
                        aggMeterdetail.CoinType6Count = cdi.Cents5Coins ?? 0;
                    }
                    return aggMeterdetail;
                }
            }
            return new AggregationMeterDetails();
        }

        #endregion

        //TODO - GTC: flesh out these methods

        #region Grouped Aggregation Details

        //todo - gtc: going to need the grouped versions of the following methods (at least):
        //GetGroupedAggregationDetails, PopulateGroupedAddMeters, PopulateGroupedAggregationData,  GetGroupedAggregationSingleSpaceMeterDetails
        //also might need to create any methods / entities required since they have different data elements.

        #endregion

        #region "Collection Details"

        /// <summary>
        /// Gets the details of a collection route
        /// </summary>
        public CollectionConfiguration GetCollectionConfiguration(long routeId, int customerId,
                                                                  bool populateOptions = false,
                                                                  bool populateAllMeters = false)
        {
            var collectionRun = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == routeId);
            if (collectionRun != null)
            {
                var collectionConfig = new CollectionConfiguration
                    {
                        ActivationDate = collectionRun.ActivationDate,
                        CollectionId = routeId,
                        CollectionName = collectionRun.CollectionRunName,
                        DaysBtwCollections = collectionRun.DaysBetweenCol ?? 0,
                        SkipFriday = collectionRun.SkipSpecificDaysOfWeekFriday,
                        SkipMonday = collectionRun.SkipSpecificDaysOfWeekMonday,
                        SkipPublicHolidays = collectionRun.SkipPublicHolidays ?? false,
                        SkipSaturday = collectionRun.SkipSpecificDaysOfWeekSaturday,
                        SkipSunday = collectionRun.SkipSpecificDaysOfWeekSunday,
                        SkipThursday = collectionRun.SkipSpecificDaysOfWeekThursday,
                        SkipTuesday = collectionRun.SkipSpecificDaysOfWeekTuesday,
                        SkipWednesday = collectionRun.SkipSpecificDaysOfWeekWednesday,
                        StatusId = collectionRun.CollectionRunStatus,

                        VendorId = collectionRun.VendorId,
                        Meters = new List<CollectionMeter>(),
                        AllMeters = new List<CollectionMeter>(),

                        DateCreated = collectionRun.DateCreated,
                        CreatedById = collectionRun.CreatedBy,
                        LastEditedOn = collectionRun.LastEdited,
                        LastEditedById = collectionRun.LastEditedBy

                    };

                //set the status and vendor text
                var status =
                    PemsEntities.CollectionRunStatus.FirstOrDefault(
                        x => x.CollectionRunStatusId == collectionRun.CollectionRunStatus);
                if (status != null)
                    collectionConfig.Status = status.CollectionRunStatusDesc;
                var vendor =
                    PemsEntities.CollectionRunVendors.FirstOrDefault(
                        x => x.CollectionRunVendorId == collectionRun.VendorId);
                if (vendor != null)
                    collectionConfig.VendorName = vendor.CollectionRunVendorName;

                //now set the meters - only grab the meters that have the correct customer ID
                foreach (var crMeter in collectionRun.CollectionRunMeters.Where(x => x.CustomerId == customerId))
                {
                    //get the meter.
                    var dbMeter = crMeter.Meter;
                    if (dbMeter != null)
                    {
                        var meterGroup = dbMeter.MeterGroup1;
                        //  var meterMap = dbMeter.MeterMaps.FirstOrDefault();
                        var meterMap =
                            PemsEntities.MeterMaps.FirstOrDefault(
                                x =>
                                x.MeterId == dbMeter.MeterId && x.Customerid == dbMeter.CustomerID &&
                                x.Areaid == dbMeter.AreaID);
                        var meter = new CollectionMeter
                            {
                                AreaId = crMeter.AreaId,
                                AreaId2 = meterMap == null ? (int?)null : meterMap.AreaId2,
                                Zone = meterMap == null ? "" : meterMap.Zone == null ? "" : meterMap.Zone.ZoneName,
                                Area =
                                    meterMap == null
                                        ? ""
                                        : meterMap.AreaId2 == null
                                              ? ""
                                              : PemsEntities.Areas.FirstOrDefault(
                                                  x =>
                                                  x.CustomerID == dbMeter.CustomerID && x.AreaID == meterMap.AreaId2) ==
                                                null
                                                    ? ""
                                                    : PemsEntities.Areas.FirstOrDefault(
                                                        x =>
                                                        x.CustomerID == dbMeter.CustomerID &&
                                                        x.AreaID == meterMap.AreaId2).AreaName,
                                CustomerId = crMeter.CustomerId,
                                MeterId = crMeter.MeterId,
                                MeterName = dbMeter.MeterName,
                                Street = dbMeter.Location,
                                CollectionRunId = meterMap == null ? (long?)null : meterMap.CollRouteId,
                                CollectionName = meterMap == null
                                                     ? ""
                                                     : meterMap.CollRouteId == null
                                                           ? ""
                                                           : PemsEntities.CollectionRuns.FirstOrDefault(
                                                               x => x.CollectionRunId == meterMap.CollRouteId) == null
                                                                 ? ""
                                                                 : PemsEntities.CollectionRuns.FirstOrDefault(
                                                                     x => x.CollectionRunId == meterMap.CollRouteId)
                                                                               .CollectionRunName,
                                CollectionRunMeterID = crMeter.CollectionRunMeterId,
                                Suburb = meterGroup == null ? "" : meterGroup.MeterGroupDesc
                            };
                        collectionConfig.Meters.Add(meter);
                    }
                }

                //now set the vendor and status opions if needed
                if (populateOptions)
                {
                    collectionConfig.StatusOptions = GetStatuses();
                    collectionConfig.VendorOptions = GetVendors(customerId);
                }



                //get al th meters for te eidt if needed. should only get meters with metergroup of single space or multi space
                if (populateAllMeters)
                    collectionConfig.AllMeters =
                        (new MeterFactory(ConnectionStringName, DateTime.Now).GetAllMeters(customerId));

                var userFac = new UserFactory();
                //now fillt he edited and created text here
                if (collectionConfig.CreatedById.HasValue)
                    collectionConfig.CreatedBy = userFac.GetUsername(collectionConfig.CreatedById.Value);
                if (collectionConfig.LastEditedById.HasValue)
                    collectionConfig.LastEditedBy = userFac.GetUsername(collectionConfig.LastEditedById.Value);

                collectionConfig.CustomerId = customerId;
                return collectionConfig;
            }
            return new CollectionConfiguration();
        }




        public List<CollectionMeter> GetMetersForCollection(int customerId, string locationtype, string locationvalue)
        {
            var items = new List<CollectionMeter>();
            List<string> types = locationvalue.Split(',').Select(x => x.Trim()).Select(x => x.ToString()).ToList();

            if (locationvalue == "All")
            {
                items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         select new CollectionMeter
                         {
                             MeterId = m.MeterId,
                             MeterName = m.MeterName,
                             Street = m.Location,
                             CustomerId = m.CustomerID,
                             AreaId = mm.Areaid,
                             AreaId2 = mm.AreaId2,
                             Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                             Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                             ZoneId = mm.ZoneId,
                             Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                             CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                         }).ToList();

                var query = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         select new CollectionMeter
                         {
                             MeterId = m.MeterId,
                             MeterName = m.MeterName,
                             Street = m.Location,
                             CustomerId = m.CustomerID,
                             AreaId = mm.Areaid,
                             AreaId2 = mm.AreaId2,
                             Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                             Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                             ZoneId = mm.ZoneId,
                             Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                             CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                         });


            }
            else if (locationtype == "Area")
            {
                items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         join a in PemsEntities.Areas on m.AreaID
                                        equals a.AreaID
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         where types.Contains(a.AreaName)
                         select new CollectionMeter
                         {
                             MeterId = m.MeterId,
                             MeterName = m.MeterName,
                             Street = m.Location,
                             CustomerId = m.CustomerID,
                             AreaId = mm.Areaid,
                             AreaId2 = mm.AreaId2,
                             Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                             Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                             ZoneId = mm.ZoneId,
                             Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                             CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                         }).ToList();


            }
            else if (locationtype == "Zone")
            {
                items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         join z in PemsEntities.Zones on mm.ZoneId
                                      equals z.ZoneId
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         where types.Contains(z.ZoneName)
                         select new CollectionMeter
                         {
                             MeterId = m.MeterId,
                             MeterName = m.MeterName,
                             Street = m.Location,
                             CustomerId = m.CustomerID,
                             AreaId = mm.Areaid,
                             AreaId2 = mm.AreaId2,
                             Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                             Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                             ZoneId = mm.ZoneId,
                             Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                             CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                         }).ToList();


            }
            else if (locationtype == "Street")
            {
                items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         where types.Contains(m.Location)
                         select new CollectionMeter
                         {
                             MeterId = m.MeterId,
                             MeterName = m.MeterName,
                             Street = m.Location,
                             CustomerId = m.CustomerID,
                             AreaId = mm.Areaid,
                             AreaId2 = mm.AreaId2,
                             Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                             Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                             ZoneId = mm.ZoneId,
                             Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                             CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                         }).ToList();


            }
            else if (locationtype == "Suburb")
            {
                items = (from m in PemsEntities.Meters
                         from mm in PemsEntities.MeterMaps
                         join c in PemsEntities.CustomGroup1 on mm.CustomGroup1
                                      equals c.CustomGroupId
                         where (!m.MeterState.HasValue || m.MeterState != (int)AssetStateType.Historic)
                         where m.MeterId == mm.MeterId
                         where m.AreaID == mm.Areaid
                         where m.CustomerID == mm.Customerid
                         where m.CustomerID == customerId
                         where (m.MeterGroup == 0 || m.MeterGroup == 1)
                         where types.Contains(c.DisplayName)
                         select new CollectionMeter
                         {
                             MeterId = m.MeterId,
                             MeterName = m.MeterName,
                             Street = m.Location,
                             CustomerId = m.CustomerID,
                             AreaId = mm.Areaid,
                             AreaId2 = mm.AreaId2,
                             Zone = mm.Zone == null ? "" : mm.Zone.ZoneName,
                             Area = mm.AreaId2 == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2) == null ? "" : PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == customerId && x.AreaID == mm.AreaId2).AreaName,
                             ZoneId = mm.ZoneId,
                             Suburb = mm.CustomGroup11 == null ? "" : mm.CustomGroup11.DisplayName,
                             CollectionName = mm.CollectionRun == null ? "" : mm.CollectionRun.CollectionRunName
                         }).ToList();


            }
            return items;
        }



        /// <summary>
        /// removes a meter from the collection run (colleciton run meters table) based ont he colleciton run meter ID
        /// </summary>
        public void RemoveCollectionRunMeter(long id)
        {
            var crMeter = PemsEntities.CollectionRunMeters.FirstOrDefault(x => x.CollectionRunMeterId == id);
            if (crMeter != null)
            {
                PemsEntities.CollectionRunMeters.Remove(crMeter);
                PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// removes all meters from the collection run (colleciton run meters table) based ont he colleciton run meter ID
        /// </summary>
        public void RemoveCollectionRunMeters(long crId)
        {
            var crMeters = PemsEntities.CollectionRunMeters.Where(x => x.CollectionRunId == crId).ToList();
            if (crMeters.Any())
            {
                foreach (var collectionRunMeter in crMeters)
                    PemsEntities.CollectionRunMeters.Remove(collectionRunMeter);
                PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a colleciton route based on the collection configuration passed in.
        /// </summary>
        public void EditCollection(CollectionConfiguration collectionConfig)
        {
            //get the collectionRun
            var collectionRun =
                PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == collectionConfig.CollectionId);
            if (collectionRun != null)
            {
                //update the values base don the model passed in
                //do not update id or name, these wont change.
                collectionRun.ActivationDate = collectionConfig.ActivationDate;
                var collectionRunStatu =
                    PemsEntities.CollectionRunStatus.FirstOrDefault(
                        x => x.CollectionRunStatusDesc == collectionConfig.Status);
                if (collectionRunStatu != null)
                    collectionRun.CollectionRunStatus = collectionRunStatu.CollectionRunStatusId;
                collectionRun.VendorId = collectionConfig.VendorId;
                collectionRun.DaysBetweenCol = collectionConfig.DaysBtwCollections;
                collectionRun.SkipPublicHolidays = collectionConfig.SkipPublicHolidays;
                collectionRun.SkipSpecificDaysOfWeek = collectionConfig.SkipDaysOfWeek;
                collectionRun.SkipSpecificDaysOfWeekFriday = collectionConfig.SkipFriday;
                collectionRun.SkipSpecificDaysOfWeekMonday = collectionConfig.SkipMonday;
                collectionRun.SkipSpecificDaysOfWeekSaturday = collectionConfig.SkipSaturday;
                collectionRun.SkipSpecificDaysOfWeekSunday = collectionConfig.SkipSunday;
                collectionRun.SkipSpecificDaysOfWeekThursday = collectionConfig.SkipThursday;
                collectionRun.SkipSpecificDaysOfWeekTuesday = collectionConfig.SkipTuesday;
                collectionRun.SkipSpecificDaysOfWeekWednesday = collectionConfig.SkipWednesday;
                collectionRun.LastEdited = DateTime.Now;
                collectionRun.LastEditedBy = WebSecurity.CurrentUserId;

                //we dont haveto worry about meters, that is done seperately

                //save it
                PemsEntities.SaveChanges();
            }
        }



        public IEnumerable<CollectionConfiguration> GetLocationTypeId(string locationType, int CurrentCity)
        {
            IEnumerable<CollectionConfiguration> details = Enumerable.Empty<CollectionConfiguration>();

            try
            {

                if (locationType == "Area")
                {
                    details = (from area in PemsEntities.Areas
                               where area.CustomerID == CurrentCity && area.AreaID != null
                               where (area.AreaID != 99 && area.AreaID != 98 && area.AreaID != 97 && area.AreaID != 1)
                               select new CollectionConfiguration
                               {
                                   Value = area.AreaID,
                                   Text = area.AreaName
                               }).Distinct();


                }
                else if (locationType == "Zone")
                {

                    details = (from zone in PemsEntities.Zones
                               where zone.customerID == CurrentCity && zone.ZoneId != null
                               select new CollectionConfiguration
                               {
                                   Value = zone.ZoneId,
                                   Text = zone.ZoneName
                               }).Distinct();

                }
                else if (locationType == "Street")
                {
                    details = (from meter in PemsEntities.Meters
                               where meter.CustomerID == CurrentCity && meter.Location != null
                               select new CollectionConfiguration
                               {
                                   Text = meter.Location,
                               }).Distinct();
                }
                else if (locationType == "Suburb")
                {
                    details = (from customGroup in PemsEntities.CustomGroup1
                               where customGroup.CustomerId == CurrentCity && customGroup.DisplayName != null
                               select new CollectionConfiguration
                               {
                                   Text = customGroup.DisplayName
                               }).Distinct();


                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetLocationTypeId Method (GIS Reports)", ex);
            }


            return details;
        }


        #endregion

        #region "Collection Copy"

        /// <summary>
        /// Clones a collection route and returns the newly created collection
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CollectionConfiguration CopyCollection(CollectionConfiguration model)
        {
            //get the original collection run
            var originalCr = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == model.CollectionId);
            if (originalCr != null)
            {
                var newCr = MakeCollectionRun(model);
                //save it so it generates the correct ID
                PemsEntities.CollectionRuns.Add(newCr);
                PemsEntities.SaveChanges();

                var originalAutoDetect = PemsEntities.Configuration.AutoDetectChangesEnabled;
                var originalValudate = PemsEntities.Configuration.ValidateOnSaveEnabled;
                PemsEntities.Configuration.AutoDetectChangesEnabled = false;
                PemsEntities.Configuration.ValidateOnSaveEnabled = false;

                //now we should have a valid ID for the collection route. Lets copy all the meters from the original to the new
                foreach (var meter in originalCr.CollectionRunMeters)
                {

                    var newMeter = new CollectionRunMeter
                        {
                            AreaId = meter.AreaId,
                            CollectionRunId = newCr.CollectionRunId,
                            CustomerId = model.CustomerId,
                            MeterId = meter.MeterId,
                            VendorId = model.VendorId.ToString()
                        };
                    PemsEntities.CollectionRunMeters.Add(newMeter);
                }
                PemsEntities.SaveChanges();
                PemsEntities.Configuration.AutoDetectChangesEnabled = originalAutoDetect;
                PemsEntities.Configuration.ValidateOnSaveEnabled = originalValudate;

                //now return the new collection configuration (so we can send them to the edit page for this colelction route)
                return GetCollectionConfiguration(newCr.CollectionRunId, model.CustomerId);
            }
            return null;
        }

        /// <summary>
        /// Adds a meter to a specific collection
        /// </summary>
        public MeterAdditionStatus AddMeterToCollection(int meterId, int areaId, long collectionRouteId, int customerId,
                                                        int? vendorId)
        {
            //NOTE: duncan determined that the only rule for editing collecitons is to allow adding meters to PENDING items. No validation on what meters to add. This means they can add the same meter mutpliple times to the same collection
            //they will have a backend process that filters, fixes all this nightly, so dont care if the meter is part of another run, etc. jsut add the meter, no questions asked
            //get all colleciton route IDs by customer, area, meter id
            //i am leaving the status login in here incase we need to use it again, will jsut be commented out. the calling method will still switch on the result and determine the course of action, we are jsut forcing Success

            //-----------------------------------------------------------------------------------------------------------
            //NOTE: 6/18/2013 - Duncan decided they wanted to ptu back in the validation jsut for the current collection run
            //-----------------------------------------------------------------------------------------------------------

            var status = new MeterAdditionStatus
                {
                    MeterId = meterId,
                    OriginalCollectionId = collectionRouteId,
                    Result = CollectionRouteResult.UnknownError
                };

            var collRouteIds =
                PemsEntities.CollectionRunMeters.Where(
                    x => x.MeterId == meterId && x.CustomerId == customerId && x.AreaId == areaId)
                            .Select(x => x.CollectionRunId)
                            .Distinct();

            ////if there were results, this meter is part of a collection route
            //check to see if its part of the current one.
            if (collRouteIds.Any())
            {
                if (collRouteIds.Contains(collectionRouteId))
                {
                    status.Result = CollectionRouteResult.ExistsCurrent;
                    status.ConflictingCollectionId = collectionRouteId;
                    return status;
                }
            }

            //if its not part of any colleciton route, then we need to mark it as success adn add the meter to the collection route
            status.Result = CollectionRouteResult.Success;
            //add the meter info to the colleciton run.

            //get the meter
            var newMeter = new CollectionRunMeter
                {
                    AreaId = areaId,
                    CollectionRunId = collectionRouteId,
                    CustomerId = customerId,
                    MeterId = meterId,
                    VendorId = vendorId > 0 ? vendorId.ToString() : null
                };
            PemsEntities.CollectionRunMeters.Add(newMeter);
            //create the crmeter and insert it
            PemsEntities.SaveChanges();
            return status;



            ////if we get here, its part of another collection route. just grab the first one and use that one (since they cannot be assigned to multiple ones)
            ////we will do a foraeach on the ids, and return on the first one we find.
            //foreach (var collRouteId in collRouteIds)
            //{
            //    var conflictingRoute =
            //       PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == collRouteId);
            //    if (conflictingRoute != null)
            //    {
            //        status.ConflictingCollectionId = collRouteId;
            //        //check active
            //        if (conflictingRoute.CollectionRunStatus == 1)
            //        {
            //            status.Result = CollectionRouteResult.ExistsActive;
            //            return status;
            //        }

            //        //check pending
            //        if (conflictingRoute.CollectionRunStatus == 2)
            //        {
            //            status.Result = CollectionRouteResult.ExistsPending;
            //            return status;
            //        }
            //    }
            //}

            //if we got here, we havent added or found out why, so leave it as unknown
            return status;
        }

        /// <summary>
        /// Creates a new collection route in the system
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public CollectionConfiguration CreateCollection(CollectionConfiguration model)
        {
            var newCr = MakeCollectionRun(model);
            //save it so it generates the correct ID
            PemsEntities.CollectionRuns.Add(newCr);
            PemsEntities.SaveChanges();
            return GetCollectionConfiguration(newCr.CollectionRunId, model.CustomerId);
        }

        /// <summary>
        /// Checks to see if the collection route with that name already exist for a specific customer
        /// </summary>
        public bool DoesCollectionExist(string collectionName, int customerId)
        {
            var items =
                PemsEntities.CollectionRuns.Where(
                    x => x.CollectionRunName.Trim() == collectionName.Trim() && x.CustomerId == customerId);
            if (items.Any())
                return true;
            return false;
        }

        /// <summary>
        /// Creates a collection run object based on a collectionconfiguratoin object (from the db)
        /// </summary>
        private CollectionRun MakeCollectionRun(CollectionConfiguration model)
        {
            //now lets make a new one
            var newCr = new CollectionRun
                {
                    ActivationDate = model.ActivationDate,
                    CollectionRunName = model.CollectionName.Trim(),
                    CollectionRunStatus = model.StatusId,
                    CreatedBy = WebSecurity.CurrentUserId,
                    DateCreated = DateTime.Now,
                    DaysBetweenCol = model.DaysBtwCollections,
                    LastEdited = DateTime.Now,
                    LastEditedBy = WebSecurity.CurrentUserId,
                    SkipPublicHolidays = model.SkipPublicHolidays,
                    SkipSpecificDaysOfWeek = model.SkipDaysOfWeek,
                    VendorId = model.VendorId,
                    SkipSpecificDaysOfWeekFriday = model.SkipFriday,
                    SkipSpecificDaysOfWeekMonday = model.SkipMonday,
                    SkipSpecificDaysOfWeekSaturday = model.SkipSaturday,
                    SkipSpecificDaysOfWeekSunday = model.SkipSunday,
                    SkipSpecificDaysOfWeekThursday = model.SkipThursday,
                    SkipSpecificDaysOfWeekTuesday = model.SkipTuesday,
                    SkipSpecificDaysOfWeekWednesday = model.SkipWednesday,
                    CustomerId = model.CustomerId
                };
            return newCr;
        }

        /// <summary>
        /// Deletes a collection route from the system.
        /// </summary>
        public void DeleteCollection(long routeId)
        {
            var collectionRun = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == routeId);
            if (collectionRun != null)
            {
                //delete the collection run meters for this route
                var crms = PemsEntities.CollectionRunMeters.Where(x => x.CollectionRunId == routeId);
                if (crms.Any())
                {
                    foreach (var collectionRunMeter in crms)
                        PemsEntities.CollectionRunMeters.Remove(collectionRunMeter);
                    PemsEntities.SaveChanges();
                }
                //now that all the meters are deleted, lets delete the collection run
                PemsEntities.CollectionRuns.Remove(collectionRun);
                PemsEntities.SaveChanges();
            }
        }

        #endregion

        #region Vendor Coin Entry

        public List<VCEListModel> GetVCEIndexModels(int customerId, string startDate, string endDate)
        {
            var models = new List<VCEListModel>();
            try
            {
                var itemsQuery = from at in PemsEntities.CollectionRuns
                                 where at.CustomerId == customerId
                                 where at.CollectionRunStatus != 0
                                 //active and pending, ignore inactive ones
                                 orderby at.ActivationDate descending
                                 select new VCEListModel
                                     {
                                         CollectionRunId = (int)at.CollectionRunId,
                                         CollectionRunName = at.CollectionRunName,
                                         ActivationDate = at.ActivationDate,
                                         CustomerId = customerId
                                     };


                var preFilterItems = itemsQuery.ToList();

                //start date and end date checkt he colldatasumm table
                //parse the dates
                if (!string.IsNullOrEmpty(startDate))
                {
                    var sDate = new DateTime(long.Parse(startDate), DateTimeKind.Utc);
                    sDate = sDate.ToLocalTime();
                    preFilterItems = preFilterItems.Where(x => x.ActivationDate >= sDate).ToList();
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var eDate = new DateTime(long.Parse(endDate), DateTimeKind.Utc);
                    eDate = eDate.ToLocalTime();
                    preFilterItems = preFilterItems.Where(x => x.ActivationDate <= eDate).ToList();
                }

                var items = preFilterItems.ToList();
                if (items.Any())
                {
                    models = items;
                    //fill totals here
                    foreach (var vceListModel in items)
                    {
                        //get the collectionrun report for this CR
                        var collRunReport =
                            PemsEntities.CollectionRunReports.FirstOrDefault(
                                x => x.CollectionRunId == vceListModel.CollectionRunId && x.CustomerId == customerId);
                        if (collRunReport == null)
                        {
                            vceListModel.TotalAmountReportedByMeter = 0;
                            vceListModel.TotalAmountCounted = 0;
                        }
                        else
                        {
                            vceListModel.TotalAmountReportedByMeter = collRunReport.TotalMeterCashAmt;
                            vceListModel.TotalAmountCounted = collRunReport.TotalManualCashAmt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetVendorCoinEntryDDLModels", ex);
                models = new List<VCEListModel>();
            }
            return models;
        }

        public void SaveVCEDetailsModel(VCEDetailsModel model)
        {
            var collectionRun = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == model.CollectionRunId && x.CustomerId == model.CustomerId);
            if (collectionRun != null)
            {
                var collRunReport = PemsEntities.CollectionRunReports.FirstOrDefault(crr => crr.CustomerId == model.CustomerId && crr.CollectionRunId == model.CollectionRunId);
                //if the collection run report doesnt exist, create it
                if (collRunReport == null)
                {
                    var newCrr = new CollectionRunReport
                        {
                            CollectionDate = model.ActivationDate.GetValueOrDefault(),
                            CollectionRunId = model.CollectionRunId,
                            CustomerId = model.CustomerId,
                            ProcessedTS = model.ActivationDate,
                            VendorId = collectionRun.VendorId,
                            TotalManualCoinCount = model.TotalCoinCount,
                            TotalManualCashAmt = model.TotalValue,
                            ManualCoinType1Count = model.Coin1Count,
                            ManualCoinType2Count = model.Coin2Count,
                            ManualCoinType3Count = model.Coin3Count,
                            ManualCoinType4Count = model.Coin4Count,
                            ManualCoinType5Count = model.Coin5Count,
                            ManualCoinType6Count = model.Coin6Count,
                            ManualCoinType7Count = model.Coin7Count,
                            ManualCoinType8Count = model.Coin8Count
                        };
                    PemsEntities.CollectionRunReports.Add(newCrr);
                    PemsEntities.SaveChanges();
                }
                else
                {
                    //update the existing one.
                    collRunReport.TotalManualCoinCount = model.TotalCoinCount;
                    collRunReport.TotalManualCashAmt = model.TotalValue;
                    collRunReport.ManualCoinType1Count = model.Coin1Count;
                    collRunReport.ManualCoinType2Count = model.Coin2Count;
                    collRunReport.ManualCoinType3Count = model.Coin3Count;
                    collRunReport.ManualCoinType4Count = model.Coin4Count;
                    collRunReport.ManualCoinType5Count = model.Coin5Count;
                    collRunReport.ManualCoinType6Count = model.Coin6Count;
                    collRunReport.ManualCoinType7Count = model.Coin7Count;
                    collRunReport.ManualCoinType8Count = model.Coin8Count;
                    PemsEntities.SaveChanges();
                }
            }
        }

        public VCEDetailsModel GetVCEDetailModel(int customerId, int collectionRunId)
        {
            var model = new VCEDetailsModel();
            // get the customer-specific coin denominations
            var coinDenominations = from cd in PemsEntities.CoinDenominations
                                    join cdc in PemsEntities.CoinDenominationCustomers on cd.CoinDenominationId
                                        equals cdc.CoinDenominationId
                                    where cdc.CustomerId == customerId
                                    where cdc.IsDisplay == true
                                    select new
                                    {
                                        cd.TransactionsCashMap, // maps to column name in TransactionsCash table
                                        cd.CoinValue, // amount in cents
                                        cdc.CoinName, // display name,
                                        CoinNameDefault = cd.CoinName
                                    };
            //get the coll run report
            // Get the numbers of each coin type
            var collRunReport = PemsEntities.CollectionRunReports.FirstOrDefault(crr => crr.CustomerId == customerId && crr.CollectionRunId == collectionRunId);
            var collectionRun = PemsEntities.CollectionRuns.FirstOrDefault(x => x.CollectionRunId == collectionRunId && x.CustomerId == customerId);
            if (collectionRun != null)
            {
                model.ActivationDate = collectionRun.ActivationDate;
                model.CollectionRunName = collectionRun.CollectionRunName;
            }
            model.CustomerId = customerId;
            model.CollectionRunId = collectionRunId;
            //if hte report doesnt exist, create it here.

            int coinCent1 = 0;
            int coinCent5 = 0;
            int coinCent10 = 0;
            int coinCent20 = 0;
            int coinCent25 = 0;
            int coinCent50 = 0;
            int coinDollar1 = 0;
            int coinDollar2 = 0;

            //set the values here
            if (collRunReport != null)
            {
                model.CollectionRunReportId = (int)collRunReport.CollectionRunReportId;
                coinCent1 = collRunReport.ManualCoinType1Count ?? 0;
                coinCent5 = collRunReport.ManualCoinType2Count ?? 0;
                coinCent10 = collRunReport.ManualCoinType3Count ?? 0;
                coinCent20 = collRunReport.ManualCoinType4Count ?? 0;
                coinCent25 = collRunReport.ManualCoinType5Count ?? 0;
                coinCent50 = collRunReport.ManualCoinType6Count ?? 0;
                coinDollar1 = collRunReport.ManualCoinType7Count ?? 0;
                coinDollar2 = collRunReport.ManualCoinType8Count ?? 0;
            }


            // Iterate over the coin denominations and, if there is a matching value in the transaction table, calculate values
            foreach (var coinDenomination in coinDenominations)
            {
                switch (coinDenomination.TransactionsCashMap)
                {
                    case "CoinCent1":
                        model.Coin1Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin1Value = coinDenomination.CoinValue;
                        model.Coin1Count = coinCent1;
                        model.Coin1Display = true;
                        break;
                    case "CoinCent5":
                        model.Coin2Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin2Value = coinDenomination.CoinValue;
                        model.Coin2Count = coinCent5;
                        model.Coin2Display = true;

                        break;
                    case "CoinCent10":
                        model.Coin3Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin3Value = coinDenomination.CoinValue;
                        model.Coin3Count = coinCent10;
                        model.Coin3Display = true;

                        break;
                    case "CoinCent20":
                        model.Coin4Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin4Value = coinDenomination.CoinValue;
                        model.Coin4Display = true;
                        model.Coin4Count = coinCent20;
                        break;
                    case "CoinCent25":
                        model.Coin5Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin5Value = coinDenomination.CoinValue;
                        model.Coin5Count = coinCent25;
                        model.Coin5Display = true;
                        break;
                    case "CoinCent50":
                        model.Coin6Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin6Value = coinDenomination.CoinValue;
                        model.Coin6Count = coinCent50;
                        model.Coin6Display = true;
                        break;
                    case "CoinDollar1":
                        model.Coin7Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin7Value = coinDenomination.CoinValue;
                        model.Coin7Count = coinDollar1;
                        model.Coin7Display = true;
                        break;
                    case "CoinDollar2":
                        model.Coin8Name = string.IsNullOrEmpty(coinDenomination.CoinName) ? coinDenomination.CoinNameDefault : coinDenomination.CoinName;
                        model.Coin8Value = coinDenomination.CoinValue;
                        model.Coin8Count = coinDollar2;
                        model.Coin8Display = true;
                        break;
                }
            }

            //now set the totals

            model.TotalValue = (model.Coin1Value * model.Coin1Count)
                + (model.Coin2Value * model.Coin2Count)
                + (model.Coin3Value * model.Coin3Count)
                + (model.Coin4Value * model.Coin4Count)
                + (model.Coin5Value * model.Coin5Count)
                + (model.Coin6Value * model.Coin6Count)
                + (model.Coin7Value * model.Coin7Count)
                + (model.Coin8Value * model.Coin8Count);

            model.TotalCoinCount = model.Coin1Count + model.Coin2Count + model.Coin3Count + model.Coin4Count +
                               model.Coin5Count + model.Coin6Count + model.Coin7Count + model.Coin8Count;


            return model;

        }





        #endregion
    }
}
