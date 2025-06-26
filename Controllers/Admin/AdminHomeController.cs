using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalStore.Controllers.Admin
{
    [Authorize(Roles = "Admin")] // ✅ Chỉ Admin mới có quyền truy cập
    public class AdminHomeController : Controller
    {
        // GET: /AdminHome/Dashboard
        public IActionResult Dashboard()
        {
            // ✅ Trả về View chào mừng Admin tại đường dẫn: /Views/Admin/AdminHome/Dashboard.cshtml
            return View("~/Views/Admin/AdminHome/Dashboard.cshtml");
        }
    }
}
