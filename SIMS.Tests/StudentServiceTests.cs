using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using SIMS.Interfaces;
using SIMS.Services;
using SIMS.SimsDbContext;
using SIMS.SimsDbContext.Entities;
using Xunit;

namespace SIMS.Tests
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _mockStudentRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ICourseRepository> _mockCourseRepository;
        private readonly SimDbContext _context;
        private readonly StudentService _studentService;

        public StudentServiceTests()
        {
            _mockStudentRepository = new Mock<IStudentRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCourseRepository = new Mock<ICourseRepository>();
            
            // Create in-memory database for testing
            // Suppress transaction warning since in-memory database doesn't support transactions
            var options = new DbContextOptionsBuilder<SimDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new SimDbContext(options);
            
            _studentService = new StudentService(
                _mockStudentRepository.Object,
                _mockUserRepository.Object,
                _mockCourseRepository.Object,
                _context
            );
        }

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsAllStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { Id = 1, StudentCode = "ST001", FullName = "Student One" },
                new Student { Id = 2, StudentCode = "ST002", FullName = "Student Two" }
            };

            _mockStudentRepository.Setup(r => r.GetAllStudentsAsync())
                .ReturnsAsync(students);

            // Act
            var result = await _studentService.GetAllStudentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.StudentCode == "ST001");
            Assert.Contains(result, s => s.StudentCode == "ST002");
            _mockStudentRepository.Verify(r => r.GetAllStudentsAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateStudentAsync_ValidData_ReturnsStudent()
        {
            // Arrange
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();
            
            var student = new Student
            {
                StudentCode = "ST001",
                FullName = "Test Student",
                Email = "test@example.com"
            };
            var username = "testuser";
            var password = "password123";

            _mockStudentRepository.Setup(r => r.StudentCodeExistsAsync("ST001"))
                .ReturnsAsync(false);
            _mockStudentRepository.Setup(r => r.StudentIdExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _mockUserRepository.Setup(r => r.GetUserByUsername(username))
                .ReturnsAsync((Users?)null);
            _mockUserRepository.Setup(r => r.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync((Users?)null);

            // Act
            var result = await _studentService.CreateStudentAsync(student, username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ST001", result.StudentCode);
            Assert.Equal("Test Student", result.FullName);
            Assert.True(result.UserId > 0); // UserId should be set after creation
            _mockStudentRepository.Verify(r => r.StudentCodeExistsAsync("ST001"), Times.Once);
            _mockUserRepository.Verify(r => r.GetUserByUsername(username), Times.Once);
            
            // Verify data was actually saved to in-memory database
            var savedStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == "ST001");
            Assert.NotNull(savedStudent);
            Assert.Equal("Test Student", savedStudent.FullName);
        }
    }
}
