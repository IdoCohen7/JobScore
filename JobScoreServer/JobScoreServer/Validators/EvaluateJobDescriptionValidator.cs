using JobScoreServer.DTOs;
using FluentValidation;

namespace JobScoreServer.Validators
{
    public class EvaluateJobDescriptionValidator : AbstractValidator<EvaluateJobDescriptionDTO>
    {
        public EvaluateJobDescriptionValidator()
        {
            RuleFor(x => x.title).NotEmpty().MaximumLength(100).WithMessage("title max length is 100");
            RuleFor(x => x.content).NotEmpty().MaximumLength(10000).WithMessage("content max length is 10000");
        }
    }
}
