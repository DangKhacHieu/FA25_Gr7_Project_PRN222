using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.IServices;
using DAL.Models;
using System.ComponentModel.DataAnnotations;

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

        public string Message { get; set; } = "";

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Message = "Please correct the highlighted errors.";
                return Page();
            }

            // Map sang Customer model
            var customer = new Customer
            {
                UserName = Input.UserName,
                FullName = Input.FullName,
                Email = Input.Email,
                Password = Input.Password,
                PhoneNumber = Input.PhoneNumber,
                Address = Input.Address,
                Sex = Input.Sex,
                DOB = Input.DOB,
                ImgCustomer = null,
                Status = 1
            };

            bool success = _customerService.Register(customer, out string msg);
            Message = msg;

            if (success)
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }
    }

    // ✅ Class input riêng để thêm validation
    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
            ErrorMessage = "Password must be at least 8 chars, include upper/lowercase, number, and special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Invalid Vietnamese phone number format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
    }
}
