namespace Coordinator.Models
{
	/// <summary>
	///	Represents a participant microservice involved in a two-phase commit (2PC) protocol transaction.
	/// </summary>
	/// <param name="microserviceName">The name of the microservice participating in the 2PC protocol.</param>
	public record Node(string microserviceName) // 2PC protokolü kapsamında işleme girecek olan mikroservisler'i ifade eden sınıftır.
	{
		public Guid Id { get; set; }
		public ICollection<NodeState> NodeStates { get; set; }
	}
}
