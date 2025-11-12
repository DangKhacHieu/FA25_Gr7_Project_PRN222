using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly PhoneContext _context;

        public FeedbackRepository(PhoneContext context)
        {
            _context = context;
        }

        // --- FEEDBACK ---
        public List<Feedback> GetAllFeedbacks() => _context.Feedbacks.ToList();

        public Feedback GetFeedbackById(int id) => _context.Feedbacks.FirstOrDefault(f => f.FeedbackID == id);

        public void DeleteFeedback(int id)
        {
            var fb = GetFeedbackById(id);
            if (fb != null)
            {
                _context.Feedbacks.Remove(fb);
                Save();
            }
        }

        // --- REPLY FEEDBACK ---
        public List<Reply_Feedback> GetRepliesByFeedbackId(int feedbackId)
        {
            return _context.Reply_Feedbacks
                .Where(r => r.FeedbackID == feedbackId)
                .ToList();
        }

        public void AddReplyFeedback(Reply_Feedback reply)
        {
            _context.Reply_Feedbacks.Add(reply);
            Save();
        }

        public void Save() => _context.SaveChanges();
    }
}
