namespace Bridge.Workflow;

public interface IStepBody
{        
    Task<ExecutionResult> RunAsync(IStepExecutionContext context);        
}