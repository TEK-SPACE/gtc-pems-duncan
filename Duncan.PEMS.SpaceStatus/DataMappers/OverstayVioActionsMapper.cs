using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Duncan.PEMS.SpaceStatus.Models;

namespace Duncan.PEMS.SpaceStatus.DataMappers
{

    public class OverstayVioActionsDTO
    {
        public int UniqueKey { get; set; }
        public int CustomerID { get; set; }
        public int AreaID { get; set; }
        public int MeterID { get; set; }
        public int BayNumber { get; set; }
        public int RBACUserID { get; set; }
        public DateTime EventTimestamp { get; set; }
        public string ActionTaken { get; set; }
    }


    public class OverstayVioActionsMapper : IDataMapper
    {
        private bool _isInitialized = false;

        private int _ordinal_UniqueKey;
        private int _ordinal_CustomerID;
        private int _ordinal_MeterID;
        private int _ordinal_AreaID;
        private int _ordinal_BayNumber;
        private int _ordinal_RBACUserID;
        private int _ordinal_EventTimestamp;
        private int _ordinal_ActionTaken;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_UniqueKey = reader.GetOrdinal("UniqueKey");
            _ordinal_CustomerID = reader.GetOrdinal("CustomerID");
            _ordinal_AreaID = reader.GetOrdinal("AreaID");
            _ordinal_MeterID = reader.GetOrdinal("MeterID");
            _ordinal_BayNumber = reader.GetOrdinal("BayNumber");
            _ordinal_RBACUserID = reader.GetOrdinal("RBACUserID");
            _ordinal_EventTimestamp = reader.GetOrdinal("EventTimestamp");
            _ordinal_ActionTaken = reader.GetOrdinal("ActionTaken");
        }

        public Object GetData(IDataReader reader)
        {
            // This is where we define the mapping between the object properties and the 
            // data columns. The convention that should be used is that the object property 
            // names are exactly the same as the column names. However if there is some 
            // compelling reason for the names to be different the mapping can be defined here.

            // We assume the reader has data and is already on the row that contains the data 
            // we need. We don't need to call read. As a general rule, assume that every field must 
            // be null  checked. If a field is null then the nullvalue for that field has already 
            // been set by the DTO constructor, we don't have to change it.

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data
            OverstayVioActionsDTO dto = new OverstayVioActionsDTO();
            if (!reader.IsDBNull(_ordinal_UniqueKey)) { dto.UniqueKey = Convert.ToInt32(reader[_ordinal_UniqueKey]); }
            if (!reader.IsDBNull(_ordinal_CustomerID)) { dto.CustomerID = Convert.ToInt32(reader[_ordinal_CustomerID]); } 
            if (!reader.IsDBNull(_ordinal_AreaID)) { dto.AreaID = Convert.ToInt32(reader[_ordinal_AreaID]); }
            if (!reader.IsDBNull(_ordinal_MeterID)) { dto.MeterID = Convert.ToInt32(reader[_ordinal_MeterID]); }
            if (!reader.IsDBNull(_ordinal_BayNumber)) { dto.BayNumber = Convert.ToInt32(reader[_ordinal_BayNumber]); }
            if (!reader.IsDBNull(_ordinal_RBACUserID)) { dto.RBACUserID = Convert.ToInt32(reader[_ordinal_RBACUserID]); }
            if (!reader.IsDBNull(_ordinal_EventTimestamp)) { dto.EventTimestamp = Convert.ToDateTime(reader[_ordinal_EventTimestamp]); }
            if (!reader.IsDBNull(_ordinal_ActionTaken)) { dto.ActionTaken = Convert.ToString(reader[_ordinal_ActionTaken]); }
            return dto;
        }
    }

}