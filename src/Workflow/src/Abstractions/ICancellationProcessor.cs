namespace Bridge.Workflow;

public interface ICancellationProcessor
{
    void ProcessCancellations(WorkflowInstance workflow, WorkflowDefinition workflowDef, WorkflowExecutorResult executionResult);
}