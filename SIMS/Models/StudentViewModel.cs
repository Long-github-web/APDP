using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        [Display(Name = "Student ID")]
        [StringLength(20, ErrorMessage = "Student ID cannot exceed 20 characters")]
        public string StudentId { get; set; } = null!;

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(200, ErrorMessage = "Full Name cannot exceed 200 characters")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Address")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [Display(Name = "Phone")]
        [StringLength(15, ErrorMessage = "Phone cannot exceed 15 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [Display(Name = "Email")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Display(Name = "Academic Program")]
        [StringLength(200, ErrorMessage = "Academic Program cannot exceed 200 characters")]
        public string? AcademicProgram { get; set; }

        [Display(Name = "Year")]
        [Range(1, 10, ErrorMessage = "Year must be between 1 and 10")]
        public int? Year { get; set; }

        [Display(Name = "GPA")]
        [Range(0, 4, ErrorMessage = "GPA must be between 0 and 4")]
        public decimal? GPA { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";

        // For display purposes
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }
    }

    public class StudentCreateViewModel : StudentViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public new string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}

