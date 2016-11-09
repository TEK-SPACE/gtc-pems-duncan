using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Web;

using Duncan.PEMS.SpaceStatus.DataMappers;

namespace Duncan.PEMS.SpaceStatus.DataSuppliers
{
    public class NullValues
    {
        public static DateTime NullDateTime = DateTime.MinValue;
        public static Guid NullGuid = Guid.Empty;
        public static int NullInt = int.MinValue;
        public static float NullFloat = float.MinValue;
        public static decimal NullDecimal = decimal.MinValue;
        public static string NullString = null;
    }

    public class SqlServerDAO
    {
        public delegate void UpdateLogCallBack(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID);
        protected UpdateLogCallBack _DefaultUpdateLogCallback = null;

        #region "Database Helper Methods"

        private string _DBConnectionString = string.Empty;

        // Constructors
        public SqlServerDAO(string dbConnectionString, UpdateLogCallBack iCallBackDelegate)
        {
            string dbString = System.Configuration.ConfigurationManager.
    ConnectionStrings[dbConnectionString].ConnectionString;

          
           if (dbString.Contains("provider connection string"))
           {
               dbString = dbString.Substring(dbString.IndexOf("provider connection string") + dbString.IndexOf("data source") - dbString.IndexOf("provider connection string"));
           dbString = dbString.Substring(0,dbString.IndexOf("App="));
           
           }

            _DefaultUpdateLogCallback = iCallBackDelegate;          
            SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(dbString);
            dbConnectBuilder.Pooling = true;
            dbConnectBuilder.MaxPoolSize = 200;
            dbConnectBuilder.ConnectTimeout = 30;
            dbConnectBuilder.MultipleActiveResultSets = false;
            _DBConnectionString = dbConnectBuilder.ToString();
        }

        // GetDbSqlCommand
        public SqlCommand GetSqlCommand(string sqlQuery)
        {
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sqlQuery;
            return command;
        }

        // GetDbSprocCommand
        public SqlCommand GetSprocCommand(string sprocName)
        {
            SqlCommand command = new SqlCommand(sprocName);
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }

        // CreateNullParameter
        public SqlParameter CreateNullParameter(string name, SqlDbType paramType)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.SqlDbType = paramType;
            parameter.ParameterName = name;
            parameter.Value = null;
            parameter.Direction = ParameterDirection.Input;
            return parameter;
        }

        // CreateNullParameter - with size for nvarchars
        public SqlParameter CreateNullParameter(string name, SqlDbType paramType, int size)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.SqlDbType = paramType;
            parameter.ParameterName = name;
            parameter.Size = size;
            parameter.Value = null;
            parameter.Direction = ParameterDirection.Input;
            return parameter;
        }

        // CreateOutputParameter
        public SqlParameter CreateOutputParameter(string name, SqlDbType paramType)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.SqlDbType = paramType;
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        // CreateOuputParameter - with size for nvarchars
        public SqlParameter CreateOutputParameter(string name, SqlDbType paramType, int size)
        {
            SqlParameter parameter = new SqlParameter();
            parameter.SqlDbType = paramType;
            parameter.Size = size;
            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        // CreateParameter - uniqueidentifier
        public SqlParameter CreateParameter(string name, Guid value)
        {
            if (value.Equals(NullValues.NullGuid))
            {
                // If value is null then create a null parameter
                return CreateNullParameter(name, SqlDbType.UniqueIdentifier);
            }
            else
            {
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = name;
                parameter.Value = value;
                parameter.Direction = ParameterDirection.Input;
                return parameter;
            }
        }

        // CreateParameter - int
        public SqlParameter CreateParameter(string name, int value)
        {
            if (value == NullValues.NullInt)
            {
                // If value is null then create a null parameter
                return CreateNullParameter(name, SqlDbType.Int);
            }
            else
            {
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.Int;
                parameter.ParameterName = name;
                parameter.Value = value;
                parameter.Direction = ParameterDirection.Input;
                return parameter;
            }
        }

        // CreateParameter - bigint
        public SqlParameter CreateParameter(string name, Int64 value)
        {
            if (value == NullValues.NullInt)
            {
                // If value is null then create a null parameter
                return CreateNullParameter(name, SqlDbType.BigInt);
            }
            else
            {
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.BigInt;
                parameter.ParameterName = name;
                parameter.Value = value;
                parameter.Direction = ParameterDirection.Input;
                return parameter;
            }
        }

        // CreateParameter - datetime
        public SqlParameter CreateParameter(string name, DateTime value)
        {
            if (value == NullValues.NullDateTime)
            {
                // If value is null then create a null parameter
                return CreateNullParameter(name, SqlDbType.DateTime);
            }
            else
            {
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.DateTime;
                parameter.ParameterName = name;
                parameter.Value = value;
                parameter.Direction = ParameterDirection.Input;
                return parameter;
            }
        }

        // CreateParameter - nvarchar
        public SqlParameter CreateParameter(string name, string value, int size)
        {
            if (String.IsNullOrEmpty(value))
            {
                // If value is null then create a null parameter
                return CreateNullParameter(name, SqlDbType.NVarChar);
            }
            else
            {
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.Size = size;
                parameter.ParameterName = name;
                parameter.Value = value;
                parameter.Direction = ParameterDirection.Input;
                return parameter;
            }
        }

        private void LogMessage(System.Diagnostics.TraceLevel logLevel, string textToLog, string classAndMethod, int threadID)
        {
            if (this._DefaultUpdateLogCallback != null)
                this._DefaultUpdateLogCallback(logLevel, textToLog, classAndMethod, threadID);
        }

        private string BuildQueryInfoForLogging(SqlCommand command)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            if (command == null)
                return result.ToString();

            try
            {
                result.Append("SQL: " + command.CommandText);
                if ((command.Parameters != null) && (command.Parameters.Count > 0))
                {
                    result.Append("; SQL Params: ");
                    string delim = string.Empty;
                    foreach (SqlParameter nextParam in command.Parameters)
                    {
                        result.Append(delim);
                        if (nextParam.Value == null)
                            result.Append(nextParam.ParameterName + "=NULL");
                        else
                            result.Append(nextParam.ParameterName + "=" + nextParam.Value.ToString());
                        if (string.IsNullOrEmpty(delim)) ;
                        delim = ", ";
                    }
                }
            }
            catch { }
            return result.ToString();
        }
        #endregion


        #region "Data Projection Methods"

        // ExecuteNonQuery
        public int ExecuteNonQuery(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error executing query: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error executing query", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
        }

        // ExecuteScalar
        public Object ExecuteScalar(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error executing query: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error executing query", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
        }

        // GetSingleValue
        public T GetSingleValue<T>(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            T returnValue = default(T);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (!reader.IsDBNull(0)) { returnValue = (T)reader[0]; }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return returnValue;
        }

        // GetSingleInt32
        public Int32 GetSingleInt32(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            Int32 returnValue = default(int);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (!reader.IsDBNull(0)) { returnValue = reader.GetInt32(0); }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return returnValue;
        }

        // GetSingleBigInt
        public Int64 GetSingleBigInt(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            Int64 returnValue = default(Int64);
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (!reader.IsDBNull(0)) { returnValue = reader.GetInt64(0); }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return returnValue;
        }

        // GetSingleString
        public string GetSingleString(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            string returnValue = null;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    if (!reader.IsDBNull(0)) { returnValue = reader.GetString(0); }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return returnValue;
        }

        // GetStringList
        public List<string> GetStringList(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            List<string> returnList = null;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    returnList = new List<string>();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0)) { returnList.Add(reader.GetString(0)); }
                    }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return returnList;
        }

        // GetSingle
        public T GetSingle<T>(SqlCommand command) where T : class
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            T dto = null;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    IDataMapper mapper = new DataMapperFactory().GetMapper(typeof(T));
                    dto = (T)mapper.GetData(reader);
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            // return the DTO, it's either populated with data or null.
            return dto;
        }

        // GetList
        public List<T> GetList<T>(SqlCommand command) where T : class
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            List<T> dtoList = new List<T>();
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    IDataMapper mapper = new DataMapperFactory().GetMapper(typeof(T));
                    while (reader.Read())
                    {
                        T dto = null;
                        dto = (T)mapper.GetData(reader);
                        dtoList.Add(dto);
                    }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            // We return either the populated list if there was data,
            // or if there was no data we return an empty list.
            return dtoList;
        }

        // GetResultRowCount
        public int GetResultRowCount(SqlCommand command)
        {
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            int result = 0;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result++;
                    }
                    reader.Close();
                    reader.Dispose();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error executing query", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return result;
        }


        public Int64 ExecuteInsertAndReturnAutoInc(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            Int64 result = 0;

            SqlCommand query = null;
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            
            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {
                    // Use a write lock for this operation


                    // Build SQL command
                    query = new SqlCommand();


                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;
                    query.Connection = dbConnect;
                    if (query.Connection.State != ConnectionState.Open)
                    {
                        query.Connection.Open();
                    }
                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    // Calculate size of data that is being inserted
                    UpdateTransactedDataSize(sqlParams);

                    // Execute the SQL command
                    query.ExecuteNonQuery();

                    // Now get the AutoInc value.  This must be done on the same connection!
                    query.Dispose();
                    query = null;
                    query = new SqlCommand();


                    query.CommandText = "SELECT Max(UniqueKey) from OverstayVioActions";
                    query.CommandType = CommandType.Text;
                    query.Connection = dbConnect;
                    if (query.Connection.State != ConnectionState.Open)
                    {
                        query.Connection.Open();
                    }
                    object scalarResult = query.ExecuteScalar();
                    result = Convert.ToInt64(scalarResult);

                    // If executing inside the context of a transaction session, let's see if
                    // its a good time to commit what we have -- we just don't want to commit 
                    // too often, due to SqLite performance

                }
                finally
                {

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
                dbConnect.Close();
                dbConnect.Dispose();
                dbConnect = null;
            }

            // return the result
            return result;
        }





        private void SetQueryParameters(SqlCommand query, Dictionary<string, object> sqlParams)
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
           // this._TransactedSizeOfData += sizeOfTheseParams;
        }

        public T GetSingle<T>(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors) where T : class
        {
            SqlConnection dbBConnection = new SqlConnection(_DBConnectionString);
            SqlCommand query = null;
            T dto = null;

            try
            {
                // Protect this section with our ReaderWriterLockSlim that handles concurrency issues for us
                try
                {


                    // Build SQL command
                    query = new SqlCommand();
                    query.Connection = dbBConnection;
                    if (query.Connection.State != ConnectionState.Open)
                    {
                        query.Connection.Open();
                    }

                    query.CommandText = SQLCommandText;
                    query.CommandType = CommandType.Text;

                    // Build and populate SQL command parameters
                    SetQueryParameters(query, sqlParams);

                    SqlDataReader reader = query.ExecuteReader();
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

                if (dbBConnection != null)
                {
                    dbBConnection.Close();
                    dbBConnection.Dispose();
                }

            }

            // return the DTO, it's either populated with data or null.
            return dto;
        }



        /*
        // GetDataPage
        public DataPage<T> GetDataPage<T>(SqlCommand command, int pageIndex, int pageSize) where T : class
        {   
            SqlConnection dbConnect = new SqlConnection(_DBConnectionString);
            command.Connection = dbConnect;

            DataPage<T> page = new DataPage<T>();
            page.PageIndex = pageIndex;
            page.PageSize = pageSize;
            try
            {
                if (command.Connection.State != ConnectionState.Open)
                {
                    command.Connection.Open();
                }
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    IDataMapper mapper = new DataMapperFactory().GetMapper(typeof(T));
                    while (reader.Read())
                    {
                        // get the data for this row
                        T dto = null;
                        dto = (T)mapper.GetData(reader);
                        page.Data.Add(dto);
                        // If we haven't set the RecordCount yet then set it
                        if (page.RecordCount == 0) { page.RecordCount = mapper.GetRecordCount(reader); }
                    }
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                LogMessage(TraceLevel.Error, "Error populating data: " + e.ToString() + "; " + BuildQueryInfoForLogging(command), System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, System.Threading.Thread.CurrentThread.ManagedThreadId);
                throw new Exception("Error populating data", e);
            }
            finally
            {
                try
                {
                    dbConnect.Close();
                    command.Connection = null;
                }
                catch { }
                dbConnect.Dispose();
                dbConnect = null;
            }
            return page;
        }
        */

        #endregion
    }
}