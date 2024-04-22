namespace PersonalPodcast.DTO
{
    public class RatingResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public int RatingValue { get; set; }
        public DateTime Date { get; set; }
    }
}
