using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalPodcast.Models
{
    public class User
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }
        public string? ConnectingIp { get; set; }
        public DateTime? Birthdate { get; set; }
        public string? Role { get; set; }

        public Admin Admin { get; set; }
        public Podcast Podcast { get; set; }
        public Episode Episode { get; set; }
        public Rating Rating { get; set; }
        public Comment Comment { get; set; }
        public AudioAnalytics AudioAnalytics { get; set; }

    }
}
