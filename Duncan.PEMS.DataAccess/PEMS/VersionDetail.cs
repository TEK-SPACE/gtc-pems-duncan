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
    
    public partial class VersionDetail
    {
        public Nullable<double> VersionID { get; set; }
        public string ObjectName { get; set; }
        public string objectType { get; set; }
    
        public virtual VersionMaster VersionMaster { get; set; }
    }
}
