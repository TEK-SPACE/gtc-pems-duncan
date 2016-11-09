using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Duncan.PEMS.Web.Areas.shared.Controllers
{
    public class LockController : Controller
    {
        /// <summary>
        /// Requests a "lock" on a record type and id.  For instance,
        /// Lock("Customer", 217) would request a lock on a customer record of customer id = 217
        /// 
        /// These values can be anything that is appropriate.  They are not restricted to a
        /// defined set of values and are not validated.
        /// </summary>
        /// <param name="type">Identifier of the type of record.</param>
        /// <param name="id">Numeric id of the record.</param>
        /// <returns></returns>
        public JsonResult Lock(string type, int id)
        {
            return Json(null);
        }
    }
}
