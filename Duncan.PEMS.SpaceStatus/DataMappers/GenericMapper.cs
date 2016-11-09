using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;



// This is the default DataMapper that will be used if we don't have a
// mapper that was built specifically for the type.  This mapper uses 
// reflection so it will be much slower than a DataMapper created 
// specifically for the DTO type.

namespace Duncan.PEMS.SpaceStatus.DataMappers
{
    class GenericMapper : IDataMapper
    {
        public System.Type DtoType { get; set; }
        private bool _isInitialized = false;
        private List<PropertyOrdinalMap> _PropertyOrdinalMappings;


        public GenericMapper(System.Type type)
        {
            DtoType = type;
        }


        private void InitializeMapper(IDataReader reader)
        {
            PopulatePropertyOrdinalMappings(reader);
            _isInitialized = true;
        }


        public void PopulatePropertyOrdinalMappings(IDataReader reader)
        {
            // Get the PropertyInfo objects for our DTO type and map them to 
            // the ordinals for the fields with the same names in our reader.  
            _PropertyOrdinalMappings = new List<PropertyOrdinalMap>();
            PropertyInfo[] properties = DtoType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                PropertyOrdinalMap map = new PropertyOrdinalMap();
                map.Property = property;
                try
                {
                    map.Ordinal = reader.GetOrdinal(property.Name);
                    _PropertyOrdinalMappings.Add(map);
                }
                catch { }
            }
        }


        public Object GetData(IDataReader reader)
        {
            if (!_isInitialized) { InitializeMapper(reader); }
            object dto = Activator.CreateInstance(DtoType);
            foreach (PropertyOrdinalMap map in _PropertyOrdinalMappings)
            {
                if (!reader.IsDBNull(map.Ordinal))
                {
                    map.Property.SetValue(dto, reader.GetValue(map.Ordinal), null);
                }
            }
            return dto;
        }


        private class PropertyOrdinalMap
        {
            public PropertyInfo Property { get; set; }
            public int Ordinal { get; set; }
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
