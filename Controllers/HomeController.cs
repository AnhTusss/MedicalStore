using System.Diagnostics;
using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MedicalStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Trang chủ với hiển thị danh sách sản phẩm và danh mục
        public IActionResult Index(int? categoryId)
        {
            var categories = _context.Categories.ToList();
            List<Product> products;

            if (categoryId.HasValue)
            {
                products = _context.Products
                    .Where(p => p.CategoryId == categoryId.Value)
                    .ToList();
            }
            else
            {
                products = _context.Products.ToList();
            }

            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = categoryId;

            // KHÔNG ẩn navbar
            ViewData["HideNavbar"] = "false";

            return View(products);
        }

        // Trang chính sách
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Chính sách bảo mật";
            return View();
        }

        // Trang liên hệ
        public IActionResult Contact()
        {
            ViewData["Title"] = "Liên hệ";
            return View();
        }

        // Xử lý lỗi hệ thống
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
