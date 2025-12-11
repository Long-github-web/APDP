using SIMS.Interfaces;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogService(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
        }

        public async Task LogActivityAsync(int? userId, string username, string action, string entityType, int? entityId = null, string? description = null, string? ipAddress = null)
        {
            try
            {
                var activityLog = new ActivityLog
                {
                    UserId = userId,
                    Username = username,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    IpAddress = ipAddress,
                    CreatedAt = DateTime.Now
                };

                await _activityLogRepository.CreateActivityLogAsync(activityLog);
            }
            catch
            {
                // Silently fail if ActivityLogs table doesn't exist yet
                // This allows the application to work even if the table hasn't been created
            }
        }

        public async Task<IEnumerable<ActivityLog>> GetAllActivityLogsAsync()
        {
            return await _activityLogRepository.GetAllActivityLogsAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetActivityLogsByUserIdAsync(int userId)
        {
            return await _activityLogRepository.GetActivityLogsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<ActivityLog>> GetRecentActivityLogsAsync(int count = 50)
        {
            return await _activityLogRepository.GetRecentActivityLogsAsync(count);
        }
    }
}

