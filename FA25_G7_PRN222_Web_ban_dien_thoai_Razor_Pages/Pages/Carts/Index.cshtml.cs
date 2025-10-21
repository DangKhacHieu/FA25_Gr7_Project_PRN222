using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using DAL.Models;

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

        public async Task OnGetAsync()
        {
            // Giả lập CustomerID = 1 (nếu bạn chưa có login)
            int customerId = 1;

            UserCart = await _cartService.GetCartAsync(customerId);
        }
    }
}
