using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

namespace ContexTweet.Data
{
    public class EFNamedEntityRepository : INamedEntityRepository
    {
        private ContexTweetDbContext context;

        public EFNamedEntityRepository(ContexTweetDbContext dbContext)
        {
            context = dbContext;
        }

        public DbSet<NamedEntity> NamedEntities => context.NamedEntities;
    }
}
