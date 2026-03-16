using Microsoft.AspNetCore.Mvc;
using FuelApp.DTOs.Associations;
using FuelApp.DTOs.Common;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;

namespace FuelApp.Controllers
{
    /// <summary>
    /// Handles CRUD operations for associations.
    /// </summary>
    /// <remarks>
    /// Base route: <c>api/Associations</c>.
    /// Supports listing, retrieving by ID, creating, updating, and deleting association records.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class AssociationsController : ControllerBase
    {
        private readonly IAssociationRepository _repo;

        public AssociationsController(IAssociationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves paged associations with optional sorting.
        /// </summary>
        [ProducesResponseType(typeof(PagedResultDto<AssociationListDto>), 200)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<AssociationListDto>>> GetAll(
            string? sortBy = null,
            string? sortDir = null,
            int page = 1,
            int pageSize = 50)
        {
            var associations = await _repo.GetAllAsync(sortBy, sortDir, page, pageSize);
            return Ok(associations);
        }

        /// <summary>
        /// Retrieves a single association by ID.
        /// </summary>
        [ProducesResponseType(typeof(Association), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Association>> GetById(int id)
        {
            var association = await _repo.GetByIdAsync(id);
            if (association == null)
            {
                return NotFound();
            }
            return Ok(association);

        }

        /// <summary>
        /// Creates a new association.
        /// </summary>
        [ProducesResponseType(typeof(Association), 201)]
        [ProducesResponseType(400)]
        [HttpPost]
        public async Task<ActionResult<Association>> Create([FromBody] Association association)
        {
            _repo.Add(association);
            await _repo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = association.AssociationID }, association);
        }

        /// <summary>
        /// Updates an existing association.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Association association)
        {
            if (id != association.AssociationID)
            {
                return BadRequest();
            }
            _repo.Update(association);
            var rowsAffected = await _repo.SaveChangesAsync();
            
            if (rowsAffected == 0)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Deletes an association by ID.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var association = await _repo.GetByIdAsync(id);
            if (association == null)
            {
                return NotFound();
            }
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}