namespace ContexTweet.Models
{
    public class UrlTweet
    {
        public string Url { get; set; }

        public string TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
