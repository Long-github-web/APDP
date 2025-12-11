using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId);
        Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId);
        Task<bool> AssignStudentToCourseAsync(int studentId, int courseId);
        Task<bool> RemoveStudentFromCourseAsync(int studentId, int courseId);
        Task<bool> UpdateStudentGradeAsync(int studentId, int courseId, string grade);
    }
}

