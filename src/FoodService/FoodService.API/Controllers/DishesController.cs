using MediatR;
using Microsoft.AspNetCore.Mvc;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.UseCases.Queries.Dish;
using FoodService.Application.DTOs.Dish;
using FoodService.Domain.Repositories.Models;
using Microsoft.AspNetCore.Authorization;
using FoodService.API.Filters;
using FoodService.Application.Models;

namespace FoodService.API.Controllers
{
    /// <summary>
    /// Controller for managing dishes.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class DishesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DishesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a dish by its ID.
        /// </summary>
        /// <param name="dishId">The dish ID.</param>
        /// <returns>The requested dish.</returns>
        [HttpGet("{dishId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(FullDishDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<FullDishDTO>> GetDishById(Guid dishId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetDishByIdQuery(dishId, userId));

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all dishes based on request parameters.
        /// </summary>
        /// <param name="parameters">Filtering and sorting parameters.</param>
        /// <returns>A list of dishes.</returns>
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(DishesResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<DishesResponse>> GetDishes([FromQuery] GetFoodRequestParameters parameters)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetDishesQuery(userId, parameters));

            return Ok(result);
        }

        /// <summary>
        /// Creates a new dish.
        /// </summary>
        /// <param name="createDishDTO">Dish data.</param>
        /// <returns>The created dish.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(FullDishDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<FullDishDTO>> CreateDish([FromForm] CreateDishDTO createDishDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            createDishDTO.UserId = userId;

            var result = await _mediator.Send(new CreateDishCommand(createDishDTO));

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing dish.
        /// </summary>
        /// <param name="updateDishDTO">Updated dish data.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateDish([FromForm] UpdateDishDTO updateDishDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateDishCommand(updateDishDTO, userId));

            return NoContent();
        }

        /// <summary>
        /// Deletes a dish by its ID.
        /// </summary>
        /// <param name="dishId">The dish ID.</param>
        [HttpDelete("{dishId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDish(Guid dishId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new DeleteDishCommand(dishId, userId));

            return NoContent();
        }

        /// <summary>
        /// Retrieves the image of a dish.
        /// </summary>
        /// <param name="dishId">The dish ID.</param>
        /// <returns>The dish image.</returns>
        [HttpGet("{dishId}/image")]
        [Authorize]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDishImage(Guid dishId)
        {
            var fileStream = await _mediator.Send(new GetDishImageQuery(dishId));

            return File(fileStream, "image/jpeg");
        }
    }
}
