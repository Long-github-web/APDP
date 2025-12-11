using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;

namespace SIMS.Controllers
{
    [Authorize]
    public class ActivityLogController : Controller
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Chỉ admin mới được xem lịch sử hoạt động
            var logs = await _activityLogService.GetAllActivityLogsAsync();
            return View(logs);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var logs = await _activityLogService.GetAllActivityLogsAsync();
            return View("Index", logs);
        }
    }
}


