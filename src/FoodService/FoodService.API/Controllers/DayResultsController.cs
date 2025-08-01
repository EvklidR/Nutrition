using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.Application.DTOs.DayResult;
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
        /// Creates a new day result.
        /// </summary>
        /// <param name="dto">Data for creating a day result.</param>
        /// <returns>The created day result.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(DayResultResponse), 200)]
        public async Task<ActionResult<DayResultResponse>> Create([FromBody] CreateDayResultDTO dto)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new CreateDayResultCommand(dto, userId));

            return Ok(result);
        }

        /// <summary>
        /// Gets or creates a day result for a specific profile.
        /// </summary>
        /// <param name="profileId">The profile ID.</param>
        /// <returns>The retrieved or created day result.</returns>
        [HttpGet("by-profile/{profileId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(DayResultResponse), 200)]
        public async Task<ActionResult<DayResultResponse>> GetOrCreate(Guid profileId)
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
        /// <param name="startDate">The start date of the period.</param>
        /// <param name="endDate">The end date of the period.</param>
        /// <returns>A list of day results.</returns>
        [HttpGet("by-period/{profileId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(IEnumerable<DayResultResponse>), 200)]
        public async Task<ActionResult<IEnumerable<DayResultResponse>>> GetByPeriod(Guid profileId, DateOnly startDate, DateOnly endDate)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var periodParams = new PeriodParameters(startDate, endDate);

            var results = await _mediator.Send(new GetStatisticsByPeriodQuery(profileId, periodParams, userId));

            return Ok(results);
        }
    }
}
