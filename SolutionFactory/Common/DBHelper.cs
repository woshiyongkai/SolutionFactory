using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SolutionFactory
{
    public class DBHelper
    {
        public static SqlConnection getConnection()
        {
            SqlConnection conn = new SqlConnection(GlobalProperty.ConnectionString);
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                else
                {
                    conn.Close();
                    conn.Open();
                }
            }
            catch (Exception)
            {
                conn.Close();
                return null;
            }
            return conn;
        }
        //查询，返回datatable
        public static DataTable ReadTable(string sql)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = getConnection();
            if (conn == null)
            {
                return null;
            }
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataAdapter adapt = new SqlDataAdapter(comm);
            adapt.Fill(dt);
            conn.Close();
            return dt;
        }
        //返回Scalar
        public static Object GetScalar(String sql)
        {
            SqlCommand comm = new SqlCommand(sql, getConnection());
            return comm.ExecuteScalar();
        }
    }
}
