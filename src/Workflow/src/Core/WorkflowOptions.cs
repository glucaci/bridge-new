using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Workflow;

public class WorkflowOptions
{
    internal Func<IServiceProvider, IPersistenceProvider> PersistenceFactory;
    internal Func<IServiceProvider, IQueueProvider> QueueFactory;
    internal Func<IServiceProvider, IDistributedLockProvider> LockFactory;
    internal Func<IServiceProvider, ILifeCycleEventHub> EventHubFactory;
    internal Func<IServiceProvider, ISearchIndex> SearchIndexFactory;
    internal TimeSpan PollInterval;
    internal TimeSpan IdleTime;
    internal TimeSpan ErrorRetryInterval;
    internal int MaxConcurrentWorkflows = Math.Max(Environment.ProcessorCount, 4);

    public IServiceCollection Services { get; private set; }

    public WorkflowOptions(IServiceCollection services)
    {
        Services = services;
        PollInterval = TimeSpan.FromSeconds(10);
        IdleTime = TimeSpan.FromMilliseconds(100);
        ErrorRetryInterval = TimeSpan.FromSeconds(60);

        LockFactory = new Func<IServiceProvider, IDistributedLockProvider>(sp => new SingleNodeLockProvider());
        SearchIndexFactory = new Func<IServiceProvider, ISearchIndex>(sp => new NullSearchIndex());
    }

    public bool EnableWorkflows { get; set; } = true;
    public bool EnableEvents { get; set; } = true;
    public bool EnableIndexes { get; set; } = true;
    public bool EnablePolling { get; set; } = true;
    public bool EnableLifeCycleEventsPublisher { get; set; } = true;

    public void UsePersistence(Func<IServiceProvider, IPersistenceProvider> factory)
    {
        PersistenceFactory = factory;
    }

    public void UseDistributedLockManager(Func<IServiceProvider, IDistributedLockProvider> factory)
    {
        LockFactory = factory;
    }

    public void UseQueueProvider(Func<IServiceProvider, IQueueProvider> factory)
    {
        QueueFactory = factory;
    }

    public void UseEventHub(Func<IServiceProvider, ILifeCycleEventHub> factory)
    {
        EventHubFactory = factory;
    }

    public void UseSearchIndex(Func<IServiceProvider, ISearchIndex> factory)
    {
        SearchIndexFactory = factory;
    }

    public void UsePollInterval(TimeSpan interval)
    {
        PollInterval = interval;
    }

    public void UseErrorRetryInterval(TimeSpan interval)
    {
        ErrorRetryInterval = interval;
    }

    public void UseIdleTime(TimeSpan interval)
    {
        IdleTime = interval;
    }

    public void UseMaxConcurrentWorkflows(int maxConcurrentWorkflows)
    {
        MaxConcurrentWorkflows = maxConcurrentWorkflows;
    }
}