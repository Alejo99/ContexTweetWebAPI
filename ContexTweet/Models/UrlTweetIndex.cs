using System.Collections.Generic;

namespace ContexTweet.Models
{
    public class UrlTweetIndex
    {
        public string Url { get; set; }

        public string TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
