using System.Text;
using GameListing.Api.Domain;
using GameListing.Api.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using GameListing.Api.Common.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using GameListing.Api.Application.Contracts;
using GameListing.Api.Application.Services;
using GameListing.Api.Application.MappingProfiles;
using GameListing.Api.Common.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("GameListingDbConnectionString");
builder.Services.AddDbContext<GameListingDbContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddIdentityCore<ApplicationUser>(options => { })
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<GameListingDbContext>();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>() // To fix error: "Store does not implement IUserRoleStore<TUser>" we need to add IdentityRole to tell identity to consider roles as we used roles in UsersService
    .AddEntityFrameworkStores<GameListingDbContext>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("JwtSettings:Key is not configured.");
}

builder.Services.AddAuthentication(options => {
    //options.DefaultAuthenticateScheme = AuthenticationDefaults.BasicScheme;
    //options.DefaultChallengeScheme = AuthenticationDefaults.BasicScheme;
    //options.DefaultAuthenticateScheme = AuthenticationDefaults.ApiKeyScheme;
    //options.DefaultChallengeScheme = AuthenticationDefaults.ApiKeyScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer, // Who creates the token
            ValidAudience = jwtSettings.Audience, // Who receives the token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)), // This will make error if key has no value
            ClockSkew = TimeSpan.Zero // Default is 5 min so i make it zero so after 10 mins token will expire instantly so it doesn't wait more 5 mins
        };
    })
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationDefaults.ApiKeyScheme, _ => { });
builder.Services.AddAuthorization();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IApiKeyValidatorService, ApiKeyValidatorService>();

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