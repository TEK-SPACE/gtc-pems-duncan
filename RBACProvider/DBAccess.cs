using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace RBACToolbox
{
    public class DBAccess
    {
        private string _DBConnectStr = string.Empty;

        internal string DBConnectStr
        {
            get { return _DBConnectStr; }
        }

        public DBAccess(string dbConnectionString)
        {
            _DBConnectStr = dbConnectionString;
        }

        public DataTable GetSchema(string collectionName, string[] restrictionValues, bool throwErrors)
        {
            SqlConnection _DBConnection = null;
            DataTable result = null;
            try
            {
                // Open database connection
                // Create SQL connection and data access object  
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(_DBConnectStr);
                dbConnectBuilder.Pooling = true;
                dbConnectBuilder.MaxPoolSize = 200;
                dbConnectBuilder.ConnectTimeout = 30;
                dbConnectBuilder.MultipleActiveResultSets = true;
                _DBConnection = new SqlConnection(dbConnectBuilder.ToString());
                _DBConnection.Open();

                result = _DBConnection.GetSchema(collectionName, restrictionValues);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (throwErrors)
                    throw ex;
                return result;
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

        public DataTable GetSchema(string collectionName, bool throwErrors)
        {
            SqlConnection _DBConnection = null;
            DataTable result = null;
            try
            {
                // Open database connection
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(_DBConnectStr);
                dbConnectBuilder.Pooling = true;
                dbConnectBuilder.MaxPoolSize = 200;
                dbConnectBuilder.ConnectTimeout = 30;
                dbConnectBuilder.MultipleActiveResultSets = true;
                _DBConnection = new SqlConnection(dbConnectBuilder.ToString());
                _DBConnection.Open();

                result = _DBConnection.GetSchema(collectionName);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (throwErrors)
                    throw ex;
                return result;
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

        public int ExecuteNonQuery(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            int result = 0;
            SqlConnection _DBConnection = null;
            SqlCommand _Query = null;

            try
            {
                // Open database connection
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(_DBConnectStr);
                dbConnectBuilder.Pooling = true;
                dbConnectBuilder.MaxPoolSize = 200;
                dbConnectBuilder.ConnectTimeout = 30;
                dbConnectBuilder.MultipleActiveResultSets = true;
                _DBConnection = new SqlConnection(dbConnectBuilder.ToString());
                _DBConnection.Open();

                // Build SQL command
                _Query = new SqlCommand();
                _Query.Connection = _DBConnection;
                _Query.CommandText = SQLCommandText;
                _Query.CommandType = CommandType.Text;

                // Build and populate SQL command parameters
                if (sqlParams != null)
                {
                    foreach (string nextKey in sqlParams.Keys)
                    {
                        if (sqlParams[nextKey].GetType() == typeof(string))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.VarChar);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Int32))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Int);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Int16))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Int);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Int64))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.BigInt);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(double))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Decimal);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Single))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Decimal);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(DateTime))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.DateTime);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(byte[]))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Binary);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(System.DBNull))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.VarChar);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                    }
                }

                // Execute the SQL command
                result = _Query.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (_Query != null)
                    _Query.Dispose();
                if (_DBConnection != null)
                {
                    _DBConnection.Close();
                    _DBConnection.Dispose();
                }
            }

            // return the result
            return result;
        }

        public DataSet GetDatasetFromSQL(string SQLCommandText, Dictionary<string, object> sqlParams, bool throwErrors)
        {
            DataSet dsResult = null;
            SqlConnection _DBConnection = null;
            SqlCommand _Query = null;
            SqlDataAdapter dataAdapter = null;

            try
            {
                // Open database connection
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(_DBConnectStr);
                dbConnectBuilder.Pooling = true;
                dbConnectBuilder.MaxPoolSize = 200;
                dbConnectBuilder.ConnectTimeout = 30;
                dbConnectBuilder.MultipleActiveResultSets = true;
                _DBConnection = new SqlConnection(dbConnectBuilder.ToString());
                _DBConnection.Open();

                // Build SQL command
                _Query = new SqlCommand();
                _Query.Connection = _DBConnection;
                _Query.CommandText = SQLCommandText;
                _Query.CommandType = CommandType.Text;

                // Build and populate SQL command parameters
                if (sqlParams != null)
                {
                    foreach (string nextKey in sqlParams.Keys)
                    {
                        if (sqlParams[nextKey].GetType() == typeof(string))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.VarChar);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Int32))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Int);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Int16))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Int);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Int64))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.BigInt);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(double))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Decimal);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(Single))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Decimal);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(DateTime))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.DateTime);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(byte[]))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.Binary);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                        else if (sqlParams[nextKey].GetType() == typeof(System.DBNull))
                        {
                            _Query.Parameters.Add(nextKey, SqlDbType.VarChar);
                            _Query.Parameters[nextKey].Value = sqlParams[nextKey];
                        }
                    }
                }

                // Fill dataset via the SQL command
                dsResult = new DataSet();
                dataAdapter = new SqlDataAdapter(_Query);
                dataAdapter.Fill(dsResult);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (throwErrors)
                    throw ex;
            }
            finally
            {
                if (_Query != null)
                    _Query.Dispose();
                if (dataAdapter != null)
                    dataAdapter.Dispose();
                
                // Don't dispose the object that we are returning to caller
                /*
                if (dsResult != null)
                    dsResult.Dispose();
                */
                
                if (_DBConnection != null)
                {
                    _DBConnection.Close();
                    _DBConnection.Dispose();
                }
            }

            // return the result
            return dsResult;
        }

        public void TryDBConnection()
        {
            SqlConnection _DBConnection = null;
            try
            {
                // Open database connection
                SqlConnectionStringBuilder dbConnectBuilder = new SqlConnectionStringBuilder(_DBConnectStr);
                dbConnectBuilder.Pooling = true;
                dbConnectBuilder.MaxPoolSize = 200;
                dbConnectBuilder.ConnectTimeout = 30;
                dbConnectBuilder.MultipleActiveResultSets = true;
                _DBConnection = new SqlConnection(dbConnectBuilder.ToString());
                _DBConnection.Open();
                // Close database
                _DBConnection.Close();
                _DBConnection.Dispose();
                _DBConnection = null;
            }
            catch (Exception ex)
            {
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
    }
}
