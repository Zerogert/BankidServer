using System;

namespace Bankid.Models.Base {
	public interface IBaseEntity {
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
