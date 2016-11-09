using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Duncan.PEMS.SpaceStatus.DataMappers
{
    interface IDataMapper
    {
        // Main method that populates dto with data
        Object GetData(IDataReader reader);

        /*
        // Gets the num results returned. Needed for data paging.
        int GetRecordCount(IDataReader reader);
        */
    }
}