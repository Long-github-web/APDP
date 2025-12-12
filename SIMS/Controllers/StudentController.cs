using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IActivityLogService _activityLogService;
        private readonly IUserRepository _userRepository;

        public StudentController(IStudentService studentService, IActivityLogService activityLogService, IUserRepository userRepository)
        {
            _studentService = studentService;
            _activityLogService = activityLogService;
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Admin, Student, Falculty")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var studentViewModels = students.Select(s => new StudentViewModel
            {
                Id = s.Id,
                StudentId = s.StudentCode ?? s.StudentId ?? "", // Use StudentCode first (required field)
                FullName = s.FullName ?? "",
                DateOfBirth = s.DateOfBirth,
                Gender = s.Gender,
                Address = s.Address,
                Phone = s.Phone,
                Email = s.Email,
                AcademicProgram = s.AcademicProgram,
                Year = s.Year,
                GPA = s.GPA,
                Status = s.Status,
                Username = s.User?.Username,
                EnrollmentDate = s.EnrollmentDate
            }).ToList();

            return View(studentViewModels);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View(new StudentCreateViewModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = new Student
                    {
                        StudentCode = model.StudentId, // Map StudentId from form to StudentCode in database
                        StudentId = model.StudentId, // Also keep StudentId for backward compatibility
                        FullName = model.FullName,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Address = model.Address,
                        Phone = model.Phone,
                        Email = model.Email,
                        AcademicProgram = model.AcademicProgram,
                        Year = model.Year,
                        GPA = model.GPA,
                        Status = model.Status
                    };

                    await _studentService.CreateStudentAsync(student, model.Username, model.Password);
                    
                    // Log activity
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null, // Will be set after student is created
                        username,
                        "Create",
                        "Student",
                        student.Id,
                        $"Created student: {student.FullName} ({student.StudentCode})",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Student created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    // Handle database constraint violations
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        if (sqlEx.Message.Contains("UNIQUE KEY") && sqlEx.Message.Contains("Email"))
                        {
                            ModelState.AddModelError("Email", "This email is already registered. Please use a different email.");
                        }
                        else if (sqlEx.Message.Contains("UNIQUE KEY") && sqlEx.Message.Contains("Username"))
                        {
                            ModelState.AddModelError("Username", "This username is already taken. Please choose a different username.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "An error occurred while saving. Please check your input and try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentViewModel
            {
                Id = student.Id,
                StudentId = student.StudentCode ?? student.StudentId ?? "", // Use StudentCode first
                FullName = student.FullName ?? "",
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Address = student.Address,
                Phone = student.Phone,
                Email = student.Email,
                AcademicProgram = student.AcademicProgram,
                Year = student.Year,
                GPA = student.GPA,
                Status = student.Status,
                Username = student.User?.Username,
                EnrollmentDate = student.EnrollmentDate
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _studentService.GetStudentByIdAsync(model.Id);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Update StudentCode (required field) and optionally StudentId
                    if (!string.IsNullOrWhiteSpace(model.StudentId))
                    {
                        student.StudentCode = model.StudentId; // Update StudentCode
                        student.StudentId = model.StudentId; // Also update StudentId for consistency
                    }
                    student.FullName = model.FullName ?? "";
                    student.DateOfBirth = model.DateOfBirth;
                    student.Gender = model.Gender;
                    student.Address = model.Address;
                    student.Phone = model.Phone;
                    student.Email = model.Email;
                    student.AcademicProgram = model.AcademicProgram;
                    student.Year = model.Year;
                    student.GPA = model.GPA;
                    student.Status = model.Status;

                    await _studentService.UpdateStudentAsync(student);
                    
                    // Log activity
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Update",
                        "Student",
                        student.Id,
                        $"Updated student: {student.FullName} ({student.StudentCode})",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Student updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin, Student, Falculty")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var courses = await _studentService.GetCoursesByStudentIdAsync(id);

            var model = new StudentViewModel
            {
                Id = student.Id,
                StudentId = student.StudentCode ?? student.StudentId ?? "", // Use StudentCode first
                FullName = student.FullName ?? "",
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Address = student.Address,
                Phone = student.Phone,
                Email = student.Email,
                AcademicProgram = student.AcademicProgram,
                Year = student.Year,
                GPA = student.GPA,
                Status = student.Status,
                Username = student.User?.Username,
                EnrollmentDate = student.EnrollmentDate
            };

            ViewBag.Courses = courses;
            return View(model);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Login");
            }

            // Get current user's student record
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetStudentByUserIdAsync(user.Id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentProfileViewModel
            {
                Id = student.Id,
                StudentId = student.StudentCode ?? student.StudentId ?? "",
                FullName = student.FullName ?? "",
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Address = student.Address,
                Phone = student.Phone
            };

            return View(model);
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(StudentProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verify that the student is editing their own profile
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

                    var student = await _studentService.GetStudentByUserIdAsync(user.Id);
                    if (student == null || student.Id != model.Id)
                    {
                        ModelState.AddModelError("", "You can only edit your own profile.");
                        return View(model);
                    }

                    // Update only basic information
                    await _studentService.UpdateStudentBasicInfoAsync(
                        model.Id,
                        model.DateOfBirth,
                        model.Gender,
                        model.Address,
                        model.Phone
                    );

                    // Log activity
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Update",
                        "StudentProfile",
                        student.Id,
                        $"Updated personal information",
                        ipAddress
                    );

                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(EditProfile));
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

            // Reload read-only fields
            var usernameReload = User.Identity?.Name;
            if (!string.IsNullOrEmpty(usernameReload))
            {
                var userReload = await _userRepository.GetUserByUsername(usernameReload);
                if (userReload != null)
                {
                    var studentReload = await _studentService.GetStudentByUserIdAsync(userReload.Id);
                    if (studentReload != null)
                    {
                        model.StudentId = studentReload.StudentCode ?? studentReload.StudentId ?? "";
                        model.FullName = studentReload.FullName ?? "";
                        model.Email = studentReload.Email;
                    }
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
                // Get student info before deletion for logging
                var student = await _studentService.GetStudentByIdAsync(id);
                
                var result = await _studentService.DeleteStudentAsync(id);
                if (result)
                {
                    // Log activity
                    var username = User.Identity?.Name ?? "System";
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await _activityLogService.LogActivityAsync(
                        null,
                        username,
                        "Delete",
                        "Student",
                        id,
                        student != null ? $"Deleted student: {student.FullName} ({student.StudentCode})" : $"Deleted student ID: {id}",
                        ipAddress
                    );
                    
                    TempData["SuccessMessage"] = "Student deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Student not found!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
