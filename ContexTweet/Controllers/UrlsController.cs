using System.Collections.Generic;
using System.Linq;
using ContexTweet.Data;
using ContexTweet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ContexTweet.Controllers
{
    [Route("[controller]")]
    public class UrlsController : Controller
    {
        private ITweetRepository tweetRepository;

        public UrlsController(ITweetRepository tweetRepo)
        {
            tweetRepository = tweetRepo;
        }

        // GET urls/
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return tweetRepository.Urls
                .Select(utw => utw.Url)
                .Distinct()
                .AsEnumerable();
        }

        // GET urls/bynamedentity
        [HttpGet]
        [Route("bynamedentity")]
        public IActionResult ByNamedEntity(string namedEntityText, string excludeUrl = "")
        {
            // Get the tweet ids that mention the named entity first
            var tweetIds = tweetRepository.NamedEntities
                .Where(ner => ner.NamedEntityText.Equals(namedEntityText))
                .Select(ner => ner.TweetId)
                .Distinct()
                .ToList();

            // Get the URLs that mention the named entity (via the previous list)
            // Exclude the URL of the request
            // Group by URL text, order by descending occurrence 
            var urls = tweetRepository.Urls
                .Where(u => tweetIds.Contains(u.TweetId) && !u.Url.Equals(excludeUrl))
                .GroupBy(u => u.Url)
                .Select(u => new { uCount = u.Count(), u.Key })
                .OrderByDescending(uc => uc.uCount)
                .Select(uc => uc.Key)
                .ToList();

            if (urls.Count > 0)
            {
                return Ok(urls);
            }

            // No other URLs mention the named entity, only the URL from the request does.
            return NotFound();
        }

        // GET urls/sentiment?url=<something>
        [HttpGet]
        [Route("sentiment")]
        public IActionResult Sentiment(string url)
        {
            // Check if the url exists in the db first
            var exists = tweetRepository.Urls
                .Where(u => u.Url.Equals(url))
                .FirstOrDefault() != null;

            if (exists)
            {
                // Get sentiment scores from the tweets that link to the URL
                var sentScores = tweetRepository.Urls
                    .Where(u => u.Url.Equals(url))
                    .Select(u => u.Tweet.SentimentScore)
                    .ToList();

                // Average the values on the list, avoid division by zero
                var avg = sentScores.Count > 0 ? sentScores.Average() : 0.0f;

                // Build viewmodel
                var urlSentiment = new UrlSentimentViewModel()
                {
                    Url = url,
                    SentimentScores = sentScores,
                    Average = avg
                };
                return Ok(urlSentiment);
            }

            return NotFound();
        }
    }
}