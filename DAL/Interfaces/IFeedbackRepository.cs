using DAL.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IFeedbackRepository
    {
        List<Feedback> GetAllFeedbacks();
        Feedback GetFeedbackById(int id);
        void DeleteFeedback(int id);
        void Save();

        // Reply Feedback
        List<Reply_Feedback> GetRepliesByFeedbackId(int feedbackId);
        void AddReplyFeedback(Reply_Feedback reply);
    }
}
