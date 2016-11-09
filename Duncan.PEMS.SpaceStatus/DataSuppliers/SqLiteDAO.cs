using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;

using System.Linq;
using System.Configuration;
using System.Web;

using System.Data.SQLite; // If not installed, run "\3rd Party Components\SQLite\SQLite-1.0.66.0-setup.exe"



using Duncan.PEMS.SpaceStatus.DataMappers;
using Duncan.PEMS.SpaceStatus.UtilityClasses;

namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{
    public class SqLiteDAO
    {
        public delegate void UpdateLogCallBack(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID);
        protected UpdateLogCallBack _DefaultUpdateLogCallback = null;

        public bool MetadataHasBeenChecked = false;
        public object MetadataHasBeenChecked_Lock = new object();

        public int ClientCountWantingTransactionToStayOpen = 0;
        public object ClientCountWantingTransactionToStayOpen_Lock = new object();

        private ReaderWriterLockSlim _SQLiteDBReadWriteLocker = new ReaderWriterLockSlim();

        private SQLiteConnection _Session_sqlConnection = null;
        private SQLiteTransaction _Session_sqlTransaction = null;

        private Int64 _TransactedSizeOfData = 0; // This should only be accessed by routines that are inside a WriteLock

        private string _DBConnectStr = string.Empty;

        internal string DBConnectStr
        {
            get { return _DBConnectStr; }
        }

        public string SqLiteDbPathAndFilename;

        public SqLiteDAO(string dbConnectionString, string fullDBPathAndFilename, UpdateLogCallBack iCallBackDelegate)
        {
            _DefaultUpdateLogCallback = iCallBackDelegate;        

            _DBConnectStr =  dbConnectionString;
            SqLiteDbPathAndFilename = fullDBPathAndFilename;
        }

        public void StartTransactionSession()
        {
            // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
            try
            {
                // Use a write lock for this operation
                _SQLiteDBReadWriteLocker.EnterWriteLock();

                // Create connection if we don't already have one
                if (_Session_sqlConnection == null)
                    _Session_sqlConnection = new SQLiteConnection(_DBConnectStr);

                // Open DB connection if necessary
                if (_Session_sqlConnection.State == ConnectionState.Closed)
                {
                    _Session_sqlConnection.Open();

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("SqLite Server Version: " + _Session_sqlConnection.ServerVersion);
                    }
                    catch { }
                }

                // We will only start a new transaction if one doesn't already exist, or if existing one doesn't have a DB connection
                if ((_Session_sqlTransaction == null) || (_Session_sqlTransaction.Connection == null))
                {
                    // Do we have left-over transaction that needs to be released first?
                    if ((_Session_sqlTransaction != null) && (_Session_sqlTransaction.Connection == null))
                    {
                        _Session_sqlTransaction.Dispose();
                        _Session_sqlTransaction = null;
                    }

                    // Start transaction
                    _Session_sqlTransaction = _Session_sqlConnection.BeginTransaction();

                    // Reset the amount of transacted data waiting to be committed
                    this._TransactedSizeOfData = 0;
                }
            }
            finally
            {
                // Release the write lock           
                if (_SQLiteDBReadWriteLocker.IsWriteLockHeld)
                    _SQLiteDBReadWriteLocker.ExitWriteLock();
            }
        }

        public void CommitTransactionSession()
        {
            // In a thread-safe manner, see if there are any requests to keep transaction open.
            // If so, then we won't actually commit yet
            lock (this.ClientCountWantingTransactionToStayOpen_Lock)
            {
                if (this.ClientCountWantingTransactionToStayOpen > 0)
                    return;
            }

            // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
            try
            {
                // Use a write lock for this operation
                _SQLiteDBReadWriteLocker.EnterWriteLock();

                // If the transaction and connection is valid, proceed with the commit
                if (_Session_sqlTransaction != null)
                {
                    if ((_Session_sqlConnection != null) && (_Session_sqlConnection.State != ConnectionState.Closed) && (_Session_sqlTransaction.Connection != null))
                    {
                        _Session_sqlTransaction.Commit();
                    }
                }

                // Release this transaction
                if (_Session_sqlTransaction != null)
                    _Session_sqlTransaction.Dispose();
                _Session_sqlTransaction = null;

                // Close and release the shared connection
                if (_Session_sqlConnection != null)
                {
                    _Session_sqlConnection.Close();
                    _Session_sqlConnection.Dispose();
                }
                _Session_sqlConnection = null;

                // Reset the amount of transacted data waiting to be committed
                this._TransactedSizeOfData = 0;
            }
            finally
            {
                // Release the write lock           
                if (_SQLiteDBReadWriteLocker.IsWriteLockHeld)
                    _SQLiteDBReadWriteLocker.ExitWriteLock();
            }
        }

        public void CommitTransactionSession_ForcedCommit()
        {
            // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
            try
            {
                // Use a write lock for this operation
                _SQLiteDBReadWriteLocker.EnterWriteLock();

                // If the transaction and connection is valid, proceed with the commit
                if (_Session_sqlTransaction != null)
                {
                    if ((_Session_sqlConnection != null) && (_Session_sqlConnection.State != ConnectionState.Closed) && (_Session_sqlTransaction.Connection != null))
                    {
                        _Session_sqlTransaction.Commit();
                    }
                }

                // Release this transaction
                if (_Session_sqlTransaction != null)
                    _Session_sqlTransaction.Dispose();
                _Session_sqlTransaction = null;

                // Close and release the shared connection
                if (_Session_sqlConnection != null)
                {
                    _Session_sqlConnection.Close();
                    _Session_sqlConnection.Dispose();
                }
                _Session_sqlConnection = null;

                // Reset the amount of transacted data waiting to be committed
                this._TransactedSizeOfData = 0;
            }
            finally
            {
                // Release the write lock           
                if (_SQLiteDBReadWriteLocker.IsWriteLockHeld)
                    _SQLiteDBReadWriteLocker.ExitWriteLock();
            }
        }

        private void CheckForCommitAndResumeTrans_MustAlreadyHaveWriteLock()
        {
            // Make sure we are being called from a routine that has already acquired the Write lock
            if (_SQLiteDBReadWriteLocker.IsWriteLockHeld == false)
                throw new Exception("CheckForCommitAndResumeTrans requires that the thread already has a WriteLock");

            // Validate that we have a DB connection to work with
            if (_Session_sqlConnection == null)
                throw new Exception("CheckForCommitAndResumeTrans requires an active connection");

            // Validate that we have a DB transaction to work with
            if (_Session_sqlTransaction == null)
                throw new Exception("CheckForCommitAndResumeTrans requires an active transaction");

            // If the transacted size is large enough, we will commit what we have, then restart a new transaction
            if (this._TransactedSizeOfData >= (5 * 1024 * 1024))  // Commit after 5MB of data has been accumulated
            {
                // Reset size of uncommited data
                this._TransactedSizeOfData = 0;

                // Open DB connection if necessary
                if (_Session_sqlConnection.State == ConnectionState.Closed)
                    _Session_sqlConnection.Open();

                // If the transaction and connection is valid, proceed with the commit
                if ((_Session_sqlConnection.State != ConnectionState.Closed) && (_Session_sqlTransaction.Connection != null))
                    _Session_sqlTransaction.Commit();

                // Release this transaction
                if (_Session_sqlTransaction != null)
                    _Session_sqlTransaction.Dispose();
                _Session_sqlTransaction = null;

                // Start a new transaction
                _Session_sqlTransaction = _Session_sqlConnection.BeginTransaction();
            }
        }

        public DataTable GetSchema(string collectionName, string[] restrictionValues, bool throwErrors)
        {
            SQLiteConnection dbConnection = null;
            DataTable result = null;
            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a read lock for this operation
                    _SQLiteDBReadWriteLocker.EnterReadLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbConnection = new SQLiteConnection(_DBConnectStr);
                        dbConnection.Open();
                        result = dbConnection.GetSchema(collectionName, restrictionValues);
                    }
                    else
                    {
                        result = _Session_sqlConnection.GetSchema(collectionName);
                    }

                    return result;
                }
                finally
                {
                    // Release the read lock           
                    if (_SQLiteDBReadWriteLocker.IsReadLockHeld)
                        _SQLiteDBReadWriteLocker.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "Failed to get DB schema info: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
                return result;
            }
            finally
            {
                // If not inside a transaction session, free local db connection
                if (_Session_sqlTransaction == null)
                {
                    if (dbConnection != null)
                    {
                        dbConnection.Close();
                        dbConnection.Dispose();
                    }
                }
            }
        }

        public DataTable GetSchema(string collectionName, bool throwErrors)
        {
            SQLiteConnection dbConnection = null;
            DataTable result = null;
            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a read lock for this operation
                    _SQLiteDBReadWriteLocker.EnterReadLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbConnection = new SQLiteConnection(_DBConnectStr);
                        dbConnection.Open();
                        result = dbConnection.GetSchema(collectionName);
                    }
                    else
                    {
                        result = _Session_sqlConnection.GetSchema(collectionName);
                    }

                    return result;
                }
                finally
                {
                    // Release the read lock           
                    if (_SQLiteDBReadWriteLocker.IsReadLockHeld)
                        _SQLiteDBReadWriteLocker.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "Failed to get DB schema info: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
                return result;
            }
            finally
            {
                // If not inside a transaction session, free local db connection
                if (_Session_sqlTransaction == null)
                {
                    if (dbConnection != null)
                    {
                        dbConnection.Close();
                        dbConnection.Dispose();
                    }
                }
            }
        }

        private void SetQueryParameters(SQLiteCommand query, Dictionary<string, object> sqlParams)
        {
            // Build and populate SQL command parameters
            if (sqlParams != null)
            {
                foreach (string nextKey in sqlParams.Keys)
                {
                    if (sqlParams[nextKey] == null)
                    {
                        query.Parameters.AddWithValue(nextKey, null);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(string))
                    {
                        query.Parameters.Add(nextKey, DbType.String);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Int32))
                    {
                        query.Parameters.Add(nextKey, DbType.Int32);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Int16))
                    {
                        query.Parameters.Add(nextKey, DbType.Int16);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Int64))
                    {
                        query.Parameters.Add(nextKey, DbType.Int64);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(decimal))
                    {
                        query.Parameters.Add(nextKey, DbType.Decimal);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(double))
                    {
                        query.Parameters.Add(nextKey, DbType.Double);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Single))
                    {
                        query.Parameters.Add(nextKey, DbType.Single);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(DateTime))
                    {
                        query.Parameters.Add(nextKey, DbType.DateTime);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(byte[]))
                    {
                        query.Parameters.Add(nextKey, DbType.Binary); // ?
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(System.DBNull))
                    {
                        query.Parameters.Add(nextKey, DbType.String);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(bool))
                    {
                        query.Parameters.Add(nextKey, DbType.Boolean);
                        query.Parameters[nextKey].Value = sqlParams[nextKey];
                    }
                    else
                    {
                        return; // Why did we get here?
                    }
                }
            }
        }

        private void UpdateTransactedDataSize(Dictionary<string, object> sqlParams)
        {
            // Make sure we are being called from a routine that has already acquired the Write lock
            if (_SQLiteDBReadWriteLocker.IsWriteLockHeld == false)
                throw new Exception("UpdateTransactedDataSize requires that the thread already has a WriteLock");

            Int64 sizeOfTheseParams = 0;

            // Build and populate SQL command parameters
            if (sqlParams != null)
            {
                foreach (string nextKey in sqlParams.Keys)
                {
                    // Note: We must check for null first to avoid object reference errors!
                    if (sqlParams[nextKey] == null)
                    {
                        sizeOfTheseParams += 0;
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(string))
                    {
                        sizeOfTheseParams += (sqlParams[nextKey] as string).Length;
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Int32))
                    {
                        sizeOfTheseParams += sizeof(Int32);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Int16))
                    {
                        sizeOfTheseParams += sizeof(Int16);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Int64))
                    {
                        sizeOfTheseParams += sizeof(Int64);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(decimal))
                    {
                        sizeOfTheseParams += sizeof(decimal);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(double))
                    {
                        sizeOfTheseParams += sizeof(double);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(Single))
                    {
                        sizeOfTheseParams += sizeof(Single);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(DateTime))
                    {
                        sizeOfTheseParams += sizeof(Int64);
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(byte[]))
                    {
                        sizeOfTheseParams += (sqlParams[nextKey] as byte[]).Length;
                    }
                    else if (sqlParams[nextKey].GetType() == typeof(bool))
                    {
                        sizeOfTheseParams += sizeof(bool);
                    }
                }
            }

            // Now add these parameter sizes to the total uncommited data size
            this._TransactedSizeOfData += sizeOfTheseParams;
        }

        public int ExecuteNonQuery(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            int result = 0;
            SQLiteConnection dbBConnection = null;
            SQLiteCommand query = null;

            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a write lock for this operation
                    _SQLiteDBReadWriteLocker.EnterWriteLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbBConnection = new SQLiteConnection(_DBConnectStr);
                        dbBConnection.Open();
                    }

                    // Build SQL command
                    query = new SQLiteCommand();

                    if (_Session_sqlTransaction == null)
                        query.Connection = dbBConnection;
                    else
                        query.Connection = _Session_sqlConnection;

                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;

                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    // Calculate size of data that is being inserted
                    UpdateTransactedDataSize(sqlParams);

                    // Execute the SQL command
                    result = query.ExecuteNonQuery();

                    // If executing inside the context of a transaction session, let's see if
                    // its a good time to commit what we have -- we just don't want to commit 
                    // too often, due to SqLite performance
                    if (_Session_sqlTransaction != null)
                    {
                        CheckForCommitAndResumeTrans_MustAlreadyHaveWriteLock();
                    }
                }
                finally
                {
                    // Release the write lock           
                    if (_SQLiteDBReadWriteLocker.IsWriteLockHeld)
                        _SQLiteDBReadWriteLocker.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, ex.ToString() + "; " + BuildQueryInfoForLogging(query), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (query != null)
                    query.Dispose();

                // Free local db connection
                if (dbBConnection != null)
                {
                    dbBConnection.Close();
                    dbBConnection.Dispose();
                }
            }

            // return the result
            return result;
        }

        // GetSingle
        public T GetSingle<T>(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors) where T : class
        {
            SQLiteConnection dbBConnection = null;
            SQLiteCommand query = null;
            T dto = null;

            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a read lock for this operation
                    _SQLiteDBReadWriteLocker.EnterReadLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbBConnection = new SQLiteConnection(_DBConnectStr);
                        dbBConnection.Open();
                    }

                    // Build SQL command
                    query = new SQLiteCommand();

                    if (_Session_sqlTransaction == null)
                        query.Connection = dbBConnection;
                    else
                        query.Connection = _Session_sqlConnection;

                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;

                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    SQLiteDataReader reader = query.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        IDataMapper mapper = new DataMapperFactory().GetMapper(typeof(T));
                        dto = (T)mapper.GetData(reader);
                        reader.Close();
                    }
                }
                finally
                {
                    // Release the read lock           
                    if (_SQLiteDBReadWriteLocker.IsReadLockHeld)
                        _SQLiteDBReadWriteLocker.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, ex.ToString() + "; " + BuildQueryInfoForLogging(query), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (query != null)
                    query.Dispose();

                // If not inside a transaction session, free local db connection
                if (_Session_sqlTransaction == null)
                {
                    if (dbBConnection != null)
                    {
                        dbBConnection.Close();
                        dbBConnection.Dispose();
                    }
                }
            }

            // return the DTO, it's either populated with data or null.
            return dto;
        }

        public DataSet GetDatasetFromSQL(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            DataSet dsResult = null;
            SQLiteConnection dbBConnection = null;
            SQLiteCommand query = null;
            SQLiteDataAdapter dataAdapter = null;

            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a read lock for this operation
                    _SQLiteDBReadWriteLocker.EnterReadLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbBConnection = new SQLiteConnection(_DBConnectStr);
                        dbBConnection.Open();
                    }

                    // Build SQL command
                    query = new SQLiteCommand();

                    if (_Session_sqlTransaction == null)
                        query.Connection = dbBConnection;
                    else
                        query.Connection = _Session_sqlConnection;

                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;

                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    // Fill dataset via the SQL command
                    dsResult = new DataSet();
                    dataAdapter = new SQLiteDataAdapter(query);
                    dataAdapter.Fill(dsResult);
                }
                finally
                {
                    // Release the read lock           
                    if (_SQLiteDBReadWriteLocker.IsReadLockHeld)
                        _SQLiteDBReadWriteLocker.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, ex.ToString() + "; " + BuildQueryInfoForLogging(query), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (query != null)
                    query.Dispose();
                if (dataAdapter != null)
                    dataAdapter.Dispose();

                /*
                if (dsResult != null)
                    dsResult.Dispose();
                */

                // If not inside a transaction session, free local db connection
                if (_Session_sqlTransaction == null)
                {
                    if (dbBConnection != null)
                    {
                        dbBConnection.Close();
                        dbBConnection.Dispose();
                    }
                }
            }

            // return the result
            return dsResult;
        }

        public Int64 ExecuteInsertAndReturnAutoInc(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            Int64 result = 0;
            SQLiteConnection dbConnection = null;
            SQLiteCommand query = null;

            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a write lock for this operation
                    _SQLiteDBReadWriteLocker.EnterWriteLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbConnection = new SQLiteConnection(_DBConnectStr);
                        dbConnection.Open();
                    }

                    // Build SQL command
                    query = new SQLiteCommand();

                    if (_Session_sqlTransaction == null)
                        query.Connection = dbConnection;
                    else
                        query.Connection = _Session_sqlConnection;

                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;

                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    // Calculate size of data that is being inserted
                    UpdateTransactedDataSize(sqlParams);

                    // Execute the SQL command
                    query.ExecuteNonQuery();

                    // Now get the AutoInc value.  This must be done on the same connection!
                    query.Dispose();
                    query = null;
                    query = new SQLiteCommand();
                    if (_Session_sqlTransaction == null)
                        query.Connection = dbConnection;
                    else
                        query.Connection = _Session_sqlConnection;

                    query.CommandText = "SELECT last_insert_rowid()";
                    query.CommandType = CommandType.Text;
                    object scalarResult = query.ExecuteScalar();
                    result = Convert.ToInt64(scalarResult);

                    // If executing inside the context of a transaction session, let's see if
                    // its a good time to commit what we have -- we just don't want to commit 
                    // too often, due to SqLite performance
                    if (_Session_sqlTransaction != null)
                    {
                        CheckForCommitAndResumeTrans_MustAlreadyHaveWriteLock();
                    }
                }
                finally
                {
                    // Release the write lock           
                    if (_SQLiteDBReadWriteLocker.IsWriteLockHeld)
                        _SQLiteDBReadWriteLocker.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, ex.ToString() + "; " + BuildQueryInfoForLogging(query), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (query != null)
                    query.Dispose();

                // Free local db connection
                if (dbConnection != null)
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
            }

            // return the result
            return result;
        }

        public object ExecuteScalar(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            object result = null;
            SQLiteConnection dbConnection = null;
            SQLiteCommand query = null;

            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a write lock for this operation
                    _SQLiteDBReadWriteLocker.EnterWriteLock();

                    // Open database connection if we're not inside a transaction session
                    if (_Session_sqlTransaction == null)
                    {
                        dbConnection = new SQLiteConnection(_DBConnectStr);
                        dbConnection.Open();
                    }

                    // Build SQL command
                    query = new SQLiteCommand();
                    if (_Session_sqlTransaction == null)
                        query.Connection = dbConnection;
                    else
                        query.Connection = _Session_sqlConnection;
                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;

                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    // Calculate size of data that is (possibly) being inserted -- Scalar routine might be just a SELECT, but it could also have an INSERT or UPDATE and still return a value...
                    UpdateTransactedDataSize(sqlParams);

                    // Execute the SQL command
                    result = query.ExecuteScalar();

                    // If executing inside the context of a transaction session, let's see if
                    // its a good time to commit what we have -- we just don't want to commit 
                    // too often, due to SqLite performance
                    if (_Session_sqlTransaction != null)
                    {
                        CheckForCommitAndResumeTrans_MustAlreadyHaveWriteLock();
                    }
                }
                finally
                {
                    // Release the write lock           
                    if (_SQLiteDBReadWriteLocker.IsWriteLockHeld)
                        _SQLiteDBReadWriteLocker.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, ex.ToString() + "; " + BuildQueryInfoForLogging(query), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);

                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (query != null)
                    query.Dispose();

                // Free local db connection
                if (dbConnection != null)
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
            }

            // return the result
            return result;
        }

        public void TryDBConnection()
        {
            SQLiteConnection _DBConnection = null;
            try
            {
                // Open database connection
                _DBConnection = new SQLiteConnection(_DBConnectStr);
                _DBConnection.Open();
                // Close database
                _DBConnection.Close();
                _DBConnection.Dispose();
                _DBConnection = null;
            }
            catch (Exception ex)
            {
                LogMessage(TraceLevel.Error, "Failed connecting to SqLite database: " + ex.ToString(), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw ex;
            }
            finally
            {
                if (_DBConnection != null)
                {
                    _DBConnection.Close();
                    _DBConnection.Dispose();
                }
            }
        }

        private void LogMessage(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID)
        {
            if (this._DefaultUpdateLogCallback != null)
                this._DefaultUpdateLogCallback(logLevel, textToLog, classAndMethod, threadID);
        }

        private string BuildQueryInfoForLogging(SQLiteCommand command)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            try
            {
                result.Append("SQL: " + command.CommandText);
                if ((command.Parameters != null) && (command.Parameters.Count > 0))
                {
                    result.Append("; SQL Params: ");
                    string delim = string.Empty;
                    foreach (SQLiteParameter nextParam in command.Parameters)
                    {
                        result.Append(delim);
                        if (nextParam.Value == null)
                            result.Append(nextParam.ParameterName + "=NULL");
                        else
                            result.Append(nextParam.ParameterName + "=" + nextParam.Value.ToString());
                        if (string.IsNullOrEmpty(delim))
                            delim = ", ";
                    }
                }
            }
            catch { }
            return result.ToString();
        }

    }
}
