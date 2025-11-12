using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order_List>> GetAllOrdersAsync();
        Task<IEnumerable<Order_List>> GetOrdersByStatusAsync(string? status);
        Task<Order_List?> GetOrderByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<bool> OrderExistsAsync(int id);
        Task<PagedResult<Order_List>> GetOrdersPaginatedAsync(int pageIndex, int pageSize, string? status = null);

        // --- TÍCH HỢP CODE MỚI (CHO KHÁCH HÀNG) ---

        /// <summary>
        /// Lấy lịch sử đơn hàng (phân trang) của một khách hàng cụ thể.
        /// </summary>
        Task<IEnumerable<Order_List>> GetOrderHistoryAsync(int customerId);

        /// <summary>
        /// Lấy chi tiết một đơn hàng VÀ kiểm tra xem nó có thuộc về khách hàng đó không.
        /// </summary>
        /// <returns>Trả về Order_List nếu hợp lệ, ngược lại trả về null.</returns>
        Task<Order_List?> GetOrderDetailsForCustomerAsync(int orderId, int customerId);

        Task<Order_List> CreateOrderFromCartAsync(
        int customerId,
        string address,
        string phoneNumber,
        string paymentMethod
        );

        /// <summary>
        /// Khách hàng hủy đơn hàng (chỉ khi 'Pending') và hoàn trả kho.
        /// </summary>
        Task CancelOrderAsync(int orderId, int customerId);

        /// <summary>
        /// Khách hàng xác nhận đã nhận hàng (chỉ khi 'Shipping').
        /// </summary>
        Task MarkOrderAsReceivedAsync(int orderId, int customerId);

        /// <summary>
        /// (HÀM MỚI 1) Chỉ đếm tổng số đơn hàng của một khách hàng.
        /// </summary>
        Task<int> CountOrdersForCustomerAsync(int customerId);

        /// <summary>
        /// (HÀM MỚI 2) Lấy một trang dữ liệu lịch sử đơn hàng của khách hàng.
        /// </summary>
        Task<IEnumerable<Order_List>> GetPagedOrdersForCustomerAsync(int customerId, int pageIndex, int pageSize);
    }
}

