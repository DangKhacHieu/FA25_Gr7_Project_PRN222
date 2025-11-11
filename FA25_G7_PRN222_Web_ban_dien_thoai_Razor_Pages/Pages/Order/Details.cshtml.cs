using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Order
{
    public class DetailsModel : PageModel
    {
        private readonly IOrderService _orderService;

        public DetailsModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Dùng để lưu chi tiết đơn hàng và hiển thị ra View
        public Order_List OrderDetails { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } // Biến này sẽ tự động lấy từ ?id=5 hoặc /Order/Details/5

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Lấy CustomerID từ Session
            int? customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                // Nếu chưa đăng nhập, chuyển về trang đăng nhập
                return RedirectToPage("/Login"); // Thay bằng trang đăng nhập của bạn
            }

            // 2. Gọi service. 
            // Hàm này đã bao gồm logic bảo mật:
            // "Chỉ lấy đơn hàng có OrderID = Id VÀ CustomerID = customerId"
            OrderDetails = await _orderService.GetOrderDetailsForCustomerAsync(Id, customerId.Value);

            // 3. Kiểm tra kết quả
            if (OrderDetails == null)
            {
                // Không tìm thấy đơn hàng, 
                // hoặc đơn hàng này không thuộc về khách hàng đang đăng nhập
                return NotFound();
            }

            // 4. Trả về trang với đầy đủ dữ liệu
            return Page();
        }
    }
}
