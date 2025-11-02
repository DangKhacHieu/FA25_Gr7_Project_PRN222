using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Carts
{
    public class AddModel : PageModel
    {
        private readonly ICartService _cartService;
        public AddModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty] public int ProductID { get; set; }
        [BindProperty] public int Quantity { get; set; } = 1;

        // XỬ LÝ CHO NÚT "MUA NGAY" (SUBMIT THƯỜNG)
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
                // Nếu mua ngay mà lỗi (ví dụ hết hàng), báo lỗi và quay lại
                TempData["Message"] = result.Message;
                TempData["Message_alert"] = true;
                return RedirectToPage("/Products/Details", new { id = ProductID });
            }

            // Mua ngay thành công -> Chuyển đến giỏ hàng
            return RedirectToPage("/Carts/Index");
        }

        public async Task<IActionResult> OnPostAjaxAsync()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Trả về lỗi JSON
                return new JsonResult(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập để thêm sản phẩm."
                });
            }

            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            // Luôn trả về JSON
            return new JsonResult(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        public async Task<IActionResult> OnGetAsync(int id, string? redirect)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Message_alert"] = "Vui lòng đăng nhập để thêm sản phẩm.";
                return RedirectToPage("/Login");
            }

            // Gán ProductID từ route
            ProductID = id;
            var result = await _cartService.AddToCartWithCheckAsync(customerId.Value, ProductID, Quantity);

            if (!result.Success)
            {
                TempData["Message"] = result.Message;
                TempData["Message_alert"] = true;
                return RedirectToPage("/Products/Details", new { id = ProductID });
            }

            // Nếu "Mua ngay" (redirect=cart) thì chuyển sang giỏ hàng
            if (redirect == "cart")
            {
                return RedirectToPage("/Carts/Index");
            }

            // Mặc định (nếu có)
            return RedirectToPage("/Index");
        }
    }
}
