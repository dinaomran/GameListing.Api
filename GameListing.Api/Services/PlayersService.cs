using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Player;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GameListing.Api.Services;

public class PlayersService(GameListingDbContext context, IMapper mapper) : IPlayersService
{
    public async Task<IEnumerable<GetPlayersDto>> GetPlayersAsync()
    {
        //var players = await context.Players
        //    .Select(p => new GetPlayersDto(
        //        p.Id, p.Username, p.Email, p.CountryId
        //    ))
        //    .ToListAsync();

        var players = await context.Players
            .ProjectTo<GetPlayersDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return players;
    }

    public async Task<GetPlayerDto?> GetPlayerAsync(int id)
    {
        //var player = await context.Players
        //    .Include(p => p.Country)
        //    .Include(p => p.Games)
        //    .Where(p => p.Id == id)
        //    .Select(p => new GetPlayerDto(
        //        p.Id,
        //        p.Username,
        //        p.Email,
        //        p.Country!.Name,
        //        p.Games.Select(g => new GetGamesDto(
        //            g.Id,
        //            g.Title,
        //            g.Category,
        //            g.ReleaseDate,
        //            g.Price
        //        )).ToList()
        //    ))
        //    .FirstOrDefaultAsync();

        var player = await context.Players
            .Include(p => p.Country)
            .Include(p => p.Games)
            .Where(p => p.Id == id)
            .ProjectTo<GetPlayerDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return player ?? null;
    }

    public async Task UpdatePlayerAsync(int id, UpdatePlayerDto playerDto)
    {
        var player = await context.Players.FindAsync(id) ?? throw new KeyNotFoundException($"Player with ID {id} not found.");

        //player.Username = playerDto.Username;
        //player.Email = playerDto.Email;
        //player.CountryId = playerDto.CountryId;

        // Use AutoMapper to update the entity
        mapper.Map(playerDto, player);

        if (playerDto.GameIds != null && playerDto.GameIds.Count > 0)
        {
            var gamesExistIds = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            var missingIds = playerDto.GameIds.Except(gamesExistIds).ToList();

            if (missingIds.Count != 0)
            {
                throw new KeyNotFoundException($"The following Game Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var games = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .ToListAsync();

            player.Games = games;
        }

        //context.Entry(player).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async Task<GetPlayerDto> CreatePlayerAsync(CreatePlayerDto playerDto)
    {
        if (playerDto.CountryId != 0)
        {
            var countryExists = await context.Countries
                .AnyAsync(c => c.Id == playerDto.CountryId);

            if (!countryExists)
            {
                throw new KeyNotFoundException($"Country with Id {playerDto.CountryId} does not exist.");
            }
        }

        //var player = new Player
        //{
        //    Username = playerDto.Username,
        //    Email = playerDto.Email,
        //    CountryId = playerDto.CountryId
        //};

        var player = mapper.Map<Player>(playerDto);

        if (playerDto.GameIds != null && playerDto.GameIds.Count > 0)
        {
            var gamesExistIds = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            var missingIds = playerDto.GameIds.Except(gamesExistIds).ToList();

            if (missingIds.Count != 0)
            {
                throw new KeyNotFoundException($"The following Game Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var games = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .ToListAsync();

            player.Games = games;
        }

        context.Players.Add(player);
        await context.SaveChangesAsync();

        // Ensure Country is loaded and not null
        var countryName = player.CountryId != null
            ? (await context.Countries.Where(c => c.Id == player.CountryId).Select(c => c.Name).FirstOrDefaultAsync()) ?? string.Empty
            : string.Empty;

        //return new GetPlayerDto(
        //    player.Id,
        //    player.Username,
        //    player.Email,
        //    countryName,
        //    player.Games.Select(g => new GetGamesDto(
        //        g.Id,
        //        g.Title,
        //        g.Category,
        //        g.ReleaseDate,
        //        g.Price
        //    )).ToList()
        //);

        return mapper.Map<GetPlayerDto>(player);
    }

    public async Task DeletePlayerAsync(int id)
    {
        var player = await context.Players.FindAsync(id) ?? throw new KeyNotFoundException($"Player with ID {id} not found.");
        context.Players.Remove(player);
        await context.SaveChangesAsync();
    }

    public async Task<bool> PlayerExistsAsync(int id)
    {
        return await context.Players.AnyAsync(e => e.Id == id);
    }
}