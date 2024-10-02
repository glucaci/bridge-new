using Squadron;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

public class MongoDockerSetup : IAsyncLifetime
{
    private readonly MongoReplicaSetResource _mongoResource;
    public static string ConnectionString { get; set; }

    public MongoDockerSetup()
    {
        _mongoResource = new MongoReplicaSetResource();
    }

    public async Task InitializeAsync()
    {
        await _mongoResource.InitializeAsync();
        ConnectionString = _mongoResource.ConnectionString;
    }

    public Task DisposeAsync()
    {
        return _mongoResource.DisposeAsync();
    }
}

[CollectionDefinition("Mongo collection")]
public class MongoCollection : ICollectionFixture<MongoDockerSetup>
{
}