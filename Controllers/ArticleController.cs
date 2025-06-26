using MedicalStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStore.Controllers
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.OrderByDescending(a => a.CreatedAt).ToList();
            return View(articles);
        }

        public IActionResult Details(int id)
        {
            var article = _context.Articles.Find(id);
            return View(article);
        }
    }
}
