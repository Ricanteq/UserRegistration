
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AUserRegister.Features;
using AUserRegister.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace AUserRegister.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;


    public UserController(IConfiguration  configuration, IUserService userService)
    {
        _userService = userService;
        _configuration = configuration;
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
       
        var token = CreateToken(user);

        return Ok(token);
        //return user;
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            //new Claim(ClaimTypes.Role, "Admin"),
            //new Claim(ClaimTypes.Role, "User"),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value!));

         var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
 
         var token = new JwtSecurityToken(
             claims: claims,
             expires: DateTime.Now.AddDays(1),
             signingCredentials: creds
         );
 
         var jwt = new JwtSecurityTokenHandler().WriteToken(token);
 
         return jwt;
    
}
}