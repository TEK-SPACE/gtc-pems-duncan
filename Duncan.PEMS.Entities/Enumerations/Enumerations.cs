using System;
using System.Reflection;

namespace Duncan.PEMS.Entities.Enumerations
{


    /// <summary>
    /// This enumerations reflects the data in table [CashBoxLocationType] table.
    /// </summary>
    public enum CashBoxLocationType
    {
        Meter = 1,
        Inventory = 2,
        RepairShop = 3
    }

    /// <summary>
    /// This enumerations reflects the data in table [AssetState] table.
    /// </summary>
    public enum AssetStateType
    {
        Undefined = 0,
        Current = 1,
        Pending = 2,
        Historic = 3
    }


    /// <summary>
    /// These reflect the hard-coded values of the three-part key
    /// AreaId value.  These were given by Duncan.  05/31/2013
    /// </summary>
    public enum AssetAreaId
    {
        Meter = 1,
        Sensor = 99,
        Gateway = 98,
        Cashbox = 97,
        Mechanism = 96,
        DataKey = 95,
        CSPark = 94
    };

    public enum AssetClass
    {
        Unknown,
        Meter,
        Sensor,
        Gateway,
        Cashbox,
        Smartcard,
        Space,
        Mechanism,
        DataKey
    };

    /// <summary>
    /// This enumerations reflects the data in table [TargetServiceDesignationMaster] table.
    /// </summary>
    public enum TargetServiceDesignations
    {
        OpenCompliant = 1,
        OpenNonCompliant = 2,
        OpenNonCompliant1Hour = 3,
        OpenNonCompliant2Hours = 4,
        ClosedCompliant = 5,
        ClosedNonCompliant = 6
    };


    /// <summary>
    /// This enumerations reflects the data in table [MeterGroup] table.
    /// </summary>
    public enum MeterGroups : int
    {
        SingleSpaceMeter = 0,
        MultiSpaceMeter = 1,
        Sensor = 10,
        Gateway = 13,
        Cashbox = 11,
        Smartcard = 12,
        Space = 20,
        Mechanism = 31,
        Datakey = 32
    };

    /// <summary>
    /// This enumerations reflects the data in table [VersionGroup] table.
    /// </summary>
    public enum VersionGroupType : int
    {
        MPV = 1,
        Software = 2,
        Hardware = 3,
        Firmware = 4
    };

    /// <summary>
    /// This enumerations reflects the data in table [OperationalStatus] table.
    /// </summary>
    public enum OperationalStatusType : int
    {
        Inactive = 0,
        Operational = 1,
        NonOpWithAlarm = 2,
        OpWithAlarm = 3,
        NonOpWithSpecialEvent = 4,
        NonOpWithWorkZone = 5,
        NonOpWithMaintenance = 6
    }

    public enum CollectionRouteResult
    {
        Success,
        ExistsPending,
        ExistsActive,
        ExistsCurrent,
        UnknownError
    };

    public enum CashBoxCoinDenomination
    {
        Unknown,
        Cents5Coins,
        Cents10Coins,
        Cents20Coins,
        Cents50Coins,
        Dollar1Coins,
        Dollar2Coins
    }

    public enum AssetListGridType
    {
        Summary,
        Configuration,
        Occupancy,
        FunctionalStatus
    }


    /// <summary>
    /// This enumerations reflects the data in table [ConfigStatus] table.
    /// </summary>
    public enum ConfigStatus : int
    {
        Active = 1,
        Inactive = 2
    }


    public enum ReportCategory : int
    {
        AssetStatus = 1,
        AlarmsEvents = 2,
        AssetMaintenance = 3,
        Financial = 4,
        CollectionRoutes = 5,
        Dashboard = 6,
        AdHocReports = 7,
        PEMSUSA = 8,
        Enforcement = 9,
        Others = 10,
        SpaceStatus = 11,
        Sensor = 12
    }


    /// <summary>
    /// This enumerations reflects the data in table [TariffState] table.
    /// </summary>
    public enum TariffStateType
    {
        New = 0,
        Current = 1,
        Pending = 2,
        Historic = 3
    }


    /// <summary>
    /// This enumerations reflects the data in table [AssetPendingReason] table.
    /// </summary>
    public enum AssetPendingReasonType
    {
        General = 0, 
	    ConfigOnly = 1,
        InfoOnly = 2, 
        ConfigAndInfo = 3
    }


    /// <summary>
    /// This enumerations reflects the data in table [SpaceType] table.
    /// </summary>
    public enum SpaceTypeType
    {
        /// <summary>
        /// Undefined is not an entry in the [SpaceType] table.
        /// </summary>
        Undefined = -1,
        SensorOnly = 0,
        MeteredSpace = 1,
        MeteredWithSensor = 2
    }

    /// <summary>
    /// Reflects data in the CustomerType table
    /// </summary>
    public enum CustomerProfileType
    {
        Admin = 0,
        Customer = 1,
        MaintenanceGroup = 2
    }

    /// <summary>
    /// Enumeration of supported report viewers (Izenda)
    /// </summary>
    public enum ReportViewerType
    {
        Undefined = 0,
        Iframes = 1,
        Internal = 2
    }

    #region Membership

    /// <summary>
    /// Membership event types
    /// Corresponds to database table MembershipEventTypes in RBAC
    /// </summary>
    public enum MembershipEvent
    {
        Login = 1,
        Logout = 2,
        ChangePassword = 3,
        LoginFailure = 4
    }

    #endregion

    #region Work Orders
    /// <summary>
    /// possible status values for work order events
    /// </summary>
    public enum WorkOrderEventStatus
    {
        Open = 0,
        Closed = 1
    }

    /// <summary>
    /// Status values for work orders
    /// </summary>
    public enum WorkOrderStatus
    {
        Open = 0,
        Closed = 1,
        Suspended = 2,
        Incomplete = 3,
        Rejected = 4
    }

    public enum AssignmentState
    {
        Assigned = 0,
        Unassigned = 1,
        Suspended = 3
    }


    /// <summary>
    /// Status values for available parts
    /// </summary>
    public enum AvailablePartStatus
    {
        InActive = 0,
        Active = 1
    }



    #endregion
}

