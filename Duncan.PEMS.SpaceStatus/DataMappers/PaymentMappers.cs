using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using Duncan.PEMS.SpaceStatus.DataShapes;

namespace Duncan.PEMS.SpaceStatus.DataMappers
{
    #region PaymentRecordDataMapper
    public class PaymentRecordDataMapper : IDataMapper
    {
        private bool _isInitialized = false;
        private int _ordinal_CustomerId;
        private int _ordinal_MeterId;
        private int _ordinal_TransactionId;
        private int _ordinal_TransactionDateTime;
        private int _ordinal_Amount;
        private int _ordinal_TimePaid;
        private int _ordinal_Method;
        private int _ordinal_BayNumber;
        private int _ordinal_PaymentType;

        private void InitializeMapper(IDataReader reader)
        {
            PopulateOrdinals(reader);
            _isInitialized = true;
        }

        public void PopulateOrdinals(IDataReader reader)
        {
            _ordinal_CustomerId = reader.GetOrdinal("CustomerId");
            _ordinal_MeterId = reader.GetOrdinal("MeterId");
            _ordinal_TransactionId = reader.GetOrdinal("TransactionId");
            _ordinal_TransactionDateTime = reader.GetOrdinal("TransactionDateTime");
            _ordinal_Amount = reader.GetOrdinal("AmountInCents");
            _ordinal_TimePaid = reader.GetOrdinal("TimePaid");
            _ordinal_Method = reader.GetOrdinal("Method");
            _ordinal_BayNumber = reader.GetOrdinal("BayNumber");
            _ordinal_PaymentType = reader.GetOrdinal("TransactionType");
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
            PaymentRecord dto = new PaymentRecord();

            if (!reader.IsDBNull(_ordinal_CustomerId)) { dto.CustomerId = Convert.ToInt32(reader[_ordinal_CustomerId]); }
            if (!reader.IsDBNull(_ordinal_MeterId)) { dto.MeterId = Convert.ToInt32(reader[_ordinal_MeterId]); }
            if (!reader.IsDBNull(_ordinal_TransactionId)) { dto.TransactionId = Convert.ToInt32(reader[_ordinal_TransactionId]); }
            if (!reader.IsDBNull(_ordinal_TransactionDateTime)) { dto.TransactionDateTime = Convert.ToDateTime(reader[_ordinal_TransactionDateTime]); }
            if (!reader.IsDBNull(_ordinal_BayNumber)) { dto.BayNumber = Convert.ToInt32(reader[_ordinal_BayNumber]); }
            if (!reader.IsDBNull(_ordinal_PaymentType)) { dto.PaymentType = Convert.ToString(reader[_ordinal_PaymentType]); }

            // These next fields are specialized, and not supported by our source code generator
            if (!reader.IsDBNull(_ordinal_Amount)) { dto.Amount = new USD(Convert.ToInt32(reader[_ordinal_Amount])); }
            if (!reader.IsDBNull(_ordinal_Method)) { dto.Method = (PaymentMethod)(reader[_ordinal_Method]); }
            if (!reader.IsDBNull(_ordinal_TimePaid)) { dto.TimePaid = TimeSpan.FromSeconds(Convert.ToInt32(reader[_ordinal_TimePaid])); }

            return dto;
        }
    }
    #endregion
}