using System.Security.Authentication;
using AUserRegister.Models;
using AUserRegister.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace AUserRegister.Features;

public class UserService : IUserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    public async Task<ValidationResult> ValidateUserAsync(User user)
    {
        var validator = new UserValidator();
        return await validator.ValidateAsync(user);
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<User> CreateUserAsync(User user)
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<User> GetUserByIdAsync(int id)
    {
               return await _context.Users.FindAsync(id) ?? 
               throw new InvalidOperationException("user not found.");
    }


    public async Task<bool> CheckUserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task UpdateUserAsync(int id, User updatedUser)
    {
        var userToUpdate = await _context.Users.FindAsync(id);
        if (userToUpdate != null)
        {
            userToUpdate.FirstName = updatedUser.FirstName;
            userToUpdate.LastName = updatedUser.LastName;
            userToUpdate.Email = updatedUser.Email;

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
                userToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var userToDelete = await _context.Users.FindAsync(id);
        if (userToDelete != null)
        {
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }
    }

    /*public async Task<User> LoginUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) return user;

        return null;
    }*/
    
    public async Task<User> LoginUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) return user;

        throw new AuthenticationException("Invalid email or password."); // Throw an exception
    }


    private class UserValidator : AbstractValidator<User>
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

}

