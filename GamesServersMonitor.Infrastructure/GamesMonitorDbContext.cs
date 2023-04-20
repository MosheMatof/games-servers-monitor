using GamesServersMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Numerics;


namespace GamesServersMonitor.Infrastructure
{
    public class GamesMonitorDbContext : DbContext
    {
        public GamesMonitorDbContext(DbContextOptions<GamesMonitorDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameServer> GameServers { get; set; }
        public DbSet<ServerUpdate> ServerUpdates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerUpdate>()
                .HasKey(e => new { e.ServerId, e.TimeStamp });
        }
    }
}
