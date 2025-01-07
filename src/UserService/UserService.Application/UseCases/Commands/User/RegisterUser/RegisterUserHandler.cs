using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Exceptions;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.UseCases.Commands
{
    public class RegisterUserHandler : ICommandHandler<RegisterUserCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;

        public RegisterUserHandler(UserManager<User> userManager, IMediator mediator, ITokenService tokenService)
        {
            _userManager = userManager;
            _mediator = mediator;
            _tokenService = tokenService;
        }

        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.email;

            var user = new User
            {
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, request.password);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequest(errorMessage);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "user");
            if (!roleResult.Succeeded)
            {
                var errorMessage = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new BadRequest(errorMessage);
            }

            await _mediator.Send(new SendConfirmationToEmailCommand(user, request.url, user.Email));
        }

    }
}
