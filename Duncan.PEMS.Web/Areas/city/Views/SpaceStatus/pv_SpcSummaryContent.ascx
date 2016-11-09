<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Duncan.PEMS.SpaceStatus.Models.SpaceStatusModel>>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Helpers" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.Models" %>
<%@ Import Namespace="Duncan.PEMS.SpaceStatus.DataShapes" %>

<%= SpaceStatusHelpers.SpaceSummary(Url, this.Model, this.ViewData)%>

<%-- Only include this script if NOT targeting Windows CE (Windows Mobile) --%>
<% if ((ViewData["IsWinCE"] == null) || (Convert.ToBoolean(ViewData["IsWinCE"]) == false))
   {%>
<script type="text/javascript">
    $(function () {
        $('#loading').css('display', 'none');
        startStatusRefreshTimer();
    });
</script>
<% }%>
