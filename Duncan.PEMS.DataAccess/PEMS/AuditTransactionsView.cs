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
    
    public partial class AuditTransactionsView
    {
        public Nullable<long> id { get; set; }
        public Nullable<int> routeId { get; set; }
        public int cid { get; set; }
        public Nullable<int> AmountInCents { get; set; }
        public string collDateTime { get; set; }
    }
}