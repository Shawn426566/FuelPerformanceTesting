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
            pageSize = Math.Clamp(pageSize, 1, 200);

            // Simple count on base entity (no joins, no projection)
            var totalCount = await _context.StaffMembers.CountAsync();

            var query = _context.StaffMembers
                .AsNoTracking()
                .GroupJoin(
                    _context.Teams.AsNoTracking(),
                    s => s.TeamID,
                    t => t.TeamID,
                    (s, teams) => new { Staff = s, Team = teams.FirstOrDefault() }
                )
                .Select(x => new StaffMemberListDto
                {
                    StaffMemberId = x.Staff.StaffMemberID,
                    FirstName = x.Staff.FirstName,
                    LastName = x.Staff.LastName,
                    Role = x.Staff.Role.ToString(),
                    Email = x.Staff.Email,
                    TeamId = x.Staff.TeamID,
                    TeamName = x.Team != null ? x.Team.Name : null,
                    EvaluationCount = x.Staff.Evaluations.Count
                })
                .OrderBy(s => s.StaffMemberId);

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
    }
}
