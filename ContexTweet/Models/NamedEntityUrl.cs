using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContexTweet.Models
{
    public class NamedEntityUrl
    {
        public string NamedEntityText { get; set; }
        public NamedEntity NamedEntity { get; set; }

        public string Url { get; set; }
    }
}
