using Coordinator.Models;
using Coordinator.Models.Contexts;
using Coordinator.Services;
using Coordinator.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Collections;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddHttpClient("OrderAPI",client=>client.BaseAddress = new Uri("http://localhost:5177"));
builder.Services.AddHttpClient("StockAPI",client=>client.BaseAddress = new Uri("http://localhost:5083"));
builder.Services.AddHttpClient("PaymentAPI",client=>client.BaseAddress = new Uri("http://localhost:5048"));

builder.Services.AddOpenApi();
builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("TwoPhaseCommitCon"));
});
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapGet("/create-order-transaction", async (ITransactionService transactionService)=>{
	//Phase 1 - Prepare
	 var transactionId = await transactionService.CreateTransactionAsync();
	 await transactionService.PrepareServicesAsync(transactionId);
	bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

	if (transactionState)
	{
		//Phase 2
		await transactionService.CommitAsync(transactionId);
		transactionState = await transactionService.CheckTransactionServicesAsync(transactionId);
	}

	if (!transactionState)
		await transactionService.RollBackAsync(transactionId);
});

app.Run();
