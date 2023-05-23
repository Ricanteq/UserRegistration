using AUserRegister.Features;
using AUserRegister.Models;
using Microsoft.AspNetCore.Mvc;

namespace AUserRegister.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        var validationResult = await _userService.ValidateUserAsync(user);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var emailExists = await _userService.CheckEmailExistsAsync(user.Email);
        if (emailExists) return BadRequest("Email is already in use");

        var createdUser = await _userService.CreateUserAsync(user);

        return Ok(createdUser);
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        return user;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, User updatedUser)
    {
        var userExists = await _userService.CheckUserExistsAsync(id);
        if (!userExists) return NotFound();

        await _userService.UpdateUserAsync(id, updatedUser);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var userExists = await _userService.CheckUserExistsAsync(id);
        if (!userExists) return NotFound();

        await _userService.DeleteUserAsync(id);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<User>> LoginUser(LoginRequest request)
    {
        var user = await _userService.LoginUserAsync(request.Email, request.Password);
        if (user == null) return Unauthorized();

        return user;
    }
}