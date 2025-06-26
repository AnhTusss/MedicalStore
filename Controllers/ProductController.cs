using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalStore.Controllers.Admin

{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách sản phẩm
        public IActionResult Index(int? categoryId)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            return View(products.ToList());
        }

        // Chi tiết sản phẩm + Đánh giá
        public IActionResult Details(int id)
        {
            var product = _context.Products
                                  .Include(p => p.Category)
                                  .FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            var ratings = _context.Ratings
                                  .Where(r => r.ProductId == id)
                                  .OrderByDescending(r => r.CreatedAt)
                                  .ToList();

            ViewBag.Ratings = ratings;

            return View(product);
        }

        // Form thêm sản phẩm
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    product.ImageUrl = "/images/" + fileName;
                }
                else
                {
                    product.ImageUrl = "/images/default.jpg";
                }

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // Form chỉnh sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // Xử lý chỉnh sửa
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product);
        }

        // Xóa sản phẩm
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Gửi đánh giá
        [HttpPost]
        public IActionResult AddRating(Rating rating)
        {
            if (ModelState.IsValid)
            {
                rating.CreatedAt = DateTime.Now;
                _context.Ratings.Add(rating);
                _context.SaveChanges();

                TempData["RatingSuccess"] = "Cảm ơn bạn đã đánh giá sản phẩm.";
            }

            return RedirectToAction("Details", new { id = rating.ProductId });
        }
        //Lấy danh mục kèm sản phẩm
        public IActionResult CategoriesWithProducts()
        {
            var categories = _context.Categories
                .Include(c => c.Products)
                .ToList();

            return View(categories);
        }

    }

}

