using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoEventScenario : EventScenario
{        
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddWorkflow(x => x.UseInMemoryBus().UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoEventScenario)));
    }
}