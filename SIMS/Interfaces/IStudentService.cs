using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student?> GetStudentByStudentIdAsync(string studentId);
        Task<Student?> GetStudentByUserIdAsync(int userId);
        Task<Student> CreateStudentAsync(Student student, string username, string password);
        Task<Student> UpdateStudentAsync(Student student);
        Task<Student> UpdateStudentBasicInfoAsync(int studentId, DateTime? dateOfBirth, string? gender, string? address, string? phone);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId);
    }
}

