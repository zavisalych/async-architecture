namespace AuthN.Models;

using AuthN.Entities;

public class UserRoleChangedModel
{
    public string AccountId { get; set; }
    public string Role { get; set; }

    public UserRoleChangedModel(User user)
    {
        AccountId = user.PublicId;
        Role = user.Role.ToString();
    }
}
