namespace PersonalPodcast.DTO
{
    public class PodcastResponse
    {
        public long id {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long CategoryId { get; set; }
        public string Tags { get; set; }
        public string PosterImg { get; set; }
        public string AudioFileUrl { get; set; }
        public string VideoFileUrl { get; set; }
        public long PublisherId { get; set; }

    }
}
