using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class RemoveModel : PageModel
    {
        private readonly ICartService _cartService;
        public RemoveModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty] public int CartItemId { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            await _cartService.RemoveCartItemAsync(CartItemId);
            return RedirectToPage("Index");
        }
    }
}
