using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuthN.Entities;
using Microsoft.AspNetCore.Authorization;
using AuthN.Services;
using AuthN.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using KafkaTransmitter.Services;

namespace AuthN.Pages.Users
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IEventProducerService _producer;
        private readonly UserManager<User> _manager;

        public CreateModel(UserManager<User> manager, IEventProducerService producer)
        {
            _producer = producer;
            _manager = manager;
        }

        public IActionResult OnGet()
        {
            if (GetUser().Role != Role.Admin)
            {
                return Forbid();
            }

            return Page();
        }

        [BindProperty]
        public new User User { get; set; } = default!;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (GetUser().Role != Role.Admin)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            User.PublicId = Guid.NewGuid().ToString();

            if (_manager.FindByNameAsync(User.UserName).Result == null)
            {
                try
                {
                    var result = await _manager.CreateAsync(User, Password);

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.ToString());
                    }
                }

                catch (Exception ex) 
                {
                    Message = ex.Message;
                    return Page();
                }

            }

            _producer.SendMessage("accounts-stream", "Account.Created", new UserCUDModel(User));

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
