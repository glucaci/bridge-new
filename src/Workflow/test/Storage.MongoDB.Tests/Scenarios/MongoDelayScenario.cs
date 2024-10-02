using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoDelayScenario : DelayScenario
{
    public MongoDelayScenario() : base()
    {
        BsonClassMap.RegisterClassMap<DelayWorkflow.MyDataClass>(map => map.AutoMap());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddWorkflow(cfg =>
        {
            cfg.UseInMemoryBus();
            cfg.UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoDelayScenario));
            cfg.UsePollInterval(TimeSpan.FromSeconds(2));
        });
    }
}