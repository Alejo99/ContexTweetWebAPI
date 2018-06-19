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

        // GET: tweets
        [HttpGet]
        public IEnumerable<Tweet> Get()
        {
            return tweetRepository.Tweets.ToList();
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
        
        // POST tweets/byurl/{p}
        [HttpPost]
        [Route("byurl/{p}")]
        public IActionResult ByUrl(int p, [FromBody] string url)
        {
            var tweetsListVM = new TweetListViewModel();
            tweetsListVM.Tweets = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .Include(u => u.Tweet)
                .Select(t => t.Tweet)
                .OrderByDescending(t => t.FavoriteCount)
                .ThenByDescending(t => t.RetweetCount)
                .Skip((p - 1) * pagingOptions.PageSize)
                .Take(pagingOptions.PageSize)
                .AsEnumerable();
            tweetsListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = p,
                ItemsPerPage = pagingOptions.PageSize,
                TotalItems = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .Select(t => t.Tweet)
                .Count()
            };

            if(tweetsListVM.Tweets.Count() > 0)
            {
                return Ok(tweetsListVM); 
            }
            return NotFound();
        }
        /*
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}
