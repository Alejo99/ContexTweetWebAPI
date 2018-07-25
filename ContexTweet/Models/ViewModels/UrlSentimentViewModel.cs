using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContexTweet.Models.ViewModels
{
    public class UrlSentimentViewModel
    {
        public string Url { get; set; }
        public IEnumerable<float> SentimentScores { get; set; }
        public float Average { get; set; }
    }
}
