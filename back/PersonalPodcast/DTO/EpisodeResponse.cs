namespace PersonalPodcast.DTO
{
    public class EpisodeResponse
    {
        public long Id { get; set; }
        public long PodcastId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? Tags { get; set; }
        public required string PosterImg { get; set; }
        public required string AudioFileUrl { get; set; }
        public long? VideoFileUrl { get; set; }
        public long Length { get; set; }
        public long Views { get; set; }
        public long PublisherId { get; set; }
    }
}
