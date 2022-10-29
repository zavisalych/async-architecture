namespace AuthN.Services;

using AuthN.Entities;
using AuthN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface IAuthenticationService
{
    Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
}

public class AuthenticationService : IAuthenticationService
{

    private readonly AppSettings _appSettings;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public AuthenticationService(IOptions<AppSettings> appSettings, ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _appSettings = appSettings.Value;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == model.Username);

        // return null if user not found
        if (user == null) return null;

        var hasher = _userManager.PasswordHasher;
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
            return null;

        // authentication successful so generate jwt token
        var token = GenerateJwtToken(user);

        return new AuthenticateResponse(user, token);
    }

    // helper methods

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.PublicId.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}