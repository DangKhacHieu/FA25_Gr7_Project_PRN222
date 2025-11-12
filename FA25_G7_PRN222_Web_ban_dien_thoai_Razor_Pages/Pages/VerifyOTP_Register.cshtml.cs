using BLL.IServices;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class VerifyOTP_RegisterModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public VerifyOTP_RegisterModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public string OTP { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        public void OnGet() { }

        // ✅ Xác minh OTP đăng ký
        public IActionResult OnPost()
        {
            var savedOTP = HttpContext.Session.GetString("RegisterOTP");
            var otpTime = HttpContext.Session.GetString("RegisterOTPTime");
            var tempData = HttpContext.Session.GetString("RegisterTempData");

            if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(tempData))
            {
                Message = "OTP expired or invalid.";
                return Page();
            }

            if (DateTime.Now > DateTime.Parse(otpTime))
            {
                Message = "OTP has expired.";
                return Page();
            }

            if (OTP != savedOTP)
            {
                Message = "Invalid OTP.";
                return Page();
            }

            var input = JsonSerializer.Deserialize<RegisterModel.RegisterInputModel>(tempData!);
            var hashedPassword = HashPassword(input!.Password!);

            var customer = new Customer
            {
                UserName = input.UserName!,
                Email = input.Email!,
                Password = hashedPassword,
                FullName = input.FullName,
                PhoneNumber = input.PhoneNumber,
                Address = input.Address,
                Sex = input.Sex,
                DOB = input.DOB,
                Status = 1
            };

            _customerService.RegisterCustomer(customer);

            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToPage("/Login");
        }

        // ✅ Gửi lại OTP
        public IActionResult OnPostResend()
        {
            var tempData = HttpContext.Session.GetString("RegisterTempData");
            if (string.IsNullOrEmpty(tempData))
            {
                Message = "Registration session expired. Please register again.";
                return RedirectToPage("/Register");
            }

            var input = JsonSerializer.Deserialize<RegisterModel.RegisterInputModel>(tempData!);
            var newOtp = _customerService.ResendRegisterOTP(input!.Email!);

            HttpContext.Session.SetString("RegisterOTP", newOtp);
            HttpContext.Session.SetString("RegisterOTPTime", DateTime.Now.AddMinutes(3).ToString());

            IsSuccess = true;
            Message = $"A new OTP has been sent to {input.Email}.";
            return Page();
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }
    }
}
