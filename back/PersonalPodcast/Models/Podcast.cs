namespace PersonalPodcast.Models
{
    public class Podcast
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public long Category { get; set; }
        public string? Tags { get; set; }
        public string? PosterImg { get; set; }
        public string? AudioFileUrl { get; set; }
        public string? VideoFileUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public long Views { get; set; }
        public long PublisherId { get; set; }
    }
}
