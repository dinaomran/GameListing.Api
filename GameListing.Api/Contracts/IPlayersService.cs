using GameListing.Api.Results;
using GameListing.Api.DTOs.Player;

namespace GameListing.Api.Contracts
{
    public interface IPlayersService
    {
        Task<Result<IEnumerable<GetPlayersDto>>> GetPlayersAsync();
        Task<Result<GetPlayerDto>> GetPlayerAsync(int id);
        Task<Result> UpdatePlayerAsync(int id, UpdatePlayerDto playerDto);
        Task<Result<GetPlayerDto>> CreatePlayerAsync(CreatePlayerDto playerDto);
        Task<Result> DeletePlayerAsync(int id);
        Task<bool> PlayerExistsAsync(int id);
        Task<bool> PlayerExistsAsync(string email);
    }
}