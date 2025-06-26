using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStore.Controllers.Admin
{
    public class AdminArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.OrderByDescending(a => a.CreatedAt).ToList();
            return View("~/Views/Admin/AdminArticle/Index.cshtml", articles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Admin/AdminArticle/Create.cshtml");
        }

        [HttpPost]
        public IActionResult Create(Article article, IFormFile? ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                article.ImageUrl = "/images/" + fileName;
            }

            article.CreatedAt = DateTime.Now;
            _context.Articles.Add(article);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var article = _context.Articles.Find(id);
            return View("~/Views/Admin/AdminArticle/Edit.cshtml", article);
        }

        [HttpPost]
        public IActionResult Edit(int id, Article updatedArticle, IFormFile? ImageFile)
        {
            var article = _context.Articles.Find(id);
            if (article == null) return NotFound();

            article.Title = updatedArticle.Title;
            article.Content = updatedArticle.Content;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                article.ImageUrl = "/images/" + fileName;
            }

            // Nếu không có ảnh mới thì giữ nguyên ImageUrl cũ
            _context.Articles.Update(article);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var article = _context.Articles.Find(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
