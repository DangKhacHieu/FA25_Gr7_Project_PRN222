using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FA25_G7_PRN222_Web_ban_dien_thoai.Controllers
{
    public class RevenueController : Controller
    {
        private readonly IRevenueService _revenueService;
        private const int PageSize = 5; // Số bản ghi / trang

        public RevenueController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        // GET: /Revenue/Index
        public async Task<IActionResult> Index(int? year, int? quarter, int? month, int page = 1)
        {
            // 1. Lấy danh sách đã lọc (hoặc tất cả)
            var orders = await _revenueService.FilterRevenueAsync(year, quarter, month);

            // 2. Tổng số bản ghi
            var totalOrders = orders.Count;

            // 3. Phân trang
            var pagedOrders = orders
                .OrderByDescending(o => o.Date)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // 4. Truyền ViewBag
            ViewBag.Orders = pagedOrders;
            ViewBag.TotalPages = (int)Math.Ceiling(totalOrders / (double)PageSize);
            ViewBag.CurrentPage = page;

            ViewBag.SelectedYear = year;
            ViewBag.SelectedQuarter = quarter;
            ViewBag.SelectedMonth = month;

            // Danh sách năm/quý/tháng cho dropdown
            ViewBag.Years = orders.Select(o => o.Date?.Year ?? 0).Distinct().OrderByDescending(y => y).ToList();
            ViewBag.Quarters = new List<int> { 1, 2, 3, 4 };
            ViewBag.Months = Enumerable.Range(1, 12).ToList();

            return View();
        }
    }
}
