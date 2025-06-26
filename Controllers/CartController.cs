using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartKey = "CartItems";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemSession>>(CartKey) ?? new List<CartItemSession>();
            return View(cart);
        }

        // ✅ Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public IActionResult Add(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItemSession>>(CartKey) ?? new List<CartItemSession>();
            var item = cart.FirstOrDefault(c => c.ProductId == id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItemSession
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        // ✅ Mua ngay – đã đăng nhập
        [HttpPost]
        [Authorize]
        public IActionResult BuyNow(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            var cart = new List<CartItemSession>
            {
                new CartItemSession
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                }
            };

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Checkout", "Order");
        }

        // ✅ Mua ngay bằng GET (nếu dùng URL /Cart/Checkout?productId=...&quantity=...)
        [HttpGet]
        [Authorize]
        public IActionResult Checkout(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            var cart = new List<CartItemSession>
            {
                new CartItemSession
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                }
            };

            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Checkout", "Order");
        }

        // ✅ Nếu chưa đăng nhập → lưu ID sản phẩm vào session rồi chuyển login
        [HttpGet]
        public IActionResult RedirectToLoginForBuyNow(int id)
        {
            HttpContext.Session.SetInt32("BuyNowProductId", id);
            return Redirect("/Account/Login?returnUrl=/Account/HandleBuyNowRedirect");
        }

        // ✅ Xoá sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemSession>>(CartKey) ?? new List<CartItemSession>();
            cart.RemoveAll(c => c.ProductId == id);
            HttpContext.Session.SetObject(CartKey, cart);
            return RedirectToAction("Index");
        }

        // ✅ Cập nhật từng sản phẩm
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemSession>>(CartKey) ?? new List<CartItemSession>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                HttpContext.Session.SetObject(CartKey, cart);
            }

            return RedirectToAction("Index");
        }

        // ✅ Cập nhật toàn bộ giỏ hàng từ 1 form
        [HttpPost]
        public IActionResult UpdateAll(List<CartItemSession> cartItems)
        {
            var updatedCart = new List<CartItemSession>();

            foreach (var item in cartItems)
            {
                if (item.Quantity > 0)
                {
                    var product = _context.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        updatedCart.Add(new CartItemSession
                        {
                            ProductId = item.ProductId,
                            ProductName = product.Name,
                            Price = product.Price,
                            Quantity = item.Quantity
                        });
                    }
                }
            }

            HttpContext.Session.SetObject("CartItems", updatedCart);
            return RedirectToAction("Index");
        }

    }
}
