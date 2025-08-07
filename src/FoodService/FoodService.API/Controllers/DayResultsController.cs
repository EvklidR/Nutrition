using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.API.Filters;
using FoodService.Application.DTOs.DayResult.Requests;
using FoodService.Application.DTOs.DayResult.Responses;
using FoodService.Domain.Interfaces.Repositories.Models;

namespace FoodService.API.Controllers
{
    /// <summary>
    /// Controller for managing profile day results.
    /// </summary>
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class DayResultsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DayResultsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets or creates a day result for a specific profile.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <returns>The retrieved or created day result.</returns>
        [HttpGet("get-or-create/{profileId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(DayResultResponse), 200)]
        public async Task<ActionResult<DayResultResponse>> GetOrCreate([FromRoute] Guid profileId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetOrCreateDayResultCommand(profileId, userId));

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing day result.
        /// </summary>
        /// <param name="dto">Updated day result data.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Update([FromBody] UpdateDayResultDTO dto)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateDayResultCommand(dto, userId));

            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of day results within a specific period.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="periodParameters">The start and end date of the period.</param>
        /// <param name="paginatedParameters">Page number and size.</param>
        /// <returns>A list of day results.</returns>
        [HttpGet("{profileId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(IEnumerable<DayResultResponse>), 200)]
        public async Task<ActionResult<IEnumerable<DayResultResponse>>> GetDayResults(
            Guid profileId,
            [FromQuery] PeriodParameters periodParameters,
            [FromQuery] PaginatedParameters paginatedParameters)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var results = await _mediator.Send(new GetAllDayResultsQuery(profileId, userId, paginatedParameters, periodParameters));

            return Ok(results);
        }

        /// <summary>
        /// Retrieves a day results by ID.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <param name="dayResultId">The day result ID.</param>
        /// <returns>Requested day result.</returns>
        [HttpGet("{profileId}/{dayResultId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(IEnumerable<DayResultResponse>), 200)]
        public async Task<ActionResult<IEnumerable<DayResultResponse>>> GetDayResult(
            [FromRoute] Guid profileId,
            [FromRoute] Guid dayResultId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var results = await _mediator.Send(new GetDayResultByIdQuery(profileId, userId, dayResultId));

            return Ok(results);
        }
    }
}
