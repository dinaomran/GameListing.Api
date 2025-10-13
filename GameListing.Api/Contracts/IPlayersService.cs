using GameListing.Api.DTOs.Player;

namespace GameListing.Api.Contracts
{
    public interface IPlayersService
    {
        Task<IEnumerable<GetPlayersDto>> GetPlayersAsync();
        Task<GetPlayerDto?> GetPlayerAsync(int id);
        Task UpdatePlayerAsync(int id, UpdatePlayerDto playerDto);
        Task<GetPlayerDto> CreatePlayerAsync(CreatePlayerDto playerDto);
        Task DeletePlayerAsync(int id);
        Task<bool> PlayerExistsAsync(int id);
    }
}