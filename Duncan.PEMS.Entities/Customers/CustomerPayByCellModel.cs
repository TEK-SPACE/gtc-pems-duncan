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
    public class CustomerPayByCellModel : CustomerBaseModel
    {
        public char Separator = '|';
        public List<SelectedIds> selectedIds { get; set; }
        public List<CustPayByCell> CustPayByCell { get; set; }
        public List<RipnetProp> Ripnet { get; set; }
    }
}
