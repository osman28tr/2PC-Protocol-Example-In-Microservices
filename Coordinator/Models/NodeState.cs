using Coordinator.Enums;

namespace Coordinator.Models
{
	public record NodeState(string transactionId)
	{
		public Guid Id { get; set; }
		/// <summary>
		/// İlgili mikroservis'in 1. aşama için hazır olup olmadığını ifade eder.
		/// </summary>
		public PrePhaseReadyType IsReady { get; set; }
		/// <summary>
		/// İlgili mikroservis için 2. aşamanın başarıyla tamamlanıp tamamlanmadığını ifade eder.
		/// </summary>
		public CommitPhaseTransactionState TransactionState { get; set; }
		public Node Node { get; set; }
	}
}
