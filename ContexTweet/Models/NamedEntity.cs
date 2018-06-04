using System.Collections.Generic;

namespace ContexTweet.Models
{
    public class NamedEntity
    {
        public string Text { get; set; }

        public string Type { get; set; }

        public List<NamedEntityTweet> Tweets { get; set; }
    }
}
