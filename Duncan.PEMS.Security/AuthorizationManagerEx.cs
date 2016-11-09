using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.Entities.Audit;
using Duncan.PEMS.Entities.Roles;
using Duncan.PEMS.Entities.Users;
using Duncan.PEMS.Utilities;
using NLog;
using NetSqlAzMan;
using NetSqlAzMan.Interfaces;

namespace Duncan.PEMS.Security
{
    /// <summary>
    /// This class encapsulates Authorization functionality for PEMS resource access control.
    /// It presently uses NetSqlAzMan to provide the authorization services.
    /// 
    /// This is a continuation of <see cref="AuthorizationManager"/> to provide bulk export and 
    /// import to NetSqlAzMan
    /// </summary>
    public partial class AuthorizationManager 
    {
        /// <summary>
        /// Property used to relate Application Group names to their SIDs.
        /// </summary>
        private Dictionary<string, string> _authorizationGroup = new Dictionary<string, string>();

        /// <summary>
        /// Property used to relate User names to their SIDs.
        /// </summary>
        private Dictionary<string, string> _authorizationUser = new Dictionary<string, string>();


        private List<string> _xmlProcessError = new List<string>();
        /// <summary>
        /// <see cref="List{String}"/> property used to report XML configuration errors.  Each entry
        /// represents an error encountered during the latest processing of an XML configuration file.  If
        /// the <see cref="XmlProcessErrors"/> is empty, no errors were encountered.
        /// </summary>
        public List<string> XmlProcessErrors { get { return _xmlProcessError; } }

        private List<string> _xmlProcessLog = new List<string>();
        /// <summary>
        /// <see cref="List{String}"/> property used to report XML configuration logging.  Each entry
        /// represents a particularly notable event encountered during the latest processing of an XML configuration file.
        /// </summary>
        public List<string> XmlProcessLogs { get { return _xmlProcessLog; } }


        #region Configuration Constants

        private const int _MenuOrderStep = 100;

        private const string _XML_Node_MenuItem = "MenuItem";
        private const string _XML_Node_MenuTree = "MenuTree";

        private const string _XML_Node_AuthTree = "AuthTree";
        private const string _XML_Node_AuthItem = "AuthItem";

        private const string _XML_Node_Applications = "Applications";
        private const string _XML_Node_Application = "Application";
        private const string _XML_Node_Operations = "Operations";
        private const string _XML_Node_Operation = "Operation";

        private const string _XML_Node_Authorizations = "Authorizations";
        private const string _XML_Node_Authorize = "Authorize";

        private const string _XML_Node_Users = "Users";
        private const string _XML_Node_User = "User";

        private const string _XML_Node_Groups = "Groups";
        private const string _XML_Node_Group = "Group";
        private const string _XML_Node_Member = "Member";

        private const string _XML_Attrib_Name = "Name";
        private const string _XML_Attrib_Action = "Action";
        private const string _XML_Attrib_Url = "url";
        private const string _XML_Attrib_NewWindow = "newwindow";
        private const string _XML_Attrib_Application = "application";
        private const string _XML_Attrib_Operation = "operation";


        #endregion

        #region Configuration Methods - Public

        /// <summary>
        /// Imports Authorization settings via an XML file.  It is suggested that the file originate from
        /// a call to <see cref="GetConfiguration"/>
        /// </summary>
        /// <param name="xmlFilePath">Path and file name of the imported settings.</param>
        /// <returns>True if import did not return any errors.</returns>
        public bool SetConfiguration(string xmlFilePath)
        {
            _xmlProcessError.Clear();
            XmlReader reader = null;

            try
            {
                reader = XmlReader.Create(xmlFilePath);
                XmlProcessUpdate( reader );
                reader.Close();
            }
            catch (Exception ex)
            {
                _xmlProcessError.Add(ex.Message);
                if ( reader != null)
                {
                    reader.Close();
                }
            }

            return !_xmlProcessError.Any();
        }

        /// <summary>
        /// Creates an xml file representing the Authorization settings.
        /// May be used to create base file to subsequently update the Authorization settings
        /// via <see cref="SetConfiguration"/>
        /// 
        /// File will be named "Duncan.Auth.YYY.MM.DD.[Customer].xml
        /// </summary>
        /// <param name="xmlFilePath">Path where file will created</param>
        public string GetConfiguration(string xmlFilePath)
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            // Create file name
            sb.Append(xmlFilePath);
            if (sb[sb.Length - 1] != '\\')
                sb.Append('\\');
            sb.Append("Duncan.Auth.").Append(now.Year).Append(".")
              .Append(now.Month).Append(".").Append(now.Day)
              .Append(".").Append(_storeName).Append(".xml");

            string fileName = sb.ToString();


            XmlWriter writer = XmlWriter.Create(fileName, settings);

            writer.WriteStartDocument();
            // Write a general comment
            sb.Clear();
            sb.Append("Configuration of NetSqlAzMan for ").Append(_storeName).Append(". ")
              .Append("Created on ").Append(DateTime.Now.ToString());
            writer.WriteComment(sb.ToString());

            writer.WriteStartElement("Customer");
            writer.WriteAttributeString(_XML_Attrib_Name, _storeName);

            // Must occur before XmlGetGroups
            XmlGetUsers(writer);

            // Must occur before XmlGetAuthTree
            XmlGetGroups(writer);

            // Must occur before XmlGetAuthTree
            XmlGetApplications(writer);

            // Must occur second to last
            XmlGetAuthTree(writer);

            // Must occur last
            XmlGetMenuTree(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();

            // return file name and path.
            return fileName;
        }

        #endregion

        #region Configuration Methods - Private (Create XML)

        private void XmlGetUsers(XmlWriter writer)
        {
            writer.WriteStartElement(_XML_Node_Users);
            foreach (var user in _store.GetDBUsers())
            {
                writer.WriteStartElement(_XML_Node_User);
                writer.WriteAttributeString(_XML_Attrib_Name, user.UserName);
                writer.WriteAttributeString("SID", user.CustomSid.StringValue);
                writer.WriteEndElement();

                // Add this user SID and name to dictionary.
                _authorizationUser.Add(user.CustomSid.StringValue, user.UserName);
          
            }
            writer.WriteEndElement();
        }

        private void XmlGetGroups(XmlWriter writer)
        {
            writer.WriteStartElement(_XML_Node_Groups);
            foreach (var group in _store.StoreGroups)
            {
                writer.WriteStartElement(_XML_Node_Group);
                writer.WriteAttributeString(_XML_Attrib_Name, group.Value.Name);
                writer.WriteAttributeString("SID", group.Value.SID.StringValue);

                // Add this group SID and name to dictionary. - only add once
                _authorizationGroup.Add(group.Value.SID.StringValue, group.Value.Name);

                var azManGroup = group.Value;
                var members = azManGroup.Members;

                foreach (var member in members)
                {
                    try
                    {
                        writer.WriteElementString(_XML_Node_Member, _authorizationUser[member.Value.SID.StringValue]);
                    }
                    catch (Exception)
                    {
                    }
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void XmlGetAuthTree(XmlWriter writer)
        {
            writer.WriteStartElement(_XML_Node_AuthTree);
            foreach (var app in _store.Applications)
            {
                if (!app.Value.Attributes.ContainsKey(Constants.Menu.AuthText))
                    continue;

                // App has authtext attribute
                writer.WriteStartElement(_XML_Node_AuthItem);
                writer.WriteAttributeString(_XML_Attrib_Name, app.Value.Name);
                writer.WriteAttributeString(Constants.Menu.AuthText, app.Value.Attributes[Constants.Menu.AuthText].Value);
                // Write out each Authorization entry
                foreach (var item in app.Value.Items)
                {
                    if (item.Value.ItemType == ItemType.Role && item.Value.Attributes.ContainsKey( Constants.Menu.AuthText ))
                    {
                        writer.WriteStartElement(_XML_Node_AuthItem);
                        writer.WriteAttributeString(_XML_Attrib_Name, item.Value.Name);
                        writer.WriteAttributeString(Constants.Menu.AuthText, item.Value.Attributes[Constants.Menu.AuthText].Value);
                        // Write out the child operations
                        writer.WriteStartElement(_XML_Node_Operations);
                        foreach (var operation in item.Value.Members.Where(operation => operation.Value.ItemType == ItemType.Operation))
                        {
                            writer.WriteElementString(_XML_Node_Operation, operation.Value.Name);
                        }
                        writer.WriteEndElement();

                        // Write out application groups authorized
                        writer.WriteStartElement(_XML_Node_Authorizations);
                        foreach (var authorization in item.Value.Authorizations)
                        {
                            if ( authorization.AuthorizationType == AuthorizationType.Allow )
                            {
                                writer.WriteElementString(_XML_Node_Authorize, _authorizationGroup[authorization.SID.StringValue]);
                            }
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void XmlGetMenuTree(XmlWriter writer)
        {
            writer.WriteStartElement(_XML_Node_MenuTree);
            foreach (var app in _store.Applications)
            {
                // Does this app have a menu item?
                if ( !app.Value.Items.ContainsKey( app.Key ) )
                    continue;
                
                writer.WriteStartElement(_XML_Node_MenuItem);
                writer.WriteAttributeString(_XML_Attrib_Name, app.Value.Name);
                XmlGetMenuTreeMenuAttributes( writer, app.Value.Items[app.Key].Attributes );
                XmlGetMenuTreeLink( writer, app.Value.Items[app.Key] );
                // Now walk each item looking for menu items (Tasks)
                foreach (var item in app.Value.Items)
                {
                    if (item.Value.ItemType == ItemType.Task && item.Key != app.Key)
                    {
                        XmlGetMenuTreeMenu(writer, item.Value);
                    }
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void XmlGetMenuTreeMenuAttributes(XmlWriter writer, Dictionary<string, IAzManAttribute<IAzManItem>> attributes)
        {
            if (attributes.ContainsKey(Constants.Menu.MenuText))
                writer.WriteAttributeString(Constants.Menu.MenuText, attributes[Constants.Menu.MenuText].Value);

            if (attributes.ContainsKey(Constants.Menu.MenuOrder))
                writer.WriteAttributeString(Constants.Menu.MenuOrder, attributes[Constants.Menu.MenuOrder].Value);

            if (attributes.ContainsKey(Constants.Menu.ToolTip))
                writer.WriteAttributeString(Constants.Menu.ToolTip, attributes[Constants.Menu.ToolTip].Value);

            if (attributes.ContainsKey(Constants.Menu.Icon))
                writer.WriteAttributeString(Constants.Menu.Icon, attributes[Constants.Menu.Icon].Value);
        }

        private void XmlGetMenuTreeMenu(XmlWriter writer, IAzManItem item)
        {
            // Make sure this is a menu item
            if ( !item.Attributes.ContainsKey( Constants.Menu.MenuText ) )
                return;

            writer.WriteStartElement(_XML_Node_MenuItem);
            writer.WriteAttributeString(_XML_Attrib_Name, item.Name);
            XmlGetMenuTreeMenuAttributes(writer, item.Attributes);
            XmlGetMenuTreeLink(writer, item);
            // Now walk each item looking for menu items (Tasks)
            foreach (var childItem in item.Members)
            {
                if (childItem.Value.ItemType == ItemType.Task && childItem.Key != item.Application.Name)
                {
                    XmlGetMenuTreeMenu(writer, childItem.Value);
                }
            }
            writer.WriteEndElement();
        }

        private void XmlGetMenuTreeLink(XmlWriter writer, IAzManItem item)
        {
            writer.WriteStartElement("Link");

            // Is there an associated operation?
            var operation = item.Members.FirstOrDefault( m => m.Value.ItemType == ItemType.Operation ).Value;
            if (operation != null)
            {
                writer.WriteAttributeString(Constants.Menu.Operation, operation.Name);
                writer.WriteAttributeString(Constants.Menu.Application, operation.Application.Name);
            }
            else
            {
                // Target is either a URL or an application/operation
                if (item.Attributes.ContainsKey(Constants.Menu.Url))
                    writer.WriteAttributeString(Constants.Menu.Url, item.Attributes[Constants.Menu.Url].Value);

                if (item.Attributes.ContainsKey(Constants.Menu.Operation))
                    writer.WriteAttributeString(Constants.Menu.Operation, item.Attributes[Constants.Menu.Operation].Value);

                if (item.Attributes.ContainsKey(Constants.Menu.Application))
                    writer.WriteAttributeString(Constants.Menu.Application, item.Attributes[Constants.Menu.Application].Value);
            }


            if (item.Attributes.ContainsKey(Constants.Menu.Target))
                writer.WriteAttributeString(Constants.Menu.Target, item.Attributes[Constants.Menu.Target].Value);

            if (item.Attributes.ContainsKey(Constants.Menu.NewWindow))
                writer.WriteAttributeString(Constants.Menu.NewWindow, item.Attributes[Constants.Menu.NewWindow].Value);

            writer.WriteEndElement();
        }

        private void XmlGetApplications(XmlWriter writer)
        {
            writer.WriteStartElement(_XML_Node_Applications);
            foreach (var app in _store.Applications)
            {
                writer.WriteStartElement(_XML_Node_Application);
                writer.WriteAttributeString(_XML_Attrib_Name, app.Value.Name);
                // Write out the child operations
                foreach (var item in app.Value.Items)
                {
                    if ( item.Value.ItemType == ItemType.Operation )
                    {
                        writer.WriteElementString(_XML_Node_Operation, item.Value.Name);
                    }
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion

        #region Configuration Methods - Private (Process XML)


        private enum XmlAction
        {
            NoOp,
            Add,
            Remove,
            Update,
            Clear
        };


        private void XmlProcessUpdate(XmlReader reader)
        {
            // Move to Element Customer
            reader.MoveToContent();

            // Now pointing to Customer node.
            _storeName = reader.GetAttribute( _XML_Attrib_Name );

            // Point 'this' to customer.
            _store = GetStore(_storeName);
            if ( _store == null )
            {
                _xmlProcessError.Add("No store named " + _storeName + " found.  Check XML input.");
                return;
            }

            // Now move to first "Start Element" under Customer.

            // Now move to Applications element
            while (reader.Read())
            {
                if ( reader.IsStartElement())
                {
                    if ( reader.Name.Equals( _XML_Node_Groups ) )
                    {
                        // Process Groups
                        XmlProcessGroups( reader );
                    }
                    else if (reader.Name.Equals(_XML_Node_Applications))
                    {
                        // Process Applications
                        XmlProcessApplications( reader );
                    }
                    else if (reader.Name.Equals(_XML_Node_AuthTree))
                    {
                        // Process Authorization Tree
                        XmlProcessAuthTree( reader );
                    }
                    else if (reader.Name.Equals(_XML_Node_MenuTree))
                    {
                        // Process Menu Tree
                        XmlProcessMenuTree( reader );
                    }
                }
            }
        }

        private void XmlProcessGroups(XmlReader reader)
        {
            XmlAction action = XmlAction.NoOp;
            XmlAction memberAction = XmlAction.NoOp;
            XElement parentNode = (XElement)XNode.ReadFrom(reader);
            foreach (XElement eGroup in parentNode.Elements())
            {
                // Only act on 'Group' elements
                if ( !eGroup.Name.LocalName.Equals( _XML_Node_Group ) )
                    continue;
                // Does this group have an action?
                action = Action( eGroup );
                switch (action)
                {
                    case XmlAction.Add:
                        // Add this customer group.
                        _xmlProcessLog.Add("Create " + _storeName + " group '" + eGroup.Attribute(_XML_Attrib_Name).Value + "'.");
                        CreateGroup(eGroup.Attribute(_XML_Attrib_Name).Value, "XML Insert");

                        // Are there Members to add to this Group?
                        if ( eGroup.HasElements )
                        {
                            foreach (XElement eMember in eGroup.Elements())
                            {
                                // Only act on 'Member' elements
                                if (!eMember.Name.LocalName.Equals(_XML_Node_Member))
                                    continue;
                                // Get action for Member
                                memberAction = Action( eMember );
                                if ( memberAction != XmlAction.Remove )
                                {
                                    _xmlProcessLog.Add("Add " + eMember.Value + " to " + _storeName + "." + eGroup.Attribute(_XML_Attrib_Name).Value);
                                    if (!AddGroupMember(eGroup.Attribute(_XML_Attrib_Name).Value, eMember.Value))
                                    {
                                        _xmlProcessError.Add("Failed to add " + eMember.Value + " to " + _storeName + "." + eGroup.Attribute(_XML_Attrib_Name).Value);
                                    }
                                }
                            }
                        }

                        break;
                    case XmlAction.Remove:
                        // Remove this customer group.
                        _xmlProcessLog.Add("Delete " + _storeName + " group '" + eGroup.Attribute(_XML_Attrib_Name).Value + "'.");
                        DeleteGroup(eGroup.Attribute(_XML_Attrib_Name).Value);
                        break;
                    case XmlAction.NoOp:
                    case XmlAction.Update:
                        // See if any group members are added or deleted.

                        // Are there Members to add/remove to/from this Group?
                        if (eGroup.HasElements)
                        {
                            foreach (XElement eMember in eGroup.Elements())
                            {
                                // Only act on 'Group' elements
                                if (!eMember.Name.LocalName.Equals(_XML_Node_Member))
                                    continue;
                                // Get action for Member
                                memberAction = Action(eMember);
                                if (memberAction == XmlAction.Remove)
                                {
                                    _xmlProcessLog.Add("Remove " + eMember.Value + " from " + _storeName + "." + eGroup.Attribute(_XML_Attrib_Name).Value);
                                    RemoveGroupMember( eGroup.Attribute( _XML_Attrib_Name ).Value, eMember.Value );
                                }
                                else if (memberAction == XmlAction.Add)
                                {
                                    _xmlProcessLog.Add("Add " + eMember.Value + " to " + _storeName + "." + eGroup.Attribute(_XML_Attrib_Name).Value);
                                    if ( !AddGroupMember( eGroup.Attribute( _XML_Attrib_Name ).Value, eMember.Value ) )
                                    {
                                        _xmlProcessError.Add("Failed to add " + eMember.Value + " to " + _storeName + "." + eGroup.Attribute(_XML_Attrib_Name).Value);
                                    }
                                }
                            }
                        }
                        break;
                }

            }
        }

        private void XmlProcessApplications(XmlReader reader)
        {
            XmlAction action = XmlAction.NoOp;
            XmlAction operationAction = XmlAction.NoOp;
            XElement parentNode = (XElement)XNode.ReadFrom(reader);
            foreach (XElement eApplication in parentNode.Elements())
            {
                // Only act on 'Application' elements
                if (!eApplication.Name.LocalName.Equals("Application"))
                    continue;
                // Does this application have an action?
                action = Action(eApplication);
                switch (action)
                {
                    case XmlAction.Add:
                        // Add this customer application.
                        _xmlProcessLog.Add("Create " + _storeName + " application '" + eApplication.Attribute(_XML_Attrib_Name).Value + "'.");
                        AzManCreateApplication( eApplication.Attribute( _XML_Attrib_Name ).Value, "XML Insert" );

                        // Are there Operation to add to this Application?
                        if (eApplication.HasElements)
                        {
                            foreach (XElement eOperation in eApplication.Elements())
                            {
                                // Only act on 'Operation' elements
                                if (!eOperation.Name.LocalName.Equals(_XML_Node_Operation))
                                    continue;
                                // Get action for Operation
                                operationAction = Action(eOperation);
                                if (operationAction != XmlAction.Remove)
                                {
                                    _xmlProcessLog.Add("Add " + eOperation.Value + " to " + _storeName + "." + eApplication.Attribute(_XML_Attrib_Name).Value);
                                    if ( AzManCreateOperation(eApplication.Attribute(_XML_Attrib_Name).Value, eOperation.Value, "XML Insert" ) == null )
                                    {
                                        _xmlProcessError.Add("Failed to add " + eOperation.Value + " to " + _storeName + "." + eApplication.Attribute(_XML_Attrib_Name).Value);
                                    }
                                }
                            }
                        }

                        break;
                    case XmlAction.Remove:
                        // Remove this customer application.
                        _xmlProcessLog.Add("Delete " + _storeName + " application '" + eApplication.Attribute(_XML_Attrib_Name).Value + "'.");
                        AzManDeleteApplication(eApplication.Attribute(_XML_Attrib_Name).Value);
                        break;
                    case XmlAction.NoOp:
                    case XmlAction.Update:
                        // See if any application operations are added or deleted.

                        // Are there Members to add/remove to/from this Group?
                        if (eApplication.HasElements)
                        {
                            foreach (XElement eOperation in eApplication.Elements())
                            {
                                // Only act on 'Group' elements
                                if (!eOperation.Name.LocalName.Equals(_XML_Node_Operation))
                                    continue;
                                // Get action for Member
                                operationAction = Action(eOperation);
                                if (operationAction == XmlAction.Remove)
                                {
                                    _xmlProcessLog.Add("Remove " + eOperation.Value + " from " + _storeName + "." + eApplication.Attribute(_XML_Attrib_Name).Value);
                                    AzManDeleteOperation( eApplication.Attribute( _XML_Attrib_Name ).Value, eOperation.Value );
                                }
                                else if (operationAction == XmlAction.Add)
                                {
                                    _xmlProcessLog.Add("Add " + eOperation.Value + " to " + _storeName + "." + eApplication.Attribute(_XML_Attrib_Name).Value);
                                    if ( AzManCreateOperation(eApplication.Attribute(_XML_Attrib_Name).Value, eOperation.Value, "XML Insert" ) == null )
                                    {
                                        _xmlProcessError.Add("Failed to add " + eOperation.Value + " to " + _storeName + "." + eApplication.Attribute(_XML_Attrib_Name).Value);
                                    }
                                }
                            }
                        }
                        break;
                }

            }
        }
        
        private void XmlProcessAuthTree(XmlReader reader)
        {
            XmlAction action = XmlAction.NoOp;
            XElement parentNode = (XElement)XNode.ReadFrom(reader);
            foreach (XElement eAuthItem in parentNode.Elements())
            {
                // Only act on 'AuthItem' elements
                if (!eAuthItem.Name.LocalName.Equals(_XML_Node_AuthItem))
                    continue;

                // Does this AuthItem have an action?
                action = Action(eAuthItem);

                switch (action)
                {
                    case XmlAction.Add:
                        SetAuthText( eAuthItem );
                        // Are there child AuthItems?
                        if ( eAuthItem.HasElements )
                        {
                            foreach (XElement eChildAuthItem in eAuthItem.Elements())
                            {
                                // Only act on 'AuthItem' elements
                                if (!eChildAuthItem.Name.LocalName.Equals(_XML_Node_AuthItem))
                                    continue;

                                ProcessRoleAuthItems( eAuthItem.Attribute(_XML_Attrib_Name).Value, null, eChildAuthItem, true);
                            }
                        }
                        break;
                    case XmlAction.Remove:
                        RemoveAuthText( eAuthItem );
                        break;
                    case XmlAction.NoOp:
                        // Are there child AuthItems?
                        if (eAuthItem.HasElements)
                        {
                            foreach (XElement eChildAuthItem in eAuthItem.Elements())
                            {
                                // Only act on 'AuthItem' elements
                                if (!eChildAuthItem.Name.LocalName.Equals(_XML_Node_AuthItem))
                                    continue;

                                ProcessRoleAuthItems(eAuthItem.Attribute(_XML_Attrib_Name).Value, null, eChildAuthItem, false);
                            }
                        }
                        break;
                }

            }
        }


        private XmlAction Action(XElement element)
        {
            XmlAction action = XmlAction.NoOp;

            if ( element.HasAttributes )
            {
                XAttribute attribute = element.Attribute(_XML_Attrib_Action);
                if ( attribute != null )
                {
                    if (attribute.Value.Equals("Add", StringComparison.CurrentCultureIgnoreCase)) 
                        action = XmlAction.Add;
                    else if (attribute.Value.Equals("Delete", StringComparison.CurrentCultureIgnoreCase))
                        action = XmlAction.Remove;
                    else if (attribute.Value.Equals("Update", StringComparison.CurrentCultureIgnoreCase))
                        action = XmlAction.Remove;
                    else if (attribute.Value.Equals("Clear", StringComparison.CurrentCultureIgnoreCase))
                        action = XmlAction.Clear;
                }
            }

            return action;
        }


        private void ProcessRoleAuthItems(string applicationName, IAzManItem parentRole, XElement element, bool defaultToAdd)
        {
            string roleName = element.Attribute( _XML_Attrib_Name ).Value;

            // Get application
            if (!_store.Applications.ContainsKey(applicationName))
            {
                _xmlProcessError.Add("ProcessRoleAuthItems - Application '" + applicationName + "' does not exist.");
                return;
            }

            // Get Role

            IAzManItem role;

            // Determine the Action
            XmlAction action = Action( element );

            switch (action)
            {
                case XmlAction.Add:
                    // Get/create role.
                    role = AzManCreateRole( applicationName, roleName, "XML Insert" );
                    _xmlProcessLog.Add("Add role '" + role.Name + "' to " + _storeName + ", application '" + applicationName + "'.");
                    SetAuthText( role, element.Attribute( Constants.Menu.AuthText ).Value );
                    AzManSetItemParent(parentRole, role);
                    break;
                case XmlAction.Remove:
                    // Remove the Role
                     role = AzManGetRole(applicationName, roleName);
                    if (role == null)
                    {
                        _xmlProcessError.Add("ProcessRoleAuthItems - Application '" + applicationName + "', Role '" + roleName + "' does not exist.");
                        return;
                    }
                    _xmlProcessLog.Add("Remove role '" + role.Name + "' from " + _storeName + ", application '" + applicationName + "'.");
                    RemoveAuthText( role );
                    AzManDeleteRole( applicationName, role.Name );
                    break;
                case XmlAction.NoOp:
                    if ( defaultToAdd )
                    {
                        // Get/create role.
                        role = AzManCreateRole( applicationName, roleName, "XML Insert" );
                        SetAuthText(role, element.Attribute(Constants.Menu.AuthText).Value);
                        AzManSetItemParent(parentRole, role);
                    }
                    break;
            }
            role = AzManGetRole(applicationName, roleName);

            // If action was Add or NoOp see if there are child AuthItems to process
            if ( (action == XmlAction.Add || action == XmlAction.NoOp) && role != null )
            {
                // Check child nodes.
                if ( element.HasElements )
                {
                    foreach (XElement eChild in element.Elements())
                    {
                        // 'AuthItem' elements
                        if ( eChild.Name.LocalName.Equals( _XML_Node_AuthItem ) )
                            ProcessRoleAuthItems( applicationName, role, eChild, defaultToAdd );

                        // 'Operations' elements
                        if (eChild.Name.LocalName.Equals(_XML_Node_Operations ))
                            ProcessRoleAuthOperations(applicationName, role, eChild, defaultToAdd);

                        // 'Authorizations' elements
                        if (eChild.Name.LocalName.Equals(_XML_Node_Authorizations))
                            ProcessRoleAuthorizations(applicationName, role, eChild, defaultToAdd);
                    }
                }
            }

        }

        /// <summary>
        /// Process the Operations that are associated with this AuthItem (Role)
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="role"></param>
        /// <param name="element"></param>
        private void ProcessRoleAuthOperations(string applicationName, IAzManItem role, XElement element, bool defaultToAdd)
        {
            // Get application
            if (!_store.Applications.ContainsKey(applicationName))
            {
                _xmlProcessError.Add("ProcessRoleAuthOperations - Application '" + applicationName + "' does not exist.");
                return;
            }

            foreach (XElement eOperation in element.Elements())
            {
                // Only act on 'Operation' elements
                if (!eOperation.Name.LocalName.Equals(_XML_Node_Operation))
                    continue;

                // This should relate to an 'Operation' under this 'Application'
                IAzManItem operation = AzManGetOperation( applicationName, eOperation.Value );

                if ( operation == null )
                {
                    _xmlProcessError.Add("ProcessRoleAuthOperations - Application '" + applicationName + "' does not have operation '" + eOperation.Value + "'.");
                    continue;
                }

                // Determine the Action
                XmlAction action = defaultToAdd ? XmlAction.Add : Action(eOperation);

                switch (action)
                {
                    case XmlAction.Add:
                        _xmlProcessLog.Add("Add operation '" + operation.Name + "' to role '" + role.Name + "' in " + _storeName + ", application '" + applicationName + "'.");
                        AzManSetItemParent(role, operation);
                        break;
                    case XmlAction.Remove:
                        _xmlProcessLog.Add("Remove operation '" + operation.Name + "' from role '" + role.Name + "' in " + _storeName + ", application '" + applicationName + "'.");
                        role.RemoveMember(operation);
                        break;
                }
            }
        }

        private void ProcessRoleAuthorizations(string applicationName, IAzManItem role, XElement element, bool defaultToAdd)
        {
            // Get application
            if (!_store.Applications.ContainsKey(applicationName))
            {
                _xmlProcessError.Add("ProcessRoleAuthorizations - Application '" + applicationName + "' does not exist.");
                return;
            }

            foreach (XElement eAuthorize in element.Elements())
            {
                // Only act on 'Authorize' elements
                if (!eAuthorize.Name.LocalName.Equals(_XML_Node_Authorize))
                    continue;

                // Determine the Action
                XmlAction action = defaultToAdd ? XmlAction.Add : Action(eAuthorize);



                IAzManStoreGroup storeGroup = GetStoreGroup( _storeName, eAuthorize.Value );

                if ( storeGroup == null )
                {
                    _xmlProcessError.Add("ProcessRoleAuthorizations - Application '" + applicationName + "' does not have group '" + eAuthorize.Value + "'.");
                    continue;
                }

                // Get 
                IAzManAuthorization[] authorizations = role.GetAuthorizationsOfMember(storeGroup.SID);

                switch (action)
                {
                    case XmlAction.Add:
                        _xmlProcessLog.Add("Add authorization for group '" + eAuthorize.Value + "' to role '" + role.Name + "' in " + _storeName + ", application '" + applicationName + "'.");
                        // See if this group is already has an authorization object.
                        if (authorizations != null && authorizations.Any())
                        {
                            authorizations[0].Update(authorizations[0].Owner, storeGroup.SID,
                                WhereDefined.Store, AuthorizationType.Allow, null, null);
                        }
                        else
                        {
                            // Set authorization on this role
                            role.CreateAuthorization(SqlAzManSID.NewSqlAzManSid(), WhereDefined.Local,
                                storeGroup.SID, WhereDefined.Store, AuthorizationType.Allow, null, null);
                        }
                        break;
                    case XmlAction.Remove:
                        // If there is an authorization object set it as Neutral
                        if (authorizations != null && authorizations.Any())
                        {
                            _xmlProcessLog.Add("Remove authorization for group '" + eAuthorize.Value + "' from role '" + role.Name + "' in " + _storeName + ", application '" + applicationName + "'.");
                            authorizations[0].Update(authorizations[0].Owner, storeGroup.SID,
                                WhereDefined.Store, AuthorizationType.Neutral, null, null);
                        }
                        break;
                }
            }
        }


        private void RemoveAuthText(XElement element)
        {
            string applicationName = element.Attribute(_XML_Attrib_Name).Value;

            // Get application
            if (!_store.Applications.ContainsKey(applicationName))
            {
                _xmlProcessError.Add("RemoveAuthText - Application '" + applicationName + "' does not exist.");
                return;
            }

            // Does this application have the authtext?
            if (_store.Applications[applicationName].Attributes.ContainsKey(Constants.Menu.AuthText))
            {
                IAzManAttribute<IAzManApplication> attrib = _store.Applications[applicationName].Attributes[Constants.Menu.AuthText];
                attrib.Delete();
                _xmlProcessLog.Add("Remove authtext from " + _storeName + "." + applicationName);
            }
        }


        private void SetAuthText(XElement element)
        {
            string applicationName = element.Attribute( _XML_Attrib_Name ).Value;
            string authText = element.Attribute(Constants.Menu.AuthText).Value;

            // Get application
            if ( !_store.Applications.ContainsKey( applicationName ) )
            {
                _xmlProcessError.Add("SetAuthText - Application '" + applicationName + "' does not exist.");
                return;
            }

            // Does this application have the authtext?
            if ( _store.Applications[applicationName].Attributes.ContainsKey( Constants.Menu.AuthText ) )
            {
                _store.Applications[applicationName].Attributes[Constants.Menu.AuthText].Update(Constants.Menu.AuthText, authText);
            }
            else
            {
                _store.Applications[applicationName].CreateAttribute( Constants.Menu.AuthText, authText );
            }

            _xmlProcessLog.Add("Add authtext '" + authText + "' to " + _storeName + "." + applicationName);

        }

        private void SetAuthText(string applicationName, XElement element)
        {
            string roleName = element.Attribute(_XML_Attrib_Name).Value;
            string authText = element.Attribute(Constants.Menu.AuthText).Value;

            // Get application
            if (!_store.Applications.ContainsKey(applicationName))
            {
                _xmlProcessError.Add("SetAuthText - Application '" + applicationName + "' does not exist.");
                return;
            }

            // Does this application have this Role?
            if (_store.Applications[applicationName].Items.ContainsKey(roleName))
            {
                SetAuthText( _store.Applications[applicationName].Items[roleName], authText );
            }
            else
            {
                _xmlProcessError.Add("SetAuthText - Role '" + roleName + "' does not exist.");
            }
        }

        private void SetAuthText(IAzManItem item, string authText)
        {
            if (item == null)
                return;

            // Does this item have the authtext?
            if (item.Attributes.ContainsKey(Constants.Menu.AuthText))
            {
                item.Attributes[Constants.Menu.AuthText].Update(Constants.Menu.AuthText, authText);
            }
            else
            {
                item.CreateAttribute(Constants.Menu.AuthText, authText);
            }

            _xmlProcessLog.Add("Add authtext '" + authText + "' to " + item.Application.Store.Name + "." + item.Application.Name + "." + item.Name);
        }

        private void RemoveAuthText(IAzManItem item)
        {
            if ( item == null )
                return;

            // Does this item have the authtext?
            if (item.Attributes.ContainsKey(Constants.Menu.AuthText))
            {
                IAzManAttribute<IAzManItem> attrib = item.Attributes[Constants.Menu.AuthText];
                attrib.Delete();
                _xmlProcessLog.Add("Remove authtext from " + item.Application.Store.Name + "." + item.Application.Name + "." + item.Name);
            }
        }


        #endregion

        #region Configuration Methods - Private (Process Menu Tree)


        private void XmlProcessMenuTree(XmlReader reader)
        {
            int orderBy = _MenuOrderStep;
            XmlAction action = XmlAction.NoOp;
            XElement parentNode = (XElement)XNode.ReadFrom(reader);

            // Check for "Clear" action at top level.
            if ( Action( parentNode ) == XmlAction.Clear )
            {
                // Clear entire menu.
                _xmlProcessLog.Add("Clear the entire menu.");
                ClearMenu();
            }


            foreach (XElement eMenuItem in parentNode.Elements())
            {
                // Only act on 'MenuItem' elements
                if (!eMenuItem.Name.LocalName.Equals(_XML_Node_MenuItem))
                    continue;

                // Does this MenuItem have an action?
                action = Action(eMenuItem);

                switch (action)
                {
                    case XmlAction.Add:
                        ProcessTopLevelMenuItem(eMenuItem, orderBy, true);
                        break;
                    case XmlAction.Remove:
                        RemoveTopLevelMenuItem(eMenuItem);
                        break;
                    case XmlAction.NoOp:
                        ProcessTopLevelMenuItem(eMenuItem, orderBy, false);
                        break;
                }

                orderBy += _MenuOrderStep;
            }
        }


        private void ClearMenu()
        {
            // For each application in store...
            foreach (var kvp in _store.Applications)
            {
                string applicationName = kvp.Key;
                IAzManApplication application = kvp.Value;
                List<IAzManItem> items = new List<IAzManItem>();

                foreach (var kvpTask in application.Items.Where( m => m.Value.ItemType == ItemType.Task ))
                {
                    // Remove any attributes.
                    AzManRemoveAnyAttributes(kvpTask.Value);

                    // If it has an Operation member, remove it.
                    AzManRemoveAnyMembers(kvpTask.Value);

                    // If it is a member of another task, break connection.
                    IAzManItem[] parents = kvpTask.Value.GetItemsWhereIAmAMember();
                    if ( parents != null )
                    {
                        foreach (var azManItem in parents)
                        {
                            azManItem.RemoveMember(kvpTask.Value);
                        }
                    }

                    items.Add(kvpTask.Value);
                }

                // Now delete the tasks.
                foreach (var azManItem in items)
                {
                    azManItem.Delete();
                }
            }
        }


        private void ProcessTopLevelMenuItem(XElement topLevelMenu, int orderBy, bool defaultToAdd)
        {
            // This should be pointing to an application...
            string applicationName = topLevelMenu.Attribute(_XML_Attrib_Name).Value;

            // Get the application
            IAzManApplication application = AzManGetApplication( applicationName );
            if ( application == null )
            {
                _xmlProcessError.Add("ProcessTopLevelMenuItem - Application '" + applicationName + "' does not exist.");
                return;
            }

            // Get/create a 'Task' with applicationName
            IAzManItem task = AzManGetTask(applicationName, applicationName);
            if ( task == null )
            {
                task = AzManCreateTask( applicationName, applicationName, "XML Insert" );
                if ( task == null )
                {
                    _xmlProcessError.Add("ProcessTopLevelMenuItem - Unable to create Task with same name as Application ('" + applicationName + "').");
                    return;
                }
                _xmlProcessLog.Add("Add top level menu item " + application.Store.Name + "." + application.Name + "." + applicationName);
            }

            // Update menutext and tooltip
            foreach (var menuAttribute in topLevelMenu.Attributes())
            {
                // Skip the _XML_Attrib_Name attribute, the Action attribute and the menuorder attribute.
                // Skipping menu order attribute because it is set just below.
                if (menuAttribute.Name.LocalName == _XML_Attrib_Name || menuAttribute.Name.LocalName == _XML_Attrib_Action) // || menuAttribute.Name.LocalName == Constants.Menu.MenuOrder)
                    continue;

                // Add/Update the attribute to the task
                AzManAddAttribute(task, menuAttribute.Name.LocalName, menuAttribute.Value);
            }

            // If there is a Link element under topLevelMenu, add its attributes.
            // Walk the children.            
            foreach (XElement eLink in topLevelMenu.Elements())
            {
                if ( eLink.Name.LocalName == "Link" )
                {
                    XmlAction action = XmlAction.NoOp;
                    action = defaultToAdd ? XmlAction.Add : Action(eLink);

                    switch (action)
                    {
                        case XmlAction.Add:
                            AddLink( task, eLink );
                            break;
                        case XmlAction.Remove:
                            RemoveLink( task, eLink );
                            break;
                    }
                }
            }

            // Always update orderBy attribute
            AzManAddAttribute( task, Constants.Menu.MenuOrder, orderBy );

            // Walk the children.            
            int orderChildrenBy = _MenuOrderStep;
            foreach (XElement eMenuItem in topLevelMenu.Elements())
            {
                if ( eMenuItem.Name.LocalName != _XML_Node_MenuItem )
                    continue;

                ProcessMenuItem(applicationName, null, eMenuItem, orderChildrenBy, defaultToAdd);
                orderChildrenBy += _MenuOrderStep;
            }

        }

        private void AddLink(IAzManItem task, XElement eLink)
        {
            // This "Link" can be either a URL or an Application/Operation
            // If it is a URL 
            //   It can optionally have a newwindow attribute.
            // If it is an Application/Operation
            //   It can either be a membership relationship with an 'Operation'
            //   under this application or it can be simply an application and operation attribute

            // First, collect the attributes.
            Dictionary<string, string> attributes = new Dictionary<string, string>();

            foreach (var attribute in eLink.Attributes())
            {
                attributes.Add(attribute.Name.LocalName, attribute.Value);   
            }

            // Is it a URL?
            if (attributes.ContainsKey(_XML_Attrib_Url))
            {
                AzManAddAttribute(task, _XML_Attrib_Url, attributes[_XML_Attrib_Url]);
                if (attributes.ContainsKey(_XML_Attrib_NewWindow))
                {
                    AzManAddAttribute(task, _XML_Attrib_NewWindow, attributes[_XML_Attrib_NewWindow]);
                }
            }
            else
            {
                // Must contain both Application and Operation
                if (!attributes.ContainsKey(_XML_Attrib_Application) || !attributes.ContainsKey(_XML_Attrib_Operation))
                {
                    _xmlProcessError.Add("AddLink - Missing application or operation attribute.");
                    return;
                }

                // Is this task under this attributes["application"]?
                IAzManItem operation = AzManGetOperation(attributes[_XML_Attrib_Application], attributes[_XML_Attrib_Operation]);
                if (operation != null && task.Application.Equals(operation.Application))
                {
                    // Relate operation to task
                    AzManSetItemParent( task, operation );
                }
                else
                {
                    // Just add attributes
                    AzManAddAttribute(task, _XML_Attrib_Application, attributes[_XML_Attrib_Application]);
                    AzManAddAttribute(task, _XML_Attrib_Operation, attributes[_XML_Attrib_Operation]);
                }
                
            }


        }


        private void RemoveLink(IAzManItem task, XElement eLink)
        {
            // This "Link" can be either a URL or an Application/Operation
            // If it is a URL 
            //   It can optionally have a newwindow attribute.
            // If it is an Application/Operation
            //   It can either be a membership relationship with an 'Operation'
            //   under this application or it can be simply an application and operation attribute

            _xmlProcessLog.Add("Remove menu link from " + task.Name + ".");

            // Remove any attributes that form a link.
            AzManRemoveAttribute(task, Constants.Menu.Url);
            AzManRemoveAttribute(task, Constants.Menu.Operation);
            AzManRemoveAttribute(task, Constants.Menu.Application);
            AzManRemoveAttribute(task, Constants.Menu.Target);
            AzManRemoveAttribute(task, Constants.Menu.NewWindow);

            // If task has a child Operation, remove it.
            AzManRemoveAnyMembers( task, ItemType.Operation );

        }



        private void ProcessMenuItem(string applicationName, IAzManItem parentTask, XElement eMenuItem, int orderBy, bool defaultToAdd)
        {
            string taskName = eMenuItem.Attribute(_XML_Attrib_Name).Value;

            // Get the application
            IAzManApplication application = AzManGetApplication(applicationName);
            if (application == null)
            {
                _xmlProcessError.Add("ProcessMenuItem - Application '" + applicationName + "' does not exist.");
                return;
            }

            XmlAction action = Action( eMenuItem );
            action = action == XmlAction.NoOp && defaultToAdd ? XmlAction.Add : action;

            IAzManItem task = null;

            switch (action)
            {
                case XmlAction.Add:
                    // Get/Create task
                    task = AzManCreateTask(applicationName, eMenuItem.Attribute(_XML_Attrib_Name).Value, "XML Insert");
                    if ( task == null )
                    {
                        _xmlProcessError.Add("ProcessMenuItem - Unable to create Task named '" + eMenuItem.Attribute(_XML_Attrib_Name).Value + "'.");
                        return;
                    }
                    _xmlProcessLog.Add("Add child menu " +  (parentTask != null ? parentTask.Name : "") + "." + task.Name);

                    foreach (var menuAttribute in eMenuItem.Attributes())
                    {
                        // Skip the _XML_Attrib_Name attribute
                        if (menuAttribute.Name.LocalName == _XML_Attrib_Name || menuAttribute.Name.LocalName == _XML_Attrib_Action)
                            continue;

                        // Add the attribute to the task
                        AzManAddAttribute(task, menuAttribute.Name.LocalName, menuAttribute.Value);
                    }

                    // If there is a Link element under eMenuItem, add its attributes.
                    // Walk the children.            
                    foreach (XElement eLink in eMenuItem.Elements())
                    {
                        if ( eLink.Name.LocalName == "Link" )
                        {
                            AddLink( task, eLink );
                            break;
                        }
                    }

                    // Is there a parentTask?
                    if (parentTask != null)
                        AzManSetItemParent(parentTask, task);

                    break;
                case XmlAction.Remove:
                    RemoveMenuItem( applicationName, eMenuItem );
                    return;
                case XmlAction.NoOp:
                    // Get task
                    task = AzManGetTask(applicationName, eMenuItem.Attribute(_XML_Attrib_Name).Value);
                    break;
            }

            if ( task == null )
            {
                _xmlProcessError.Add("ProcessMenuItem - Task '" + eMenuItem.Attribute(_XML_Attrib_Name).Value + "' under '" + applicationName + "' does not exist.");
                return;
            }

            // Always update orderBy attribute
            AzManAddAttribute(task, Constants.Menu.MenuOrder, orderBy);

            // Walk the children.            
            int orderChildrenBy = _MenuOrderStep;
            foreach (XElement eChildMenuItem in eMenuItem.Elements())
            {
                if (eChildMenuItem.Name.LocalName != _XML_Node_MenuItem)
                    continue;

                ProcessMenuItem(applicationName, task, eChildMenuItem, orderChildrenBy, defaultToAdd);
                orderChildrenBy += _MenuOrderStep;
            }

        }


        private void RemoveTopLevelMenuItem(XElement topLevelMenu)
        {
            string applicationName = topLevelMenu.Attribute( _XML_Attrib_Name ).Value;

            // Walk the children looking for MenuItem nodes
            foreach (XElement eMenuItem in topLevelMenu.Elements())
            {
                if (eMenuItem.Name.LocalName == _XML_Node_MenuItem)
                {
                    RemoveMenuItem(applicationName, eMenuItem);
                }
            }

            // Get the Task.  Remove any attributes.
            IAzManItem task = AzManGetTask(applicationName, applicationName);
            if (task == null)
                return;

            // Remove any attributes.
            AzManRemoveAnyAttributes(task);

            // If it has an Operation member, remove it.
            AzManRemoveAnyMembers(task);

            // Now delete the task.
            task.Delete();

        }

        private void RemoveMenuItem(string applicationName, XElement eMenuItem)
        {
            // Walk the children looking for MenuItem nodes
            foreach (XElement eChildMenuItem in eMenuItem.Elements())
            {
                if (eChildMenuItem.Name.LocalName == _XML_Node_MenuItem)
                {
                    RemoveMenuItem(applicationName, eChildMenuItem);
                }
            }

            // Get the Task.  Remove any attributes.
            IAzManItem task = AzManGetTask( applicationName, eMenuItem.Attribute( _XML_Attrib_Name ).Value );
            if ( task == null )
                return;

            // Remove any attributes.
            AzManRemoveAnyAttributes(task);

            // If it has an Operation member, remove it.
            AzManRemoveAnyMembers( task );

            // If it is a member of another task, break connection.
            IAzManItem[] parents = task.GetItemsWhereIAmAMember();
            if ( parents != null )
            {
                foreach (var azManItem in parents)
                {
                    azManItem.RemoveMember(task);
                }
            }

            // Now delete the task.
            task.Delete();
        }


        #endregion
    }
}
