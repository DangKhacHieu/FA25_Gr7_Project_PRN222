using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using DAL.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly PhoneContext _context;

        public ForgotPasswordModel(PhoneContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var user = _context.Customers.FirstOrDefault(c => c.Email == Email);

            if (user == null)
            {
                Message = "Email not found!";
                return Page();
            }

            // ✅ Sinh OTP 6 chữ số
            var otp = new Random().Next(100000, 999999).ToString();

            // ✅ Lưu OTP & Email vào Session (hết hạn sau 5 phút)
            HttpContext.Session.SetString("ResetEmail", Email);
            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("ResetOTPTime", DateTime.Now.AddMinutes(5).ToString());

            // ✅ Gửi OTP qua Email
            SendOtpEmail(Email, otp);

            Message = "OTP has been sent to your email. Please check your inbox.";
            return RedirectToPage("/ForgotPassword/VerifyOTP");
        }

        private void SendOtpEmail(string toEmail, string otp)
        {
            var fromEmail = "trannhuy095@gmail.com";
            var password = "fqojhikpixktpkvy"; // 🔒 app password Gmail (không phải mật khẩu thật)

            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(fromEmail, password);

                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = "Password Reset OTP";
                mail.Body = $"Your OTP code is: {otp}\nIt will expire in 5 minutes.";

                client.Send(mail);
            }
        }
    }
}
