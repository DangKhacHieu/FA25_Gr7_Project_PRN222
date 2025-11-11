using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly IProductService _productService;
        private readonly DbContext _dbContext; //DbContext để quản lý Transaction

        
        public OrderService(IOrderRepository orderRepo, IProductService productService, DbContext dbContext)
        {
            _orderRepo = orderRepo;
            _productService = productService;
            _dbContext = dbContext; // Gán DbContext
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
                // Kiểm tra điều kiện hoàn trả kho hàng: chuyển sang Failed, và chưa Failed/Completed trước đó
                bool shouldReturnStock = (currentStatus != "Failed" && currentStatus != "Completed")
                                         && newStatus == "Failed";

                // --- BẮT ĐẦU GIAO DỊCH ĐỂ ĐẢM BẢO TÍNH TOÀN VẸN ---
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    // 2. THAO TÁC 1: Cập nhật trạng thái đơn hàng
                    order.Status = newStatus;
                    await _orderRepo.UpdateAsync(order);

                    // 3. THAO TÁC 2: Hoàn trả kho hàng nếu cần
                    if (shouldReturnStock)
                    {
                        if (order.Order_Details != null)
                        {
                            foreach (var detail in order.Order_Details)
                            {
                                // ProductID là int non-nullable, Quantity là int? nullable
                                if (detail.Quantity.HasValue)
                                {
                                    await _productService.IncreaseProductQuantityAsync(
                                        detail.ProductID, // Dùng trực tiếp
                                        detail.Quantity.Value
                                    );
                                }
                            }
                        }
                    }

                    // 4. COMMIT giao dịch nếu cả hai thao tác thành công
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    // 5. Nếu có lỗi (DB, logic, tồn kho), ROLLBACK
                    await transaction.RollbackAsync();
                    // Ném lỗi lên Controller để hiển thị thông báo
                    throw;
                }
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
        public async Task<PagedResult<Order_List>> GetOrdersPaginatedAsync(int pageIndex, int pageSize, string? status = null)
        {
            if (pageIndex < 1) pageIndex = 1;

            var totalCount = await _orderRepo.CountOrdersAsync(status);
            var items = await _orderRepo.GetPagedOrdersAsync(pageIndex, pageSize, status);

            // TRẢ VỀ PagedResult<Order_List>
            return new PagedResult<Order_List>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
