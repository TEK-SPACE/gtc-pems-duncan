using System;
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
    //DEBUG:  Review this website for info on how to better do this?  http://msdn.microsoft.com/en-us/vs2010trainingcourse_aspnetmvc3formsandvalidation_topic3.aspx


    public static class HtmlHelpers
    {

        // DEBUG: Dataset-based routine --> Will be deprecated
        public static string ClusterStatusBlock(DataSet srcDataSet, CustomerConfig customerCfg)
        {
            StringBuilder sb = new StringBuilder();
            DataTable tbl = srcDataSet.Tables["Meter"];

            // Loop through each meter
            foreach (DataRow row in tbl.Rows)
            {
                sb.AppendLine("<div class=\" rounded-corners NewMeterBlock\"> ");
                sb.AppendLine("<div class=\"NewMeterTitle\">Meter " + row["ID"].ToString());
                sb.AppendLine("<span class=\"NewMeterSubTitle\"> [Idle For: " + row["imin"].ToString() + " minutes]  [Last Updated: " + row["upTSString"].ToString() + "]");
                sb.AppendLine("</span> ");
                sb.AppendLine("</div> ");

                // Loop through each bay of current meter
                DataRow[] MeterBays = srcDataSet.Tables["Bay"].Select("Meter_Id = " + row["Meter_Id"]);
                foreach (DataRow bayRow in MeterBays)
                {
                    sb.AppendLine("<div class=\"NewBayBlockGavin\"> ");
                    sb.AppendLine("<div class=\"NewBayTitle\">" + bayRow["ID"].ToString() + "</div> ");

                    // Determine the payment expiration state
                    Duncan.PEMS.SpaceStatus.Models.ExpiryState es = Duncan.PEMS.SpaceStatus.Models.ExpiryState.Inoperational;
                    try
                    {
                        if (bayRow["ExpiryState"] != DBNull.Value)
                        {
                            es = (Duncan.PEMS.SpaceStatus.Models.ExpiryState)bayRow["ExpiryState"];
                        }
                    }
                    catch
                    {
                        es = Duncan.PEMS.SpaceStatus.Models.ExpiryState.Inoperational;
                    }

                    // Output appropriate HTML elements for the expiration state
                    if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Safe)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeSafe\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Expired)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeExpired\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Critical)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeCritical\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Grace)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeGracePeriod\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else
                    {
                        sb.AppendLine("<div class=\"CenteredBlock NewBayExpiryTimeInoperational\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }

                    /////////////////////////////

                    // Get the vehicle occupancy state
                    Duncan.PEMS.SpaceStatus.Models.OccupancyState os = Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable;
                    try
                    {
                        if (bayRow["OccupancyState"] != DBNull.Value)
                            os = (Duncan.PEMS.SpaceStatus.Models.OccupancyState)bayRow["OccupancyState"];
                    }
                    catch
                    {
                        os = Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable;
                    }

                    // Output appropriate HTML elements for the occupancy state
                    switch (os)
                    {
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Empty:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyEmpty\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOut\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Occupied:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyOccupied\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\">" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyOutOfDate\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Violation:
                            sb.AppendLine("<div class=\"CenteredBlock Violation\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.MeterFeeding:
                            sb.AppendLine("<div class=\"CenteredBlock MeterFeeding\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Unknown:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                    }

                    sb.AppendLine("</div> "); // End of Bay block
                }

                sb.AppendLine("</div> "); // End of Meter block
            }

            // Finalize the result from the string builder contents
            return sb.ToString();
        }

        // Strongly type input instead of dataset
        public static string ClusterStatusBlock(List<SpaceStatusModel> modelForView, CustomerConfig customerCfg)
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

                sb.AppendLine("<div class=\" rounded-corners NewMeterBlock\"> ");
                sb.AppendLine("<div class=\"NewMeterTitle\">Meter " + currentSpaceStatusModel.MeterID.ToString());
                sb.AppendLine("<span class=\"NewMeterSubTitle\"> [Idle For: " + currentSpaceStatusModel.Meter_imin.ToString() + " minutes]  [Last Updated: " +
                    currentSpaceStatusModel.Meter_upTS.ToString() + "]"); // DEBUG: We need formatted string like:  row["upTSString"].ToString() 
                sb.AppendLine("</span> ");
                sb.AppendLine("</div> ");

                // Loop through each bay of current meter
                for (int loIdx = 0; loIdx < itemIndexesForCurrentMeter.Count; loIdx++)
                {
                    currentSpaceStatusModel = modelForView[itemIndexesForCurrentMeter[loIdx]];

                    sb.AppendLine("<div class=\"NewBayBlockGavin\"> ");
                    sb.AppendLine("<div class=\"NewBayTitle\">" + currentSpaceStatusModel.BayID.ToString() + "</div> ");

                    // Determine the payment expiration state
                    Duncan.PEMS.SpaceStatus.Models.ExpiryState es = currentSpaceStatusModel.BayExpiryState;

                    string ExpiryTimeString = currentSpaceStatusModel.GetExpiryTimeString(customerCfg);

                    // Output appropriate HTML elements for the expiration state
                    if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Safe)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeSafe\">" + ExpiryTimeString + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Expired)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeExpired\">" + ExpiryTimeString + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Critical)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeCritical\">" + ExpiryTimeString + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Grace)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeGracePeriod\">" + ExpiryTimeString + "</div> ");
                    }
                    else
                    {
                        sb.AppendLine("<div class=\"CenteredBlock NewBayExpiryTimeInoperational\">" + ExpiryTimeString + "</div> ");
                    }

                    /////////////////////////////

                    // Get the vehicle occupancy state
                    Duncan.PEMS.SpaceStatus.Models.OccupancyState os = currentSpaceStatusModel.BayOccupancyState;

                    // Output appropriate HTML elements for the occupancy state
                    string TimeSinceLastInOut = BindShortTimeSpan(currentSpaceStatusModel.TimeSinceLastInOut, currentSpaceStatusModel.BayOccupancyState, customerCfg);
                    string LastInOutTime = BindTimeOfDay(currentSpaceStatusModel.BayVehicleSensingTimestamp, currentSpaceStatusModel.BayOccupancyState, customerCfg);
                    switch (os)
                    {
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Empty:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyEmpty\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOut\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Occupied:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyOccupied\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\">" + LastInOutTime + "</div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyOutOfDate\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Violation:
                            sb.AppendLine("<div class=\"CenteredBlock Violation\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.MeterFeeding:
                            sb.AppendLine("<div class=\"CenteredBlock MeterFeeding\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Unknown:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + TimeSinceLastInOut + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + LastInOutTime + "</span> </div>");
                            break;
                    }

                    sb.AppendLine("</div> "); // End of Bay block
                }

                sb.AppendLine("</div> "); // End of Meter block
            }

            // Finalize the result from the string builder contents
            return sb.ToString();
        }

        public static string MeterStatusBlock()
        {
            /*
            // Create tag builder
            var builder = new TagBuilder("img");
            
            // Create valid id
            builder.GenerateId(id);

            // Add attributes
            builder.MergeAttribute("src", url);
            builder.MergeAttribute("alt", alternateText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            // Render tag
            return builder.ToString(TagRenderMode.SelfClosing);
            */
            return "not implemented yet";
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


        public static string BindShortTimeSpan(DataRow Container, string Item, string OccupancyStateItem, CustomerConfig customerCfg)
        {
            string ReturnValue = string.Empty;
            try
            {
                if (Container[OccupancyStateItem] == DBNull.Value)
                    ReturnValue = "N/A";
                else if (((Duncan.PEMS.SpaceStatus.Models.OccupancyState)(Container[OccupancyStateItem])) == Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate)
                    ReturnValue = "N/A";
                else if (((Duncan.PEMS.SpaceStatus.Models.OccupancyState)(Container[OccupancyStateItem])) == Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable)
                    ReturnValue = "N/A";
                else
                {
                    if (Container[Item] == System.DBNull.Value)
                        ReturnValue = "N/A";
                    else
                        ReturnValue = SpaceStatusProvider.FormatShortTimeSpan((TimeSpan)Container[Item], customerCfg);
                }
            }
            catch
            {
                return "N/A";
            }

            return ReturnValue;
        }

        public static string BindTimeOfDay(DataRow Container, string Item, string OccupancyStateItem, CustomerConfig customerCfg)
        {
            string ReturnValue;
            try
            {
                if (Container[OccupancyStateItem] == DBNull.Value)
                    ReturnValue = "N/A";
                else if (((OccupancyState)(Container[OccupancyStateItem])) == OccupancyState.OutOfDate)
                    ReturnValue = "N/A";
                else if (((OccupancyState)(Container[OccupancyStateItem])) == OccupancyState.NotAvailable)
                    ReturnValue = "N/A";
                else
                {
                    if (Container[Item] == System.DBNull.Value)
                        ReturnValue = "N/A";
                    else
                        ReturnValue = Convert.ToDateTime(Container[Item]).ToString("hh:mm:ss tt"); // 24-hour format
                }
            }
            catch
            {
                return "N/A";
            }

            return ReturnValue;
        }

        public static string MeterSelectionTreeView(UrlHelper Url, CustomerConfig customerCfg)
        {
            // We will use ID attributes for the anchors in the HTML.  Clusters will be prefixed with "tCL_" (tree cluster) or "tME_" (tree meter), and then followed by the ClusterID or MeterID

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ul> ");

            // Get a list of the unique cluster IDs
            List<int> uniqueClusterIDs = new List<int>();
            foreach (PAMClusterAsset clusterAsset in CustomerLogic.CustomerManager.GetClusterAssetsForCustomer(customerCfg))
            {
                if (uniqueClusterIDs.IndexOf(clusterAsset.ClusterID) == -1)
                    uniqueClusterIDs.Add(clusterAsset.ClusterID);
            }
            uniqueClusterIDs.Sort();

            // DEBUG: We are now building the tree based on our asset inventory acquired from the database instead of legacy cluster list from XML file
            foreach (int clusterID in uniqueClusterIDs)
            {
                // Begin group-level node
                sb.AppendLine("<li rel=\"folder\">"); // This is telling the treeview to display as a "folder", but we will customize the icon to be a group of meters instead

                // NOTE: Instead of a standard link, we will use the onclick and/or selection event of the js treeviewto update our partial view contents instead of loading a complete page....
                //       This allows us to do things in a bit more complex manner, such as using javascript to show a "busy" indicator prior to sending off request to server,
                //       and also updating just a portion of the webpage with the response (similar to an AJAX-powered UpdatePanel in legacy WebForms platform)

                // Begin an Anchor node, and use "#" for the href, which means its an internal page link (top of current page in this case)
                // We will also give this anchor an ID attribute that tells us what ClusterID it is associated with
                sb.AppendLine("<a id=\"" + "tCL_" + clusterID.ToString() + "\" href=\"#\"");
                sb.Append(">");

                // Add the anchor display text, then close the anchor node
                sb.AppendLine(clusterID.ToString() + "</a>");

                // Now begin a sub-group section for the items in the group
                sb.AppendLine("<ul>");

                foreach (MeterAsset mtrAsset in CustomerLogic.CustomerManager.GetMeterAssetsForCustomer(customerCfg))
                {
                    // Skip this if its not for the same cluster
                    if (mtrAsset.PAMClusterID != clusterID)
                    {
                        continue;
                    }

                    // DEBUG: Need to know if the meter is a Single-space or Multi-space, and we can use an appropriate icon, customize the URL, etc...
                    bool isMSM = false;
                    if (mtrAsset.MeterGroupID >= 1)
                        isMSM = true;

                    if (isMSM)
                        sb.AppendLine("<li rel=\"MSMNode\">");
                    else
                        sb.AppendLine("<li rel=\"file\">");

                    // Begin an Anchor node, and use "#" for the href, which means its an internal page link (top of current page in this case)
                    // We will also give this anchor an ID attribute that tells us what MeterID it is associated with
                    sb.AppendLine("<a href=\"#\" ");
                    sb.AppendLine("<a id=\"" + "tME_" + mtrAsset.MeterID.ToString() + "\" href=\"#\"");
                    sb.Append(">");

                    // Add the anchor display text, then close the anchor node
                    sb.AppendLine(mtrAsset.MeterID.ToString() + "</a>");
                    sb.AppendLine("</li>");
                }

                // End the sub-group node 
                sb.AppendLine("</ul>");

                // End group-level node
                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ul> ");

            // Finalize the result from the string builder contents
            return sb.ToString();

            #region Deprecated
            /*
            // DEBUG: This should be deprecated and we will use the customer assets list instead of the legacy cluster list
            foreach (DataRow clusterRow in customerCfg.ClusterList.Tables["Cluster"].Rows)
            {
                // Begin group-level node
                sb.AppendLine("<li rel=\"folder\">"); // This is telling the treeview to display as a "folder", but we will customize the icon to be a group of meters instead

                // NOTE: Instead of a standard link, we will use the onclick and/or selection event of the js treeviewto update our partial view contents instead of loading a complete page....
                //       This allows us to do things in a bit more complex manner, such as using javascript to show a "busy" indicator prior to sending off request to server,
                //       and also updating just a portion of the webpage with the response (similar to an AJAX-powered UpdatePanel in legacy WebForms platform)

                // Begin an Anchor node, and use "#" for the href, which means its an internal page link (top of current page in this case)
                // We will also give this anchor an ID attribute that tells us what ClusterID it is associated with
                sb.AppendLine("<a id=\"" + "tCL_" + clusterRow["ID"].ToString() + "\" href=\"#\"");

                // We are no longer creating an onclick event for each anchor.  Instead, the whole treeview will have a "select_node.jstree" event
                // that generically handles the navigation based on the selected item.
                //// // Add JavaScript actions for the anchor's onclick event
                //// // Note: We are putting some info in a "targetID" JavaScript variable so the refresh timer function has knowledge of the current value too
                ////sb.AppendLine("onclick=\"stopStatusRefreshTimer();" +
                ////    " $('#UpdatingBlock').css('visibility', 'visible'); " +
                ////    " targetID = 'CL:" + clusterRow["ID"].ToString() + "';" +
                ////    " $('#partial1').load('" +
                ////    Url.Content("~/SpaceStatus/allofthem_partialview") + "?targetID=' + encodeURIComponent(targetID));" + 
                ////    "\"" +
                ////    ">");
                sb.Append(">");

                // Add the anchor display text, then close the anchor node
                sb.AppendLine(clusterRow["ID"].ToString() + "</a>");

                // Now begin a sub-group section for the items in the group
                sb.AppendLine("<ul>");

                foreach (DataRow meterRow in customerCfg.ClusterList.Tables["Meter"].Rows)
                {
                    // Skip this if its not for the same cluster
                    if (Convert.ToInt32(meterRow["Cluster_id"]) != Convert.ToInt32(clusterRow["Cluster_id"]))
                    {
                        continue;
                    }

                    // DEBUG: Need to know if the meter is a Single-space or Multi-space, and we can use an appropriate icon, customize the URL, etc...
                    bool isMSM = false;
                    if (Convert.ToInt32(meterRow["ID"]) == 7001)
                        isMSM = true;

                    if (isMSM)
                        sb.AppendLine("<li rel=\"MSMNode\">");
                    else
                        sb.AppendLine("<li rel=\"file\">");

                    // Begin an Anchor node, and use "#" for the href, which means its an internal page link (top of current page in this case)
                    // We will also give this anchor an ID attribute that tells us what MeterID it is associated with
                    sb.AppendLine("<a href=\"#\" ");
                    sb.AppendLine("<a id=\"" + "tME_" + meterRow["ID"].ToString() + "\" href=\"#\"");

                    // We are no longer creating an onclick event for each anchor.  Instead, the whole treeview will have a "select_node.jstree" event
                    // that generically handles the navigation based on the selected item.
                    //// // Add JavaScript actions for the anchor's onclick event
                    //// // Note: We are putting some info in a "targetID" JavaScript variable so the refresh timer function has knowledge of the current value too
                    //// sb.AppendLine("onclick=\"stopStatusRefreshTimer();" +
                    ////    " $('#UpdatingBlock').css('visibility', 'visible'); " +
                    ////    " targetID = 'MID:" + meterRow["ID"].ToString() + "';" +
                    ////    " $('#partial1').load('" +
                    ////    Url.Content("~/SpaceStatus/allofthem_partialview") + "?targetID=' + encodeURIComponent(targetID));\"" +
                    ////    ">");
                    sb.Append(">");

                    // Add the anchor display text, then close the anchor node
                    sb.AppendLine(meterRow["ID"].ToString() + "</a>");

                    sb.AppendLine("</li>");
                }

                // End the sub-group node 
                sb.AppendLine("</ul>");

                // End group-level node
                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ul> ");

            // Finalize the result from the string builder contents
            return sb.ToString();
             */
            #endregion
        }

        public static string MobileMeterSummaryBlock(DataSet srcDataSet, CustomerConfig customerCfg)
        {
            StringBuilder sb = new StringBuilder();
            DataTable tbl = srcDataSet.Tables["Meter"];

            // Loop through each meter
            foreach (DataRow row in tbl.Rows)
            {
                sb.AppendLine("<div class=\"rounded-corners NewMeterBlock\"> ");

                sb.AppendLine("<div> ");

                sb.AppendLine("<div class=\"todo\" style=\"float:left;\">Meter ID");
                sb.AppendLine("</div> ");

                sb.AppendLine("<div class=\"todo\" style=\"float:right; text-indent: 14px;\"># of Spaces");
                sb.AppendLine("</div> ");

                sb.AppendLine("</div> ");

                sb.AppendLine("<div> ");

                sb.AppendLine("<div class=\"todo\" style=\"float:left;\">" + row["ID"].ToString());
                sb.AppendLine("</div> ");

                // Loop through each bay of current meter
                DataRow[] MeterBays = srcDataSet.Tables["Bay"].Select("Meter_Id = " + row["Meter_Id"]);

                sb.AppendLine("<div class=\"todo\" style=\"float:right;\">" + MeterBays.Length.ToString());
                sb.AppendLine("</div> ");

                sb.AppendLine("</div> ");

                /*
                foreach (DataRow bayRow in MeterBays)
                {
                    sb.AppendLine("<div class=\"NewBayBlockGavin\"> ");
                    sb.AppendLine("<div class=\"NewBayTitle\">" + bayRow["ID"].ToString() + "</div> ");

                    // Determine the payment expiration state
                    Duncan.PEMS.SpaceStatus.Models.ExpiryState es = Duncan.PEMS.SpaceStatus.Models.ExpiryState.Inoperational;
                    try
                    {
                        if (bayRow["ExpiryState"] != DBNull.Value)
                        {
                            es = (Duncan.PEMS.SpaceStatus.Models.ExpiryState)bayRow["ExpiryState"];
                        }
                    }
                    catch
                    {
                        es = Duncan.PEMS.SpaceStatus.Models.ExpiryState.Inoperational;
                    }

                    // Output appropriate HTML elements for the expiration state
                    if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Safe)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeSafe\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Expired)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeExpired\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Critical)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeCritical\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else if (es == Duncan.PEMS.SpaceStatus.Models.ExpiryState.Grace)
                    {
                        sb.AppendLine("<div class=\"CenteredBlock BayExpiryTimeGracePeriod\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }
                    else
                    {
                        sb.AppendLine("<div class=\"CenteredBlock NewBayExpiryTimeInoperational\">" + bayRow["ExpiryTimeString"].ToString() + "</div> ");
                    }

                    /////////////////////////////

                    // Get the vehicle occupancy state
                    Duncan.PEMS.SpaceStatus.Models.OccupancyState os = Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable;
                    try
                    {
                        if (bayRow["OccupancyState"] != DBNull.Value)
                            os = (Duncan.PEMS.SpaceStatus.Models.OccupancyState)bayRow["OccupancyState"];
                    }
                    catch
                    {
                        os = Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable;
                    }

                    // Output appropriate HTML elements for the occupancy state
                    switch (os)
                    {
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Empty:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyEmpty\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOut\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.NotAvailable:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Occupied:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyOccupied\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\">" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.OutOfDate:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyOutOfDate\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Violation:
                            sb.AppendLine("<div class=\"CenteredBlock Violation\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.MeterFeeding:
                            sb.AppendLine("<div class=\"CenteredBlock MeterFeeding\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeIn\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDay\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                        case Duncan.PEMS.SpaceStatus.Models.OccupancyState.Unknown:
                            sb.AppendLine("<div class=\"CenteredBlock BayOccupancyNotAvailable\"></div>");
                            sb.AppendLine("<div class=\"LastInOutTimeNA\">" + BindShortTimeSpan(bayRow, "TimeSinceLastInOut", "OccupancyState", customerCfg) + "</div>");
                            sb.AppendLine("<div class=\"LastInOutTimeOfDayNA\"> <span>" + BindTimeOfDay(bayRow, "LastInOutTime", "OccupancyState", customerCfg) + "</span> </div>");
                            break;
                    }

                    sb.AppendLine("</div> "); // End of Bay block
                }
                */

                sb.AppendLine("</div> "); // End of Meter block
            }

            // Finalize the result from the string builder contents
            return sb.ToString();
        }



        public static string MeterStatusSummaryDrillDown(List<SpaceStatusModel> modelForView, CustomerConfig customerCfg)
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
                int totalExpired = 0;
                int totalPaid = 0;
                int totalOther = 0;
                foreach (int itemIdx in itemIndexesForCurrentMeter)
                {
                    SpaceStatusModel nextSpaceModel = modelForView[itemIdx];
                    totalSpaces++;
                    /*
                    if (nextSpaceModel.BayExpiryState == ExpiryState.Expired)
                        totalExpired++;
                    else if (nextSpaceModel.BayExpiryState == ExpiryState.Safe)
                        totalPaid++;
                    else if (nextSpaceModel.BayExpiryState == ExpiryState.Critical)
                        totalPaid++;
                    else
                        totalOther++;
                    */
                    if (nextSpaceModel.BayOccupancyState == OccupancyState.Empty)
                        totalExpired++;
                    else if (nextSpaceModel.BayOccupancyState == OccupancyState.Occupied)
                        totalPaid++;
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

                // Create a tag that will be used as a header row -- child of the outer boundary
                BaseTagHelper tbTitleRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbTitleRow);
                tbTitleRow.AddCssClass("dtr"); // display table-row
                tbTitleRow.MergeAttribute("style", "background-color:#CCFFFF;");

                // Create tags that will be used as column headers -- child of the title row
                BaseTagHelper tbColHeader = new BaseTagHelper("span");
                tbTitleRow.Children.Add(tbColHeader);
                tbColHeader.AddCssClass("dtc hcenter"); //display table-cell and horizontally centered
                tbColHeader.SetInnerText("Group");

                tbColHeader = new BaseTagHelper("span");
                tbTitleRow.Children.Add(tbColHeader);
                tbColHeader.AddCssClass("dtc hcenter");
                tbColHeader.SetInnerText("Expired");

                tbColHeader = new BaseTagHelper("span");
                tbTitleRow.Children.Add(tbColHeader);
                tbColHeader.AddCssClass("dtc hcenter");
                tbColHeader.SetInnerText("Vacant");

                // Create a tag to be used as a group row -- child of the outer panel
                BaseTagHelper tbGroupRow = new BaseTagHelper("span");
                OuterPanel.Children.Add(tbGroupRow);
                tbGroupRow.AddCssClass("dtr");
                tbGroupRow.MergeAttribute("style", "background-color:#33FFFF;");

                // Create a tag to be used as a column in the group row -- child of the group row
                BaseTagHelper tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell and both horizontally and vertically centered
                tbGroupCol.SetInnerText("Meter: " + currentSpaceStatusModel.MeterID.ToString());

                // Column #2 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell and both horizontally and vertically centered
                tbGroupCol.SetInnerText(" "); // &nbsp;

                BaseTagHelper tbRoundPanel = new BaseTagHelper("span");
                tbGroupCol.Children.Add(tbRoundPanel);
                tbRoundPanel.AddCssClass("rounded-corners NewMeterBlock");
                tbRoundPanel.MergeAttribute("style", "width:120px; text-align:center; background-color:#99FFCC;  border-collapse:separate; boder-spacing:1px;");

                BaseTagHelper helloCell = new BaseTagHelper("span");
                tbRoundPanel.Children.Add(helloCell);
                helloCell.SetInnerText(totalExpired.ToString() + "/" + totalSpaces.ToString()); 

                BaseTagHelper tbTbl1 = new BaseTagHelper("div");
                tbRoundPanel.Children.Add(tbTbl1);
                tbTbl1.MergeAttribute("style", "display:table; border:1px solid black; width:90%; margin:4px; border-collapse:separate; boder-spacing:1px;");

                BaseTagHelper tbTbl1Row1 = new BaseTagHelper("span");
                tbTbl1.Children.Add(tbTbl1Row1);
                tbTbl1Row1.AddCssClass("dtr");

                BaseTagHelper tempCell = new BaseTagHelper("span");
                tbTbl1Row1.Children.Add(tempCell);
                tempCell.AddCssClass("dtc");
                tempCell.SetInnerText("Col1");

                tempCell = new BaseTagHelper("span");
                tbTbl1Row1.Children.Add(tempCell);
                tempCell.AddCssClass("dtc");
                tempCell.MergeAttribute("style", "background-color:gray; width:100%;");

                BaseTagHelper tbTbl2 = new BaseTagHelper("div");
                tempCell.Children.Add(tbTbl2);
                tbTbl2.MergeAttribute("style", "display:table; width:100%;");

                BaseTagHelper tbTbl2Row1 = new BaseTagHelper("span");
                tbTbl2.Children.Add(tbTbl2Row1);
                tbTbl2Row1.AddCssClass("dtr");

                int percent = 0;
                if (totalSpaces > 0) 
                    percent = Convert.ToInt32((Convert.ToDouble(totalExpired) / Convert.ToDouble(totalSpaces)) * 100.0f);
                if (percent > 0)
                {
                    tempCell = new BaseTagHelper("span");
                    tbTbl2Row1.Children.Add(tempCell);
                    tempCell.AddCssClass("dtc");
                    tempCell.MergeAttribute("style", "width:" + percent.ToString() + "%; padding:0px; background-color:Red; overflow:none;");
                    tempCell.SetInnerHtml("&nbsp;"); // DEBUG: How do we use a not-breaking space?  &nbsp;
                }

                percent = 0; 
                if (totalSpaces > 0)
                    percent = Convert.ToInt32((Convert.ToDouble(totalOther) / Convert.ToDouble(totalSpaces)) * 100.0f);
                if (percent > 0)
                {
                    tempCell = new BaseTagHelper("span");
                    tbTbl2Row1.Children.Add(tempCell);
                    tempCell.AddCssClass("dtc");
                    tempCell.MergeAttribute("style", "width:" + percent.ToString() + "%; padding:0px; background-color:Yellow; overflow:none;");
                    tempCell.SetInnerHtml("&nbsp;");
                }

                percent = 0;
                if (totalSpaces > 0)
                    percent = Convert.ToInt32((Convert.ToDouble(totalPaid) / Convert.ToDouble(totalSpaces)) * 100.0f);
                if (percent > 0)
                {
                    tempCell = new BaseTagHelper("span");
                    tbTbl2Row1.Children.Add(tempCell);
                    tempCell.AddCssClass("dtc");
                    tempCell.MergeAttribute("style", "width:" + percent.ToString() + "%; padding:0px; background-color:Green; overflow:none;");
                    tempCell.SetInnerHtml("&nbsp;");
                    //DEBUG:  onclick="alert('clicked the green');"
                }



                // Column #3 of group row
                tbGroupCol = new BaseTagHelper("span");
                tbGroupRow.Children.Add(tbGroupCol);
                tbGroupCol.AddCssClass("dtc hcenter vcenter"); //display table-cell and both horizontally and vertically centered
                tbGroupCol.SetInnerText("col3"); // &nbsp;

                tbRoundPanel = new BaseTagHelper("span");
                tbGroupCol.Children.Add(tbRoundPanel);
                tbRoundPanel.AddCssClass("rounded-corners NewMeterBlock");
                tbRoundPanel.MergeAttribute("style", "width:120px; text-align:center; background-color:#99FFCC; border-collapse:separate; boder-spacing:1px;");
                tbRoundPanel.SetInnerText("Hello2"); // &nbsp;



                // Now render the outer panel tag, and we will obtain the Html for all of its children too
                sb.AppendLine(OuterPanel.ToString(TagRenderMode.Normal));
            }

            return sb.ToString();
        }

    
    }

}
