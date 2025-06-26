using MedicalStore.Data;
using MedicalStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login");
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                if (roles.Contains("Admin"))
                    return RedirectToAction("Dashboard", "AdminHome");

                return RedirectToAction("Index", "Product");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            ViewData["ReturnUrl"] = returnUrl ?? string.Empty;
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var info = new AppUserInfo
                {
                    UserId = user.Id,
                    FullName = username
                };
                _context.AppUserInfos.Add(info);
                await _context.SaveChangesAsync();

                const string defaultRole = "User";
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                    await _roleManager.CreateAsync(new IdentityRole(defaultRole));

                await _userManager.AddToRoleAsync(user, defaultRole);

                TempData["RegisterSuccess"] = "Chúc mừng bạn đã đăng ký thành công tài khoản!";
                return RedirectToAction("Login");
            }

            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();

        [Authorize(Roles = "User")]
        public IActionResult Dashboard() => View();

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var info = await _context.AppUserInfos.FirstOrDefaultAsync(i => i.UserId == user.Id);

            var model = new UpdateProfileModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = info?.FullName,
                Address = info?.Address
            };
            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);

            var info = await _context.AppUserInfos.FirstOrDefaultAsync(i => i.UserId == user.Id);
            if (info != null)
            {
                info.FullName = model.FullName;
                info.Address = model.Address;
                _context.AppUserInfos.Update(info);
                await _context.SaveChangesAsync();
            }

            if (result.Succeeded)
                ViewBag.Message = "Cập nhật thành công";
            else
                ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));

            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                ViewBag.Message = "Thay đổi mật khẩu thành công.";
                return View();
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [Authorize]
        public IActionResult HandleBuyNowRedirect()
        {
            var productId = HttpContext.Session.GetInt32("BuyNowProductId");
            if (productId.HasValue)
            {
                HttpContext.Session.Remove("BuyNowProductId");

                var product = _context.Products.Find(productId.Value);
                if (product != null)
                {
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

                    HttpContext.Session.SetObject("CartItems", cart);
                }

                return RedirectToAction("Checkout", "Order");
            }

            return RedirectToAction("Index", "Product");
        }
    }
}
