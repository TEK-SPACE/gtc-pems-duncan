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
    
    public partial class TariffRateConfiguration
    {
        public TariffRateConfiguration()
        {
            this.TariffRateForConfigurations = new HashSet<TariffRateForConfiguration>();
        }
    
        public long TariffRateConfigurationId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public int CustomerId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> ConfiguredOn { get; set; }
        public Nullable<int> ConfiguredBy { get; set; }
        public int State { get; set; }
        public Nullable<long> ConfigProfileId { get; set; }
    
        public virtual ConfigProfile ConfigProfile { get; set; }
        public virtual ConfigurationIDGen ConfigurationIDGen { get; set; }
        public virtual TariffState TariffState { get; set; }
        public virtual ICollection<TariffRateForConfiguration> TariffRateForConfigurations { get; set; }
    }
}
