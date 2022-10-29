using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using TaskTracker.Entities;
using TaskTracker.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TaskTracker.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace TaskTracker.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {

        [BindProperty]
        public string? ReturnUrl { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;

        public string? Message { get; set; }

        private readonly AppSettings _appSettings;

        public LoginModel(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!string.IsNullOrEmpty(User.FindFirstValue(ClaimTypes.Name)))
            {
                Message = "Already logged in.";
                return Page();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = new AuthenticateRequest() { Password = Password, Username = Username };

            HttpClient client = new HttpClient();

            var response = await client.PostAsJsonAsync(_appSettings.AuthServer, request);

            if (!response.IsSuccessStatusCode)
            {

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var reason = await response.Content.ReadFromJsonAsync<AuthBadResponse>();

                    if (reason != null)
                    {
                        Message = reason.Message;
                    }
                    else
                        Message = "Unknown auth request. Unable to authorize.";

                    return Page();

                }
                else
                    return StatusCode((int)response.StatusCode);
            }

            AuthenticateResponse? content = await response.Content.ReadFromJsonAsync<AuthenticateResponse>();

            if (content == null)
            {
                Message = "Auth server is bad";
                return Page();
            }

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, content.Username),
                new Claim(ClaimTypes.Role, content.Role)
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            HttpContext.Response.Cookies.Append("token", content.Token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });

            return Redirect(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
        }
    }
}
