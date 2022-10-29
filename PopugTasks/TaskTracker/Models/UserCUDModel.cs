namespace TaskTracker.Models;

using TaskTracker.Entities;

public class UserCUDModel
{
    public string PublicId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty;
}
