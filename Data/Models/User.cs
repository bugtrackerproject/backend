namespace bugtracker_backend_net.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }

        // Navigation Properties
        public List<Project> Projects { get; set; } = new(); // Many-to-many relationship
        public List<Ticket> TicketsSubmitted { get; set; } = new(); // Tickets submitted by the user
        public List<Ticket> TicketsAssigned { get; set; } = new(); // Tickets assigned to the user
    }

    public enum UserRole
    {
        Admin = 1,
        Developer = 2,
        Submitter = 3,
    }
}
