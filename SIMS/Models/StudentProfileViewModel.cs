using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class StudentProfileViewModel
    {
        public int Id { get; set; }

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

        // Read-only fields for display
        [Display(Name = "Student ID")]
        public string StudentId { get; set; } = null!;

        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}

