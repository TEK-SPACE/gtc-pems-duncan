<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusModel>>" %>

<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">

<title>Mobile VS Space Details</title>

<style type="text/css">
#container {width:450px;}
#header, #footer2 {width:450px; text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}
#content {width:450px; height:100%; text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}
div.side-by-side { /*width: 100%;*/ /*margin-bottom: 1em;*/ }
div.side-by-side > div { float: left; /*width: 50%;*/ padding-left:8px; }
/*div.side-by-side > div > em { margin-bottom: 10px; display: block; }*/
.clearfix:after {content: "\0020"; display: block; height: 0; clear: both; overflow: hidden; visibility: hidden;}
.dtNoLinesOrGaps {display:table; border-collapse:collapse; border-spacing:0px; border-width:0px;}
.dtr {display: table-row;}
.dtc {display: table-cell;}
.hcenter {text-align: center;}
.vcenter {vertical-align:middle;}
</style>

<link rel="stylesheet" href="<%= Url.Content("~/Scripts/Chosen/chosen.css") %>" type="text/css" />
<script src="<%= Url.Content("~/Scripts/jquery-1.7.2.min.js") %>" type="text/javascript"></script>  
<script src="<%= Url.Content("~/Scripts/Chosen/chosen.jquery.min.js") %>" type="text/javascript"></script>

</head>


<script type="text/javascript">
    var targetID = "";
    var customerID = '<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>';

    function drilldownToSelected() {
        window.location.replace('<%= Url.Action("MobileSpaceDetails","SpaceStatus") %>' + '?targetID=' + encodeURIComponent(targetID) + '&CID=' + encodeURIComponent(customerID));
    }
</script>

<body style="margin:0px; padding:0px; font-family: helvetica,arial,sans-serif; font-size: 14px; font-weight: 700;" >

<div id="container">

<div id="myheader" style="position:absolute; top:0px; left:0px; overflow:hidden; color:#004475;">
  <div style="font-style:italic; width:204px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Vacancies - Space <%=ViewData["CurrentSpace"]%></div>

  <div  class="side-by-side clearfix" style="height:400px; border-style:none; border-color:yellow; overflow:visible;" >

  <div>
  Space:
  </div>
  <div>
 <%= Html.DropDownList("GroupSelect", ViewData["SpaceList"] as SelectList, new { @class = "chzn-select", style = "min-width:100px;" })%>
  <script type="text/javascript">      $(".chzn-select").chosen(); $(".chzn-select-deselect").chosen({ allow_single_deselect: true }); </script>
  
  <%-- This is at a space-detail level, so there are no other spaces details that can be scrolled to on this page --%>
  <%-- 
  <script type="text/javascript">
      $("#GroupSelect").chosen().change(function () {
          targetID = $("#GroupSelect option:selected").val();
          document.getElementById("m" + $("#GroupSelect option:selected").val()).scrollIntoView();
      });
   </script>
  --%>
  </div>

  <div>
   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-right-blue.gif") %>);
   background-repeat: no-repeat; background-position:0 0;" onclick="drilldownToSelected();"  />

  </div>
 </div>
</div>

<div id="mycontent" style="position:absolute; top:50px; bottom:28px; left:0px; overflow:auto;   border-style:none; border-color:blue; " >

<%= MobileOccupancyStatusHelpers.SpaceDetails(this.Model, (this.ViewData["CustomerCfg"] as CustomerConfig))%>

</div>

<div id="myfooter"  style="position:absolute; bottom:0px; height:28px; left:0px; overflow:hidden;    border-style:none; border-color:red; ">
   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-left-blue.gif") %>);
   background-repeat: no-repeat; background-position:0 0;" onclick="window.history.back();"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:36px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/zoom-btn-blue.png") %>);
   background-repeat: no-repeat; background-position:0 0;"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:36px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/sort-btn-blue.gif") %>);
   background-repeat: no-repeat; background-position:0 0;"  />

   <input type="button" class="treeActionBtnBOGUS" value="&nbsp;" style="width:24px; height:24px; border-style:none; background-image: url(<%= Url.Content("~/content/images/arrow-refresh-blue.png") %>);
   background-repeat: no-repeat; background-position:0 0;" onclick="window.location.reload( true );"  />
</div>

</div>

</body>
</html>

