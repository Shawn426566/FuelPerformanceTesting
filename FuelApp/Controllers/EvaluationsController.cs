using FuelApp.DTOs.Common;
using FuelApp.DTOs.Evaluations;
using FuelApp.Models;
using FuelApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FuelApp.Controllers
{
    /// <summary>
    /// Handles CRUD operations for evaluations.
    /// </summary>
    /// <remarks>
    /// Base route: <c>api/Evaluations</c>.
    /// Supports listing, retrieving by ID, creating, updating, and deleting evaluation records.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluationsController : ControllerBase
    {
        private readonly IEvaluationRepository _repo;

        public EvaluationsController(IEvaluationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Retrieves all evaluations with pagination.
        /// </summary>
        [ProducesResponseType(typeof(PagedResultDto<EvaluationListDto>), 200)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<EvaluationListDto>>> GetAll(string? sortBy = null, string? sortDir = null, int page = 1, int pageSize = 50)
        {
            var result = await _repo.GetAllAsync(sortBy, sortDir, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a single evaluation by ID.
        /// </summary>
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Evaluation>> GetById(int id)
        {
            var evaluation = await _repo.GetByIdAsync(id);
            if (evaluation == null)
            {
                return NotFound();
            }
            return Ok(evaluation);
        }

        /// <summary>
        /// Creates a new evaluation.
        /// </summary>
        [ProducesResponseType(typeof(Evaluation), 201)]
        [HttpPost]
        public async Task<ActionResult<Evaluation>> Create([FromBody] Evaluation evaluation)
        {
            _repo.Add(evaluation);
            await _repo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = evaluation.EvaluationId }, evaluation);
        }

        /// <summary>
        /// Updates an existing evaluation.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Evaluation evaluation)
        {
            if (id != evaluation.EvaluationId)
            {
                return BadRequest();
            }
            _repo.Update(evaluation);
            var rowsAffected = await _repo.SaveChangesAsync();
            
            if (rowsAffected == 0)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        /// <summary>
        /// Deletes an evaluation by ID.
        /// </summary>
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingEvaluation = await _repo.GetByIdAsync(id);
            if (existingEvaluation == null)
            {
                return NotFound();
            }
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}
