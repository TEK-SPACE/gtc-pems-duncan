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
    
    public partial class TransactionPackageStatu
    {
        public TransactionPackageStatu()
        {
            this.TransactionPackages = new HashSet<TransactionPackage>();
        }
    
        public int StatusId { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<TransactionPackage> TransactionPackages { get; set; }
    }
}