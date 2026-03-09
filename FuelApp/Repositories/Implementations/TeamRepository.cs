using FuelApp.Data;
using FuelApp.DTOs.Common;
using FuelApp.DTOs.Teams;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuelApp.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for team data access operations.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        private readonly FuelAppContext _context;

        /// <summary>
        /// Initializes a new repository instance.
        /// </summary>
        /// <param name="context">Application database context.</param>
        public TeamRepository(FuelAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paged set of teams with optional sorting.
        /// </summary>
        public async Task<PagedResultDto<TeamListDto>> GetAllAsync(
            string? sortBy = null,
            string? sortDir = null,
            int page = 1,
            int pageSize = 50)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            // Simple count on base entity (no joins, no projection)
            var totalCount = await _context.Teams.CountAsync();

            var query = _context.Teams
                .AsNoTracking()
                .Join(
                    _context.Associations.AsNoTracking(),
                    t => t.AssociationID,
                    a => a.AssociationID,
                    (t, a) => new { Team = t, Association = a }
                )
                .Select(x => new TeamListDto
                {
                    TeamId = x.Team.TeamID,
                    Name = x.Team.Name,
                    AssociationId = x.Association.AssociationID,
                    AssociationName = x.Association.Name,
                    PlayerCount = x.Team.Players.Count
                })
                .OrderBy(t => t.TeamId);

            bool isDescending = sortDir?.ToLowerInvariant() == "desc";
            var sortedQuery = (sortBy ?? "id").ToLowerInvariant() switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(t => t.Name).ThenByDescending(t => t.TeamId)
                    : query.OrderBy(t => t.Name).ThenBy(t => t.TeamId),
                "association" => isDescending
                    ? query.OrderByDescending(t => t.AssociationName).ThenByDescending(t => t.TeamId)
                    : query.OrderBy(t => t.AssociationName).ThenBy(t => t.TeamId),
                "playercount" => isDescending
                    ? query.OrderByDescending(t => t.PlayerCount).ThenByDescending(t => t.TeamId)
                    : query.OrderBy(t => t.PlayerCount).ThenBy(t => t.TeamId),
                _ => isDescending
                    ? query.OrderByDescending(t => t.TeamId)
                    : query.OrderBy(t => t.TeamId)
            };
            var items = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<TeamListDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Retrieves a single team by identifier.
        /// </summary>
        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams
                .AsNoTracking()
                .Include(t => t.Association)
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.TeamID == id);
        }

        /// <summary>
        /// Adds a new team to the current unit of work.
        /// </summary>
        public void Add(Team team)
        {
            _context.Teams.Add(team);
        }

        /// <summary>
        /// Marks an existing team as modified.
        /// </summary>
        public void Update(Team team)
        {
            _context.Entry(team).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a team by identifier.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _context.Teams.Where(t => t.TeamID == id).ExecuteDeleteAsync();
        }

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
