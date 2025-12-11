using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course Code is required")]
        [Display(Name = "Course Code")]
        [StringLength(20, ErrorMessage = "Course Code cannot exceed 20 characters")]
        public string CourseCode { get; set; } = null!;

        [Required(ErrorMessage = "Course Name is required")]
        [Display(Name = "Course Name")]
        [StringLength(200, ErrorMessage = "Course Name cannot exceed 200 characters")]
        public string CourseName { get; set; } = null!;

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Credits is required")]
        [Range(1, 10, ErrorMessage = "Credits must be between 1 and 10")]
        [Display(Name = "Credits")]
        public int Credits { get; set; }

        [Display(Name = "Department")]
        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
        public string? Department { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Active";
    }
}










