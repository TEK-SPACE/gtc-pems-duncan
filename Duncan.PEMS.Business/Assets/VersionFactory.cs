using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.Assets;
using Duncan.PEMS.Entities.Enumerations;
using NLog;

namespace Duncan.PEMS.Business.Assets
{
    /// <summary>
    /// This is the class that handle PEMS Version business logic
    /// </summary>
    public class VersionFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Factory constructor taking a connection string name.
        /// </summary>
        /// <param name="connectionStringName">
        /// This is the string name indicating the connection string to use when opening a connection to
        /// the context for the Entity Framework.  This name should point to a connection string in the web.config
        /// or connectionStrings.config.
        /// </param>
        public VersionFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// Gets an instance of <see cref="AssetVersionViewModel"/> that is used by the view web page
        /// to view all of the attributes that are available for a Version.
        /// </summary>
        /// <returns>Instance of <see cref="AssetVersionViewModel"/></returns>
        public AssetVersionViewModel GetAssetVersionViewModel()
        {
            AssetVersionViewModel model = new AssetVersionViewModel();


            // Get file types from [FDFileType]
            model.FileTypesId = -1;
            model.FileTypes = new List<SelectListItem>();
            foreach (var fdFileType in PemsEntities.FDFileTypes)
            {
                if ( model.FileTypesId < 0 )
                    model.FileTypesId = fdFileType.FileType;

                model.FileTypes.Add(new SelectListItem()
                    {
                        Selected = model.FileTypesId == fdFileType.FileType,
                        Text = fdFileType.FileExtension + " - " + fdFileType.FileDesc,
                        Value = fdFileType.FileType.ToString()
                    });
            }

            // Get the meter groups list from [MeterGroup]
            model.MeterGroupsId = -1;
            model.MeterGroups = new List<SelectListItem>();
            // Add the default selection
            model.MeterGroups.Add(new SelectListItem()
            {
                Selected = true,
                Text = "All",
                Value = "-1"
            });
            foreach (var meterGroup in PemsEntities.MeterGroups)
            {
                model.MeterGroups.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = meterGroup.MeterGroupDesc,
                    Value = meterGroup.MeterGroupId.ToString()
                });
            }

            // Get the version groups list from [VersionGroup]
            model.VersionGroupsId = -1;
            model.VersionGroups = new List<SelectListItem>();
            model.VersionGroups.Add(new SelectListItem()
            {
                Selected = true,
                Text = "All",
                Value = "-1"
            });
            foreach (var versionGroup in PemsEntities.VersionGroups)
            {
                model.VersionGroups.Add(new SelectListItem()
                {
                    Selected = false,
                    Text = versionGroup.VersionGroupDesc,
                    Value = versionGroup.VersionGroupId.ToString()
                });
            }


            return model;
        }

        /// <summary>
        /// Refreshes an instance of <see cref="AssetVersionViewModel"/> that is used by the view web page
        /// to view all of the attributes that are available for a Version.
        /// </summary>
        /// <param name="model">Instance of <see cref="AssetVersionViewModel"/> to refresh</param>
        /// <returns>Instance of <see cref="AssetVersionViewModel"/></returns>
        public AssetVersionViewModel GetAssetVersionViewModel(AssetVersionViewModel model)
        {
            
            // Get file types from [FDFileType]
            model.FileTypes = new List<SelectListItem>();
            foreach (var fdFileType in PemsEntities.FDFileTypes)
            {
                model.FileTypes.Add(new SelectListItem()
                {
                    Selected = model.FileTypesId == fdFileType.FileType,
                    Text = fdFileType.FileExtension + " - " + fdFileType.FileDesc,
                    Value = fdFileType.FileType.ToString()
                });
            }

            // Get the meter groups list from [MeterGroup]
            model.MeterGroups = new List<SelectListItem>();
            // Add the default selection
            model.MeterGroups.Add(new SelectListItem()
            {
                Selected = model.MeterGroupsId == -1,
                Text = "All",
                Value = "-1"
            });
            foreach (var meterGroup in PemsEntities.MeterGroups)
            {
                model.MeterGroups.Add(new SelectListItem()
                {
                    Selected = model.MeterGroupsId == meterGroup.MeterGroupId,
                    Text = meterGroup.MeterGroupDesc,
                    Value = meterGroup.MeterGroupId.ToString()
                });
            }

            // Get the version groups list from [VersionGroup]
            model.VersionGroups = new List<SelectListItem>();
            model.VersionGroups.Add(new SelectListItem()
            {
                Selected = model.VersionGroupsId == -1,
                Text = "All",
                Value = "-1"
            });
            foreach (var versionGroup in PemsEntities.VersionGroups)
            {
                model.VersionGroups.Add(new SelectListItem()
                {
                    Selected = model.VersionGroupsId == versionGroup.VersionGroupId,
                    Text = versionGroup.VersionGroupDesc,
                    Value = versionGroup.VersionGroupId.ToString()
                });
            }


            return model;
        }

        /// <summary>
        /// Gets an instance of <see cref="AssetVersionSetModel"/> for a particular <paramref name="fileType"/>, <paramref name="meterGroup"/>,
        /// and <paramref name="versionGroup"/>.  
        /// </summary>
        /// <param name="fileType">File type id</param>
        /// <param name="meterGroup">Meter Group id <see cref="MeterGroups"/></param>
        /// <param name="versionGroup">Version group id</param>
        /// <returns>Instance of <see cref="AssetVersionSetModel"/></returns>
        public AssetVersionSetModel GetVersionSetModel(int fileType, int meterGroup, int versionGroup)
        {
            AssetVersionSetModel model = new AssetVersionSetModel();

            //get the file type groups
            var filteredGroups = PemsEntities.FDFileTypeMeterGroups.Where( m => m.FileType == fileType);

            //then if the meter group is set, filter by those
            if (meterGroup > -1)
                filteredGroups = filteredGroups.Where(x => x.MeterGroup == meterGroup);

            //then checkf or version group
            if (versionGroup > -1)
                filteredGroups = filteredGroups.Where(x => x.VersionGroup == versionGroup);

            if (filteredGroups.Any())
            {
                foreach (var fdftmg in filteredGroups)
                {
                    var list =
                        PemsEntities.AssetVersionMasters.Where(
                            m => m.FDFileTypeMeterGroupID == fdftmg.FDFileTypeMeterGroupID);

                    if (list.Any())
                    {
                        var fdft = PemsEntities.FDFileTypes.FirstOrDefault(m => m.FileType == fileType);
                        AssetVersionFileTypeModel assetVersionFileTypeModel = new AssetVersionFileTypeModel()
                            {
                                Id = fdft.FileType,
                                Extension = fdft.FileExtension,
                                Description = fdft.FileDesc
                            };

                        // Get meter group name
                        var mg = PemsEntities.MeterGroups.FirstOrDefault(m => m.MeterGroupId ==fdftmg.MeterGroup);
                        string meterGroupName = mg == null ? " " : mg.MeterGroupDesc;


                        // Get version group description.
                        var vg = PemsEntities.VersionGroups.FirstOrDefault(m => m.VersionGroupId ==fdftmg.VersionGroup);
                        string versionGroupName = vg == null ? " " : vg.VersionGroupDesc;

                        foreach (var assetVersionMaster in list)
                        {
                            model.Versions.Add(new AssetVersionModel()
                                {
                                    Id = assetVersionMaster.AssetVersionMasterId,
                                    Name = assetVersionMaster.VersionName,
                                    CreateDate = assetVersionMaster.CreateDate,
                                    CreateDateDisplay = assetVersionMaster.CreateDate.ToString("G"),
                                    FileType = assetVersionFileTypeModel.DisplayName,
                                    MeterGroup = meterGroupName,
                                    VersionGroup = versionGroupName
                                });
                        }
                    }
                }
            }

            model.Versions.Sort();

            return model;
        }


        /// <summary>
        /// Checks for the existance of a version (name) for a particular <paramref name="fileType"/>, <paramref name="meterGroup"/>,
        /// and <paramref name="versionGroup"/>.  
        /// </summary>
        /// <param name="name">Version name to check</param>
        /// <param name="fileType">File type id</param>
        /// <param name="meterGroup">Meter Group id <see cref="MeterGroups"/></param>
        /// <param name="versionGroup">Version group id</param>
        /// <returns>True if name already exists, false if not</returns>
        public bool VersionExists(string name, int fileType, int meterGroup, int versionGroup)
        {
            AssetVersionSetModel assetVersionSetModel = GetVersionSetModel( fileType, meterGroup, versionGroup );
            return assetVersionSetModel.Versions.FirstOrDefault( m => m.Name.Equals( name.Trim(), StringComparison.CurrentCultureIgnoreCase ) ) != null;
        }

        /// <summary>
        /// Adds a new version (name) for a particular <paramref name="fileType"/>, <paramref name="meterGroup"/>,
        /// and <paramref name="versionGroup"/>.  
        /// </summary>
        /// <param name="name">Version name to add</param>
        /// <param name="fileType">File type id</param>
        /// <param name="meterGroup">Meter Group id <see cref="MeterGroups"/></param>
        /// <param name="versionGroup">Version group id</param>
        public void AddVersion(string name, int fileType, int meterGroup, int versionGroup)
        {

            // Get/create a row in [FDFileTypeMeterGroup]
            var fdftmg = PemsEntities.FDFileTypeMeterGroups.FirstOrDefault(m => m.FileType == fileType
                && (meterGroup == -1 ? m.MeterGroup == null : m.MeterGroup == meterGroup)
                && (versionGroup == -1 ? m.VersionGroup == null : m.VersionGroup == versionGroup));

            if ( fdftmg == null )
            {
                fdftmg = new FDFileTypeMeterGroup()
                    {
                        FileType = fileType,
                        MeterGroup = meterGroup == -1 ? null : (int?)meterGroup,
                        VersionGroup = versionGroup == -1 ? null : (int?)versionGroup
                    };
                PemsEntities.FDFileTypeMeterGroups.Add( fdftmg );
                PemsEntities.SaveChanges();
            }

            // Add a row in [AssetVersionMaster]
            var avm = new AssetVersionMaster()
                {
                    FDFileTypeMeterGroupID = fdftmg.FDFileTypeMeterGroupID,
                    VersionName = name.Trim(),
                    CreateDate = DateTime.Now
                };

            PemsEntities.AssetVersionMasters.Add( avm );
            PemsEntities.SaveChanges();
        }
    }
}
