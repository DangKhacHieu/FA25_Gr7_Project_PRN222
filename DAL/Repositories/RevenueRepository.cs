using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
   public class RevenueRepository : IRevenueRepository
    {
        private readonly PhoneContext _context;
        private const string SuccessStatus = "Thành công";

        public RevenueRepository(PhoneContext context)
        {
            _context = context;
        }

        public async Task<List<Order_List>> GetSuccessfulOrdersAsync()
        {
            return await _context.Order_Lists
                .Where(o => o.Status == SuccessStatus)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<List<Order_List>> FilterOrdersAsync(int? year, int? quarter, int? month)
        {
            var query = _context.Order_Lists
                                .Where(o => o.Status == SuccessStatus)
                                .AsQueryable();

            if (year.HasValue)
                query = query.Where(o => o.Date.HasValue && o.Date.Value.Year == year.Value);

            if (quarter.HasValue)
                query = query.Where(o => o.Date.HasValue && ((o.Date.Value.Month - 1) / 3 + 1) == quarter.Value);

            if (month.HasValue)
                query = query.Where(o => o.Date.HasValue && o.Date.Value.Month == month.Value);

            return await query.OrderByDescending(o => o.Date).ToListAsync();
        }
    }
}
