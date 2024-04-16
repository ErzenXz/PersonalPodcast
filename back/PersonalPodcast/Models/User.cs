namespace PersonalPodcast.Models
{
    public class User
    {
        public long Id { get; set; }
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }
        public required string ConnectingIp { get; set; }
        public DateTime? Birthdate { get; set; }
    }
}
