using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

namespace ContexTweet.Data
{
    public class ContexTweetDbContext : DbContext
    {
        public DbSet<Tweet> Tweets { get; set; }
        public DbSet<NamedEntity> NamedEntities { get; set; }
        public DbSet<UrlTweet> UrlsTweets { get; set; }
        public DbSet<NamedEntityTweet> NamedEntitiesTweets { get; set; }
        public DbSet<UrlTweetIndex> UrlTweetIndex { get; set; }

        public ContexTweetDbContext(DbContextOptions<ContexTweetDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Base OnModelCreating
            base.OnModelCreating(modelBuilder);

            // Tweet model
            modelBuilder.Entity<Tweet>()
                .HasKey("Id");

            // Named entity model
            modelBuilder.Entity<NamedEntity>()
                .HasKey("Text");

            // Url Tweet model
            modelBuilder.Entity<UrlTweet>()
                .HasKey(ut => new { ut.Url, ut.TweetId });

            // One tweet -> many urls
            modelBuilder.Entity<UrlTweet>()
                .HasOne(ut => ut.Tweet)
                .WithMany(t => t.Urls)
                .HasForeignKey(ut => ut.TweetId);

            // Named Entity Tweet model
            modelBuilder.Entity<NamedEntityTweet>()
                .HasKey(net => new { net.NamedEntityText, net.TweetId });

            // One named entity -> many tweets
            modelBuilder.Entity<NamedEntityTweet>()
                .HasOne(net => net.NamedEntity)
                .WithMany(ne => ne.Tweets)
                .HasForeignKey(net => net.NamedEntityText);

            // One tweet -> many named entities
            modelBuilder.Entity<NamedEntityTweet>()
                .HasOne(net => net.Tweet)
                .WithMany(t => t.NamedEntities)
                .HasForeignKey(net => net.TweetId);

            // Url Tweet Index model
            modelBuilder.Entity<UrlTweetIndex>()
                .HasKey(uti => new { uti.Url, uti.TweetId });

            // One tweet -> many urls
            modelBuilder.Entity<UrlTweetIndex>()
                .HasOne(uti => uti.Tweet)
                .WithMany()
                .HasForeignKey(uti => uti.TweetId);

        }
    }
}
