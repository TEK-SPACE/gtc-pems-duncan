<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Duncan.PEMS.SpaceStatus.Models.SpaceStatusProvider>>" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataSuppliers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Meter and Space Status Information
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%-- /*DEVELOPER NOTE: This type of syntax block is a server-side comment, so it won't get rendered into the webpage. Refer to: http://msdn.microsoft.com/en-us/library/4acf8afk%28v=vs.71%29.aspx */ --%>
    <%-- /*Refresh every 5 seconds, via window.setInterval function*/ --%>
    <%-- /*This will Update contents of element with partial view from ASP.NET*/ --%>
    <%-- /*When disabling the timer, we just need to clear the interval if its already set*/ --%>
    <script type="text/javascript">
        var intervalStatusRefresh = "";
        var targetID = "";
        var customerID = '<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>';

        function startStatusRefreshTimer() {
            if (intervalStatusRefresh == "") {
                intervalStatusRefresh = window.setInterval(function () {
                    stopStatusRefreshTimer();
                    /*Make the "updating" element visible. Here is alternatate sytax: $('#UpdatingBlock').css({ "visibility": "visible" }); */
                    $('#UpdatingBlock').css('visibility', 'visible');
                    $('#partial1').load('<%= Url.Action("allofthem_partialview","SpaceStatus") %>' + '?targetID=' + encodeURIComponent(targetID) + '&CID=' + encodeURIComponent(customerID));
                }, <%: (this.ViewData["CustomerCfg"] as CustomerConfig).GetJavaScriptTicksForDesktopRefreshInterval() %>); /* 5000 = 5 seconds */
            }
        }

        function stopStatusRefreshTimer() {
            if (intervalStatusRefresh != "") {
                window.clearInterval(intervalStatusRefresh);
                intervalStatusRefresh = "";
            }
        }
    </script>

    <form id="form1" runat="server">
    <table width="100%">
        <tr>
            <td>
                <div id="container2">
                    <div id="container1">
                        <div id="col1">
                            <h4>
                                Available Meter Groups</h4>
                            <input type="button" class="treeActionBtn" value="Expand All" style="background-image: url(<%= Url.Content("~/content/images/ExpandAll.gif") %>);
                                background-repeat: no-repeat; background-position: 4px center;" onclick="$('#demo1').jstree('open_all');" />
                            <input type="button" class="treeActionBtn" value="Collapse All" style="background-image: url(<%= Url.Content("~/content/images/CollapseAll.gif") %>);
                                background-repeat: no-repeat; background-position: 4px center;" onclick="$('#demo1').jstree('close_all');" />
                            <%--
                            <!-- Example of button using Anchor and Span (Shows link in lower-left of browser?) -->
                            <!--
                            <a href="#" class="ASpanBtn" onclick="$('#demo1').jstree('close_all');">
                            <span class="spanBtnImg_SSMCluster"  onmousedown="return false;" onselectstart="return false;">Collapse (ASpanBtn)</span>
                            </a>
                            -->
                            <!-- Example of button using Div and Span (Clickable, but not keyboard accessible) -->
                            <!--
                            <div class="DivSpanBtn" onclick="$('#demo1').jstree('close_all');">
                            <span class="spanBtnImg_SSMCluster" onmousedown="return false;" onselectstart="return false;">Collapse (DivSpanBtn)</span>
                            </div>
                            -->
                            --%>
                            <div id="demo1" class="demo" style=" max-height:480px; overflow: auto;"> <%-- height: 100px; --%>
                                <%= MeterReportMVC.Helpers.HtmlHelpers.MeterSelectionTreeView(Url, this.ViewData["CustomerCfg"] as CustomerConfig)%>
                            </div>

                            <%-- /* Use jQuery to convert our list to a jsTree */ --%>
                            <script type="text/javascript">
                                var tree = $("#demo1");
                                tree.bind("loaded.jstree", function (event, data) {
                                    tree.jstree("open_all");
                                });


                                <%-- 
                                // Example of doing something when tree is clicked -- but this also occurs when clicking + and - to expand/collapse nodes
                                /*
                                tree.bind("click.jstree", function (event, data) {
                                    alert("click.jstree event: " + event.type);
                                });
                                */
                               --%>

                                <%-- 
                                /*
                                It is a better solution for us to put our navigation logic into the "select_node.jstree" event of jstree instead of "click.jstree" 
                                or onclick event of the child anchor items for a few reasons:
                                  * We can reduce bandwidth because we will have a common click event instead of creating seperate event code for each anchor.
                                  * The selection event can occur from collapsing a tree, but doesn't fire a click event
                                  * We can't figure out how to programmatically fire the click event from the code in the selection event (at least not in a cross-browser manner)
                                */
                               --%>
                                tree.bind("select_node.jstree", function (e, data) {
                                    var href = data.rslt.obj.children("a").attr("href");
                                    <%-- 
                                    // This will follow the link, but we don't need to do that because we handle in click event
                                    /*document.location.href = href;*/
                                    // Below is just an example of how to get some info about selected object
                                    /* console.log(data.inst.get_text(data.rslt.obj) + " - " + data.rslt.obj.attr("id")); // ID - Text */
                                    --%>

                                    stopStatusRefreshTimer();
                                    $('#UpdatingBlock').css('visibility', 'visible');
                                    targetID = data.rslt.obj.children("a").attr("id");  <%-- // We will use the ID attribute of child anchor of the selected node --%>
                                    $('#partial1').load('<%= Url.Action("allofthem_partialview","SpaceStatus") %>' + 
                                      '?targetID=' + encodeURIComponent(targetID)  + '&CID=' + encodeURIComponent(customerID));
                                    <%-- /*alert('Selected event: ' + data.inst.get_text(data.rslt.obj) + " - " + data.rslt.obj.children("a").attr("id"));*/ --%>
                                });

                                $(function () {
                                    $("#demo1").jstree({
                                        "core": { "animation": 0 },
                                        "themes": { "theme": "classic", "dots": true, "icons": true },
                                        "types": {
                                            "valid_children": ["folder"],
                                            "types": {
                                                "folder": {
                                                    "valid_children": ["file"],
                                                    "icon": { "image": '<%= Url.Content("~/content/images/SSMCluster16x16.png") %>' },
                                                    "max_depth": 1
                                                },

                                                "file": {
                                                    "valid_children": ["none"],
                                                    "icon": { "image": '<%= Url.Content("~/content/images/SSM16x16.png") %>' }
                                                },

                                                "MSMNode": {
                                                    "valid_children": ["none"],
                                                    "icon": { "image": '<%= Url.Content("~/content/images/DuncanMSM16X16.png") %>' }
                                                }
                                            }
                                        },
                                        "plugins": ["themes", "html_data", "ui", "types"]
                                    });

                                    $("#demo1").jstree("open_all");
                                });
                            </script>

                            <%-- /* This is the "Legend" panel */ --%>
                            <div id="TreePane" style="font-family: Arial,Verdana; font-size: 9pt;">
                                Key/Legend:
                                <div id="tablediv" runat="server">
                                    <table class="tblLegend" id="table1" border="1" cellspacing="1" runat="server" style="width: 199px">
                                        <tr>
                                            <td style="width: 125px; height: 19px;">
                                                &nbsp; &nbsp; <strong>Bay / Space #</strong>
                                            </td>
                                        </tr>
                                        <tr id="legendCreditStatus">
                                            <td style="width: 125px; height: 35px;" colspan="2">
                                                <strong>&nbsp; &nbsp; Meter Credit or Expiration<br />
                                                    &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; (hh:MM</strong> or <strong>Days)</strong><br />
                                                <br />
                                                <table class="tblLegendInner" id="CreditLegend" style="width: 193px">
                                                    <tr>
                                                        <td style="width: 16px; height: 8px">
                                                            <span class="CenteredBlock BayExpiryTimeSafe" style="width: 50px; font-weight: bold;">
                                                                2:00</span>
                                                        </td>
                                                        <td style="width: 103px; height: 8px; font-weight: bold;">
                                                            Credit Remaining
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 16px">
                                                            <span class="CenteredBlock BayExpiryTimeExpired" style="width: 50px; font-weight: bold;">
                                                                1D</span>
                                                        </td>
                                                        <td style="width: 103px; height: 17px; font-weight: bold;">
                                                            Expired Duration
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 16px; height: 17px;">
                                                            <span class="CenteredBlock BayExpiryTimeCritical" style="width: 50px; font-weight: bold;">
                                                                0:10</span>
                                                        </td>
                                                        <td style="width: 103px; font-weight: bold; height: 17px;">
                                                            Almost Expired
                                                        </td>
                                                    </tr>
                                                    <%if (true/*this.blnGracePeriodEnabled*/)
                                                      { %>
                                                    <tr id="trGrace">
                                                        <td style="width: 16px; height: 17px;">
                                                            <span class="CenteredBlock BayExpiryTimeGracePeriod" style="width: 50px; font-weight: bold;">
                                                                5:50</span>
                                                        </td>
                                                        <td style="width: 103px; font-weight: bold; height: 17px;">
                                                            Grace Until Expired
                                                        </td>
                                                    </tr>
                                                    <%} %>
                                                    <tr>
                                                        <td style="width: 16px">
                                                            <span class="CenteredBlock BayExpiryTimeInoperational" style="width: 50px; font-weight: bold;">
                                                                --</span>
                                                        </td>
                                                        <td style="width: 103px; font-weight: bold;">
                                                            Not Available
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr id="legendVehStatus">
                                            <td id="Td3" runat="server" style="width: 125px; height: 83px;">
                                                <strong>&nbsp; &nbsp; &nbsp;&nbsp; Vehicle Status</strong>&nbsp;
                                                <div style="width: 195px;">
                                                    <table class="tblLegendInner" style="width: 100%">
                                                        <tr>
                                                            <td class="imgVSOccupied" style="width: 59px; background-repeat: no-repeat; height: 19px">
                                                            </td>
                                                            <td style="font-weight: bold;">
                                                                Occupied
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="imgVSVacant" style="width: 59px; background-repeat: no-repeat; height: 19px">
                                                            </td>
                                                            <td style="height: 19px; font-weight: bold;">
                                                                Empty
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="imgVSOutOfDate" style="width: 59px; background-repeat: no-repeat; height: 19px">
                                                            </td>
                                                            <td style="font-weight: bold;">
                                                                Out of Date
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="imgVSNotAvail" style="width: 59px; background-repeat: no-repeat; height: 21px">
                                                            </td>
                                                            <td style="font-weight: bold; height: 21px;">
                                                                Not Available
                                                            </td>
                                                        </tr>
                                                        <%if (true/*streetLineenable*/)
                                                          { %>
                                                        <tr>
                                                            <td class="imgVSViolation" style="width: 59px; background-repeat: no-repeat; height: 21px;">
                                                            </td>
                                                            <td style="font-weight: bold; height: 21px;">
                                                                Violation
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="imgVSFeeding" style="width: 59px; background-repeat: no-repeat; height: 21px;">
                                                            </td>
                                                            <td style="font-weight: bold; height: 21px;">
                                                                Meter Feeding
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="imgVSNotAvail" style="width: 59px; background-repeat: no-repeat; height: 21px;">
                                                            </td>
                                                            <td style="font-weight: bold; height: 21px;">
                                                                Unknown
                                                            </td>
                                                        </tr>
                                                        <%} %>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr id="legendOccupancyTime">
                                            <td style="width: 125px; height: 35px;" colspan="2">
                                                <strong>&nbsp; &nbsp; &nbsp; Occupancy Time<br />
                                                    &nbsp; &nbsp; &nbsp; (hh:MM or Days)<br />
                                                </strong>
                                                <table class="tblLegendInner" style="width: 193px">
                                                    <tr>
                                                        <td style="width: 59px; height: 8px">
                                                            <span class="LastInOutTimeIn" style="width: 50px; font-weight: bold;">2:00</span>
                                                        </td>
                                                        <td style="height: 8px; font-weight: bold;" colspan="">
                                                            Occupied Duration
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 59px; height: 19px;">
                                                            <span class="LastInOutTimeOut" style="width: 50px; font-weight: bold;">1D</span>
                                                        </td>
                                                        <td style="height: 19px; font-weight: bold;">
                                                            Vacant Duration
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 59px; height: 19px;">
                                                            <span class="LastInOutTimeNA" style="width: 50px; font-weight: bold;">N/A</span>
                                                        </td>
                                                        <td style="font-weight: bold; height: 19px;">
                                                            Not Available
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr id="legendOccupancyTimeStamp">
                                            <td style="width: 125px; height: 35px;" colspan="2">
                                                <strong>&nbsp;&nbsp; Occupancy EventTime<br />
                                                    &nbsp; &nbsp; &nbsp;(hh:mm:ss 24HR)<br />
                                                </strong>
                                                <table class="tblLegendInner" style="width: 193px">
                                                    <tr>
                                                        <td class="imgVSSmallClock" style="width: 50px; color: Black; background-repeat: no-repeat;
                                                            background-position: left center">
                                                            <span style="width: 38px; display: inline-block; font-weight: bold; background-color: Gray;
                                                                font-size: smaller; margin-left: 12px;">14:35</span>
                                                        </td>
                                                        <td style="height: 17px; font-weight: bold;" colspan="">
                                                            Event DateTime
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 59px; height: 19px;">
                                                            <span class="LastInOutTimeNA" style="width: 50px; font-weight: bold; font-size: smaller;
                                                                color: Black;">N/A</span>
                                                        </td>
                                                        <td style="font-weight: bold; height: 19px;">
                                                            Not Available
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div id="col2">
                            <h2>
                                Meter and Space Status Information</h2>
                            <noscript>
                                <div class="javascript-warning">
                                    <div>
                                        This page requires JavaScript to be enabled.</div>
                                </div>
                            </noscript>
                            <div id="accordion" class="basic rounded-corners" style="width: 400px;  display:none;">
                                <h3>
                                    <a href="#">Display Options (click to toggle)</a></h3>
                                <div style="height:auto;">
                                    <p style="height:auto; font-family:Verdana;">
                                        Mauris mauris ante, blandit et, ultrices a, suscipit eget, quam. Integer ut neque.
                                        Vivamus nisi metus, molestie vel, gravida in, condimentum sit amet, nunc. Nam a
                                        nibh. Donec suscipit eros. Nam mi. Proin viverra leo ut odio. Curabitur malesuada.
                                        Vestibulum a velit eu ante scelerisque vulputate.</p>
                                </div>
                            </div>
                            <script type="text/javascript">
                                $(function () {
                                    var icons = {
                                        header: "imgAccordExpand",
                                        headerSelected: "imgAccordShrink"
                                    };
                                    $("#accordion").accordion({
                                        collapsible: true,
                                        autoHeight: true,
                                        alwaysOpen: false,
                                        active: false,
                                        navigation: false,
                                        icons: icons,
                                        animated: true,
                                        fillSpace: true
                                    });
                                });
                            </script>
                            <div id="partial1">
                                <% if (this.ViewData["Area"] == null)
                                   {%>
                                <% Html.RenderPartial("PartialMustSelectArea"); %>
                                <% } %>
                                <% else
                                   { %>
                                <% Html.RenderPartial("PartialVS"); %>
                                <% } %>
                            </div>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    </form>
</asp:Content>
