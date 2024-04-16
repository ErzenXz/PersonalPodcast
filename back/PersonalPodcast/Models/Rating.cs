namespace PersonalPodcast.Models
{
    public class Rating
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long EpisodeId { get; set; }
        public int RatingValue { get; set; }
        public DateTime Date { get; set; }
        //test push
    }
}
