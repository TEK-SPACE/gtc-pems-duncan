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

namespace Duncan.PEMS.SpaceStatus.Models
{
    public class ComplianceReportEngine
    {
        public enum ActivityRestrictions
        {
            AllActivity, OnlyDuringRegulatedHours, OnlyDuringUnregulatedHours
        }

        #region Private/Protected Members
        protected CustomerConfig _CustomerConfig = null;
        protected ActivityRestrictions _ActivityRestriction = ActivityRestrictions.AllActivity;

        protected SensingAndPaymentStatistic_ReportData _ReportDataModel = new SensingAndPaymentStatistic_ReportData();
        #endregion

        #region Public Methods
        public ComplianceReportEngine(CustomerConfig customerCfg)
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
                ws.Cells[rowIdx, 1].Value = "Compliance Statistics Report";
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
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Meter Compliance'!A1\", \"Click here for Meter Compliance summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeSpaceSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Space Compliance'!A1\", \"Click here for Space Compliance summary\")";
                    // Even though its a hyperlink, it won't look like one unless we style it
                    ws.Cells[rowIdx, 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    ws.Cells[rowIdx, 1].Style.Font.UnderLine = true;
                    ws.Cells[rowIdx, 1, rowIdx, 10].Merge = true;
                }
                if (includeAreaSummary == true)
                {
                    rowIdx++;
                    ws.Cells[rowIdx, 1].Formula = "HYPERLINK(\"#'Area Compliance'!A1\", \"Click here for Area Compliance summary\")";
                    //ws.Cells[rowIdx, 1].Hyperlink = new ExcelHyperLink("#'Area Compliance'!A1", "Click here to jump to Area Compliance summary"); 
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
                ws.Cells[rowIdx, 2].Value = "Occupied and Paid Duration";
                ws.Cells[rowIdx, 3].Value = "Max Possible Occupied Duration";
                ws.Cells[rowIdx, 4].Value = "Occupied and Paid %";
                ws.Cells[rowIdx, 5].Value = "Occupied and Not Paid %";
                ws.Cells[rowIdx, 6].Value = "Vehicle Arrivals";
                ws.Cells[rowIdx, 7].Value = "Vehicle Departures";
                ws.Cells[rowIdx, 8].Value = "Payment Count";
                
                // Format the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, 8])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Increment the row index, which will now be the 1st row of our data
                rowIdx++;
                // We only have one row of data for Overall summary, so output it now
                ws.Cells[rowIdx, 1].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats.TotalOccupancyTime);
                ws.Cells[rowIdx, 2].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats.TotalOccupancyPaidTime);
                ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(this._ReportDataModel.OverallStats.MaximumPotentialOccupancyTime);
                ws.Cells[rowIdx, 4].Value = this._ReportDataModel.OverallStats.PercentageOccupiedPaid;
                ws.Cells[rowIdx, 5].Value = this._ReportDataModel.OverallStats.PercentageOccupiedNotPaid;
                ws.Cells[rowIdx, 6].Value = this._ReportDataModel.OverallStats.ingress;
                ws.Cells[rowIdx, 7].Value = this._ReportDataModel.OverallStats.egress;
                ws.Cells[rowIdx, 8].Value = this._ReportDataModel.OverallStats.PaymentCount;
                

                // Column 4 is numeric float
                ApplyNumberStyleToColumn(ws, 4, rowIdx, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                // Column 5 is numeric float
                ApplyNumberStyleToColumn(ws, 5, rowIdx, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                // Column 6 is numeric integer
                ApplyNumberStyleToColumn(ws, 6, rowIdx, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                // Column 7 is numeric integer
                ApplyNumberStyleToColumn(ws, 7, rowIdx, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                // Column 8 is numeric integer
                ApplyNumberStyleToColumn(ws, 8, rowIdx, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                // Column 8 is numeric integer
                ApplyNumberStyleToColumn(ws, 8, rowIdx, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                // And now lets size the columns
                for (int autoSizeColIdx = 1; autoSizeColIdx <= 8; autoSizeColIdx++)
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
                    ws = pck.Workbook.Worksheets.Add("Meter Compliance");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Meter #";
                    ws.Cells[rowIdx, 2].Value = "Occupied Duration";
                    ws.Cells[rowIdx, 3].Value = "Occupied and Paid Duration";
                    ws.Cells[rowIdx, 4].Value = "Max Possible Occupied Duration";
                    ws.Cells[rowIdx, 5].Value = "Occupied and Paid %";
                    ws.Cells[rowIdx, 6].Value = "Occupied and Not Paid %";
                    ws.Cells[rowIdx, 7].Value = "Vehicle Arrivals";
                    ws.Cells[rowIdx, 8].Value = "Vehicle Departures";
                    ws.Cells[rowIdx, 9].Value = "Payment Count";

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
                            col.Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
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

                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = meterStat.MeterID;
                        ws.Cells[rowIdx, 2].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.TotalOccupancyTime);
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.TotalOccupancyPaidTime);
                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(meterStat.MaximumPotentialOccupancyTime);
                        ws.Cells[rowIdx, 5].Value = meterStat.PercentageOccupiedPaid;
                        ws.Cells[rowIdx, 6].Value = meterStat.PercentageOccupiedNotPaid;
                        ws.Cells[rowIdx, 7].Value = meterStat.ingress;
                        ws.Cells[rowIdx, 8].Value = meterStat.egress;
                        ws.Cells[rowIdx, 9].Value = meterStat.PaymentCount;

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 9])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Meter ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 5 is numeric float
                    ApplyNumberStyleToColumn(ws, 5, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 6 is numeric float
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 6 is numeric integer
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 7 is numeric integer
                    ApplyNumberStyleToColumn(ws, 8, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 8 is numeric integer
                    ApplyNumberStyleToColumn(ws, 9, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 9; autoSizeColIdx++)
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
                    ws = pck.Workbook.Worksheets.Add("Space Compliance");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Space #";
                    ws.Cells[rowIdx, 2].Value = "Meter #";
                    ws.Cells[rowIdx, 3].Value = "Occupied Duration";
                    ws.Cells[rowIdx, 4].Value = "Occupied and Paid Duration";
                    ws.Cells[rowIdx, 5].Value = "Max Possible Occupied Duration";
                    ws.Cells[rowIdx, 6].Value = "Occupied and Paid %";
                    ws.Cells[rowIdx, 7].Value = "Occupied and Not Paid %";
                    ws.Cells[rowIdx, 8].Value = "Vehicle Arrivals";
                    ws.Cells[rowIdx, 9].Value = "Vehicle Departures";
                    ws.Cells[rowIdx, 10].Value = "Payment Count";

                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 10])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    foreach (SensingAndPaymentStatistic_BayAndMeter bayStat in this._ReportDataModel.BayStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = bayStat.BayID;
                        ws.Cells[rowIdx, 2].Value = bayStat.MeterID;
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.TotalOccupancyTime);
                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.TotalOccupancyPaidTime);
                        ws.Cells[rowIdx, 5].Value = FormatTimeSpanAsHoursMinutesAndSeconds(bayStat.MaximumPotentialOccupancyTime);
                        ws.Cells[rowIdx, 6].Value = bayStat.PercentageOccupiedPaid;
                        ws.Cells[rowIdx, 7].Value = bayStat.PercentageOccupiedNotPaid;
                        ws.Cells[rowIdx, 8].Value = bayStat.ingress;
                        ws.Cells[rowIdx, 9].Value = bayStat.egress;
                        ws.Cells[rowIdx, 10].Value = bayStat.PaymentCount;

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 10])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 is numeric integer (Bay ID)
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 2 is numeric integer (Meter ID)
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                    // Column 6 is numeric float
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 7 is numeric float
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 8 is numeric integer
                    ApplyNumberStyleToColumn(ws, 8, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 9 is numeric integer
                    ApplyNumberStyleToColumn(ws, 9, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // Column 10 is numeric integer
                    ApplyNumberStyleToColumn(ws, 10, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= 10; autoSizeColIdx++)
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
                    ws = pck.Workbook.Worksheets.Add("Area Compliance");

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.Cells[rowIdx, 1].Value = "Area";
                    ws.Cells[rowIdx, 2].Value = "Occupied Duration";
                    ws.Cells[rowIdx, 3].Value = "Occupied and Paid Duration";
                    ws.Cells[rowIdx, 4].Value = "Max Possible Occupied Duration";
                    ws.Cells[rowIdx, 5].Value = "Occupied and Paid %";
                    ws.Cells[rowIdx, 6].Value = "Occupied and Not Paid %";
                    ws.Cells[rowIdx, 7].Value = "Vehicle Arrivals";
                    ws.Cells[rowIdx, 8].Value = "Vehicle Departures";
                    ws.Cells[rowIdx, 9].Value = "Payment Count";

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

                    foreach (SensingAndPaymentStatistic_Area areaStat in this._ReportDataModel.AreaStats)
                    {
                        // Output row values for data
                        ws.Cells[rowIdx, 1].Value = areaStat.AreaName;
                        ws.Cells[rowIdx, 2].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalOccupancyTime);
                        ws.Cells[rowIdx, 3].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.TotalOccupancyPaidTime);
                        ws.Cells[rowIdx, 4].Value = FormatTimeSpanAsHoursMinutesAndSeconds(areaStat.MaximumPotentialOccupancyTime);
                        ws.Cells[rowIdx, 5].Value = areaStat.PercentageOccupiedPaid;
                        ws.Cells[rowIdx, 6].Value = areaStat.PercentageOccupiedNotPaid;
                        ws.Cells[rowIdx, 7].Value = areaStat.ingress;
                        ws.Cells[rowIdx, 8].Value = areaStat.egress;
                        ws.Cells[rowIdx, 9].Value = areaStat.PaymentCount;

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, 9])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 5 is numeric float
                    ApplyNumberStyleToColumn(ws, 5, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 6 is numeric float
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "###0.0", ExcelHorizontalAlignment.Right);

                    // Column 7 is numeric integer
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "########0", ExcelHorizontalAlignment.Right);

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
                        }
                    }
                }


                // All cells in spreadsheet are populated now, so render (save the file) to a memory stream 
                byte[] bytes = pck.GetAsByteArray();
                ms.Write(bytes, 0, bytes.Length);
            }

            // Stop diagnostics timer
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Compliance Report Generation took: " + sw.Elapsed.ToString());
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

            // Gather all applicable payment data (minimizes how many individual SQL queries must be exectured)
            List<PaymentRecord> RawPaymentDataForAllMeters =  new PaymentDatabaseSource(this._CustomerConfig).GetHistoricalPaymentDataForMeters_StronglyTyped(
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
                    AreaStatsObj = new SensingAndPaymentStatistic_Area(null);
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
                this._ReportDataModel.OverallStats.TotalOccupancyNotPaidTime += meterStat.TotalOccupancyNotPaidTime;
            }
            this._ReportDataModel.OverallStats.AggregateSelf();

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

                        // Only accumulate as potential time if its inside the regulated hours
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

                            // Is occupancy flag currently set?
                            if (IsOccupied)
                            {
                                // Update occupied duration, but only if timeslot is in regulated hours
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
                                }

                                // Update paid duration, but only if timeslot is in regulated hours
                                if (IsTimestampAllowedForActivityRestrictionInEffect(NextReportObjTimeStamp, meterId, bayNumber))
                                    currHourlyStatObj.TotalPaidTime += elpased;

                                // Is it also occupied?
                                if (IsOccupied)
                                {
                                    // Update occupied with payment durations, but only if timeslot is in regulated hours
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

                            // Does the raw data indicate the space is occupied?
                            if (currSensing.IsOccupied == true)
                            {
                                // Increase ingress count only if we detected a change from vacant to occupied, but only if timeslot is in regulated hours
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