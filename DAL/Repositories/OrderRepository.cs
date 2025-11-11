using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PhoneContext _context;

        public OrderRepository(PhoneContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order_List>> GetAllOrdersAsync()
        {
            // Lấy Order_List và Include thông tin Customer và Staff liên quan
            // Để hiển thị Tên khách hàng và Tên nhân viên xử lý
            return await _context.Order_Lists
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .OrderByDescending(o => o.Date) // Sắp xếp đơn hàng mới nhất lên đầu
                .ToListAsync();
        }

        public async Task<Order_List?> GetOrderByIdAsync(int id)
        {
            // Lấy Order_List và Include tất cả các chi tiết liên quan
            return await _context.Order_Lists
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .Include(o => o.Order_Details!) // Bắt buộc phải có Order_Details
                    .ThenInclude(od => od.Product) // Trong chi tiết, phải có thông tin Product
                .FirstOrDefaultAsync(o => o.OrderID == id);
        }

        public async Task UpdateAsync(Order_List updatedOrder)
        {
            // Lấy Entity hiện tại từ DB (không Include các Navigation Property)
            var existingOrder = await _context.Order_Lists.FindAsync(updatedOrder.OrderID);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {updatedOrder.OrderID} not found in DB for update.");
            }

            // CHỈ CẬP NHẬT thuộc tính Status
            existingOrder.Status = updatedOrder.Status;

            // Đánh dấu chỉ thuộc tính Status là Modified, an toàn hơn
            _context.Entry(existingOrder).Property(o => o.Status).IsModified = true;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order_List>> GetOrdersByStatusAsync(string status)
        {
            // Lấy query ban đầu
            var query = _context.Order_Lists
                .Include(o => o.Customer)
                .Include(o => o.Staff)
                .OrderByDescending(o => o.Date)
                .AsQueryable();

            // Áp dụng lọc
            query = query.Where(o => o.Status == status);

            return await query.ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Order_Lists.AnyAsync(e => e.OrderID == id);
        }
        public async Task<IEnumerable<Order_List>> GetPagedOrdersAsync(int pageIndex, int pageSize, string? status = null)
        {
            var query = _context.Order_Lists.AsQueryable();

            // Áp dụng lọc trạng thái (filter)
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(o => o.Status == status);
            }

            // Áp dụng phân trang (Skip/Take)
            return await query.Include(o => o.Customer)
                              .Include(o => o.Staff)
                              .OrderByDescending(o => o.Date)
                              .Skip((pageIndex - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }

        // THÊM: Phương thức đếm tổng (có lọc)
        public async Task<int> CountOrdersAsync(string? status = null)
        {
            var query = _context.Order_Lists.AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(o => o.Status == status);
            }
            return await query.CountAsync();
        }
    }
}
