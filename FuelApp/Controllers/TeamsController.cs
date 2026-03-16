using FuelApp.DTOs.Common;
using FuelApp.DTOs.Teams;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FuelApp.Controllers
{
    /// <summary>
    /// Handles CRUD operations for teams.
    /// </summary>
    /// <remarks>
    /// Base route: <c>api/teams</c>.
    /// Supports listing, retrieving by ID, creating, updating, and deleting team records.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamRepository _repo;

        public TeamsController(ITeamRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves all teams with pagination.
        /// </summary>
        [ProducesResponseType(typeof(PagedResultDto<TeamListDto>), 200)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TeamListDto>>> GetAll(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50)
        {
            var result = await _repo.GetAllAsync(sortBy, sortDir, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single team by ID.
        /// </summary>
        [ProducesResponseType(typeof(Team), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetById(int id)
        {
            var team = await _repo.GetByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        /// <summary>
        /// Creates a new team.
        /// </summary>
        [ProducesResponseType(typeof(Team), 201)]
        [HttpPost]
        public async Task<ActionResult<Team>> Create([FromBody] Team team)
        {
            _repo.Add(team);
            await _repo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = team.TeamID }, team);
        }

        /// <summary>
        /// Updates an existing team.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Team team)
        {
            if (id != team.TeamID)
            {
                return BadRequest();
            }
            _repo.Update(team);
            var rowsAffected = await _repo.SaveChangesAsync();
            
            if (rowsAffected == 0)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Deletes a team by ID.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingTeam = await _repo.GetByIdAsync(id);
            if (existingTeam == null)
            {
                return NotFound();
            }
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}
