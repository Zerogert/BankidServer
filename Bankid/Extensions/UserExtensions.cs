using Bankid.Models.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bankid.Extensions {
	public static class UserExtensions {
		public static async Task<IEnumerable<Claim>> GetClaimsAsync(this User user) {
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, user?.Email ?? ""),
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role),
					new Claim(ClaimTypes.GivenName, user.FirstName),
					new Claim(ClaimTypes.Name, user.LastName)
				};

			return claims;
		}
	}
}
