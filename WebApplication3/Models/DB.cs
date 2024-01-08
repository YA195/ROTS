using System;
using System.Data;
using System.Data.SqlClient;
using WebApplication3.Pages; // Add this line

namespace WebApplication3.Models
{
    public class DB
    {
        private readonly string _connectionString;

        public DB(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to read data from a table and return as DataTable
        public DataTable CustomQuery(string query)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(query, connection);
                    dt.Load(cmd.ExecuteReader());
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    throw new Exception("Error executing the query.", ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return dt;
        }

        

        

      
    }
}
