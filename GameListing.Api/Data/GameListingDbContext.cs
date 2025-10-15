using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GameListing.Api.Data;

public class GameListingDbContext : IdentityDbContext<ApplicationUser>
{
    public GameListingDbContext(DbContextOptions<GameListingDbContext> options) : base(options)
    {

    }
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Player> Players => Set<Player>();
}

//// Old version before using IdentityDbContext (Security)

//public class GameListingDbContext : DbContext
//{
//    public GameListingDbContext(DbContextOptions<GameListingDbContext> options) : base(options)
//    {

//    }
//    public DbSet<Game> Games => Set<Game>();
//    public DbSet<Country> Countries => Set<Country>();
//    public DbSet<Player> Players => Set<Player>();
//}