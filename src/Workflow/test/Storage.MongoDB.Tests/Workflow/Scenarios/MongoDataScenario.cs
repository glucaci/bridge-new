using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Bridge.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoDataScenario : DataIOScenario
{
    public MongoDataScenario() : base()
    {
        BsonClassMap.RegisterClassMap<MyDataClass>(map => map.AutoMap());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddWorkflow(x => x.UseInMemoryBus().UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoDataScenario)));
    }
}