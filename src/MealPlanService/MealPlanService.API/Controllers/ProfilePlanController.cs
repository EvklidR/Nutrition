using MealPlanService.API.Filters;
using MealPlanService.BusinessLogic.DTOs;
using MealPlanService.BusinessLogic.Services;
using MealPlanService.Core.Entities;
using MealPlanService.Infrastructure.Repositories.Models;
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
        public async Task<IActionResult> CreateProfilePlan(ProfileMealPlanDTO profilePlan)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            await _userPlanService.CreateProfilePlanAsync(userId, profilePlan);

            return Ok();
        }

        /// <summary>
        /// Retrieves the meal plan history for a specific profile of the user.
        /// </summary>
        /// <param name="profileId">The profile identifier for which to retrieve meal plans.</param>
        /// <param name="paginationParameters">Pagination parameters.</param>
        /// <param name="periodParameters">Period parameters.</param>
        [HttpGet("history")]
        [ServiceFilter(typeof(UserIdFilter))]
        [Authorize]
        [ProducesResponseType(typeof(ProfileMealPlansResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileMealPlansResponse>> GetAllProfilePlans(
            string profileId, 
            [FromQuery] PaginationParameters? paginationParameters, 
            [FromQuery] PeriodParameters? periodParameters)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var response = await _userPlanService.GetProfilePlansAsync(userId, profileId, paginationParameters, periodParameters);

            return Ok(response);
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
        [HttpGet("recommendations")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Recommendation>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recommendation>>> GetRecommendations(string profileId)
        {
            var recommendations = await _userPlanService.GetRecommendations(profileId);

            return Ok(recommendations);
        }

        /// <summary>
        /// Retrieves active meal plan of user.
        /// </summary>
        /// <param name="profileId">The profile identifier for which to retrieve meal plans.</param>
        [HttpGet("active-plan/{profileId}")]
        [ServiceFilter(typeof(UserIdFilter))]
        [Authorize]
        [ProducesResponseType(typeof(ProfileMealPlanWithDetailsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProfileMealPlanWithDetailsDto?>> GetActiveProfilePlan(string profileId)
        {
            var userId = (string)HttpContext.Items["UserId"]!;

            var plans = await _userPlanService.GetActiveProfilePlanAsync(userId, profileId);

            return Ok(plans);
        }
    }
}
