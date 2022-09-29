using System;

namespace Bankid.Models {
	public class BaseError : Exception {
		protected readonly object[] Parameters;

		public string Type { get; set; }
		public string ErrorMessage => GetMesage();

		public BaseError(params object[] parameters) {
			Type = GetType().Name;
			Parameters = parameters;
		}

		protected virtual string GetMesage() {
			return "";
		}
	}
}
