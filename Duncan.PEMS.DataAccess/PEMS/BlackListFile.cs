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
    
    public partial class BlackListFile
    {
        public int BlackListFilesID { get; set; }
        public string FileName { get; set; }
        public System.DateTime DateTimeGenerated { get; set; }
        public int CustomerID { get; set; }
        public Nullable<int> BlackListFormatVersion { get; set; }
        public string BlackListMAC { get; set; }
        public Nullable<int> BlackListChecksum { get; set; }
    
        public virtual Customer Customer { get; set; }
    }
}
