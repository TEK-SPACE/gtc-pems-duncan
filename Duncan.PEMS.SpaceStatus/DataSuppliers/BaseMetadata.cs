using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;

using System.Data.SQLite; // If not installed, run "\3rd Party Components\SQLite\SQLite-1.0.66.0-setup.exe"


namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{

    #region Metadata Objects
    public class MetadataBaseObject : IComparable<MetadataBaseObject>, IEquatable<MetadataBaseObject>
    {
        #region Properties and Members
        internal string _Name = "";
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [XmlIgnoreAttribute]
        public MetadataBaseObject Parent;
        #endregion

        /// <summary>
        /// Determines whether two TObjBase objects have the same name.
        /// (For supporting the IEquatable interface)
        /// </summary>
        public virtual bool Equals(MetadataBaseObject obj)
        {
            return _Name.Equals(obj.Name);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// (For supporting the IComparable interface)
        /// </summary>
        public virtual int CompareTo(MetadataBaseObject obj)
        {
            return _Name.CompareTo(obj.Name);
        }
    }

    public class IndexMetadata : MetadataBaseObject
    {
        protected string _Field = "";

        protected List<int> _IndexFieldNos = new List<int>();
        public List<int> fIndexFieldNos
        {
            get { return _IndexFieldNos; }
            set { _IndexFieldNos = value; }
        }

        public TableMetadata ParentTable { get { return (Parent as TableMetadata); } }

        public bool IsPrimaryKey = false;

        public bool IsUniqueConstraint = false;

        public IndexMetadata(TableMetadata parent, String indexName, String[] columnNames, bool isPrimary, bool isUnique)
            : base()
        {
            if (parent.FindIndexByName(indexName) != null)
                return; // this index already exists. Don't add it again.

            Parent = parent;
            Name = indexName;
            int loFldNdx;
            // add the fields defined
            foreach (String loColumnName in columnNames)
            {
                // is this field defined in the table? Can't be indexed if it isn't.
                if ((loFldNdx = ParentTable.GetFldNo(loColumnName)) < 0)
                    continue;
                // it is, so add it index.
                _IndexFieldNos.Add(loFldNdx);
            }

            // don't add ourself to our parent if we are empty. 
            if (_IndexFieldNos.Count == 0)
                return;
            IsPrimaryKey = isPrimary;
            IsUniqueConstraint = isUnique;
            // finally, add ourself to our parent.
            ParentTable.Indexes.Add(this);
        }

        public String ColumnNames(int indexColumnNumber)
        {
            return ParentTable.Fields[_IndexFieldNos[indexColumnNumber]].Name;
        }

        public int GetIndexFieldCnt()
        {
            return fIndexFieldNos.Count;
        }

        public int GetTableFieldIdxForIndexFieldNo(int iFieldNdx)
        {
            if (fIndexFieldNos.Count == 0)
                return -1;
            return fIndexFieldNos[iFieldNdx];
        }

        public IndexMetadata()
            : base()
        {
        }
    }

    public class TableMetadata : MetadataBaseObject
    {
        #region Properties and Members
        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public String fTblName;

        [XmlIgnoreAttribute] // We don't want the following public property/member serialized in XML
        public int fPrimaryKeyFldCnt;	    // number of fields in the primary key, i.e., depth of sorting 
        #endregion

        protected int _PrimaryKeyFldCnt = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public int PrimaryKeyFldCnt
        {
            get { return _PrimaryKeyFldCnt; }
            set { _PrimaryKeyFldCnt = value; }
        }

        [XmlIgnore]
        public IndexMetadata PrimaryKey
        {
            get
            {
                foreach (IndexMetadata loTableIndex in Indexes)
                {
                    if (loTableIndex.IsPrimaryKey) return loTableIndex;
                }
                return null;
            }
        }
        /// <summary>
        /// _UniqueContraintFields
        /// A host-only list of fields the define a unique constraint.
        /// </summary>
        private List<string> _UniqueContraintFields;
        public List<string> UniqueContraintFields
        {
            get { return _UniqueContraintFields; }
            set { _UniqueContraintFields = value; }
        }

        public IndexMetadata FindIndexByName(String iIndexName)
        {
            return Indexes.Find(new MetadataBaseObjectPredicate(iIndexName).CompareByName);
        }

        public List<IndexMetadata> Indexes = new List<IndexMetadata>();

        public List<ColumnMetadata> Fields = new List<ColumnMetadata>();

        public int FindIndexCollectionElementNoByName(string iFindName)
        {
            MetadataBaseObjectPredicate loPredicate = new MetadataBaseObjectPredicate(iFindName);
            return this.Indexes.FindIndex(loPredicate.CompareByName);
        }

        public IndexMetadata GetIndex(int iItemNdx)
        {
            if (this.Indexes == null) return null;
            return this.Indexes[iItemNdx];
        }

        public IndexMetadata GetIndex(String iIndexName)
        {
            int loIndex = FindIndexCollectionElementNoByName(iIndexName);
            if (loIndex < 0) return null;
            return this.Indexes[loIndex];
        }

        public int GetTableIndexCnt()
        {
            if (this.Indexes == null) return 0;
            return this.Indexes.Count;
        }

        public TableMetadata()
            : base()
        {
        }

        /// <summary>
        /// Returns a substring starting at iStart position and upto iMaxLength characters.
        /// If iStart is invalid, and empty string is returned. If there are not enough characters
        /// to satisfy iMaxLength, then the returned substring will contain as many characters as
        /// possible.
        /// </summary>
        private static string SafeSubString(string iString, int iStart, int iMaxLength)
        {
            if (iStart > iString.Length - 1)
                return "";
            return iString.Substring(iStart, Math.Min(iMaxLength, (iString.Length - iStart)));
        }

        public void AddField(ColumnMetadata iField)
        {
            this.Fields.Add(iField);
        }

        public ColumnMetadata GetField(int iFieldNdx)
        {
            if (iFieldNdx < 0)
                return null;
            if (iFieldNdx < this.Fields.Count)
                return this.Fields[iFieldNdx];
            return null;
        }

        /// <summary>
        ///  Gets the field number for a field name.  Virtual field numbers begin after the
        ///  real field numbers end.
        /// </summary>
        /// <param name="iName"></param>
        /// <returns></returns>
        public int GetFldNo(String iName)
        {
            // Don't bother searching for an invalid name
            if ((iName == null) || (iName.Equals("")))
                return -1;

            MetadataBaseObjectPredicate loPredicate = new MetadataBaseObjectPredicate(iName);
            int loResult = this.Fields.FindIndex(loPredicate.CompareByName_CaseInsensitive);
            if (loResult >= 0)
                return loResult;

            return -1;
        }

        public ColumnMetadata GetField(String iName)
        {
            // construct this in a way such that invalid field names 
            // don't cause an internal exception - let the caller deal 
            // with missing or invalid field names
            int loFldNo = GetFldNo(iName);
            if (loFldNo != -1)
            {
                return GetField(loFldNo);
            }
            else
            {
                return null;
            }
        }

        public int GetFieldCnt()
        {
            return this.Fields.Count;
        }
    }

    public class ColumnMetadata : MetadataBaseObject
    {
        #region Properties and Members
        /// <summary>
        /// Indicates if the column in the database table should have the "not null" constraint.
        /// </summary>
        private bool _dbNotNull = false;
        [System.ComponentModel.DefaultValue(false)] // This prevents serialization of default values
        public virtual bool dbNotNull
        {
            get { return (_dbNotNull); }
            set { _dbNotNull = value; }
        }

        /// <summary>
        /// DefaultCurrentTime is an attribute required by the host side to determine if the
        /// underlying table column should have a default.
        /// </summary>
        protected bool _DefaultCurrentTime;
        [XmlIgnore]
        public virtual bool DefaultCurrentTime
        {
            get
            {
                return _DefaultCurrentTime;
            }
            set { _DefaultCurrentTime = value; }
        }

        /// <summary>
        /// Host only. Returns whether the column is an sql server "identity"; a generated, primary key field.
        /// </summary>
        /// <returns></returns>
        protected bool _IsIdentityColumn = false;
        public virtual bool IsIdentityColumn
        {
            get { return (_IsIdentityColumn); }
            set { _IsIdentityColumn = value; }
        }

        protected int _Size = 0;
        [System.ComponentModel.DefaultValue(0)] // This prevents serialization of default values
        public virtual int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }
        #endregion

        public ColumnMetadata(String iName, int iSize, bool idbNotNull)
            : base()
        {
            Name = iName;
            Size = iSize;
            dbNotNull = idbNotNull;
        }

        public ColumnMetadata()
            : base()
        {
            //All of the serialized classes must have a parameterless constructor
        }
    }

    public class IntColumnMetadata : ColumnMetadata
    {
        public IntColumnMetadata()
            : base()
        {
        }

        public IntColumnMetadata(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }
    }

    public class BlobColumnMetadata : ColumnMetadata
    {
        public BlobColumnMetadata()
            : base()
        {
        }

        public BlobColumnMetadata(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }
    }

    public class StringColumnMetadata : ColumnMetadata
    {
        public StringColumnMetadata()
            : base()
        {
        }

        public StringColumnMetadata(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }
    }

    public class RealColumnMetadata : ColumnMetadata
    {
        public RealColumnMetadata()
            : base()
        {
        }

        public RealColumnMetadata(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }
    }

    public class DateTimeColumnMetadata : ColumnMetadata
    {
        public DateTimeColumnMetadata()
            : base()
        {
        }

        public DateTimeColumnMetadata(String iName, int iSize, bool idbNotNull, bool iDefaultCurrentTime)
            : base(iName, iSize, idbNotNull)
        {
            DefaultCurrentTime = iDefaultCurrentTime;
        }
    }

    public class BooleanColumnMetadata : ColumnMetadata
    {
        public BooleanColumnMetadata()
            : base()
        {
        }

        public BooleanColumnMetadata(String iName, int iSize, bool idbNotNull)
            : base(iName, iSize, idbNotNull)
        {
        }
    }
    #endregion

    #region MetadataBaseObjectPredicate
    public class MetadataBaseObjectPredicate
    {
        private string _CompareName;
        private Type _CompareClassType;

        // Constructor used when comparing strings (ie. Name or DisplayName)
        public MetadataBaseObjectPredicate(string CompareName)
        {
            _CompareName = CompareName;
        }

        // Constructor used when comparing class types
        public MetadataBaseObjectPredicate(Type CompareClassType)
        {
            _CompareClassType = CompareClassType;
        }

        // Compare by Name (Case-Sensitive)
        public bool CompareByName(MetadataBaseObject pObject)
        {
            return (pObject._Name == _CompareName);
        }


        // Compare by Name - Starts With (Case-Sensitive)
        public bool CompareByName_StartsWith(MetadataBaseObject pObject)
        {
            return (pObject._Name.StartsWith(_CompareName));
        }


        // Compare by Name (Case-Insensitive)
        public bool CompareByName_CaseInsensitive(MetadataBaseObject pObject)
        {
            return (System.String.Compare(pObject._Name, this._CompareName, true) == 0);
        }

        // Compare by Class Type
        public bool CompareByClassType(MetadataBaseObject pObject)
        {
            return _CompareClassType.IsAssignableFrom(pObject.GetType());
        }
    }
    #endregion

    #region DBMetadataProvider
    public abstract class DBMetadataProvider
    {
        public Boolean AutoCreateDB = false;
        public Boolean CreateDatabaseFailed = false;

        public virtual String IdentityPropertyClause
        {
            // Only return a value for DB plaforms that have an "AutoInc" equivalent, like SQL Server or SqLite
            // SqLite uses   " AUTOINCREMENT(1,1)"
            // SqlSever uses " IDENTITY (1, 1)"
            get { return ""; }
        }

        /// <summary>
        /// The ordinal_position column in some schema tables can either start from 0 or 1.
        /// Firebird is 0, sql server is 1.  The default behavior will be 0, sql server will override this to return 1.
        /// </summary>
        public virtual int OrdinalPositionBase
        {
            get { return 0; }
        }

        protected virtual String GetSchemaColumnsTableName()
        {
            return "Columns";
        }
        protected virtual String GetSchemaTablesTableName()
        {
            return "Tables";
        }

        /// <summary>
        /// Provider specific name of "Columns" table used to get schema
        /// </summary>
        public String SchemaColumnsTableName { get { return GetSchemaColumnsTableName(); } }
        public String SchemaTablesTableName { get { return GetSchemaTablesTableName(); } }

        /// <summary>
        /// Provider specific list of restrictions to get all the columns for a table.
        /// </summary>
        /// <param name="iTableName"></param>
        /// <returns></returns>
        public virtual String[] SchemaColumnsRestrictions(String iTableName, String iColumnName)
        {
            return null;
        }

        public virtual String[] SchemaTablesRestrictions(String iTableName)
        {
            return null;
        }

        /// <summary>
        /// FldDefFromSchemaColumn
        /// 
        /// Passed a row containing the info for a column from the shema database, creates
        /// a ColumnMetadata to match.
        /// </summary>
        /// <param name="iSchemaColumn"></param>
        /// <returns></returns>
        public virtual ColumnMetadata FldDefFromSchemaColumn(DataRow iSchemaColumn)
        {
            return null;
        }

        /// <summary>
        /// name of the index_columns table. Is the same for SqlServer, Oracle and Firebird...
        /// </summary>
        /// <returns></returns>
        public virtual String SchemaIndexColumnsTableName(bool iPrimaryKey)
        {
            return "IndexColumns";
        }

        /// <summary>
        /// Populate the restructions array so that GetSchema on IndexColumns table returns
        /// all the rows that include the provided table and column names.
        /// </summary>
        /// <param name="iTableName"></param>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        public virtual String[] SchemaIndexColumnsRestrictions(String iTableName, String iColumnName, bool iPrimaryKey)
        {
            return null;
        }

        /// <summary>
        /// Returns the name of the ordinal_position column in the indexcolumns table
        /// </summary>
        /// <returns></returns>
        public virtual String SchemaIndexColumnsOrdinalPositionName()
        {
            return "ORDINAL_POSITION"; // SqlServer is lower case, Firebird is upper, is it case sensitive?
        }

        /// <summary>
        /// Returns the name of the index's name column in the indexcolumns table
        /// </summary>
        /// <returns></returns>
        public virtual String SchemaIndexColumnsIndexName(bool iPrimaryKey)
        {
            return "INDEX_NAME"; // SqlServer is lower case, Firebird and Oracle is upper, is it case sensitive?
        }

        public virtual String CreateStringColumnClause(ColumnMetadata iTableFldDef)
        {
            int desiredSize = iTableFldDef.Size;
            return "VARCHAR(" + desiredSize.ToString() + ")";
        }

        public virtual String CreateIntColumnClause(ColumnMetadata iTableFldDef)
        {
            // SqlServer & Firebird are different. Implement this is descendant class
            throw new Exception("Descendant DBMetadataProvider must implement CreateIntClause");
        }

        public virtual String CreateRealColumnClause(ColumnMetadata iTableFldDef)
        {
            // SqlServer & Firebird are different. Implement this is descendant class
            throw new Exception("Descendant DBMetadataProvider must implement CreateRealClause");
        }

        public virtual String CreateDateTimeColumnClause(ColumnMetadata iTableFldDef)
        {
            // SqlServer & Firebird are different. Implement this is descendant class
            throw new Exception("Descendant DBMetadataProvider must implement CreateDateTimeClause");
        }

        public virtual String CreateBooleanColumnClause(ColumnMetadata iTableFldDef)
        {
            // SqlServer & Firebird are different. Implement this is descendant class
            throw new Exception("Descendant DBMetadataProvider must implement CreateDateTimeClause");
        }

        public virtual String CreateBlobColumnClause(ColumnMetadata iTableFldDef)
        {
            // SqlServer & Firebird are different. Implement this is descendant class
            throw new Exception("Descendant DBMetadataProvider must implement CreateBlobColumnClause");
        }

        public String CreateColumnClause(ColumnMetadata iColumnFldDef)
        {
            if (iColumnFldDef is StringColumnMetadata)
                return CreateStringColumnClause(iColumnFldDef);
            if (iColumnFldDef is IntColumnMetadata)
                return CreateIntColumnClause(iColumnFldDef);
            if (iColumnFldDef is DateTimeColumnMetadata)
                return CreateDateTimeColumnClause(iColumnFldDef);
            if (iColumnFldDef is RealColumnMetadata)
                return CreateRealColumnClause(iColumnFldDef);
            if (iColumnFldDef is BlobColumnMetadata)
                return CreateBlobColumnClause(iColumnFldDef);
            if (iColumnFldDef is BooleanColumnMetadata)
                return CreateBooleanColumnClause(iColumnFldDef);

            throw new Exception("Unhandled ColumnMetadata class in CreateColumnClause " + iColumnFldDef.GetType().ToString());
        }

        /// <summary>
        /// Return provider specific "not null" clause for column creation
        /// </summary>
        /// <returns></returns>
        public virtual String NotNullClause()
        {
            return " NOT NULL"; // works for Firebird, Oracle and SQL server
        }

        public virtual String DefaultCurrentTimeClause()
        {
            throw new Exception("Descendant DBMetadataProvider must implement DefaultCurrentTimeClause");
        }
    }
    #endregion

}