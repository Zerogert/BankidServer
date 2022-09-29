using Bankid.Models;
using Bankid.Models.Entities;
using System.Threading.Tasks;

namespace Bankid.Interfaces {
	public interface IUsersService {
		public Task<ServiceResult<User>> SignInAsync();
	}
}
