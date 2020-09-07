using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Podplayer.Core.Models;
using Podplayer.Entity.Identity;

namespace Podplayer.Entity
{
    public class PodplayerDbContext : IdentityDbContext<AppUser>
    {

        #region DbSets

        public DbSet<Podcast> Podcasts { get; set; }

        public DbSet<AppUserPodcastJoin> AppUserPodcasts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<PodcastCategory> PodcastCategories { get; set; }

        public DbSet<Creator> Creators { get; set; }
        public DbSet<PodcastCreator> PodcastCreators { get; set; }

        #endregion

        public PodplayerDbContext(DbContextOptions options) : base(options) {}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // set ignore on unwanted Podcast properties
            modelBuilder.Entity<Podcast>().Ignore(p => p.Episodes);
            modelBuilder.Entity<Podcast>().Ignore(p => p.Subscribed);

            // set up uni-directional relationship between AppUser and Podcast
           modelBuilder.Entity<AppUserPodcastJoin>()
              .HasKey(sc => new { sc.PodcastId, sc.AppUserId });

            modelBuilder.Entity<AppUserPodcastJoin>()
                .HasOne(ap => ap.AppUser)
                .WithMany(au => au.SubscribedPodcasts)
                .HasForeignKey(ap => ap.AppUserId);

            // set up join table betwen Podcast and Category
            modelBuilder.Entity<PodcastCategory>()
                .HasKey(pc => new { pc.PodcastId, pc.CategoryId });
            modelBuilder.Entity<PodcastCategory>()
                .HasOne(pc => pc.Podcast)
                .WithMany(p => p.Categories)
                .HasForeignKey(pc => pc.PodcastId);
            modelBuilder.Entity<PodcastCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(p => p.Podcasts)
                .HasForeignKey(pc => pc.CategoryId);

            // set up join table between Podcast and Creator
            modelBuilder.Entity<PodcastCreator>()
                .HasKey(pc => new { pc.PodcastId, pc.CreatorId });
            modelBuilder.Entity<PodcastCreator>()
                .HasOne(pc => pc.Podcast)
                .WithOne(p => p.Creator)
                .HasForeignKey<PodcastCreator>(pc => pc.PodcastId);
            modelBuilder.Entity<PodcastCreator>()
                .HasOne(pc => pc.Creator)
                .WithMany(c => c.Podcasts)
                .HasForeignKey(pc => pc.CreatorId);

        }
    }
}
