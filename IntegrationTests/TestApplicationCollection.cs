namespace IntegrationTests;

public static class CollectionDefintions
{
	public const string TestCollection = nameof(TestCollection);
}

[CollectionDefinition(CollectionDefintions.TestCollection)]
public class TestApplicationCollection : ICollectionFixture<TestWebApplicationFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition]
}