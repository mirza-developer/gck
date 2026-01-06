using FluentValidation;
using Gck.Application.Features.Users.Commands.AddUser;

namespace Gck.Application.Validators;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(256).WithMessage("Username cannot exceed 256 characters.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(512).WithMessage("Name cannot exceed 512 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
