using SIMS.SimsDbContext.Entities;

namespace SIMS.Interfaces
{
    public interface IActivityLogService
    {
        Task LogActivityAsync(int? userId, string username, string action, string entityType, int? entityId = null, string? description = null, string? ipAddress = null);
        Task<IEnumerable<ActivityLog>> GetAllActivityLogsAsync();
        Task<IEnumerable<ActivityLog>> GetActivityLogsByUserIdAsync(int userId);
        Task<IEnumerable<ActivityLog>> GetRecentActivityLogsAsync(int count = 50);
    }
}















