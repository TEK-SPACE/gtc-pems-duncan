using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace RBACToolbox
{
    public class ApplicationLogonResponse
    {
        public string SessionId = string.Empty;
        public string Username = string.Empty;
        public string DomainUsername = string.Empty;
        public string FullName = string.Empty;
        public string ErrorMsg = string.Empty;
        public int RbacUserId = 0;
        public int ActiveDuncanCustomerID = 0;
        public List<RBACItemInfo> GrantedItems = new List<RBACItemInfo>();
        public List<RBACCustomerInfo> GrantedCustomers = new List<RBACCustomerInfo>();

        public ApplicationLogonResponse()
        {
        }

        public bool HasGrantedItem(string itemName)
        {
            RBACItemInfo grantedItem = null;
            RBACItemInfoPredicate rolePredicate = new RBACItemInfoPredicate(itemName, RBACItemType.Role);
            grantedItem = this.GrantedItems.Find(rolePredicate.CompareByNameAndItemType);
            if (grantedItem != null)
                return true;
            else
                return false;
        }

        public bool HasWriteAccessForCustomer(int CustomerID)
        {
            // Always return true if we are working with offline/local file envrionment
            if (this.OfflineAndLocalFilesOnly == true)
                return true;

            RBACCustomerInfoPredicate customerPredicate = new RBACCustomerInfoPredicate(CustomerID);
            RBACToolbox.RBACCustomerInfo grantedCustomer = this.GrantedCustomers.Find(customerPredicate.CompareByCustomerID);
            if (grantedCustomer != null)
            {
                RBACItemInfo grantedItem = null;

                // User access to customer, so we will assume they have read/write access also.  However, if they have the 
                // "ReadOnlyAccess" role, then we will return false

                RBACItemInfoPredicate rolePredicate = new RBACItemInfoPredicate("ReadOnlyAccess", RBACItemType.Role);
                grantedItem = this.GrantedItems.Find(rolePredicate.CompareByNameAndItemType);
                if (grantedItem != null)
                    return false;
                else
                    return true;
            }
            else
            {
                // User doesn't have access to customer at all (not even to read)
                return false;
            }
        }

        public bool OfflineAndLocalFilesOnly = false;
    }

    public class RBACItemInfo
    {
        [XmlAttribute("ItemID")]
        public int ItemID;

        [XmlAttribute("ItemName")]
        public string ItemName;

        [XmlAttribute("Description")]
        public string Description;

        [XmlAttribute("ItemType")]
        public RBACItemType ItemType;

        public RBACItemInfo()
        {
        }

        public override string ToString()
        {
            if (ItemName.StartsWith("Customer:"))
            {
                return Description + " [" + ItemName.Substring("Customer:".Length) + "]";
            }
            else
            {
                return ItemName;
            }
        }
    }

    public enum RBACItemType
    {
        Role = 0,
        Task = 1,
        Operation = 2,
    }

    public class RBACItemInfoPredicate
    {
        private string _CompareItemName;
        private int _CompareItemID;
        private RBACItemType _CompareItemType;

        // Constructor used when comparing names
        public RBACItemInfoPredicate(string CompareName)
        {
            _CompareItemName = CompareName;
        }

        // Constructor used when comparing objects on both the Name and ItemType
        public RBACItemInfoPredicate(string CompareName, RBACItemType CompareItemType)
        {
            _CompareItemName = CompareName;
            _CompareItemType = CompareItemType;
        }

        // Constructor used when comparing Item IDs
        public RBACItemInfoPredicate(int CompareItemID)
        {
            _CompareItemID = CompareItemID;
        }

        // Compare by Name (Case-Sensitive)
        public bool CompareByName(RBACItemInfo pObject)
        {
            return (System.String.Compare(pObject.ItemName, _CompareItemName, false) == 0);
        }

        // Compare by Name (Case-Insensitive)
        public bool CompareByName_CaseInsensitive(RBACItemInfo pObject)
        {
            return (System.String.Compare(pObject.ItemName, _CompareItemName, true) == 0);
        }

        // Compare by both Name and ItemType
        public bool CompareByNameAndItemType(RBACItemInfo pObject)
        {
            bool result = (System.String.Compare(pObject.ItemName, _CompareItemName, false) == 0);
            if (result == true)
            {
                result = (pObject.ItemType.CompareTo(_CompareItemType) == 0);
            }
            return result;
        }

        // Compare by ID
        public bool CompareByItemID(RBACItemInfo pObject)
        {
            return (pObject.ItemID == _CompareItemID);
        }
    }

    public class RBACCustomerInfo
    {
        [XmlAttribute("RBACItemId")]
        public int RBACItemId;

        [XmlAttribute("RBACItemName")]
        public string RBACItemName;

        [XmlAttribute("CustomerName")]
        public string CustomerName;

        [XmlAttribute("CustomerId")]
        public int CustomerId;

        [XmlAttribute("SFParkFunctionality")]
        public bool SFParkFunctionality = false;

        public RBACCustomerInfo()
        {
        }

        public override string ToString()
        {
            return CustomerName;
        }
    }

    public class RBACCustomerInfoPredicate
    {
        private string _CompareCustomerName;
        private int _CompareID;

        // Constructor used when comparing names
        public RBACCustomerInfoPredicate(string CompareName)
        {
            _CompareCustomerName = CompareName;
        }

        // Constructor used when comparing Item IDs
        public RBACCustomerInfoPredicate(int CompareID)
        {
            _CompareID = CompareID;
        }

        // Compare by Name (Case-Sensitive)
        public bool CompareByName(RBACCustomerInfo pObject)
        {
            return (System.String.Compare(pObject.CustomerName, _CompareCustomerName, false) == 0);
        }

        // Compare by Name (Case-Insensitive)
        public bool CompareByName_CaseInsensitive(RBACCustomerInfo pObject)
        {
            return (System.String.Compare(pObject.CustomerName, _CompareCustomerName, true) == 0);
        }

        // Compare by RBAC ID
        public bool CompareByRbacID(RBACCustomerInfo pObject)
        {
            return (pObject.RBACItemId == _CompareID);
        }

        // Compare by Customer ID
        public bool CompareByCustomerID(RBACCustomerInfo pObject)
        {
            return (pObject.CustomerId == _CompareID);
        }
    }

    public class SerializationHelper
    {
        public static string SerializeObjectAsXML(object objectToSerialize, string forcedNameSpace)
        {
            XmlSerializer serializer = null;
            if (!string.IsNullOrEmpty(forcedNameSpace))
                serializer = new XmlSerializer(objectToSerialize.GetType(), forcedNameSpace);
            else
                serializer = new XmlSerializer(objectToSerialize.GetType());

            XmlWriterSettings loXMLSettings = new XmlWriterSettings();
            loXMLSettings.Indent = true;
            loXMLSettings.Encoding = System.Text.UTF8Encoding.UTF8;
            loXMLSettings.OmitXmlDeclaration = true;

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlWriter xtWriter = XmlWriter.Create(stream, loXMLSettings);

            if (string.IsNullOrEmpty(forcedNameSpace))
            {
                // Create our own namespaces for the output, which effectively omits all namespaces
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(xtWriter, objectToSerialize, ns);
            }
            else
            {
                serializer.Serialize(xtWriter, objectToSerialize);
            }
            xtWriter.Flush();
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            // Now load the serialized XML into an XMLDocument for some post-processing
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(stream);

            // Let's remove the unwanted "xsd" and "xsi" namespaces from attributes of first child node
            if (xmlDoc.FirstChild.Attributes != null)
            {
                for (int loIdx = xmlDoc.FirstChild.Attributes.Count - 1; loIdx >= 0; loIdx--)
                {
                    XmlAttribute nextAttr = xmlDoc.FirstChild.Attributes[loIdx];
                    if ((nextAttr.Name == "xmlns:xsd") || (nextAttr.Name == "xmlns:xsi"))
                    {
                        xmlDoc.FirstChild.Attributes.Remove(nextAttr);
                    }
                }
            }

            // Capture the outer XML of document as the result 
            string ObjAsString = xmlDoc.OuterXml;

            // Cleanup resources
            xtWriter.Close();
            stream.Close();
            stream.Dispose();

            // Return final result, which is a string representing the serialized XML of object
            return ObjAsString;
        }

        public static object DeSerializeObject(string objectAsString, Type objectType)
        {
            // This is the object we will return (null if fail).
            object objectToReturn = null;

            try
            {
                // Deserialize the object.
                StringReader reader = new StringReader(objectAsString);
                XmlSerializer serializer = new XmlSerializer(objectType);
                objectToReturn = serializer.Deserialize(reader);
                reader.Close();
            }
            catch
            {
            }

            // Return object we created.
            return objectToReturn;
        }
    }
}
