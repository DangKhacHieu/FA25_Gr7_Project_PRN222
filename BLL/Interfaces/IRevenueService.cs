using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface IRevenueService
    {
        Task<List<Order_List>> GetSuccessfulRevenueOrdersAsync();
        Task<List<Order_List>> FilterRevenueAsync(int? year, int? quarter, int? month);
    }
}
