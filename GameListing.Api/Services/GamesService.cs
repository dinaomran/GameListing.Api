using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.Results;
using GameListing.Api.Constants;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Game;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GameListing.Api.Services;

public class GamesService(GameListingDbContext context, IMapper mapper) : IGamesService
{
    public async Task<Result<IEnumerable<GetGamesDto>>> GetGamesAsync()
    {
        try
        {
            //var games = await context.Games
            //    .Select(g => new GetGamesDto(g.Id, g.Title, g.Category, g.ReleaseDate, g.Price))
            //    .ToListAsync();

            var games = await context.Games
                .ProjectTo<GetGamesDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<GetGamesDto>>.Success(games);
        }
        catch(Exception ex)
        {
            return Result<IEnumerable<GetGamesDto>>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result<GetGameDto>> GetGameAsync(int id)
    {
        try
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

            return game == null ? Result<GetGameDto>.NotFound(new Error(ErrorCodes.NotFound, $"Game with ID {id} not found.")) : Result<GetGameDto>.Success(game);
        }
        catch(Exception ex)
        {
            return Result<GetGameDto>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result> UpdateGameAsync(int id, UpdateGameDto gameDto)
    {
        try
        {
            if (id != gameDto.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, $"The ID in the URL ({id}) does not match the ID in the body ({gameDto.Id})."));
            }

            var game = await context.Games.FindAsync(id);

            if (game == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Game with ID {id} not found."));
            }

            var gameDuplicateTitle = await GameExistsAsync(gameDto.Title);

            if (gameDuplicateTitle == true)
            {
                return Result.NotFound(new Error(ErrorCodes.Conflict, $"Game with Title {gameDto.Title} already exists."));
            }

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
                    return Result.NotFound(new Error(ErrorCodes.NotFound, $"The following Player Ids do not exist: {string.Join(", ", missingIds)}"));
                }

                var players = await context.Players
                    .Where(p => gameDto.PlayerIds.Contains(p.Id))
                    .ToListAsync();

                game.Players = players;
            }

            //context.Entry(game).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch(Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result<GetGameDto>> CreateGameAsync(CreateGameDto gameDto)
    {
        try
        {
            var gameExist = await GameExistsAsync(gameDto.Title);

            if (gameExist == true)
            {
                return Result<GetGameDto>.Failure(new Error(ErrorCodes.Conflict, $"Game with Title {gameDto.Title} already exists."));
            }

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
                    return Result<GetGameDto>.NotFound(new Error(ErrorCodes.NotFound, $"The following Player Ids do not exist: {string.Join(", ", missingIds)}"));
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

            return Result<GetGameDto>.Success(mapper.Map<GetGameDto>(game));
        }
        catch(Exception ex)
        {
            return Result<GetGameDto>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result> DeleteGameAsync(int id)
    {
        try
        {
            var game = await context.Games.FindAsync(id);

            if (game == null) 
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Game with ID {id} not found."));
            }

            context.Games.Remove(game);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch(Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<bool> GameExistsAsync(int id)
    {
        return await context.Games.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> GameExistsAsync(string title)
    {
        return await context.Games.AnyAsync(e => e.Title.ToLower().Trim() == title.ToLower().Trim());
    }
}