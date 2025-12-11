using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface IStudentCourseRepository
    {
        Task<IEnumerable<StudentCourse>> GetStudentCoursesByCourseIdAsync(int courseId);
        Task<IEnumerable<StudentCourse>> GetStudentCoursesByStudentIdAsync(int studentId);
        Task<StudentCourse?> GetStudentCourseAsync(int studentId, int courseId);
        Task<StudentCourse> AssignStudentToCourseAsync(StudentCourse studentCourse);
        Task<bool> RemoveStudentFromCourseAsync(int studentId, int courseId);
        Task<bool> UpdateStudentCourseAsync(StudentCourse studentCourse);
        Task<bool> IsStudentEnrolledAsync(int studentId, int courseId);
    }
}










