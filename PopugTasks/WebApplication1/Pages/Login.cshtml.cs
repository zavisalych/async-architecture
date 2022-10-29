using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using AuthN.Controllers;
using AuthN.Entities;
using AuthN.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthN.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AuthN.Pages
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

        [BindProperty]
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string Message { get; set; } = string.Empty;

        private readonly SignInManager<User> _signInManager;

        private bool IsLoggedIn()
        {

            return User?.Identity?.IsAuthenticated ?? false;
        }

        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        public IActionResult OnGet()
        {
            if (IsLoggedIn())
            {
                return Redirect(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
            }
            else
                return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (IsLoggedIn())
            {
                Message = "Already logged in.";
                return Page();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(Username, Password, RememberMe, false);

            if (result.Succeeded)
            {
                return Redirect(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);
            }
            else
            {
                Message = "Invalid login: " + result.ToString();
                return Page();
            }

        }
    }
}
