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

<title>Violations - Summary</title>

<style type="text/css">
body {font-size: 12px; line-height :12px; height:320; min-height:320; max-height:320px; width:224px; max-width:240px; overflow:hidden;}
#header, #footer2 {text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}
#content {text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}
div.javascript-warning{font-family:"Arial",sans-serif;font-size:14pt;line-height:18pt;font-weight:bold;color:rgb(255,255,255);background-color:rgb(230,0,0);border-color:rgb(0,0,0);border-width:1px;border-style:solid;margin:10px 20px 20px 20px;padding:10px 10px 10px 10px;text-align:center;}
div.side-by-side > div { float: left;  }

.dtNoLinesOrGaps {display:block; margin:0px; padding:0px; overflow:auto; border:none; border-collapse: collapse; border-spacing: 0px; border-width:0px;}
.dtr {display:block; border:none; border-collapse: collapse; border-spacing: 0px; border-width:0px; overflow:hidden; }
.dtrHead {display: table-row; border:none; padding:4px; color: black;}
.dtc {font-size :12px; line-height:12px; display:block;  margin:0px; border-top:2px solid black; border-bottom:2px solid black;  float:left; white-space:nowrap;}
.dtcLeft { overflow:hidden; font-size:12px;  font-weight:normal; line-height:12px; display:block;  margin:0px; margin-left:2px; border-left:2px solid black; border-top:2px solid black; border-bottom:2px solid black; border-right:none; float:left; white-space:nowrap;  }
.dtcRight {font-size:12px; line-height:12px; display:block;  margin:0px; border-right:2px solid black; border-top:2px solid black; border-bottom:2px solid black;  float:left; white-space:nowrap;  }
.dtcHead {font-size :12px; line-height:12px; display:block;  margin:0px; float:left; white-space:nowrap;}

.hcenter {text-align: center;}
.vcenter {vertical-align:middle;}

.loading {display: inline-block; left:24px; height: 24px; line-height: 24px; top:0px; width:172px; border:1px solid #000; position:absolute; background-color:Yellow; background-color: rgb(255,255,0); color:Black; text-align:center;}

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

.colCellMP{margin-top:0px; padding-top:1px; padding-bottom:4px;}

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
.siw {width:70px; min-width:70px; max-width:70px;}

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


.ActiveOffset:active { 
	position:relative;
	top:1px;
}

</style>


<%-- Windows Mobile doesn't support jQuery library, so we will use this neat little script from http://www.openjs.com/scripts/jx/ that allows us to do AJAX calls for our "partial view" panel updates... --%>
<script src="<%= Url.Content("~/Scripts/jx_compressed.js") %>" type="text/javascript"></script>

</head>

<script type="text/javascript">
    var targetID = "";
    var groupType = "<%= ViewData["groupType"]%>";  <%-- // A=Area, M=Meter.  Maybe in future will have others, such as PC=PAMCluster, ER=EnfRoute, MR=MaintRoute, CR=CollRoute --%>
    var viewType = "<%= ViewData["viewType"]%>"; <%-- // L=List, T=Tile --%>

    function drilldownToSpecificTarget(specificTargetID) {
        targetID = specificTargetID;
        drilldownToSelected();
    }

    function drilldownToSelected() {
       
        if (targetID == "") {
            <%-- /* Windows Mobile doesn't support jQuery library, so we can't use the handy selectors */ --%>
          <%-- /* targetID = $("#GroupSelect option:selected").attr('value'); */ --%>
            var esel = document.getElementById("GroupSelect");
            targetID = esel.options[esel.selectedIndex].value;
        }

        if (targetID != "") {
            stopStatusRefreshTimer();
            <%-- /* Windows Mobile doesn't support jQuery library, so we can't use the handy selectors */ --%>
            <%-- /*$('#loading').css('display', 'inline-block');*/ --%>
            var e = document.getElementById("loading");
            e.style.display = 'inline-block';

            window.location.href = '<%= Url.Action("StayVioSpcSum","SpaceStatus") %>' + '?T=' + encodeURIComponent(targetID) +
            '&G=' + encodeURIComponent(groupType) +
            '&V=' + encodeURIComponent(viewType) +
            '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>);
        }
    }

    function GroupSelectChanged() {
        var esel = document.getElementById("GroupSelect");
        targetID = esel.options[esel.selectedIndex].value;
        document.getElementById(targetID).scrollIntoView();
    }

    function navigateBackClick() {
         stopStatusRefreshTimer();
        var e = document.getElementById("loading");
        e.style.display = 'inline-block';

      <%-- /* window.history.back(); */ --%>
        window.location.href = '<%= Url.Action("ReportSpaceStatus","Spacestatus") %>' +
          '?CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>);
  }

    function refreshCurrentInfo() {
        stopStatusRefreshTimer();
        <%--
        /* jQuery not supported */
        /*$('#loading').css('display', 'inline-block');*/ 
        --%>
        var e = document.getElementById("loading");
        e.style.display = 'inline-block';
        <%--
        /* jQuery not supported */
        /*
        $('#partial1').load('<%= Url.Action("GrpSummaryPartial","SpaceStatus") %>' + 
          '?G=' + encodeURIComponent('<%= ViewData["groupType"]%>') + 
          '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') + 
          '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>));
       */
       --%>
        var elem = document.getElementById("partial1");

        jx.load('<%= Url.Action("StayVioGrpSumPartial","SpaceStatus") %>' +
            '?G=' + encodeURIComponent('<%= ViewData["groupType"]%>') +
          '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') +
          '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>),
          function (data) {
              elem.innerHTML = data;

              var e = document.getElementById("loading");
              e.style.display = 'none';
                 startStatusRefreshTimer();
          }, 'text');
 }
</script>


<body style="margin:0px; padding:0px; font-family: helvetica,arial,sans-serif; font-weight: 700; max-height:200px; max-width:240px;" >

<div id="container" >

<%-- 
<div id="myheader" class="headerarea">
--%>
  <noscript>
    <div class="javascript-warning">
      <div>This page requires JavaScript to be enabled.</div>
    </div>
  </noscript>


   <input type="image" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-left-blue.png") %>"
   class="ActiveOffset" style="display:inline-block; float:left; width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="navigateBackClick();"  />

   <span class="ActiveOffset" style="display:inline-block; float:left; color:#004475; cursor:pointer; vertical-align:middle; margin-left:4px; padding:0px; margin-bottom:0px; height:24px; line-height:24px; width: 56px; overflow:hidden;" onclick="navigateBackClick();" >
   BACK
   </span>

   <span style="display:inline-block; float:left; vertical-align:middle; margin-left:4px; padding:0px; margin-bottom:0px; height:24px; line-height:24px; width:45px;" >
   &nbsp;
   </span>

   <span class="ActiveOffset" style="display:inline-block; float:left; color:#004475; cursor:pointer; vertical-align:middle; margin-left:4px; padding:0px; margin-bottom:0px; height:24px; line-height:24px; width:56px; overflow:hidden;" onclick="refreshCurrentInfo();" >
   REFRESH
   </span>

   <input type="image" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-refresh-blue.png") %>"
   class="ActiveOffset" style="display:inline-block; float:left; margin-left: 3px;  width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />

   <div style="clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;">
   </div>    

  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Violations - Summary</div>



  <span style="display:inline-block; float:left; margin-left: 3px; height:24px; line-height:24px; vertical-align:middle;" ><%= ViewData["groupPrompt"]%></span>
  
  <%-- <span style="display:table-cell; height:24px; line-height:24px; vertical-align:middle;"> --%>
 <%= Html.DropDownList("GroupSelect", ViewData["groupChoices"] as SelectList, new { @class = "chzn-select", 
    style = "display:inline-block; float:left; vertical-align:middle; margin-top:3px; width:100px; min-width:100px; max-width:100px; margin-left:3px;",
    onchange = "GroupSelectChanged();"})%>
  <%-- </span> --%>

   <input type="image" class="drillDownBtn" src="<%= Url.Content("~/content/images/arrow-right-blue.gif") %>"
   value="&nbsp;" style="display:inline-block; float:left; margin-left: 3px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-right-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="drilldownToSelected();"  />


  <span class="desktopRefreshBtn" >
   <input type="button" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-for-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />
  </span>

     <div style="clear: both; visibility: hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;">
     </div>    


<%-- 
  </span>
  --%>

<%-- 
 </div>
  --%>

<%-- 
</div>
  --%>

<div id="mycontent" class="contentarea"  >

<span id="loading" class="loading" style="display:none;">LOADING...</span>

<script type="text/javascript">
    var intervalStatusRefresh = "";
             
    function startStatusRefreshTimer() {
        if (intervalStatusRefresh == "") {
            intervalStatusRefresh = window.setInterval(function () {
                stopStatusRefreshTimer();

        <%--
        /* jQuery not supported */
        /*$('#loading').css('display', 'inline-block');*/ 
        --%>
                var e = document.getElementById("loading");
                e.style.display = 'inline-block';

        <%--
        /* jQuery not supported */
        /*
                $('#partial1').load('<%= Url.Content("~/SpaceStatus/GrpSummaryPartial") %>' + 
                  '?G=' + encodeURIComponent('<%= ViewData["groupType"]%>') + 
                  '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') + 
                  '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>));
        */ 
        --%>
                var elem = document.getElementById("partial1");
                jx.load('<%= Url.Action("StayVioGrpSumPartial","SpaceStatus") %>' +                
          '?G=' + encodeURIComponent('<%= ViewData["groupType"]%>') + 
          '&V=' + encodeURIComponent('<%= ViewData["viewType"]%>') + 
          '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>),
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
<%-- 
Click the refresh button to load this section via AJAX!
--%>
  <% Html.RenderPartial("pv_StayVioGrpSumContent"); %>
</span>

<%-- <%= MobileOccupancyStatusHelpers.EnfSummaryByGroups(this.Model, this.ViewData)%> --%>

  <%-- This hidden input and "onload" event will help reload the page when navigating back to it (instead of using browser's cached version) --%>
  <input type="hidden" id="refreshed" value="no"/>
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

<%-- position:absolute; bottom:0px; height:28px; left:0px; overflow:hidden;    border-style:none; border-color:red; --%>
<%-- 
<div id="myfooter"  class="footerarea mobileFooterNav" >
   <input type="image" class="treeActionBtnBOGUS" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-left-blue.png") %>"
   style="width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="navigateBackClick();"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="visibility:hidden; width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/zoom-btn-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="visibility:hidden; width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/sort-btn-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;"  />

   <input type="image" class="treeActionBtnBOGUS" value="&nbsp;" src="<%= Url.Content("~/content/images/arrow-refresh-blue.gif") %>"
   style="width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />
</div>
--%>

</div>

</body>
</html>

