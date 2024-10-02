using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Bridge.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoActivityScenario : ActivityScenario
{        
    protected override void ConfigureServices(IServiceCollection services)
    {
        BsonClassMap.RegisterClassMap<ActivityScenario.ActivityInput>(x => x.AutoMap());
        BsonClassMap.RegisterClassMap<ActivityScenario.ActivityOutput>(x => x.AutoMap());

        services.AddWorkflow(x => x.UseInMemoryBus().UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoActivityScenario)));
    }
}