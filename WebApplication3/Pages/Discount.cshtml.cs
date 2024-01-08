// DiscountModel.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebApplication3.Models;

namespace WebApplication3.Pages
{
    public class DiscountModel : PageModel
    {
        private readonly string _connectionString;

        public DiscountModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Coupon> Coupons { get; set; } = new List<Coupon>();
        public int NewCouponId { get; set; }

        [BindProperty]
        public Coupon NewCoupon { get; set; }

        public int CouponIdToDelete { get; set; }

        [BindProperty]
        public Coupon UpdatedCoupon { get; set; }

        public void OnGet()
        {
            Coupons = GetCoupons() ?? new List<Coupon>();
            NewCouponId = GetNextCouponId();
        }

        public int GetNextCouponId()
        {
            int nextId = 1;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT MAX(CouponID) FROM Coupon";
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

        private List<Coupon> GetCoupons()
        {
            List<Coupon> coupons = new List<Coupon>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Coupon";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coupons.Add(new Coupon
                            {
                                CouponID = Convert.ToInt32(reader["CouponID"]),
                                Code = reader["Code"].ToString(),
                                DiscountPercentage = Convert.ToInt32(reader["DiscountPercentage"]),
                                ExpiryDate = Convert.ToDateTime(reader["ExpiryDate"]),
                                MinimumTotal = Convert.ToInt32(reader["MinimumTotal"])
                            });
                        }
                    }
                }
            }

            return coupons;
        }

        [HttpPost]
        public IActionResult OnPost()
        {
            try
            {
                switch (Request.Form["handler"])
                {
                    case "delete":
                        int selectedCouponId = Convert.ToInt32(Request.Form["couponid"]);
                        return DeleteCoupon(selectedCouponId);

                    case "update":
                        int couponIdToUpdate = Convert.ToInt32(Request.Form["couponid"]);
                        string codeToUpdate = Request.Form["code"];
                        int discountPercentageToUpdate = Convert.ToInt32(Request.Form["discountpercentage"]);
                        DateTime expiryDateToUpdate = Convert.ToDateTime(Request.Form["expirydate"]);
                        int minimumTotalToUpdate = Convert.ToInt32(Request.Form["minimumtotal"]);

                        UpdateCoupon(couponIdToUpdate, codeToUpdate, discountPercentageToUpdate, expiryDateToUpdate, minimumTotalToUpdate);
                        break;

                    case "add":
                        int couponIdToAdd = Convert.ToInt32(Request.Form["couponid"]);
                        string codeToAdd = Request.Form["code"];
                        int discountPercentageToAdd = Convert.ToInt32(Request.Form["discountpercentage"]);
                        DateTime expiryDateToAdd = Convert.ToDateTime(Request.Form["expirydate"]);
                        int minimumTotalToAdd = Convert.ToInt32(Request.Form["minimumtotal"]);

                        AddCoupon(couponIdToAdd, codeToAdd, discountPercentageToAdd, expiryDateToAdd, minimumTotalToAdd);
                        break;

                        // Handle other cases as needed
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = $"Error: {ex.Message}";
                return RedirectToPage();
            }
        }

        [HttpPost]
        public IActionResult UpdateCoupon(
            [FromForm(Name = "coupon.couponid")] int couponId,
            [FromForm(Name = "coupon.code")] string code,
            [FromForm(Name = "coupon.discountpercentage")] int discountPercentage,
            [FromForm(Name = "coupon.expirydate")] DateTime expiryDate,
            [FromForm(Name = "coupon.minimumtotal")] int minimumTotal)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    TempData["ResultMessage"] = "Error updating coupon: Code parameter is null or empty.";
                    return RedirectToPage();
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Coupon SET Code = @Code, DiscountPercentage = @DiscountPercentage, " +
                                   "ExpiryDate = @ExpiryDate, MinimumTotal = @MinimumTotal " +
                                   "WHERE CouponID = @CouponID";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@CouponID", couponId);
                    cmd.Parameters.AddWithValue("@Code", code);
                    cmd.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    cmd.Parameters.AddWithValue("@MinimumTotal", minimumTotal);

                    cmd.ExecuteNonQuery();

                    TempData["ResultMessage"] = $"Coupon with ID {couponId} updated successfully.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = $"Error updating coupon: {ex.Message}";
                return RedirectToPage();
            }
        }

        [HttpPost]
        public IActionResult DeleteCoupon([FromForm(Name = "couponid")] int couponId)
        {
            string resultMessage;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = $"DELETE FROM Coupon WHERE CouponID = @CouponID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CouponID", couponId);

                    int rowsAffected = command.ExecuteNonQuery();

                    resultMessage = rowsAffected > 0
                        ? $"Coupon with ID {couponId} deleted successfully."
                        : $"Coupon with ID {couponId} not found or deletion failed.";
                }
            }

            TempData["ResultMessage"] = resultMessage;
            return RedirectToPage();
        }

        public IActionResult AddCoupon(
            [FromForm(Name = "couponid")] int couponId,
            [FromForm(Name = "code")] string code,
            [FromForm(Name = "discountpercentage")] int discountPercentage,
            [FromForm(Name = "expirydate")] DateTime expiryDate,
            [FromForm(Name = "minimumtotal")] int minimumTotal)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    TempData["ResultMessage"] = "Error adding coupon: Code parameter is null or empty.";
                    return RedirectToPage();
                }

                if (GetCouponById(couponId) != null)
                {
                    TempData["ResultMessage"] = $"Error adding coupon: Coupon with ID {couponId} already exists.";
                    return RedirectToPage();
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Coupon (CouponID, Code, DiscountPercentage, ExpiryDate, MinimumTotal) " +
                                   "VALUES (@CouponID, @Code, @DiscountPercentage, @ExpiryDate, @MinimumTotal)";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@CouponID", couponId);
                    cmd.Parameters.AddWithValue("@Code", code);
                    cmd.Parameters.AddWithValue("@DiscountPercentage", discountPercentage);
                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate);
                    cmd.Parameters.AddWithValue("@MinimumTotal", minimumTotal);

                    cmd.ExecuteNonQuery();

                    TempData["ResultMessage"] = $"Coupon with ID {couponId} added successfully.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = $"Error adding coupon: {ex.Message}";
                return RedirectToPage();
            }
        }

        private Coupon GetCouponById(int couponId)
        {
            return GetCoupons()?.FirstOrDefault(coupon => coupon.CouponID == couponId);
        }

        // Coupon.cs
        public class Coupon
        {
            public int CouponID { get; set; }
            public string Code { get; set; }
            public int DiscountPercentage { get; set; }
            public DateTime ExpiryDate { get; set; }
            public int MinimumTotal { get; set; }
        }
    }
}
