using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventProducerService _producer;
        private readonly UserManager<User> _manager;

        public EditModel(ApplicationDbContext context, IEventProducerService producer, UserManager<User> manager)
        {
            _context = context;
            _producer = producer;
            _manager = manager;
        }

        [BindProperty]
        public new User User { get; set; } = default!;

        [BindProperty]
        public string? Password { get; set; }

        public string Message { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (GetCurrentUser().Role != Role.Admin)
            {
                return Forbid();
            }

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user =  await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            User = user;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (GetCurrentUser().Role != Role.Admin)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            User? realUser = await _context.Users.FirstOrDefaultAsync(m => m.Id == User.Id);
            if (realUser == null)
            {
                return NotFound();
            }

            realUser.FirstName = User.FirstName;
            realUser.LastName = User.LastName;
            realUser.Role = User.Role;

            if (!string.IsNullOrEmpty(Password))
            {
                var hasher = _manager.PasswordHasher;
                realUser.PasswordHash = hasher.HashPassword(realUser, Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(realUser.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _producer.SendMessage("accounts-stream", "Account.Updated", new UserCUDModel(realUser));

            return RedirectToPage("./Index");
        }

        private bool UserExists(string id)
        {
          return _context.Users.Any(e => e.Id == id);
        }

        private User GetCurrentUser()
        {
            Task<User> user = _manager.GetUserAsync(HttpContext.User);
            user.Wait();

            return user.Result;
        }
    }
}
