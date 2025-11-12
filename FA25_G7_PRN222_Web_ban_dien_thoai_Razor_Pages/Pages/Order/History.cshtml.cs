using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FA25_G7_PRN222_Web_ban_dien_thoai_Razor_Pages.Pages.Order
{
    public class HistoryModel : PageModel
    {
        private readonly IOrderService _orderService;

        // 1. Dữ liệu (chỉ của trang hiện tại)
        public IEnumerable<Order_List> Orders { get; set; } = new List<Order_List>();

        // 2. Trang hiện tại (lấy từ URL ?p=...)
        [BindProperty(SupportsGet = true)]
        public int p { get; set; } = 1;

        // 3. Tổng số trang (sẽ được tính toán)
        public int TotalPages { get; set; }

        // 4. Kích thước trang (cố định)
        public int PageSize { get; } = 5; // 10 đơn hàng mỗi trang

        public HistoryModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

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
            // --- LOGIC PHÂN TRANG MỚI ---

            // GỌI BLL LẦN 1: Đếm tổng số đơn hàng
            var totalCount = await _orderService.CountOrdersForCustomerAsync(customerId.Value);

            // GỌI BLL LẦN 2: Lấy đơn hàng cho trang hiện tại (trang 'p')
            var items = await _orderService.GetPagedOrdersForCustomerAsync(customerId.Value, p, PageSize);

            // Gán giá trị cho View
            Orders = items;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Đảm bảo 'p' không vượt quá TotalPages
            if (p > TotalPages && TotalPages > 0)
            {
                p = TotalPages;
                // Tải lại nếu 'p' không hợp lệ (ví dụ: ?p=99)
                return RedirectToPage(new { p = this.p });
            }

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

            return RedirectToPage(new { p = this.p }); // Tải lại trang History
        }

        // --- THÊM HANDLER CHO NÚT NHẬN HÀNG (RECEIVE) ---
        public async Task<IActionResult> OnPostReceiveAsync(int id)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToPage("/Login");
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

            return RedirectToPage(new { p = this.p }); // Tải lại trang History
        }
    }
}
