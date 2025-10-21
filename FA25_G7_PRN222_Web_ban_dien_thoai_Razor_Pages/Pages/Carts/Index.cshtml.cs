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

        public DAL.Models.Cart? UserCart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? customerId = 1;
            // ✅ Check session
            //int? customerId = HttpContext.Session.GetInt32("CustomerId");
            //if (customerId == null)
            //{
            //    TempData["Message_alert"] = true;
            //    TempData["Message"] = "⚠️ Bạn cần đăng nhập để xem giỏ hàng.";
            //    return RedirectToPage("/Account/Login");
            //}

            UserCart = await _cartService.GetCartAsync(customerId.Value);
            return Page();
        }
    }
}
