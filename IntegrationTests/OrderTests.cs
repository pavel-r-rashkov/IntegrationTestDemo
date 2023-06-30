using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.Orders;
using WireMock.Admin.Mappings;

namespace IntegrationTests;

public class OrderTests : BaseTest
{
	public OrderTests(TestWebApplicationFactory appFactory)
        : base(appFactory)
    {
    }

    [Fact]
    public async Task GetOrder_WithExistingOrder_ShouldReturn200()
    {
		await DeliveryServiceMock.PostMappingAsync(WithDeliveryStatus(OrderStatus.Delivered));
		var order = new Order { Total = 100 };
		var id = await OrdersRepository.AddOrder(order);

		var response = await Client.GetAsync($"/orders/{id}");

		Assert.True(response.IsSuccessStatusCode);
    }

	[Fact]
    public async Task GetOrder_WithNonExistingOrder_ShouldReturn404()
    {
		var id = 123;

		var response = await Client.GetAsync($"/orders/{id}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

	[Fact]
    public async Task GetOrder_WithPendingOrder_ShouldReturnPendingStatus()
    {
		await DeliveryServiceMock.PostMappingAsync(WithDeliveryStatus(OrderStatus.Pending));
		var order = new Order { Total = 100 };
		var id = await OrdersRepository.AddOrder(order);

		var response = await Client.GetAsync($"/orders/{id}");

		var body = await response.Content.ReadFromJsonAsync<OrderResource>();
		Assert.Equal(OrderStatus.Pending, body?.Status);
    }

	[Fact]
    public async Task PostOrder_ShouldReturn201()
    {
		var order = new OrderResource { Total = 300 };

		var response = await Client.PostAsync($"/orders", new JsonHttpContent(order));

		// TODO optionally try to consume the generated message from Service Bus
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

	private static MappingModel WithDeliveryStatus(OrderStatus status)
	{
		return new MappingModel
		{
			Request = new RequestModel
			{
				Methods = new[] { "GET" },
				Path = "/status",
			},
			Response = new ResponseModel
			{
				Body = JsonSerializer.Serialize(new OrderStatusResponse
				{
					Status = status,
				}),
				Headers = new Dictionary<string, object>
				{
					{ "Content-Type", "application/json" },
				},
			},
		};
	}
}