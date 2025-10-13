using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Game;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GameListing.Api.Services;

public class GamesService(GameListingDbContext context, IMapper mapper) : IGamesService
{
    public async Task<IEnumerable<GetGamesDto>> GetGamesAsync()
    {
        //var games = await context.Games
        //    .Select(g => new GetGamesDto(g.Id, g.Title, g.Category, g.ReleaseDate, g.Price))
        //    .ToListAsync();

        var games = await context.Games
            .ProjectTo<GetGamesDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return games;
    }

    public async Task<GetGameDto?> GetGameAsync(int id)
    {
        //var game = await context.Games
        //    .Where(g => g.Id == id)
        //    .Select(g => new GetGameDto(
        //        g.Id,
        //        g.Title,
        //        g.Category,
        //        g.ReleaseDate,
        //        g.Price,
        //        g.Players.Select(p => new GetPlayersDto(
        //            p.Id,
        //            p.Username,
        //            p.Email,
        //            p.CountryId
        //        )).ToList()
        //    ))
        //    .FirstOrDefaultAsync();

        var game = await context.Games
            .Where(g => g.Id == id)
            .ProjectTo<GetGameDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return game ?? null;
    }

    public async Task UpdateGameAsync(int id, UpdateGameDto gameDto)
    {
        var game = await context.Games.FindAsync(id) ?? throw new KeyNotFoundException($"Game with ID {id} not found.");

        //game.Title = gameDto.Title;
        //game.Category = gameDto.Category;
        //game.ReleaseDate = gameDto.ReleaseDate;
        //game.Price = gameDto.Price;

        // Use AutoMapper to update the entity
        mapper.Map(gameDto, game);

        if (gameDto.PlayerIds != null && gameDto.PlayerIds.Count > 0)
        {
            var playersExistIds = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = gameDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                throw new KeyNotFoundException($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var players = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            game.Players = players;
        }

        //context.Entry(game).State = EntityState.Modified;

        await context.SaveChangesAsync();
    }

    public async Task<GetGameDto> CreateGameAsync(CreateGameDto gameDto)
    {
        //var game = new Game
        //{
        //    Title = gameDto.Title,
        //    Category = gameDto.Category,
        //    ReleaseDate = gameDto.ReleaseDate,
        //    Price = gameDto.Price
        //};

        var game = mapper.Map<Game>(gameDto);

        if (gameDto.PlayerIds != null && gameDto.PlayerIds.Count > 0)
        {
            var playersExistIds = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = gameDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                throw new KeyNotFoundException($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var players = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            game.Players = players;
        }

        context.Games.Add(game);
        await context.SaveChangesAsync();

        //var createdGame = new GetGameDto(
        //    game.Id,
        //    game.Title,
        //    game.Category,
        //    game.ReleaseDate,
        //    game.Price,
        //    game.Players.Select(p => new GetPlayersDto(
        //        p.Id,
        //        p.Username,
        //        p.Email,
        //        p.CountryId
        //    )).ToList()
        //);
        //return createdGame;

        return mapper.Map<GetGameDto>(game);
    }

    public async Task DeleteGameAsync(int id)
    {
        var game = await context.Games.FindAsync(id) ?? throw new KeyNotFoundException($"Game with ID {id} not found.");
        context.Games.Remove(game);
        await context.SaveChangesAsync();
    }

    public async Task<bool> GameExistsAsync(int id)
    {
        return await context.Games.AnyAsync(e => e.Id == id);
    }
}