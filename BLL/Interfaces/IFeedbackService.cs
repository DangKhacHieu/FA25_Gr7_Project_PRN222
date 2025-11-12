
using BLL.Common;
using DAL.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IFeedbackService
    {
        // Feedback
        List<Feedback> GetAllFeedbacks();
        Feedback GetFeedbackById(int id);
        void DeleteFeedback(int id);

        // Reply Feedback
        List<Reply_Feedback> GetRepliesByFeedbackId(int feedbackId);
        void AddReplyFeedback(Reply_Feedback reply);
        public enum FeedbackEligibility
        {
            Eligible,           // Được phép viết
            AlreadySubmitted,   // Đã viết rồi (trên sản phẩm này)
            PurchaseNotVerified // Chưa mua hàng
        }
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

﻿