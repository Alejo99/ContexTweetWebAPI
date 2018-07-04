using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

namespace ContexTweet.Data
{
    /// <summary>
    /// Interface for operations related with Tweet persistence layer.
    /// </summary>
    public interface ITweetRepository
    {
        /// <summary>
        /// Tweets table
        /// </summary>
        DbSet<Tweet> Tweets { get; }
        
        /// <summary>
        /// URLs table
        /// </summary>
        DbSet<UrlTweet> Urls { get; }
        
        /// <summary>
        /// Named Entities table
        /// </summary>
        DbSet<NamedEntityTweet> NamedEntities { get; }
        
        /// <summary>
        /// Indexed URLs table
        /// </summary>
        DbSet<UrlTweetIndex> IndexedUrls { get; }
        
        /// <summary>
        /// Saves changes to the persistence layer
        /// </summary>
        void Commit();
    }
}
