using BLL.Interfaces;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FA25_G7_PRN222_Web_ban_dien_thoai.Controllers
{
    public class StaffsController : Controller
    {
        private readonly IStaffService _staffService;
        private readonly PhoneContext _context;

        public StaffsController(PhoneContext context, IStaffService staffService)
        {
            _context = context;
            _staffService = staffService;
        }

        // GET: Staffs
        public async Task<IActionResult> Index()
        {
            var activeStaffs = await _context.Staffs
                .Where(s => s.Status == 0)
                .ToListAsync();

            return View(activeStaffs);
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .FirstOrDefaultAsync(m => m.StaffID == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffID,Address,Email,Password,FullName,PhoneNumber,Username,CCCD,Province_City,DOB,Sex,Status")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                staff.Status = 0;
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffID,Address,Email,Password,FullName,PhoneNumber,Username,CCCD,Province_City,DOB,Sex,Status")] Staff staff)
        {
            if (id != staff.StaffID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.StaffID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }


        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                staff.Status = 1; // Xóa mềm
                _context.Update(staff);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
            return _context.Staffs.Any(e => e.StaffID == id);
        }

        public IActionResult Login()
        {
            // Kiểm tra Session đơn giản
            if (HttpContext.Session.GetInt32("StaffId") != null)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        // [POST] /Staff/Login
        [HttpPost]
       
        public async Task<IActionResult> Login(string username, string password)
        {
          

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                // Thiết lập thông báo lỗi tạm thời để View có thể hiển thị
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            // 1. Xác thực (Service sẽ so sánh plaintext)
            var staff = await _staffService.AuthenticateStaffAsync(username, password);

            if (staff == null)
            {
                TempData["ErrorMessage"] = "Sai tên đăng nhập hoặc mật khẩu.";
                return View();
            }

            // 2. TẠO PHIÊN ĐĂNG NHẬP
            HttpContext.Session.SetInt32("StaffId", staff.StaffID);
            HttpContext.Session.SetString("StaffUsername", staff.Username!);

            // 3. Chuyển hướng thành công
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ✨ THÊM BẢO MẬT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa tất cả session (đăng xuất)
                                         // Chuyển hướng về trang Login
            return RedirectToAction("Login", "Staffs");
        }
    }
}
