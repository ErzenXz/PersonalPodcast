using Microsoft.EntityFrameworkCore;
using PersonalPodcast.Models;
using PersonalPodcast.Models.Security;

namespace PersonalPodcast.Data
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Podcast> Podcasts { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Rating> ratings { get; set; }
        public DbSet<Comment> comments { get; set; }
        public DbSet<Stats> stats { get; set; }
        public DbSet<AudioAnalytics> audioAnalytics { get; set; }
        public DbSet<AccountSecurity> accountSecurity { get; set; }
        public DbSet<IpMitigations> ipMitigations { get; set; }
    }
}
