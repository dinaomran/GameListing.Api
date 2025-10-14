using GameListing.Api.Results;
using GameListing.Api.DTOs.Game;

namespace GameListing.Api.Contracts
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