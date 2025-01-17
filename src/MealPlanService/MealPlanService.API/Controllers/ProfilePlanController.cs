using MealPlanService.API.Filters;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanService.API.Controllers
{
    /// <summary>
    /// API controller for managing binding users with treir meal plans.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProfilePlanController : ControllerBase
    {
        private readonly ProfilePlanService _userPlanService;

        public ProfilePlanController(ProfilePlanService userPlanService)
        {
            _userPlanService = userPlanService;
        }

        /// <summary>
        /// Creates a new bind for user with meal plan.
        /// </summary>
        /// <param name="profilePlan">Details of the profile's meal plan.</param>
        [HttpPost]
        [ServiceFilter(typeof(UserIdFilter))]
        [Authorize]
        [ProducesResponseType(typeof(MealPlan), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateProfilePlan(ProfileMealPlanDTO profilePlan)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var mealplan = await _userPlanService.CreateProfilePlanAsync(userId, profilePlan);

            return Ok(mealplan);
        }

        /// <summary>
        /// Retrieves the meal plan history for a specific profile of the user.
        /// </summary>
        /// <param name="profileId">The profile identifier for which to retrieve meal plans.</param>
        [HttpGet("get_history")]
        [ServiceFilter(typeof(UserIdFilter))]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<MealPlan>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProfilePlans(string profileId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var plans = await _userPlanService.GetProfilePlansAsync(userId, profileId);

            return Ok(plans);
        }

        /// <summary>
        /// Marks a meal plan as completed for the specified profile.
        /// </summary>
        /// <param name="profileId">The profile identifier for which to complete the meal plan.</param>
        [HttpPost("complete")]
        [ServiceFilter(typeof(UserIdFilter))]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CompleteProfilePlan(string profileId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _userPlanService.CompleteProfilePlanAsync(userId, profileId);

            return NoContent();
        }

        /// <summary>
        /// Retrieves meal plan recommendations for the specified profile based on active meal plan and current day.
        /// </summary>
        /// <param name="profileId">The profile identifier for which to retrieve recommendations.</param>
        [HttpGet("get_recommendations")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Recommendation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendations(string profileId)
        {
            var recommendations = await _userPlanService.GetRecommendations(profileId);

            return Ok(recommendations);
        }
    }
}
