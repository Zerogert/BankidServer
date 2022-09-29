namespace Bankid.Models.Options {
	public class AuthTokenOptions {
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public int LifetimeMinutes { get; set; }
		public string SecretKey { get; set; }
	}
}
