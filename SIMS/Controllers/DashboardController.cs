using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;

namespace SIMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IUserRepository _userRepository;

        public DashboardController(
            IStudentService studentService,
            ICourseService courseService,
            IUserRepository userRepository)
        {
            _studentService = studentService;
            _courseService = courseService;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Admin, Student, Falculty")]
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var courses = await _courseService.GetAllCoursesAsync();
            var allUsers = await _userRepository.GetUsersByRoleAsync("Student");
            
            // Count enrollments
            int totalEnrollments = 0;
            foreach (var course in courses)
            {
                var enrolledStudents = await _courseService.GetStudentsByCourseIdAsync(course.Id);
                totalEnrollments += enrolledStudents.Count();
            }

            ViewBag.TotalStudents = students.Count();
            ViewBag.TotalCourses = courses.Count();
            ViewBag.TotalEnrollments = totalEnrollments;
            ViewBag.ActiveStudents = students.Count(s => s.Status == "Active");
            ViewBag.ActiveCourses = courses.Count(c => c.Status == "Active");

            return View();
        }
    }
}
