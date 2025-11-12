using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class RevenueService: IRevenueService
    {
        private readonly IRevenueRepository _revenueRepository;

        public RevenueService(IRevenueRepository revenueRepository)
        {
            _revenueRepository = revenueRepository;
        }

        public async Task<List<Order_List>> GetSuccessfulRevenueOrdersAsync()
        {
            return await _revenueRepository.GetSuccessfulOrdersAsync();
        }

        public async Task<List<Order_List>> FilterRevenueAsync(int? year, int? quarter, int? month)
        {
            return await _revenueRepository.FilterOrdersAsync(year, quarter, month);
        }
    }
}
