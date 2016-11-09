using System;

namespace Duncan.PEMS.Entities.Customers
{
    public class Setting : IComparable<Setting>
    {
        public int Id { get; set; }
        public string Value { set; get; }
        public bool Default { get; set; }
        public int SortOrder { get; set; }

        public int CompareTo(Setting other)
        {
            return SortOrder.CompareTo( other.SortOrder );
        }
    }
}