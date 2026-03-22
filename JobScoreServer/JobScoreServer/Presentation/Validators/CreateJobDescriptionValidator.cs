using FluentValidation;
using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Presentation.Validators
{
    public class CreateJobDescriptionValidator : AbstractValidator<CreateJobDescriptionDTO>
    {
        public CreateJobDescriptionValidator()
        {
            RuleFor(x => x.title).NotEmpty().MaximumLength(100).WithMessage("title max length is 100");
            RuleFor(x => x.content).NotEmpty().MaximumLength(10000).WithMessage("content max length is 10000");
        }
    }
}
