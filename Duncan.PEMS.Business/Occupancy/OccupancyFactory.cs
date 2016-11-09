using System;
using System.Collections.Generic;
using System.Linq;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Alarms;
using Duncan.PEMS.Entities.Occupancy;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Utilities;
using Kendo.Mvc.UI;
using NLog;

namespace Duncan.PEMS.Business.Occupancy
{
    public class OccupancyFactory : BaseFactory
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
        public OccupancyFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Gets a list of occupancy items for the occupancy index grid based on the data source request and any associated filters
        /// </summary>
        public List<OccupancyInquiryItem> GetOccupancyItems(DataSourceRequest request, string defaultOrderBy, string spName)
        {
            string paramValues = string.Empty;
            var spParams = GetSpParams( request, defaultOrderBy, out paramValues );
            IEnumerable<OccupancyInquiryItem> occupancies = PemsEntities.Database.SqlQuery<OccupancyInquiryItem>( spName + " " + paramValues, spParams );

            List<OccupancyInquiryItem> occupancyInquiryItems = occupancies.ToList();

            foreach (OccupancyInquiryItem item in occupancyInquiryItems)
            {
                //item.TotalOccupiedMinuteDisplay = FormatHelper.FormatTimeFromMinutes( item.TotalOccupiedMinute ); //Changed By Rajesh on 22/11/2015
                item.TotalOccupiedMinuteDisplay = FormatHelper.FormatTimeFromMinutesTwoDigit(item.TotalOccupiedMinute, (int)PEMSEnums.PEMSEnumsTimeFormats.timeFormat_HH_MM_SS);
                //item.TotalTimePaidMinuteDisplay = FormatHelper.FormatTimeFromMinutes( item.TotalTimePaidMinute );//Changed By Rajesh on 22/11/2015
                item.TotalTimePaidMinuteDisplay = FormatHelper.FormatTimeFromSecondsToHHMMSS(item.TotalTimePaidMinute);
                item.TotalAmountInCentDisplay = FormatHelper.FormatCurrency( item.TotalAmountInCent );

                //
                item.ViolationMinuteDisplay = FormatHelper.FormatTimeFromSecondsToHHMMSS(item.ViolationMinute); //Violation Seconds grid column
                item.RemaingPaidTimeMinuteDisplay = FormatHelper.FormatTimeFromSecondsToHHMMSS(item.RemaingPaidTimeMinute); // Seconds Cleared grid column 
                item.SecondsResold = FormatHelper.FormatTimeFromSecondsToHHMMSS(item.Minuteresold); //Seconds Resold grid column
            }

            return occupancyInquiryItems;
        }

        /// <summary>
        /// Gets details for an occupancy transaction
        /// </summary>
        public OccupancyDetailItem GetDetails(int cityId, long id)
        {
            var sensorpaymentitem = PemsEntities.SensorPaymentTransactions.FirstOrDefault(x => x.SensorPaymentTransactionID == id);
            var metermapitem = sensorpaymentitem.ParkingSpace.Meter.MeterMaps.FirstOrDefault(x => x.Customerid == cityId && x.MeterId == sensorpaymentitem.ParkingSpace.MeterId);
            var area = PemsEntities.Areas.FirstOrDefault(x => x.CustomerID == cityId && x.AreaID == metermapitem.AreaId2);
            var occupancystatuses = PemsEntities.ParkingSpaceOccupancies.FirstOrDefault(x => x.ParkingSpaceId == sensorpaymentitem.ParkingSpaceId);

            int lastmethodpaymenttypeid = sensorpaymentitem.LastTxPaymentMethod ?? 0;
            TransactionType lastpaymenttype = PemsEntities.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == lastmethodpaymenttypeid);
            string lastpaymentmethod = "";
            if (lastpaymenttype != null)
                lastpaymentmethod = lastpaymenttype.TransactionTypeDesc;

            int firstmethodpaymenttypeid = sensorpaymentitem.FirstTxPaymentMethod ?? 0;
            TransactionType firstpaymenttype = PemsEntities.TransactionTypes.FirstOrDefault(x => x.TransactionTypeId == firstmethodpaymenttypeid);
            string firstpaymentmethod = "";
            if (firstpaymenttype != null)
                firstpaymentmethod = firstpaymenttype.TransactionTypeDesc;


            string discounttype = "";
            int discountschemaid = sensorpaymentitem.DiscountSchema ?? 0;
            DiscountScheme ds = PemsEntities.DiscountSchemes.FirstOrDefault(x => x.DiscountSchemeID == discountschemaid);
            if (ds != null)
                discounttype = ds.SchemeName;
            var spaceDetails = sensorpaymentitem.ParkingSpace.ParkingSpaceDetails.FirstOrDefault();
            string spacetype = "";
            if (spaceDetails != null)
                spacetype = spaceDetails.SpaceType1.SpaceTypeDesc;
            string suburb = "";
            CustomGroup1 cg = PemsEntities.CustomGroup1.FirstOrDefault(x => x.CustomerId == cityId && x.CustomGroupId == metermapitem.CustomGroup1);
            if (cg != null)
                suburb = cg.DisplayName;
            long sensorid = -1;
            Sensor sen = sensorpaymentitem.ParkingSpace.Sensors.FirstOrDefault(x => x.CustomerID == cityId && x.ParkingSpaceId == sensorpaymentitem.ParkingSpaceId);
            if (sen != null)
                sensorid = sen.SensorID;
            OccupancyDetailItem ret = new OccupancyDetailItem()
            {
                SpaceId = sensorpaymentitem.ParkingSpaceId,
                SpaceType = spacetype, //sensorpaymentitem.ParkingSpace.ParkingSpaceType.ToString(), // need to map
                Area = area.AreaName,
                Zone = metermapitem.Zone.ZoneName,
                Suburb = suburb,
                //PemsEntities.CustomGroup1.FirstOrDefault(x=>x.CustomerId==cityId&&x.CustomGroupId==metermapitem.CustomGroup1).DisplayName,
                SensorId = sensorid,
                //sensorpaymentitem.ParkingSpace.Sensors.FirstOrDefault(x=>x.CustomerID==cityId&& x.ParkingSpaceId==sensorpaymentitem.ParkingSpaceId).SensorID,
                MeterId = sensorpaymentitem.ParkingSpace.MeterId,
                Latitude = metermapitem.Meter.Latitude ?? 0,
                Longitude = metermapitem.Meter.Longitude ?? 0,
                DemandArea = (metermapitem.Meter.DemandZone ?? 0),
                ArrivalTimestamp = sensorpaymentitem.ArrivalTime,
                DepartureTimestamp = sensorpaymentitem.DepartureTime,
                TimeType1 = (sensorpaymentitem.TimeType1 != null
                                  ? PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == sensorpaymentitem.TimeType1).TimeTypeDesc
                                  : ""),
                TimeType2 = (sensorpaymentitem.TimeType2 != null
                                  ? PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == sensorpaymentitem.TimeType2).TimeTypeDesc
                                  : ""),
                TimeType3 = (sensorpaymentitem.TimeType3 != null
                                  ? PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == sensorpaymentitem.TimeType3).TimeTypeDesc
                                  : ""),
                TimeType4 = (sensorpaymentitem.TimeType4 != null
                                  ? PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == sensorpaymentitem.TimeType4).TimeTypeDesc
                                  : ""),
                TimeType5 = (sensorpaymentitem.TimeType5 != null
                                  ? PemsEntities.TimeTypes.FirstOrDefault(x => x.TimeTypeId == sensorpaymentitem.TimeType5).TimeTypeDesc
                                  : ""),
                ViolationMinutes = sensorpaymentitem.ViolationMinute ?? 0,
                ViolationSegmentCount = sensorpaymentitem.ViolationSegmentCount ?? 0,
                OccupancyDuration = sensorpaymentitem.TotalOccupiedMinute ?? 0,
                OccupancyStatus = occupancystatuses != null ? occupancystatuses.OccupancyStatu.StatusDesc : "",

                OccupancyTimestamp = sensorpaymentitem.OccupancyDate,
                //OperationalStatus = ( sensorpaymentitem.ParkingSpace.OperationalStatu != null
                //                          ? sensorpaymentitem.ParkingSpace.OperationalStatu.OperationalStatusDesc
                //                          : "" ),
                OperationalStatus = sensorpaymentitem.OperationalStatu.OperationalStatusDesc,
                NonCompliantStatus = (sensorpaymentitem.NonCompliantStatu != null ? sensorpaymentitem.NonCompliantStatu.NonCompliantStatusDesc : ""),
                UnusedPaidTime = sensorpaymentitem.RemaingPaidTimeMinute ?? 0,
                TimeCleared = sensorpaymentitem.ZeroOutTime,
                PaidTimeStart = sensorpaymentitem.FirstTxStartTime,
                FreeParkingTime = sensorpaymentitem.FreeParkingTime ?? 0,
                FirstPaymentTime = sensorpaymentitem.FirstTxPaymentTime,
                FirstPaymentAmount = sensorpaymentitem.FirstTxAmountInCent ?? 0,
                FirstPaymentMethod = firstpaymentmethod,
                LastPaymentTime = sensorpaymentitem.LastTxPaymentTime,
                LastPaymentAmount = sensorpaymentitem.LastTxAmountInCent ?? 0,
                LastPaymentMethod = lastpaymentmethod,
                TotalAmountPaid = sensorpaymentitem.TotalAmountInCent ?? 0,
                TotalPaymentCount = sensorpaymentitem.TotalNumberOfPayment ?? 0,
                DiscountType = discounttype,
                PaidTimeEnd = sensorpaymentitem.LastTxExpiryTime,
                PaidTimeDuration = sensorpaymentitem.LastTxTimePaidMinute ?? 0,
                GracePeriodUsed = sensorpaymentitem.GracePeriodMinute ?? 0,
                BayNumber = sensorpaymentitem.ParkingSpace.BayNumber,
                BayName = sensorpaymentitem.ParkingSpace.DisplaySpaceNum,
                Street = metermapitem.Meter.Location,
                Zeroout = sensorpaymentitem.Zeroout, //Changed On 11/11/2015
                Minuteresold = sensorpaymentitem.Minuteresold
            };
            return ret;
        }

        /// <summary>
        /// Gets a lsit of timetypes (dropdown items)
        /// </summary>
        /// <returns></returns>
        public List<OccupancyDDL> GetTimeTypes()
        {
            var timeTypesQuery = PemsEntities.TimeTypes.Select(tt => new OccupancyDDL
            {
                Id = tt.TimeTypeId,
                Text = tt.TimeTypeDesc
            });

            var timeTypes = timeTypesQuery.ToList();
            timeTypes.Insert(0, new OccupancyDDL { Id = -1, Text = "All" });
            return timeTypes;
        }

        /// <summary>
        /// Gets a list of dropdown items that represent operational status
        /// </summary>
        public List<OccupancyDDLString> GetOperationalStatuses()
        {
            var query = PemsEntities.OperationalStatus.Select(val => new OccupancyDDLString
            {
                Id = val.OperationalStatusDesc,
                Text = val.OperationalStatusDesc
            });
            var results = query.ToList();
            results.Insert(0, new OccupancyDDLString { Id = "All", Text = "All" });
            return results;
        }

        /// <summary>
        /// Gets a list of dropdown items that represent occupancy status
        /// </summary>
        public List<OccupancyDDLString> GetOccupancyStatuses()
        {
            var query = PemsEntities.OccupancyStatus.Select(val => new OccupancyDDLString
            {
                Id = val.StatusDesc,
                Text = val.StatusDesc
            });
            var results = query.ToList();
            results.Insert(0, new OccupancyDDLString { Id = "All", Text = "All" });
            return results;
        }

        /// <summary>
        /// Gets a list of dropdown items that represent asset types for this customer
        /// </summary>
        public List<OccupancyDDLString> GetAssetTypes(int customerId)
        {
            var assetTypesQuery = from at in PemsEntities.AssetTypes
                                  where at.CustomerId == customerId && at.IsDisplay == true
                                  select new OccupancyDDLString
                                  {
                                      Id = at.MeterGroupDesc,
                                      Text = at.MeterGroupDesc
                                  };
            var assetTypes = assetTypesQuery.ToList();
            return assetTypes;
        }

        /// <summary>
        /// Gets a list of dropdown items that represent non-complaine status's
        /// </summary>
        public List<OccupancyDDLString> GetNoncompliantStatuses()
        {
            var query = PemsEntities.NonCompliantStatus.Select(val => new OccupancyDDLString
            {
                Id = val.NonCompliantStatusDesc,
                Text = val.NonCompliantStatusDesc
            });
            var results = query.ToList();
            results.Insert(0, new OccupancyDDLString { Id = "All", Text = "All" });
            return results;
        }
    }
}