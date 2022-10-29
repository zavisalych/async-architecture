using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Construction;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Entities;
using TaskTracker.Services;

namespace TaskTracker.Pages.Tasks;

[Authorize]
public class CompleteModel : PageModel
{
    private readonly TaskTracker.Services.ApplicationDbContext _context;

    public CompleteModel(TaskTracker.Services.ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public PopugTask PopugTask { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null || _context.Tasks == null)
        {
            return NotFound();
        }

        var popugtask = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id);

        if (popugtask == null)
        {
            return NotFound();
        }
        else 
        {
            PopugTask = popugtask;
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id, [FromServices] TaskCompletionService completionService)
    {
        if (id == null || _context.Tasks == null)
        {
            return NotFound();
        }

        if (int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            return Unauthorized();

        var popugtask = await _context.Tasks.FindAsync(id);

        if (popugtask != null)
        {
            if (popugtask.Assignee?.Id != userId)
                return Forbid();

            var result = await completionService.CompleteTaskAsync(popugtask);

            if (!result.IsSuccess)
                return BadRequest();
        }

        return RedirectToPage("./Index");
    }
}
