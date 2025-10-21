using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class UpdateModel : PageModel
    {
        private readonly ICartService _cartService;
        public UpdateModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty] public int CartItemId { get; set; }
        [BindProperty] public int Quantity { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            await _cartService.UpdateCartItemAsync(CartItemId, Quantity);
            return RedirectToPage("Index");
        }
    }
}
