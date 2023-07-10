namespace AUserRegister.Models;

public class RefreshToken
{
    public string Token { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime Expires { get; set; }
}