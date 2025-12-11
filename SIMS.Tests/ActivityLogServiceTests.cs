using Moq;
using SIMS.Interfaces;
using SIMS.Services;
using SIMS.SimsDbContext.Entities;
using Xunit;

namespace SIMS.Tests
{
    public class ActivityLogServiceTests
    {
        private readonly Mock<IActivityLogRepository> _mockActivityLogRepository;
        private readonly ActivityLogService _activityLogService;

        public ActivityLogServiceTests()
        {
            _mockActivityLogRepository = new Mock<IActivityLogRepository>();
            _activityLogService = new ActivityLogService(_mockActivityLogRepository.Object);
        }

        [Fact]
        public async Task LogActivityAsync_ValidData_CreatesActivityLog()
        {
            // Arrange
            var userId = 1;
            var username = "admin";
            var action = "Login";
            var entityType = "User";
            var entityId = 1;
            var description = "User logged in";
            var ipAddress = "127.0.0.1";

            var activityLog = new ActivityLog
            {
                Id = 1,
                UserId = userId,
                Username = username,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                IpAddress = ipAddress,
                CreatedAt = DateTime.Now
            };

            _mockActivityLogRepository.Setup(r => r.CreateActivityLogAsync(It.IsAny<ActivityLog>()))
                .ReturnsAsync(activityLog);

            // Act
            await _activityLogService.LogActivityAsync(userId, username, action, entityType, entityId, description, ipAddress);

            // Assert
            _mockActivityLogRepository.Verify(r => r.CreateActivityLogAsync(It.Is<ActivityLog>(al =>
                al.UserId == userId &&
                al.Username == username &&
                al.Action == action &&
                al.EntityType == entityType &&
                al.EntityId == entityId &&
                al.Description == description &&
                al.IpAddress == ipAddress
            )), Times.Once);
        }
    }
}





