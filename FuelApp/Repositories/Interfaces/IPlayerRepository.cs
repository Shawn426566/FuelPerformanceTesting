using FuelApp.DTOs.Common;
using FuelApp.DTOs.Players;
using FuelApp.Models;

namespace FuelApp.Repositories.Interfaces
{
    /// <summary>
    /// Defines data access operations for players.
    /// </summary>
    public interface IPlayerRepository
    {
        /// <summary>
        /// Retrieves a paged set of players with optional sorting.
        /// </summary>
        Task<PagedResultDto<PlayerListDto>> GetAllAsync(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves a single player by identifier.
        /// </summary>
        Task<Player?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new player to the current unit of work.
        /// </summary>
        void Add(Player player);

        /// <summary>
        /// Marks an existing player as modified.
        /// </summary>
        void Update(Player player);

        /// <summary>
        /// Deletes a player by identifier.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// List of all players with only their first and last names.
        /// </summary>
        Task<List<PlayerNamesDto>> GetNamesAsync();
    }
}