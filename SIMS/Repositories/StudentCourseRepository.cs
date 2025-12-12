using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Repositories
{
    public class StudentCourseRepository : IStudentCourseRepository
    {
        private readonly SimDbContext _context;

        public StudentCourseRepository(SimDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentCourse>> GetStudentCoursesByCourseIdAsync(int courseId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.User) // Include User for Student
                .Include(sc => sc.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentCourse>> GetStudentCoursesByStudentIdAsync(int studentId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Course)
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.User) // Include User for Student
                .ToListAsync();
        }

        public async Task<StudentCourse?> GetStudentCourseAsync(int studentId, int courseId)
        {
            return await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);
        }

        public async Task<StudentCourse> AssignStudentToCourseAsync(StudentCourse studentCourse)
        {
            studentCourse.EnrollmentDate = DateTime.Now;
            studentCourse.Status = studentCourse.Status ?? "Enrolled";
            
            // Use raw SQL to avoid OUTPUT clause conflict with database triggers
            // SQL Server doesn't allow OUTPUT clause when triggers exist on the table
            var sql = @"
                INSERT INTO StudentCourses (StudentId, CourseId, EnrollmentDate, Grade, Status) 
                VALUES (@StudentId, @CourseId, @EnrollmentDate, @Grade, @Status)";
            
            // Use parameterized query to avoid SQL injection
            var studentIdParam = new Microsoft.Data.SqlClient.SqlParameter("@StudentId", studentCourse.StudentId);
            var courseIdParam = new Microsoft.Data.SqlClient.SqlParameter("@CourseId", studentCourse.CourseId);
            var enrollmentDateParam = new Microsoft.Data.SqlClient.SqlParameter("@EnrollmentDate", studentCourse.EnrollmentDate);
            var gradeParam = new Microsoft.Data.SqlClient.SqlParameter("@Grade", (object?)studentCourse.Grade ?? DBNull.Value);
            var statusParam = new Microsoft.Data.SqlClient.SqlParameter("@Status", studentCourse.Status ?? "Enrolled");
            
            // Execute SQL insert without OUTPUT clause
            await _context.Database.ExecuteSqlRawAsync(sql, 
                studentIdParam, courseIdParam, enrollmentDateParam, gradeParam, statusParam);
            
            // Query for the inserted record to get the generated ID and all properties
            var inserted = await _context.StudentCourses
                .Include(sc => sc.Student)
                    .ThenInclude(s => s.User)
                .Include(sc => sc.Course)
                .OrderByDescending(sc => sc.Id)
                .FirstOrDefaultAsync(sc => sc.StudentId == studentCourse.StudentId && 
                                           sc.CourseId == studentCourse.CourseId);
            
            if (inserted != null)
            {
                return inserted;
            }
            
            // Fallback: if query fails, return the original object (ID will be 0)
            return studentCourse;
        }

        public async Task<bool> RemoveStudentFromCourseAsync(int studentId, int courseId)
        {
            var studentCourse = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (studentCourse == null) return false;

            _context.StudentCourses.Remove(studentCourse);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStudentCourseAsync(StudentCourse studentCourse)
        {
            // Use raw SQL to avoid OUTPUT clause conflict with database triggers
            // SQL Server doesn't allow OUTPUT clause when triggers exist on the table
            var sql = @"
                UPDATE StudentCourses 
                SET Grade = @Grade, 
                    Status = @Status
                WHERE StudentId = @StudentId AND CourseId = @CourseId";
            
            // Use parameterized query to avoid SQL injection
            var studentIdParam = new Microsoft.Data.SqlClient.SqlParameter("@StudentId", studentCourse.StudentId);
            var courseIdParam = new Microsoft.Data.SqlClient.SqlParameter("@CourseId", studentCourse.CourseId);
            var gradeParam = new Microsoft.Data.SqlClient.SqlParameter("@Grade", (object?)studentCourse.Grade ?? DBNull.Value);
            var statusParam = new Microsoft.Data.SqlClient.SqlParameter("@Status", studentCourse.Status ?? "Enrolled");
            
            // Execute SQL update without OUTPUT clause
            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql, 
                studentIdParam, courseIdParam, gradeParam, statusParam);
            
            return rowsAffected > 0;
        }

        public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);
        }
    }
}

