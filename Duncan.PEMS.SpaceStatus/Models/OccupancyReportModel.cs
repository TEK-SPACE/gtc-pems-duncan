using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

using OfficeOpenXml; // Namespace inside the open source EPPlus.dll from http://epplus.codeplex.com/
using OfficeOpenXml.Style;


using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.Models
{
    #region Report statistic data classes
    public class SensingAndPaymentStatistic_Common
    {
        public int ingress { get; set; }       // Count of vehicle arrivals
        public int egress { get; set; }        // Count of vehicle departures
        public int PaymentCount { get; set; }

        public TimeSpan TotalPaidTime { get; set; }
        public TimeSpan TotalOccupancyTime { get; set; }
        public TimeSpan TotalOccupancyPaidTime { get; set; }
        public TimeSpan TotalOccupancyNotPaidTime { get; set; }

        public TimeSpan MaximumPotentialOccupancyTime { get; set; }

        public TimeSpan AverageOccupancyTime { get; set; }
        public TimeSpan AveragePaidTime { get; set; }
        public float PercentageOccupancy { get; set; }
        public float PercentageOccupiedPaid { get; set; }
        public float PercentageOccupiedNotPaid { get; set; }

        public SensingAndPaymentStatistic_Common()
        {
            InitValues();
        }

        protected void InitValues()
        {
            this.PercentageOccupancy = 0.00f;
            this.PercentageOccupiedPaid = 0.00f;
            this.PercentageOccupiedNotPaid = 0.00f;
            this.TotalOccupancyPaidTime = new TimeSpan();
            this.TotalOccupancyNotPaidTime = new TimeSpan();
            this.TotalOccupancyTime = new TimeSpan();
            this.AveragePaidTime = new TimeSpan();
            this.AverageOccupancyTime = new TimeSpan();
            this.MaximumPotentialOccupancyTime = new TimeSpan();
            this.TotalPaidTime = new TimeSpan();
            this.ingress = 0;
            this.egress = 0;
            this.PaymentCount = 0;
        }

        public virtual void AggregateSelf()
        {
            if (this.TotalOccupancyTime.TotalMilliseconds == 0)
            {
                this.PercentageOccupiedPaid = (float)0.00f;
                this.PercentageOccupiedNotPaid = (float)0.00f;
            }
            else
            {
                //this.PercentageOccupiedPaid = (float)Math.Round(((this.TotalOccupancyPaidTime.TotalMilliseconds / this.TotalOccupancyTime.TotalMilliseconds) * 100), 2);
                //this.PercentageOccupiedNotPaid = (float)Math.Round((((this.TotalOccupancyTime.TotalMilliseconds - this.TotalOccupancyPaidTime.TotalMilliseconds) / this.TotalOccupancyTime.TotalMilliseconds) * 100), 2);

                // Hans -- modified to remove rounding of decimal formore precise reporting
                this.PercentageOccupiedPaid = (float)((this.TotalOccupancyPaidTime.TotalMilliseconds / this.TotalOccupancyTime.TotalMilliseconds) * 100.0f);
                this.PercentageOccupiedNotPaid = (float)(((this.TotalOccupancyTime.TotalMilliseconds - this.TotalOccupancyPaidTime.TotalMilliseconds) / this.TotalOccupancyTime.TotalMilliseconds) * 100.0f);
            }

            if (this.MaximumPotentialOccupancyTime.TotalMilliseconds == 0)
            {
                this.PercentageOccupancy = (float)0.00f;
            }
            else
            {
                //this.PercentageOccupancy = (float)Math.Round(((this.TotalOccupancyTime.TotalMilliseconds / this.MaximumPotentialOccupancyTime.TotalMilliseconds) * 100), 2);
                this.PercentageOccupancy = (float)((this.TotalOccupancyTime.TotalMilliseconds / this.MaximumPotentialOccupancyTime.TotalMilliseconds) * 100.0f);
            }

            if (Math.Max(this.ingress, this.egress) > 0)
            {
                this.AverageOccupancyTime = new TimeSpan(this.TotalOccupancyTime.Ticks / Math.Max(this.ingress, this.egress));
            }

            if (this.PaymentCount != 0)
            {
                this.AveragePaidTime = new TimeSpan(this.TotalPaidTime.Ticks / this.PaymentCount);
            }
        }
    }

    public abstract class SensingAndPaymentStatistic_WithChildren : SensingAndPaymentStatistic_Common
    {
        public bool WantHourlyStats { get; set; }

        public Dictionary<int, SensingAndPaymentStatistic_Common> ChildStats_Hourly = new Dictionary<int, SensingAndPaymentStatistic_Common>();

        public SensingAndPaymentStatistic_WithChildren()
            : base()
        {
            WantHourlyStats = true;
        }
    }

    public class SensingAndPaymentStatistic_BayAndMeter : SensingAndPaymentStatistic_WithChildren
    {
        public int MeterID { get; set; }
        public int BayID { get; set; }

        public int AreaID { get; set; }
        public string AreaName { get; set; }

        public SensingAndPaymentStatistic_BayAndMeter()
            : base()
        {
        }

        public override void AggregateSelf()
        {
            // Just to be safe, let's make sure all existing values are reset to original state
            InitValues();

            // Determine our composite values by adding up the children stats for each hour (24 hours)
            if ((this.ChildStats_Hourly != null) && (this.ChildStats_Hourly.Keys.Count == 24))
            {
                for (int hourKey = 0; hourKey < 24; hourKey++)
                {
                    SensingAndPaymentStatistic_Common NextBayHourlyRec = this.ChildStats_Hourly[hourKey];

                    this.TotalOccupancyTime += NextBayHourlyRec.TotalOccupancyTime;
                    this.TotalPaidTime += NextBayHourlyRec.TotalPaidTime;
                    this.TotalOccupancyPaidTime += NextBayHourlyRec.TotalOccupancyPaidTime;
                    this.MaximumPotentialOccupancyTime += NextBayHourlyRec.MaximumPotentialOccupancyTime;
                    this.egress += NextBayHourlyRec.egress;
                    this.ingress += NextBayHourlyRec.ingress;
                    this.PaymentCount += NextBayHourlyRec.PaymentCount;
                }
            }

            // Now do the common self-aggregation
            base.AggregateSelf();
        }
    }

    public class SensingAndPaymentStatistic_Meter : SensingAndPaymentStatistic_WithChildren
    {
        public int MeterID { get; set; }

        public int AreaID { get; set; }
        public string AreaName { get; set; }

        public SensingAndPaymentStatistic_Meter()
            : base()
        {
        }

        public void AggregateSelfFromBayStats(SensingAndPaymentStatistic_ReportData ReportDataModel)
        {
            // Just to be safe, let's make sure all existing values are reset to original state
            InitValues();
            this.ChildStats_Hourly.Clear();

            SensingAndPaymentStatistic_Common MeterStatsForHour = null;
            SensingAndPaymentStatistic_Common BayStatsForHour = null;

            // Do we want hourly information also?
            if (this.WantHourlyStats)
            {
                // Add 24 shell objects to account for each hour of the day
                for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
                {
                    MeterStatsForHour = new SensingAndPaymentStatistic_Common();
                    this.ChildStats_Hourly.Add(hourOfDay, MeterStatsForHour);
                }
            }

            // Update our summary from summary of each applicable bay
            foreach (SensingAndPaymentStatistic_BayAndMeter nextBayAndMeterStats in ReportDataModel.BayStats)
            {
                // Exclude from our calculations if its not for the same meter
                if (nextBayAndMeterStats.MeterID != this.MeterID)
                    continue;

                this.TotalOccupancyTime += nextBayAndMeterStats.TotalOccupancyTime;
                this.TotalPaidTime += nextBayAndMeterStats.TotalPaidTime;
                this.TotalOccupancyPaidTime += nextBayAndMeterStats.TotalOccupancyPaidTime;
                this.MaximumPotentialOccupancyTime += nextBayAndMeterStats.MaximumPotentialOccupancyTime;
                this.egress += nextBayAndMeterStats.egress;
                this.ingress += nextBayAndMeterStats.ingress;
                this.PaymentCount += nextBayAndMeterStats.PaymentCount;

                // Do we want hourly meter status also?
                if (this.WantHourlyStats)
                {
                    // Loop to process 24 hourly slots
                    for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
                    {
                        // Get the bay stats for current hour
                        BayStatsForHour = nextBayAndMeterStats.ChildStats_Hourly[hourOfDay];

                        // Get the meter stats for current hour
                        MeterStatsForHour = this.ChildStats_Hourly[hourOfDay];

                        // Update current hour stats for meter from current hour stats for bay
                        MeterStatsForHour.TotalOccupancyTime += BayStatsForHour.TotalOccupancyTime;
                        MeterStatsForHour.TotalPaidTime += BayStatsForHour.TotalPaidTime;
                        MeterStatsForHour.TotalOccupancyPaidTime += BayStatsForHour.TotalOccupancyPaidTime;
                        MeterStatsForHour.MaximumPotentialOccupancyTime += BayStatsForHour.MaximumPotentialOccupancyTime;
                        MeterStatsForHour.egress += BayStatsForHour.egress;
                        MeterStatsForHour.ingress += BayStatsForHour.ingress;
                        MeterStatsForHour.PaymentCount += BayStatsForHour.PaymentCount;
                    }
                }
            }

            // Do we want hourly meter status also?
            if (this.WantHourlyStats)
            {
                // Now that all data for the hourly data has been accumulated, we need to aggregate each hour
                for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
                {
                    // Get the meter stats for current hour, and then tell it to aggregate itself
                    MeterStatsForHour = this.ChildStats_Hourly[hourOfDay];
                    MeterStatsForHour.AggregateSelf();
                }
            }

            // Everything has been updated from children, so now its time to do the common self-aggregation 
            this.AggregateSelf();
        }

        public override void AggregateSelf()
        {
            // Now do the common self-aggregation
            base.AggregateSelf();
        }
    }

    public class SensingAndPaymentStatistic_Area : SensingAndPaymentStatistic_WithChildren
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }

        private CachedMeterAndAreaAssets _cachedAssets = null;

        public SensingAndPaymentStatistic_Area(CachedMeterAndAreaAssets cachedAssets)
            : base()
        {
            this._cachedAssets = cachedAssets;
        }

        private int GetAreaIDForMeterID(int meterID, CustomerConfig customerCfg)
        {
            int result = -1;
            if ((this._cachedAssets != null) && (this._cachedAssets.enumedMeterAssets != null))
            {
                foreach (MeterAsset asset in this._cachedAssets.enumedMeterAssets)
                {
                    if (asset.MeterID == meterID)
                    {
                        result = asset.AreaID_PreferLibertyBeforeInternal;
                        break;
                    }
                }
            }
            else
            {
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg))
                {
                    if (asset.MeterID == meterID)
                    {
                        result = asset.AreaID_PreferLibertyBeforeInternal;
                        break;
                    }
                }
            }
            return result;
        }

        public void AggregateSelfFromMeterStats(SensingAndPaymentStatistic_ReportData ReportDataModel, CustomerConfig customerCfg)
        {
            // Just to be safe, let's make sure all existing values are reset to original state
            InitValues();
            this.ChildStats_Hourly.Clear();

            SensingAndPaymentStatistic_Common AreaStatsForHour = null;
            SensingAndPaymentStatistic_Common MeterStatsForHour = null;

            // Do we want hourly information also?
            if (this.WantHourlyStats)
            {
                // Add 24 shell objects to account for each hour of the day
                for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
                {
                    AreaStatsForHour = new SensingAndPaymentStatistic_Common();
                    this.ChildStats_Hourly.Add(hourOfDay, AreaStatsForHour);
                }
            }

            // Update our summary from summary of each applicable bay
            foreach (SensingAndPaymentStatistic_Meter nextMeterStats in ReportDataModel.MeterStats)
            {
                // Exclude from our calculations if its not related to the same area.
                // First, we will need to resolve which area is associated with the meter
                int resolvedAreaIDForMeterID = GetAreaIDForMeterID(nextMeterStats.MeterID, customerCfg);
                if (resolvedAreaIDForMeterID != this.AreaID)
                    continue;

                this.TotalOccupancyTime += nextMeterStats.TotalOccupancyTime;
                this.TotalPaidTime += nextMeterStats.TotalPaidTime;
                this.TotalOccupancyPaidTime += nextMeterStats.TotalOccupancyPaidTime;
                this.MaximumPotentialOccupancyTime += nextMeterStats.MaximumPotentialOccupancyTime;
                if (nextMeterStats.egress > 0)
                    this.egress += nextMeterStats.egress;
                this.ingress += nextMeterStats.ingress;
                this.PaymentCount += nextMeterStats.PaymentCount;

                // Do we want hourly meter status also?
                if (this.WantHourlyStats)
                {
                    // Loop to process 24 hourly slots
                    for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
                    {
                        // Get the meter stats for current hour
                        MeterStatsForHour = nextMeterStats.ChildStats_Hourly[hourOfDay];

                        // Get the area stats for current hour
                        AreaStatsForHour = this.ChildStats_Hourly[hourOfDay];

                        // Update current hour stats for area from current hour stats for meter
                        AreaStatsForHour.TotalOccupancyTime += MeterStatsForHour.TotalOccupancyTime;
                        AreaStatsForHour.TotalPaidTime += MeterStatsForHour.TotalPaidTime;
                        AreaStatsForHour.TotalOccupancyPaidTime += MeterStatsForHour.TotalOccupancyPaidTime;
                        AreaStatsForHour.MaximumPotentialOccupancyTime += MeterStatsForHour.MaximumPotentialOccupancyTime;
                        AreaStatsForHour.egress += MeterStatsForHour.egress;
                        AreaStatsForHour.ingress += MeterStatsForHour.ingress;
                        AreaStatsForHour.PaymentCount += MeterStatsForHour.PaymentCount;
                    }
                }
            }

            // Do we want hourly meter status also?
            if (this.WantHourlyStats)
            {
                // Now that all data for the hourly data has been accumulated, we need to aggregate each hour
                for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
                {
                    // Get the area stats for current hour, and then tell it to aggregate itself
                    AreaStatsForHour = this.ChildStats_Hourly[hourOfDay];
                    AreaStatsForHour.AggregateSelf();
                }
            }

            // Everything has been updated from children, so now its time to do the common self-aggregation 
            this.AggregateSelf();
        }
    }
    #endregion

    #region Report statistic sorters
    public sealed class SensingAndPaymentStatistic_AreaLogicalComparer : System.Collections.Generic.IComparer<SensingAndPaymentStatistic_Area>
    {
        private static readonly System.Collections.Generic.IComparer<SensingAndPaymentStatistic_Area> _default = new SensingAndPaymentStatistic_AreaLogicalComparer(true);
        private bool _SortByName = false;

        public SensingAndPaymentStatistic_AreaLogicalComparer(bool SortByName)
        {
            _SortByName = SortByName;
        }

        public static System.Collections.Generic.IComparer<SensingAndPaymentStatistic_Area> Default
        {
            get { return _default; }
        }

        public int Compare(SensingAndPaymentStatistic_Area s1, SensingAndPaymentStatistic_Area s2)
        {
            // Are we sorting by Name or ID?
            if (this._SortByName == true)
                return string.CompareOrdinal(s1.AreaName, s2.AreaName);
            else
                return s1.AreaID.CompareTo(s2.AreaID);
        }
    }

    public sealed class SensingAndPaymentStatistic_WithChildrenLogicalComparer : System.Collections.Generic.IComparer<SensingAndPaymentStatistic_WithChildren>
    {
        private static readonly System.Collections.Generic.IComparer<SensingAndPaymentStatistic_WithChildren> _default = new SensingAndPaymentStatistic_WithChildrenLogicalComparer();

        public SensingAndPaymentStatistic_WithChildrenLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<SensingAndPaymentStatistic_WithChildren> Default
        {
            get { return _default; }
        }

        public int Compare(SensingAndPaymentStatistic_WithChildren s1, SensingAndPaymentStatistic_WithChildren s2)
        {
            // We are sorting by ID, but there are multiple type of objects that could be passed to us
            int s1_ID = 0;
            int s2_ID = 0;

            if (s1 is SensingAndPaymentStatistic_BayAndMeter)
                s1_ID = (s1 as SensingAndPaymentStatistic_BayAndMeter).BayID;
            else if (s1 is SensingAndPaymentStatistic_Meter)
                s1_ID = (s1 as SensingAndPaymentStatistic_Meter).MeterID;
            else if (s1 is SensingAndPaymentStatistic_Area)
                s1_ID = (s1 as SensingAndPaymentStatistic_Area).AreaID;

            if (s2 is SensingAndPaymentStatistic_BayAndMeter)
                s2_ID = (s2 as SensingAndPaymentStatistic_BayAndMeter).BayID;
            else if (s2 is SensingAndPaymentStatistic_Meter)
                s2_ID = (s2 as SensingAndPaymentStatistic_Meter).MeterID;
            else if (s2 is SensingAndPaymentStatistic_Area)
                s2_ID = (s2 as SensingAndPaymentStatistic_Area).AreaID;

            return s1_ID.CompareTo(s2_ID);
        }
    }

    public sealed class SensingAndPaymentStatistic_BayAndMeterLogicalComparer : System.Collections.Generic.IComparer<SensingAndPaymentStatistic_BayAndMeter>
    {
        private static readonly System.Collections.Generic.IComparer<SensingAndPaymentStatistic_BayAndMeter> _default = new SensingAndPaymentStatistic_BayAndMeterLogicalComparer();

        public SensingAndPaymentStatistic_BayAndMeterLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<SensingAndPaymentStatistic_BayAndMeter> Default
        {
            get { return _default; }
        }

        public int Compare(SensingAndPaymentStatistic_BayAndMeter s1, SensingAndPaymentStatistic_BayAndMeter s2)
        {
            // We are sorting primarly by BayID, and then by MeterID
            int s1_ID = s1.BayID;
            int s2_ID = s2.BayID;

            // Compare the Bay IDs
            int result = s1_ID.CompareTo(s2_ID);

            // If Bay IDs are the same, then compare the MeterIDs
            if (result == 0)
            {
                s1_ID = s1.MeterID;
                s2_ID = s2.MeterID;
                result = s1_ID.CompareTo(s2_ID);
            }

            return result;
        }
    }

    #endregion

    #region Report data model
    public class SensingAndPaymentStatistic_ReportData
    {
        public List<SensingAndPaymentStatistic_BayAndMeter> BayStats = new List<SensingAndPaymentStatistic_BayAndMeter>();
        public List<SensingAndPaymentStatistic_Meter> MeterStats = new List<SensingAndPaymentStatistic_Meter>();
        public List<SensingAndPaymentStatistic_Area> AreaStats = new List<SensingAndPaymentStatistic_Area>();
        public SensingAndPaymentStatistic_Common OverallStats = new SensingAndPaymentStatistic_Common();
        public Dictionary<int, SensingAndPaymentStatistic_Common> OverallStats_Hourly = new Dictionary<int, SensingAndPaymentStatistic_Common>();

        public bool WantBayStats { get; set; }
        public bool WantBayStats_Hourly { get; set; }

        public bool WantMeterStats { get; set; }
        public bool WantMeterStats_Hourly { get; set; }

        public bool WantAreaStats { get; set; }
        public bool WantAreaStats_Hourly { get; set; }

        public SensingAndPaymentStatistic_ReportData()
        {
            WantBayStats = false;
            WantBayStats_Hourly = false;

            WantMeterStats = false;
            WantMeterStats_Hourly = false;

            WantAreaStats = false;
            WantAreaStats_Hourly = false;
        }
    }
    #endregion

    public class CachedMeterAndAreaAssets
    {
        public List<MeterAsset> enumedMeterAssets = null;
        public List<AreaAsset> enumedAreaAssets = null;

        public CachedMeterAndAreaAssets()
        {
            List<MeterAsset> enumedMeterAssets = new List<MeterAsset>();
            List<AreaAsset> enumedAreaAssets = new List<AreaAsset>();
        }
    }

    public class OccupancyReportEngine
    {
        public enum ActivityRestrictions
        {
            AllActivity, OnlyDuringRegulatedHours, OnlyDuringUnregulatedHours
        }

        #region Private/Protected Members
        protected CustomerConfig _CustomerConfig = null;
        protected ActivityRestrictions _ActivityRestriction = ActivityRestrictions.AllActivity;

        protected SensingAndPaymentStatistic_ReportData _ReportDataModel = new SensingAndPaymentStatistic_ReportData();

        public CachedMeterAndAreaAssets cachedAssets = null;

        #endregion

        #region Public Methods
        public OccupancyReportEngine(CustomerConfig customerCfg)
        {
            _CustomerConfig = customerCfg;
        }

        protected void AddRichTextNameAndValue(OfficeOpenXml.Style.ExcelRichTextCollection rtfCollection, string name, string value)
        {
            OfficeOpenXml.Style.ExcelRichText ert = rtfCollection.Add(name);
            ert.Bold = false;
            ert.Color = System.Drawing.Color.Black;

            ert = rtfCollection.Add(value);
            ert.Bold = true;
            ert.Color = System.Drawing.Color.Blue;
        }

        public void GetReportAsExcelSpreadsheet(List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime, MemoryStream ms,
            bool includeAreaSummary, bool includeMeterSummary, bool includeSpaceSummary, ActivityRestrictions activityRestriction,
            string scopedAreaName, string scopedMeter)
        {

            // Get a list of meter and area assets
            cachedAssets = new CachedMeterAndAreaAssets();
            cachedAssets.enumedMeterAssets = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(_CustomerConfig);
            cachedAssets.enumedAreaAssets = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(_CustomerConfig);

            // Start diagnostics timer
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

            this._ActivityRestriction = activityRestriction;

            this.GatherReportData(listOfMeterIDs, StartTime, EndTime);

            OfficeOpenXml.ExcelWorksheet ws = null;
            int rowIdx = -1;

            using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
            {
                // Let's create a report coversheet and overall summary page, with hyperlinks to the other worksheets
                // Create the worksheet
                ws = pck.Workbook.Worksheets.Add("Summary");

                // Render the header row
                rowIdx = 1; // Excel uses 1-based indexes
                ws.SetValue(rowIdx, 1, "Occupancy Statistics Report");
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, 10])
                {
                    rng.Merge = true; //Merge columns start and end range
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Italic = true;
                    rng.Style.Font.Size = 22;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                rowIdx++;
                ws.Cells[rowIdx, 1].IsRichText = true;
                OfficeOpenXml.Style.ExcelRichTextCollection rtfCollection = ws.Cells[rowIdx, 1].RichText;
                AddRichTextNameAndValue(rtfCollection, "Client: ", this._CustomerConfig.CustomerName);
                AddRichTextNameAndValue(rtfCollection, ",  Generated: ", NowAtDestination.ToShortDateString() + "  " + NowAtDestination.ToShortTimeString());
                ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;

                rowIdx++;
                rtfCollection = ws.Cells[rowIdx, 1].RichText;
                AddRichTextNameAndValue(rtfCollection, "Date Range:  ", StartTime.ToShortDateString() + "  " + StartTime.ToShortTimeString());
                AddRichTextNameAndValue(rtfCollection, " to ", EndTime.ToShortDateString() + "  " + EndTime.ToShortTimeString());
                ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;

                if (!string.IsNullOrEmpty(scopedAreaName))
                {
                    rowIdx++;
                    rtfCollection = ws.Cells[rowIdx, 1].RichText;
                    AddRichTextNameAndValue(rtfCollection, "Report limited to area: ", scopedAreaName);
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }

                if (!string.IsNullOrEmpty(scopedMeter))
                {
                    rowIdx++;
                    rtfCollection = ws.Cells[rowIdx, 1].RichText;
                    AddRichTextNameAndValue(rtfCollection, "Report limited to meter: ", scopedMeter);
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }

                rowIdx++;
                rtfCollection = ws.Cells[rowIdx, 1].RichText;
                if (activityRestriction == ActivityRestrictions.AllActivity)
                {
                    AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "All");
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                else if (activityRestriction == ActivityRestrictions.OnlyDuringRegulatedHours)
                {
                    AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "During Regulated Hours");
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                else if (activityRestriction == ActivityRestrictions.OnlyDuringUnregulatedHours)
                {
                    AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "During Unregulated Hours");
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }


                using (OfficeOpenXml.ExcelRange rng = ws.Cells[2, 1, rowIdx, 10])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 221, 237));  //Set color to lighter blue FromArgb(184, 204, 228)
                }


                rowIdx++;
                int hyperlinkstartRowIdx = rowIdx;

                if (includeMeterSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Meter Occupancy'!A1\", \"Click here for Meter Occupancy summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeSpaceSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Space Occupancy'!A1\", \"Click here for Space Occupancy summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeAreaSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Area Occupancy'!A1\", \"Click here for Area Occupancy summary\")";
                    //ws.Cells[rowIdx, 1].Hyperlink = new ExcelHyperLink("#'Area Occupancy'!A1", "Click here to jump to Area Occupancy summary"); 
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }

                rowIdx++;
                rowIdx++;

                using (OfficeOpenXml.ExcelRange rng = ws.Cells[hyperlinkstartRowIdx, 1, rowIdx, 10])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }


                ws.Cells[rowIdx, 1].Value = "Overall Summary";
                ws.Cells[rowIdx, 1, rowIdx, 5].Merge = true; //Merge columns start and end range
                ws.Cells[rowIdx, 1, rowIdx, 5].Style.Font.Bold = true; //Font should be bold
                ws.Cells[rowIdx, 1, rowIdx, 5].Style.Font.Size = 12;
                ws.Cells[rowIdx, 1, rowIdx, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Render the header row of overall summary
                rowIdx++;
                int overallSummaryHeaderRowIdx = rowIdx;
                ws.Cells[rowIdx, 1].Value = "Occupied Duration";

                ws.Cells[rowIdx, 2].Value = "Max Possible" + Environment.NewLine + "Occupied Duration";
                ws.Cells[rowIdx, 2].Style.WrapText = true;
                ws.Column(2).Width = 20;

                ws.Cells[rowIdx, 3].Value = "Occupied %";

                ws.Cells[rowIdx, 4].Value = "Vehicle" + Environment.NewLine + "Arrivals";
                ws.Cells[rowIdx, 4].Style.WrapText = true;
                ws.Column(4).Width = 12;

                ws.Cells[rowIdx, 5].Value = "Vehicle" + Environment.NewLine + "Departures";
                ws.Cells[rowIdx, 5].Style.WrapText = true;
                ws.Column(5).Width = 12;

                ws.Cells[rowIdx, 6].Value = "Hourly Category";
                ws.Column(6).Width = 24;

                DateTime tempHourlyTime = DateTime.Today;
                DateTime tempHourlyTime2 = DateTime.Today.AddHours(1);
                for (int hourlyColumn = 7; hourlyColumn < 7 + 24; hourlyColumn++)
                {
                    ws.Cells[rowIdx, hourlyColumn].Value = tempHourlyTime.ToString("h ") + "-" + tempHourlyTime2.ToString(" h tt").ToLower();
                    tempHourlyTime = tempHourlyTime.AddHours(1);
                    tempHourlyTime2 = tempHourlyTime2.AddHours(1);
                    ws.Column(hourlyColumn).Width = 14;
                }

                // Format the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, 5])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }
                // Hourly portion of the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 6, rowIdx, 6])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OliveDrab);
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }
                // Hourly portion of the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 7, rowIdx, 7 + 23])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateBlue /*System.Drawing.Color.FromArgb(79, 129, 189)*/);  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }

                // Increment the row index, which will now be the 1st row of our data
                rowIdx++;
                // We only have one row of data for Overall summary, so output it now
                ws.Cells[rowIdx, 1].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats.TotalOccupancyTime);
                ws.Cells[rowIdx, 2].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats.MaximumPotentialOccupancyTime);
                ws.Cells[rowIdx, 3].Value = this._ReportDataModel.OverallStats.PercentageOccupancy;
                ws.Cells[rowIdx, 4].Value = this._ReportDataModel.OverallStats.ingress;
                ws.Cells[rowIdx, 5].Value = this._ReportDataModel.OverallStats.egress;

                // Column 3 is numeric float
                ApplyNumberStyleToColumn(ws, 3, rowIdx, rowIdx, "###0.00", ExcelHorizontalAlignment.Right);

                // Column 4 is numeric integer
                ApplyNumberStyleToColumn(ws, 4, rowIdx, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                // Column 5 is numeric integer
                ApplyNumberStyleToColumn(ws, 5, rowIdx, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                // And now lets size the columns
                for (int autoSizeColIdx = 1; autoSizeColIdx <= 5; autoSizeColIdx++)
                {
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[overallSummaryHeaderRowIdx, autoSizeColIdx, rowIdx, autoSizeColIdx])
                    {
                        col.AutoFitColumns();
                        col.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }
                }

                // Some columns will have set width instead of autofit (autofit doesn't work when we have wrap text?)
                ws.Column(2).Width = 20;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;

                // Hourly data
                /*
                ws.Cells[rowIdx, 6].Value = "Occupied Duration" + Environment.NewLine +
                    "Max Possible Duration" + Environment.NewLine +
                    "% Occupied" + Environment.NewLine +
                    "Arrivals" + Environment.NewLine +
                    "Departures";
                ws.Cells[rowIdx, 6].Style.WrapText = true;
                ws.Cells[rowIdx, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                ws.Cells[rowIdx, 6].Style.Font.Bold = true;
                ws.Row(rowIdx).Height = ws.DefaultRowHeight * 5;
                */
                ws.Cells[rowIdx + 0, 6].Value = "Occupied Duration";
                ws.Cells[rowIdx + 1, 6].Value = "Max Possible Duration";
                ws.Cells[rowIdx + 2, 6].Value = "% Occupied";
                ws.Cells[rowIdx + 3, 6].Value = "Arrivals";
                ws.Cells[rowIdx + 4, 6].Value = "Departures";
                using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 6, rowIdx + 4, 6])
                {
                    /*
                    col.Style.WrapText = true;
                    col.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    */
                    col.Style.Font.Bold = true;
                }
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 5])
                {
                    try
                    {
                        rng.Merge = true; //Merge columns start and end range
                    }
                    catch { }
                }


                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Overall hourly statistics
                    /*
                    ws.Cells[rowIdx, 7 + hourIdx].Value =
                        FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                        FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                        this._ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                        this._ReportDataModel.OverallStats_Hourly[hourIdx].ingress + Environment.NewLine +
                        this._ReportDataModel.OverallStats_Hourly[hourIdx].egress;

                    ws.Cells[rowIdx, 7 + hourIdx].Style.WrapText = true;
                    ws.Cells[rowIdx, 7 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    */
                    ws.Cells[rowIdx + 0, 7 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime);
                    ws.Cells[rowIdx + 1, 7 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                    ws.Cells[rowIdx + 2, 7 + hourIdx].Value = this._ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy;
                    ApplyNumberStyleToCell(ws, rowIdx + 2, 7 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                    ws.Cells[rowIdx + 3, 7 + hourIdx].Value = this._ReportDataModel.OverallStats_Hourly[hourIdx].ingress;
                    ApplyNumberStyleToCell(ws, rowIdx + 3, 7 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                    ws.Cells[rowIdx + 4, 7 + hourIdx].Value = this._ReportDataModel.OverallStats_Hourly[hourIdx].egress;
                    ApplyNumberStyleToCell(ws, rowIdx + 4, 7 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                    /*
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 7, rowIdx + 4, 7 + hourIdx])
                    {
                        ws.Cells[rowIdx, 7 + hourIdx].Style.WrapText = true;
                        ws.Cells[rowIdx, 7 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }
                    */
                }


                //  --- END OF OVERALL SUMMARY WORKSHEET ---


                // Should we include a worksheet with Meter aggregates?
                if (includeMeterSummary == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Meter Occupancy");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Meter #";
                    ws.Cells[rowIdx, 2].Value = "Area #";
                    ws.Cells[rowIdx, 3].Value = "Area";
                    ws.Cells[rowIdx, 4].Value = "Occupied Duration";

                    ws.Cells[rowIdx, 5].Value = "Max Possible" + Environment.NewLine + "Occupied Duration";
                    ws.Cells[rowIdx, 5].Style.WrapText = true;
                    ws.Column(5).Width = 20;

                    ws.Cells[rowIdx, 6].Value = "Occupied %";

                    ws.Cells[rowIdx, 7].Value = "Vehicle" + Environment.NewLine + "Arrivals";
                    ws.Cells[rowIdx, 7].Style.WrapText = true;
                    ws.Column(7).Width = 12;

                    ws.Cells[rowIdx, 8].Value = "Vehicle" + Environment.NewLine + "Departures";
                    ws.Cells[rowIdx, 8].Style.WrapText = true;
                    ws.Column(8).Width = 12;

                    ws.Cells[rowIdx, 9].Value = "Hourly Category";
                    ws.Column(9).Width = 24;

                    tempHourlyTime = DateTime.Today;
                    tempHourlyTime2 = DateTime.Today.AddHours(1);
                    for (int hourlyColumn = 10; hourlyColumn < 10 + 24; hourlyColumn++)
                    {
                        ws.Cells[rowIdx, hourlyColumn].Value = tempHourlyTime.ToString("h ") + "-" + tempHourlyTime2.ToString(" h tt").ToLower();
                        tempHourlyTime = tempHourlyTime.AddHours(1);
                        tempHourlyTime2 = tempHourlyTime2.AddHours(1);
                        ws.Column(hourlyColumn).Width = 14;
                    }

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 8])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    // Format hourly portion of the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 9, rowIdx, 9])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OliveDrab);
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }
                    // Format hourly portion of the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 10, rowIdx, 10 + 23])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateBlue /*System.Drawing.Color.FromArgb(79, 129, 189)*/);  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;


                    // We need to group and subtotal by areas, so we will loop through each area and then report meter stats within each area
                    foreach (SensingAndPaymentStatistic_Area areaStat in this._ReportDataModel.AreaStats)
                    {
                        foreach (SensingAndPaymentStatistic_Meter meterStat in this._ReportDataModel.MeterStats)
                        {
                            #region Unused code, but useful examples
                            // Example of how we could automatically render a dataset to worksheet
                            /*
                            // Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
                            ws.Cells["A1"].LoadFromDataTable(nextTable, true);
                            */

                            // Example of how we could automatically render a strongly-typed collection of objects to worksheet
                            /*
                            List<MeterStatisticObj> statObjects = new List<MeterStatisticObj>();
                            statObjects.Add(meterStatCollection.MeterStats_Summary);
                            ws.Cells["A1"].LoadFromCollection(statObjects, true);
                            */

                            // Example of formatting a column for Date/Time
                            /*
                            ws.Column(3).Width = 20;
                            using (OfficeOpenXml.ExcelRange col = ws.Cells[2, 3, 2 + nextTable.Rows.Count, 3])
                            {
                                col.Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss tt";
                                col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            }
                            */

                            // Example of using RichText in a cell for advanced formatting possibilites
                            /*
                            ws.Cells[rowIdx, 1].IsRichText = true;
                            ws.Cells[rowIdx, 1].Style.WrapText = true; // Need this if we want multi-line
                            OfficeOpenXml.Style.ExcelRichTextCollection rtfCollection = ws.Cells[rowIdx, 1].RichText;
                            OfficeOpenXml.Style.ExcelRichText ert = rtfCollection.Add(areaStat.AreaName + "\r\n");

                            ert = rtfCollection.Add(" (ID=" + areaStat.AreaID.ToString() + ")");
                            ert.Bold = false;
                            ert.Italic = true;
                            ert.Size = 8;
                            */
                            #endregion

                            // Skip this meter stat if its not applicable to current area
                            if (meterStat.AreaID != areaStat.AreaID)
                                continue;

                            // Output row values for data
                            ws.Cells[rowIdx, 1].Value = meterStat.MeterID;
                            ws.Cells[rowIdx, 2].Value = meterStat.AreaID;
                            ws.Cells[rowIdx, 3].Value = meterStat.AreaName;
                            ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.TotalOccupancyTime);
                            ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.MaximumPotentialOccupancyTime);
                            ws.Cells[rowIdx, 6].Value = meterStat.PercentageOccupancy;
                            ws.Cells[rowIdx, 7].Value = meterStat.ingress;
                            ws.Cells[rowIdx, 8].Value = meterStat.egress;

                            // Hourly data
                            ws.Cells[rowIdx + 0, 9].Value = "Occupied Duration";
                            ws.Cells[rowIdx + 1, 9].Value = "Max Possible Duration";
                            ws.Cells[rowIdx + 2, 9].Value = "% Occupied";
                            ws.Cells[rowIdx + 3, 9].Value = "Arrivals";
                            ws.Cells[rowIdx + 4, 9].Value = "Departures";
                            using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 9, rowIdx + 4, 9])
                            {
                                col.Style.Font.Bold = true;
                            }
                            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 8])
                            {
                                try
                                {
                                    rng.Merge = true; //Merge columns start and end range
                                }
                                catch { }
                            }

                            for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                            {
                                // Overall hourly statistics
                                /*
                                ws.Cells[rowIdx, 10 + hourIdx].Value =
                                    FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                                    FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                                    meterStat.ChildStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                                    meterStat.ChildStats_Hourly[hourIdx].ingress + Environment.NewLine +
                                    meterStat.ChildStats_Hourly[hourIdx].egress;

                                ws.Cells[rowIdx, 10 + hourIdx].Style.WrapText = true;
                                ws.Cells[rowIdx, 10 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                */
                                ws.Cells[rowIdx + 0, 10 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime);
                                ws.Cells[rowIdx + 1, 10 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                                ws.Cells[rowIdx + 2, 10 + hourIdx].Value = meterStat.ChildStats_Hourly[hourIdx].PercentageOccupancy;
                                ApplyNumberStyleToCell(ws, rowIdx + 2, 10 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                                ws.Cells[rowIdx + 3, 10 + hourIdx].Value = meterStat.ChildStats_Hourly[hourIdx].ingress;
                                ApplyNumberStyleToCell(ws, rowIdx + 3, 10 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                                ws.Cells[rowIdx + 4, 10 + hourIdx].Value = meterStat.ChildStats_Hourly[hourIdx].egress;
                                ApplyNumberStyleToCell(ws, rowIdx + 4, 10 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                            }

                            // Increment the row index, which will now be the next row of our data
                            rowIdx += 5;
                        }

                        // Each meter for the current area has been reported, so no we need to output a subtotal for the area
                        ws.Cells[rowIdx, 1].Value = "SUBTOTAL AREA";
                        ws.Cells[rowIdx, 1].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 1, rowIdx, 3].Merge = true;

                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalOccupancyTime);
                        ws.Cells[rowIdx, 4].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.MaximumPotentialOccupancyTime);
                        ws.Cells[rowIdx, 5].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 6].Value = areaStat.PercentageOccupancy;
                        ws.Cells[rowIdx, 6].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 7].Value = areaStat.ingress;
                        ws.Cells[rowIdx, 7].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 8].Value = areaStat.egress;
                        ws.Cells[rowIdx, 8].Style.Font.Bold = true;

                        // Hourly data
                        ws.Cells[rowIdx + 0, 9].Value = "Occupied Duration";
                        ws.Cells[rowIdx + 1, 9].Value = "Max Possible Duration";
                        ws.Cells[rowIdx + 2, 9].Value = "% Occupied";
                        ws.Cells[rowIdx + 3, 9].Value = "Arrivals";
                        ws.Cells[rowIdx + 4, 9].Value = "Departures";
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 9, rowIdx + 4, 9])
                        {
                            col.Style.Font.Bold = true;
                        }
                        using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 8])
                        {
                            try
                            {
                                rng.Merge = true; //Merge columns start and end range
                            }
                            catch { }
                        }

                        for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                        {
                            // Overall hourly statistics
                            /*
                            ws.Cells[rowIdx, 10 + hourIdx].Value =
                                FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                                FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].ingress + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].egress;

                            ws.Cells[rowIdx, 10 + hourIdx].Style.WrapText = true;
                            ws.Cells[rowIdx, 10 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            */
                            ws.Cells[rowIdx + 0, 10 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime);
                            ws.Cells[rowIdx + 1, 10 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                            ws.Cells[rowIdx + 2, 10 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].PercentageOccupancy;
                            ApplyNumberStyleToCell(ws, rowIdx + 2, 10 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                            ws.Cells[rowIdx + 3, 10 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].ingress;
                            ApplyNumberStyleToCell(ws, rowIdx + 3, 10 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                            ws.Cells[rowIdx + 4, 10 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].egress;
                            ApplyNumberStyleToCell(ws, rowIdx + 4, 10 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                        }

                        // Increment the row index, which will now be the next row of our data
                        rowIdx += 5;
                        rowIdx++;
                    }

                    // All meters and area are outputted, so now we need a final total also
                    ws.Cells[rowIdx, 1].Value = "TOTAL";
                    ws.Cells[rowIdx, 1].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 1, rowIdx, 3].Merge = true;

                    ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats.TotalOccupancyTime);
                    ws.Cells[rowIdx, 4].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats.MaximumPotentialOccupancyTime);
                    ws.Cells[rowIdx, 5].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 6].Value = _ReportDataModel.OverallStats.PercentageOccupancy;
                    ws.Cells[rowIdx, 6].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 7].Value = _ReportDataModel.OverallStats.ingress;
                    ws.Cells[rowIdx, 7].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 8].Value = _ReportDataModel.OverallStats.egress;
                    ws.Cells[rowIdx, 8].Style.Font.Bold = true;

                    // Hourly data
                    ws.Cells[rowIdx + 0, 9].Value = "Occupied Duration";
                    ws.Cells[rowIdx + 1, 9].Value = "Max Possible Duration";
                    ws.Cells[rowIdx + 2, 9].Value = "% Occupied";
                    ws.Cells[rowIdx + 3, 9].Value = "Arrivals";
                    ws.Cells[rowIdx + 4, 9].Value = "Departures";
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 9, rowIdx + 4, 9])
                    {
                        col.Style.Font.Bold = true;
                    }
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 8])
                    {
                        try
                        {
                            rng.Merge = true; //Merge columns start and end range
                        }
                        catch { }
                    }

                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Overall hourly statistics
                        /*
                        ws.Cells[rowIdx, 10 + hourIdx].Value =
                            FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                            FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].ingress + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].egress;

                        ws.Cells[rowIdx, 10 + hourIdx].Style.WrapText = true;
                        ws.Cells[rowIdx, 10 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        */
                        ws.Cells[rowIdx + 0, 10 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime);
                        ws.Cells[rowIdx + 1, 10 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                        ws.Cells[rowIdx + 2, 10 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy;
                        ApplyNumberStyleToCell(ws, rowIdx + 2, 10 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                        ws.Cells[rowIdx + 3, 10 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].ingress;
                        ApplyNumberStyleToCell(ws, rowIdx + 3, 10 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                        ws.Cells[rowIdx + 4, 10 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].egress;
                        ApplyNumberStyleToCell(ws, rowIdx + 4, 10 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                    }


                    // Autofilters aren't appropriate anymore since we introduced grouping rows by area, and also have subtotal rows
                    /*
                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 8])
                    {
                        rng.AutoFilter = true;
                    }
                    */

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Meter ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 2 is numeric integer (Area ID)
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 6 is numeric float
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "###0.00", ExcelHorizontalAlignment.Right);

                    // Column 7 is numeric integer
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 8 is numeric integer
                    ApplyNumberStyleToColumn(ws, 8, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 8; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                            col.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        }
                    }

                    // Some columns will have set width instead of autofit (autofit doesn't work when we have wrap text?)
                    ws.Column(5).Width = 20;
                    ws.Column(7).Width = 12;
                    ws.Column(8).Width = 12;
                }


                // Should we include a worksheet with Space aggregates?
                if (includeSpaceSummary == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Space Occupancy");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Space #";
                    ws.Cells[rowIdx, 2].Value = "Meter #";

                    ws.Cells[rowIdx, 3].Value = "Area #";
                    ws.Cells[rowIdx, 4].Value = "Area";

                    ws.Cells[rowIdx, 5].Value = "Occupied Duration";

                    ws.Cells[rowIdx, 6].Value = "Max Possible" + Environment.NewLine + "Occupied Duration";
                    ws.Cells[rowIdx, 6].Style.WrapText = true;
                    ws.Column(6).Width = 20;

                    ws.Cells[rowIdx, 7].Value = "Occupied %";

                    ws.Cells[rowIdx, 8].Value = "Vehicle" + Environment.NewLine + "Arrivals";
                    ws.Cells[rowIdx, 8].Style.WrapText = true;
                    ws.Column(8).Width = 12;

                    ws.Cells[rowIdx, 9].Value = "Vehicle" + Environment.NewLine + "Departures";
                    ws.Cells[rowIdx, 9].Style.WrapText = true;
                    ws.Column(9).Width = 12;

                    ws.Cells[rowIdx, 10].Value = "Hourly Category";
                    ws.Column(10).Width = 24;

                    tempHourlyTime = DateTime.Today;
                    tempHourlyTime2 = DateTime.Today.AddHours(1);
                    for (int hourlyColumn = 11; hourlyColumn < 11 + 24; hourlyColumn++)
                    {
                        ws.Cells[rowIdx, hourlyColumn].Value = tempHourlyTime.ToString("h ") + "-" + tempHourlyTime2.ToString(" h tt").ToLower();
                        tempHourlyTime = tempHourlyTime.AddHours(1);
                        tempHourlyTime2 = tempHourlyTime2.AddHours(1);
                        ws.Column(hourlyColumn).Width = 14;
                    }

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 9])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    // Format hourly portion of the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 10, rowIdx, 10])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OliveDrab);
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }
                    // Format hourly portion of the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 11, rowIdx, 11 + 23])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateBlue /*System.Drawing.Color.FromArgb(79, 129, 189)*/);  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    // We need to group and subtotal by areas, so we will loop through each area and then report meter stats within each area
                    foreach (SensingAndPaymentStatistic_Area areaStat in this._ReportDataModel.AreaStats)
                    {
                        foreach (SensingAndPaymentStatistic_BayAndMeter bayStat in this._ReportDataModel.BayStats)
                        {
                            // Skip this bay stat if its not applicable to current area
                            if (bayStat.AreaID != areaStat.AreaID)
                                continue;

                            // Output row values for data
                            ws.Cells[rowIdx, 1].Value = bayStat.BayID;
                            ws.Cells[rowIdx, 2].Value = bayStat.MeterID;
                            ws.Cells[rowIdx, 3].Value = bayStat.AreaID;
                            ws.Cells[rowIdx, 4].Value = bayStat.AreaName;
                            ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.TotalOccupancyTime);
                            ws.Cells[rowIdx, 6].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.MaximumPotentialOccupancyTime);
                            ws.Cells[rowIdx, 7].Value = bayStat.PercentageOccupancy;
                            ws.Cells[rowIdx, 8].Value = bayStat.ingress;
                            ws.Cells[rowIdx, 9].Value = bayStat.egress;

                            // Hourly data
                            ws.Cells[rowIdx + 0, 10].Value = "Occupied Duration";
                            ws.Cells[rowIdx + 1, 10].Value = "Max Possible Duration";
                            ws.Cells[rowIdx + 2, 10].Value = "% Occupied";
                            ws.Cells[rowIdx + 3, 10].Value = "Arrivals";
                            ws.Cells[rowIdx + 4, 10].Value = "Departures";
                            using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 10, rowIdx + 4, 10])
                            {
                                col.Style.Font.Bold = true;
                            }
                            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 9])
                            {
                                try
                                {
                                    rng.Merge = true; //Merge columns start and end range
                                }
                                catch { }
                            }

                            for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                            {
                                // Overall hourly statistics
                                /*
                                ws.Cells[rowIdx, 11 + hourIdx].Value =
                                    FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                                    FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                                    bayStat.ChildStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                                    bayStat.ChildStats_Hourly[hourIdx].ingress + Environment.NewLine +
                                    bayStat.ChildStats_Hourly[hourIdx].egress;

                                ws.Cells[rowIdx, 11 + hourIdx].Style.WrapText = true;
                                ws.Cells[rowIdx, 11 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                */
                                ws.Cells[rowIdx + 0, 11 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime);
                                ws.Cells[rowIdx + 1, 11 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                                ws.Cells[rowIdx + 2, 11 + hourIdx].Value = bayStat.ChildStats_Hourly[hourIdx].PercentageOccupancy;
                                ApplyNumberStyleToCell(ws, rowIdx + 2, 11 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                                ws.Cells[rowIdx + 3, 11 + hourIdx].Value = bayStat.ChildStats_Hourly[hourIdx].ingress;
                                ApplyNumberStyleToCell(ws, rowIdx + 3, 11 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                                ws.Cells[rowIdx + 4, 11 + hourIdx].Value = bayStat.ChildStats_Hourly[hourIdx].egress;
                                ApplyNumberStyleToCell(ws, rowIdx + 4, 11 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                            }

                            // Increment the row index, which will now be the next row of our data
                            rowIdx += 5;
                        }

                        // Each bay for the current area has been reported, so no we need to output a subtotal for the area
                        ws.Cells[rowIdx, 1].Value = "SUBTOTAL AREA";
                        ws.Cells[rowIdx, 1].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 1, rowIdx, 4].Merge = true;

                        ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalOccupancyTime);
                        ws.Cells[rowIdx, 5].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 6].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.MaximumPotentialOccupancyTime);
                        ws.Cells[rowIdx, 6].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 7].Value = areaStat.PercentageOccupancy;
                        ws.Cells[rowIdx, 7].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 8].Value = areaStat.ingress;
                        ws.Cells[rowIdx, 8].Style.Font.Bold = true;
                        ws.Cells[rowIdx, 9].Value = areaStat.egress;
                        ws.Cells[rowIdx, 9].Style.Font.Bold = true;

                        // Hourly data
                        ws.Cells[rowIdx + 0, 10].Value = "Occupied Duration";
                        ws.Cells[rowIdx + 1, 10].Value = "Max Possible Duration";
                        ws.Cells[rowIdx + 2, 10].Value = "% Occupied";
                        ws.Cells[rowIdx + 3, 10].Value = "Arrivals";
                        ws.Cells[rowIdx + 4, 10].Value = "Departures";
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 10, rowIdx + 4, 10])
                        {
                            col.Style.Font.Bold = true;
                        }
                        using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 9])
                        {
                            try
                            {
                                rng.Merge = true; //Merge columns start and end range
                            }
                            catch { }
                        }

                        for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                        {
                            // Overall hourly statistics
                            /*
                            ws.Cells[rowIdx, 11 + hourIdx].Value =
                                FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                                FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].ingress + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].egress;

                            ws.Cells[rowIdx, 11 + hourIdx].Style.WrapText = true;
                            ws.Cells[rowIdx, 11 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            */
                            ws.Cells[rowIdx + 0, 11 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime);
                            ws.Cells[rowIdx + 1, 11 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                            ws.Cells[rowIdx + 2, 11 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].PercentageOccupancy;
                            ApplyNumberStyleToCell(ws, rowIdx + 2, 11 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                            ws.Cells[rowIdx + 3, 11 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].ingress;
                            ApplyNumberStyleToCell(ws, rowIdx + 3, 11 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                            ws.Cells[rowIdx + 4, 11 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].egress;
                            ApplyNumberStyleToCell(ws, rowIdx + 4, 11 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                        }

                        // Increment the row index, which will now be the next row of our data
                        rowIdx += 5;
                        rowIdx++;
                    }

                    // All bays and areas are outputted, so now we need a final total also
                    ws.Cells[rowIdx, 1].Value = "TOTAL";
                    ws.Cells[rowIdx, 1].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 1, rowIdx, 4].Merge = true;

                    ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats.TotalOccupancyTime);
                    ws.Cells[rowIdx, 5].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 6].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats.MaximumPotentialOccupancyTime);
                    ws.Cells[rowIdx, 6].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 7].Value = _ReportDataModel.OverallStats.PercentageOccupancy;
                    ws.Cells[rowIdx, 7].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 8].Value = _ReportDataModel.OverallStats.ingress;
                    ws.Cells[rowIdx, 8].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 9].Value = _ReportDataModel.OverallStats.egress;
                    ws.Cells[rowIdx, 9].Style.Font.Bold = true;

                    // Hourly data
                    ws.Cells[rowIdx + 0, 10].Value = "Occupied Duration";
                    ws.Cells[rowIdx + 1, 10].Value = "Max Possible Duration";
                    ws.Cells[rowIdx + 2, 10].Value = "% Occupied";
                    ws.Cells[rowIdx + 3, 10].Value = "Arrivals";
                    ws.Cells[rowIdx + 4, 10].Value = "Departures";
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 10, rowIdx + 4, 10])
                    {
                        col.Style.Font.Bold = true;
                    }
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 9])
                    {
                        try
                        {
                            rng.Merge = true; //Merge columns start and end range
                        }
                        catch { }
                    }

                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Overall hourly statistics
                        /*
                        ws.Cells[rowIdx, 11 + hourIdx].Value =
                            FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                            FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].ingress + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].egress;

                        ws.Cells[rowIdx, 11 + hourIdx].Style.WrapText = true;
                        ws.Cells[rowIdx, 11 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        */
                        ws.Cells[rowIdx + 0, 11 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime);
                        ws.Cells[rowIdx + 1, 11 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                        ws.Cells[rowIdx + 2, 11 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy;
                        ApplyNumberStyleToCell(ws, rowIdx + 2, 11 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                        ws.Cells[rowIdx + 3, 11 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].ingress;
                        ApplyNumberStyleToCell(ws, rowIdx + 3, 11 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                        ws.Cells[rowIdx + 4, 11 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].egress;
                        ApplyNumberStyleToCell(ws, rowIdx + 4, 11 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                    }

                    // Autofilters aren't appropriate anymore since we introduced grouping rows by area, and also have subtotal rows
                    /*
                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 9])
                    {
                        rng.AutoFilter = true;
                    }
                    */

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Bay ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 2 is numeric integer (Meter ID)
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 3 is numeric integer (Area ID)
                    ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 7 is numeric float
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "###0.00", ExcelHorizontalAlignment.Right);

                    // Column 8 is numeric integer
                    ApplyNumberStyleToColumn(ws, 8, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 9 is numeric integer
                    ApplyNumberStyleToColumn(ws, 9, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 9; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                            col.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        }
                    }

                    // Some columns will have set width instead of autofit (autofit doesn't work when we have wrap text?)
                    ws.Column(6).Width = 20;
                    ws.Column(7).Width = 12;
                    ws.Column(9).Width = 12;
                }


                // Should we include a worksheet with Area aggregates?
                if (includeAreaSummary == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Area Occupancy");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Area #";
                    ws.Cells[rowIdx, 2].Value = "Area";
                    ws.Cells[rowIdx, 3].Value = "Occupied Duration";
                    ws.Cells[rowIdx, 4].Value = "Max Possible" + Environment.NewLine + "Occupied Duration";
                    ws.Cells[rowIdx, 4].Style.WrapText = true;
                    ws.Column(4).Width = 20;
                    ws.Cells[rowIdx, 5].Value = "Occupied %";
                    ws.Cells[rowIdx, 6].Value = "Vehicle" + Environment.NewLine + "Arrivals";
                    ws.Cells[rowIdx, 6].Style.WrapText = true;
                    ws.Column(6).Width = 12;
                    ws.Cells[rowIdx, 7].Value = "Vehicle" + Environment.NewLine + "Departures";
                    ws.Cells[rowIdx, 7].Style.WrapText = true;
                    ws.Column(7).Width = 12;
                    ws.Cells[rowIdx, 8].Value = "Hourly Category";
                    ws.Column(8).Width = 24;

                    tempHourlyTime = DateTime.Today;
                    tempHourlyTime2 = DateTime.Today.AddHours(1);
                    for (int hourlyColumn = 9; hourlyColumn < 9 + 24; hourlyColumn++)
                    {
                        ws.Cells[rowIdx, hourlyColumn].Value = tempHourlyTime.ToString("h ") + "-" + tempHourlyTime2.ToString(" h tt").ToLower();
                        tempHourlyTime = tempHourlyTime.AddHours(1);
                        tempHourlyTime2 = tempHourlyTime2.AddHours(1);
                        ws.Column(hourlyColumn).Width = 14;
                    }

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 7])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    // Format hourly portion of the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 8, rowIdx, 8])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OliveDrab);
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }
                    // Format hourly portion of the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 9, rowIdx, 9 + 23])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateBlue /*System.Drawing.Color.FromArgb(79, 129, 189)*/);  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                        rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (SensingAndPaymentStatistic_Area areaStat in this._ReportDataModel.AreaStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = areaStat.AreaID;
                        ws.Cells[rowIdx, 2].Value = areaStat.AreaName;
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalOccupancyTime);
                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.MaximumPotentialOccupancyTime);
                        ws.Cells[rowIdx, 5].Value = areaStat.PercentageOccupancy;
                        ws.Cells[rowIdx, 6].Value = areaStat.ingress;
                        ws.Cells[rowIdx, 7].Value = areaStat.egress;

                        // Hourly data
                        ws.Cells[rowIdx + 0, 8].Value = "Occupied Duration";
                        ws.Cells[rowIdx + 1, 8].Value = "Max Possible Duration";
                        ws.Cells[rowIdx + 2, 8].Value = "% Occupied";
                        ws.Cells[rowIdx + 3, 8].Value = "Arrivals";
                        ws.Cells[rowIdx + 4, 8].Value = "Departures";
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 8, rowIdx + 4, 8])
                        {
                            col.Style.Font.Bold = true;
                        }
                        using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 7])
                        {
                            try
                            {
                                rng.Merge = true; //Merge columns start and end range
                            }
                            catch { }
                        }

                        for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                        {
                            // Overall hourly statistics
                            /*
                            ws.Cells[rowIdx, 9 + hourIdx].Value =
                                FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                                FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].ingress + Environment.NewLine +
                                areaStat.ChildStats_Hourly[hourIdx].egress;

                            ws.Cells[rowIdx, 9 + hourIdx].Style.WrapText = true;
                            ws.Cells[rowIdx, 9 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            */
                            ws.Cells[rowIdx + 0, 9 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].TotalOccupancyTime);
                            ws.Cells[rowIdx + 1, 9 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.ChildStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                            ws.Cells[rowIdx + 2, 9 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].PercentageOccupancy;
                            ApplyNumberStyleToCell(ws, rowIdx + 2, 9 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                            ws.Cells[rowIdx + 3, 9 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].ingress;
                            ApplyNumberStyleToCell(ws, rowIdx + 3, 9 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                            ws.Cells[rowIdx + 4, 9 + hourIdx].Value = areaStat.ChildStats_Hourly[hourIdx].egress;
                            ApplyNumberStyleToCell(ws, rowIdx + 4, 9 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                        }


                        // Increment the row index, which will now be the next row of our data
                        rowIdx += 5;
                    }

                    rowIdx++;

                    // All meters and area are outputted, so now we need a final total also
                    ws.Cells[rowIdx, 1].Value = "TOTAL";
                    ws.Cells[rowIdx, 1].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 1, rowIdx, 2].Merge = true;

                    ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats.TotalOccupancyTime);
                    ws.Cells[rowIdx, 3].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats.MaximumPotentialOccupancyTime);
                    ws.Cells[rowIdx, 4].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 5].Value = _ReportDataModel.OverallStats.PercentageOccupancy;
                    ws.Cells[rowIdx, 5].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 6].Value = _ReportDataModel.OverallStats.ingress;
                    ws.Cells[rowIdx, 6].Style.Font.Bold = true;
                    ws.Cells[rowIdx, 7].Value = _ReportDataModel.OverallStats.egress;
                    ws.Cells[rowIdx, 7].Style.Font.Bold = true;

                    // Hourly data
                    ws.Cells[rowIdx + 0, 8].Value = "Occupied Duration";
                    ws.Cells[rowIdx + 1, 8].Value = "Max Possible Duration";
                    ws.Cells[rowIdx + 2, 8].Value = "% Occupied";
                    ws.Cells[rowIdx + 3, 8].Value = "Arrivals";
                    ws.Cells[rowIdx + 4, 8].Value = "Departures";
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[rowIdx, 8, rowIdx + 4, 8])
                    {
                        col.Style.Font.Bold = true;
                    }
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx + 1, 1, rowIdx + 4, 7])
                    {
                        try
                        {
                            rng.Merge = true; //Merge columns start and end range
                        }
                        catch { }
                    }


                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Overall hourly statistics
                        /*
                        ws.Cells[rowIdx, 9 + hourIdx].Value =
                            FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime) + Environment.NewLine +
                            FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime) + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy.ToString("###0.00") + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].ingress + Environment.NewLine +
                            _ReportDataModel.OverallStats_Hourly[hourIdx].egress;

                        ws.Cells[rowIdx, 9 + hourIdx].Style.WrapText = true;
                        ws.Cells[rowIdx, 9 + hourIdx].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        */
                        ws.Cells[rowIdx + 0, 9 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].TotalOccupancyTime);
                        ws.Cells[rowIdx + 1, 9 + hourIdx].Value = FormatTimeSpanAsHoursMinutesAndSeconds(_ReportDataModel.OverallStats_Hourly[hourIdx].MaximumPotentialOccupancyTime);

                        ws.Cells[rowIdx + 2, 9 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].PercentageOccupancy;
                        ApplyNumberStyleToCell(ws, rowIdx + 2, 9 + hourIdx, "###0.00", ExcelHorizontalAlignment.Left);

                        ws.Cells[rowIdx + 3, 9 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].ingress;
                        ApplyNumberStyleToCell(ws, rowIdx + 3, 9 + hourIdx, "########0", ExcelHorizontalAlignment.Left);

                        ws.Cells[rowIdx + 4, 9 + hourIdx].Value = _ReportDataModel.OverallStats_Hourly[hourIdx].egress;
                        ApplyNumberStyleToCell(ws, rowIdx + 4, 9 + hourIdx, "########0", ExcelHorizontalAlignment.Left);
                    }

                    // Autofilters aren't appropriate anymore since we introduced grouping rows by area, and also have subtotal rows
                    /*
                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 7])
                    {
                        rng.AutoFilter = true;
                    }
                    */

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Area ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 5 is numeric float
                    ApplyNumberStyleToColumn(ws, 5, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 6 is numeric integer
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 7 is numeric integer
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 7; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                            col.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                        }
                    }

                    // Some columns will have set width instead of autofit (autofit doesn't work when we have wrap text?)
                    ws.Column(4).Width = 20;
                    ws.Column(6).Width = 12;
                    ws.Column(7).Width = 12;
                }


                // All cells in spreadsheet are populated now, so render (save the file) to a memory stream 
                byte[] bytes = pck.GetAsByteArray();
                ms.Write(bytes, 0, bytes.Length);
            }

            // Stop diagnostics timer
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Occupancy Report Generation took: " + sw.Elapsed.ToString());
        }
        #endregion

        #region Private/Protected Methods
        protected void GatherReportData(List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime)
        {
            this._ReportDataModel = new SensingAndPaymentStatistic_ReportData();
            this._ReportDataModel.WantMeterStats = true;
            this._ReportDataModel.WantMeterStats_Hourly = true;
            this._ReportDataModel.WantBayStats = true;
            this._ReportDataModel.WantBayStats_Hourly = true;
            this._ReportDataModel.WantAreaStats = true;
            this._ReportDataModel.WantAreaStats_Hourly = true;

            // Adjust the date ranges as needed for our SQL queries.
            // The end time needs to be inclusive of the entire minute (seconds and milliseconds are not in the resolution of the passed EndTime parameter)
            DateTime AdjustedStartTime;
            DateTime AdjustedEndTime;
            AdjustedStartTime = StartTime;
            AdjustedEndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTime.Hour, EndTime.Minute, 0).AddMinutes(1);

            // Gather all applicable vehicle sensing data (minimizes how many individual SQL queries must be executed)
            List<HistoricalSensingRecord> RawSensingDataForAllMeters = new SensingDatabaseSource(_CustomerConfig).GetHistoricalVehicleSensingDataForMeters_StronglyTyped(
                this._CustomerConfig.CustomerId, listOfMeterIDs, AdjustedStartTime, AdjustedEndTime, true);

            // DEBUG: We are not gathering payment records for this report yet, so for now we will just send an empty list...
            List<PaymentRecord> RawPaymentDataForAllMeters = new List<PaymentRecord>();

            // Analyze data for each meter
            foreach (int nextMeterID in listOfMeterIDs)
            {
                AnalyzeDataForMeter(RawSensingDataForAllMeters, RawPaymentDataForAllMeters, nextMeterID, AdjustedStartTime, AdjustedEndTime);
            }

            // Gather a unique list of AreaIDs associated with all meters involved in this report
            List<int> listOfAreaIDs = new List<int>();
            foreach (int nextMeterID in listOfMeterIDs)
            {
                int resolvedAreaIDForMeterID = ResolveAreaIDForMeterID(nextMeterID);
                if (listOfAreaIDs.Contains(resolvedAreaIDForMeterID) == false)
                    listOfAreaIDs.Add(resolvedAreaIDForMeterID);
            }

            // Now for each unique area included in the report, we will gather aggregate data (based on meter aggregates)
            foreach (int nextAreaID in listOfAreaIDs)
            {
                // Find or create an area stats object for the current AreaID
                SensingAndPaymentStatistic_Area AreaStatsObj = null;
                foreach (SensingAndPaymentStatistic_Area existingArea in this._ReportDataModel.AreaStats)
                {
                    if (existingArea.AreaID == nextAreaID)
                    {
                        AreaStatsObj = existingArea;
                        break;
                    }
                }
                if (AreaStatsObj == null)
                {
                    AreaStatsObj = new SensingAndPaymentStatistic_Area(this.cachedAssets);
                    AreaStatsObj.WantHourlyStats = this._ReportDataModel.WantAreaStats_Hourly;
                    AreaStatsObj.AreaID = nextAreaID;
                    AreaStatsObj.AreaName = ResolvedAreaNameForAreaID(nextAreaID);
                    AreaStatsObj.ChildStats_Hourly.Clear();
                    this._ReportDataModel.AreaStats.Add(AreaStatsObj);
                }

                // Now do aggregation for the area
                AreaStatsObj.AggregateSelfFromMeterStats(this._ReportDataModel, this._CustomerConfig);
            }

            // We will also do an overall stats (based on meter aggregates)
            foreach (SensingAndPaymentStatistic_Meter meterStat in this._ReportDataModel.MeterStats)
            {
                this._ReportDataModel.OverallStats.egress += meterStat.egress;
                this._ReportDataModel.OverallStats.ingress += meterStat.ingress;
                this._ReportDataModel.OverallStats.MaximumPotentialOccupancyTime += meterStat.MaximumPotentialOccupancyTime;
                this._ReportDataModel.OverallStats.PaymentCount += meterStat.PaymentCount;
                this._ReportDataModel.OverallStats.TotalOccupancyPaidTime += meterStat.TotalOccupancyPaidTime;
                this._ReportDataModel.OverallStats.TotalOccupancyTime += meterStat.TotalOccupancyTime;
                this._ReportDataModel.OverallStats.TotalPaidTime += meterStat.TotalPaidTime;
            }
            this._ReportDataModel.OverallStats.AggregateSelf();


            // Need to accumulate the overall hourly stats too
            this._ReportDataModel.OverallStats_Hourly.Clear();
            SensingAndPaymentStatistic_Common overallStatsForHour = null;
            SensingAndPaymentStatistic_Common MeterStatsForHour = null;
            // Add 24 shell objects to account for each hour of the day
            for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
            {
                overallStatsForHour = new SensingAndPaymentStatistic_Common();
                this._ReportDataModel.OverallStats_Hourly.Add(hourOfDay, overallStatsForHour);

                foreach (SensingAndPaymentStatistic_Meter meterStat in this._ReportDataModel.MeterStats)
                {
                    // Get the meter stats for current hour
                    MeterStatsForHour = meterStat.ChildStats_Hourly[hourOfDay];

                    // Update current hour stats for meter from current hour stats for bay
                    overallStatsForHour.TotalOccupancyTime += MeterStatsForHour.TotalOccupancyTime;
                    overallStatsForHour.TotalPaidTime += MeterStatsForHour.TotalPaidTime;
                    overallStatsForHour.TotalOccupancyPaidTime += MeterStatsForHour.TotalOccupancyPaidTime;
                    overallStatsForHour.MaximumPotentialOccupancyTime += MeterStatsForHour.MaximumPotentialOccupancyTime;
                    overallStatsForHour.egress += MeterStatsForHour.egress;
                    overallStatsForHour.ingress += MeterStatsForHour.ingress;
                    overallStatsForHour.PaymentCount += MeterStatsForHour.PaymentCount;
                }
                overallStatsForHour.AggregateSelf();
            }







            // Sort this.ReportDataModel.AreaStats by AreaName so it renders in Excel in a nice sort order
            this._ReportDataModel.AreaStats.Sort(new SensingAndPaymentStatistic_AreaLogicalComparer(true));

            // Sort this.ReportDataModel.MeterStats by MeterID so it renders in Excel in a nice sort order
            this._ReportDataModel.MeterStats.Sort(new SensingAndPaymentStatistic_WithChildrenLogicalComparer());

            // Sort this.ReportDataModel.BayStats by BayID so it renders in Excel in a nice sort order
            this._ReportDataModel.BayStats.Sort(new SensingAndPaymentStatistic_BayAndMeterLogicalComparer());
        }

        protected void AnalyzeDataForMeter(List<HistoricalSensingRecord> allSensingData, List<PaymentRecord> allPaymentData, int meterId, DateTime startTime, DateTime endTime_NotInclusive)
        {
            // Get list of all Spaces associated with the MeterID
            List<int> SortedBayIds = new List<int>();
            List<SpaceAsset> spaceAssets = CustomerLogic.CustomerManager.GetSpaceAssetsForMeter(_CustomerConfig, meterId);
            foreach (SpaceAsset asset in spaceAssets)
            {
                SortedBayIds.Add(asset.SpaceID);
            }
            SortedBayIds.Sort();

            // Process data for each space associated with the MeterID
            foreach (int nextBayID in SortedBayIds)
            {
                // Get filtered list of vehicle sensing and payment data that applies to the current meter and space
                List<HistoricalSensingRecord> VSDataForCurrentMeterAndBay = GetSensingDataSubsetForMeterIDAndBayID(allSensingData, meterId, nextBayID);
                List<PaymentRecord> PaymentDataForCurrentMeterAndBay = GetPaymentDataSubsetForMeterIDAndBayID(allPaymentData, meterId, nextBayID);

                // Analyze the data for this space
                AnalyzeDataForSpace(VSDataForCurrentMeterAndBay, PaymentDataForCurrentMeterAndBay,
                    nextBayID, meterId, nextBayID, startTime, endTime_NotInclusive);
            }

            // Create a meter statistics object for the current meter, and add to our report list
            SensingAndPaymentStatistic_Meter MeterStatsObj = new SensingAndPaymentStatistic_Meter();
            MeterStatsObj.WantHourlyStats = this._ReportDataModel.WantMeterStats_Hourly;
            MeterStatsObj.MeterID = meterId;
            MeterStatsObj.ChildStats_Hourly.Clear();
            MeterStatsObj.AreaID = ResolveAreaIDForMeterID(meterId);
            MeterStatsObj.AreaName = ResolvedAreaNameForAreaID(MeterStatsObj.AreaID);
            this._ReportDataModel.MeterStats.Add(MeterStatsObj);

            // Now do aggregation for the meter
            MeterStatsObj.AggregateSelfFromBayStats(_ReportDataModel);
        }

        protected void AnalyzeDataForSpace(List<HistoricalSensingRecord> Sensing, List<PaymentRecord> Payment, int legacyBitNumber,
            int meterId, int bayNumber, DateTime startTime, DateTime endTime_NotInclusive)
        {
            // Create a bay statistics object for the current bay (and meter), and add to our report list
            SensingAndPaymentStatistic_BayAndMeter BayStatsObj = new SensingAndPaymentStatistic_BayAndMeter();
            BayStatsObj.WantHourlyStats = this._ReportDataModel.WantBayStats_Hourly;
            BayStatsObj.BayID = bayNumber;
            BayStatsObj.MeterID = meterId;

            BayStatsObj.AreaID = ResolveAreaIDForMeterID(meterId);
            BayStatsObj.AreaName = ResolvedAreaNameForAreaID(BayStatsObj.AreaID);

            BayStatsObj.ChildStats_Hourly.Clear();
            this._ReportDataModel.BayStats.Add(BayStatsObj);

            SensingAndPaymentStatistic_Common currHourlyStatObj = null;

            // Init the result object with a child statistic object for each hour of a day
            for (int hourOfDay = 0; hourOfDay < 24; hourOfDay++)
            {
                currHourlyStatObj = new SensingAndPaymentStatistic_Common();

                DateTime tempStartTime = startTime;
                DateTime tempEndTime = new DateTime(endTime_NotInclusive.Year, endTime_NotInclusive.Month, endTime_NotInclusive.Day, endTime_NotInclusive.Hour,
                    endTime_NotInclusive.Minute, endTime_NotInclusive.Second, endTime_NotInclusive.Millisecond);

                while (tempStartTime < tempEndTime)
                {
                    if (tempStartTime.Hour == hourOfDay)
                    {
                        DateTime TestStartTime1 = new DateTime(tempStartTime.Ticks);

                        // Get a time that is the end of the current hour
                        DateTime TestEndTime1 = new DateTime(TestStartTime1.Year, TestStartTime1.Month, TestStartTime1.Day,
                            TestStartTime1.Hour, 59, 59, 999);

                        // Normally we want to use the end of the current hour, but we don't want to exceed the reports ending timestamp
                        long TestEndTimeTicks = Math.Min(TestEndTime1.Ticks, endTime_NotInclusive.Ticks);
                        TestEndTime1 = new DateTime(TestEndTimeTicks);

                        // Only accumulate as potential time if its inside a time period allowed by the chosen activity restrictions (All, regulated hours, or unregulated hours)
                        if (IsTimestampAllowedForActivityRestrictionInEffect(TestStartTime1, meterId, bayNumber))
                        {
                            // Get difference between times, then add 1 millisecond (so we generally end up with 1 hour instead of 59 minutes, 59 seconds, and 999 milliseconds, etc...)
                            currHourlyStatObj.MaximumPotentialOccupancyTime += (TestEndTime1 - TestStartTime1) + (new TimeSpan(0, 0, 0, 0, 1));
                        }
                    }
                    tempStartTime = tempStartTime.AddHours(1);
                }
                BayStatsObj.ChildStats_Hourly.Add(hourOfDay, currHourlyStatObj);
            }

            // Start Analyzing data here 

            List<HistoricalSensingRecord> rawSensingData = Sensing;
            List<PaymentRecord> PaymentsFilteredForBay = Payment;

            // Need to know current time in customer's timezone
            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

            int SensingCursor = 0;
            bool SensingCursorAtEnd = (rawSensingData.Count == 0);

            int PaymentCursor = 0;
            bool PaymentCursorAtEnd = (PaymentsFilteredForBay.Count == 0);

            bool HourCursorAtEnd = false;

            bool IsOccupied = false;
            DateTime NextExpiration = DateTime.MinValue;

            DateTime nextDateToAnalyze = startTime;
            DateTime HourStartTime = new DateTime();
            DateTime HourEndTime = new DateTime();
            DateTime ProcessedTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 0, 0, 0, 0);

            object NextReportObj = null;
            DateTime NextReportObjTimeStamp = new DateTime();

            while (nextDateToAnalyze < endTime_NotInclusive)
            {
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    currHourlyStatObj = BayStatsObj.ChildStats_Hourly[hourIdx];

                    // Find the start and end of the current hour period, including the date
                    HourStartTime = new DateTime(nextDateToAnalyze.Year, nextDateToAnalyze.Month, nextDateToAnalyze.Day, hourIdx, 0, 0, 0);
                    HourEndTime = new DateTime(nextDateToAnalyze.Year, nextDateToAnalyze.Month, nextDateToAnalyze.Day, hourIdx, 59, 59, 999);

                    // Reset end-of-hour cursor indicator at the start of every new reportable hour period
                    HourCursorAtEnd = false;

                    // Loop until no more data (or forced break out of loop)
                    while (!SensingCursorAtEnd || !PaymentCursorAtEnd || !HourCursorAtEnd)
                    {
                        // End-of-data indicators need to be updated now to reflect current reality
                        SensingCursorAtEnd = (SensingCursor == rawSensingData.Count);
                        PaymentCursorAtEnd = (PaymentCursor == PaymentsFilteredForBay.Count);

                        // Find the next reportable object (payment, sensing, or end-of-hour)
                        // Start with the end of the hour, then see if there are any earlier
                        // payments or sensing that records that should be considered first
                        NextReportObj = HourEndTime;
                        NextReportObjTimeStamp = HourEndTime;

                        if (!SensingCursorAtEnd)
                        {
                            if (rawSensingData[SensingCursor].DateTime <= NextReportObjTimeStamp)
                            {
                                NextReportObj = rawSensingData[SensingCursor];
                                NextReportObjTimeStamp = rawSensingData[SensingCursor].DateTime;
                            }
                        }

                        if (!PaymentCursorAtEnd)
                        {
                            if (PaymentsFilteredForBay[PaymentCursor].TransactionDateTime <= NextReportObjTimeStamp)
                            {
                                NextReportObj = PaymentsFilteredForBay[PaymentCursor];
                                NextReportObjTimeStamp = PaymentsFilteredForBay[PaymentCursor].TransactionDateTime;
                            }
                        }

                        if (NextReportObj is DateTime)
                        {
                            // Determine if the current hour start time or the last processed time is more recent
                            DateTime GreaterOfHourStartTimeOrProcessedTime = ProcessedTime;
                            if (HourStartTime > ProcessedTime)
                                GreaterOfHourStartTimeOrProcessedTime = HourStartTime;

                            // Figure elapsed time from either the HourStartTime or the last ProcessedTime (whichever is greater)
                            TimeSpan elpased = (NextReportObjTimeStamp - GreaterOfHourStartTimeOrProcessedTime);

                            // We can't entirely trust the DB to have valid data, so don't accumulate occupancy elapsed times that are in the future
                            // (This might happen if the vehicle sensor provider (meter or gateway) has an incorrect time, or if the report end time
                            // surpasses actual time
                            if ((NextReportObjTimeStamp > NowAtDestination) && (NowAtDestination > HourEndTime))
                            {
                                elpased = new TimeSpan(0);
                            }
                            else if (NextReportObjTimeStamp > NowAtDestination)
                            {
                                if (NowAtDestination < GreaterOfHourStartTimeOrProcessedTime)
                                    elpased = new TimeSpan(0);
                                else
                                    elpased = (NowAtDestination - GreaterOfHourStartTimeOrProcessedTime);
                            }

                            // Clean-up the elapsed time by removing the milliseconds portion and adding 1 complete second
                            if (elpased.Ticks > 0)
                            {
                                elpased = elpased.Add(new TimeSpan(0, 0, 0, 0, (-1) * (elpased.Milliseconds)));
                                elpased = elpased.Add(new TimeSpan(0, 0, 1));
                            }
                            else if (elpased.Ticks < 0)
                            {
                                Debug.WriteLine("Why is elapsed time a negative value?");
                            }

                            // Is occupancy flag currently set?
                            if (IsOccupied)
                            {
                                // Update occupied duration, but only if timeslot is in a timeframe allowed by chosen activity restrictions
                                if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                    currHourlyStatObj.TotalOccupancyTime += elpased;
                            }

                            // Is it currently paid?
                            if (NextExpiration > ProcessedTime)
                            {
                                // Is expiration before the next report obj timestamp?
                                // If so, elapsed duration will now be up-to expiration date instead
                                if (NextExpiration < NextReportObjTimeStamp)
                                {
                                    elpased = (NextExpiration - GreaterOfHourStartTimeOrProcessedTime);

                                    // Clean-up the elapsed time by removing the milliseconds portion and adding 1 complete second
                                    if (elpased.Ticks > 0)
                                    {
                                        elpased = elpased.Add(new TimeSpan(0, 0, 0, 0, (-1) * (elpased.Milliseconds)));
                                        elpased = elpased.Add(new TimeSpan(0, 0, 1));
                                    }
                                    else if (elpased.Ticks < 0)
                                    {
                                        Debug.WriteLine("Why is elapsed time a negative value?");
                                    }
                                }

                                // Update paid duration, but only if timeslot is in a timeframe allowed by chosen activity restrictions
                                if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                    currHourlyStatObj.TotalPaidTime += elpased;

                                // Is it also occupied?
                                if (IsOccupied)
                                {
                                    // Update occupied with payment durations, but only if timeslot is in a timeframe allowed by chosen activity restrictions
                                    if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                        currHourlyStatObj.TotalOccupancyPaidTime += elpased;
                                }
                            }

                            // Set the report object timestamp as the last processed timestamp
                            ProcessedTime = NextReportObjTimeStamp.AddMilliseconds(1);

                            // We finished processing this hour, so set end-of-hour cursor indicator
                            HourCursorAtEnd = true;

                            // No more payment or sensing records were for this hour, so we can break out of
                            // payment and sensing data search loop now
                            break;
                        }
                        else if (NextReportObj is HistoricalSensingRecord)
                        {
                            HistoricalSensingRecord currSensing = (HistoricalSensingRecord)(NextReportObj);

                            // Determine if the current hour start time or the last processed time is more recent
                            DateTime GreaterOfHourStartTimeOrProcessedTime = ProcessedTime;
                            if (HourStartTime > ProcessedTime)
                                GreaterOfHourStartTimeOrProcessedTime = HourStartTime;

                            // Figure elapsed time from either the HourStartTime or the last ProcessedTime (whichever is greater)
                            TimeSpan elpased = (NextReportObjTimeStamp - GreaterOfHourStartTimeOrProcessedTime);

                            // We can't entirely trust the DB to have valid data, so don't accumulate occupancy elapsed times that are in the future
                            // (This might happen if the vehicle sensor provider (meter or gateway) has an incorrect time
                            if ((NextReportObjTimeStamp > NowAtDestination) && (NowAtDestination > HourEndTime))
                            {
                                elpased = new TimeSpan(0);
                            }
                            else if (NextReportObjTimeStamp > NowAtDestination)
                            {
                                if (NowAtDestination < GreaterOfHourStartTimeOrProcessedTime)
                                    elpased = new TimeSpan(0);
                                else
                                    elpased = (NowAtDestination - GreaterOfHourStartTimeOrProcessedTime);
                            }

                            // Clean-up the elapsed time by removing the milliseconds portion and adding 1 complete second
                            elpased = elpased.Add(new TimeSpan(0, 0, 0, 0, (-1) * (elpased.Milliseconds)));
                            elpased = elpased.Add(new TimeSpan(0, 0, 1));

                            if (elpased.Ticks < 0)
                            {
                                Debug.WriteLine("Why is elapsed time a negative value?");
                            }

                            // Does the raw data indicate the space is occupied?
                            if (currSensing.IsOccupied == true)
                            {
                                // Increase ingress count only if we detected a change from vacant to occupied, but only if timeslot is in a timeframe allowed by chosen activity restrictions
                                if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                {
                                    if (IsOccupied == false)
                                    {
                                        currHourlyStatObj.ingress++;
                                    }
                                    else
                                    {
                                        currHourlyStatObj.ingress++;
                                    }
                                }

                                // Now flag it as currently occupied
                                IsOccupied = true;
                            }
                            else
                            {
                                // Was the bay previously determined to be occupied (and now raw data indicates its vacant)?
                                if (IsOccupied == true)
                                {
                                    if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                    {
                                        // Increase egress count because we detected a change from occupied to vacant.
                                        currHourlyStatObj.egress++;

                                        // It was occupied until now, so update the occupied duration
                                        currHourlyStatObj.TotalOccupancyTime += elpased;
                                    }

                                    // Is it currently paid?
                                    if (NextExpiration > ProcessedTime)
                                    {
                                        // Is expiration before the next report obj timestamp?
                                        // If so, elapsed duration will now be up-to expiration date instead
                                        if (NextExpiration < NextReportObjTimeStamp)
                                        {
                                            if (HourStartTime > ProcessedTime)
                                                elpased = (NextExpiration - HourStartTime);
                                            else
                                                elpased = (NextExpiration - ProcessedTime);
                                        }

                                        // Update paid duration and duration of paid with occupancy
                                        if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                        {
                                            currHourlyStatObj.TotalPaidTime += elpased;
                                            currHourlyStatObj.TotalOccupancyPaidTime += elpased;
                                        }
                                    }
                                }
                                else
                                {
                                    if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                        currHourlyStatObj.egress++;
                                }
                                // Now flag it as currently vacant
                                IsOccupied = false;
                            }

                            // Set the report object timestamp as the last processed timestamp
                            ProcessedTime = NextReportObjTimeStamp;

                            // This sensing record has been used, so increment the sensing cursor
                            SensingCursor++;
                        }

                        else if (NextReportObj is PaymentRecord)
                        {
                            PaymentRecord currPayment = (NextReportObj as PaymentRecord);

                            // Determine if the current hour start time or the last processed time is more recent
                            DateTime GreaterOfHourStartTimeOrProcessedTime = ProcessedTime;
                            if (HourStartTime > ProcessedTime)
                                GreaterOfHourStartTimeOrProcessedTime = HourStartTime;

                            // Figure elapsed time from either the HourStartTime or the last ProcessedTime (whichever is greater)
                            TimeSpan elpased = (NextReportObjTimeStamp - GreaterOfHourStartTimeOrProcessedTime);

                            // We can't entirely trust the DB to have valid data, so don't accumulate occupancy elapsed times that are in the future
                            // (This might happen if the vehicle sensor provider (meter or gateway) has an incorrect time
                            if ((NextReportObjTimeStamp > NowAtDestination) && (NowAtDestination > HourEndTime))
                            {
                                elpased = new TimeSpan(0);
                            }
                            else if (NextReportObjTimeStamp > NowAtDestination)
                            {
                                if (NowAtDestination < GreaterOfHourStartTimeOrProcessedTime)
                                    elpased = new TimeSpan(0);
                                else
                                    elpased = (NowAtDestination - GreaterOfHourStartTimeOrProcessedTime);
                            }

                            // Clean-up the elapsed time by removing the milliseconds portion and adding 1 complete second
                            elpased = elpased.Add(new TimeSpan(0, 0, 0, 0, (-1) * (elpased.Milliseconds)));
                            elpased = elpased.Add(new TimeSpan(0, 0, 1));

                            if (elpased.Ticks < 0)
                            {
                                Debug.WriteLine("Why is elapsed time a negative value?");
                            }

                            // Update the payment count
                            if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                currHourlyStatObj.PaymentCount++;

                            // If occupied, update the occupied duration
                            if (IsOccupied)
                            {
                                if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                    currHourlyStatObj.TotalOccupancyTime += elpased;
                            }

                            // Is it currently paid before this payment?
                            if (NextExpiration >= NextReportObjTimeStamp)
                            {
                                if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                    currHourlyStatObj.TotalPaidTime += elpased;

                                // If its also occupied, then update the duration of paid with occupancy
                                if (IsOccupied)
                                {
                                    if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                        currHourlyStatObj.TotalOccupancyPaidTime += elpased;
                                }
                            }

                            // Now update the next expiration
                            if (NextExpiration >= NextReportObjTimeStamp)
                                NextExpiration = NextExpiration.Add(currPayment.TimePaid);
                            else
                                NextExpiration = currPayment.TransactionDateTime.Add(currPayment.TimePaid);

                            // Set the report object timestamp as the last processed timestamp
                            ProcessedTime = NextReportObjTimeStamp;

                            // This payment record has been used, so increment the payment cursor
                            PaymentCursor++;
                        }


                        // Update end-of-data indicator for the payment and sensing data cursors
                        SensingCursorAtEnd = (SensingCursor == rawSensingData.Count);

                        PaymentCursorAtEnd = (PaymentCursor == PaymentsFilteredForBay.Count);
                    }

                    // Finished the data gathering for current hourly period, so aggregate its totals
                    currHourlyStatObj.AggregateSelf();
                }

                // DEBUG: If we wanted daily aggregates for the bay, the aggregate logic would need to go here...

                // Increment to process the next day in the reportable period
                nextDateToAnalyze = nextDateToAnalyze.AddDays(1);

                // Reset the data counters to process the next day
                SensingCursorAtEnd = (rawSensingData.Count == 0);

                PaymentCursorAtEnd = (PaymentsFilteredForBay.Count == 0);

                HourCursorAtEnd = false;
            }

            // Finished the data analysis of the current bay for entire reporting period, so aggregate its totals
            BayStatsObj.AggregateSelf();
        }

        protected List<HistoricalSensingRecord> GetSensingDataSubsetForMeterIDAndBayID(List<HistoricalSensingRecord> allSensingData, int meterID, int bayID)
        {
            List<HistoricalSensingRecord> result = new List<HistoricalSensingRecord>();
            foreach (HistoricalSensingRecord nextRec in allSensingData)
            {
                if ((nextRec.MeterMappingId == meterID) && (nextRec.BayId == bayID))
                    result.Add(nextRec);
            }

            // Make sure this set of data is sorted by date, in ascending order!
            result.Sort(new Duncan.PEMS.SpaceStatus.DataSuppliers.HistoricalSensingRecordsSorter(true));

            return result;
        }

        protected List<PaymentRecord> GetPaymentDataSubsetForMeterIDAndBayID(List<PaymentRecord> allPaymentData, int meterID, int bayID)
        {
            List<PaymentRecord> result = new List<PaymentRecord>();
            foreach (PaymentRecord nextRec in allPaymentData)
            {
                if ((nextRec.MeterId == meterID) && (nextRec.BayNumber == bayID))
                    result.Add(nextRec);
            }

            // Make sure this set of data is sorted by date, in ascending order!
            result.Sort(new Duncan.PEMS.SpaceStatus.DataSuppliers.HistoricalPaymentRecordsSorter(true));

            return result;
        }

        protected int ResolveAreaIDForMeterID(int meterID)
        {
            int result = -1;

            if ((cachedAssets != null) && (cachedAssets.enumedMeterAssets != null))
            {
                foreach (MeterAsset asset in cachedAssets.enumedMeterAssets)
                {
                    if (asset.MeterID == meterID)
                    {
                        result = asset.AreaID_PreferLibertyBeforeInternal;
                        break;
                    }
                }
            }
            else
            {
                foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(_CustomerConfig))
                {
                    if (asset.MeterID == meterID)
                    {
                        result = asset.AreaID_PreferLibertyBeforeInternal;
                        break;
                    }
                }
            }
            return result;
        }

        protected string ResolvedAreaNameForAreaID(int areaID)
        {
            string result = string.Empty;

            if ((cachedAssets != null) && (cachedAssets.enumedAreaAssets != null))
            {
                foreach (AreaAsset asset in cachedAssets.enumedAreaAssets)
                {
                    if (asset.AreaID == areaID)
                    {
                        result = asset.AreaName;
                        break;
                    }
                }
            }
            else
            {
                foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(_CustomerConfig))
                {
                    if (asset.AreaID == areaID)
                    {
                        result = asset.AreaName;
                        break;
                    }
                }
            }

            // If we don't have any name, we will just have to use the Area ID as the name instead
            if (string.IsNullOrEmpty(result))
                result = areaID.ToString();

            return result;
        }

        protected string FormatTimeSpan(TimeSpan T)
        {
            string SpanFormat = "H:mm:ss";

            int NoDays = Math.Abs(T.Days);
            int Hours = Math.Abs(T.Hours);
            int Minutes = Math.Abs(T.Minutes);
            int Seconds = Math.Abs(T.Seconds);

            DateTime TAsDateTime = new DateTime().AddDays(NoDays).AddHours(Hours).AddMinutes(Minutes).AddSeconds(Seconds);

            // if more than a whole day, include those in the display
            if (T.Days > 0)
            {
                return T.Days.ToString() + "d " + TAsDateTime.ToString(SpanFormat);
            }
            else
            {
                return TAsDateTime.ToString(SpanFormat);
            }
        }

        protected string FormatTimeSpanAsHoursMinutesAndSeconds(TimeSpan T)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            /*bool isEmpty = true;*/

            int NoDays = Math.Abs(T.Days);
            int Hours = Math.Abs(T.Hours);
            int Minutes = Math.Abs(T.Minutes);
            int Seconds = Math.Abs(T.Seconds);

            // Days doesn't make sense, so we want to convert days to accumulate in the hour instead
            if (NoDays > 0)
                Hours += (NoDays * 24);

            sb.Append(string.Format("{0:D2}:{1:D2}:{2:D2}", Hours, Minutes, Seconds));
            return sb.ToString();

            // This code is if we wanted to display days also
            /*
            bool showDays = false;
            if (showDays == true)
            {
                if (NoDays == 1)
                {
                    if ((Hours == 0) && (Minutes == 0) && (Seconds == 0))
                        sb.Append("24 Hours");
                    else
                        sb.Append(NoDays.ToString() + " day");

                    isEmpty = false;
                }
                else if (NoDays > 1)
                {
                    sb.Append(NoDays.ToString() + " days");
                    isEmpty = false;
                }

                if ((isEmpty == false) && (Hours == 0) && (Minutes == 0) && (Seconds == 0))
                {
                    return sb.ToString();
                }

                if (isEmpty == false)
                {
                    sb.Append(", ");
                }

                sb.Append(string.Format("{0:D2}:{1:D2}:{2:D2}", Hours, Minutes, Seconds));
                return sb.ToString();
            }
            */
        }

        protected void ApplyNumberStyleToColumn(ExcelWorksheet ws, int colIdx, int rowStartIdx, int rowEndIdx, string numberFormat, ExcelHorizontalAlignment horzAlign)
        {
            using (OfficeOpenXml.ExcelRange col = ws.Cells[rowStartIdx, colIdx, rowEndIdx, colIdx])
            {
                col.Style.Numberformat.Format = numberFormat;
                col.Style.HorizontalAlignment = horzAlign;
            }
        }

        protected void ApplyNumberStyleToCell(ExcelWorksheet ws, int Row, int Col, string numberFormat, ExcelHorizontalAlignment horzAlign)
        {
            ApplyNumberStyleToColumn(ws, Col, Row, Row, numberFormat, horzAlign);
        }

        protected bool IsTimestampAllowedForActivityRestrictionInEffect(DateTime timestamp, int meterID, int bayID)
        {
            // Simply return true if we are not supposed to restrict reported content to enforcement/regulated hours
            if (this._ActivityRestriction == ActivityRestrictions.AllActivity)
                return true;

            // Resolve the associated area for the meter
            int areaID = ResolveAreaIDForMeterID(meterID);

            // Try to obtain the regulated hours applicable to this meter
            RegulatedHoursGroup regulatedHours = RegulatedHoursGroupRepository.Repository.GetBestGroupForMeter(this._CustomerConfig.CustomerId, areaID, meterID);

            // If no regulated hour defintions came back, then we will default to assumption that regulated period is 24-hours a day
            if ((regulatedHours == null) || (regulatedHours.Details == null) || (regulatedHours.Details.Count == 0))
                return true;

            // Determine the day of week that is involved
            int dayOfWeek = (int)timestamp.DayOfWeek;

            bool result = false;
            TimeSpan earliestAllowed;
            TimeSpan latestAllowed;
            TimeSpan justTimeFromTimestamp = new TimeSpan(timestamp.Hour, timestamp.Minute, timestamp.Second);

            // Loop through the daily rules and see if the timestamp falls within a Regulated or No Parking timeslot for the appropriate day
            foreach (RegulatedHoursDetail detail in regulatedHours.Details)
            {
                // Skip this one if its for a different day of the week
                if (detail.DayOfWeek != dayOfWeek)
                    continue;

                // We are interested in timeslots that are Regulated (or No Parking), so skip ones that are for "Unregulated"
                if (string.Compare(detail.Type, "Unregulated", true) == 0)
                    continue;

                // Determine if the time of day is within this timeslot
                earliestAllowed = new TimeSpan(detail.StartTime.Hour, detail.StartTime.Minute, 0);
                latestAllowed = new TimeSpan(detail.EndTime.Hour, detail.EndTime.Minute, 59);
                if ((justTimeFromTimestamp >= earliestAllowed) && (justTimeFromTimestamp < latestAllowed))
                {
                    // Timestamp falls within this category, so no need to look further
                    result = true;
                    break;
                }
            }

            // If we are wanting to restrict to unregulated hours, simply swap the boolean result we determined above
            if (this._ActivityRestriction == ActivityRestrictions.OnlyDuringUnregulatedHours)
                result = !result;

            return result;
        }
        #endregion
    }
}