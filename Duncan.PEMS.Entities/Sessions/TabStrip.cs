namespace Duncan.PEMS.Entities.Sessions
{
    public class TabStrip
    {
        public string SelectedTab { get; set; }
    }


    public class WorkOrderFilters : FilterBase
    {
        public string CreationDateFrom { get; set; }
        public string CreationDateTo { get; set; }
        public string DeadlineFrom { get; set; }
        public string DeadlineTo { get; set; }
        public string ClosedDateFrom { get; set; }
        public string ClosedDateTo { get; set; }
        public string WorkOrderId { get; set; }
        public string WorkOrderState { get; set; }
        public string AssetType { get; set; }
        public string AssetId { get; set; }
        public string AlarmCode { get; set; }
        public string Priority { get; set; }
        public string AssignmentState { get; set; }
        public string Technician { get; set; }
        public string LocationType { get; set; }
        public string Location { get; set; }
        public string LocationLabel { get; set; }
        public string Zone { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
    }


    public class CollectionFilters : FilterBase
    {
        public string Status { get; set; }
        public string ConfigurationId { get; set; }
        public string RouteName { get; set; }
        public string MeterId { get; set; }
        public string MeterName { get; set; }
        public string ActivatedStart { get; set; }
        public string ActivatedEnd { get; set; }
        public string CreatedStart { get; set; }
        public string CreatedEnd { get; set; }
    }

    public class AggregationFilters : FilterBase
    {
        public string ConfigurationId { get; set; }
        public string RouteName { get; set; }
        public string MeterId { get; set; }
        public string MeterName { get; set; }
        public string DateTimeStart { get; set; }
        public string DateTimeEnd { get; set; }
        public string Variance { get; set; }
        public string LocationType { get; set; }
        public string Location { get; set; }
        public string LocationLabel { get; set; }
        public string Zone { get; set; }
        public string Suburb { get; set; }
        public string Area { get; set; }
    }

    public class CustomerFilters : FilterBase
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class MaintenanceGroupFilters : FilterBase
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }

    public class AlarmFilters : FilterBase
    {
        public string AssetType { get; set; }
        public string AlarmStatus { get; set; }
        public string AlarmCode { get; set; }
        public string TargetService { get; set; }
        public string AlarmId { get; set; }
        public string DateRangeFrom { get; set; }
        public string DateRangeTo { get; set; }
        public string AssetId { get; set; }
        public string AssetState { get; set; }
        public string AlarmSource { get; set; }
        public string TimeType { get; set; }
        public string AreaId { get; set; }
        public string AssetName { get; set; }
        public string AlarmSeverity { get; set; }
        public string TechnicianId { get; set; }
        public string LocationType { get; set; }
        public string Location { get; set; }
        public string LocationLabel { get; set; }
    }

    public class TariffFilters : FilterBase
    {
        public string CreateDateStart { get; set; }
        public string CreateDateEnd { get; set; }
        public string TariffConfigId { get; set; }
    }

    public class AssetHistoryFilters : FilterBase
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string UserName { get; set; }
        public string ReasonForChangeId { get; set; }
    }

    public class DiscountSummaryFilters : FilterBase
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string AccountStatus { get; set; }
        public string DiscountScheme { get; set; }
        public string SchemeStatus { get; set; }
        public string CreateDateStart { get; set; }
        public string CreateDateEnd { get; set; }
    }

    public class RoleFilters : FilterBase
    {
       // public string CustomerId { get; set; }
        public string RoleName { get; set; }
    }

    public class UserFilters : FilterBase
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string RoleName { get; set; }
    }

    public class VCEFilters : FilterBase
    {
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public string CollectionRunName { get; set; }
    }

    public class SortValue
    {
        public string field { get; set; }
        public string dir { get; set; }
    }

    /// <summary>
    ///     Base class for filters - common items among all grids
    /// </summary>
    public abstract class FilterBase
    {
        public int SelectedIndex { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}