using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

using System.Web.Mvc;

using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;
using Duncan.PEMS.SpaceStatus.DataSuppliers;


namespace Duncan.PEMS.SpaceStatus.Helpers
{
    public static class AdminInfoRenderer
    {
        public static string RenderSystemInfo(ViewDataDictionary ViewData)
        {
            string result = string.Empty;
            BaseTagHelper table = null;
            BaseTagHelper tblRow = null;
            BaseTagHelper tblHeader = null;
            BaseTagHelper tblData = null;

            // Create table
            table = new BaseTagHelper("table");

            // Create table header
            tblRow = new BaseTagHelper("tr");
            table.Children.Add(tblRow);
            tblRow.AddCssClass("yellow");
            tblHeader = new BaseTagHelper("th");
            tblRow.Children.Add(tblHeader);
            tblHeader.SetInnerText("Property");
            tblHeader = new BaseTagHelper("th");
            tblRow.Children.Add(tblHeader);
            tblHeader.SetInnerText("Value");

            // Try to log some application and framework component version information. Ignore any exceptions here
            try
            {
                Assembly thisWebAppAssembly = MethodBase.GetCurrentMethod().DeclaringType.Assembly;

                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("Application");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(thisWebAppAssembly.GetName().Name + " v" + thisWebAppAssembly.GetName().Version.ToString());

                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("MS MVC");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("ASP.NET MVC " + typeof(Controller).Assembly.ImageRuntimeVersion);

                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("MS .NET");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(".NET Framework " + typeof(Object).Assembly.ImageRuntimeVersion);
            }
            catch { }

            // Try to get info about the IIS version
            try
            {
                string iisVersion = string.Empty;
                iisVersion = (ViewData["SERVER_SOFTWARE"] as string); //Request.ServerVariables["SERVER_SOFTWARE"];
                if (string.IsNullOrEmpty(iisVersion))
                {
                    string loFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\inetinfo.exe");
                    if (!File.Exists(loFilename))
                        loFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "inetsrv\\InetMgr.exe");

                    if (File.Exists(loFilename))
                    {
                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(loFilename);

                        // Add data row and its cells
                        tblRow = new BaseTagHelper("tr");
                        table.Children.Add(tblRow);
                        tblData = new BaseTagHelper("td");
                        tblRow.Children.Add(tblData);
                        tblData.AddCssClass("tdLeft");
                        tblData.SetInnerText("MS IIS");
                        tblData = new BaseTagHelper("td");
                        tblRow.Children.Add(tblData);
                        tblData.AddCssClass("tdLeft");
                        tblData.SetInnerText("Internet Information Services v" + myFileVersionInfo.FileVersion);
                    }
                }
                else
                {
                    // Add data row and its cells
                    tblRow = new BaseTagHelper("tr");
                    table.Children.Add(tblRow);
                    tblData = new BaseTagHelper("td");
                    tblRow.Children.Add(tblData);
                    tblData.AddCssClass("tdLeft");
                    tblData.SetInnerText("MS IIS");
                    tblData = new BaseTagHelper("td");
                    tblRow.Children.Add(tblData);
                    tblData.AddCssClass("tdLeft");
                    tblData.SetInnerText(iisVersion);
                }
            }
            catch { }


            // Try to log info about which user the website's AppPool/WorkerProcess runs as. Ignore any exceptions here
            try
            {
                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("Process User Context");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            }
            catch { }

            // Try to log info about which app pool we are running under. Ignore any exceptions here
            try
            {
                string webAppPool = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);
                if (!string.IsNullOrEmpty(webAppPool))
                {
                    // Add data row and its cells
                    tblRow = new BaseTagHelper("tr");
                    table.Children.Add(tblRow);
                    tblData = new BaseTagHelper("td");
                    tblRow.Children.Add(tblData);
                    tblData.AddCssClass("tdLeft");
                    tblData.SetInnerText("Web App Pool");
                    tblData = new BaseTagHelper("td");
                    tblRow.Children.Add(tblData);
                    tblData.AddCssClass("tdLeft");
                    tblData.SetInnerText(webAppPool);
                }
            }
            catch { }


            // Try to log some Operating System info. Ignore any exceptions here
            try
            {
                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("Machine Name");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(Environment.MachineName);

                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("Machine Type");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                if (Environment.Is64BitOperatingSystem)
                    tblData.SetInnerText("64-Bit, " + Environment.ProcessorCount.ToString() + " processors");
                else
                    tblData.SetInnerText("32-Bit, " + Environment.ProcessorCount.ToString() + " processors");

                // Try to get processer info
                try
                {
                    string Query = "Select Name from Win32_Processor";
                    System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(Query);
                    string processorInfo = string.Empty;
                    foreach (System.Management.ManagementObject Win32 in searcher.Get())
                    {
                        processorInfo = Win32["Name"] as string;
                        if (!string.IsNullOrEmpty(processorInfo))
                        {
                            // Add data row and its cells
                            tblRow = new BaseTagHelper("tr");
                            table.Children.Add(tblRow);
                            tblData = new BaseTagHelper("td");
                            tblRow.Children.Add(tblData);
                            tblData.AddCssClass("tdLeft");
                            tblData.SetInnerText("Processor");
                            tblData = new BaseTagHelper("td");
                            tblRow.Children.Add(tblData);
                            tblData.AddCssClass("tdLeft");
                            tblData.SetInnerText(processorInfo);
                        }
                    }
                    searcher.Dispose();
                }
                catch
                {
                }

                // Try to get memory information
                try
                {
                    string Query = "Select TotalVisibleMemorySize from Win32_OperatingSystem";
                    System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(Query);
                    string sysMem = string.Empty;
                    foreach (System.Management.ManagementObject Win32 in searcher.Get())
                    {
                        sysMem = (Convert.ToInt32(Win32["TotalVisibleMemorySize"]) / 1024).ToString() + " MB";
                        // Add data row and its cells
                        tblRow = new BaseTagHelper("tr");
                        table.Children.Add(tblRow);
                        tblData = new BaseTagHelper("td");
                        tblRow.Children.Add(tblData);
                        tblData.AddCssClass("tdLeft");
                        tblData.SetInnerText("System Memory");
                        tblData = new BaseTagHelper("td");
                        tblRow.Children.Add(tblData);
                        tblData.AddCssClass("tdLeft");
                        tblData.SetInnerText(sysMem);
                    }
                    searcher.Dispose();
                }
                catch
                {
                }

                // Try to get the operating system name from management searcher. If that fails,
                // fallback to Environment.OSVersion, which unfortunately isn't as accurate
                try
                {
                    string Query = "Select Name from Win32_OperatingSystem";
                    System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(Query);
                    string OSVersion = string.Empty;
                    foreach (System.Management.ManagementObject Win32 in searcher.Get())
                    {
                        OSVersion = Win32["Name"] as string;
                        if (!string.IsNullOrEmpty(OSVersion))
                        {
                            // Add data row and its cells
                            tblRow = new BaseTagHelper("tr");
                            table.Children.Add(tblRow);
                            tblData = new BaseTagHelper("td");
                            tblRow.Children.Add(tblData);
                            tblData.AddCssClass("tdLeft");
                            tblData.SetInnerText("Windows OS");
                            tblData = new BaseTagHelper("td");
                            tblRow.Children.Add(tblData);
                            tblData.AddCssClass("tdLeft");
                            tblData.SetInnerText(OSVersion);
                            //break;
                        }

                        if (string.IsNullOrEmpty(OSVersion))
                        {
                            // Add data row and its cells
                            tblRow = new BaseTagHelper("tr");
                            table.Children.Add(tblRow);
                            tblData = new BaseTagHelper("td");
                            tblRow.Children.Add(tblData);
                            tblData.AddCssClass("tdLeft");
                            tblData.SetInnerText("Windows OS");
                            tblData = new BaseTagHelper("td");
                            tblRow.Children.Add(tblData);
                            tblData.AddCssClass("tdLeft");
                            tblData.SetInnerText(Environment.OSVersion.ToString());
                        }
                    }
                }
                catch
                {
                }
            }
            catch { }

            // Let's try to get some info about server times
            try
            {
                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("IIS Server Timezone");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(CustomerLogic.CustomerManager.Instance.ServerTimeZone.StandardName + "  [ " + CustomerLogic.CustomerManager.Instance.ServerTimeZone.DisplayName + " ]");

                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("IIS Server Clock");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(DateTime.Now.ToString("yyyy-M-d H:m:s"));

                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("DB Server Clock");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(CustomerLogic.CustomerManager.GetDBServerTime().ToString("yyyy-M-d H:m:s"));

                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("Your Browser Clock");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerHtml(
                    "<script language=\"javascript\">" + Environment.NewLine +
                    "ourDate = new Date();" + Environment.NewLine +
                    "document.write(ourDate.getFullYear() + \"-\" + (ourDate.getMonth() + 1) + \"-\" + ourDate.getDate() + \" \" +" +
                    "ourDate.getHours() + \":\" + ourDate.getMinutes() + \":\" + ourDate.getSeconds());" + Environment.NewLine +
                    /*"document.write(ourDate.toLocaleString());" + Environment.NewLine +*/
                    "</script>"
                    );
            }
            catch { }

            try
            {
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["RBACDBConnection"].ConnectionString);

                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("RBAC DB");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(dbConnectBuilder.InitialCatalog + " on " + dbConnectBuilder.DataSource + " via user " + dbConnectBuilder.UserID);
            }
            catch { }

            try
            {
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(CustomerLogic.CustomerManager.Instance._DBConnectStr_ReinoComm);

                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("ReinoComm DB");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText(dbConnectBuilder.InitialCatalog + " on " + dbConnectBuilder.DataSource + " via user " + dbConnectBuilder.UserID);
            }
            catch { }

            try
            {
                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("PAM Connection");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                if (!string.IsNullOrEmpty(CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress))
                    tblData.SetInnerText(CustomerLogic.CustomerManager.Instance.PAMServerTCPAddress + ":" + CustomerLogic.CustomerManager.Instance.PAMServerTCPPort);
                else
                    tblData.SetInnerText(CustomerLogic.CustomerManager.Instance.PaymentXMLSource);
            }
            catch { }

            try
            {
                // Add data row and its cells
                tblRow = new BaseTagHelper("tr");
                table.Children.Add(tblRow);
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");
                tblData.SetInnerText("Loaded Customer Count");
                tblData = new BaseTagHelper("td");
                tblRow.Children.Add(tblData);
                tblData.AddCssClass("tdLeft");

                List<CustomerConfig> allDTOs = CustomerLogic.CustomerManager.GetAllCustomerDTOs();
                tblData.SetInnerText(allDTOs.Count.ToString());
            }
            catch { }

            // Get final string from the tag builder for our table
            result = table.ToString();
            return result;
        }
    }
}