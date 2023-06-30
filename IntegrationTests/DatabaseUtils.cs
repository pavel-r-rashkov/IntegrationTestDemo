using Respawn;
using Respawn.Graph;

namespace IntegrationTests;

public class DatabaseUtils
{
	private readonly Lazy<Task<Respawner>> _respawner;
	private readonly string _connectionString;

	public DatabaseUtils(string connectionString)
	{
		_connectionString = connectionString;
		_respawner = new Lazy<Task<Respawner>>(async () =>
		{
			return await Respawner.CreateAsync(connectionString, new RespawnerOptions
			{
				TablesToIgnore = new Table[]
				{
					"SchemaVersions"
				},
				SchemasToInclude = new []
				{
					"dbo"
				}
			});
		});
	}

	public async Task Reset()
	{
		await (await _respawner.Value).ResetAsync(_connectionString);
	}
}