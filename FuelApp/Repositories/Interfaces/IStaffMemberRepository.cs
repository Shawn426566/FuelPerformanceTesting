using FuelApp.DTOs.Common;
using FuelApp.DTOs.StaffMembers;
using FuelApp.Models;

namespace FuelApp.Repositories.Interfaces
{
    /// <summary>
    /// Defines data access operations for staff members.
    /// </summary>
    public interface IStaffMemberRepository
    {
        /// <summary>
        /// Retrieves a paged set of staff members with optional sorting.
        /// </summary>
        Task<PagedResultDto<StaffMemberListDto>> GetAllAsync(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50);

        /// <summary>
        /// Retrieves a single staff member by identifier.
        /// </summary>
        Task<StaffMember?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new staff member to the current unit of work.
        /// </summary>
        void Add(StaffMember staffMember);

        /// <summary>
        /// Marks an existing staff member as modified.
        /// </summary>
        void Update(StaffMember staffMember);

        /// <summary>
        /// Deletes a staff member by identifier.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Persists pending changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
