using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthN.Models;
using AuthN.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

namespace AuthN.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        private Services.IAuthenticationService _userService;
        private ApplicationDbContext _dbContext;

        public UsersController(Services.IAuthenticationService userService, ApplicationDbContext dbContext)
        {
            _userService = userService;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _dbContext.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }


    }
}
