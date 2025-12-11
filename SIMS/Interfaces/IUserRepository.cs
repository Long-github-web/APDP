using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface IUserRepository
    {
        Task<Users?> GetUserByUsername(string username);
        Task<Users?> GetUserById(int id);
        Task<Users?> GetUserByEmail(string email);
        Task<IEnumerable<Users>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users> CreateUserAsync(Users user);
        Task<Users> UpdateUserAsync(Users user);
        Task<bool> DeleteUserAsync(int id);
    }
}
