namespace SIMS.SimsDbContext.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = null!; // Mã sinh viên (ví dụ: SV001) - NOT NULL trong database
        public string? StudentId { get; set; } // Mã sinh viên phụ (có thể dùng cho mục đích khác)
        public string FullName { get; set; } = null!; // NOT NULL trong database
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? AcademicProgram { get; set; } // Chương trình học (ví dụ: Computer Science)
        public int? Year { get; set; } // Năm học (1, 2, 3, 4)
        public decimal? GPA { get; set; } // Điểm trung bình
        public int UserId { get; set; } // Foreign key to Users table
        public DateTime? EnrollmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Active"; // Active, Inactive, Graduated, Suspended
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Users User { get; set; } = null!;
    }
}

