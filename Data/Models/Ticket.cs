using System.Net.Sockets;

namespace bugtracker_backend_net.Data.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Enum for the ticket type
        public TicketType Type { get; set; }  
        
        // Enum for the ticket status
        public TicketStatus Status { get; set; }

        // Enum for the ticket priority
        public TicketPriority Priority { get; set; }

        // Foreign Keys
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public Guid? AssigneeId { get; set; } // nullable - ticket not assigned to a user yet
        public User? Assignee { get; set; }
        
        public Guid SubmitterId { get; set; }
        public User Submitter { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }

    public enum TicketStatus
    {
        ToDo = 1,
        InProgress = 2,
        Completed = 3,
        Closed = 4
    }

    public enum TicketType
    {
        Bug = 1,
        FeatureRequest = 2,
        Improvement = 3
    }

    public enum TicketPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}
