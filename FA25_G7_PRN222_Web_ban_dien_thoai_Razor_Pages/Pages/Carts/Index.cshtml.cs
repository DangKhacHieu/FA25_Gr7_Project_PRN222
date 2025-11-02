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
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToPage("/Login");
            }

            if (customerId == null)
            {
                // Nếu chưa đăng nhập, chuyển về trang Login
                return RedirectToPage("/Login", new { returnUrl = "/Carts/Index" });
            }

            UserCart = await _cartService.GetCartAsync(customerId.Value);
            return Page();
        }
    }
}
