using Bankid.Models.Entities;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bankid.Interfaces {
	public interface ISessionTokenService {
		public Task<string> GenerateTokenAsync(User user);
		public Task<JwtSecurityToken> GenerateJwtTokenAsync(User user);
		public string GenerateToken(IEnumerable<Claim> claims);
		public JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims);
	}
}
