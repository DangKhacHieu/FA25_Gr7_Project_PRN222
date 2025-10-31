using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class AddModel : PageModel
    {
        private readonly ICartService _cartService;
        public AddModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty] public int ProductID { get; set; }
        [BindProperty] public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnPostAsync()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để thêm sản phẩm.";
                return RedirectToPage("/Login");
            }
            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            TempData["Message"] = result.Message;
            if (result.Success)
                TempData["Message_success"] = true;
            else
                TempData["Message_alert"] = true;

            return RedirectToPage("Index");
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để thêm sản phẩm.";
                return RedirectToPage("/Login");
            }
            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            TempData["Message"] = result.Message;
            if (result.Success)
                TempData["Message_success"] = true;
            else
                TempData["Message_alert"] = true;

            return RedirectToPage("Index");
        }
    }
}
