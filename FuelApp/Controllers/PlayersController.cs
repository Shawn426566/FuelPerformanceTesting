using FuelApp.DTOs.Common;
using FuelApp.DTOs.Players;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FuelApp.Controllers
{
    /// <summary>
    /// Handles CRUD operations for players.
    /// </summary>
    /// <remarks>
    /// Base route: <c>api/Players</c>.
    /// Supports listing, retrieving by ID, creating, updating, and deleting player records.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepository _repo;

        public PlayersController(IPlayerRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves all players with pagination.
        /// </summary>
        [ProducesResponseType(typeof(PagedResultDto<PlayerListDto>), 200)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<PlayerListDto>>> GetAll(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50)
        {
            var result = await _repo.GetAllAsync(sortBy, sortDir, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single player by ID.
        /// </summary>
        [ProducesResponseType(typeof(Player), 200)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetById(int id)
        {
            var player = await _repo.GetByIdAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            return Ok(player);
        }

        /// <summary>
        /// Creates a new player.
        /// </summary>
        [ProducesResponseType(typeof(Player), 201)]
        [HttpPost]
        public async Task<ActionResult<Player>> Create([FromBody] Player player)
        {
            _repo.Add(player);
            await _repo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = player.PlayerID }, player);
        }

        /// <summary>
        /// Updates an existing player.
        /// </summary>
        [ProducesResponseType(typeof(void), 204)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Player player)
        {
            if (id != player.PlayerID)
            {
                return BadRequest();
            }
            _repo.Update(player);
            var rowsAffected = await _repo.SaveChangesAsync();
            
            if (rowsAffected == 0)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Deletes a player by ID.
        /// </summary>
        [ProducesResponseType(typeof(void), 204)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingPlayer = await _repo.GetByIdAsync(id);
            if (existingPlayer == null)
            {
                return NotFound();
            }
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of player names and IDs for dropdowns.
        /// </summary>
        [ProducesResponseType(typeof(List<PlayerNamesDto>), 200)]
        [ProducesResponseType(typeof(string), 500)]
        [HttpGet("names")]
        public async Task<ActionResult<List<PlayerNamesDto>>> GetNames()
        {
            try
            {
                var result = await _repo.GetNamesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}