using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskTracker.Entities;
using TaskTracker.Services;

namespace TaskTracker.Pages.Tasks;

[Authorize]
public class CreateModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public PopugTask PopugTask { get; set; } = default!;

    public string Message { get; internal set; } = string.Empty;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync([FromServices] TaskCreationService creationService)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await creationService.CreateTaskAsync(PopugTask);

        if (result.IsSuccess)
        {
            Message = $"Task created successfully.";
        }
        else
        {
            Message = $"Error creating task. Error: {result.ErrorMessage}";
        }

        return RedirectToPage("./Index");
    }
}
