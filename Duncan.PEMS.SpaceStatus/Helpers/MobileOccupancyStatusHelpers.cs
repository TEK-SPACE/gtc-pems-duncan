using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Mvc;


using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;
using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.Helpers
{
    public static class MobileOccupancyStatusHelpers
    {
        public static string ListView_GroupByMeter(List<SpaceStatusModel> modelForView, CustomerConfig customerCfg)
        {
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

                int totalSpaces = 0;
                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOther = 0;
                foreach (int itemIdx in itemIndexesForCurrentMeter)
                {
                    SpaceStatusModel nextSpaceModel = modelForView[itemIdx];
                    totalSpaces++;

                    if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                        totalVacant++;
                    else if (nextSpaceModel.BayOccupancyState == OccupancyState.Occupied)
                        totalOccupied++;
                    else
                        totalOther++;
                }

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps");
                /*OuterPanel.MergeAttribute("style", "width:230px; cursor: pointer;"); //204px*/
                OuterPanel.MergeAttribute("style", "cursor: pointer; margin-bottom:2px;"); 
                string targetID = nextMeterID.ToString();
                OuterPanel.MergeAttribute("ID", "m" + targetID);

                // VirtualPathUtility.ToAbsolute() is similar to Url.Conent
                OuterPanel.MergeAttribute("onclick", "window.location.href = \"" + VirtualPathUtility.ToAbsolute("~/SpaceStatus/SpaceDrillDown") + 
                    "?targetID=" + HttpUtility.HtmlEncode(targetID) + "&viewType=" + HttpUtility.HtmlEncode("List") + "&CID=" + 
                    HttpUtility.HtmlEncode(customerCfg.CustomerId.ToString()) + "\";");

                int percent = 0;
                if (totalSpaces > 0)
                    percent = Convert.ToInt32((Convert.ToDouble(totalOccupied) / Convert.ToDouble(totalSpaces)) * 100.0f);

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr");
                if (percent >= 75)
                    tbGroupRow.MergeAttribute("style", "background-color:red; padding:4px; border: 1px solid #C0504D; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); //reddish
                else
                    tbGroupRow.MergeAttribute("style", "background-color:green; padding:4px; border: 1px solid #9BBB59; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // greenish

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell and both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;"); //tbGroupCol.MergeAttribute("style", "width:33%;");
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                tbGroupCol.SetInnerText(MeterName);

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell and both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");
                tbGroupCol.SetInnerText(totalOccupied.ToString() + "/" + totalSpaces.ToString());


                // Column #3 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell and both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");
                tbGroupCol.SetInnerText(percent.ToString() + "%");

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string ListView_GroupBySpace(List<SpaceStatusModel> modelForView, CustomerConfig customerCfg)
        {
            StringBuilder sb = new StringBuilder();

            // Build list of unique bays -- which will be our grouping
            List<int> uniqueBayIDs = new List<int>();
            foreach (SpaceStatusModel nextSpaceModel in modelForView)
            {
                if (uniqueBayIDs.IndexOf(nextSpaceModel.BayID) == -1)
                    uniqueBayIDs.Add(nextSpaceModel.BayID);
            }

            // Loop for each bay
            foreach (int nextBayID in uniqueBayIDs)
            {
                // Build list of indexes inside modelForView that are for the current meter
                List<int> itemIndexesForCurrentBay = new List<int>();
                for (int loIdx = 0; loIdx < modelForView.Count; loIdx++)
                {
                    if (modelForView[loIdx].BayID == nextBayID)
                        itemIndexesForCurrentBay.Add(loIdx);
                }

                SpaceStatusModel currentSpaceStatusModel = modelForView[itemIndexesForCurrentBay[0]];

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps");
                /*OuterPanel.MergeAttribute("style", "width:205px; background-color:yellow;");*/
                OuterPanel.MergeAttribute("style", "cursor: pointer; margin-bottom:2px;");
                string targetID = nextBayID.ToString();
                OuterPanel.MergeAttribute("ID", "s" + nextBayID.ToString());

                // VirtualPathUtility.ToAbsolute() is similar to Url.Conent
                OuterPanel.MergeAttribute("onclick", "window.location.href = \"" + VirtualPathUtility.ToAbsolute("~/SpaceStatus/MobileSpaceDetails") +
                    "?targetID=" + HttpUtility.HtmlEncode(targetID) + "&MID=" + HttpUtility.HtmlEncode(currentSpaceStatusModel.MeterID.ToString()) + "&viewType=" + HttpUtility.HtmlEncode("Details") + "&CID=" +
                    HttpUtility.HtmlEncode(customerCfg.CustomerId.ToString()) + "\";");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr");
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                {
                    tbGroupRow.MergeAttribute("style", "background-color:red; padding:4px; border: 1px solid #C0504D; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); //reddish
                }
                else
                {
                    tbGroupRow.MergeAttribute("style", "background-color:green; padding:4px; border: 1px solid #9BBB59; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // greenish
                }

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px;"); //tbGroupCol.MergeAttribute("style", "width:33%;");
                tbGroupCol.SetInnerText("Space " + currentSpaceStatusModel.BayID.ToString());

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px;");
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                {
                    tbGroupCol.SetInnerText("+ " + FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                }
                else
                {
                    tbGroupCol.SetInnerText("- " + FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                }

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string SpaceDetails(List<SpaceStatusModel> modelForView, CustomerConfig customerCfg)
        {
            StringBuilder sb = new StringBuilder();

            // Build list of unique bays -- which will be our grouping (there should only be one item though!)
            List<int> uniqueBayIDs = new List<int>();
            foreach (SpaceStatusModel nextSpaceModel in modelForView)
            {
                if (uniqueBayIDs.IndexOf(nextSpaceModel.BayID) == -1)
                    uniqueBayIDs.Add(nextSpaceModel.BayID);
            }

            // Loop for each bay
            foreach (int nextBayID in uniqueBayIDs)
            {
                // Build list of indexes inside modelForView that are for the current meter
                List<int> itemIndexesForCurrentBay = new List<int>();
                for (int loIdx = 0; loIdx < modelForView.Count; loIdx++)
                {
                    if (modelForView[loIdx].BayID == nextBayID)
                        itemIndexesForCurrentBay.Add(loIdx);
                }

                SpaceStatusModel currentSpaceStatusModel = modelForView[itemIndexesForCurrentBay[0]];

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps");
                /*OuterPanel.MergeAttribute("style", "width:205px;");*/
                OuterPanel.MergeAttribute("ID", "s" + nextBayID.ToString());

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                BaseTagHelper tbGroupCol = null;

                // Add 2 columns: A field name and field value
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Space ID:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                tbGroupCol.SetInnerText(currentSpaceStatusModel.BayID.ToString());

                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel); 
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Status:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                    tbGroupCol.SetInnerText("Occupied");
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                    tbGroupCol.SetInnerText("Vacant");
                else
                    tbGroupCol.SetInnerText("Unknown");

                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                    tbGroupCol.SetInnerText("Arrival:");
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                    tbGroupCol.SetInnerText("Departure:");
                else
                    tbGroupCol.SetInnerText("Event Time:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Duration:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));


                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }



        // Mobile "Vehicle Sensing" pages support
        public static string VSSummaryByGroups(List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            IEnumerable AssetsToGroupBy = null;
            if (groupType == "A")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg);
            }
            else if (groupType == "M")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg);
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " + 
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Loop for each group
            foreach (object nextGroupObj in AssetsToGroupBy)
            {
                // Build list of indexes inside modelForView that are associated with the current asset
                List<int> itemIndexesForCurrentGroupAsset = new List<int>();
                for (int loIdx = 0; loIdx < dataModel.Count; loIdx++)
                {
                    if (nextGroupObj is AreaAsset)
                    {
                        MeterAsset relatedMeterAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[loIdx].MeterID);
                        if (relatedMeterAsset.AreaID_PreferLibertyBeforeInternal == (nextGroupObj as AreaAsset).AreaID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                    else if (nextGroupObj is MeterAsset)
                    {
                        if (dataModel[loIdx].MeterID == (nextGroupObj as MeterAsset).MeterID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                }

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 0;
                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOther = 0;
                foreach (int itemIdx in itemIndexesForCurrentGroupAsset)
                {
                    SpaceStatusModel nextSpaceModel = dataModel[itemIdx];
                    totalSpaces++;

                    if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                    {
                        totalVacant++;
                    }
                    else if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        totalOccupied++;
                    }
                    else
                    {
                        totalOther++;
                    }
                }

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps"); // Display as table
                OuterPanel.MergeAttribute("style", "cursor: pointer; margin-bottom:2px;"); // width:230px;
                string targetID = string.Empty;

                if (groupType == "A")
                {
                    targetID = "a" + (nextGroupObj as AreaAsset).AreaID.ToString();
                    OuterPanel.MergeAttribute("ID", targetID);
                }
                else if (groupType == "M")
                {
                    targetID = "m" + (nextGroupObj as MeterAsset).MeterID.ToString();
                    OuterPanel.MergeAttribute("ID", targetID);
                }

                // VirtualPathUtility.ToAbsolute() is similar to Url.Conent
                /*
                OuterPanel.MergeAttribute("onclick", "window.location.href = \"" + VirtualPathUtility.ToAbsolute("~/SpaceStatus/mVSSpcSummary") +
                    "?T=" + HttpUtility.HtmlEncode(targetID) +
                    "&G=" + HttpUtility.HtmlEncode(groupType) +
                    "&V=" + HttpUtility.HtmlEncode(viewType) + 
                    "&CID=" + HttpUtility.HtmlEncode(customerCfg.CustomerId.ToString()) + "\";");
                */
                OuterPanel.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Get percentage of spaces that are vacant
                int percent = 0;
                int percentOther = 0;
                if (totalSpaces > 0)
                {
                    percent = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOther = Convert.ToInt32((Convert.ToDouble(totalOther) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr"); // Display as table-row

                // TODO: Different colors based on % or # or some other criteria for vacancies (or occupied).
                if (percentOther >= 33.0f)
                {
                    // Grayish color
                    tbGroupRow.MergeAttribute("style", "background-color:#484848; padding:4px; border: 2px solid #707070; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // gray
                }
                else if (percent < 33.0f)
                    tbGroupRow.MergeAttribute("style", "background-color:red; padding:4px; border: 2px solid #C0504D; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); //reddish
                else if (percent < 66.0f)
                    tbGroupRow.MergeAttribute("style", "background-color:orange; padding:4px; border: 2px solid #FFC000; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // orange variant
                else 
                    tbGroupRow.MergeAttribute("style", "background-color:green; padding:4px; border: 2px solid #9BBB59; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // greenish

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");

                if (groupType == "A")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as AreaAsset).AreaName);
                }
                else if (groupType == "M")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as MeterAsset).MeterID.ToString());
                }

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");
                tbGroupCol.SetInnerText(totalVacant.ToString() + "/" + totalSpaces.ToString());

                // Column #3 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");
                tbGroupCol.SetInnerText(percent.ToString() + "%");

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string VSSummaryBySpacesForGroup(List<SpaceStatusModel> modelForView, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            string parentTarget = ViewData["parentTarget"].ToString();

            foreach (SpaceStatusModel currentSpaceStatusModel in modelForView)
            {
                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps"); // Display as table
                OuterPanel.MergeAttribute("style", "cursor: pointer; margin-bottom:2px;");

                string targetID = string.Empty;
                targetID = "m" + currentSpaceStatusModel.MeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();
                OuterPanel.MergeAttribute("ID", targetID);

                // VirtualPathUtility.ToAbsolute() is similar to Url.Conent
                /*
                OuterPanel.MergeAttribute("onclick", "window.location.href = \"" + VirtualPathUtility.ToAbsolute("~/SpaceStatus/mVSSpcDetail") +
                    "?T=" + HttpUtility.HtmlEncode(targetID) +
                    "&PT=" + HttpUtility.HtmlEncode(parentTarget) +
                    "&G=" + HttpUtility.HtmlEncode(groupType) +
                    "&V=" + HttpUtility.HtmlEncode(viewType) +
                    "&CID=" + HttpUtility.HtmlEncode(customerCfg.CustomerId.ToString()) + "\";");
                */
                OuterPanel.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr"); // Display as table-row
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    // A purplish color
                    tbGroupRow.MergeAttribute("style", "background-color:#604A7B; padding:4px; border: 2px solid #B3A2C7; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // purplish
                }
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    tbGroupRow.MergeAttribute("style", "background-color:green; padding:4px; border: 2px solid #9BBB59; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // greenish
                }
                else
                {
                    // A grayish color
                    tbGroupRow.MergeAttribute("style", "background-color:#484848; padding:4px; border: 2px solid #707070; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // grayish
                }

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px;"); //tbGroupCol.MergeAttribute("style", "width:33%;");

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px;");
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                {
                    tbGroupCol.SetInnerText("+" + FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                }
                else
                {
                    tbGroupCol.SetInnerText("-" + FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                }

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string VSSpaceDetails(List<SpaceStatusModel> modelForView, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            foreach (SpaceStatusModel currentSpaceStatusModel in modelForView)
            {
                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                BaseTagHelper tbGroupCol = null;

                // Add 2 columns: A field name and field value
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Space ID:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-"+ currentSpaceStatusModel.BayID.ToString()); // currentSpaceStatusModel.MeterID.ToString() + "-" + currentSpaceStatusModel.BayID.ToString()

                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel); 
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Status:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                    tbGroupCol.SetInnerText("Occupied");
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                    tbGroupCol.SetInnerText("Vacant");
                else
                    tbGroupCol.SetInnerText("Unknown");

                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                    tbGroupCol.SetInnerText("Arrival:");
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                    tbGroupCol.SetInnerText("Departure:");
                else
                    tbGroupCol.SetInnerText("Event Time:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Duration:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }


        // Mobile "Enforcement"  pages support
        public static string EnfSummaryByGroups(List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            IEnumerable AssetsToGroupBy = null;
            if (groupType == "A")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg);
            }
            else if (groupType == "M")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg);
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Loop for each group
            foreach (object nextGroupObj in AssetsToGroupBy)
            {
                // Build list of indexes inside modelForView that are associated with the current asset
                List<int> itemIndexesForCurrentGroupAsset = new List<int>();
                for (int loIdx = 0; loIdx < dataModel.Count; loIdx++)
                {
                    if (nextGroupObj is AreaAsset)
                    {
                        MeterAsset relatedMeterAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[loIdx].MeterID);
                        if (relatedMeterAsset.AreaID_PreferLibertyBeforeInternal == (nextGroupObj as AreaAsset).AreaID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                    else if (nextGroupObj is MeterAsset)
                    {
                        if (dataModel[loIdx].MeterID == (nextGroupObj as MeterAsset).MeterID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                }

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 0;
                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                foreach (int itemIdx in itemIndexesForCurrentGroupAsset)
                {
                    SpaceStatusModel nextSpaceModel = dataModel[itemIdx];
                    totalSpaces++;

                    if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                    {
                        totalVacant++;
                    }
                    else if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        totalOccupied++;
                    }
                    else
                    {
                        totalOtherVS++;
                    }



                    // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                    if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace))
                            totalEnfVioSoon++;
                        else if (nextSpaceModel.BayExpiryState == ExpiryState.Expired)
                            totalEnfVio++;
                        else if ((nextSpaceModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                            (nextSpaceModel.BayEnforcementState == EnforcementState.Discretionary) ||
                            (nextSpaceModel.BayEnforcementState == EnforcementState.MeterViolation))
                        {
                            // We will only count this as a violation if no "Action Taken" has been recorded!
                            if (string.IsNullOrEmpty(nextSpaceModel.EnforcementActionTaken) == true)
                                totalEnfVio++;
                        }
                    }

                    if ((nextSpaceModel.IsSensorOnly == false) && (nextSpaceModel.BayExpiryState == ExpiryState.Inoperational))
                        totalUnknownExpiry++;
                }

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps"); // Display as table
                OuterPanel.MergeAttribute("style", "cursor: pointer; margin-bottom:2px;"); // width:230px;
                string targetID = string.Empty;

                if (groupType == "A")
                {
                    targetID = "a" + (nextGroupObj as AreaAsset).AreaID.ToString();
                    OuterPanel.MergeAttribute("ID", targetID);
                }
                else if (groupType == "M")
                {
                    targetID = "m" + (nextGroupObj as MeterAsset).MeterID.ToString();
                    OuterPanel.MergeAttribute("ID", targetID);
                }
                OuterPanel.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                int percentOtherVS = 0;
                int percentEnfVio = 0;
                int percentEnfSoon = 0;
                int percentEnfUnknown = 0;

                if (totalSpaces > 0)
                {
                    percentVacant = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr"); // Display as table-row

                // TODO: Different colors based on % or # or some other criteria for possible violations
                if (percentEnfVio >= 33.0f)
                {
                    // Red
                    tbGroupRow.MergeAttribute("style", "background-color:red; padding:4px; border: 2px solid #C0504D; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); //reddish
                }
                else if ((percentEnfVio + percentEnfSoon) >= 33.0f)
                {
                    // Orange
                    tbGroupRow.MergeAttribute("style", "background-color:orange; padding:4px; border: 2px solid #FFC000; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // orange variant
                }
                else if ((percentEnfUnknown >= 33.0f) || (percentOtherVS >= 33.0f))
                {
                    // Grayish color
                    tbGroupRow.MergeAttribute("style", "background-color:#484848; padding:4px; border: 2px solid #707070; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // gray
                }
                else
                {
                    // Green
                    tbGroupRow.MergeAttribute("style", "background-color:green; padding:4px; border: 2px solid #9BBB59; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // greenish
                }

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");

                if (groupType == "A")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as AreaAsset).AreaName);
                }
                else if (groupType == "M")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as MeterAsset).MeterID.ToString());
                }

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");
                tbGroupCol.SetInnerText(totalEnfVio.ToString() + "/" + totalSpaces.ToString());

                // Column #3 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:68px;");
                tbGroupCol.SetInnerText(percentEnfVio.ToString() + "%");

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string EnfSummaryBySpacesForGroup(List<SpaceStatusModel> modelForView, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            string parentTarget = ViewData["parentTarget"].ToString();

            foreach (SpaceStatusModel currentSpaceStatusModel in modelForView)
            {
                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGaps"); // Display as table
                OuterPanel.MergeAttribute("style", "cursor: pointer; margin-bottom:2px;");

                string targetID = string.Empty;
                targetID = "m" + currentSpaceStatusModel.MeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();
                OuterPanel.MergeAttribute("ID", targetID);
                OuterPanel.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr"); // Display as table-row

                // Is the space occupied?
                bool isEnfVio = false;
                bool isEnfVioSoon = false;
                bool isEnfUnknown = false;

                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace))
                        isEnfVioSoon = true;
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                        isEnfVio = true;
                    else if ((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                        (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary) ||
                        (currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation))
                    {
                        // We will only count this as a violation if no "Action Taken" has been recorded!
                        if (string.IsNullOrEmpty(currentSpaceStatusModel.EnforcementActionTaken) == true)
                            isEnfVio = true;
                    }
                }

                if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational)
                    isEnfUnknown = true;


                if (isEnfVio)
                {
                    // Red
                    tbGroupRow.MergeAttribute("style", "background-color:red; padding:4px; border: 2px solid #C0504D; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); //reddish
                }
                else if (isEnfVioSoon)
                {
                    // Orange
                    tbGroupRow.MergeAttribute("style", "background-color:orange; padding:4px; border: 2px solid #FFC000; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // orange variant
                }
                else if (isEnfUnknown)
                {
                    // A grayish color
                    tbGroupRow.MergeAttribute("style", "background-color:#484848; padding:4px; border: 2px solid #707070; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // grayish
                }
                else
                {
                    // Green
                    tbGroupRow.MergeAttribute("style", "background-color:green; padding:4px; border: 2px solid #9BBB59; width:100%; color: white; font-family: helvetica,arial,sans-serif; font-size: 16px; font-weight: 700;"); // greenish
                }

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px;"); //tbGroupCol.MergeAttribute("style", "width:33%;");

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px;");
                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied)
                {
                    tbGroupCol.SetInnerText("+" + FormatMobileElapsed(currentSpaceStatusModel.BayExpiration_AsTimeSpan));
                }
                else
                {
                    tbGroupCol.SetInnerText("-" + FormatMobileElapsed(currentSpaceStatusModel.BayExpiration_AsTimeSpan));
                }

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string EnfSpaceDetails(List<SpaceStatusModel> modelForView, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            foreach (SpaceStatusModel currentSpaceStatusModel in modelForView)
            {
                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                BaseTagHelper tbGroupCol = null;

                // Add 2 columns: A field name and field value
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Space ID:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName +"-" + currentSpaceStatusModel.BayID.ToString()); // currentSpaceStatusModel.MeterID.ToString() + "-" + currentSpaceStatusModel.BayID.ToString()

                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                // Enforcement Status
                if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.AlreadyCited)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Already Cited");
                    tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Good)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.MergeAttribute("style", "background-color:red; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.MergeAttribute("style", "background-color:red; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Discretionary");
                    tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Unknown)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Unknown");
                }

                // Time-limit if its an overstay or discretionary violation
                if ((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) || (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                {
                    if (overstayInfo != null)
                    {
                        tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                        tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                        tbGroupCol.SetInnerText("Time Limit (minutes):");
                        tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                        tbGroupCol.SetInnerText(overstayInfo.Regulation_MaxStayMinutes.ToString());
                        tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                    }
                }


                // Vehicle sensor information
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding))
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Occupied");
                    tbGroupCol.MergeAttribute("style", "background-color:yellow; color: black;", true);

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Occupied Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Arrival:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());
                }
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Vacant");

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Vacant Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Departure:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());
                }
                else
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Unknown");
                }

                // Payment status information
                string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);
                if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Expired Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(ExpiryTimeString);
                    tbGroupCol.MergeAttribute("style", "background-color:red; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Will Expire In:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(ExpiryTimeString);
                    tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Remaining Time:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(ExpiryTimeString);
                    tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Remaining Grace:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(ExpiryTimeString);
                    tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Payment Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Unknown");
                }


                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }



        private static BaseTagHelper AddGroupRowForFieldsAndValues(BaseTagHelper parent)
        {
            BaseTagHelper tbGroupRow = new BaseTagHelper("span");
            parent.Children.Add(tbGroupRow);
            tbGroupRow.AddCssClass("dtr");
            tbGroupRow.MergeAttribute("style", "padding:4px; width:100%; color: black; font-family: helvetica,arial,sans-serif; font-size: 14px; font-weight: 700;");
            return tbGroupRow;
        }

        private static BaseTagHelper AddLeftColumnForFieldName(BaseTagHelper parent)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            tbGroupCol.AddCssClass("dtc hcenter vcenter LCol"); //display table-cell horizontally and vertically centered
            /*tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px; text-align:left;"); //tbGroupCol.MergeAttribute("style", "width:33%;");*/
            return tbGroupCol;
        }

        private static BaseTagHelper AddRightColumnForFieldValue(BaseTagHelper parent)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            tbGroupCol.AddCssClass("dtc hcenter vcenter RCol"); //display table-cell horizontally and vertically centered
            /*tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px; text-align:right;"); //tbGroupCol.MergeAttribute("style", "width:33%;");*/
            return tbGroupCol;
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
    
    }

    public static class SpaceStatusHelpers
    {
        public static string StayVioGroupSummary(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            IEnumerable AssetsToGroupBy = null;
            int countOfGroups = 0;
            if (groupType == "A")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg);
                countOfGroups = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg).Count;
            }
            else if (groupType == "M")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg);
                countOfGroups = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg).Count;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            BaseTagHelper tbTitleBlock_WholePage = new BaseTagHelper("table"); // Regular block
            tbTitleBlock_WholePage.MergeAttribute("style", "font-size:12px; line-height:12px; background-color:rgb(239, 243, 251);");

            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("tr"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("dtNoLinesOrGaps"); // Display as table-row //pageColHead2
            tbTitleBlock_ForColumn.MergeAttribute("style", "margin-bottom:2px; font-size:12px; line-height:12px; background-color:rgb(239, 243, 251);");
            tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);

            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            if (groupType == "A")
                tbTitleCol.SetInnerText("Area");
            else if (groupType == "M")
                tbTitleCol.SetInnerText("Meter");
            tbTitleCol.MergeAttribute("style", "width:70px;");
            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Cars/Bays");
            tbTitleCol.MergeAttribute("style", "width:70px;");
            // Column #3 of title row
            tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Violations");
            tbTitleCol.MergeAttribute("style", "width:70px;");

            sb.AppendLine(tbTitleBlock_WholePage.ToString(TagRenderMode.Normal));

            BaseTagHelper tbTable = new BaseTagHelper("span");  // div
            tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidthRENAMED mb0"); // Display as table
            tbTable.MergeAttribute("style", "color:white; cursor: pointer; min-width:200px; width:224px; max-width:240px; min-height:158px; font-size:1px; line-height:1px;"); //height: 158px; 

            // Loop for each group
            foreach (object nextGroupObj in AssetsToGroupBy)
            {
                // Build list of indexes inside modelForView that are associated with the current asset
                List<int> itemIndexesForCurrentGroupAsset = new List<int>();
                for (int loIdx = 0; loIdx < dataModel.Count; loIdx++)
                {
                    if (nextGroupObj is AreaAsset)
                    {
                        MeterAsset relatedMeterAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[loIdx].MeterID);
                        if (relatedMeterAsset.AreaID_PreferLibertyBeforeInternal == (nextGroupObj as AreaAsset).AreaID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                    else if (nextGroupObj is MeterAsset)
                    {
                        if (dataModel[loIdx].MeterID == (nextGroupObj as MeterAsset).MeterID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                }

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 0;

                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                int totalPaid = 0;

                foreach (int itemIdx in itemIndexesForCurrentGroupAsset)
                {
                    SpaceStatusModel nextSpaceModel = dataModel[itemIdx];
                    totalSpaces++;

                    if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                    {
                        totalVacant++;
                    }
                    else if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        totalOccupied++;
                    }
                    else
                    {
                        totalOtherVS++;
                    }

                    // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                    if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace))
                            totalEnfVioSoon++;
                        /*else if (nextSpaceModel.BayExpiryState == ExpiryState.Expired)
                            totalEnfVio++;*/
                    }

                    if (nextSpaceModel.BayEnforcementState == EnforcementState.MeterViolation)
                    {
                        totalEnfVio++;
                    }


                    if ((nextSpaceModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                        (nextSpaceModel.BayEnforcementState == EnforcementState.Discretionary))
                    {
                        // We will only count this as a violation if no "Action Taken" has been recorded!
                        if (string.IsNullOrEmpty(nextSpaceModel.EnforcementActionTaken) == true)
                            totalEnfVio++;
                    }

                    if ((nextSpaceModel.IsSensorOnly == false) && (nextSpaceModel.BayExpiryState == ExpiryState.Inoperational))
                        totalUnknownExpiry++;

                    if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace) ||
                        (nextSpaceModel.BayExpiryState == ExpiryState.Safe))
                    {
                        totalPaid++;
                    }
                }

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                string targetID = string.Empty;

                if (groupType == "A")
                {
                    targetID = "a" + (nextGroupObj as AreaAsset).AreaID.ToString();
                }
                else if (groupType == "M")
                {
                    targetID = "m" + (nextGroupObj as MeterAsset).MeterID.ToString();
                }

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                if (totalSpaces > 0)
                    percentVacant= Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);

                int percentOtherVS = 0;
                if (totalSpaces > 0)
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                int percentEnfVio = 0;
                if (totalSpaces > 0)
                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);

                int percentEnfSoon = 0;
                if (totalSpaces > 0)
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);

                int percentEnfUnknown = 0;
                if (totalSpaces > 0)
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol); tbGroupCol.AddCssClass("dtcLeft hcenter vcenter siw colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("ID", targetID);
                if (groupType == "A")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as AreaAsset).AreaName);
                }
                else if (groupType == "M")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as MeterAsset).MeterID.ToString());
                }
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.SetInnerText(totalOccupied.ToString() + "/" + totalSpaces.ToString());

                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #3 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.SetInnerText(totalEnfVio.ToString());

                if (percentEnfVio > 0.0f)
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP redCell");
                /*else if ((percentEnfVio + percentEnfSoon) >= 25.0f)
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP orangeCell");*/
                /*else if ((percentEnfUnknown >= 33.0f) || (percentOtherVS >= 33.0f))
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell");*/
                else
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP greenCell");
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Add a "ClearFix" tag
                tbGroupCol = new BaseTagHelper("div");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");
            }

            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }

        public static string StayVioSpaceSummary(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            // Determine the filter style
            var filter = ViewData["filter"].ToString();
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable -- only want to see violations)

            int countOfGroups = dataModel.Count;

            string parentTarget = "P";// ViewData["parentTarget"].ToString();

            int itemsAddedToDataArea = 0;

            #region Header Area
            // Output column headers
            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("div"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbTitleBlock_ForColumn.MergeAttribute("style", "display:block; font-size:12px; line-height:12px; background-color:rgb(239, 243, 251); color:Black; width:266px;"); //height: 158px; 

            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Space");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw1 colCellMP"); 

            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Arrival");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw2 colCellMP");
            
            // Column #3 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Expiry Time");
           // tbTitleCol.SetInnerText("Time Over");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw3 colCellMP");
            
            // Column #4 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Action");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw4 colCellMP");
            sb.AppendLine(tbTitleBlock_ForColumn.ToString(TagRenderMode.Normal));

            // We need a "clearfix" element to do a line-break
            tbTitleBlock_ForColumn = new BaseTagHelper("div");
            tbTitleBlock_ForColumn.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");
            sb.AppendLine(tbTitleBlock_ForColumn.ToString(TagRenderMode.Normal));
            #endregion

            #region Data Area

            // Output body of all data

            BaseTagHelper tbTable = new BaseTagHelper("div");  // span
            //OuterPanel.Children.Add(tbTable);
            tbTable.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            /*tbTable.MergeAttribute("style", "color:white; cursor: pointer; min-width:200px; width:224px; max-width:240px; min-height:158px; font-size:1px; line-height:1px;"); //height: 158px; */
            tbTable.MergeAttribute("style", "display:block; color:white; cursor: pointer; min-width:200px; min-height:158px; font-size:1px; line-height:1px; "); /*width:224px;*/


            // Loop for each space
            foreach (SpaceStatusModel currentSpaceStatusModel in dataModel)
            {
                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 1;

                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                int totalPaid = 0;

                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    totalVacant++;
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    totalOccupied++;
                }
                else
                {
                    totalOtherVS++;
                }

                // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace))
                        totalEnfVioSoon++;
                    /*else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                        totalEnfVio++;*/
                }

                if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation)
                {
                    totalEnfVio++;
                }

                if ((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                    (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                {
                    // We will only count this as a violation if no "Action Taken" has been recorded!
                    if (string.IsNullOrEmpty(currentSpaceStatusModel.EnforcementActionTaken) == true)
                        totalEnfVio++;
                }


                if ((currentSpaceStatusModel.IsSensorOnly == false) && (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational))
                    totalUnknownExpiry++;

                if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace) ||
                    (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe))
                {
                    totalPaid++;
                }

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                int percentOtherVS = 0;

                int percentEnfVio = 0;
                int percentEnfSoon = 0;
                int percentEnfUnknown = 0;
                if (totalSpaces > 0)
                {
                    percentVacant = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }

                string targetID = string.Empty;
                targetID = "m" + currentSpaceStatusModel.MeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();

                // Check the filter options, and if this space doesn't qualify for filters, omit it from the response HTML
                if (string.Compare(filter, "V", true) == 0)
                {
                    // Only want vacancies
                    if (totalOccupied > 0)
                        continue;
                }
                else if (string.Compare(filter, "O", true) == 0)
                {
                    // Only want occupied
                    if (totalOccupied == 0)
                        continue;
                }
                else if (string.Compare(filter, "E", true) == 0)
                {
                    // Only want violations (Enforceable)
                    if (totalEnfVio == 0)
                        continue;
                }
                else if (string.Compare(filter, "OE", true) == 0)
                {
                    // Only want occupied or violations (Enforceable)
                    if ((totalOccupied == 0) && (totalEnfVio == 0))
                        continue;
                }


                // Increment the number of items that we are including (after active filters are accounted for)
                itemsAddedToDataArea++;

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                BaseTagHelper tbGroupSection = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupSection);
                tbGroupSection.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
                tbGroupSection.MergeAttribute("style", "float:left; color:white; cursor:pointer; font-size:1px; line-height:1px;"); //height: 158px; 

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupSection.Children.Add(tbGroupCol);
                tbGroupCol.MergeAttribute("ID", targetID);
                tbGroupCol.AddCssClass("dtcLeft Bold hcenter vcenter siw1 colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
                // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupSection.Children.Add(tbGroupCol);
                /*tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered*/

                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    // Space is occupied
                    string arrivalTimeString = currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("hh:mm:ss tt");
                    // If occupied for more than a day, we need to put an indicator so user knows there is something different about this timestamp
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) >= 1)
                        arrivalTimeString = arrivalTimeString + " *";
                    tbGroupCol.SetInnerText(arrivalTimeString);
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw2 colCellMP grayCell Bold"); // horizontally and vertically centered // orangecell // purpleCell
                }
                else
                {
                    tbGroupCol.SetInnerHtml("&nbsp;");
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw2 colCellMP grayCell Bold"); // horizontally and vertically centered
                }

                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #3 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupSection.Children.Add(tbGroupCol);
                
               TimeSpan VioDuration = new TimeSpan(0);
                /*
                if (currentSpaceStatusModel.CurrentOverstayViolation != null)
                    VioDuration = currentSpaceStatusModel.CurrentOverstayViolation.DurationOfTimeBeyondStayLimits;
                else if (currentSpaceStatusModel.AllOverstayViolations.Count > 0)
                    VioDuration = currentSpaceStatusModel.AllOverstayViolations[currentSpaceStatusModel.AllOverstayViolations.Count - 1].DurationOfTimeBeyondStayLimits;
                */
                //if (overstayInfo != null)
                //    VioDuration = overstayInfo.DurationOfTimeBeyondStayLimits;

                //if (VioDuration.TotalSeconds > 0)
                //{
                //    // Space is occupied
                //    tbGroupCol.SetInnerText(FormatElapsedAsMinutes(VioDuration));
                //    if (VioDuration.TotalMinutes < 15)
                //    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP orangeCell notBold"); // horizontally and vertically centered 
                //    else
                //        tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP redCell notBold"); // horizontally and vertically centered 
                //}
                //else
                //{
                //    tbGroupCol.SetInnerHtml("&nbsp;");
                //    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP grayCell notBold"); // horizontally and vertically centered
                //}


                if (currentSpaceStatusModel.IsSensorOnly == false)
                {
                    string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);

                    if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                    {


                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP redCell notBold");
                        tbGroupCol.SetInnerText(ExpiryTimeString);

                    }
                    else
                    {
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP orangeCell notBold");
                    }
                }
                else
                {


                    tbGroupCol.SetInnerHtml("&nbsp;");
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP grayCell notBold"); // horizontally and vertically centered
                }




                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");


                // Column #4 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupSection.Children.Add(tbGroupCol);
                if (totalEnfVio >= 1)
                {
                    tbGroupCol.SetInnerText("Action");
                    if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary)
                        tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw4 colCellMP orangeCell Bold underline"); // horizontally and vertically centered
                    else
                        tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw4 colCellMP redCell Bold underline"); // horizontally and vertically centered
                }
                else
                {
                    tbGroupCol.SetInnerText("Action");
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw4 colCellMP greenCell Bold underline"); // horizontally and vertically centered
                }

                tbGroupCol.MergeAttribute("onclick", "actionForSpecificTarget(\"" + targetID + "\");");
            }

            // If no items qualified, output an indicator for that info
            if (itemsAddedToDataArea == 0)
            {
                BaseTagHelper tbNoDataBlock = new BaseTagHelper("span");
                tbTable.Children.Add(tbNoDataBlock);
                if (string.Compare(filter, "E", true) == 0) 
                    tbNoDataBlock.SetInnerText("No Unactioned Violations");
                else
                    tbNoDataBlock.SetInnerText("No results for filter");
                tbNoDataBlock.MergeAttribute("style", "cursor: auto; font-size:12px; display: inline-block; left:4px; height: 24px; line-height: 24px; top:90px; width:212px; border:1px solid #000; position:absolute; background-color:Orange; color:Black; text-align:center;"); 
            }
            #endregion


            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }

        public static string StayVioSpaceAction(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            BaseTagHelper tbTable = new BaseTagHelper("span");  // div
            tbTable.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbTable.MergeAttribute("style", "display:block; color:white; cursor: pointer; min-width:200px; width:224px; max-width:240px; font-size:1px; line-height:1px;");

            BaseTagHelper tbGroupCol = null;

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

                // Space/Meter identity
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                tbGroupCol.SetInnerText("Space:");
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcRight txtL vcenter colCellMP whiteCell underlineCell");
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());
            }

            // We need a "clearfix" element to do a line-break
            tbGroupCol = new BaseTagHelper("div");
            tbTable.Children.Add(tbGroupCol);
            tbGroupCol.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");

            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }

        public static string StayVioSpaceDetails(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            // Determine the filter style
            var filter = ViewData["filter"].ToString();
            if (string.IsNullOrEmpty(filter))
                filter = "E"; // Default to "E" (Enforceable -- only want to see violations)

            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);



            // Reiteration of SpcSummary table (1-record only), that includes the link for "Action Taken"
            #region Header Area
            // Output column headers
            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("div"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbTitleBlock_ForColumn.MergeAttribute("style", "display:block; font-size:12px; line-height:12px; background-color:rgb(239, 243, 251); color:Black; width:266px;"); 

            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Space");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw colCellMP");

            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Arrival");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw2 colCellMP");

            // Column #3 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Expiry Time");
           //tbTitleCol.SetInnerText("Time Over");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw3 colCellMP");

            // Column #4 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.SetInnerText("Action");
            tbTitleCol.AddCssClass("dtcHead Bold hcenter vcenter siw4 colCellMP");
            sb.AppendLine(tbTitleBlock_ForColumn.ToString(TagRenderMode.Normal));

            // We need a "clearfix" element to do a line-break
            tbTitleBlock_ForColumn = new BaseTagHelper("div");
            tbTitleBlock_ForColumn.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");
            sb.AppendLine(tbTitleBlock_ForColumn.ToString(TagRenderMode.Normal));
            #endregion

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
            tbGroupCol.AddCssClass("dtcLeft Bold hcenter vcenter siw colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
            // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
            MeterAsset mtrAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[0].MeterID);
            String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, dataModel[0].MeterID);
            if (mtrAsset.MeterGroupID <= 0) // SSM
                tbGroupCol.SetInnerText(MeterName);
            else
                tbGroupCol.SetInnerText(MeterName + "-" + dataModel[0].BayID.ToString());
            /*tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");*/

            // Column #2 of group row
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            if ((dataModel[0].BayOccupancyState == OccupancyState.Occupied) ||
                (dataModel[0].BayOccupancyState == OccupancyState.MeterFeeding) ||
                (dataModel[0].BayOccupancyState == OccupancyState.Violation))
            {
                // Space is occupied
                string arrivalTimeString = dataModel[0].BayVehicleSensingTimestamp.ToString("hh:mm:ss tt");
                // If occupied for more than a day, we need to put an indicator so user knows there is something different about this timestamp
                if (Math.Abs(dataModel[0].TimeSinceLastInOut.Days) >= 1)
                    arrivalTimeString = arrivalTimeString + " *";

                tbGroupCol.SetInnerText(arrivalTimeString);
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw2 colCellMP grayCell Bold"); // horizontally and vertically centered // orangecell // purpleCell
            }
            else
            {
                tbGroupCol.SetInnerHtml("&nbsp;");
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw2 colCellMP grayCell Bold"); // horizontally and vertically centered
            }

            /*tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");*/

            // Determine if we will consider this an active violation (current or discretionary, but not already actioned)
            bool isEnforceable = false;
            if (dataModel[0].BayEnforcementState == EnforcementState.MeterViolation)
                isEnforceable = true;

            if ((dataModel[0].BayEnforcementState == EnforcementState.OverstayViolation) || (dataModel[0].BayEnforcementState == EnforcementState.Discretionary))
            {
                // At the detail level, it is enforceable, even if action already taken
                /*
                // We will only count this as a violation if no "Action Taken" has been recorded!
                if (string.IsNullOrEmpty(dataModel[0].EnforcementActionTaken) == true)
                */
                isEnforceable = true;
            }



            // Column #3 of group row
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);

            TimeSpan VioDuration = new TimeSpan(0);
            //if (isEnforceable == true)
            //{
            //    if (dataModel[0].CurrentOverstayViolation != null)
            //        VioDuration = dataModel[0].CurrentOverstayViolation.DurationOfTimeBeyondStayLimits;
            //    else if (dataModel[0].AllOverstayViolations.Count > 0)
            //        VioDuration = dataModel[0].AllOverstayViolations[dataModel[0].AllOverstayViolations.Count - 1].DurationOfTimeBeyondStayLimits;
            //}

            //if (VioDuration.TotalSeconds > 0)
            //{
            //    // Space is occupied
            //    tbGroupCol.SetInnerText(FormatElapsedAsMinutes(VioDuration));
            //    if (VioDuration.TotalMinutes < 15)
            //        tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP orangeCell notBold"); // horizontally and vertically centered 
            //    else
            //        tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP redCell notBold"); // horizontally and vertically centered 
            //}
            //else
            //{
            //    tbGroupCol.SetInnerHtml("&nbsp;");
            //    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP grayCell notBold"); // horizontally and vertically centered
            //}



            if (dataModel[0].IsSensorOnly == false)
            {
                string ExpiryTimeString = dataModel[0].GetExpiryTimeString(customerCfg);
                if (dataModel[0].BayExpiryState == ExpiryState.Expired)
                {
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP redCell notBold");
                    tbGroupCol.SetInnerText(ExpiryTimeString);
                }
                else
                {
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP orangeCell notBold");
                }
            }
            else
            {


                tbGroupCol.SetInnerHtml("&nbsp;");
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw3 colCellMP grayCell notBold"); // horizontally and vertically centered
            }




            /*tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");*/

            // Column #4 of group row
            tbGroupCol = new BaseTagHelper("span");
            tbGroupSection.Children.Add(tbGroupCol);
            if (isEnforceable == true)
            {
                tbGroupCol.SetInnerText("Action");
                if (dataModel[0].BayEnforcementState == EnforcementState.Discretionary) 
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw4 colCellMP orangeCell Bold underline"); // horizontally and vertically centered
                else 
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw4 colCellMP redCell Bold underline"); // horizontally and vertically centered
            }
            else
            {
                tbGroupCol.SetInnerText("Action");
                tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw4 colCellMP greenCell Bold underline"); // horizontally and vertically centered
            }
            tbGroupCol.MergeAttribute("onclick", "actionForSpecificTarget(\"" + targetID + "\");");
            tbGroupCol.MergeAttribute("style", "cursor: pointer;");

            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));


            // Output body of all normal data

            tbTable = new BaseTagHelper("span");  // div
            tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidthRENAMED mb0"); // Display as table
            tbTable.MergeAttribute("style", "color:white; cursor: auto; min-width:200px; width:224px; max-width:240px; min-height:50px; font-size:1px; line-height:1px;"); //height: 158px; 

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

                /*
                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcLeft hcenter vcenter siw colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
                // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
                MeterAsset asset = customerCfg.GetMeterAsset_ForMeter(currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.MeterID.ToString());
                else
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.MeterID.ToString() + "-" + currentSpaceStatusModel.BayID.ToString());
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");
                */

                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);

                // Don't need to output this because we already have it in our grid
                /*
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
                */

                // Previous "Action Taken", if applicable
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

                // Enforcement Status
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
                    tbGroupCol.MergeAttribute("style", "width:143px; font-size:12px;  line-height:12px; height: 12px; display:block;  margin:0px; float:left; white-space:nowrap; background-color:yellow; color:Black; overflow:hidden;");
                    tbGroupCol.AddCssClass("txtR vcenter colCellMP");
                    tbGroupCol.SetInnerText("Overstay in prior period");

                    if (overstayInfo != null)
                    {
                        /*OverstayViolationInfo discretionVio = currentSpaceStatusModel.AllOverstayViolations[currentSpaceStatusModel.AllOverstayViolations.Count - 1];*/
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

                // Current overstay regulations
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

                    DateTime TodayAtDestination = Convert.ToDateTime(customerCfg.DestinationTimeZoneDisplayName);//UtilityClasses.TimeZoneInfo.ConvertTimeZoneToTimeZone(DateTime.Now, customerCfg.ServerTimeZone, customerCfg.CustomerTimeZone).Date;
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
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
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
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
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


                    // We will just output either the violation of the current regulation period, or the last violated regulation period (instead of showing all)
                    /*
                    for (int loIdx = 0; loIdx < currentSpaceStatusModel.ViolatedRegulationPeriods.Count; loIdx++)
                    {
                        if (loIdx < currentSpaceStatusModel.ViolatedRegulationPeriods.Count - 1)
                        {
                            tbGroupCol = new BaseTagHelper("span");
                            tbTable.Children.Add(tbGroupCol);
                            tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail");
                        }
                        else
                        {
                            tbGroupCol = new BaseTagHelper("span");
                            tbTable.Children.Add(tbGroupCol);
                            tbGroupCol.AddCssClass("dtc_Detail txtR vcenter colCellMP whiteCell_Detail underlineCell");
                        }

                        tbGroupCol.SetInnerText(((DayOfWeek)(currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].StartTime.ToString("hh:mm:ss tt") + " - " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].EndTime.ToString("hh:mm:ss tt") + ", " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].MaxStayMinutes.ToString() + " mins");
                        tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                    }
                    */

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

                // Payment status information
                if (currentSpaceStatusModel.IsSensorOnly == false)
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

                if (currentSpaceStatusModel.HasSensor == true)
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




        public static string GroupSummary_WinCE(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            IEnumerable AssetsToGroupBy = null;
            int countOfGroups = 0;
            if (groupType == "A")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg);
                countOfGroups = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg).Count;
            }
            else if (groupType == "M")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg);
                countOfGroups = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg).Count;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            BaseTagHelper tbTitleBlock_WholePage = new BaseTagHelper("table"); // Regular block
            tbTitleBlock_WholePage.MergeAttribute("style", "font-size:12px; line-height:12px; background-color:rgb(239, 243, 251);");

            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("tr"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("pageColHead1 dtNoLinesOrGaps pageColWidth"); // Display as table-row //pageColHead2
            tbTitleBlock_ForColumn.MergeAttribute("style", "margin-bottom:2px; font-size:12px; line-height:12px; background-color:rgb(239, 243, 251);");
            tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);

            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            if (groupType == "A")
                tbTitleCol.SetInnerText("Area");
            else if (groupType == "M")
                tbTitleCol.SetInnerText("Meter");
            tbTitleCol.MergeAttribute("style", "width:58px;");
            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Car16.gif); background-repeat:no-repeat; background-position:center center; height:17px; width:58px; min-width:42px;");
            // Column #3 of title row
            if (includePAMElements)
            {
                tbTitleCol = new BaseTagHelper("td");
                tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Paid16.png); background-repeat:no-repeat; background-position:center center; height:17px; width:58px; min-width:42px;");
            }
            // Column #4 of title row
            tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.MergeAttribute("style", "background-image: url(/Images/violation-icon.gif); background-repeat:no-repeat; background-position:center center; height:17px; width:58px; min-width:42px;");


            sb.AppendLine(tbTitleBlock_WholePage.ToString(TagRenderMode.Normal));

            BaseTagHelper tbTable = new BaseTagHelper("span");  // div
            tbTable.AddCssClass("dtNoLinesOrGaps mb0"); // Display as table
            tbTable.MergeAttribute("style", "color:white; cursor: pointer; min-width:200px; width:224px; max-width:240px; min-height:158px; font-size:1px; line-height:1px;"); //height: 158px; 

            // Loop for each group
            foreach (object nextGroupObj in AssetsToGroupBy)
            {
                // Build list of indexes inside modelForView that are associated with the current asset
                List<int> itemIndexesForCurrentGroupAsset = new List<int>();
                for (int loIdx = 0; loIdx < dataModel.Count; loIdx++)
                {
                    if (nextGroupObj is AreaAsset)
                    {
                        MeterAsset relatedMeterAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[loIdx].MeterID);
                        if (relatedMeterAsset.AreaID_PreferLibertyBeforeInternal == (nextGroupObj as AreaAsset).AreaID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                    else if (nextGroupObj is MeterAsset)
                    {
                        if (dataModel[loIdx].MeterID == (nextGroupObj as MeterAsset).MeterID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                }

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 0;

                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                int totalPaid = 0;

                foreach (int itemIdx in itemIndexesForCurrentGroupAsset)
                {
                    SpaceStatusModel nextSpaceModel = dataModel[itemIdx];
                    totalSpaces++;

                    if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                    {
                        totalVacant++;
                    }
                    else if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        totalOccupied++;
                    }
                    else
                    {
                        totalOtherVS++;
                    }

                    // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                    if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace))
                            totalEnfVioSoon++;
                        /*else if (nextSpaceModel.BayExpiryState == ExpiryState.Expired)
                            totalEnfVio++;*/
                    }

                    if ((nextSpaceModel.BayEnforcementState == EnforcementState.MeterViolation) ||
                        (nextSpaceModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                        (nextSpaceModel.BayEnforcementState == EnforcementState.Discretionary))
                    {
                        // We will only count this as a violation if no "Action Taken" has been recorded!
                        if (string.IsNullOrEmpty(nextSpaceModel.EnforcementActionTaken) == true)
                            totalEnfVio++;
                    }

                    if ((nextSpaceModel.IsSensorOnly == false) && (nextSpaceModel.BayExpiryState == ExpiryState.Inoperational))
                        totalUnknownExpiry++;

                    if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace) ||
                        (nextSpaceModel.BayExpiryState == ExpiryState.Safe))
                    {
                        totalPaid++;
                    }
                }

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                string targetID = string.Empty;

                if (groupType == "A")
                {
                    targetID = "a" + (nextGroupObj as AreaAsset).AreaID.ToString();
                }
                else if (groupType == "M")
                {
                    targetID = "m" + (nextGroupObj as MeterAsset).MeterID.ToString();
                }

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                int percentOtherVS = 0;

                int percentEnfVio = 0;
                int percentEnfSoon = 0;
                int percentEnfUnknown = 0;
                if (totalSpaces > 0)
                {
                    percentVacant = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol); tbGroupCol.AddCssClass("dtcLeft hcenter vcenter siw colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
                tbGroupCol.MergeAttribute("ID", targetID);
                if (groupType == "A")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as AreaAsset).AreaName);
                }
                else if (groupType == "M")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as MeterAsset).MeterID.ToString());
                }
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.SetInnerText(totalOccupied.ToString() + "/" + totalSpaces.ToString());

                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #3 of group row
                if (includePAMElements)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                    tbGroupCol.SetInnerText(totalPaid.ToString() + "/" + totalSpaces.ToString());

                    tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");
                }

                // Column #4 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.SetInnerText(totalEnfVio.ToString() + "/" + totalSpaces.ToString());

                if (percentEnfVio >= 50.0f)
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP redCell");
                else if ((percentEnfVio + percentEnfSoon) >= 25.0f)
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP orangeCell");
                /*else if ((percentEnfUnknown >= 33.0f) || (percentOtherVS >= 33.0f))
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell");*/
                else
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP greenCell");
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Add a "ClearFix" tag
                tbGroupCol = new BaseTagHelper("div");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");
            }

            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }

        public static string GroupSummary(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            // For Windows Mobile device, we must render different HTML than for other browsers
            bool IsWinCE = false;
            try
            {
                IsWinCE = Convert.ToBoolean(ViewData["IsWinCE"]);
            }
            catch { }
            if (IsWinCE)
                return GroupSummary_WinCE(Url, dataModel, ViewData);
            
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            IEnumerable AssetsToGroupBy = null;
            int countOfGroups = 0;
            if (groupType == "A")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg);
                countOfGroups = CustomerLogic.CustomerManager.GetAreaAssetsForCustomer(customerCfg).Count;
            }
            else if (groupType == "M")
            {
                AssetsToGroupBy = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg);
                countOfGroups = CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg).Count;
            }
            else
            {
                throw new Exception("Support for group type '" + groupType + "' not implemented in " +
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            BaseTagHelper tbTitleBlock_WholePage = new BaseTagHelper("div"); // Regular block

            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("pageColHead1 pageColWidth"); // Display as table-row //pageColHead2
            tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
            BaseTagHelper tbTitleTable_ForColumn = new BaseTagHelper("div");  // Table
            tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
            tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
            tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;");
            BaseTagHelper tbTitleRow_ForColumn = new BaseTagHelper("span");
            tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
            tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row
            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("span");
            tbTitleRow_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
            if (groupType == "A")
                tbTitleCol.SetInnerText("Area");
            else if (groupType == "M")
                tbTitleCol.SetInnerText("Meter Name");
            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleRow_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Car16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
            // Column #3 of title row
            if (includePAMElements)
            {
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Paid16.png);  background-repeat:no-repeat; background-position:center center; min-width:42px;");
            }
            // Column #4 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleRow_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.MergeAttribute("style", "background-image: url(/Images/violation-icon.gif); background-repeat:no-repeat; background-position:center center; min-width:42px;");

            // Start of another block that is column headers when our page has enough room for another "column" of data
            // Only add this one if there are at least 2 groups to be displayed
            if (countOfGroups >= 2)
            {
                tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
                tbTitleBlock_ForColumn.AddCssClass("pageColHead2 pageColWidth"); // Display as table-row //pageColHead2
                tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
                tbTitleTable_ForColumn = new BaseTagHelper("div");
                tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
                tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
                tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;"); // width:230px;
                // Title row for 2nd area
                tbTitleRow_ForColumn = new BaseTagHelper("span");
                tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
                tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row //pageColHead2
                // Column #1 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                if (groupType == "A")
                    tbTitleCol.SetInnerText("Area");
                else if (groupType == "M")
                    tbTitleCol.SetInnerText("Meter Name");
                // Column #2 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Car16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
                // Column #3 of title row
                if (includePAMElements)
                {
                    tbTitleCol = new BaseTagHelper("span");
                    tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                    tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                    tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Paid16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
                }
                // Column #4 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/violation-icon.gif); background-repeat:no-repeat; background-position:center center; min-width:42px;");
            }

            // Start of another block that is column headers when our page has enough room for another "column" of data
            // Only add this one if there are at least 3 groups to be displayed
            if (countOfGroups >= 3)
            {
                tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
                tbTitleBlock_ForColumn.AddCssClass("pageColHead3 pageColWidth"); // Display as table-row //pageColHead2
                tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
                tbTitleTable_ForColumn = new BaseTagHelper("div");
                tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
                tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
                tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;"); // width:230px;
                // Title row for 2nd area
                tbTitleRow_ForColumn = new BaseTagHelper("span");
                tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
                tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row
                // Column #1 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                if (groupType == "A")
                    tbTitleCol.SetInnerText("Area");
                else if (groupType == "M")
                    tbTitleCol.SetInnerText("Meter Name");
                // Column #2 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Car16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
                // Column #3 of title row
                if (includePAMElements)
                {
                    tbTitleCol = new BaseTagHelper("span");
                    tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                    tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                    tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Paid16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
                }
                // Column #4 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/violation-icon.gif); background-repeat:no-repeat; background-position:center center; min-width:42px;");
            }

            // Start of another block that is column headers when our page has enough room for another "column" of data
            // Only add this one if there are at least 4 groups to be displayed
            if (countOfGroups >= 4)
            {
                tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
                tbTitleBlock_ForColumn.AddCssClass("pageColHead4 pageColWidth"); // Display as table-row //pageColHead2
                tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
                tbTitleTable_ForColumn = new BaseTagHelper("div");
                tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
                tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
                tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;"); // width:230px;
                // Title row for 2nd area
                tbTitleRow_ForColumn = new BaseTagHelper("span");
                tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
                tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row
                // Column #1 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                if (groupType == "A")
                    tbTitleCol.SetInnerText("Area");
                else if (groupType == "M")
                    tbTitleCol.SetInnerText("Meter Name");
                // Column #2 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Car16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
                // Column #3 of title row
                if (includePAMElements)
                {
                    tbTitleCol = new BaseTagHelper("span");
                    tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                    tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                    tbTitleCol.MergeAttribute("style", "background-image: url(/Images/Paid16.png); background-repeat:no-repeat; background-position:center center; min-width:42px;");
                }
                // Column #4 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth1"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.MergeAttribute("style", "background-image: url(/Images/violation-icon.gif); background-repeat:no-repeat; background-position:center center; min-width:42px;");
            }

            sb.AppendLine(tbTitleBlock_WholePage.ToString(TagRenderMode.Normal));


            // Loop for each group
            foreach (object nextGroupObj in AssetsToGroupBy)
            {
                // Build list of indexes inside modelForView that are associated with the current asset
                List<int> itemIndexesForCurrentGroupAsset = new List<int>();
                for (int loIdx = 0; loIdx < dataModel.Count; loIdx++)
                {
                    if (nextGroupObj is AreaAsset)
                    {
                        MeterAsset relatedMeterAsset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, dataModel[loIdx].MeterID);
                        if (relatedMeterAsset.AreaID_PreferLibertyBeforeInternal == (nextGroupObj as AreaAsset).AreaID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                    else if (nextGroupObj is MeterAsset)
                    {
                        if (dataModel[loIdx].MeterID == (nextGroupObj as MeterAsset).MeterID)
                            itemIndexesForCurrentGroupAsset.Add(loIdx);
                    }
                }

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 0;
                int totalMetersCount = 0;
                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                int totalPaid = 0;

                foreach (int itemIdx in itemIndexesForCurrentGroupAsset)
                {

                    SpaceStatusModel nextSpaceModel = dataModel[itemIdx];
                    totalMetersCount++;
                    if (nextSpaceModel.HasSensor == true)
                    {
                        totalSpaces++;
                    }

                    if (nextSpaceModel.HasSensor == true)
                    {

                        if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                        {
                            totalVacant++;
                        }
                        else if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                            (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                            (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                        {
                            totalOccupied++;
                        }
                        {
                            totalOtherVS++;
                        }
                    }
                    else
                    {
                        totalOtherVS++;
                    }



                    // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                    if ((nextSpaceModel.BayOccupancyState == OccupancyState.Occupied) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                        (nextSpaceModel.BayOccupancyState == OccupancyState.Violation))
                    {
                        if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace))
                            totalEnfVioSoon++;
                        /*else if (nextSpaceModel.BayExpiryState == ExpiryState.Expired)
                            totalEnfVio++;*/
                    }

                    if ((nextSpaceModel.BayEnforcementState == EnforcementState.MeterViolation) ||
                        (nextSpaceModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                        (nextSpaceModel.BayEnforcementState == EnforcementState.Discretionary))
                    {
                        // We will only count this as a violation if no "Action Taken" has been recorded!
                        if (string.IsNullOrEmpty(nextSpaceModel.EnforcementActionTaken) == true)
                            totalEnfVio++;
                    }

                    if ((nextSpaceModel.IsSensorOnly == false) && (nextSpaceModel.BayExpiryState == ExpiryState.Inoperational))
                        totalUnknownExpiry++;

                    if ((nextSpaceModel.BayExpiryState == ExpiryState.Critical) || (nextSpaceModel.BayExpiryState == ExpiryState.Grace) ||
                        (nextSpaceModel.BayExpiryState == ExpiryState.Safe))
                    {
                        totalPaid++;
                    }

                }

                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!


                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("span"); // Inline-block
                OuterPanel.AddCssClass("pageColWidth grpOutPnl");

                BaseTagHelper InnerPanel = new BaseTagHelper("span"); // Inline-block
                InnerPanel.AddCssClass("pageColTableWidth grpInPnl");
                OuterPanel.Children.Add(InnerPanel);

                BaseTagHelper tbTable = new BaseTagHelper("div");
                InnerPanel.Children.Add(tbTable);
                tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidth mb0"); // Display as table
                tbTable.MergeAttribute("style", "color:white; cursor: pointer;"); 



                string targetID = string.Empty;

                if (groupType == "A")
                {
                    targetID = "a" + (nextGroupObj as AreaAsset).AreaID.ToString();
                    tbTable.MergeAttribute("ID", targetID);
                }
                else if (groupType == "M")
                {
                    targetID = "m" + (nextGroupObj as MeterAsset).MeterID.ToString();
                    tbTable.MergeAttribute("ID", targetID);
                }
                tbTable.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                int percentOtherVS = 0;
                int percentEnfVio = 0;
                int percentEnfSoon = 0;
                int percentEnfUnknown = 0;

                if (totalSpaces > 0)
                {
                    percentVacant = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr appleblueCell np"); // Display as table-row

                // Grayish color for row
                tbGroupRow.MergeAttribute("style", "background-color:#484848; color: white; "); // gray

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP appleblueCell"); // Display as table-cell, both horizontally and vertically centered

                if (groupType == "A")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as AreaAsset).AreaName);
                }
                else if (groupType == "M")
                {
                    tbGroupCol.SetInnerText((nextGroupObj as MeterAsset).MeterName);
                }

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter siw1 colCellMP"); // Display as table-cell, both horizontally and vertically centered
                tbGroupCol.SetInnerText(totalOccupied.ToString() + "/" + totalSpaces.ToString());

                // Column #3 of group row
                if (includePAMElements)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbGroupRow.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw1 colCellMP"); // Display as table-cell, both horizontally and vertically centered
                    tbGroupCol.SetInnerText(totalPaid.ToString() + "/" + totalMetersCount.ToString());
                }

                // Column #4 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.SetInnerText(totalEnfVio.ToString());// + "/" + totalSpaces.ToString());

                if (percentEnfVio >= 50.0f)
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw1 colCellMP redCell");
                else if ((percentEnfVio + percentEnfSoon) >= 25.0f)
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw1 colCellMP orangeCell");
                /*else if ((percentEnfUnknown >= 33.0f) || (percentOtherVS >= 33.0f))
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw1 colCellMP grayCell");*/
                else
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw1 colCellMP greenCell");

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string SpaceSummary_WinCE(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            string displayStyle = ViewData["displayStyle"].ToString();
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (Graphic and text normally, but text-only on small devices)
            bool forceSingleRow = (string.Compare(displayStyle, "1", true) == 0);
            bool forceDoubleRow = (string.Compare(displayStyle, "2", true) == 0);

            // Determine the filter style
            var filter = ViewData["filter"].ToString();
            if (string.IsNullOrEmpty(filter))
                filter = "A"; // Default to "A" (All, or no filtering)

            int countOfGroups = dataModel.Count;

            string parentTarget = ViewData["parentTarget"].ToString();

            #region Header Area
            // Output column headers

            BaseTagHelper tbTitleBlock_WholePage = new BaseTagHelper("table"); // Regular block
            tbTitleBlock_WholePage.MergeAttribute("style", "font-size:12px; line-height:12px; background-color:rgb(239, 243, 251);");

            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("tr"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("pageColHead1 dtNoLinesOrGaps"); // pageColWidth Display as table-row //pageColHead2
            tbTitleBlock_ForColumn.MergeAttribute("style", "margin-bottom:2px; font-size:12px; line-height:12px; background-color:rgb(239, 243, 251);");
            tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);

            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Space");
            tbTitleCol.MergeAttribute("style", "width:58px;");
            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Sensor");
            tbTitleCol.MergeAttribute("style", "width:58px;");
            // Column #3 of title row
            if (includePAMElements)
            {
                tbTitleCol = new BaseTagHelper("td");
                tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Payment");
                tbTitleCol.MergeAttribute("style", "width:58px;");
            }
            // Column #4 of title row
            tbTitleCol = new BaseTagHelper("td");
            tbTitleBlock_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtcHead hcenter vcenter"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Enforce");
            tbTitleCol.MergeAttribute("style", "width:58px;");

            sb.AppendLine(tbTitleBlock_WholePage.ToString(TagRenderMode.Normal));
            #endregion

            #region Data Area

            // Output body of all data

            BaseTagHelper tbTable = new BaseTagHelper("span");  // div
            //OuterPanel.Children.Add(tbTable);
            tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidthRENAMED mb0"); // Display as table
            tbTable.MergeAttribute("style", "color:white; cursor: pointer; min-width:200px; width:224px; max-width:240px; min-height:158px; font-size:1px; line-height:1px;"); //height: 158px; 


            // Loop for each space
            foreach (SpaceStatusModel currentSpaceStatusModel in dataModel)
            {
                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 1;

                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                int totalPaid = 0;

                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    totalVacant++;
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    totalOccupied++;
                }
                else
                {
                    totalOtherVS++;
                }

                // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace))
                        totalEnfVioSoon++;
                    /*else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                        totalEnfVio++;*/
                }

                if ((currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation) ||
                    (currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                    (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                {
                    // We will only count this as a violation if no "Action Taken" has been recorded!
                    if (string.IsNullOrEmpty(currentSpaceStatusModel.EnforcementActionTaken) == true)
                        totalEnfVio++;
                }

                if ((currentSpaceStatusModel.IsSensorOnly == false) && (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational))
                    totalUnknownExpiry++;

                if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace) ||
                    (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe))
                {
                    totalPaid++;
                }

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                int percentOtherVS = 0;

                int percentEnfVio = 0;
                int percentEnfSoon = 0;
                int percentEnfUnknown = 0;
                if (totalSpaces > 0)
                {
                    percentVacant = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }

                string targetID = string.Empty;
                targetID = "m" + currentSpaceStatusModel.MeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();

                // Check the filter options, and if this space doesn't qualify for filters, omit it from the response HTML
                if (string.Compare(filter, "V", true) == 0)
                {
                    // Only want vacancies
                    if (totalOccupied > 0)
                        continue;
                }
                else if (string.Compare(filter, "O", true) == 0)
                {
                    // Only want occupied
                    if (totalOccupied == 0)
                        continue;
                }
                else if (string.Compare(filter, "E", true) == 0)
                {
                    // Only want violations (Enforceable)
                    if (totalEnfVio == 0)
                        continue;
                }
                else if (string.Compare(filter, "OE", true) == 0)
                {
                    // Only want occupied or violations (Enforceable)
                    if ((totalOccupied == 0) && (totalEnfVio == 0))
                        continue;
                }


                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.MergeAttribute("ID", targetID);
                tbGroupCol.AddCssClass("dtcLeft hcenter vcenter siw colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
                // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                /*tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered*/

                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    // Space is occupied
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) > 30)
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell notBold"); // horizontally and vertically centered // orangecell // purpleCell
                    else
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // horizontally and vertically centered // orangecell // purpleCell
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty))
                {
                    // Space is empty
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) > 30)
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP ltgreenCell notBold"); // horizontally and vertically centered // greenCell
                    else
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP ltgreenCell"); // horizontally and vertically centered // greenCell
                }
                else
                {
                    // Unknown or unreliable info, so will be gray color
                    /*tbGroupCol.SetInnerText("");*/
                    tbGroupCol.SetInnerHtml("&nbsp;");
                    tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // horizontally and vertically centered
                }

                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Column #3 of group row
                if (includePAMElements)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    /*tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered*/

                    if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) ||
                        (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace) ||
                        (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe))
                    {
                        // Space is paid, almost expired, or still covered by grace period
                        tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.BayExpiration_AsTimeSpan));
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP ltgreenCell"); // horizontally and vertically centered  // greenCell
                    }
                    else if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired))
                    {
                        // Space is expired
                        tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.BayExpiration_AsTimeSpan));
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // horizontally and vertically centered //orangeCell
                    }
                    else
                    {
                        // Payment status is unknown
                        /*tbGroupCol.SetInnerText("");*/
                        tbGroupCol.SetInnerHtml("&nbsp;");
                        tbGroupCol.AddCssClass("dtc hcenter vcenter siw colCellMP grayCell"); // horizontally and vertically centered
                    }

                    tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");
                }

                // Column #4 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);

                /*tbGroupCol.SetInnerText(totalEnfVio.ToString() + "/" + totalSpaces.ToString());*/
                if (totalEnfVio >= 1)
                {
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP redCell"); // horizontally and vertically centered
                }
                else if (totalEnfVioSoon >= 1)
                {
                    tbGroupCol.SetInnerText("Soon");
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP orangeCell"); // horizontally and vertically centered
                }
                else
                {
                    tbGroupCol.SetInnerText("No");
                    tbGroupCol.AddCssClass("dtcRight hcenter vcenter siw colCellMP greenCell"); // horizontally and vertically centered
                }

                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");


                // We need a "clearfix" element to do a line-break
                tbGroupCol = new BaseTagHelper("div");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.MergeAttribute("style", "clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;");
            }
            #endregion


            sb.AppendLine(tbTable.ToString(TagRenderMode.Normal));
            return sb.ToString();
        }

        public static string SpaceSummary(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            // For Windows Mobile device, we must render different HTML than for other browsers
            bool IsWinCE = false;
            try
            {
                IsWinCE = Convert.ToBoolean(ViewData["IsWinCE"]);
            }
            catch { }
            if (IsWinCE)
                return SpaceSummary_WinCE(Url, dataModel, ViewData);

            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            string displayStyle = ViewData["displayStyle"].ToString();
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (Graphic and text normally, but text-only on small devices)
            bool forceSingleRow = (string.Compare(displayStyle, "1", true) == 0);
            bool forceDoubleRow = (string.Compare(displayStyle, "2", true) == 0);

            // Determine the filter style
            var filter = ViewData["filter"].ToString();
            if (string.IsNullOrEmpty(filter))
                filter = "A"; // Default to "A" (All, or no filtering)

           
            int countOfGroups = dataModel.Count;

            string parentTarget = ViewData["parentTarget"].ToString();

            #region Header area

            BaseTagHelper tbTitleBlock_WholePage = new BaseTagHelper("div"); // Regular block
            BaseTagHelper tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
            tbTitleBlock_ForColumn.AddCssClass("pageColHead1 pageColWidth"); // Display as table-row //pageColHead2
            tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
            BaseTagHelper tbTitleTable_ForColumn = new BaseTagHelper("div");  // Table
            tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
            tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
            tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;");
            BaseTagHelper tbTitleRow_ForColumn = new BaseTagHelper("span");
            tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
            tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row
            // Column #1 of title row
            BaseTagHelper tbTitleCol = new BaseTagHelper("span");
            tbTitleRow_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Space");
            // Column #2 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleRow_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Sensor");
            // Column #3 of title row
            if (includePAMElements)
            {
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Payment");
            }
            // Column #4 of title row
            tbTitleCol = new BaseTagHelper("span");
            tbTitleRow_ForColumn.Children.Add(tbTitleCol);
            tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
            tbTitleCol.SetInnerText("Enforce");

            // Start of another block that is column headers when our page has enough room for another "column" of data
            // Only add this one if there are at least 2 groups to be displayed
            if (countOfGroups >= 2)
            {
                tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
                tbTitleBlock_ForColumn.AddCssClass("pageColHead2 pageColWidth"); // Display as table-row //pageColHead2
                tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
                tbTitleTable_ForColumn = new BaseTagHelper("div");
                tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
                tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
                tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;"); // width:230px;
                // Title row for 2nd area
                tbTitleRow_ForColumn = new BaseTagHelper("span");
                tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
                tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row //pageColHead2
                // Column #1 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Space");
                // Column #2 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Sensor");
                // Column #3 of title row
                if (includePAMElements)
                {
                    tbTitleCol = new BaseTagHelper("span");
                    tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                    tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                    tbTitleCol.SetInnerText("Payment");
                }
                // Column #4 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Enforce");
            }

            // Start of another block that is column headers when our page has enough room for another "column" of data
            // Only add this one if there are at least 3 groups to be displayed
            if (countOfGroups >= 3)
            {
                tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
                tbTitleBlock_ForColumn.AddCssClass("pageColHead3 pageColWidth"); // Display as table-row //pageColHead2
                tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
                tbTitleTable_ForColumn = new BaseTagHelper("div");
                tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
                tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
                tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;"); // width:230px;
                // Title row for 2nd area
                tbTitleRow_ForColumn = new BaseTagHelper("span");
                tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
                tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row
                // Column #1 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Space");
                // Column #2 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Sensor");
                // Column #3 of title row
                if (includePAMElements)
                {
                    tbTitleCol = new BaseTagHelper("span");
                    tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                    tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                    tbTitleCol.SetInnerText("Payment");
                }
                // Column #4 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Enforce");
            }

            // Start of another block that is column headers when our page has enough room for another "column" of data
            // Only add this one if there are at least 4 groups to be displayed
            if (countOfGroups >= 4)
            {
                tbTitleBlock_ForColumn = new BaseTagHelper("span"); // Inline-block
                tbTitleBlock_ForColumn.AddCssClass("pageColHead4 pageColWidth"); // Display as table-row //pageColHead2
                tbTitleBlock_WholePage.Children.Add(tbTitleBlock_ForColumn);
                tbTitleTable_ForColumn = new BaseTagHelper("div");
                tbTitleBlock_ForColumn.Children.Add(tbTitleTable_ForColumn);
                tbTitleTable_ForColumn.AddCssClass("dtNoLinesOrGaps pageColWidth"); // Display as table
                tbTitleTable_ForColumn.MergeAttribute("style", "margin-bottom:2px;"); // width:230px;
                // Title row for 2nd area
                tbTitleRow_ForColumn = new BaseTagHelper("span");
                tbTitleTable_ForColumn.Children.Add(tbTitleRow_ForColumn);
                tbTitleRow_ForColumn.AddCssClass("dtrHead"); // Display as table-row
                // Column #1 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Space");
                // Column #2 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Sensor");
                // Column #3 of title row
                if (includePAMElements)
                {
                    tbTitleCol = new BaseTagHelper("span");
                    tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                    tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                    tbTitleCol.SetInnerText("Payment");
                }
                // Column #4 of title row
                tbTitleCol = new BaseTagHelper("span");
                tbTitleRow_ForColumn.Children.Add(tbTitleCol);
                tbTitleCol.AddCssClass("dtc hcenter vcenter sectionWidth2"); // Display as table-cell, both horizontally and vertically centered
                tbTitleCol.SetInnerText("Enforce");
            }

            sb.AppendLine(tbTitleBlock_WholePage.ToString(TagRenderMode.Normal));
            #endregion

            // Loop for each space
            foreach (SpaceStatusModel currentSpaceStatusModel in dataModel)
            {
                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                // Evaluate combined statistics from each space in the current group
                int totalSpaces = 1;

                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                int totalEnfVio = 0;
                int totalEnfVioSoon = 0;
                int totalUnknownExpiry = 0;

                int totalPaid = 0;

                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    totalVacant++;
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    totalOccupied++;
                }
                else
                {
                    totalOtherVS++;
                }

                // If the space is occupied, see if payment status makes it a potential violation (or will be a violation pretty soon)
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace))
                        totalEnfVioSoon++;
                    /*else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                        totalEnfVio++;*/
                }

                if ((currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation) ||
                    (currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) ||
                    (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                {
                    // We will only count this as a violation if no "Action Taken" has been recorded!
                    if (string.IsNullOrEmpty(currentSpaceStatusModel.EnforcementActionTaken) == true)
                        totalEnfVio++;
                }

                if ((currentSpaceStatusModel.IsSensorOnly == false) && (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational))
                    totalUnknownExpiry++;

                if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) || (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace) ||
                    (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe))
                {
                    totalPaid++;
                }

                // Get percentage of spaces that are vacant
                int percentVacant = 0;
                int percentOtherVS = 0;
                int percentEnfVio = 0;
                int percentEnfSoon = 0;
                int percentEnfUnknown = 0;
                if (totalSpaces > 0)
                {
                    percentVacant = Convert.ToInt32((Convert.ToDouble(totalVacant) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentOtherVS = Convert.ToInt32((Convert.ToDouble(totalOtherVS) / Convert.ToDouble(totalSpaces)) * 100.0f);

                    percentEnfVio = Convert.ToInt32((Convert.ToDouble(totalEnfVio) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfSoon = Convert.ToInt32((Convert.ToDouble(totalEnfVioSoon) / Convert.ToDouble(totalSpaces)) * 100.0f);
                    percentEnfUnknown = Convert.ToInt32((Convert.ToDouble(totalUnknownExpiry) / Convert.ToDouble(totalSpaces)) * 100.0f);
                }


                // Check the filter options, and if this space doesn't qualify for filters, omit it from the response HTML
                if (string.Compare(filter, "V", true) == 0)
                {
                    // Only want vacancies
                    if (totalOccupied > 0)
                        continue;
                }
                else if (string.Compare(filter, "O", true) == 0)
                {
                    // Only want occupied
                    if (totalOccupied == 0)
                        continue;
                }
                else if (string.Compare(filter, "E", true) == 0)
                {
                    // Only want violations (Enforceable)
                    if (totalEnfVio == 0)
                        continue;
                }
                else if (string.Compare(filter, "OE", true) == 0)
                {
                    // Only want occupied or violations (Enforceable)
                    if ((totalOccupied == 0) && (totalEnfVio == 0))
                        continue;
                }


                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("span"); // Inline-block
                OuterPanel.AddCssClass("pageColWidth grpOutPnl");

                BaseTagHelper InnerPanel = new BaseTagHelper("span"); // Inline-block
                InnerPanel.AddCssClass("pageColTableWidth grpInPnl");
                OuterPanel.Children.Add(InnerPanel);

                BaseTagHelper tbTable = new BaseTagHelper("div");
                InnerPanel.Children.Add(tbTable);
                tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidth mb0"); // Display as table
                tbTable.MergeAttribute("style", "color:white; cursor: pointer;"); // width:230px; margin-bottom:0px;

                string targetID = string.Empty;
                targetID = "m" + currentSpaceStatusModel.MeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();
                tbTable.MergeAttribute("ID", targetID);
                tbTable.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tblRow = new BaseTagHelper("span");
                tbTable.Children.Add(tblRow);
                tblRow.AddCssClass("dtr appleblueCell np"); // Display as table-row, background is iPhone-style blue

                // Column #1 of group row
                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tblColCell = new BaseTagHelper("span");
                tblRow.Children.Add(tblColCell);
                tblColCell.AddCssClass("dtc hcenter vcenter siw colCellMP"); // Display as table-cell, both horizontally and vertically centered
                // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);

                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tblColCell.SetInnerText(MeterName);
                else
                    tblColCell.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());

                // Column #2 of group row
                tblColCell = new BaseTagHelper("span");
                tblRow.Children.Add(tblColCell);

                BaseTagHelper cellInnerTbl = new BaseTagHelper("div");
                tblColCell.Children.Add(cellInnerTbl);
                cellInnerTbl.AddCssClass("dtNoLinesOrGaps siw mb0 fh"); // Display as nested table

                BaseTagHelper cellInnerTblTopRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblTopRow);
                cellInnerTblTopRow.AddCssClass("dtr np"); // Display as table-row

                BaseTagHelper cellInnerTblBottomRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblBottomRow);
                cellInnerTblBottomRow.AddCssClass("dtr fh np"); // Display as table-row

                BaseTagHelper cellTopContent = null;
                if (forceSingleRow == false)
                    cellTopContent = new BaseTagHelper("div");
                BaseTagHelper cellBottomContent = new BaseTagHelper("span");

                // We will only actually add this element to our object if we are using 2-row style of display
                if (forceSingleRow == false)
                    cellInnerTblTopRow.Children.Add(cellTopContent);
                cellInnerTblBottomRow.Children.Add(cellBottomContent);

                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    // Space is occupied
                    if (forceSingleRow == false)
                    {
                        if(currentSpaceStatusModel.HasSensor == false)
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx4");
                            else
                        cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx1"); // horizontally and vertically centered
                    }
                    cellBottomContent.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) > 30)
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw grayCell notBold"); // horizontally and vertically centered // orangecell // purpleCell
                    else
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw grayCell"); // horizontally and vertically centered // orangecell // purpleCell
                    tblColCell.AddCssClass("dtc hcenter vcenter siw mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered //orangeCell
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty))
                {
                    // Space is empty
                    if (forceSingleRow == false)
                    {
                        if (currentSpaceStatusModel.HasSensor == false)
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx4");
                        else
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx2"); // horizontally and vertically centered
                    }
                    cellBottomContent.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) > 30)
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw ltgreenCell notBold"); // horizontally and vertically centered // greenCell
                    else
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw ltgreenCell"); // horizontally and vertically centered // greenCell
                    tblColCell.AddCssClass("dtc hcenter vcenter siw mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered //greenCell
                }
                else
                {
                    // Unknown or unreliable info, so will be gray color
                    if (forceSingleRow == false)
                    {
                        if (currentSpaceStatusModel.HasSensor == false)
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx4");
                        else
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx3"); // horizontally and vertically centered
                    }
                    cellBottomContent.SetInnerText("" /*"N/A"*/);
                    cellBottomContent.AddCssClass("dtc hcenter vcenter siw grayCell"); // horizontally and vertically centered
                    tblColCell.AddCssClass("dtc hcenter vcenter siw mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                }

                // Column #3 of group row
                if (includePAMElements)
                {
                    tblColCell = new BaseTagHelper("span");
                    tblRow.Children.Add(tblColCell);

                    cellInnerTbl = new BaseTagHelper("div");
                    tblColCell.Children.Add(cellInnerTbl);
                    cellInnerTbl.AddCssClass("dtNoLinesOrGaps siw2 mb0 fh"); // Display as table

                    cellInnerTblTopRow = new BaseTagHelper("span");
                    cellInnerTbl.Children.Add(cellInnerTblTopRow);
                    //cellInnerTbl.Children.Add(cellInnerTblTopRow);
                    cellInnerTblTopRow.AddCssClass("dtr np"); // Display as table-row

                    cellInnerTblBottomRow = new BaseTagHelper("span");
                    cellInnerTbl.Children.Add(cellInnerTblBottomRow);
                    cellInnerTblBottomRow.AddCssClass("dtr fh np"); // Display as table-row

                    cellTopContent = null;
                    if (forceSingleRow == false)
                        cellTopContent = new BaseTagHelper("div");
                    cellBottomContent = new BaseTagHelper("span");

                    // We will only actually add this element to our object if we are using 2-row style of display
                    if (forceSingleRow == false)
                        cellInnerTblTopRow.Children.Add(cellTopContent);
                    cellInnerTblBottomRow.Children.Add(cellBottomContent);

                    if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical) ||
                        (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace) ||
                        (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe))
                    {
                        // Space is paid, almost expired, or still covered by grace period
                        if (forceSingleRow == false)
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw2 mb0 PayGfx1"); // horizontally and vertically centered
                        cellBottomContent.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.BayExpiration_AsTimeSpan));
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw2 ltgreenCell"); // horizontally and vertically centered  // greenCell
                        tblColCell.AddCssClass("dtc hcenter vcenter siw2 mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                    }
                    else if ((currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired))
                    {
                        // Space is expired
                        if (forceSingleRow == false)
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw2 mb0 PayGfx2"); // horizontally and vertically centered
                        cellBottomContent.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.BayExpiration_AsTimeSpan));
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw2 grayCell"); // horizontally and vertically centered //orangeCell
                        tblColCell.AddCssClass("dtc hcenter vcenter siw2 mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                    }
                    else
                    {
                        // Payment status is unknown
                        if (forceSingleRow == false)
                            cellTopContent.AddCssClass("hcenter vcenter statusIcon siw2 mb0 PayGfx3"); // horizontally and vertically centered
                        cellBottomContent.SetInnerText("" /*"N/A"*/);
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw2 grayCell"); // horizontally and vertically centered
                        tblColCell.AddCssClass("dtc hcenter vcenter siw2 mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                    }
                }

                // Column #4 of group row
                tblColCell = new BaseTagHelper("span");
                tblRow.Children.Add(tblColCell);

                cellInnerTbl = new BaseTagHelper("div");
                tblColCell.Children.Add(cellInnerTbl);
                cellInnerTbl.AddCssClass("dtNoLinesOrGaps siw2 mb0 fh"); // Display as table

                cellInnerTblTopRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblTopRow);
                //cellInnerTbl.Children.Add(cellInnerTblTopRow);
                cellInnerTblTopRow.AddCssClass("dtr np"); // Display as table-row

                cellInnerTblBottomRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblBottomRow);
                cellInnerTblBottomRow.AddCssClass("dtr fh np"); // Display as table-row

                cellTopContent = null;
                if (forceSingleRow == false)
                    cellTopContent = new BaseTagHelper("div");
                cellBottomContent = new BaseTagHelper("span");

                // We will only actually add this element to our object if we are using 2-row style of display
                if (forceSingleRow == false)
                    cellInnerTblTopRow.Children.Add(cellTopContent);
                cellInnerTblBottomRow.Children.Add(cellBottomContent);

                if (totalEnfVio >= 1)
                {
                    cellBottomContent.SetInnerText("Violation");
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("ctc statusIcon siw2 EnfGfx1"); // horizontally and vertically centered
                    cellBottomContent.AddCssClass("cbc siw2 redCell"); // horizontally and vertically centered
                    tblColCell.AddCssClass("cc siw2 redCell"); // Display as table-cell, both horizontally and vertically centered
                }
                else if (totalEnfVioSoon >= 1)
                {
                    cellBottomContent.SetInnerText("Soon");
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("ctc statusIcon siw2 EnfGfx2"); // horizontally and vertically centered
                    cellBottomContent.AddCssClass("cbc siw2 orangeCell"); // horizontally and vertically centered
                    tblColCell.AddCssClass("cc siw2 orangeCell"); // Display as table-cell, both horizontally and vertically centered
                }
                else
                {
                    cellBottomContent.SetInnerText("No");
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("ctc statusIcon siw2 EnfGfx3"); // horizontally and vertically centered
                    cellBottomContent.AddCssClass("cbc siw2 greenCell"); // horizontally and vertically centered
                    tblColCell.AddCssClass("cc siw2 greenCell"); // Display as table-cell, both horizontally and vertically centered
                }

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

        public static string SpaceDetails_WinCE(List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            // Output body of all data

            BaseTagHelper tbTable = new BaseTagHelper("span");  // div
            tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidthRENAMED mb0"); // Display as table
            tbTable.MergeAttribute("style", "color:white; cursor: pointer; min-width:200px; width:224px; max-width:240px; min-height:158px; font-size:1px; line-height:1px;"); //height: 158px; 

            // Loop for each space
            BaseTagHelper tbGroupCol = null;
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

                /*
                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcLeft hcenter vcenter siw colCellMP appleblueCell"); //appleblueCell Display as table-cell, both horizontally and vertically centered
                // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
                MeterAsset asset = customerCfg.GetMeterAsset_ForMeter(currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.MeterID.ToString());
                else
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.MeterID.ToString() + "-" + currentSpaceStatusModel.BayID.ToString());
                tbGroupCol.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");
                */

                // Get a local reference to the overstay info, if applicable
                OverstayViolationInfo overstayInfo = currentSpaceStatusModel.CurrentOverstayOrLatestDiscretionaryVio;

                // Space/Meter identity
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                tbGroupCol.SetInnerText("Space:");
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());

                // Enforcement Status
                if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.AlreadyCited)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Already Cited");
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP greenCell underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Good)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP greenCell underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP redCell underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP redCell underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("Discretionary");
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP redCell underlineCell");
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Unknown)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Enforcement:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP greenCell underlineCell");
                }

                // Violated overstay regulations
                if (((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) || (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                    && (overstayInfo != null))
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Vio Duration:");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText(FormatMobileElapsed(overstayInfo.DurationOfTimeBeyondStayLimits));


                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc txtL vcenter colCellMP NavyTextCell");
                    tbGroupCol.SetInnerText("Violated Regulation:");

                    /*
                    for (int loIdx = 0; loIdx < currentSpaceStatusModel.ViolatedRegulationPeriods.Count; loIdx++)
                    {
                        if (loIdx < currentSpaceStatusModel.ViolatedRegulationPeriods.Count - 1)
                        {
                            tbGroupCol = new BaseTagHelper("span");
                            tbTable.Children.Add(tbGroupCol);
                            tbGroupCol.AddCssClass("dtc txtR vcenter colCellMP whiteCell");
                        }
                        else
                        {
                            tbGroupCol = new BaseTagHelper("span");
                            tbTable.Children.Add(tbGroupCol);
                            tbGroupCol.AddCssClass("dtc txtR vcenter colCellMP whiteCell underlineCell");
                        }

                        tbGroupCol.SetInnerText(((DayOfWeek)(currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].StartTime.ToString("hh:mm:ss tt") + " - " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].EndTime.ToString("hh:mm:ss tt") + ", " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].MaxStayMinutes.ToString() + " mins");
                        tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                    }
                    */
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtc txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText(((DayOfWeek)(overstayInfo.Regulation_DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                        overstayInfo.Regulation_StartTime.ToString("hh:mm:ss tt") + " - " +
                        overstayInfo.Regulation_EndTime.ToString("hh:mm:ss tt") + ", " +
                        overstayInfo.Regulation_MaxStayMinutes.ToString() + " mins");
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }

                // Current overstay regulations
                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc txtL vcenter colCellMP NavyTextCell");
                tbGroupCol.SetInnerText("Current Regulations:");

                tbGroupCol = new BaseTagHelper("span");
                tbTable.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc txtR vcenter colCellMP whiteCell underlineCell");
                if (currentSpaceStatusModel.ActiveRegulationPeriod != null)
                {
                    tbGroupCol.SetInnerText(((DayOfWeek)(currentSpaceStatusModel.ActiveRegulationPeriod.DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.StartTime.ToString("hh:mm:ss tt") + " - " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.EndTime.ToString("hh:mm:ss tt") + ", " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.MaxStayMinutes.ToString() + " mins");
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupCol.SetInnerText("None");
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }


                // Vehicle sensor information
                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding))
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell");
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell");
                    tbGroupCol.SetInnerText("Occupied");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtR vcenter colCellMP NavyTextCell underlineCell italic");
                    tbGroupCol.SetInnerText("Duration:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell");
                    tbGroupCol.SetInnerText("Arrival:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerHtml("&nbsp;");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("ddd, MMM d"));
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell");
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell");
                    tbGroupCol.SetInnerText("Vacant");

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtR vcenter colCellMP NavyTextCell underlineCell italic");
                    tbGroupCol.SetInnerText("Duration:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell");
                    tbGroupCol.SetInnerText("Departure:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerHtml("&nbsp;");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("ddd, MMM d"));
                    tbGroupCol.MergeAttribute("style", "font-size: 10px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText("Unknown");
                }


                if (currentSpaceStatusModel.HasSensor == true)
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Space Type:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                    tbGroupCol.SetInnerText("Meter with Sensor");
                }
                else
                {
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                    tbGroupCol.SetInnerText("Space Type:");
                    tbGroupCol = new BaseTagHelper("span");
                    tbTable.Children.Add(tbGroupCol);
                    tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
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


                // Payment status information
                if (currentSpaceStatusModel.IsSensorOnly == false)
                {
                    string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);
                    if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                        tbGroupCol.SetInnerText("Expired Duration:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP redCell underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                        tbGroupCol.SetInnerText("Will Expire In:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP orangeCell underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                        tbGroupCol.SetInnerText("Remaining Time:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP greenCell underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                        tbGroupCol.SetInnerText("Remaining Grace:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP orangeCell underlineCell");
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational)
                    {
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcLeft txtL vcenter colCellMP NavyTextCell underlineCell");
                        tbGroupCol.SetInnerText("Payment Status:");
                        tbGroupCol = new BaseTagHelper("span");
                        tbTable.Children.Add(tbGroupCol);
                        tbGroupCol.AddCssClass("dtcRight txtR vcenter colCellMP whiteCell underlineCell");
                        tbGroupCol.SetInnerText("Unknown");
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

        public static string SpaceDetails(List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            // For Windows Mobile device, we must render different HTML than for other browsers
            bool IsWinCE = false;
            try
            {
                IsWinCE = Convert.ToBoolean(ViewData["IsWinCE"]);
            }
            catch { }
            if (IsWinCE)
                return SpaceDetails_WinCE(dataModel, ViewData);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string groupType = ViewData["groupType"].ToString();
            if (string.IsNullOrEmpty(groupType))
                groupType = "A"; // Default to A (Area) when not explicitly declared

            string viewType = ViewData["viewType"].ToString();
            if (string.IsNullOrEmpty(viewType))
                viewType = "L"; // Default to L (List) when not explicitly declared

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

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                BaseTagHelper tbGroupCol = null;

                // Add 2 columns: A field name and field value
                tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                tbGroupCol.SetInnerText("Space:");
                tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);

                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tbGroupCol.SetInnerText(MeterName);
                else
                    tbGroupCol.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString()); // currentSpaceStatusModel.MeterID.ToString() + "-" + currentSpaceStatusModel.BayID.ToString()


                // Enforcement Status
                if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.AlreadyCited)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Already Cited");
                    tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Good)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.MeterViolation)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.MergeAttribute("style", "background-color:red; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Violation");
                    tbGroupCol.MergeAttribute("style", "background-color:red; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Discretionary");
                    tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                }
                else if (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Unknown)
                {
                    /*
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Unknown");
                    */
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Enforcement Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("No Action");
                    tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                }

                /*
                // Violation duration, and Time-limit rules if its an overstay violation
                if ((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) && (currentSpaceStatusModel.OverstayBasedOnRuleDetail != null))
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Vio Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.DurationOfTimeBeyondStayLimits));

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow, true);
                    tbGroupCol.SetInnerText("Time Limit (minutes):");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow, true);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.OverstayBasedOnRuleDetail.MaxStayMinutes.ToString());
                    tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);


                    sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                    OuterPanel = new BaseTagHelper("div");
                    OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddFullRowColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(((DayOfWeek)(currentSpaceStatusModel.OverstayBasedOnRuleDetail.DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                        currentSpaceStatusModel.OverstayBasedOnRuleDetail.StartTime.ToString("hh:mm:ss tt") + " - "  +
                        currentSpaceStatusModel.OverstayBasedOnRuleDetail.EndTime.ToString("hh:mm:ss tt"));
                    tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);
                    sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                    OuterPanel = new BaseTagHelper("div");
                    OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                }
                */

                // Violated overstay regulations
                if (((currentSpaceStatusModel.BayEnforcementState == EnforcementState.OverstayViolation) || (currentSpaceStatusModel.BayEnforcementState == EnforcementState.Discretionary))
                    && (overstayInfo != null))
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Vio Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(FormatMobileElapsed(overstayInfo.DurationOfTimeBeyondStayLimits));

                    sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                    OuterPanel = new BaseTagHelper("div");
                    OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddFullRowLeftColumnForFieldValue(tbGroupRow, true);
                    tbGroupCol.SetInnerText("Violated Regulation:");

                    /*
                    for (int loIdx = 0; loIdx < currentSpaceStatusModel.ViolatedRegulationPeriods.Count; loIdx++)
                    {
                        if (loIdx < currentSpaceStatusModel.ViolatedRegulationPeriods.Count - 1)
                        {
                            sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                            OuterPanel = new BaseTagHelper("div");
                            OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                            tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                            tbGroupCol = AddFullRowRightColumnForFieldValue(tbGroupRow, true);
                        }
                        else
                        {
                            sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                            OuterPanel = new BaseTagHelper("div");
                            OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                            tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                            tbGroupCol = AddFullRowRightColumnForFieldValue(tbGroupRow, false);
                        }

                        tbGroupCol.SetInnerText(((DayOfWeek)(currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].StartTime.ToString("hh:mm:ss tt") + " - " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].EndTime.ToString("hh:mm:ss tt") + ", " +
                            currentSpaceStatusModel.ViolatedRegulationPeriods[loIdx].MaxStayMinutes.ToString() + " mins");
                        tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);
                    }
                    */
                    sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                    OuterPanel = new BaseTagHelper("div");
                    OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddFullRowRightColumnForFieldValue(tbGroupRow, false);
                    tbGroupCol.SetInnerText(((DayOfWeek)(overstayInfo.Regulation_DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                        overstayInfo.Regulation_StartTime.ToString("hh:mm:ss tt") + " - " +
                        overstayInfo.Regulation_EndTime.ToString("hh:mm:ss tt") + ", " +
                        overstayInfo.Regulation_MaxStayMinutes.ToString() + " mins");
                    tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);

                    sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                    OuterPanel = new BaseTagHelper("div");
                    OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                }

                // Current overstay regulations
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                tbGroupCol = AddFullRowLeftColumnForFieldValue(tbGroupRow, true);
                tbGroupCol.SetInnerText("Current Regulations:");
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");
                tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                tbGroupCol = AddFullRowRightColumnForFieldValue(tbGroupRow, false);
                if (currentSpaceStatusModel.ActiveRegulationPeriod != null)
                {
                    tbGroupCol.SetInnerText(((DayOfWeek)(currentSpaceStatusModel.ActiveRegulationPeriod.DayOfWeek)).ToString().Substring(0, 3).ToUpper() + " " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.StartTime.ToString("hh:mm:ss tt") + " - " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.EndTime.ToString("hh:mm:ss tt") + ", " +
                        currentSpaceStatusModel.ActiveRegulationPeriod.MaxStayMinutes.ToString() + " mins");
                    tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupCol.SetInnerText("None");
                    tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);
                }
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
                OuterPanel = new BaseTagHelper("div");
                OuterPanel.AddCssClass("dtNoLinesOrGapsBOGUS");

                // Vehicle sensor information

                if(currentSpaceStatusModel.HasSensor == false)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("NA");
                }

               else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding))
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Occupied");
                    //tbGroupCol.MergeAttribute("style", "background-color:yellow; color: black;", true);

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Occupied Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow, true);
                    tbGroupCol.SetInnerText("Arrival:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow, true);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("ddd, MMM d"));
                    tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);
                }
                else if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Vacant");

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Vacant Duration:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow, true);
                    tbGroupCol.SetInnerText("Departure:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow, true);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToShortTimeString());

                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText(currentSpaceStatusModel.BayVehicleSensingTimestamp.ToString("ddd, MMM d"));
                    tbGroupCol.MergeAttribute("style", "font-size: 12px; font-weight: 700;", true);
                }
                else
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Sensor Status:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Unknown");
                }

                if (currentSpaceStatusModel.HasSensor == true)
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Space Type:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                    tbGroupCol.SetInnerText("Meter with Sensor");
                }
                else
                {
                    tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                    tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                    tbGroupCol.SetInnerText("Space Type:");
                    tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
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

                // Payment status information
                if (currentSpaceStatusModel.IsSensorOnly == false)
                {
                    string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);
                    if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Expired)
                    {
                        tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                        tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                        tbGroupCol.SetInnerText("Expired Duration:");
                        tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                        tbGroupCol.MergeAttribute("style", "background-color:red; color: white;", true);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Critical)
                    {
                        tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                        tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                        tbGroupCol.SetInnerText("Will Expire In:");
                        tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                        tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Safe)
                    {
                        tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                        tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                        tbGroupCol.SetInnerText("Remaining Time:");
                        tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                        tbGroupCol.MergeAttribute("style", "background-color:green; color: white;", true);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Grace)
                    {
                        tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                        tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                        tbGroupCol.SetInnerText("Remaining Grace:");
                        tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                        tbGroupCol.SetInnerText(ExpiryTimeString);
                        tbGroupCol.MergeAttribute("style", "background-color:orange; color: white;", true);
                    }
                    else if (currentSpaceStatusModel.BayExpiryState == ExpiryState.Inoperational)
                    {
                        tbGroupRow = AddGroupRowForFieldsAndValues(OuterPanel);
                        tbGroupCol = AddLeftColumnForFieldName(tbGroupRow);
                        tbGroupCol.SetInnerText("Payment Status:");
                        tbGroupCol = AddRightColumnForFieldValue(tbGroupRow);
                        tbGroupCol.SetInnerText("Unknown");
                    }
                }

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
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
                sb.Append("> 1 month");
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

        private static BaseTagHelper AddGroupRowForFieldsAndValues(BaseTagHelper parent)
        {
            BaseTagHelper tbGroupRow = new BaseTagHelper("span");
            parent.Children.Add(tbGroupRow);
            tbGroupRow.AddCssClass("dtr");
            tbGroupRow.MergeAttribute("style", "padding:4px; width:100%; color: black; font-family: helvetica,arial,sans-serif; font-size: 14px; font-weight: 700;");
            return tbGroupRow;
        }

        private static BaseTagHelper AddLeftColumnForFieldName(BaseTagHelper parent, bool suppressBorder = false)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            if (suppressBorder == false)
                tbGroupCol.AddCssClass("dtc hcenter vcenter LCol NavyText"); //display table-cell horizontally and vertically centered
            else
                tbGroupCol.AddCssClass("dtcnb hcenter vcenter LCol NavyText"); //display table-cell horizontally and vertically centered
            /*tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px; text-align:left;"); //tbGroupCol.MergeAttribute("style", "width:33%;");*/
            return tbGroupCol;
        }

        private static BaseTagHelper AddRightColumnForFieldValue(BaseTagHelper parent, bool suppressBorder = false)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            if (suppressBorder == false)
                tbGroupCol.AddCssClass("dtc hcenter vcenter RCol"); //display table-cell horizontally and vertically centered
            else
                tbGroupCol.AddCssClass("dtcnb hcenter vcenter RCol"); //display table-cell horizontally and vertically centered
            /*tbGroupCol.MergeAttribute("style", "width:102px; padding-bottom:4px; text-align:right;"); //tbGroupCol.MergeAttribute("style", "width:33%;");*/
            return tbGroupCol;
        }

        private static BaseTagHelper AddFullRowColumnForFieldValue(BaseTagHelper parent, bool suppressBorder = false)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            if (suppressBorder == false)
                tbGroupCol.AddCssClass("hcenter vcenter FRCol"); //display table-cell horizontally and vertically centered
            else
                tbGroupCol.AddCssClass("hcenter vcenter FRCol"); //display table-cell horizontally and vertically centered
            return tbGroupCol;
        }

        private static BaseTagHelper AddFullRowLeftColumnForFieldValue(BaseTagHelper parent, bool suppressBorder = false)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            if (suppressBorder == false)
                tbGroupCol.AddCssClass("hcenter vcenter FRLeftCol NavyText"); //display table-cell horizontally and vertically centered
            else
                tbGroupCol.AddCssClass("dtcnb hcenter vcenter FRLeftCol NavyText"); //display table-cell horizontally and vertically centered
            return tbGroupCol;
        }

        private static BaseTagHelper AddFullRowRightColumnForFieldValue(BaseTagHelper parent, bool suppressBorder = false)
        {
            BaseTagHelper tbGroupCol = new BaseTagHelper("span");
            parent.Children.Add(tbGroupCol);
            if (suppressBorder == false)
                tbGroupCol.AddCssClass("dtc hcenter vcenter FRRightCol"); //display table-cell horizontally and vertically centered
            else
                tbGroupCol.AddCssClass("dtcnb hcenter vcenter FRRightCol"); //display table-cell horizontally and vertically centered
            return tbGroupCol;
        }

        public static string SimpleValidationSummary(HtmlHelper helper, string validationMessage = "")
        {
            string retVal = "";
            if (helper.ViewData.ModelState.IsValid)
                return "";
            retVal += "<div class='input-validation-error validation-summary-errors'><span>";
            if (!String.IsNullOrEmpty(validationMessage))
                retVal += helper.Encode(validationMessage);
            retVal += "</span>";

            /*
            retVal += "<div class='validation-summary-errors-simple'>";
            foreach (var key in helper.ViewData.ModelState.Keys)
            {
                foreach (var err in helper.ViewData.ModelState[key].Errors)
                    retVal += "<p>" + helper.Encode(err.ErrorMessage) + "</p>";
            }
            retVal += "</div></div>";
            */

            foreach (var key in helper.ViewData.ModelState.Keys)
            {
                foreach (var err in helper.ViewData.ModelState[key].Errors)
                    retVal += "<div class='validation-summary-errors-simple'>" + helper.Encode("* " + err.ErrorMessage) + "</div>";
            }

            return retVal.ToString();
        }

        public static bool DataModelHasPAMData(List<SpaceStatusModel> dataModel)
        {
            // If we have any item that is not a "Sensor Only" element, then we must be dealing with PAM/Payment stuff too
            bool result = false;
            if (dataModel == null)
                return result;

            foreach (SpaceStatusModel nextModel in dataModel)
            {
                if (nextModel.IsSensorOnly == false)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static string GroupSummary_CSSDynamicValue(string cssClassName, bool forSmallDevice, List<SpaceStatusModel> dataModel)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            if (string.Compare("pageColWidth", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "240px";
                    else
                        return "320px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "180px";
                    else
                        return "240px";
                }
            }

            if (string.Compare("sectionWidth", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "60px";
                    else
                        return "80px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "60px";
                    else
                        return "80px";
                }
            }

            if (string.Compare("sectionWidth1", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "30px";
                    else
                        return "50px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "30px";
                    else
                        return "50px";
                }
            }

            if (string.Compare("sectionWidth2", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "50px";
                    else
                        return "70px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "50px";
                    else
                        return "70px";
                }
            }

            if (string.Compare("pageColTableWidth", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "236px";
                    else
                        return "316px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "176px";
                    else
                        return "236px";
                }
            }

            if (string.Compare("siw", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "59px";
                    else
                        return "79px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "59px";
                    else
                        return "79px";
                }
            }

            if (string.Compare("siw1", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "29px";
                    else
                        return "49px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "29px";
                    else
                        return "49px";
                }
            }
            if (string.Compare("siw2", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "49px";
                    else
                        return "69px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "49px";
                    else
                        return "69px";
                }
            }


            
            if (includePAMElements)
            {
                if (string.Compare("mediaWidth_VerySmall", cssClassName, true) == 0)
                    return "300px";

                if (string.Compare("mediaWidth_Small", cssClassName, true) == 0)
                    return "301px";

                if (string.Compare("mediaWidth_1Column", cssClassName, true) == 0)
                    return "332px";

                if (string.Compare("mediaWidth_2Column", cssClassName, true) == 0)
                    return "669px";

                if (string.Compare("mediaWidth_3Column", cssClassName, true) == 0)
                    return "997px";

                if (string.Compare("mediaWidth_4Column", cssClassName, true) == 0)
                    return "1325px";

                if (string.Compare("mediaWidth_4Column_MaxWidth", cssClassName, true) == 0)
                    return "1332px";
            }
            else
            {
                if (string.Compare("mediaWidth_VerySmall", cssClassName, true) == 0)
                    return "300px";

                if (string.Compare("mediaWidth_Small", cssClassName, true) == 0)
                    return "301px";

                if (string.Compare("mediaWidth_1Column", cssClassName, true) == 0)
                    return "252px";

                if (string.Compare("mediaWidth_2Column", cssClassName, true) == 0)
                    return "509px";

                if (string.Compare("mediaWidth_3Column", cssClassName, true) == 0)
                    return "757px";

                if (string.Compare("mediaWidth_4Column", cssClassName, true) == 0)
                    return "1005px";

                if (string.Compare("mediaWidth_4Column_MaxWidth", cssClassName, true) == 0)
                    return "1012px";
            }
            
            return "";
        }

        public static string StatusIconDisplayForDisplayStyle(bool forSmallDevice, string displayStyle /*ViewData["displayStyle"]*/)
        {
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (Graphic and text normally, but text-only on small devices)
            bool forceSingleRow = (string.Compare(displayStyle, "1", true) == 0);
            bool forceDoubleRow = (string.Compare(displayStyle, "2", true) == 0);

            if (forceSingleRow)
                return "display:none;";
            else if (forceDoubleRow)
                return "display:block;";
            else
            {
                // Dynamic -- if we're targeting a small device, it will be hidden, otherwise its displayed as a block
                if (forSmallDevice)
                    return "display:none;";
                else
                    return "display:block;";
            }
        }


        public static string SensorStatusDemo_CSSDynamicValue(string cssClassName, bool forSmallDevice, List<SpaceStatusModel> dataModel)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            if (string.Compare("pageColWidth", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "160px";
                    else
                        return "240px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "100px";
                    else
                        return "160px";
                }
            }

            if (string.Compare("sectionWidth", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "60px";
                    else
                        return "80px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "60px";
                    else
                        return "80px";
                }
            }

            if (string.Compare("pageColTableWidth", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "156px";
                    else
                        return "236px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "96x";
                    else
                        return "156px";
                }
            }

            if (string.Compare("siw", cssClassName, true) == 0)
            {
                if (includePAMElements)
                {
                    if (forSmallDevice == true)
                        return "59px";
                    else
                        return "79px";
                }
                else
                {
                    if (forSmallDevice == true)
                        return "59px";
                    else
                        return "79px";
                }
            }

            if (includePAMElements)
            {
                if (string.Compare("mediaWidth_VerySmall", cssClassName, true) == 0)
                    return "300px";

                if (string.Compare("mediaWidth_Small", cssClassName, true) == 0)
                    return "301px";

                if (string.Compare("mediaWidth_1Column", cssClassName, true) == 0)
                    return "332px";

                if (string.Compare("mediaWidth_2Column", cssClassName, true) == 0)
                    return "669px";

                if (string.Compare("mediaWidth_3Column", cssClassName, true) == 0)
                    return "997px";

                if (string.Compare("mediaWidth_4Column", cssClassName, true) == 0)
                    return "1325px";

                if (string.Compare("mediaWidth_4Column_MaxWidth", cssClassName, true) == 0)
                    return "1332px";
            }
            else
            {
                if (string.Compare("mediaWidth_VerySmall", cssClassName, true) == 0)
                    return "300px";

                if (string.Compare("mediaWidth_Small", cssClassName, true) == 0)
                    return "301px";

                if (string.Compare("mediaWidth_1Column", cssClassName, true) == 0)
                    return "252px";

                if (string.Compare("mediaWidth_2Column", cssClassName, true) == 0)
                    return "509px";

                if (string.Compare("mediaWidth_3Column", cssClassName, true) == 0)
                    return "757px";

                if (string.Compare("mediaWidth_4Column", cssClassName, true) == 0)
                    return "1005px";

                if (string.Compare("mediaWidth_4Column_MaxWidth", cssClassName, true) == 0)
                    return "1012px";
            }

            return "";
        }

        public static string SensorStatusDemo(UrlHelper Url, List<SpaceStatusModel> dataModel, ViewDataDictionary ViewData)
        {
            bool includePAMElements = SpaceStatusHelpers.DataModelHasPAMData(dataModel);

            StringBuilder sb = new StringBuilder();

            CustomerConfig customerCfg = (ViewData["CustomerCfg"] as CustomerConfig);

            string displayStyle = string.Empty;
            if (ViewData["displayStyle"] != null)
                displayStyle = ViewData["displayStyle"].ToString();
            if (string.IsNullOrEmpty(displayStyle))
                displayStyle = "D"; // Default to "Dynamic" (Graphic and text normally, but text-only on small devices)
            bool forceSingleRow = (string.Compare(displayStyle, "1", true) == 0);
            bool forceDoubleRow = (string.Compare(displayStyle, "2", true) == 0);

            int countOfGroups = dataModel.Count;

            //BaseTagHelper tbTitleBlock_WholePage = new BaseTagHelper("div"); // Regular block

            // Loop for each space
            foreach (SpaceStatusModel currentSpaceStatusModel in dataModel)
            {
                int totalVacant = 0;
                int totalOccupied = 0;
                int totalOtherVS = 0;

                if (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty)
                {
                    totalVacant++;
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    totalOccupied++;
                }
                else
                {
                    totalOtherVS++;
                }


                // The TagBuilder class built-into the .NET framework is pretty nice, but it can get pretty tedious 
                // trying to properly handle the InnerHtml if there are a lot of nested tags.  To alleviate this problem,
                // I created a "BaseTagHelper" class that is very similar to the TagBuilder, but it hides the InnerHtml
                // property and replaces it with a "Children" property which is used to hold a a collection of nested
                // BaseTagHelper objects. When you call the ToString() method on BaseTagHelper, the InnerHtml will be
                // rendered automatically based on the children, and handles any level of nesting.  This makes the whole
                // process simpler, because you can create a BaseTagHelper, add as many children (and grand-children) as 
                // needed, then call one ToString() method that will render the main tag and all of its nested children!

                // Create tag for outer boundary of this entire block
                BaseTagHelper OuterPanel = new BaseTagHelper("span"); // Inline-block
                OuterPanel.AddCssClass("pageColWidth grpOutPnl");

                BaseTagHelper InnerPanel = new BaseTagHelper("span"); // Inline-block
                InnerPanel.AddCssClass("pageColTableWidth grpInPnl");
                OuterPanel.Children.Add(InnerPanel);

                BaseTagHelper tbTable = new BaseTagHelper("div");
                InnerPanel.Children.Add(tbTable);
                tbTable.AddCssClass("dtNoLinesOrGaps pageColTableWidth mb0"); // Display as table
                tbTable.MergeAttribute("style", "color:white; cursor: pointer;"); // width:230px; margin-bottom:0px;

                string targetID = string.Empty;
                targetID = "m" + currentSpaceStatusModel.MeterID.ToString() + "s" + currentSpaceStatusModel.BayID.ToString();
                tbTable.MergeAttribute("ID", targetID);
                /*tbTable.MergeAttribute("onclick", "drilldownToSpecificTarget(\"" + targetID + "\");");*/

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tblRow = new BaseTagHelper("span");
                tbTable.Children.Add(tblRow);
                tblRow.AddCssClass("dtr appleblueCell np"); // Display as table-row, background is iPhone-style blue

                // Column #1 of group row
                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tblColCell = new BaseTagHelper("span");
                tblRow.Children.Add(tblColCell);
                tblColCell.AddCssClass("dtc hcenter vcenter siw colCellMP"); // Display as table-cell, both horizontally and vertically centered
                // The display of Space# will vary depending on if its for MultiSpace or SingleSpace meter
                MeterAsset asset = CustomerLogic.CustomerManager.GetMeterAsset(customerCfg, currentSpaceStatusModel.MeterID);
                String MeterName = CustomerLogic.CustomerManager.GetMeterAssetName(customerCfg, currentSpaceStatusModel.MeterID);
                if (asset.MeterGroupID <= 0) // SSM
                    tblColCell.SetInnerText(MeterName);
                else
                    tblColCell.SetInnerText(MeterName + "-" + currentSpaceStatusModel.BayID.ToString());

                // Column #2 of group row
                tblColCell = new BaseTagHelper("span");
                tblRow.Children.Add(tblColCell);

                BaseTagHelper cellInnerTbl = new BaseTagHelper("div");
                tblColCell.Children.Add(cellInnerTbl);
                cellInnerTbl.AddCssClass("dtNoLinesOrGaps siw mb0 fh"); // Display as nested table

                BaseTagHelper cellInnerTblTopRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblTopRow);
                cellInnerTblTopRow.AddCssClass("dtr np"); // Display as table-row

                BaseTagHelper cellInnerTblBottomRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblBottomRow);
                cellInnerTblBottomRow.AddCssClass("dtr fh np"); // Display as table-row

                BaseTagHelper cellTopContent = null;
                if (forceSingleRow == false)
                    cellTopContent = new BaseTagHelper("div");
                BaseTagHelper cellBottomContent = new BaseTagHelper("span");

                // We will only actually add this element to our object if we are using 2-row style of display
                if (forceSingleRow == false)
                    cellInnerTblTopRow.Children.Add(cellTopContent);
                cellInnerTblBottomRow.Children.Add(cellBottomContent);

                if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Occupied) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.MeterFeeding) ||
                    (currentSpaceStatusModel.BayOccupancyState == OccupancyState.Violation))
                {
                    // Space is occupied
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx1"); // horizontally and vertically centered
                    cellBottomContent.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) > 30)
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw redCell notBold"); // horizontally and vertically centered // orangecell // purpleCell
                    else
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw redCell"); // horizontally and vertically centered // orangecell // purpleCell
                    tblColCell.AddCssClass("dtc hcenter vcenter siw mb0 fh colCellMP redCell"); // Display as table-cell, both horizontally and vertically centered //orangeCell
                }
                else if ((currentSpaceStatusModel.BayOccupancyState == OccupancyState.Empty))
                {
                    // Space is empty
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx2"); // horizontally and vertically centered
                    cellBottomContent.SetInnerText(FormatMobileElapsed(currentSpaceStatusModel.TimeSinceLastInOut));
                    if (Math.Abs(currentSpaceStatusModel.TimeSinceLastInOut.Days) > 30)
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw greenCell notBold"); // horizontally and vertically centered // greenCell
                    else
                        cellBottomContent.AddCssClass("dtc hcenter vcenter siw greenCell"); // horizontally and vertically centered // greenCell
                    tblColCell.AddCssClass("dtc hcenter vcenter siw mb0 fh colCellMP greenCell"); // Display as table-cell, both horizontally and vertically centered //greenCell
                }
                else
                {
                    // Unknown or unreliable info, so will be gray color
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("hcenter vcenter statusIcon siw mb0 VSGfx3"); // horizontally and vertically centered
                    cellBottomContent.SetInnerText("" /*"N/A"*/);
                    cellBottomContent.AddCssClass("dtc hcenter vcenter siw grayCell"); // horizontally and vertically centered
                    tblColCell.AddCssClass("dtc hcenter vcenter siw mb0 fh colCellMP grayCell"); // Display as table-cell, both horizontally and vertically centered
                }

                // Column #3 of group row
                includePAMElements = false;
                if (includePAMElements)
                {
                }

                // Column #4 of group row
                /*
                tblColCell = new BaseTagHelper("span");
                tblRow.Children.Add(tblColCell);

                cellInnerTbl = new BaseTagHelper("div");
                tblColCell.Children.Add(cellInnerTbl);
                cellInnerTbl.AddCssClass("dtNoLinesOrGaps siw mb0 fh"); // Display as table

                cellInnerTblTopRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblTopRow);
                //cellInnerTbl.Children.Add(cellInnerTblTopRow);
                cellInnerTblTopRow.AddCssClass("dtr np"); // Display as table-row

                cellInnerTblBottomRow = new BaseTagHelper("span");
                cellInnerTbl.Children.Add(cellInnerTblBottomRow);
                cellInnerTblBottomRow.AddCssClass("dtr fh np"); // Display as table-row

                cellTopContent = null;
                if (forceSingleRow == false)
                    cellTopContent = new BaseTagHelper("div");
                cellBottomContent = new BaseTagHelper("span");

                // We will only actually add this element to our object if we are using 2-row style of display
                if (forceSingleRow == false)
                    cellInnerTblTopRow.Children.Add(cellTopContent);
                cellInnerTblBottomRow.Children.Add(cellBottomContent);

                {
                    cellBottomContent.SetInnerText("No");
                    if (forceSingleRow == false)
                        cellTopContent.AddCssClass("ctc statusIcon siw EnfGfx3"); // horizontally and vertically centered
                    cellBottomContent.AddCssClass("cbc siw greenCell"); // horizontally and vertically centered
                    tblColCell.AddCssClass("cc siw greenCell"); // Display as table-cell, both horizontally and vertically centered
                }
                */

                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }
    
    }

}