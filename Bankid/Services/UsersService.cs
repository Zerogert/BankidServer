using AutoMapper;
using Bankid.Data;
using Bankid.Interfaces;
using Bankid.Models;
using Bankid.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bankid.Services {
	public class UsersService : BaseService, IUsersService {
		private const int MIN_AGE = 13;

		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;

		public UsersService(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, ICurrentUser currentUser, AppDbContext dbContext) : base(mapper, currentUser, dbContext) {
			_userManager = userManager;
			_signInManager = signInManager;
		}


		public async Task<ServiceResult<User>> SignInAsync() {
			var info = await _signInManager.GetExternalLoginInfoAsync();

			var validUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
			if (validUser == null) {
				var email = info.Principal.FindFirstValue(ClaimTypes.Email);
				if (!string.IsNullOrWhiteSpace(email)) validUser = await _userManager.FindByEmailAsync(email);
				if (validUser == null) {
					validUser = new User() {
						UserName = Guid.NewGuid().ToString().Replace("-", ""),
						FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
						LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
						Email = info.Principal.FindFirstValue(ClaimTypes.Email),
						Role = Role.UserRoleName
					};
					await _userManager.CreateAsync(validUser);
				};
				await _userManager.AddLoginAsync(validUser, info);
			}

			await _signInManager.SignInAsync(validUser, false);

			return ServiceResult<User>.Ok(validUser);
		}
	}
}
