using Microsoft.EntityFrameworkCore;

namespace KaDiFi.Entities
{

    public class KaDifiEntities : DbContext
    {
        public KaDifiEntities(DbContextOptions<KaDifiEntities> options) : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<MediaViews> MediaViews { get; set; }
        public DbSet<MediaComment> MediaCommnet { get; set; }
        public DbSet<AccountActivation> AccountActivation { get; set; }


    }
}
