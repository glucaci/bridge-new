using Bridge.Workflow.Tests;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bridge.Workflow.Storage.MongoDB.Tests;

[Collection("Mongo collection")]
public class MongoSagaScenario : SagaScenario
{        
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddWorkflow(x => x.UseInMemoryBus().UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoSagaScenario)));
    }
}