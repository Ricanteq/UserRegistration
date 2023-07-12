using AUserRegister.Models;
using FluentValidation.Results;

namespace AUserRegister.Features;

public interface IUserService
{
    string GetMyEmail();
    Task<ValidationResult> ValidateUserAsync(User user);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<bool> CheckUserExistsAsync(int id);
    Task UpdateUserAsync(int id, User updatedUser);
    Task DeleteUserAsync(int id);
    Task<User> LoginUserAsync(string email, string password);
}