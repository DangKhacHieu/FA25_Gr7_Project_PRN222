using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _repo;

        public FeedbackService(IFeedbackRepository repo)
        {
            _repo = repo;
        }

        // --- FEEDBACK ---
        public List<Feedback> GetAllFeedbacks() => _repo.GetAllFeedbacks();

        public Feedback GetFeedbackById(int id) => _repo.GetFeedbackById(id);

        public void DeleteFeedback(int id) => _repo.DeleteFeedback(id);

        // --- REPLY FEEDBACK ---
        public List<Reply_Feedback> GetRepliesByFeedbackId(int feedbackId)
        {
            return _repo.GetRepliesByFeedbackId(feedbackId);
        }

        public void AddReplyFeedback(Reply_Feedback reply)
        {
            _repo.AddReplyFeedback(reply);
        }
    }
}
