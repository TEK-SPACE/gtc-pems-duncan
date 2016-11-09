/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                   DESCRIPTION
 * ___________      ___________________        _________________________________________________________________________________________________________
 * 
 * 02/12/2014       Sergey Ostrerov            Issue: DPTXPEMS-237 - Edit Screen for Event Code Crashes.Added validation rule for Abbreviated Description field.
 * 
 * *******************************************************************************************************************************************************************/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Events
{
    public class EventCodesCustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerDisplayName { get; set; }
        public CustomerStatusModel Status { get; set; }
    }

    public class EventCodesViewModel : EventCodesCustomerModel
    {
        public List<EventCodeViewModel> Codes { get; set; }

        public EventCodesViewModel()
        {
            Status = new CustomerStatusModel();
            Codes = new List<EventCodeViewModel>();
        }
    }

    public class EventCodeViewModel : EventCodesCustomerModel
    {
        public string Source { get; set; }
        public int SourceId { get; set; }

        public string Type { get; set; }
        public int TypeId { get; set; }

        public string Category { get; set; }
        public int CategoryId { get; set; }

        public string AssetType { get; set; }
        public int AssetTypeId { get; set; }

        public string AlarmTier { get; set; }
        public int AlarmTierId { get; set; }

        public int Id  { get; set; }
        public string DescAbbrev { get; set; }
        public string DescVerbose { get; set; }

        public int? SLAMinutes { get; set; }
        public bool ApplySLA { get; set; }

        public bool IsAlarm { get; set; }
    }


    public class EventCodeEditModel : EventCodesCustomerModel, IValidatableObject
    {
        public List<SelectListItemWrapper> Source { get; set; }
        public string SourceDisplay { get; set; }
        public int SourceId { get; set; }

        public List<SelectListItemWrapper> Type { get; set; }
        public int TypeId { get; set; }
        public int AlarmTypeId { get; set; }

        public List<SelectListItemWrapper> Category { get; set; }
        public int CategoryId { get; set; }

        public List<SelectListItemWrapper> AssetType { get; set; }
        public int AssetTypeId { get; set; }

        public List<SelectListItemWrapper> AlarmTier { get; set; }
        public int AlarmTierId { get; set; }

        public int Id { get; set; }
        public string DescAbbrev { get; set; }
        public string DescVerbose { get; set; }

        public int? SLAMinutes { get; set; }
        public bool ApplySLA { get; set; }

        public bool IsAlarm { get; set; }

        // Helper variable to determine which page was referrer
        public bool FromDetailsPage { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // Long Description
            if (DescVerbose.Length > 50)
            {
                errors.Add(new ValidationResult("Long Description must be 50 characters or less.", new[] { "DescVerbose" }));
            }

            // Abbreviated Description

            if (DescAbbrev.Length > 16)
            {
                errors.Add(new ValidationResult("Abbreviated Description must be 16 characters or less.", new[] { "DescAbbrev" }));
            }


            // Source
            if (SourceId < 0)
            {
                errors.Add(new ValidationResult("Event Source is required.", new[] { "Source" }));
            }

            // Type
            if (TypeId < 0)
            {
                errors.Add(new ValidationResult("Event Type is required.", new[] { "Type" }));
            }
            else
            {
                // Is Event Type an alarm?
                if ( TypeId == AlarmTypeId )
                {
                    // Check if "Is Alarm" checked.
                    if ( !IsAlarm )
                    {
                        errors.Add(new ValidationResult("An alarm Type was selected, Is Alarm must be checked.", new[] { "IsAlarm" }));
                    }

                    // Check if Alarm Tier selected.
                    if(AlarmTierId < 0)
                    {
                        errors.Add(new ValidationResult("An alarm Type was selected, Alarm Severity must be selected.", new[] { "AlarmTier" }));
                    }
                }
            }

            // Category
            if (CategoryId < 0)
            {
                errors.Add(new ValidationResult("Event Category is required.", new[] { "Category" }));
            }

            // Asset Type
            if (AssetTypeId < 0)
            {
                errors.Add(new ValidationResult("Asset Type is required.", new[] { "AssetType" }));
            }

            // Is an SLA required?
            if ( ApplySLA )
            {
                if ( !SLAMinutes.HasValue || SLAMinutes <= 0 )
                {
                    errors.Add(new ValidationResult("Non-zero SLA is required.", new[] { "SLAMinutes" }));
                }
            }

            // Alarm Tier
            if (AlarmTierId < 0)
            {
                errors.Add(new ValidationResult("Alarm Severity is required.", new[] { "AlarmTier" }));
            }

            return errors;
        }
    }


    public class EventCodesUploadResultsModel
    {
        public int CustomerId { get; set; }
        public List<string> Results { get; set; }
        public List<string> Errors { get; set; }

        public string UploadedFileName { get; set; }

        public bool HasErrors { get { return Errors.Count > 0; } }

        public EventCodesUploadResultsModel()
        {
            Results = new List<string>();
            Errors = new List<string>();
        }
    }

}
