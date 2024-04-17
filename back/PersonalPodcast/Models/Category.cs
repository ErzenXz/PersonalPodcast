namespace PersonalPodcast.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string? Name { get; set; }

        public Podcast Podcast { get; set; }
    }
}
