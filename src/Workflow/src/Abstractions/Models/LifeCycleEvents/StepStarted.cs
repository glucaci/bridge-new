namespace Bridge.Workflow;

public class StepStarted : LifeCycleEvent
{
    public string ExecutionPointerId { get; set; }

    public int StepId { get; set; }
}