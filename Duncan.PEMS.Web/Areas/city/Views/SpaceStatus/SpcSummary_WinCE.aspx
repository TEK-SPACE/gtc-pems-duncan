<%-- 
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MeterReportMVC.Models.SpaceStatusModel>>" %>
--%>

<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusModel>>" %>

<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >

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

<title>Space Summary</title>

<style type="text/css">
<%-- 
/* The section below is a CSS "reset". Might be useful info, but we don't need it for this page
html, body, div, span, applet, object, iframe,
h1, h2, h3, h4, h5, h6, p, blockquote, pre,
a, abbr, acronym, address, big, cite, code,
del, dfn, em, img, ins, kbd, q, s, samp,
small, strike, strong, sub, sup, tt, var,
b, u, i, center,
dl, dt, dd, ol, ul, li,
fieldset, form, label, legend,
table, caption, tbody, tfoot, thead, tr, th, td,
article, aside, canvas, details, embed, 
figure, figcaption, footer, header, hgroup, 
menu, nav, output, ruby, section, summary,
time, mark, audio, video {
	margin: 0;
	padding: 0;
	border: 0;
	font-size: 12px;
	font: inherit;
	vertical-align: baseline;
}

article, aside, details, figcaption, figure, 
footer, header, hgroup, menu, nav, section {
	display: block;
}
body {
	line-height: 12px;
}
ol, ul {
	list-style: none;
}
blockquote, q {
	quotes: none;
}
blockquote:before, blockquote:after,
q:before, q:after {
	content: '';
	content: none;
}
table {
	border-collapse: collapse;
	border-spacing: 0;
}
*/
--%>


body {font-size: 12px; line-height :12px; height:320; min-height:320; max-height:320px; width:224px; max-width:240px; overflow:hidden;}
#header, #footer2 {text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}
#content {text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}
div.javascript-warning{font-family:"Arial",sans-serif;font-size:14pt;line-height:18pt;font-weight:bold;color:rgb(255,255,255);background-color:rgb(230,0,0);border-color:rgb(0,0,0);border-width:1px;border-style:solid;margin:10px 20px 20px 20px;padding:10px 10px 10px 10px;text-align:center;}
div.side-by-side > div { float: left;  }

.dtNoLinesOrGaps {display:block; margin:0px; padding:0px; overflow:auto; border:none; border-collapse: collapse; border-spacing: 0px; border-width:0px;}
.dtr {display:block; border:none; border-collapse: collapse; border-spacing: 0px; border-width:0px; overflow:hidden; }
.dtrHead {display: table-row; border:none; padding:4px; color: black;}
.dtc {font-size :12px; line-height:12px; height: 12px; display:block;  margin:0px; border-top:2px solid black; border-bottom:2px solid black;  float:left; white-space:nowrap;}
.dtcLeft {font-size:12px;  font-weight:normal; line-height:12px; display:block;  margin:0px; margin-left:2px; border-left:2px solid black; border-top:2px solid black; border-bottom:2px solid black; border-right:none; float:left; white-space:nowrap;  }
.dtcRight {font-size:12px; line-height:12px; display:block;  margin:0px; border-right:2px solid black; border-top:2px solid black; border-bottom:2px solid black;  float:left; white-space:nowrap;  }
.dtcHead {font-size :12px; line-height:12px; display:block;  margin:0px; float:left; white-space:nowrap;}

.hcenter {text-align: center;}
.vcenter {vertical-align:middle;}

.loading {display: inline-block; left:8px; height: 24px; line-height: 24px; top:50px; width:188px; border:1px solid #000; position:absolute; background-color:Yellow; background-color: rgb(255,255,0); color:Black; text-align:center;}

.mb0{margin-bottom:0px;}
.fh{height:100%;}

.grpOutPnl{ margin:0px; padding:0px; background-color:Yellow; clear:both;  overflow: hidden; white-space: nowrap; border:1px solid black; min-width:205px; min-height:22px; font-size:12px; line-height:12px;}
.grpInPnl{display:block;  margin:0px; padding:0px; border: 2px solid black;}
.np{padding:0px;}

.appleblueCell{background-color:#1A2644; color:White;}
.orangeCell{background-color:Orange; color:White;}
.greenCell{background-color:Green; color: White;}
.redCell{background-color:Red; color: White;}
.grayCell{background-color:#484848; color: White;}
.purpleCell{background-color:#484848; color: White;}
.ltgreenCell{background-color:#9BBB59; color: White;}


.colCellMP{margin-top:0px; padding-top:1px; padding-bottom:4px;}
.notBold{ font-weight:normal;}

.VSGfx1{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OccupancyOccupied.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.VSGfx2{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OccupancyEmpty.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.VSGfx3{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OccupancyNotAvailable.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.VSGfx4{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/SensorNotAvailable.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}



.PayGfx1{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Paid16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.PayGfx2{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Expired48x16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.PayGfx3{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Paid16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}

.EnfGfx1{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/violation-icon.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.EnfGfx2{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Stop16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.EnfGfx3{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OK16x16.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}

.ctc{text-align: center; vertical-align:middle; margin-bottom:0px;} 
.cbc{display: table-cell; text-align: center; vertical-align:middle;} 
.cc{display: table-cell; text-align: center; vertical-align:middle; margin-bottom:0px; height:100%; margin-top:0px; padding-top:0px; padding-bottom:0px;} 


.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", true, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", true, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>; min-width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", true, this.Model) %>; min-width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", true, this.Model) %>;}

  body {font-size: 12px;}

  div.side-by-side { height:400px;  }
  .clearfix:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .footerarea  {height:28px; overflow:hidden; border-style:none;}
  .headerarea {border-style:none;}
  .contentarea {overflow:auto; border-style:none; background-color:rgb(239, 243, 251);}
  
  .statusIcon{display:none;}
  
  .desktopBackBtn{display:none;} 
  .desktopRefreshBtn{display:none;}

  .mobileFooterNav{display:block;}
  .drillDownBtn{width:24px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", true, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", true, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", true, this.Model) %>;}
</style>


<%-- Windows Mobile doesn't support jQuery library, so we will use this neat little script from http://www.openjs.com/scripts/jx/ that allows us to do AJAX calls for our "partial view" panel updates... --%>
<script src="<%= Url.Content("~/Scripts/jx_compressed.js") %>" type="text/javascript"></script>

</head>

<script type="text/javascript">
    var targetID = "";  
    var groupType = "<%= ViewData["groupType"]%>";  <%-- // A=Area, M=Meter, PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute --%>
    var viewType = "<%= ViewData["viewType"]%>"; <%-- // L=List, T=Tile --%>

    function drilldownToSpecificTarget(specificTargetID) {
      targetID = specificTargetID;
      drilldownToSelected();
    }

    function drilldownToSelected() {
        if (targetID == ""){
          var esel = document.getElementById("GroupSelect");
          targetID = esel.options[esel.selectedIndex].value;
        }
        
        if (targetID != "") {
            stopStatusRefreshTimer();
            var e = document.getElementById("loading");
            e.style.display = 'inline-block';

            window.location.href = '<%= Url.Action("SpcDetail","SpaceStatus") %>' + 
            '?T=' + encodeURIComponent(targetID) + 
            '&PT=' + encodeURIComponent('<%= this.ViewData["parentTarget"].ToString() %>') +
            '&G=' + encodeURIComponent('<%= this.ViewData["groupType"].ToString() %>') +
            '&V=' + encodeURIComponent('<%= this.ViewData["viewType"].ToString() %>') +
            '&CID=' + encodeURIComponent('<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>') +
            '&DS=' + encodeURIComponent('<%= this.ViewData["displayStyle"].ToString() %>');
        }
    }

    function GroupSelectChanged(){
          var esel = document.getElementById("GroupSelect");
          targetID = esel.options[esel.selectedIndex].value;
          document.getElementById(targetID).scrollIntoView();
    }
 
    function navigateBackClick(){
      stopStatusRefreshTimer();
      <%-- /*$('#loading').css('display', 'inline-block'); */ --%>
      var e = document.getElementById("loading");
      e.style.display = 'inline-block';
      
      <%-- /* window.history.back(); */ --%>
      window.location.href = '<%= Url.Action("GrpSummary","SpaceStatus") %>' + 
        '?G=' + encodeURIComponent('<%= this.ViewData["groupType"].ToString() %>') + 
        '&V=' + encodeURIComponent('<%= this.ViewData["viewType"].ToString() %>') + 
        '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>);
    }

    function refreshCurrentInfo() {
       stopStatusRefreshTimer();
     
       var e = document.getElementById("loading");
       e.style.display = 'inline-block';
     
       var elem = document.getElementById("partial1");

     jx.load('<%= Url.Action("SpcSummaryPartial","SpaceStatus") %>' + 
          '?T=' + encodeURIComponent('<%= ViewData["parentTarget"]%>') + 
          '&G=' + encodeURIComponent('<%= ViewData["groupType"]%>') + 
          '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') + 
          '&CID=' + encodeURIComponent('<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>') +
          '&DS=' + encodeURIComponent('<%= this.ViewData["displayStyle"].ToString() %>') +
          '&F=' + encodeURIComponent('<%= this.ViewData["filter"].ToString() %>'),
          function(data){
            elem.innerHTML = data;

            var e = document.getElementById("loading");
            e.style.display = 'none';
            startStatusRefreshTimer();
          },'text');
    }

    function DisplayOptions() {
        stopStatusRefreshTimer();
        window.location.href = '<%= Url.Action("SpcSummaryOptions","SpaceStatus") %>' + 
          '?T=' + encodeURIComponent('<%= ViewData["parentTarget"]%>') + 
          '&G=' + encodeURIComponent('<%= ViewData["groupType"]%>') + 
          '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') + 
          '&CID=' + encodeURIComponent('<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>') +
          '&DS=' + encodeURIComponent('<%= this.ViewData["displayStyle"].ToString() %>') +
          '&F=' + encodeURIComponent('<%= this.ViewData["filter"].ToString() %>');
    }
</script>


<body style="margin:0px; padding:0px; font-family: helvetica,arial,sans-serif; font-weight: 700; max-height:200px; max-width:240px;" >

<div id="container" >

  <noscript>
    <div class="javascript-warning">
      <div>This page requires JavaScript to be enabled.</div>
    </div>
  </noscript>

  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Enforcement - Summary
  </div>

  <div style="font-style:normal; width:220px; overflow:hidden; height:12px; max-height:12px; text-align:left; font-size: 10px; color:Black; padding-bottom:0px; margin-bottom:4px;">

    <input type="image" class="treeActionBtnBOGUS" value="&nbsp;" src="<%= Url.Content("~/content/images/Gears3BlueX12.gif") %>"
     style="display:inline-block; float:left; width:12px; height:12px; border-style:none; background-image: url(<%= Url.Content("~/content/images/Gears3BlueX12.gif") %>);
     background-repeat : no-repeat; background-position:center center; background-color:#004475;" onclick="DisplayOptions();"  />

      <span style="display:inline-block; float:left; vertical-align:middle; margin-left:4px; padding:0px; margin-bottom:0px;">
      <%: Html.ActionLink("Filter", 
      "SpcSummaryOptions", "SpaceStatus", new {T = ViewData["parentTarget"],
                                               G = ViewData["groupType"],
                                               V = ViewData["ViewType"],
                                               CID = (ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString(),
                                               DS = ViewData["displayStyle"],
                                               F = ViewData["filter"]},
      new {@class = "HeaderAnchor"}) %> 
      </span>

      <span style="display:inline-block; float:left; vertical-align:middle; margin-left:8px; padding:0px; margin-bottom:0px; font-weight:normal; white-space:nowrap;"> <%= ViewData["FilterDisplay"] %>
      </span>

    <div style="clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;"></div>    

  </div>

   <input type="image" class="treeActionBtnBOGUS" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-left-blue.png") %>"
   style="display:inline-block; float:left; width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="navigateBackClick();"  />

  <span style="display:inline-block; float:left; margin-left: 3px; height:24px; line-height:24px; vertical-align:middle;" ><%= ViewData["groupPrompt"]%></span>

  <%= Html.DropDownList("GroupSelect", ViewData["groupChoices"] as SelectList, new { @class = "chzn-select", 
    style = "display:inline-block; float:left; vertical-align:middle; margin-top:3px; width:100px; min-width:100px; max-width:100px; margin-left:3px;",
    onchange = "GroupSelectChanged();"})%>

   <input type="image" class="drillDownBtn" src="<%= Url.Content("~/content/images/arrow-right-blue.gif") %>"
   value="&nbsp;" style="display:inline-block; float:left; margin-left: 3px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-right-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="drilldownToSelected();"  />

   <input type="image" class="treeActionBtnBOGUS" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-refresh-blue.png") %>"
   style="display:inline-block; float:left; margin-left: 3px;  width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />

   <div style="clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;"></div>    

<div id="mycontent" class="contentarea"  >

<span id="loading" class="loading" style="display:none;">LOADING...</span>

<script type="text/javascript">
    var intervalStatusRefresh = "";
             
    function startStatusRefreshTimer() {
        if (intervalStatusRefresh == "") {
            intervalStatusRefresh = window.setInterval(function () {
                stopStatusRefreshTimer();

        var e = document.getElementById("loading");
        e.style.display = 'inline-block';

       var elem = document.getElementById("partial1");
     jx.load('<%= Url.Action("SpcSummaryPartial","SpaceStatus") %>' + 
          '?T=' + encodeURIComponent('<%= ViewData["parentTarget"]%>') + 
          '&G=' + encodeURIComponent('<%= ViewData["groupType"]%>') + 
          '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') + 
          '&CID=' + encodeURIComponent('<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>') +
          '&DS=' + encodeURIComponent('<%= this.ViewData["displayStyle"].ToString() %>') +
          '&F=' + encodeURIComponent('<%= this.ViewData["filter"].ToString() %>'),
          function(data){
            elem.innerHTML = data;

            var e = document.getElementById("loading");
            e.style.display = 'none';
            startStatusRefreshTimer();
          },'text');

          }, <%: (this.ViewData["CustomerCfg"] as CustomerConfig).GetJavaScriptTicksForMobileRefreshInterval() %> );
        }
    }

    function stopStatusRefreshTimer() {
        if (intervalStatusRefresh != "") {
            window.clearInterval(intervalStatusRefresh);
            intervalStatusRefresh = "";
        }
    }
</script>


<span id="partial1">
  <% Html.RenderPartial("pv_SpcSummaryContent"); %>
</span>

  <%-- This hidden input and "onload" event will help reload the page when navigating back to it (instead of using browser's cached version) --%>
  <input type="hidden" id="Hidden1" value="no"/>
  <script type="text/javascript">
      onload = function () {
          <%-- 
          /* This doesn't work properly in Windows Mobile, so we'll just restart the timer instead */
          /*
          var e = document.getElementById("refreshed");
          if (e.value == "no") {
              e.value = "yes";
          }
          else {
              e.value = "no";
              /* Page is being displayed from browser cache, so show the "Loading" indictor and force a reload of the page */ 
              var enode = document.getElementById("loading");
              enode.style.display = 'inline-block';

              location.reload();
          }
          */
          --%>

            var e = document.getElementById("loading");
            e.style.display = 'none';

            stopStatusRefreshTimer();
            startStatusRefreshTimer();
      }

      <%-- /* "onunload" event is a fix for Firefox Backward-Forward Cache. See http://stackoverflow.com/questions/2638292/after-travelling-back-in-firefox-history-javascript-wont-run for info */ --%>
      window.onunload = function () { };
  </script>


</div>

<%-- 
<div id="myfooter"  class="footerarea mobileFooterNav" >
   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="navigateBackClick();"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="visibility:hidden; width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/zoom-btn-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="visibility:hidden; display:none; width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/sort-btn-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-for-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/Gears3Blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="DisplayOptions();"  />
</div>
--%>

</div>

</body>
</html>

