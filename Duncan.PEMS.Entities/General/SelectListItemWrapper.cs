using System;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.General
{
    /// <summary>
    ///     Wrapper class to allow setting of "Value" string in base class in a linq statement.
    ///     (from mmc in PemsEntities.MechanismMasterCustomers
    ///     where mmc.CustomerId == customerId && mmc.IsDisplay == true
    ///     select new SelectListItem() {
    ///     Selected = false,
    ///     Text = mmc.MechanismDesc,
    ///     Cannot do this ==>                 Value = mmc.MechanismId.ToString()
    ///     }).ToList();
    /// </summary>
    public class SelectListItemWrapper : SelectListItem
    {
        public int TextInt
        {
            set { this.Text = value.ToString(); }
        }

        public Int64 TextInt64
        {
            set { this.Text = value.ToString(); }
        }

        public int ValueInt
        {
            set { this.Value = value.ToString(); }
        }

        public Int64 ValueInt64
        {
            set { this.Value = value.ToString(); }
        }
    }
}