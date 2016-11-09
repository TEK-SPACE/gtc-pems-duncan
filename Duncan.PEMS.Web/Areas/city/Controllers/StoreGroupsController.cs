using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duncan.PEMS.Security;
using Duncan.PEMS.Framework.Controller;

namespace Duncan.PEMS.Web.Areas.city.Controllers
{
    public class StoreGroupsController : PemsController
    {
        //
        // GET: /city/StoreGroup/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Creates a group for a store by store name
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="groupName"></param>
        /// <param name="groupDescription"></param>
        /// <returns></returns>
        public ActionResult CreateStoreGroup(string storeName, string groupName, string groupDescription)
        {
            CurrentAuthorizationManager.CreateGroup( groupName, groupDescription);
            return View();
        }

    }
}
