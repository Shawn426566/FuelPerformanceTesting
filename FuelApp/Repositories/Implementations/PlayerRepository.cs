using FuelApp.Data;
using FuelApp.DTOs.Common;
using FuelApp.DTOs.Players;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuelApp.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for player data access operations.
    /// </summary>
    public class PlayerRepository : IPlayerRepository
    {
        private readonly FuelAppContext _context;

        /// <summary>
        /// Initializes a new repository instance.
        /// </summary>
        /// <param name="context">Application database context.</param>
        public PlayerRepository(FuelAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paged set of players with optional sorting.
        /// </summary>
        public async Task<PagedResultDto<PlayerListDto>> GetAllAsync(
            string? sortBy = null,
            string? sortDir = null,
            int page = 1,
            int pageSize = 50)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            // Simple count on base entity (no joins, no projection)
            var totalCount = await _context.Players.CountAsync();

            var query = _context.Players
                .AsNoTracking()
                .Join(
                    _context.Teams.AsNoTracking(),
                    p => p.TeamID,
                    t => t.TeamID,
                    (p, t) => new { Player = p, Team = t }
                )
                .LeftJoin(
                    _context.Associations.AsNoTracking(),
                    x => x.Player.AssociationID,
                    a => a.AssociationID,
                    (x, a) => new { x.Player, x.Team, Association = a }
                )
                .Select(x => new PlayerListDto
                {
                    PlayerId = x.Player.PlayerID,
                    FirstName = x.Player.FirstName,
                    LastName = x.Player.LastName,
                    TeamId = x.Player.TeamID,
                    TeamName = x.Team != null ? x.Team.Name : null,
                    AssociationId = x.Player.AssociationID,
                    AssociationName = x.Association != null ? x.Association.Name : null,
                    Position = x.Player.Position != null ? x.Player.Position.ToString() : null,
                    JerseyNumber = x.Player.JerseyNumber
                });

            bool isDescending = sortDir?.ToLowerInvariant() == "desc";
            var sortedQuery = (sortBy ?? "lastname").ToLowerInvariant() switch
            {
                "firstname" => isDescending
                    ? query.OrderByDescending(p => p.FirstName).ThenByDescending(p => p.PlayerId)
                    : query.OrderBy(p => p.FirstName).ThenBy(p => p.PlayerId),
                "lastname" => isDescending
                    ? query.OrderByDescending(p => p.LastName).ThenByDescending(p => p.PlayerId)
                    : query.OrderBy(p => p.LastName).ThenBy(p => p.PlayerId),
                "position" => isDescending
                    ? query.OrderByDescending(p => p.Position).ThenByDescending(p => p.PlayerId)
                    : query.OrderBy(p => p.Position).ThenBy(p => p.PlayerId),
                "jerseynumber" => isDescending
                    ? query.OrderByDescending(p => p.JerseyNumber).ThenByDescending(p => p.PlayerId)
                    : query.OrderBy(p => p.JerseyNumber).ThenBy(p => p.PlayerId),
                _ => isDescending
                    ? query.OrderByDescending(p => p.PlayerId)
                    : query.OrderBy(p => p.PlayerId)
            };
            var items = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<PlayerListDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Retrieves a single player by identifier.
        /// </summary>
        public async Task<Player?> GetByIdAsync(int id)
        {
            var player = await _context.Players
                .AsNoTracking()
                .Include(p => p.Team)
                .Include(p => p.Association)
                .Include(p => p.Evaluations)
                .FirstOrDefaultAsync(p => p.PlayerID == id);
            return player;
        }

        /// <summary>
        /// Adds a new player to the current unit of work.
        /// </summary>
        public void Add(Player player)
        {
            _context.Players.Add(player);
        }

        /// <summary>
        /// Marks an existing player as modified.
        /// </summary>
        public void Update(Player player)
        {
            _context.Entry(player).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a player by identifier.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _context.Players.Where(p => p.PlayerID == id).ExecuteDeleteAsync();
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