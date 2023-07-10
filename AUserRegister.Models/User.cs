using System.Text.Json.Serialization;

namespace AUserRegister.Models;

public class User
{
    [JsonIgnore] public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    [JsonIgnore] public string RefreshToken { get; set; } = string.Empty;

    [JsonIgnore] public DateTime TokenCreated { get; set; }

    [JsonIgnore] public DateTime TokenExpires { get; set; }
}