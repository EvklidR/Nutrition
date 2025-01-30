using FoodService.Application.DTOs.Product;
using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.UseCases.Queries.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.API.Filters;
using FoodService.Application.Models;
using FoodService.Domain.Repositories.Models;

namespace FoodService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts([FromQuery] GetFoodRequestParameters parameters)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetProductsQuery(userId, parameters));

            return Ok(result);
        }

        [HttpGet("api/products/search")]
        [Authorize]
        public async Task<ActionResult<List<ProductResponse>>> GetProductsFromAPI(string name)
        {
            var result = await _mediator.Send(new GetProductsFromAPIQuery(name));

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromBody] CreateProductDTO createProductDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            createProductDTO.UserId = userId;

            var result = await _mediator.Send(new CreateProductCommand(createProductDTO));

            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDTO updateProductDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateProductCommand(updateProductDTO, userId));

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new DeleteProductCommand(productId, userId));

            return NoContent();
        }
    }
}