using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Filters;
using UserService.Application.DTOs.Requests.Profile;
using UserService.Application.DTOs.Responces.Profile;
using UserService.Application.DTOs.Responses.Profile;
using UserService.Application.UseCases.Commands;
using UserService.Application.UseCases.Commands.Profile.IncreaseDesiredGlassesOfWater;
using UserService.Application.UseCases.Queries;

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
    public async Task<ActionResult<ProfileResponse>> CreateProfile([FromBody] CreateProfileDTO profileDto, CancellationToken cancellationToken)
    {
        Guid userId = (Guid)HttpContext.Items["UserId"]!;

        var command = new CreateProfileCommand(profileDto, userId);

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpGet("{profileId}/daily-needs")]
    public async Task<ActionResult<DailyNeedsResponse>> CalculateDailyNeeds(Guid profileId, CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var query = new CalculateDailyNutrientsQuery(profileId, userId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpDelete("{profileId}")]
    public async Task<IActionResult> DeleteProfile(Guid profileId, CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var command = new DeleteProfileCommand(profileId, userId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpGet("by-user")]
    public async Task<ActionResult<IEnumerable<ShortProfileResponse>>> GetUserProfiles(CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var query = new GetUserProfilesQuery(userId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpGet("by-id/{profileId}")]
    public async Task<ActionResult<ProfileResponse>> GetProfileById(Guid profileId, CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;
        var query = new GetProfileByIdQuery(profileId, userId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO profileDto, CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        var command = new UpdateProfileCommand(profileDto, userId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    [HttpPut]
    public async Task<IActionResult> ChangeDesiredGlassesOfWater(
        [FromBody] int desiredGlassesOfWater,
        [FromBody] Guid profileid,
        CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        var command = new ChangeDesiredGlassesOfWaterCommand(desiredGlassesOfWater, profileid, userId);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
