using System.Data;
using Api;
using Api.Orders;
using Azure.Messaging.ServiceBus;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<AppConfiguration>(builder.Configuration);
builder.Services.AddTransient<IOrdersRepository, OrdersRepository>();
builder.Services.AddTransient<IDbConnection>((sp) =>
{
	var config = sp.GetRequiredService<IOptions<AppConfiguration>>().Value;
	return new SqlConnection(config.DatabaseConnectioString);
});
builder.Services.AddSingleton((sp) =>
{
	var config = sp.GetRequiredService<IOptions<AppConfiguration>>().Value;
	return new ServiceBusClient(config.ServiceBusConnectionString);
});
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var config = app.Services.GetRequiredService<IOptions<AppConfiguration>>().Value;
Database.Program.PerformUpgrade(config.DatabaseConnectioString);

app.Run();

// Allows test project to reference Program
namespace Api
{
    public partial class Program { }
}