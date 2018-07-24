using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ContexTweet.Data;
using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        [Route("urls")]
        public IEnumerable<string> Urls()
        {
            return tweetRepository.Urls
                .Select(utw => utw.Url)
                .Distinct()
                .AsEnumerable();
        }

        // POST tweets/byurl
        [HttpPost]
        [Route("byurl")]
        public IActionResult ByUrl([FromBody] string url)
        {
            // Check if tweets were clustered for this url
            var indexedTweets = tweetRepository.IndexedUrls
                .Where(u => u.Url.Equals(url))
                .Select(t => t.Tweet)
                .OrderByDescending(t => t.FavoriteCount)
                .ThenByDescending(t => t.RetweetCount)
                .AsEnumerable();
            if (indexedTweets.Count() > 0)
            {
                return Ok(indexedTweets);
            }

            // Check if tweets were not clustered for this url
            var tweets = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .Select(t => t.Tweet)
                .OrderByDescending(t => t.FavoriteCount)
                .ThenByDescending(t => t.RetweetCount)
                .AsEnumerable();
            if (tweets.Count() > 0)
            {
                return Ok(tweets);
            }

            // No tweets (neither clustered nor unclustered) for this url
            return NotFound();
        }

        // POST tweets/namedentities
        [HttpPost]
        [Route("namedentities")]
        public IActionResult NamedEntities([FromBody] string url)
        {
            // Get tweet ids that mention the URL first
            var tweetIds = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .Select(t => t.TweetId)
                .Distinct()
                .ToList();

            // Get the named entities related to the URL (via the previous list)
            // Group by the text, order by descencing occurrence 
            var ners = tweetRepository.NamedEntities
                .Where(ner => tweetIds.Contains(ner.TweetId))
                .GroupBy(ner => ner.NamedEntityText)
                .Select(ner => new { nCount = ner.Count(), ner.Key })
                .OrderByDescending(nert => nert.nCount)
                .Select(nert => nert.Key)
                .ToList();

            if (ners.Count > 0)
            {
                // Get rid of duplicates not detected because capitalisation
                return Ok(ners.Distinct(StringComparer.InvariantCultureIgnoreCase));
            }

            // No named entities were found for the URL, that's unfortunate
            return NotFound();
        }

        // POST tweets/ners
        [HttpPost]
        [Route("neurls")]
        public IActionResult NeUrls([FromBody] NamedEntityUrlsReqViewModel nerUrlsReq)
        {
            // Get the tweet ids that mention the named entity first
            var tweetIds = tweetRepository.NamedEntities
                .Where(ner => ner.NamedEntityText.Equals(nerUrlsReq.NamedEntityText))
                .Select(ner => ner.TweetId)
                .Distinct()
                .ToList();

            // Get the URLs that mention the named entity (via the previous list)
            // Exclude the URL of the request
            // Group by URL text, order by descending occurrence 
            var urls = tweetRepository.Urls
                .Where(u => tweetIds.Contains(u.TweetId) && !u.Url.Equals(nerUrlsReq.Url))
                .GroupBy(u => u.Url)
                .Select(u => new { uCount = u.Count(), u.Key })
                .OrderByDescending(uc => uc.uCount)
                .Select(uc => uc.Key)
                .ToList();

            if(urls.Count > 0)
            {
                return Ok(urls);
            }
            
            // No other URLs mention the named entity, only the URL from the request does.
            return NotFound();
        }

        // POST tweets/namedentities
        [HttpPost]
        [Route("sentiment")]
        public IActionResult Sentiment([FromBody] string url)
        {
            // Get sentiment scores from the tweets that link to the URL
            var sentScores = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .Select(u => u.Tweet.SentimentScore)
                .ToList();

            // Average the values on the list, avoid division by zero
            var avg = sentScores.Count > 0 ? sentScores.Average() : 0.0;

            return Ok(avg);
        }
    }
}
