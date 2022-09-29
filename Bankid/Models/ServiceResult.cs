using System.Collections.Generic;

namespace Bankid.Models {
	public class ServiceResult {
		public List<BaseError> Errors { get; set; } = new List<BaseError>();
		public bool Succeeded { get; set; }

		public static ServiceResult Ok() => new() { Succeeded = true };
		public static ServiceResult Fail(BaseError error) => new() { Succeeded = false, Errors = new List<BaseError> { error } };
	}

	public class ServiceResult<T> : ServiceResult {
		public T Data { get; set; }

		public static ServiceResult<T> Ok(T data) => new() { Succeeded = true, Data = data };
		public static ServiceResult<T> Fail(BaseError error) => new() { Succeeded = false, Errors = new List<BaseError> { error } };
		public static ServiceResult<T> Fail(List<BaseError> errors) => new() { Succeeded = false, Errors = errors };
	}
}
