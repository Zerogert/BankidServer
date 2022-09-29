using Bankid.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Bankid.Services {
    public class CurrentUserResolver : ICurrentUser {
        private readonly IHttpContextAccessor _contextAccessor;

        public int UserId => int.Parse(_contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        public bool IsAuth => _contextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string Role => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        public CurrentUserResolver(IHttpContextAccessor httpContextAccessor) {
            _contextAccessor = httpContextAccessor;
        }
    }
}
