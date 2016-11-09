<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%-- 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>MobileIETest</title>
</head>
<body>
    <div>
    
    </div>
</body>
</html>
--%>

<%--
<HTML xmlns:IE>
<HEAD>
<STYLE>
@media all {
IE\:CLIENTCAPS {behavior:url(#default#clientCaps)}
}
</STYLE>
<SCRIPT>
    function window.onload() {
        sTempStr = "availHeight    = " + oClientCaps.availHeight + "\n" +
"availWidth     = " + oClientCaps.availWidth + "\n" +
"bufferDepth    = " + oClientCaps.bufferDepth + "\n" +
"colorDepth     = " + oClientCaps.colorDepth + "\n" +
"connectionType = " + oClientCaps.connectionType + "\n" +
"cookieEnabled  = " + oClientCaps.cookieEnabled + "\n" +
"cpuClass       = " + oClientCaps.cpuClass + "\n" +
"height         = " + oClientCaps.height + "\n" +
"javaEnabled    = " + oClientCaps.javaEnabled + "\n" +
"platform       = " + oClientCaps.platform + "\n" +
"systemLanguage = " + oClientCaps.systemLanguage + "\n" +
"userLanguage   = " + oClientCaps.userLanguage + "\n" +
"width          = " + oClientCaps.width + "\n";
        oPre.innerText = sTempStr;
    }
</SCRIPT>
</HEAD>
<BODY>
<H1>clientCaps Behavior Sample</H1>
<P>This example shows how to use the new <B>clientCaps</B>
behavior, introduced in Microsoft Internet Explorer 5, to obtain
client capabilities information. The following is a sampling of the
information that can be obtained:</P>
<IE:CLIENTCAPS ID="oClientCaps" />
<PRE id="oPre"></PRE>
</BODY>
</HTML>
--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ie>
<head>
<title>IE Specific Browser Detect</title>
<meta http-equiv="content-type" content="text/html; charset=ISO-8859-1" />
<style>
<!--
@media all { IE\:clientCaps {behavior:url(#default#clientCaps)}
} 
-->
</style>
</head>
<body>
<ie:clientcaps id="oClientCaps" />
<script type="text/jscript">
<!--
document.write("<h2>Screen Capabilities</h2>");
document.write("Screen Height: "  + oClientCaps.height + "<br />");
document.write("Screen Width: "  + oClientCaps.width + "<br />");
document.write("Available Height: "  + oClientCaps.availHeight + "<br />");
document.write("Available Width: "  + oClientCaps.availWidth + "<br />");
document.write("Color Depth: "  + oClientCaps.colorDepth + "bit<br />");
document.write("<h2>Browser Capabilites</h2>");
document.write("Cookies On? "  + oClientCaps.cookieEnabled + "<br />");
document.write("Java Enabled? "  + oClientCaps.javaEnabled + "<br />");
document.write("<h2>System and Connection Characteristics</h2>");
document.write("Connection Type: "  + oClientCaps.connectionType + "<br />");
document.write("CPU: "  + oClientCaps.cpuClass + "<br />");
document.write("Platform: "  + oClientCaps.platform + "<br />");
document.write("<h2>Language Issues</h2>");
document.write("System Language: "  + oClientCaps.systemLanguage + "<br />");
document.write("User Language: "  + oClientCaps.userLanguage + "<br />");
// -->
</script>
</body>
</html>