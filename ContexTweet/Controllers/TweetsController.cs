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
            tweetsListVM.PagingInfo = new PagingInfo()
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
            if(tweet == null)
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
            if(indexedTweets.Count() > 0)
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
    }
}
