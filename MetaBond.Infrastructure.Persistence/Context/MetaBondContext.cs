using MetaBond.Domain;
using MetaBond.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Context
{
    public class MetaBondContext(DbContextOptions<MetaBondContext> options) : DbContext(options)
    {
        #region Models

        public DbSet<Communities> Communities { get; set; }

        public DbSet<Events> Events { get; set; }

        public DbSet<Friendship> Friendships { get; set; }

        public DbSet<ParticipationInEvent> Participations { get; set; }

        public DbSet<Posts> Posts { get; set; }

        public DbSet<Rewards> Rewards { get; set; }

        public DbSet<ProgressBoard> ProgressBoard { get; set; }

        public DbSet<ProgressEntry> ProgressEntry { get; set; }

        public DbSet<EventParticipation> EventParticipation { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

        public DbSet<Interest> Interests { get; set; }

        public DbSet<UserInterest> UserInterests { get; set; }

        public DbSet<Roles> Roles { get; set; }

        public DbSet<CommunityMembership> CommunityMembership { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Tables

            modelBuilder.Entity<Communities>()
                .ToTable("Communities");

            modelBuilder.Entity<Roles>()
                .ToTable("Roles");

            modelBuilder.Entity<CommunityMembership>()
                .ToTable("CommunityMemberships");

            modelBuilder.Entity<Events>()
                .ToTable("Events");

            modelBuilder.Entity<EventParticipation>()
                .ToTable("EventParticipation");

            modelBuilder.Entity<Friendship>()
                .ToTable("Friendship");

            modelBuilder.Entity<ParticipationInEvent>()
                .ToTable("ParticipationInEvent");

            modelBuilder.Entity<Posts>()
                .ToTable("Posts");

            modelBuilder.Entity<Rewards>()
                .ToTable("Rewards");

            modelBuilder.Entity<ProgressBoard>()
                .ToTable("ProgressBoard");

            modelBuilder.Entity<ProgressEntry>()
                .ToTable("ProgressEntry");

            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<Admin>()
                .ToTable("Admins");

            modelBuilder.Entity<Interest>()
                .ToTable("Interests");

            modelBuilder.Entity<UserInterest>()
                .ToTable("UserInterests");

            modelBuilder.Entity<EmailConfirmationToken>()
                .ToTable("EmailConfirmationTokens");

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


            modelBuilder.Entity<User>()
                .HasKey(x => x.Id)
                .HasName("PkUser");

            modelBuilder.Entity<Admin>()
                .HasKey(x => x.Id)
                .HasName("PkAdmin");

            modelBuilder.Entity<Interest>()
                .HasKey(x => x.Id)
                .HasName("PkInterest");

            modelBuilder.Entity<EmailConfirmationToken>()
                .HasKey(x => x.Id)
                .HasName("PkEmailConfirmationToken");

            modelBuilder.Entity<Roles>()
                .HasKey(x => x.Id)
                .HasName("PkRoles");

            modelBuilder.Entity<CommunityMembership>()
                .HasKey(us => new { us.UserId, us.CommunityId });

            modelBuilder.Entity<UserInterest>()
                .HasKey(uc => new { uc.UserId, uc.InterestId });

            #endregion

            #region Relationships

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(ro => ro.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired()
                .HasConstraintName("FkUserRolesId");

            modelBuilder.Entity<User>()
                .HasMany(us => us.CommunityMemberships)
                .WithOne(cm => cm.User)
                .HasForeignKey(cm => cm.UserId)
                .IsRequired()
                .HasConstraintName("FkCommunityMembershipsUserId");

            modelBuilder.Entity<CommunityMembership>()
                .HasOne(cm => cm.Community)
                .WithMany(cm => cm.CommunityMemberships)
                .HasForeignKey(cm => cm.CommunityId)
                .IsRequired()
                .HasConstraintName("FkCommunityMembershipsCommunityId");

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

            modelBuilder.Entity<UserInterest>()
                .HasOne(ui => ui.Interest)
                .WithMany(interest => interest.UserInterests)
                .HasForeignKey(ui => ui.InterestId)
                .HasConstraintName("FkInterestId");

            modelBuilder.Entity<UserInterest>()
                .HasOne(ui => ui.User)
                .WithMany(interest => interest.Interests)
                .HasForeignKey(ui => ui.UserId)
                .HasConstraintName("FkUserInterestsUserId");

            modelBuilder.Entity<Interest>()
                .HasMany(interest => interest.UserInterests)
                .WithOne(ui => ui.Interest)
                .HasForeignKey(iui => iui.InterestId)
                .IsRequired()
                .HasConstraintName("FkInterestId");

            modelBuilder.Entity<Friendship>()
                .HasOne(fp => fp.Requester)
                .WithMany(us => us.SentFriendRequests)
                .HasForeignKey(fp => fp.RequesterId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FkRequesterId");

            modelBuilder.Entity<Friendship>()
                .HasOne(fp => fp.Addressee)
                .WithMany(us => us.ReceivedFriendRequests)
                .HasForeignKey(fp => fp.AddresseeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FkAddresseeId");

            modelBuilder.Entity<EmailConfirmationToken>()
                .HasOne(em => em.User)
                .WithMany(us => us.EmailConfirmationTokens)
                .HasForeignKey(em => em.UserId)
                .HasConstraintName("FkEmailConfirmationTokensUserId")
                .IsRequired();

            modelBuilder.Entity<Posts>()
                .HasOne(po => po.CreatedBy)
                .WithMany(us => us.Posts)
                .HasForeignKey(po => po.CreatedById)
                .HasConstraintName("FkPostsCreatedById")
                .IsRequired();

            modelBuilder.Entity<ProgressEntry>()
                .HasOne(pe => pe.User)
                .WithMany(us => us.ProgressEntries)
                .HasForeignKey(pe => pe.UserId)
                .HasConstraintName("FkProgressEntriesUserId")
                .IsRequired();

            modelBuilder.Entity<ProgressBoard>()
                .HasOne(pb => pb.User)
                .WithMany(us => us.ProgressBoards)
                .HasForeignKey(pb => pb.UserId)
                .HasConstraintName("FkProgressBoardsUserId")
                .IsRequired();

            modelBuilder.Entity<Rewards>()
                .HasOne(ep => ep.User)
                .WithMany(rs => rs.Rewards)
                .HasForeignKey(ep => ep.UserId)
                .HasConstraintName("FkRewardsUserId")
                .IsRequired();

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
                p.Property(post => post.Title)
                    .HasMaxLength(50)
                    .IsRequired();

                p.Property(post => post.Content)
                    .HasMaxLength(150)
                    .IsRequired();

                p.Property(post => post.Image)
                    .HasMaxLength(250)
                    .IsRequired();
            });

            #endregion

            #region Events

            modelBuilder.Entity<Events>(e =>
            {
                e.Property(events => events.Title)
                    .HasMaxLength(50)
                    .IsRequired();

                e.Property(events => events.Description)
                    .HasMaxLength(maxLength: 250)
                    .IsRequired();
            });

            #endregion

            #region Rewards

            modelBuilder.Entity<Rewards>(p =>
            {
                p.Property(rewards => rewards.Description)
                    .HasMaxLength(250)
                    .IsRequired();

                p.Property(rewards => rewards.PointAwarded)
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

            #region User

            modelBuilder.Entity<User>(us =>
            {
                us.Property(p => p.FirstName)
                    .HasMaxLength(50)
                    .IsRequired();

                us.Property(p => p.LastName)
                    .HasMaxLength(50)
                    .IsRequired();

                us.Property(u => u.Email)
                    .HasMaxLength(60)
                    .IsRequired();

                us.Property(u => u.Password)
                    .HasMaxLength(60)
                    .IsRequired();

                us.Property(u => u.Photo)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            #endregion

            #region Interest

            modelBuilder.Entity<Interest>(interest =>
            {
                interest.Property(i => i.Name)
                    .HasMaxLength(25)
                    .IsRequired();
            });

            #endregion

            #region Email

            modelBuilder.Entity<EmailConfirmationToken>(email =>
            {
                email.Property(em => em.Code)
                    .HasMaxLength(50)
                    .IsRequired();
            });

            #endregion
            
            #region Roles

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(25)
                    .IsRequired();
            });
            
            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsRequired();
            });


            #endregion
        }
    }
}