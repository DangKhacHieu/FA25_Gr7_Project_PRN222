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
            // ✅ Check session
            //int? customerId = HttpContext.Session.GetInt32("CustomerId");
            //if (customerId == null)
            //{
            //    TempData["Message_alert"] = true;
            //    TempData["Message"] = "⚠️ Bạn cần đăng nhập để thao tác với giỏ hàng.";
            //    return RedirectToPage("/Account/Login");
            //}

            await _cartService.RemoveCartItemAsync(CartItemId);
            TempData["Message"] = "🗑️ Đã xóa sản phẩm khỏi giỏ hàng.";
            TempData["Message_success"] = true;

            return RedirectToPage("Index");
        }
    }
}
