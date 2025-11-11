using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IOrderRepository
    {
        // Lấy tất cả đơn hàng, bao gồm Customer và Staff để hiển thị thông tin chi tiết
        Task<IEnumerable<Order_List>> GetAllOrdersAsync();

        Task<IEnumerable<Order_List>> GetOrdersByStatusAsync(string status);


        // Lấy chi tiết đơn hàng (bao gồm Order_Details và Product)
        Task<Order_List?> GetOrderByIdAsync(int id);

        // Cập nhật trạng thái đơn hàng
        Task UpdateAsync(Order_List order);

        // Kiểm tra Order có tồn tại không (dùng cho Update)
        Task<bool> ExistsAsync(int id);
    }
}
