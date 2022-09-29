using System.Text;

namespace Bankid.Extensions {
	public static class StringExtensions {
		public static string AddUrlParameter(this string url, string key, string value) {
			return url.Contains('?') ? $"{url}&{key}={value}" : $"{url}?{key}={value}";
		}
	}
}
