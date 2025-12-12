using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SimDbContext _context;

        public StudentRepository(SimDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.User)
                .Where(s => s.StudentCode != null && s.FullName != null) // Filter out records with null required fields
                .OrderBy(s => s.StudentCode ?? "")
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetStudentByStudentIdAsync(string studentId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentId == studentId || s.StudentCode == studentId);
        }
        
        public async Task<Student?> GetStudentByStudentCodeAsync(string studentCode)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
        }

        public async Task<Student?> GetStudentByUserIdAsync(int userId)
        {
            return await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;
            if (student.EnrollmentDate == null)
            {
                student.EnrollmentDate = DateTime.Now;
            }
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            student.UpdatedAt = DateTime.Now;
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            // Check if student exists first
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            // First, delete all StudentCourses related to this student using raw SQL
            // This avoids OUTPUT clause conflict with database triggers
            var deleteStudentCoursesSql = "DELETE FROM StudentCourses WHERE StudentId = @StudentId";
            var studentIdParam = new Microsoft.Data.SqlClient.SqlParameter("@StudentId", id);
            await _context.Database.ExecuteSqlRawAsync(deleteStudentCoursesSql, studentIdParam);

            // Then delete the student itself using raw SQL
            // Use raw SQL to avoid OUTPUT clause conflict with database triggers
            // SQL Server doesn't allow OUTPUT clause when triggers exist on the table
            var deleteStudentSql = "DELETE FROM Students WHERE Id = @Id";
            var idParam = new Microsoft.Data.SqlClient.SqlParameter("@Id", id);
            
            // Execute SQL delete without OUTPUT clause
            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(deleteStudentSql, idParam);
            
            return rowsAffected > 0;
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> StudentIdExistsAsync(string studentId)
        {
            // Check both StudentCode and StudentId
            return await _context.Students.AnyAsync(s => s.StudentCode == studentId || s.StudentId == studentId);
        }
        
        public async Task<bool> StudentCodeExistsAsync(string studentCode)
        {
            return await _context.Students.AnyAsync(s => s.StudentCode == studentCode);
        }
    }
}

