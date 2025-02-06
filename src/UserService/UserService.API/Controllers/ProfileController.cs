using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Filters;
using UserService.Application.DTOs;
using UserService.Application.UseCases.Commands;
using UserService.Application.UseCases.Queries;

namespace UserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDTO profileDto)
        {
            Guid userId = (Guid)HttpContext.Items["UserId"]!;
            profileDto.UserId = userId;
            var command = new CreateProfileCommand(profileDto);

            var result = await _mediator.Send(command);

            return CreatedAtAction(null, result);
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpGet("{profileId}/daily-needs")]
        public async Task<IActionResult> CalculateDailyNeeds(Guid profileId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var query = new CalculateDailyNutrientsQuery(profileId, userId);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpPatch("choose-meal-plan")]
        public async Task<IActionResult> ChooseMealPlan(Guid profileId, Guid mealPlanId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var command = new ChooseMealPlanCommand(profileId, userId);

            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpDelete("{profileId}")]
        public async Task<IActionResult> DeleteProfile(Guid profileId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var command = new DeleteProfileCommand(profileId, userId);

            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpGet("by-user")]
        public async Task<IActionResult> GetUserProfiles()
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var query = new GetUserProfilesQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpGet("by-id/{profileId}")]
        public async Task<IActionResult> GetUserById(Guid profileId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var query = new GetProfileByIdQuery(profileId, userId);

            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpPatch("{profileId}/revoke-meal-plan")]
        public async Task<IActionResult> RevokeMealPlan(Guid profileId)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var command = new RevokeMealPlanCommand(profileId, userId);

            await _mediator.Send(command);

            return NoContent();
        }

        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO profileDto)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            var command = new UpdateProfileCommand(profileDto, userId);

            await _mediator.Send(command);

            return NoContent();
        }

    }
}
