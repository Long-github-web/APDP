using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly SimDbContext _context;

        public SearchController(
            ICourseService courseService,
            IStudentService studentService,
            SimDbContext context)
        {
            _courseService = courseService;
            _studentService = studentService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { courses = new List<object>(), students = new List<object>() });
            }

            var searchTerm = query.Trim().ToLower();

            var courses = await _context.Courses
                .Where(c => c.Status == "Active" && 
                    (c.CourseCode.ToLower().Contains(searchTerm) || 
                     c.CourseName.ToLower().Contains(searchTerm)))
                .Take(10)
                .Select(c => new
                {
                    id = c.Id,
                    code = c.CourseCode,
                    name = c.CourseName,
                    type = "course"
                })
                .ToListAsync();

            var studentIdsByName = await _context.Students
                .Where(s => s.Status == "Active" &&
                    ((s.StudentCode != null && s.StudentCode.ToLower().Contains(searchTerm)) ||
                     (s.StudentId != null && s.StudentId.ToLower().Contains(searchTerm)) ||
                     s.FullName.ToLower().Contains(searchTerm)))
                .Select(s => s.Id)
                .ToListAsync();

            var studentIdsByCourse = await _context.StudentCourses
                .Include(sc => sc.Course)
                .Include(sc => sc.Student)
                .Where(sc => sc.Student.Status == "Active" && 
                    sc.Course.Status == "Active" &&
                    (sc.Course.CourseCode.ToLower().Contains(searchTerm) ||
                     sc.Course.CourseName.ToLower().Contains(searchTerm)))
                .Select(sc => sc.Student.Id)
                .Distinct()
                .ToListAsync();

            var allStudentIds = studentIdsByName
                .Concat(studentIdsByCourse)
                .Distinct()
                .Take(10)
                .ToList();

            var allStudents = await _context.Students
                .Where(s => allStudentIds.Contains(s.Id))
                .Select(s => new
                {
                    id = s.Id,
                    studentCode = s.StudentCode,
                    studentId = s.StudentId ?? "",
                    fullName = s.FullName,
                    type = "student"
                })
                .ToListAsync();

            return Json(new
            {
                courses = courses,
                students = allStudents
            });
        }

        [HttpGet]
        public IActionResult Results(string query)
        {
            ViewData["SearchQuery"] = query;
            return View();
        }
    }
}