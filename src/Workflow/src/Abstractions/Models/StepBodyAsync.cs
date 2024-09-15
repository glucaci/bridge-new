namespace Bridge.Workflow;

public abstract class StepBodyAsync : IStepBody
{
    public abstract Task<ExecutionResult> RunAsync(IStepExecutionContext context);
}