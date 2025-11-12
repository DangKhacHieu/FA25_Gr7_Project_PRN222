using BLL.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class VerifyOTPModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public VerifyOTPModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public string OTP { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = false;

        public void OnGet() { }

        // ✅ Xác minh OTP quên mật khẩu
        public IActionResult OnPost()
        {
            var savedOTP = HttpContext.Session.GetString("ResetOTP");
            var otpTime = HttpContext.Session.GetString("ResetOTPTime");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(email))
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

            // ✅ Nếu đúng OTP → chuyển sang trang Reset Password
            return RedirectToPage("/ForgotPassword/ResetPassword");
        }

        // ✅ Gửi lại OTP khi người dùng bấm “Resend OTP”
        public IActionResult OnPostResend()
        {
            var email = HttpContext.Session.GetString("ResetEmail");

            if (string.IsNullOrEmpty(email))
            {
                Message = "Your session has expired. Please try again.";
                return RedirectToPage("/ForgotPassword/ForgotPassword");
            }

            // Gửi lại OTP mới qua email
            var newOtp = _customerService.ResendRegisterOTP(email);

            // Cập nhật Session
            HttpContext.Session.SetString("ResetOTP", newOtp);
            HttpContext.Session.SetString("ResetOTPTime", DateTime.Now.AddMinutes(3).ToString());

            // Hiển thị thông báo
            IsSuccess = true;
            Message = $"A new OTP has been sent to {email}.";
            return Page();
        }
    }
}
