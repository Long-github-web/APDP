using Microsoft.EntityFrameworkCore;
using SIMS.SimsDbContext.Entities;

namespace SIMS.SimsDbContext
{
    public class SimDbContext : DbContext
    {
        public SimDbContext(DbContextOptions<SimDbContext> options) : base(options) { }
        public DbSet<Users> User { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users configuration
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<Users>().HasKey("Id");
            modelBuilder.Entity<Users>().HasIndex("Username").IsUnique();
            modelBuilder.Entity<Users>().HasIndex("Email").IsUnique();
            modelBuilder.Entity<Users>().Property(u => u.Status).HasDefaultValue("Active");
            modelBuilder.Entity<Users>().Property(u => u.Role).HasDefaultValue("Admin");

            // Course configuration
            modelBuilder.Entity<Course>().ToTable("Courses");
            modelBuilder.Entity<Course>().HasKey("Id");
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
            modelBuilder.Entity<Course>().Property(c => c.Status).HasDefaultValue("Active");

            // StudentCourse configuration (many-to-many)
            modelBuilder.Entity<StudentCourse>().ToTable("StudentCourses");
            modelBuilder.Entity<StudentCourse>().HasKey("Id");
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany()
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StudentCourse>()
                .HasIndex(sc => new { sc.StudentId, sc.CourseId })
                .IsUnique();
            modelBuilder.Entity<StudentCourse>().Property(sc => sc.Status).HasDefaultValue("Enrolled");
            
            // Configure to avoid OUTPUT clause conflict with database triggers
            // Use Identity column generation
            modelBuilder.Entity<StudentCourse>()
                .Property(sc => sc.Id)
                .UseIdentityColumn();

            // Student configuration
            modelBuilder.Entity<Student>().ToTable("Students");
            modelBuilder.Entity<Student>().HasKey("Id");
            modelBuilder.Entity<Student>().HasIndex(s => s.StudentCode).IsUnique(); // Index on StudentCode (NOT NULL)
            if (modelBuilder.Entity<Student>().Metadata.FindProperty("StudentId") != null)
            {
                // Only create index on StudentId if it exists and is not null
            }
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Student>().Property(s => s.Status).HasDefaultValue("Active");

            // ActivityLog configuration
            modelBuilder.Entity<ActivityLog>().ToTable("ActivityLogs");
            modelBuilder.Entity<ActivityLog>().HasKey("Id");
            modelBuilder.Entity<ActivityLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull); // Don't delete logs when user is deleted
            modelBuilder.Entity<ActivityLog>().Property(a => a.CreatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
