namespace SIMS.SimsDbContext.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; } // Nullable for system actions
        public string Username { get; set; } = null!; // Store username for reference even if user is deleted
        public string Action { get; set; } = null!; // e.g., "Login", "Create Student", "Update Course"
        public string EntityType { get; set; } = null!; // e.g., "Student", "Course", "User"
        public int? EntityId { get; set; } // ID of the affected entity
        public string? Description { get; set; } // Additional details
        public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Users? User { get; set; }
    }
}















