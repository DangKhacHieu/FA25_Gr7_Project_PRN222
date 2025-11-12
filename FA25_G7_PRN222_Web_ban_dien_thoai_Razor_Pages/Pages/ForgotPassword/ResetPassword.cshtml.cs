using BLL.Interfaces;
using BLL.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public ResetPasswordModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty] public string NewPassword { get; set; } = string.Empty;
        [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public IActionResult OnPost()
        {
            var email = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(email))
            {
                Message = "Session expired.";
                return RedirectToPage("/ForgotPassword/ForgotPassword");
            }

            // ✅ Kiểm tra mật khẩu khớp nhau
            if (NewPassword != ConfirmPassword)
            {
                Message = "Passwords do not match.";
                return Page();
            }

            // ✅ Kiểm tra độ mạnh của mật khẩu (ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số, ký tự đặc biệt)
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(NewPassword, passwordPattern))
            {
                Message = "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.";
                return Page();
            }

            // ✅ Lấy mật khẩu cũ
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
            {
                Message = "User not found.";
                return Page();
            }

            var oldHashedPassword = customer.Password;
            var newHashedPassword = HashPassword(NewPassword);

            // ✅ Không cho phép trùng mật khẩu cũ
            if (oldHashedPassword == newHashedPassword)
            {
                Message = "New password cannot be the same as the old password.";
                return Page();
            }

            // ✅ Cập nhật mật khẩu mới
            var result = _customerService.UpdatePassword(email, NewPassword);
            if (!result)
            {
                Message = "Failed to update password.";
                return Page();
            }

            // ✅ Xóa session sau khi đặt lại
            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("ResetOTP");
            HttpContext.Session.Remove("ResetOTPTime");

            Message = "Password reset successful! Please log in again.";
            return RedirectToPage("/Login");
        }

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
