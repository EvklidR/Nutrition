using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Models;
using MealPlanService.Core.Entities;
using MealPlanService.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanService.API.Controllers
{
    /// <summary>
    /// API controller for managing meal plans.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class MealPlanController : ControllerBase
    {
        private readonly BusinessLogic.Services.MealPlanService _mealPlanService;

        public MealPlanController(BusinessLogic.Services.MealPlanService mealPlanService)
        {
            _mealPlanService = mealPlanService;
        }

        /// <summary>
        /// Retrieves a list of meal plans with optional pagination and type selection.
        /// </summary>
        /// <param name="type">Optional filter for the type of meal plan (e.g., Weight loss, Weight gain).</param>
        /// <param name="page">Page number for pagination (optional).</param>
        /// <param name="size">Number of items per page (optional).</param>
        [ProducesResponseType(typeof(MealPlansResponse), StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet("meal_plans")]
        public async Task<IActionResult> GetMealPlans([FromQuery] MealPlanType? type, [FromQuery] int? page, [FromQuery] int? size)
        {
            MealPlansResponse response = await _mealPlanService.GetMealPlansAsync(type, page, size);
            return Ok(response);
        }

        /// <summary>
        /// Creates a new meal plan.
        /// </summary>
        /// <param name="mealPlanDto">Details of the meal plan to be created.</param>
        [ProducesResponseType(typeof(MealPlan), StatusCodes.Status200OK)]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateMealPlan([FromBody] CreateMealPlanDTO mealPlanDto)
        {
            var createdMealPlan = await _mealPlanService.CreateMealPlanAsync(mealPlanDto);
            return Ok(createdMealPlan);
        }

        /// <summary>
        /// Deletes a meal plan by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the meal plan to delete.</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMealPlan(string id)
        {
            await _mealPlanService.DeleteMealPlanAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing meal plan with new details.
        /// </summary>
        /// <param name="updatedMealPlanDto">The updated meal plan with all properties and necessarily valid Id.</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateMealPlan([FromBody] MealPlan updatedMealPlanDto)
        {
            await _mealPlanService.UpdateMealPlanAsync(updatedMealPlanDto);
            return NoContent();
        }
    }
}
