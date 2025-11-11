using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
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