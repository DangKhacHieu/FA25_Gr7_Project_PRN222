using BLL.Common;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepo;
        private readonly IOrderRepository _orderRepo; // Dùng để xác minh đã mua hàng

        public FeedbackService(IFeedbackRepository feedbackRepo, IOrderRepository orderRepo)
        {
            _feedbackRepo = feedbackRepo;
            _orderRepo = orderRepo;
        }

        // (Chức năng 1) Thêm Feedback
        public async Task<OperationResult> AddFeedbackAsync(int customerId, int productId, string content, int ratePoint)
        {
            // Do model cũ, chúng ta chỉ có thể check xem đã submit CHUNG chưa
            if (await _feedbackRepo.HasFeedbackSubmittedAsync(customerId, productId))
            {
                return new OperationResult(false, "Bạn đã đánh giá sản phẩm này rồi.");
            }

            Feedback newFeedback = new Feedback
            {
                CustomerID = customerId,
                ProductID = productId,
                Content = content,
                RatePoint = ratePoint
            };

            await _feedbackRepo.AddFeedbackAsync(newFeedback);
            return new OperationResult(true, "Gửi đánh giá thành công!");
        }

        // (Chức năng 2) Lấy Feedbacks cho Product Details
        public async Task<IEnumerable<Feedback>> GetFeedbacksForProductAsync(int productId)
        {
            return await _feedbackRepo.GetFeedbacksByProductIdAsync(productId);
        }

        // (Chức năng 1 - Helper) Kiểm tra quyền
        public async Task<FeedbackEligibility> CheckFeedbackEligibilityAsync(int customerId, int orderId, int productId)
        {
            // Check 1: Xác minh đã mua hàng (Check xem user có sở hữu order đó VÀ product có trong order đó)
            var order = await _orderRepo.GetOrderByIdAsync(orderId); // Giả định hàm này đã Include Order_Details

            if (order == null || order.CustomerID != customerId)
            {
                return FeedbackEligibility.PurchaseNotVerified; // Không phải chủ đơn
            }

            bool productInOrder = order.Order_Details != null && order.Order_Details.Any(od => od.ProductID == productId);
            if (!productInOrder)
            {
                return FeedbackEligibility.PurchaseNotVerified; // Sản phẩm không có trong đơn
            }

            // Check 2: Kiểm tra xem đã review (bất kỳ lúc nào) chưa (Do giới hạn của model)
            if (await _feedbackRepo.HasFeedbackSubmittedAsync(customerId, productId))
            {
                return FeedbackEligibility.AlreadySubmitted; // Đã review rồi
            }

            return FeedbackEligibility.Eligible; // Được phép review
        }
    }
}