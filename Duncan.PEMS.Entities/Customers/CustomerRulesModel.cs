using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerRulesModel : CustomerBaseModel, IValidatableObject
    {
        public string SchemaPrefix = "SCHEMA";
        public char Separator = '_';

        public CustomerRulesModel()
        {
            DiscountSchemas = new List<CustomerRulesDiscountSchemaModel>();
            ZeroOutMeter = false;
            BlacklistCC = false;
            Streetline = false;
            CloseWorkOrderEvents = false;
            DisplayFullMenu = false;
        }

        public bool DiscountSchema { get; set; }
        public List<CustomerRulesDiscountSchemaModel> DiscountSchemas { get; set; }

        public bool CloseWorkOrderEvents { get; set; }
        public bool ZeroOutMeter { get; set; }
        public bool BlacklistCC { get; set; }
        public bool Streetline { get; set; }
        public bool DisplayFullMenu { get; set; }


        //hidden filters
        public bool FieldDemandArea { get; set; }
        public bool FieldZone { get; set; }
        public bool FieldDiscountScheme { get; set; }
        public bool FieldCG1 { get; set; }
        public bool FieldCG2 { get; set; }
        public bool FieldCG3 { get; set; }



        public string PMEventCode { get; set; }

        public int GracePeriod { get; set; }
        public int FreeParkingLimit { get; set; }

        public CustomerRulesOperationalTimeModel OperationalTimes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if ( OperationalTimes != null )
            {
                errors.AddRange( OperationalTimes.Validate( validationContext ) );
            }

            return errors;
        }
    }

    public class CustomerRulesDiscountSchemaModel
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public bool Enabled { get; set; }

        public string Type { get; set; }
        public string ExpirationType { get; set; }

        public int? DiscountPercent { get; set; }
        public int? DiscountMinutes { get; set; }
        public int? MaxAmountCents { get; set; }

        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    public class CustomerRulesOperationalTimeModel : IValidatableObject
    {
        public CustomerRulesOperationalTimeModel()
        {
            HasOperationTime = false;
            HasPeakTime = false;
            HasEveningTime = false;
        }

        public bool HasOperationTime { get; set; }
        public DateTime OperationStart { get; set; }
        public string OperationStartDisplay { get; set; }
        public DateTime OperationEnd { get; set; }
        public string OperationEndDisplay { get; set; }

        public bool HasPeakTime { get; set; }
        public DateTime PeakStart { get; set; }
        public string PeakStartDisplay { get; set; }
        public DateTime PeakEnd { get; set; }
        public string PeakEndDisplay { get; set; }

        public bool HasEveningTime { get; set; }
        public DateTime EveningStart { get; set; }
        public string EveningStartDisplay { get; set; }
        public DateTime EveningEnd { get; set; }
        public string EveningEndDisplay { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            if ( HasOperationTime )
            {
                if ( HasOperationTime && OperationStart > OperationEnd )
                    errors.Add( new ValidationResult( "Starting time later than ending time.", new[] {"OperationTime"} ) );
                if ( HasOperationTime && OperationStart == OperationEnd )
                    errors.Add( new ValidationResult( "Time span cannot be zero.", new[] {"OperationTime"} ) );
                if ( HasPeakTime && PeakStart > PeakEnd )
                    errors.Add( new ValidationResult( "Starting time later than ending time.", new[] {"PeakTime"} ) );
                if ( HasPeakTime && PeakStart == PeakEnd )
                    errors.Add( new ValidationResult( "Time span cannot be zero.", new[] {"PeakTime"} ) );
                if ( HasEveningTime && EveningStart > EveningEnd )
                    errors.Add( new ValidationResult( "Starting time later than ending time.", new[] {"EveningTime"} ) );
                if ( HasEveningTime && EveningStart == EveningEnd )
                    errors.Add( new ValidationResult( "Time span cannot be zero.", new[] {"EveningTime"} ) );
            }
            return errors;
        }
    }
}