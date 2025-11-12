using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.IServices;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public LoginModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Message = "Please fill in both username and password.";
                return Page();
            }

            // ✅ Kiểm tra username trước
            var customer = _customerService.GetAllCustomers()
                .FirstOrDefault(c => c.UserName == Username);

            if (customer == null)
            {
                Message = "Username does not exist.";
                return Page();
            }

            // ✅ Hash password nhập vào để so sánh
            string hashedPassword = HashPassword(Password);

            if (customer.Password != hashedPassword)
            {
                Message = "Incorrect password.";
                return Page();
            }

            if (customer.Status == 0)
            {
                Message = "Your account has been locked. Please contact support.";
                return Page();
            }

            // ✅ Lưu session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FullName ?? customer.UserName);

            // ✅ Chuyển hướng sang trang chính
            return RedirectToPage("/Index");
        }

        // ✅ Hàm hash password (SHA256)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
