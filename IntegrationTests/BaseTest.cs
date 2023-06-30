using Api;
using Api.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RestEase;
using WireMock.Client;

namespace IntegrationTests;

[Collection(CollectionDefintions.TestCollection)]
public class BaseTest : IAsyncLifetime
{
	private readonly DatabaseUtils _dbUtils;

	public BaseTest(
        TestWebApplicationFactory appFactory)
    {
		AppFactory = appFactory;
        Client = appFactory.CreateClient();
		var config = appFactory.Services.GetRequiredService<IOptions<AppConfiguration>>().Value;
		DeliveryServiceMock = RestClient.For<IWireMockAdminApi>(config.DeliveryServiceUrl);
		OrdersRepository = appFactory.Services.GetRequiredService<IOrdersRepository>();
		_dbUtils = appFactory.Services.GetRequiredService<DatabaseUtils>();
	}

	protected HttpClient Client { get; }

	protected TestWebApplicationFactory AppFactory { get; }

	protected IWireMockAdminApi DeliveryServiceMock { get; }

	protected IOrdersRepository OrdersRepository { get; }

	/// <summary>
	/// Common setup before each test.
	/// </summary>
	/// <returns></returns>
	public Task InitializeAsync()
	{
		// Setup Database state
		// Optionally set initial Database state for all tests

		// Setup HTTP Mock
		// Optionally set HTTP Mock configuration for all tests

		return Task.CompletedTask;
	}

	/// <summary>
	/// Common cleanup after each test
	/// </summary>
	/// <returns></returns>
	public async Task DisposeAsync()
	{
		// Reset Database state
		await _dbUtils.Reset();

		// Reset HTTP mock
		await DeliveryServiceMock.ResetMappingsAsync();

		// Reset Service Bus queues
		// TODO
	}
}