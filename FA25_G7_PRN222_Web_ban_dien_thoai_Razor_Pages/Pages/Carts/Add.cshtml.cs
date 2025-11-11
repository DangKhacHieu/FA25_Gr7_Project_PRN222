using BLL.Interfaces;
using DAL.Models;
using FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Hubs; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class AddModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly IHubContext<DataSignalR> _hubContext;

        public AddModel(ICartService cartService, IHubContext<DataSignalR> hubContext)
        {
            _cartService = cartService;
            _hubContext = hubContext;
        }

        [BindProperty] public int ProductID { get; set; }
        [BindProperty] public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnPostAsync()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để mua hàng.";
                return RedirectToPage("/Login");
            }
            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            if (!result.Success)
            {
                TempData["Message"] = result.Message;
                TempData["Message_alert"] = true;
                return RedirectToPage("/Products/Details", new { id = ProductID });
            }

            return RedirectToPage("/Carts/Index");
        }

        public async Task<IActionResult> OnPostAjaxAsync()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return new JsonResult(new { success = false, message = "Vui lòng đăng nhập để thêm sản phẩm." });
            }

            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            string type = result.Success ? "success" : "danger";

            // Gửi sự kiện CHỈ ĐỂ HIỂN THỊ TOAST
            await _hubContext.Clients.All.SendAsync("ReceiveCartNotification", result.Message, type);

            return new JsonResult(new { success = result.Success });
        }

        public async Task<IActionResult> OnGetAsync(int id, string? redirect)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để thêm sản phẩm.";
                return RedirectToPage("/Login");
            }

            ProductID = id;
            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            if (!result.Success)
            {
                TempData["Message"] = result.Message;
                TempData["Message_alert"] = true;
                return RedirectToPage("/Products/Details", new { id = ProductID });
            }

            if (redirect == "cart")
            {
                return RedirectToPage("/Carts/Index");
            }

            return RedirectToPage("/Index");
        }
    }
}