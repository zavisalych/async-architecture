namespace TaskTracker.Entities;
using System.Text.Json.Serialization;

public class Popug
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Role Role { get; set; }

}

public enum Role
{
    Admin,
    User,
    Manager,
    Accountant
}