<%-- We are basing this view on a generic DataSet model instead of an object of our own creaton --%>

<%-- <%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<MeterReportMVC.Models.SpaceStatus>>" %> --%>
<%-- <%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Data.DataSet>" %> --%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<MeterReportMVC.Models.SpaceStatusModel>>" %>


<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="MeterReportMVC.Helpers" %>
<%@ Import Namespace="MeterReportMVC.Models" %>
<%@ Import Namespace="MeterReportMVC.DataShapes" %>

<div id="quote">

    <p id="UpdatingBlock" style="color: Black; font-weight: bold; width: auto; height: auto;
        display: inline-block; padding: 4px; letter-spacing: 3px; background: #ffce79 url( <%= Url.Content("~/content/images/busy_solid.gif") %> ) left center no-repeat !important;
        text-indent: 16px; visibility: hidden">
        Updating</p>

    <%-- We are not using the "IEnumerable<MeterReportMVC.Models.SpaceStatus>>" Model here. Instead we are using a DataSet present in the ViewData --%>
    <%-- This code is here just as a reference example of something that could be done... -->
    <%-- 
    <% foreach (var item in Model)
       { %>
    <div class=" rounded-corners NewMeterBlock">
        <div class="NewMeterTitle">
            Meter
            <%: item.MeterID %>
            <span class="NewMeterSubTitle">[Idle For: 30171 minutes] [Last Updated:
                <%: String.Format("{0}", item.OccupancyEventTS.ToString("HH:mm:ss.f")) %>] </span>
        </div>
        <div class="NewBayBlockGavin">
            <div class="NewBayTitle">
                <%: item.SpaceID %></div>
            <div class="CenteredBlock NewBayExpiryTimeInoperational">
                -+-</div>
            <% if (item.IsOccupied == true)
               {%>
            <div class="BayOccupancyOccupied">
            </div>
            <% } %>
            <% else if (item.IsOccupied == false)
               {%>
            <div class="BayOccupancyVacant">
            </div>
            <% } %>
            <div class="LastInOutTimeNA">
                N/A</div>
            <div class="LastInOutTimeOfDayNA">
                <span>N/A</span>
            </div>
        </div>
    </div>
    <% } %>
    --%>

</div>

<%-- Next line is example of processing DataSet extracted from the ViewData collection --%>
<%--
<%= MeterReportMVC.Helpers.HtmlHelpers.ClusterStatusBlock((this.ViewData["MeterStatusDataset"] as DataSet), (this.ViewData["CustomerCfg"] as CustomerConfig))%>
--%>

<%-- Next line is example of processing DataSet from the Model that this view is based on (System.Data.DataSet) --%>
<%--
<%= MeterReportMVC.Helpers.HtmlHelpers.ClusterStatusBlock(this.Model, (this.ViewData["CustomerCfg"] as CustomerConfig))%>
--%>

<%-- Next line is example of processing data when the model is List<SpaceStatusModel> --%>
<%= MeterReportMVC.Helpers.HtmlHelpers.ClusterStatusBlock(this.Model, (this.ViewData["CustomerCfg"] as CustomerConfig))%>

<%-- Next line is a playground of new aggregate display... --%>
<%--
<%= MeterReportMVC.Helpers.HtmlHelpers.MeterStatusSummaryDrillDown(this.Model, (this.ViewData["CustomerCfg"] as CustomerConfig))%>

<%= MobileOccupancyStatusHelpers.ListView_GroupByMeter(this.Model, (this.ViewData["CustomerCfg"] as CustomerConfig))%>
--%>

<%--  DEBUG: This is a test of the new mobile space status views... --%>
<%--
<%= MeterReportMVC.Helpers.HtmlHelpers.MobileMeterSummaryBlock(this.Model, (this.ViewData["CustomerCfg"] as CustomerConfig))%>
--%>

<%--
<!-- Sample of simple table to grid from ViewData
<table>
    <thead>
    <tr>
    <% foreach (DataColumn col in (this.ViewData["MeterStatusDataset"] as DataSet).Tables["Meter"].Columns)    %>
<%    {%>         
        <th><%= col.ColumnName%></th>
  <%  }  %>  
    </tr>
    </thead>        
    <tbody>
    <% foreach (DataRow row in (this.ViewData["MeterStatusDataset"] as DataSet).Tables["Meter"].Rows)    %>
    <%{    %>    
        <tr>
        <% foreach (DataColumn col in (this.ViewData["MeterStatusDataset"] as DataSet).Tables["Meter"].Columns)        %>
      <%  {  %>           
            <td><%= row[col.ColumnName]%></td>
        <%}    %>    
        </tr>
    <%}    %>
    </tbody>
</table>
-->
--%>

<%-- Sample of simple table to grid from Model, when Model is a DataSet --%>
<%--
<table>
    <thead>
    <tr>
    <% foreach (DataColumn col in Model.Tables["Meter"].Columns)  %>
<%    {%>         
        <th><%= col.ColumnName%></th>
  <%  }  %>  
    </tr>
    </thead>        
    <tbody>
    <% foreach (DataRow row in Model.Tables["Meter"].Rows)    %>
    <%{    %>    
        <tr>
        <% foreach (DataColumn col in Model.Tables["Meter"].Columns)        %>
      <%  {  %>           
            <td><%= row[col.ColumnName]%></td>
        <%}    %>    
        </tr>
    <%}    %>
    </tbody>
</table>
--%>


<script type="text/javascript">
    $(function () {
        startStatusRefreshTimer();
    });
</script>
