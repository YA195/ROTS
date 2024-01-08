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

        public ItemsModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public int NewItemId { get; set; }

        [BindProperty]
        public MenuItem NewMenuItem { get; set; }

        public int ItemIdToDelete { get; set; }

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

        [HttpPost]
        [HttpPost]
        public IActionResult OnPost()
        {
            try
            {
                switch (Request.Form["handler"])
                {
                    case "delete":
                        int selectedItemId = Convert.ToInt32(Request.Form["itemid"]);
                        return DeleteMenuItem(selectedItemId);

                    case "update":
                        // Extract parameters from the form submission
                        int itemIdToUpdate = Convert.ToInt32(Request.Form["itemid"]);
                        string itemNameToUpdate = Request.Form["itemname"];
                        string descriptionToUpdate = Request.Form["description"];
                        string imagePathToUpdate = Request.Form["imagepath"];
                        string categoryToUpdate = Request.Form["category"];
                        decimal priceToUpdate = Convert.ToDecimal(Request.Form["price"]);

                        UpdateMenuItem(itemIdToUpdate, itemNameToUpdate, descriptionToUpdate, priceToUpdate, imagePathToUpdate, categoryToUpdate);
                        break;

                    case "add":
                        // Extract parameters for adding a new item
                        int itemIdToAdd = Convert.ToInt32(Request.Form["itemid"]);
                        string itemNameToAdd = Request.Form["itemName"];
                        string descriptionToAdd = Request.Form["description"];
                        string imagePathToAdd = Request.Form["imagePath"];
                        string categoryToAdd = Request.Form["category"];
                        decimal priceToAdd = Convert.ToDecimal(Request.Form["price"]);

                        AddMenuItem(itemIdToAdd, itemNameToAdd, descriptionToAdd, priceToAdd, imagePathToAdd, categoryToAdd);
                        break;
                        // item to add itemname to add item path to add description to addd image pathh to add category to add 
                        

                        // Handle other cases as needed

                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData["ResultMessage"] = $"Error: {ex.Message}";
                return RedirectToPage();
            }
        }


        [HttpPost]
        public IActionResult UpdateMenuItem(
            [FromForm(Name = "itemid")] int itemId,
            [FromForm(Name = "itemname")] string itemName,
            [FromForm(Name = "description")] string description,
            [FromForm(Name = "price")] decimal price,
            [FromForm(Name = "imagepath")] string imagePath,
            [FromForm(Name = "category")] string category)
        {
            try
            {
                if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(imagePath) || string.IsNullOrEmpty(category))
                {
                    TempData["ResultMessage"] = "Error updating item: Some parameters are null or empty.";
                    return RedirectToPage();
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE MenuItem SET Name = @Name, Description = @Description, " +
                                   "Price = @Price, ImagePath = @ImagePath, Category = @Category " +
                                   "WHERE ItemID = @ItemID";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                    cmd.Parameters.AddWithValue("@Name", itemName);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    cmd.Parameters.AddWithValue("@Category", category);

                    cmd.ExecuteNonQuery();

                    TempData["ResultMessage"] = $"Item with ID {itemId} updated successfully.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData["ResultMessage"] = $"Error updating item: {ex.Message}";
                return RedirectToPage();
            }
        }

        [HttpPost]
        public IActionResult DeleteMenuItem([FromForm(Name = "itemid")] int itemId)
        {
            string resultMessage;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"DELETE FROM MenuItem WHERE ItemID = @ItemID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ItemID", itemId);

                    int rowsAffected = command.ExecuteNonQuery();

                    resultMessage = rowsAffected > 0
                        ? $"Item with ID {itemId} deleted successfully."
                        : $"Item with ID {itemId} not found or deletion failed.";
                }
            }

            TempData["ResultMessage"] = resultMessage;
            return RedirectToPage();
        }
        public IActionResult AddMenuItem(
     [FromForm(Name = "itemid")] int itemId,
     [FromForm(Name = "itemname")] string itemName,
     [FromForm(Name = "description")] string description,
     [FromForm(Name = "price")] decimal price,
     [FromForm(Name = "imagepath")] string imagePath,
     [FromForm(Name = "category")] string category)
        {
            try
            {
                // Check if any of the required parameters are null or empty
                if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(imagePath) || string.IsNullOrEmpty(category))
                {
                    TempData["ResultMessage"] = "Error adding item: Some parameters are null or empty.";
                    return RedirectToPage();
                }

                // Check if the item with the specified ItemID exists
                if (GetItemById(itemId) != null)
                {
                    TempData["ResultMessage"] = $"Error adding item: Item with ID {itemId} already exists.";
                    return RedirectToPage();
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Insert the new item
                    string query = "INSERT INTO MenuItem (ItemID, Name, Description, Price, ImagePath, Category) " +
                                   "VALUES (@ItemID, @Name, @Description, @Price, @ImagePath, @Category)";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                    cmd.Parameters.AddWithValue("@Name", itemName);
                    cmd.Parameters.AddWithValue("@Description", description);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    cmd.Parameters.AddWithValue("@Category", category);

                    cmd.ExecuteNonQuery();

                    TempData["ResultMessage"] = $"Item with ID {itemId} added successfully.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData["ResultMessage"] = $"Error adding item: {ex.Message}";
                return RedirectToPage();
            }
        }

        

        // Function to check if an item with a specific ID exists
        private MenuItem GetItemById(int itemId)
        {
            // Implement your logic to retrieve an item by ID from the database
            // You can use the existing GetMenuItems() method and filter the result
            return GetMenuItems()?.FirstOrDefault(item => item.ItemID == itemId);
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
    }
}
