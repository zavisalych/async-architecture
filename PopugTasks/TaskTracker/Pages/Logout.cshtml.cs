using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace TaskTracker.Pages
{
    public class LogoutModel : PageModel
    {
        [BindProperty]
        public string? ReturnUrl { get; set; } = string.Empty;

        public async Task<IActionResult> OnGet()
        {
            if (ModelState.IsValid)
            {
                await HttpContext.SignOutAsync();

                HttpContext.Response.Cookies.Delete("token");
                return Redirect(string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl);

            }
            else
                return BadRequest(ModelState);
        }
    }
}
