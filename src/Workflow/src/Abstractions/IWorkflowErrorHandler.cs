namespace Bridge.Workflow;

public interface IWorkflowErrorHandler
{
    WorkflowErrorHandling Type { get; }
    void Handle(WorkflowInstance workflow, WorkflowDefinition def, ExecutionPointer pointer, WorkflowStep step, Exception exception, Queue<ExecutionPointer> bubbleUpQueue);
}