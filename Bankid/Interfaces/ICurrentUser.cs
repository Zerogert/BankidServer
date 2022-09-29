namespace Bankid.Interfaces {
	public interface ICurrentUser {
		public int UserId { get; }
		public bool IsAuth { get; }
		public string Role { get; }
    }
}
