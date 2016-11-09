<%-- 
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MeterReportMVC.Models.MeterStatisticObj>>" %>
--%>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Object>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Compliance Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


<style type="text/css">
    .scopeElementNotDisplayed { display:none; }
</style>

<%-- CSS and Scripts for jQuery DateTimePicker --%>
<style type="text/css">
  .ui-timepicker-div .ui-widget-header { margin-bottom: 8px; }
  .ui-timepicker-div dl { text-align: left; }
  .ui-timepicker-div dl dt { height: 25px; margin-bottom: -25px; }
  .ui-timepicker-div dl dd { margin: 0 10px 10px 65px; }
  .ui-timepicker-div td { font-size: 90%; }
  .ui-tpicker-grid-label { background: none; border: none; margin: 0; padding: 0; }
</style>
<script src="<%= Url.Content("~/Scripts/jquery-ui-timepicker-addon.js") %>" type="text/javascript"></script>

<%-- CSS and Scripts for "Chosen" (Nicer combobox) --%>
<link rel="stylesheet" href="<%= Url.Content("~/Scripts/Chosen/chosen.css") %>" type="text/css" />
<script src="<%= Url.Content("~/Scripts/Chosen/chosen.jquery.min.js") %>" type="text/javascript"></script>

<%-- 
  Function that will be called before Form submit to do client-side validation.  This will ensure that at least one of the
  checkboxes is selected.  If none are selected, it will show validation error block and return "false" which prevents the 
  form from being submitted
--%>
<script type="text/javascript">
    check = function (form) {
        try {
            with (form) {
                var fields = $("input[name='chkRptContent']").serializeArray();
                if (fields.length == 0) {
                    $("#pnlRptContentValidation").show();
                    return false;
                }
                else {
                    $("#pnlRptContentValidation").hide();
                }
            }
        } catch (e) {
            $("#pnlRptContentValidation").show();
            return false;
        }
    }
</script>

<h2>Space Compliance Reporting</h2>

  <p><%: Html.ActionLink("Return to menu", "Index", new { controller = "Home", CID = this.ViewData["CID"] }, new { @class = "BackAction" })%></p>


<form name="form1" id="my_form" method="post" action="" onsubmit="return check( this );" style="font-size: 13px; color: Black; padding-left: 8px; border-collapse:separate;">

  <%-- Start -- Date Range parameters --%>
  <div class="rounded-corners" style="display:block; min-width:308px; max-width:308px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px;" >
    
    <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;">
        <span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Date Range:</span>
    </div>

    <div style="display:inline-table; margin-top:4px; margin-bottom:4px;">
    
    <div style="display:table-row">
      <div style="display:table-cell">
	    <div style="margin-top:4px; margin-bottom:4px; margin-right:4px;">
	      <label style="padding-left:12px; margin-right:4px; margin-bottom:4px;">Start Date:</label>
	    </div>
      </div>
      <div style="display:table-cell">
	    <input type="text" name="startDate" id="startDate" value="" style="margin-left:4px; width:auto;" />
      </div>
    </div>

    <div style="display:table-row">
      <div style="display:table-cell">
	    <div style="margin-top:4px; margin-bottom:4px; margin-right:4px;">
	      <label style="padding-left:12px; margin-right:4px; margin-bottom:4px;">End Date:</label>
	    </div>
      </div>
      <div style="display:table-cell">
        <input type="text" name="endDate" id="endDate" value="" style="margin-left:4px; width:auto;" />
      </div>
    </div>

    </div>

    <%-- This function will convert the start and end date inputs into the nicer jQuery DateTimePicker.
         I also contains ViewData place holders that will be injected with server-generated javascript
         to set the default date and time values  --%>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#startDate").datetimepicker({ dateFormat: 'yy-mm-dd', timeFormat: 'hh:mm:ss tt', showOn: "both", buttonImage: "<%= Url.Content("~/Content/images/calendar.gif") %>", buttonImageOnly: true});
            <%= ViewData["ReportStartDate_JavascriptDefaulter"]%>

            $("#endDate").datetimepicker({ dateFormat: 'yy-mm-dd', timeFormat: 'hh:mm:ss tt', showOn: "both", buttonImage: "<%= Url.Content("~/Content/images/calendar.gif") %>", buttonImageOnly: true});
            <%= ViewData["ReportEndDate_JavascriptDefaulter"]%>

            <%-- /* Let's get a nicer alignment of the calendar trigger button so it is more vertically centered with the edit control */ --%>
            $("img[class='ui-datepicker-trigger']").each(function () {
                $(this).attr('style', 'margin-bottom:-3px; margin-left:3px;');
            });

            <%-- 
            /* Note that months for Date object in javascript are 0 - 11 instead of 1 - 12! */
            /* This example would set the date/time to Dec 25th at 12:25 AM */
            /*
            $("#startDate").datetimepicker('setDate', (new Date(2012, 11, 25, 00, 25)));
            */
            --%>
          }); 
    </script>

  </div>
  <%-- End -- Date Range parameters --%>

  <%-- Start -- Zone/Scope parameters --%>
  <div class="rounded-corners" style="display:block; min-width:308px; max-width:308px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px;" >
    <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;
        "><span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Limit report to:</span></div>

    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input id="ScopeChoiceDefault" type="radio" name="ScopeRBtn" value="pnlSiteWide" checked="checked" />All (Site-wide)</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input type="radio" name="ScopeRBtn" value="pnlArea" />Specific Area</label>
        <div id="pnlArea" class="scopeElementNotDisplayed" style="padding-left: 24px; display: inline-block; vertical-align: middle;">
        <span style="display: block; vertical-align: middle">
          <%= Html.DropDownList("AreaSelect", ViewData["AreaNameList"] as SelectList, new { @class = "chzn-select", style = "min-width:200px; width:200px;" })%>
        </span>
        </div>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input type="radio" name="ScopeRBtn" value="pnlMeter" />Specific Meter</label>
        <div id="pnlMeter" class="scopeElementNotDisplayed" style="padding-left: 24px; display: inline-block; vertical-align: middle;">
        <span style="display: block; vertical-align: middle">
          <%= Html.DropDownList("MeterSelect", ViewData["MeterList"] as SelectList, new { @class = "chzn-select", style = "min-width:100px; width:200px;" })%>
        </span>
        </div>
    </div>
  </div>
  <%-- End -- Zone/Scope parameters --%>


  <%-- 
  <div class="rounded-corners" style="display:block; min-width:308px; max-width:308px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px; padding-bottom:8px;" >
      <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;
        "><span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Aggregate Level:</span>
    </div>
    <span style="padding-left: 12px; padding-right:4px; min-height: 28px; vertical-align: middle;">
        <label><input id="AggregateChoiceDefault" type="radio" name="AggregateRBtn" value="Area" checked="checked" />Area</label>
    </span>
    <span style="padding-left: 12px; padding-right:4px; min-height: 28px; vertical-align: middle;">
        <label><input type="radio" name="AggregateRBtn" value="Meter" />Meter</label>
    </span>
    <span style="padding-left: 12px; padding-right:4px; min-height: 28px; vertical-align: middle;">
        <label><input type="radio" name="AggregateRBtn" value="Space" />Space</label>
    </span>
  </div>
  --%>

  <%-- Start -- Report Content parameters --%>
  <div id="pnlRptContent" class="rounded-corners" style="display:block; min-width:308px; max-width:308px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px; padding-bottom:8px;" >
    <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;
        "><span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Content to Include:</span>
    </div>
    <div style="padding-left: 12px; padding-right:4px;"><label><input name="chkRptContent" id="chkRptContent_0" type="checkbox" value="inclAreaSum" checked="checked" />Area summary</label></div>
    <div style="padding-left: 12px; padding-right:4px;"><label><input name="chkRptContent" id="chkRptContent_1" type="checkbox" value="inclMeterSum" checked="checked" />Meter summary</label></div>
    <div style="padding-left: 12px; padding-right:4px;"><label><input name="chkRptContent" id="chkRptContent_2" type="checkbox" value="inclSpaceSum" checked="checked" />Space summary</label></div>
    <div id="pnlRptContentValidation" style="padding-left: 12px; padding-right:4px; margin-top:4px; display:none;">
      <span class="input-validation-error" style="font-size: 11px; font-style:italic; padding: 4px; box-shadow: 0 0 2px red; -moz-box-shadow: 0 0 2px red; -webkit-box-shadow: 0 0 2px red;">* At least one option must be selected</span>
    </div>
  </div>
  <%-- End -- Report Content parameters --%>

  <%-- This function will run after the document is loaded and will assign "OnChange" events to the checkboxes which will do client-side
       validations to make sure at least one option is checked --%>
  <script type="text/javascript">
      $(document).ready(function () {
          $('#pnlRptContent :checkbox').change(function () {
              var fields = $("input[name='chkRptContent']").serializeArray();
              if (fields.length == 0) {
                  $("#pnlRptContentValidation").show();
              }
              else {
                  $("#pnlRptContentValidation").hide();
              }
          });
      });
  </script>

  <%-- Start -- Activity Restriction parameters --%>
  <div class="rounded-corners" style="display:block; min-width:308px; max-width:308px; border:1px solid #a38d12; overflow:visible; margin-bottom:8px; padding-bottom:8px;" >
      <div style="background-color:#565ba3; padding-right:1px; width:99%; overflow:hidden; margin:1px; outline:none; padding-bottom:2px;  
        -moz-border-top-left-radius: 20px; -webkit-border-top-left-radius: 20px; -khtml-border-top-left-radius: 20px; border-top-left-radius: 6px;
        -moz-border-top-right-radius: 20px; -webkit-border-top-right-radius: 20px; -khtml-border-top-right-radius: 20px; border-top-right-radius: 6px;
        "><span style="font-size: 13px; padding-left:6px; color:White; font-weight:bold;">Activity Restrictions:</span>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input id="ActRestrictChoiceDefault" type="radio" name="ActRestrictRBtn" value="All" checked="checked" />All Activity</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input type="radio" name="ActRestrictRBtn" value="Regulated" />Only from Regulated hours</label>
    </div>
    <div style="padding-left: 12px; padding-right:4px; min-height: 18px; vertical-align: middle;">
        <label><input type="radio" name="ActRestrictRBtn" value="Unregulated" />Only from Unregulated hours</label>
    </div>
  </div>
  <%-- End -- Activity Restriction parameters --%>

  <%-- Start -- Submit button --%>
  <div>
    <input type="submit" class="rounded-corners" value="Run Report" style="background-image: url(<%= Url.Content("~/content/images/XLS_File.gif") %>);
        background-repeat: no-repeat; background-position: 4px center; text-indent:18px; padding:4px; margin-top:8px; font-size:medium;
        background-color:#EFD883;"/> 
  </div>
  <%-- End -- Submit button --%>

  <%-- Force a specific radio button to be selected after page load, because some browsers don't respect the "checked" attribute... --%>
  <script type="text/javascript">
      $(document).ready(function () {
          $("#ScopeChoiceDefault").attr("checked", true);
          $("#AggregateChoiceDefault").attr("checked", true);
          $("#ActRestrictChoiceDefault").attr("checked", true);
      });
  </script>

</form>
    
    <%-- Turns the standard comboboxes into the nicer "Chosen" from http://harvesthq.github.com/chosen/ --%>
    <script type="text/javascript">
        $(".chzn-select").chosen(); $(".chzn-select-deselect").chosen({ allow_single_deselect: true });
        $("div.scopeElementNotDisplayed").hide();
        $("#ScopeChoiceDefault").attr("checked", true);
    </script>

    <%-- Click event for the radio buttons in "Scope" group. We want to show the combobox applicable to current radio button, and hide the ones that aren't --%>
    <script type="text/javascript">
        $(document).ready(function () {
            $("input[name$='ScopeRBtn']").click(function () {

                var test = $(this).val();
                $("div.scopeElementNotDisplayed").hide();
                $("div.scopeElementNotDisplayed_Sometimes").hide();
                $("#" + test).show();

                // Chosen doesn't show up well if it was originally hidden. To rectify, we will have to remove the style and reapply it
                var element = $(".chzn-select");
                if (element.hasClass('chzn-done')) {
                    element.next('[id*=_chzn]').remove();
                    element.removeClass('chzn-done').css('display', 'block');
                }

                // Re-init the comboboxes to become "Chosen" comboboxes
                $(".chzn-select").chosen(); $(".chzn-select-deselect").chosen({ allow_single_deselect: true });

                // Set the desired font size for the "Chosen" comboboxes
                element = $(".chzn-container")
                element.css('font-size', '13px');
            });
        });
    </script>

</asp:Content>

