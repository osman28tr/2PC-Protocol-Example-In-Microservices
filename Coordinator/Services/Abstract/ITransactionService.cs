namespace Coordinator.Services.Abstract
{
	public interface ITransactionService
	{
		Task<Guid> CreateTransactionAsync();
		Task PrepareServicesAsync(Guid transactionId); //İlgili transaction ile ilgili servislerin hazır olup olmadıklarının kontrolü
		Task<bool> CheckReadyServicesAsync(Guid transactionId);
		Task CommitAsync(Guid transactionId);
		Task<bool> CheckTransactionServicesAsync(Guid transactionId);
		Task RollBackAsync(Guid transactionId);
	}
}
