using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using OfficeOpenXml; // Namespace inside the open source EPPlus.dll from http://epplus.codeplex.com/
using OfficeOpenXml.Style;


using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.Models
{
    public class ViolationReportModel
    {
        public List<ViolationReportDetail> Violations = new List<ViolationReportDetail>();
        public ViolationGroupStats OverallGroupStats = new ViolationGroupStats();

        public List<ViolationGroupStats_BayAndMeter> BayStats = new List<ViolationGroupStats_BayAndMeter>();
        public List<ViolationGroupStats_Meter> MeterStats = new List<ViolationGroupStats_Meter>();
        public List<ViolationGroupStats_Area> AreaStats = new List<ViolationGroupStats_Area>();
    }

    public class ViolationReportDetail
    {
        public int MeterID { get; set; }
        public int BayID { get; set; }
        public int AreaID { get; set; }

        public DateTime DateTime_Start_Occupancy { get; set; }
        public DateTime DateTime_End_Occupancy { get; set; }

        public DateTime DateTime_PayExpiration { get; set; }

        public DateTime DateTime_StartOfViolation { get; set; }
        public TimeSpan DurationInVioCondition { get; set; }

        public RegulatedHoursDetail ViolationBasedOnRuleDetail { get; set; }
        public RegulatedHoursGroup ViolationBasedOnRuleGroup { get; set; }

        public ViolationReportDetail()
        {
            this.MeterID = -1;
            this.BayID = -1;
            this.AreaID = -1;
            this.DateTime_Start_Occupancy = DateTime.MinValue;
            this.DateTime_End_Occupancy = DateTime.MinValue;
            this.DateTime_StartOfViolation = DateTime.MinValue;
            this.DurationInVioCondition = new TimeSpan();

            this.ViolationBasedOnRuleDetail = null;
            this.ViolationBasedOnRuleGroup = null;
        }
    }

    public class ViolationGroupStats
    {
        public TimeSpan TotalViolationDuration { get; set; }
        public int ViolationCount { get; set; }

        public ViolationGroupStats()
        {
            InitValues();
        }

        protected void InitValues()
        {
            TotalViolationDuration = new TimeSpan();
            ViolationCount = 0;
        }
    }

    public class ViolationGroupStats_BayAndMeter : ViolationGroupStats
    {
        public int MeterID { get; set; }
        public int BayID { get; set; }

        public ViolationGroupStats_BayAndMeter()
            : base()
        {
        }
    }

    public class ViolationGroupStats_Meter : ViolationGroupStats
    {
        public int MeterID { get; set; }

        public ViolationGroupStats_Meter()
            : base()
        {
        }
    }

    public class ViolationGroupStats_Area : ViolationGroupStats
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }

        public ViolationGroupStats_Area()
            : base()
        {
        }

        private int GetAreaIDForMeterID(int meterID, CustomerConfig customerCfg)
        {
            int result = -1;
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg))
            {
                if (asset.MeterID == meterID)
                {
                    result = asset.AreaID_PreferLibertyBeforeInternal;
                    break;
                }
            }
            return result;
        }
    }

    #region Report statistic sorters
    public sealed class ViolationGroupStats_AreaLogicalComparer : System.Collections.Generic.IComparer<ViolationGroupStats_Area>
    {
        private static readonly System.Collections.Generic.IComparer<ViolationGroupStats_Area> _default = new ViolationGroupStats_AreaLogicalComparer(true);
        private bool _SortByName = false;

        public ViolationGroupStats_AreaLogicalComparer(bool SortByName)
        {
            _SortByName = SortByName;
        }

        public static System.Collections.Generic.IComparer<ViolationGroupStats_Area> Default
        {
            get { return _default; }
        }

        public int Compare(ViolationGroupStats_Area s1, ViolationGroupStats_Area s2)
        {
            // Are we sorting by Name or ID?
            if (this._SortByName == true)
                return string.CompareOrdinal(s1.AreaName, s2.AreaName);
            else
                return s1.AreaID.CompareTo(s2.AreaID);
        }
    }

    public sealed class ViolationGroupStats_BayAndMeterLogicalComparer : System.Collections.Generic.IComparer<ViolationGroupStats_BayAndMeter>
    {
        private static readonly System.Collections.Generic.IComparer<ViolationGroupStats_BayAndMeter> _default = new ViolationGroupStats_BayAndMeterLogicalComparer();

        public ViolationGroupStats_BayAndMeterLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<ViolationGroupStats_BayAndMeter> Default
        {
            get { return _default; }
        }

        public int Compare(ViolationGroupStats_BayAndMeter s1, ViolationGroupStats_BayAndMeter s2)
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

    public sealed class ViolationGroupStats_MeterLogicalComparer : System.Collections.Generic.IComparer<ViolationGroupStats_Meter>
    {
        private static readonly System.Collections.Generic.IComparer<ViolationGroupStats_Meter> _default = new ViolationGroupStats_MeterLogicalComparer();

        public ViolationGroupStats_MeterLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<ViolationGroupStats_Meter> Default
        {
            get { return _default; }
        }

        public int Compare(ViolationGroupStats_Meter s1, ViolationGroupStats_Meter s2)
        {
            int s1_ID = s1.MeterID;
            int s2_ID = s2.MeterID;
            int result = s1_ID.CompareTo(s2_ID);
            return result;
        }
    }
    #endregion

    public class VioReport_VSAndPayInfo
    {
        public bool IsOccupied = false;
        public DateTime Arrival = DateTime.MinValue;
        public DateTime Departure = DateTime.MinValue;
        public DateTime ExpirationTimestamp = DateTime.MinValue;
        public DateTime PaymentTimestamp = DateTime.MinValue;
    }

    public class ViolationReportEngine
    {
        public enum ActivityRestrictions
        {
            AllActivity, OnlyDuringRegulatedHours, OnlyDuringUnregulatedHours
        }

        #region Private/Protected Members
        protected CustomerConfig _CustomerConfig = null;

        protected ViolationReportModel _ViolationReportModel = new ViolationReportModel();
        private ActivityRestrictions _ActivityRestriction = ActivityRestrictions.AllActivity;
        #endregion

        #region Public Methods
        public ViolationReportEngine(CustomerConfig customerCfg)
        {
            _CustomerConfig = customerCfg;
        }

        public void GetReportAsExcelSpreadsheet(List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime, MemoryStream ms,
            bool includeAreaSummary, bool includeMeterSummary, bool includeSpaceSummary,
            string scopedAreaName, string scopedMeter, ActivityRestrictions _activityRestriction)
        {
            bool includeDetails = true;
            this._ActivityRestriction = _activityRestriction;

            // Start diagnostics timer
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

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
                ws.Cells[rowIdx, 1].Value = "Violation Statistics Report";
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
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Meter Violation'!A1\", \"Click here for Meter Violation summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeSpaceSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Space Violation'!A1\", \"Click here for Space Violation summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeAreaSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Area Violation'!A1\", \"Click here for Area Violation summary\")";
                    //ws.Cells[rowIdx, 1].Hyperlink = new ExcelHyperLink("#'Area Violation'!A1", "Click here to jump to Area Violation summary"); 
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeDetails == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Details'!A1\", \"Click here for Violation details\")";
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
                ws.Cells[rowIdx, 1].Value = "Violation Count";
                ws.Cells[rowIdx, 2].Value = "Violation Duration";

                // Format the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, 2])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Increment the row index, which will now be the 1st row of our data
                rowIdx++;
                // We only have one row of data for Overall summary, so output it now
                ws.Cells[rowIdx, 1].Value = this._ViolationReportModel.OverallGroupStats.ViolationCount;
                ws.Cells[rowIdx, 2].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ViolationReportModel.OverallGroupStats.TotalViolationDuration);

                // Column 1 is numeric integer
                ApplyNumberStyleToColumn(ws, 1, rowIdx, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                // Column 2 should be aligned right
                ApplyNumberStyleToColumn(ws, 2, rowIdx, rowIdx, "@", ExcelHorizontalAlignment.Right);

                // And now lets size the columns
                for (int autoSizeColIdx = 1; autoSizeColIdx <= 2; autoSizeColIdx++)
                {
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[overallSummaryHeaderRowIdx, autoSizeColIdx, rowIdx, autoSizeColIdx])
                    {
                        col.AutoFitColumns();
                    }
                }


                //  --- END OF OVERALL SUMMARY WORKSHEET ---

                // Should we include a worksheet with Meter aggregates?
                if (includeMeterSummary == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Meter Violation");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Meter #";
                    ws.Cells[rowIdx, 2].Value = "Violation Count";
                    ws.Cells[rowIdx, 3].Value = "Violation Duration";

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 3])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (ViolationGroupStats_Meter meterStat in this._ViolationReportModel.MeterStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = meterStat.MeterID;
                        ws.Cells[rowIdx, 2].Value = meterStat.ViolationCount;
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.TotalViolationDuration);

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 3])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Meter ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 2 is numeric integer
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 3 is text, but right-justified
                    ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "@", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 3; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                        }
                    }
                }



                // Should we include a worksheet with Space aggregates?
                if (includeSpaceSummary == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Space Violation");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Space #";
                    ws.Cells[rowIdx, 2].Value = "Meter #";
                    ws.Cells[rowIdx, 3].Value = "Violation Count";
                    ws.Cells[rowIdx, 4].Value = "Violation Duration";

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 4])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (ViolationGroupStats_BayAndMeter bayStat in this._ViolationReportModel.BayStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = bayStat.BayID;
                        ws.Cells[rowIdx, 2].Value = bayStat.MeterID;
                        ws.Cells[rowIdx, 3].Value = bayStat.ViolationCount;
                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.TotalViolationDuration);

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 4])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Bay ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 2 is numeric integer (Meter ID)
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 3 is numeric integer
                    ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 4 is text, but right-justified
                    ApplyNumberStyleToColumn(ws, 4, 2, rowIdx, "@", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 4; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                        }
                    }
                }


                // Should we include a worksheet with Area aggregates?
                if (includeAreaSummary == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Area Violation");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Area";
                    ws.Cells[rowIdx, 2].Value = "Violation Count";
                    ws.Cells[rowIdx, 3].Value = "Violation Duration";

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 3])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (ViolationGroupStats_Area areaStat in this._ViolationReportModel.AreaStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = areaStat.AreaName;
                        ws.Cells[rowIdx, 2].Value = areaStat.ViolationCount;
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalViolationDuration);

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 3])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 2 is numeric integer
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 3 is text, but right-justified
                    ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "@", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 3; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                        }
                    }
                }

                // Should we include a Details worksheet?
                if (includeDetails == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Details");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Space #";
                    ws.Cells[rowIdx, 2].Value = "Meter #";
                    ws.Cells[rowIdx, 3].Value = "Area";
                    ws.Cells[rowIdx, 4].Value = "Arrival";
                    ws.Cells[rowIdx, 5].Value = "Departure";
                    ws.Cells[rowIdx, 6].Value = "Expiration";
                    ws.Cells[rowIdx, 7].Value = "Start of Violation";
                    ws.Cells[rowIdx, 8].Value = "Duration of Violation";
                    ws.Cells[rowIdx, 9].Value = "Regulations";

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 9])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (ViolationReportDetail Violation in this._ViolationReportModel.Violations)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = Violation.BayID;
                        ws.Cells[rowIdx, 2].Value = Violation.MeterID;
                        ws.Cells[rowIdx, 3].Value = ResolvedAreaNameForAreaID(Violation.AreaID);
                        ws.Cells[rowIdx, 4].Value = Violation.DateTime_Start_Occupancy.ToShortDateString() + "  " + Violation.DateTime_Start_Occupancy.ToShortTimeString();
                        ws.Cells[rowIdx, 5].Value = Violation.DateTime_End_Occupancy.ToShortDateString() + "  " + Violation.DateTime_End_Occupancy.ToShortTimeString();
                        ws.Cells[rowIdx, 6].Value = Violation.DateTime_PayExpiration.ToShortDateString() + "  " + Violation.DateTime_PayExpiration.ToShortTimeString();
                        ws.Cells[rowIdx, 7].Value = Violation.DateTime_StartOfViolation.ToShortDateString() + "  " + Violation.DateTime_StartOfViolation.ToShortTimeString();
                        ws.Cells[rowIdx, 8].Value = FormatTimeSpanAsHoursMinutesAndSeconds(Violation.DurationInVioCondition);

                        StringBuilder sb = new StringBuilder();
                        if (Violation.ViolationBasedOnRuleDetail != null)
                        {
                            sb.Append(Enum.ToObject(typeof(DayOfWeek), Violation.ViolationBasedOnRuleDetail.DayOfWeek).ToString() + " ");
                            sb.Append(Violation.ViolationBasedOnRuleDetail.StartTime.ToString("hh:mm:ss tt") + " - " +
                                Violation.ViolationBasedOnRuleDetail.EndTime.ToString("hh:mm:ss tt") + ", ");
                            sb.Append(Violation.ViolationBasedOnRuleDetail.Type + ", Max Stay: " + Violation.ViolationBasedOnRuleDetail.MaxStayMinutes.ToString());
                            if (Violation.ViolationBasedOnRuleGroup != null)
                            {
                                if ((Violation.ViolationBasedOnRuleGroup.MID.HasValue) && (Violation.ViolationBasedOnRuleGroup.MID.Value > -1))
                                    sb.Append(" (Meter-level regulations)");
                                else if ((Violation.ViolationBasedOnRuleGroup.AID.HasValue) && (Violation.ViolationBasedOnRuleGroup.AID.Value > -1))
                                    sb.Append(" (Area-level regulations)");
                                else if ((Violation.ViolationBasedOnRuleGroup.CID.HasValue) && (Violation.ViolationBasedOnRuleGroup.CID.Value > -1))
                                    sb.Append(" (Site-wide regulations)");
                            }
                        }
                        ws.Cells[rowIdx, 9].Value = sb.ToString();

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 9])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 & 2 are numeric integer
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    ApplyNumberStyleToColumn(ws, 4, 2, rowIdx, "mm/dd/yyyy hh:mm:ss tt", ExcelHorizontalAlignment.Right);
                    ApplyNumberStyleToColumn(ws, 5, 2, rowIdx, "mm/dd/yyyy hh:mm:ss tt", ExcelHorizontalAlignment.Right);
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "mm/dd/yyyy hh:mm:ss tt", ExcelHorizontalAlignment.Right);
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "mm/dd/yyyy hh:mm:ss tt", ExcelHorizontalAlignment.Right);

                    ApplyNumberStyleToColumn(ws, 8, 2, rowIdx, "@", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 9; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                        }
                    }
                }

                // All cells in spreadsheet are populated now, so render (save the file) to a memory stream 
                byte[] bytes = pck.GetAsByteArray();
                ms.Write(bytes, 0, bytes.Length);
            }


            // Stop diagnostics timer
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Violation Report Generation took: " + sw.Elapsed.ToString());
        }
        #endregion

        #region Private/Protected Methods
        protected void GatherReportData(List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime)
        {
            // Adjust the date ranges as needed for our SQL queries.
            // The end time needs to be inclusive of the entire minute (seconds and milliseconds are not in the resolution of the passed EndTime parameter)
            DateTime AdjustedStartTime;
            DateTime AdjustedEndTime;
            AdjustedStartTime = StartTime;
            AdjustedEndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTime.Hour, EndTime.Minute, 0).AddMinutes(1);

            // Gather all applicable vehicle sensing data (minimizes how many individual SQL queries must be executed)
            List<HistoricalSensingRecord> RawSensingDataForAllMeters = new SensingDatabaseSource(_CustomerConfig).GetHistoricalVehicleSensingDataForMeters_StronglyTyped(
                this._CustomerConfig.CustomerId, listOfMeterIDs, AdjustedStartTime, AdjustedEndTime, true);

            // Gather all applicable payment data (minimizes how many individual SQL queries must be exectured)
            List<PaymentRecord> RawPaymentDataForAllMeters = new PaymentDatabaseSource(this._CustomerConfig).GetHistoricalPaymentDataForMeters_StronglyTyped(
                this._CustomerConfig.CustomerId, listOfMeterIDs, AdjustedStartTime, AdjustedEndTime, true);

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
                ViolationGroupStats_Area AreaStatsObj = null;
                foreach (ViolationGroupStats_Area existingArea in this._ViolationReportModel.AreaStats)
                {
                    if (existingArea.AreaID == nextAreaID)
                    {
                        AreaStatsObj = existingArea;
                        break;
                    }
                }
                if (AreaStatsObj == null)
                {
                    AreaStatsObj = new ViolationGroupStats_Area();
                    AreaStatsObj.AreaID = nextAreaID;
                    AreaStatsObj.AreaName = ResolvedAreaNameForAreaID(nextAreaID);
                    this._ViolationReportModel.AreaStats.Add(AreaStatsObj);
                }

                // Now aggregate info for this area
                foreach (ViolationReportDetail ViolationVio in _ViolationReportModel.Violations)
                {
                    // Skip next Violation vio record if its not for the desired area
                    if (ViolationVio.AreaID != nextAreaID)
                        continue;

                    AreaStatsObj.ViolationCount++;
                    AreaStatsObj.TotalViolationDuration = AreaStatsObj.TotalViolationDuration.Add(ViolationVio.DurationInVioCondition);
                }
            }


            // We will also do an overall stats 
            foreach (ViolationReportDetail ViolationVio in _ViolationReportModel.Violations)
            {
                _ViolationReportModel.OverallGroupStats.ViolationCount++;
                _ViolationReportModel.OverallGroupStats.TotalViolationDuration = _ViolationReportModel.OverallGroupStats.TotalViolationDuration.Add(ViolationVio.DurationInVioCondition);
            }

            // Sort this.ReportDataModel.AreaStats by AreaName so it renders in Excel in a nice sort order
            this._ViolationReportModel.AreaStats.Sort(new ViolationGroupStats_AreaLogicalComparer(true));

            // Sort this.ReportDataModel.MeterStats by MeterID so it renders in Excel in a nice sort order
            this._ViolationReportModel.MeterStats.Sort(new ViolationGroupStats_MeterLogicalComparer());

            // Sort this.ReportDataModel.BayStats by BayID so it renders in Excel in a nice sort order
            this._ViolationReportModel.BayStats.Sort(new ViolationGroupStats_BayAndMeterLogicalComparer());
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
            ViolationGroupStats_Meter MeterStatsObj = new ViolationGroupStats_Meter();
            MeterStatsObj.MeterID = meterId;
            this._ViolationReportModel.MeterStats.Add(MeterStatsObj);

            // Now aggregate info for this meter
            foreach (ViolationReportDetail ViolationVio in _ViolationReportModel.Violations)
            {
                // Skip next Violation vio record if its not for the desired meter
                if (ViolationVio.MeterID != meterId)
                    continue;

                MeterStatsObj.ViolationCount++;
                MeterStatsObj.TotalViolationDuration = MeterStatsObj.TotalViolationDuration.Add(ViolationVio.DurationInVioCondition);
            }
        }

        protected void AnalyzeDataForSpace(List<HistoricalSensingRecord> filteredSensingData, List<PaymentRecord> filteredPaymentData, int legacyBitNumber,
            int meterId, int bayNumber, DateTime startTime, DateTime endTime_NotInclusive)
        {

            // Need to know current time in customer's timezone
            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

            // Create a bay statistics object for the current bay (and meter), and add to our report list
            ViolationGroupStats_BayAndMeter BayStatsObj = new ViolationGroupStats_BayAndMeter();
            BayStatsObj.BayID = bayNumber;
            BayStatsObj.MeterID = meterId;
            this._ViolationReportModel.BayStats.Add(BayStatsObj);

            // Start Analyzing data here 
            List<HistoricalSensingRecord> rawSensingData = filteredSensingData;
            List<PaymentRecord> PaymentsFilteredForBay = filteredPaymentData;

            int SensingCursor = 0;
            bool SensingCursorAtEnd = (rawSensingData.Count == 0);

            int PaymentCursor = 0;
            bool PaymentCursorAtEnd = (PaymentsFilteredForBay.Count == 0);

            VioReport_VSAndPayInfo currVSAndPayInfo = null; 

            object NextReportObj = null;
            DateTime NextReportObjTimeStamp = new DateTime();

            // Loop until no more data (or forced break out of loop)
            while (!SensingCursorAtEnd || !PaymentCursorAtEnd)
            {
                // Create a current state object if necessary
                if (currVSAndPayInfo == null)
                    currVSAndPayInfo = new VioReport_VSAndPayInfo();

                // End-of-data indicators need to be updated now to reflect current reality
                SensingCursorAtEnd = (SensingCursor == rawSensingData.Count);
                PaymentCursorAtEnd = (PaymentCursor == PaymentsFilteredForBay.Count);

                // Find the next reportable object (payment or sensing)
                NextReportObj = null;

                if (!SensingCursorAtEnd)
                {
                    if ((rawSensingData[SensingCursor].DateTime >= NextReportObjTimeStamp) || (NextReportObjTimeStamp == DateTime.MinValue))
                    {
                        NextReportObj = rawSensingData[SensingCursor];
                        NextReportObjTimeStamp = rawSensingData[SensingCursor].DateTime;
                    }
                }

                if (!PaymentCursorAtEnd)
                {
                    if ((PaymentsFilteredForBay[PaymentCursor].TransactionDateTime >= NextReportObjTimeStamp) || (NextReportObjTimeStamp == DateTime.MinValue))
                    {
                        NextReportObj = PaymentsFilteredForBay[PaymentCursor];
                        NextReportObjTimeStamp = PaymentsFilteredForBay[PaymentCursor].TransactionDateTime;
                    }
                }

                if ((!SensingCursorAtEnd) && (!PaymentCursorAtEnd))
                {
                    if (rawSensingData[SensingCursor].DateTime <= PaymentsFilteredForBay[PaymentCursor].TransactionDateTime)
                    {
                        NextReportObj = rawSensingData[SensingCursor];
                        NextReportObjTimeStamp = rawSensingData[SensingCursor].DateTime;
                    }
                    else if (PaymentsFilteredForBay[PaymentCursor].TransactionDateTime <= rawSensingData[SensingCursor].DateTime)
                    {
                        NextReportObj = PaymentsFilteredForBay[PaymentCursor];
                        NextReportObjTimeStamp = PaymentsFilteredForBay[PaymentCursor].TransactionDateTime;
                    }
                }

                if (NextReportObj is HistoricalSensingRecord)
                {
                    HistoricalSensingRecord nextSensingRec = (HistoricalSensingRecord)(NextReportObj);

                    // Does the raw data indicate a vehicle arrival event?
                    if (nextSensingRec.IsOccupied == true)
                    {
                        // Only update current arrival time if the occupancy state is actually changing
                        if ((currVSAndPayInfo.IsOccupied != nextSensingRec.IsOccupied) || (currVSAndPayInfo.Arrival == DateTime.MinValue))
                            currVSAndPayInfo.Arrival = nextSensingRec.DateTime;

                        // Always update current occupancy state after the arrival time has been evaluated
                        currVSAndPayInfo.IsOccupied = nextSensingRec.IsOccupied;
                       
                        // Note that when a vehicle arrival is detected, it might be the start of a violation condition, but we won't count it as 
                        // such yet, because we don't know the duration of the violation period yet! (Need to wait for earliest of either the next vacancy, 
                        // the next payment, the end of the reporting period, the current time in customer's timezone, or reach the end of data to process)
                    }
                    else
                    {
                        // The current occupancy state is vacant, which might mean its the end of a violation condition

                        // Only update current departure time if the occupancy state is actually changing
                        if ((currVSAndPayInfo.IsOccupied != nextSensingRec.IsOccupied) || (currVSAndPayInfo.Departure == DateTime.MinValue))
                        {
                            currVSAndPayInfo.Departure = nextSensingRec.DateTime;

                            // Find the later of the expiration or the arrival event
                            DateTime LaterOfExpirationOrVehArrival = new DateTime(Math.Max(currVSAndPayInfo.Arrival.Ticks, currVSAndPayInfo.ExpirationTimestamp.Ticks));
                            if ((LaterOfExpirationOrVehArrival > DateTime.MinValue) && (currVSAndPayInfo.Departure > LaterOfExpirationOrVehArrival) &&
                                (currVSAndPayInfo.ExpirationTimestamp > DateTime.MinValue))
                            {
                                // Its a violation if the vehicle departure occurred after the later of vehicle arrival or payment expiration,
                                // and the space is expired
                                if ((LaterOfExpirationOrVehArrival.Ticks == currVSAndPayInfo.ExpirationTimestamp.Ticks) ||
                                     (currVSAndPayInfo.ExpirationTimestamp.Ticks < currVSAndPayInfo.Arrival.Ticks))
                                {
                                    ViolationReportDetail vioToAdd = new ViolationReportDetail();
                                    vioToAdd.AreaID = ResolveAreaIDForMeterID(meterId);
                                    vioToAdd.BayID = bayNumber;
                                    vioToAdd.MeterID = meterId;
                                    vioToAdd.DateTime_End_Occupancy = currVSAndPayInfo.Departure;
                                    vioToAdd.DateTime_Start_Occupancy = currVSAndPayInfo.Arrival;
                                    vioToAdd.DateTime_PayExpiration = currVSAndPayInfo.ExpirationTimestamp;
                                    // The start of the violation is the later of when the payment expired, or when the vehicle arrived
                                    vioToAdd.DateTime_StartOfViolation = new DateTime(LaterOfExpirationOrVehArrival.Ticks);
                                    vioToAdd.DurationInVioCondition = new TimeSpan(currVSAndPayInfo.Departure.Ticks - LaterOfExpirationOrVehArrival.Ticks);
                                    
                                    // Add the violation to the report's list, but only if it occurred during regulated hours
                                    if (IsTimestampAllowedForActivityRestrictionInEffect(vioToAdd, meterId, bayNumber) == true)
                                        this._ViolationReportModel.Violations.Add(vioToAdd);
                                }
                            }
                        }

                        // Always update current occupancy state after the departure time has been evaluated
                        currVSAndPayInfo.IsOccupied = nextSensingRec.IsOccupied;
                    }

                    // This sensing record has been used, so increment the sensing cursor
                    SensingCursor++;
                }
                else if (NextReportObj is PaymentRecord)
                {
                    PaymentRecord nextPaymentRec = (NextReportObj as PaymentRecord);

                    // If the space is occupied, we might be at the end of the previous violation condition
                    if (currVSAndPayInfo.IsOccupied == true)
                    {
                        DateTime LaterOfExpirationOrVehArrival = new DateTime(Math.Max(currVSAndPayInfo.Arrival.Ticks, currVSAndPayInfo.ExpirationTimestamp.Ticks));
                        // Find the later of the expiration or the arrival event
                        if ((LaterOfExpirationOrVehArrival > DateTime.MinValue) && (nextPaymentRec.TransactionDateTime > LaterOfExpirationOrVehArrival))
                        {
                            // Its a violation if the vehicle departure occurred after the later of vehicle arrival or payment expiration,
                            // and the space is expired
                            if ( ((LaterOfExpirationOrVehArrival.Ticks == currVSAndPayInfo.ExpirationTimestamp.Ticks) ||
                                 (currVSAndPayInfo.ExpirationTimestamp.Ticks < currVSAndPayInfo.Arrival.Ticks)) &&
                                 (currVSAndPayInfo.ExpirationTimestamp > DateTime.MinValue) )
                            {
                                ViolationReportDetail vioToAdd = new ViolationReportDetail();
                                vioToAdd.AreaID = ResolveAreaIDForMeterID(meterId);
                                vioToAdd.BayID = bayNumber;
                                vioToAdd.MeterID = meterId;
                                vioToAdd.DateTime_End_Occupancy = currVSAndPayInfo.Departure;
                                vioToAdd.DateTime_Start_Occupancy = currVSAndPayInfo.Arrival;
                                vioToAdd.DateTime_PayExpiration = currVSAndPayInfo.ExpirationTimestamp;
                                // The start of the violation is the later of when the payment expired, or when the vehicle arrived
                                vioToAdd.DateTime_StartOfViolation = new DateTime(LaterOfExpirationOrVehArrival.Ticks);
                                vioToAdd.DurationInVioCondition = new TimeSpan(nextPaymentRec.TransactionDateTime.Ticks - LaterOfExpirationOrVehArrival.Ticks);

                                // Add the violation to the report's list, but only if it occurred during regulated hours
                                if (IsTimestampAllowedForActivityRestrictionInEffect(vioToAdd, meterId, bayNumber) == true)
                                    this._ViolationReportModel.Violations.Add(vioToAdd);
                            }
                        }
                    }

                    // Update the payment timestamp
                    currVSAndPayInfo.PaymentTimestamp = nextPaymentRec.TransactionDateTime;

                    // Update the payment expiration
                    if (currVSAndPayInfo.ExpirationTimestamp >= NextReportObjTimeStamp)
                        currVSAndPayInfo.ExpirationTimestamp = currVSAndPayInfo.ExpirationTimestamp.Add(nextPaymentRec.TimePaid);
                    else
                        currVSAndPayInfo.ExpirationTimestamp = nextPaymentRec.TransactionDateTime.Add(nextPaymentRec.TimePaid);

                    // This payment record has been used, so increment the payment cursor
                    PaymentCursor++;
                }


                // Update end-of-data indicator for the payment and sensing data cursors
                SensingCursorAtEnd = (SensingCursor == rawSensingData.Count);
                PaymentCursorAtEnd = (PaymentCursor == PaymentsFilteredForBay.Count);
            }


            // Reset the data counters to process the next day
            SensingCursorAtEnd = (rawSensingData.Count == 0);
            PaymentCursorAtEnd = (PaymentsFilteredForBay.Count == 0);

            // Now aggregate info for this bay
            foreach (ViolationReportDetail violation in _ViolationReportModel.Violations)
            {
                // Skip next overstay vio record if its not for the desired bay
                if ((violation.BayID != bayNumber) || (violation.MeterID != meterId))
                    continue;

                BayStatsObj.ViolationCount++;
                BayStatsObj.TotalViolationDuration = BayStatsObj.TotalViolationDuration.Add(violation.DurationInVioCondition);
            }
        }

        protected List<HistoricalSensingRecord> GetSensingDataSubsetForMeterIDAndBayID(List<HistoricalSensingRecord> allSensingData, int meterID, int bayID)
        {
            List<HistoricalSensingRecord> result = new List<HistoricalSensingRecord>();
            foreach (HistoricalSensingRecord nextRec in allSensingData)
            {
                if ((nextRec.MeterMappingId == meterID) && (nextRec.BayId == bayID))
                    result.Add(nextRec);
            }
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
            return result;
        }

        protected int ResolveAreaIDForMeterID(int meterID)
        {
            int result = -1;
            foreach (MeterAsset asset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(_CustomerConfig))
            {
                if (asset.MeterID == meterID)
                {
                    result = asset.AreaID_PreferLibertyBeforeInternal;
                    break;
                }
            }
            return result;
        }

        protected string ResolvedAreaNameForAreaID(int areaID)
        {
            string result = string.Empty;
            foreach (AreaAsset asset in CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(_CustomerConfig))
            {
                if (asset.AreaID == areaID)
                {
                    result = asset.AreaName;
                    break;
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

            int NoDays = Math.Abs(T.Days);
            int Hours = Math.Abs(T.Hours);
            int Minutes = Math.Abs(T.Minutes);
            int Seconds = Math.Abs(T.Seconds);

            // Days doesn't make sense, so we want to convert days to accumulate in the hour instead
            if (NoDays > 0)
                Hours += (NoDays * 24);

            sb.Append(string.Format("{0:D2}:{1:D2}:{2:D2}", Hours, Minutes, Seconds));
            return sb.ToString();
        }

        protected void ApplyNumberStyleToColumn(ExcelWorksheet ws, int colIdx, int rowStartIdx, int rowEndIdx, string numberFormat, ExcelHorizontalAlignment horzAlign)
        {
            using (OfficeOpenXml.ExcelRange col = ws.Cells[rowStartIdx, colIdx, rowEndIdx, colIdx])
            {
                col.Style.Numberformat.Format = numberFormat;
                col.Style.HorizontalAlignment = horzAlign;
            }
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

        protected bool IsTimestampAllowedForActivityRestrictionInEffect(ViolationReportDetail vioToAdd, int meterID, int bayID)
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
            int dayOfWeek = (int)vioToAdd.DateTime_StartOfViolation.DayOfWeek;

            bool result = false;
            TimeSpan earliestAllowed;
            TimeSpan latestAllowed;
            TimeSpan justTimeFromTimestamp = new TimeSpan(vioToAdd.DateTime_StartOfViolation.Hour, vioToAdd.DateTime_StartOfViolation.Minute, vioToAdd.DateTime_StartOfViolation.Second);

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
                    vioToAdd.ViolationBasedOnRuleDetail = detail;
                    vioToAdd.ViolationBasedOnRuleGroup = regulatedHours;
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