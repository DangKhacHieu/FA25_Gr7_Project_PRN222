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
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IProductService _productService;
        private readonly DbContext _dbContext; //DbContext để quản lý Transaction


        public OrderService(IOrderRepository orderRepo, IProductService productService, DbContext dbContext, ICartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _productService = productService;
            _dbContext = dbContext;
            _cartRepo = cartRepo;// Gán DbContext
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

        // --- TRIỂN KHAI CODE MỚI (CHO KHÁCH HÀNG) ---

        public async Task<IEnumerable<Order_List>> GetOrderHistoryAsync(int customerId)
        {
            // Gọi thẳng xuống Repository
            return await _orderRepo.GetOrdersByCustomerIdAsync(customerId);
        }

        // Hàm GetOrderDetailsForCustomerAsync (từ câu trả lời trước) vẫn hoạt động tốt
        public async Task<Order_List?> GetOrderDetailsForCustomerAsync(int orderId, int customerId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null || order.CustomerID != customerId)
            {
                return null;
            }
            return order;
        }
        public async Task<Order_List> CreateOrderFromCartAsync(
        int customerId,
        string address,
        string phoneNumber,
        string paymentMethod)
        {
            var cart = await _cartRepo.GetCartByCustomerAsync(customerId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Giỏ hàng của bạn đang trống.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var newOrder = new Order_List
                {
                    CustomerID = customerId,
                    Address = address,
                    PhoneNumber = phoneNumber,
                    Date = DateTime.Now,

                    // --- SỬA LỖI 1 ---
                    Total = Convert.ToInt32(cart.TotalPrice), // Ép kiểu từ decimal sang int?

                    Order_Details = new List<Order_Details>()
                };

                foreach (var item in cart.CartItems)
                {
                    if (item.Product == null)
                        continue;

                    newOrder.Order_Details.Add(new Order_Details 
                    {
                        ProductID = item.ProductId,
                        Quantity = item.Quantity, // 'Quantity' là int, không phải int?
                    });

                    // --- SỬA LỖI 2 ---
                    await _productService.DecreaseProductQuantityAsync(item.ProductId, item.Quantity); // Bỏ ?? 0
                }

                newOrder.Status = (paymentMethod == "Momo") ? "Complete" : "Pending";

                var createdOrder = await _orderRepo.CreateOrderAsync(newOrder);
                await _cartRepo.ClearCartAsync(customerId);
                await transaction.CommitAsync();
                return createdOrder;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CancelOrderAsync(int orderId, int customerId)
        {
            // 1. Lấy đơn hàng (BẮT BUỘC phải .Include Order_Details để hoàn kho)
            //    Hàm GetOrderByIdAsync của bạn đã làm việc này rồi.
            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            // 2. Kiểm tra bảo mật và logic
            if (order == null || order.CustomerID != customerId)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn hàng hoặc bạn không có quyền.");
            }
            if (order.Status != "Pending")
            {
                throw new InvalidOperationException("Chỉ có thể hủy đơn hàng khi đang ở trạng thái 'Pending'.");
            }

            // 3. Bắt đầu Transaction để đảm bảo an toàn
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 4. THAO TÁC 1: HỦY ĐƠN
                // Đổi trạng thái đơn hàng
                order.Status = "Failed"; // (Hoặc "Cancelled", tùy bạn)

                // --- SỬA LỖI ---
                // Báo cho EF Core biết là 'order' đã bị thay đổi
                // Điều này đảm bảo trạng thái sẽ được lưu cùng lúc với kho hàng.
                _dbContext.Entry(order).State = EntityState.Modified;
                // --- KẾT THÚC SỬA LỖI ---

                // 5. THAO TÁC 2: HOÀN KHO (CỘNG VÔ TRONG KIA)
                if (order.Order_Details != null)
                {
                    // Lặp qua "tổng hết mấy cái sản phẩm"
                    foreach (var detail in order.Order_Details)
                    {
                        // Dưới đây là 2 cách, chọn 1 cách bạn đang dùng:

                        // CÁCH 1: Nếu bạn dùng "Unit of Work" (khuyên dùng)
                        // (Hãy chắc chắn hàm InContext của bạn không gọi SaveChangesAsync)
                        // await _productService.IncreaseProductQuantityInContext(
                        //     detail.ProductID, 
                        //     detail.Quantity ?? 0 
                        // );

                        // CÁCH 2: Nếu hàm Increase của bạn tự gọi SaveChangesAsync
                        // (Đây là cách bạn nói đang chạy "cộng vô" được)
                        await _productService.IncreaseProductQuantityAsync(
                            detail.ProductID,
                            detail.Quantity ?? 0 // Đã sửa lỗi CS1503 (int? to int)
                        );
                    }
                }

                // 6. LƯU TẤT CẢ THAY ĐỔI
                // Nếu bạn dùng CÁCH 1 (Unit of Work), dòng này là BẮT BUỘC:
                await _dbContext.SaveChangesAsync();

                // (Nếu bạn dùng CÁCH 2, dòng SaveChangesAsync() ở trên vẫn cần
                // để lưu thay đổi 'order.Status' mà 'IncreaseProductQuantityAsync' không biết)

                // 7. Hoàn tất Transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Nếu có lỗi, Rollback cả việc đổi Status và cộng kho
                await transaction.RollbackAsync();
                throw; // Ném lỗi lên PageModel
            }
        }

        // --- TRIỂN KHAI HÀM NHẬN HÀNG (RECEIVE) ---
        public async Task MarkOrderAsReceivedAsync(int orderId, int customerId)
        {
            // 1. Lấy đơn hàng
            var order = await _orderRepo.GetOrderByIdAsync(orderId); // Hàm này không cần Include

            // 2. Kiểm tra bảo mật và logic
            if (order == null || order.CustomerID != customerId)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn hàng hoặc bạn không có quyền.");
            }
            if (order.Status != "Shipping")
            {
                throw new InvalidOperationException("Chỉ có thể xác nhận khi đơn hàng đang 'Shipping'.");
            }

            // 3. Cập nhật trạng thái
            order.Status = "Completed";

            // 4. Lưu thay đổi
            await _orderRepo.UpdateAsync(order); // Giả định hàm Update của bạn chỉ cập nhật Status và Save
        }
    }
}

