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

        // Method to add a new MenuItem
        public string AddMenuItem(ItemsModel.MenuItem menuItem)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO MenuItem (ItemID, Name, Description, Price, ImagePath, Category) " +
                                   "VALUES (@ItemID, @Name, @Description, @Price, @ImagePath, @Category)";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@ItemID", menuItem.ItemID);
                    cmd.Parameters.AddWithValue("@Name", menuItem.Name);
                    cmd.Parameters.AddWithValue("@Description", menuItem.Description);
                    cmd.Parameters.AddWithValue("@Price", menuItem.Price);
                    cmd.Parameters.AddWithValue("@ImagePath", menuItem.ImagePath);
                    cmd.Parameters.AddWithValue("@Category", menuItem.Category);

                    cmd.ExecuteNonQuery();

                    return "Item added successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    return $"Error adding item: {ex.Message}";
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Method to delete a MenuItem by ItemID
        public string DeleteMenuItem(ItemsModel.Item item)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM MenuItem WHERE ItemID = @itemid";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ItemID", item.itemid);
                    cmd.ExecuteNonQuery();

                    return "Item deleted successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    return $"Error deleting item: {ex.Message}";
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Method to update a MenuItem
        public string UpdateMenuItem(ItemsModel.MenuItem menuItem)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE MenuItem SET Name = @Name, Description = @Description, " +
                                   "Price = @Price, ImagePath = @ImagePath, Category = @Category " +
                                   "WHERE ItemID = @ItemID";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@ItemID", menuItem.ItemID);
                    cmd.Parameters.AddWithValue("@Name", menuItem.Name);
                    cmd.Parameters.AddWithValue("@Description", menuItem.Description);
                    cmd.Parameters.AddWithValue("@Price", menuItem.Price);
                    cmd.Parameters.AddWithValue("@ImagePath", menuItem.ImagePath);
                    cmd.Parameters.AddWithValue("@Category", menuItem.Category);

                    cmd.ExecuteNonQuery();

                    return "Item updated successfully.";
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    return $"Error updating item: {ex.Message}";
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
