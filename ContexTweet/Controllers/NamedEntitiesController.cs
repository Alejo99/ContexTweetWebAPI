using ContexTweet.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContexTweet.Controllers
{
    [Route("[controller]")]
    public class NamedEntitiesController : Controller
    {
        private ITweetRepository tweetRepository;

        public NamedEntitiesController(ITweetRepository tweetRepo)
        {
            tweetRepository = tweetRepo;
        }

        // GET namedentities/byurl
        [HttpGet]
        [Route("byurl")]
        public IActionResult NamedEntities(string url)
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
    }
}
