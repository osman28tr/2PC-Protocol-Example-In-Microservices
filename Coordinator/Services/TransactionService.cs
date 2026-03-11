using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Coordinator.Services
{
	public class TransactionService(TwoPhaseCommitContext _context, IHttpClientFactory _clientFactory) : ITransactionService
	{
		HttpClient _orderHttpClient = _clientFactory.CreateClient("OrderAPI");
		HttpClient _stockHttpClient = _clientFactory.CreateClient("StockAPI");
		HttpClient _paymentHttpClient = _clientFactory.CreateClient("PaymentAPI");
		public async Task<Guid> CreateTransactionAsync()
		{
			Guid transactionId = Guid.NewGuid();
			var nodes = await _context.Nodes.ToListAsync();
			nodes.ForEach(node => node.NodeStates = new List<NodeState>
			{
				new NodeState(transactionId.ToString()){IsReady = Enums.PrePhaseReadyType.Pending,
					TransactionState = Enums.CommitPhaseTransactionState.Pending}
			});

			await _context.SaveChangesAsync();
			return transactionId;
		}
		public async Task PrepareServicesAsync(Guid transactionId)
		{
			var transactionNodes = await _context.NodeStates.Where(x => x.transactionId == transactionId.ToString()).Include(ns => ns.Node)
				.ToListAsync();
			transactionNodes.ForEach(async transactionNode =>
			{
				try
				{
					HttpResponseMessage response = await CheckPrepareServicesAsync(transactionNode);
					var result = bool.Parse(await response.Content.ReadAsStringAsync());
					transactionNode.IsReady = result ? Enums.PrePhaseReadyType.Ready : Enums.PrePhaseReadyType.UnReady;
				}
				catch (Exception)
				{
					transactionNode.IsReady = Enums.PrePhaseReadyType.UnReady;
				}
			});

			await _context.SaveChangesAsync();
		}

		private async Task<HttpResponseMessage> CheckPrepareServicesAsync(NodeState transactionNode)
		{
			return await (transactionNode.Node.microserviceName switch
			{
				"OrderAPI" => _orderHttpClient.GetAsync("ready"),
				"StockAPI" => _orderHttpClient.GetAsync("ready"),
				"PaymentAPI" => _orderHttpClient.GetAsync("ready")
			});
		}

		public async Task<bool> CheckReadyServicesAsync(Guid transactionId) =>
			(await _context.NodeStates.Where(ns => ns.transactionId == transactionId.ToString()).ToListAsync())
				.TrueForAll(ns => ns.IsReady == Enums.PrePhaseReadyType.Ready);

		public async Task CommitAsync(Guid transactionId)
		{
			var transactionNodes = await _context.NodeStates.Where(ns => ns.transactionId == transactionId.ToString()).
				Include(n => n.Node).ToListAsync();

			transactionNodes.ForEach(async transactionNode =>
			{
				try
				{
					var response = await (transactionNode.Node.microserviceName switch
					{
						"OrderAPI" => _orderHttpClient.GetAsync("commit"),
						"StockAPI" => _orderHttpClient.GetAsync("commit"),
						"PaymentAPI" => _orderHttpClient.GetAsync("commit")
					});

					var result = bool.Parse(await response.Content.ReadAsStringAsync());
					transactionNode.TransactionState = result ? Enums.CommitPhaseTransactionState.Done : Enums.CommitPhaseTransactionState.Abort;
				}
				catch (Exception)
				{
					transactionNode.TransactionState = Enums.CommitPhaseTransactionState.Abort;
				}
			});

			await _context.SaveChangesAsync();
		}
		public async Task<bool> CheckTransactionServicesAsync(Guid transactionId)
			=>
			(await _context.NodeStates.Where(ns => ns.transactionId == transactionId.ToString()).ToListAsync())
				.TrueForAll(ns => ns.TransactionState == Enums.CommitPhaseTransactionState.Done);
		public async Task RollBackAsync(Guid transactionId)
		{
			var transactionNodes = await _context.NodeStates.Include(n=>n.Node).
				Where(ns => ns.transactionId == transactionId.ToString()).ToListAsync();
			foreach (var transactionNode in transactionNodes)
			{
				try
				{
					if (transactionNode.TransactionState == Enums.CommitPhaseTransactionState.Done)
					{
						_= await (transactionNode.Node.microserviceName switch
						{
							"OrderAPI"=> _orderHttpClient.GetAsync("rollback"),
							"StockAPI"=> _stockHttpClient.GetAsync("rollback"),
							"PaymentAPI"=> _paymentHttpClient.GetAsync("rollback")
						});
					}
					transactionNode.TransactionState = Enums.CommitPhaseTransactionState.Abort;
				}
				catch (Exception)
				{
					transactionNode.TransactionState = Enums.CommitPhaseTransactionState.Abort;
				}
			}
			await _context.SaveChangesAsync();
		}
	}
}
