using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContexTweet.Data;
using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContexTweet.Controllers
{
    [Route("[controller]")]
    public class TweetsController : Controller
    {
        private ITweetRepository tweetRepository;

        public TweetsController(ITweetRepository tweetRepo)
        {
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
        
        // POST tweets/byurl
        [HttpPost]
        [Route("byurl")]
        public IActionResult Post([FromBody] string url)
        {
            var tweets = tweetRepository.Urls
                .Where(u => u.Url.Contains(url))
                .Include(u => u.Tweet)
                .Select(t => t.Tweet)
                .AsEnumerable();

            if(tweets.Count() > 0)
            {
                return Ok(tweets); 
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
