using FuelApp.DTOs.Common;
using FuelApp.DTOs.Teams;
using FuelApp.Models;

namespace FuelApp.Repositories.Interfaces
{
    /// <summary>
    /// Defines data access operations for teams.
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// Retrieves a paged set of teams with optional sorting.
        /// </summary>
        Task<PagedResultDto<TeamListDto>> GetAllAsync(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves a single team by identifier.
        /// </summary>
        Task<Team?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new team to the current unit of work.
        /// </summary>
        void Add(Team team);

        /// <summary>
        /// Marks an existing team as modified.
        /// </summary>
        void Update(Team team);

        /// <summary>
        /// Deletes a team by identifier.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
