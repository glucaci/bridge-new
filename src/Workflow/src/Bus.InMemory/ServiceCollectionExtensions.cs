using Bridge.Workflow;
using Bridge.Workflow.Bus.InMemory;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // TODO: WorkflowOptions should be in abstractions
    public static WorkflowOptions UseInMemoryBus(this WorkflowOptions options)
    {
        options.UseQueueProvider(sp => new SingleNodeQueueProvider());
        options.UseEventHub(sp => new SingleNodeEventHub(sp.GetService<ILoggerFactory>()));
        return options;
    }
}