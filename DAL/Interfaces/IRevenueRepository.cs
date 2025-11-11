using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Interfaces
{
    public interface IRevenueRepository
    {
        Task<List<Order_List>> GetSuccessfulOrdersAsync();

        // 🔹 Thêm phương thức lọc
        Task<List<Order_List>> FilterOrdersAsync(int? year, int? quarter, int? month);
    }
}

