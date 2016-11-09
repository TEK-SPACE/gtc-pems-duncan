/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/22/2014       Sergey Ostrerov                 DPTXPEMS-45 - Can't create new customer; Replace text box to Drop Down Box for Area editing.
 * *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Customers
{
    public enum CustomerStatus
    {
        Inactive = -1,
        New = 0,
        Active = 1
    }

    public class CustomerModel
    {
        [Required]
        public string InternalName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        [Range(1, 65535)]
        public int Id { get; set; }

        [Required]
        public string ConnectionStringName { get; set; }
        public List<SelectListItem> ConnectionStrings { get; set; }

        [Required]
        public string ReportingConnectionStringName { get; set; }
        public List<SelectListItem> ReportingConnectionStrings { get; set; }
    }

    public class ListCustomerModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn == DateTime.MinValue ? string.Empty : CreatedOn.ToString("d"); } }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedOnDisplay { get { return UpdatedOn == DateTime.MinValue ? string.Empty : UpdatedOn.ToString("d"); } }
        public string PemsConnectionStringName { get; set; }
        public string MaintenanceConnectionStringName { get; set; }
        public string ReportingConnectionStringName { get; set; }
    }

    public class CustomerBaseModel
    {
        public CustomerStatusModel Status { get; set; }
        public int CustomerId { get; set; }
        public string DisplayName { get; set; }
        public bool Is24HrFormat { get; set; }
    }

    public class CustomerArea : IComparable<CustomerArea>
    {
        public int CustomerId { get; set; }
        public Int64 GlobalId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CompareTo(CustomerArea other)
        {
            return Name.CompareTo( other.Name );
        }
    }

    public class CustomerZone : IComparable<CustomerZone>
    {
        public int CustomerId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }

        public int CompareTo(CustomerZone other)
        {
            return Name.CompareTo(other.Name);
        }
    }


    public class CustomerCustomGroup : IComparable<CustomerCustomGroup>
    {
        public int CustomerId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CompareTo(CustomerCustomGroup other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public class CustomerDemandZone
    {        
        public int CustomerId { get; set; }
        public int DemandZoneId { get; set; }
        public string DemandZoneDesc { get; set; }
        public bool IsDisplay { get; set; }
        public int DemandZoneCustomerId { get; set; }

        public string DemandZoneCustomerName { get; set; }
        public string getResponse { get; set; }
    }

    public class RipnetProp : IComparable<RipnetProp>
    {
        public string Name { get; set; }
        public string KeyText { get; set; }
        public string ValueText { get; set; }
        public int CompareTo(RipnetProp other)
        {
            return Name.CompareTo(other.Name);
        }

    }

    public class CustPayByCell : IComparable<CustPayByCell>
    {

        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string Name { get; set; }
        public string KeyText { get; set; }
        public string ValueText { get; set; }
        public int CompareTo(CustPayByCell other)
        {
            return Name.CompareTo(other.Name);
        }
    }

    public class SelectedIds : IComparable<SelectedIds>
    {
        public int VendorID { get; set; }
        public string Name { get; set; }
        public int CompareTo(SelectedIds other)
        {
            return Name.CompareTo(other.Name);
        }
    }


}