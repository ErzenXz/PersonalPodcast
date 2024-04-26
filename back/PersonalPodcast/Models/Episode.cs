using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace PersonalPodcast.Models
{
    public class Episode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long PodcastId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Tags { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public required string PosterImg { get; set; }
        public required string AudioFileUrl { get; set; }
        public string? VideoFileUrl { get; set; }
        public long Length { get; set; }
        public long Views { get; set; }
        public long PublisherId { get; set; }

        [ForeignKey("PublisherId")]
        public User User { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<AudioAnalytics> AudioAnalytics { get; set; }
    }
}
