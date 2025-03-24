using MetaBond.Application.Interfaces.Repository;
using MetaBond.Infrastructure.Persistence.Context;
using MetaBond.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MetaBond.Infrastructure.Persistence
{
    public static class DependecyInjection
    {

        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext
            services.AddDbContext<MetaBondContext>(postgres =>
            {
                postgres.UseNpgsql(configuration.GetConnectionString("MetaBondBackend"), b =>
                {
                    b.MigrationsAssembly("MetaBond.Infrastructure.Persistence");
                });
            });

            #endregion

            #region Repositories
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<ICommunitiesRepository, CommunitiesRepository>(); 
            services.AddTransient<IEventsRepository, EventsRepository>();
            services.AddTransient<IFriendshipRepository, FriendshipRepository>();
            services.AddTransient<IParticipationInEventRepository, ParticipationInEventRepository>();
            services.AddTransient<IPostsRepository, PostsRepository>();
            services.AddTransient<IRewardsRepository, RewardsRepository>();
            services.AddTransient<IProgressEntryRepository, ProgressEntryRepository>();
            services.AddTransient<IProgressBoardRepository, ProgressBoardRepository>();
            services.AddTransient<IEventParticipationRepository, EventParticipationRepository>();
            #endregion

        }
    }
}
