﻿using System;
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
    public class OverstayReportModel
    {
        public List<VSEventAndDuration> OverstayViolations = new List<VSEventAndDuration>();
        public OverstayGroupStats OverallGroupStats = new OverstayGroupStats();

        public List<OverstayGroupStats_BayAndMeter> BayStats = new List<OverstayGroupStats_BayAndMeter>();
        public List<OverstayGroupStats_Meter> MeterStats = new List<OverstayGroupStats_Meter>();
        public List<OverstayGroupStats_Area> AreaStats = new List<OverstayGroupStats_Area>();
    }

    public class OverstayGroupStats
    {
        public TimeSpan TotalOverstayDuration { get; set; }
        public int OverstayCount { get; set; }
        
        public OverstayGroupStats()
        {
            InitValues();
        }

        protected void InitValues()
        {
            TotalOverstayDuration = new TimeSpan();
            OverstayCount = 0;
        }
    }

    public class OverstayGroupStats_BayAndMeter : OverstayGroupStats
    {
        public int MeterID { get; set; }
        public int BayID { get; set; }

        public OverstayGroupStats_BayAndMeter()
            : base()
        {
        }
    }

    public class OverstayGroupStats_Meter : OverstayGroupStats
    {
        public int MeterID { get; set; }

        public OverstayGroupStats_Meter()
            : base()
        {
        }
    }

    public class OverstayGroupStats_Area : OverstayGroupStats
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; }

        public OverstayGroupStats_Area()
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
    public sealed class OverstayGroupStats_AreaLogicalComparer : System.Collections.Generic.IComparer<OverstayGroupStats_Area>
    {
        private static readonly System.Collections.Generic.IComparer<OverstayGroupStats_Area> _default = new OverstayGroupStats_AreaLogicalComparer(true);
        private bool _SortByName = false;

        public OverstayGroupStats_AreaLogicalComparer(bool SortByName)
        {
            _SortByName = SortByName;
        }

        public static System.Collections.Generic.IComparer<OverstayGroupStats_Area> Default
        {
            get { return _default; }
        }

        public int Compare(OverstayGroupStats_Area s1, OverstayGroupStats_Area s2)
        {
            // Are we sorting by Name or ID?
            if (this._SortByName == true)
                return string.CompareOrdinal(s1.AreaName, s2.AreaName);
            else
                return s1.AreaID.CompareTo(s2.AreaID);
        }
    }

    public sealed class OverstayGroupStats_BayAndMeterLogicalComparer : System.Collections.Generic.IComparer<OverstayGroupStats_BayAndMeter>
    {
        private static readonly System.Collections.Generic.IComparer<OverstayGroupStats_BayAndMeter> _default = new OverstayGroupStats_BayAndMeterLogicalComparer();

        public OverstayGroupStats_BayAndMeterLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<OverstayGroupStats_BayAndMeter> Default
        {
            get { return _default; }
        }

        public int Compare(OverstayGroupStats_BayAndMeter s1, OverstayGroupStats_BayAndMeter s2)
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

    public sealed class OverstayGroupStats_MeterLogicalComparer : System.Collections.Generic.IComparer<OverstayGroupStats_Meter>
    {
        private static readonly System.Collections.Generic.IComparer<OverstayGroupStats_Meter> _default = new OverstayGroupStats_MeterLogicalComparer();

        public OverstayGroupStats_MeterLogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<OverstayGroupStats_Meter> Default
        {
            get { return _default; }
        }

        public int Compare(OverstayGroupStats_Meter s1, OverstayGroupStats_Meter s2)
        {
            int s1_ID = s1.MeterID;
            int s2_ID = s2.MeterID;
            int result = s1_ID.CompareTo(s2_ID);
            return result;
        }
    }
    #endregion


    public class VSEventAndDuration
    {
        public int MeterID { get; set; }
        public int BayID { get; set; }
        public int AreaID { get; set; }
        public DateTime DateTime_Start { get; set; }
        public DateTime DateTime_End { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsOccupied;

        public DateTime DateTime_StartOfOverstayViolation { get; set; }
        public TimeSpan DurationOfTimeBeyondStayLimits { get; set; }

        public RegulatedHoursDetail OverstayBasedOnRuleDetail { get; set; }
        public RegulatedHoursGroup OverstayBasedOnRuleGroup { get; set; }

        public VSEventAndDuration()
        {
            this.MeterID = -1;
            this.BayID = -1;
            this.AreaID = -1;
            this.DateTime_Start = DateTime.MinValue;
            this.DateTime_End = DateTime.MinValue;
            this.Duration = new TimeSpan();
            this.IsOccupied = false;

            this.DateTime_StartOfOverstayViolation = DateTime.MinValue;
            this.DurationOfTimeBeyondStayLimits = new TimeSpan();

            this.OverstayBasedOnRuleDetail = null;
            this.OverstayBasedOnRuleGroup = null;
        }
    }


    public class OverstayReportEngine
    {

        #region Private/Protected Members
        protected CustomerConfig _CustomerConfig = null;
        protected List<SpaceAsset> _cachedSpaceAssets = null;

        protected SensingAndPaymentStatistic_ReportData _ReportDataModel = new SensingAndPaymentStatistic_ReportData();

        protected OverstayReportModel _OverstayReportModel = new OverstayReportModel();
        #endregion

        #region Public Methods
        public OverstayReportEngine(CustomerConfig customerCfg)
        {
            _CustomerConfig = customerCfg;
            _cachedSpaceAssets = CustomerLogic.CustomerManager.GetSpaceAssetsForCustomer(_CustomerConfig);
        }

        public void GetReportAsExcelSpreadsheet(List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime, MemoryStream ms,
            bool includeAreaSummary, bool includeMeterSummary, bool includeSpaceSummary,
            string scopedAreaName, string scopedMeter)
        {
            bool includeDetails = true;

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
                ws.Cells[rowIdx, 1].Value = "Overstay Violation Statistics Report";
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
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Meter Overstay'!A1\", \"Click here for Meter Overstay summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeSpaceSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Space Overstay'!A1\", \"Click here for Space Overstay summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeAreaSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Area Overstay'!A1\", \"Click here for Area Overstay summary\")";
                    //ws.Cells[rowIdx, 1].Hyperlink = new ExcelHyperLink("#'Area Overstay'!A1", "Click here to jump to Area Overstay summary"); 
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeDetails == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Details'!A1\", \"Click here for Overstay details\")";
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
                ws.Cells[rowIdx, 1].Value = "Overstay Violation Count";
                ws.Cells[rowIdx, 2].Value = "Overstay Duration";

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
                ws.Cells[rowIdx, 1].Value = this._OverstayReportModel.OverallGroupStats.OverstayCount;
                ws.Cells[rowIdx, 2].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._OverstayReportModel.OverallGroupStats.TotalOverstayDuration);

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
                    ws = pck.Workbook.Worksheets.Add("Meter Overstay");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Meter #";
                    ws.Cells[rowIdx, 2].Value = "Overstay Violation Count";
                    ws.Cells[rowIdx, 3].Value = "Overstay Duration";

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

                    foreach (OverstayGroupStats_Meter meterStat in this._OverstayReportModel.MeterStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = meterStat.MeterID;
                        ws.Cells[rowIdx, 2].Value = meterStat.OverstayCount;
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.TotalOverstayDuration);

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
                    ws = pck.Workbook.Worksheets.Add("Space Overstay");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Space #";
                    ws.Cells[rowIdx, 2].Value = "Meter #";
                    ws.Cells[rowIdx, 3].Value = "Overstay Violation Count";
                    ws.Cells[rowIdx, 4].Value = "Overstay Duration";

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

                    foreach (OverstayGroupStats_BayAndMeter bayStat in this._OverstayReportModel.BayStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = bayStat.BayID;
                        ws.Cells[rowIdx, 2].Value = bayStat.MeterID;
                        ws.Cells[rowIdx, 3].Value = bayStat.OverstayCount;
                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.TotalOverstayDuration);

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
                    ws = pck.Workbook.Worksheets.Add("Area Overstay");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Area";
                    ws.Cells[rowIdx, 2].Value = "Overstay Violation Count";
                    ws.Cells[rowIdx, 3].Value = "Overstay Duration";

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

                    foreach (OverstayGroupStats_Area areaStat in this._OverstayReportModel.AreaStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = areaStat.AreaName;
                        ws.Cells[rowIdx, 2].Value = areaStat.OverstayCount;
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalOverstayDuration);

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
                    ws.Cells[rowIdx, 6].Value = "Duration of Overstay";
                    ws.Cells[rowIdx, 7].Value = "Overstay Rule";

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 7])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (VSEventAndDuration overstay in this._OverstayReportModel.OverstayViolations)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = overstay.BayID;
                        ws.Cells[rowIdx, 2].Value = overstay.MeterID;
                        ws.Cells[rowIdx, 3].Value = ResolvedAreaNameForAreaID(overstay.AreaID);
                        ws.Cells[rowIdx, 4].Value = overstay.DateTime_Start.ToShortDateString() + "  " + overstay.DateTime_Start.ToShortTimeString();
                        ws.Cells[rowIdx, 5].Value = overstay.DateTime_End.ToShortDateString() + "  " + overstay.DateTime_End.ToShortTimeString();
                        ws.Cells[rowIdx, 6].Value = FormatTimeSpanAsHoursMinutesAndSeconds(overstay.DurationOfTimeBeyondStayLimits);

                        StringBuilder sb = new StringBuilder();
                        if (overstay.OverstayBasedOnRuleDetail != null)
                        {
                            sb.Append(Enum.ToObject(typeof(DayOfWeek), overstay.OverstayBasedOnRuleDetail.DayOfWeek).ToString() + " ");
                            sb.Append(overstay.OverstayBasedOnRuleDetail.StartTime.ToString("hh:mm:ss tt") + " - " +
                                overstay.OverstayBasedOnRuleDetail.EndTime.ToString("hh:mm:ss tt") + ", ");
                            sb.Append(overstay.OverstayBasedOnRuleDetail.Type + ", Max Stay: " + overstay.OverstayBasedOnRuleDetail.MaxStayMinutes.ToString());
                            if (overstay.OverstayBasedOnRuleGroup != null)
                            {
                                if ((overstay.OverstayBasedOnRuleGroup.MID.HasValue) && (overstay.OverstayBasedOnRuleGroup.MID.Value > -1))
                                    sb.Append(" (Meter-level regulations)");
                                else if ((overstay.OverstayBasedOnRuleGroup.AID.HasValue) && (overstay.OverstayBasedOnRuleGroup.AID.Value > -1))
                                    sb.Append(" (Area-level regulations)");
                                else if ((overstay.OverstayBasedOnRuleGroup.CID.HasValue) && (overstay.OverstayBasedOnRuleGroup.CID.Value > -1))
                                    sb.Append(" (Site-wide regulations)");
                            }
                        }
                        ws.Cells[rowIdx, 7].Value = sb.ToString();

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 7])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 & 2 are numeric integer
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    ApplyNumberStyleToColumn(ws, 4, 2, rowIdx, "mm/dd/yyyy hh:mm:ss tt", ExcelHorizontalAlignment.Right);
                    ApplyNumberStyleToColumn(ws, 5, 2, rowIdx, "mm/dd/yyyy hh:mm:ss tt", ExcelHorizontalAlignment.Right);

                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "@", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 7; autoSizeColIdx++)
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
            System.Diagnostics.Debug.WriteLine("Overstay Report Generation took: " + sw.Elapsed.ToString());
        }
        #endregion

        #region Private/Protected Methods
        protected void GatherReportData(List<int> listOfMeterIDs, DateTime StartTime, DateTime EndTime)
        {
            this._ReportDataModel = new SensingAndPaymentStatistic_ReportData();

            // Adjust the date ranges as needed for our SQL queries.
            // The end time needs to be inclusive of the entire minute (seconds and milliseconds are not in the resolution of the passed EndTime parameter)
            DateTime AdjustedStartTime;
            DateTime AdjustedEndTime;
            AdjustedStartTime = StartTime;
            AdjustedEndTime = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTime.Hour, EndTime.Minute, 0).AddMinutes(1);

            // Gather all applicable vehicle sensing data (minimizes how many individual SQL queries must be executed)
            List<HistoricalSensingRecord> RawSensingDataForAllMeters = new SensingDatabaseSource(_CustomerConfig).GetHistoricalVehicleSensingDataForMeters_StronglyTyped(
                this._CustomerConfig.CustomerId, listOfMeterIDs, AdjustedStartTime, AdjustedEndTime, true);

            // Payment data is no applicable to this report, so we will just use an empty data set instead of getting from database
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
                OverstayGroupStats_Area AreaStatsObj = null;
                foreach (OverstayGroupStats_Area existingArea in this._OverstayReportModel.AreaStats)
                {
                    if (existingArea.AreaID == nextAreaID)
                    {
                        AreaStatsObj = existingArea;
                        break;
                    }
                }
                if (AreaStatsObj == null)
                {
                    AreaStatsObj = new OverstayGroupStats_Area();
                    AreaStatsObj.AreaID = nextAreaID;
                    AreaStatsObj.AreaName = ResolvedAreaNameForAreaID(nextAreaID);
                    this._OverstayReportModel.AreaStats.Add(AreaStatsObj);
                }

                // Now aggregate info for this area
                foreach (VSEventAndDuration overstayVio in _OverstayReportModel.OverstayViolations)
                {
                    // Skip next overstay vio record if its not for the desired area
                    if (overstayVio.AreaID != nextAreaID)
                        continue;

                    AreaStatsObj.OverstayCount++;
                    AreaStatsObj.TotalOverstayDuration = AreaStatsObj.TotalOverstayDuration.Add(overstayVio.DurationOfTimeBeyondStayLimits);
                }
            }


            // We will also do an overall stats 
            foreach (VSEventAndDuration overstayVio in _OverstayReportModel.OverstayViolations)
            {
                _OverstayReportModel.OverallGroupStats.OverstayCount++;
                _OverstayReportModel.OverallGroupStats.TotalOverstayDuration = _OverstayReportModel.OverallGroupStats.TotalOverstayDuration.Add(overstayVio.DurationOfTimeBeyondStayLimits);
            }

            // Sort this.ReportDataModel.AreaStats by AreaName so it renders in Excel in a nice sort order
            this._OverstayReportModel.AreaStats.Sort(new OverstayGroupStats_AreaLogicalComparer(true));

            // Sort this.ReportDataModel.MeterStats by MeterID so it renders in Excel in a nice sort order
            this._OverstayReportModel.MeterStats.Sort(new OverstayGroupStats_MeterLogicalComparer());

            // Sort this.ReportDataModel.BayStats by BayID so it renders in Excel in a nice sort order
            this._OverstayReportModel.BayStats.Sort(new OverstayGroupStats_BayAndMeterLogicalComparer());
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
            OverstayGroupStats_Meter MeterStatsObj = new OverstayGroupStats_Meter();
            MeterStatsObj.MeterID = meterId;
            this._OverstayReportModel.MeterStats.Add(MeterStatsObj);

            // Now aggregate info for this meter
            foreach (VSEventAndDuration overstayVio in _OverstayReportModel.OverstayViolations)
            {
                // Skip next overstay vio record if its not for the desired meter
                if (overstayVio.MeterID != meterId)
                    continue;

                MeterStatsObj.OverstayCount++;
                MeterStatsObj.TotalOverstayDuration = MeterStatsObj.TotalOverstayDuration.Add(overstayVio.DurationOfTimeBeyondStayLimits);
            }
        }

        protected void AnalyzeDataForSpace(List<HistoricalSensingRecord> filteredSensingData, List<PaymentRecord> Payment, int legacyBitNumber,
            int meterId, int bayNumber, DateTime startTime, DateTime endTime_NotInclusive)
        {
            VSEventAndDuration currentVSEventToProcess = null;
            
            /*this._ReportDataModel.BayStats.Add(BayStatsObj);*/

            List<VSEventAndDuration> spaceEventsAndDurations = new List<VSEventAndDuration>();

            // Need to know current time in customer's timezone
            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

            // Start Analyzing data here 
            foreach (HistoricalSensingRecord rawVSDataRec in filteredSensingData)
            {
                // The data should be filtered to the correct meter and bay, but we might as well check it
                if ((rawVSDataRec.MeterMappingId != meterId) || (rawVSDataRec.BayId != bayNumber))
                    continue;

                // Are we ready to begin a new object?
                if (currentVSEventToProcess == null)
                {
                    currentVSEventToProcess = new VSEventAndDuration();
                    currentVSEventToProcess.MeterID = meterId;
                    currentVSEventToProcess.BayID = bayNumber;
                    currentVSEventToProcess.DateTime_Start = rawVSDataRec.DateTime;
                    currentVSEventToProcess.IsOccupied = rawVSDataRec.IsOccupied;

                    // Add newly created object to our list
                    spaceEventsAndDurations.Add(currentVSEventToProcess);

                    // Nothing more to do until we get to the next record (or end of records)
                    continue;
                }

                // Need to see if there is a change in occupancy status. If so, we need to finalize our previous object
                // and then start a new one
                if (currentVSEventToProcess.IsOccupied != rawVSDataRec.IsOccupied)
                {
                    currentVSEventToProcess.DateTime_End = rawVSDataRec.DateTime;
                    currentVSEventToProcess.Duration = (currentVSEventToProcess.DateTime_End - currentVSEventToProcess.DateTime_Start);

                    // Determine duration, and see it it qualifies as an overstay violation
                    AnalyzeVSEventAndDurationForOverstayViolation(currentVSEventToProcess);

                    // Finished getting data for previous event, so clear current object reference so a new one will be started
                    currentVSEventToProcess = null;

                    // JLA 2013-08-01: We must be careful not to skip events! This means the next event object needs to be started based on the current record, not the next!
                    currentVSEventToProcess = new VSEventAndDuration();
                    currentVSEventToProcess.MeterID = meterId;
                    currentVSEventToProcess.BayID = bayNumber;
                    currentVSEventToProcess.DateTime_Start = rawVSDataRec.DateTime;
                    currentVSEventToProcess.IsOccupied = rawVSDataRec.IsOccupied;
                    // Add newly created object to our list
                    spaceEventsAndDurations.Add(currentVSEventToProcess);
                }
            }

            // No more records, so finalize our current object (if there is one)
            // We will need to assume an end time, but let's make sure it does't exceed current time (in customer timezone), or the report end time
            if (currentVSEventToProcess != null)
            {
                DateTime LesserOfCurrentTimeOrReportEndTime = NowAtDestination;
                if (NowAtDestination >  endTime_NotInclusive)
                    LesserOfCurrentTimeOrReportEndTime = endTime_NotInclusive;

                currentVSEventToProcess.DateTime_End = LesserOfCurrentTimeOrReportEndTime;
                currentVSEventToProcess.Duration = (currentVSEventToProcess.DateTime_End - currentVSEventToProcess.DateTime_Start);

                // DEBUG: Need to determine duration, then see if we qualify as an overstay, etc...
                AnalyzeVSEventAndDurationForOverstayViolation(currentVSEventToProcess);
            }


            // Create a meter statistics object for the current meter, and add to our report list
            OverstayGroupStats_BayAndMeter BayStatsObj = new OverstayGroupStats_BayAndMeter();
            BayStatsObj.BayID = bayNumber;
            BayStatsObj.MeterID = meterId;
            this._OverstayReportModel.BayStats.Add(BayStatsObj);

            // Now aggregate info for this bay
            foreach (VSEventAndDuration overstayVio in _OverstayReportModel.OverstayViolations)
            {
                // Skip next overstay vio record if its not for the desired bay
                if ((overstayVio.BayID != bayNumber) || (overstayVio.MeterID != meterId))
                    continue;

                BayStatsObj.OverstayCount++;
                BayStatsObj.TotalOverstayDuration = BayStatsObj.TotalOverstayDuration.Add(overstayVio.DurationOfTimeBeyondStayLimits);
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

        protected void AnalyzeVSEventAndDurationForOverstayViolation(VSEventAndDuration currentVSEventToProcess)
        {
            // There is nothing to do if this event is not for occupied status
            if (currentVSEventToProcess.IsOccupied == false)
                return;

            // Find the space asset associated with this data model.  If the space is "inactive" (based on the "IsActive" column of "HousingMaster" table in database),
            // then we will not consider the space to be in a violating state, because the sensor is effectively marked as bad/untrusted
            SpaceAsset spcAsset = GetSpaceAsset(currentVSEventToProcess.MeterID, currentVSEventToProcess.BayID);
            if (spcAsset != null)
            {
                // Nothing more to do if the space isn't active
                if (spcAsset.IsActive == false)
                    return;
            }

            // Resolve the associated area for the meter
            int areaID = ResolveAreaIDForMeterID(currentVSEventToProcess.MeterID);
            
            // Try to obtain the regulated hours applicable to this meter
            RegulatedHoursGroup regulatedHours = RegulatedHoursGroupRepository.Repository.GetBestGroupForMeter(this._CustomerConfig.CustomerId, areaID, currentVSEventToProcess.MeterID);

            // If no regulated hour defintions came back, then we are unable to calculate any overstay violation, so just exit
            if ((regulatedHours == null) || (regulatedHours.Details == null) || (regulatedHours.Details.Count == 0))
                return;

            DateTime ruleStart = DateTime.MinValue;
            DateTime ruleEnd = DateTime.MinValue;

            TimeSlot OccupiedSegment = new TimeSlot(currentVSEventToProcess.DateTime_Start, currentVSEventToProcess.DateTime_End);

            // We need to check if this single occupancy event is an overstay violation for multiple rules, or even for more than one day, etc.
            while (OccupiedSegment.Start < currentVSEventToProcess.DateTime_End)
            {
                // Determine the day of week that is involved
                int dayOfWeek = (int)OccupiedSegment.Start.DayOfWeek;

                // Loop through the daily rules and see which ones overlap with our occupied period
                foreach (RegulatedHoursDetail detail in regulatedHours.Details)
                {
                    // Skip this one if its for a different day of the week
                    if (detail.DayOfWeek != dayOfWeek)
                        continue;

                    // Determine if the occupied timeslot overlaps with the rule's timeslot
                    ruleStart = new DateTime(OccupiedSegment.Start.Year, OccupiedSegment.Start.Month, OccupiedSegment.Start.Day, detail.StartTime.Hour, detail.StartTime.Minute, 0);
                    ruleEnd = new DateTime(OccupiedSegment.Start.Year, OccupiedSegment.Start.Month, OccupiedSegment.Start.Day, detail.EndTime.Hour, detail.EndTime.Minute, 59);
                    TimeSlot RuleSegment = new TimeSlot(ruleStart, ruleEnd);

                    // We only care about this overlapping rule if the MaxStayMinutes is greater than zero (zero or less means there is no MaxStay that is enforced),
                    // or if it's explicitly set as a "No Parking" regulation
                    if ( (RuleSegment.OverlapsWith(OccupiedSegment) == true) && 
                         ((detail.MaxStayMinutes > 0) || (string.Compare(detail.Type, "No Parking", true) == 0)) 
                       )
                    {
                        // Normally we will use the verbatim value of the max stay minutes, but if its a "No Parking", we will always take that to mean 0 minutes is the actual max
                        int timeLimitMinutes = detail.MaxStayMinutes;
                        if (string.Compare(detail.Type, "No Parking", true) == 0)
                            timeLimitMinutes = 0;

                        // Get the intersection of the overlaps so we know how long the vehicle has been occupied during this rule
                        TimeSlot OccupiedIntersection = RuleSegment.GetIntersection(OccupiedSegment);

                        // Determine if the vehicle has been occupied during this rule segment in excess of the MaxStayMinutes
                        if (OccupiedIntersection != null)
                        {
                            if (OccupiedIntersection.Duration.TotalMinutes >= timeLimitMinutes)
                            {
                                // Since this event might qualify for multiple overstay violations, we must clone the object instead of manipulating the same one
                                VSEventAndDuration reportableEvent = new VSEventAndDuration();
                                reportableEvent.MeterID = currentVSEventToProcess.MeterID;
                                reportableEvent.BayID = currentVSEventToProcess.BayID;
                                reportableEvent.DateTime_Start = currentVSEventToProcess.DateTime_Start;
                                reportableEvent.DateTime_End = currentVSEventToProcess.DateTime_End;
                                reportableEvent.Duration = currentVSEventToProcess.Duration;
                                reportableEvent.IsOccupied = currentVSEventToProcess.IsOccupied;

                                reportableEvent.AreaID = areaID;
                                reportableEvent.OverstayBasedOnRuleDetail = detail;
                                reportableEvent.OverstayBasedOnRuleGroup = regulatedHours;

                                reportableEvent.DateTime_StartOfOverstayViolation = new DateTime(OccupiedIntersection.Start.Ticks).AddMinutes(timeLimitMinutes);
                                reportableEvent.DurationOfTimeBeyondStayLimits = new TimeSpan(OccupiedIntersection.Duration.Ticks).Add(new TimeSpan(0, (-1) * timeLimitMinutes, 0));

                                // Since this has been evaluated as an overstay violation, we will add it to the list that the report will use
                                _OverstayReportModel.OverstayViolations.Add(reportableEvent);
                            }
                        }
                    }
                }

                // Rules for current day of week have been processed.  So now we will advance to beginning of next day and see if there are more violations that we will use
                // to add accumulated time in violation state...
                OccupiedSegment = new TimeSlot(new DateTime(OccupiedSegment.Start.Year, OccupiedSegment.Start.Month, OccupiedSegment.Start.Day).AddDays(1),
                    currentVSEventToProcess.DateTime_End);
            }
        }

        protected SpaceAsset GetSpaceAsset(int meterID, int spaceID)
        {
            SpaceAsset result = null;

            // If we have a list of cached assets, look through it, which should be faster than constantly asking the customer manager for it
            // since we would have a local copy and not need to worry about concurrency issues
            if (_cachedSpaceAssets != null)
            {
                foreach (SpaceAsset asset in _cachedSpaceAssets)
                {
                    if ((asset.MeterID == meterID) && (asset.SpaceID == spaceID))
                    {
                        result = asset;
                        break;
                    }
                }
            }
            else
            {
                result = CustomerLogic.CustomerManager.GetSpaceAsset(_CustomerConfig, meterID, spaceID);
            }

            return result;
        }

        #endregion
    }
}