using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;
using SIMS.Models;

namespace SIMS.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IActivityLogService _activityLogService;

        public SettingsController(IUserRepository userRepository, IActivityLogService activityLogService)
        {
            _userRepository = userRepository;
            _activityLogService = activityLogService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Status = user.Status
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userRepository.GetUserById(model.Id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Verify this is the current user
                    var currentUsername = User.Identity?.Name;
                    if (user.Username != currentUsername && !User.IsInRole("Admin"))
                    {
                        TempData["ErrorMessage"] = "You can only update your own profile.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Check if email is being changed and if new email already exists
                    if (user.Email != model.Email)
                    {
                        var existingEmail = await _userRepository.GetUserByEmail(model.Email);
                        if (existingEmail != null && existingEmail.Id != user.Id)
                        {
                            ModelState.AddModelError("Email", "Email already exists.");
                            return View("Index", model);
                        }
                    }

                    user.Email = model.Email;
                    user.Phone = model.Phone;
                    user.UpdatedAt = DateTime.Now;

                    await _userRepository.UpdateUserAsync(user);

                    // Log activity
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        user.Id,
                        user.Username,
                        "Update",
                        "Profile",
                        user.Id,
                        "Updated profile information",
                        ipAddress
                    );

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View("Index", model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name;
                    if (string.IsNullOrEmpty(username))
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    var user = await _userRepository.GetUserByUsername(username);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Verify current password
                    if (user.Password != model.CurrentPassword)
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                        return View(model);
                    }

                    // Update password
                    user.Password = model.NewPassword;
                    user.UpdatedAt = DateTime.Now;

                    await _userRepository.UpdateUserAsync(user);

                    // Log activity
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        user.Id,
                        user.Username,
                        "Update",
                        "Password",
                        user.Id,
                        "Changed password",
                        ipAddress
                    );

                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(model);
        }
    }
}


