namespace SIMS.SimsDbContext.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public string Role { get; set; } = "Admin";
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime? CreatedAt {  get; set; } = DateTime.Now;
        public DateTime? UpdatedAt {  get; set; } = DateTime.Now;
    }
}
