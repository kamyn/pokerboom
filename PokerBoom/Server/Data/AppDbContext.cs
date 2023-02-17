using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PokerBoom.Server.Entities;
using Microsoft.AspNetCore.Identity;

namespace PokerBoom.Server.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Board> Boards { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            base.Database.EnsureCreated();
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Game>().HasOne<Table>(g => g.Table).WithMany(t => t.Games);
            builder.Entity<Game>().HasMany<Player>(g => g.Players).WithOne(p => p.Game);
            builder.Entity<Game>().HasOne<Board>(g => g.Board).WithOne(b => b.Game);
            builder.Entity<Game>().HasMany<Bet>(g => g.Bets).WithOne(b => b.Game);

            builder.Entity<Player>().HasOne<ApplicationUser>(p => p.User).WithMany(u => u.Players);
            builder.Entity<Player>().HasMany<Bet>(p => p.Bets).WithOne(u => u.Player);

            builder.Entity<Table>().HasData(new Table { Id = 1, Name = "стол #1", SmallBlind = 10, Players = 0 });

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "User", 
                                                                      NormalizedName = "USER", 
                                                                      Id = Guid.NewGuid().ToString(), 
                                                                      ConcurrencyStamp = Guid.NewGuid().ToString() });

            builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Administrator", 
                                                                      NormalizedName = "ADMINISTRATOR", 
                                                                      Id = Guid.NewGuid().ToString(), 
                                                                      ConcurrencyStamp = Guid.NewGuid().ToString() });
        }
    }
}
