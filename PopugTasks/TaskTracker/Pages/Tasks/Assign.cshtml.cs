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
public class AssignModel : PageModel
{
    private readonly TaskTracker.Services.ApplicationDbContext _context;

    public AssignModel(TaskTracker.Services.ApplicationDbContext context)
    {
        _context = context;
    }

    public Role Role { get; internal set; }

    public string Message { get; internal set; } = string.Empty;

    private bool GetRole()
    {
        if (!Enum.TryParse(User?.FindFirstValue(ClaimTypes.Role), out Role userRole))
            return false;

        Role = userRole;
        return true;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromServices] TasksAssignmentService assignmentService)
    {
        if (!GetRole() || !(Role.Manager | Role.Admin).HasFlag(Role))
            return Forbid();

        var result = await assignmentService.AssignTasks();

        if (result.IsSuccess)
        {
            Message = $"{result.NumTasksAssigned} tasks are assigned successfully.";
        }
        else
        {
            Message = $"Error assigning tasks. Error: {result.ErrorMessage}";
        }

        return Page();

        
    }
}
