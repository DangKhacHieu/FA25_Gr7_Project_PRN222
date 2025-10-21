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
            // ✅ Check session
            //int? customerId = HttpContext.Session.GetInt32("CustomerId");
            //if (customerId == null)
            //{
            //    TempData["Message_alert"] = true;
            //    TempData["Message"] = "⚠️ Bạn cần đăng nhập để cập nhật giỏ hàng.";
            //    return RedirectToPage("/Account/Login");
            //}

            var result = await _cartService.UpdateCartItemWithCheckAsync(CartItemId, Quantity);
            TempData["Message"] = result.Message;
            if (result.Success)
                TempData["Message_success"] = true;
            else
                TempData["Message_alert"] = true;

            return RedirectToPage("Index");
        }
    }
}
