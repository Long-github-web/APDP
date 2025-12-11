namespace SIMS.SimsDbContext.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; } = null!;
        public string CourseName { get; set; } = null!;
        public string? Description { get; set; }
        public int? Credits { get; set; } // Made nullable to match database
        public string? Department { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property for many-to-many relationship
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}

