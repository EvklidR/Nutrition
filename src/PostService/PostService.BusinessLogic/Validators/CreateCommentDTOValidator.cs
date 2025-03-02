using FluentValidation;
using PostService.BusinessLogic.DTOs.Comment;

namespace PostService.BusinessLogic.Validators
{
    public class CreateCommentDTOValidator : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidator() 
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Text must be provided")
                .MaximumLength(1000)
                .WithMessage("Text length must be less than 1000 letters");
        }
    }
}
