namespace AuthN.Models;

using AuthN.Entities;

public class UserCUDModel
{
    public string PublicId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; }

    public UserCUDModel(User user)
    {
        PublicId = user.PublicId;
        UserName = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Role = user.Role.ToString();
    }
}
