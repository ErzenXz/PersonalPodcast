using PersonalPodcast.Models;

namespace PersonalPodcast.DTO
{
    public class CommentResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public DateTime Date { get; set; }
        public required string Message { get; set; }
    }
}
