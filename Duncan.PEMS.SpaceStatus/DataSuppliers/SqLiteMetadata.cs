using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;

using System.Data.SQLite; // If not installed, run "\3rd Party Components\SQLite\SQLite-1.0.66.0-setup.exe"

namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{
    #region SQLiteMetadataProvider
    public class SQLiteMetadataProvider : DBMetadataProvider
    {
        public SQLiteMetadataProvider()
            : base()
        {
        }

        public override String IdentityPropertyClause
        {
            get { return " AUTOINCREMENT(1,1)"; } // SQL Server would use " IDENTITY (1, 1)"
        }

        public override int OrdinalPositionBase
        {
            // SqLite uses 0-based indexes, like Firebird.  SqlServer uses 1-based indexes
            get { return 0; }
        }

        /// <summary>
        /// SqlServer resrictions for the columns table are:
        /// 1:Catalog -> Obtained from connection string.
        /// 2:Owner/Schema ->dbo by default, but we'll leave it null for now.
        /// 3:Table name -> provided as a parameter
        /// </summary>
        /// <param name="iTableName"></param>
        /// <returns></returns>
        public override String[] SchemaColumnsRestrictions(String iTableName, String iColumnName)
        {
            List<String> loRestricts = new List<String>();
            loRestricts.Add(null);  // SQL Server would use InitialCatalog here
            loRestricts.Add(null);
            loRestricts.Add(iTableName);
            loRestricts.Add(iColumnName);
            return loRestricts.ToArray();
        }

        public override String[] SchemaTablesRestrictions(String iTableName)
        {
            List<String> loRestricts = new List<String>();
            loRestricts.Add(null); // SQL Server would use InitialCatalog here
            loRestricts.Add(null);
            loRestricts.Add(iTableName);
            loRestricts.Add(null);  // SQL Server would use "BASE TABLE" here
            return loRestricts.ToArray();
        }

        public Type MapDbTypeToClrType(DbType anDbType)
        {
            if (anDbType == DbType.Int64)
                return typeof(Int64);
            if (anDbType == DbType.Binary)
                return typeof(Byte[]);
            if (anDbType == DbType.Boolean)
                return typeof(Boolean);
            if (anDbType == DbType.String)
                return typeof(String);
            if (anDbType == DbType.Currency)
                return typeof(Decimal);
            if (anDbType == DbType.Date)
                return typeof(DateTime);
            if (anDbType == DbType.DateTime)
                return typeof(DateTime);
            if (anDbType == DbType.Decimal)
                return typeof(Decimal);
            if (anDbType == DbType.Double)
                return typeof(Double);
            if (anDbType == DbType.Guid)
                return typeof(Guid);
            if (anDbType == DbType.Int32)
                return typeof(Int32);
            if (anDbType == DbType.Single)
                return typeof(Single);
            if (anDbType == DbType.Int16)
                return typeof(Int16);
            if (anDbType == DbType.SByte)
                return typeof(SByte);
            if (anDbType == DbType.UInt64)
                return typeof(UInt64);
            if (anDbType == DbType.UInt32)
                return typeof(UInt32);
            if (anDbType == DbType.UInt16)
                return typeof(UInt16);
            if (anDbType == DbType.Byte)
                return typeof(Byte);
            if (anDbType == DbType.Binary)
                return typeof(Byte[]);
            if (anDbType == DbType.VarNumeric)
                return typeof(Decimal);

            return typeof(Object);
        }

        /// <summary>
        /// FldDefFromSchemaColumn
        /// Implements SQL Server specific version of inherited method.
        /// </summary>
        /// <param name="iSchemaColumn"></param>
        /// <returns></returns>
        public override ColumnMetadata FldDefFromSchemaColumn(DataRow iSchemaColumn)
        {
            ColumnMetadata loFldDef;
            if (iSchemaColumn == null)
                throw new Exception("FldDefFromSchemaColumn passed null iSchemaColumn");

            // map sql server column types to ColumnMetadata descendants
            string dataTypeAsString = Convert.ToString(iSchemaColumn["DATA_TYPE"]);
            DbType DbType = DbType.String;
            // Now translate the known values to a DbType
            if (string.Compare(dataTypeAsString, "integer", true) == 0)
                DbType = DbType.Int64;
            else if (string.Compare(dataTypeAsString, "timestamp", true) == 0) // SQL Server uses "datetime"
                DbType = DbType.DateTime;
            else if (string.Compare(dataTypeAsString, "varchar", true) == 0)
                DbType = DbType.String;
            else if (string.Compare(dataTypeAsString, "blob", true) == 0)
                DbType = DbType.Binary;
            else if (string.Compare(dataTypeAsString, "boolean", true) == 0)
                DbType = DbType.Boolean;
            else
                throw new Exception("Unhandled schema column datatype: " + dataTypeAsString);

            Type mappedDbType = MapDbTypeToClrType(DbType);

            // start w/ most likely type, varchar
            if (mappedDbType == typeof(string))
            {
                loFldDef = new StringColumnMetadata();
                loFldDef.Size = Convert.ToInt32(iSchemaColumn["CHARACTER_MAXIMUM_LENGTH"].ToString());
            }
            else if (mappedDbType == typeof(DateTime))
            {
                loFldDef = new DateTimeColumnMetadata();
            }
            else if (mappedDbType == typeof(Boolean))
            {
                loFldDef = new BooleanColumnMetadata();
            }
            else if (mappedDbType == typeof(Int64))
            {
                loFldDef = new IntColumnMetadata();
                loFldDef.Size = 19; // maximum decimal size of 64-bit number
            }
            else if (mappedDbType == typeof(Int32))
            {
                loFldDef = new IntColumnMetadata();
                loFldDef.Size = 10; // maximum decimal size of 32-bit number
            }
            else if (mappedDbType == typeof(Int16))
            {
                loFldDef = new IntColumnMetadata();
                loFldDef.Size = 5; // maximum decimal size of 16-bit number
            }
            else if (mappedDbType == typeof(SByte))
            {
                loFldDef = new IntColumnMetadata();
                loFldDef.Size = 3; // maximum decimal size of 8-bit number
            }
            else if (mappedDbType == typeof(Decimal))
            {
                loFldDef = new RealColumnMetadata();
                loFldDef.Size = Convert.ToInt32(iSchemaColumn["NUMERIC_PRECISION"].ToString());  // Use NUMERIC_PRECISION and NUMERIC_SCALE?
            }
            else if (mappedDbType == typeof(Single)) // real is the same as float
            {
                loFldDef = new RealColumnMetadata();
                loFldDef.Size = Convert.ToInt32(iSchemaColumn["NUMERIC_PRECISION"].ToString());   // Use NUMERIC_PRECISION and NUMERIC_SCALE?
            }
            else if (mappedDbType == typeof(Byte[]))
            {
                loFldDef = new BlobColumnMetadata();
                loFldDef.Size = 8000; // Convert.ToInt32(iSchemaColumn["NUMERIC_PRECISION"].ToString());   // Use NUMERIC_PRECISION and NUMERIC_SCALE?
            }
            else
                return null;

            loFldDef.Name = iSchemaColumn["COLUMN_NAME"].ToString();

            try
            {
                if ((bool)iSchemaColumn["IS_NULLABLE"] == false)
                    loFldDef.dbNotNull = true;
                else if (string.Compare(iSchemaColumn["IS_NULLABLE"].ToString(), "NO", true) == 0)
                    loFldDef.dbNotNull = true;
            }
            catch { }

            return loFldDef;
        }

        /// <summary>
        /// Firebird & SqlServer have same restrictions on IndexColumns table. 3 is table name, 5 is column name
        /// </summary>
        /// <param name="iTableName"></param>
        /// <param name="iColumnName"></param>
        /// <returns></returns>
        public override String[] SchemaIndexColumnsRestrictions(String iTableName, String iColumnName, bool iPrimaryKey)
        {
            List<String> loRestricts = new List<String>();
            loRestricts.Add(null);
            loRestricts.Add(null);
            loRestricts.Add(iTableName);
            loRestricts.Add(null);
            loRestricts.Add(iColumnName);
            return loRestricts.ToArray();
        }

        #region Provider Specific Column Types
        public override String CreateIntColumnClause(ColumnMetadata iTableFldDef)
        {
            if (iTableFldDef.IsIdentityColumn == true)
                return "INTEGER NOT NULL PRIMARY KEY"; //"INTEGER PRIMARY KEY AUTOINCREMENT"; //"AUTOINCREMENT(1,1)";
            else if (iTableFldDef.Size <= 3)
                return "INTEGER"; //"TINYINT";
            else if (iTableFldDef.Size <= 5)
                return "INTEGER"; //"SMALLINT";
            else if (iTableFldDef.Size <= 10)
                return "INTEGER"; //"INTEGER";
            else if (iTableFldDef.Size <= 19)
                return "INTEGER"; //"CURRENCY"; // "DECIMAL"; // "FLOAT";
            return "INTEGER";
        }

        public override String CreateDateTimeColumnClause(ColumnMetadata iTableFldDef)
        {
            return "TIMESTAMP"; // "DATETIME";
        }

        public override String CreateBooleanColumnClause(ColumnMetadata iTableFldDef)
        {
            return "BOOLEAN"; // "LOGICAL";
        }

        public override String CreateRealColumnClause(ColumnMetadata iTableFldDef)
        {
            if (iTableFldDef.Size <= 18)
                return "REAL"; //"DECIMAL";
            else if (iTableFldDef.Size <= 19)
                return "REAL"; //"CURRENCY";
            else
                return "REAL"; //"DECIMAL";
        }

        public override String CreateBlobColumnClause(ColumnMetadata iTableFldDef)
        {
            return "BLOB"; // "IMAGE";
        }
        #endregion

        public override String DefaultCurrentTimeClause()
        {
            return " DEFAULT CURRENT_TIMESTAMP";
        }
    }
    #endregion

    #region SqLiteMetadataManager
    public class SqLiteMetadataManager
    {
        public delegate void UpdateLogCallBack(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID);
        public delegate void OnDBTableCreated(string tableName);

        private SqLiteDAO _DBAccess = null;

        private DBMetadataProvider _DBMetadataProvider;
        public DBMetadataProvider MetadataProvider { get { return _DBMetadataProvider; } }

        private List<TableMetadata> _MetadataObjects = null;
        protected UpdateLogCallBack _DefaultUpdateLogCallback = null;

        public event OnDBTableCreated DBTableCreatedEvent = null;

        public SqLiteMetadataManager(Boolean AutoCreateDB, UpdateLogCallBack iCallBackDelegate, SqLiteDAO dbAccess, List<TableMetadata> MetadataObjects)
        {
            _DBAccess = dbAccess;
            _MetadataObjects = MetadataObjects;
            _DefaultUpdateLogCallback = iCallBackDelegate;

            InitOrCreateDatabase(AutoCreateDB);
            /*this.DBTableCreatedEvent += new OnDBTableCreated(DBMetadata_DBTableCreatedEvent);*/
        }

        private void LogMessage(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID)
        {
            if (this._DefaultUpdateLogCallback != null)
                this._DefaultUpdateLogCallback(logLevel, textToLog, classAndMethod, threadID);
        }

        private string GetClassAndMethodForLogging()
        {
            // This is useful code, but we don't need to clog the log with this additional info for database Metadata checks,
            // so we will just return "string.Emtpy" object here
            return string.Empty;

            /*
            // Neat little helper class that enhances logging capabilities
            System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1, true);
            return this.GetType().Name + "." + sf.GetMethod().Name + "()";
            */
        }

        public void InitOrCreateDatabase(Boolean AutoCreateDB)
        {
            // create a DbMetadataProvider
            _DBMetadataProvider = new SQLiteMetadataProvider();
            _DBMetadataProvider.AutoCreateDB = AutoCreateDB;

            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(_DBAccess.DBConnectStr);
            try
            {
                // Let caller know about anything we logged to this point
                LogMessage(TraceLevel.Verbose, "Attempting database connection...", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                _DBAccess.TryDBConnection();
                LogMessage(TraceLevel.Verbose, "  Connection successful. (DataSource: " + builder.DataSource + ")", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception loExcept)
            {
                LogMessage(TraceLevel.Error, "  Connection Failed! (DataSource: " + builder.DataSource + ")", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // Let caller know about anything we logged to this point
                if (loExcept is SQLiteException)
                {
                    SQLiteException loSQLError = (loExcept as SQLiteException);
                    LogMessage(TraceLevel.Error, loSQLError.Message + "; ErrorCode = " + loSQLError.ErrorCode.ToString(), 
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
                else
                {
                    LogMessage(TraceLevel.Error, loExcept.Message, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }

                // re-throw the error
                throw loExcept;
            }
        }

        public bool CheckTableIndex(IndexMetadata iTableIndex)
        {
            if (IndexExists(iTableIndex.Name, iTableIndex.ParentTable.Name) == false)
            {
                return false;
            }
            else
            {
                LogMessage(TraceLevel.Verbose, "  Verified Index '" + iTableIndex.Name + "' already exists.", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                return true;
            }
        }

        private bool IndexExists(String iIndexName, String iTableName)
        {
            // There seems to be a bug in that the restriction for IndexName doesn't find a match, so we
            // will only restrict to table name, then look through dataset for row with matching index name
            DataTable loTmpTable;
            loTmpTable = this._DBAccess.GetSchema("Indexes", new string[] { null, null, iTableName /*, iIndexName*/ }, true);
            foreach (DataRow nextRow in loTmpTable.Rows)
            {
                if (string.Compare(iIndexName, Convert.ToString(nextRow["INDEX_NAME"]), true) == 0)
                    return true;
            }
            return false;
        }

        public void CheckTableIndexes(TableMetadata iTableDef, bool iCreateIfNecessary)
        {
            foreach (IndexMetadata loTableIndex in iTableDef.Indexes)
            {
                if (iCreateIfNecessary)
                {
                    if (!CheckTableIndex(loTableIndex))
                    {
                        CreateTableIndex(loTableIndex);
                    }
                }
                else
                {
                    CheckTableIndex(loTableIndex);
                }
            }
        }

        private bool TableExists(String iTableName)
        {
            DataTable loTmpTable;
            loTmpTable = this._DBAccess.GetSchema(_DBMetadataProvider.SchemaTablesTableName,
                    _DBMetadataProvider.SchemaTablesRestrictions(iTableName), true);
            return loTmpTable.Rows.Count != 0;
        }

        private DataTable GetTableColumn(String iTableName, String iColumnName)
        {
            DataTable loTmpTable;
            loTmpTable = this._DBAccess.GetSchema(_DBMetadataProvider.SchemaColumnsTableName,
                _DBMetadataProvider.SchemaColumnsRestrictions(iTableName, iColumnName), true);
            return loTmpTable;
        }

        private void AddTableColStatement(ColumnMetadata iColumnFldDef, TableMetadata iTableDef, ref StringBuilder iSqlStatement, bool isForAlterStatement)
        {
            iSqlStatement.Append(" " + iColumnFldDef.Name + " ");
            iSqlStatement.Append(_DBMetadataProvider.CreateColumnClause(iColumnFldDef));

            // handle any "not null" or default conditions
            if (iColumnFldDef.dbNotNull)
                iSqlStatement.Append(_DBMetadataProvider.NotNullClause());

            // Handle default current time or Identity attribute only when doing a CREATE statement
            if (isForAlterStatement == false)
            {
                if (iColumnFldDef.DefaultCurrentTime)
                    iSqlStatement.Append(_DBMetadataProvider.DefaultCurrentTimeClause());
                /*
                if (iColumnFldDef.IsIdentityColumn == true)
                    iSqlStatement.Append(_DBMetadataProvider.IdentityPropertyClause);
                */

                /*
                if (iTableDef.PrimaryKey != null)
                {
                    foreach (int loFldNo in iTableDef.PrimaryKey.fIndexFieldNos)
                    {
                        if (iTableDef.Fields[loFldNo].Name == iColumnFldDef.Name)
                        {
                            iSqlStatement.Append(" CONSTRAINT PK_" + iTableDef.Name + " PRIMARY KEY");
                        }
                    }
                }
                */
            }

            iSqlStatement.Append(',');
        }

        public void CheckTable(TableMetadata iTableDef, bool iCreateIfNecessary)
        {
            try
            {
                LogMessage(TraceLevel.Verbose, "Checking Table " + iTableDef.Name + "...", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // 1st, get the info for the table.
                if (!TableExists(iTableDef.Name))
                {
                    if (iCreateIfNecessary)
                    {
                        CreateTable(iTableDef);
                    }
                    else
                    {
                        LogMessage(TraceLevel.Verbose, "  Table '" + iTableDef.Name + "' doesn't exist", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                    }
                    return;
                }

                // table exists, now get all of the existing columns from DB schema 
                DataTable loSchemaColumnsForTable = GetTableColumn(iTableDef.Name, null);
                bool loColumnAlreadyExists = false;
                foreach (ColumnMetadata loColumnDef in iTableDef.Fields)
                {
                    // See if the column already exists in the schema
                    loColumnAlreadyExists = false;
                    foreach (DataRow nextRow in loSchemaColumnsForTable.Rows)
                    {
                        if (string.Compare(nextRow["COLUMN_NAME"].ToString(), loColumnDef.Name, true) == 0)
                        {
                            loColumnAlreadyExists = true;
                            break;
                        }
                    }

                    if (loColumnAlreadyExists == false)
                    {
                        if (iCreateIfNecessary)
                        {
                            AddTableColumn(loColumnDef, iTableDef, iTableDef.Name);
                        }
                        else
                        {
                            LogMessage(TraceLevel.Verbose, "  Column '" + loColumnDef.Name + "' doesn't exist", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                        }
                        continue;
                    }

                    StringBuilder sbColumnDefn = new StringBuilder();
                    AddTableColStatement(loColumnDef, iTableDef, ref sbColumnDefn, true);
                    // delete the last comma
                    sbColumnDefn.Remove(sbColumnDefn.Length - 1, 1);

                    LogMessage(TraceLevel.Verbose, "  Verified column Metadata: " + sbColumnDefn.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }

                // check this table's indexes
                CheckTableIndexes(iTableDef, iCreateIfNecessary);
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "  Checking table Metadata failed: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void CreateTable(TableMetadata iTableDef)
        {
            StringBuilder loSqlStatment = new StringBuilder();

            loSqlStatment.Append("CREATE TABLE [" + iTableDef.Name + "] (");
            // add all the fields
            foreach (ColumnMetadata loColumnDef in iTableDef.Fields)
            {
                AddTableColStatement(loColumnDef, iTableDef, ref loSqlStatment, false);
            }

            // delete the last comma
            loSqlStatment.Remove(loSqlStatment.Length - 1, 1);

            // close the last parenthesis
            loSqlStatment.Append(")");

            // now submit the statement
            try
            {
                _DBAccess.ExecuteNonQuery(loSqlStatment.ToString(), new Dictionary<string, object>(), true);

                LogMessage(TraceLevel.Verbose, "  Created Table '" + iTableDef.Name + "'", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                // check & build the indexes
                CheckTableIndexes(iTableDef, true);

                if (DBTableCreatedEvent != null)
                {
                    DBTableCreatedEvent(iTableDef.Name);
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "  Create table failed: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void CreatePrimaryKey(IndexMetadata iTableIndex)
        {
            try
            {
                StringBuilder loSqlStatement = new StringBuilder();
                string loIndexName = iTableIndex.Name; // "PK_" + iTableIndex.ParentTableDef.Name

                loSqlStatement.Append("ALTER TABLE [" + iTableIndex.ParentTable.Name + "] ADD CONSTRAINT " + loIndexName + " PRIMARY KEY (");
                // now add all the column names
                for (int loNdx = 0; loNdx < iTableIndex.GetIndexFieldCnt(); loNdx++)
                {
                    // ask MetadataMgr for the index columns for this table that include this column at this position
                    loSqlStatement.Append(iTableIndex.ColumnNames(loNdx) + ",");
                }
                // drop the last comma
                loSqlStatement.Remove(loSqlStatement.Length - 1, 1);
                // add a closing parenthesis
                loSqlStatement.Append(')');
                // now submit the statement
                _DBAccess.ExecuteNonQuery(loSqlStatement.ToString(), new Dictionary<string, object>(), true);

                LogMessage(TraceLevel.Verbose, "  Created Primary Key '" + loIndexName + "'", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "  Create Primary Key failed: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void CreateTableIndex(IndexMetadata iTableIndex)
        {
            string loIndexName = iTableIndex.Name;

            if (iTableIndex.IsPrimaryKey)
            {
                CreatePrimaryKey(iTableIndex);
                return;
            }

            // We are creating index because it doesn't exist, not because it might be different, so there is no existing index to drop...
            /*
            // drop this index if it exists
            try
            {
                _DBAccess.ExecuteNonQuery("DROP INDEX " + loIndexName + " ON [" + iTableIndex.ParentTableDef.Name + "]", new Dictionary<string, object>(), true);
            }
            catch { }
            */

            StringBuilder loSqlStatement = new StringBuilder();

            loSqlStatement.Append("CREATE ");
            if (iTableIndex.IsUniqueConstraint)
                loSqlStatement.Append("UNIQUE ");

            loSqlStatement.Append("INDEX " + loIndexName + " ON [" + iTableIndex.ParentTable.Name + "] (");
            // now add all the column names
            for (int loNdx = 0; loNdx < iTableIndex.GetIndexFieldCnt(); loNdx++)
            {
                // ask MetadataMgr for the index columns for this table that include this column at this position
                loSqlStatement.Append(iTableIndex.ColumnNames(loNdx) + ",");
            }
            // drop the last comma
            loSqlStatement.Remove(loSqlStatement.Length - 1, 1);
            // add a closing parenthesis
            loSqlStatement.Append(')');
            // now submit the statement
            try
            {
                _DBAccess.ExecuteNonQuery(loSqlStatement.ToString(), new Dictionary<string, object>(), true);
                LogMessage(TraceLevel.Verbose, "  Created Index '" + iTableIndex.Name + "'", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "  Create index failed: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void CheckAllTables(bool iFixErrors)
        {
            if (_MetadataObjects != null)
            {
                foreach (TableMetadata loTableDef in _MetadataObjects)
                {
                    CheckTable(loTableDef, iFixErrors);

                    /*if (this._DefaultUpdateLogCallback != null)
                        this._DefaultUpdateLogCallback(oLog, System.Threading.Thread.CurrentThread.ManagedThreadId, GetClassAndMethodForLogging());*/
                }
            }
        }

        public void AddTableColumn(ColumnMetadata iColumnDef, TableMetadata iTableDef, string iTableName)
        {
            StringBuilder loSqlStatment = new StringBuilder();

            loSqlStatment.Append("ALTER TABLE [" + iTableName + "] ADD ");
            AddTableColStatement(iColumnDef, iTableDef, ref loSqlStatment, false);

            // delete the last comma
            loSqlStatment.Remove(loSqlStatment.Length - 1, 1);

            // now submit the statement
            try
            {
                _DBAccess.ExecuteNonQuery(loSqlStatment.ToString(), new Dictionary<string, object>(), true);

                LogMessage(TraceLevel.Verbose, "  Created Column '" + iColumnDef.Name + "'", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "  Create column failed: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }

        public void AlterTableColumn(ColumnMetadata iTableFldDef, TableMetadata iTableDef, string iTableName)
        {
            StringBuilder loSqlStatment = new StringBuilder();
            loSqlStatment.Append("ALTER TABLE [" + iTableName + "] ALTER COLUMN ");
            AddTableColStatement(iTableFldDef, iTableDef, ref loSqlStatment, true);
            loSqlStatment.Remove(loSqlStatment.Length - 1, 1);

            // now submit the statement
            try
            {
                try
                {
                    _DBAccess.ExecuteNonQuery(loSqlStatment.ToString(), new Dictionary<string, object>(), true);
                }
                catch (Exception ex1stTry)
                {
                    // DEBUG: Example of what can be done
                    /*
                    // Watch out for know problems where a Foreign Key contraint interfers with updating the column
                    if (ex1stTry.Message.Contains("'FK_CustomerBaseMeterFiles_Meters' is dependent"))
                    {
                        try
                        {
                            _DBAccess.ExecuteNonQuery("ALTER TABLE CustomerBaseMeterFiles DROP CONSTRAINT FK_CustomerBaseMeterFiles_Meters", new Dictionary<string, object>(), false);
                        }
                        catch { }

                        // Now try the main query again
                        _DBAccess.ExecuteNonQuery(loSqlStatment.ToString(), new Dictionary<string, object>(), true);

                        // Then re-add the constraint
                        _DBAccess.ExecuteNonQuery("ALTER TABLE CustomerBaseMeterFiles WITH CHECK ADD CONSTRAINT FK_CustomerBaseMeterFiles_Meters" +
                            " FOREIGN KEY(CustomerID, AreaID, MeterID) REFERENCES Meters (CustomerID, AreaID, MeterID)", new Dictionary<string, object>(), false);
                    }
                    */
                    throw ex1stTry;
                }

                LogMessage(TraceLevel.Verbose, "  Altered Column '" + iTableFldDef.Name + "'", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "  Alter column failed: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }

        private void DBMetadata_DBTableCreatedEvent(string tableName)
        {
            // Example of what can be done here
            /*
            // See if we need to populate the "Customer" table
            try
            {
                // If the Customer table was just created, we need to add a default record
                if (string.Compare(tableName, "Customer", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    Dictionary<string, object> sqlParams = new Dictionary<string, object>();
                    sqlParams.Add("Company", "A System Sale");
                    sqlParams.Add("FirstName", "System");
                    sqlParams.Add("LastName", "Sales");
                    sqlParams.Add("Deleted", true);
                    _DBAccess.ExecuteNonQuery("INSERT INTO CUSTOMER (Company, FirstName, LastName, Deleted) VALUES (?, ?, ?, ?)", sqlParams, true);
                    LogMessage(TraceLevel.Verbose, "Seeded '" + tableName + "' table with default record", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception ex)
            {
                    LogMessage(TraceLevel.Error, "  Failed to seed table with default record(s): " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
            */
        }

    }
    #endregion
}

