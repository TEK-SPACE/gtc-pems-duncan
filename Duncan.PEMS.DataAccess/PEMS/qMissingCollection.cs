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
    
    public partial class qMissingCollection
    {
        public int CustomerId { get; set; }
        public int AreaId { get; set; }
        public int MeterId { get; set; }
        public Nullable<System.DateTime> DateTimeRem { get; set; }
        public System.DateTime DateTimeRemHist { get; set; }
        public Nullable<double> AmtManual { get; set; }
        public Nullable<double> AmtAuto { get; set; }
        public float AmtHist { get; set; }
        public Nullable<int> CashboxSequenceNo { get; set; }
        public Nullable<int> xFileProcessId { get; set; }
        public Nullable<long> FileProcessId { get; set; }
        public string Remarks { get; set; }
    }
}
