using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerLabelsModel : CustomerBaseModel
    {
        public const string FieldLabelsPrefix = "FIELD";
        public const string GridLabelsPrefix = "GRID";

        public char Separator = '_';

        public CustomerLabelsModel()
        {
            LabelGroups = new Dictionary<string, List<CustomerLabel>>();
        }

        public Dictionary<string, List<CustomerLabel>> LabelGroups { get; set; }

    }

    public class CustomerLabel : IComparable<CustomerLabel>
    {
        public string TextType = "Text";
        public string LabelName { get; set; }
        public string StockLabel { get; set; }
        public int LabelId { get; set; }

        public string Type
        {
            get { return CustomLabelList == null ? TextType : "List"; }
        }

        public List<SelectListItem> CustomLabelList { get; set; }

        public string CustomLabel { get; set; }

        public int CompareTo(CustomerLabel other)
        {
            return this.LabelName.CompareTo(other.LabelName);
        }


    }
}