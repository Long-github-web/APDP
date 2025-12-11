using Moq;
using SIMS.Interfaces;
using SIMS.Services;
using SIMS.SimsDbContext.Entities;
using Xunit;

namespace SIMS.Tests
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly Mock<IStudentCourseRepository> _mockStudentCourseRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockCourseRepository = new Mock<ICourseRepository>();
            _mockStudentCourseRepository = new Mock<IStudentCourseRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockStudentRepository = new Mock<IStudentRepository>();
            
            _courseService = new CourseService(
                _mockCourseRepository.Object,
                _mockStudentCourseRepository.Object,
                _mockUserRepository.Object,
                _mockStudentRepository.Object
            );
        }

        [Fact]
        public async Task CreateCourseAsync_ValidData_ReturnsCourse()
        {
            // Arrange
            var course = new Course
            {
                CourseCode = "CS101",
                CourseName = "Introduction to Computer Science",
                Credits = 3,
                Status = "Active"
            };
            var createdCourse = new Course
            {
                Id = 1,
                CourseCode = "CS101",
                CourseName = "Introduction to Computer Science",
                Credits = 3,
                Status = "Active"
            };

            _mockCourseRepository.Setup(r => r.GetCourseByCodeAsync("CS101"))
                .ReturnsAsync((Course?)null);
            _mockCourseRepository.Setup(r => r.CreateCourseAsync(course))
                .ReturnsAsync(createdCourse);

            // Act
            var result = await _courseService.CreateCourseAsync(course);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CS101", result.CourseCode);
            Assert.Equal(1, result.Id);
            _mockCourseRepository.Verify(r => r.GetCourseByCodeAsync("CS101"), Times.Once);
            _mockCourseRepository.Verify(r => r.CreateCourseAsync(course), Times.Once);
        }
    }
}
