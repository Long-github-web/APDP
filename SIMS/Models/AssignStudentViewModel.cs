using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class AssignStudentViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public string CourseCode { get; set; } = null!;

        [Required(ErrorMessage = "Please select a student")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        public List<StudentOption> AvailableStudents { get; set; } = new List<StudentOption>();
        public List<EnrolledStudentViewModel> EnrolledStudents { get; set; } = new List<EnrolledStudentViewModel>();
    }

    public class StudentOption
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }

    public class EnrolledStudentViewModel
    {
        public int StudentId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? EnrollmentDate { get; set; }
        public string? Grade { get; set; }
        public string Status { get; set; } = null!;
    }
}















