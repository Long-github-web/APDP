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
            return await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<Users?> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            return await _context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
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
            // Check if entity is already being tracked
            var existingUser = await _context.User.FindAsync(user.Id);
            
            if (existingUser != null)
            {
                // Entity is already tracked, update its properties
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Role = user.Role;
                existingUser.Status = user.Status;
                existingUser.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return existingUser;
            }
            else
            {
                // Entity is not tracked, attach and update
            user.UpdatedAt = DateTime.Now;
            _context.User.Update(user);
            await _context.SaveChangesAsync();
            return user;
            }
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
