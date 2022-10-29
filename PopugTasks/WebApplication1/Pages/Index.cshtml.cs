using AuthN.Entities;
using AuthN.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace AuthN.Pages;

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
