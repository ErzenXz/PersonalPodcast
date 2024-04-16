namespace PersonalPodcast.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public DateTime Date { get; set; }
        public required string Message { get; set; }
    }
}
