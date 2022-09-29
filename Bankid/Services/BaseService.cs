using AutoMapper;
using Bankid.Data;
using Bankid.Interfaces;

namespace Bankid.Services {
	public class BaseService {
		protected readonly IMapper Mapper;
		protected readonly ICurrentUser CurrentUser;
		protected AppDbContext DbContext;

		public BaseService(IMapper mapper, ICurrentUser currentUser, AppDbContext dbContext) {
			Mapper = mapper;
			CurrentUser = currentUser;
			DbContext = dbContext;
		}
	}
}
