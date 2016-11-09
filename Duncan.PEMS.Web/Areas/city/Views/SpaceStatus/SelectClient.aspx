<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">

<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true))
   {%>

    <% if ((ViewData["IsWinMobile6_5"] != null) && (Convert.ToBoolean(ViewData["IsWinMobile6_5"]) == true))
       { %>
    <%-- Windows Mobile 6.5 seems pretty screwed up without this magic line that took ages of experimentation to figure out!!! --%>
<meta name="viewport" content="initial-scale=2.0" />
    <% }
       else
       { %>
    <%-- Meta tags for better viewing on mobile devices, such as the iPhone or Android --%>
    <meta name="viewport" content="width=220, initial-scale=1" />
    <meta name="HandheldFriendly" content="True" />
    <meta name="MobileOptimized" content="220" />
    <% }  %>

<style type="text/css">
body, form, table {
	margin: 0;
	padding: 0;
	border: 0;
	font-size: 12px;
	font: inherit;
	vertical-align: baseline;
}
body {
	line-height: 12px;
}
table {
	border-collapse: collapse;
	border-spacing: 0;
}

a.HeaderAnchor{color:white; text-decoration:underline;}
a:hover.HeaderAnchor{color:white; text-decoration: underline;}
a:link.HeaderAnchor{color:white; text-decoration: underline;}
a:visited.HeaderAnchor{color:white; text-decoration: underline;}
a:active.HeaderAnchor{position:relative; top:1px;}
</style>

<% } else { %>
<style type="text/css">
a.HeaderAnchor{color:white; text-decoration:underline;}
a:hover.HeaderAnchor{color:white; text-decoration: underline;}
a:link.HeaderAnchor{color:white; text-decoration: underline;}
a:visited.HeaderAnchor{color:white; text-decoration: underline;}
a:active.HeaderAnchor{position:relative; top:1px;}
</style>
<% } %>
    
    <title>Select Client</title>
</head>
<body>

<%-- Inject a header if targeting Windows Mobile --%>
<% if ((ViewData["IsWinCE"] != null) && (Convert.ToBoolean(ViewData["IsWinCE"]) == true))
   { %>
 <div style="display:block;  border-collapse:collapse; border:none; margin-top: 2px; margin-left: 2px; margin-bottom: 8px;  
    width:220px; max-width:220px; background-color:#004475;" >
 <span style="display:block; width:220px; color:White; border:none; font-family: Arial, Helvetica, sans-serif; font-size:12px; font-weight:bold; color:White;">
     <img alt="" src="<%= Url.Content("~/Content/images/DSLogoTrans38x24.gif")%>" style="display:inline-block; width:38px; height:24px; float:left;"/>
     <span style="display:inline-block; padding-left: 2px; text-align:center; height:12px; line-height:10px; font-size:10px; font-weight:bold; font-style:italic; vertical-align:middle; float:left; width: 165px;">Parking Enterprise Manager</span>
     <span style="display:inline-block; padding-left: 2px; text-align:center; height:12px; line-height:12px; font-size:12px; font-weight:bold; font-style:italic; vertical-align:middle; float:left; width:160px;">MeterREPORT</span>
     <span style="clear:both; visibility:hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;"></span>
 </span>
 <span style="display:inline-block; width:220px; border:none;">
  <span style="display:inline-block; vertical-align: top;" >
   <span style="display:inline-block; color:White; border:none; 
    font-family: Arial, Helvetica, sans-serif; font-size:12px; color:White;">
    <span id="WinCE_UserBlock" style="display:inline-block; height:50%; margin-top:8px;">
      <span style="display:inline-block; vertical-align: bottom; padding-left:4px; padding-right:4px">
       <span style="display:inline-block; background-color:#1A2644; border:none; padding-left: 8px; padding-right: 8px; padding-top: 4px; padding-bottom: 8px;
       vertical-align: middle; "><% Html.RenderPartial("LogOnUserControl", this.ViewData); %></span>
      </span>
    </span>
   </span>
  </span>
</span>
</div>
<% } %>

<div>
    
<% using (Html.BeginForm()) { %>

<table id="Table1" cellspacing="0" cellpadding="0" border="0" 
<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true))
   {%>
    style="width:210px; font-size:0.8em;font-family:Verdana; margin-top:0px; margin-left:3px; margin-right:0px;
     color:#333333;border-width:1px;border-style:Solid;border-color:#B5C7DE;background-color:#EFF3FB;border-collapse:collapse;"
<% } else { %>
    style="width:300px; <%-- height:119px; --%> font-size:0.8em;font-family:Verdana;
     color:#333333;border-width:1px;border-style:Solid;border-color:#B5C7DE;background-color:#EFF3FB;border-collapse:collapse;
     position:absolute; left:50%; margin-left:-150px; top:50%; margin-top:-91px;"
<% } %>
>


<%-- Inject a header if not targeting Windows Mobile ?--%>
<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true))
{%>
<% } else { %>
<tr>
<td>
<table style="display:block;  border-collapse:collapse; border:none; margin-top: 2px; margin-left: 2px; margin-right: 4px; margin-bottom: 8px;  
    width:99%; background-color:#004475;" >
    <tr>
 <td style="display:block; width:100%; color:White; border:none; font-family: Arial, Helvetica, sans-serif; font-size:12px; font-weight:bold; color:White;">
     <img alt="" src="<%= Url.Content("~/Content/images/DSLogoTrans38x24.gif")%>" style="display:inline-block; width:38px; height:24px; float:left;"/>
     <span style="display:inline-block; padding-left: 2px; text-align:center; height:12px; line-height:10px; font-size:10px; font-weight:bold; font-style:normal; vertical-align:middle; float:left; width: 165px;">Parking Enterprise Manager</span>
     <span style="display:inline-block; padding-left: 2px; text-align:center; height:12px; line-height:12px; font-size:12px; font-weight:bold; font-style:normal; vertical-align:middle; float:left; width:160px;">MeterREPORT</span>
     <span style="clear:both; visibility:hidden; height:1px; max-height:1px; font-size:1px; line-height:1px;"></span>
 </td>
 <td style="display:inline-block; width:220px; border:none;">
  <span style="display:inline-block; vertical-align: top;" >
   <span style="display:inline-block; color:White; border:none; 
    font-family: Arial, Helvetica, sans-serif; font-size:12px; color:White;">
    <span id="Span1" style="display:inline-block; height:50%; margin-top:8px;">
      <span style="display:inline-block; vertical-align: bottom; padding-left:4px; padding-right:4px">
       <span style="display:inline-block; background-color:#1A2644; border:none; padding-left: 8px; padding-right: 8px; padding-top: 4px; padding-bottom: 8px;
       vertical-align: middle; "><% Html.RenderPartial("LogOnUserControl", this.ViewData); %></span>
      </span>
    </span>
   </span>
  </span>
</td>
</tr>
</table>
</td>
</tr>
<% } %>



<tr>
<td <%-- style="height: 159px"  --%> >
<table border="0" cellpadding="4" cellspacing="0" style="border-collapse: collapse; <%-- height: 1px; --%>">
<tr>
<td <%-- style="height: 92px"--%>>
<table border="0" cellpadding="0" 
<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true))
   {%>
  style="width: 210px; <%-- height: 119px --%> ">
<% } else { %>
  style="width: 300px; <%-- height: 119px --%> ">
<% } %>
<tr>
<td align="center" colspan="2" style="font-weight: bold; font-size: 0.9em;
color: white; background-color: #507cd1; padding:4px;" >
<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true))
{%>
Select Desired Client</td>
<% } else { %>
Select Desired Client for MeterREPORT</td>
<% } %>
</tr>
<tr>
<td id="Welcome" align="center" colspan="2" style="color: black;
background-color: #eff3fb">
<div id="lblWelcome" style="padding:4px;">
<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true))
{%>
&nbsp;
<% } else { %>
Welcome Back
<% } %>
</div>
</td>
</tr>
<tr>
<td align="right">
<asp:Label ID="Label2" runat="server">Client:</asp:Label></td>
<td align="center">

<%= Html.DropDownList("Customers", ViewData["AvailCustomers"] as SelectList) %>

</td>
</tr>

<tr>
<td align="center" colspan="2">
<input type="submit" value="Continue" 
<% if ((ViewData["IsWinCE"] != null) &&(Convert.ToBoolean(ViewData["IsWinCE"]) == true)) {%> 
style="margin-top: 8px; background-color:White; border-color:#507CD1; border-style:solid; border-width:1px; color:#284E98;" 
<% } else { %>
style="background-color:White; border-color:#507CD1; border-style:solid; border-width:1px; color:#284E98;" 
<% } %>
/>
&nbsp;</td>
</tr>
</table>
</td>
</tr>
</table>
</td>
</tr>
</table>

<% } %>
</div>
</body>
</html>
