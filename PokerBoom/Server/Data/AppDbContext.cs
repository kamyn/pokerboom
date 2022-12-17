using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PokerBoom.Server.Entities;
using Microsoft.AspNetCore.Identity;

namespace PokerBoom.Server.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
