using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.Pages
{
    public class ItemsModel : PageModel
    {
        private readonly string _connectionString;
        private readonly DB _db;

        public ItemsModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _db = new DB(_connectionString);
        }
        public List<MenuItem> MenuItems { get; set; }

        public void OnGet()
        {
            MenuItems = GetMenuItems();
        }
        // MenuItem.cs
        public class MenuItem
        {
            public int ItemID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImagePath { get; set; }
            public string Category { get; set; }
        }


        private List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM MenuItem";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            menuItems.Add(new MenuItem
                            {
                                ItemID = Convert.ToInt32(reader["ItemID"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImagePath = reader["ImagePath"].ToString(),
                                Category = reader["Category"].ToString()
                            });
                        }
                    }
                }
            }

            return menuItems;
        }
    }
}
