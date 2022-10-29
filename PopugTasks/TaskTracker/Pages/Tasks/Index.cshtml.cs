using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Entities;
using TaskTracker.Services;

namespace TaskTracker.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly TaskTracker.Services.ApplicationDbContext _context;

    public IndexModel(TaskTracker.Services.ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<PopugTask> PopugTask { get;set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        if (_context.Tasks == null)
            return NotFound();

        if (int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            return Unauthorized();

        PopugTask = await _context.Tasks.Where(x => x.Assignee.Id == userId).ToListAsync();

        return Page();
    }
}
