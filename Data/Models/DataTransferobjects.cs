using bugtracker_backend_net.Data.Models;
using System.Data;

namespace bugtracker_backend_net.Data.DataTransferObjects
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string? Password { get; set; }

        public UserRole Role { get; set; }
    }

    public class UserIdDto
    {
        public Guid Id { get; set; }
    }

    public class ProjectDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid[] Users { get; set; } = Array.Empty<Guid>();
    }

    public class ProjectResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public List<Guid> Users { get; set; } = new();
    }

    public class TicketDto
    {
        public string Name { get; set; }
        public string Description { get; set; }     
        public TicketType Type { get; set; }   
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssigneeId { get; set; }
    }

    public class TicketUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TicketType? Type { get; set; } = null;
        public TicketStatus? Status { get; set; } = null;
        public TicketPriority? Priority { get; set; } = null;
        public Guid? ProjectId { get; set; } = null;
        public Guid? AssigneeId { get; set; } = null;
    }

    public class TicketResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public Guid Project { get; set; }
        public Guid? Assignee { get; set; }
        public Guid Submitter { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }

}