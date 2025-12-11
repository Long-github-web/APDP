using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IActivityLogService _activityLogService;

        public UserController(IUserRepository userRepository, IActivityLogService activityLogService)
        {
            _userRepository = userRepository;
            _activityLogService = activityLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Users());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Users user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username already exists
                    var existingUser = await _userRepository.GetUserByUsername(user.Username);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Username", "Username already exists.");
                        return View(user);
                    }

                    // Check if email already exists
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var existingEmail = await _userRepository.GetUserByEmail(user.Email);
                        if (existingEmail != null)
                        {
                            ModelState.AddModelError("Email", "Email already exists.");
                            return View(user);
                        }
                    }

                    await _userRepository.CreateUserAsync(user);
                    
                    // Log activity
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Create",
                        "User",
                        user.Id,
                        $"Created user: {user.Username} ({user.Role})",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "User created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Users user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username already exists (excluding current user)
                    var existingUser = await _userRepository.GetUserByUsername(user.Username);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        ModelState.AddModelError("Username", "Username already exists.");
                        return View(user);
                    }

                    // Check if email already exists (excluding current user)
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var existingEmail = await _userRepository.GetUserByEmail(user.Email);
                        if (existingEmail != null && existingEmail.Id != user.Id)
                        {
                            ModelState.AddModelError("Email", "Email already exists.");
                            return View(user);
                        }
                    }

                    await _userRepository.UpdateUserAsync(user);
                    
                    // Log activity
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Update",
                        "User",
                        user.Id,
                        $"Updated user: {user.Username} ({user.Role})",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get user info before deletion for logging
                var user = await _userRepository.GetUserById(id);
                
                var result = await _userRepository.DeleteUserAsync(id);
                if (result)
                {
                    // Log activity
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Delete",
                        "User",
                        id,
                        user != null ? $"Deleted user: {user.Username} ({user.Role})" : $"Deleted user ID: {id}",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

