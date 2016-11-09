<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusModel>>" %>

<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">

<meta http-equiv="cache-control" content="no-cache"/> <%-- <!-- tells browser not to cache -->  --%>
<meta http-equiv="expires" content="-1"/> <%-- <!-- says that the cache expires 'now' --> --%>
<meta http-equiv="pragma" content="no-cache"/> <%-- <!-- says not to use cached stuff, if there is any --> --%>

<%-- Meta tags for better viewing on mobile devices, such as the iPhone or Android --%>
<meta name="viewport" content="width=device-width, initial-scale=1" />
<meta name="HandheldFriendly" content="True">
<meta name="MobileOptimized" content="320">

<title>Space Summary Options</title>

<style type="text/css">
<%--     
/*#container {width:450px;}*/
/*#header, #footer2 {width:450px; text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}*/
/*#content {width:450px; height:100%; text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}*/
/*div.side-by-side > div > em { margin-bottom: 10px; display: block; }*/
--%>
body {font-size: 13px;}
#header, #footer2 {text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}
#content {height:100%; text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}
div.side-by-side > div { float: left; padding-left:8px; }
.dtNoLinesOrGaps {display:table; border-collapse:collapse; border-spacing:0px; border-width:0px;}
.dtr {display: table-row; width:100%; border:none; }
.dtrHead {display: table-row; width:100%; border:none; padding:4px; color: black;}
.dtc {display: table-cell;}
.hcenter {text-align: center;}
.vcenter {vertical-align:middle;}
.loading {display: inline-block; left:8px; height: 24px; line-height: 24px; top:50px; width:188px; border:1px solid #000; position:fixed; background-color:#FFFF00; background-color: rgba(255,255,0,.8); color:Black; text-align:center;}
.mb0{margin-bottom:0px;}
.fh{height:100%;}
.grpOutPnl{display:inline-block; margin-bottom:4px; margin-right:4px; vertical-align:top;}
.grpInPnl{display:inline-block; margin:0px; padding:0px; border: 2px solid black;}
.np{padding:0px;}
.notBold{ font-weight:normal;}


.rounded-corners {
     -moz-border-radius: 20px;
    -webkit-border-radius: 20px;
    -khtml-border-radius: 20px;
    border-radius: 6px;
}

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

.ctc{text-align: center; vertical-align:middle; margin-bottom:0px;} /*statusIcon siw */
.cbc{display: table-cell; text-align: center; vertical-align:middle;} /*siw*/
.cc{display: table-cell; text-align: center; vertical-align:middle; margin-bottom:0px; height:100%; margin-top:0px; padding-top:0px; padding-bottom:0px;} /* siw */


* html #loading {position:absolute;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", true, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", true, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", true, this.Model) %>;}



@media screen and (max-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_VerySmall", true, this.Model) %>) <%-- /* for really small mobile devices */ --%>
{
  body {font-size: 13px;}
  div.side-by-side { height:400px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .footerarea  { position:absolute; bottom:0px; height:28px; left:0px; overflow:hidden; border-style:none;  }
  .headerarea {position:absolute; top:0px; left:0px; overflow:hidden; /*color:#004475;*/}
  .contentarea {position:absolute; top:65px; bottom:28px; left:0px; overflow:auto; border-style:none; background-color:white; }
  .pageColHead1{display:inline-block;}
  .pageColHead2{display:none;}
  .pageColHead3{display:none;}
  .pageColHead4{display:none;}

  .statusIcon{display:none;}
  
  .desktopBackBtn{display:none;} 
  .desktopRefreshBtn{display:none;}

  .mobileFooterNav{display:block;}
  .drillDownBtn{width:24px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", true, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", true, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", true, this.Model) %>;}
}

@media screen and (min-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_Small", true, this.Model) %>) <%-- /* for devices with screens at least 301 pixels wide */ --%>
{
  body {font-size: 14px;}
  div.side-by-side {  max-height:24px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix2:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .headerarea {display:block; height:65px; max-height:65px; overflow:visible;}
  .footerarea  { height:28px; left:0px; overflow:hidden;  border-style:none; }
  .contentarea{ /*max-width: 320px;*/ /*max-height:522px;*/ overflow:auto; background-color:white;}
  .pageColHead1{display:inline-block;}
  .pageColHead2{display:none;}
  .pageColHead3{display:none;}
  .pageColHead4{display:none;}

  .statusIcon{display:none;}

  .desktopBackBtn{display:none;} 
  .desktopRefreshBtn{display:none;}

  .mobileFooterNav{display:block;}
  .drillDownBtn{width:24px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", true, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", true, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", true, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", true, this.Model) %>;}
}

@media screen and (min-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_1Column", false, this.Model) %>) <%-- /* for displays wide enough for 1 column (328px>)*/ --%>
{
  body {font-size: 16px;}
  div.side-by-side {  max-height:24px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix2:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .headerarea {display:block; height:65px; max-height:65px; overflow:visible;}
  .footerarea  { height:28px; left:0px; overflow:hidden; border-style:none; }
  .contentarea{ /*max-width: 360px;*/ /*max-height:522px;*/ overflow:auto; background-color:white;}
  .pageColHead1{display:inline-block;}
  .pageColHead2{display:none;}
  .pageColHead3{display:none;}
  .pageColHead4{display:none;}

  .statusIcon{display:none;}

  .desktopBackBtn{display:block;} 
  .desktopRefreshBtn{display:block;}

  .mobileFooterNav{display:none;}
  .drillDownBtn{width:40px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", false, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", false, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", false, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", false, this.Model) %>;}
}

@media screen and (min-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_2Column", false, this.Model) %>) <%-- /* for displays wide enough for 2 columns */ --%>
{
  body {font-size: 16px;}
  div.side-by-side {  max-height:24px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix2:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .headerarea {display:block; height:65px; overflow:visible;}
  .footerarea  { height:28px; left:0px; overflow:hidden; border-style:none; }
  .contentarea{ /*max-width: 680px;*/ /*max-height:522px;*/ overflow:auto; background-color:white;}
  .pageColHead1{display:inline-block;}
  .pageColHead2{display:inline-block;}
  .pageColHead3{display:none;}
  .pageColHead4{display:none;}

  .statusIcon{display:block;}

  .desktopBackBtn{display:block;} 
  .desktopRefreshBtn{display:block;}

  .mobileFooterNav{display:none;}
  .drillDownBtn{width:40px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", false, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", false, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", false, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", false, this.Model) %>;}
}

@media screen and (min-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_3Column", false, this.Model) %>) <%-- /* for displays wide enough for 3 columns */ --%>
{
  body {font-size: 16px;}
  div.side-by-side {  max-height:24px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix2:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .headerarea {display:block; height:65px; overflow:visible;}
  .footerarea  { height:28px; left:0px; overflow:hidden;  border-style:none; }
  .contentarea{ /*max-width: 1000px;*/ /*max-height:522px;*/ overflow:auto; background-color:white;}
  .pageColHead1{display:inline-block;}
  .pageColHead2{display:inline-block;}
  .pageColHead3{display:inline-block;}
  .pageColHead4{display:none;}

  .statusIcon{display:block;}

  .desktopBackBtn{display:block;} 
  .desktopRefreshBtn{display:block;}

  .mobileFooterNav{display:none;}
  .drillDownBtn{width:40px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", false, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", false, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", false, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", false, this.Model) %>;}
}

@media screen and (min-width: <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_4Column", false, this.Model) %>) <%-- /* for displays wide enough for 4 columns */ --%>
{
  body {font-size: 16px;}
  div.side-by-side {  max-height:24px;  <%-- /*width: 100%;*/ /*margin-bottom: 1em;*/ --%> }
  .clearfix2:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden; }
  .headerarea {display:block; height:65px; overflow:visible;}
  .footerarea  { height:28px; left:0px; overflow:hidden;  border-style:none; }
  .contentarea{ max-width: /*1332px*/ <%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("mediaWidth_4Column_MaxWidth", false, this.Model) %>;  /*max-height:522px;*/ overflow:auto; background-color:white;}
  .pageColHead1{display:inline-block;} /*table-row*/
  .pageColHead2{display:inline-block;}
  .pageColHead3{display:inline-block;}
  .pageColHead4{display:inline-block;}

  .statusIcon{display:block;}

  .desktopBackBtn{display:block;} 
  .desktopRefreshBtn{display:block;}

  .mobileFooterNav{display:none;}
  .drillDownBtn{width:40px; height:24px;}

.pageColWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColWidth", false, this.Model) %>;}
.sectionWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("sectionWidth", false, this.Model) %>;}
.pageColTableWidth {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("pageColTableWidth", false, this.Model) %>;}
.siw {width:<%= SpaceStatusHelpers.GroupSummary_CSSDynamicValue("siw", false, this.Model) %>;}
}

</style>


<%-- <!-- Grab Google CDN's jQuery, with a protocol relative URL; fall back to local if offline --> --%>
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js" type="text/javascript"></script>
<script type="text/javascript">    window.jQuery || document.write('<script src="<%= Url.Content("~/Scripts/jquery-1.7.2.min.js") %>"><\/script>')</script>

<link rel="stylesheet" href="<%= Url.Content("~/Scripts/Chosen/chosen.css") %>" type="text/css" />
<script src="<%= Url.Content("~/Scripts/Chosen/chosen.jquery.min.js") %>" type="text/javascript"></script>

</head>

<script type="text/javascript">
    function navigateBackClick(){
      $('#loading').css('display', 'inline-block');
      window.history.back();    
    }

</script>


<body style="margin:0px; padding:0px; font-family: helvetica,arial,sans-serif; font-weight: 700;" >

<div id="container" >

<div id="myheader" class="headerarea">

  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Enforcement - Summary
  </div>
  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Display Options
  </div>

  <div  class="side-by-side clearfix" style="border-style:none; overflow:visible;" >

  <%-- 
  <div class="desktopBackBtn">
  <input type="button" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="navigateBackClick();"  />
  </div>
  --%>

  <%-- 
  <div class="desktopRefreshBtn" >
   <input type="button" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-for-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />
  </div>
  --%>

 </div>

</div>

<div id="mycontent" class="contentarea"  >

<form method="post" action="" style="font-size: 14px; font-weight:normal;">
  <%-- Start -- Display Style parameters --%>
  <div class="rounded-corners" style="display:block; min-width:236px; max-width:236px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px; padding-bottom:8px;" >
      <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;
        "><span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Display Style:</span>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input <%= ViewData["DSBTN1ID"] %> type="radio" name="DSRBtn" value="Dynamic" checked="checked" />Dynamic</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input <%= ViewData["DSBTN2ID"] %> type="radio" name="DSRBtn" value="2Row" />2-Row (Text & Image)</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input <%= ViewData["DSBTN3ID"] %> type="radio" name="DSRBtn" value="1Row" />1-Row (Text only)</label>
    </div>
  </div>
  <%-- End -- Display Style parameters --%>

  <%-- Start -- Submit button --%>
  <div>
    <input type="submit" class="rounded-corners" value="OK" style="width:60px; padding:4px; 
        margin-left: 0px; margin-top: 8px; font-size:medium; background-color: Green; color:White;"/> 

    <input type="button" class="rounded-corners" value="Cancel" style="width:60px; margin-left:4px; padding: 4px;
        margin-left: 0px; margin-top: 8px; font-size: medium; background-color: Red; color: White;" onclick="navigateBackClick();"/>

  </div>
  <%-- End -- Submit button --%>

  <%-- Force a specific radio button to be selected after page load, because some browsers don't respect the "checked" attribute... --%>
  <script type="text/javascript">
      $(document).ready(function () {
          $("#DSRBtnChoiceDefault").attr("checked", true);
      });
  </script>
</form>

</div>

<%-- position:absolute; bottom:0px; height:28px; left:0px; overflow:hidden;    border-style:none; border-color:red; --%>
<div id="myfooter"  class="footerarea mobileFooterNav" >
<%-- 
   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="navigateBackClick();"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="visibility:hidden; width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/zoom-btn-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="visibility:hidden; width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/sort-btn-blue.gif") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;"  />
--%>

<%-- 
   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:40px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-for-blue.png") %>);
   background-repeat: no-repeat; background-position:center center; background-color:#004475;" onclick="refreshCurrentInfo();"  />
--%>
</div>

</div>

</body>
</html>

