using FuelApp.DTOs.Common;
using FuelApp.DTOs.StaffMembers;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FuelApp.Controllers
{
    /// <summary>
    /// Handles CRUD operations for staff members.
    /// </summary>
    /// <remarks>
    /// Base route: <c>api/StaffMembers</c>.
    /// Supports listing, retrieving by ID, creating, updating, and deleting staff member records.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class StaffMembersController : ControllerBase
    {
        private readonly IStaffMemberRepository _repo;

        public StaffMembersController(IStaffMemberRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves all staff members with pagination.
        /// </summary>
        [ProducesResponseType(typeof(PagedResultDto<StaffMemberListDto>), 200)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<StaffMemberListDto>>> GetAll(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50)
        {
            var result = await _repo.GetAllAsync(sortBy, sortDir, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single staff member by ID.
        /// </summary>
        [ProducesResponseType(typeof(StaffMember), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffMember>> GetById(int id)
        {
            var staffMember = await _repo.GetByIdAsync(id);
            if (staffMember == null)
            {
                return NotFound();
            }
            return Ok(staffMember);
        }

        /// <summary>
        /// Creates a new staff member.
        /// </summary>
        [ProducesResponseType(typeof(StaffMember), 201)]
        [HttpPost]
        public async Task<ActionResult<StaffMember>> Create([FromBody] StaffMember staffMember)
        {
            _repo.Add(staffMember);
            await _repo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = staffMember.StaffMemberID }, staffMember);
        }

        /// <summary>
        /// Updates an existing staff member.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] StaffMember staffMember)
        {
            if (id != staffMember.StaffMemberID)
            {
                return BadRequest();
            }
            _repo.Update(staffMember);
            var rowsAffected = await _repo.SaveChangesAsync();
            
            if (rowsAffected == 0)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Deletes a staff member by ID.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingStaffMember = await _repo.GetByIdAsync(id);
            if (existingStaffMember == null)
            {
                return NotFound();
            }
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}
