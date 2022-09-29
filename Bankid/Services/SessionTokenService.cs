using Bankid.Extensions;
using Bankid.Interfaces;
using Bankid.Models.Entities;
using Bankid.Models.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bankid.Services {
	public class SessionTokenService : ISessionTokenService {
		public const string AUTHORIZATION_KEY = "Authorization";
		public const string AUTHORIZATION_EXPIRES_KEY = "Authorization_expires";

		private readonly AuthTokenOptions _authTokenOptions;

		public SessionTokenService(AuthTokenOptions tokenOptions) {
			_authTokenOptions = tokenOptions;
		}

		public async Task<string> GenerateTokenAsync(User user) {
			var accessTokenString = new JwtSecurityTokenHandler().WriteToken(await GenerateJwtTokenAsync(user));
			return accessTokenString;
		}

		public async Task<JwtSecurityToken> GenerateJwtTokenAsync(User user) {
			var claims = await user.GetClaimsAsync();
			return GenerateJwtToken(claims);
		}

		public string GenerateToken(IEnumerable<Claim> claims) {
			var jwtToken = GenerateJwtToken(claims);
			var accessTokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);
			return accessTokenString;
		}

		public JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims) {
			return new JwtSecurityToken(
				_authTokenOptions.Issuer,
				_authTokenOptions.Audience,
				claims,
				expires: DateTime.UtcNow.AddMinutes(_authTokenOptions.LifetimeMinutes),
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authTokenOptions.SecretKey)), SecurityAlgorithms.HmacSha256));
		}
	}
}
