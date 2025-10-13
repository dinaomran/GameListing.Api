using GameListing.Api.DTOs.Game;

namespace GameListing.Api.Contracts
{
    public interface IGamesService
    {
        Task<IEnumerable<GetGamesDto>> GetGamesAsync();
        Task<GetGameDto?> GetGameAsync(int id);
        Task UpdateGameAsync(int id, UpdateGameDto gameDto);
        Task<GetGameDto> CreateGameAsync(CreateGameDto gameDto);
        Task DeleteGameAsync(int id);
        Task<bool> GameExistsAsync(int id);
    }
}