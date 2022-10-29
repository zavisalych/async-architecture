namespace TaskTracker.Models;

using TaskTracker.Entities;

public class AuthenticateResponse
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class AuthBadResponse
{
    public string? Message { get; set; }
}