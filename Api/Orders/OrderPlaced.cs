namespace Api.Orders;

public class OrderPlaced
{
	public int Id { get; set; }

	public decimal Total { get; set; }

	public DateTime PlacementDate { get; set; }

	public static OrderPlaced From(Order order)
	{
		return new OrderPlaced
		{
			Id = order.Id,
			Total = order.Total,
			PlacementDate = DateTime.Now,
		};
	}
}