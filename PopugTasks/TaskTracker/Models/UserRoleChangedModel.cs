namespace TaskTracker.Models;

using TaskTracker.Entities;

public class UserRoleChangedModel
{
    public string AccountId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

}
