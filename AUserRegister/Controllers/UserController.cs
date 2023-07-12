using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AUserRegister.Features;
using AUserRegister.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AUserRegister.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;


    public UserController(IConfiguration configuration, IUserService userService)
    {
        _userService = userService;
        _configuration = configuration;
    }
    
    [HttpGet, Authorize]
    public ActionResult<string> GetMyEmail()
    {
        return Ok(_userService.GetMyEmail());

        //var userName = User?.Identity?.Name;
        //var roleClaims = User?.FindAll(ClaimTypes.Role);
        //var roles = roleClaims?.Select(c => c.Value).ToList();
        //var roles2 = User?.Claims
        //    .Where(c => c.Type == ClaimTypes.Role)
        //    .Select(c => c.Value)
        //    .ToList();
        //return Ok(new { userName, roles, roles2 });
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

    [HttpGet, Authorize(Roles = "Admin")]
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


        // To set a refresh Token
        var refreshToken = GenerateRefreshToken();
        SetRefreshToken(user, refreshToken);

        return Ok(token);
        //return user;
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<string>> RefreshToken(User user)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!user.RefreshToken.Equals(refreshToken))
            return Unauthorized("Invalid Refresh Token");
        if (user.TokenExpires < DateTime.Now) return Unauthorized("Token expired.");

        var token = CreateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        SetRefreshToken(user, newRefreshToken);
        return Ok(token);
    }


    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7)
        };
        return refreshToken;
    }

    private void SetRefreshToken(User user, RefreshToken newRefreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = newRefreshToken.Expires
        };
        Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        user.RefreshToken = newRefreshToken.Token;
        user.TokenCreated = newRefreshToken.CreatedDate;
        user.TokenExpires = newRefreshToken.Expires;
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User")
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