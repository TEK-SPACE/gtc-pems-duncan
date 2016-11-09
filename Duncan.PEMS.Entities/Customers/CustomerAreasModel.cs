/******************* CHANGE LOG **************************************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             ___________________________________________________________________________________________________
 * 
 * 01/22/2014       Sergey Ostrerov                 DPTXPEMS-45 - Can't create new customer; Replace text box to Drop Down Box for Area editing.
 * 01/29/2014       Sergey Ostrerov                 DPTXPEMS-8, 14, 45 Reopened - Can't create new customer; Replace text box to Drop Down Box for Area editing.
 * 01/31/2014       Sergey Ostrerov                 DPTXPEMS - 45 Reopened - Can't create new customer; Replace Drop Down Box to text box for Area editing. 
 * *******************************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.Customers
{
    public class CustomerAreasModel : CustomerBaseModel
    {
        public char Separator = '|';

        // Area
        public List<CustomerArea> Areas { get; set; }

        // Zones
        public List<CustomerZone> Zones { get; set; }
        public List<CustomerCustomGroup> CustomGroup1 { get; set; }
        public List<CustomerCustomGroup> CustomGroup2 { get; set; }
        public List<CustomerCustomGroup> CustomGroup3 { get; set; }

        //Groups: 1, 2, 3
        public List<string> NewAreas { get; set; }
        public List<string> NewZones { get; set; }
        public List<string> NewCustomGroup1s { get; set; }
        public List<string> NewCustomGroup2s { get; set; }
        public List<string> NewCustomGroup3s { get; set; }
    }
}
