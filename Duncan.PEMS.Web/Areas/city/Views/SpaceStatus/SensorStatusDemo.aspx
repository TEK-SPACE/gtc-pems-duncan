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



    <link rel="SHORTCUT ICON" href="<%= Url.Content("~/Content/images/favicon.ico") %>" type="image/x-icon"/>
    <link href="<%= Url.Content("~/Content/Site.css") %>" rel="stylesheet" type="text/css" />
    
    <%-- There are JavaScript libraries that we can only include when NOT targeting Windows Mobile --%>
<% if ((ViewData["IsWinCE"] == null) || (Convert.ToBoolean(ViewData["IsWinCE"]) == false))
   {%>

    <%-- <!-- Grab Google CDN's jQuery, with a protocol relative URL; fall back to local if offline --> --%>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js" type="text/javascript"></script>
    <script type="text/javascript">        window.jQuery || document.write('<script src="<%= Url.Content("~/Scripts/jquery-1.7.2.min.js") %>"><\/script>')</script>

    <%-- 
    <!-- CSS for jQuery UI  -->
    <link rel="stylesheet" media="all" type="text/css" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.23/themes/redmond/jquery-ui.css" />
        
    <!-- jQuery.UI from Google's CDN with local fallback -->  
    <script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.8.21/jquery-ui.min.js"></script>
    <script>        window.jQuery.ui || document.write('<script src="<%= Url.Content("~/Scripts/jquery-ui-1.8.21.min.js") %>"><\/script>') </script>
    --%>
<% } %>





<meta http-equiv="cache-control" content="no-cache"/> <%-- <!-- tells browser not to cache -->  --%>
<meta http-equiv="expires" content="-1"/> <%-- <!-- says that the cache expires 'now' --> --%>
<meta http-equiv="pragma" content="no-cache"/> <%-- <!-- says not to use cached stuff, if there is any --> --%>

<title>Space Status Demo</title>

<style type="text/css">
body {font-size: 13px;}
/*#header, #footer2 {text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}*/
#content {height:100%; text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}
div.side-by-side > div { float: left; padding-left:8px; }
.dtNoLinesOrGaps {display:table; border-collapse:collapse; border-spacing:0px; border-width:0px;}
.dtr {display: table-row; width:100%; border:none; }
.dtrHead {display: table-row; width:100%; border:none; padding:4px; color: black;}
.dtc {display: table-cell;}
.hcenter {text-align: center;}
.vcenter {vertical-align:middle;}
.loading {display: inline-block; margin-left:8px; padding-left:8px; left:8px; height: 24px; line-height: 24px; /*top:50px;*/ width:188px; border:1px solid #000; /*position:fixed;*/ background-color:#FFFF00; background-color: rgba(255,255,0,.8); color:Black; text-align:center;}
.mb0{margin-bottom:0px;}
.fh{height:100%;}
.grpOutPnl{display:inline-block; margin-bottom:4px; margin-right:4px; vertical-align:top;}
.grpInPnl{display:inline-block; margin:0px; padding:0px; border: 2px solid black;}
.np{padding:0px;}
.notBold{ font-weight:normal;}


.appleblueCell{background-color:#1A2644; color:White;}
.orangeCell{background-color:Orange; color:White;}
.greenCell{background-color:Green; color: White;}
.purpleCell{background-color:#484848/*#B3A2C7*/; color: White;}
.ltgreenCell{background-color:#9BBB59; color: White;}
.redCell{background-color:Red; color: White;}
.grayCell{background-color:#484848; color: White;}

.greenCellBorder{border: 2px solid #9BBB59; }
.purpleCellBorder{border: 2px solid #B3A2C7; }
.redCellBorder{border: 2px solid Red; }

.colCellMP{margin-top:0px; padding-top:0px; padding-bottom:0px;}

.VSGfx1{/*margin-top:0px; padding-bottom:0px;*/ padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OccupancyOccupied.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.VSGfx2{/*margin-top:0px; padding-bottom:0px;*/ padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OccupancyEmpty.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.VSGfx3{/*margin-top:0px; padding-bottom:0px;*/ padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/OccupancyNotAvailable.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.VSGfx4{/*margin-top:0px; padding-bottom:0px;*/ padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/SensorNotAvailable.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}




.PayGfx1{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Paid16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.PayGfx2{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Expired48x16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.PayGfx3{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/Paid16.png")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}

.EnfGfx1{padding-top:0px; background-image: url('<%= Url.Content("~/Content/Images/violation-icon.gif")%>'); background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.EnfGfx2{padding-top:0px; /*background-image: url('<%= Url.Content("~/Content/Images/Stop16.png")%>');*/ background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}
.EnfGfx3{padding-top:0px; /*background-image: url('<%= Url.Content("~/Content/Images/OK16x16.gif")%>');*/ background-repeat:no-repeat; background-position:center center; background-color:white; min-height:18px;}

.ctc{text-align: center; vertical-align:middle; margin-bottom:0px;} 
.cbc{display: table-cell; text-align: center; vertical-align:middle;} /*siw*/
.cc{display: table-cell; text-align: center; vertical-align:middle; margin-bottom:0px; height:100%; margin-top:0px; padding-top:0px; padding-bottom:0px;} /* siw */


/* * html #loading {position:absolute;} */

.pageColWidth {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("pageColWidth", true, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("sectionWidth", true, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("siw", true, this.Model) %>;}




@media screen and (min-width: <%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("mediaWidth_4Column", false, this.Model) %>) <%-- /* for displays wide enough for 4 columns */ --%>
{
  body {font-size: 16px;}
  div.side-by-side {  max-height:24px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix2:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .headerarea {display:block; height:50px; overflow:visible;}
  .footerarea  { height:28px; left:0px; overflow:hidden;  border-style:none; }
  .contentarea{ /*max-width: <%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("mediaWidth_4Column_MaxWidth", false, this.Model) %>;*/  /*max-height:522px;*/ overflow:auto; background-color:rgb(239, 243, 251);}
  .pageColHead1{display:inline-block;} /*table-row*/
  .pageColHead2{display:inline-block;}
  .pageColHead3{display:inline-block;}
  .pageColHead4{display:inline-block;}

  .statusIcon{/*display:block;*/<%= SpaceStatusHelpers.StatusIconDisplayForDisplayStyle(false, this.ViewData["displayStyle"].ToString()) %>}

  .desktopBackBtn{display:block;} 
  .desktopRefreshBtn{display:block;}
  .desktopGearsBtn{display:block;}

  .mobileFooterNav{display:none;}
  .drillDownBtn{width:40px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("pageColWidth", false, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("sectionWidth", false, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("pageColTableWidth", false, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.SensorStatusDemo_CSSDynamicValue("siw", false, this.Model) %>;}
}

</style>


<%-- <!-- Grab Google CDN's jQuery, with a protocol relative URL; fall back to local if offline --> --%>
<%-- <!-- 
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js" type="text/javascript"></script>
<script type="text/javascript">    window.jQuery || document.write('<script src="<%= Url.Content("~/Scripts/jquery-1.7.2.min.js") %>"><\/script>')</script>

<link rel="stylesheet" href="<%= Url.Content("~/Scripts/Chosen/chosen.css") %>" type="text/css" />
<script src="<%= Url.Content("~/Scripts/Chosen/chosen.jquery.min.js") %>" type="text/javascript"></script>
--> --%>
</head>

<script type="text/javascript">

    function refreshCurrentInfo() {
        /*
        $('#loading').css('display', 'inline-block');
        window.location.reload(true);
        */

        stopStatusRefreshTimer();
        <%-- /*Make the "updating" element visible. Here is alternatate sytax: $('#UpdatingBlock').css({ "visibility": "visible" }); */ --%>
        $('#loading').css('display', 'inline-block');
        $('#partial1').load('<%= Url.Action("SensorStatusDemoPartial","SpaceStatus") %>' + 
          '?CID=' + encodeURIComponent('<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>'));
    }

</script>


<body style="margin:0px; padding:0px; font-family: helvetica,arial,sans-serif;  background-color:White;" > <%-- font-weight: 700; --%>





<%-- Below is the main content when NOT targeting Windows Mobile --%>
<% if ((ViewData["IsWinCE"] == null) || (Convert.ToBoolean(ViewData["IsWinCE"]) == false))
   {%>

 <span style="display:table;  border-collapse:collapse; border:none; margin-top: 2px; margin-left: 2px; background-image:url(<%= Url.Content("~/Content/images/PEMTitleGradient1x119.gif")%>); 
    background-repeat:repeat-x; width:100%; max-width:1403px; height:119px;" >

 <span style="display:table-row; width:100%; border:none;">

  <span style="display:table-cell; background-image:url(<%= Url.Content("~/Content/images/PEMTitleLeftGradient2x119.gif")%>); background-repeat:no-repeat; width:2px; height:119px;"></span>

  <span style="display:table-cell; width:113px;">&nbsp;</span>
  <span style="display:table-cell; background-image:url(<%= Url.Content("~/Content/images/PEMLogoAndLine144x119.gif")%>); background-repeat:no-repeat; width:144px; height:119px;
  cursor: pointer;"  onclick="document.location='<%= Url.Content("~/") %>' + '?CID=' + encodeURIComponent(<%= this.ViewData["CID"] %>); return false;"></span>

  <span style="display:table-cell; width:16px;">&nbsp;</span>
 
  <span style="display:table-cell; font-family: Arial, Helvetica, sans-serif; font-size:23px; color:White; padding-top:21px; width:573px;
      vertical-align:middle; ">Parking Enterprise Manager</span>

  <span style="display:table-cell; vertical-align: top;" >
   <span style="display:inline-table; color:White; border:none; height:117px;
    font-family: Arial, Helvetica, sans-serif; font-size:15px; color:White;">
    <span style="display:table-row; height:50%;">
      <span style="display:table-cell; vertical-align:middle;">Customer: <%: Html.ActionLink(this.ViewData["CustomerName"] + " (ID# " + this.ViewData["CID"] + ")", 
      "SelectClient", "SpaceStatus", new {ReturnAction = this.ViewContext.RouteData.Values["action"].ToString(), ReturnController = this.ViewContext.RouteData.Values["controller"].ToString()},
      new {@class = "HeaderAnchor"}) %> 
      </span>
    </span>
    <span style="display:table-row; height:50%;">
      <span style="display:table-cell; vertical-align: bottom;">
       <span class="rounded-corners" style="display:inline-block; background-color:#064571; border:none; padding-left: 12px; padding-right: 12px; padding-top: 5px; padding-bottom: 5px;
       vertical-align: middle; line-height: 34px; height: 34px;"><% Html.RenderPartial("LogOnUserControl"); %></span>
      </span>
    </span>
   </span>
  </span>
 <span style="display:table-cell; width:4px;">&nbsp;</span>
</span>
</span>
<div id="header">
</div>
<% } %>





<div id="container" style="font-weight: 700;" >

<div id="myheader" class="headerarea">

  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">&nbsp;
  </div>

  <div  class="side-by-side clearfix" style="border-style:none; overflow:visible;" >

  <div><%= ViewData["groupPrompt"]%></div>

  <div class="desktopRefreshBtn" >
   <input type="button" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-for-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />
  </div>

<div id="loading" class="loading" style="display:none;">LOADING...</div>

 </div>

</div>

<div id="mycontent" class="contentarea"  >

<%--
<span id="loading" class="loading" style="display:none;">LOADING...</span>
--%>

<script type="text/javascript">
    var intervalStatusRefresh = "";
             
    function startStatusRefreshTimer() {
        if (intervalStatusRefresh == "") {
            intervalStatusRefresh = window.setInterval(function () {
                stopStatusRefreshTimer();
                <%-- /*Make the "updating" element visible. Here is alternatate sytax: $('#UpdatingBlock').css({ "visibility": "visible" }); */ --%>
                $('#loading').css('display', 'inline-block');
                $('#partial1').load('<%= Url.Action("SensorStatusDemoPartial","SpaceStatus") %>' + 
                  '?CID=' + encodeURIComponent('<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>'));
            }, <%: (this.ViewData["CustomerCfg"] as CustomerConfig).GetJavaScriptTicksForDemoRefreshInterval() %> );
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
  <% Html.RenderPartial("pv_SensorStatusDemoContent"); %>
</span>


  <%-- This hidden input and "onload" event will help reload the page when navigating back to it (instead of using browser's cached version) --%>
  <input type="hidden" id="refreshed" value="no">
  <script type="text/javascript">
      onload = function () {
          var e = document.getElementById("refreshed");
          if (e.value == "no") {
              e.value = "yes";
          }
          else {
              e.value = "no";
              <%-- /* Page is being displayed from browser cache, so show the "Loading" indictor and force a reload of the page */ --%>
              $('#loading').css('display', 'inline-block');
              location.reload();
          }
      }

      <%-- /* "onunload" event is a fix for Firefox Backward-Forward Cache. See http://stackoverflow.com/questions/2638292/after-travelling-back-in-firefox-history-javascript-wont-run for info */ --%>
      window.onunload = function () { };
  </script>

</div>

<%-- position:absolute; bottom:0px; height:28px; left:0px; overflow:hidden;    border-style:none; border-color:red; --%>
<div id="myfooter"  class="footerarea mobileFooterNav" >

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-for-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />

</div>

</div>

</body>
</html>

