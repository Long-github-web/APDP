using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly SimDbContext _context;

        public ActivityLogRepository(SimDbContext context)
        {
            _context = context;
        }

        public async Task<ActivityLog> CreateActivityLogAsync(ActivityLog activityLog)
        {
            try
            {
                activityLog.CreatedAt = DateTime.Now;
                _context.ActivityLogs.Add(activityLog);
                await _context.SaveChangesAsync();
                return activityLog;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // If table doesn't exist, return the log object without saving
                // This allows the app to continue working
                return activityLog;
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetAllActivityLogsAsync()
        {
            try
            {
                return await _context.ActivityLogs
                    .Include(a => a.User)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                // Return empty list if table doesn't exist
                return new List<ActivityLog>();
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetActivityLogsByUserIdAsync(int userId)
        {
            try
            {
                return await _context.ActivityLogs
                    .Include(a => a.User)
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                // Return empty list if table doesn't exist
                return new List<ActivityLog>();
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetActivityLogsByActionAsync(string action)
        {
            try
            {
                return await _context.ActivityLogs
                    .Include(a => a.User)
                    .Where(a => a.Action == action)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                // Return empty list if table doesn't exist
                return new List<ActivityLog>();
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetRecentActivityLogsAsync(int count = 50)
        {
            try
            {
                return await _context.ActivityLogs
                    .Include(a => a.User)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch
            {
                // Return empty list if table doesn't exist
                return new List<ActivityLog>();
            }
        }
    }
}

