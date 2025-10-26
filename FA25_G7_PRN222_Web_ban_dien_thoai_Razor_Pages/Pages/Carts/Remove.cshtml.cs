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

        public async Task<IActionResult> OnPostAsync([FromForm] int CartItemId)
        {
            int customerId = 1;
            await _cartService.RemoveCartItemAsync(CartItemId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cart = await _cartService.GetCartAsync(customerId);

                return new JsonResult(new
                {
                    success = true,
                    message = "🗑️ Đã xóa sản phẩm khỏi giỏ hàng.",
                    total = cart?.TotalPrice ?? 0
                });
            }

            TempData["Message"] = "🗑️ Đã xóa sản phẩm khỏi giỏ hàng.";
            TempData["Message_success"] = true;
            return RedirectToPage("Index");
        }
    }
}
