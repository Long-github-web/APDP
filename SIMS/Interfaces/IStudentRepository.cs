using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student?> GetStudentByStudentIdAsync(string studentId);
        Task<Student?> GetStudentByStudentCodeAsync(string studentCode);
        Task<Student?> GetStudentByUserIdAsync(int userId);
        Task<Student> CreateStudentAsync(Student student);
        Task<Student> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task<bool> StudentExistsAsync(int id);
        Task<bool> StudentIdExistsAsync(string studentId);
        Task<bool> StudentCodeExistsAsync(string studentCode);
    }
}

