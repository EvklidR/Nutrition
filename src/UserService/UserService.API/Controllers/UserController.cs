using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Filters;
using UserService.Application.UseCases.Commands;

namespace UserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            _logger.LogInformation("here");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("register")]
        [ServiceFilter(typeof(RequestOriginFilter))]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var origin = (string)HttpContext.Items["RequestOrigin"]!;
            await _mediator.Send(command with { url = origin });
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("revoke")]
        [Authorize]
        [ServiceFilter(typeof(UserIdFilter))]
        public async Task<IActionResult> Revoke(RevokeTokenCommand command)
        {
            var userId = (Guid)HttpContext.Items["UserId"]!;
            await _mediator.Send(command with { userId = userId });
            return NoContent();
        }

        [HttpGet("sendConfirmation")]
        [ServiceFilter(typeof(RequestOriginFilter))]
        public async Task<IActionResult> ConfirmEmail([FromQuery] SendConfirmationToEmailCommand request)
        {
            var origin = (string)HttpContext.Items["RequestOrigin"]!;
            await _mediator.Send(request with { url = origin });
            return Ok();
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command) 
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
