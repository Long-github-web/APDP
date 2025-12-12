using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course?> GetCourseByCodeAsync(string courseCode);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId);
        Task<bool> CourseExistsAsync(int id);
    }
}













