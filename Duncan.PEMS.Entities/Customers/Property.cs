using System;

namespace Duncan.PEMS.Entities.Customers
{
    public class Property : IComparable<Property>
    {
        public int Id { get; set; }
        public string Value { set; get; }
        public string AdditionalValue { set; get; }
        public string PropertyGroupName { get; set; }
        public string ScreenName { set; get; }
        public bool IsRequired { get; set; }
        public bool IsDisplayed { get; set; }
        public int SortOrder { get; set; }

        public int CompareTo(Property other)
        {
            return SortOrder.CompareTo( other.SortOrder );
        }

        public string GetValue()
        {
            return AdditionalValue ?? ( PropertyGroupName.Equals( Value ) ? "" : Value );
        }
    }

    public class PropertyGroupItem : IComparable<PropertyGroupItem>
    {
        public int Id { get; set; }
        public string Value { set; get; }
        public int SortOrder { get; set; }

        public int CompareTo(PropertyGroupItem other)
        {
            return SortOrder.CompareTo( other.SortOrder );
        }
    }
}