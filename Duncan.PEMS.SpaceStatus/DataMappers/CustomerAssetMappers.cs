using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Duncan.PEMS.SpaceStatus.DataShapes;

namespace Duncan.PEMS.SpaceStatus.DataMappers
{

    #region AreaAssetDataMapper
    public class AreaAssetDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_AreaID;
        private int _ordinal_AreaName;
        private int _ordinal_Description;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_AreaID = reader.GetOrdinal("AreaID");
            _ordinal_AreaName = reader.GetOrdinal("AreaName");
            _ordinal_Description = reader.GetOrdinal("Description");
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different, the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data into the DTO object from the DB reader
            AreaAsset dto = new AreaAsset();

            if (!reader.IsDBNull(_ordinal_AreaID)) { dto.AreaID = Convert.ToInt32(reader[_ordinal_AreaID]); }
            if (!reader.IsDBNull(_ordinal_AreaName)) { dto.AreaName = Convert.ToString(reader[_ordinal_AreaName]); }
            if (!reader.IsDBNull(_ordinal_Description)) { dto.AreaDescription = Convert.ToString(reader[_ordinal_Description]); }
            return dto;
        }
    }
    #endregion


    #region MeterAssetDataMapper
    public class MeterAssetDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_MeterID;
        private int _ordinal_AreaID;
        private int _ordinal_LibertyArea;
        private int _ordinal_ClusterID;
        private int _ordinal_MeterName;
        private int _ordinal_Description;
        private int _ordinal_MeterGroup;
        private int _ordinal_MeterGroupDesc;
        private int _ordinal_MeterType;
        private int _ordinal_Latitude;
        private int _ordinal_Longitude;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_MeterID = reader.GetOrdinal("MeterID");
            _ordinal_AreaID = reader.GetOrdinal("AreaID");
            _ordinal_LibertyArea = reader.GetOrdinal("LibertyArea");
            _ordinal_ClusterID = reader.GetOrdinal("ClusterID");
            _ordinal_MeterName = reader.GetOrdinal("MeterName");
            _ordinal_Description = reader.GetOrdinal("Description");
            _ordinal_MeterGroup = reader.GetOrdinal("MeterGroup");
            _ordinal_MeterGroupDesc = reader.GetOrdinal("MeterGroupDesc");
            _ordinal_MeterType = reader.GetOrdinal("MeterType");
            _ordinal_Latitude = reader.GetOrdinal("Latitude");
            _ordinal_Longitude = reader.GetOrdinal("Longitude");
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different, the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data into the DTO object from the DB reader
            MeterAsset dto = new MeterAsset();

            if (!reader.IsDBNull(_ordinal_MeterID)) { dto.MeterID = Convert.ToInt32(reader[_ordinal_MeterID]); }
            if (!reader.IsDBNull(_ordinal_AreaID)) { dto.AreaID_Internal = Convert.ToInt32(reader[_ordinal_AreaID]); }
            if (!reader.IsDBNull(_ordinal_LibertyArea)) { dto.AreaID_Liberty = Convert.ToInt32(reader[_ordinal_LibertyArea]); }
            if (!reader.IsDBNull(_ordinal_ClusterID)) { dto.PAMClusterID = Convert.ToInt32(reader[_ordinal_ClusterID]); }
            if (!reader.IsDBNull(_ordinal_MeterName)) { dto.MeterName = Convert.ToString(reader[_ordinal_MeterName]); }
            if (!reader.IsDBNull(_ordinal_Description)) { dto.MeterDescription = Convert.ToString(reader[_ordinal_Description]); }
            if (!reader.IsDBNull(_ordinal_MeterGroup)) { dto.MeterGroupID = Convert.ToInt32(reader[_ordinal_MeterGroup]); }
            if (!reader.IsDBNull(_ordinal_MeterGroupDesc)) { dto.MeterGroupDesc = Convert.ToString(reader[_ordinal_MeterGroupDesc]); }
            if (!reader.IsDBNull(_ordinal_MeterType)) { dto.MeterTypeID = Convert.ToInt32(reader[_ordinal_MeterType]); }
            if (!reader.IsDBNull(_ordinal_Latitude)) { dto.Latitude = Convert.ToSingle(reader[_ordinal_Latitude]); }
            if (!reader.IsDBNull(_ordinal_Longitude)) { dto.Longitude = Convert.ToSingle(reader[_ordinal_Longitude]); }
            return dto;
        }
    }
    #endregion

    #region PAMClusterAssetDataMapper
    public class PAMClusterAssetDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_ClusterID;
        private int _ordinal_Description;
        private int _ordinal_HostedBayStart;
        private int _ordinal_HostedBayEnd;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_ClusterID = reader.GetOrdinal("ClusterID");
            _ordinal_Description = reader.GetOrdinal("Description");
            _ordinal_HostedBayStart = reader.GetOrdinal("HostedBayStart");
            _ordinal_HostedBayEnd = reader.GetOrdinal("HostedBayEnd");
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different, the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data into the DTO object from the DB reader
            PAMClusterAsset dto = new PAMClusterAsset();

            if (!reader.IsDBNull(_ordinal_ClusterID)) { dto.ClusterID = Convert.ToInt32(reader[_ordinal_ClusterID]); }
            if (!reader.IsDBNull(_ordinal_Description)) { dto.ClusterDescription = Convert.ToString(reader[_ordinal_Description]); }
            if (!reader.IsDBNull(_ordinal_HostedBayStart)) { dto.HostedBayStart = Convert.ToInt32(reader[_ordinal_HostedBayStart]); }
            if (!reader.IsDBNull(_ordinal_HostedBayEnd)) { dto.HostedBayEnd = Convert.ToInt32(reader[_ordinal_HostedBayEnd]); }
            return dto;
        }
    }
    #endregion

    #region SpaceAssetDataMapper
    public class SpaceAssetDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_ParkingSpaceID;
        private int _ordinal_BayNumber;
        private int _ordinal_AreaID;
        private int _ordinal_LibertyArea;
        private int _ordinal_MeterID;
        private int _ordinal_ClusterID;
        private int _ordinal_HasSensor;
        private int _ordinal_SpaceType;
        private int _ordinal_Latitude;
        private int _ordinal_Longitude;
        private int _ordinal_CollRouteID;
        private int _ordinal_EnfRouteID;
        private int _ordinal_MaintRouteID;
        private int _ordinal_CustomGroup1;
        private int _ordinal_CustomGroup2;
        private int _ordinal_CustomGroup3;
        
        // We won't use an IsActive column from the database anymore.  Now we are using a local XML file in the webservice to keep track of inactive spaces
        /*private int _ordinal_IsActive;*/

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_ParkingSpaceID = reader.GetOrdinal("ParkingSpaceID");
            _ordinal_BayNumber = reader.GetOrdinal("BayNumber");
            _ordinal_AreaID = reader.GetOrdinal("AreaID");
            _ordinal_LibertyArea = reader.GetOrdinal("LibertyArea");
            _ordinal_MeterID = reader.GetOrdinal("MeterID");
            _ordinal_ClusterID = reader.GetOrdinal("ClusterID");
            _ordinal_HasSensor = reader.GetOrdinal("HasSensor");
            _ordinal_SpaceType = reader.GetOrdinal("SpaceType");
            _ordinal_Latitude = reader.GetOrdinal("Latitude");
            _ordinal_Longitude = reader.GetOrdinal("Longitude");
            _ordinal_CollRouteID = reader.GetOrdinal("CollRouteID");
            _ordinal_EnfRouteID = reader.GetOrdinal("EnfRouteID");
            _ordinal_MaintRouteID = reader.GetOrdinal("MaintRouteID");
            _ordinal_CustomGroup1 = reader.GetOrdinal("CustomGroup1");
            _ordinal_CustomGroup2 = reader.GetOrdinal("CustomGroup2");
            _ordinal_CustomGroup3 = reader.GetOrdinal("CustomGroup3");

            // We won't use an IsActive column from the database anymore.  Now we are using a local XML file in the webservice to keep track of inactive spaces
            /*_ordinal_IsActive = reader.GetOrdinal("IsActive");*/
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different, the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data into the DTO object from the DB reader
            SpaceAsset dto = new SpaceAsset();

            if (!reader.IsDBNull(_ordinal_ParkingSpaceID)) { dto.ParkingSpaceId_Internal = Convert.ToInt64(reader[_ordinal_ParkingSpaceID]); }
            if (!reader.IsDBNull(_ordinal_BayNumber)) { dto.SpaceID = Convert.ToInt32(reader[_ordinal_BayNumber]); }
            if (!reader.IsDBNull(_ordinal_AreaID)) { dto.AreaID_Internal = Convert.ToInt32(reader[_ordinal_AreaID]); }
            if (!reader.IsDBNull(_ordinal_LibertyArea)) { dto.AreaID_Liberty = Convert.ToInt32(reader[_ordinal_LibertyArea]); }
            if (!reader.IsDBNull(_ordinal_MeterID)) { dto.MeterID = Convert.ToInt32(reader[_ordinal_MeterID]); }
            if (!reader.IsDBNull(_ordinal_ClusterID)) { dto.PAMClusterID = Convert.ToInt32(reader[_ordinal_ClusterID]); }
            if (!reader.IsDBNull(_ordinal_HasSensor)) { dto.HasSensor = Convert.ToBoolean(reader[_ordinal_HasSensor]); }
            if (!reader.IsDBNull(_ordinal_SpaceType)) { dto.SpaceType = Convert.ToInt32(reader[_ordinal_SpaceType]); }
            if (!reader.IsDBNull(_ordinal_Latitude)) { dto.Latitude = Convert.ToSingle(reader[_ordinal_Latitude]); }
            if (!reader.IsDBNull(_ordinal_Longitude)) { dto.Longitude = Convert.ToSingle(reader[_ordinal_Longitude]); }
            if (!reader.IsDBNull(_ordinal_CollRouteID)) { dto.CollectionRouteID = Convert.ToInt32(reader[_ordinal_CollRouteID]); }
            if (!reader.IsDBNull(_ordinal_EnfRouteID)) { dto.EnforcementRouteID = Convert.ToInt32(reader[_ordinal_EnfRouteID]); }
            if (!reader.IsDBNull(_ordinal_MaintRouteID)) { dto.MaintRouteID = Convert.ToInt32(reader[_ordinal_MaintRouteID]); }
            if (!reader.IsDBNull(_ordinal_CustomGroup1)) { dto.CustomGroup1ID = Convert.ToInt32(reader[_ordinal_CustomGroup1]); }
            if (!reader.IsDBNull(_ordinal_CustomGroup2)) { dto.CustomGroup2ID = Convert.ToInt32(reader[_ordinal_CustomGroup2]); }
            if (!reader.IsDBNull(_ordinal_CustomGroup3)) { dto.CustomGroup3ID = Convert.ToInt32(reader[_ordinal_CustomGroup3]); }

            // We won't use an IsActive column from the database anymore.  Now we are using a local XML file in the webservice to keep track of inactive spaces
            /*
            if (!reader.IsDBNull(_ordinal_IsActive)) { dto.IsActive = Convert.ToBoolean(reader[_ordinal_IsActive]); }

            // We might as well log the fact that we loaded a space that is marked as "inactive".  This might be useful info to have available when 
            // researching an issue in the future...
            if (dto.IsActive == false)
            {
                Duncan.PEMS.SpaceStatus.UtilityClasses.Logging.AddTextToGenericLog(UtilityClasses.Logging.LogLevel.Debug,
                string.Format("Space is flagged as inactive: ParkingSpaceID={0}, MID={1}, BAY={2} (Based on HousingMaster.IsActive)", dto.ParkingSpaceId_Internal, dto.MeterID, dto.SpaceID),
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, 
                System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            */

            return dto;
        }
    }
    #endregion

    #region CustomGroup1AssetDataMapper
    public class CustomGroup1AssetDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_CustomGroupId;
        private int _ordinal_DisplayName;
        private int _ordinal_Comment;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_CustomGroupId = reader.GetOrdinal("CustomGroupId");
            _ordinal_DisplayName = reader.GetOrdinal("DisplayName");
            _ordinal_Comment = reader.GetOrdinal("Comment");
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different, the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data into the DTO object from the DB reader
            CustomGroup1Asset dto = new CustomGroup1Asset();

            if (!reader.IsDBNull(_ordinal_CustomGroupId)) { dto.CustomGroupId = Convert.ToInt32(reader[_ordinal_CustomGroupId]); }
            if (!reader.IsDBNull(_ordinal_DisplayName)) { dto.DisplayName = Convert.ToString(reader[_ordinal_DisplayName]); }
            if (!reader.IsDBNull(_ordinal_Comment)) { dto.Comment = Convert.ToString(reader[_ordinal_Comment]); }
            return dto;
        }
    }
    #endregion

    #region CustomerNameAndIdDataMapper
    public class CustomerNameAndIdDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_CustomerId;
        private int _ordinal_Name;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_CustomerId = reader.GetOrdinal("CustomerId");
            _ordinal_Name = reader.GetOrdinal("Name");
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different, the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data into the DTO object from the DB reader
            CustomerNameAndId dto = new CustomerNameAndId();

            if (!reader.IsDBNull(_ordinal_CustomerId)) { dto.CustomerId = Convert.ToInt32(reader[_ordinal_CustomerId]); }
            if (!reader.IsDBNull(_ordinal_Name)) { dto.Name = Convert.ToString(reader[_ordinal_Name]); }
            return dto;
        }
    }
    #endregion

}