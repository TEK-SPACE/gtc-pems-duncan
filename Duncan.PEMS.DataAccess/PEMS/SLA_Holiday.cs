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
    
    public partial class SLA_Holiday
    {
        public long SLA_HolidayID { get; set; }
        public int CustomerId { get; set; }
        public System.DateTime HolidayDate { get; set; }
        public string HolidyName { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}