using ContexTweet.Models;
using Microsoft.EntityFrameworkCore;

namespace ContexTweet.Data
{
    public interface INamedEntityRepository
    {
        DbSet<NamedEntity> NamedEntities { get; }
    }
}
