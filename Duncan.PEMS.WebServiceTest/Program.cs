using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Duncan.PEMS.Business.WebServices;

namespace Duncan.PEMS.WebServiceTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Posting to Duncan
          // TestCreateAlarm();
            //TestCloseAlarm();
           
            //Posting to Unidev
          //  TestClearAlarms();
           TestGetWorkOrders();
            Console.ReadLine();
        }

        #region Duncan Web Services 
       
        #region Create Alarm
        private static void TestCreateAlarm()
        {
            Console.WriteLine("Test Work Orders");
            const string strUrl = "http://115.119.37.66:8080/ripnet/RClAl/createalarm.rmws";
            // The client will be oblivious to server side data type
            // Create the xml document in a memory stream - Recommended       
            byte[] dataByte = GenerateTestXml("CreateAlarmTestFile");
            PostRequest(strUrl, dataByte);
        }
        #endregion

        #region Close Alarm
        private static void TestCloseAlarm()
        {
            Console.WriteLine("Test Work Orders");
            const string strUrl = "http://115.119.37.66:8080/ripnet/RClAl/closealarm.rmws";
            // The client will be oblivious to server side data type
            // Create the xml document in a memory stream - Recommended       
            byte[] dataByte = GenerateTestXml("CloseAlarmTestFile");
            PostRequest(strUrl, dataByte);
        }

        #endregion
        #endregion

        #region Unidev Web Services
        #region Clear Alarms
        private static void TestClearAlarms()
        {
            Console.WriteLine("Test Clear Alarms");
            //you need to set iss up and point it to the web services project folder.
            const string strUrl = "http://duncanwebservices/ClearAlarms";
            // The client will be oblivious to server side data type
            // Create the xml document in a memory stream - Recommended       
            byte[] dataByte = GenerateTestXml("ClearAlarmsTestFile");
            //PostRequest(strUrl, dataByte);

            Data data = Deserialize(dataByte);

            //now that we have our request, we need to pass it to the web wervice factory to modify it. 
            //It will return a Data itht he correct return types, this will perform no busienss logic.
            var response = (new WebServiceFactory()).ClearAlarms(data);
            var value = Serialize(response);
            Console.Write(value);
            Console.ReadLine();
        }
        #endregion

        #region Get Work Orders
        private static void TestGetWorkOrders()
        {
            Console.WriteLine("Test Work Orders");
            //you need to set iss up and point it to the web services project folder.
            const string strUrl = "http://duncanwebservices/GetWorkOrders";
            // The client will be oblivious to server side data type
            // Create the xml document in a memory stream - Recommended     
            byte[] dataByte = GenerateTestXml("GetWorkOrdersTestFile");


            Data data = Deserialize(dataByte);

            //now that we have our request, we need to pass it to the web wervice factory to modify it. 
            //It will return a Data itht he correct return types, this will perform no busienss logic.
            var response = (new WebServiceFactory()).GetWorkOrders(data);
            var value = Serialize(response);
            Console.Write(value);
            Console.ReadLine();

            //  PostRequest(strUrl, dataByte);
        }
        #endregion

        #endregion

        #region Helpers

        /// <summary>
        /// Generate a data XML stream of bytes
        /// </summary>
        /// <returns>Data XML in bytes</returns>
        private static byte[] GenerateTestXml(string fileName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load( "SampleFiles\\" + fileName + ".xml");
            return Encoding.UTF8.GetBytes(xmlDoc.OuterXml);
        }


        private static void PostRequest(string strUrl, byte[] dataByte)
        {
            var postRequest = (HttpWebRequest)WebRequest.Create(strUrl);
            //Method type
            postRequest.Method = "POST";
            // Data type - message body coming in xml
            postRequest.ContentType = "text/xml";
            postRequest.KeepAlive = false;
            //Content length of message body
            postRequest.ContentLength = dataByte.Length;

            // Get the request stream
            Stream posTstream = postRequest.GetRequestStream();
            // Write the data bytes in the request stream
            posTstream.Write(dataByte, 0, dataByte.Length);

            //Get response from server
            var postResponse = (HttpWebResponse)postRequest.GetResponse();
            var reader = new StreamReader(postResponse.GetResponseStream(), Encoding.UTF8);
            Console.WriteLine("Response");
            var value = reader.ReadToEnd();
            Console.WriteLine(value);
        }


        /// <summary>
        /// Method - Deserialize Class XML
        /// </summary>
        /// <param name="xmlByteData"></param>
        /// <returns></returns>
        protected static Data Deserialize(byte[] xmlByteData)
        {
            try
            {
                var ds = new XmlSerializer(typeof (Data));
                var memoryStream = new MemoryStream(xmlByteData);
                var data = (Data) ds.Deserialize(memoryStream);
                return data;
            }
            catch (Exception ex)
            {

                //build the error and send it back
                //build the data object
                //build the error
                //serialize the error and send it back
                return null;
            }
        }

        /// <summary>
        /// Method - Serialize Class to XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected static String Serialize(Data data)
        {
            try
            {
                String xmlizedString = null;
                var xs = new XmlSerializer(typeof (Data));
                //create an instance of the MemoryStream class since we intend to keep the XML string 
                //in memory instead of saving it to a file.
                var memoryStream = new MemoryStream();
                //XmlTextWriter - fast, non-cached, forward-only way of generating streams or files 
                //containing XML data
                var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //Serialize emp in the xmlTextWriter
                xs.Serialize(xmlTextWriter, data);
                //Get the BaseStream of the xmlTextWriter in the Memory Stream
                memoryStream = (MemoryStream) xmlTextWriter.BaseStream;
                //Convert to array
                xmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return xmlizedString;
            }
            catch (Exception ex)
            {
                //build the error and send it back
                //build the data object
                //build the error
                //serialize the error and send it back
                return null;
            }
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }


        private static byte[] StringToByteArray(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        #endregion
    }
}