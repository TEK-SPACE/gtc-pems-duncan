using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;



namespace Duncan.PEMS.SpaceStatus.UtilityClasses
{
    public class JsonPox : ActionFilterAttribute
    {
        private String[] _actionParams;

        // for deserialization
        public JsonPox(params String[] parameters)
        {
            this._actionParams = parameters;
        }

        // SERIALIZE
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Result is ViewResult)) return;

            // SETUP
            UTF8Encoding utf8 = new UTF8Encoding(false);
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            String contentType = request.ContentType ?? string.Empty;
            ViewResult view = (ViewResult)(filterContext.Result);
            var data = view.ViewData.Model;

            // JSON
            if (contentType.Contains("application/json") || request.IsAjaxRequest())
            {
                using (var stream = new MemoryStream())
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();

                    String content = js.Serialize(data);
                    filterContext.Result = new ContentResult
                    {
                        ContentType = "application/json",
                        Content = content,
                        ContentEncoding = utf8
                    };
                }

            }

            // POX
            else if (/*(string.IsNullOrEmpty(contentType)) ||*/ (contentType.Contains("text/xml")))
            {
                // Let's use XML serializer instead of DataContractSerializer
                string objectAsXml = JsonXmlXsdOrHtml_BasedOnQueryString.SerializeObjectAsXML(data, null);
                filterContext.Result = new ContentResult
                {
                    ContentType = "text/xml",
                    Content = objectAsXml,
                    ContentEncoding = utf8
                };

                /*
                // MemoryStream to encapsulate as UTF-8 (default UTF-16)
                // http://stackoverflow.com/questions/427725/
                //
                // MemoryStream also used for atomicity but not here
                // http://stackoverflow.com/questions/486843/
                using (MemoryStream stream = new MemoryStream(500))
                {
                    using (var xmlWriter =
                    XmlTextWriter.Create(stream,
                    new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true,
                        Encoding = utf8,
                        Indent = true
                    }))
                    {

                        new DataContractSerializer(
                        data.GetType(),
                        null, // knownTypes
                        65536, // maxItemsInObjectGraph
                        false, // ignoreExtensionDataObject
                        true, // preserveObjectReference - overcomes cyclical reference issues
                        null // dataContractSurrogate
                        ).WriteObject(stream, data);
                    }

                    filterContext.Result = new ContentResult
                    {
                        ContentType = "text/xml",
                        Content = utf8.GetString(stream.ToArray()),
                        ContentEncoding = utf8
                    };
                }
                */
            }
        }

        // DESERIALIZE
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (_actionParams == null || _actionParams.Length == 0) return;

            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            String contentType = request.ContentType ?? string.Empty;
            Boolean isJson = contentType.Contains("application/json");

            if (!isJson) return;
            //@@todo Deserialize POX

            // JavascriptSerialier expects a single type to deserialize
            // so if the response contains multiple disparate objects to deserialize
            // we dynamically build a new wrapper class with fields representing those
            // object types, deserialize and then unwrap
            ParameterDescriptor[] paramDescriptors =
            filterContext.ActionDescriptor.GetParameters();
            Boolean complexType = paramDescriptors.Length > 1;

            Type wrapperClass;
            if (complexType)
            {
                Dictionary<String, Type> parameterInfo = new Dictionary<string, Type>();
                foreach (ParameterDescriptor p in paramDescriptors)
                {
                    parameterInfo.Add(p.ParameterName, p.ParameterType);
                }
                wrapperClass = BuildWrapperClass(parameterInfo);
            }
            else
            {
                wrapperClass = paramDescriptors[0].ParameterType;
            }

            String json;
            using (var sr = new StreamReader(request.InputStream))
            {
                json = sr.ReadToEnd();
            }

            // then deserialize json as instance of dynamically created wrapper class
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var result = typeof(JavaScriptSerializer)
            .GetMethod("Deserialize")
            .MakeGenericMethod(wrapperClass)
            .Invoke(serializer, new object[] { json });

            // then get fields from wrapper class assign the values back to the action params
            if (complexType)
            {
                for (Int32 i = 0; i < paramDescriptors.Length; i++)
                {
                    ParameterDescriptor pd = paramDescriptors[i];
                    filterContext.ActionParameters[pd.ParameterName] =
                    wrapperClass.GetField(pd.ParameterName).GetValue(result);

                }
            }
            else
            {
                ParameterDescriptor pd = paramDescriptors[0];
                filterContext.ActionParameters[pd.ParameterName] = result;
            }
        }

        private Type BuildWrapperClass(Dictionary<string, Type> parameterInfo)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "DynamicAssembly";
            AppDomain appDomain = AppDomain.CurrentDomain;
            AssemblyBuilder assemblyBuilder =
            appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder =
            assemblyBuilder.DefineDynamicModule("DynamicModule");
            TypeBuilder typeBuilder =
            moduleBuilder.DefineType("DynamicClass",
            TypeAttributes.Public | TypeAttributes.Class);

            foreach (KeyValuePair<String, Type> entry in parameterInfo)
            {
                String paramName = entry.Key;
                Type paramType = entry.Value;
                FieldBuilder field = typeBuilder.DefineField(paramName,
                paramType, FieldAttributes.Public);
            }

            Type generatedType = typeBuilder.CreateType();
            // object generatedObject = Activator.CreateInstance(generatedType);

            return generatedType;
        }
    }

    public class JsonXmlXsdOrHtml_BasedOnQueryString : ActionFilterAttribute
    {
        private String[] _actionParams;

        // for deserialization
        public JsonXmlXsdOrHtml_BasedOnQueryString(params String[] parameters)
        {
            this._actionParams = parameters;
        }

        // SERIALIZE
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Can't do anything if the result hasn't been set as a ViewResult
            if (!(filterContext.Result is ViewResult)) return;

            // Don't change anything if an error code has been set
            try
            {
                if (filterContext.HttpContext.Response.StatusCode >= 300) return;
            }
            catch { }

            // SETUP
            UTF8Encoding utf8 = new UTF8Encoding(false);
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            String contentType = request.ContentType ?? string.Empty;
            ViewResult view = (ViewResult)(filterContext.Result);
            var data = view.ViewData.Model;
            

            // Did caller request the XSD?
            if (request.QueryString.ToString().IndexOf("XSD", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                // Generate an XSD document from the class
                string finalXSD = string.Empty;
                var soapReflectionImporter = new SoapReflectionImporter();
                var xmlTypeMapping = soapReflectionImporter.ImportTypeMapping(data.GetType());
                var xmlSchemas = new XmlSchemas();
                var xmlSchema = new XmlSchema();
                xmlSchemas.Add(xmlSchema);
                var xmlSchemaExporter = new XmlSchemaExporter(xmlSchemas);
                xmlSchemaExporter.ExportTypeMapping(xmlTypeMapping);
                MemoryStream ms = new MemoryStream();
                xmlSchema.Write(ms);
                ms.Position = 0;
                ms.Capacity = Convert.ToInt32(ms.Length);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(ms);
                finalXSD = xmldoc.InnerXml;
                ms.Close();
                ms.Dispose();

                filterContext.Result = new ContentResult
                {
                    ContentType = "text/xml",
                    Content = finalXSD,
                    ContentEncoding = System.Text.Encoding.UTF8
                };
            }
            // Did caller request XML result?
            else if (request.QueryString.ToString().IndexOf("XML", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                string objectAsXml = SerializeObjectAsXML(data, null);
                filterContext.Result = new ContentResult
                {
                    ContentType = "text/xml",
                    Content = objectAsXml,
                    ContentEncoding = System.Text.Encoding.UTF8
                };
            }
            // Did caller request JSON result?
            else if (request.QueryString.ToString().IndexOf("JSON", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                using (var stream = new MemoryStream())
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();

                    String content = js.Serialize(data);
                    filterContext.Result = new ContentResult
                    {
                        ContentType = "application/json",
                        Content = content,
                        ContentEncoding = System.Text.Encoding.UTF8
                    };
                }
            }
            // Did caller request HTML result?
            else if (request.QueryString.ToString().IndexOf("HTML", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                filterContext.Result = filterContext.Result; // Nothing to modify here
            }
            // Not specified, so we will default response to XML
            else
            {
                string objectAsXml = SerializeObjectAsXML(data, null);
                filterContext.Result = new ContentResult
                {
                    ContentType = "text/xml",
                    Content = objectAsXml,
                    ContentEncoding = System.Text.Encoding.UTF8
                };
            }
        }

        public static string SerializeObjectAsXML(object objectToSerialize, string forcedNameSpace)
        {
            XmlSerializer serializer = null;
            if (!string.IsNullOrEmpty(forcedNameSpace))
                serializer = new XmlSerializer(objectToSerialize.GetType(), forcedNameSpace);
            else
                serializer = new XmlSerializer(objectToSerialize.GetType());

            XmlWriterSettings loXMLSettings = new XmlWriterSettings();
            loXMLSettings.Indent = true;
            loXMLSettings.Encoding = System.Text.UTF8Encoding.UTF8;
            loXMLSettings.OmitXmlDeclaration = true;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlWriter xtWriter = XmlWriter.Create(stream, loXMLSettings);

            if (string.IsNullOrEmpty(forcedNameSpace))
            {
                // Create our own namespaces for the output, which effectively omits all namespaces
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(xtWriter, objectToSerialize, ns);
            }
            else
            {
                serializer.Serialize(xtWriter, objectToSerialize);
            }
            xtWriter.Flush();
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            // Now load the serialized XML into an XMLDocument for some post-processing
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(stream);

            // Let's remove the unwanted "xsd" and "xsi" namespaces from attributes of first child node
            if (xmlDoc.FirstChild.Attributes != null)
            {
                for (int loIdx = xmlDoc.FirstChild.Attributes.Count - 1; loIdx >= 0; loIdx--)
                {
                    XmlAttribute nextAttr = xmlDoc.FirstChild.Attributes[loIdx];
                    if ((nextAttr.Name == "xmlns:xsd") || (nextAttr.Name == "xmlns:xsi"))
                    {
                        xmlDoc.FirstChild.Attributes.Remove(nextAttr);
                    }
                }
            }

            // Capture the outer XML of document as the result 
            string ObjAsString = xmlDoc.OuterXml;

            // Cleanup resources
            xtWriter.Close();
            stream.Close();
            stream.Dispose();

            // Return final result, which is a string representing the serialized XML of object
            return ObjAsString;
        }

    }

}