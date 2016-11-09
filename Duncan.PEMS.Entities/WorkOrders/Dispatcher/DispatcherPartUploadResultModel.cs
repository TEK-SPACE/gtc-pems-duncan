using System.Collections.Generic;
using LINQtoCSV;

namespace Duncan.PEMS.Entities.WorkOrders.Dispatcher
{
    public class DispatcherPartUploadResultModel
    {
        public DispatcherPartUploadResultModel()
        {
            Results = new List<string>();
            Errors = new List<string>();
        }
        public List<string> Results { get; set; }
        public List<string> Errors { get; set; }
        public string UploadedFileName { get; set; }
        public bool HasErrors { get { return Errors.Count > 0; } }
    }

    public class UploadPartModel
    {
        public int Id { get; set; }

        [CsvColumn(Name = "PartName", CanBeNull = false)]
        public string PartName { get; set; }

        [CsvColumn(Name = "MeterGroup", CanBeNull = false)]
        public int MeterGroup { get; set; }

        [CsvColumn(Name = "Category", CanBeNull = false)]
        public int Category { get; set; }            // [MechanismMaster]

        [CsvColumn(Name = "PartDesc", CanBeNull = true)]
        public string PartDesc { get; set; }


        [CsvColumn(Name = "CostInCents", CanBeNull = false)]
        public int CostInCents { get; set; }            // [AssetState]

        [CsvColumn(Name = "Status", CanBeNull = false)]
        public int Status { get; set; }
    }
}