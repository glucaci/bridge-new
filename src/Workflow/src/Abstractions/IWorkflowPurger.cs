namespace Bridge.Workflow;

public interface IWorkflowPurger
{
    Task PurgeWorkflows(WorkflowStatus status, DateTime olderThan, CancellationToken cancellationToken = default);
}