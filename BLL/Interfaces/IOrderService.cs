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
    }
}
