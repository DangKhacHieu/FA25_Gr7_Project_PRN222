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
    }
}
