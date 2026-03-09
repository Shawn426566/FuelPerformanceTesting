using FuelApp.Models;
using FuelApp.DTOs.Associations;
using FuelApp.DTOs.Common;

namespace FuelApp.Repositories.Interfaces
{
    /// <summary>
    /// Defines data access operations for associations.
    /// </summary>
    public interface IAssociationRepository
    {
        /// <summary>
        /// Retrieves a paged set of associations with optional sorting.
        /// </summary>
        Task<PagedResultDto<AssociationListDto>> GetAllAsync(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves a single association by identifier.
        /// </summary>
        Task<Association?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new association to the current unit of work.
        /// </summary>
        void Add(Association association);

        /// <summary>
        /// Marks an existing association as modified.
        /// </summary>
        void Update(Association association);

        /// <summary>
        /// Deletes an association by identifier.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}