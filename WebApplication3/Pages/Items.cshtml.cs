// ItemsModel.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public int NewItemId { get; set; }

        [BindProperty]
        public MenuItem NewMenuItem { get; set; }

        [BindProperty]
        public Item ItemIdToDelete { get; set; }

        [BindProperty]
        public MenuItem UpdatedMenuItem { get; set; }

        public void OnGet()
        {
            MenuItems = GetMenuItems() ?? new List<MenuItem>();
            NewItemId = GetNextId();
        }

        public int GetNextId()
        {
            int nextId = 1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(ItemID) FROM MenuItem";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        nextId = Convert.ToInt32(result) + 1;
                    }
                }
            }

            return nextId;
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

        public IActionResult OnPost()
        {
            switch (Request.Form["handler"])
            {
                case "delete":
                    return DeleteMenuItem(ItemIdToDelete);

                default:
                    return RedirectToPage();
            }
        }


        public IActionResult DeleteMenuItem(Item item)
        {
            string resultMessage;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"DELETE FROM MenuItem WHERE ItemID = {item.itemid}";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    resultMessage = rowsAffected > 0
                        ? "Item deleted successfully."
                        : "Item not found or deletion failed.";
                }
            }

            TempData["ResultMessage"] = resultMessage;
            return RedirectToPage();
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
        public Item item { get; set; }
        public class Item
        {
            [BindProperty]
            public string itemname { get; set; }
            [BindProperty]
            public int itemid { get; set; }

            [BindProperty]
            public string description { get; set; }
            [BindProperty]
            public string imagepath { get; set; }
            [BindProperty]
            public string category { get; set; }
            [BindProperty]
            public decimal price { get; set; }


        }

    }
}
