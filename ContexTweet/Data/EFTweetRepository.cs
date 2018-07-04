using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

namespace ContexTweet.Data
{
    public class EFTweetRepository : ITweetRepository
    {
        private ContexTweetDbContext context;

        public EFTweetRepository(ContexTweetDbContext dbContext)
        {
            context = dbContext;
        }

        public DbSet<Tweet> Tweets => context.Tweets;

        public DbSet<UrlTweet> Urls => context.UrlsTweets;

        public DbSet<NamedEntityTweet> NamedEntities => context.NamedEntitiesTweets;

        public DbSet<UrlTweetIndex> IndexedUrls => context.UrlTweetIndex;

        public void Commit()
        {
            context.SaveChanges();
        }
    }
}
