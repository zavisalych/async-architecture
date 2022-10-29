namespace AuthN.Entities;

using Microsoft.AspNetCore.Identity;

public class User: IdentityUser
{
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

