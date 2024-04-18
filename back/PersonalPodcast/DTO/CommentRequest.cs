using PersonalPodcast.Models;

namespace PersonalPodcast.DTO
{
    public class CommentRequest
    {
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public DateTime Date { get; set; }
        public required string Message { get; set; }
    }
}
