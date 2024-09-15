using Bridge.Storage.InMemory;
using Bridge.Workflow;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // TODO: WorkflowOptions should be in abstractions
    public static WorkflowOptions UseInMemoryStorage(this WorkflowOptions options)
    {
        options.UsePersistence(sp => new TransientMemoryPersistenceProvider(sp.GetService<ISingletonMemoryProvider>()));
        options.Services.AddSingleton<ISingletonMemoryProvider, MemoryPersistenceProvider>();
        return options;
    }
}