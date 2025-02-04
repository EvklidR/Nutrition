using FoodService.Application.DTOs.Meal;
using FoodService.Application.UseCases.Commands.Meal;
using FoodService.Application.UseCases.Queries.Meal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.API.Filters;

namespace FoodService.API.Controllers
{
    /// <summary>
    /// Controller for managing meals.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MealsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a meal by its ID.
        /// </summary>
        /// <param name="mealId">The meal ID.</param>
        /// <param name="dayId">The day ID associated with the meal.</param>
        /// <returns>The requested meal.</returns>
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<FullMealDTO>> GetMealById([FromQuery] Guid mealId, Guid dayId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetMealByIdQuery(mealId, dayId, userId));

            return Ok(result);
        }

        /// <summary>
        /// Creates a new meal.
        /// </summary>
        /// <param name="createMealDTO">The meal data.</param>
        /// <returns>The created meal.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<FullMealDTO>> CreateMeal([FromBody] CreateMealDTO createMealDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new CreateMealCommand(createMealDTO, userId));

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing meal.
        /// </summary>
        /// <param name="updateMealDTO">Updated meal data.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> UpdateMeal([FromBody] UpdateMealDTO updateMealDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateMealCommand(updateMealDTO, userId));

            return NoContent();
        }

        /// <summary>
        /// Deletes a meal by its ID.
        /// </summary>
        /// <param name="mealId">The meal ID.</param>
        /// <param name="dayId">The day ID associated with the meal.</param>
        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> DeleteMeal([FromQuery] Guid mealId, Guid dayId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new DeleteMealCommand(mealId, dayId, userId));

            return NoContent();
        }
    }
}
