using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SimDbContext _context;
        public UserRepository(SimDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<Users?> GetUserById(int id)
        {
            return await _context.User.FindAsync(id).AsTask();
        }

        public async Task<Users?> GetUserByUsername(string username)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Users?> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            return await _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Users>> GetUsersByRoleAsync(string role)
        {
            return await _context.User
                .Where(u => u.Role == role && u.Status == "Active")
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<Users>> GetAllUsersAsync()
        {
            return await _context.User
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<Users> UpdateUserAsync(Users user)
        {
            user.UpdatedAt = DateTime.Now;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null) return false;

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
