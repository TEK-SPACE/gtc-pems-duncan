<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusModel>>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>

<%= MobileOccupancyStatusHelpers.EnfSummaryBySpacesForGroup(this.Model, this.ViewData)%>

<script type="text/javascript">
    $(function () {
        $('#loading').css('display', 'none');
        startStatusRefreshTimer();
    });
</script>
