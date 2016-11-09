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
using Duncan.PEMS.SpaceStatus.DataMappers;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.Models
{
    public abstract class BaseExcelReport
    {
        public BaseExcelReport()
        {
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

        protected void ApplyNumberStyleToCell(ExcelWorksheet ws, int Row, int Col, string numberFormat, ExcelHorizontalAlignment horzAlign)
        {
            ApplyNumberStyleToColumn(ws, Col, Row, Row, numberFormat, horzAlign);
        }

        protected void ApplyVertAlignToCell(ExcelWorksheet ws, int Row, int Col, ExcelVerticalAlignment vertAlign)
        {
            using (OfficeOpenXml.ExcelRange col = ws.Cells[Row, Col])
            {
                col.Style.VerticalAlignment = vertAlign;
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

        protected void MergeCellRange(ExcelWorksheet ws, int FromRow, int FromCol, int ToRow, int ToCol)
        {
            try
            {
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[FromRow, FromCol, ToRow, ToCol])
                {
                    rng.Merge = true;
                }
            }
            catch { }
        }

        protected void ApplyWrapTextStyleToCell(ExcelWorksheet ws, int FromRow, int FromCol)
        {
            try
            {
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[FromRow, FromCol])
                {
                    rng.Style.WrapText = true;
                }
            }
            catch { }
        }
    }

    public abstract class BaseSensorReport : BaseExcelReport
    {
        protected string _ReportName = "Generic Report"; // Should be set to correct value by inherited class
        protected int numberOfHourlyCategories = 1; // Should be set to correct value by inherited class
        protected int numColumnsMergedForHeader = 10;
        protected SensorAndPaymentReportEngine.TimeIsolationOptions timeIsolation = new SensorAndPaymentReportEngine.TimeIsolationOptions();

        protected int rowIdx = 1;
        protected int colIdx = 1;
        protected int colIdx_1stCommonColumn = 1;
        protected int colIdx_HourlyCategory = -1;

        protected CustomerConfig _CustomerConfig = null;
        protected SensorAndPaymentReportEngine.CommonReportParameters _ReportParams = null;
        protected SensorAndPaymentReportEngine _ReportEngine = null;

        public static bool SupportsHourlyStatistics = true;
        public static bool SupportsAreaSummary = true;
        public static bool SupportsMeterSummary = true;
        public static bool SupportsSpaceSummary = true;
        public static bool SupportsDailySummary = true;
        public static bool SupportsMonthlySummary = true;
        public static bool SupportsDetailRecords = true;

        public BaseSensorReport(CustomerConfig customerCfg, SensorAndPaymentReportEngine.CommonReportParameters reportParams)
            : base()
        {
            _CustomerConfig = customerCfg;
            _ReportParams = reportParams;
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
        }

        protected void RenderCommonReportTitle(ExcelWorksheet ws, string reportName)
        {
            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

            OfficeOpenXml.Style.ExcelRichTextCollection rtfCollection = null;

            // Render the header row
            ws.SetValue(rowIdx, 1, reportName);
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, numColumnsMergedForHeader])
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
            using (OfficeOpenXml.ExcelRange richCell = ws.Cells[rowIdx, 1])
            {
                richCell.IsRichText = true;
                rtfCollection = richCell.RichText;
                AddRichTextNameAndValue(rtfCollection, "Client: ", this._CustomerConfig.CustomerName);
                AddRichTextNameAndValue(rtfCollection, ",  Generated: ", NowAtDestination.ToString("yyyy-MM-dd") + "  " + NowAtDestination.ToShortTimeString());
            }
            MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);

            rowIdx++;
            using (OfficeOpenXml.ExcelRange richCell = ws.Cells[rowIdx, 1])
            {
                richCell.IsRichText = true;
                rtfCollection = richCell.RichText;
                AddRichTextNameAndValue(rtfCollection, "Date Range:  ", _ReportParams.StartTime.ToString("yyyy-MM-dd") + "  " + _ReportParams.StartTime.ToShortTimeString());
                AddRichTextNameAndValue(rtfCollection, " to ", _ReportParams.EndTime.ToString("yyyy-MM-dd") + "  " + _ReportParams.EndTime.ToShortTimeString());
            }
            MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);

            if (!string.IsNullOrEmpty(_ReportParams.ScopedAreaName))
            {
                rowIdx++;
                using (OfficeOpenXml.ExcelRange richCell = ws.Cells[rowIdx, 1])
                {
                    richCell.IsRichText = true;
                    rtfCollection = richCell.RichText;
                    AddRichTextNameAndValue(rtfCollection, "Report limited to area: ", _ReportParams.ScopedAreaName);
                }
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }

            if (!string.IsNullOrEmpty(_ReportParams.ScopedMeter))
            {
                rowIdx++;
                using (OfficeOpenXml.ExcelRange richCell = ws.Cells[rowIdx, 1])
                {
                    richCell.IsRichText = true;
                    rtfCollection = richCell.RichText;
                    AddRichTextNameAndValue(rtfCollection, "Report limited to meter: ", _ReportParams.ScopedMeter);
                }
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
        }

        protected void RenderCommonReportFilterHeader_ActionTakenRestrictions(ExcelWorksheet ws)
        {
            // Nothing special to output for all activity (no restrictions on Action Taken violation status)
            if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.AllActivity)
                return;

            OfficeOpenXml.Style.ExcelRichTextCollection rtfCollection = null;
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1])
            {
                rng.IsRichText = true;
                rtfCollection = rng.RichText;
            }
            /*
            if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.AllActivity)
            {
                AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "All");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else 
            */
            if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.Actioned)
            {
                AddRichTextNameAndValue(rtfCollection, "Action Taken Filter: ", "Actioned violations");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.NotActioned)
            {
                AddRichTextNameAndValue(rtfCollection, "Action Taken Filter: ", "Non-actioned violations");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.Enforced)
            {
                AddRichTextNameAndValue(rtfCollection, "Action Taken Filter: ", "Enforced violations");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.NotEnforced)
            {
                AddRichTextNameAndValue(rtfCollection, "Action Taken Filter: ", "Non-enforced violations");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.Cautioned)
            {
                AddRichTextNameAndValue(rtfCollection, "Action Taken Filter: ", "Cautions");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.ActionTakenRestrictionFilter == SensorAndPaymentReportEngine.ReportableEnforcementActivity.Fault)
            {
                AddRichTextNameAndValue(rtfCollection, "Action Taken Filter: ", "Faults");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
        }

        protected void RenderCommonReportFilterHeader_RegulatedHourRestrictions(ExcelWorksheet ws)
        {
            OfficeOpenXml.Style.ExcelRichTextCollection rtfCollection = null;
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1])
            {
                rng.IsRichText = true;
                rtfCollection = rng.RichText;
            }

            if (this._ReportParams.RegulatedHoursRestrictionFilter == SensorAndPaymentReportEngine.RegulatedHoursRestrictions.AllActivity)
            {
                AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "All");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.RegulatedHoursRestrictionFilter == SensorAndPaymentReportEngine.RegulatedHoursRestrictions.OnlyDuringRegulatedHours)
            {
                AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "During Regulated Hours");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
            else if (this._ReportParams.RegulatedHoursRestrictionFilter == SensorAndPaymentReportEngine.RegulatedHoursRestrictions.OnlyDuringUnregulatedHours)
            {
                AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "During Unregulated Hours");
                MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
            }
        }

        protected void RenderWorksheetHyperlink(ExcelWorksheet ws, string workSheetName, string linkDescription)
        {
            rowIdx++;
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1])
            {
                rng.Formula = "HYPERLINK(\"#'" + workSheetName + "'!A1\", \"Click here for " + linkDescription + "\")";
                // Even though its a hyperlink, it won't look like one unless we style it
                rng.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                rng.Style.Font.UnderLine = true;
            }
            MergeCellRange(ws, rowIdx, 1, rowIdx, numColumnsMergedForHeader);
        }

        protected virtual void RenderCommonHeader(ExcelWorksheet ws, int startRowIdx, int startColIdx, ref int colIdx_HourlyCategory)
        {
            throw new NotImplementedException("RenderCommonHeader is not implemented at base level of 'BaseSensorReport' class");
        }

        protected virtual void RenderCommonData(ExcelWorksheet ws, int startRowIdx, int startColIdx, ref int colIdx_HourlyCategory, GroupStats statsObj)
        {
            throw new NotImplementedException("RenderCommonData is not implemented at base level of 'BaseSensorReport' class");
        }

        protected virtual void RenderCommonHourlyData(ExcelWorksheet ws, int startRowIdx, ref int colIdx_HourlyCategory, int hourIdx, GroupStats hourlyStats)
        {
            throw new NotImplementedException("RenderCommonHourlyData is not implemented at base level of 'BaseSensorReport' class");
        }

        protected void RenderOverallReportSummary(ExcelWorksheet ws)
        {
            ws.SetValue(rowIdx, 1, "Overall Summary");
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, 5])
            {
                rng.Merge = true; //Merge columns start and end range
                rng.Style.Font.Bold = true; //Font should be bold
                rng.Style.Font.Size = 12;
                rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Render the header row of overall summary
            int overallSummaryHeaderRowIdx = rowIdx;
            colIdx_1stCommonColumn = 1;
            colIdx_HourlyCategory = -1;

            // Render common portion of header
            rowIdx++;
            RenderCommonHeader(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory);

            // We only have one row of data for Overall summary, so output it now
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            GroupStats overallStats = this._ReportEngine.GetOverallStats(timeIsolation);

            // Increment the row index, which will now be the 1st row of our data
            rowIdx++;
            RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, overallStats);

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                // And then loop for each hour of the day, to render hourly summary data
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Get applicable data for this hour
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                    timeIsolation.Hour = hourIdx;
                    GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                    RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                }

                // Account for the rows added for hourly data
                rowIdx += numberOfHourlyCategories;
            }
        }

        protected void RenderMeterSummaryWorksheet(OfficeOpenXml.ExcelPackage pck, string worksheetName)
        {
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            GroupStats subtotalStats = null;

            // Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheetName);

            // Render the header row
            rowIdx = 1; // Excel uses 1-based indexes

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "Meter #");

            colIdx++;
            ws.SetValue(rowIdx, colIdx, "Area #");

            colIdx++;
            ws.SetValue(rowIdx, colIdx, "Area");

            // Format the header row
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            colIdx++;
            colIdx_1stCommonColumn = colIdx;

            // Render common portion of header
            RenderCommonHeader(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory);

            // Increment the row index, which will now be the 1st row of our data
            rowIdx++;
            colIdx = 1;

            // We will be grouping by area, then loop for each meter that is related to the area
            foreach (AreaAsset areaAsset in this._ReportEngine.ReportDataModel.AreasIncludedInReport)
            {
                foreach (MeterAsset meterAsset in this._ReportEngine.ReportDataModel.MetersIncludedInReport)
                {
                    // Skip this meter asset if it doesn't match the current area we are grouping by
                    if (meterAsset.AreaID_PreferLibertyBeforeInternal != areaAsset.AreaID)
                        continue;

                    // Collect the stats applicable to this meter
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
                    GroupStats meterStats = this._ReportEngine.GetMeterStats(meterAsset.MeterID, timeIsolation);

                    // Output row values for data
                    colIdx = 1;
                    ws.SetValue(rowIdx, colIdx, meterAsset.MeterID);

                    colIdx++;
                    ws.SetValue(rowIdx, colIdx, areaAsset.AreaID);

                    colIdx++;
                    ws.SetValue(rowIdx, colIdx, areaAsset.AreaName);

                    RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, meterStats);

                    if (this._ReportParams.IncludeHourlyStatistics == true)
                    {
                        for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                        {
                            // Get applicable data for this hour
                            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                            timeIsolation.Hour = hourIdx;
                            GroupStats hourlyStats = this._ReportEngine.GetMeterStats(meterAsset.MeterID, timeIsolation);

                            RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                        }

                        // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                        for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                        {
                            ws.Row(loGroupIdx).OutlineLevel = 1;
                            ws.Row(loGroupIdx).Collapsed = true;
                        }

                        // Increment the row index, which will now be the next row of our data
                        // but also account for the rows added for hourly data
                        rowIdx += numberOfHourlyCategories;
                    }

                    // And now add a blank row for the benefit of grouping/outlining in the worksheet
                    rowIdx++;
                }

                // Each meter for the current area has been reported, so no we need to output a subtotal for the area
                timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
                subtotalStats = this._ReportEngine.GetAreaStats(areaAsset.AreaID, timeIsolation);

                colIdx = 1;
                ws.SetValue(rowIdx, colIdx, "SUBTOTAL AREA");
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, 3])
                {
                    rng.Style.Font.Bold = true;
                    try
                    {
                        rng.Merge = true;
                    }
                    catch { }
                }

                RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

                if (this._ReportParams.IncludeHourlyStatistics == true)
                {
                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Get applicable data for this hour
                        timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                        timeIsolation.Hour = hourIdx;
                        GroupStats hourlyStats = this._ReportEngine.GetAreaStats(areaAsset.AreaID, timeIsolation);

                        RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                    }

                    // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                    for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                    {
                        ws.Row(loGroupIdx).OutlineLevel = 1;
                        ws.Row(loGroupIdx).Collapsed = true;
                    }

                    // Increment the row index, which will now be the next row of our data
                    // but also account for the rows added for hourly data
                    rowIdx += numberOfHourlyCategories;
                }

                // And now add a blank row for the benefit of grouping/outlining in the worksheet
                rowIdx++;
            }


            // Each area for the current area has been reported, so no we need to output a grand total
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "TOTAL");
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, 3])
            {
                rng.Style.Font.Bold = true;
                try
                {
                    rng.Merge = true;
                }
                catch { }
            }

            RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Get applicable data for this hour
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                    timeIsolation.Hour = hourIdx;
                    GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                    RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                }

                // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                {
                    ws.Row(loGroupIdx).OutlineLevel = 1;
                    ws.Row(loGroupIdx).Collapsed = true;
                }

                // Increment the row index, which will now be the next row of our data
                // but also account for the rows added for hourly data
                rowIdx += numberOfHourlyCategories;
            }

            // And now add a blank row for the benefit of grouping/outlining in the worksheet
            rowIdx++;

            /*
            // We will add autofilters to our headers so user can sort the columns easier
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 3])
            {
                rng.AutoFilter = true;
            }
            */

            // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

            // Column 1 is numeric integer (Meter ID)
            ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

            // Column 2 is numeric integer (Area ID)
            ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

            // Column 3 is text
            ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
        }

        protected void RenderSpaceSummaryWorksheet(OfficeOpenXml.ExcelPackage pck, string worksheetName)
        {
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            GroupStats subtotalStats = null;

            // Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheetName);

            // Render the header row
            rowIdx = 1; // Excel uses 1-based indexes

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "Space #");

            colIdx++;
            ws.SetValue(rowIdx, colIdx, "Meter #");

            colIdx++;
            ws.SetValue(rowIdx, colIdx, "Area #");

            colIdx++;
            ws.SetValue(rowIdx, colIdx, "Area");

            // Format the header row
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            colIdx++;
            colIdx_1stCommonColumn = colIdx;

            // Render common portion of header
            RenderCommonHeader(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory);

            // Increment the row index, which will now be the 1st row of our data
            rowIdx++;
            colIdx = 1;

            // We will be grouping by area, then loop for each meter that is related to the area
            foreach (AreaAsset areaAsset in this._ReportEngine.ReportDataModel.AreasIncludedInReport)
            {
                foreach (SpaceAsset spaceAsset in this._ReportEngine.ReportDataModel.SpacesIncludedInReport)
                {
                    // Skip this meter asset if it doesn't match the current area we are grouping by
                    if (spaceAsset.AreaID_PreferLibertyBeforeInternal != areaAsset.AreaID)
                        continue;

                    // Collect the stats applicable to this meter
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
                    GroupStats SpaceStats = this._ReportEngine.GetSpaceStats(spaceAsset.MeterID, spaceAsset.SpaceID, timeIsolation);

                    // Output row values for data
                    colIdx = 1;
                    ws.SetValue(rowIdx, colIdx, spaceAsset.SpaceID);

                    colIdx++;
                    ws.SetValue(rowIdx, colIdx, spaceAsset.MeterID);

                    colIdx++;
                    ws.SetValue(rowIdx, colIdx, areaAsset.AreaID);

                    colIdx++;
                    ws.SetValue(rowIdx, colIdx, areaAsset.AreaName);

                    RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, SpaceStats);

                    if (this._ReportParams.IncludeHourlyStatistics == true)
                    {
                        for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                        {
                            // Get applicable data for this hour
                            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                            timeIsolation.Hour = hourIdx;
                            GroupStats hourlyStats = this._ReportEngine.GetSpaceStats(spaceAsset.MeterID, spaceAsset.SpaceID, timeIsolation);

                            RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                        }

                        // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                        for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                        {
                            ws.Row(loGroupIdx).OutlineLevel = 1;
                            ws.Row(loGroupIdx).Collapsed = true;
                        }

                        // Increment the row index, which will now be the next row of our data
                        // but also account for the rows added for hourly data
                        rowIdx += numberOfHourlyCategories;
                    }

                    // And now add a blank row for the benefit of grouping/outlining in the worksheet
                    rowIdx++;
                }

                // Each meter for the current area has been reported, so no we need to output a subtotal for the area
                timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
                subtotalStats = this._ReportEngine.GetAreaStats(areaAsset.AreaID, timeIsolation);

                colIdx = 1;
                ws.SetValue(rowIdx, colIdx, "SUBTOTAL AREA");
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, 3])
                {
                    rng.Style.Font.Bold = true;
                    try
                    {
                        rng.Merge = true;
                    }
                    catch { }
                }

                RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

                if (this._ReportParams.IncludeHourlyStatistics == true)
                {
                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Get applicable data for this hour
                        timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                        timeIsolation.Hour = hourIdx;
                        GroupStats hourlyStats = this._ReportEngine.GetAreaStats(areaAsset.AreaID, timeIsolation);

                        RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                    }

                    // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                    for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                    {
                        ws.Row(loGroupIdx).OutlineLevel = 1;
                        ws.Row(loGroupIdx).Collapsed = true;
                    }

                    // Increment the row index, which will now be the next row of our data
                    // but also account for the rows added for hourly data
                    rowIdx += numberOfHourlyCategories;
                }

                // And now add a blank row for the benefit of grouping/outlining in the worksheet
                rowIdx++;
            }


            // Each area for the current area has been reported, so no we need to output a grand total
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "TOTAL");
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, 3])
            {
                rng.Style.Font.Bold = true;
                try
                {
                    rng.Merge = true;
                }
                catch { }
            }

            RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Get applicable data for this hour
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                    timeIsolation.Hour = hourIdx;
                    GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                    RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                }

                // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                {
                    ws.Row(loGroupIdx).OutlineLevel = 1;
                    ws.Row(loGroupIdx).Collapsed = true;
                }

                // Increment the row index, which will now be the next row of our data
                // but also account for the rows added for hourly data
                rowIdx += numberOfHourlyCategories;
            }

            // And now add a blank row for the benefit of grouping/outlining in the worksheet
            rowIdx++;

            /*
            // We will add autofilters to our headers so user can sort the columns easier
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 3])
            {
                rng.AutoFilter = true;
            }
            */

            // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

            // Column 1 is numeric integer (Bay ID)
            ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

            // Column 2 is numeric integer (Meter ID)
            ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

            // Column 3 is numeric integer
            ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

            // Column 4 is text
            ApplyNumberStyleToColumn(ws, 4, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
        }

        protected void RenderAreaSummaryWorksheet(OfficeOpenXml.ExcelPackage pck, string worksheetName)
        {
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            GroupStats subtotalStats = null;

            // Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheetName);

            // Render the header row
            rowIdx = 1; // Excel uses 1-based indexes

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "Area #");

            colIdx++;
            ws.SetValue(rowIdx, colIdx, "Area");

            // Format the header row
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            colIdx++;
            colIdx_1stCommonColumn = colIdx;

            // Render common portion of header
            RenderCommonHeader(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory);

            // Increment the row index, which will now be the 1st row of our data
            rowIdx++;
            colIdx = 1;

            // We will be grouping by area, then loop for each meter that is related to the area
            foreach (AreaAsset areaAsset in this._ReportEngine.ReportDataModel.AreasIncludedInReport)
            {
                // Collect the stats applicable to this meter
                timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
                subtotalStats = this._ReportEngine.GetAreaStats(areaAsset.AreaID, timeIsolation);

                // Output row values for data
                colIdx = 1;
                ws.SetValue(rowIdx, colIdx, areaAsset.AreaID);

                colIdx++;
                ws.SetValue(rowIdx, colIdx, areaAsset.AreaName);

                RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

                if (this._ReportParams.IncludeHourlyStatistics == true)
                {
                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Get applicable data for this hour
                        timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                        timeIsolation.Hour = hourIdx;
                        GroupStats hourlyStats = this._ReportEngine.GetAreaStats(areaAsset.AreaID, timeIsolation);

                        RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                    }

                    // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                    for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                    {
                        ws.Row(loGroupIdx).OutlineLevel = 1;
                        ws.Row(loGroupIdx).Collapsed = true;
                    }

                    // Increment the row index, which will now be the next row of our data
                    // but also account for the rows added for hourly data
                    rowIdx += numberOfHourlyCategories;
                }

                // And now add a blank row for the benefit of grouping/outlining in the worksheet
                rowIdx++;
            }


            // Each area for the current area has been reported, so no we need to output a grand total
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "TOTAL");
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, 2])
            {
                rng.Style.Font.Bold = true;
                try
                {
                    rng.Merge = true;
                }
                catch { }
            }

            RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Get applicable data for this hour
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                    timeIsolation.Hour = hourIdx;
                    GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                    RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                }

                // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                {
                    ws.Row(loGroupIdx).OutlineLevel = 1;
                    ws.Row(loGroupIdx).Collapsed = true;
                }

                // Increment the row index, which will now be the next row of our data
                // but also account for the rows added for hourly data
                rowIdx += numberOfHourlyCategories;
            }

            // And now add a blank row for the benefit of grouping/outlining in the worksheet
            rowIdx++;

            // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

            // Column 1 is numeric integer
            ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

            // Column 2 is text, but right-justified
            ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
        }

        protected void RenderDailySummaryWorksheet(OfficeOpenXml.ExcelPackage pck, string worksheetName)
        {
            GroupStats subtotalStats = null;

            // Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheetName);

            // Render the header row
            rowIdx = 1; // Excel uses 1-based indexes

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "Date");

            // Format the header row
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            colIdx++;
            colIdx_1stCommonColumn = colIdx;

            // Render common portion of header
            RenderCommonHeader(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory);

            // Increment the row index, which will now be the 1st row of our data
            rowIdx++;
            colIdx = 1;

            // Loop for each date in the reportable range
            DateTime targetDate = _ReportParams.StartTime.Date;
            while (targetDate < _ReportParams.EndTime.Date)
            {
                timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Date;
                timeIsolation.Date = targetDate;
                subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

                // Output row values for data
                colIdx = 1;
                ws.SetValue(rowIdx, colIdx, targetDate);

                RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

                if (this._ReportParams.IncludeHourlyStatistics == true)
                {
                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Get applicable data for this hour
                        timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.DateAndHour;
                        timeIsolation.Date = targetDate;
                        timeIsolation.Hour = hourIdx;
                        GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                        RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                    }

                    // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                    for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                    {
                        ws.Row(loGroupIdx).OutlineLevel = 1;
                        ws.Row(loGroupIdx).Collapsed = true;
                    }

                    // Increment the row index, which will now be the next row of our data
                    // but also account for the rows added for hourly data
                    rowIdx += numberOfHourlyCategories;
                }

                // And now add a blank row for the benefit of grouping/outlining in the worksheet
                rowIdx++;

                // Increment the target date by 1 day
                targetDate = targetDate.AddDays(1);
            }

            // Each area for the current area has been reported, so no we need to output a grand total
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "TOTAL");
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                try
                {
                    rng.Merge = true;
                }
                catch { }
            }

            RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Get applicable data for this hour
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                    timeIsolation.Hour = hourIdx;
                    GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                    RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                }

                // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                {
                    ws.Row(loGroupIdx).OutlineLevel = 1;
                    ws.Row(loGroupIdx).Collapsed = true;
                }

                // Increment the row index, which will now be the next row of our data
                // but also account for the rows added for hourly data
                rowIdx += numberOfHourlyCategories;
            }

            // And now add a blank row for the benefit of grouping/outlining in the worksheet
            rowIdx++;

            /*
            // We will add autofilters to our headers so user can sort the columns easier
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 3])
            {
                rng.AutoFilter = true;
            }
            */

            // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

            // Column 1 is date (without time)
            ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "yyyy-mm-dd", ExcelHorizontalAlignment.Left);

            // And now lets size the non-common columns
            for (int autoSizeColIdx = 1; autoSizeColIdx <= 1; autoSizeColIdx++)
            {
                using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                {
                    col.AutoFitColumns();
                }
            }
        }

        protected void RenderMonthlySummaryWorksheet(OfficeOpenXml.ExcelPackage pck, string worksheetName)
        {
            GroupStats subtotalStats = null;

            // Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheetName);


            // Render the header row
            rowIdx = 1; // Excel uses 1-based indexes

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "Month");

            // Format the header row
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            }

            colIdx++;
            colIdx_1stCommonColumn = colIdx;

            // Render common portion of header
            RenderCommonHeader(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory);

            // Increment the row index, which will now be the 1st row of our data
            rowIdx++;
            colIdx = 1;

            // Loop for each date in the reportable range
            DateTime targetDate = new DateTime(_ReportParams.StartTime.Date.Year, _ReportParams.StartTime.Date.Month, 1); // Must start at day 1 of the month for complete reporting
            while (targetDate < _ReportParams.EndTime.Date)
            {
                timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Month;
                timeIsolation.Month = targetDate.Month;
                subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

                // Output row values for data
                colIdx = 1;
                ws.SetValue(rowIdx, colIdx, targetDate.ToString("MMM yyyy"));

                RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

                if (this._ReportParams.IncludeHourlyStatistics == true)
                {
                    for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                    {
                        // Get applicable data for this hour
                        timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.MonthAndHour;
                        timeIsolation.Month = targetDate.Month;
                        timeIsolation.Hour = hourIdx;
                        GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                        RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                    }

                    // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                    for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                    {
                        ws.Row(loGroupIdx).OutlineLevel = 1;
                        ws.Row(loGroupIdx).Collapsed = true;
                    }

                    // Increment the row index, which will now be the next row of our data
                    // but also account for the rows added for hourly data
                    rowIdx += numberOfHourlyCategories;
                }

                // And now add a blank row for the benefit of grouping/outlining in the worksheet
                rowIdx++;

                // Increment the target date by 1 month
                targetDate = targetDate.AddMonths(1);
            }

            // Each area for the current area has been reported, so no we need to output a grand total
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;
            subtotalStats = this._ReportEngine.GetOverallStats(timeIsolation);

            colIdx = 1;
            ws.SetValue(rowIdx, colIdx, "TOTAL");
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, colIdx, rowIdx, colIdx])
            {
                rng.Style.Font.Bold = true;
                try
                {
                    rng.Merge = true;
                }
                catch { }
            }

            RenderCommonData(ws, rowIdx, colIdx_1stCommonColumn, ref colIdx_HourlyCategory, subtotalStats);

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                for (int hourIdx = 0; hourIdx < 24; hourIdx++)
                {
                    // Get applicable data for this hour
                    timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.Hour;
                    timeIsolation.Hour = hourIdx;
                    GroupStats hourlyStats = this._ReportEngine.GetOverallStats(timeIsolation);

                    RenderCommonHourlyData(ws, rowIdx, ref colIdx_HourlyCategory, hourIdx, hourlyStats);
                }

                // Set grouping/outline in Excel worksheet so detail can be expanded or collapsed
                for (int loGroupIdx = rowIdx + 1; loGroupIdx < rowIdx + numberOfHourlyCategories; loGroupIdx++)
                {
                    ws.Row(loGroupIdx).OutlineLevel = 1;
                    ws.Row(loGroupIdx).Collapsed = true;
                }

                // Increment the row index, which will now be the next row of our data
                // but also account for the rows added for hourly data
                rowIdx += numberOfHourlyCategories;
            }

            // And now add a blank row for the benefit of grouping/outlining in the worksheet
            rowIdx++;

            /*
            // We will add autofilters to our headers so user can sort the columns easier
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 3])
            {
                rng.AutoFilter = true;
            }
            */

            // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

            // Column 1 is text (Month)
            ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);

            // And now lets size the non-common columns
            for (int autoSizeColIdx = 1; autoSizeColIdx <= 1; autoSizeColIdx++)
            {
                using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                {
                    col.AutoFitColumns();
                }
            }
        }
    }
}