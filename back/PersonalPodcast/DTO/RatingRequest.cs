namespace PersonalPodcast.DTO
{
    public class RatingRequest
    {
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public int RatingValue { get; set; }
        public DateTime Date { get; set; }
    }
}
