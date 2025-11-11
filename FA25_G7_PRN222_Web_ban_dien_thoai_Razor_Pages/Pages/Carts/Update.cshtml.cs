using BLL.Interfaces;
using DAL.Models;
using FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class UpdateModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IHubContext<DataSignalR> _hubContext;

        public UpdateModel(ICartService cartService, IHubContext<DataSignalR> hubContext)
        {
            _cartService = cartService;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> OnPostAsync([FromForm] int CartItemId, [FromForm] int Quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return new JsonResult(new { success = false, message = "Phiên đăng nhập hết hạn. Vui lòng tải lại trang." });
            }

            var result = await _cartService.UpdateCartItemWithCheckAsync(CartItemId, Quantity);

            if (!result.Success)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveCartNotification", result.Message, "warning");
                return new JsonResult(new { success = false, message = result.Message });
            }

            var cart = await _cartService.GetCartAsync(customerId.Value);
            var item = cart?.CartItems.FirstOrDefault(x => x.CartItemId == CartItemId);

            await _hubContext.Clients.All.SendAsync("ReceiveCartUpdate", new
            {
                cartItemId = CartItemId,
                newQuantity = item?.Quantity ?? Quantity,
                subtotal = item?.SubTotal ?? 0,
                total = cart?.TotalPrice ?? 0,
                message = result.Message 
            });

            return new JsonResult(new { success = true, message = result.Message });
        }
    }
}