using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FA25_G7_PRN222_Web_ban_dien_thoai.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Order
        public async Task<IActionResult> Index(string? status, int page = 1, int pageSize = 10)
        {
            // 1. Lấy dữ liệu đã phân trang (kiểu PagedResult<Order_List>)
            var pagedResultOrder = await _orderService.GetOrdersPaginatedAsync(page, pageSize, status);

            // 2. ÉP KIỂU SANG PagedResult<object> ĐỂ TRÁNH LỖI INVALID OPERATION
            var pagedResultObject = new PagedResult<object>
            {
                Items = pagedResultOrder.Items.Cast<object>(), // Ép kiểu danh sách
                TotalCount = pagedResultOrder.TotalCount,
                PageIndex = pagedResultOrder.PageIndex,
                PageSize = pagedResultOrder.PageSize
            };

            // Khai báo danh sách trạng thái cố định cho ViewBag (dùng trong Dropdown)
            ViewBag.StatusList = new List<string> { "All", "Pending", "Shipping", "Completed", "Failed" };
            ViewBag.CurrentStatus = status ?? "All";

            // Trả về View với PagedResult<object>
            return View(pagedResultObject);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
                // Thông báo thành công bằng Tiếng Anh
                TempData["SuccessMessage"] = $"Order #{orderId} status updated to {newStatus} successfully!";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Error: Order not found.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message; // Thông báo lỗi logic
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Update failed: The order was modified by another user. Please try again.";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Database Error: Could not update the status. (Detail: " + ex.Message + ")";
            }
            catch (Exception ex)
            {
                // Sử dụng thông báo chi tiết hơn để debug
                TempData["ErrorMessage"] = "Unknown Error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
