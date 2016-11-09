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

    public class AssetListing_Space
    {
        public int MeterID { get; set; }
        public int AreaID { get; set; }
        public int SpaceID { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public AssetListing_Space()
        {
        }
    }

    #endregion

    #region Report statistic sorters 
    public sealed class AssetListing_LogicalComparer : System.Collections.Generic.IComparer<AssetListing_Space>
    {
        private static readonly System.Collections.Generic.IComparer<AssetListing_Space> _default = new AssetListing_LogicalComparer();

        public AssetListing_LogicalComparer()
        {
        }

        public static System.Collections.Generic.IComparer<AssetListing_Space> Default
        {
            get { return _default; }
        }

        public int Compare(AssetListing_Space s1, AssetListing_Space s2)
        {
            // We are sorting by ID, but there are multiple type of objects that could be passed to us
            int s1_ID = (s1 as AssetListing_Space).MeterID;
            int s2_ID = (s2 as AssetListing_Space).MeterID;
            return s1_ID.CompareTo(s2_ID);
        }
    }
    #endregion

    #region Report data model
    public class AssetListing_ReportData
    {
        public List<AssetListing_Space> SpaceDetailsList = new List<AssetListing_Space>();
        
        public AssetListing_ReportData()
        {
        }
    }
    #endregion

    public class AssetListingReportEngine
    {
        public enum ActivityRestrictions
        {
            AllActivity
        }

        #region Private/Protected Members
        protected CustomerConfig _CustomerConfig = null;
        protected ActivityRestrictions _ActivityRestriction = ActivityRestrictions.AllActivity;

        protected AssetListing_ReportData _ReportDataModel = new AssetListing_ReportData();
        #endregion

        #region Public Methods
        public AssetListingReportEngine(CustomerConfig customerCfg)
        {
            _CustomerConfig = customerCfg;
        }

        public void GetReportAsExcelSpreadsheet(List<int> listOfMeterIDs, MemoryStream ms,
            ActivityRestrictions activityRestriction, string scopedAreaName, string scopedMeter)
        {
            // Start diagnostics timer
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            DateTime NowAtDestination = Convert.ToDateTime(this._CustomerConfig.DestinationTimeZoneDisplayName);
            this._ActivityRestriction = activityRestriction;
            this.GatherReportData(listOfMeterIDs);

            OfficeOpenXml.ExcelWorksheet ws = null;
            int rowIdx = -1;

            using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
            {
                // Let's create a report coversheet and overall summary page, with hyperlinks to the other worksheets
                // Create the worksheet
                ws = pck.Workbook.Worksheets.Add("Summary");
                
                // Render the header row
                rowIdx = 1; // Excel uses 1-based indexes
                ws.Cells[rowIdx, 1].Value = "Asset Listings Report";
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
                AddRichTextNameAndValue(rtfCollection, "Included Activity: ", "Asset Listing");
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
               
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[2, 1, rowIdx, 10])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;   
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);        
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 221, 237));  //Set color to lighter blue FromArgb(184, 204, 228)
                }


                rowIdx++;
                int hyperlinkstartRowIdx = rowIdx;

                rowIdx++;
                rowIdx++;

                using (OfficeOpenXml.ExcelRange rng = ws.Cells[hyperlinkstartRowIdx, 1, rowIdx, 13])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                }
               
                // Render the header row
                rowIdx = 7; // Excel uses 1-based indexes

                // have to start at column 2, does not work when start column is 1. Will come back when more time is avail
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 2, rowIdx, 6])
                {                    
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Merge = true; //Merge columns start and end range
                    rng.Style.Font.Bold = true;

                }
                ws.Cells[rowIdx, 2].Value = "Site Details";

                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 1, rowIdx, 6])
                {
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(23, 55, 93));
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
      
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[rowIdx, 7, rowIdx, 13])
                {
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Merge = true; //Merge columns start and end range
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 221, 237)); 
                }
                ws.Cells[rowIdx, 7].Value = "Regulations";

                rowIdx++;

                ws.Cells[rowIdx, 1].Value = "Meter ID";
                ws.Cells[rowIdx, 2].Value = "Space ID";
                ws.Cells[rowIdx, 3].Value = "Area #";
                ws.Cells[rowIdx, 4].Value = "Site Details Area";
                ws.Cells[rowIdx, 5].Value = "Co-Ordinates Lat";
                ws.Cells[rowIdx, 6].Value = "Co-Ordinates Long";
                ws.Cells[rowIdx, 7].Value = "Regulations - Sun";
                ws.Cells[rowIdx, 8].Value = "Regulations - Mon";
                ws.Cells[rowIdx, 9].Value = "Regulations - Tues";
                ws.Cells[rowIdx, 10].Value = "Regulations - Wed";
                ws.Cells[rowIdx, 11].Value = "Regulations - Thurs";
                ws.Cells[rowIdx, 12].Value = "Regulations - Fri";
                ws.Cells[rowIdx, 13].Value = "Regulations - Sat";
                
                // Format the header row
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[1, 1, 1, 6])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;                 //Set Pattern for the background to Solid
                    rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));  //Set color to dark blue
                    rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Increment the row index, which will now be the 1st row of our data
                rowIdx++;

                foreach (AssetListing_Space meterStat in this._ReportDataModel.SpaceDetailsList)
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

                    // Output row values for data
                    ws.Cells[rowIdx, 1].Value = meterStat.MeterID;
                    ws.Cells[rowIdx, 2].Value = meterStat.SpaceID;
                    ws.Cells[rowIdx, 3].Value = meterStat.AreaID;
                    ws.Cells[rowIdx, 4].Value = meterStat.Location;
                    ws.Cells[rowIdx, 5].Value = meterStat.Latitude;
                    ws.Cells[rowIdx, 6].Value = meterStat.Longitude;
                    RegulatedHoursGroupRepository.Repository = new RegulatedHoursGroupRepository();
                    RegulatedHoursGroup regulatedHours = RegulatedHoursGroupRepository.Repository.GetBestGroupForMeter(this._CustomerConfig.CustomerId,
                        meterStat.AreaID, meterStat.MeterID);

                    // If no regulated hour defintions came back, then we will default to assumption that regulated period is 24-hours a day
                    if ((regulatedHours == null) || (regulatedHours.Details == null) || (regulatedHours.Details.Count == 0))
                    {
                        rowIdx++;
                        continue;
                    }

                    // Loop through the daily rules and see if the timestamp falls within a Regulated or No Parking timeslot for the appropriate day
                    foreach (RegulatedHoursDetail detail in regulatedHours.Details)
                    {
                        string regulationTxt =
                            detail.StartTime.ToString("hh:mm:ss tt") + " - " +
                            detail.EndTime.ToString("hh:mm:ss tt") + ", " +
                            detail.MaxStayMinutes.ToString() + " mins";

                        if (string.Compare(detail.Type, "Unregulated", true) == 0)
                            regulationTxt = "(Unregulated) " +
                            detail.StartTime.ToString("hh:mm:ss tt") + " - " +
                            detail.EndTime.ToString("hh:mm:ss tt") + ", " +
                            detail.MaxStayMinutes.ToString() + " mins";
                        else if (string.Compare(detail.Type, "No Parking", true) == 0)
                            regulationTxt = "(No Parking) " +
                            detail.StartTime.ToString("hh:mm:ss tt") + " - " +
                            detail.EndTime.ToString("hh:mm:ss tt");
                        else if (detail.MaxStayMinutes < 1)
                            regulationTxt = "(No Limit) " +
                            detail.StartTime.ToString("hh:mm:ss tt") + " - " +
                            detail.EndTime.ToString("hh:mm:ss tt");

                        // Determine which column of the spreadsheet is used for this day of the week
                        int columnIdxForDayOfWeek = 7 + detail.DayOfWeek;

                        // If the cell is empty, just add the regulation text.  If something is already there, append the regulation text 
                        // (There might be more than one regulated period for the same day)
                        if ((ws.Cells[rowIdx, columnIdxForDayOfWeek].Value == null) || ((ws.Cells[rowIdx, columnIdxForDayOfWeek].Value as string) == null))
                            ws.Cells[rowIdx, columnIdxForDayOfWeek].Value = regulationTxt;
                        else
                        {
                            ws.Cells[rowIdx, columnIdxForDayOfWeek].Value = (ws.Cells[rowIdx, columnIdxForDayOfWeek].Value as string) + Environment.NewLine + regulationTxt;

                            // And increment the row height also
                            ws.Row(rowIdx).Height = ws.Row(rowIdx).Height + ws.DefaultRowHeight;
                            ws.Cells[rowIdx, columnIdxForDayOfWeek].Style.WrapText = true;

                            using (OfficeOpenXml.ExcelRange rowrange = ws.Cells[rowIdx, 1, rowIdx, 14])
                            {
                                rowrange.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                            }
                        
                        }
                    }

                    // Increment the row index, which will now be the next row of our data
                    rowIdx++;
                }

                // We will add autofilters to our headers so user can sort the columns easier
                using (OfficeOpenXml.ExcelRange rng = ws.Cells[8, 1, 8, 13])
                {
                    rng.AutoFilter = true;
                }
                    
                // Column 1 is numeric integer (Meter ID)
                ApplyNumberStyleToColumn(ws, 1, 2, rowIdx, "########0", ExcelHorizontalAlignment.Left);

                // And now lets size the columns
                for (int autoSizeColIdx = 1; autoSizeColIdx <= 13; autoSizeColIdx++)
                {
                    using (OfficeOpenXml.ExcelRange col = ws.Cells[1, autoSizeColIdx, rowIdx, autoSizeColIdx])
                    {
                        col.AutoFitColumns();
                    }
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
        protected void GatherReportData(List<int> listOfMeterIDs)
        {
            this._ReportDataModel = new AssetListing_ReportData();

            // Gather all applicable vehicle sensing data (minimizes how many individual SQL queries must be executed)
            List<AssetListingInformation> assetInfo = new AssetListingDatabaseSource(_CustomerConfig).GetAssetListingData_StronglyTyped(
                this._CustomerConfig.CustomerId, listOfMeterIDs, true);

            foreach (AssetListingInformation asset in assetInfo)
            {
                AssetListing_Space SpaceAssetDetailObj = new AssetListing_Space();
                SpaceAssetDetailObj.MeterID = asset.MeterID;
                SpaceAssetDetailObj.AreaID = asset.AreaID;
                SpaceAssetDetailObj.SpaceID = asset.BayNumber;
                SpaceAssetDetailObj.Location = asset.Location;
                SpaceAssetDetailObj.Latitude = asset.Latitude;
                SpaceAssetDetailObj.Longitude = asset.Longitude;
                this._ReportDataModel.SpaceDetailsList.Add(SpaceAssetDetailObj);
            }

            // Sort this._ReportDataModel.SpaceDetailsList by MeterID so it renders in Excel in a nice sort order
            //this._ReportDataModel.SpaceDetailsList.Sort(new AssetListing_LogicalComparer());
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

        protected void ApplyNumberStyleToColumn(ExcelWorksheet ws, int colIdx, int rowStartIdx, int rowEndIdx, string numberFormat, ExcelHorizontalAlignment horzAlign)
        {
            using (OfficeOpenXml.ExcelRange col = ws.Cells[rowStartIdx, colIdx, rowEndIdx, colIdx])
            {
                col.Style.Numberformat.Format = numberFormat;
                col.Style.HorizontalAlignment = horzAlign;
            }
        }

        #endregion
    }
}