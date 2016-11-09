using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Duncan.PEMS.Utilities;
using NetSqlAzMan;
using NetSqlAzMan.Interfaces;

namespace Duncan.PEMS.Security.Menu
{
    /// <summary>
    /// Class that represents the menu collection for PEMS web page.  This collection which inherits 
    /// from <see cref="System.Collections.Generic.List{T}"/> is generated based upon the user authorization
    /// rights from NetSqlAzMan.
    /// </summary>
    /// 
    /// 
    [Serializable]
    public class PemsMenu : List<PemsMenuItem>
    {
        /// <summary>
        /// NetSqlAzMan storage context.
        /// </summary>
        private IAzManStorage _storage;

        /// <summary>
        /// NetSqlAzMan "database"-based user
        /// </summary>
        private IAzManDBUser _dbUser;

        /// <summary>
        /// The base url that is used to build the fully qualified urls for any menu item that is
        /// defined by an 'application' and an 'operation' from NetSqlAzMan.
        /// </summary>
        private string _baseUrl;

        /// <summary>
        /// menu url
        /// </summary>
        private string _menuUrl;

        /// <summary>
        /// String that represents the store that the menu is built upon.
        /// </summary>
        private string _store;

        private bool _displayFullMenu;

        /// <summary>
        /// Constructor that builds a hierarchical menu based upon authorization right of the user
        /// in the given store. 
        /// </summary>
        /// <param name="store">String that represents the store that the menu is built upon.</param>
        /// <param name="user">User name.</param>
        /// <param name="baseUrl">Base url for generated menu urls.</param>
        /// <param name="menuUrl">Base url for menu resolved.</param>
        /// <param name="displayFullMenu">Whether to display full menu</param>
        public PemsMenu(string store, string user, string baseUrl, string menuUrl, bool displayFullMenu)
        {
            _displayFullMenu = displayFullMenu; 
            _storage = new SqlAzManStorage(connectionString: ConfigurationManager.ConnectionStrings[Constants.Security.RbacConnectionStringName].ConnectionString);
            _dbUser = _storage.GetDBUser(user);
            _baseUrl = baseUrl;
            _store = store;
            _menuUrl = menuUrl;
            // Create the menu
            GetMenu();
        }

        /// <summary>
        /// Creates an instance of a <see cref="PemsMenuItem"/> based upon the data contained in
        /// the 'task' parameter attributes or an 'operation' associated with the 'task'.
        /// </summary>
        /// <param name="task">An instance of a <see cref="IAzManItem"/> task.</param>
        /// <returns>An instance of a <see cref="PemsMenuItem"/> of null if invalid or incomplete  <see cref="PemsMenuItem"/> specification.</returns>
        private PemsMenuItem GetTaskMenu(IAzManItem task)
        {
            // task must be of type Task.
            if (task.ItemType != ItemType.Task)
                return null;

            // Create a menu instance from the task.
            var taskMenuItem = new PemsMenuItem()
                {
                    Label = task.Attributes.ContainsKey( Constants.Menu.MenuText ) ? task.Attributes[Constants.Menu.MenuText].Value : null,
                    ToolTip = task.Attributes.ContainsKey( Constants.Menu.ToolTip ) ? task.Attributes[Constants.Menu.ToolTip].Value : null,
                    NewWindow = task.Attributes.ContainsKey( Constants.Menu.NewWindow ) && task.Attributes[Constants.Menu.NewWindow].Value == "true",
                    Icon = task.Attributes.ContainsKey( Constants.Menu.Icon ) ? task.Attributes[Constants.Menu.Icon].Value : null,
                    ID = task.ItemId.ToString()
                };

            try
            {
                taskMenuItem.Order = Int32.Parse( task.Attributes.ContainsKey( Constants.Menu.MenuOrder ) ? task.Attributes[Constants.Menu.MenuOrder].Value : "0" );
            }
            catch (Exception)
            {
                taskMenuItem.Order = 0;
            }

            // Does the task have a defined (external) Url defined?
            if (task.Attributes.ContainsKey(Constants.Menu.Url))
            {
                taskMenuItem.Url = task.Attributes[Constants.Menu.Url].Value;
                taskMenuItem.MenuUrl = string.Empty;
                return taskMenuItem;
            }

            // Does this task have a child item of type ItemType.Operation
            foreach (var azManItem in task.Members)
            {
                if (azManItem.Value.ItemType == ItemType.Operation)
                {
                    // If an operation was associated with this task, create a url based on that operation.
                    taskMenuItem.Url = _baseUrl + azManItem.Value.Application.Name + "/" + azManItem.Value.Name;
                    taskMenuItem.MenuUrl = _menuUrl + azManItem.Value.Application.Name + "/" + azManItem.Value.Name;
                    return taskMenuItem;
                }
            }

            // At this point, the remaining possibility is that the task has attributes that 
            // point to an operation in a different application.  This would be indicated by
            // the task haveing two attributes defined: "application" and "operation"
            if (task.Attributes.ContainsKey(Constants.Menu.Application) && task.Attributes.ContainsKey(Constants.Menu.Operation))
            {
                taskMenuItem.Url = _baseUrl + task.Attributes[Constants.Menu.Application].Value + "/" + task.Attributes[Constants.Menu.Operation].Value;
                taskMenuItem.MenuUrl = _menuUrl + task.Attributes[Constants.Menu.Application].Value + "/" + task.Attributes[Constants.Menu.Operation].Value;
                return taskMenuItem;
            }

            // If made it to this point, there is no link associated with this
            // menu.  It may be a parent or a dead branch.  A cleanup process after the entire menu
            // structure is built will prune any dead branches.
            return taskMenuItem;
        }

        /// <summary>
        /// Builds any child menus associated with the parent menu item.
        /// </summary>
        /// <param name="parentKey">String representing the key of the <see cref="System.Collections.Generic.KeyValuePair{K, V}"/> of the parent menu item.  
        /// Used to skip the parent "value" where a parent is a top-level menu.</param>
        /// <param name="parentMenuItem">Instance of <see cref="PemsMenuItem"/> representing the parent menu item.</param>
        /// <param name="items">Enumerable list of <see cref="IAzManItem"/> that may contain cild menu definitions.</param>
        /// <param name="level">Integer representing the menu level.  This is used to handle a quirk in 
        /// NetSqlAzMan and the items that are in a collection of <paramref name="items"/></param>
        private void GetTaskMenuItems(string parentKey, PemsMenuItem parentMenuItem, Dictionary<string, IAzManItem> items, int level)
        {
            // Walk this list and add child menu item for each task.
            foreach (var itemKvp in items)
            {
                // If the item is a Task and it is not a member of another task, examine it for rights.
                IAzManItem item = itemKvp.Value;

                // If item is not a Task then move to next item.
                if (item.ItemType != ItemType.Task) continue;

                if (item.ItemsWhereIAmAMember.Count == level && !itemKvp.Key.Equals(parentKey))
                {
                    // Do I have authorization to this Task or does it have an Operation I have access to?
                    var accessAllowed = AccessAllowed(item);

                    if (!accessAllowed && !_displayFullMenu)
                        continue;

                    // Build a menu for this task.
                    PemsMenuItem taskMenuItem = GetTaskMenu(item);
                    if (taskMenuItem == null)
                        continue;

                    // Add to parent menu
                    parentMenuItem.Add(taskMenuItem);
                    // Does this task have any sub-tasks?
                    if (item.HasMembers())
                    {
                        GetTaskMenuItems(itemKvp.Key, taskMenuItem, item.Members, level + 1);
                        taskMenuItem.Sort();
                    }

                    if (!accessAllowed)
                    {
                        taskMenuItem.MenuUrl = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Determines if user is authorized to view the resource that is indicated by
        /// this <see cref="IAzManItem"/> task.  If the user is not authorized for the task or
        /// authorization cannot be determined, return false.
        /// </summary>
        /// <param name="task">Instance of <see cref="IAzManItem"/> to examine.</param>
        /// <returns>True if authorized, otherwise returns false.</returns>
        private bool AccessAllowed(IAzManItem task)
        {
            // item must be of type Task.
            if (task.ItemType != ItemType.Task)
                return false;

            // Does the task have a defined (external) Url defined?
            // If it does then access rights are based upon the task
            if (task.Attributes.ContainsKey(Constants.Menu.Url))
                return task.CheckAccess(_dbUser, DateTime.Now, null) == AuthorizationType.Allow;

            // Does this task have a child item of type ItemType.Operation
            foreach (var azManItem in task.Members)
            {
                if (azManItem.Value.ItemType == ItemType.Operation)
                {
                    // If an operation was associated with this task, return access rights
                    // based on that operation.
                    return azManItem.Value.CheckAccess(_dbUser, DateTime.Now, null) == AuthorizationType.Allow;
                }
            }

             
            // At this point, the remaining possibility is that the task has attributes that 
            // point to an operation in a different application.  This would be indicated by
            // the task haveing two attributes defined: "application" and "operation"
            if (task.Attributes.ContainsKey(Constants.Menu.Application) && task.Attributes.ContainsKey(Constants.Menu.Operation))
            {
                return _storage.CheckAccess(task.Application.Store.Name,
                    task.Attributes[Constants.Menu.Application].Value, task.Attributes[Constants.Menu.Operation].Value,
                    _dbUser, DateTime.Now, true, null)
                    == AuthorizationType.Allow;
            }

            // If made it to this point, there is something not defined correctly so 
            // return a false since cannot determine access rights.
            return false;
        }

        /// <summary>
        /// Primary method that build menu.  This method examines all of the 'applications' that are
        /// present in a 'store' under the NetSqlAzMan model. Any valid menus are added to this instance.
        /// </summary>
        private void GetMenu()
        {
            try
            {
                IAzManStore store = _storage.GetStore(_store);

                // Am I allowed in this store (city)?  This is top-level check.
                if (store.CheckStoreAccess(_dbUser, DateTime.Now, null))
                {
                    foreach (var applicationKvp in store.Applications)
                    {
                        IAzManApplication application = applicationKvp.Value;

                        // Are there any menu items under this application that user has rights to?
                        // If not then continue on to next application
                        if (!_displayFullMenu && !application.CheckApplicationAccess(_dbUser, DateTime.Now, null))
                            continue;

                        // Check if there is a menu at this (app) level.
                        // If the application has a task named the same as the application then derive 
                        // the MenuItem item from that Task.  It is possible for the application to not have a menu.
                        PemsMenuItem appMenuItem = application.Items.ContainsKey(application.Name)
                                                   ? GetTaskMenu(application.Items[application.Name]) : null;

                        if (appMenuItem != null)
                        {
                            // Get any child menu items.
                            GetTaskMenuItems(application.Name, appMenuItem, application.Items, 0);
                            appMenuItem.Sort();

                            // Add present top-level app menu if it has children or has explicit link.
                            if (appMenuItem.Url != null || appMenuItem.Count > 0)
                                Add(appMenuItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // If any exception, clear menu.
                Clear();
            }

            // Prune dead branches.
            Prune(this);

            // Sort the menu list by Order
            this.Sort();

        }

        /// <summary>
        /// Walks a list of <see cref="PemsMenuItem"/> until reaches leaf.  Checks if
        /// leaf has a Url.  If not, leaf is trimmed.
        /// </summary>
        /// <param name="list">List of <see cref="PemsMenuItem"/></param>
        private void Prune(List<PemsMenuItem> list)
        {
            List<PemsMenuItem> deadLeaf = new List<PemsMenuItem>();
            foreach (PemsMenuItem pemsMenuItem in list)
            {
                Prune(pemsMenuItem);
                if(!pemsMenuItem.HasLink() && !pemsMenuItem.Any())
                    deadLeaf.Add(pemsMenuItem);
            }
            foreach (PemsMenuItem pemsMenuItem in deadLeaf)
            {
                list.Remove( pemsMenuItem );
            }

        }


        /// <summary>
        /// Override of ToString().  Primarily used for debugging.
        /// </summary>
        /// <returns>String representation of menu including all child menu items.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Menus for ").AppendLine(_store);

            foreach (PemsMenuItem menu in this)
            {
                sb.Append(" ").AppendLine(menu.ToString());
                if (menu.Any())
                {
                    foreach (PemsMenuItem taskMenu in menu)
                    {
                        sb.Append("   ").AppendLine(taskMenu.ToString());
                        if (taskMenu.Any())
                        {
                            foreach (PemsMenuItem taskSubMenu in taskMenu)
                            {
                                sb.Append("     ").AppendLine(taskSubMenu.ToString());
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
