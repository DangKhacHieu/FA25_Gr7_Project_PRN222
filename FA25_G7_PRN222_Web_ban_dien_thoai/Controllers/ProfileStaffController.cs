using System;
using System.Collections.Generic;
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
    public class ProfileStaffController : Controller
    {
        private readonly IProfileStaffService _profileService;

        public ProfileStaffController(IProfileStaffService profileService)
        {
            _profileService = profileService;
        }

        // Hiển thị trang thông tin cá nhân
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var staff = await _profileService.GetProfileByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // Hiển thị form chỉnh sửa
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _profileService.GetProfileByIdAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // Lưu thay đổi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Staff model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existing = await _profileService.GetProfileByIdAsync(model.StaffID);
            if (existing == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin
            existing.FullName = model.FullName;
            existing.Email = model.Email;
            existing.PhoneNumber = model.PhoneNumber;
            existing.Address = model.Address;
            existing.Province_City = model.Province_City;
            existing.Sex = model.Sex;
            existing.DOB = model.DOB;

            await _profileService.UpdateProfileAsync(existing);

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index", new { id = existing.StaffID });
        }
    }
}
