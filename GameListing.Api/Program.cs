using GameListing.Api.Data;
using GameListing.Api.Handlers;
using GameListing.Api.Services;
using GameListing.Api.Constants;
using GameListing.Api.Contracts;
using Microsoft.EntityFrameworkCore;
using GameListing.Api.MappingProfiles;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("GameListingDbConnectionString");
builder.Services.AddDbContext<GameListingDbContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddIdentityCore<ApplicationUser>(options => { })
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<GameListingDbContext>();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options => { })
    .AddEntityFrameworkStores<GameListingDbContext>();
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = AuthenticationDefaults.BasicScheme;
    options.DefaultChallengeScheme = AuthenticationDefaults.BasicScheme;
})
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { });
builder.Services.AddAuthorization();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<PlayerMappingProfile>();
    cfg.AddProfile<CountryMappingProfile>();
    cfg.AddProfile<GameMappingProfile>();
});

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();