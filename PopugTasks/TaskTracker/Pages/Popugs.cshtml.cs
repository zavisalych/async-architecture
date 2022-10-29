using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Entities;
using TaskTracker.Services;

namespace TaskTracker.Pages
{
    [Authorize]
    public class PopugsModel : PageModel
    {
        private readonly TaskTracker.Services.ApplicationDbContext _context;

        public PopugsModel(TaskTracker.Services.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Popug> Popug { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Popugs != null)
            {
                Popug = await _context.Popugs.ToListAsync();
            }
        }
    }
}
