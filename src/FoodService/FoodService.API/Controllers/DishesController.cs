using MediatR;
using Microsoft.AspNetCore.Mvc;
using FoodService.Application.UseCases.Commands.Dish;
using FoodService.Application.UseCases.Queries.Dish;
using FoodService.Application.DTOs.Dish;
using FoodService.Domain.Repositories.Models;
using Microsoft.AspNetCore.Authorization;
using FoodService.API.Filters;

namespace FoodService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DishesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DishesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{dishId}")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<FullDishDishDTO>> GetDishById(Guid dishId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetDishByIdQuery(dishId, userId));

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<IEnumerable<BriefDishDishDTO>>> GetAllDishes([FromQuery] GetFoodRequestParameters parameters)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetAllDishesQuery(userId, parameters));

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<FullDishDishDTO>> CreateDish([FromBody] CreateDishDTO createDishDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            createDishDTO.UserId = userId;

            var result = await _mediator.Send(new CreateDishCommand(createDishDTO));

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> UpdateDish([FromBody] UpdateDishDTO updateDishDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateDishCommand(updateDishDTO, userId));

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> DeleteDish(Guid dishId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new DeleteDishCommand(dishId, userId));

            return NoContent();
        }
    }
}