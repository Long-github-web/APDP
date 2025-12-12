using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly SimDbContext _context;

        public CourseRepository(SimDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.StudentCourses)
                    .ThenInclude(sc => sc.Student)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course?> GetCourseByCodeAsync(string courseCode)
        {
            return await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            course.CreatedAt = DateTime.Now;
            course.UpdatedAt = DateTime.Now;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            // If course is already tracked, just save changes
            // Otherwise, attach and update
            var entry = _context.Entry(course);
            if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Detached)
            {
                _context.Courses.Update(course);
            }
            
            course.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            // Check if course exists first
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            // First, delete all StudentCourses related to this course using raw SQL
            // This avoids OUTPUT clause conflict with database triggers
            var deleteStudentCoursesSql = "DELETE FROM StudentCourses WHERE CourseId = @CourseId";
            var courseIdParam = new Microsoft.Data.SqlClient.SqlParameter("@CourseId", id);
            await _context.Database.ExecuteSqlRawAsync(deleteStudentCoursesSql, courseIdParam);

            // Then delete the course itself using raw SQL
            // Use raw SQL to avoid OUTPUT clause conflict with database triggers
            // SQL Server doesn't allow OUTPUT clause when triggers exist on the table
            var deleteCourseSql = "DELETE FROM Courses WHERE Id = @Id";
            var idParam = new Microsoft.Data.SqlClient.SqlParameter("@Id", id);
            
            // Execute SQL delete without OUTPUT clause
            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(deleteCourseSql, idParam);
            
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Include(sc => sc.Course)
                .Select(sc => sc.Course)
                .ToListAsync();
        }

        public async Task<bool> CourseExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id);
        }
    }
}


