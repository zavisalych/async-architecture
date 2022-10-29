using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuthN.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using AuthN.Services;
using AuthN.Models;
using Microsoft.AspNetCore.Identity;
using KafkaTransmitter.Services;

namespace AuthN.Pages.Users
{

    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventProducerService _producer;
        private readonly UserManager<User> _manager;

        public DeleteModel(ApplicationDbContext context, IEventProducerService producer, UserManager<User> manager)
        {
            _context = context;
            _producer = producer;
            _manager = manager;
        }

        [BindProperty]
        public new User User { get; set; } = default!;

        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (GetUser().Role != Role.Admin)
            {
                return Forbid();
            }

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            else 
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            if (GetUser().Role != Role.Admin)
            {
                return Forbid();
            }

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                User = user;
                _context.Users.Remove(User);
                await _context.SaveChangesAsync();

                _producer.SendMessage("accounts-stream", "Account.Deleted", new UserCUDModel(User));
            }


            return RedirectToPage("./Index");
        }
        private User GetUser()
        {
            Task<User> user = _manager.GetUserAsync(HttpContext.User);
            user.Wait();

            return user.Result;
        }
    }
}
