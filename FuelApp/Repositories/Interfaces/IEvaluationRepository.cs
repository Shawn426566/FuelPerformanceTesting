using FuelApp.DTOs.Common;
using FuelApp.DTOs.Evaluations;
using FuelApp.Models;

namespace FuelApp.Repositories.Interfaces
{
    /// <summary>
    /// Defines data access operations for evaluations.
    /// </summary>
    public interface IEvaluationRepository
    {
        /// <summary>
        /// Retrieves a paged set of evaluations with optional sorting.
        /// </summary>
        Task<PagedResultDto<EvaluationListDto>> GetAllAsync(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves a single evaluation by identifier.
        /// </summary>
        Task<Evaluation?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new evaluation to the current unit of work.
        /// </summary>
        void Add(Evaluation evaluation);

        /// <summary>
        /// Marks an existing evaluation as modified.
        /// </summary>
        void Update(Evaluation evaluation);

        /// <summary>
        /// Deletes an evaluation by identifier.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
