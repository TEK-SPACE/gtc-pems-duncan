<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%-- Original code.  We have modified this to match PEM's style a little bit more --%>
<%--
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%: Page.User.Identity.Name %></b>!
        [ <%: Html.ActionLink("Log Off", "LogOff", "Account") %> ]
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Log On", "Login", "Account") %> ]
<%
    }
%>
--%>

<%
    if (Request.IsAuthenticated) {
%>

<% if ((ViewData["IsWinCE"] == null) || (Convert.ToBoolean(ViewData["IsWinCE"]) == false))
   {%>
Logged in as <u><%: MeterReportMVC.Models.LogOnModel.RemoveDomainFromUsername(Page.User.Identity.Name)%></u> |
<% } else {  %>        
User: <%: MeterReportMVC.Models.LogOnModel.RemoveDomainFromUsername(Page.User.Identity.Name)%> |
<% } %>        
<%: Html.ActionLink("Log Out", "LogOff", "Account", new {}, new {@class = "HeaderAnchor"}) %> 
<%
    }
    else {
%> 
        [ <%: Html.ActionLink("Log In", "Login", "Account", new {}, new {@class = "HeaderAnchor"}) %> ]
<%
    }
%>
