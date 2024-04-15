namespace PersonalPodcast.Models
{
    public class Episode
    {
        public long Id { get; set; }
        public long PodcastId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public string PosterImg { get; set; }
        public string AudioFileUrl { get; set; }
        public long? VideoFileUrl { get; set; }
        public long Length { get; set; }
        public long Views { get; set; }
        public long PublisherId { get; set; }
    }
}
