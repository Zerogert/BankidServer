using AutoMapper;
using Bankid.Data;
using Bankid.Extensions;
using Bankid.Interfaces;
using Bankid.Models.Entities;
using Bankid.Services;
using Common.BusinessLayer.Models.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Bankid.Controllers {
	[AllowAnonymous]
	public class AuthsController : BaseApiController {
		private const string BASE_SESSIONS_RESOURCE = "/api/Sessions/callback";
		private readonly SignInManager<User> _signInManager;
		private readonly InstanseOptions _instanseOptions;
		private readonly IUsersService _usersService;
		private readonly ISessionTokenService _sessionTokenService;

		public AuthsController(ISessionTokenService sessionTokenService, IUsersService usersService, InstanseOptions instanseOptions, SignInManager<User> signInManager, IMapper mapper, AppDbContext dbContext) : base(dbContext, mapper) {
			_signInManager = signInManager;
			_instanseOptions = instanseOptions;
			_usersService = usersService;
			_sessionTokenService = sessionTokenService;
		}

		[HttpGet]
		public async Task<IActionResult> SignIn(string returnUrl) {
			var redirectUrl = $"{_instanseOptions.Host}{BASE_SESSIONS_RESOURCE}?returnUrl={returnUrl}";
			var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
			properties.AllowRefresh = true;
			return Challenge(properties, "Google");
		}

		[HttpGet]
		[Route("/api/Sessions/callback")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> SessionCallBack(string returnUrl) {
			var result = await _usersService.SignInAsync();
			if (!result.Succeeded) return Redirect(returnUrl);
			var accessToken = await _sessionTokenService.GenerateTokenAsync(result.Data);
            this.Response.Headers.Append(SessionTokenService.AUTHORIZATION_KEY, "Bearer " + accessToken);
            return this.Redirect(returnUrl.AddUrlParameter(SessionTokenService.AUTHORIZATION_KEY, accessToken));
		}
	}
}
