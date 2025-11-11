using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Order
{
    public class HistoryModel : PageModel
    {
        private readonly IOrderService _orderService;

        public HistoryModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IEnumerable<Order_List> Orders { get; set; } = new List<Order_List>();

        private int? GetCurrentCustomerId()
        {
            // Dùng key "CustomerId" (giống như code cũ của bạn)
            return HttpContext.Session.GetInt32("CustomerId");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy CustomerID (int) từ Session
            int? customerId = HttpContext.Session.GetInt32("CustomerId"); // Đổi tên key cho nhất quán

            if (customerId == null)
            {
                // Nếu chưa đăng nhập, chuyển về trang đăng nhập
                // (Bạn nên dùng [Authorize] như tôi đề xuất ở câu trước sẽ tốt hơn)
                return RedirectToPage("/Login"); // Thay bằng trang đăng nhập của bạn
            }

            // Gọi service để lấy lịch sử đơn hàng (hàm này giờ đã tồn tại)
            Orders = await _orderService.GetOrderHistoryAsync(customerId.Value);

            return Page();
        }
        public async Task<IActionResult> OnPostCancelAsync(int id) // 'id' khớp với asp-route-id
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                await _orderService.CancelOrderAsync(id, customerId.Value);
                TempData["SuccessMessage"] = $"Đã hủy thành công đơn hàng #{id}.";
            }
            catch (Exception ex)
            {
                // Gửi lỗi ra View
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage(); // Tải lại trang History
        }

        // --- THÊM HANDLER CHO NÚT NHẬN HÀNG (RECEIVE) ---
        public async Task<IActionResult> OnPostReceiveAsync(int id)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                await _orderService.MarkOrderAsReceivedAsync(id, customerId.Value);
                TempData["SuccessMessage"] = $"Đã xác nhận nhận hàng cho đơn #{id}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage(); // Tải lại trang History
        }
    }
}
