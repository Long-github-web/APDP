using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface IActivityLogRepository
    {
        Task<ActivityLog> CreateActivityLogAsync(ActivityLog activityLog);
        Task<IEnumerable<ActivityLog>> GetAllActivityLogsAsync();
        Task<IEnumerable<ActivityLog>> GetActivityLogsByUserIdAsync(int userId);
        Task<IEnumerable<ActivityLog>> GetActivityLogsByActionAsync(string action);
        Task<IEnumerable<ActivityLog>> GetRecentActivityLogsAsync(int count = 50);
    }
}














