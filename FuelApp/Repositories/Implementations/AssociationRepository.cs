using FuelApp.Data;
using FuelApp.DTOs.Associations;
using FuelApp.DTOs.Common;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuelApp.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for association data access operations.
    /// </summary>
    public class AssociationRepository : IAssociationRepository
    {
        private readonly FuelAppContext _context;

        /// <summary>
        /// Initializes a new repository instance.
        /// </summary>
        /// <param name="context">Application database context.</param>
        public AssociationRepository(FuelAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paged set of associations with optional sorting.
        /// </summary>
        public async Task<PagedResultDto<AssociationListDto>> GetAllAsync(
            string? sortBy = null,
            string? sortDir = null,
            int page = 1,
            int pageSize = 50)
        {
            page = Math.Max(1, page);
            
            // Simple count on base entity set (no projection, no sorting)
            // Generates: SELECT COUNT(*) FROM Associations
            var totalCount = await _context.Associations.CountAsync();
            
            // Main query for paginated items with sorting
            var query = _context.Associations
                .Include(a => a.Teams)
                .AsNoTracking()
                .Select(a => new AssociationListDto
                {
                    AssociationId = a.AssociationID,
                    Name = a.Name,
                    TeamCount = a.Teams.Count
                })
                .AsQueryable();

            bool isDescending = sortDir?.ToLowerInvariant() == "desc";
            query = (sortBy ?? "id").ToLowerInvariant() switch
            {
                "name" => isDescending
                    ? query.OrderByDescending(a => a.Name).ThenByDescending(a => a.AssociationId)
                    : query.OrderBy(a => a.Name).ThenBy(a => a.AssociationId),
                "teamcount" => isDescending
                    ? query.OrderByDescending(a => a.TeamCount).ThenByDescending(a => a.AssociationId)
                    : query.OrderBy(a => a.TeamCount).ThenBy(a => a.AssociationId),
                _ => isDescending
                    ? query.OrderByDescending(a => a.AssociationId)
                    : query.OrderBy(a => a.AssociationId)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<AssociationListDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
                
        /// <summary>
        /// Retrieves a single association by identifier.
        /// </summary>
        public async Task<Association?> GetByIdAsync(int id)
        {
            var association = await _context.Associations
                .AsNoTracking()
                .Include(a => a.Teams)
                .FirstOrDefaultAsync(a => a.AssociationID == id);
            return association;
        }

        /// <summary>
        /// Adds a new association to the current unit of work.
        /// </summary>
        public void Add(Association association)
        {
            _context.Associations.Add(association);
        }

        /// <summary>
        /// Marks an existing association as modified.
        /// </summary>
        public void Update(Association association)
        {
            _context.Entry(association).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes an association by identifier.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _context.Associations.Where(a => a.AssociationID == id).ExecuteDeleteAsync();
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
