using Microsoft.EntityFrameworkCore;

namespace GameListing.Api.Data;

public class GameListingDbContext : DbContext
{
    public GameListingDbContext(DbContextOptions<GameListingDbContext> options) : base(options)
    {
        
    }
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Player> Players => Set<Player>();
}