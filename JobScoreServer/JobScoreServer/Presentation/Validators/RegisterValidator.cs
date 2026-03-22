using FluentValidation;
using JobScoreServer.Application.DTOs;

namespace JobScoreServer.Presentation.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.firstName)
                .MaximumLength(50)
                .NotEmpty()
                .WithMessage("first name is required, 50 chars max");

            RuleFor(x => x.lastName)
                .MaximumLength(50)
                .NotEmpty()
                .WithMessage("last name is required, 50 chars max");

            RuleFor(x => x.email)
                .EmailAddress()
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("enter a valid email address, 100 chars max");

            RuleFor(x => x.password)
                .NotEmpty()
                .MaximumLength(30)
                .MinimumLength(6)
                .WithMessage("password must be between 6-30 chars");

        }
    }
}
