using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WebApplication3.Models;

namespace WebApplication3.Pages
{
    public class menuModel : PageModel
    {
        private readonly string _connectionString;
        private readonly DB _db;

        public menuModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _db = new DB(_connectionString);
        }

        // MenuItem class definition
        public class MenuItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public string ImagePath { get; set; }
            public string Category { get; set; }
        }

        // Static method to get the image path based on item type


        // Property to store the list of menu items
        public List<MenuItem> MenuItems { get; private set; }

        // OnGet method to retrieve data from the database
        public async Task OnGetAsync()
        {
            // Modify your SQL query to include ORDER BY NEWID() and TOP 10
            string query = "SELECT * FROM MenuItem ORDER BY NEWID()";

            DataTable dataTable = await Task.Run(() => _db.CustomQuery(query));
            MenuItems = new List<MenuItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                MenuItems.Add(new MenuItem
                {
                    Id = Convert.ToInt32(row["ItemID"]),
                    Name = row["Name"].ToString(),
                    Description = row["Description"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    ImagePath = row["ImagePath"].ToString(), // Use the ImagePath directly from the database
                    Category = row["Category"].ToString()
                });
            }
        }
    }
}
