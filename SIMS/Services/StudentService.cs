using Microsoft.EntityFrameworkCore.Storage;
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
            if (string.IsNullOrWhiteSpace(student.StudentCode))
            {
                throw new InvalidOperationException("Student Code is required.");
            }
            
            if (await _studentRepository.StudentCodeExistsAsync(student.StudentCode))
            {
                throw new InvalidOperationException($"Student with Code {student.StudentCode} already exists.");
            }
            
            if (!string.IsNullOrWhiteSpace(student.StudentId))
            {
                if (await _studentRepository.StudentIdExistsAsync(student.StudentId))
                {
                    throw new InvalidOperationException($"Student with ID {student.StudentId} already exists.");
                }
            }
            
            if (string.IsNullOrWhiteSpace(student.FullName))
            {
                throw new InvalidOperationException("Full Name is required.");
            }

            var existingUser = await _userRepository.GetUserByUsername(username);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Username {username} already exists.");
            }

            var emailToUse = string.IsNullOrWhiteSpace(student.Email) ? null : student.Email.Trim();
            if (!string.IsNullOrEmpty(emailToUse))
            {
                var existingUserByEmail = await _userRepository.GetUserByEmail(emailToUse);
                if (existingUserByEmail != null)
                {
                    throw new InvalidOperationException($"Email {emailToUse} is already registered. Please use a different email.");
                }
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(
                operation: async (context, state, cancellationToken) =>
                {
                    using var transaction = await ((SimDbContext)context).Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        var user = new Users
                        {
                            Username = username,
                            Password = password,
                            Email = emailToUse ?? "",
                            Phone = student.Phone,
                            Role = "Student",
                            Status = "Active",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        ((SimDbContext)context).User.Add(user);
                        await ((SimDbContext)context).SaveChangesAsync(cancellationToken);
                        
                        student.UserId = user.Id;
                        student.CreatedAt = DateTime.Now;
                        student.UpdatedAt = DateTime.Now;
                        if (student.EnrollmentDate == null)
                        {
                            student.EnrollmentDate = DateTime.Now;
                        }

                        ((SimDbContext)context).Students.Add(student);
                        await ((SimDbContext)context).SaveChangesAsync(cancellationToken);
                        
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

            if (existingStudent.StudentCode != student.StudentCode)
            {
                if (await _studentRepository.StudentCodeExistsAsync(student.StudentCode))
                {
                    throw new InvalidOperationException($"Student with Code {student.StudentCode} already exists.");
                }
            }
            
            if (!string.IsNullOrWhiteSpace(student.StudentId) && existingStudent.StudentId != student.StudentId)
            {
                if (await _studentRepository.StudentIdExistsAsync(student.StudentId))
                {
                    throw new InvalidOperationException($"Student with ID {student.StudentId} already exists.");
                }
            }

            return await _studentRepository.UpdateStudentAsync(student);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student == null)
            {
                return false;
            }

            return await _studentRepository.DeleteStudentAsync(id);
        }

        public async Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _courseRepository.GetCoursesByStudentIdAsync(studentId);
        }
    }
}