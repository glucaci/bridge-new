using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Workflow.Tests;

[Obsolete]
public abstract class BaseScenario<TWorkflow, TData> : IDisposable
    where TWorkflow : IWorkflow<TData>, new()
    where TData : class, new()
{
    protected IWorkflowHost Host;
    protected IPersistenceProvider PersistenceProvider;

    public BaseScenario()
    {
        Setup();
    }

    protected void Setup()
    {
        //setup dependency injection
        IServiceCollection services = new ServiceCollection();
        services.AddLogging();
        Configure(services);

        var serviceProvider = services.BuildServiceProvider();

        PersistenceProvider = serviceProvider.GetService<IPersistenceProvider>();
        Host = serviceProvider.GetService<IWorkflowHost>();
        Host.RegisterWorkflow<TWorkflow, TData>();
        Host.Start();
    }

    protected virtual void Configure(IServiceCollection services)
    {
        services.AddWorkflow(o => o
            .UseInMemoryStorage()
            .UseInMemoryBus());
    }

    public void Dispose()
    {
        Host.Stop();
    }
}