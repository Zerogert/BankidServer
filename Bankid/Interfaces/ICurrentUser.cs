namespace Bankid.Interfaces {
	public interface ICurrentUser {
		public int UserId { get; }
		public bool IsAuth { get; }
		public string Role { get; }
		public string FirstName { get; }
        public string LastName { get; }
    }
}
