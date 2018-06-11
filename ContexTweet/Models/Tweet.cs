using System;
using System.Collections.Generic;

namespace ContexTweet.Models
{
    public class Tweet
    {
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public float SentimentScore { get; set; }

        public List<UrlTweet> Urls { get; set; }

        public List<NamedEntityTweet> NamedEntities { get; set; }

        public int RetweetCount { get; set; }

        public int FavoriteCount { get; set; }
    }
}
