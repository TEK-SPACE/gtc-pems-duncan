using System;
using System.Collections.Generic;
using Duncan.PEMS.Entities.General;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Duncan.PEMS.Entities.Enforcement
{
    /// <summary>
    /// 
    /// </summary>
    public class EnforcementModel
    {
        /// <summary>
        /// Vehicle Number
        /// </summary>
        public string VehLicNo { get; set; }

        /// <summary>
        /// Vehicle State
        /// </summary>
        public string VehLicState { get; set; }

        /// <summary>
        /// License Type
        /// </summary>
        public string LicenseType { get; set; }

        /// <summary>
        /// VIN
        /// </summary>
        public string VehVIN { get; set; }


        /// <summary>
        /// Issue Number Prefix
        /// </summary>
        public string IssueNoPfx { get; set; }

        /// <summary>
        /// Agency
        /// </summary>
        public string Agency { get; set; }

        /// <summary>
        /// Issue Number
        /// </summary>
        public long IssueNo { get; set; }
        public long myCitationID { get; set; } //** Sairam added on Nov 4th 2014



        public string IssueNo_final { get; set; }

        /// <summary>
        /// Beat
        /// </summary>
        public string Beat { get; set; }

        /// <summary>
        /// Issue Number Suffix
        /// </summary>
        public string IssueNoSfx { get; set; }

        /// <summary>
        /// Meter Id
        /// </summary>
        public string MeterID { get; set; }

        /// <summary>
        /// Officer id
        /// </summary>


        public DateTime? IssueDateTime { get; set; }

        [OriginalGridPosition(Position = 0)]
        public string IssueDateTime_final { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string IssueNoWithPfxSfx { get; set; }

        [OriginalGridPosition(Position = 2)]
        public string OfficerName { get; set; }

        [OriginalGridPosition(Position = 3)]
        public int OfficerID { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string Status { get; set; }

        public string IssueDateTime_Only { get; set; }

        ///// <summary>
        ///// Status
        ///// </summary>
        //public string Status { get; set; }

        ///// <summary>
        ///// Type
        ///// </summary>
        public string Type { get; set; }

        public string Class { get; set; }
        public string Code { get; set; }

        public string ViolationDescription { get; set; }

        // Properties for Enforcement Details
        public string UnitSerial { get; set; }
        public DateTime? VehLicExpDate { get; set; }
        public string VEHLICTYPE { get; set; }

        public bool isAutoBindReqd { get; set; }
        public string VehMake { get; set; }
        public string VehModel { get; set; }
        public string VehBodyStyle { get; set; }
        public string VehVIN4 { get; set; }
        public string VEHCHECKDIGIT { get; set; }
        public string VehColor1 { get; set; }
        public string VehColor2 { get; set; }
        public string PermitNo { get; set; }
        public string LocBlock { get; set; }
        public string LocStreet { get; set; }
        public string LocDescriptor { get; set; }
        public string LOCSIDEOFSTREET { get; set; }
        public string LocSuburb { get; set; }
        public string LocLot { get; set; }
        public string LocCrossStreet1 { get; set; }
        public string LocCrossStreet2 { get; set; }
        public string LocCity { get; set; }
        public string LocState { get; set; }
        public string LOCZIP { get; set; }
        public string MeterNo { get; set; }
        public string MeterBayNo { get; set; }
        public string LocDirection { get; set; }
        public string Remark1 { get; set; }
        public string Remark2 { get; set; }
        public string Remark3 { get; set; }
        public string VioCode { get; set; }
        public string VioDescription1 { get; set; }
        //public float? VioLateFee1 { get; set; }
        //public float? VioLateFee2 { get; set; }
        //public float? VioFine { get; set; }
        public string VioFine { get; set; }
        public string VioLateFee1 { get; set; }
        public string VioLateFee2 { get; set; }
        public DateTime? StatusDateTime { get; set; }
        public string StatusValue { get; set; }
        public string StatusReason { get; set; }
        public DateTime? ExportDateTime { get; set; }
        public int ExportId { get; set; }
        public int? ExportType { get; set; }
        public int ParkingId { get; set; }
        public int? Count { get; set; }

        //***********************************************

    }

    /// <summary>
    /// 
    /// </summary>
    public class NoteModel
    {
        public int ParkNoteId { get; set; }

        public int ParkingId { get; set; }

        public DateTime NoteDate { get; set; }
        public DateTime NoteTime { get; set; }

        public DateTime NoteDateTime { get; set; }

        public int OfficerID { get; set; }

        public string NotesMemo { get; set; }

        public string MultimediaNoteDataType { get; set; }

        public byte[] MultimediaNoteData { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

    }
}