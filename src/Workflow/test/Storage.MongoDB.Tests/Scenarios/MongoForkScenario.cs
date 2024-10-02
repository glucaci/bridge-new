using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoForkScenario : ForkScenario
{        
    protected override void Configure(IServiceCollection services)
    {
        services.AddWorkflow(x => x.UseInMemoryBus().UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoForkScenario)));
    }
}