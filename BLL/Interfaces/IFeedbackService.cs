using BLL.Common; // File OperationResult của bạn
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    // Enum để xác định trạng thái cho UI
    public enum FeedbackEligibility
    {
        Eligible,           // Được phép viết
        AlreadySubmitted,   // Đã viết rồi (trên sản phẩm này)
        PurchaseNotVerified // Chưa mua hàng
    }

    public interface IFeedbackService
    {
        /// <summary>
        /// (Chức năng 2) Lấy tất cả feedback cho trang Product Details.
        /// </summary>
        Task<IEnumerable<Feedback>> GetFeedbacksForProductAsync(int productId);

        /// <summary>
        /// (Chức năng 1) Thêm một feedback mới.
        /// </summary>
        Task<OperationResult> AddFeedbackAsync(int customerId, int productId, string content, int ratePoint);

        /// <summary>
        /// (Chức năng 1 - Helper) Kiểm tra xem khách hàng có quyền review sản phẩm này
        /// (dựa trên 1 đơn hàng cụ thể) hay không.
        /// </summary>
        Task<FeedbackEligibility> CheckFeedbackEligibilityAsync(int customerId, int orderId, int productId);
    }
}