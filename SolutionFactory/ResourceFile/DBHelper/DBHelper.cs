using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SolutionFactory.DBAccess
{
    /// <summary>
    /// SqlServer database operations class.
    /// </summary>
    internal partial class DBHelper : HelperBase 
    {
        public DBHelper()
        {
            ConnectionString = default_connection_str;
            Connection = new SqlConnection();
            Command = Connection.CreateCommand();
        }

        public DBHelper(int ConnectionStringsIndex)
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsIndex].ConnectionString;
            Connection = new SqlConnection();
            Command = Connection.CreateCommand();
        }

        public DBHelper(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
            Connection = new SqlConnection();
            Command = Connection.CreateCommand();
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, object value)
        {
            return AddParameter(ParameterName, type, value, ParameterDirection.Input);
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, object value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter(ParameterName, type);
            param.Value = value;
            param.Direction = direction;
            Command.Parameters.Add(param);
            return param;
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, int size, object value)
        {
            return AddParameter(ParameterName, type, size, value, ParameterDirection.Input);
        }

        public SqlParameter AddParameter(string ParameterName, SqlDbType type, int size, object value, ParameterDirection direction)
        {
            SqlParameter param = new SqlParameter(ParameterName, type, size);
            param.Direction = direction;
            param.Value = value;
            Command.Parameters.Add(param);
            return param;
        }

        public void AddRangeParameters(SqlParameter[] parameters)
        {
            Command.Parameters.AddRange(parameters);
        }
    }
}
