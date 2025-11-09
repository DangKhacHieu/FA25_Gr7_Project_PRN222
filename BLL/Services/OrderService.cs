using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IEnumerable<Order_List>> GetAllOrdersAsync()
            => await _orderRepo.GetAllOrdersAsync();

        public async Task<Order_List?> GetOrderByIdAsync(int id)
            => await _orderRepo.GetOrderByIdAsync(id);

        public async Task<bool> OrderExistsAsync(int id)
            => await _orderRepo.ExistsAsync(id);

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            // --- LOGIC KIỂM TRA TRẠNG THÁI SỬ DỤNG TIẾNG ANH ---

            bool canUpdate = true;
            // Sử dụng "Pending" làm giá trị mặc định nếu Status là null
            string currentStatus = order.Status ?? "Pending";

            // Đảm bảo newStatus là một trong 4 giá trị hợp lệ
            if (newStatus != "Pending" && newStatus != "Shipping" && newStatus != "Completed" && newStatus != "Failed")
            {
                throw new InvalidOperationException("Invalid new status value.");
            }

            if (currentStatus == "Completed" || currentStatus == "Failed")
            {
                canUpdate = false; // Không chỉnh sửa 2 trạng thái cuối
            }
            else if (currentStatus == "Pending" && newStatus != "Shipping")
            {
                canUpdate = false; // Pending chỉ chuyển sang Shipping
            }
            else if (currentStatus == "Shipping" && (newStatus != "Completed" && newStatus != "Failed"))
            {
                canUpdate = false; // Shipping chỉ chuyển sang Completed hoặc Failed
            }

            if (canUpdate)
            {
                order.Status = newStatus;
                await _orderRepo.UpdateAsync(order);
            }
            else
            {
                throw new InvalidOperationException($"Cannot change status from '{currentStatus}' to '{newStatus}'.");
            }
        }
        public async Task<IEnumerable<Order_List>> GetOrdersByStatusAsync(string? status)
        {
            // Lọc trạng thái ngay trong Service nếu cần logic nghiệp vụ, 
            // hoặc gọi thẳng Repository (cách này thường nhanh hơn)
            if (string.IsNullOrEmpty(status) || status == "All")
            {
                return await _orderRepo.GetAllOrdersAsync();
            }
            return await _orderRepo.GetOrdersByStatusAsync(status);
        }
    }
}
