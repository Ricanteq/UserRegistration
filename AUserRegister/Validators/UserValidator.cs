/*public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty().WithMessage("Firstname cannot be empty");
        RuleFor(u => u.LastName).NotEmpty().WithMessage("Lastname cannot be empty");
        RuleFor(u => u.Email).NotEmpty().WithMessage("Email cannot be empty");
        RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
        RuleFor(u => u.Password).NotEmpty().WithMessage("Password cannot be empty");
    }
}*/
using AUserRegister.Models;
using FluentValidation;

namespace AUserRegister.Validators;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.FirstName).NotEmpty().WithMessage("Firstname cannot be empty");
        RuleFor(u => u.LastName).NotEmpty().WithMessage("Lastname cannot be empty");
        RuleFor(u => u.Email).NotEmpty().WithMessage("Email cannot be empty");
        RuleFor(u => u.Email).EmailAddress().WithMessage("Invalid email address");
        RuleFor(u => u.Password).NotEmpty().WithMessage("Password cannot be empty");
        RuleFor(u => u.Password).MinimumLength(8).WithMessage("Password must be at least 8 characters long");
        RuleFor(u => u.Password).Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter");
        RuleFor(u => u.Password).Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter");
        RuleFor(u => u.Password).Matches("[0-9]").WithMessage("Password must contain at least one digit");
        RuleFor(u => u.Password).Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
    }
}
