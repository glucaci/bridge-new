namespace Bridge.Workflow;

public class StepCompleted : LifeCycleEvent
{
    public string ExecutionPointerId { get; set; }

    public int StepId { get; set; }
}