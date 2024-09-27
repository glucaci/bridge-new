using Bridge.Workflow;
using Bridge.Workflow.Bus.AzureServiceBus;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static WorkflowOptions UseAzureServiceBus(this WorkflowOptions options, string connectionString)
    {
        options.UseQueueProvider(sp => new AzureServiceBusProvider());
        return options;
    }
}