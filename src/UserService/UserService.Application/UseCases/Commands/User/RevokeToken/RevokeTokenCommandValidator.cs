using FluentValidation;

namespace UserService.Application.UseCases.Commands;

public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id must be provided");
    }
}
