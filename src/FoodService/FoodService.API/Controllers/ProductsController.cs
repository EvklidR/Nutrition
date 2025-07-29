using FoodService.Application.UseCases.Commands.Product;
using FoodService.Application.UseCases.Queries.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodService.API.Filters;
using FoodService.Domain.Repositories.Models;
using FoodService.Application.DTOs.Product.Responses;
using FoodService.Application.DTOs.Product.Requests;

namespace FoodService.API.Controllers
{
    /// <summary>
    /// Controller for managing products.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a list of products based on query parameters.
        /// </summary>
        /// <param name="parameters">Filtering and sorting parameters.</param>
        /// <returns>List of products.</returns>
        [HttpGet]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<ProductsResponse>> GetProducts([FromQuery] GetFoodRequestParameters parameters)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new GetProductsQuery(userId, parameters));

            return Ok(result);
        }

        /// <summary>
        /// Searches for products in an external API by name.
        /// </summary>
        /// <param name="name">The product name to search for.</param>
        /// <returns>List of products from an external API.</returns>
        [HttpGet("search-product/{name}")]
        [Authorize]
        public async Task<ActionResult<List<ProductResponseFromAPI>>> GetProductsFromAPI(string name)
        {
            var result = await _mediator.Send(new GetProductsFromAPIQuery(name));

            return Ok(result);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="createProductDTO">The product data.</param>
        /// <returns>The created product.</returns>
        [HttpPost]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductDTO createProductDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            var result = await _mediator.Send(new CreateProductCommand(createProductDTO, userId));

            return Ok(result);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="updateProductDTO">The updated product data.</param>
        [HttpPut]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDTO updateProductDTO)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;

            await _mediator.Send(new UpdateProductCommand(updateProductDTO, userId));

            return NoContent();
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="productId">The ID of the product to delete.</param>
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
