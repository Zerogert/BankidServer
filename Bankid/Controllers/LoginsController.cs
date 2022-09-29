using AutoMapper;
using Bankid.Data;
using Bankid.Interfaces;
using Bankid.Models.Entities;
using Bankid.Models.Requests;
using Bankid.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bankid.Controllers {
    [AllowAnonymous]
    public class LoginsController : BaseApiController {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ISessionTokenService _sessionTokenService;

        public LoginsController(IMapper mapper, ISessionTokenService sessionTokenService, AppDbContext dbContext, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager) : base(dbContext, mapper) {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _sessionTokenService = sessionTokenService;
        }

        [HttpPost]
        [Route("/api/login")]
        public async Task<IActionResult> SignInAsync(UserLogin login) {
            var result = await _signInManager.PasswordSignInAsync(login.Login, login.Password, true, true);
            if (result.Succeeded) {
                var user = await _userManager.FindByNameAsync(login.Login);
                var accessToken = await _sessionTokenService.GenerateTokenAsync(user);
                this.Response.Headers.Append(SessionTokenService.AUTHORIZATION_KEY, "Bearer " + accessToken);
                return Ok();
            }

            return Forbid();
        }

        [HttpPost]
        [Route("/api/login/create")]
        public async Task<IActionResult> CreateAsync(UserLogin login) {
            var user = new Models.Entities.User() { UserName = login.Login, Role = Role.UserRoleName, FirstName = login.FirstName, LastName = login.LastName };
            var result = await _userManager.CreateAsync(user, login.Password);
            if (result.Succeeded) {
                await _userManager.AddToRoleAsync(user, Role.UserRoleName);
            }

            if (result.Succeeded) {
                user = await _userManager.FindByNameAsync(login.Login);
                var accessToken = await _sessionTokenService.GenerateTokenAsync(user);
                this.Response.Headers.Append(SessionTokenService.AUTHORIZATION_KEY, "Bearer " + accessToken);
                return Ok();
            }

            return Forbid();
        }
    }
}
