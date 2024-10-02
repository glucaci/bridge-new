using Bridge.Workflow;
using Bridge.Workflow.Bus.AzureQueueStorage;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static WorkflowOptions UseAzureSynchronization(this WorkflowOptions options, string connectionString)
    {
        options.UseQueueProvider(sp => new AzureStorageQueueProvider(connectionString, sp.GetService<ILoggerFactory>()));
        options.UseDistributedLockManager(sp => new AzureLockManager(connectionString, sp.GetService<ILoggerFactory>()));
        return options;
    }
}