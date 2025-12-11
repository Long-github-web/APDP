namespace SIMS.SimsDbContext.Entities
{
    public class StudentCourse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now; // NOT NULL trong database
        public string? Grade { get; set; }
        public string Status { get; set; } = "Enrolled";

        // Navigation properties
        public Student Student { get; set; } = null!; // Fixed: Should be Student, not Users
        public Course Course { get; set; } = null!;
    }
}

