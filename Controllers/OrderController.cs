using MedicalStore.Data;
using MedicalStore.Models;
using MedicalStore.Helpers; // ✅ Thêm dòng này
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const string CartKey = "CartItems";

        public OrderController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Order/Checkout
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemSession>>(CartKey);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            return View();
        }

        // POST: /Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string customerName, string address, string phone, string paymentMethod)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemSession>>(CartKey);
            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                CustomerName = customerName,
                Address = address,
                Phone = phone,
                PaymentMethod = paymentMethod,
                OrderDate = DateTime.Now,
                Status = "Chờ xác nhận",
                UserId = user?.Id,
                OrderItems = cart.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove(CartKey);

            if (paymentMethod == "BankTransfer")
            {
                string qrImage = QRHelper.GenerateBankTransferQR(
                    bankCode: "BIDV",
                    accountNumber: "0123456789123456789",
                    accountName: "NHOM TAM",
                    amount: cart.Sum(x => x.Price * x.Quantity),
                    note: $"Don hang #{order.Id}"
                );

                TempData["PaymentMethod"] = paymentMethod;
                TempData["QrImage"] = qrImage;
                TempData["OrderId"] = order.Id;
            }

            return RedirectToAction("Success");
        }

        // GET: /Order/Success
        public IActionResult Success()
        {
            ViewBag.PaymentMethod = TempData["PaymentMethod"];
            ViewBag.QrImage = TempData["QrImage"];
            ViewBag.OrderId = TempData["OrderId"];
            return View();
        }

        //  POST: /Order/UpdateStatus/{id}
        [HttpPost("/Order/UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = "Đã chuyển khoản";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: /Order/UserHistory
        public async Task<IActionResult> UserHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: /Order/Cancel/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound();

            if (order.Status == "Chờ xác nhận" || order.Status == "Đang xử lý")
            {
                order.Status = "Đã huỷ";
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đơn hàng đã được huỷ.";
            }
            else
            {
                TempData["Error"] = "Đơn hàng đã được xử lý, không thể huỷ.";
            }

            return RedirectToAction("UserHistory");
        }
    }
}
