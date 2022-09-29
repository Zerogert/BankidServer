using System.Collections.Generic;

namespace Bankid.Models {
	public class PaginationResult<T> {
		public IEnumerable<T> Data { get; set; }
		public int Index { get; set; }
		public int PageSize { get; set; }
		public int PagesTotal { get; set; }
		public int ItemsTotal { get; set; }
		public bool IsLastPage => Index + 1 >= PagesTotal;
	}
}
