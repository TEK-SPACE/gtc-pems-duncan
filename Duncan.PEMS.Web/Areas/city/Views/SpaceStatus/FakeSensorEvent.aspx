<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>
<%@ Import Namespace="System.Linq" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Fake Sensor Event
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<link rel="stylesheet" href="<%= Url.Content("~/Scripts/Chosen/chosen.css") %>" type="text/css" />
<script src="<%= Url.Content("~/Scripts/Chosen/chosen.jquery.min.js") %>" type="text/javascript"></script>

<style type="text/css">
    .fieldsetCompact
    {
        margin: 2px; margin-left: 0px; padding: 4px; /*display: inline-block; border:none; */
    }
    
    .display-label, .editor-label
    {
        display: inline-block; margin: 4px 4px 4px 4px;
    }
    
    .display-field, .editor-field
    {
        display: inline-block; margin: 4px 8px 4px 2px;
    }
</style>

<div style="margin-left: 12px; max-width:420px;">

    <h2>Fake Sensor Event</h2>
    
    <span>Select the desired space below, and choose if you want a vehicle sensing event to be triggered with Occupied or Vacant status.</span>

<form name="form1" id="my_form" method="post" action="" >
<fieldset class="fieldsetCompact" style="display: inline-block; border:none;">

 <div class="rounded-corners" style="display:block; width:600px; max-width:600px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px;" >
    <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;
        "><span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Sensor Event Data:</span></div>

  <div style="margin-top:8px; margin-left:12px; margin-right:12px;">
   <span style="line-height: 24px; vertical-align: top;">Space:</span>
   <span>
    <%= Html.DropDownList("GroupSelect", ViewData["groupChoices"] as SelectList, new { @class = "chzn-select", style = "min-width:100px;" })%>
   </span>
  </div>

  <div style="margin-top:8px; margin-bottom:8px; margin-left:12px; margin-right:12px;"><label><input 
    <% if (ViewData["StatusCode"].ToString() == "1") {%>
    <%= "class=\"CheckBoxIsChecked\"" %>
    <%}%> 
    name="chkIsOccupied" id="chkIsOccupied_0" type="checkbox" value="IsOccupied" />Is Occupied?</label></div>

    <% if (ViewData["ResponseText"].ToString().Length > 0) {%>
    <br />
    <div style="margin-top:8px; margin-bottom:8px; margin-left:12px; margin-right:12px;">
    <strong>Last Response:</strong> <span><%: ViewData["ResponseText"].ToString() %></span>
    </div>
    <%}%> 

    </div>

</fieldset>

    <div>
        <input type="submit" class="rounded-corners" value="SUBMIT" style="padding: 4px;
            margin-left: 0px; margin-top: 8px; font-size: medium; background-color: Green; color: White" />
    </div>

</form>

    <script type="text/javascript">
        $(".chzn-select").chosen(); $(".chzn-select-deselect").chosen({ allow_single_deselect: true }); 
    </script>
    
    <script type="text/javascript">
        $("#GroupSelect").chosen().change(function () {
            targetID = $("#GroupSelect option:selected").val();
        });
     </script>

  <%-- Force a specific checkbox to be selected after page load, because some browsers don't respect the "checked" attribute... --%>
  <script type="text/javascript">
      $(document).ready(function () {
          $(".CheckBoxIsChecked").attr("checked", true);
      });
  </script>

  <script type="text/javascript">
      onload = function () {
            $("#GroupSelect").prop("selectedIndex", <%=ViewData["SelectedIndex"]%>);
          }

      /* "onunload" event is a fix for Firefox Backward-Forward Cache. See http://stackoverflow.com/questions/2638292/after-travelling-back-in-firefox-history-javascript-wont-run for info */
      window.onunload = function () { };
  </script>

  </div>
</asp:Content>
