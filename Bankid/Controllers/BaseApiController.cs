using AutoMapper;
using Bankid.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bankid.Controllers {

	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class BaseApiController : ControllerBase {
		protected readonly IMapper Mapper;
		protected readonly AppDbContext DbContext;

		public BaseApiController(AppDbContext dbContext, IMapper mapper) {
			Mapper = mapper;
			DbContext = dbContext;
		}

	}
}
