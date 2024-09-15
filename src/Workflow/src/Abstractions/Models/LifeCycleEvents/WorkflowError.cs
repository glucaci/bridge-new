namespace Bridge.Workflow;

public class WorkflowError : LifeCycleEvent
{
    public string Message { get; set; }

    public string ExecutionPointerId { get; set; }

    public int StepId { get; set; }
}