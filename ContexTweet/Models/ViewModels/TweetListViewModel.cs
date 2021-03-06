﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContexTweet.Models.ViewModels
{
    public class TweetListViewModel
    {
        public IEnumerable<Tweet> Tweets { get; set; }
        public PagingInfoViewModel PagingInfo { get; set; }
    }
}
