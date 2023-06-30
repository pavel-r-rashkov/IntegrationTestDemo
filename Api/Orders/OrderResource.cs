using System.Text.Json.Serialization;

namespace Api.Orders;

public class OrderResource
{
	public int Id { get; set; }

	public decimal Total { get; set; }

	[JsonConverter(typeof(JsonStringEnumConverter))]
	public OrderStatus? Status { get; set; }

	public static OrderResource From(Order order)
	{
		return new OrderResource
		{
			Id = order.Id,
			Total = order.Total,
		};
	}

	public Order To()
	{
		return new Order
		{
			Id = Id,
			Total = Total,
		};
	}
}

public enum OrderStatus
{
	Pending,
	Delivered,
}
