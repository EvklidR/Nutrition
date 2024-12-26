using UserService.Application.Validators;
using FluentValidation;

namespace UserService.Application.UseCases.Commands
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(CreateUserDtoValidator createUserDtoValidator)
        {
            RuleFor(x => x.createUserDto)
                .NotNull().WithMessage("User data must not be null")
                .SetValidator(createUserDtoValidator);
        }
    }
}
