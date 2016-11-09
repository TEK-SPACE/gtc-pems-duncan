using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.Assets
{
    public class AssetVersionViewModel
    {
        public List<SelectListItem> FileTypes { get; set; }
        public int FileTypesId { get; set; }
        public List<SelectListItem> MeterGroups { get; set; }
        public int MeterGroupsId { get; set; }
        public List<SelectListItem> VersionGroups { get; set; }
        public int VersionGroupsId { get; set; }
    }

    public class AssetVersionSetModel
    {
        public AssetVersionSetModel()
        {
            Versions = new List<AssetVersionModel>();
        }

        public List<AssetVersionModel> Versions { get; set; }
    }

    public class AssetVersionModel : IComparable<AssetVersionModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateDisplay { get; set; }

        public string FileType { get; set; }

        public string VersionGroup { get; set; }

        public string MeterGroup { get; set; }

        public int CompareTo(AssetVersionModel other)
        {
            return other.CreateDate.CompareTo( CreateDate );
        }
    }

    public class AssetVersionFileTypeModel
    {
        public int Id { get; set; }

        public string DisplayName
        {
            get { return Extension + " - " + Description; }
        }

        public string Extension { get; set; }
        public string Description { get; set; }
    }

    public class AssetVersionGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}