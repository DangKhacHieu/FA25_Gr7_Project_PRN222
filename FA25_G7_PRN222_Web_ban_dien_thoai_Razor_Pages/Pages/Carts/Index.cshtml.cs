using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class IndexModel : PageModel
    {
        private readonly ICartService _cartService;
        public IndexModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public Cart? UserCart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int customerId = 1; // sẽ thay bằng Session sau
            UserCart = await _cartService.GetCartAsync(customerId);
            return Page();
        }
    }
}
