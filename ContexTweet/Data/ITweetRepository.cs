using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

namespace ContexTweet.Data
{
    public interface ITweetRepository
    {
        DbSet<Tweet> Tweets { get; }
        DbSet<UrlTweet> Urls { get; }
        DbSet<NamedEntityTweet> NamedEntities { get; }
    }
}
