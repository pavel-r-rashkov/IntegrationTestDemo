using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Orders;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
	private readonly IOrdersRepository _ordersRepository;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ServiceBusClient _serviceBusClient;
	private readonly AppConfiguration _appConfiguration;

    public OrdersController(
		IOrdersRepository ordersRepository,
		IHttpClientFactory httpClientFactory,
		ServiceBusClient serviceBusClient,
		IOptions<AppConfiguration> appConfiguration)
    {
        _ordersRepository = ordersRepository;
		_httpClientFactory = httpClientFactory;
		_serviceBusClient = serviceBusClient;
		_appConfiguration = appConfiguration.Value;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResource>> Get(int id)
    {
        var order = await _ordersRepository.GetOrder(id);

		if (order == null)
		{
			return NotFound();
		}

		// Demonstrate HTTP service dependency
		var client = _httpClientFactory.CreateClient();
		var response = await client.GetAsync($"{_appConfiguration.DeliveryServiceUrl}status");
		response.EnsureSuccessStatusCode();
		var options = new JsonSerializerOptions();
		options.Converters.Add(new JsonStringEnumConverter());
		var body = await response.Content.ReadFromJsonAsync<OrderStatusResponse>(options);

		var resource = OrderResource.From(order);
		resource.Status = body?.Status;

		return Ok(resource);
    }

	[HttpPost]
    public async Task<ActionResult> Post(OrderResource orderResource)
    {
		var order = orderResource.To();
        var id = await _ordersRepository.AddOrder(order);

		// Demonstrate service bus dependency
		var sender = _serviceBusClient.CreateSender("order-placed");
		var message = JsonSerializer.Serialize(OrderPlaced.From(order));
		await sender.SendMessageAsync(new ServiceBusMessage(message));

		return CreatedAtAction(nameof(Get), new { id }, null);
    }
}
