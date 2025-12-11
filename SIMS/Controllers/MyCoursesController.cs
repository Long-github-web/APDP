using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class MyCoursesController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IUserRepository _userRepository;

        public MyCoursesController(IStudentService studentService, IUserRepository userRepository)
        {
            _studentService = studentService;
            _userRepository = userRepository;
        }

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

            // Get student by user ID
            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null)
            {
                ViewBag.Message = "Student profile not found.";
                return View(Enumerable.Empty<SIMS.SimsDbContext.Entities.Course>());
            }

            var courses = await _studentService.GetCoursesByStudentIdAsync(student.Id);
            return View(courses);
        }
    }
}










