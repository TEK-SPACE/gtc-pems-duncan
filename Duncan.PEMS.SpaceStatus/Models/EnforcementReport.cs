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

    public class EnforcementReport : BaseSensorReport
    {
        public EnforcementReport(CustomerConfig customerCfg, SensorAndPaymentReportEngine.CommonReportParameters reportParams)
            : base(customerCfg, reportParams)
        {
            _ReportName = "Enforcement Report";
            numberOfHourlyCategories = 12;

            if (this._ReportParams.IncludeHourlyStatistics == false)
                numberOfHourlyCategories = 0;
        }

        public void GetReportAsExcelSpreadsheet(List<int> listOfMeterIDs, MemoryStream ms, CustomerLogic result)
        {
            timeIsolation.IsolationType = SensorAndPaymentReportEngine.TimeIsolations.None;

            // Start diagnostics timer
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);

            // Now gather and analyze data for the report
            SensorAndPaymentReportEngine.RequiredDataElements requiredDataElements = new SensorAndPaymentReportEngine.RequiredDataElements();
            requiredDataElements.NeedsSensorData = true;
            requiredDataElements.NeedsPaymentData = true;
            requiredDataElements.NeedsOverstayData = true;
            requiredDataElements.NeedsEnforcementActionData = true;

            this._ReportEngine = new SensorAndPaymentReportEngine(this._CustomerConfig, this._ReportParams);
            this._ReportEngine.GatherReportData(listOfMeterIDs, requiredDataElements,result);

            OfficeOpenXml.ExcelWorksheet ws = null;

            using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
            {
                // Let's create a report coversheet and overall summary page, with hyperlinks to the other worksheets
                ws = pck.Workbook.Worksheets.Add("Summary");

                // Render the standard report title lines
                rowIdx = 1; // Excel uses 1-based indexes
                colIdx = 1;
                RenderCommonReportTitle(ws, this._ReportName);

                // Render common report header for enforcement activity restriction filter, but only if its not for all activity
                if (this._ReportParams.ActionTakenRestrictionFilter != SensorAndPaymentReportEngine.ReportableEnforcementActivity.AllActivity)
                {
                    rowIdx++;
                    colIdx = 1;
                    RenderCommonReportFilterHeader_ActionTakenRestrictions(ws);
                }

                // Render common report header for regulated hour restriction filter
                rowIdx++;
                colIdx = 1;
                RenderCommonReportFilterHeader_RegulatedHourRestrictions(ws);

                using (OfficeOpenXml.ExcelRange rng = ws.Cells[2, 1, rowIdx, numColumnsMergedForHeader])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 221, 237));  //Set color to lighter blue FromArgb(184, 204, 228)
                }

                rowIdx++;
                colIdx = 1;
                int hyperlinkstartRowIdx = rowIdx;

                if (_ReportParams.IncludeMeterSummary == true)
                    RenderWorksheetHyperlink(ws, "Meter Enforcement", "Meter Enforcement summary");
                if (_ReportParams.IncludeSpaceSummary == true)
                    RenderWorksheetHyperlink(ws, "Space Enforcement", "Space Enforcement summary");
                if (_ReportParams.IncludeAreaSummary == true)
                    RenderWorksheetHyperlink(ws, "Area Enforcement", "Area Enforcement summary");
                if (_ReportParams.IncludeDailySummary == true)
                    RenderWorksheetHyperlink(ws, "Daily Enforcement", "Daily Enforcement summary");
                if (_ReportParams.IncludeMonthlySummary == true)
                    RenderWorksheetHyperlink(ws, "Monthly Enforcement", "Monthly Enforcement summary");
                if (_ReportParams.IncludeDetailRecords == true)
                    RenderWorksheetHyperlink(ws, "Details", "Enforcement details");

                rowIdx++;
                rowIdx++;
                colIdx = 1;

                using (OfficeOpenXml.ExcelRange rng = ws.Cells[hyperlinkstartRowIdx, 1, rowIdx, numColumnsMergedForHeader])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }

                // Now start the report data summary header
                RenderOverallReportSummary(ws);

                //  --- END OF OVERALL SUMMARY WORKSHEET ---

                // Should we include a worksheet with Meter aggregates?
                if (_ReportParams.IncludeMeterSummary == true)
                {
                    RenderMeterSummaryWorksheet(pck, "Meter Enforcement");
                }

                // Should we include a worksheet with Space aggregates?
                if (_ReportParams.IncludeSpaceSummary == true)
                {
                    RenderSpaceSummaryWorksheet(pck, "Space Enforcement");
                }

                // Should we include a worksheet with Area aggregates?
                if (_ReportParams.IncludeAreaSummary == true)
                {
                    RenderAreaSummaryWorksheet(pck, "Area Enforcement");
                }

                // Should we include a worksheet with Daily aggregates?
                if (_ReportParams.IncludeDailySummary == true)
                {
                    RenderDailySummaryWorksheet(pck, "Daily Enforcement");
                }

                // Should we include a worksheet with Monthly aggregates?
                if (_ReportParams.IncludeDailySummary == true)
                {
                    RenderMonthlySummaryWorksheet(pck, "Monthly Enforcement");
                }



                // Should we include a Details worksheet?
                if (_ReportParams.IncludeDetailRecords == true)
                {
                    // Create the worksheet
                    ws = pck.Workbook.Worksheets.Add("Details");
                    int detailColumnCount = 18;

                    // Render the header row
                    rowIdx = 1; // Excel uses 1-based indexes
                    ws.SetValue(rowIdx, 1, "Space #");
                    ws.SetValue(rowIdx, 2, "Meter #");
                    ws.SetValue(rowIdx, 3, "Area #");
                    ws.SetValue(rowIdx, 4, "Area");
                    ws.SetValue(rowIdx, 5, "Arrival");
                    ws.SetValue(rowIdx, 6, "Departure");

                    ws.SetValue(rowIdx, 7, "Start of" + Environment.NewLine + "Overstay Violation");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 7);
                    ws.SetValue(rowIdx, 8, "Overstay  Violation" + Environment.NewLine + "Duration");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 8);
                    ws.SetValue(rowIdx, 9, "Overstay Violation" + Environment.NewLine + "Action Taken");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 9);
                    ws.SetValue(rowIdx, 10, "Overstay Violation" + Environment.NewLine + "Action Taken Timestamp");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 10);

                    ws.SetValue(rowIdx, 11, "Overstay Rule");

                    ws.SetValue(rowIdx, 12, "Payment Timestamp");
                    ws.SetValue(rowIdx, 13, "Payment Expiration");
                    ws.SetValue(rowIdx, 14, "Payment Zeroed-out" + Environment.NewLine + "Timestamp");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 14);

                    ws.SetValue(rowIdx, 15, "Start of" + Environment.NewLine + "Payment Violation");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 15);
                    ws.SetValue(rowIdx, 16, "Payment Violation" + Environment.NewLine + "Duration");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 16);
                    ws.SetValue(rowIdx, 17, "Payment Violation" + Environment.NewLine + "Action Taken");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 17);
                    ws.SetValue(rowIdx, 18, "Payment Violation" + Environment.NewLine + "Action Taken Timestamp");
                    ApplyWrapTextStyleToCell(ws, rowIdx, 18);

                    Dictionary<int, List<string>> ColumnLinesForRow = new Dictionary<int, List<string>>();
                    
                    // Format the header row
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, detailColumnCount])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }
                   
                    // Increment the row index, which will now be the 1st row of our data
                    rowIdx++;

                    #region Populate data for each record

                    foreach (SensorAndPaymentReportEngine.CommonSensorAndPaymentEvent repEvent in this._ReportEngine.ReportDataModel.ReportableEvents)
                    {
                        // Ignore unoccupied sections or dummy sensor events
                        if (repEvent.SensorEvent_IsOccupied == false)
                            continue;
                        if (repEvent.IsDummySensorEvent == true)
                            continue;

                        // The details only need to list records that are involved in either payment or overstay violations (unenforceable sensor and payment events can be ignored)
                        if ((repEvent.PaymentVios.Count == 0) && (repEvent.Overstays.Count == 0))
                            continue;

                        // Start with fresh collections for each column's text lines for current row
                        for (int nextCol = 1; nextCol <= detailColumnCount; nextCol++)
                        {
                            ColumnLinesForRow[nextCol] = new List<string>();
                        }

                        AreaAsset areaAsset = _ReportEngine.GetAreaAsset(repEvent.BayInfo.AreaID_PreferLibertyBeforeInternal);

                            // Output row values for data
                            ColumnLinesForRow[1].Add(repEvent.BayInfo.SpaceID.ToString());
                            ColumnLinesForRow[2].Add(repEvent.BayInfo.MeterID.ToString());
                            if (areaAsset != null)
                            {
                                ColumnLinesForRow[3].Add(areaAsset.AreaID.ToString());
                                ColumnLinesForRow[4].Add(areaAsset.AreaName);
                            }
                            ColumnLinesForRow[5].Add(repEvent.SensorEvent_Start.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            ColumnLinesForRow[6].Add(repEvent.SensorEvent_End.ToString("yyyy-MM-dd hh:mm:ss tt"));

                            // Add sensor ins/outs for each "repeat" sensor event
                            foreach (SensorAndPaymentReportEngine.CommonSensorAndPaymentEvent repeatEvent in repEvent.RepeatSensorEvents)
                            {
                                ColumnLinesForRow[5].Add(repEvent.SensorEvent_Start.ToString("yyyy-MM-dd hh:mm:ss tt"));
                                ColumnLinesForRow[6].Add(repEvent.SensorEvent_End.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            }

                        foreach (SensorAndPaymentReportEngine.OverstayVioEvent overstay in repEvent.Overstays)
                        {
                            ColumnLinesForRow[7].Add(overstay.StartOfOverstayViolation.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            ColumnLinesForRow[8].Add(FormatTimeSpanAsHoursMinutesAndSeconds(overstay.DurationOfTimeBeyondStayLimits));

                            if (!string.IsNullOrEmpty(overstay.EnforcementActionTaken))
                                ColumnLinesForRow[9].Add(overstay.EnforcementActionTaken);
                            else
                                ColumnLinesForRow[9].Add("");

                            if (overstay.EnforcementActionTakenTimeStamp > DateTime.MinValue)
                                ColumnLinesForRow[10].Add(overstay.EnforcementActionTakenTimeStamp.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            else
                                ColumnLinesForRow[10].Add("");

                            if (overstay.OverstayBasedOnRuleDetail != null)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append(Enum.ToObject(typeof(DayOfWeek), overstay.OverstayBasedOnRuleDetail.DayOfWeek).ToString() + " ");
                                sb.Append(overstay.OverstayBasedOnRuleDetail.StartTime.ToString("hh:mm:ss tt") + " - " +
                                        overstay.OverstayBasedOnRuleDetail.EndTime.ToString("hh:mm:ss tt") + ", ");
                                sb.Append(overstay.OverstayBasedOnRuleDetail.Type + ", Max Stay: " + overstay.OverstayBasedOnRuleDetail.MaxStayMinutes.ToString());
                                
                                ColumnLinesForRow[11].Add(sb.ToString());
                            }
                            else
                            {
                                ColumnLinesForRow[11].Add("");
                            }
                        }

                        foreach (SensorAndPaymentReportEngine.PaymentEvent payEvent in repEvent.PaymentEvents)
                        {
                            if (payEvent.PaymentEvent_IsPaid == false)
                                continue;

                            ColumnLinesForRow[12].Add(payEvent.PaymentEvent_Start .ToString("yyyy-MM-dd hh:mm:ss tt"));
                            if (payEvent.WasStoppedShortViaZeroOutTrans == true)
                            {
                                ColumnLinesForRow[13].Add(payEvent.OriginalPaymentEvent_End.ToString("yyyy-MM-dd hh:mm:ss tt"));
                                ColumnLinesForRow[14].Add(payEvent.PaymentEvent_End.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            }
                            else
                            {
                                ColumnLinesForRow[13].Add(payEvent.PaymentEvent_End.ToString("yyyy-MM-dd hh:mm:ss tt"));
                                ColumnLinesForRow[14].Add("");
                           }
                        }

                        foreach (SensorAndPaymentReportEngine.PaymentVioEvent payVio in repEvent.PaymentVios)
                        {
                            ColumnLinesForRow[15].Add(payVio.StartOfPayViolation.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            ColumnLinesForRow[16].Add(FormatTimeSpanAsHoursMinutesAndSeconds(payVio.DurationOfTimeInViolation));

                            if (!string.IsNullOrEmpty(payVio.EnforcementActionTaken))
                                ColumnLinesForRow[17].Add(payVio.EnforcementActionTaken);
                            else
                                ColumnLinesForRow[17].Add("");

                            if (payVio.EnforcementActionTakenTimeStamp > DateTime.MinValue)
                                ColumnLinesForRow[18].Add(payVio.EnforcementActionTakenTimeStamp.ToString("yyyy-MM-dd hh:mm:ss tt"));
                            else
                                ColumnLinesForRow[18].Add("");
                        }

                        int linesForRow = 1;
                        for (int nextCol = 1; nextCol <= detailColumnCount; nextCol++)
                        {
                            int columnRowLines = 0;
                            StringBuilder sb = new StringBuilder();
                            bool firstLine = true;
                            foreach (string nextLine in ColumnLinesForRow[nextCol])
                            {
                                columnRowLines++;
                                if (firstLine == false)
                                {
                                    sb.AppendLine();
                                }
                                sb.Append(nextLine);
                                firstLine = false;
                            }
                            ws.SetValue(rowIdx, nextCol, sb.ToString());

                            if (columnRowLines > linesForRow)
                                linesForRow = columnRowLines;

                            if (columnRowLines > 1)
                            {
                                using (OfficeOpenXml.ExcelRange rowrange = ws.Cells[rowIdx, nextCol])
                                {
                                    ws.Cells[rowIdx, nextCol].Style.WrapText = true;
                                }
                            }
                        }

                        // Do we need to resize the row?
                        if (linesForRow > 1)
                        {
                            ws.Row(rowIdx).Height = (ws.DefaultRowHeight * linesForRow);
                            using (OfficeOpenXml.ExcelRange rowrange = ws.Cells[rowIdx, 1, rowIdx, detailColumnCount])
                            {
                                rowrange.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            }
                        }

                        // Increment the row index, which will now be the next row of our data
                        rowIdx++;
                    }
                    #endregion

                    // We will add autofilters to our headers so user can sort the columns easier
                    using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, rowIdx, detailColumnCount])
                    {
                        rng.AutoFilter = true;
                    }

                    // Apply formatting to the columns as appropriate (Starting row is 2 (first row of data), and ending row is the current rowIdx value)

                    // Column 1 & 2 are numeric integer
                    ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 2, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 3, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 4, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 5, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 6, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 7, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 8, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 9, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 10, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 11, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 12, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 13, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 14, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 15, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 16, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 17, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);
                    ApplyNumberStyleToColumn(ws, 18, 2, rowIdx, "@", ExcelHorizontalAlignment.Left);

                    // And now lets size the columns
                    for (int autoSizeColIdx = 1; autoSizeColIdx <= detailColumnCount; autoSizeColIdx++)
                    {
                        using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                        {
                            col.AutoFitColumns();
                        }
                    }

                    // And now finally we must manually size the columns that have wrap text (autofit doesn't work nicely when we have wrap text)
                    ws.Column(1 + 6).Width = 24;
                    ws.Column(1 + 7).Width = 24;
                    ws.Column(1 + 8).Width = 24;
                    ws.Column(1 + 9).Width = 27;

                    ws.Column(1 + 13).Width = 24;
                    ws.Column(1 + 14).Width = 24;
                    ws.Column(1 + 15).Width = 24;
                    ws.Column(1 + 16).Width = 24;
                    ws.Column(1 + 17).Width = 27;
                }

                // All cells in spreadsheet are populated now, so render (save the file) to a memory stream 
                byte[] bytes = pck.GetAsByteArray();
                ms.Write(bytes, 0, bytes.Length);
            }

            // Stop diagnostics timer
            sw.Stop();
            System.Diagnostics.Debug.WriteLine(this._ReportName + " generation took: " + sw.Elapsed.ToString());
        }

        protected override void RenderCommonHeader(ExcelWorksheet ws, int startRowIdx, int startColIdx, ref int colIdx_HourlyCategory)
        {
            int overallSummaryHeaderRowIdx = startRowIdx;
            int renderRowIdx = startRowIdx;
            int renderColIdx = startColIdx;
            int colIdx_1stCommonColumn = startColIdx;

            renderColIdx = colIdx_1stCommonColumn;
            ws.SetValue(renderRowIdx, renderColIdx, "Overstay" + Environment.NewLine + "Violations"); // Column 1
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Payment" + Environment.NewLine + "Violations"); // Column 2
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total" + Environment.NewLine + "Violations"); // Column 3
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total" + Environment.NewLine + "Actioned"); // Column 4
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total" + Environment.NewLine + "Enforced"); // Column 5
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total" + Environment.NewLine + "Cautioned"); // Column 6
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total" + Environment.NewLine + "Not Enforced"); // Column 7
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total" + Environment.NewLine + "Faulty"); // Column 8
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total Actioned" + Environment.NewLine + "< 15 Minutes"); // Column 9
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total Actioned" + Environment.NewLine + "15 - 40 Minutes"); // Column 10
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total Actioned" + Environment.NewLine + "> 40 Minutes"); // Column 11
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, "Total Missed" + Environment.NewLine + "(Unactioned)"); // Column 12
            ApplyWrapTextStyleToCell(ws, renderRowIdx, renderColIdx);
            ws.Column(renderColIdx).Width = 20;


            // Format current portion of the header row
            using (OfficeOpenXml.ExcelRange rng = ws.Cells[renderRowIdx, colIdx_1stCommonColumn, renderRowIdx, renderColIdx])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                renderColIdx++;
                colIdx_HourlyCategory = renderColIdx; // Retain this column index as the known column index for hourly category
                ws.SetValue(renderRowIdx, renderColIdx, "Hourly Category");  // Column 13
                ws.Column(renderColIdx).Width = 35;

                // Format hourly category portion of the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[renderRowIdx, renderColIdx, renderRowIdx, renderColIdx])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.OliveDrab);
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }

                renderColIdx++; // Column 15 is start of hourly items (Midnight hour)
                DateTime tempHourlyTime = DateTime.Today;
                DateTime tempHourlyTime2 = DateTime.Today.AddHours(1);
                for (int hourlyIdx = 0; hourlyIdx < 24; hourlyIdx++)
                {
                    ws.SetValue(renderRowIdx, renderColIdx + hourlyIdx, tempHourlyTime.ToString("h ") + "-" + tempHourlyTime2.ToString(" h tt").ToLower());
                    tempHourlyTime = tempHourlyTime.AddHours(1);
                    tempHourlyTime2 = tempHourlyTime2.AddHours(1);
                    ws.Column(renderColIdx + hourlyIdx).Width = 14;
                }

                // Format hourly portion of the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[renderRowIdx, renderColIdx, renderRowIdx, renderColIdx + 23])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkSlateBlue);
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }
            }
        }

        protected override void RenderCommonData(ExcelWorksheet ws, int startRowIdx, int startColIdx, ref int colIdx_HourlyCategory, GroupStats statsObj)
        {
            int colIdx_1stCommonColumn = startColIdx;
            int renderColIdx = colIdx_1stCommonColumn;
            int renderRowIdx = startRowIdx;

            ws.SetValue(renderRowIdx, renderColIdx, statsObj.OverstayCount);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.PayVioCount);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.OverstayCount + statsObj.PayVioCount);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalOverstaysActioned + statsObj.TotalPayViosActioned);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalOverstaysEnforced + statsObj.TotalPayViosEnforced);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalOverstaysCautioned + statsObj.TotalPayViosCautioned);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalOverstaysNotEnforced + statsObj.TotalPayViosNotEnforced);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalOverstaysFaulty + statsObj.TotalPayViosFaulty);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalViosActioned0To15Mins);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalViosActioned15To40Mins);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, statsObj.TotalViosActionedOver40Mins);

            renderColIdx++;
            ws.SetValue(renderRowIdx, renderColIdx, (statsObj.OverstayCount + statsObj.PayVioCount) - (statsObj.TotalOverstaysActioned + statsObj.TotalPayViosActioned));


            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 0, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 1, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 2, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 3, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 4, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 5, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 6, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 7, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 8, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 9, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 10, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);
            ApplyNumberStyleToColumn(ws, colIdx_1stCommonColumn + 11, renderRowIdx, renderRowIdx, "########0", ExcelHorizontalAlignment.Right);

            // And now lets autosize the columns
            for (int autoSizeColIdx = colIdx_1stCommonColumn; autoSizeColIdx <= renderColIdx; autoSizeColIdx++)
            {
                using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, renderRowIdx, autoSizeColIdx])
                {
                    col.AutoFitColumns();
                    col.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                }
            }

            // And now finally we must manually size the columns that have wrap text (autofit doesn't work nicely when we have wrap text)
            ws.Column(colIdx_1stCommonColumn + 0).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 1).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 2).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 3).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 4).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 5).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 6).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 8).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 9).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 10).Width = 20;
            ws.Column(colIdx_1stCommonColumn + 11).Width = 20;

            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                // Now we will construct the hourly category column, followed by hour detail columns

                ws.SetValue(renderRowIdx + 0, colIdx_HourlyCategory, "Overstay Violations");
                ws.SetValue(renderRowIdx + 1, colIdx_HourlyCategory, "Payment Violations");
                ws.SetValue(renderRowIdx + 2, colIdx_HourlyCategory, "Total Violations");
                ws.SetValue(renderRowIdx + 3, colIdx_HourlyCategory, "Total Actioned");
                ws.SetValue(renderRowIdx + 4, colIdx_HourlyCategory, "Total Enforced");
                ws.SetValue(renderRowIdx + 5, colIdx_HourlyCategory, "Total Cautioned");
                ws.SetValue(renderRowIdx + 6, colIdx_HourlyCategory, "Total Not Enforced");
                ws.SetValue(renderRowIdx + 7, colIdx_HourlyCategory, "Total Faulty");
                ws.SetValue(renderRowIdx + 8, colIdx_HourlyCategory, "Actioned < 15 mins");
                ws.SetValue(renderRowIdx + 9, colIdx_HourlyCategory, "Actioned 15 - 40 mins");
                ws.SetValue(renderRowIdx + 10, colIdx_HourlyCategory, "Actioned > 40 mins");
                ws.SetValue(renderRowIdx + 11, colIdx_HourlyCategory, "Total Missed");

                using (OfficeOpenXml.ExcelRange col = ws.Cells[renderRowIdx, colIdx_HourlyCategory, renderRowIdx + (numberOfHourlyCategories - 1), colIdx_HourlyCategory])
                {
                    col.Style.Font.Bold = true;
                }

                MergeCellRange(ws, renderRowIdx + 1, 1, renderRowIdx + (numberOfHourlyCategories - 1), colIdx_HourlyCategory - 1);
            }
        }

        protected override void RenderCommonHourlyData(ExcelWorksheet ws, int startRowIdx, ref int colIdx_HourlyCategory, int hourIdx, GroupStats hourlyStats)
        {
            if (this._ReportParams.IncludeHourlyStatistics == true)
            {
                int renderRowIdx = startRowIdx;
                int renderColIdx = colIdx_HourlyCategory + 1 + hourIdx;

                // Output and format hourly data cells
                ws.SetValue(renderRowIdx + 0, renderColIdx, hourlyStats.OverstayCount);
                ApplyNumberStyleToCell(ws, renderRowIdx + 0, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 1, renderColIdx, hourlyStats.PayVioCount);
                ApplyNumberStyleToCell(ws, renderRowIdx + 1, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 2, renderColIdx, hourlyStats.OverstayCount + hourlyStats.PayVioCount);
                ApplyNumberStyleToCell(ws, renderRowIdx + 2, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 3, renderColIdx, hourlyStats.TotalOverstaysActioned + hourlyStats.TotalPayViosActioned);
                ApplyNumberStyleToCell(ws, renderRowIdx + 3, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 4, renderColIdx, hourlyStats.TotalOverstaysEnforced + hourlyStats.TotalPayViosEnforced);
                ApplyNumberStyleToCell(ws, renderRowIdx + 4, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 5, renderColIdx, hourlyStats.TotalOverstaysCautioned + hourlyStats.TotalPayViosCautioned);
                ApplyNumberStyleToCell(ws, renderRowIdx + 5, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 6, renderColIdx, hourlyStats.TotalOverstaysNotEnforced + hourlyStats.TotalPayViosNotEnforced);
                ApplyNumberStyleToCell(ws, renderRowIdx + 6, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 7, renderColIdx, hourlyStats.TotalOverstaysFaulty + hourlyStats.TotalPayViosFaulty);
                ApplyNumberStyleToCell(ws, renderRowIdx + 7, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 8, renderColIdx, hourlyStats.TotalViosActioned0To15Mins);
                ApplyNumberStyleToCell(ws, renderRowIdx + 8, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 9, renderColIdx, hourlyStats.TotalViosActioned15To40Mins);
                ApplyNumberStyleToCell(ws, renderRowIdx + 9, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 10, renderColIdx, hourlyStats.TotalViosActionedOver40Mins);
                ApplyNumberStyleToCell(ws, renderRowIdx + 10, renderColIdx, "########0", ExcelHorizontalAlignment.Left);

                ws.SetValue(renderRowIdx + 11, renderColIdx, (hourlyStats.OverstayCount + hourlyStats.PayVioCount) - (hourlyStats.TotalOverstaysActioned + hourlyStats.TotalPayViosActioned));
                ApplyNumberStyleToCell(ws, renderRowIdx + 11, renderColIdx, "########0", ExcelHorizontalAlignment.Left);
            }
        }
    }

}