using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IFeedbackRepository
    {
        /// <summary>
        /// (Chức năng 2) Lấy tất cả feedback của một sản phẩm để hiển thị ở trang Details.
        /// </summary>
        Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(int productId);

        /// <summary>
        /// (Chức năng 1) Thêm một feedback mới vào DB.
        /// </summary>
        Task AddFeedbackAsync(Feedback feedback);

        /// <summary>
        /// (Chức năng 1) Kiểm tra xem khách hàng đã review sản phẩm này BAO GIỜ chưa.
        /// </summary>
        Task<bool> HasFeedbackSubmittedAsync(int customerId, int productId);
    }
}