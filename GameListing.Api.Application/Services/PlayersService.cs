using AutoMapper;
using GameListing.Api.Domain;
using Microsoft.EntityFrameworkCore;
using GameListing.Api.Common.Results;
using AutoMapper.QueryableExtensions;
using GameListing.Api.Common.Constants;
using GameListing.Api.Application.Contracts;
using GameListing.Api.Application.DTOs.Player;

namespace GameListing.Api.Application.Services;

public class PlayersService(GameListingDbContext context, IMapper mapper) : IPlayersService
{
    public async Task<Result<IEnumerable<GetPlayersDto>>> GetPlayersAsync()
    {
        try
        {
            //var players = await context.Players
            //    .Select(p => new GetPlayersDto(
            //        p.Id, p.Username, p.Email, p.CountryId
            //    ))
            //    .ToListAsync();

            var players = await context.Players
                .ProjectTo<GetPlayersDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<GetPlayersDto>>.Success(players);
        }
        catch(Exception ex)
        {
            return Result<IEnumerable<GetPlayersDto>>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result<GetPlayerDto>> GetPlayerAsync(int id)
    {
        try
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
                .FirstOrDefaultAsync(p => p.Id == id);

            var playerDto = mapper.Map<GetPlayerDto>(player);

            return player == null ? Result<GetPlayerDto>.NotFound(new Error(ErrorCodes.NotFound, $"Player with ID {id} not found.")) : Result<GetPlayerDto>.Success(playerDto);
        }
        catch(Exception ex)
        {
            return Result<GetPlayerDto>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result> UpdatePlayerAsync(int id, UpdatePlayerDto playerDto)
    {
        try
        {
            if (id != playerDto.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, $"The ID in the URL ({id}) does not match the ID in the body ({playerDto.Id})."));
            }

            var player = await context.Players.FindAsync(id);
        
            if (player == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Player with ID {id} not found."));
            }

            var playerDuplicateEmail = await PlayerExistsAsync(playerDto.Email);

            if (playerDuplicateEmail == true)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, $"Player with Email {playerDto.Email} already exists."));
            }

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
                    return Result.NotFound(new Error(ErrorCodes.NotFound, $"The following Game Ids do not exist: {string.Join(", ", missingIds)}"));
                }

                var games = await context.Games
                    .Where(g => playerDto.GameIds.Contains(g.Id))
                    .ToListAsync();

                player.Games = games;
            }

            //context.Entry(player).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch(Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result<GetPlayerDto>> CreatePlayerAsync(CreatePlayerDto playerDto)
    {
        try
        {
            var emailExist = await PlayerExistsAsync(playerDto.Email);

            if (emailExist == true)
            {
                return Result<GetPlayerDto>.Failure(new Error(ErrorCodes.Conflict, $"Player with Email {playerDto.Email} already exists."));
            }

            if (playerDto.CountryId != 0)
            {
                var countryExists = await context.Countries
                    .AnyAsync(c => c.Id == playerDto.CountryId);

                if (!countryExists)
                {
                    return Result<GetPlayerDto>.NotFound(new Error(ErrorCodes.NotFound, $"Country with Id {playerDto.CountryId} does not exist."));
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
                    return Result<GetPlayerDto>.NotFound(new Error(ErrorCodes.NotFound, $"The following Game Ids do not exist: {string.Join(", ", missingIds)}"));
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
                ? await context.Countries.Where(c => c.Id == player.CountryId).Select(c => c.Name).FirstOrDefaultAsync() ?? string.Empty
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

            return Result<GetPlayerDto>.Success(mapper.Map<GetPlayerDto>(player));
        }
        catch(Exception ex)
        {
            return Result<GetPlayerDto>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result> DeletePlayerAsync(int id)
    {
        try
        {
            var player = await context.Players.FindAsync(id);
        
            if (player == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Player with ID {id} not found."));
            }

            context.Players.Remove(player);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch(Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<bool> PlayerExistsAsync(int id)
    {
        return await context.Players.AnyAsync(e => e.Id == id);
    }
    public async Task<bool> PlayerExistsAsync(string email)
    {
        return await context.Players.AnyAsync(e => e.Email.ToLower().Trim() == email.ToLower().Trim());
    }
}