using SIMS.Interfaces;
using SIMS.SimsDbContext.Entities;

namespace SIMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStudentRepository _studentRepository;

        public CourseService(
            ICourseRepository courseRepository,
            IStudentCourseRepository studentCourseRepository,
            IUserRepository userRepository,
            IStudentRepository studentRepository)
        {
            _courseRepository = courseRepository;
            _studentCourseRepository = studentCourseRepository;
            _userRepository = userRepository;
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _courseRepository.GetAllCoursesAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _courseRepository.GetCourseByIdAsync(id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            // Check if course code already exists
            var existingCourse = await _courseRepository.GetCourseByCodeAsync(course.CourseCode);
            if (existingCourse != null)
            {
                throw new InvalidOperationException($"Course with code {course.CourseCode} already exists.");
            }

            return await _courseRepository.CreateCourseAsync(course);
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            var existingCourse = await _courseRepository.GetCourseByIdAsync(course.Id);
            if (existingCourse == null)
            {
                throw new InvalidOperationException($"Course with id {course.Id} not found.");
            }

            // Check if course code is being changed and if new code already exists
            if (existingCourse.CourseCode != course.CourseCode)
            {
                var courseWithNewCode = await _courseRepository.GetCourseByCodeAsync(course.CourseCode);
                if (courseWithNewCode != null && courseWithNewCode.Id != course.Id)
                {
                    throw new InvalidOperationException($"Course with code {course.CourseCode} already exists.");
                }
            }

            // Update properties of the tracked entity instead of creating a new one
            existingCourse.CourseCode = course.CourseCode;
            existingCourse.CourseName = course.CourseName;
            existingCourse.Description = course.Description;
            existingCourse.Credits = course.Credits;
            existingCourse.Department = course.Department;
            existingCourse.Status = course.Status;
            existingCourse.UpdatedAt = DateTime.Now;

            return await _courseRepository.UpdateCourseAsync(existingCourse);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                return false;
            }

            return await _courseRepository.DeleteCourseAsync(id);
        }

        public async Task<IEnumerable<Student>> GetStudentsByCourseIdAsync(int courseId)
        {
            var studentCourses = await _studentCourseRepository.GetStudentCoursesByCourseIdAsync(courseId);
            return studentCourses.Select(sc => sc.Student).ToList();
        }

        public async Task<IEnumerable<Course>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _courseRepository.GetCoursesByStudentIdAsync(studentId);
        }

        public async Task<bool> AssignStudentToCourseAsync(int studentId, int courseId)
        {
            // Validate input
            if (studentId <= 0)
            {
                throw new InvalidOperationException("Invalid student ID. Student ID must be greater than 0.");
            }

            if (courseId <= 0)
            {
                throw new InvalidOperationException("Invalid course ID. Course ID must be greater than 0.");
            }

            // Check if student exists (studentId is Students.Id, not Users.Id)
            var student = await _studentRepository.GetStudentByIdAsync(studentId);
            if (student == null)
            {
                throw new InvalidOperationException($"Student with ID {studentId} not found in Students table. Please ensure the student exists before assigning to a course.");
            }

            // Verify the student has a valid UserId
            if (student.UserId <= 0)
            {
                throw new InvalidOperationException($"Student with ID {studentId} has invalid UserId. The student record is incomplete.");
            }

            // Check if course exists
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                throw new InvalidOperationException($"Course with ID {courseId} not found.");
            }

            // Check if already enrolled
            if (await _studentCourseRepository.IsStudentEnrolledAsync(studentId, courseId))
            {
                throw new InvalidOperationException("Student is already enrolled in this course.");
            }

            var studentCourse = new StudentCourse
            {
                StudentId = studentId, // This is Students.Id (must exist in Students table)
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Status = "Enrolled"
            };

            await _studentCourseRepository.AssignStudentToCourseAsync(studentCourse);
            return true;
        }

        public async Task<bool> RemoveStudentFromCourseAsync(int studentId, int courseId)
        {
            return await _studentCourseRepository.RemoveStudentFromCourseAsync(studentId, courseId);
        }

        public async Task<bool> UpdateStudentGradeAsync(int studentId, int courseId, string grade)
        {
            var studentCourse = await _studentCourseRepository.GetStudentCourseAsync(studentId, courseId);
            if (studentCourse == null)
            {
                return false;
            }

            studentCourse.Grade = grade;
            return await _studentCourseRepository.UpdateStudentCourseAsync(studentCourse);
        }
    }
}

