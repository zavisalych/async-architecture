using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TaskTracker.Entities;

namespace TaskTracker.Pages;

public class IndexModel : PageModel
{
    public string? Username { get; internal set; }

    public Role Role { get; internal set; }

    public IActionResult OnGet()
    {
        Username = User?.FindFirstValue(ClaimTypes.Name) ?? "";

        if (!Enum.TryParse(User?.FindFirstValue(ClaimTypes.Role), out Role userRole))
            return BadRequest();

        Role = userRole;

        return Page();
    }
}