using FuelApp.Data;
using FuelApp.DTOs.Common;
using FuelApp.DTOs.StaffMembers;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuelApp.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for staff member data access operations.
    /// </summary>
    public class StaffMemberRepository : IStaffMemberRepository
    {
        private readonly FuelAppContext _context;

        /// <summary>
        /// Initializes a new repository instance.
        /// </summary>
        /// <param name="context">Application database context.</param>
        public StaffMemberRepository(FuelAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paged set of staff members with optional sorting.
        /// </summary>
        public async Task<PagedResultDto<StaffMemberListDto>> GetAllAsync(
            string? sortBy = null,
            string? sortDir = null,
            int page = 1,
            int pageSize = 50)
        {
            page = Math.Max(1, page);

            // Simple count on base entity (no joins, no projection)
            var totalCount = await _context.StaffMembers.CountAsync();

            var query = _context.StaffMembers
                .AsNoTracking()
                .Include(s => s.Team)
                .Include(s => s.Evaluations)
                .Select(s => new StaffMemberListDto
                {
                    StaffMemberId = s.StaffMemberID,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Role = s.Role.ToString(),
                    Email = s.Email ?? string.Empty,
                    TeamId = s.TeamID ?? null,
                    TeamName = s.Team != null ? s.Team.Name : string.Empty,
                    EvaluationCount = s.Evaluations.Count
                })
                .AsQueryable();

            bool isDescending = sortDir?.ToLowerInvariant() == "desc";
            var sortedQuery = (sortBy ?? "lastname").ToLowerInvariant() switch
            {
                "firstname" => isDescending
                    ? query.OrderByDescending(s => s.FirstName).ThenByDescending(s => s.StaffMemberId)
                    : query.OrderBy(s => s.FirstName).ThenBy(s => s.StaffMemberId),
                "lastname" => isDescending
                    ? query.OrderByDescending(s => s.LastName).ThenByDescending(s => s.StaffMemberId)
                    : query.OrderBy(s => s.LastName).ThenBy(s => s.StaffMemberId),
                "role" => isDescending
                    ? query.OrderByDescending(s => s.Role).ThenByDescending(s => s.StaffMemberId)
                    : query.OrderBy(s => s.Role).ThenBy(s => s.StaffMemberId),
                _ => isDescending
                    ? query.OrderByDescending(s => s.StaffMemberId)
                    : query.OrderBy(s => s.StaffMemberId)
            };
            var items = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<StaffMemberListDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Retrieves a single staff member by identifier.
        /// </summary>
        public async Task<StaffMember?> GetByIdAsync(int id)
        {
            return await _context.StaffMembers
                .AsNoTracking()
                .Include(s => s.Team)
                .Include(s => s.Evaluations)
                .FirstOrDefaultAsync(s => s.StaffMemberID == id);
        }

        /// <summary>
        /// Adds a new staff member to the current unit of work.
        /// </summary>
        public void Add(StaffMember staffMember)
        {
            _context.StaffMembers.Add(staffMember);
        }

        /// <summary>
        /// Marks an existing staff member as modified.
        /// </summary>
        public void Update(StaffMember staffMember)
        {
            _context.Entry(staffMember).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a staff member by identifier.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _context.StaffMembers.Where(s => s.StaffMemberID == id).ExecuteDeleteAsync();
        }

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a set of staff member names sorted by last name then first name. 
        /// </summary>
        public async Task<List<StaffMemberNamesDto>> GetNamesAsync()
        {
            var names = await _context.StaffMembers
                .AsNoTracking()
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Select(s => new StaffMemberNamesDto
                {
                    StaffMemberId = s.StaffMemberID,
                    FirstName = s.FirstName,
                    LastName = s.LastName
                })
                .ToListAsync();

            return names;
        }
    }
}
