using BLL.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class ChangePassWordModel : PageModel
    {
        private readonly ICustomerService _customerService;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        // ViewModel cho Form (InputModel)
        public class InputModel
        {
            [Required(ErrorMessage = "Mật khẩu cũ là bắt buộc.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu cũ")]
            public string OldPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            [StrongPassword] // ÁP DỤNG VALIDATION LỒNG
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận không khớp.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        // --- ĐỊNH NGHĨA CUSTOM VALIDATION ATTRIBUTE BÊN TRONG NAMESPACE ---
        public class StrongPasswordAttribute : ValidationAttribute
        {
            // Yêu cầu: 8 ký tự, 1 chữ hoa, 1 chữ thường, 1 số, 1 ký tự đặc biệt
            private const string PasswordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

            public override bool IsValid(object? value)
            {
                if (value == null) return false;
                var password = value.ToString();
                return Regex.IsMatch(password!, PasswordRegex);
            }

            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (IsValid(value))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt (@$!%*?&).");
            }
        }
        // -------------------------------------------------------------------


        public ChangePassWordModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("CustomerId") == null)
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // GỌI SERVICE VÀ NHẬN KẾT QUẢ DƯỚI DẠNG TUPLE
            var result = await _customerService.ChangePasswordAsync(
                customerId.Value,
                Input.OldPassword,
                Input.NewPassword
            );

            if (result.Success)
            {
                HttpContext.Session.Clear();
                TempData["LoginStatusMessage"] = "Mật khẩu đã được đổi thành công. Vui lòng đăng nhập lại.";

                return RedirectToPage("/Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return Page();
            }
        }
    }
}
