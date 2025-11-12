using System.Linq;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace FA25_G7_PRN222_Web_ban_dien_thoai.Controllers
{
    public class ImportProductController : Controller
    {
        private readonly IImportProductService _service;

        public ImportProductController(IImportProductService service)
        {
            _service = service;
        }

        // GET: Hiển thị danh sách sản phẩm
        [HttpGet]
        public IActionResult Index()
        {
            var products = _service.GetAllProducts();
            return View(products);
        }

        // GET: Manual Add/Update sản phẩm
        [HttpGet]
        public IActionResult ManualImport(int? id)
        {
            if (id == null)
                return View(new Product());

            var product = _service.GetAllProducts().FirstOrDefault(p => p.ProductID == id);
            if (product == null)
                return NotFound();

            product.Quantity_Product = null; // reset số lượng nhập
            return View(product);
        }

        // POST: Manual Add/Update sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ManualImport(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _service.AddOrUpdateProductManual(product);
            return RedirectToAction("Index");
        }
    }
}
