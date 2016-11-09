using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using Duncan.PEMS.Entities.General;

namespace Duncan.PEMS.Entities.Tariffs
{
    #region Tariff Rate Models

    public class TariffRateConfigurationModel : IValidatableObject
    {
        public int CustomerId { get; set; }

        public Int64 ConfigProfileId { get; set; }
        public string ConfigProfileIdDisplay { get { return ConfigProfileId.ToString(); } }

        public Int64 TariffRateConfigurationId { get; set; }
        public string TariffRateConfigurationIdDisplay { get { return TariffRateConfigurationId.ToString(); } }

        public int TariffRateCount { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("g") : string.Empty; } }
        public int? CreatedBy { get; set; }

        public DateTime? ConfiguredOn { get; set; }
        public string ConfiguredOnDisplay { get { return ConfiguredOn.HasValue ? ConfiguredOn.Value.ToString("g") : string.Empty; } }
        public int? ConfiguredBy { get; set; }

        public TariffStateType State { get; set; }
        public string StateName { get { return State.ToString(); } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            //// Name
            //if (string.IsNullOrWhiteSpace(Name))
            //{
            //    errors.Add(new ValidationResult("Configuration Name is required.", new[] { "Name" }));
            //}

            return errors;
        }

        public override bool Equals(object obj)
        {
            if ( !( obj is TariffRateConfigurationModel ) ) return false;
            var trcm = obj as TariffRateConfigurationModel;
            return this.TariffRateConfigurationId == trcm.TariffRateConfigurationId;
        }
    }

    public class TariffRateModel : IValidatableObject, IComparable<TariffRateModel>
    {
        /// <summary>
        /// Details the space associated with this tariff rate.  This may be null.
        /// </summary>
        public SpaceViewModel Space { get; set; }

        public int CustomerId { get; set; }

        public Int64 TariffRateId { get; set; }
        public string TariffRateIdDisplay { get { return TariffRateId.ToString(); } }

        public string RateName { get; set; }
        public int RateNameIndex { get; set; }
        public string RateDescription { get; set; }

        public int RateInCents { get; set; }

        public int? PerTimeValue { get; set; }
        public int? PerTimeUnitId { get; set; }
        public string PerTimeUnitName { get; set; }

        public int? MaxTimeValue { get; set; }
        public int? MaxTimeUnitId { get; set; }
        public string MaxTimeUnitName { get; set; }

        public int? GracePeriodMinute { get; set; }

        public string LinkedTariffRateName { get; set; }
        public Int64? LinkedTariffRateId { get; set; }

        public TariffRateModel LinkedTariffRate { get; set; }

        public bool LockMaxTime { get; set; }

        public bool IsChanged { get; set; }

        public bool IsSaved { get; set; }
        
        /// <summary>
        /// This is a flag used by a sorting algorithm class.  Do not use this flag for anything else.
        /// </summary>
        public bool Used { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string CreatedOnDisplay { get { return CreatedOn.HasValue ? CreatedOn.Value.ToString("g") : string.Empty; } }
        public int? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedOnDisplay { get { return UpdatedOn.HasValue ? UpdatedOn.Value.ToString("g") : string.Empty; } }
        public int? UpdatedBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            // RateName
            if (string.IsNullOrWhiteSpace(RateName))
            {
                errors.Add(new ValidationResult("Rate Name is required.", new[] { "RateName" }));
            }

            return errors;
        }

        /// <summary>
        /// Compare method
        /// </summary>
        /// <param name="other"><see cref="TariffRateModel"/> to compare</param>
        /// <returns>1 if <see cref="other"/> is larger, 0 if equal, -1 if less</returns>
        public int CompareTo(TariffRateModel other)
        {
            if ( this.CreatedOn.HasValue && other.CreatedOn.HasValue ) return CreatedOn.Value.CompareTo( other.CreatedOn.Value );
            if ( this.CreatedOn.HasValue ) return 1;
            if ( other.CreatedOn.HasValue ) return -1;
            return 0;
        }

    }

    #endregion

    #region Tariff Rate Utilities

    public class TariffRateModelUtility
    {
        /// <summary>
        /// This method sorts a list of <see cref="TariffRateModel"/> into an ordered list of linked lists
        /// of tariff rate such that each tariff rate in the returned list represents the starting 
        /// tariff rate of a linked list of tariff rates.  It is possible that the linked list 
        /// only has one member tariff rate.
        ///
        /// This method returns a list that is two dimensions.  Now we should have a list that is composed of tariff rates.  Some of these 
        /// tariff rates link to child tariff rates. An example model is:
        /// 
        ///  leafNodes (5 entries) (9 total tariffs)
        ///    tr1
        ///    tr2
        ///    tr3 -- tr6 -- tr9
        ///    tr4 -- tr5
        ///    tr7 -- tr8
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<TariffRateModel> LinkedList(List<TariffRateModel> list)
        {
            // Prepare source list
            foreach (var trm in list)
            {
                trm.Used = false;
                trm.LinkedTariffRate = null;
            }

            // Create a list of the tariff rates that have no linked rate.  These are 
            // the "leaf" nodes in the tariff rate tree.
            List<TariffRateModel> leafNodes = new List<TariffRateModel>();
            foreach (var trm in list)
            {
                if (!trm.LinkedTariffRateId.HasValue || trm.LinkedTariffRateId == 0)
                {
                    trm.Used = true;
                    leafNodes.Add(trm);
                }
            }

            // If there are remaining nodes in list that have not been examined then build
            // linked lists of tariff rates.
            while (list.Any(m => !m.Used))
            {
                bool nodeWasAssigned = false;
                // For each "unused node" in original list...
                // Is this "unused" node from original list the parent of a item in the leafNodes list?
                foreach (var trm in list.Where(m => !m.Used))
                {
                    var leafNode = leafNodes.FirstOrDefault(m => m.TariffRateId == trm.LinkedTariffRateId);
                    if (leafNode != null)
                    {
                        // Found a linked rate parent/child.
                        trm.LinkedTariffRate = leafNode;
                        trm.Used = true;
                        leafNodes.Add(trm);
                        leafNodes.Remove(leafNode);
                        nodeWasAssigned = true;
                    }
                }

                // If the unused count does not change then there is a circular tariff rate dependency.
                // Order the remaining unused tariff rates by date and grab the oldest one 
                // as an arbitrary starting point.
                if (!nodeWasAssigned)
                {
                    var unusedList = list.Where(m => !m.Used).ToList();
                    unusedList.Sort();
                    leafNodes.Add(unusedList[0]);
                    unusedList[0].Used = true;
                }
            }

            // Now we should have a list that is composed of tariff rates.  Some of these 
            // tariff rates link to child tariff rates. An example model is:
            // 
            //  leafNodes (5 entries) (9 total tariffs)
            //    tr1
            //    tr2
            //    tr3 -- tr6 -- tr9
            //    tr4 -- tr5
            //    tr7 -- tr8

            // Now sort the leafNodes list on date.
            leafNodes.Sort();

            return leafNodes;
        }



        /// <summary>
        /// This method sorts a list of <see cref="TariffRateModel"/> into an ordered list such that 
        /// tariff rates are sorted by the "root rate" (the rate that is either at the beginning of a linked
        /// chain of tariff rates or a stand-alone tariff rate).  These "root rates" are sorted by
        /// the update date.
        /// </summary>
        /// <param name="list">List of unsorted/ordered <see cref="TariffRateModel"/></param>
        /// <returns>Sorted and order list of <see cref="TariffRateModel"/> based on date and linked tariffs</returns>
        public static List<TariffRateModel> Sort(List<TariffRateModel> list)
        {
            // Create the "linked list" version of this set of tariff rates.
            List<TariffRateModel> leafNodes = LinkedList(list);

            // Now flatten the list so it can be returned.
            // Continuing with the example model, leafNodes will be flattened in to
            //
            //  tariffs (9 entries) (9 total tariffs)
            //    tr1
            //    tr2
            //    tr3
            //    tr6
            //    tr9
            //    tr4
            //    tr5
            //    tr8
            //    tr8

            List<TariffRateModel> tariffs = new List<TariffRateModel>();

            foreach (var trm in leafNodes)
            {
                tariffs.Add(trm);
                var childRate = trm.LinkedTariffRate;
                // Protect against circular list
                while (childRate != null && childRate != trm)
                {
                    tariffs.Add(childRate);
                    childRate = childRate.LinkedTariffRate;
                }
            }

            // Clear any "instance references" to linked rates.
            tariffs.ForEach(m => m.LinkedTariffRate = null);

            return tariffs;
        }


        /// <summary>
        /// This method filters a list of <see cref="TariffRateModel"/> returning a list of tariff rates
        /// that are not assigned as linked rates to another tariff.   
        /// </summary>
        /// <param name="list">List of unsorted/ordered <see cref="TariffRateModel"/></param>
        /// <returns>Order list of <see cref="TariffRateModel"/> that are not linked back to a another tariff rate.</returns>
        public static List<TariffRateModel> UnlinkedTariffs(List<TariffRateModel> list)
        {
            // Create the "linked list" version of this set of tariff rates.
            List<TariffRateModel> leafNodes = LinkedList(list);

            // Now create a list of tariff rate based on the first entry of 
            // each linked list of tariffs in the leafNodes list.
            // Continuing with the example model, leafNodes will be flattened in to
            // 
            //  unassignedTariffs (5 entries) (5 total tariffs)
            //    tr1
            //    tr2
            //    tr3
            //    tr4
            //    tr7


            List<TariffRateModel> unassignedTariffs = new List<TariffRateModel>();

            foreach (var trm in leafNodes)
            {
                // An unlinked tariff rate is represented by a "root node".  To aid in
                // avoiding nonsensical tariff rate relationships, if a "root node" ends 
                // with a tariff rate that does not link to another rate then that 
                // linked is a candidate to link to.
                // 
                if (trm.LinkedTariffRate == null)
                {
                    // A single rate linked list.
                    unassignedTariffs.Add(trm);
                }
                else
                {
                    // Walk the linked list to see if it terminates.
                    var childRate = trm.LinkedTariffRate;
                    while (childRate != null && childRate.LinkedTariffRateId != trm.TariffRateId)
                    {
                        childRate = childRate.LinkedTariffRate;
                    }
                    if (childRate == null)
                    {
                        // A linked list of rates that is not a circular reference.
                        unassignedTariffs.Add(trm);
                    }
                }
            }

            return unassignedTariffs;
        }

    }

    #endregion
}
