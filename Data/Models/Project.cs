

using System.Text.Json.Serialization;

namespace bugtracker_backend_net.Data.Models
{
    public class Project
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<User> Users { get; set; } = new();

        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
