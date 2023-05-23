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
    }
}