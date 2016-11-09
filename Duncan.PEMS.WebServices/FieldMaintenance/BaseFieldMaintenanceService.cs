
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Duncan.PEMS.WebServices.ServiceManagers;

namespace Duncan.PEMS.WebServices.FieldMaintenance
{
   public abstract class BaseFieldMaintenanceService
    {
        #region Utility Functions
        /// <summary>
        /// Method - Writes into the Response stream
        /// </summary>
        /// <param name="strMessage"></param>
       protected static void WriteResponse(string strMessage)
        {
            HttpContext.Current.Response.Write(strMessage);
        }

        /// <summary>
        /// Method - Deserialize Class XML
        /// </summary>
        /// <param name="xmlByteData"></param>
        /// <returns></returns>
        protected Data Deserialize(byte[] xmlByteData)
        {
            try
            {
                var ds = new XmlSerializer(typeof(Data));
                var memoryStream = new MemoryStream(xmlByteData);
                 var data = (Data)ds.Deserialize(memoryStream);
                return data;
            }
            catch (Exception ex)
            {
                var errorData = ErrorHandler.GenerateError(ex, "500");
                return errorData;
            }
        }

        /// <summary>
        /// Method - Serialize Class to XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected String Serialize(Data data)
        {
            try
            {
                String xmlizedString = null;
                var xs = new XmlSerializer(typeof(Data));
                //create an instance of the MemoryStream class since we intend to keep the XML string 
                //in memory instead of saving it to a file.
                var memoryStream = new MemoryStream();
                //XmlTextWriter - fast, non-cached, forward-only way of generating streams or files 
                //containing XML data
                var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //Serialize emp in the xmlTextWriter
                xs.Serialize(xmlTextWriter, data);
                //Get the BaseStream of the xmlTextWriter in the Memory Stream
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                //Convert to array
                xmlizedString = Utf8ByteArrayToString(memoryStream.ToArray());
                return xmlizedString;
            }
            catch (Exception ex)
            {
                var errorData = ErrorHandler.GenerateError(ex, "500");
                WriteResponse(Serialize(errorData));
            }
            //update this to return a hard coded data element, since serialization might break here.
            return string.Empty;
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private String Utf8ByteArrayToString(Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            if (constructedString.StartsWith(_byteOrderMarkUtf8))
                constructedString = constructedString.Remove(0, _byteOrderMarkUtf8.Length);

            return (constructedString);
        }
        private readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        #endregion
    }
}
