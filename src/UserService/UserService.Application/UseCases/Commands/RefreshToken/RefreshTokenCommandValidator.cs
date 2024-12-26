using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.accessToken)
                .NotEmpty().WithMessage("AccessToken is required.");

            RuleFor(x => x.refreshToken)
                .NotEmpty().WithMessage("RefreshToken is required.");
        }
    }
}
