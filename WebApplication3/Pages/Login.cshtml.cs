using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly string _connectionString;

        public LoginModel(ILogger<LoginModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public IActionResult OnPost()
        {
            try
            {
                switch (Request.Form["handler"])
                {
                    case "login":
                        // Extract parameters for login
                        string usernameToLogin = Request.Form["lusername"];
                        string passwordToLogin = Request.Form["lpassword"];

                        return OnPostLoginUser(usernameToLogin, passwordToLogin);

                    case "register":
                        // Extract parameters for user registration
                        string firstNameToRegister = Request.Form["firstName"];
                        string lastNameToRegister = Request.Form["lastName"];
                        string usernameToRegister = Request.Form["username"];
                        string passwordToRegister = Request.Form["password"];
                        string confirmPasswordToRegister = Request.Form["confirmPassword"];
                        string phoneNumToRegister = Request.Form["phoneNum"];

                        return OnPostRegisterUser(firstNameToRegister, lastNameToRegister, usernameToRegister, passwordToRegister, confirmPasswordToRegister, phoneNumToRegister);

                    default:
                        TempData["ErrorMessage"] = "Invalid handler.";
                        return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during form submission: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred during form submission. Please try again.";
                return Page();
            }
        }




        public IActionResult OnPostLoginUser(
     [FromForm(Name = "lusername")] string lusername,
     [FromForm(Name = "lpassword")] string lpassword)
        {
            try
            {
                if (lusername.StartsWith("mngr"))
                {
                    // Do something for lusername starting with "mngr"
                }
                else if (lusername.StartsWith("dliv"))
                {
                    // Do something for lusername starting with "dliv"
                }
                else if (lusername.StartsWith("sup"))
                {
                    // Do something for lusername starting with "sup"
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM Customer WHERE UserName = @UserName AND Password = @Password";
                        SqlCommand cmd = new SqlCommand(query, connection);

                        cmd.Parameters.AddWithValue("@UserName", lusername);
                        cmd.Parameters.AddWithValue("@Password", lpassword);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return RedirectToPage("/Index");
                            }
                            else
                            {
                                TempData["ResultMessage"] = $"Error login user: {lusername}";
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData["ResultMessage"] = $"Error logging in user: {ex.Message}";
                TempData["WrapperActive"] = "active"; // Set TempData to indicate the wrapper should be active
                return Page();
            }
        }


        public IActionResult OnPostRegisterUser(
     [FromForm(Name = "firstName")] string firstName,
     [FromForm(Name = "lastName")] string lastName,
     [FromForm(Name = "username")] string username,
     [FromForm(Name = "password")] string password,
     [FromForm(Name = "confirmPassword")] string confirmPassword,
     [FromForm(Name = "phoneNum")] string phoneNum)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // If there are validation errors, return the same page with errors
                    TempData["ResultMessage"] = "Error registering user: Validation failed.";
                    TempData["WrapperActive"] = "active"; // Set TempData to indicate the wrapper should be active
                    return Page();
                }

                // Check if the user with the specified username exists
                if (UserExists(username))
                {
                    TempData["ResultMessage"] = $"Error registering user: User with username {username} already exists.";
                    TempData["WrapperActive"] = "active"; // Set TempData to indicate the wrapper should be active
                    return Page();
                }

                // Check if the password and confirmation match
                if (!password.Equals(confirmPassword))
                {
                    TempData["ResultMessage"] = "Error registering user: Password and confirmation do not match.";
                    TempData["WrapperActive"] = "active"; // Set TempData to indicate the wrapper should be active
                    return Page();
                }
                if (username.StartsWith("mngr") || username.StartsWith("dliv") || username.StartsWith("sup"))
                {
                    TempData["ResultMessage"] = $"Error registering user: User with username {username} already exists.";
                    TempData["WrapperActive"] = "active";
                    return Page();
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();

                        // Insert the new user
                        string query = "INSERT INTO Customer (FName, LName, UserName, Password, Phone) " +
                                       "VALUES (@FirstName, @LastName, @UserName, @Password, @Phone)";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Password", password); // Store plain text password
                        cmd.Parameters.AddWithValue("@Phone", phoneNum);
                        cmd.ExecuteNonQuery();
                        TempData["ResultMessage"] = $"User {username} registered successfully.";
                    }
                }
                return RedirectToPage("./Login"); // Redirect to the login page after successful registration
            }
            catch (Exception ex)
            {
                // Handle exceptions
                TempData["ResultMessage"] = $"Error registering user: {ex.Message}";
                TempData["WrapperActive"] = "active"; // Set TempData to indicate the wrapper should be active
                return Page();
            }
        }



        private bool UserExists([FromForm(Name = "username")] string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Customer WHERE UserName = @UserName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserName", username);

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

       
    }
}
