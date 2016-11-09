using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Duncan.PEMS.SpaceStatus.Models;

namespace Duncan.PEMS.SpaceStatus.DataMappers
{
    // DEBUG: RegulatedHours_ExperimentalDBSchemaV1Repository was partially implemented for RegulatedHours storage in ReinoComm DB,
    // but was never completed because the DB schema wasn't what we wanted.  For interim, we are using an XML file for repository
    // instead of the database. Therefore, RegulatedHours_ExperimentalDBSchemaV1Repository code is still a useful starting point
    // incase we end up moving to DB instead of XML, but it will need to be finalized and work with the current schema...

    public class RegulatedHours_ExperimentalDBSchemaV1_DTOMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_ID;
        private int _ordinal_ParkingSpaceID;
        private int _ordinal_DayOfWeek;
        private int _ordinal_RegulatedStartTime;
        private int _ordinal_RegulatedEndTime;
        private int _ordinal_MaxStayMinute;
        
        private int _ordinal_CustomerID;
        private int _ordinal_AreaID;
        private int _ordinal_MeterID;
        private int _ordinal_BayNumber;


        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }


        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_ID = reader.GetOrdinal("ID");
            _ordinal_ParkingSpaceID = reader.GetOrdinal("ParkingSpaceID");
            _ordinal_DayOfWeek = reader.GetOrdinal("DayOfWeek");
            _ordinal_RegulatedStartTime = reader.GetOrdinal("RegulatedStartTime");
            _ordinal_RegulatedEndTime = reader.GetOrdinal("RegulatedEndTime");
            _ordinal_MaxStayMinute = reader.GetOrdinal("MaxStayMinute");

            _ordinal_CustomerID = reader.GetOrdinal("CustomerID");
            _ordinal_AreaID = reader.GetOrdinal("AreaID");
            _ordinal_MeterID = reader.GetOrdinal("MeterID");
            _ordinal_BayNumber = reader.GetOrdinal("BayNumber");
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

            // For reference, here is an example of SQL that gathers data from the db
            /*
            SELECT rh.ID, rh.ParkingSpaceId, rh.DayOfWeek, rh.RegulatedStartTime, rh.RegulatedEndTime, rh.MaxStayMinute,
             ps.CustomerId, ps.AreaId, ps.MeterId, ps.BayNumber
             FROM RegulatedHours as rh, ParkingSpaces ps
             where rh.ParkingSpaceId = ps.ParkingSpaceId
             and ps.CustomerId = @CustomerId
             order by ps.MeterId, ps.BayNumber, rh.DayOfWeek, rh.RegulatedStartTime
             */

            if (!_isInitialized) { InitializeMapper(reader); }

            // Now we can load the data
            RegulatedHours_ExperimentalDBSchemaV1DTO dto = new RegulatedHours_ExperimentalDBSchemaV1DTO();
            if (!reader.IsDBNull(_ordinal_ID)) { dto.ID_PrimaryKey = reader.GetInt32(_ordinal_ID); }
            if (!reader.IsDBNull(_ordinal_ParkingSpaceID)) { dto.ParkingSpaceID = reader.GetInt64(_ordinal_ParkingSpaceID); }

            if (!reader.IsDBNull(_ordinal_DayOfWeek)) 
            { 
                dto.DayOfWeek = (DayOfWeek)(reader.GetByte(_ordinal_DayOfWeek) - 1); 
            }

            if (!reader.IsDBNull(_ordinal_RegulatedStartTime)) { dto.RegulatedStartTime_Minutes = reader.GetInt32(_ordinal_RegulatedStartTime); }
            if (!reader.IsDBNull(_ordinal_RegulatedEndTime)) { dto.RegulatedEndTime_Minutes = reader.GetInt32(_ordinal_RegulatedEndTime); }
            if (!reader.IsDBNull(_ordinal_MaxStayMinute)) { dto.MaxStayMinute = reader.GetInt32(_ordinal_MaxStayMinute); }

            if (!reader.IsDBNull(_ordinal_CustomerID)) { dto.CID = Convert.ToInt32(reader[_ordinal_CustomerID]); } // This isn't optimal, but safer incase its not an Int32 in DB (for example, might be tinyint or smallint)
            if (!reader.IsDBNull(_ordinal_AreaID)) { dto.AID = Convert.ToInt32(reader[_ordinal_AreaID]); }
            if (!reader.IsDBNull(_ordinal_MeterID)) { dto.MID = Convert.ToInt32(reader[_ordinal_MeterID]); }
            if (!reader.IsDBNull(_ordinal_BayNumber)) { dto.BayNumber = Convert.ToInt32(reader[_ordinal_BayNumber]); }
            return dto;
        }


        /*
        public int GetRecordCount(IDataReader reader)
        {
            Object count = reader["RecordCount"];
            return count == null ? 0 : Convert.ToInt32(count);
        }
        */
    }

}