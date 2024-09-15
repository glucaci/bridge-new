namespace Bridge.Workflow;

public interface IWorkflowExecutor
{
    Task<WorkflowExecutorResult> Execute(WorkflowInstance workflow, CancellationToken cancellationToken = default);
}