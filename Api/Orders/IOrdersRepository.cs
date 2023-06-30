namespace Api.Orders;

public interface IOrdersRepository
{
	Task<Order?> GetOrder(int id);

	Task<int> AddOrder(Order order);
}