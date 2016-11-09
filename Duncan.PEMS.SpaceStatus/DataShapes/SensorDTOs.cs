using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Duncan.PEMS.SpaceStatus.DataShapes
{
    // We will use [XmlElement] attributes to trigger our DataMapper source code generator
    // to know when the DB column name differs from the property name.

    // Also note that we can use the [XmlIgnore] attribute if we don't want to attempt
    // populating a property from a database field

    public class AssetListingInformation
    {
        [XmlElement("MeterID")]
        public int MeterID { get; set; }

        [XmlElement("BayNumber")]
        public int BayNumber { get; set; }

        [XmlElement("AreaID")]
        public int AreaID { get; set; }

        [XmlElement("Location")]
        public string Location { get; set; }

        [XmlElement("Latitude")]
        public double Latitude { get; set; }

        [XmlElement("Longitude")]
        public double Longitude { get; set; }

        public AssetListingInformation()
        {
        }
    }

    public class HistoricalSensingRecord
    {
        [XmlElement("LastUpdatedTS")]
        public DateTime DateTime { get; set; }

        [XmlElement("RecCreationDate")]
        public DateTime RecCreationDateTime { get; set; }

        [XmlElement("LastStatus")]
        public bool IsOccupied { get; set; }

        [XmlElement("BayNumber")]
        public int BayId { get; set; }

        [XmlElement("MeterId")]
        public int MeterMappingId { get; set; }

        public HistoricalSensingRecord()
        {
        }
    }

    public class CurrentSpaceOccupancyInformation
    {
        [XmlElement("LastUpdatedTS")]
        public DateTime LastInOut { get; set; }

        [XmlElement("LastStatus")]
        public bool IsOccupied { get; set; }

        [XmlElement("CustomerId")]
        public int CustomerID { get; set; }

        [XmlElement("MeterId")]
        public int MeterID { get; set; }

        [XmlElement("BayNumber")]
        public int BayID { get; set; }

        [XmlElement("HasSensor")]
        public bool HasSensor { get; set; }

        public CurrentSpaceOccupancyInformation()
        {
        }
    }

    public class SensorEventAndCommsRecord
    {
        [XmlElement("MeterId")]
        public int MeterID { get; set; }

        [XmlElement("BayNumber")]
        public int BayID { get; set; }

        [XmlElement("AreaId")]
        public int AreaID { get; set; }
        
        public string MeterName { get; set; }
        public bool HasSensor { get; set; }
        public int MaxBaysEnabled { get; set; }
        public int MeterGroup { get; set; }
        public int MeterType { get; set; }

        /// <summary>
        /// When reporting on historical data, this is not necessarily of the latest status!
        /// </summary>
        public DateTime LastSensorStatusTime { get; set; }

        /// <summary>
        /// When reporting on historical data, this is not necessarily of the latest status!
        /// </summary>
        public int LastSensorStatus { get; set; }

        /// <summary>
        /// Not applicable when doing a historical report -- only valid when reporting on current status
        /// </summary>
        public DateTime LastCommunication { get; set; }

        /// <summary>
        /// Not applicable when doing a historical report -- only valid when reporting on current status
        /// </summary>
        [XmlElement("LastCommunicationMessage")]
        public string LastCommunicationType { get; set; }

        [XmlIgnore]
        public TimeSpan CalculatedDuration { get; set; }

        public SensorEventAndCommsRecord()
        {
            LastCommunication = DateTime.MinValue;
            LastSensorStatusTime = DateTime.MinValue;
            CalculatedDuration = new TimeSpan();
        }
    }

    public class SensorHeartbeatRecord
    {
        [XmlElement("MeterId")]
        public int MeterID { get; set; }

        [XmlElement("BayNumber")]
        public int BayID { get; set; }

        [XmlElement("AreaId")]
        public int AreaID { get; set; }

        public string MeterName { get; set; }
        public bool HasSensor { get; set; }
        public int MaxBaysEnabled { get; set; }
        public int MeterGroup { get; set; }
        public int MeterType { get; set; }

        public DateTime LastCommunication { get; set; }

        [XmlElement("LastCommunicationMessage")]
        public string LastCommunicationType { get; set; }

        [XmlIgnore]
        public TimeSpan CalculatedDuration { get; set; }

        public SensorHeartbeatRecord()
        {
            LastCommunication = DateTime.MinValue;
            CalculatedDuration = new TimeSpan();
        }
    }

    public class SensorBatteryDiagnostics
    {
        [XmlElement("MeterId")]
        public int MeterID { get; set; }

        [XmlElement("BayNumber")]
        public int BayID { get; set; }

        [XmlElement("AreaId")]
        public int AreaID { get; set; }

        public string MeterName { get; set; }

        public bool HasSensor { get; set; }
        public int MaxBaysEnabled { get; set; }
        public int MeterGroup { get; set; }
        public int MeterType { get; set; }

        public string DryBattCurrV { get; set; }
        public string RechargeBattCurrV { get; set; }
        public string DryBattMinV { get; set; }
        public string RechargeBattMinV { get; set; }

        public DateTime TimestampOfLatestBatteryVoltageReport { get; set; }

        public Single GetNumericValue_DryBattCurrV()
        {
            Single result = 0.0f;
            if (string.IsNullOrEmpty(this.DryBattCurrV))
                return result;

            string numStr = "";
            foreach (char nextChar in this.DryBattCurrV)
            {
                if (((nextChar >= '0') && (nextChar <= '9')) || (nextChar == '.'))
                    numStr += nextChar;
            }
            Single.TryParse(numStr, out result);
            return result;
        }

        public Single GetNumericValue_RechargeBattCurrV()
        {
            Single result = 0.0f;
            if (string.IsNullOrEmpty(this.RechargeBattCurrV))
                return result;

            string numStr = "";
            foreach (char nextChar in this.RechargeBattCurrV)
            {
                if (((nextChar >= '0') && (nextChar <= '9')) || (nextChar == '.'))
                    numStr += nextChar;
            }
            Single.TryParse(numStr, out result);
            return result;
        }

        public Single GetNumericValue_DryBattMinV()
        {
            Single result = 0.0f;
            if (string.IsNullOrEmpty(this.DryBattMinV))
                return result;

            string numStr = "";
            foreach (char nextChar in this.DryBattMinV)
            {
                if (((nextChar >= '0') && (nextChar <= '9')) || (nextChar == '.'))
                    numStr += nextChar;
            }
            Single.TryParse(numStr, out result);
            return result;
        }

        public Single GetNumericValue_RechargeBattMinV()
        {
            Single result = 0.0f;
            if (string.IsNullOrEmpty(this.RechargeBattMinV))
                return result;

            string numStr = "";
            foreach (char nextChar in this.RechargeBattMinV)
            {
                if (((nextChar >= '0') && (nextChar <= '9')) || (nextChar == '.'))
                    numStr += nextChar;
            }
            Single.TryParse(numStr, out result);
            return result;
        }

        public SensorBatteryDiagnostics()
        {
            TimestampOfLatestBatteryVoltageReport = new DateTime();
        }
    }

}