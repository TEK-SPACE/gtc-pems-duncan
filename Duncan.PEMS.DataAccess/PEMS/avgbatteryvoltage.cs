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
    
    public partial class avgbatteryvoltage
    {
        public int customerid { get; set; }
        public int areaid { get; set; }
        public int AssetId { get; set; }
        public System.DateTime Time_of_Last_Status_Report { get; set; }
        public Nullable<double> avgbatt { get; set; }
        public string AssetType { get; set; }
        public string AssetName { get; set; }
        public Nullable<int> ZoneId { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string Area { get; set; }
        public string Zone { get; set; }
        public string DemandArea { get; set; }
        public Nullable<long> sl1 { get; set; }
    }
}