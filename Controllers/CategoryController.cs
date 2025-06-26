using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị sản phẩm theo từng danh mục: /Category/Index?id=2
        public IActionResult Index(int id)
        {
            var category = _context.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = category.Name;
            return View(category.Products.ToList()); // Views/Category/Index.cshtml
        }

        // ✅ Trang danh sách tất cả danh mục 
        public IActionResult List()
        {
            var categories = _context.Categories.ToList();
            return View(categories); // Views/Category/List.cshtml 
        }

        // Trang thêm danh mục
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // Views/Category/Create.cshtml
        }

        // Xử lý thêm danh mục
        [HttpPost]
        public IActionResult Create(Category c)
        {
            if (!ModelState.IsValid)
                return View(c);

            _context.Categories.Add(c);
            _context.SaveChanges();
            return RedirectToAction("List"); // Chuyển về danh sách danh mục sau khi thêm
        }
    }
}
