using MediatR;
using Microsoft.AspNetCore.Mvc;
using FoodService.Application.UseCases.Queries.Dish;
using FoodService.Domain.Repositories.Models;
using Microsoft.AspNetCore.Authorization;
using FoodService.API.Filters;
using FoodService.Application.DTOs.Recipe.Requests;
using FoodService.Application.DTOs.Recipe.Responses;
using FoodService.Application.DTOs.Dish.Responses;
using FoodService.Application.UseCases.Queries.Recipe;
using FoodService.Application.UseCases.Commands.Recipes;
using FoodService.Application.UseCases.Commands.Dishes;

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
        /// Gets a recipe by its ID.
        /// </summary>
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>The requested recipe.</returns>
        [HttpGet("{recipeId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(CalculatedRecipeResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CalculatedRecipeResponse>> GetRecipeById(Guid recipeId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetRecipeByIdQuery(recipeId, userId));

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
        /// Creates a new recipe.
        /// </summary>
        /// <param name="createRecipeDTO">Recipe data.</param>
        /// <returns>The created recipe.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(CalculatedRecipeResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<CalculatedRecipeResponse>> CreateDish([FromForm] CreateRecipeDTO createRecipeDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new CreateRecipeCommand(createRecipeDTO, userId));

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing recipe.
        /// </summary>
        /// <param name="updateRecipeDTO">Updated recipe data.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateDish([FromForm] UpdateRecipeDTO updateRecipeDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateRecipeCommand(updateRecipeDTO, userId));

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
        /// <param name="recipeId">The recipe ID.</param>
        /// <returns>The dish image.</returns>
        [HttpGet("{recipeId}/image")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRecipeImage(Guid recipeId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var fileStream = await _mediator.Send(new GetRecipeImageQuery(recipeId, userId));

            return File(fileStream, "image/jpeg");
        }
    }
}
