using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL;
using BLL.Services;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FA25_G7_PRN222_Web_ban_dien_thoai.Controllers
{
    public class CustomersController : Controller
    {
        private readonly PhoneContext _context;

        // này dùng để tìm kiếm 

        private readonly CustomerService _service;
        //private readonly ICustomerService _service;

        public CustomersController(PhoneContext context, CustomerService service)
        {
            _context = context;
            _service = service;
        }
        /* public CustomersController(PhoneContext context, ICustomerService service)
         {
             _context = context;
             _service = service;
         }*/


        // GET: Customers
        /*  public async Task<IActionResult> Index()
          {
              //return View(await _context.Customers.ToListAsync());

              // Chỉ lấy những khách hàng chưa bị block
              var activeCustomers = await _context.Customers
                  .Where(c => c.Status >= 0)
                  .ToListAsync();

              return View(activeCustomers);
          }*/

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,UserName,FullName,Email,PhoneNumber,Address,Password,Status,Sex,DOB,ImgCustomer")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,UserName,FullName,Email,PhoneNumber,Address,Password,Status,Sex,DOB,ImgCustomer")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.Status = -1;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }

        // update status
        public IActionResult ToggleStatus(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            // Nếu tài khoản bị xóa mềm (-1) thì không cho toggle
            if (customer.Status == -1)
            {
                TempData["Error"] = "Tài khoản này đã bị xóa mềm, không thể thay đổi trạng thái.";
                return RedirectToAction(nameof(Index));
            }

            // Đảo trạng thái
            customer.Status = (customer.Status == 1) ? 0 : 1;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // search account 
        /* public async Task<IActionResult> Index(string searchString = "")
         {
             List<Customer> customers;

             if (string.IsNullOrEmpty(searchString))
             {
                 customers = await _context.Customers
                     .Where(c => c.Status >= 0)
                     .ToListAsync();
             }
             else
             {
                 // Search chính xác theo FullName
                 customers = await _context.Customers
                     .Where(c => c.Status >= 0 && c.FullName.Contains(searchString))
                     .ToListAsync();
             }

             ViewData["CurrentFilter"] = searchString; // để giữ giá trị search trong textbox
             return View(customers);
         }*/

        // CustomersController.cs
        public async Task<IActionResult> Index(string searchString)
        {
            List<Customer> customers;

            if (string.IsNullOrEmpty(searchString))
            {
                customers = await _context.Customers
                    .Where(c => c.Status >= 0)
                    .ToListAsync();
            }
            else
            {
                // Search chính xác theo FullName
                customers = await _context.Customers
                    .Where(c => c.Status >= 0 && c.FullName.Contains(searchString))
                    .ToListAsync();
            }

            ViewData["CurrentFilter"] = searchString; // để giữ giá trị search trong textbox
            return View(customers);
        }


        [HttpGet]
        public JsonResult SuggestCustomer(string term)
        {
            var suggestions = _service.Suggest(term);
            return Json(suggestions);
        }


    }
}
