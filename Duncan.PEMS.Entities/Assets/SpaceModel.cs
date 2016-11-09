
/******************* CHANGE LOG ***********************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 01/31/2014           Sergey Ostrerov        DPTXPEMS - 45 - Allow 1-9 value for Bay Number field while adding space on Add Space dialog box. 
 *                                                                 Updated Latitude/Longitude, Demand Status validation. 
 * *****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Assets
{
    /// <summary>
    ///     Tariff status.  Reflects values in table [ConfigStatus]
    /// </summary>
    public enum TariffStatus
    {
        Active = 1,
        Inactive = 2
    };

    public class SimpleSpaceModel
    {
        private Int64 _spaceId;
        public Int64 SpaceId
        {
            get { return _spaceId; }
            set { SpaceIdDisplay = (_spaceId = value).ToString(); }
        }
        public string SpaceIdDisplay { get; set; }
        public int BayNumber { get; set; }

        public int MeterID { get; set; }
        public string MeterName { get; set; }
    }


    public class SpaceListModel : AssetBaseModel, IValidatableObject
    {
        /// <summary>
        /// Meter that is assigned to space.
        /// </summary>
        public int MeterId { get; set; }
        public int MeterAreaId { get; set; }


        public string DemandStatus { get; set; }
        public int DemandStatusId { get; set; }

        public string SpaceId { get { return this.AssetId.ToString(); } }

        public AssetIdentifier Sensor { get; set; }

        public string SensorId { get; set; }

        public int BayNumber { get; set; }
        public string BayName { get; set; }

        public int OperationalStatus { get; set; }
        public int OrgOperationalStatus { get; set; }

        /// <summary>
        /// Indicates that this model's data has been modified from the original retieval from
        /// the database.  Will indicate if need to save changes.
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// Indicates that this model has been created by program and not
        /// retrieved from database.
        /// </summary>
        public bool IsNew { get; set; }

        public SpaceListModel()
        {
            OperationalStatus = (int)OperationalStatusType.Inactive;
            OrgOperationalStatus = (int)OperationalStatusType.Inactive;
            IsChanged = false;
            State = AssetStateType.Pending;
            IsNew = true;
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // Bay Name
            if (string.IsNullOrWhiteSpace(BayName))
            {
                errors.Add(new ValidationResult("Bay Name is required.", new[] { "BayName" }));
            }

            // Latitude
            if (Latitude < -90.0 || Latitude > 90.0 || Latitude == 0.0)
            {
                errors.Add(new ValidationResult("Must be between -90.0 and 90.0 and non-zero", new[] { "Latitude" }));
            }
            else if(String.IsNullOrEmpty(Latitude.ToString())) {
                errors.Add(new ValidationResult("Latitude is Reqiured Field", new[] { "Latitude" }));
            }

            // Longitude
            if (Longitude < -180.0 || Longitude > 180.0 || Longitude == 0.0)
            {
                errors.Add(new ValidationResult("Must be between -180.0 and 180.0 and non-zero", new[] { "Longitude" }));
            }
            else if (String.IsNullOrEmpty(Longitude.ToString()))
            {
                errors.Add(new ValidationResult("Longitude is Reqiured Field", new[] { "Longitude" }));
            }

            // Demand Status
            if(DemandStatusId < 0)
            {
                errors.Add(new ValidationResult("Demand Status is required", new[] { "DemandStatusId" }));
            }

            return errors;
        }


    }

    public class SpaceViewModel : AssetBaseModel
    {
        public const string DefaultType = "SPACE";

        public int BayNumber { get; set; }

        public string DemandStatus { get; set; }

        public AssetIdentifier Meter { get; set; }
        public AssetIdentifier Sensor { get; set; }

        public SpaceConfigurationModel Configuration { get; set; }

        public SpaceStatusModel Occupancy { get; set; }

        public bool ViewOnly { get; set; }

        public int ActiveSensor { get; set; }

        public SpaceViewModel()
        {
            ViewOnly = false;
        }
    }

    public class SpaceStatusModel
    {
        public string OperationalStatus { get; set; }
        public DateTime? OperationalStatusDate { get; set; }

        public string OccupancyStatus { get; set; }
        public DateTime? OccupancyStatusDate { get; set; }

        public string NonComplianceStatus { get; set; }
        public DateTime? NonComplianceStatusDate { get; set; }
    }

    public class SpaceConfigurationModel
    {
        public DateTime? DateInstalled { get; set; }
        public string DateInstalledDisplay { get { return DateInstalled.HasValue ? DateInstalled.Value.ToString("d") : string.Empty; } }
        public string ActiveTariff { get; set; }
        public Int64 TariffConfigProfileId { get; set; }
        public DateTime? ActiveTariffDateCreated { get; set; }
        public string ActiveTariffDateCreatedDisplay { get { return ActiveTariffDateCreated.HasValue ? ActiveTariffDateCreated.Value.ToString("d") : string.Empty; } }
        public DateTime? ActiveTariffDateScheduled { get; set; }
        public string ActiveTariffDateScheduledDisplay { get { return ActiveTariffDateScheduled.HasValue ? ActiveTariffDateScheduled.Value.ToString("d") : string.Empty; } }
        public DateTime? ActiveTariffDateActivated { get; set; }
        public string ActiveTariffDateActivatedDisplay { get { return ActiveTariffDateActivated.HasValue ? ActiveTariffDateActivated.Value.ToString("d") : string.Empty; } }
        
        public string PendingTariff { get; set; }
        public Int64 PendingTariffConfigProfileId { get; set; }
        public DateTime? PendingTariffDateCreated { get; set; }
        public string PendingTariffDateCreatedDisplay { get { return PendingTariffDateCreated.HasValue ? PendingTariffDateCreated.Value.ToString("d") : string.Empty; } }
        public DateTime? PendingTariffDateScheduled { get; set; }
        public string PendingTariffDateScheduledDisplay { get { return PendingTariffDateScheduled.HasValue ? PendingTariffDateScheduled.Value.ToString("d") : string.Empty; } }
    }

    public class SpaceConfigurationEditModel
    {
        public string ActiveTariff { get; set; }
        public Int64 TariffConfigProfileId { get; set; }
        public List<SelectListItemWrapper> PendingTariff { get; set; }
        public Int64 PendingTariffConfigProfileId { get; set; }
        public DateTime? PendingToActiveDate { get; set; }
    }

    public class SpaceEditModel : AssetEditBaseModel, IValidatableObject
    {
        public int BayNumber { get; set; }

        public List<SelectListItemWrapper> DemandStatus { get; set; }
        public int? DemandStatusId { get; set; }

        public List<SelectListItemWrapper> AssociatedMeter { get; set; }
        public int AssociatedMeterId { get; set; }

        public List<SelectListItemWrapper> AssociatedSensor { get; set; }
        public int AssociatedSensorId { get; set; }

        public SpaceConfigurationEditModel Configuration { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            errors.AddRange( base.Validate( validationContext ) );

            if ( Configuration != null )
            {
                // PendingToActive
                if (Configuration.PendingToActiveDate > DateTime.MinValue && Configuration.PendingToActiveDate < DateTime.Now)
                {
                    errors.Add(new ValidationResult("Date must be in the future", new[] { "Configuration.PendingToActiveDate" }));
                }

                // If Pending Tariff then require Pending to Active date.
                if (Configuration.PendingTariffConfigProfileId > -1 && (Configuration.PendingToActiveDate == null || Configuration.PendingToActiveDate == DateTime.MinValue))
                {
                    errors.Add(new ValidationResult("Pending to active date required for Pending Tariff", new[] { "Configuration.PendingToActiveDate" }));
                }
            }

            return errors;
        }
    }

    public class SpaceMassEditModel : AssetMassUpdateBaseModel, IValidatableObject
    {
        public List<SelectListItemWrapper> Area { get; set; }
        public int AreaListId { get; set; }

        public string Street { get; set; }
        
        public List<SelectListItemWrapper> Zone { get; set; }
        public int ZoneId { get; set; }

        public List<SelectListItemWrapper> Suburb { get; set; }
        public int SuburbId { get; set; }


        public List<SelectListItemWrapper> DemandStatus { get; set; }
        public int DemandStatusId { get; set; }

        public DateTime? PendingToActiveDate { get; set; }

        public List<SelectListItemWrapper> ActiveTariff { get; set; }
        public int ActiveTariffId { get; set; }

        public List<SelectListItemWrapper> PendingTariff { get; set; }
        public int PendingTariffId { get; set; }



        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // PendingToActive
            if (PendingToActiveDate > DateTime.MinValue && PendingToActiveDate < DateTime.Now)
            {
                errors.Add(new ValidationResult("Date must be in the future", new[] { "PendingToActiveDate" }));
            }

            // If Pending Tariff then require Pending to Active date.
            if ( PendingTariffId > -1 && ( PendingToActiveDate == null || PendingToActiveDate == DateTime.MinValue ) )
            {
                errors.Add(new ValidationResult("Pending to active date required for Pending Tariff", new[] { "PendingToActiveDate" }));
            }

            return errors;
        }

    }

    public class SpaceHistoryModel
    {
        public List<SpaceViewModel> History { get; set; }
    }
}