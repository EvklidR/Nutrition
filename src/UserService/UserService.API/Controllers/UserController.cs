using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Filters;
using UserService.Application.DTOs.Responses.User;
using UserService.Application.UseCases.Commands;

namespace UserService.API.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthenticatedResponse>> Login(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(RequestOriginFilter))]
    public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var origin = (string)HttpContext.Items["RequestOrigin"]!;

        await _mediator.Send(command with { Url = origin }, cancellationToken);

        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthenticatedResponse>> Refresh(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    [ServiceFilter(typeof(UserIdFilter))]
    public async Task<IActionResult> Revoke(RevokeTokenCommand command, CancellationToken cancellationToken)
    {
        var userId = (Guid)HttpContext.Items["UserId"]!;

        await _mediator.Send(command with { UserId = userId }, cancellationToken);

        return NoContent();
    }

    [HttpPost("sendConfirmation")]
    [ServiceFilter(typeof(RequestOriginFilter))]
    public async Task<IActionResult> ConfirmEmail(SendConfirmationToEmailCommand request, CancellationToken cancellationToken)
    {
        var origin = (string)HttpContext.Items["RequestOrigin"]!;

        await _mediator.Send(request with { Url = origin }, cancellationToken);

        return NoContent();
    }

    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command, CancellationToken cancellationToken) 
    {
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
