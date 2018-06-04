namespace ContexTweet.Models
{
    public class NamedEntityTweet
    {
        public string NamedEntityText { get; set; }
        public NamedEntity NamedEntity { get; set; }

        public string TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
