using System.Data;
using Dapper;

namespace Api.Orders;

public sealed class OrdersRepository : IOrdersRepository, IDisposable
{
	private readonly IDbConnection _connection;

	public OrdersRepository(IDbConnection connection)
	{
		_connection = connection;
	}

	public async Task<int> AddOrder(Order order)
	{
		var id = await _connection.QueryFirstAsync<int>(
			@"INSERT INTO Orders(Total)
			VALUES
				(@Total);
			SELECT SCOPE_IDENTITY()",
			order);
		order.Id = id;

		return id;
	}

	public async Task<Order?> GetOrder(int id)
	{
		var or = await _connection.QueryFirstOrDefaultAsync<Order>(
			@"SELECT * 
			FROM Orders 
			WHERE Id = @Id",
			new { Id = id });
		return or;
	}

	public void Dispose()
	{
		_connection.Dispose();
	}
}