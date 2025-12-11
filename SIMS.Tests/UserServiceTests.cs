using Moq;
using SIMS.Interfaces;
using SIMS.Services;
using SIMS.SimsDbContext.Entities;
using Xunit;

namespace SIMS.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "admin";
            var password = "admin123";
            var user = new Users
            {
                Id = 1,
                Username = username,
                Password = password,
                Role = "Admin",
                Status = "Active"
            };

            _mockUserRepository.Setup(r => r.GetUserByUsername(username))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginUser(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal(password, result.Password);
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Once);
        }
    }
}
