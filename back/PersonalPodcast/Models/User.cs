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
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int TokenVersion { get; set; }


        // One-to-many
        public ICollection<Podcast> Podcasts { get; set; }
        public ICollection<Episode> Episodes { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AudioAnalytics> AudioAnalytics { get; set; }

        // One-to-one
        public Admin Admin { get; set; }

    }
}
