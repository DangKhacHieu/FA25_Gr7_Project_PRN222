using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using BLL.IServices;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public RegisterModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public string? Message { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Please correct the errors below.";
                return Page();
            }

            if (_customerService.IsUsernameExist(Input.UserName!))
            {
                Message = "Username already exists.";
                return Page();
            }

            if (_customerService.IsEmailExist(Input.Email!))
            {
                Message = "Email already exists.";
                return Page();
            }

            // ✅ Tạo OTP và lưu Session
            var otp = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("RegisterOTP", otp);
            HttpContext.Session.SetString("RegisterOTPTime", DateTime.Now.AddMinutes(3).ToString());
            HttpContext.Session.SetString("RegisterTempData", System.Text.Json.JsonSerializer.Serialize(Input));

            // ✅ Gửi email OTP
            _customerService.SendOTPEmail(Input.Email!, otp);

            return RedirectToPage("/VerifyOTP_Register");
        }

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Username is required")]
            public string? UserName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string? Email { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string? Password { get; set; }

            public string? FullName { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Address { get; set; }
            public string? Sex { get; set; }
            [DataType(DataType.Date)]
            public DateTime? DOB { get; set; }
        }
    }
}
