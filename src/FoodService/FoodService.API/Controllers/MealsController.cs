using FoodService.Application.DTOs.Meal;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.UseCases.Queries.Meal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.API.Filters;

namespace FoodService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<FullMealDTO>> GetMealById([FromQuery] Guid mealId, Guid dayId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetMealByIdQuery(mealId, dayId, userId));

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<FullMealDTO>> CreateMeal([FromBody] CreateMealDTO createMealDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new CreateMealCommand(createMealDTO, userId));

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> UpdateMeal([FromBody] UpdateMealDTO updateMealDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateMealCommand(updateMealDTO, userId));

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> DeleteMeal(Guid mealId, [FromQuery] Guid dayId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new DeleteMealCommand(mealId, dayId, userId));

            return NoContent();
        }
    }
}