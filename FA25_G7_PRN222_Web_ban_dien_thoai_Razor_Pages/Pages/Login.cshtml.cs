using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.IServices;
using DAL.Models;
using Microsoft.AspNetCore.Http;

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
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; } = "";

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

            // Gọi hàm login trong service
            var customer = _customerService.GetByLogin(Username, Password);


            if (customer == null)
            {
                Message = "Invalid username or password!";
                return Page();
            }
            // Lưu session
            HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
            HttpContext.Session.SetString("CustomerName", customer.FullName ?? "");
            // Nếu login thành công => chuyển hướng sang trang chính
            return RedirectToPage("/Index");
        }
    }
}
