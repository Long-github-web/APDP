using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly SimDbContext _context;

        public StudentService(
            IStudentRepository studentRepository,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            SimDbContext context)
        {
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllStudentsAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _studentRepository.GetStudentByIdAsync(id);
        }

        public async Task<Student?> GetStudentByStudentIdAsync(string studentId)
        {
            return await _studentRepository.GetStudentByStudentIdAsync(studentId);
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            return await _studentRepository.GetStudentByUserIdAsync(userId);
        }

        public async Task<Student> CreateStudentAsync(Student student, string username, string password)
        {
            // Check if StudentCode already exists (StudentCode is required and NOT NULL)
            if (string.IsNullOrWhiteSpace(student.StudentCode))
            {
                throw new InvalidOperationException("Student Code is required.");
            }
            
            if (await _studentRepository.StudentCodeExistsAsync(student.StudentCode))
            {
                throw new InvalidOperationException($"Student with Code {student.StudentCode} already exists.");
            }
            
            // Check if StudentId already exists (if provided)
            if (!string.IsNullOrWhiteSpace(student.StudentId))
            {
                if (await _studentRepository.StudentIdExistsAsync(student.StudentId))
                {
                    throw new InvalidOperationException($"Student with ID {student.StudentId} already exists.");
                }
            }
            
            // Ensure FullName is set (required in database)
            if (string.IsNullOrWhiteSpace(student.FullName))
            {
                throw new InvalidOperationException("Full Name is required.");
            }

            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsername(username);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Username {username} already exists.");
            }

            // Check if email already exists (if provided)
            var emailToUse = string.IsNullOrWhiteSpace(student.Email) ? null : student.Email.Trim();
            if (!string.IsNullOrEmpty(emailToUse))
            {
                var existingUserByEmail = await _userRepository.GetUserByEmail(emailToUse);
                if (existingUserByEmail != null)
                {
                    throw new InvalidOperationException($"Email {emailToUse} is already registered. Please use a different email.");
                }
            }

            // Use execution strategy to handle transaction with retry policy
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(
                operation: async (context, state, cancellationToken) =>
                {
                    var dbContext = (SimDbContext)context;
                    using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        // Create User account first
                        var user = new Users
                        {
                            Username = username,
                            Password = password, // Should be hashed in production
                            Email = emailToUse ?? "",
                            Phone = student.Phone,
                            Role = "Student",
                            Status = "Active",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        dbContext.User.Add(user);
                        await dbContext.SaveChangesAsync(cancellationToken); // Save user first to get Id
                        
                        // Set UserId for student
                        student.UserId = user.Id;
                        student.CreatedAt = DateTime.Now;
                        student.UpdatedAt = DateTime.Now;
                        if (student.EnrollmentDate == null)
                        {
                            student.EnrollmentDate = DateTime.Now;
                        }

                        dbContext.Students.Add(student);
                        await dbContext.SaveChangesAsync(cancellationToken); // Save student
                        
                        await transaction.CommitAsync(cancellationToken);
                        return student;
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                },
                state: _context,
                verifySucceeded: null);
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            var existingStudent = await _studentRepository.GetStudentByIdAsync(student.Id);
            if (existingStudent == null)
            {
                throw new InvalidOperationException($"Student with id {student.Id} not found.");
            }

            // Check if StudentCode is being changed and if new Code already exists
            if (existingStudent.StudentCode != student.StudentCode)
            {
                if (await _studentRepository.StudentCodeExistsAsync(student.StudentCode))
                {
                    throw new InvalidOperationException($"Student with Code {student.StudentCode} already exists.");
                }
            }
            
            // Check if StudentId is being changed and if new ID already exists (if provided)
            if (!string.IsNullOrWhiteSpace(student.StudentId) && existingStudent.StudentId != student.StudentId)
            {
                if (await _studentRepository.StudentIdExistsAsync(student.StudentId))
                {
                    throw new InvalidOperationException($"Student with ID {student.StudentId} already exists.");
                }
            }

            return await _studentRepository.UpdateStudentAsync(student);
        }

        public async Task<Student> UpdateStudentBasicInfoAsync(int studentId, DateTime? dateOfBirth, string? gender, string? address, string? phone)
        {
            var existingStudent = await _studentRepository.GetStudentByIdAsync(studentId);
            if (existingStudent == null)
            {
                throw new InvalidOperationException($"Student with id {studentId} not found.");
            }

            // Only update basic information fields
            existingStudent.DateOfBirth = dateOfBirth;
            existingStudent.Gender = gender;
            existingStudent.Address = address;
            existingStudent.Phone = phone;
            existingStudent.UpdatedAt = DateTime.Now;

            return await _studentRepository.UpdateStudentAsync(existingStudent);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return false;
            }

            // Get UserId before deletion
            var userId = student.UserId;

            // Delete using repository (which will handle StudentCourses cascade via raw SQL if needed)
            var studentDeleted = await _studentRepository.DeleteStudentAsync(id);
            
            if (studentDeleted && userId > 0)
            {
                // After deleting student, delete the associated User account
                // Use raw SQL to avoid OUTPUT clause conflict with database triggers
                var deleteUserSql = "DELETE FROM Users WHERE Id = @UserId";
                var userIdParam = new Microsoft.Data.SqlClient.SqlParameter("@UserId", userId);
                await _context.Database.ExecuteSqlRawAsync(deleteUserSql, userIdParam);
            }

            return studentDeleted;
        }

        public async Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _courseRepository.GetCoursesByStudentIdAsync(studentId);
        }
    }
}
