using MedicalStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalStore.Controllers.Admin
{
    public class AdminRatingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminRatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ratings = _context.Ratings
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(ratings);
        }

    }
}
