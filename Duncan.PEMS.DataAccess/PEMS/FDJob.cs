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
    
    public partial class FDJob
    {
        public FDJob()
        {
            this.FDJobHistories = new HashSet<FDJobHistory>();
            this.RateTransmissionJobs = new HashSet<RateTransmissionJob>();
        }
    
        public long JobID { get; set; }
        public long FileID { get; set; }
        public int CustomerID { get; set; }
        public int AreaID { get; set; }
        public int MeterId { get; set; }
        public System.DateTime SubmittedDate { get; set; }
        public System.DateTime AvailableDate { get; set; }
        public int ActiveJob { get; set; }
        public System.DateTime ActivationDate { get; set; }
        public int JobStatus { get; set; }
    
        public virtual ICollection<FDJobHistory> FDJobHistories { get; set; }
        public virtual FDJobStatu FDJobStatu { get; set; }
        public virtual Meter Meter { get; set; }
        public virtual ICollection<RateTransmissionJob> RateTransmissionJobs { get; set; }
        public virtual FDFile FDFile { get; set; }
    }
}