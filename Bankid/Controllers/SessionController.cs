using AutoMapper;
using Bankid.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppDbContext = Bankid.Data.AppDbContext;

namespace Bankid.Controllers {
    public class SessionController : BaseApiController {
        private readonly ICurrentUser _currentUser;
        public SessionController(IMapper mapper, AppDbContext dbContext, ICurrentUser currentUser) : base(dbContext, mapper) {
            _currentUser = currentUser;
        }

        [HttpGet]
        [Route("/api/session")]
        public async Task<IActionResult> GetAsync() {
            return Ok(_currentUser);
        }
    }
}
