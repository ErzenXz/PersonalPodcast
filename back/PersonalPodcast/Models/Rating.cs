using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PersonalPodcast.Models
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public int RatingValue { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("EpisodeId")]
        public Episode Episode { get; set; }
    }
}
