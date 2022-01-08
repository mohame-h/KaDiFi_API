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
        public DbSet<MediaComment> MediaComment { get; set; }
        public DbSet<MediaCommentReply> MediaCommentReply { get; set; }

        public DbSet<AccountVerification> AccountVerification { get; set; }

        public DbSet<AccountDeactivation> AccountDeactivation { get; set; }


    }
}
