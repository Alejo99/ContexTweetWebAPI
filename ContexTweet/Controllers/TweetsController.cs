using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ContexTweet.Data;
using Microsoft.Extensions.Configuration;
using ContexTweet.Models.ViewModels;
using ContexTweet.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace ContexTweet.Controllers
{
    [Route("[controller]")]
    public class TweetsController : Controller
    {
        private readonly PagingOptions pagingOptions;
        private ITweetRepository tweetRepository;

        public TweetsController(IOptions<PagingOptions> pagOptions, ITweetRepository tweetRepo)
        {
            pagingOptions = pagOptions.Value;
            tweetRepository = tweetRepo;
        }

        // GET: tweets?{p=1}
        [HttpGet]
        public IActionResult Get(int p = 1)
        {
            var tweetsListVM = new TweetListViewModel();
            tweetsListVM.Tweets = tweetRepository.Tweets
                .OrderByDescending(t => t.FavoriteCount)
                .ThenByDescending(t => t.RetweetCount)
                .ThenByDescending(t => t.Timestamp)
                .Skip((p - 1) * pagingOptions.PageSize)
                .Take(pagingOptions.PageSize)
                .AsEnumerable();
            tweetsListVM.PagingInfo = new PagingInfoViewModel()
            {
                CurrentPage = p,
                ItemsPerPage = pagingOptions.PageSize,
                TotalItems = tweetRepository.Tweets
                .Count()
            };

            if (tweetsListVM.Tweets.Count() > 0)
            {
                return Ok(tweetsListVM);
            }
            return NotFound();
        }

        // GET tweets/{id}
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var tweet = tweetRepository.Tweets.SingleOrDefault(tw => tw.Id == id);
            if (tweet == null)
            {
                return NotFound();
            }
            return Ok(tweet);
        }

        // GET tweets/byurl
        [HttpGet]
        [Route("byurl")]
        public IActionResult ByUrl(string url, string filter="no-filter")
        {
            // Check if tweets were clustered for this url
            var tweets = tweetRepository.IndexedUrls
                .Where(u => u.Url.Equals(url))
                .Select(t => t.Tweet);
            tweets = FilterTweets(tweets, filter);
            
            if (tweets.Count() > 0)
            {
                return Ok(tweets.AsEnumerable());
            }

            // If tweets were not clustered for this url
            tweets = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .Select(t => t.Tweet);
            tweets = FilterTweets(tweets, filter);

            if (tweets.Count() > 0)
            {
                return Ok(tweets.AsEnumerable());
            }

            // No tweets (neither clustered nor unclustered) for this url
            return NotFound();
        }
        
        private IQueryable<Models.Tweet> FilterTweets(IQueryable<Models.Tweet> tweets, string filter="no-filter")
        {
            switch (filter)
            {
                case "pos-sentiment":
                    tweets = tweets.Where(t => t.SentimentScore >= 0.05f)
                        .OrderByDescending(t => t.SentimentScore)
                        .ThenByDescending(t => t.Timestamp);
                    break;
                case "neg-sentiment":
                    tweets = tweets.Where(t => t.SentimentScore <= -0.05f)
                        .OrderBy(t => t.SentimentScore)
                        .ThenByDescending(t => t.Timestamp);
                    break;
                case "neutral-sentiment":
                    tweets = tweets.Where(t => t.SentimentScore < 0.05f && t.SentimentScore > -0.05f)
                        .OrderByDescending(t => t.RetweetCount)
                        .ThenByDescending(t => t.RetweetCount)
                        .ThenByDescending(t => t.Timestamp);
                    break;
                case "no-filter":
                default:
                    tweets = tweets.OrderByDescending(t => t.FavoriteCount)
                        .ThenByDescending(t => t.RetweetCount)
                        .ThenByDescending(t => t.Timestamp);
                    break;
            }
            return tweets;
        }
    }
}
