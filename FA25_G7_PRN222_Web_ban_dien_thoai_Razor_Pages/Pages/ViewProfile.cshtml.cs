using BLL.IServices;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages
{
    public class ViewProfileModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public ViewProfileModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty]
        public Customer Customer { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        public string? Message { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                // giả sử có session lưu ID người đăng nhập
                id = HttpContext.Session.GetInt32("CustomerId");
            }

            if (id == null)
            {
                return RedirectToPage("/Login");
            }

            var customer = await _customerService.GetCustomerByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            Customer = customer;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingCustomer = await _customerService.GetCustomerByIdAsync(Customer.CustomerId);
            if (existingCustomer == null)
            {
                Message = "Customer information not found!";
                return Page();
            }

            // ✅ Chỉ cho update những field này
            existingCustomer.FullName = Customer.FullName;
            existingCustomer.Email = Customer.Email;
            existingCustomer.PhoneNumber = Customer.PhoneNumber;
            existingCustomer.Address = Customer.Address;
            existingCustomer.Sex = Customer.Sex;
            existingCustomer.DOB = Customer.DOB;

            await _customerService.UpdateCustomerAsync(existingCustomer);

            SuccessMessage = "Profile update successful!";
            return RedirectToPage(new { id = Customer.CustomerId });
        }
    }
}
