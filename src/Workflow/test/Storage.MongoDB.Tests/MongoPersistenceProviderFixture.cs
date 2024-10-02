using MongoDB.Driver;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoPersistenceProviderFixture : BasePersistenceFixture
{
    MongoDockerSetup _dockerSetup;

    public MongoPersistenceProviderFixture(MongoDockerSetup dockerSetup)
    {
        _dockerSetup = dockerSetup;
    }

    protected override IPersistenceProvider Subject
    {
        get
        {
            var client = new MongoClient(MongoDockerSetup.ConnectionString);
            var db = client.GetDatabase(nameof(MongoPersistenceProviderFixture));
            return new MongoPersistenceProvider(db);
        }
    }
}