using MedicalStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalStore.Models;

namespace MedicalStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUserController(UserManager<IdentityUser> userManager,
                                   RoleManager<IdentityRole> roleManager,
                                   ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // Hiển thị danh sách người dùng (ngoại trừ admin chính) và vai trò
        public async Task<IActionResult> Index()
        {
            // Bỏ tài khoản admin ra khỏi danh sách
            var users = await _userManager.Users
                                          .Where(u => u.UserName != "admin")
                                          .ToListAsync();

            var roles = await _roleManager.Roles.ToListAsync();

            // Tạo dictionary chứa danh sách vai trò của mỗi người dùng
            var userRolesDict = new Dictionary<string, List<string>>();
            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRolesDict[user.Id] = rolesForUser.ToList();
            }

            ViewBag.UserRoles = userRolesDict;
            return View((users, roles));
        }

        // Cập nhật vai trò người dùng
        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.UserName == "admin")
                return BadRequest("Không thể thay đổi quyền của tài khoản admin chính.");

            // Xóa tất cả vai trò hiện tại
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Tạo vai trò nếu chưa tồn tại
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            // Gán vai trò mới
            await _userManager.AddToRoleAsync(user, role);

            // Ghi lại lịch sử phân quyền
            var assignment = new RoleAssignment
            {
                UserId = user.Id,
                RoleName = role,
                AssignedAt = DateTime.Now
            };
            _context.RoleAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
