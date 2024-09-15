namespace Bridge.Workflow;

public interface IPersistenceProvider : IWorkflowRepository, ISubscriptionRepository, IEventRepository, IScheduledCommandRepository
{        

    Task PersistErrors(IEnumerable<ExecutionError> errors, CancellationToken cancellationToken = default);

    void EnsureStoreExists();

}