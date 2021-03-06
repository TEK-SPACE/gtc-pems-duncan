//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Duncan.PEMS.DataAccess.PEMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class FDSummary
    {
        public long summaryid { get; set; }
        public short customerid { get; set; }
        public byte areaid { get; set; }
        public int meterid { get; set; }
        public long fileid { get; set; }
        public string fillename { get; set; }
        public long filesizebytes { get; set; }
        public System.DateTime fileadditiondate { get; set; }
        public string filetype { get; set; }
        public string filehash { get; set; }
        public Nullable<System.DateTime> firstchunkts { get; set; }
        public Nullable<System.DateTime> mostrecentfirstchunkts { get; set; }
        public Nullable<int> firstchunkcount { get; set; }
        public Nullable<System.DateTime> lastchunkts { get; set; }
        public string lastchunkoffset { get; set; }
        public Nullable<int> dfgcount { get; set; }
        public long jobid { get; set; }
        public System.DateTime submitteddate { get; set; }
        public System.DateTime availabledate { get; set; }
        public byte activejob { get; set; }
        public System.DateTime activationdate { get; set; }
        public string lastjobstatus { get; set; }
        public Nullable<System.DateTime> downloadedts { get; set; }
        public Nullable<System.DateTime> downloadFailTs { get; set; }
        public Nullable<System.DateTime> activatedts { get; set; }
        public Nullable<System.DateTime> activationfailurets { get; set; }
    }
}
