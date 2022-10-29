namespace AuthN.Models;

using AuthN.Entities;

public class AuthenticateResponse
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string? Token { get; set; }


    public AuthenticateResponse(User user, string token)
    {
        Id = user.PublicId.ToString();
        Username = user.UserName;
        Role = user.Role.ToString();
        Token = token;
    }
}