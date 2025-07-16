using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Filters;
using UserService.Application.DTOs.Requests.Profile;
using UserService.Application.DTOs.Responces.Profile;
using UserService.Application.DTOs.Responses.Profile;
using UserService.Application.UseCases.Commands;
using UserService.Application.UseCases.Queries;
using UserService.Domain.Entities;

namespace UserService.API.Controllers;

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
    public async Task<ActionResult<ProfileResponseDto>> CreateProfile([FromBody] CreateProfileDTO profileDto)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        var command = new CreateProfileCommand(profileDto, userId);

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpGet("{profileId}/daily-needs")]
    public async Task<ActionResult<DailyNeedsResponse>> CalculateDailyNeeds(Guid profileId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var query = new CalculateDailyNutrientsQuery(profileId, userId);

        var result = await _mediator.Send(query);

        return Ok(result);
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
    public async Task<ActionResult<IEnumerable<Profile>?>> GetUserProfiles()
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var query = new GetUserProfilesQuery(userId);

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpGet("by-id/{profileId}")]
    public async Task<ActionResult<Profile>> GetUserById(Guid profileId)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var query = new GetProfileByIdQuery(profileId, userId);

        var result = await _mediator.Send(query);

        return Ok(result);
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
