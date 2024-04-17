using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalPodcast.Models
{
    public class Podcast
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public long CategoryId { get; set; }
        public string? Tags { get; set; }
        public string? PosterImg { get; set; }
        public string? AudioFileUrl { get; set; }
        public string? VideoFileUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public long Views { get; set; }
        public long PublisherId { get; set; }

        [ForeignKey("PublisherId")]
        public User User { get; set; }

        [ForeignKey("CategoryId")]
        public  Category Category { get; set; }

        public Episode Episode { get; set; }
    }
}
