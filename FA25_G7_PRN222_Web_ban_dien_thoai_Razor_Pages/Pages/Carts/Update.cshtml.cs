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

        public int CartItemId { get; set; }
        public int Quantity { get; set; }

        public async Task<IActionResult> OnPostAsync([FromForm] int CartItemId, [FromForm] int Quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để cập nhật sản phẩm.";
                return RedirectToPage("/Login");
            }
            var result = await _cartService.UpdateCartItemWithCheckAsync(CartItemId, Quantity);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (!result.Success)
                    return new JsonResult(new { success = false, message = result.Message });

                var cart = await _cartService.GetCartAsync(customerId.Value);
                var item = cart?.CartItems.FirstOrDefault(x => x.CartItemId == CartItemId);

                return new JsonResult(new
                {
                    success = true,
                    message = result.Message,
                    subtotal = item?.SubTotal ?? 0,
                    total = cart?.TotalPrice ?? 0
                });
            }

            TempData["Message"] = result.Message;
            if (result.Success) TempData["Message_success"] = true;
            else TempData["Message_alert"] = true;

            return RedirectToPage("Index");
        }

    }
}