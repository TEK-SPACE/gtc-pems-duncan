<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusModel>>" %>

<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
   

    <% if ((ViewData["IsWinMobile6_5"] != null) && (Convert.ToBoolean(ViewData["IsWinMobile6_5"]) == true))
       { %>
    <%-- Windows Mobile 6.5 seems pretty screwed up without this magic line that took ages of experimentation to figure out!!! --%>
<meta name="viewport" content="initial-scale=2.0" />
    <% }
       else
       { %>
    <%-- Meta tags for better viewing on mobile devices, such as the iPhone or Android --%>
    <meta name="viewport" content="width=224, initial-scale=1" />
    <meta name="HandheldFriendly" content="True" />
    <meta name="MobileOptimized" content="224" />
    <% }  %>

    <title>Enforcement Action</title>


    <% if ((ViewData["IsWinCE"] != null) && (Convert.ToBoolean(ViewData["IsWinCE"]) == true))
       { %>
    <style type="text/css">
        body
        {
            font-size: 12px;
            line-height: 12px;
            height: 100%;
            width: 224px;
            max-width: 240px;
            background-color: white;
        }
        .contentarea
        {
            border-style: none;
            background-color: white;
            width: 266px;
        }
    </style>
    <% }
       else
       { %>
    <style type="text/css">
        body
        {
            font-size: 12px;
            line-height: 12px;
            height: 100%;
            background-color: white;
        }
        .contentarea
        {
            border-style: none;
            background-color: white;
        }
    </style>
    <% }  %>

    <style type="text/css">
        #header, #footer2
        {
            text-align: center;
            background: #f00;
            color: #fff;
            font: 900 1em arial,sans-serif;
        }
        #content
        {
            text-align: left;
            background: #fff;
            color: #000;
            font: 1em arial,sans-serif;
        }
        div.javascript-warning
        {
            font-family: "Arial" ,sans-serif;
            font-size: 14pt;
            line-height: 18pt;
            font-weight: bold;
            color: rgb(255,255,255);
            background-color: rgb(230,0,0);
            border-color: rgb(0,0,0);
            border-width: 1px;
            border-style: solid;
            margin: 10px 20px 20px 20px;
            padding: 10px 10px 10px 10px;
            text-align: center;
        }
        div.side-by-side > div
        {
            float: left;
        }
        
        
        .dtNoLinesOrGaps
        {
            display: block;
            margin: 0px;
            padding: 0px;
            overflow: auto;
            border: none;
            border-collapse: collapse;
            border-spacing: 0px;
            border-width: 0px;
        }
        .dtr
        {
            display: block;
            border: none;
            border-collapse: collapse;
            border-spacing: 0px;
            border-width: 0px;
            overflow: hidden;
        }
        .dtrHead
        {
            display: table-row;
            border: none;
            padding: 4px;
            color: black;
        }
        .dtc
        {
            width: 218px;
            font-size: 12px;
            line-height: 12px;
            height: 12px;
            display: block;
            margin: 0px;
            margin-left: 2px;
            float: left;
            white-space: nowrap;
        }
        .dtcLeft
        {
            width: 109px;
            font-size: 12px;
            line-height: 12px;
            display: block;
            margin: 0px;
            margin-left: 2px;
            float: left;
            white-space: nowrap;
        }
        .dtcRight
        {
            width: 109px;
            font-size: 12px;
            line-height: 12px;
            display: block;
            margin: 0px;
            float: left;
            white-space: nowrap;
        }
        .dtcHead
        {
            font-size: 12px;
            line-height: 12px;
            display: block;
            margin: 0px;
            float: left;
            white-space: nowrap;
        }
        
        .hcenter
        {
            text-align: center;
        }
        .txtL
        {
            text-align: left;
        }
        .txtR
        {
            text-align: right;
        }
        .vcenter
        {
            vertical-align: middle;
        }
        
        
        .loading
        {
            display: inline-block;
            left: 24px;
            height: 24px;
            line-height: 24px;
            top: 0px;
            width: 172px;
            border: 1px solid #000;
            position: absolute;
            background-color: Yellow;
            background-color: rgb(255,255,0);
            color: Black;
            text-align: center;
        }
        
        .mb0
        {
            margin-bottom: 2px;
            margin-right: 2px;
        }
        .fh
        {
            height: 100%;
        }
        
        .grpOutPnl
        {
            margin: 0px;
            padding: 0px;
            background-color: Yellow;
            clear: both;
            overflow: hidden;
            white-space: nowrap;
            border: 1px solid black;
            min-width: 205px;
            min-height: 22px;
            font-size: 12px;
            line-height: 12px;
        }
        .grpInPnl
        {
            display: block;
            margin: 0px;
            padding: 0px;
            border: 2px solid black;
        }
        .np
        {
            padding: 0px;
        }
        
        
        .whiteCell
        {
            background-color: white;
            color: Black;
        }
        .NavyTextCell
        {
            background-color: white;
            color: #000066;
        }
        .appleblueCell
        {
            background-color: #1A2644;
            color: White;
        }
        .orangeCell
        {
            background-color: Orange;
            color: White;
        }
        .greenCell
        {
            background-color: Green;
            color: White;
        }
        .redCell
        {
            background-color: Red;
            color: White;
        }
        .grayCell
        {
            background-color: #484848;
            color: White;
        }
        .purpleCell
        {
            background-color: #484848;
            color: White;
        }
        .ltgreenCell
        {
            background-color: #9BBB59;
            color: White;
        }
        
        .underlineCell
        {
            border-bottom: 1px solid #e8eef4;
        }
        
        .colCellMP
        {
            margin-top: 0px;
            padding-top: 1px;
            padding-bottom: 4px;
        }
        .notBold
        {
            font-weight: normal;
        }
        .Bold
        {
            font-weight: bold;
        }
        
        .VSGfx1
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/OccupancyOccupied.gif")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        .VSGfx2
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/OccupancyEmpty.gif")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        .VSGfx3
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/OccupancyNotAvailable.gif")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
         .VSGfx4
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/SensorNotAvailable.gif")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        
        .PayGfx1
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/Paid16.png")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        .PayGfx2
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/Expired48x16.png")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        .PayGfx3
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/Paid16.png")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        
        .EnfGfx1
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/violation-icon.gif")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        .EnfGfx2
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/Stop16.png")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        .EnfGfx3
        {
            padding-top: 0px;
            background-image: url('<%= Url.Content("~/Content/Images/OK16x16.gif")%>');
            background-repeat: no-repeat;
            background-position: center center;
            background-color: white;
            min-height: 18px;
        }
        
        .ctc
        {
            text-align: center;
            vertical-align: middle;
            margin-bottom: 0px;
        }
        .cbc
        {
            display: table-cell;
            text-align: center;
            vertical-align: middle;
        }
        .cc
        {
            display: table-cell;
            text-align: center;
            vertical-align: middle;
            margin-bottom: 0px;
            height: 100%;
            margin-top: 0px;
            padding-top: 0px;
            padding-bottom: 0px;
        }
        
        
        .pageColWidth
        {
            width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", true, this.Model) %>;
        }
        .sectionWidth
        {
            width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", true, this.Model) %>;
        }
        .pageColTableWidth
        {
            width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;
            min-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;
        }
        .siw
        {
            width: 70px;
            min-width: 70px;
            max-width: 70px;
        }
        
        .siw1
        {
            width: 46px;
            min-width: 46px;
            max-width: 46px;
        }
        .siw2
        {
            width: 72px;
            min-width: 72px;
            max-width: 72px;
        }
        .siw3
        {
            width: 70px;
            min-width: 70px;
            max-width: 70px;
        }
        .siw4
        {
            width: 46px;
            min-width: 46px;
            max-width: 46px;
        }
        
        /*
        body
        {
            font-size: 12px;
        }
        */
        
        div.side-by-side
        {
            height: 400px;
        }
        
        .clearfix:after
        {
            content: "\0020";
            display: block;
            height: 0;
            clear: both;
            overflow: hidden;
            visibility: hidden;
        }
        
        .footerarea
        {
            height: 28px;
            overflow: hidden;
            border-style: none;
        }
        .headerarea
        {
            border-style: none;
        }
        
        .statusIcon
        {
            display: none;
        }
        
        .desktopBackBtn
        {
            display: none;
        }
        .desktopRefreshBtn
        {
            display: none;
        }
        
        .mobileFooterNav
        {
            display: block;
        }
        .drillDownBtn
        {
            width: 24px;
            height: 24px;
        }
        
        .ActiveOffset:active
        {
            position: relative;
            top: 1px;
        }
        
        @media screen and (min-width: 642px)
        {
            body
            {
                font-size: 12px;
                line-height: 12px;
                height: 100%;
                width: 100%;
                max-width: 100%;
            }
            .dtNoLinesOrGaps
            {
                display: inline-block;
                margin: 0px;
                padding: 0px;
                border: none;
                border-collapse: collapse;
                border-spacing: 0px;
                border-width: 0px;
            }
            .contentarea
            {
                border-style: none;
                background-color: white;
                width: 100%;
                max-width: 1024px;
            }
        }
    </style>
    <%-- Windows Mobile doesn't support jQuery library, so we will use this neat little script from http://www.openjs.com/scripts/jx/ that allows us to do AJAX calls for our "partial view" panel updates... --%>
    <script src="<%= Url.Content("~/Scripts/jx_compressed.js") %>" type="text/javascript"></script>
</head>
<script type="text/javascript">
    function ActionSelectChanged(){
          var esel = document.getElementById("ActionSelect");
    }
 
    function navigateBackClick(){
      var e = document.getElementById("loading");
      e.style.display = 'inline-block';
      
      <%-- /* window.history.back(); */ --%>
      window.location.href = '<%= this.ViewData["returnURL"] %>';
    }
</script>
<%-- 
  Function that will be called before Form submit to do special processing.  We will use AJAX to load content from the local webserver (on the PDA device), which is 
  actually just our way of sending some info to the device that the HTTP listener (compact web server) uses to detect a signal to tell the AutoCITE issuance application
  that an Overstay Violation is detected and ready to be written.  After the AJAX signal is sent, we will continue on with our normal form POST.
--%>
<script type="text/javascript">
    var intervalStatusRefresh = "";
    var actiontarget = "";

    function postFormToDuncan() {
        var e = document.getElementById("loading");
        e.style.display = 'inline-block';
        e.innerHTML = 'SAVING';

        var formelem = document.getElementById("form1");
        formelem.submit();
    }

    function signalIssueApp() {
        try {
            // We only want to signal the AutoCITE issuance application if "Enforced" is the chosen action taken
            var esel = document.getElementById("ActionSelect");
            actiontarget = esel.options[esel.selectedIndex].value;
            if (actiontarget == 'Enforced') {
                var e = document.getElementById("loading");
                e.style.display = 'inline-block';
                e.innerHTML = 'PREPARING';

                // XSS security and/or "Same Origin Policy" prevents us from using AJAX to signal the local machine,
                // so we have to use a trick like "JSONP" to load our local URL into a script, which bypasses the
                // same origin policy because XMLHttpRequest isn't used for this technique
                var s = document.createElement('script');
                s.type = 'text/javascript';
                s.src = 'http://127.0.0.1:8088/Virtual/OverstayVio.cmd' +
                  '?MID=' + encodeURIComponent('<%= Convert.ToString(ViewData["MID"]) %>') +
                  '&BAY=' + encodeURIComponent('<%= Convert.ToString(ViewData["SNUM"]) %>') +
                  '&ARRIVAL=' + encodeURIComponent('<%= Convert.ToString(ViewData["ARRIVAL"]) %>') +
                  '&REGULATIONS=' + encodeURIComponent('<%= Convert.ToString(ViewData["REGULATIONS"]) %>') +
                  '&RND=' + encodeURIComponent('<%= DateTime.Now.Ticks.ToString() %>');
                document.body.appendChild(s);

                // Allow sufficient time for local HTTP listener to receive this, then issue our normal POST back to the MeterReportMVC server.
                // (We introduce a 3 second delay before sending the post command)
                if (intervalStatusRefresh == "") {
                    intervalStatusRefresh = window.setInterval(function () {
                        if (intervalStatusRefresh != "") {
                            window.clearInterval(intervalStatusRefresh);
                            intervalStatusRefresh = "";
                        }

                        postFormToDuncan();
                    }, 3000);
                }
            }
            else {
                // Not enforced, so don't need to signal AutoCITE issuance application (in the future, maybe we signal for "cautioned" also?)
                postFormToDuncan();
            }

        } catch (e) {
            alert('Error: ' + e.message + ", Code: " + (e.number & 0xFFFF) + ", Name: " + e.name);
        }
    }
</script>
<body style="margin: 0px; padding: 0px; font-family: helvetica,arial,sans-serif;
    font-weight: bold;">
    <div id="container">
        <noscript>
            <div class="javascript-warning">
                <div>
                    This page requires JavaScript to be enabled.</div>
            </div>
        </noscript>


    <% if ((ViewData["IsWinMobile6_5"] != null) && (Convert.ToBoolean(ViewData["IsWinMobile6_5"]) == true))
       { %>
        <div style="background-color: White; width: 100%; margin: 0px; padding: 0px; border: none;
            border-collapse: collapse; border-spacing: 0px; border-width: 0px;">
            <div style="width: 224px; margin: 0px; padding: 0px; border: none; border-collapse: collapse;
                border-spacing: 0px; border-width: 0px;">
                <input type="image" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-left-blue.png") %>"
                    class="ActiveOffset" style="display: inline-block; float: left; width: 24px;
                    height: 24px; border-style: none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
                    background-repeat: no-repeat; background-position: center center; background-color: #004475;"
                    onclick="navigateBackClick();" />
                <span class="ActiveOffset" style=" display: inline-block; float: left; color: #004475;
                    cursor: pointer; vertical-align: middle; margin-left: 4px; padding: 0px; margin-bottom: 0px;
                    height: 24px; line-height: 24px; width: 56px; overflow: hidden;" onclick="navigateBackClick();">
                    BACK </span><span style="display: inline-block; float: left; vertical-align: middle;
                        margin-left: 4px; padding: 0px; margin-bottom: 0px; height: 24px; line-height: 24px;
                        width: 45px;">&nbsp; </span>
                <div style="clear: both; visibility: hidden; height: 1px; max-height: 1px; font-size: 1px;
                    line-height: 1px;">
                </div>
            </div>
            <div style="font-style: italic; width: 204px; overflow: hidden; max-height: 15px;
                text-align: center; font-size: 14px; color: Blue; padding-bottom: 4px;">
                ENFORCEMENT ACTION</div>
            <div style="clear: both; visibility: hidden; height: 1px; max-height: 1px; font-size: 1px;
                line-height: 1px;">
            </div>
    <% }
       else
       { %>
        <div style="background-color: White; width: 100%; margin: 0px; padding: 0px; border: none;
            border-collapse: collapse; border-spacing: 0px; border-width: 0px;">
            <div style="width: 224px; margin: 0px; padding: 0px; border: none; border-collapse: collapse;
                border-spacing: 0px; border-width: 0px;">
                <input type="image" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-left-blue.png") %>"
                    class="ActiveOffset" style="display: inline-block; float: left; width: 24px;
                    height: 24px; border-style: none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
                    background-repeat: no-repeat; background-position: center center; background-color: #004475;"
                    onclick="navigateBackClick();" />
                <span class="ActiveOffset" style=" display: inline-block; float: left; color: #004475;
                    cursor: pointer; vertical-align: middle; margin-left: 4px; padding: 0px; margin-bottom: 0px;
                    height: 24px; line-height: 24px; width: 56px; overflow: hidden;" onclick="navigateBackClick();">
                    BACK </span><span style="display: inline-block; float: left; vertical-align: middle;
                        margin-left: 4px; padding: 0px; margin-bottom: 0px; height: 24px; line-height: 24px;
                        width: 45px;">&nbsp; </span>
                <div style="clear: both; visibility: hidden; height: 1px; max-height: 1px; font-size: 1px;
                    line-height: 1px;">
                </div>
            </div>
            <div style="font-style: italic; width: 204px; overflow: hidden; max-height: 15px;
                text-align: center; font-size: 14px; color: Blue; padding-bottom: 4px;">
                ENFORCEMENT ACTION</div>
            <div style="clear: both; visibility: hidden; height: 1px; max-height: 1px; font-size: 1px;
                line-height: 1px;">
            </div>
    <% }  %>




        </div>
        <div id="mycontent" class="contentarea">
            <span id="loading" class="loading" style="display: none;">LOADING...</span>
            <div style="max-width: 220px; width: 220px; text-align: center; font-size: 11px;
                font-weight: normal;">
                Please record the enforcement action you are performing for this space.</div>
            <span id="partial1">
                <%= SpaceStatusHelpers.StayVioSpaceAction(Url, this.Model, this.ViewData)%>
            </span><span id="overstaypartial"></span>
            <form id="form1" method="post" action="" style="font-size: 14px; font-weight: normal;">
            <%-- Start -- Action parameters --%>
            <div style="display: block; width: 220px; min-width: 220px; max-width: 220px; border: 1px solid #a38d12;
                overflow: visible; margin-top: 8px; margin-bottom: 8px; padding-bottom: 8px;">
                <div style="background-color: #565ba3; padding-right: 0px; width: 100%; overflow: hidden;
                    margin: 0px; outline: none; padding-bottom: 4px; padding-top: 4px;">
                    <span style="font-size: 13px; padding-left: 6px; padding-top: 4px; padding-bottom: 4px;
                        color: White; font-weight: bold;">Action Taken:</span>
                </div>
                <%= Html.DropDownList("ActionSelect", ViewData["actionChoices"] as SelectList, new { @class = "chzn-select", 
    style = "display:inline-block; float:left; vertical-align:middle; margin-top:8px; width:200px; min-width:200px; max-width:200px; margin-left:3px;",
    onchange = "ActionSelectChanged();"})%>
                <div style="clear: both; visibility: hidden; height: 1px; max-height: 1px; font-size: 1px;
                    line-height: 1px;">
                </div>
            </div>
            <%-- End -- Display Style parameters --%>
            <%-- Start -- Submit button --%>
            <div>
                <span style="display: inline-block; width: 42px; margin: 0px; padding: 0px;">&nbsp;</span>
                <input type="button" class="rounded-corners" value="OK" style="width: 60px; padding: 4px;
                    margin-left: 0px; margin-top: 8px; font-size: medium; background-color: Green;
                    color: White;" onclick="signalIssueApp();" />
                <input type="button" class="rounded-corners" value="Cancel" style="width:60px; padding: 4px;
                    margin-left: 4px; margin-top: 8px; font-size: medium; background-color: Red;
                    color: White;" onclick="navigateBackClick();" />
            </div>
            <%-- End -- Submit button --%>
            </form>
            <%-- This hidden input and "onload" event will help reload the page when navigating back to it (instead of using browser's cached version) --%>
            <input type="hidden" id="Hidden1" value="no" />
            <script type="text/javascript">
      onload = function () {
            var e = document.getElementById("loading");
            e.style.display = 'none';
      }

      <%-- /* "onunload" event is a fix for Firefox Backward-Forward Cache. See http://stackoverflow.com/questions/2638292/after-travelling-back-in-firefox-history-javascript-wont-run for info */ --%>
      window.onunload = function () { };
            </script>
        </div>
    </div>
</body>
</html>
