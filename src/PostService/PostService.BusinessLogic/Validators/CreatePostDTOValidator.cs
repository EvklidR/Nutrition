using FluentValidation;
using PostService.BusinessLogic.DTOs.Post;

namespace PostService.BusinessLogic.Validators
{
    public class CreatePostDTOValidator : AbstractValidator<CreatePostDTO>
    {
        public CreatePostDTOValidator() 
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .WithMessage("Text must be provided")
                .MaximumLength(2000)
                .WithMessage("Text length must be less than 2000 letters");

            RuleFor(x => x.Title)
                .MaximumLength(100)
                .WithMessage("Title length must be less than 100 letters");
        }
    }
}
