using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Services;

namespace SIMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        private readonly IActivityLogService _activityLogService;

        public LoginController(UserService service, IActivityLogService activityLogService)
        {
            _userService = service;
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // khong co loi cua nguoi dung
                string username = model.Username;
                string password = model.Password;
                var userInfo = await _userService.LoginUser(username, password);
                if (userInfo != null)
                {
                    // luu thong tin nguoi dung dang nhp vao cookies
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userInfo.Username),
                        new Claim(ClaimTypes.Role, userInfo.Role)
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    // Cấu hình cookie authentication với Remember Me
                    var authProperties = new AuthenticationProperties();
                    if (model.RememberMe)
                    {
                        // Nếu chọn "Ghi nhớ mật khẩu", cookie sẽ tồn tại trong 30 ngày
                        authProperties.IsPersistent = true;
                        authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30);
                    }
                    else
                    {
                        // Nếu không chọn, cookie sẽ hết hạn khi đóng trình duyệt
                        authProperties.IsPersistent = false;
                    }
                    
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(identity),
                        authProperties);

                    // Log activity
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        userInfo.Id,
                        userInfo.Username,
                        "Login",
                        "Authentication",
                        userInfo.Id,
                        "User logged in successfully",
                        ipAddress
                    );

                    // chuyen vao trang dashboard
                    return RedirectToAction("Index", "Dashboard");
                }
                ViewData["InvalidAccount"] = "Your Account Invalid";
            }
            return View(model); // co loi tu nguoi dung, quay tra lai giao dien login
        }

        [Authorize(Roles = "Admin, Student, Falculty")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Log activity before logout
            var username = User.Identity?.Name;
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            if (!string.IsNullOrEmpty(username))
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                await _activityLogService.LogActivityAsync(
                    userId,
                    username,
                    "Logout",
                    "Authentication",
                    userId,
                    "User logged out",
                    ipAddress
                );
            }

            // xoa cookies da tao ra o login
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Index", "Login");
        }
    }
}
