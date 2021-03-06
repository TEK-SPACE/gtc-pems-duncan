﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Mvc;

using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;

namespace Duncan.PEMS.SpaceStatus.Helpers
{
    public static class MeterStatusRenderer
    {
        public enum RenderStyle { Default, OneLine, TwoLine, ThreeLine };

        // Strongly type input instead of dataset
        public static string RenderMeters(List<SpaceStatusModel> modelForView, CustomerConfig customerCfg, ViewDataDictionary ViewData)
        {
            // Default the render style, then try to get the correct one from ViewData
            RenderStyle renderStyle = RenderStyle.Default;
            try
            {
                string selectedDisplayStyle = ViewData["DisplayStyle"].ToString();
                if (string.Compare(selectedDisplayStyle, "D", true) == 0)
                    renderStyle = RenderStyle.Default;
                else if (string.Compare(selectedDisplayStyle, "1L", true) == 0)
                    renderStyle = RenderStyle.OneLine;
                else if (string.Compare(selectedDisplayStyle, "2L", true) == 0)
                    renderStyle = RenderStyle.TwoLine;
                else if (string.Compare(selectedDisplayStyle, "3L", true) == 0)
                    renderStyle = RenderStyle.ThreeLine;
            }
            catch { }

            bool showSensorTimestamps = false;
            try
            {
                showSensorTimestamps = (ViewData["OptionIncludeSensorTimestamp"].ToString() == Convert.ToInt32(true).ToString());
            }
            catch { }

            bool showLastMeterCommTimestamp = false;

            StringBuilder sb = new StringBuilder();

            // Build list of unique meters -- which will be our grouping
            List<int> uniqueMeterIDs = new List<int>();
            foreach (SpaceStatusModel nextSpaceModel in modelForView)
            {
                if (uniqueMeterIDs.IndexOf(nextSpaceModel.MeterID) == -1)
                    uniqueMeterIDs.Add(nextSpaceModel.MeterID);
            }

            // Loop for each meter
            foreach (int nextMeterID in uniqueMeterIDs)
            {
                // Build list of indexes inside modelForView that are for the current meter
                List<int> itemIndexesForCurrentMeter = new List<int>();
                for (int loIdx = 0; loIdx < modelForView.Count; loIdx++)
                {
                    if (modelForView[loIdx].MeterID == nextMeterID)
                        itemIndexesForCurrentMeter.Add(loIdx);
                }

                SpaceStatusModel currentSpaceStatusModel = modelForView[itemIndexesForCurrentMeter[0]];

                sb.AppendLine("<div class=\"Rounded Meter\"> ");
                sb.AppendLine("<div class=\"MtrTitle\">Meter " + currentSpaceStatusModel.MeterID.ToString());

                if (showLastMeterCommTimestamp == true)
                {
                    if (currentSpaceStatusModel.Meter_upTS != DateTime.MinValue)
                    {
                        sb.AppendLine("<span class=\"MtrSubTitle\"> [Idle For: " + currentSpaceStatusModel.Meter_imin.ToString() + " minutes]  [Last Updated: " +
                            currentSpaceStatusModel.Meter_upTS.ToString() + "]"); // DEBUG: We need formatted string like:  row["upTSString"].ToString() 
                    }
                }
                else
                {
                    if (currentSpaceStatusModel.Meter_upTS != DateTime.MinValue)
                    {
                        sb.AppendLine("<span class=\"MtrSubTitle\"> [Idle: " + currentSpaceStatusModel.Meter_imin.ToString() + " minutes]");
                    }
                }
                sb.AppendLine("</span> ");
                sb.AppendLine("</div> ");

                // Loop through each bay of current meter
                for (int loIdx = 0; loIdx < itemIndexesForCurrentMeter.Count; loIdx++)
                {
                    currentSpaceStatusModel = modelForView[itemIndexesForCurrentMeter[loIdx]];
                    string detailTarget = "m" + nextMeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();
                    sb.AppendLine("<div class=\"Bay\" style=\"cursor: pointer;\" onclick=\"showDetails(event, '" + detailTarget + "');\" >");

                    if (renderStyle == RenderStyle.OneLine)
                        sb.AppendLine("<div class=\"InlineBayTitle\">" + currentSpaceStatusModel.BayID.ToString() + "</div> ");
                    else
                        sb.AppendLine("<div class=\"BayTitle\">" + currentSpaceStatusModel.BayID.ToString() + "</div> ");

                    // Determine the payment expiration state
                    Duncan.PEMS.SpaceStatus.Models.ExpiryState es = currentSpaceStatusModel.BayExpiryState;

                    string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);

                    string commonPayElementClass = "CIB SH"; // Centered inline block, same height
                    if (renderStyle == RenderStyle.ThreeLine)
                        commonPayElementClass = "CIB SH FW"; // Centered inline block, same height, full width
                    if (renderStyle == RenderStyle.Default)
                        commonPayElementClass = "CIB SH FW"; // Centered inline block, same height, full width

                    // Output appropriate HTML elements for the expiration state
                    if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Safe)
                    {
                        sb.AppendLine("<div class=\"" + commonPayElementClass + " PaySafe\">" + ExpiryTimeString + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Expired)
                    {
                        sb.AppendLine("<div class=\"" + commonPayElementClass + " PayExpired\">" + ExpiryTimeString + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Critical)
                    {
                        sb.AppendLine("<div class=\"" + commonPayElementClass + " PayCritical\">" + ExpiryTimeString + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Grace)
                    {
                        sb.AppendLine("<div class=\"" + commonPayElementClass + " PayGracePeriod\">" + ExpiryTimeString + "</div> ");
                    }
                    else
                    {
                        sb.AppendLine("<div class=\"" + commonPayElementClass + " PayInoperational\">" + ExpiryTimeString + "</div> ");
                    }

                    /////////////////////////////

                    // Get the vehicle occupancy state
                    Duncan.PEMS.SpaceStatus.Models.OccupancyState os = currentSpaceStatusModel.BayOccupancyState;

                    string commonVSElementClass = "CB";
                    if (renderStyle == RenderStyle.Default)
                    {
                        sb.AppendLine("<div class=\"B VSSectionVert\">"); // VSSection vertical block
                        commonVSElementClass = "CB";
                    }
                    else if (renderStyle == RenderStyle.OneLine)
                    {
                        sb.AppendLine("<div class=\"IB VSSectionHorz\">"); // VSSection horizontal inline-block
                        commonVSElementClass = "CIB";
                    }
                    else if (renderStyle == RenderStyle.TwoLine)
                    {
                        sb.AppendLine("<div class=\"IB VSSectionHorz\">"); // VSSection horizontal inline-block
                        commonVSElementClass = "CIB";
                    }
                    else if (renderStyle == RenderStyle.ThreeLine)
                    {
                        sb.AppendLine("<div class=\"B VSSectionHorz\">"); // VSSection horizontal block
                        commonVSElementClass = "CIB";
                    }


                    // Output appropriate HTML elements for the occupancy state
                    string TimeSinceLastInOut = BindShortTimeSpan(currentSpaceStatusModel.TimeSinceLastInOut, currentSpaceStatusModel.BayOccupancyState, customerCfg);
                    string LastInOutTime = BindTimeOfDay(currentSpaceStatusModel.BayVehicleSensingTimestamp, currentSpaceStatusModel.BayOccupancyState, customerCfg);


                    switch (os)
                    {
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Empty:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSEmpty\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOut\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDay\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeNA\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDayNA\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Occupied:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSOccupied\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeIn\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDay\">" + LastInOutTime + "</div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSOutOfDate\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeNA\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDayNA\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Violation:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " Violation\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeIn\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDay\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.MeterFeeding:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " MeterFeeding\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeIn\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDay\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Unknown:
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeNA\">" + TimeSinceLastInOut + "</div>");
                            if (showSensorTimestamps == true)
                                sb.AppendLine("<div class=\"" + commonVSElementClass + " VSTimeOfDayNA\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                    }

                    sb.AppendLine("</div> "); // End of VSSection

                    sb.AppendLine("</div> "); // End of Bay block
                }

                sb.AppendLine("</div> "); // End of Meter block
            }

            // Finalize the result from the string builder contents
            return sb.ToString();
        }

        public static string RenderSpaceDetails(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();
            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            // Summary header
            #region Header Area
            // Output column headers
            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("div"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbTitleBlock_ForColumn.MergeAttribute("style", "display:block; font-size:12px; line-height:12px; background-color:rgb(239, 243, 251); color:Black; width:240px;");

            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Meter");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw3 colCellMP");

            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Space");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw3 colCellMP");

            sb.AppendLine(tbTitleBlock_ForColumn.ToString(TagRenderMode.Normal));

            // We need a "clearfix" element to do a line-break
            tbTitleBlock_ForColumn = new BaseTagHelper("div");
            tbTitleBlock_ForColumn.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");
            sb.AppendLine(tbTitleBlock_ForColumn.ToString(TagRenderMode.Normal));
            #endregion

            // Summary row
            BaseTagHelper tbTable = new BaseTagHelper("div");
            tbTable.AddCssClass("dtNoLinesOrGaps mb0");
            tbTable.MergeAttribute("style", "display:block; color:white; cursor: auto; min-width:200px; min-height:25px; font-size:1px; line-height:1px; ");

            BaseTagHelper tbGroupSection = new BaseTagHelper("span");
            tbTable.Children.Add(tbGroupSection);
            tbGroupSection.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbGroupSection.MergeAttribute("style", "float:left; color:white; cursor:auto; font-size:1px; line-height:1px;");

            string targetID = string.Empty;
            targetID = "m" + dataModel[0].MeterID.ToString() + "s" + dataModel[0].BayID.ToString();

            // Create a tag to be used as a column in the group row -- child of the group row
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            tbGroupCol.AddCssClass("dtcLeft Bold hcenter vcenter siw3 colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
            // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
            MeterAsset mtrAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[0].MeterID);
            tbGroupCol.SetInnerText(dataModel[0].MeterID.ToString());

            // Column #2 of group row
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            tbGroupCol.AddCssClass("dtcRight Bold hcenter vcenter siw3 colCellMP grayCell"); //grayCell Display as table-cell, both horizontally and vertically centered
            tbGroupCol.SetInnerText(dataModel[0].BayID.ToString());
            // Determine if we will consider this an active violation (current or discretionary, but not already actioned)
            bool isEnforceable = false;
            if (dataModel[0].BayEnforcementState == EnforcementState.MeterViolation)
                isEnforceable = true;

            if ((dataModel[0].BayEnforcementState == EnforcementState.OverstayViolation) || (dataModel[0].BayEnforcementState == EnforcementState.Discretionary))
            {
                isEnforceable = true;
            }

            TimeSpan VioDuration = new TimeSpan(0);
            if (isEnforceable == true)
            {
                if (dataModel[0].CurrentOverstayViolation != null)
                    VioDuration = dataModel[0].CurrentOverstayViolation.DurationOfTimeBeyondStayLimits;
                else if (dataModel[0].AllOverstayViolations.Count > 0)
                    VioDuration = dataModel[0].AllOverstayViolations[dataModel[0].AllOverstayViolations.Count - 1].DurationOfTimeBeyondStayLimits;
            }



            if ((dataModel[0].BayExpiryState == ExpiryState.Safe) || (dataModel[0].BayExpiryState == ExpiryState.Grace) || (dataModel[0].BayExpiryState == ExpiryState.Critical))
            {
                tbGroupCol = new BaseTagHelper("span");
                tbGroupSection.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("CIB Bold hcenter vcenter whiteCell PaidIcon");
                tbGroupCol.MergeAttribute("style", "margin-left:8px; margin-top:2px;");
                tbGroupCol.SetInnerText("");
            }
            else if (dataModel[0].BayExpiryState == ExpiryState.Expired)
            {
                tbGroupCol = new BaseTagHelper("span");
                tbGroupSection.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("CIB Bold hcenter vcenter whiteCell ExpiredIcon");
                tbGroupCol.MergeAttribute("style", "margin-left:8px; margin-top:2px;");
                tbGroupCol.SetInnerText("");
            }
            
            Duncan.PEMS.SpaceStatus.Models.OccupancyState os = dataModel[0].BayOccupancyState;
            string vsIconClass = string.Empty;
            switch (os)
            {
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Empty:
                    vsIconClass = "VSEmpty";
                    break;
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable:
                    vsIconClass = "VSNotAvailable";
                    break;
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Occupied:
                    vsIconClass = "VSOccupied";
                    break;
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate:
                    vsIconClass = "VSOutOfDate";
                    break;
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Violation:
                    vsIconClass = "Violation";
                    break;
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.MeterFeeding:
                    vsIconClass = "MeterFeeding";
                    break;
                case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Unknown:
                    vsIconClass = "VSNotAvailable";
                    break;
            }
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            //tbGroupCol.AddCssClass("dtcRight Bold hcenter vcenter siw3 colCellMP whiteCell " + vsIconClass);
            tbGroupCol.AddCssClass("CIB Bold hcenter vcenter whiteCell " + vsIconClass);
            tbGroupCol.MergeAttribute("style", "margin-left:8px; margin-top:2px;");
            tbGroupCol.SetInnerText("");


            /*
            // Column #3 of group row
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            if (VioDuration.TotalSeconds > 0)
            {
                // Space is occupied
                tbGroupCol.SetInnerText(FormatElapsedAsMinutes(VioDuration));
                if (VioDuration.TotalMinutes < 15)
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP orangeCell notBold"); // horizontally and vertically centered 
                else
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP redCell notBold"); // horizontally and vertically centered 
            }
            else
            {
                tbGroupCol.SetInnerHtml("&nbsp;");
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP grayCell notBold"); // horizontally and vertically centered
            }
            */

            /*
            // Column #4 of group row
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            if (isEnforceable == true)
            {
                tbGroupCol.SetInnerText("Action");
                if (dataModel[0].BayEnforcementState == EnforcementState.Discretionary)
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw3 colCellMP orangeCell Bold underline"); // horizontally and vertically centered
                else
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw3 colCellMP redCell Bold underline"); // horizontally and vertically centered
            }
            else
            {
                tbGroupCol.SetInnerText("Action");
                tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw3 colCellMP greenCell Bold underline"); // horizontally and vertically centered
            }
            tbGroupCol.MergeAttribute("onclick", "actionForSpecificTarget(\"" + targetID + "\");");
            tbGroupCol.MergeAttribute("style", "cursor: pointer;");
            */

            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));





            // Output body of all normal data

            tbTable = new BaseTagHelper("span");  // div
            tbTable.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbTable.MergeAttribute("style", "color:white; cursor: auto; min-width:200px; width:240px; max-width:240px; min-height:50px; font-size:1px; line-height:1px;"); //height: 158px; 

            // Loop for each space
            tbGroupCol = null;
            foreach (SpaceStatusModel currentSpaceStatusModel in dataModel)
            {
                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);

                // Space/Meter identity
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                tbGroupCol.SetInnerText("Space:");
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.MeterID.ToString());
                else
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.MeterID.ToString() + "-" + currentSpaceStatusModel.BayID.ToString());


                if (currentSpaceStatusModel.Meter_upTS != DateTime.MinValue)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Last Updated:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.Meter_upTS.ToShortTimeString());

                    DateTime TodayAtDestination = Convert.ToDateTime(customerCfg.DestinationTimeZoneDisplayName);// UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, customerCfg.ServerTimeZone, customerCfg.CustomerTimeZone).Date;
                    DateTime DateOfEvent = currentSpaceStatusModel.Meter_upTS.Date;
                    string dateDescriptionPfx = "";
                    if (DateOfEvent.Ticks == TodayAtDestination.Ticks)
                        dateDescriptionPfx = "(Today) ";
                    else if (DateOfEvent.Ticks == TodayAtDestination.AddDays(-1).Ticks)
                        dateDescriptionPfx = "(Yesterday) ";
                    else if (DateOfEvent.Ticks < TodayAtDestination.AddDays(-1).Ticks)
                    {
                        int NumOfDays = (new TimeSpan(TodayAtDestination.AddDays(-1).Ticks - DateOfEvent.Ticks)).Days;
                        dateDescriptionPfx = "(" + NumOfDays.ToString() + " days ago) ";
                    }

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText(dateDescriptionPfx + currentSpaceStatusModel.Meter_upTS.ToString("ddd, MMM d"));
                    //tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Last Updated:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail");
                    tbGroupCol.SetInnerText("N/A");
                }

                // Payment status information
//                if (currentSpaceStatusModel.IsSensorOnly == false)
                {
                    string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);
                    if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                        tbGroupCol.SetInnerText("Expired Duration:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP redCell_Detail underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                        tbGroupCol.SetInnerText("Will Expire In:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP orangeCell_Detail underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                        tbGroupCol.SetInnerText("Remaining Time:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP greenCell_Detail underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                        tbGroupCol.SetInnerText("Remaining Grace:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP orangeCell_Detail underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                        tbGroupCol.SetInnerText("Payment Status:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                        tbGroupCol.SetInnerText("Unknown");
                    }
                }


                // Previous "Action Taken", if applicable
                /*
                if (!string.IsNullOrEmpty(currentSpaceStatusModel.EnforcementActionTaken))
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Action Taken:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.EnforcementActionTaken);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP orangeCell_Detail underlineCell");
                }
                */


                // Enforcement Status
                /*
                if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.AlreadyCited)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Already Cited");
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP greenCell_Detail underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Good)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP greenCell_Detail underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP redCell_Detail underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP redCell_Detail underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Discretionary");
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP orangeCell_Detail underlineCell");


                    // Output reason for discretionary violation...
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.MergeAttribute("style", "width:75px; font-size:12px;  line-height:12px; display:block;  margin:0px; margin-left:2px; float:left; white-space:nowrap;");
                    tbGroupCol.AddCssClass("txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Reason:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.MergeAttribute("style", "width:159px; font-size:12px;  line-height:12px; height: 12px; display:block;  margin:0px; float:left; white-space:nowrap; background-color:yellow; color:Black; overflow:hidden;");
                    tbGroupCol.AddCssClass("txtR vcenter colCellMP");
                    tbGroupCol.SetInnerText("Overstay in prior period");

                    if (overstayInfo != null)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");

                        string regulationTxt = ((DayOfWeek)(overstayInfo.Regulation_DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                            overstayInfo.Regulation_StartTime.ToString("hh:mm:ss tt") + " - " +
                            overstayInfo.Regulation_EndTime.ToString("hh:mm:ss tt") + ", " +
                            overstayInfo.Regulation_MaxStayMinutes.ToString() + " mins";

                        if (string.Compare(overstayInfo.Regulation_Type, "Unregulated", true) == 0)
                            regulationTxt = "(Unregulated) " + regulationTxt;
                        else if (string.Compare(overstayInfo.Regulation_Type, "No Parking", true) == 0)
                            regulationTxt = "(No Parking) " + regulationTxt;
                        else if (overstayInfo.Regulation_MaxStayMinutes < 1)
                            regulationTxt = "(No Limit) " + regulationTxt;

                        tbGroupCol.SetInnerText(regulationTxt);
                        tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                    }
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Unknown)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP greenCell_Detail underlineCell");
                }
                */

                // Current overstay regulations
                /*
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                tbGroupCol.SetInnerText("Current Regulations:");

                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                if (currentSpaceStatusModel.ActiveRegulationPeriod != null)
                {
                    string regulationTxt = ((DayOfWeek)(currentSpaceStatusModel.ActiveRegulationPeriod.DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.StartTime.ToString("hh:mm:ss tt") + " - " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.EndTime.ToString("hh:mm:ss tt") + ", " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.MaxStayMinutes.ToString() + " mins";

                    if (string.Compare(currentSpaceStatusModel.ActiveRegulationPeriod.Type, "Unregulated", true) == 0)
                        regulationTxt = "(Unregulated) " + regulationTxt;
                    else if (string.Compare(currentSpaceStatusModel.ActiveRegulationPeriod.Type, "No Parking", true) == 0)
                        regulationTxt = "(No Parking) " + regulationTxt;
                    else if (currentSpaceStatusModel.ActiveRegulationPeriod.MaxStayMinutes < 1)
                        regulationTxt = "(No Limit) " + regulationTxt;

                    tbGroupCol.SetInnerText(regulationTxt);
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupCol.SetInnerText("None");
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                */

                // Vehicle sensor information
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding))
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail");
                    tbGroupCol.SetInnerText("Occupied");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtR vcenter colCellMP NavyTextCell_Detail underlineCell italic");
                    tbGroupCol.SetInnerHtml("Duration:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Arrival:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                    DateTime TodayAtDestination = Convert.ToDateTime(customerCfg.DestinationTimeZoneDisplayName);// UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, customerCfg.ServerTimeZone, customerCfg.CustomerTimeZone).Date;
                    DateTime DateOfEvent = currentSpaceStatusModel.BayVehicleSensingTimestamp.Date;
                    string dateDescriptionPfx = "";
                    if (DateOfEvent.Ticks == TodayAtDestination.Ticks)
                        dateDescriptionPfx = "(Today) ";
                    else if (DateOfEvent.Ticks == TodayAtDestination.AddDays(-1).Ticks)
                        dateDescriptionPfx = "(Yesterday) ";
                    else if (DateOfEvent.Ticks < TodayAtDestination.AddDays(-1).Ticks)
                    {
                        int NumOfDays = (new TimeSpan(TodayAtDestination.AddDays(-1).Ticks - DateOfEvent.Ticks)).Days;
                        dateDescriptionPfx = "(" + NumOfDays.ToString() + " days ago) ";
                    }

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText(dateDescriptionPfx + currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("ddd, MMM d"));
                    //tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail");
                    tbGroupCol.SetInnerText("Vacant");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtR vcenter colCellMP NavyTextCell_Detail underlineCell italic");
                    tbGroupCol.SetInnerHtml("Duration:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Departure:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                    DateTime TodayAtDestination = Convert.ToDateTime(customerCfg.DestinationTimeZoneDisplayName);// UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, customerCfg.ServerTimeZone, customerCfg.CustomerTimeZone).Date;
                    DateTime DateOfEvent = currentSpaceStatusModel.BayVehicleSensingTimestamp.Date;
                    string dateDescriptionPfx = "";
                    if (DateOfEvent.Ticks == TodayAtDestination.Ticks)
                        dateDescriptionPfx = "(Today) ";
                    else if (DateOfEvent.Ticks == TodayAtDestination.AddDays(-1).Ticks)
                        dateDescriptionPfx = "(Yesterday) ";
                    else if (DateOfEvent.Ticks < TodayAtDestination.AddDays(-1).Ticks)
                    {
                        int NumOfDays = (new TimeSpan(TodayAtDestination.AddDays(-1).Ticks - DateOfEvent.Ticks)).Days;
                        dateDescriptionPfx = "(" + NumOfDays.ToString() + " days ago) ";
                    }

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText(dateDescriptionPfx + currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("ddd, MMM d"));
                    //tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Unknown");
                }

                // Violated overstay regulations
                /*
                if (((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) || (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                    && (overstayInfo != null))
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Vio Duration:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");

                    VioDuration = new TimeSpan(0);
                    if (overstayInfo != null)
                        VioDuration = overstayInfo.DurationOfTimeBeyondStayLimits;

                    tbGroupCol.SetInnerText(FormatMobileElapsed(VioDuration));

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc_Detail txtL vcenter colCellMP NavyTextCell_Detail");
                    tbGroupCol.SetInnerText("Violated Regulation:");

                    if (overstayInfo != null)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");

                        string regulationTxt = ((DayOfWeek)(overstayInfo.Regulation_DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                            overstayInfo.Regulation_StartTime.ToString("hh:mm:ss tt") + " - " +
                            overstayInfo.Regulation_EndTime.ToString("hh:mm:ss tt") + ", " +
                            overstayInfo.Regulation_MaxStayMinutes.ToString() + " mins";

                        if (string.Compare(overstayInfo.Regulation_Type, "Unregulated", true) == 0)
                            regulationTxt = "(Unregulated) " + regulationTxt;
                        else if (string.Compare(overstayInfo.Regulation_Type, "No Parking", true) == 0)
                            regulationTxt = "(No Parking) " + regulationTxt;
                        else if (overstayInfo.Regulation_MaxStayMinutes < 1)
                            regulationTxt = "(No Limit) " + regulationTxt;

                        tbGroupCol.SetInnerText(regulationTxt);
                        tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                    }
                }
                */


/*
                if (currentSpaceStatusModel.IsSensorOnly == true)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Space Type:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Meter with Sensor");
                }
                else
*/
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft_Detail txtL vcenter colCellMP NavyTextCell_Detail underlineCell");
                    tbGroupCol.SetInnerText("Space Type:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                    try
                    {
                        if (asset != null)
                        {
                            if (asset.MeterGroupID <= 0) // SSM
                                tbGroupCol.SetInnerText("Metered (SSM)");
                            else
                                tbGroupCol.SetInnerText("Metered (MSM)");
                        }
                        else
                        {
                            tbGroupCol.SetInnerText("Metered");
                        }
                    }
                    catch
                    {
                        tbGroupCol.SetInnerText("Metered");
                    }
                }

            }

            // We need a "clearfix" element to do a line-break
            tbGroupCol = new BaseTagHelper("div");
            tbTable.Children.Add(tbGroupCol);
            tbGroupCol.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");


            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }


        public static string RenderLegend(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            // Default the render style, then try to get the correct one from ViewData
            RenderStyle renderStyle = RenderStyle.Default;
            try
            {
                string selectedDisplayStyle = ViewData["DisplayStyle"].ToString();
                if (string.Compare(selectedDisplayStyle, "D", true) == 0)
                    renderStyle = RenderStyle.Default;
                else if (string.Compare(selectedDisplayStyle, "1L", true) == 0)
                    renderStyle = RenderStyle.OneLine;
                else if (string.Compare(selectedDisplayStyle, "2L", true) == 0)
                    renderStyle = RenderStyle.TwoLine;
                else if (string.Compare(selectedDisplayStyle, "3L", true) == 0)
                    renderStyle = RenderStyle.ThreeLine;
            }
            catch { }

            bool showSensorTimestamps = false;
            try
            {
                showSensorTimestamps = (ViewData["OptionIncludeSensorTimestamp"].ToString() == Convert.ToInt32(true).ToString());
            }
            catch { }


            string tblWidth = string.Empty;
            if (renderStyle == RenderStyle.Default)
                tblWidth = " width:199px; ";

            StringBuilder sb = new StringBuilder();
            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            BaseTagHelper pnlLegend = new BaseTagHelper("div");
            pnlLegend.GenerateId("TreePane2");
            pnlLegend.MergeAttribute("style", "font-family: Arial,Verdana; font-size: 9pt;      ");  //width: 1000px;
            pnlLegend.SetInnerText("Key/Legend:");

            BaseTagHelper tbldiv = new BaseTagHelper("div");
            pnlLegend.Children.Add(tbldiv);
            tbldiv.GenerateId("tablediv2");
            tbldiv.MergeAttribute("style", "border-color:Black; border-style:outset; border-width:1px; border-spacing:1px 1px; margin-top:1px; width:auto; height:auto; display:table-cell;"); //table
           
            BaseTagHelper tbl = new BaseTagHelper("div");
            tbldiv.Children.Add(tbl);

            if ((renderStyle == RenderStyle.TwoLine) || (renderStyle == RenderStyle.ThreeLine))
            {
                tbl.MergeAttribute("style", "display:table;" + tblWidth + "border:none; margin-top:0px;   width:100%; ");
                BaseTagHelper prevtbl = tbl;
                tbl = new BaseTagHelper("div");
                prevtbl.Children.Add(tbl);
            }
            if (renderStyle == RenderStyle.OneLine) // inline-table
                tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;   height:100%;");
            else if ((renderStyle == RenderStyle.TwoLine) || (renderStyle == RenderStyle.ThreeLine)) // inline-table
                tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px;    height:100%;");
            else 
                tbl.MergeAttribute("style", "display:table;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;");

            BaseTagHelper row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");
            BaseTagHelper cell = new BaseTagHelper("div");
            row.Children.Add(cell);
            row.MergeAttribute("style", "display:table-cell;");
            cell.SetInnerHtml("&nbsp; &nbsp; <strong>Bay / Space #</strong>&nbsp;");


            tbl = new BaseTagHelper("div");
            tbldiv.Children.Add(tbl);

            if ((renderStyle == RenderStyle.ThreeLine))
            {
                tbl.MergeAttribute("style", "display:table;" + tblWidth + "border:none; margin-top:0px;   width:100%; ");
                BaseTagHelper prevtbl = tbl;
                tbl = new BaseTagHelper("div");
                prevtbl.Children.Add(tbl);
            }
            if ((renderStyle == RenderStyle.OneLine) || (renderStyle == RenderStyle.TwoLine))
                tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;    height:100%;");
            else if ((renderStyle == RenderStyle.TwoLine) || (renderStyle == RenderStyle.ThreeLine)) // inline-table
                tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px;    height:100%;");
            else
                tbl.MergeAttribute("style", "display:table;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;");
            
            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");
            cell = new BaseTagHelper("div");
            row.Children.Add(cell);
            cell.MergeAttribute("style", "display:table-cell;");
            cell.SetInnerHtml("<strong>&nbsp; &nbsp; Meter Credit or Expiration</strong>");
            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");
            cell = new BaseTagHelper("div");
            row.Children.Add(cell);
            cell.MergeAttribute("style", "display:table-cell; font-size: smaller;");
            cell.SetInnerHtml("<strong>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; (hh:mm:ss tt</strong> or <strong>Days)</strong>");
        
            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");

            BaseTagHelper innerTbl = new BaseTagHelper("div");
            row.Children.Add(innerTbl);
            innerTbl.MergeAttribute("style", "display:inline-table;    border:none;");

            BaseTagHelper innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            BaseTagHelper innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("CenteredBlock PaySafe");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("2:00");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 8px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Credit Remaining");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("CenteredBlock PayExpired");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("1D");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Expired Duration");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("CenteredBlock PayCritical");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("0:10");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Almost Expired");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("CenteredBlock PayGracePeriod");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("5:50");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Grace Until Expired");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("CenteredBlock PayInoperational");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("--");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Not Available");





            tbl = new BaseTagHelper("div");
            tbldiv.Children.Add(tbl);
            if ((renderStyle == RenderStyle.OneLine) || (renderStyle == RenderStyle.TwoLine) || (renderStyle == RenderStyle.ThreeLine))
                tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px; height:100%;");
            else
                tbl.MergeAttribute("style", "display:table;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;");
            
            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");
            cell = new BaseTagHelper("div");
            row.Children.Add(cell);
            cell.MergeAttribute("style", "display:table-cell;");
            cell.SetInnerHtml("<strong>&nbsp; &nbsp; &nbsp;&nbsp; Vehicle Status</strong>&nbsp;");
            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");

            innerTbl = new BaseTagHelper("div");
            row.Children.Add(innerTbl);
            innerTbl.MergeAttribute("style", "display:inline-table;    border:none;");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSOccupied");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Occupied");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSVacant");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Empty");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSOutOfDate");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Out of Date");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSNotAvail");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Not Available");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSViolation");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Violation");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSFeeding");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Meter Feeding");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("imgVSNotAvail");
            innerCell.MergeAttribute("style", "display:table-cell; width: 59px; background-repeat: no-repeat; height: 19px");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;  height: 19px; line-height: 19px; vertical-align:middle;   padding-left: 0px; padding-right:4px;");
            innerCell.SetInnerText("Unknown");





            tbl = new BaseTagHelper("div");
            tbldiv.Children.Add(tbl);
            if ((renderStyle == RenderStyle.OneLine) || (renderStyle == RenderStyle.TwoLine) || (renderStyle == RenderStyle.ThreeLine))
                tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px; height:100%;");
            else
                tbl.MergeAttribute("style", "display:table;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;");

            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");
            cell = new BaseTagHelper("div");
            row.Children.Add(cell);
            cell.MergeAttribute("style", "display:table-cell;");
            cell.SetInnerHtml("<strong>&nbsp; &nbsp; &nbsp; Occupancy Time</strong>");
            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");
            cell = new BaseTagHelper("div");
            row.Children.Add(cell);
            cell.MergeAttribute("style", "display:table-cell; font-size: smaller;");
            cell.SetInnerHtml("<strong>&nbsp; &nbsp; &nbsp; (hh:mm:ss tt</strong> or <strong>Days)</strong>");

            row = new BaseTagHelper("div");
            tbl.Children.Add(row);
            row.MergeAttribute("style", "display:table-row;");

            innerTbl = new BaseTagHelper("div");
            row.Children.Add(innerTbl);
            innerTbl.MergeAttribute("style", "display:inline-table;    border:none;");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("LastInOutTimeIn");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("2:00");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Occupied Duration");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("LastInOutTimeOut");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("1D");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Vacant Duration");

            innerRow = new BaseTagHelper("div");
            innerTbl.Children.Add(innerRow);
            innerRow.MergeAttribute("style", "display:table-row;");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.AddCssClass("LastInOutTimeNA");
            innerCell.MergeAttribute("style", "display:table-cell; width: 50px; font-weight: bold;");
            innerCell.SetInnerText("N/A");
            innerCell = new BaseTagHelper("div");
            innerRow.Children.Add(innerCell);
            innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
            innerCell.SetInnerText("Not Available");


            if (showSensorTimestamps == true)
            {
                tbl = new BaseTagHelper("div");
                tbldiv.Children.Add(tbl);
                if ((renderStyle == RenderStyle.OneLine) || (renderStyle == RenderStyle.TwoLine) || (renderStyle == RenderStyle.ThreeLine))
                    tbl.MergeAttribute("style", "display:table-cell;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;  height:100%;");
                else
                    tbl.MergeAttribute("style", "display:table;" + tblWidth + "border-color:Black; border-style:inset; border-width:1px; border-spacing:1px 1px; margin-top:1px;");

                row = new BaseTagHelper("div");
                tbl.Children.Add(row);
                row.MergeAttribute("style", "display:table-row;");
                cell = new BaseTagHelper("div");
                row.Children.Add(cell);
                cell.MergeAttribute("style", "display:table-cell;");
                cell.SetInnerHtml("<strong>&nbsp; &nbsp; Occupancy EventTime</strong>");
                row = new BaseTagHelper("div");
                tbl.Children.Add(row);
                row.MergeAttribute("style", "display:table-row;");
                cell = new BaseTagHelper("div");
                row.Children.Add(cell);
                cell.MergeAttribute("style", "display:table-cell; font-size: smaller;");
                cell.SetInnerHtml("<strong>&nbsp; &nbsp; &nbsp; (hh:mm:ss tt 24HR)</strong>");

                row = new BaseTagHelper("div");
                tbl.Children.Add(row);
                row.MergeAttribute("style", "display:table-row;");

                innerTbl = new BaseTagHelper("div");
                row.Children.Add(innerTbl);
                innerTbl.MergeAttribute("style", "display:inline-table;    border:none;");

                innerRow = new BaseTagHelper("div");
                innerTbl.Children.Add(innerRow);
                innerRow.MergeAttribute("style", "display:table-row;");
                innerCell = new BaseTagHelper("div");
                innerRow.Children.Add(innerCell);
                innerCell.AddCssClass("VSTimeOfDay");
                innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;");
                innerCell.SetInnerText("14:35");
                innerCell = new BaseTagHelper("div");
                innerRow.Children.Add(innerCell);
                innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
                innerCell.SetInnerText("Event DateTime");

                innerRow = new BaseTagHelper("div");
                innerTbl.Children.Add(innerRow);
                innerRow.MergeAttribute("style", "display:table-row;");
                innerCell = new BaseTagHelper("div");
                innerRow.Children.Add(innerCell);
                innerCell.AddCssClass("VSTimeOfDayNA");
                innerCell.MergeAttribute("style", "display:table-cell; font-weight: bold;");
                innerCell.SetInnerHtml("&nbsp; N/A");
                innerCell = new BaseTagHelper("div");
                innerRow.Children.Add(innerCell);
                innerCell.MergeAttribute("style", "display:table-cell; height: 17px; font-weight: bold;    padding-left: 8px; padding-right:4px;");
                innerCell.SetInnerText("Not Available");
            }


            sb.AppendLine(pnlLegend.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }

        public static string BindShortTimeSpan(TimeSpan Item, OccupancyState OccupancyStateItem, CustomerConfig customerCfg)
        {
            string ReturnValue = string.Empty;
            try
            {
                if (OccupancyStateItem == Duncan.PEMS.SpaceStatus.Models.OccupancyState.Unknown)
                    ReturnValue = "N/A";
                else if (OccupancyStateItem == Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate)
                    ReturnValue = "N/A";
                else if (OccupancyStateItem == Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable)
                    ReturnValue = "N/A";
                else
                {
                    if (Item == null)
                        ReturnValue = "N/A";
                    else
                        ReturnValue = SpaceStatusProvider.FormatShortTimeSpan(Item, customerCfg);
                }
            }
            catch
            {
                return "N/A";
            }

            return ReturnValue;
        }

        public static string BindTimeOfDay(DateTime Item, OccupancyState OccupancyStateItem, CustomerConfig customerCfg)
        {
            string ReturnValue;
            try
            {
                if (OccupancyStateItem == OccupancyState.Unknown)
                    ReturnValue = "N/A";
                else if (((OccupancyState)(OccupancyStateItem)) == OccupancyState.OutOfDate)
                    ReturnValue = "N/A";
                else if (((OccupancyState)(OccupancyStateItem)) == OccupancyState.NotAvailable)
                    ReturnValue = "N/A";
                else
                {
                    if ((Item == DateTime.MinValue) || (Item == null))
                        ReturnValue = "N/A";
                    else
                        ReturnValue = Item.ToString("hh:mm:ss tt"); // 24-hour format
                }
            }
            catch
            {
                return "N/A";
            }

            return ReturnValue;
        }

        private static string FormatMobileElapsed(TimeSpan T)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int NoDays = Math.Abs(T.Days);
            int Hours = Math.Abs(T.Hours);
            int Minutes = Math.Abs(T.Minutes);
            int Seconds = Math.Abs(T.Seconds);


            if (NoDays == 1)
            {
                if ((Hours == 0) && (Minutes == 0) && (Seconds == 0))
                    sb.Append("24 Hours");
                else
                    sb.Append(NoDays.ToString() + " day");

                return sb.ToString();
            }
            else if (NoDays > 30)
            {
                sb.Append("(> 1 month)");
                return sb.ToString();
            }
            else if (NoDays > 1)
            {
                sb.Append(NoDays.ToString() + " days");
                return sb.ToString();
            }

            sb.Append(string.Format("{0:D2}:{1:D2}:{2:D2}", Hours, Minutes, Seconds));
            return sb.ToString();
        }

        private static string FormatElapsedAsMinutes(TimeSpan T)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int Minutes = Convert.ToInt32(Math.Abs(T.TotalMinutes));

            if (Minutes <= 0)
                sb.Append("< 1 min");
            else
                sb.Append(Minutes.ToString() + " mins");

            return sb.ToString();
        }
    }
}