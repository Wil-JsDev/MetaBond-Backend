using MetaBond.Domain;
using MetaBond.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace MetaBond.Infrastructure.Persistence.Context
{
    public class MetaBondContext : DbContext
    {
        public MetaBondContext(DbContextOptions<MetaBondContext> options) : base(options)
        { }

        #region Models
        public DbSet<Communities> Communities { get; set; }

        public DbSet<Events> Events { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<ParticipationInEvent> Participations { get; set; }

        public DbSet<Posts> Posts { get; set; }

        public DbSet<Rewards> Rewards { get; set; }

        public DbSet<ProgressBoard> ProgressBoard { get; set; }

        public DbSet<ProgressEntry> ProgressEntry { get; set; }

        public DbSet<EventParticipation> eventParticipations { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Tables

            modelBuilder.Entity<Communities>()
                        .ToTable("Communities");

            modelBuilder.Entity<Events>()
                        .ToTable("Events");


            modelBuilder.Entity<Friendship>()
                        .ToTable("Friendship");

            modelBuilder.Entity<ParticipationInEvent>()
                        .ToTable("ParticiationInEvent");

            modelBuilder.Entity<Posts>()
                        .ToTable("Posts");

            modelBuilder.Entity<Rewards>()
                        .ToTable("Rewards");

            modelBuilder.Entity<ProgressBoard>()
                        .ToTable("ProgressBoard");

            modelBuilder.Entity<ProgressEntry>()
                        .ToTable("ProgressEntry");

            #endregion

            #region PrimaryKey
            modelBuilder.Entity<Communities>()
                        .HasKey(x => x.Id)
                        .HasName("PkCommunities");

            modelBuilder.Entity<EventParticipation>()
                        .HasKey(ep => new { ep.EventId, ep.ParticipationInEventId });

            modelBuilder.Entity<ParticipationInEvent>()
                        .HasKey(x => x.Id)
                        .HasName("PkParticipationInEvent");

            modelBuilder.Entity<Events>()
                        .HasKey(x => x.Id)
                        .HasName("PkEvents");

            modelBuilder.Entity<Friendship>()
                        .HasKey(x => x.Id)
                        .HasName("PkFriendship");

            modelBuilder.Entity<Posts>()
                        .HasKey(x => x.Id)
                        .HasName("PkPosts");

            modelBuilder.Entity<Rewards>()
                        .HasKey(x => x.Id)
                        .HasName("PkRewards");

            modelBuilder.Entity<ProgressEntry>()
                        .HasKey(x => x.Id)
                        .HasName("PkProgressEntry");

            modelBuilder.Entity<ProgressBoard>()
                        .HasKey(x => x.Id)
                        .HasName("PkProgressBoard");
            #endregion

            #region Relationships
            modelBuilder.Entity<Communities>()
                        .HasMany(c => c.Events)
                        .WithOne(v => v.Communities)
                        .HasForeignKey(x => x.CommunitiesId)
                        .IsRequired()
                        .HasConstraintName("FkCommunitiesId");

            modelBuilder.Entity<Communities>()
                        .HasMany(c => c.Posts)
                        .WithOne(p => p.Communities)
                        .HasForeignKey(x => x.CommunitiesId)
                        .IsRequired()
                        .HasConstraintName("FkCommunities");

            modelBuilder.Entity<EventParticipation>()
                        .HasOne(ep => ep.Event)
                        .WithMany(e => e.EventParticipations)
                        .HasForeignKey(ep => ep.EventId)
                        .IsRequired()
                        .HasConstraintName("FkEvent");

            modelBuilder.Entity<EventParticipation>()
                        .HasOne(ep => ep.ParticipationInEvent)
                        .WithMany(p => p.EventParticipations)
                        .HasForeignKey(ep => ep.ParticipationInEventId)
                        .IsRequired()
                        .HasConstraintName("FkParticipationInEvent");

            modelBuilder.Entity<ProgressBoard>()
                        .HasOne(x => x.Communities)
                        .WithOne(x => x.ProgressBoard)
                        .HasForeignKey<ProgressBoard>(x => x.CommunitiesId)
                        .IsRequired()
                        .HasConstraintName("FkCommunitiesId");

            modelBuilder.Entity<ProgressEntry>()
                        .HasOne(x => x.ProgressBoard)
                        .WithMany(x => x.ProgressEntries)
                        .HasForeignKey(x => x.ProgressBoardId)
                        .IsRequired()
                        .HasConstraintName("FkProgressBoardId");


            #endregion

            #region Communities

            modelBuilder.Entity<Communities>(x =>
            {
                x.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

                x.Property(c => c.Description)
                 .IsRequired()
                 .HasMaxLength(maxLength: 250);

                x.Property(c => c.Category)
                  .HasMaxLength(25)
                  .IsRequired();
            });

            #endregion

            #region Friendship

            modelBuilder.HasPostgresEnum<Status>();

            #endregion

            #region Posts
            modelBuilder.Entity<Posts>(p =>
            {
                p.Property(p => p.Title)
                  .HasMaxLength(50)
                  .IsRequired();

                p.Property(p => p.Content)
                  .HasMaxLength(150)
                  .IsRequired();
                
                p.Property(p => p.Image)
                 .HasMaxLength(250)
                 .IsRequired();
            });
            #endregion

            #region Events

            modelBuilder.Entity<Events>(e =>
            {
                e.Property(e => e.Title)
                 .HasMaxLength(50)
                 .IsRequired();

                e.Property(e => e.Description)
                 .HasMaxLength(maxLength: 250)
                 .IsRequired();
            });

            #endregion

            #region Rewards

            modelBuilder.Entity<Rewards>(p =>
            {

                p.Property(p => p.Description)
                 .HasMaxLength(250)
                 .IsRequired();

                p.Property(p => p.PointAwarded)
                 .IsRequired();
            });

            #endregion

            #region Progress Entry
            modelBuilder.Entity<ProgressEntry>(x =>
            {
                x.Property(p => p.Description)
                 .HasMaxLength(250)
                 .IsRequired();
            });

            #endregion
        }
    }
}
