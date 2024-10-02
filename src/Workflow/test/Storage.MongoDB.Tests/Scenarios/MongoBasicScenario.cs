using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoBasicScenario : BasicScenario
{        
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddWorkflow(x => x.UseInMemoryBus().UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoBasicScenario)));
    }
}