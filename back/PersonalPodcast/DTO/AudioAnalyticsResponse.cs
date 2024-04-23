namespace PersonalPodcast.DTO
{
    public class AudioAnalyticsResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public DateTime FirstPlay { get; set; }
        public DateTime LastPlay { get; set; }
        public long Length { get; set; }
    }
}
