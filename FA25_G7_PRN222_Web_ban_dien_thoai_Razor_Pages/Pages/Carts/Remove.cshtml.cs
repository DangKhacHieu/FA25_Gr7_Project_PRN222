using BLL.Interfaces;
using FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Hubs; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class RemoveModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IHubContext<DataSignalR> _hubContext; 
        public RemoveModel(ICartService cartService, IHubContext<DataSignalR> hubContext)
        {
            _cartService = cartService;
            _hubContext = hubContext; 
        }

        [BindProperty] public int CartItemId { get; set; }
        public async Task<IActionResult> OnPostAsync([FromForm] int CartItemId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return new JsonResult(new { success = false, message = "Phiên đăng nhập hết hạn." });
            }

            await _cartService.RemoveCartItemAsync(CartItemId);

            var cart = await _cartService.GetCartAsync(customerId.Value);

            await _hubContext.Clients.All.SendAsync("ReceiveCartUpdate", new
            {
                cartItemId = CartItemId,
                newQuantity = 0, 
                subtotal = 0,
                total = cart?.TotalPrice ?? 0,
                message = "🗑️ Đã xóa sản phẩm khỏi giỏ hàng."
            });

            return new JsonResult(new { success = true });
        }
    }
}