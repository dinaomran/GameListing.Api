using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GameListing.Api.Data;

public class GameListingDbContext(DbContextOptions<GameListingDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    //public GameListingDbContext(DbContextOptions<GameListingDbContext> options) : base(options)
    //{

    //}

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApiKey>(b =>
        {
            b.HasIndex(k => k.Key).IsUnique(); // Put index on Key property and make it unique
        });
    }
}

//// Old version before using IdentityDbContext for Security

//public class GameListingDbContext : DbContext
//{
//    public GameListingDbContext(DbContextOptions<GameListingDbContext> options) : base(options)
//    {

//    }
//    public DbSet<Game> Games => Set<Game>();
//    public DbSet<Country> Countries => Set<Country>();
//    public DbSet<Player> Players => Set<Player>();
//}