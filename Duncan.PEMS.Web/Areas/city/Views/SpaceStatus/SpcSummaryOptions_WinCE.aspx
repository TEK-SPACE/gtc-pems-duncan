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

<title>Space Summary Options</title>

<style type="text/css">
body {font-size: 12px; line-height :12px; height:320; min-height:320; max-height:320px; width:224px; max-width:240px; overflow:hidden;}
#header, #footer2 {text-align:center; background:#f00; color:#fff; font:900 1em arial,sans-serif;}
#content {text-align:left; background:#fff; color:#000; font:1em arial,sans-serif; overflow:auto;}
div.javascript-warning{font-family:"Arial",sans-serif;font-size:14pt;line-height:18pt;font-weight:bold;color:rgb(255,255,255);background-color:rgb(230,0,0);border-color:rgb(0,0,0);border-width:1px;border-style:solid;margin:10px 20px 20px 20px;padding:10px 10px 10px 10px;text-align:center;}
div.side-by-side > div { float: left;  }

.dtNoLinesOrGaps {display:block; margin:0px; padding:0px; overflow:auto; border:none; border-collapse: collapse; border-spacing: 0px; border-width:0px;}
.dtr {display:block; border:none; border-collapse: collapse; border-spacing: 0px; border-width:0px; overflow:hidden; }
.dtrHead {display: table-row; border:none; padding:4px; color: black;}
.dtc {width:218px; font-size :12px; line-height:12px; height: 12px; display:block; margin:0px; margin-left:2px; float:left; white-space:nowrap;}
.dtcLeft {width:109px; font-size:12px;  line-height:12px; display:block;  margin:0px; margin-left:2px; float:left; white-space:nowrap;  }
.dtcRight {width:109px; font-size:12px; line-height:12px; display:block;  margin:0px; float:left; white-space:nowrap;  }
.dtcHead {font-size :12px; line-height:12px; display:block;  margin:0px; float:left; white-space:nowrap;}

.hcenter {text-align: center;}
.txtL {text-align:left;}
.txtR {text-align:right;}
.vcenter {vertical-align:middle;}

.loading {display: inline-block; left:8px; height: 24px; line-height: 24px; top:50px; width:188px; border:1px solid #000; position:absolute; background-color:Yellow; background-color: rgb(255,255,0); color:Black; text-align:center;}

.mb0{margin-bottom:0px;}
.fh{height:100%;}

.grpOutPnl{ margin:0px; padding:0px; background-color:Yellow; clear:both;  overflow: hidden; white-space: nowrap; border:1px solid black; min-width:205px; min-height:22px; font-size:12px; line-height:12px;}
.grpInPnl{display:block;  margin:0px; padding:0px; border: 2px solid black;}
.np{padding:0px;}

.whiteCell{background-color:white; color:Black;}
.NavyTextCell{background-color:white; color:#000066;}
.appleblueCell{background-color:#1A2644; color:White;}
.orangeCell{background-color:Orange; color:White;}
.greenCell{background-color:Green; color: White;}
.redCell{background-color:Red; color: White;}
.grayCell{background-color:#484848; color: White;}
.purpleCell{background-color:#484848; color: White;}
.ltgreenCell{background-color:#9BBB59; color: White;}

.underlineCell{border-bottom: 1px solid #e8eef4;}

.colCellMP{margin-top:0px; padding-top:1px; padding-bottom:4px;}
.notBold{ font-weight:normal;}
.italic{ font-style:italic;}

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
  .contentarea {overflow:auto; border-style:none; /*background-color:rgb(239, 243, 251);*/}
  
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
<%-- <script src="<%= Url.Content("~/Scripts/jx_compressed.js") %>" type="text/javascript"></script> --%>

</head>

<script type="text/javascript">
    function navigateBackClick(){
      window.history.back(); 
      
      <%-- 
      /* 
      window.location.href = '<%= Url.Action("SpcSummary","SpaceStatus") %>' + '?T=' + encodeURIComponent(targetID) + 
        '&G=' + encodeURIComponent('<%= this.ViewData["groupType"].ToString() %>') + 
        '&V=' + encodeURIComponent('<%= this.ViewData["viewType"].ToString() %>') + 
        '&CID=' + encodeURIComponent(<%= (this.ViewData["CustomerCfg"] as CustomerConfig).CustomerId.ToString() %>);
      */
      --%>
    }

<%-- /* We can't use jQuery on Windows Mobile, so we will use this script from https://github.com/ded/domready which gives us an equivalent to the Document.Ready function */ --%>
/*!
* domready (c) Dustin Diaz 2012 - License MIT
*/
!function (name, definition) {
  if (typeof module != 'undefined') module.exports = definition()
  else if (typeof define == 'function' && typeof define.amd == 'object') define(definition)
  else this[name] = definition()
}('domready', function (ready) {

  var fns = [], fn, f = false
    , doc = document
    , testEl = doc.documentElement
    , hack = testEl.doScroll
    , domContentLoaded = 'DOMContentLoaded'
    , addEventListener = 'addEventListener'
    , onreadystatechange = 'onreadystatechange'
    , readyState = 'readyState'
    , loaded = /^loade|c/.test(doc[readyState])

  function flush(f) {
    loaded = 1
    while (f = fns.shift()) f()
  }

  doc[addEventListener] && doc[addEventListener](domContentLoaded, fn = function () {
    doc.removeEventListener(domContentLoaded, fn, f)
    flush()
  }, f)

  hack && doc.attachEvent(onreadystatechange, fn = function () {
    if (/^c/.test(doc[readyState])) {
      doc.detachEvent(onreadystatechange, fn)
      flush()
    }
  })

  return (ready = hack ?
    function (fn) {
      self != top ?
        loaded ? fn() : fns.push(fn) :
        function () {
          try {
            testEl.doScroll('left')
          } catch (e) {
            return setTimeout(function() { ready(fn) }, 50)
          }
          fn()
        }()
    } :
    function (fn) {
      loaded ? fn() : fns.push(fn)
    })
})
</script>


<body style="margin:0px; padding:0px; font-family: helvetica,arial,sans-serif; font-weight: 700; max-height:200px; max-width:240px;" >

<div id="container" >

  <noscript>
    <div class="javascript-warning">
      <div>This page requires JavaScript to be enabled.</div>
    </div>
  </noscript>

  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Space Summary</div>
  <div style="font-style:italic; width:204px; overflow:hidden; max-height:15px; text-align:center; font-size: 14px; color:Blue; padding-bottom:4px;">Display Options</div>

<div id="mycontent" class="contentarea"  >

<form method="post" action="" style="font-size: 14px; font-weight:normal;">
  <%-- Start -- Display Style parameters --%>
  <div style="display:block; width: 220px; min-width:220px; max-width:220px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px; padding-bottom:8px;" >
    <div style="background-color:#565ba3; padding-right:0px; width:100%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;">
      <span style="font-size: 13px; padding-left:6px; padding-top:2px; padding-bottom:2px; color:White; font-weight:bold;">Filter to show:</span>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label for="radio"><input <%= ViewData["FBTN1ID"] %> type="radio" name="FRBtn" value="All" <% if (ViewData["filter"].ToString() == "A") {%> checked="checked" <%}%> />All</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label for="radio"><input <%= ViewData["FBTN2ID"] %> type="radio" name="FRBtn" value="Occupied" <% if (ViewData["filter"].ToString() == "O") {%> checked="checked" <%}%> />Occupied</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label for="radio"><input <%= ViewData["FBTN3ID"] %> type="radio" name="FRBtn" value="Violations" <% if (ViewData["filter"].ToString() == "E") {%> checked="checked" <%}%> />Violations</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label for="radio"><input <%= ViewData["FBTN4ID"] %> type="radio" name="FRBtn" value="OccupiedOrViolations" <% if (ViewData["filter"].ToString() == "OE") {%> checked="checked" <%}%> />Occupied or Violations</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label for="radio"><input <%= ViewData["FBTN5ID"] %> type="radio" name="FRBtn" value="Vacant" <% if (ViewData["filter"].ToString() == "V") {%> checked="checked" <%}%> />Vacant</label>
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
      domready(function () {
          var esel = document.getElementById("FRBtnChoiceDefault");
          esel.checked = true;
      });
  </script>
</form>

</div>

</div>

</body>
</html>

