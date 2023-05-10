using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GamesServersMonitor.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static void ConfigureMyDbContext(this DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public static void AddMyDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GamesMonitorDbContext>(options =>
            {
                options.ConfigureMyDbContext(connectionString);
            });
        }
    }
}
