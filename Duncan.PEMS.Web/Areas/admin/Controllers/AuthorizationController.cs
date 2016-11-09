using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Business.Customers;
using Duncan.PEMS.Entities.Customers;
using Duncan.PEMS.Framework.Controller;
using NLog;

namespace Duncan.PEMS.Web.Areas.admin.Controllers
{
    public class AuthorizationController : PemsController
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        
        #endregion


        #region Edit Default Password

        // TODO:  Add support to set the default password for Admin site.

        public ActionResult DefaultPwd()
        {
            var model = (new CustomerFactory(CurrentCity.PemsConnectionStringName)).GetAdminCustomerIdentificationModel( CurrentCity.Id );

            return View(model);
        }

        [HttpPost]
        public ActionResult DefaultPwd(AdminCustomerIdentificationModel model)
        {
            // Save values
            if (ModelState.IsValid)
            {
                (new CustomerFactory(CurrentCity.PemsConnectionStringName)).SetAdminCustomerIdentificationModel(CurrentCity.Id, model);
            }

            return View(model);
        }

        #endregion


    }
}
