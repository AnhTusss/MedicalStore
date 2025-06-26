using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStore.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Add(int productId, string content, int rating)
        {
            var comment = new Comment
            {
                ProductId = productId,
                Content = content,
                Rating = rating,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Details", "Product", new { id = productId });
        }

        public IActionResult ProductComments(int productId)
        {
            var comments = _context.Comments
                .Where(c => c.ProductId == productId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            var positive = comments.Where(c => c.Rating >= 4).ToList();
            var negative = comments.Where(c => c.Rating <= 2).ToList();

            ViewBag.Positive = positive;
            ViewBag.Negative = negative;
            return View(comments);
        }
    }
}
