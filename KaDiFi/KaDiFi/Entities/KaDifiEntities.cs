using Microsoft.EntityFrameworkCore;

namespace KaDiFi.Entities
{

    public class KaDifiEntities : DbContext
    {
        public KaDifiEntities(DbContextOptions<KaDifiEntities> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }

    }
}
