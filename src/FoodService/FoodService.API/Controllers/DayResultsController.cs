using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.Application.DTOs.DayResult;
using FoodService.Application.UseCases.Commands.DayResult;
using FoodService.Application.UseCases.Queries.DayResult;
using FoodService.API.Filters;

namespace FoodService.API.Controllers
{
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

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> Create([FromBody] CreateDayResultDTO dto)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new CreateDayResultCommand(dto, userId));

            return Ok(result);
        }

        [HttpDelete("{dayResultId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> Delete(Guid dayResultId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new DeleteDayResultCommand(dayResultId, userId));

            return NoContent();
        }

        [HttpGet("profile/{profileId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> GetOrCreate(Guid profileId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetOrCreateDayResultCommand(profileId, userId));

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> Update([FromBody] UpdateDayResultDTO dto)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateDayResultCommand(dto, userId));

            return NoContent();
        }

        [HttpGet("profile/{profileId}/period")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> GetByPeriod(Guid profileId, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var results = await _mediator.Send(new GetDayResultsByPeriodQuery(profileId, startDate, endDate, userId));

            return Ok(results);
        }
    }
}
