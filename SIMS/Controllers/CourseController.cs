using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IUserRepository _userRepository;
        private readonly IStudentService _studentService;
        private readonly IActivityLogService _activityLogService;

        public CourseController(ICourseService courseService, IUserRepository userRepository, IStudentService studentService, IActivityLogService activityLogService)
        {
            _courseService = courseService;
            _userRepository = userRepository;
            _studentService = studentService;
            _activityLogService = activityLogService;
        }

        [Authorize(Roles = "Admin, Student, Falculty")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            var courseViewModels = courses.Select(c => new CourseViewModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                Description = c.Description,
                Credits = c.Credits ?? 0,
                Department = c.Department,
                Status = c.Status
            }).ToList();

            return View(courseViewModels);
        }

        [Authorize(Roles = "Admin, Student, Falculty")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var enrolledStudents = await _courseService.GetStudentsByCourseIdAsync(id);

            var model = new CourseViewModel
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Credits = course.Credits ?? 0,
                Department = course.Department,
                Status = course.Status
            };

            ViewBag.EnrolledStudents = enrolledStudents;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View(new CourseViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = new Course
                    {
                        CourseCode = model.CourseCode,
                        CourseName = model.CourseName,
                        Description = model.Description,
                        Credits = model.Credits,
                        Department = model.Department,
                        Status = model.Status
                    };

                    await _courseService.CreateCourseAsync(course);
                    
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Create",
                        "Course",
                        course.Id,
                        $"Created course: {course.CourseName} ({course.CourseCode})",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Course created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var model = new CourseViewModel
            {
                Id = course.Id,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Description = course.Description,
                Credits = course.Credits ?? 0,
                Department = course.Department,
                Status = course.Status
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var course = new Course
                    {
                        Id = model.Id,
                        CourseCode = model.CourseCode,
                        CourseName = model.CourseName,
                        Description = model.Description,
                        Credits = model.Credits,
                        Department = model.Department,
                        Status = model.Status
                    };

                    await _courseService.UpdateCourseAsync(course);
                    
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Update",
                        "Course",
                        course.Id,
                        $"Updated course: {course.CourseName} ({course.CourseCode})",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                
                var result = await _courseService.DeleteCourseAsync(id);
                if (result)
                {
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Delete",
                        "Course",
                        id,
                        course != null ? $"Deleted course: {course.CourseName} ({course.CourseCode})" : $"Deleted course ID: {id}",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Course deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Course not found!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting course: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignStudents(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var enrolledStudents = await _courseService.GetStudentsByCourseIdAsync(id);
            var allStudentEntities = await _studentService.GetAllStudentsAsync();
            var enrolledStudentIds = enrolledStudents.Select(s => s.Id).ToHashSet();

            var model = new AssignStudentViewModel
            {
                CourseId = course.Id,
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                AvailableStudents = allStudentEntities
                    .Where(s => !enrolledStudentIds.Contains(s.Id))
                    .Select(s => new StudentOption
                    {
                        Id = s.Id,
                        Username = s.User?.Username ?? "",
                        Email = s.User?.Email ?? s.Email ?? "",
                        DisplayName = $"{s.User?.Username ?? s.FullName} ({s.User?.Email ?? s.Email ?? ""})"
                    })
                    .ToList(),
                EnrolledStudents = enrolledStudents
                    .Select(s => new EnrolledStudentViewModel
                    {
                        StudentId = s.Id,
                        Username = s.User?.Username ?? "",
                        Email = s.User?.Email ?? s.Email ?? "",
                        EnrollmentDate = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.EnrollmentDate,
                        Grade = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.Grade,
                        Status = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.Status ?? "Enrolled"
                    })
                    .ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudents(AssignStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _studentService.GetStudentByIdAsync(model.StudentId);
                    if (student == null)
                    {
                        ModelState.AddModelError("StudentId", $"Student with ID {model.StudentId} does not exist in Students table. Please ensure the student record exists before assigning to a course.");
                        var courseForReload = await _courseService.GetCourseByIdAsync(model.CourseId);
                        if (courseForReload != null)
                        {
                            var enrolledStudents = await _courseService.GetStudentsByCourseIdAsync(model.CourseId);
                            var allStudentEntities = await _studentService.GetAllStudentsAsync();
                            var enrolledStudentIds = enrolledStudents.Select(s => s.Id).ToHashSet();

                            model.CourseName = courseForReload.CourseName;
                            model.CourseCode = courseForReload.CourseCode;
                            model.AvailableStudents = allStudentEntities
                                .Where(s => !enrolledStudentIds.Contains(s.Id))
                                .Select(s => new StudentOption
                                {
                                    Id = s.Id,
                                    Username = s.User?.Username ?? "",
                                    Email = s.User?.Email ?? s.Email ?? "",
                                    DisplayName = $"{s.User?.Username ?? s.FullName} ({s.User?.Email ?? s.Email ?? ""})"
                                })
                                .ToList();
                            model.EnrolledStudents = enrolledStudents
                                .Select(s => new EnrolledStudentViewModel
                                {
                                    StudentId = s.Id,
                                    Username = s.User?.Username ?? "",
                                    Email = s.User?.Email ?? s.Email ?? "",
                                    EnrollmentDate = courseForReload.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.EnrollmentDate,
                                    Grade = courseForReload.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.Grade,
                                    Status = courseForReload.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.Status ?? "Enrolled"
                                })
                                .ToList();
                        }
                        return View(model);
                    }

                    await _courseService.AssignStudentToCourseAsync(model.StudentId, model.CourseId);
                    
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    var courseForLog = await _courseService.GetCourseByIdAsync(model.CourseId);
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Assign",
                        "StudentCourse",
                        model.StudentId,
                        $"Assigned student ID {model.StudentId} to course: {courseForLog?.CourseName ?? "N/A"}",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Student assigned to course successfully!";
                    return RedirectToAction(nameof(AssignStudents), new { id = model.CourseId });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            var course = await _courseService.GetCourseByIdAsync(model.CourseId);
            if (course != null)
            {
                var enrolledStudents = await _courseService.GetStudentsByCourseIdAsync(model.CourseId);
                var allStudentEntities = await _studentService.GetAllStudentsAsync();
                var enrolledStudentIds = enrolledStudents.Select(s => s.Id).ToHashSet();

                model.CourseName = course.CourseName;
                model.CourseCode = course.CourseCode;
                model.AvailableStudents = allStudentEntities
                    .Where(s => !enrolledStudentIds.Contains(s.Id))
                    .Select(s => new StudentOption
                    {
                        Id = s.Id,
                        Username = s.User?.Username ?? "",
                        Email = s.User?.Email ?? s.Email ?? "",
                        DisplayName = $"{s.User?.Username ?? s.FullName} ({s.User?.Email ?? s.Email ?? ""})"
                    })
                    .ToList();
                model.EnrolledStudents = enrolledStudents
                    .Select(s => new EnrolledStudentViewModel
                    {
                        StudentId = s.Id,
                        Username = s.User?.Username ?? "",
                        Email = s.User?.Email ?? s.Email ?? "",
                        EnrollmentDate = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.EnrollmentDate,
                        Grade = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.Grade,
                        Status = course.StudentCourses.FirstOrDefault(sc => sc.StudentId == s.Id)?.Status ?? "Enrolled"
                    })
                    .ToList();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int courseId, int studentId, string grade)
        {
            try
            {
                var result = await _courseService.UpdateStudentGradeAsync(studentId, courseId, grade);
                if (result)
                {
                    TempData["SuccessMessage"] = "Grade updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update grade.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating grade: {ex.Message}";
            }

            return RedirectToAction(nameof(AssignStudents), new { id = courseId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int courseId, int studentId)
        {
            try
            {
                var result = await _courseService.RemoveStudentFromCourseAsync(studentId, courseId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Student removed from course successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to remove student from course!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(AssignStudents), new { id = courseId });
        }
    }
}