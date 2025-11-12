using DAL.Data;
using DAL.Interfaces;
using DAL.Models;

using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


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
            // ✅ Xóa hết reply liên quan đến feedback trước
            var replies = _context.Reply_Feedbacks.Where(r => r.FeedbackID == id).ToList();
            if (replies.Any())
            {
                _context.Reply_Feedbacks.RemoveRange(replies);
            }

            // ✅ Sau đó xóa feedback
            var feedback = _context.Feedbacks.FirstOrDefault(f => f.FeedbackID == id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }

            _context.SaveChanges();
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
    

        public async Task AddFeedbackAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByProductIdAsync(int productId)
        {
            return await _context.Feedbacks
                .Where(f => f.ProductID == productId)
                .Include(f => f.Customer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasFeedbackSubmittedAsync(int customerId, int productId)
        {
            return await _context.Feedbacks
                .AnyAsync(f => f.CustomerID == customerId && f.ProductID == productId);
        }
    }
}

