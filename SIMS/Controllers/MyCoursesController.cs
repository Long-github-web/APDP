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
        private readonly IStudentCourseRepository _studentCourseRepository;

        public MyCoursesController(
            IStudentService studentService, 
            IUserRepository userRepository,
            IStudentCourseRepository studentCourseRepository)
        {
            _studentService = studentService;
            _userRepository = userRepository;
            _studentCourseRepository = studentCourseRepository;
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
                ViewBag.StudentCourses = Enumerable.Empty<SIMS.SimsDbContext.Entities.StudentCourse>();
                ViewBag.GPA = null;
                return View();
            }

            // Get StudentCourses with Grade information
            var studentCourses = await _studentCourseRepository.GetStudentCoursesByStudentIdAsync(student.Id);
            
            // Calculate GPA from grades
            var gpa = CalculateGPA(studentCourses);
            
            ViewBag.StudentCourses = studentCourses;
            ViewBag.GPA = gpa;
            ViewBag.Student = student;
            
            return View();
        }

        private decimal? CalculateGPA(IEnumerable<SIMS.SimsDbContext.Entities.StudentCourse> studentCourses)
        {
            var gradedCourses = studentCourses
                .Where(sc => !string.IsNullOrEmpty(sc.Grade) && sc.Course?.Credits.HasValue == true)
                .ToList();

            if (!gradedCourses.Any())
                return null;

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var sc in gradedCourses)
            {
                var gradePoints = ConvertGradeToPoints(sc.Grade);
                if (gradePoints.HasValue && sc.Course?.Credits.HasValue == true)
                {
                    totalPoints += gradePoints.Value * sc.Course.Credits.Value;
                    totalCredits += sc.Course.Credits.Value;
                }
            }

            if (totalCredits == 0)
                return null;

            return totalPoints / totalCredits;
        }

        private decimal? ConvertGradeToPoints(string? grade)
        {
            return grade?.ToUpper() switch
            {
                "A+" => 4.0m,
                "A" => 4.0m,
                "A-" => 3.7m,
                "B+" => 3.3m,
                "B" => 3.0m,
                "B-" => 2.7m,
                "C+" => 2.3m,
                "C" => 2.0m,
                "C-" => 1.7m,
                "D+" => 1.3m,
                "D" => 1.0m,
                "F" => 0.0m,
                _ => null
            };
        }
    }
}













