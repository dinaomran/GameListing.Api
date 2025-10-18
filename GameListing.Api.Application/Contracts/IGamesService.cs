using GameListing.Api.Common.Results;
using GameListing.Api.Application.DTOs.Game;

namespace GameListing.Api.Application.Contracts
{
    public interface IGamesService
    {
        Task<Result<IEnumerable<GetGamesDto>>> GetGamesAsync();
        Task<Result<GetGameDto>> GetGameAsync(int id);
        Task<Result> UpdateGameAsync(int id, UpdateGameDto gameDto);
        Task<Result<GetGameDto>> CreateGameAsync(CreateGameDto gameDto);
        Task<Result> DeleteGameAsync(int id);
        Task<bool> GameExistsAsync(int id);
        Task<bool> GameExistsAsync(string title);
    }
}