using FuelApp.Data;
using FuelApp.DTOs.Common;
using FuelApp.DTOs.Evaluations;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FuelApp.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for evaluation data access operations.
    /// </summary>
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly FuelAppContext _context;

        /// <summary>
        /// Initializes a new repository instance.
        /// </summary>
        /// <param name="context">Application database context.</param>
        public EvaluationRepository(FuelAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paged set of evaluations with optional sorting.
        /// </summary>
        public async Task<PagedResultDto<EvaluationListDto>> GetAllAsync(
            string? sortBy = null,
            string? sortDir = null,
            int page = 1,
            int pageSize = 50)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            // Simple count on base entity (no joins, no projection)
            var totalCount = await _context.Evaluations.CountAsync();

            var query = from e in _context.Evaluations.AsNoTracking()
                        join p in _context.Players.AsNoTracking()
                            on e.PlayerId equals p.PlayerID
                        join s in _context.StaffMembers.AsNoTracking()
                            on e.StaffMemberId equals s.StaffMemberID into staffJoin
                        from staff in staffJoin.DefaultIfEmpty()
                        select new EvaluationListDto
                        {
                            EvaluationId = e.EvaluationId,
                            Date = e.Date,
                            Summary = e.Summary,
                            PlayerId = e.PlayerId,
                            PlayerName = p.FirstName + " " + p.LastName,
                            StaffMemberId = e.StaffMemberId,
                            StaffMemberName = staff != null
                                ? staff.FirstName + " " + staff.LastName
                                : null
                        };

            bool isDescending = sortDir?.ToLowerInvariant() == "desc";
            var sortedQuery = (sortBy ?? "date").ToLowerInvariant() switch
            {
                "date" => isDescending
                    ? query.OrderByDescending(e => e.Date).ThenByDescending(e => e.EvaluationId)
                    : query.OrderBy(e => e.Date).ThenBy(e => e.EvaluationId),
                "player" => isDescending
                    ? query.OrderByDescending(e => e.PlayerName).ThenByDescending(e => e.EvaluationId)
                    : query.OrderBy(e => e.PlayerName).ThenBy(e => e.EvaluationId),
                "evaluator" => isDescending
                    ? query.OrderByDescending(e => e.StaffMemberName).ThenByDescending(e => e.EvaluationId)
                    : query.OrderBy(e => e.StaffMemberName).ThenBy(e => e.EvaluationId),
                _ => isDescending
                    ? query.OrderByDescending(e => e.EvaluationId)
                    : query.OrderBy(e => e.EvaluationId)
            };
            var items = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<EvaluationListDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Retrieves a single evaluation by identifier.
        /// </summary>
        public async Task<Evaluation?> GetByIdAsync(int id)
        {
            return await _context.Evaluations
                .AsNoTracking()
                .Include(e => e.Player)
                .Include(e => e.StaffMember)
                .FirstOrDefaultAsync(e => e.EvaluationId == id);
        }

        /// <summary>
        /// Adds a new evaluation to the current unit of work.
        /// </summary>
        public void Add(Evaluation evaluation)
        {
            _context.Evaluations.Add(evaluation);
        }

        /// <summary>
        /// Marks an existing evaluation as modified.
        /// </summary>
        public void Update(Evaluation evaluation)
        {
            _context.Entry(evaluation).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes an evaluation by identifier.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            await _context.Evaluations.Where(e => e.EvaluationId == id).ExecuteDeleteAsync();
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
