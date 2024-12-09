namespace bugtracker_backend_net.Data.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Guid UserId { get; set; }
    }
}
