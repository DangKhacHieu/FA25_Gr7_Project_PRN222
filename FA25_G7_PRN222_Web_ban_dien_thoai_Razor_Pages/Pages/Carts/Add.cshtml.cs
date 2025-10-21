using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class AddModel : PageModel
    {
        private readonly ICartService _cartService;
        public AddModel(ICartService cartService) => _cartService = cartService;

        [BindProperty] public int ProductID { get; set; }
        [BindProperty] public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnPostAsync()
        {
            int customerId = 1;
            var result = await _cartService.AddToCartWithCheckAsync(customerId, ProductID, Quantity);

            TempData["Message"] = result.Message;
            if (result.Success)
                TempData["Message_success"] = true;
            else
                TempData["Message_alert"] = true;

            return RedirectToPage("/Carts/Index");
        }
    }
}
